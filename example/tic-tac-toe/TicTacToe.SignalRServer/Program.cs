using System.Data.Common;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using OrgnalR.SignalR;
using Orleans.Runtime;
using TicTacToe.SignalRServer.Hubs;
using TicTacToe.SignalRServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrleansClient(orleans =>
{
    var connectionString = builder.Configuration.GetConnectionString("OrleansCluster");

    if (connectionString == null || connectionString.Length == 0)
    {
        throw new InvalidOperationException("Missing OrleansCluster connection string");
    }
    var connStringObj = new DbConnectionStringBuilder { ConnectionString = connectionString };
    var uris = ((string)connStringObj["endpoints"]).Split(",");
    if (uris.Length == 0)
    {
        throw new InvalidOperationException("Missing uris in connection string");
    }

    var endpoints = uris.SelectMany(ipAndPort =>
    {
        var split = ipAndPort.Split(":");
        if (!IPAddress.TryParse(split[0], out var iP))
        {
            return Dns.GetHostAddresses(split[0])
                .Select(ip => new IPEndPoint(ip, int.Parse(split[1])));
        }
        return new[] { new IPEndPoint(iP, int.Parse(split[1])) };
    });
    orleans.UseStaticClustering(endpoints.ToArray());
});

builder.Services.AddCors(
    options =>
        options.AddPolicy(
            "CorsPolicy",
            builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(s => true)
                    .AllowCredentials();
            }
        )
);

builder.Services
    .AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .UseOrgnalR();

builder.Services.AddSingleton<GameService>();

var app = builder.Build();

app.UseCors("CorsPolicy");
app.MapHub<GameHub>("/game");

app.Run();
