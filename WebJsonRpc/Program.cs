using System.Net.WebSockets;
using WebJsonRpc.Rpc.Background;
using WebJsonRpc.Rpc.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<JsonRpcHost>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<JsonRpcHttpMiddleware>();

app.UseWebSockets()
    .UseMiddleware<WebSocketJsonRpcMiddleware>();

app.MapControllers();

app.Run();