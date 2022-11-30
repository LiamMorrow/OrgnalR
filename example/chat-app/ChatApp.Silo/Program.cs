var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans(silo => silo.UseLocalhostClustering());
var app = builder.Build();
app.Run();
