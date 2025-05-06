# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.18 AS auth-service
ARG AZURE_KUDU_SSH_USER
ARG FULL_TAG
ENV VERSION=${FULL_TAG}

COPY src/Dfe.Sww.Ecf.Cli/bin/Release/net8.0/publish/ Apps/EcfCli/
COPY src/Dfe.Sww.Ecf.AuthorizeAccess/bin/Release/net8.0/publish/ Apps/AuthorizeAccess/
RUN rm Apps/AuthorizeAccess/appsettings.*
COPY src/Dfe.Sww.Ecf.AuthorizeAccess/appsettings.Azure.json Apps/AuthorizeAccess/appsettings.json
WORKDIR /Apps
COPY entry-point.sh ./entry-point.sh
RUN chmod +x entry-point.sh

RUN sed -i \
    -e "s|\"Version\": *\"[^\"]*\"|\"Version\": \"${VERSION}\"|" \
    AuthorizeAccess/appsettings.json

# Install culture prerequisities
RUN apk add --no-cache \
    tzdata \
    icu-data-full \
    icu-libs

# Enable all cultures
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Install Postgres client / SSH Server for Kudu
RUN apk --no-cache add postgresql15-client \
    openssh \
    shadow

# Support kudu tools SSH for remote debug
COPY sshd_config /etc/ssh/sshd_config
RUN --mount=type=secret,id=AZURE_KUDU_SSH_PASSWORD \
    export AZURE_KUDU_SSH_PASSWORD=$(cat /run/secrets/AZURE_KUDU_SSH_PASSWORD) \
    && echo "$AZURE_KUDU_SSH_USER:$AZURE_KUDU_SSH_PASSWORD" | chpasswd \
    && > /etc/motd \
    && mkdir -p /var/run/sshd \
    && chmod 0755 /var/run/sshd \
    && ssh-keygen -A
        
# Fix for invoking ecfcli
RUN apk --no-cache add libc6-compat

# Install fonts for PDF generation
RUN apk --no-cache add msttcorefonts-installer fontconfig && \
    update-ms-fonts && \
    fc-cache -f

ENV PATH="${PATH}:/Apps/EcfCli"
ENV ASPNETCORE_HTTP_PORTS=80

WORKDIR /Apps/AuthorizeAccess
ENTRYPOINT ["/Apps/entry-point.sh"]
CMD ["dotnet","/Apps/AuthorizeAccess/Dfe.Sww.Ecf.AuthorizeAccess.dll"]

EXPOSE 80
