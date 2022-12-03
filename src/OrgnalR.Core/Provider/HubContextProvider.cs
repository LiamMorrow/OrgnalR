using Orleans.Serialization;

namespace OrgnalR.Core.Provider;

/// <summary>
/// Provides methods for getting <see cref="HubContext"/> instances for sending messages to connected clients from within grains
/// </summary>
public sealed class HubContextProvider
{
    private readonly IActorProviderFactory providerFactory;
    private readonly Serializer serializer;

    public HubContextProvider(IActorProviderFactory providerFactory, Serializer serializer)
    {
        this.providerFactory = providerFactory;
        this.serializer = serializer;
    }

    /// <summary>
    /// Gets a HubContext for sending messages to connected clients
    /// </summary>
    /// <typeparam name="THub">The type of the hub which will send messages to connected clients.  Can be an interface with the same name as the hub.</typeparam>
    /// <returns>A <see cref="HubContext"/> to send messages to clients</returns>
    public HubContext GetHubContext<THub>()
    {
        var hubType = typeof(THub);
        var hubName =
            hubType.IsInterface && hubType.Name.StartsWith("I") ? hubType.Name[1..] : hubType.Name;
        return GetHubContext(hubName);
    }

    /// Gets a HubContext for sending messages to connected clients
    /// </summary>
    /// <param name="hubName">The class name of the hub which clients are connected to</param>
    /// <returns>A <see cref="HubContext"/> to send messages to clients</returns>
    public HubContext GetHubContext(string hubName)
    {
        return new HubContext(hubName, providerFactory, serializer);
    }
}
