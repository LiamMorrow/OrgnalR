using OrgnalR.SignalR;
using TicTacToe.SignalRServer.Hubs;
using TicTacToe.SignalRServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrleansClient(orleans =>
{
    orleans.UseLocalhostClustering();
});

builder.Services.AddSignalR().UseOrgnalR();

builder.Services.AddSingleton<GameService>();

var app = builder.Build();

app.MapHub<GameHub>("/game");

app.Run();
