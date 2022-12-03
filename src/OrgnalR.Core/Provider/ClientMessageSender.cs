using System.Collections.Generic;
using System.Threading.Tasks;
using OrgnalR.Core.State;

namespace OrgnalR.Core.Provider;

/// <summary>
/// A class which can be used to send messages to connected clients.
/// <see cref="HubContext"> </see>
/// </summary>
public sealed class ClientMessageSender
{
    private readonly IMessageAcceptor messageAcceptor;
    private readonly IMessageArgsSerializer serializer;
    private readonly ISet<string> excluding;

    public ClientMessageSender(
        IMessageAcceptor messageAcceptor,
        IMessageArgsSerializer serializer,
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
                new MethodMessage(methodName, serializer.Serialize(parameters))
            )
        );
    }
}
