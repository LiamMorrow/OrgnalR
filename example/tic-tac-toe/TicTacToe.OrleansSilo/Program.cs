using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrgnalR.Silo;
using Orleans.Configuration;
using TicTacToe.OrleansSilo.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOrleans(
    orleans =>
        orleans
            .UseDevelopmentClustering(primarySiloEndpoint: null)
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "default";
                options.ServiceId = "default";
            })
            .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
            .AddOrgnalRWithMemoryGrainStorage()
);

builder.Services.AddSingleton<IGameStateNotifier, OrgnalRGameHubGameStateNotifier>();
builder.Services.AddSingleton<IGameStateNotifier, OrleansBotGameStateNotifier>();

var app = builder.Build();

await app.RunAsync();
