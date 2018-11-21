# Web NGINX Web Server Dockerfile

#---------------------
FROM node:10.13-alpine AS restore

WORKDIR /app

COPY package.json /app
COPY package-lock.json /app

RUN set -x \
    && npm -v \
    && npm set progress=false \
    && npm install --no-progress

#---------------------
FROM restore AS build
ARG ANGULAR_ENV=

WORKDIR /app

COPY . /app

RUN npm run build:${ANGULAR_ENV}

#---------------------
FROM nginx:1.15.6-alpine AS final

EXPOSE 80

COPY --from=build /app/dist/browser /usr/share/nginx/html

COPY --from=build /app/nginx-custom.conf /etc/nginx/conf.d/default.conf
