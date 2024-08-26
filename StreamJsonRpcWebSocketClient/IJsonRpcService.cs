namespace StreamJsonRpcClientDemo;

public interface IJsonRpcService
{
    Task<HelloReply> SayHelloAsync(HelloRequest request);
}



public class HelloRequest
{
    public string Name { get; set; }
}

public class HelloReply
{
    public string Message { get; set; }
}
