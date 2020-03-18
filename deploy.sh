echo Starting deploy script
#if [ "$TRAVIS_PULL_REQUEST" = "false" ]; then
    echo $TRAVIS_PULL_REQUEST;
    echo $TRAVIS_BRANCH;
    version=$(head -n 1 VERSION);
    echo $version;
    docker image ls;
    # TODO: Tag dni docker images
    # TODO: Auth with docker hub
    # TODO: Push to docker hub
    # TODO: Log on to DO box
    # TODO: Pull docker hub images
    # TODO: Potentially copy NGINX files
    # TODO: Run images on DO box
    # TODO: Clean up old images?
#; fi
