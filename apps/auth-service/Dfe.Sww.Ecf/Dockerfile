# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.18
ARG GIT_SHA
ENV SENTRY_RELEASE ${GIT_SHA}
ENV ASPNETCORE_HTTP_PORTS 80
COPY src/Dfe.Sww.Ecf.Cli/bin/Release/net8.0/publish/ Apps/TrsCli/
COPY src/Dfe.Sww.Ecf.AuthorizeAccess/bin/Release/net8.0/publish/ Apps/AuthorizeAccess/
COPY db.sh Apps/db.sh
WORKDIR /Apps

RUN chmod +x /Apps/db.sh

# Install Culture prerequisities
RUN apk add --no-cache \
    tzdata \
    icu-data-full \
    icu-libs

# Enable all cultures
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Install Postgres client
RUN apk --no-cache add postgresql14-client

# Fix for invoking ecfcli
RUN apk --no-cache add libc6-compat

# Install fonts for PDF generation
RUN apk --no-cache add msttcorefonts-installer fontconfig && \
    update-ms-fonts && \
    fc-cache -f

ENV PATH="${PATH}:/Apps/EcfCli"
