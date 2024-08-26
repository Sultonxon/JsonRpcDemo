using StreamJsonRpc;
using WebJsonRpc.Rpc.Services;

namespace WebJsonRpc.Rpc.Middlewares;

public class WebSocketJsonRpcMiddleware
{
    private RequestDelegate _next;

    public WebSocketJsonRpcMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest && context.Request.Path == "/jsonrpcws")
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            IJsonRpcMessageHandler jsonRpcMessageHandler = new WebSocketMessageHandler(webSocket);

            using (var jsonRpc = new JsonRpc(jsonRpcMessageHandler, new JsonRpcService()))
            {
                jsonRpc.StartListening();

                await jsonRpc.Completion;
            }
        }
        else
        {
            await _next(context);
        }
    }
}