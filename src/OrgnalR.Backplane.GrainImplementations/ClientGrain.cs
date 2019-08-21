using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Protocol;
using OrgnalR.Backplane.GrainInterfaces;
using Orleans;

namespace OrgnalR.Backplane.GrainImplementations
{
    public class ClientGrain : Grain, IClientGrain
    {
        private readonly HashSet<IClientMessageObserver> observers = new HashSet<IClientMessageObserver>();

        public override Task OnDeactivateAsync()
        {
            foreach (var observer in observers)
            {
                observer.SubscriptionEnded();
            }
            return base.OnDeactivateAsync();
        }

        public Task AcceptMessageAsync(HubInvocationMessage message, GrainCancellationToken cancellationToken)
        {
            foreach (var observer in observers)
            {
                cancellationToken.CancellationToken.ThrowIfCancellationRequested();
                observer.ReceiveMessage(message);
            }
            return Task.CompletedTask;
        }

        public Task SubscribeToMessages(IClientMessageObserver observer)
        {
            observers.Add(observer);
            return Task.CompletedTask;
        }

        public Task UnsubscribeFromMessages(IClientMessageObserver observer)
        {
            observers.Remove(observer);
            return Task.CompletedTask;
        }

    }
}