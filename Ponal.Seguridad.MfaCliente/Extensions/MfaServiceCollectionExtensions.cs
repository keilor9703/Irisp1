using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ponal.Seguridad.MfaCliente.Interfaces;
using Ponal.Seguridad.MfaCliente.Servicios;
using Ponal.Seguridad.MfaCliente.Modelos; // Importante para que reconozca ApiGatewayUrls
using System;

namespace Ponal.Seguridad.MfaCliente.Extensions
{
    public static class MfaServiceCollectionExtensions
    {
        public static IServiceCollection AddPonalMfa(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            // ✅ ESTO ES LO QUE ARREGLA TU ERROR ACTUAL
            services.AddSingleton(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();

                var pipBase = cfg.GetValue<string>("ApiGatewayUrl") ?? "";
                var mfaBase = cfg.GetValue<string>("MfaCentral:BaseUrl")
                              ?? throw new InvalidOperationException("Falta la llave 'MfaCentral:BaseUrl' en el appsettings.json.");

                return new ApiGatewayUrls(pipBase, mfaBase);
            });

            services.AddScoped<IDbMfaCentralWs, DbMfaCentralWs>();

            services.AddHttpClient<IMfaWebServices, MfaWebServices>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestVersion = new Version(1, 1);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new System.Net.Http.SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                MaxConnectionsPerServer = 50,
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });

            return services;
        }
    }
}