using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Core.Provider;

namespace OrgnalR.Core.State
{
    public interface IUserActor
    {
        Task AddToUserAsync(string connectionId, CancellationToken cancellationToken = default);
        Task RemoveFromUserAsync(string connectionId, CancellationToken cancellationToken = default);
        Task AcceptMessageAsync(AnonymousMessage targetedMessage, CancellationToken cancellationToken = default);
    }
}