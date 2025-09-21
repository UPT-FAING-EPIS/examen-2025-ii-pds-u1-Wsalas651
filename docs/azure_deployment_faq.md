# Preguntas Frecuentes sobre Despliegue en Azure

## Generales

### ¿Por qué usar Azure para el despliegue?

Azure ofrece una plataforma completa para aplicaciones web con servicios integrados que facilitan el despliegue, escalado y gestión. Para esta aplicación de venta de entradas, Azure proporciona:

- **Escalabilidad**: Capacidad para manejar picos de tráfico durante la venta de entradas populares
- **Seguridad**: Protección de datos sensibles de usuarios y transacciones
- **Integración con CI/CD**: Despliegue automatizado mediante GitHub Actions
- **Servicios complementarios**: CDN para mejorar el rendimiento, Storage para archivos estáticos, etc.

### ¿Cuánto costará el despliegue en Azure?

El costo depende de varios factores:

- **App Service**: Plan B1 (~$13/mes) para entornos de desarrollo/prueba
- **Storage Account**: Costos mínimos para sitios web estáticos (~$0.10/GB/mes)
- **CDN**: Costos basados en transferencia de datos (~$0.08/GB)

Para una aplicación de producción con tráfico moderado, estima entre $30-100/mes. Considera usar [Azure Pricing Calculator](https://azure.microsoft.com/en-us/pricing/calculator/) para estimaciones precisas.

## Configuración

### ¿Necesito instalar algo localmente para desplegar en Azure?

No es estrictamente necesario, ya que el despliegue se realiza mediante GitHub Actions. Sin embargo, para la configuración inicial o solución de problemas, es útil tener:

- **Azure CLI**: Para gestionar recursos de Azure desde la línea de comandos
- **PowerShell**: Para ejecutar el script de configuración automatizada
- **Visual Studio Code con extensión de Azure**: Para una gestión visual de recursos

### ¿Cómo puedo verificar que mi despliegue funciona correctamente?

1. Verifica que el flujo de trabajo de GitHub Actions se complete sin errores
2. Accede a la URL de la API (https://{AZURE_WEBAPP_NAME}.azurewebsites.net)
3. Accede a la URL del frontend (https://{AZURE_CDN_ENDPOINT_NAME}.azureedge.net)
4. Prueba las funcionalidades principales de la aplicación
5. Revisa los logs en Azure Portal > App Service > Logs

## Solución de Problemas

### El despliegue falla con error de autenticación

Verifica que:

1. El secreto `AZURE_CREDENTIALS` está correctamente configurado en GitHub
2. El Service Principal tiene permisos suficientes en el grupo de recursos
3. Las credenciales no han expirado (los Service Principals pueden tener fecha de caducidad)

Solución: Genera nuevas credenciales con `az ad sp create-for-rbac` y actualiza el secreto en GitHub.

### El frontend no puede comunicarse con la API

Posibles causas y soluciones:

1. **CORS mal configurado**: Verifica que la API tenga configurado CORS para permitir solicitudes desde el dominio del frontend
2. **URL incorrecta**: Asegúrate de que `REACT_APP_API_URL` apunta a la URL correcta de la API
3. **Problemas de red**: Verifica que no haya restricciones de firewall bloqueando las conexiones

### Los archivos estáticos no se actualizan después del despliegue

Esto puede ocurrir debido al caché del CDN. Soluciones:

1. Purga el caché del CDN manualmente desde Azure Portal
2. Asegúrate de que el flujo de trabajo incluye un paso para purgar el caché del CDN
3. Implementa versionado de archivos en tu aplicación frontend

## Optimización

### ¿Cómo puedo reducir los costos de Azure?

1. **Escala automática**: Configura reglas para escalar hacia abajo durante períodos de baja demanda
2. **Reservas**: Para entornos de producción estables, considera las reservas de Azure que ofrecen descuentos
3. **Nivel gratuito**: Para desarrollo/pruebas, usa componentes del nivel gratuito cuando sea posible
4. **Optimización de recursos**: Monitorea el uso y ajusta los recursos según las necesidades reales

### ¿Cómo puedo mejorar el rendimiento de la aplicación en Azure?

1. **CDN**: Asegúrate de que todos los recursos estáticos se sirven a través del CDN
2. **Caché**: Implementa estrategias de caché tanto en el frontend como en la API
3. **Compresión**: Habilita la compresión GZIP/Brotli en App Service
4. **Application Insights**: Configura Application Insights para identificar cuellos de botella
5. **Bases de datos**: Optimiza consultas y considera usar servicios gestionados como Azure SQL

## Seguridad

### ¿Cómo protejo los secretos en mi aplicación desplegada?

1. **Key Vault**: Considera migrar secretos sensibles a Azure Key Vault
2. **Identidades Administradas**: Usa identidades administradas para acceder a recursos de Azure sin credenciales
3. **Rotación de claves**: Implementa un proceso para rotar regularmente claves y secretos
4. **HTTPS**: Asegúrate de que toda la comunicación usa HTTPS

### ¿Cómo configuro un dominio personalizado con HTTPS?

1. Adquiere un dominio y configura los registros DNS para apuntar a tu aplicación en Azure
2. En Azure Portal > App Service > Dominios personalizados, añade tu dominio
3. Configura un certificado SSL:
   - Genera un certificado gratuito gestionado por Azure
   - O importa tu propio certificado
4. Configura el binding de SSL para tu dominio
5. Para el frontend en CDN, configura el dominio personalizado en Azure CDN y habilita HTTPS

## Avanzado

### ¿Cómo implemento despliegues sin tiempo de inactividad?

1. **Deployment Slots**: Configura slots de despliegue en App Service
2. **Swap**: Despliega a un slot de staging y luego realiza un swap con producción
3. **Testing**: Implementa pruebas automáticas en el slot de staging antes del swap

### ¿Cómo configuro múltiples entornos (desarrollo, pruebas, producción)?

1. **Grupos de recursos separados**: Crea un grupo de recursos para cada entorno
2. **Variables de entorno**: Configura variables específicas para cada entorno
3. **GitHub Environments**: Utiliza environments en GitHub Actions para gestionar despliegues a diferentes entornos
4. **Aprobaciones**: Configura aprobaciones requeridas para despliegues a producción