FROM mcr.microsoft.com/devcontainers/dotnet:8.0
ARG NODE_VERSION=20

RUN apt update && apt install curl -y

# Install just command runner
RUN sudo curl --proto '=https' --tlsv1.2 -sSf https://just.systems/install.sh | bash -s -- --to /usr/local/bin
