# Diagrama de Infraestructura - Event Ticketing Application

Este diagrama muestra la arquitectura de infraestructura en Azure para la aplicacion de venta de entradas para eventos.

## Arquitectura de Infraestructura en Azure

```mermaid
graph TB
    subgraph "Internet"
        Users[Usuarios]
        CDN[Azure CDN<br/>Content Delivery Network]
    end
    
    subgraph "Azure Subscription"
        subgraph "Resource Group: event-ticketing-rg"
            subgraph "Frontend Layer"
                Storage[Azure Storage Account<br/>Static Website<br/>React Application]
                CustomDomain[Custom Domain<br/>eventticketing.com]
            end
            
            subgraph "Application Layer"
                AppPlan[App Service Plan<br/>Linux B1<br/>.NET 7.0]
                WebApp[Azure Web App<br/>API Service<br/>event-ticketing-api]
            end
            
            subgraph "Data Layer"
                PostgreSQL[PostgreSQL Flexible Server<br/>Supabase Compatible<br/>Version 14]
                Database[(Database<br/>event_ticketing)]
            end
            
            subgraph "Security & Networking"
                Firewall[Firewall Rules<br/>Azure Services Access]
                SSL[SSL/TLS Certificates<br/>HTTPS Encryption]
            end
        end
    end
    
    subgraph "External Services"
        GitHub[GitHub<br/>Source Code & CI/CD]
        Terraform[Terraform<br/>Infrastructure as Code]
    end
    
    %% User Flow
    Users -->|HTTPS| CDN
    CDN -->|Static Content| Storage
    Users -->|HTTPS| CustomDomain
    CustomDomain -->|Redirect| Storage
    
    %% Frontend to Backend
    Storage -->|API Calls<br/>HTTPS| SSL
    SSL -->|Secure Connection| WebApp
    
    %% Backend to Database
    WebApp -->|SQL Queries<br/>Connection Pool| PostgreSQL
    PostgreSQL -->|Contains| Database
    
    %% Security
    Firewall -->|Protects| PostgreSQL
    SSL -->|Secures| WebApp
    
    %% Infrastructure Management
    GitHub -->|Triggers| Terraform
    Terraform -->|Provisions| AppPlan
    Terraform -->|Provisions| WebApp
    Terraform -->|Provisions| PostgreSQL
    Terraform -->|Provisions| Storage
    
    %% Dependencies
    WebApp -->|Runs on| AppPlan
    
    %% Styling
    classDef userClass fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef frontendClass fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef backendClass fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
    classDef dataClass fill:#fff3e0,stroke:#e65100,stroke-width:2px
    classDef securityClass fill:#ffebee,stroke:#b71c1c,stroke-width:2px
    classDef infraClass fill:#f1f8e9,stroke:#33691e,stroke-width:2px
    
    class Users,CDN userClass
    class Storage,CustomDomain frontendClass
    class AppPlan,WebApp backendClass
    class PostgreSQL,Database dataClass
    class Firewall,SSL securityClass
    class GitHub,Terraform infraClass
```

## Diagrama de Despliegue Detallado

```mermaid
flowchart TD
    subgraph "Azure Cloud Platform"
        subgraph "Resource Group: event-ticketing-rg"
            subgraph "Frontend Tier"
                SA[Storage Account<br/>Static Website<br/>React SPA]
            end
            
            subgraph "Application Tier"
                ASP[App Service Plan<br/>Linux B1]
                API[Web App<br/>.NET 7 API<br/>Always On]
                ASP --> API
            end
            
            subgraph "Data Tier"
                PG[PostgreSQL Server<br/>Flexible Server<br/>Version 14]
                DB[(Database<br/>event_ticketing<br/>32GB Storage)]
                PG --> DB
            end
        end
    end
    
    subgraph "Development & Operations"
        DEV[Developer Machine<br/>Terraform CLI]
        GHA[GitHub Actions<br/>CI/CD Pipeline]
    end
    
    subgraph "End Users"
        USERS[Web Users<br/>Mobile Users]
    end
    
    USERS -->|HTTPS Requests| SA
    SA -->|API Calls| API
    API -->|SQL Queries| PG
    DEV -->|terraform apply| "Resource Group: event-ticketing-rg"
    GHA -->|Deploy Code| API
    GHA -->|Upload Assets| SA
```

## Flujo de Datos y Comunicacion

