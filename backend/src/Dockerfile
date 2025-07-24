# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

# Build simple y estable (sin trimming que causa seg fault)
RUN dotnet restore && dotnet publish -c Release -o /app/publish \
    --no-restore \
    --self-contained false \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS runtime

# Dependencias para IronPDF Linux
RUN apt-get update && apt-get install -y --no-install-recommends \
    wget \
    libgdiplus \
    libfontconfig1 \
    fonts-dejavu-core \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN groupadd -r appgroup && useradd -r -g appgroup appuser

WORKDIR /app
COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

# HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=2 \
#     CMD wget -q --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Devsu.API.dll"]