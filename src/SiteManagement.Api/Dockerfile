# syntax=docker/dockerfile:1.7
# Multi-stage build for SiteManagement.Api
# Build context expected to be the repository root (handled by docker-compose).

# ---------- 1) Restore --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

# Copy NuGet config + central package manifest first so they participate in cache.
COPY NuGet.config Directory.Packages.props ./

# Copy only csproj files to maximize layer caching for restore.
COPY src/SiteManagement.Domain/SiteManagement.Domain.csproj         src/SiteManagement.Domain/
COPY src/SiteManagement.Application/SiteManagement.Application.csproj src/SiteManagement.Application/
COPY src/SiteManagement.Infrastructure/SiteManagement.Infrastructure.csproj src/SiteManagement.Infrastructure/
COPY src/SiteManagement.Api/SiteManagement.Api.csproj               src/SiteManagement.Api/

RUN dotnet restore src/SiteManagement.Api/SiteManagement.Api.csproj

# ---------- 2) Build + Publish ------------------------------------------------
FROM restore AS publish
COPY src/ src/
RUN dotnet publish src/SiteManagement.Api/SiteManagement.Api.csproj \
        --configuration Release \
        --no-restore \
        --output /app/publish \
        /p:UseAppHost=false

# ---------- 3) Runtime --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# `curl` is needed for the docker-compose HEALTHCHECK; the aspnet image doesn't
# ship it. ~3 MB extra is acceptable for a dev/showcase image.
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Non-root user (asp.net uid is exposed via $APP_UID by the base image).
USER $APP_UID

ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true

EXPOSE 8080

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

ENTRYPOINT ["dotnet", "SiteManagement.Api.dll"]
