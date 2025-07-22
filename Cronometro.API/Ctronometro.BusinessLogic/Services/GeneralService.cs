using Cronometro.BusinessLogic;
using Cronometro.DataAccess.Repositories;
using Cronometro.Entities.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Ctronometro.BusinessLogic.Services
{
    public class GeneralService
    {
        private readonly CronometroReposotory _cronometroReposotory;
        private readonly WebSocketConnectionManager _connectionManager;


        public GeneralService(CronometroReposotory cronometroReposotory, WebSocketConnectionManager connectionManager)
        {
            _cronometroReposotory = cronometroReposotory;
            _connectionManager = connectionManager;

        }
        public async Task EnviarConteoCronometroAsync(WebSocket socket, int registroID)
        {
            var startTime = DateTime.UtcNow;

            while (socket.State == WebSocketState.Open)
            {
                var tiempoActual = DateTime.UtcNow - startTime;
                var tiempoFormateado = tiempoActual.ToString(@"hh\:mm\:ss");

                var buffer = Encoding.UTF8.GetBytes(tiempoFormateado);
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

                await Task.Delay(1000); // Espera de 1 segundo
            }
        }


        public async Task IniciarConteoAsync(string socketId, int registroId, CancellationToken cancellationToken)
        {
            var socket = _connectionManager.GetSocketById(socketId);

            if (socket == null || socket.State != WebSocketState.Open)
                return;

            var startTime = DateTime.UtcNow;

            try
            {
                while (!cancellationToken.IsCancellationRequested && socket.State == WebSocketState.Open)
                {
                    var elapsed = DateTime.UtcNow - startTime;
                    var message = $"{{\"registroId\":{registroId},\"elapsedSeconds\":{(int)elapsed.TotalSeconds}}}";
                    var bytes = Encoding.UTF8.GetBytes(message);

                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Cancelling, no acción necesaria
            }
            catch (Exception ex)
            {
                // Loguear excepción si es necesario
                Console.WriteLine($"Error en WebSocket cronómetro: {ex.Message}");
            }
            finally
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Conteo finalizado", CancellationToken.None);
                }
                await _connectionManager.RemoveSocket(socketId);
            }
        }
        public ServiceResult IniciarCronometro(tbProyectosTiempos item)
        {
            var result = new ServiceResult();
            try
            {
                var insert = _cronometroReposotory.Insert(item);
                return result.Ok(insert);
            }
            catch (Exception ex)
            {
                return result.Error(ex.Message);
            }
        }

        public ServiceResult FinalizarCronometro(int registroID, TimeSpan horaFin)
        {
            var result = new ServiceResult();
            try
            {
                var update = _cronometroReposotory.Finalizar(registroID, horaFin);
                return result.Ok(update);
            }
            catch (Exception ex)
            {
                return result.Error(ex.Message);
            }
        }


        public class WebSocketConnectionManager
        {
            private readonly ConcurrentDictionary<string, (WebSocket Socket, CancellationTokenSource CancellationTokenSource)> _connections
                = new();

            public void AddSocket(string id, WebSocket socket)
            {
                var cts = new CancellationTokenSource();
                _connections.TryAdd(id, (socket, cts));
            }

            public CancellationTokenSource GetCancellationTokenSource(string id)
            {
                if (_connections.TryGetValue(id, out var tuple))
                    return tuple.CancellationTokenSource;
                return null;
            }

            public WebSocket GetSocketById(string id)
            {
                if (_connections.TryGetValue(id, out var tuple))
                    return tuple.Socket;
                return null;
            }

            public async Task RemoveSocket(string id)
            {
                if (_connections.TryRemove(id, out var tuple))
                {
                    tuple.CancellationTokenSource.Cancel();
                    await tuple.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }
            }
        }


    }
}
