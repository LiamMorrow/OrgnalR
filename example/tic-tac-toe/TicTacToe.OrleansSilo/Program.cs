using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrgnalR.Silo;
using TicTacToe.OrleansSilo.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOrleans(
    orleans => orleans.UseLocalhostClustering().AddOrgnalRWithMemoryGrainStorage()
);

builder.Services.AddSingleton<IGameStateNotifier, OrgnalRGameHubGameStateNotifier>();

var app = builder.Build();

await app.RunAsync();
