using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrgnalR.Core.Provider
{
    public interface IMessageObservable
    {
        Task<SubscriptionHandle> SubscribeToAllAsync(Func<AnonymousMessage, Task> messageCallback, Func<SubscriptionHandle, Task> onSubscriptionEnd, CancellationToken cancellationToken = default);
        Task UnsubscribeFromAllAsync(SubscriptionHandle subscriptionHandle, CancellationToken cancellationToken = default);
        Task SubscribeToConnectionAsync(string connectionId, Func<AddressedMessage, Task> messageCallback, Func<string, Task> onSubscriptionEnd, CancellationToken cancellationToken = default);
        Task UnsubscribeFromSpecificAsync(string connectionId, CancellationToken cancellationToken = default);
    }

    public class SubscriptionHandle
    {
        public Guid SubscriptionId { get; }
        public SubscriptionHandle(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }

}