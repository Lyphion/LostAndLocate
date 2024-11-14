using System.Buffers;
using System.Net.WebSockets;
using System.Text;

namespace LostAndLocate.Utils;

public static class WebSocketExtensions
{
    public static async Task SendAsync(this WebSocket socket, string data, CancellationToken token = default)
    {
        var buffer = Encoding.UTF8.GetBytes(data);
        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, token);
    }

    public static async Task<string> ReceiveStringAsync(this WebSocket socket, CancellationToken token = default)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(8192);

        using var ms = new MemoryStream();
        WebSocketReceiveResult result;

        do
        {
            token.ThrowIfCancellationRequested();

            result = await socket.ReceiveAsync(buffer, token);
            ms.Write(buffer, 0, result.Count);
        } while (!result.EndOfMessage);

        ArrayPool<byte>.Shared.Return(buffer);

        if (result.MessageType != WebSocketMessageType.Text)
            return string.Empty;

        ms.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(ms, Encoding.UTF8);

        return await reader.ReadToEndAsync();
    }

    public static Task SendSaveAsync(this WebSocket socket, string data, CancellationToken token = default)
    {
        if (socket.State != WebSocketState.Open)
            return Task.CompletedTask;
        
        try
        {
            return socket.SendAsync(data, token);
        }
        catch
        {
            return Task.CompletedTask;
        }
    }
}