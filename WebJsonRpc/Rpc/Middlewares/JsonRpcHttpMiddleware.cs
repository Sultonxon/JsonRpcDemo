using System.Text;
using StreamJsonRpc;
using WebJsonRpc.Rpc.Services;

namespace WebJsonRpc.Rpc.Middlewares;

public class JsonRpcHttpMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JsonRpcHttpMiddleware> _logger;

    public JsonRpcHttpMiddleware(RequestDelegate next, ILogger<JsonRpcHttpMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/rpc"))
        {
            _logger.LogInformation("Handling JSON-RPC HTTP request");

            var buffer = new byte[(int)context.Request.ContentLength];
            
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);

            var json = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(json);
            
            var jsonRpcFormatter = new JsonMessageFormatter(Encoding.UTF8);
            var jsonRpcHandler = new LengthHeaderMessageHandler(context.Response.Body, context.Request.Body, jsonRpcFormatter);

            using (var jsonRpc = new JsonRpc(jsonRpcHandler, new JsonRpcService()))
            {
                jsonRpc.StartListening();
                
                await jsonRpc.Completion;
            }

            return;
        }

        await _next(context);
    }
    
    
}
