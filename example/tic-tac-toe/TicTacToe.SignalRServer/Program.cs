using OrgnalR.SignalR;
using TicTacToe.SignalRServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrleansClient(orleans =>
{
    orleans.UseLocalhostClustering();
});

builder.Services.AddSignalR().UseOrgnalR();

var app = builder.Build();

app.MapHub<GameHub>("/game");

app.Run();
