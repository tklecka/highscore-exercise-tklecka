# Build the webpack-application
FROM node:alpine AS node
WORKDIR /app

COPY . .
RUN npm install
RUN npm run-script build

FROM nginx:alpine
COPY --from=node /app/dist/ /usr/share/nginx/html
