using System.Collections.Generic;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainImplementations
{
    public class AnonymousMessageGrain : Grain, IAnonymousMessageGrain
    {
        private readonly HashSet<IAnonymousMessageObserver> observers = new HashSet<IAnonymousMessageObserver>();

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
            foreach (var observer in observers)
            {
                cancellationToken.CancellationToken.ThrowIfCancellationRequested();
                observer.ReceiveMessage(message);
            }
            return Task.CompletedTask;
        }

        public Task SubscribeToMessages(IAnonymousMessageObserver observer)
        {
            observers.Add(observer);
            return Task.CompletedTask;
        }

        public Task UnsubscribeFromMessages(IAnonymousMessageObserver observer)
        {
            observers.Remove(observer);
            return Task.CompletedTask;
        }
    }
}