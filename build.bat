docker-compose build
docker tag highscore-exercise-tklecka_frontend tklecka/highscore-exercise-tklecka_frontend:latest
docker tag highscore-exercise-tklecka_backend tklecka/highscore-exercise-tklecka_backend:latest
docker push highscore-exercise-tklecka_backend:latest
docker push highscore-exercise-tklecka_frontend:latest
pause