# Node Angular SSR Dockerfile

#---------------------
FROM node:12.18.3-alpine AS restore

WORKDIR /app

COPY package.json /app
COPY package-lock.json /app

RUN set -x \
    && npm -v \
    && npm set progress=false \
    && npm install --no-progress

#---------------------
FROM restore AS build
ARG ANGULAR_ENV

WORKDIR /app

COPY . /app

RUN echo $ANGULAR_ENV

RUN npm run build:ssr

FROM node:12.18.3-alpine AS final
WORKDIR /app

COPY --from=build /app/package.json /app

COPY --from=build /app/dist /app/dist

EXPOSE 4200

CMD ["npm", "run", "serve:ssr"]
