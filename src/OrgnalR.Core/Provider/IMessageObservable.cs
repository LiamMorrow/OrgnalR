using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrgnalR.Core.Provider
{
    public interface IMessageObservable
    {
        Task<SubscriptionHandle> SubscribeToAllAsync(Func<AllMessage, Task> messageCallback, CancellationToken cancellationToken = default);
        Task UnsubscribeFromAllAsync(SubscriptionHandle subscriptionHandle, CancellationToken cancellationToken = default);
    }

    public class SubscriptionHandle
    {
        public string SubscriptionId { get; }
        public SubscriptionHandle(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }

}