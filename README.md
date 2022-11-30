# OrgnalR

[![Actions Status](https://github.com/LiamMorrow/OrgnalR/workflows/build/badge.svg)](https://github.com/LiamMorrow/OrgnalR/actions)
[![Actions Status](https://github.com/LiamMorrow/OrgnalR/workflows/test/badge.svg)](https://github.com/LiamMorrow/OrgnalR/actions)
OrgnalR is a backplane for [SignalR core](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR), implemented through [Orleans](https://github.com/dotnet/orleans)!
It allows your SignalR servers to scale out with all the capacity of Orleans grains.

This is an alternative to the Redis backplane, and [SignalR.Orleans](https://github.com/OrleansContrib/SignalR.Orleans). This implementation does not use Orleans streams at all. This project was born out of issues with deadlocks that occured with Orleans streams, and since [SignalR.Orleans](https://github.com/OrleansContrib/SignalR.Orleans) uses them, issues with signalr messages not going through.

## Getting started

### Installing

OrgnalR comes in two packages, one for the Orleans Silo, and one for the SignalR application.

#### SignalR

<a href="https://www.nuget.org/packages/OrgnalR.Signalr">![OrgnalR SignalR](https://img.shields.io/nuget/v/OrgnalR.SignalR?logo=SignalR)</a>

```
dotnet add package OrgnalR.SignalR
```

#### Orleans Silo

<a href="https://www.nuget.org/packages/OrgnalR.OrleansSilo">![OrgnalR OrleansSilo](https://img.shields.io/nuget/v/OrgnalR.OrleansSilo?logo=OrleansSilo)</a>

```
dotnet add package OrgnalR.OrleansSilo
```

### Configuring

OrgnalR can be configured via extension methods on both the Orleans client/silo builders, and the SignalR builder.

#### SignalR

Somewhere in your `Startup.cs` (or wherever you configure your SignalR server), you will need to add an extension method to the SignalR builder. The extension method lives in the `OrgnalR.SignalR` namespace, so be sure to add a using for that namespace.

```c#
using OrgnalR.SignalR;
class Startup {
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR()
                .UseOrgnalR();
    }
}
```

That's it on the SignalR side. There is no difference between a production and a development environment for the SignalR client.

#### Orleans Silo

Wherever you configure your orleans Silo, you will need to configure OrgnalR's grains. This is again accomplished by an extension method, however there are two different modes. For development, it is easiest to use the `AddOrgnalRWithMemoryGrainStorage` extension method, which registers the storage providers for the grains with memory storage. This is undesirable for production as if the silo dies, the information on connections in which groups is lost.

For production usage it is best to configure actual persistent storage for `ORGNALR_USER_STORAGE`, `ORGNALR_GROUP_STORAGE`, and `MESSAGE_STORAGE_PROVIDER`, then use the `AddOrgnalR` extension method.

Both of these methods are found in the `OrgnalR.Silo` namespace.

##### Development

```c#
var builder = new SiloHostBuilder()
/* Your other configuration options */
// Note here we use the memory storage option.
// This is good for quick development, but we should register proper storage for production
                .AddOrgnalRWithMemoryGrainStorage()
```

##### Production

```c#
var builder = new SiloHostBuilder()
/* Your other configuration options */
// Note here we specify the storage we will use for group and user membership
                .ConfigureServices(services =>
                {
                    services.AddSingletonNamedService<IGrainStorage, YourStorageProvider>(Extensions.USER_STORAGE_PROVIDER);
                    services.AddSingletonNamedService<IGrainStorage, YourStorageProvider>(Extensions.GROUP_STORAGE_PROVIDER);
                    services.AddSingletonNamedService<IGrainStorage, YourStorageProvider>(Extensions.MESSAGE_STORAGE_PROVIDER);
                })
                .AddOrgnalR()
```

And that's it! Your SignalR server will now use the OrgnalR backplane to send messages, and maintain groups / users.

# Contributing

Contributions are welcome! Simply fork the repository, and submit a PR. If you have an issue, feel free to submit an issue :)
