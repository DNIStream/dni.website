#!/bin/bash
# DNI Stream Deployment Script
# Three additional files must exist on the deployment server for this script to function:
#
# .env              - Contains all the required environment variables for the build
# docker-username   - Contains the account name that has "Read" access to the below DOCKER_REGISTRY
# docker-password   - Contains the GitHub Personal Access Key for the above account
#
# In addition, this script must have execute permissions to run.
set +x;
ERRORS_TRAPPED=0;
VERSION=$1;
OFF="\033[0m";
RED="\e[31m%s\n${OFF}";
YLW="\e[33m%s\n${OFF}";
if [ -z "$VERSION" ]; then
    printf "${YLW}" "Version expected as script input (e.g. './deploy.sh 2.0.0')."
    ((ERRORS_TRAPPED++));
fi
if [[ $EUID -ne 0 ]]; then
   printf "${YLW}" "This script must be run in sudo mode.";
   ((ERRORS_TRAPPED++));
fi

DOCKER_REGISTRY=docker.pkg.github.com;
DOCKER_PATH=${DOCKER_REGISTRY}/dnistream/dni.website/;
GIT_URL=git@github.com:DNIStream/dni.website.git;
TEMP_GIT_DIR_NAME=dni.website.temp/;
API_IMAGE=${DOCKER_PATH}dniapi;
WEB_IMAGE=${DOCKER_PATH}dniweb;
DOCKER_USERNAME_FILE=docker-username;
DOCKER_PASSWORD_FILE=docker-password;
ENV_FILE=.env;

if [ ! -f ./${ENV_FILE} ]; then
    printf "${YLW}" "${ENV_FILE} not found."
    ((ERRORS_TRAPPED++));
fi
DOCKER_USERNAME=$( cat ./$DOCKER_USERNAME_FILE );
if [ $? -ne 0 ]; then
    printf "${YLW}" "$DOCKER_USERNAME_FILE not found."
    ((ERRORS_TRAPPED++));
fi
DOCKER_PASSWORD=$( cat ./$DOCKER_PASSWORD_FILE );
if [ $? -ne 0 ]; then
    printf "${YLW}" "$DOCKER_PASSWORD_FILE not found."
    ((ERRORS_TRAPPED++));
fi

if [ $ERRORS_TRAPPED -gt 0 ]; then
    printf "${RED}" "$ERRORS_TRAPPED script validation errors."
    return 1;
fi

set -e;

# Log in to github registry
echo ${DOCKER_PASSWORD} | docker login ${DOCKER_REGISTRY} --username ${DOCKER_USERNAME} --password-stdin

set -x;

# Delete the temp folder
rm -rf ${TEMP_GIT_DIR_NAME};

# Clone the tag
git clone --branch $VERSION --depth 1 ${GIT_URL} ${TEMP_GIT_DIR_NAME};

# Pull down the versioned images
docker pull ${API_IMAGE}:latest-restore;
docker pull ${API_IMAGE}:latest-build;
docker pull ${API_IMAGE}:latest;
docker pull ${WEB_IMAGE}:latest-restore;
docker pull ${WEB_IMAGE}:latest-build;
docker pull ${WEB_IMAGE}:latest;

docker logout ${DOCKER_REGISTRY};

# Copy required files
cp $DOCKER_USERNAME_FILE ${TEMP_GIT_DIR_NAME};
cp $DOCKER_PASSWORD_FILE ${TEMP_GIT_DIR_NAME};
cp $ENV_FILE ${TEMP_GIT_DIR_NAME};

# Switch to the temp git dir
pushd ${TEMP_GIT_DIR_NAME};

# Spin up latest images
docker-compose up -d --no-build --force-recreate --scale dni.api.restore=0 --scale dni.api.build=0 --scale dni.web.restore=0 --scale dni.web.build=0;

# Cleanup irrelevant images
docker image rm -f ${API_IMAGE}:latest-restore;
docker image rm -f ${API_IMAGE}:latest-build;
docker image rm -f ${WEB_IMAGE}:latest-restore;
docker image rm -f ${WEB_IMAGE}:latest-build;

# Update NGINX
cp nginx/http-to-https.conf /etc/nginx/sites-available/http-to-https.conf;
cp nginx/prod-www.dnistream.live.conf /etc/nginx/sites-available/prod-www.dnistream.live.conf;

ln -s /etc/nginx/sites-available/http-to-https.conf /etc/nginx/sites-enabled || true;
ln -s /etc/nginx/sites-available/prod-www.dnistream.live.conf /etc/nginx/sites-enabled || true;

service nginx reload;

popd;

# Remove temp git dir
rm -rf ${TEMP_GIT_DIR_NAME};
