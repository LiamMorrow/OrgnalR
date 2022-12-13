using System.Text.Json.Serialization;
using OrgnalR.SignalR;
using TicTacToe.SignalRServer.Hubs;
using TicTacToe.SignalRServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrleansClient(orleans =>
{
    orleans.UseLocalhostClustering();
});

builder.Services
    .AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .UseOrgnalR();

builder.Services.AddSingleton<GameService>();

var app = builder.Build();

app.MapHub<GameHub>("/game");

app.Run();
