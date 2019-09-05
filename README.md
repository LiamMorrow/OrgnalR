# OrgnalR

[![Actions Status](https://github.com/LiamMorrow/OrgnalR/workflows/build/badge.svg)](https://github.com/LiamMorrow/OrgnalR/actions)
OrgnalR is a backplane for [SignalR core](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR), implemented through [Orleans](https://github.com/dotnet/orleans)!  
It allows your SignalR servers to scale out with all the capacity of Orleans grains.

This is an alternative to the Redis backplane.  
Inspiration was taken from the original Orleans backplane, [SignalR.Orleans](https://github.com/OrleansContrib/SignalR.Orleans). OrgnalR was born from hard to debug issues with orleans streams that seemed unresolvable without a complete rewrite of the original library.  Big thanks to the contributers of SignalR.Orleans for all their hard work.

## Getting started

### Installing

OrgnalR comes in two packages, one for the Orleans Silo, and one for the SignalR application.

#### SignalR

![OrgnalR SignalR Nuget](https://img.shields.io/nuget/v/OrgnalR.SignalR?logo=SignalR)
```
dotnet add package OrgnalR.SignalR
```

#### Orleans Silo
![OrgnalR Silo Nuget](https://img.shields.io/nuget/v/OrgnalR.OrleansSilo?logo=OrleansSilo)
```
dotnet add package OrgnalR.OrleansSilo
```

### Configuring

OrgnalR can be configured via extension methods on both the Orleans client/silo builders, and the SignalR builder.

#### SignalR

Somewhere in your `Startup.cs` (or wherever you configure your SignalR server), you will need to add two extension methods. One on the SignalR builder, and one on the Orleans ClientBuilder. These extension methods live in the `OrgnalR.SignalR` namespace, so be sure to add a using for that namespace.

```c#
using OrgnalR.SignalR;
class Startup {
        public void ConfigureServices(IServiceCollection services)
        {
            /* All your other services */
            services.AddSingleton<IClusterClient>(serviceProvider =>
            {
                return new ClientBuilder()
                /* Your other orleans client configuration */
                    .UseOrgnalR()
                    .Build();
            });
            services.AddSignalR()
                    .UseOrgnalR();
        }
}
```

That's it on the SignalR side. There is no difference between a production and a development environment for the SignalR client.

#### Orleans Silo

Wherever you configure your orleans Silo, you will need to configure OrgnalR's grains. This is again accomplished by an extension method, however there are two different modes. For development, it is easiest to use the `AddOrgnalRWithMemoryGrainStorage` extension method, which registers the storage providers for the grains with memory storage. This is undesirable for production as if the silo dies, the information on connections in which groups is lost.

For production usage it is best to configure actual persistent storage for `ORGNALR_USER_STORAGE` and `ORGNALR_GROUP_STORAGE`, then use the `AddOrgnalR` extension method.

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
                })
                .AddOrgnalR()
```

And that's it! Your SignalR server will now use the OrgnalR backplane to send messages, and maintain groups / users.

# Contributing

Contributions are welcome! Simply fork the repository, and submit a PR. If you have an issue, feel free to submit an issue :)
