#!/bin/bash
# DNI Stream Deployment Script
set -e;

DOCKER_REGISTRY=docker.pkg.github.com;
DOCKER_PATH=${DOCKER_REGISTRY}/dnistream/dni.website/;
API_IMAGE=${DOCKER_PATH}dniapi;
WEB_IMAGE=${DOCKER_PATH}dniweb;
DOCKER_USERNAME_FILE=docker-username;
DOCKER_PASSWORD_FILE=docker-password;
# TODO: Move these into environment variables on the server
DOCKER_USERNAME=$( cat ./$DOCKER_USERNAME_FILE );
DOCKER_PASSWORD=$( cat ./$DOCKER_PASSWORD_FILE );
# Log in to github registry
echo ${DOCKER_PASSWORD} | docker login ${DOCKER_REGISTRY} --username ${DOCKER_USERNAME} --password-stdin

set -x;

docker pull ${API_IMAGE}:latest-restore;
docker pull ${API_IMAGE}:latest-build;
docker pull ${API_IMAGE}:latest;
docker pull ${WEB_IMAGE}:latest-restore;
docker pull ${WEB_IMAGE}:latest-build;
docker pull ${WEB_IMAGE}:latest;

docker logout ${DOCKER_REGISTRY}

docker image ls;

# Run latest images
docker-compose up -d --no-build --force-recreate --scale dni.api.restore=0 --scale dni.api.build=0 --scale dni.web.restore=0 --scale dni.web.build=0;

# Cleanup
docker image rm -f ${API_IMAGE}:latest-restore;
docker image rm -f ${API_IMAGE}:latest-build;
docker image rm -f ${WEB_IMAGE}:latest-restore;
docker image rm -f ${WEB_IMAGE}:latest-build;
