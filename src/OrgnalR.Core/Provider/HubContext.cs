using System.Collections.Generic;
using OrgnalR.Core.Data;
using OrgnalR.Core.State;
using Orleans.Serialization;

namespace OrgnalR.Core.Provider;

/// <summary>
/// Exposes methods for creating <see cref=ClientMessageSender></see>s to send messages to clients connected to the SignalR hub from inside grains
/// </summary>
public sealed class HubContext
{
    private readonly string hubName;
    private readonly IActorProviderFactory providerFactory;
    private readonly Serializer serializer;

    public HubContext(string hubName, IActorProviderFactory providerFactory, Serializer serializer)
    {
        this.hubName = hubName;
        this.providerFactory = providerFactory;
        this.serializer = serializer;
    }

    /// <summary>
    /// Gets a message sender for sending messages to a specific connection
    /// </summary>
    /// <param name="connectionId">the connectionId of the client to send messages to</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to the specified client</returns>
    public ClientMessageSender Connection(string connectionId) =>
        new(
            providerFactory.GetClientActor(hubName, connectionId),
            serializer,
            EmptySet<string>.Instance
        );

    /// <summary>
    /// Gets a message sender for sending messages to all connections in a hub
    /// </summary>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients</returns>
    public ClientMessageSender All() =>
        new(providerFactory.GetAllActor(hubName), serializer, EmptySet<string>.Instance);

    /// <summary>
    /// Gets a message sender for sending messages to all connections in a hub except for the specified ones
    /// </summary>
    /// <param name="excludingConnectionIds">All connection Ids to exclude from receiving messages</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients except those specified</returns>
    public ClientMessageSender AllExcept(ISet<string> excludingConnectionIds) =>
        new(providerFactory.GetAllActor(hubName), serializer, excludingConnectionIds);

    /// <summary>
    /// Gets a message sender for sending messages to all connections that are in the specified group
    /// </summary>
    /// <param name="groupName">The name of the group to send messages to</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients in the group</returns>
    public ClientMessageSender Group(string groupName) =>
        new(
            providerFactory.GetGroupActor(hubName, groupName),
            serializer,
            EmptySet<string>.Instance
        );

    /// <summary>
    /// Gets a message sender for sending messages to all connections that are in the specified group,
    /// except those clients which are specified
    /// </summary>
    /// <param name="groupName">The name of the group to send messages to</param>
    /// <param name="excludingConnectionIds">All connection Ids to exclude from receiving messages</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients in the group, except those which are specified</returns>
    public ClientMessageSender GroupExcept(string groupName, ISet<string> excludingConnectionIds) =>
        new(providerFactory.GetGroupActor(hubName, groupName), serializer, excludingConnectionIds);

    /// <summary>
    /// Gets a message sender for sending messages to all connections that a user is connected with
    /// </summary>
    /// <param name="userId">The id of the user to send messages to</param>
    /// <param name="excludingConnectionIds">All connection Ids to exclude from receiving messages</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients which the user is connected with</returns>
    public ClientMessageSender User(string userId) =>
        new(providerFactory.GetUserActor(hubName, userId), serializer, EmptySet<string>.Instance);
}
