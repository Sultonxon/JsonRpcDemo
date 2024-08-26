using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Connections;
using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;
using WebJsonRpc.Rpc.Services;

namespace WebJsonRpc.Rpc.Background;

public class JsonRpcHost : BackgroundService
{
    protected readonly IConnectionListenerFactory connectionListenerFactory;
    private readonly ConcurrentDictionary<string, (ConnectionContext Context, Task ExecutionTask)> _connections = new ConcurrentDictionary<string, (ConnectionContext, Task)>();
    private ILogger<JsonRpcHost> _logger;

    public JsonRpcHost(IConnectionListenerFactory connectionListenerFactory, ILogger<JsonRpcHost> logger)
    {
        this.connectionListenerFactory = connectionListenerFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = await connectionListenerFactory.BindAsync(new IPEndPoint(IPAddress.Any, 6000), stoppingToken);

        _logger.LogInformation("Listening for connections on port 6000");
        while (true)
        {
            
            ConnectionContext connectionContext = await listener.AcceptAsync(stoppingToken);

            // AcceptAsync will return null upon disposing the listener
            if (connectionContext == null)
            {
                break;
            }

            _connections[connectionContext.ConnectionId] = (connectionContext, AcceptAsync(connectionContext));
        }
    }

    private async Task AcceptAsync(ConnectionContext connectionContext)
    {
        try
        {
            await Task.Yield();
            _logger.LogInformation("Connection {ConnectionId} connected", connectionContext.ConnectionId);

            
            
            IJsonRpcMessageFormatter jsonRpcMessageFormatter = new JsonMessageFormatter(Encoding.UTF8);
            IJsonRpcMessageHandler jsonRpcMessageHandler = new LengthHeaderMessageHandler(connectionContext.Transport, jsonRpcMessageFormatter);

            
            
            using (var jsonRpc = new JsonRpc(jsonRpcMessageHandler, new JsonRpcService()))
            {
                jsonRpc.StartListening();

                await jsonRpc.Completion;
            }

        }
        catch (ConnectionResetException)
        { }
        catch (ConnectionAbortedException)
        { }
        catch (Exception e)
        {
            _logger.LogError(e, "Connection {ConnectionId} threw an exception", connectionContext.ConnectionId);
        }
        finally
        {
            await connectionContext.DisposeAsync();

            _connections.TryRemove(connectionContext.ConnectionId, out _);

            _logger.LogInformation("Connection {ConnectionId} disconnected", connectionContext.ConnectionId);
        }
    }
}