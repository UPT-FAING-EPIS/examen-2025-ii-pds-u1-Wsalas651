using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace EventTicketing.Infrastructure.Data
{
    /// <summary>
    /// Ejemplo de cómo configurar la conexión a Supabase utilizando variables de entorno
    /// </summary>
    public static class SupabaseConnectionExample
    {
        public static IServiceCollection AddSupabaseConnection(this IServiceCollection services, IConfiguration configuration)
        {
            // Obtener la cadena de conexión desde la configuración (que puede venir de variables de entorno)
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            // Validar que la cadena de conexión existe
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión a Supabase no está configurada.");
            }

            // Construir la cadena de conexión de Npgsql a partir de la cadena de Supabase
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            
            // Configurar opciones adicionales si es necesario
            builder.Pooling = true;
            builder.MinPoolSize = 1;
            builder.MaxPoolSize = 20;
            builder.SslMode = SslMode.Require; // Recomendado para producción
            
            // Registrar el DbContext con la cadena de conexión
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.ConnectionString));
            
            return services;
        }
    }
}