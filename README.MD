# Devsu App - Aplicación Bancaria

Esta es una aplicación bancaria completa desarrollada con .NET 8 (Backend) y React + TypeScript (Frontend) que permite gestionar usuarios, cuentas y transacciones bancarias.

## 🏗️ Arquitectura del Proyecto

### Backend (.NET 8)
- **Arquitectura**: Clean Architecture con separación de capas
- **Base de Datos**: PostgreSQL
- **ORM**: Entity Framework Core
- **Validación**: FluentValidation
- **Documentación**: Swagger/OpenAPI
- **Generación de PDFs**: IronPDF

### Frontend (React + TypeScript)
- **Framework**: React 19 con TypeScript
- **Build Tool**: Vite
- **Routing**: React Router DOM
- **Formularios**: Formik + Yup
- **HTTP Client**: Axios
- **Notificaciones**: React Toastify

## 📋 Prerrequisitos

Antes de comenzar, asegúrate de tener instalado:

### Para el Backend:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Para el Frontend:
- [Node.js](https://nodejs.org/) (versión 18 o superior)
- [npm](https://www.npmjs.com/) o [yarn](https://yarnpkg.com/)

## 🚀 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd Devsu-app
```

### 2. Configurar el Backend

#### Opción A: Usando Docker (Recomendado)

1. **Navegar al directorio del backend:**
```bash
cd backend
```

2. **Ejecutar con Docker Compose:**
```bash
docker-compose up --build
```

Esto iniciará:
- La API en `http://localhost:5001`
- PostgreSQL en `localhost:5432`
- Base de datos: `Devsu`
- Usuario: `postgres`
- Contraseña: `postgres`

#### Opción B: Ejecución Local

1. **Navegar al directorio del backend:**
```bash
cd backend/src
```

2. **Restaurar dependencias:**
```bash
dotnet restore
```

3. **Aplicar migraciones de base de datos:**
```bash
cd Devsu.API
dotnet ef database update
```

4. **Ejecutar la aplicación:**
```bash
dotnet run
```

### 3. Configurar el Frontend

1. **Navegar al directorio del frontend:**
```bash
cd front
```

2. **Instalar dependencias:**
```bash
npm install
```

3. **Ejecutar en modo desarrollo:**
```bash
npm run dev
```

El frontend estará disponible en `http://localhost:5173`

## 📚 Documentación de la API

Una vez que el backend esté ejecutándose, puedes acceder a la documentación de la API en:

- **Swagger UI**: `http://localhost:5001/swagger`
- **Colección de Postman**: `backend/postman/DEVSU APP API.postman_collection.json`

## 🗄️ Base de Datos

### Configuración por defecto:
- **Host**: localhost
- **Puerto**: 5432
- **Base de datos**: Devsu
- **Usuario**: postgres
- **Contraseña**: postgres

### Scripts de inicialización:
Los scripts SQL se encuentran en `backend/scripts/tables.sql`

## 🔧 Configuración de Variables de Entorno

### Backend
Las variables de entorno están configuradas en el `docker-compose.yml`:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=Devsu;Username=postgres;Password=postgres
  - LICENSE_KEY=IRONSUITE.ORBIS.ALONZO.UNICARIBE.EDU.DO.8326-C5C3E2FB5B-G6BW27T56NMHDS-VBLXRNHM4QF5-2T323ZHMLHIQ-UN4IVXVSIAEZ-KE2UEVSSIBIF-OEYDE2J3LDQV-RA6PKB-TGYJBEY6MDGQEA-DEPLOYMENT.TRIAL-LBWCQX.TRIAL.EXPIRES.22.AUG.2025

```

LICENSE_KEY : importante esta key va a caducar en los proximos 20 dias

### Frontend
El frontend utiliza Vite para la configuración de entornos. Los archivos de configuración están en:
- `vite.config.ts`
- `src/config/axios.config.ts`

## 🧪 Ejecutar Tests

### Backend Tests
```bash
cd backend/src
dotnet test
```

## 📁 Estructura del Proyecto

```
Devsu-app/
├── backend/                 # Backend .NET 8
│   ├── src/
│   │   ├── Devsu.API/      # Capa de presentación
│   │   ├── Devsu.Application/ # Capa de aplicación
│   │   ├── Devsu.Domain/   # Capa de dominio
│   │   └── Devsu.Infrastructure/ # Capa de infraestructura
│   ├── docker-compose.yml
│   └── postman/
├── front/                   # Frontend React + TypeScript
│   ├── src/
│   │   ├── components/     # Componentes reutilizables
│   │   ├── modules/        # Módulos de la aplicación
│   │   ├── pages/          # Páginas principales
│   │   └── shared/         # Utilidades compartidas
│   └── package.json
└── README.md
```

## 🎯 Funcionalidades Principales

### Gestión de Usuarios
- Crear, editar y eliminar usuarios
- Validación de datos
- Paginación de resultados

### Gestión de Cuentas
- Crear cuentas bancarias
- Diferentes tipos de cuenta
- Asociación con usuarios

### Gestión de Transacciones
- Realizar transferencias
- Consultar historial de transacciones
- Exportar reportes en PDF
- Límites diarios de transacción

## 🐛 Solución de Problemas

### Problemas Comunes

1. **Puerto 5001 ocupado:**
   - Cambiar el puerto en `docker-compose.yml`
   - O detener servicios que usen ese puerto

2. **Error de conexión a la base de datos:**
   - Verificar que Docker esté ejecutándose
   - Verificar que PostgreSQL esté iniciado
   - Revisar las credenciales en `docker-compose.yml`

3. **Error de dependencias del frontend:**
   - Eliminar `node_modules` y `package-lock.json`
   - Ejecutar `npm install` nuevamente

4. **Error de migraciones:**
   - Verificar que la base de datos esté creada
   - Ejecutar `dotnet ef database update` desde el directorio `Devsu.API`


**¡Listo!** Ahora puedes ejecutar la aplicación Devsu completa. El backend estará disponible en `http://localhost:5001` y el frontend en `http://localhost:5173`.
