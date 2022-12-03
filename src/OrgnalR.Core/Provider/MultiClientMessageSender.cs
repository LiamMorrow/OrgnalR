using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OrgnalR.Core.State;

namespace OrgnalR.Core.Provider;

/// <summary>
/// A class which can be used to send messages to multiple connected clients.
/// <see cref="ClientMessageSender"> </see>
/// </summary>
internal sealed class MultiClientMessageSender : IClientProxy
{
    private readonly IReadOnlyList<IClientProxy> clientProxies;

    public MultiClientMessageSender(IReadOnlyList<IClientProxy> clientProxies)
    {
        this.clientProxies = clientProxies;
    }

    public Task SendCoreAsync(
        string methodName,
        object?[] parameters,
        CancellationToken cancellationToken = default
    )
    {
        return Task.WhenAll(
            clientProxies.Select(c => c.SendCoreAsync(methodName, parameters, cancellationToken))
        );
    }
}
