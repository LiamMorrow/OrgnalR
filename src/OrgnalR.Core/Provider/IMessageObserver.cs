using System.Threading;
using System.Threading.Tasks;

namespace OrgnalR.Core.Provider
{
    public interface IMessageObserver
    {
        Task SendAllMessageAsync(AnonymousMessage allMessage, CancellationToken cancellationToken = default);
        Task SendAddressedMessageAsync(AddressedMessage msg, CancellationToken cancellationToken = default);
    }
}