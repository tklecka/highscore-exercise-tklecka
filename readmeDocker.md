# Docker

## Build the image
~~~docker
docker build -t <name>:<tag> Backend/HighscoreBackend/HighscoreBackend/
docker build -t <name>:<tag> Game/
~~~~

## Start the container
~~~~docker
docker run -p 5000:5000 <backend>
docker run -p 8085:80 <frontend>
~~~~
In the browser you can access the game via:
~~~~
localhost:8085
~~~~

## Docker Compose
### To build/start the Container in a simpler way:
~~~~docker
docker-compose -f "docker-compose.yml" up -d --build
~~~~


## PS: In the current version this won't work anymore, due to the fact that we designed it to work in Azure (other links, reCAPTCHA)

## Access the game on Azure with the following link: https://highscore-exercise-tklecka.azurewebsites.net/