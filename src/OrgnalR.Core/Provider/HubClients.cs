using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using OrgnalR.Core.Data;

namespace OrgnalR.Core.Provider;

internal sealed class HubClients : IHubClients
{
    private readonly string hubName;
    private readonly IActorProviderFactory providerFactory;
    private readonly IMessageArgsSerializer serializer;

    public HubClients(
        string hubName,
        IActorProviderFactory providerFactory,
        IMessageArgsSerializer serializer
    )
    {
        this.hubName = hubName;
        this.providerFactory = providerFactory;
        this.serializer = serializer;
    }

    /// <summary>
    /// Gets a message sender for sending messages to all connections in a hub
    /// </summary>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients</returns>
    public IClientProxy All => AllExcept(EmptyList<string>.Instance);

    /// <summary>
    /// Gets a message sender for sending messages to all connections in a hub except for the specified ones
    /// </summary>
    /// <param name="excludingConnectionIds">All connection Ids to exclude from receiving messages</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients except those specified</returns>
    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) =>
        new ClientMessageSender(
            providerFactory.GetAllActor(hubName),
            serializer,
            excludedConnectionIds.ToSet()
        );

    /// <summary>
    /// Gets a message sender for sending messages to a specific connection
    /// </summary>
    /// <param name="connectionId">the connectionId of the client to send messages to</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to the specified client</returns>
    public IClientProxy Client(string connectionId) =>
        new ClientMessageSender(
            providerFactory.GetClientActor(hubName, connectionId),
            serializer,
            EmptySet<string>.Instance
        );

    /// <summary>
    /// Gets a message sender for sending messages to many a specific connections
    /// </summary>
    /// <param name="connectionIds">the connectionIds of the clients to send messages to</param>
    /// <returns>A <see cref="MultiClientMessageSender"/> which will send messages to the specified clients</returns>
    public IClientProxy Clients(IReadOnlyList<string> connectionIds)
    {
        return new MultiClientMessageSender(connectionIds.Select(Client).ToList());
    }

    /// <summary>
    /// Gets a message sender for sending messages to all connections that are in the specified group
    /// </summary>
    /// <param name="groupName">The name of the group to send messages to</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients in the group</returns>
    public IClientProxy Group(string groupName) =>
        GroupExcept(groupName, EmptyList<string>.Instance);

    /// <summary>
    /// Gets a message sender for sending messages to all connections that are in the specified group,
    /// except those clients which are specified
    /// </summary>
    /// <param name="groupName">The name of the group to send messages to</param>
    /// <param name="excludedConnectionIds">All connection Ids to exclude from receiving messages</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients in the group, except those which are specified</returns>
    public IClientProxy GroupExcept(
        string groupName,
        IReadOnlyList<string> excludedConnectionIds
    ) =>
        new ClientMessageSender(
            providerFactory.GetGroupActor(hubName, groupName),
            serializer,
            excludedConnectionIds.ToSet()
        );

    /// <summary>
    /// Gets a message sender for sending messages to all connections that are in the specified groups
    /// </summary>
    /// <param name="groupNames">The name of the groups to send messages to</param>
    /// <returns>A <see cref="MultiClientMessageSender"/> which will send messages to all clients in the groups</returns>
    public IClientProxy Groups(IReadOnlyList<string> groupNames) =>
        new MultiClientMessageSender(groupNames.Select(Group).ToList());

    /// <summary>
    /// Gets a message sender for sending messages to all connections that a user is connected with
    /// </summary>
    /// <param name="userId">The id of the user to send messages to</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients which the user is connected with</returns>
    public IClientProxy User(string userId) =>
        new ClientMessageSender(
            providerFactory.GetUserActor(hubName, userId),
            serializer,
            EmptySet<string>.Instance
        );

    /// <summary>
    /// Gets a message sender for sending messages to all connections that the users are connected with
    /// </summary>
    /// <param name="userIds">The id of the users to send messages to</param>
    /// <returns>A <see cref="ClientMessageSender"/> which will send messages to all clients which the users are connected with</returns>
    public IClientProxy Users(IReadOnlyList<string> userIds) =>
        new MultiClientMessageSender(userIds.Select(User).ToList());
}
