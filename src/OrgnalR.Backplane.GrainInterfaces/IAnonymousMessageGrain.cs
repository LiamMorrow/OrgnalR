using System.Threading.Tasks;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainInterfaces
{
    public interface IAnonymousMessageGrain : IGrainWithStringKey
    {
        Task AcceptMessageAsync(AnonymousMessage message, GrainCancellationToken cancellationToken);
        Task SubscribeToMessages(IAnonymousMessageObserver observer, MessageHandle since = default);
        Task UnsubscribeFromMessages(IAnonymousMessageObserver observer);

    }

    public interface IAnonymousMessageObserver : IGrainObserver
    {
        void ReceiveMessage(AnonymousMessage message, MessageHandle handle);
        void SubscriptionEnded();
    }
}
