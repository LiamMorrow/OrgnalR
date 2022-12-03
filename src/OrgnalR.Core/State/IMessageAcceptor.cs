using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Core.Provider;

namespace OrgnalR.Core.State;

public interface IMessageAcceptor
{
    Task AcceptMessageAsync(
        AnonymousMessage targetedMessage,
        CancellationToken cancellationToken = default
    );
}
