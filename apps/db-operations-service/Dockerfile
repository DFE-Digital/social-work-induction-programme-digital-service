FROM mcr.microsoft.com/azure-functions/dotnet:4-dotnet8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

COPY DfeSwwEcf.DbOperationsService/*.csproj ./DfeSwwEcf.DbOperationsService/
RUN dotnet restore DfeSwwEcf.DbOperationsService/DfeSwwEcf.DbOperationsService.csproj

COPY . ./

FROM build AS publish
RUN dotnet publish DfeSwwEcf.DbOperationsService/DfeSwwEcf.DbOperationsService.csproj -c Release -o /app/publish /p:UseAppHost=false

RUN apt-get update && apt-get install -y \
    postgresql-client \
    && rm -rf /var/lib/apt/lists/*

COPY run-backup.sh /app/scripts/
COPY run-restore.sh /app/scripts/
RUN chmod +x /app/scripts/*.sh

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .

# Update PATH to include the scripts directory
ENV PATH="${PATH}:/app/scripts"

ENV AzureWebJobsScriptRoot=/home/site/wwwroot