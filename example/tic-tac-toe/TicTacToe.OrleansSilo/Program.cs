using Microsoft.Extensions.Hosting;
using OrgnalR.Silo;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOrleans(
    orleans => orleans.UseLocalhostClustering().AddOrgnalRWithMemoryGrainStorage()
);

var app = builder.Build();

await app.RunAsync();
