using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Core.Provider;

namespace OrgnalR.Core.State
{
    public interface IUserActor : IMessageAcceptor
    {
        Task AddToUserAsync(string connectionId, CancellationToken cancellationToken = default);
        Task RemoveFromUserAsync(
            string connectionId,
            CancellationToken cancellationToken = default
        );
    }
}
