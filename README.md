# OrgnalR

OrgnalR is a backplane for [SignalR core](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR), implemented through [Orleans](https://github.com/dotnet/orleans)!  
It allows your SignalR servers to scale out with all the capacity of Orleans grains.

This is an alternative to the Redis backplane.  
Inspiration was taken from the original Orleans backplane, [SignalR.Orleans](https://github.com/OrleansContrib/SignalR.Orleans).  I'm very grateful for its existence, however I ran into hard to debug issues with the orleans streams that I was unable to resolve.  

## Getting started  

### Installing

OrgnalR comes in two packages, one for the Orleans Silo, and one for the SignalR application.  
#### SignalR
```
dotnet add package OrgnalR.SignalR
``` 
OrgnalR can be configured via extension methods on both the Orleans client/silo builders, and the SignalR builder.  

