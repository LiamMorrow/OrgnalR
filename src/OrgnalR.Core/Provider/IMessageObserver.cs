using System.Threading;
using System.Threading.Tasks;

namespace OrgnalR.Core.Provider
{
    public interface IMessageObserver
    {
        Task SendAllMessageAsync(AllMessage allMessage, CancellationToken cancellationToken = default);

    }
}