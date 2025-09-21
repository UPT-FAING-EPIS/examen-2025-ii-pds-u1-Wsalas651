# Variables de Entorno para Despliegue en Azure

Este documento detalla las variables de entorno necesarias para el despliegue en Azure utilizando GitHub Actions.

## Variables de Entorno para GitHub Actions

Estas variables deben configurarse como secretos en GitHub (Settings > Secrets and variables > Actions):

| Nombre del Secreto | Descripción | Ejemplo |
|-------------------|-------------|--------|
| `AZURE_CREDENTIALS` | Credenciales de servicio principal de Azure en formato JSON | `{"clientId": "...", "clientSecret": "...", "subscriptionId": "...", "tenantId": "..."}` |
| `AZURE_WEBAPP_NAME` | Nombre de tu Azure Web App | `event-ticketing-api` |
| `AZURE_STORAGE_ACCOUNT` | Nombre de la cuenta de almacenamiento | `eventticketingstore` |
| `AZURE_STORAGE_KEY` | Clave de acceso de la cuenta de almacenamiento | `tu-clave-de-acceso` |
| `AZURE_CDN_PROFILE_NAME` | Nombre del perfil de CDN | `EventTicketingCDN` |
| `AZURE_CDN_ENDPOINT_NAME` | Nombre del endpoint de CDN | `event-ticketing-cdn` |
| `AZURE_RESOURCE_GROUP` | Nombre del grupo de recursos | `EventTicketingRG` |

## Variables de Entorno para la Aplicación

Estas variables son necesarias para el funcionamiento de la aplicación y deben configurarse como secretos en GitHub:

| Nombre del Secreto | Descripción | Ejemplo |
|-------------------|-------------|--------|
| `SUPABASE_CONNECTION_STRING` | Cadena de conexión a la base de datos | `Host=db.example.supabase.co;Database=postgres;Username=postgres;Password=your-password` |
| `JWT_SECRET_KEY` | Clave secreta para firmar tokens JWT | `tu-clave-secreta-muy-segura` |
| `REACT_APP_SUPABASE_URL` | URL de tu proyecto Supabase | `https://your-project.supabase.co` |
| `REACT_APP_SUPABASE_ANON_KEY` | Clave anónima de tu proyecto Supabase | `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...` |
| `REACT_APP_API_URL` | URL de tu API desplegada | `https://event-ticketing-api.azurewebsites.net` |

## Variables de Entorno para Notificaciones (Opcionales)

| Nombre del Secreto | Descripción | Ejemplo |
|-------------------|-------------|--------|
| `SLACK_WEBHOOK_URL` | URL del webhook de Slack para notificaciones | `https://hooks.slack.com/services/T00000000/B00000000/XXXXXXXXXXXXXXXXXXXXXXXX` |
| `TEAMS_WEBHOOK_URL` | URL del webhook de Microsoft Teams para notificaciones | `https://outlook.office.com/webhook/...` |
| `NOTIFICATION_EMAIL` | Correo electrónico para notificaciones | `admin@example.com` |

## Configuración en Azure App Service

Algunas variables de entorno deben configurarse directamente en Azure App Service para el entorno de producción:

1. Ve al portal de Azure > App Service > Configuration > Application settings
2. Añade las siguientes variables:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `SUPABASE_CONNECTION_STRING`: La cadena de conexión a tu base de datos
   - `JWT_SECRET_KEY`: Tu clave secreta para JWT
   - `CORS_ORIGINS`: Los orígenes permitidos para CORS, separados por comas (ej. `https://event-ticketing-cdn.azureedge.net,https://eventticketingstore.z13.web.core.windows.net`)

## Obtención de Valores

### Clave de la Cuenta de Almacenamiento

Para obtener `AZURE_STORAGE_KEY`:

```bash
az storage account keys list --resource-group EventTicketingRG --account-name eventticketingstore --query "[0].value" -o tsv
```

### Credenciales de Service Principal

Para obtener `AZURE_CREDENTIALS`, ejecuta:

```bash
az ad sp create-for-rbac --name "EventTicketingGitHubActions" --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} --sdk-auth
```

O utiliza el script `setup-azure.ps1` que generará estas credenciales automáticamente.

## Verificación de Variables

El flujo de trabajo de GitHub Actions incluye un paso para verificar que todas las variables necesarias estén configuradas antes de iniciar el despliegue. Si falta alguna variable, el flujo de trabajo fallará con un mensaje de error indicando qué variables faltan.