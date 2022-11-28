using System;
using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
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

        private IRewindableMessageGrain<MethodMessage> rewoundMessagesGrain = null!;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            rewoundMessagesGrain = GrainFactory.GetGrain<IRewindableMessageGrain<MethodMessage>>(this.GetPrimaryKeyString());
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            foreach (var observer in observers)
            {
                observer.SubscriptionEnded();
            }
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task AcceptMessageAsync(MethodMessage message, GrainCancellationToken cancellationToken)
        {
            var handle = await rewoundMessagesGrain.PushMessageAsync(message);
            observers.Notify(x => x.ReceiveMessage(message, handle));
        }

        public async Task SubscribeToMessages(IClientMessageObserver observer, MessageHandle since)
        {
            observers.Subscribe(observer);
            if (since != default)
            {
                foreach (var (message, handle) in await rewoundMessagesGrain.GetMessagesSinceAsync(since))
                {
                    observer.ReceiveMessage(message, handle);
                }
            }
        }

        public Task UnsubscribeFromMessages(IClientMessageObserver observer)
        {
            observers.Unsubscribe(observer);
            return Task.CompletedTask;
        }

    }
}
