using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Protocol;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainInterfaces
{
    public interface IClientGrain : IGrainWithStringKey
    {
        Task AcceptMessageAsync(HubInvocationMessage message, GrainCancellationToken cancellationToken);
        Task SubscribeToMessages(IClientMessageObserver observer, MessageHandle since = default);
        Task UnsubscribeFromMessages(IClientMessageObserver observer);
    }

    public interface IClientMessageObserver : IGrainObserver
    {
        void ReceiveMessage(HubInvocationMessage message, MessageHandle handle);
        void SubscriptionEnded();
    }
}
