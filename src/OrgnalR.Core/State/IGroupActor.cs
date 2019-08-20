using System.Threading;
using System.Threading.Tasks;

namespace OrgnalR.Core.State
{
    public interface IGroupActor
    {
        Task AddToGroupAsync(string connectionId, CancellationToken cancellationToken = default);
        Task RemoveFromGroupAsync(string connectionId, CancellationToken cancellationToken = default);
    }
}