```mermaid
sequenceDiagram
    participant U as Usuario
    participant SA as Storage Account
    participant WA as Web App API
    participant PG as PostgreSQL
    participant GH as GitHub Actions
    participant TF as Terraform
    
    Note over U,PG: Flujo de Usuario Normal
    U->>SA: GET / (Solicita aplicacion web)
    SA-->>U: index.html + assets (React App)
    
    U->>WA: POST /api/auth/login
    WA->>PG: SELECT user WHERE email = ?
    PG-->>WA: User data
    WA-->>U: JWT Token
    
    U->>WA: GET /api/events
    WA->>PG: SELECT events WHERE active = true
    PG-->>WA: Events list
    WA-->>U: JSON Response
    
    Note over GH,PG: Flujo de Despliegue
    GH->>TF: terraform plan & apply
    TF->>PG: Create PostgreSQL Server
    TF->>WA: Create Web App
    TF->>SA: Create Storage Account
    
    GH->>SA: Upload React build files
    GH->>WA: Deploy .NET API
    GH->>PG: Run database migrations
```

## Configuracion de Recursos Azure

### Storage Account (Frontend)
- **Nombre**: eventticketing{random}
- **Tipo**: StorageV2 con Static Website
- **Replicacion**: LRS (Locally Redundant Storage)
- **Configuracion**:
  - Index document: index.html
  - Error 404 document: index.html
- **CORS**: Configurado para API calls

### App Service (Backend API)
- **Nombre**: event-ticketing-api-{random}
- **Plan**: Linux B1 (Basic, 1 Core, 1.75GB RAM)
- **Runtime**: .NET 7.0
- **Configuracion**:
  - Always On: Habilitado
  - ASPNETCORE_ENVIRONMENT: Production
  - Connection String: PostgreSQL
  - CORS_ORIGINS: Storage Account URL

### PostgreSQL Flexible Server
- **Nombre**: supabase-pg-{random}
- **Version**: PostgreSQL 14
- **SKU**: B_Standard_B1ms (Burstable)
- **Storage**: 32 GB
- **Configuracion**:
  - Backup retention: 7 dias
  - Charset: UTF8
  - Collation: en_US.utf8

## Consideraciones de Seguridad

### 1. Comunicacion Segura
- **HTTPS Only**: Todas las comunicaciones externas
- **TLS 1.2+**: Conexiones a base de datos encriptadas
- **CORS Policy**: Configurado especificamente para el dominio frontend

### 2. Acceso a Base de Datos
- **Firewall Rules**: Solo Azure Services permitidos (0.0.0.0)
- **Authentication**: Usuario/contrasena administrados
- **Network Security**: Sin acceso publico directo

### 3. Gestion de Secretos
- **App Settings**: Variables sensibles en configuracion segura
- **Connection Strings**: Almacenadas en App Service Configuration
- **Terraform State**: Backend remoto recomendado

## Escalabilidad y Rendimiento

### Escalabilidad Automatica
- **App Service**: Scale up/out disponible
- **PostgreSQL**: Escalado vertical de compute y storage
- **Storage Account**: Escalado automatico ilimitado

### Optimizaciones de Rendimiento
- **Static Content**: Servido directamente desde Storage
- **API Caching**: Headers de cache configurables
- **Database**: Connection pooling en .NET
- **Compression**: Gzip habilitado automaticamente

## Estimacion de Costos (USD/mes)

| Recurso | SKU/Tier | Uso Estimado | Costo Mensual |
|---------|----------|--------------|---------------|
| App Service Plan | B1 Linux | 24/7 | ~$13.14 |
| PostgreSQL Flexible | B1ms | 24/7 | ~$12.41 |
| Storage Account | Standard LRS | 1GB + transactions | ~$2.00 |
| Bandwidth | Outbound | 10GB | ~$0.87 |
| **Total Estimado** | | | **~$28.42** |

*Precios basados en region West US, pueden variar*

## Comandos de Gestion

### Terraform - Infraestructura
```bash
# Inicializar Terraform
cd terraform
terraform init

# Planificar cambios
terraform plan -var="subscription_id=YOUR_SUB_ID"

# Aplicar infraestructura
terraform apply -auto-approve

# Destruir recursos (cuidado!)
terraform destroy
```

### Azure CLI - Gestion Manual
```bash
# Login a Azure
az login

# Listar recursos del grupo
az resource list --resource-group event-ticketing-rg-XXX

# Ver logs de Web App
az webapp log tail --name event-ticketing-api-XXX --resource-group event-ticketing-rg-XXX

# Escalar App Service
az appservice plan update --name event-ticketing-plan-XXX --resource-group event-ticketing-rg-XXX --sku B2
```

---

*Diagrama de infraestructura generado automaticamente*
*Basado en configuracion Terraform: /terraform/main.tf*
