namespace WebJsonRpc.Rpc.Services;

public class JsonRpcService
{
    public async Task<HelloReply> SayHelloAsync(HelloRequest request)
    {
        return new HelloReply
        {
            Message = "Hello " + request.Name
        };
    }
}

public class HelloRequest
{
    public string Name { get; set; }
}

public class HelloReply
{
    public string Message { get; set; }
}