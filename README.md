# Documentation Not Included Website

![CD Build Status](https://github.com/DNIStream/dni.website/workflows/DNI%20Stream%20Website%20CD%20Build/badge.svg)

![Latest CI Build Status](https://github.com/DNIStream/dni.website/workflows/DNI%20Stream%20Website%20CI%20Build/badge.svg?branch=feature%2F*)

This repository contains the source code for https://www.dnistream.live - a platform agnostic development podcast hosted by [Chris Sebok](https://github.com/Bidthedog) and [Josey Howarth](https://github.com/sudomistress).

## Code Coverage

Front-end test coverage is slightly lower than normal, though all pertinent back-end service code is covered adequately. Front end tests are being worked on, along with 100% coverage of Web API controllers.

## Solution Parts

This solution consists of the following:

* A .Net Core 2.2.1 RESTful Web API ([src/DNI.API](src/DNI.API))
* An Angular front-end (with a small number of Karma / Jasmine tests) ([src/DNI.Web](src/DNI.Web))
* A docker / docker-compose configuration for production deployment ([docker-compose.yml](docker-compose.yml))
* NGINX configuration files for a docker reverse proxy deployment ([NGINX folder](nginx))

All data comes from Fireside's RSS feed.

## How to Run

The solution relies heavily on a number of configuration values and secret keys for API integration. These keys are *not* pushed to the repository, but are accessed from a local `.env` file on the developer's machine, and within the CI / CD pipeline. All APIs are configured with both development and production keys with the relevant security restrictions (IP, referrer etc). Please speak to [Chris Sebok](https://github.com/Bidthedog) if you need access to these keys, though you can set up and configure your own if you are forking this repository to create a similar website.

A number of helper scripts are included to facilitate the setup of a development environment on a Windows box. Docker is **not** used for development, but can be configured for "local production" testing.

The local development scripts **and** local docker production deployment testing require a `.env` file to be created in the repository root with the following contents (sensitive info redacted):

##### Production values
```
BUILD_ENVIRONMENT=prod
ASPNETCORE_ENVIRONMENT=Production
CAPTCHA_KEY=<REDACTED>
ASPNET_CONFIGURATION=Release
SMTP_SERVER=<REDACTED>
SMTP_PORT=25
SMTP_ENABLE_SSL=True
SMTP_USERNAME=<REDACTED>
SMTP_PASSWORD=<REDACTED>
LOG_MOUNT_PATH=/app/api.dnistream.live/logs
LOCAL_WEB_PORT=8080
LOCAL_API_PORT=8181
ERROR_EMAIL_FROM=<REDACTED>
ERROR_EMAIL_TO=<REDACTED>
CONTACT_EMAIL_TO=<REDACTED>
```
##### Development values (local Windows development and local docker deployment testing):
```
BUILD_ENVIRONMENT=dev
ASPNETCORE_ENVIRONMENT=Development
CAPTCHA_KEY=<REDACTED>
ASPNET_CONFIGURATION=Debug
SMTP_SERVER=host.docker.internal
SMTP_ENABLE_SSL=False
SMTP_USERNAME=
SMTP_PASSWORD=
LOG_MOUNT_PATH=f:/docker-mounts/dni/apilogs
LOCAL_WEB_PORT=4200
LOCAL_API_PORT=12341
ERROR_EMAIL_FROM=<REDACTED>
ERROR_EMAIL_TO=<REDACTED>
CONTACT_EMAIL_TO=<REDACTED>
```

### Development (local IDE)

* Ensure the `.env` file has been created and contains the correct **Development** key / value pairs for all config values
* To run the .Net Core Web API, run the following scripts (in order) in a Powershell window:
    * `setup-env-vars.ps1`
    * `run-service-hosts.ps1`
* To host the Angular website, run the following command in your shell of choice:
    * `npm start`
* Open the `DNI.sln` file in visual Studio to work on the .Net Core WebAPI solution
* Open the `src/DNI.Web` folder in your text editor of choice (I use Visual Studio Code or Notepad++) to work on the Angular front-end application

### Production (manual deployment via docker)

*This deployment is now handled with Travis CI, which is configured to build docker images, push them to docker hub, then automatically host the containers on the web host. You can see this deployment configuration in [.travis.yml](travis.yml). All configuration values (listed above) are stored in Travis CI's environment variables.*

* Ensure the `.env` file has been created and contains the correct **Production** key / value pairs for all config values
* Pull the repository on to the server
* Run the following command from the solution root to build and host all the docker containers:
    * `docker-compose up -d --build`

## Angular SSR

This project is a public facing website, and as such uses Angular Universal / SSR. The following commands can be run inside the [src/DNI.Web](src/DNI.Web) folder to build and serve the SSR website via Node.js and Express:

* `npm run build:ssr`
* `npm run serve:ssr`

[This Dockerfile](src/DNI.Web/Dockerfile) contains all steps needed to build and compile the Angular SSR app for production.

## Versioning

A Powershell script has been provided that iterates through all files that need to be changed when a new version is released. The API, .Net Core assemblies and Angular app are all updated with the same version. This project uses GitFlow for branching and https://semver.org/ versioning.

For example, to update the app to version 3.4.5, run the following commands on a **clean commit**

* `./version.ps1 3 4 5`
* `git commit -am "Version 3.4.5"`
* `git push`
* `git tag 3.4.5`
* `git push --tags`

## Development pre-requisites:

At the time of writing, the following SDK / tool versions are in use:

* Node.js LTS 12.16.1
* Angular CLI 8.3.25
* Dotnet Core SDK 2.2.401
* Docker 19.03.5, build 633a0ea (Windows)
* Docker-Compose 1.25.4, build 8d51620a (Windows)
* Visual Studio Professional 2019 (16.4.5)
* VSCode 1.43.0
