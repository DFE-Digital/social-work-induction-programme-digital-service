FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["apps/Dfe.Sww.Ecf.Ui/Dfe.Sww.Ecf.Ui.csproj", "apps/Dfe.Sww.Ecf.Ui/"]
RUN dotnet restore "apps/Dfe.Sww.Ecf.Ui/Dfe.Sww.Ecf.Ui.csproj"
COPY . .
WORKDIR "/src/apps/Dfe.Sww.Ecf.Ui"
RUN dotnet build "Dfe.Sww.Ecf.Ui.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Dfe.Sww.Ecf.Ui.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dfe.Sww.Ecf.Ui.dll"]
