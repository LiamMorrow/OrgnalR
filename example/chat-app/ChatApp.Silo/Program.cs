using OrgnalR.Silo;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans(silo => silo.UseLocalhostClustering().AddOrgnalRWithMemoryGrainStorage());
var app = builder.Build();
app.Run();
