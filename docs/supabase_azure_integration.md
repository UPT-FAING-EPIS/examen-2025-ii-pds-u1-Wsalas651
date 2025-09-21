# Integración de Supabase con Azure

Este documento explica cómo integrar Supabase con la infraestructura de Azure desplegada mediante Terraform.

## Arquitectura

La integración de Supabase con Azure utiliza los siguientes componentes:

1. **Azure PostgreSQL Flexible Server**: Base de datos compatible con Supabase
2. **Azure App Service**: Para alojar la API que se conecta a Supabase
3. **Azure Storage**: Para alojar el frontend que consume la API y se conecta directamente a Supabase
4. **Azure CDN**: Para mejorar el rendimiento del frontend

## Configuración de PostgreSQL para Supabase

Después de desplegar la infraestructura con Terraform, debes configurar el servidor PostgreSQL para que sea compatible con Supabase:

1. Conéctate al servidor PostgreSQL usando la cadena de conexión proporcionada por Terraform
2. Ejecuta el script SQL de configuración de Supabase ubicado en `../database/supabase_setup.sql`

```bash
# Ejemplo de conexión usando psql
psql "Host=supabase-pg-123.postgres.database.azure.com;Database=event_ticketing;Username=postgres;Password=YourPassword"

# O puedes usar la herramienta Azure Data Studio con la cadena de conexión
```

## Configuración de variables de entorno

### Variables para el backend (API)

Estas variables se configuran automáticamente en Azure App Service durante el despliegue con Terraform:

- `SUPABASE_CONNECTION_STRING`: Cadena de conexión a la base de datos PostgreSQL
- `CORS_ORIGINS`: Orígenes permitidos para CORS (frontend)

### Variables para el frontend

Estas variables deben configurarse como secretos en GitHub para el flujo de trabajo de despliegue:

- `REACT_APP_SUPABASE_URL`: URL de tu proyecto Supabase (normalmente `https://your-project.supabase.co`)
- `REACT_APP_SUPABASE_ANON_KEY`: Clave anónima de tu proyecto Supabase
- `REACT_APP_API_URL`: URL de la API desplegada en Azure App Service

## Configuración de Supabase

### Opción 1: Usar Supabase Cloud con Azure PostgreSQL

1. Crea una cuenta en [Supabase](https://supabase.com/)
2. Crea un nuevo proyecto
3. En la configuración del proyecto, selecciona "Database" y luego "Connection Pooling"
4. Configura la conexión a tu base de datos PostgreSQL en Azure
5. Obtén la URL y la clave anónima de Supabase para configurar el frontend

### Opción 2: Configurar Supabase Self-hosted en Azure

1. Clona el repositorio de Supabase: `git clone https://github.com/supabase/supabase`
2. Modifica el archivo `docker-compose.yml` para usar tu base de datos PostgreSQL en Azure
3. Despliega los servicios de Supabase en Azure Container Instances o Azure Kubernetes Service

## Integración con la aplicación

### Backend (API)

La API se conecta directamente a la base de datos PostgreSQL en Azure usando la cadena de conexión configurada en las variables de entorno.

```csharp
// Ejemplo de conexión en C#
var connectionString = Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING");
var connection = new NpgsqlConnection(connectionString);
```

### Frontend

El frontend se conecta a Supabase usando la URL y la clave anónima configuradas en las variables de entorno.

```javascript
// Ejemplo de conexión en JavaScript
import { createClient } from '@supabase/supabase-js'

const supabaseUrl = process.env.REACT_APP_SUPABASE_URL
const supabaseAnonKey = process.env.REACT_APP_SUPABASE_ANON_KEY

export const supabase = createClient(supabaseUrl, supabaseAnonKey)
```

## Seguridad

### Reglas de firewall

El despliegue de Terraform configura una regla de firewall para permitir el acceso desde servicios de Azure al servidor PostgreSQL. Para entornos de producción, se recomienda restringir el acceso solo a las direcciones IP necesarias.

### Autenticación

Supabase proporciona autenticación integrada que puedes utilizar en tu aplicación. Configura los proveedores de autenticación en el panel de control de Supabase.

## Monitoreo

Utiliza Azure Monitor para supervisar el rendimiento y la disponibilidad de tu infraestructura:

1. Configura alertas para el servidor PostgreSQL
2. Configura métricas para el App Service
3. Configura logs para el Storage Account y CDN

## Resolución de problemas

### Problemas de conexión a la base de datos

- Verifica que la regla de firewall permita el acceso desde tu aplicación
- Verifica que la cadena de conexión sea correcta
- Verifica que el usuario tenga los permisos necesarios

### Problemas de CORS

- Verifica que la variable `CORS_ORIGINS` esté configurada correctamente en el App Service
- Verifica que las solicitudes desde el frontend incluyan las cabeceras CORS necesarias

## Recursos adicionales

- [Documentación de Supabase](https://supabase.com/docs)
- [Documentación de Azure PostgreSQL](https://docs.microsoft.com/es-es/azure/postgresql/)
- [Documentación de Azure App Service](https://docs.microsoft.com/es-es/azure/app-service/)