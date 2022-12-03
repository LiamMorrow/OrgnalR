using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using OrgnalR.Core.State;
using Orleans.Concurrency;
using Orleans.Serialization;

namespace OrgnalR.Core.Provider;

/// <summary>
/// A class which can be used to send messages to connected clients.
/// <see cref=HubContext> </see>
/// </summary>
public sealed class ClientMessageSender
{
    private readonly IMessageAcceptor messageAcceptor;
    private readonly Serializer serializer;
    private readonly ISet<string> excluding;

    public ClientMessageSender(
        IMessageAcceptor messageAcceptor,
        Serializer serializer,
        ISet<string> excluding
    )
    {
        this.messageAcceptor = messageAcceptor;
        this.serializer = serializer;
        this.excluding = excluding;
    }

    public Task SendAsync(string methodName, params object[] parameters)
    {
        return messageAcceptor.AcceptMessageAsync(
            new AnonymousMessage(
                excluding,
                new MethodMessage(methodName, serializer.SerializeToArray(parameters))
            )
        );
    }
}
