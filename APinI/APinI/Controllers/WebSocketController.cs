using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace APinI.Controllers
{
    [Route("api/websocket")]
    public class WebSocketController : ControllerBase
    {
        [HttpGet("/ws")]
        public async Task<IActionResult> WebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var buffer = new byte[1024 * 4]; // Increase buffer size if needed
                var connectionTimeSpend = TimeSpan.FromSeconds(0);
                if (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        // Handle the received message (e.g., log, process, or respond).
                        var responseMessage = $"Received: {receivedMessage}";
                        var responseBytes = Encoding.UTF8.GetBytes(responseMessage);

                        await webSocket.SendAsync(
                            new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                    }
                }
                while (webSocket.State == WebSocketState.Open && connectionTimeSpend <= TimeSpan.FromMinutes(1))
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    connectionTimeSpend += TimeSpan.FromSeconds(1);
                    var responseMessage = $"Ping from server: {DateTime.Now.ToLongTimeString()}";
                    var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                    await webSocket.SendAsync(
                            new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                }
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);

                return new EmptyResult(); // Return empty response after WebSocket session ends
            }
            else
            {
                return BadRequest("This endpoint requires a WebSocket connection.");
            }
        }
    }
}
