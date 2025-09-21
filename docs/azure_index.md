# Documentación de Despliegue en Azure

Esta documentación proporciona una guía completa para desplegar la aplicación de venta de entradas en Azure utilizando GitHub Actions.

## Índice de Contenidos

### Guías Principales

1. [**Guía de Despliegue en Azure**](azure_deployment.md)
   - Instrucciones paso a paso para configurar y desplegar la aplicación
   - Opciones de configuración manual y automatizada
   - Verificación del despliegue

2. [**Variables de Entorno para Azure**](azure_environment_variables.md)
   - Lista completa de secretos y variables necesarios
   - Instrucciones para obtener valores de variables
   - Configuración en GitHub y Azure

3. [**Preguntas Frecuentes**](azure_deployment_faq.md)
   - Solución a problemas comunes
   - Optimizaciones recomendadas
   - Consideraciones de seguridad y rendimiento

### Recursos Adicionales

- [**Diagrama de Arquitectura**](azure_architecture.svg) - Visualización de la arquitectura de despliegue

### Scripts de Automatización

- [`setup-azure.ps1`](../scripts/setup-azure.ps1) - Script de PowerShell para configuración automatizada de recursos en Azure

## Flujo de Trabajo de Despliegue

El proceso de despliegue sigue estos pasos:

1. **Configuración de Recursos en Azure**
   - Creación de grupo de recursos, App Service, Storage Account y CDN
   - Configuración de Service Principal para GitHub Actions

2. **Configuración de Secretos en GitHub**
   - Adición de credenciales y variables de entorno como secretos

3. **Ejecución del Flujo de Trabajo de GitHub Actions**
   - Compilación y pruebas de la API
   - Compilación del frontend con variables de entorno
   - Despliegue de la API en Azure App Service
   - Despliegue del frontend en Azure Storage
   - Purga del caché de CDN (opcional)
   - Notificaciones de despliegue (opcional)

4. **Verificación del Despliegue**
   - Comprobación de la API y el frontend
   - Revisión de logs

## Próximos Pasos Recomendados

1. Configurar dominios personalizados y certificados SSL
2. Implementar monitoreo con Azure Application Insights
3. Configurar despliegues por entornos (desarrollo, pruebas, producción)
4. Optimizar costos y rendimiento