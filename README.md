# Documentation Not Included Website

![DNI Stream Website CD Build](https://github.com/DNIStream/dni.website/workflows/DNI%20Stream%20Website%20CD%20Build/badge.svg?branch=master)

This repository contains the source code for https://www.dnistream.live - a platform agnostic development podcast website hosted by [Chris Sebok](https://github.com/Bidthedog) and [Josey Howarth](https://github.com/sudomistress).

## Code Coverage

Front-end test coverage is slightly lower than normal, though all pertinent back-end service code is covered adequately.

## Solution Parts

This solution consists of the following artifacts:

* A .Net Core 3.1 RESTful Web API ([src/DNI.API](src/DNI.API))
* An Angular front-end (with a small number of Karma / Jasmine tests) ([src/DNI.Web](src/DNI.Web))
* A docker / docker-compose configuration for production deployment ([docker-compose.yml](docker-compose.yml))
* NGINX configuration files for a docker reverse proxy deployment ([NGINX folder](nginx))

All data comes from Fireside's RSS feed.

## How to Run

The solution relies heavily on a number of configuration values and secret keys for API integration. These keys are *not* pushed to the repository, but are accessed from a local `.env` file on the developer's machine, and within the CI / CD pipeline. All APIs are configured with both development and production keys with the relevant security restrictions (IP, referrer etc). Please speak to [Chris Sebok](https://github.com/Bidthedog) if you need access to these keys, though you can set up and configure your own if you are forking this repository to create a similar website.

A number of helper scripts are included to facilitate the setup of a development environment on a Windows box. Docker is **not** used for development, but can be configured for local testing & debugging of production deployment.

Both the local development environment **and** the local docker deployment testing environments require a `.env` file to be created in the repository root with the following contents (sensitive info redacted):

##### Production values
```
IMAGE_PREFIX=docker.pkg.github.com/dnistream/dni.website/
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
IMAGE_PREFIX=docker.pkg.github.com/dnistream/dni.website/ (or empty)
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

### Development (local IDE development and debugging)

* Clone the repository
* Create a `feature/*` branch
* Ensure the `.env` file has been created and contains the correct **development** key / value pairs for all config values
* To run the .Net Core Web API in local developer testing mode, run the following scripts (in order) in a Powershell window:
    * [setup-env-vars.ps1](setup-env-vars.ps1)
    * [run-service-hosts.ps1](run-service-hosts.ps1)
* To host the Angular website, run the following command in your shell of choice:
    * `npm start`
* Open the [DNI.sln](DNI.sln) file in Visual Studio to work on the .Net Core WebAPI solution. You can attach to the `dotnet` process to perform line by line debugging.
* Open the [src/DNI.Web](src/DNI.Web) folder in your text editor of choice to work on the Angular front-end application. I use Visual Studio Code and Notepad++.
* Push code to your feature/* branch
* Wait for the [CI GitHub Action](.github/workflows/ci.yml) to complete.

### Production & Release

The production build and docker image packages are compiled with [this GitHub Action](.github/workflows/cd.yml). Deployment of these packages is performed manually<sup>&#177;</sup>, but has been scripted in [deploy.sh](deploy.sh).

<sup>&#177;</sup> *Deployment is manual because installing a GitHub Actions Runner on a production server for a public GitHub repo is a security risk. See [here](https://help.github.com/en/actions/hosting-your-own-runners/adding-self-hosted-runners) for further information.*

It is recommended that you create a new user on your production server and add it to the sudo group, rather than run these commands as root.

#### Release

To deploy this website, considering you want to deploy version `2.0.4`:

* Commit your changes to a `feature/*` branch
* Run the `version.ps1 2.0.4` in powershell
* Commit the various version file changes
* Create a PR into `master`
* Wait for all CI / PR actions to complete
* Merge the PR into master
* Tag the changes with the version `git tag 2.0.4`
* Push the tags with `git push --tags`
* Wait for the CD action to complete

On the deployment target (server):

* Copy the latest [deploy.sh](deploy.sh) script to the server
* Grant the [deploy.sh](deploy.sh) script execute permissions
```
chmod +x deploy.sh
```
* Ensure the `.env` file has been created and contains the correct **production** key / value pairs for all config values
* Ensure the `docker-username` and `docker-password` files has been created and contain valid values
* Execute the script targeting the tag to deploy:
```
./deploy.sh 2.0.0
```

## Angular SSR

This project contains a public facing website, and as such uses Angular Universal Server Side Rendering. The following commands can be run inside the [src/DNI.Web](src/DNI.Web) folder to build and serve the Universal website via Node.js and Express:

### Production build (will not work fully locally):
```
npm run build:ssr-prod
npm run serve:ssr-prod
```
### Development Build (for local testing)
```
npm run build:ssr-dev
npm run serve:ssr-dev
```

[This Dockerfile](src/DNI.Web/Dockerfile) contains all steps needed to build and compile the Angular Universal app for production.

## Versioning

A Powershell script has been provided that iterates through all files that need to be changed when a new version is released. The API, .Net Core assemblies and Angular website are versioned together.

This project uses GitFlow for branching and https://semver.org/ for versioning.

For example, to update the app to version 3.4.5, run the following commands on a **clean commit**

* `./version.ps1 3 4 5`
* `git commit -am "Version 3.4.5"`
* `git push`
* `git tag 3.4.5`
* `git push --tags`

Only once the repository is tagged can it be deployed using the [deploy.sh](deploy.sh) script.

## Development pre-requisites:

At the time of writing, the following SDK / tool versions are in use:

* Node.js LTS 12.16.1
* Angular CLI 8.3.25
* Dotnet Core SDK 3.1.201
* Docker 19.03.8, build afacb8b (Windows)
* Docker-Compose 1.25.4, build 8d51620a (Windows)
* Visual Studio Professional 2019 (16.4.5)
* VSCode 1.45.0
