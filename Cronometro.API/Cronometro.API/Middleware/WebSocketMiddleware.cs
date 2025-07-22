using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using static Ctronometro.BusinessLogic.Services.GeneralService;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebSocketConnectionManager _manager;


    public WebSocketMiddleware(RequestDelegate next, WebSocketConnectionManager manager)
    {
        _next = next;
        _manager = manager;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await _next(context);
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        var connectionId = Guid.NewGuid().ToString();

        _manager.AddSocket(connectionId, socket);

        // Guardar connectionId en contexto para usarlo en controlador (por ejemplo, headers o query)
        context.Items["WebSocketConnectionId"] = connectionId;

        await Receive(socket, async (result, buffer) =>
        {
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _manager.RemoveSocket(connectionId);
            }
        });
    }
    // En tu servicio o middleware WebSocket, algo como:

    public async Task StartCronometroAsync(WebSocket socket, CancellationToken token)
    {
        int segundos = 0;

        while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
        {
            segundos++;

            var mensaje = JsonSerializer.Serialize(new { elapsedSeconds = segundos });
            var buffer = Encoding.UTF8.GetBytes(mensaje);
            var segment = new ArraySegment<byte>(buffer);

            await socket.SendAsync(segment, WebSocketMessageType.Text, true, token);

            await Task.Delay(1000, token); // Espera 1 segundo
        }
    }

    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            handleMessage(result, buffer);
        }
    }
    
}
