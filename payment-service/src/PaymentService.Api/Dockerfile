# syntax=docker/dockerfile:1.7
# Multi-stage build for PaymentService.Api
# Build context expected to be the repository root (handled by docker-compose),
# because the central package manifest + NuGet config live there and are shared.

# ---------- 1) Restore --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

# Shared NuGet config + central package manifest first (cache-friendly).
COPY NuGet.config Directory.Packages.props ./

# Copy only csproj files to maximize layer caching for restore.
COPY payment-service/src/PaymentService.Domain/PaymentService.Domain.csproj                 payment-service/src/PaymentService.Domain/
COPY payment-service/src/PaymentService.Application/PaymentService.Application.csproj         payment-service/src/PaymentService.Application/
COPY payment-service/src/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj payment-service/src/PaymentService.Infrastructure/
COPY payment-service/src/PaymentService.Api/PaymentService.Api.csproj                         payment-service/src/PaymentService.Api/

RUN dotnet restore payment-service/src/PaymentService.Api/PaymentService.Api.csproj

# ---------- 2) Build + Publish ------------------------------------------------
FROM restore AS publish
COPY payment-service/src/ payment-service/src/
RUN dotnet publish payment-service/src/PaymentService.Api/PaymentService.Api.csproj \
        --configuration Release \
        --no-restore \
        --output /app/publish \
        /p:UseAppHost=false

# ---------- 3) Runtime --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# curl for the docker-compose HEALTHCHECK (aspnet image doesn't ship it).
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID

ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_HTTP_PORTS=8090 \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8090

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

ENTRYPOINT ["dotnet", "PaymentService.Api.dll"]
