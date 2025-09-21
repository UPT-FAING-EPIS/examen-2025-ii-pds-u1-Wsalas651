# Infraestructura como Código (IaC) con Terraform para Supabase en Azure

Este directorio contiene los archivos de Terraform necesarios para desplegar la infraestructura requerida para la aplicación de venta de entradas utilizando Supabase en Azure.

## Recursos creados

- **Grupo de recursos**: Contenedor lógico para todos los recursos de Azure
- **PostgreSQL Flexible Server**: Servidor de base de datos compatible con Supabase
- **Base de datos PostgreSQL**: Base de datos para almacenar los datos de la aplicación
- **Plan de App Service**: Plan para alojar la API
- **Web App**: Aplicación web para la API
- **Cuenta de almacenamiento**: Para alojar el frontend estático
- **Perfil CDN**: Para mejorar el rendimiento del frontend
- **Endpoint CDN**: Punto de acceso para el contenido del CDN

## Requisitos previos

1. [Terraform](https://www.terraform.io/downloads.html) instalado localmente
2. [Azure CLI](https://docs.microsoft.com/es-es/cli/azure/install-azure-cli) instalado y configurado
3. Una suscripción de Azure activa

## Configuración local

1. Inicia sesión en Azure:

```bash
az login
```

2. Crea un archivo `terraform.tfvars` basado en el ejemplo:

```bash
cp terraform.tfvars.example terraform.tfvars
```

3. Edita el archivo `terraform.tfvars` con tus valores específicos:

```
# El subscription_id se configura como variable de entorno TF_VAR_subscription_id
postgres_admin_username = "postgres"
postgres_admin_password = "TuContraseñaSegura123!"
```

4. Alternativamente, puedes configurar el ID de suscripción como una variable de entorno:

```bash
# En Linux/macOS
export TF_VAR_subscription_id="tu-id-de-suscripcion-azure"

# En Windows PowerShell
$env:TF_VAR_subscription_id="tu-id-de-suscripcion-azure"
```

## Despliegue manual

1. Inicializa Terraform:

```bash
terraform init
```

2. Verifica el plan de ejecución:

```bash
terraform plan
```

3. Aplica la configuración:

```bash
terraform apply
```

4. Confirma la creación de recursos escribiendo `yes` cuando se te solicite.

## Despliegue automatizado con GitHub Actions

Este repositorio incluye un flujo de trabajo de GitHub Actions (`infra.yml`) que automatiza el despliegue de la infraestructura.

### Configuración de secretos en GitHub

Para que el flujo de trabajo funcione correctamente, debes configurar los siguientes secretos en tu repositorio de GitHub:

1. `AZURE_CLIENT_ID`: ID de cliente del Service Principal
2. `AZURE_CLIENT_SECRET`: Secreto del Service Principal
3. `AZURE_SUBSCRIPTION_ID`: ID de tu suscripción de Azure (usado tanto para autenticación como para la variable subscription_id)
4. `AZURE_TENANT_ID`: ID del tenant de Azure
5. `POSTGRES_ADMIN_USERNAME`: Nombre de usuario administrador para PostgreSQL
6. `POSTGRES_ADMIN_PASSWORD`: Contraseña del administrador para PostgreSQL
7. `GH_PA_TOKEN`: Token de acceso personal de GitHub con permisos para actualizar secretos

### Creación del Service Principal

Para crear un Service Principal con los permisos necesarios, ejecuta:

```bash
az ad sp create-for-rbac --name "GitHubActionsTerraform" --role contributor --scopes /subscriptions/tu-id-de-suscripcion --sdk-auth
```

Utiliza la salida JSON para configurar los secretos `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`, `AZURE_SUBSCRIPTION_ID` y `AZURE_TENANT_ID`.

## Variables de salida

Después del despliegue, Terraform generará las siguientes salidas:

- `resource_group_name`: Nombre del grupo de recursos creado
- `api_url`: URL de la API desplegada
- `frontend_url`: URL del frontend estático
- `cdn_url`: URL del endpoint CDN
- `postgres_server_name`: Nombre del servidor PostgreSQL
- `postgres_connection_string`: Cadena de conexión para PostgreSQL (sensible)

## Integración con Supabase

Consulta el documento [supabase_azure_integration.md](../docs/supabase_azure_integration.md) para obtener instrucciones detalladas sobre cómo integrar Supabase con la infraestructura de Azure desplegada.

## Limpieza de recursos

Para eliminar todos los recursos creados:

```bash
terraform destroy
```

Confirma la eliminación escribiendo `yes` cuando se te solicite.