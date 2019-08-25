using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainImplementations
{
    public class AnonymousMessageGrain : Grain, IAnonymousMessageGrain
    {
        private readonly GrainObserverManager<IAnonymousMessageObserver> observers = new GrainObserverManager<IAnonymousMessageObserver>
        {
            ExpirationDuration = TimeSpan.FromMinutes(5),
            OnFailBeforeDefunct = x => x.SubscriptionEnded()
        };

        public override Task OnDeactivateAsync()
        {
            foreach (var observer in observers)
            {
                observer.SubscriptionEnded();
            }
            return base.OnDeactivateAsync();
        }

        public Task AcceptMessageAsync(AnonymousMessage message, GrainCancellationToken cancellationToken)
        {
            observers.Notify(x => x.ReceiveMessage(message));
            return Task.CompletedTask;
        }

        public Task SubscribeToMessages(IAnonymousMessageObserver observer)
        {
            observers.Subscribe(observer);
            return Task.CompletedTask;
        }

        public Task UnsubscribeFromMessages(IAnonymousMessageObserver observer)
        {
            observers.Unsubscribe(observer);
            return Task.CompletedTask;
        }
    }
}
