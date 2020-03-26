import { Scene, Types, CANVAS, Game, Physics, Input, GameObjects } from 'phaser';
import { Spaceship, Direction } from './spaceship';
import { Bullet } from './bullet';
import { Meteor } from './meteor';

/**
 * Space shooter scene
 * 
 * Learn more about Phaser scenes at 
 * https://photonstorm.github.io/phaser3-docs/Phaser.Scenes.Systems.html.
 */
class ShooterScene extends Scene {
    private spaceShip: Spaceship;
    private meteors: Physics.Arcade.Group;
    private bullets: Physics.Arcade.Group;
    private points: GameObjects.Text;

    private bulletTime = 0;
    private meteorTime = 0;

    private cursors: Types.Input.Keyboard.CursorKeys;
    private spaceKey: Input.Keyboard.Key;
    private isGameOver = false;
    private hits = 0;

    private doHighscoreListBool = false;
    private nameString = "";
    private sentPost = false;

    preload() {
        // Preload images so that we can use them in our game
        this.load.image('space', 'images/deep-space.jpg');
        this.load.image('bullet', 'images/scratch-laser.png');
        this.load.image('ship', 'images/scratch-spaceship.png');
        this.load.image('meteor', 'images/scratch-meteor.png');
    }

    create() {
        if (this.isGameOver) {
            return;
        }

        //  Add a background
        this.add.tileSprite(0, 0, this.game.canvas.width, this.game.canvas.height, 'space').setOrigin(0, 0);

        this.points = this.add.text(this.game.canvas.width * 0.1, this.game.canvas.height * 0.1, "0",
            { font: "32px Arial", fill: "#ff0044", align: "left" });

        // Create bullets and meteors
        this.bullets = this.physics.add.group({
            classType: Bullet,
            maxSize: 10,
            runChildUpdate: true
        });
        this.meteors = this.physics.add.group({
            classType: Meteor,
            maxSize: 20,
            runChildUpdate: true
        });

        // Add the sprite for our space ship.
        this.spaceShip = new Spaceship(this);
        this.physics.add.existing(this.children.add(this.spaceShip));

        // Position the spaceship horizontally in the middle of the screen
        // and vertically at the bottom of the screen.
        this.spaceShip.setPosition(this.game.canvas.width / 2, this.game.canvas.height * 0.9);

        // Setup game input handling
        this.cursors = this.input.keyboard.createCursorKeys();
        this.input.keyboard.addCapture([' ']);
        this.spaceKey = this.input.keyboard.addKey(Input.Keyboard.KeyCodes.SPACE);

        this.physics.add.collider(this.bullets, this.meteors, (bullet: Bullet, meteor: Meteor) => {
            if (meteor.active && bullet.active) {
                this.points.setText((++this.hits).toString())
                meteor.kill();
                bullet.kill();
            }
        }, null, this);
        this.physics.add.collider(this.spaceShip, this.meteors, this.gameOver, null, this);
    }

    update(_, delta: number) {
        // Move ship if cursor keys are pressed
        if (this.cursors.left.isDown) {
            this.spaceShip.move(delta, Direction.Left);
        }
        else if (this.cursors.right.isDown) {
            this.spaceShip.move(delta, Direction.Right);
        }

        if (this.spaceKey.isDown) {
            this.fireBullet();
        }

        this.handleMeteors();
    }

    fireBullet() {
        if (this.time.now > this.bulletTime) {
            // Find the first unused (=unfired) bullet
            const bullet = this.bullets.get() as Bullet;
            if (bullet) {
                bullet.fire(this.spaceShip.x, this.spaceShip.y);
                this.bulletTime = this.time.now + 100;
            }
        }
    }

    handleMeteors() {
        // Check if it is time to launch a new meteor.
        if (this.time.now > this.meteorTime) {
            // Find first meteor that is currently not used
            const meteor = this.meteors.get() as Meteor;
            if (meteor) {
                meteor.fall();
                this.meteorTime = this.time.now + 500 + 1000 * Math.random();
            }
        }
    }

    gameOver() {
        this.isGameOver = true;
        this.bullets.getChildren().forEach((b: Bullet) => b.kill());
        this.meteors.getChildren().forEach((m: Meteor) => m.kill());
        this.spaceShip.kill();

        // Display "game over" text
        const text = this.add.text(this.game.canvas.width / 2, this.game.canvas.height / 2, "Game Over :-(",
            { font: "65px Arial", fill: "#ff0044", align: "center" });
        text.setOrigin(0.5, 0.5);
        if (!this.doHighscoreListBool) {
            this.doHighscoreListBool = true;
            document.getElementById("nameform").innerHTML = '<form><label for="lname">Name:</label><input type="text" id="lname" name="lname"><input maxlength="3" type="submit" value="Submit"></form>';
            this.doHighscoreList();
        }

    }

    doHighscoreList() {
        const form = document.querySelector('form');
        form.onsubmit = () => {
            const data = new FormData(form);
            const name = data.get('lname') as string;
            if (!this.sentPost) {
                this.doPostScore(name, this.hits);
            }
            return false; // prevent reload
        };
    }

    doPostScore(name: string, points: number) {
        this.sentPost = true;
        name = name.substr(0, 3);

        const highscoreEntry = { 'Initials': name, 'Score': points };
        console.log(JSON.stringify(highscoreEntry));

        const headers = new Headers();

        headers.append('Accept', 'application/json'); // This one is enough for GET requests
        headers.append('Content-Type', 'application/json'); // This one sends body

        fetch('http://localhost:5000/api/addScore', {
            method: 'POST',
            mode: 'cors',
            headers: headers,
            body: JSON.stringify(highscoreEntry),
        }).then((postres) => {
            fetch('http://localhost:5000/api/highscore', {
                method: 'GET',
                mode: 'cors',
                headers: headers
            }).then((response) => {
                response.json().then(highscores => {
                    let highscore = "List: ";
                    highscore += "<ul>";
    
                    for (const curH of highscores) {
                        highscore += "<li>" + curH.initials + "\t" + curH.score + "</li>";
                    }
    
                    highscore += "</ul>";
                    document.getElementById("hslist").innerHTML = highscore;
                });
            });
        });
    }
}

interface Highscore {
    name: string;
    points: number;
}

const config = {
    type: CANVAS,
    width: 512,
    height: 512,
    scene: [ShooterScene],
    physics: { default: 'arcade' },
    audio: { noAudio: true }
};

new Game(config);
