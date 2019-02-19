# Documentation Not Included Website

This repository contains the source code for https://www.dnistream.live - a platform agnostic development podcast hosted by [Chris Sebok](https://github.com/Bidthedog), [Josey Howarth](https://github.com/sudomistress) and [Patryk Kowalik](https://github.com/imrooniel).

## Code Coverage

Code coverage is slightly lower than normal as this is a personal project, though all pertinent back-end service code is covered adequately. Front end tests are being worked on, along with 100% coverage of Web API controllers.

## Solution Parts

This solution consists of the following:

* A .Net Core 2.2.1 RESTful Web API ([src/DNI.API](src/DNI.API))
* An Angular 7.2 front-end (with a small number of Karma / Jasmine tests) ([src/DNI.Web](src/DNI.Web))
* An SMTP Server for sending emails ([src/DNI.SMTP](src/DNI.SMTP))
* A docker / docker-compose configuration for production deployment ([docker-compose.yml](docker-compose.yml))
* NGINX configuration files for deployment ([NGINX folder](nginx))

All data comes from Fireside and YouTube APIs, so there is no data persistence - caching will be implemented in the near future.

## How to run

The solution relies heavily on a number of configuration values and secret keys for API integration. These keys are *not* pushed to the repository, but are accessed from a local `.env` file on the developer's machine. All APIs are configured with both development and production keys with the relevant security restrictions (IP, referrer etc). Please speak to [Chris Sebok](https://github.com/Bidthedog) if you need access to these keys, though you can set up and configure your own if you are forking this repositry to create a similar website.

A number of helper scripts are included to get the solution working in development mode on a Windows box. Docker is **not** used for development, but can be configured for "local production" testing and full production deployment.

Both local development and dev / prod deployment via docker relies on the below file:

* Create a `.env` file in the repository root with the following contents (sensitive info redacted):

##### Production values (Linux deployment):
```
BUILD_ENVIRONMENT=prod
ASPNETCORE_ENVIRONMENT=Production
CAPTCHA_KEY=<REDACTED>
ASPNET_CONFIGURATION=Release
SMTP_SERVER=dni.smtp
SMTP_MAILNAME=dniapi.prod.website_default
SMARTHOST_ADDRESS=<REDACTED>
SMARTHOST_PORT=25
SMARTHOST_USER=<REDACTED>
SMARTHOST_PASSWORD=<REDACTED>
SMARTHOST_ALIASES=*.<REDACTED>
LOG_MOUNT_PATH=/app/api.dnistream.live/logs
LOCAL_WEB_PORT=8080
LOCAL_API_PORT=8181
ERROR_EMAIL_FROM=<REDACTED>
ERROR_EMAIL_TO=<REDACTED>
CONTACT_EMAIL_TO=<REDACTED>
YOUTUBE_API_KEY=<REDACTED>
YOUTUBE_REFERRER=https://www.dnistream.live
YOUTUBE_ORIGIN=https://www.dnistream.live
```
##### Development values (local Windows development and docker deployment testing):
```
BUILD_ENVIRONMENT=dev
ASPNETCORE_ENVIRONMENT=Development
CAPTCHA_KEY=<REDACTED>
ASPNET_CONFIGURATION=Debug
SMTP_SERVER=localhost
SMTP_MAILNAME=localhost
SMARTHOST_ADDRESS=<REDACTED>
SMARTHOST_PORT=25
SMARTHOST_USER=<REDACTED>
SMARTHOST_PASSWORD=<REDACTED>
SMARTHOST_ALIASES=*.<REDACTED>
LOG_MOUNT_PATH=f:/docker-mounts/dni/apilogs
LOCAL_WEB_PORT=4200
LOCAL_API_PORT=12341
ERROR_EMAIL_FROM=<REDACTED>
ERROR_EMAIL_TO=<REDACTED>
CONTACT_EMAIL_TO=<REDACTED>
YOUTUBE_API_KEY=<REDACTED>
YOUTUBE_REFERRER=http://localhost:4200
YOUTUBE_ORIGIN=http://localhost:4200
```

### Development (local IDE)

* Ensure the `.env` file has been created and contains the correct **Development** key / value pairs for all config values
* To run the .Net Core Web API, run the following scripts (in order) in a Powershell window:
    * `setup-env-vars.ps1`
    * `run-service-hosts.ps1`
* To run the Angular cli website, run the following in a separate Command Prompt, Git Bash shell or Powershell Window:
    * `npm start`
* Open the `DNI.sln` file in visual Studio to work on the .Net Core WebAPI solution
* Open the `src/DNI.Web` folder in your text editor of choice (I use Visual Studio Code or Notepad++) to work on the Angular front-end application

### Production (deployed via docker)

* Ensure the `.env` file has been created and contains the correct **Production** key / value pairs for all config values
* Run the following command from the solution root to build and host all the docker containers:
    * `docker-compose up -d --build`

## Angular SSR

This project is a public facing website, and as such uses Angular Universal / SSR to compile the front-end website. The following commands can be run inside the [src/DNI.Web](src/DNI.Web) folder to build and serve the SSR website via Node.js and Express:

* `npm run build:ssr`
* `npm run serve:ssr`

[This Dockerfile](src/DNI.Web/Dockerfile) contains all steps needed to build and compile the Angular SSR app for production.

## Versioning

A Powershell script has been provided that iterates through all files that need to be changed when a new version is released. The API, .Net Core assemblies and Angular app are all updated with the same version. This project uses GitFlow and https://semver.org/ for branching and versioning respectively.

For example, to update the app to version 3.4.5, run the following commands on a **clean commit**

* `version.ps1 3 4 5`
* `git commit -am "Version 3.4.5"`
* `git push`
* `git tag 3.4.5`
* `git push --tags`

## Development pre-requisites:

Generally this project uses the latest releases. At the time of writing, the following versions are in use:

* Node.js LTS 10.13.0
* Angular CLI 7.2.2
* Dotnet Core SDK 2.2.102
* Docker 18.09.0 (Windows)
* Docker-Compose 1.23.2 (Windows)