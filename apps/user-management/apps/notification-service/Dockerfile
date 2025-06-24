# Base image for the Azure Functions runtime for .NET 8
FROM mcr.microsoft.com/azure-functions/dotnet:4-dotnet8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY DfeSwwEcf.NotificationService/*.csproj ./DfeSwwEcf.NotificationService/
COPY DfeSwwEcf.NotificationService.Models/*.csproj ./DfeSwwEcf.NotificationService.Models/
COPY DfeSwwEcf.NotificationService.Services/*.csproj ./DfeSwwEcf.NotificationService.Services/

RUN dotnet restore DfeSwwEcf.NotificationService/DfeSwwEcf.NotificationService.csproj

COPY . ./

#RUN dotnet publish -c Release -o /out
FROM build AS publish
RUN dotnet publish DfeSwwEcf.NotificationService/DfeSwwEcf.NotificationService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final image, copy the published assets
FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot