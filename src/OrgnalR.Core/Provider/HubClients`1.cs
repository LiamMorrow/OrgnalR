using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using OrgnalR.Core.Data;

namespace OrgnalR.Core.Provider;

internal sealed class HubClients<THubClient> : IHubClients<THubClient> where THubClient : class
{
    private readonly IHubClients hubClients;

    public HubClients(IHubClients hubClients)
    {
        this.hubClients = hubClients;
    }

    public THubClient All => TypedClientBuilder<THubClient>.Build(hubClients.All);

    public THubClient AllExcept(IReadOnlyList<string> excludedConnectionIds) =>
        TypedClientBuilder<THubClient>.Build(hubClients.AllExcept(excludedConnectionIds));

    public THubClient Client(string connectionId) =>
        TypedClientBuilder<THubClient>.Build(hubClients.Client(connectionId));

    public THubClient Clients(IReadOnlyList<string> connectionIds) =>
        TypedClientBuilder<THubClient>.Build(hubClients.Clients(connectionIds));

    public THubClient Group(string groupName) =>
        TypedClientBuilder<THubClient>.Build(hubClients.Group(groupName));

    public THubClient GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) =>
        TypedClientBuilder<THubClient>.Build(
            hubClients.GroupExcept(groupName, excludedConnectionIds)
        );

    public THubClient Groups(IReadOnlyList<string> groupNames) =>
        TypedClientBuilder<THubClient>.Build(hubClients.Groups(groupNames));

    public THubClient User(string userId) =>
        TypedClientBuilder<THubClient>.Build(hubClients.User(userId));

    public THubClient Users(IReadOnlyList<string> userIds) =>
        TypedClientBuilder<THubClient>.Build(hubClients.Users(userIds));
}
