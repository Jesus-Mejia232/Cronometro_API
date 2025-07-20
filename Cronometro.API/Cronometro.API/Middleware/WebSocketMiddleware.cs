using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    public WebSocketMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path == "/ws/cronometro")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await IniciarCronometroTiempoReal(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
        else
        {
            await _next(context);
        }
    }

    private async Task IniciarCronometroTiempoReal(WebSocket socket)
    {
        var cts = new CancellationTokenSource();
        var startTime = DateTime.Now;

        while (!cts.Token.IsCancellationRequested && socket.State == WebSocketState.Open)
        {
            var elapsed = DateTime.Now - startTime;
            var tiempo = $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            var buffer = Encoding.UTF8.GetBytes(tiempo);

            await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            await Task.Delay(1000); // enviar cada segundo
        }
    }
}
