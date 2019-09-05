using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrgnalR.Core.Provider
{
    public interface IMessageObservable
    {
        /// <summary>
        /// Subscribes to messages for all messages to this hub, optionally replaying all messages since a last seen message
        /// </summary>
        /// <param name="since">The handle to get messages since, exclusive</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the message buffer does not go back as far as the requested message</exception>
        Task<SubscriptionHandle> SubscribeToAllAsync(
            Func<AnonymousMessage, MessageHandle, Task> messageCallback,
            Func<SubscriptionHandle, Task> onSubscriptionEnd,
            MessageHandle since = default,
            CancellationToken cancellationToken = default
        );
        Task UnsubscribeFromAllAsync(SubscriptionHandle subscriptionHandle, CancellationToken cancellationToken = default);
        /// <summary>
        /// Subscribes to messages for this client, optionally replaying all messages since a last seen message
        /// </summary>
        /// <param name="since">The handle to get messages since, exclusive</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the message buffer does not go back as far as the requested message</exception>
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
