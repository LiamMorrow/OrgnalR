using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrgnalR.Core.Provider
{
    public interface IMessageObservable
    {
        Task<SubscriptionHandle> SubscribeToAllAsync(
            Func<AnonymousMessage, MessageHandle, Task> messageCallback,
            Func<SubscriptionHandle, Task> onSubscriptionEnd,
            MessageHandle since = default,
            CancellationToken cancellationToken = default
        );
        Task UnsubscribeFromAllAsync(SubscriptionHandle subscriptionHandle, CancellationToken cancellationToken = default);
        Task SubscribeToConnectionAsync(
            string connectionId,
            Func<AddressedMessage, MessageHandle, Task> messageCallback,
            Func<string, Task> onSubscriptionEnd,
            MessageHandle since = default,
            CancellationToken cancellationToken = default
        );
        Task UnsubscribeFromConnectionAsync(string connectionId, CancellationToken cancellationToken = default);
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
