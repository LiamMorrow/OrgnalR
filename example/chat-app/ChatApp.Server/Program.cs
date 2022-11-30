using ChatApp.Server.Hubs;
using OrgnalR.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseOrleansClient(client =>
{
    client.UseLocalhostClustering();
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToFile("index.html");

app.Run();
