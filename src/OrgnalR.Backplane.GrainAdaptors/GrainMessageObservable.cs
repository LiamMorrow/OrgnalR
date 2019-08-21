using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class GrainMessageObservable : IMessageObservable
    {
        private readonly string hubName;
        private readonly IGrainFactory grainFactory;
        private readonly ConcurrentDictionary<Guid, (IAnonymousMessageObserver raw, IAnonymousMessageObserver obj)> anonymousObservers
        = new ConcurrentDictionary<Guid, (IAnonymousMessageObserver raw, IAnonymousMessageObserver obj)>();
        private readonly ConcurrentDictionary<string, (IClientMessageObserver raw, IClientMessageObserver obj)> clientObservers
        = new ConcurrentDictionary<string, (IClientMessageObserver raw, IClientMessageObserver obj)>();

        public GrainMessageObservable(
            string hubName,
            IGrainFactory grainFactory
        )
        {
            this.hubName = hubName ?? throw new ArgumentNullException(nameof(hubName));
            this.grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        }
        public async Task<SubscriptionHandle> SubscribeToAllAsync(Func<AnonymousMessage, Task> messageCallback, Func<SubscriptionHandle, Task> onSubscriptionEnd, CancellationToken cancellationToken = default)
        {
            var handle = new SubscriptionHandle(Guid.NewGuid());
            var handler = new DelegateMessageGrainMessageObserver(handle, messageCallback, onSubscriptionEnd);
            var handlerRef = await grainFactory.CreateObjectReference<IAnonymousMessageObserver>(handler).ConfigureAwait(false);
            anonymousObservers[handle.SubscriptionId] = (handler, handlerRef);
            var messageGrain = grainFactory.GetGrain<IAnonymousMessageGrain>(hubName);
            await messageGrain.SubscribeToMessages(handlerRef).ConfigureAwait(false);
            return handle;
        }

        public async Task UnsubscribeFromAllAsync(SubscriptionHandle subscriptionHandle, CancellationToken cancellationToken = default)
        {
            if (!anonymousObservers.TryRemove(subscriptionHandle.SubscriptionId, out var handler))
            {
                return;
            }
            var messageGrain = grainFactory.GetGrain<IAnonymousMessageGrain>(hubName);
            await messageGrain.UnsubscribeFromMessages(handler.obj).ConfigureAwait(false);
        }

        public async Task SubscribeToConnectionAsync(string connectionId, Func<AddressedMessage, Task> messageCallback, Func<string, Task> onSubscriptionEnd, CancellationToken cancellationToken = default)
        {
            var handler = new DelegateClientGrainMessageObserver(connectionId, messageCallback, onSubscriptionEnd);
            var handlerRef = await grainFactory.CreateObjectReference<IClientMessageObserver>(handler).ConfigureAwait(false);
            clientObservers[connectionId] = (handler, handlerRef);
            var messageGrain = grainFactory.GetGrain<IClientGrain>($"{hubName}::{connectionId}");
            await messageGrain.SubscribeToMessages(handlerRef).ConfigureAwait(false);
        }

        public async Task UnsubscribeFromSpecificAsync(string connectionId, CancellationToken cancellationToken = default)
        {
            if (!clientObservers.TryRemove(connectionId, out var handler))
            {
                return;
            }
            var messageGrain = grainFactory.GetGrain<IClientGrain>($"{hubName}::{connectionId}");
            await messageGrain.UnsubscribeFromMessages(handler.obj).ConfigureAwait(false);
        }

    }
}