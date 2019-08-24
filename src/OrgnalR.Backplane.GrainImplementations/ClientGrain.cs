using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Protocol;
using OrgnalR.Backplane.GrainInterfaces;
using Orleans;

namespace OrgnalR.Backplane.GrainImplementations
{
    public class ClientGrain : Grain, IClientGrain
    {
        private readonly GrainObserverManager<IClientMessageObserver> observers = new GrainObserverManager<IClientMessageObserver>
        {
            ExpirationDuration = TimeSpan.FromMinutes(5),
            OnFailBeforeDefunct = x => x.SubscriptionEnded()
        };

        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();

        }

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
            observers.Notify(x => x.ReceiveMessage(message));
            return Task.CompletedTask;
        }

        public Task SubscribeToMessages(IClientMessageObserver observer)
        {
            observers.Subscribe(observer);
            return Task.CompletedTask;
        }

        public Task UnsubscribeFromMessages(IClientMessageObserver observer)
        {
            observers.Unsubscribe(observer);
            return Task.CompletedTask;
        }

    }
}