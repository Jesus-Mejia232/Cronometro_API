using AutoMapper;
using Cronometro.API_.Models;
using Cronometro.DataAccess.Repositories;
using Cronometro.Entities.Entities;
using Ctronometro.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using static Ctronometro.BusinessLogic.Services.GeneralService;

namespace Cronometro.API_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CronometroController : Controller
    {
        private readonly GeneralService _generalService;
        private readonly IMapper _mapper;
        private readonly WebSocketConnectionManager _connectionManager;

        public CronometroController(
            GeneralService generalService,
            IMapper mapper,
            WebSocketConnectionManager connectionManager)
        {
            _generalService = generalService;
            _mapper = mapper;
            _connectionManager = connectionManager;
        }

        [HttpGet("/wscronometro")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var socketId = Guid.NewGuid().ToString();
                _connectionManager.AddSocket(socketId, socket);

                // Enviar el socketId al cliente inmediatamente
                var welcomeMessage = $"{{\"type\":\"connection\",\"socketId\":\"{socketId}\",\"message\":\"Connected successfully\"}}";
                var welcomeBytes = Encoding.UTF8.GetBytes(welcomeMessage);
                await socket.SendAsync(new ArraySegment<byte>(welcomeBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                // Mantener la conexión activa y manejar mensajes entrantes
                var buffer = new byte[1024 * 4];
                try
                {
                    while (socket.State == WebSocketState.Open)
                    {
                        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error if needed
                    Console.WriteLine($"WebSocket error: {ex.Message}");
                }
                finally
                {
                    await _connectionManager.RemoveSocket(socketId);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> IniciarCronometro([FromBody] ProyectoTiempoParcialDto item)
        {
            var mapped = _mapper.Map<tbProyectosTiempos>(item);
            var result = _generalService.IniciarCronometro(mapped);

            if (!result.Success)
                return BadRequest(result);

            // Extraer el RegistroID correctamente del result.Data
            int registroId;
            if (result.Data != null)
            {
                // Si result.Data es un RequestStatus, necesitamos extraer el ID de ahí
                if (result.Data is RequestStatus requestStatus)
                {
                    registroId = requestStatus.RegistroID ?? 0; // Usar la propiedad RegistroID
                }
                else
                {
                    // Si es directamente un número, convertirlo
                    registroId = Convert.ToInt32(result.Data);
                }
            }
            else
            {
                return BadRequest(new { message = "No se pudo obtener el ID del registro" });
            }

            // Extraemos el socketId del header (enviado por el cliente JS)
            var socketId = Request.Headers["X-Socket-Id"].ToString();
            
            if (!string.IsNullOrEmpty(socketId))
            {
                var cts = new CancellationTokenSource();
                _ = Task.Run(() =>
                    _generalService.IniciarConteoAsync(socketId, registroId, cts.Token)
                );
            }

            // Devolver una respuesta JSON válida con el registroId
            return Ok(new { 
                success = true, 
                data = registroId,
                message = "Cronómetro iniciado correctamente"
            });
        }

        [HttpPut("Finalizar/{registroID}/{horaFin}")]
        public async Task<IActionResult> FinalizarCronometro(int registroID, TimeSpan horaFin)
        {
            var result = _generalService.FinalizarCronometro(registroID, horaFin);

            if (result.Code == 1)
            {
                if (Request.Headers.TryGetValue("X-Connection-Id", out var connectionIds))
                {
                    var connectionId = connectionIds.FirstOrDefault();
                    if (!string.IsNullOrEmpty(connectionId))
                    {
                        var cts = _connectionManager.GetCancellationTokenSource(connectionId);
                        cts?.Cancel();
                    }
                }
            }

            return Ok(result);
        }
    }
}
