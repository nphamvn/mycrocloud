#!/bin/bash

# Check if migration name is provided as an argument
if [ $# -eq 0 ]; then
  echo "Error: Migration name is required."
  exit 1
fi

# Assign the migration name from the command line argument
migration_name=$1

# Execute the migration command
dotnet ef migrations add $migration_name -s ../WebApp.RestApi/WebApp.RestApi.csproj
