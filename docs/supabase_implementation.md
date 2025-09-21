# Implementación de Supabase en el Sistema de Venta de Entradas

Este documento describe cómo implementar y configurar Supabase como base de datos para el sistema de venta de entradas, así como la configuración necesaria para el despliegue con GitHub Actions, con énfasis en las mejores prácticas de seguridad.

## Configuración de Supabase

### 1. Crear un proyecto en Supabase

1. Regístrate o inicia sesión en [Supabase](https://supabase.com/)
2. Crea un nuevo proyecto
3. Anota la URL del proyecto y la clave anónima (API Key)

### 2. Crear las tablas y políticas de seguridad

1. Ve a la sección SQL Editor en el panel de control de Supabase
2. Ejecuta el script SQL ubicado en `database/supabase_setup.sql`
3. Verifica que las tablas y políticas se hayan creado correctamente

## Configuración del Proyecto

### Backend (.NET)

1. Actualiza el archivo `appsettings.json` con la cadena de conexión de Supabase:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.your-project-id.supabase.co;Database=postgres;Username=postgres;Password=your-password;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Jwt": {
    "Key": "tu-clave-secreta-jwt",
    "Issuer": "EventTicketing",
    "Audience": "EventTicketing.Users"
  }
}
```

2. Asegúrate de tener instalado el paquete NuGet `Npgsql.EntityFrameworkCore.PostgreSQL`

### Frontend (React)

1. Crea un archivo `.env` en la carpeta `src/Frontend` basado en `.env.example`
2. Configura las variables de entorno:

```
REACT_APP_SUPABASE_URL=https://your-project-url.supabase.co
REACT_APP_SUPABASE_ANON_KEY=your-anon-key
REACT_APP_USE_SUPABASE=true
REACT_APP_API_URL=http://localhost:5000/api
```

## Configuración de GitHub Actions para Despliegue

### 1. Configurar Secretos en GitHub

1. Ve a la configuración de tu repositorio en GitHub > Settings > Secrets and variables > Actions
2. Añade los siguientes secretos:
   - `SUPABASE_CONNECTION_STRING`: La cadena de conexión completa a tu base de datos Supabase
   - `SUPABASE_URL`: URL de tu proyecto Supabase
   - `SUPABASE_ANON_KEY`: Clave anónima de tu proyecto Supabase
   - `USE_SUPABASE`: Establecer a 'true' para usar Supabase directamente desde el frontend
   - `JWT_SECRET_KEY`: Una clave secreta fuerte para la generación de tokens JWT
   - `API_URL`: URL donde se desplegará la API
   - `AZURE_WEBAPP_NAME`: Nombre de tu aplicación web en Azure (si usas Azure)
   - `AZURE_WEBAPP_PUBLISH_PROFILE`: Perfil de publicación de Azure (si usas Azure)

### 2. Flujo de Trabajo de GitHub Actions

El archivo de flujo de trabajo `.github/workflows/deploy.yml` ya está configurado para:

1. Compilar y probar la aplicación .NET
2. Compilar la aplicación React con las variables de entorno de Supabase
3. Actualizar el archivo `appsettings.json` con los secretos de GitHub
4. Desplegar la aplicación

## Uso de Supabase en el Frontend

El servicio `eventService.js` está configurado para usar Supabase directamente o la API backend según la variable de entorno `REACT_APP_USE_SUPABASE`.

### Ventajas de usar Supabase directamente desde el frontend:

1. Menor latencia al eliminar una capa intermedia
2. Aprovechamiento de las políticas de seguridad de Supabase (RLS)
3. Acceso a funcionalidades en tiempo real

### Ventajas de usar la API backend:

1. Mayor control sobre la lógica de negocio
2. Validaciones adicionales
3. Abstracción de la base de datos

## Migración de Datos

Si necesitas migrar datos de SQL Server a Supabase:

1. Exporta los datos de SQL Server en formato CSV
2. Usa la funcionalidad de importación de datos de Supabase para cargar los CSV
3. Verifica la integridad de los datos después de la migración

## Mejores Prácticas de Seguridad

### Manejo de Credenciales

1. **Nunca hardcodear credenciales**:
   - No incluir contraseñas, tokens o claves API directamente en el código fuente
   - Utilizar siempre variables de entorno o servicios de gestión de secretos

2. **Uso de variables de entorno**:
   - En desarrollo local: Utilizar archivos `.env` (asegurarse de incluirlos en `.gitignore`)
   - En producción: Utilizar variables de entorno del sistema o servicios de configuración

3. **Plantillas para archivos de configuración**:
   - Crear archivos de ejemplo como `appsettings.json.example` con placeholders para valores sensibles
   - Utilizar el formato `#{VARIABLE_NAME}#` para indicar dónde se deben insertar variables

4. **Rotación de credenciales**:
   - Cambiar regularmente las claves y contraseñas
   - Implementar un proceso para actualizar credenciales sin tiempo de inactividad

### Seguridad en GitHub Actions

1. **Uso de secretos de GitHub**:
   - Almacenar todas las credenciales como secretos en GitHub
   - Nunca exponer secretos en logs o salidas de acciones

2. **Sustitución de variables**:
   - Utilizar la acción `microsoft/variable-substitution` para reemplazar placeholders en archivos de configuración
   - Ejemplo: `ConnectionStrings.DefaultConnection: ${{ secrets.SUPABASE_CONNECTION_STRING }}`

3. **Alcance de los secretos**:
   - Limitar el acceso a secretos solo a los flujos de trabajo que los necesitan
   - Considerar el uso de secretos a nivel de entorno para mayor seguridad

### Seguridad en Supabase

1. **Políticas RLS restrictivas**:
   - Implementar políticas con el principio de mínimo privilegio
   - Probar exhaustivamente las políticas antes de desplegar a producción

2. **API Keys**:
   - Utilizar claves anónimas solo para operaciones de lectura pública
   - Implementar autenticación JWT para operaciones sensibles
   - Restringir el origen de las solicitudes API mediante CORS

## Solución de Problemas

### Problemas de conexión

- Verifica que la cadena de conexión sea correcta
- Asegúrate de que el firewall de Supabase permita conexiones desde tu IP

### Problemas con las políticas de seguridad

- Revisa las políticas RLS en el panel de Supabase
- Verifica que estás autenticado correctamente

### Problemas con el despliegue

- Verifica los secretos en GitHub
- Revisa los logs de GitHub Actions para identificar errores