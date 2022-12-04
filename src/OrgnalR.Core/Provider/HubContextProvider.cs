using Microsoft.AspNetCore.SignalR;

namespace OrgnalR.Core.Provider;

/// <summary>
/// Provides methods for getting <see cref="IHubContext"/> instances for sending messages to connected clients from within grains
/// </summary>
public interface IHubContextProvider
{
    /// <summary>
    /// Gets a HubContext for sending messages to connected clients
    /// </summary>
    /// <typeparam name="THub">The type of the hub which will send messages to connected clients.  Can be an interface with the same name as the hub.</typeparam>
    /// <returns>A <see cref="HubContext"/> to send messages to clients</returns>
    IHubContext GetHubContext<THub>();

    /// <summary>
    /// Gets a HubContext for sending messages to connected clients
    /// </summary>
    /// <param name="hubName">The class name of the hub which clients are connected to</param>
    /// <returns>A <see cref="HubContext"/> to send messages to clients</returns>
    IHubContext GetHubContext(string hubName);

    /// <summary>
    /// Gets a typed HubContext for sending messages to connected clients in a strongly typed manner
    /// </summary>
    /// <returns>A <see cref="HubContext"/> to send messages to clients</returns>
    public IHubContext<Hub<THubClient>, THubClient> GetHubContext<THub, THubClient>()
        where THubClient : class;

    /// <summary>
    /// Gets a typed HubContext for sending messages to connected clients in a strongly typed manner
    /// </summary>
    /// <param name="hubName">The class name of the hub which clients are connected to</param>
    /// <returns>A <see cref="HubContext"/> to send messages to clients</returns>
    public IHubContext<Hub<THubClient>, THubClient> GetHubContext<THubClient>(string hubName)
        where THubClient : class;
}

/// <summary>
/// The default implementation of the <see cref="IHubContextProvider"/>
/// </summary>
public sealed class HubContextProvider : IHubContextProvider
{
    private readonly IActorProviderFactory providerFactory;
    private readonly IMessageArgsSerializer serializer;

    public HubContextProvider(
        IActorProviderFactory providerFactory,
        IMessageArgsSerializer serializer
    )
    {
        this.providerFactory = providerFactory;
        this.serializer = serializer;
    }

    ///<inheritdoc/>
    public IHubContext GetHubContext<THub>()
    {
        var hubType = typeof(THub);
        var hubName =
            hubType.IsInterface && hubType.Name.StartsWith("I") ? hubType.Name[1..] : hubType.Name;
        return GetHubContext(hubName);
    }

    ///<inheritdoc/>
    public IHubContext GetHubContext(string hubName)
    {
        return new HubContext(hubName, providerFactory, serializer);
    }

    ///<inheritdoc/>
    public IHubContext<Hub<THubClient>, THubClient> GetHubContext<THub, THubClient>()
        where THubClient : class
    {
        var hubType = typeof(THub);
        var hubName =
            hubType.IsInterface && hubType.Name.StartsWith("I") ? hubType.Name[1..] : hubType.Name;
        return GetHubContext<THubClient>(hubName);
    }

    ///<inheritdoc/>
    public IHubContext<Hub<THubClient>, THubClient> GetHubContext<THubClient>(string hubName)
        where THubClient : class
    {
        return new HubContext<THubClient>(new HubContext(hubName, providerFactory, serializer));
    }
}
