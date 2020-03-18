echo Starting deploy script
#if [ "$TRAVIS_PULL_REQUEST" = "false" ]; then
    echo $TRAVIS_PULL_REQUEST;
    echo $TRAVIS_BRANCH;
    $version = line=$(head -n 1 VERSION);
    echo $version;
    docker image ls;
#; fi
