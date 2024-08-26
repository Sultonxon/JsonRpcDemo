using System.Net.WebSockets;
using StreamJsonRpc;
using StreamJsonRpcClientDemo;

using (var webSocket = new ClientWebSocket())
{
    await webSocket.ConnectAsync(new Uri("ws://localhost:5164/jsonrpcws"), CancellationToken.None);

    IJsonRpcMessageHandler jsonRpcMessageHandler = new WebSocketMessageHandler(webSocket);

    IJsonRpcService jsonRpcGreeterClient = JsonRpc.Attach<IJsonRpcService>(jsonRpcMessageHandler);

    HelloReply helloReply = await jsonRpcGreeterClient.SayHelloAsync(new HelloRequest { Name = "Sultonxon" });

    Console.WriteLine(helloReply.Message);

    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
}
