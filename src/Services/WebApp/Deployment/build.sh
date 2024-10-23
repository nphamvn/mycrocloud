#!/bin/bash

# Variables
IMAGE_NAME="nodejs-builder"
CONTAINER_NAME="nodejs-build-container"
REPO_URL=${1:-"github.com/your-repo-url.git"}  # Pass as the first argument, or default to your URL
REPO_TOKEN=${2:-"your-access-token"}            # Pass as the second argument, or default to a token
BUILD_WORKING_DIR=${3:-"/app"}                  # Pass as the third argument, default is root dir
BUILD_OUTPUT_DIR=${4:-"/app/dist"}              # Pass as the fourth argument, default is /app/dist
LOCAL_OUTPUT_DIR=${5:-"./dist"}                 # Local output dir, defaults to ./dist
REBUILD_IMAGE=${6:-"no"}                        # Pass 'yes' to rebuild image, default is no

# Function to build the Docker image
build_image() {
  echo "Building Docker image: $IMAGE_NAME"
  docker build -t $IMAGE_NAME \
    --build-arg REPO_URL=$REPO_URL \
    --build-arg REPO_TOKEN=$REPO_TOKEN \
    --build-arg BUILD_WORKING_DIR=$BUILD_WORKING_DIR \
    --build-arg BUILD_OUTPUT_DIR=$BUILD_OUTPUT_DIR \
    .
}

# Build image if it doesn't exist or REBUILD_IMAGE is set to "yes"
if [[ "$REBUILD_IMAGE" == "yes" || "$(docker images -q $IMAGE_NAME 2> /dev/null)" == "" ]]; then
  build_image
else
  echo "Docker image $IMAGE_NAME already exists. Skipping build."
fi

# Stop and remove any existing container with the same name
if [ "$(docker ps -q -f name=$CONTAINER_NAME)" ]; then
    echo "Stopping existing container $CONTAINER_NAME"
    docker stop $CONTAINER_NAME
    docker rm $CONTAINER_NAME
fi

# Run the container
echo "Running Docker container: $CONTAINER_NAME"
docker run --name $CONTAINER_NAME -d $IMAGE_NAME

# Wait for the build to finish
echo "Waiting for build to finish..."
docker logs -f $CONTAINER_NAME

# Copy the built files from the container to the local directory
echo "Copying built files from $BUILD_OUTPUT_DIR in container to $LOCAL_OUTPUT_DIR"
docker cp $CONTAINER_NAME:$BUILD_OUTPUT_DIR $LOCAL_OUTPUT_DIR

# Clean up the container after build is complete
echo "Cleaning up..."
docker rm -f $CONTAINER_NAME

echo "Build completed. Built files are available in $LOCAL_OUTPUT_DIR"
