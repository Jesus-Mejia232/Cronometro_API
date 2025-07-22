using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using static Ctronometro.BusinessLogic.Services.GeneralService;

namespace Cronometro.API_.Configuration
{
    public static class WebSocketConfiguration
    {
        public static void ConfigureWebSockets(this IServiceCollection services)
        {
            // Registrar el WebSocketConnectionManager como singleton
            services.AddSingleton<WebSocketConnectionManager>();
        }

        public static void UseWebSocketConfiguration(this IApplicationBuilder app)
        {
            // Habilitar WebSockets con opciones
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            
            app.UseWebSockets(webSocketOptions);
            
            // Usar el middleware personalizado si es necesario
            // app.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
