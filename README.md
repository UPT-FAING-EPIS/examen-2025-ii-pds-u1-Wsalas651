[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/A-aUFMBb)
[![Open in Codespaces](https://classroom.github.com/assets/launch-codespace-2972f46106e565e64193e422d61a12cf1da4916b45550586e14ef0a7c637dd04.svg)](https://classroom.github.com/open-in-codespaces?assignment_repo_id=20616774)

# Aplicación de Venta de Entradas a Eventos

## Descripción
Este proyecto implementa una plataforma web para la compra de entradas a eventos (conciertos, teatro, deportes, etc.) siguiendo principios de diseño SOLID, código limpio y dependency inversion.

## Estructura del Proyecto
El proyecto sigue una arquitectura limpia (Clean Architecture) con separación clara de responsabilidades:

- **Core**: Contiene las entidades de dominio, interfaces de repositorios y servicios.
- **Infrastructure**: Implementaciones concretas de repositorios, servicios externos y acceso a datos.
- **API**: Controladores REST y configuración de la API.
- **Frontend**: Aplicación cliente desarrollada con React.

## Principios de Diseño Aplicados
- **SOLID**: Cada componente tiene una única responsabilidad, las interfaces están segregadas adecuadamente, y las dependencias se invierten mediante inyección.
- **YAGNI**: Se implementan solo las funcionalidades necesarias sin añadir complejidad innecesaria.
- **DRY**: Se evita la duplicación de código mediante abstracciones adecuadas.
- **Dependency Inversion**: Las capas de alto nivel no dependen de implementaciones concretas sino de abstracciones.

## Tecnologías
- **Backend**: .NET Core 8
- **Frontend**: React con TypeScript
- **Base de datos**: SQL Server
- **Autenticación**: JWT

## Instrucciones de Ejecución
1. Clonar el repositorio
2. Configurar las variables de entorno:
   - **Opción recomendada**: Ejecutar el script `scripts/setup-env.ps1` que guiará en la configuración segura
   - **Opción manual**: Copiar `appsettings.json.example` a `appsettings.Development.json` y `.env.example` a `.env`, luego editar con tus credenciales
3. Ejecutar migraciones: `dotnet ef database update`
4. Iniciar el backend: `dotnet run`
5. Iniciar el frontend: `npm start`

## Seguridad y Mejores Prácticas
1. **Nunca** incluir credenciales directamente en el código fuente
2. **Siempre** usar variables de entorno o archivos de configuración excluidos del control de versiones
3. Los archivos con información sensible ya están incluidos en `.gitignore`
4. Para más detalles sobre seguridad, consultar `docs/supabase_implementation.md`

## Despliegue con GitHub Actions
Este proyecto está configurado para desplegarse automáticamente utilizando GitHub Actions. Para configurar el despliegue:

1. Ve a la configuración de tu repositorio en GitHub > Settings > Secrets and variables > Actions
2. Añade los siguientes secretos:
   - `SUPABASE_CONNECTION_STRING`: La cadena de conexión completa a tu base de datos Supabase
   - `SUPABASE_URL`: URL de tu proyecto Supabase (ej. https://your-project-url.supabase.co)

### Despliegue en Azure
Para desplegar esta aplicación en Azure utilizando GitHub Actions, consulta la guía detallada en [docs/azure_deployment.md](docs/azure_deployment.md) que incluye:

- Configuración de recursos necesarios en Azure
- Lista completa de secretos requeridos para GitHub Actions
- Instrucciones para obtener credenciales de Azure
- Solución de problemas comunes
- Optimizaciones recomendadas
   - `SUPABASE_ANON_KEY`: Clave anónima de tu proyecto Supabase
   - `USE_SUPABASE`: Establecer a 'true' para usar Supabase directamente desde el frontend, 'false' para usar la API
   - `JWT_SECRET_KEY`: Una clave secreta fuerte para la generación de tokens JWT
   - `API_URL`: URL donde se desplegará la API
   - `AZURE_WEBAPP_NAME`: Nombre de tu aplicación web en Azure (si usas Azure)
   - `AZURE_WEBAPP_PUBLISH_PROFILE`: Perfil de publicación de Azure (si usas Azure)

3. El despliegue se activará automáticamente con cada push a la rama main

## Estructura de Carpetas
```
/src
  /Core
    /Domain
    /Application
    /Interfaces
  /Infrastructure
    /Data
    /Services
    /Repositories
  /API
    /Controllers
    /Middleware
    /DTOs
  /Frontend
    /src
      /components
      /services
      /pages
/tests
  /UnitTests
  /IntegrationTests
```
