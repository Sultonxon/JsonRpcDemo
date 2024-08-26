using System.Net.Sockets;
using System.Text;
using StreamJsonRpc;
using StreamJsonRpcClientDemo;

TcpClient tcpClient = new TcpClient("localhost", 6000);

Stream jsonRpcStream = tcpClient.GetStream();
IJsonRpcMessageFormatter jsonRpcMessageFormatter = new JsonMessageFormatter(Encoding.UTF8);
IJsonRpcMessageHandler jsonRpcMessageHandler = new LengthHeaderMessageHandler(jsonRpcStream, jsonRpcStream, jsonRpcMessageFormatter);

IJsonRpcService jsonRpcGreeterClient = JsonRpc.Attach<IJsonRpcService>(jsonRpcMessageHandler);

HelloReply helloReply = await jsonRpcGreeterClient.SayHelloAsync(new HelloRequest { Name = "Sultonxon" });

Console.WriteLine(helloReply.Message);

jsonRpcStream.Close();

Console.ReadKey();