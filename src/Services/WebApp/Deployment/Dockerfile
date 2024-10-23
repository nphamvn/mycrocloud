# Base Node.js image
FROM node:latest

# Set working directory inside the container
WORKDIR /app

# Arguments for Git repository, access token, build output directory, and working directory
ARG REPO_URL
ARG REPO_TOKEN
ARG BUILD_WORKING_DIR
ARG BUILD_OUTPUT_DIR=dist

# Install git and other dependencies
RUN apt-get update && apt-get install -y git

# Clone the repository using the provided Git URL and Access Token
RUN git clone https://$REPO_TOKEN@$REPO_URL /app

# Set working directory to the specified build working directory
WORKDIR /app/$BUILD_WORKING_DIR

# Install Node.js dependencies
RUN npm install

# Run the build command and move the output to the specified build output directory
RUN npm run build && mv /app/$BUILD_OUTPUT_DIR/* /dist

# Default CMD for running the container
CMD ["npm", "run", "build"]
