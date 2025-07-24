FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY . .

RUN dotnet restore -a $TARGETARCH && dotnet publish -a $TARGETARCH -c Release -o /app/publish \
    --no-restore \
    --self-contained false 

RUN apt update \
    && apt install -y sudo libxkbcommon-x11-0 libc6 libc6-dev libgtk2.0-0 libnss3 

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS runtime

RUN apt update \
    && apt install -y sudo libxkbcommon-x11-0 libc6 libc6-dev libgtk2.0-0 libnss3

RUN mkdir -p /home/appuser/.ironsoftware \
    && chmod 777 /home/appuser/.ironsoftware

RUN groupadd -g 1001 appgroup && \
    useradd -m -u 1001 -g appgroup appuser

WORKDIR /app

COPY --from=build --chown=appuser:appgroup /app/publish .

# USER appuser

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Devsu.API.dll"]