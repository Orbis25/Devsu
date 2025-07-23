# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos toda la solución y proyectos
COPY . .

# Restauramos paquetes
RUN dotnet restore Devsu.Api.sln

# Publicamos en modo Release, salida en /app/publish
RUN dotnet publish Devsu.Api.sln -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Instalamos dependencias necesarias para IronPDF y .NET
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus libc6-dev libx11-6 libxrender1 libfontconfig1 libfreetype6 libxext6 libxinerama1 fonts-dejavu-core curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copiamos los archivos publicados
COPY --from=build /app/publish .

# Comando para iniciar la aplicación (asegúrate de que el nombre coincida exactamente)
ENTRYPOINT ["dotnet", "Devsu.API.dll"]
