# Build Script

Our `build.sh` script knows a few tricks. By default it runs with the `Test` target.

The buildserver passes in `BITBUCKET_BUILD_NUMBER` as an integer to version the results and `BUILD_DOCKER_REGISTRY` to point to a Docker registry to push the resulting Docker images.

## NpmInstall

Run an `npm install` to setup [Commitizen](https://commitizen.github.io/cz-cli/) and [Semantic Release](https://semantic-release.gitbook.io/semantic-release/).

## DotNetCli

Checks if the requested .NET Core SDK and runtime version defined in `global.json` are available.
We are pedantic about these being the exact versions to have identical builds everywhere.

## Clean

Make sure we have a clean build directory to start with.

## Restore

Restore dependencies for `debian.8-x64` and `win10-x64` using dotnet restore and Paket.

## Build

Builds the solution in Release mode with the .NET Core SDK and runtime specified in `global.json`
It builds it platform-neutral, `debian.8-x64` and `win10-x64` version.

## Test

Runs `dotnet test` against the test projects.

## Publish

Runs a `dotnet publish` for the `debian.8-x64` and `win10-x64` version as a self-contained application.
It does this using the Release configuration.

## Pack

Packs the solution using Paket in Release mode and places the result in the `dist` folder.
This is usually used to build documentation NuGet packages.

## Containerize

Executes a `docker build` to package the application as a docker image. It does not use a Docker cache.
The result is tagged as latest and with the current version number.

## DockerLogin

Executes `ci-docker-login.sh`, which does an aws ecr login to login to Amazon Elastic Container Registry.
This uses the local aws settings, make sure they are working!

## Push

Executes `docker push` to push the built images to the registry.
