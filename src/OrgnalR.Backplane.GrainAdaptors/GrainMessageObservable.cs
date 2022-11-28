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
        public async Task<SubscriptionHandle> SubscribeToAllAsync(
            Func<AnonymousMessage, MessageHandle, Task> messageCallback,
            Func<SubscriptionHandle, Task> onSubscriptionEnd,
            MessageHandle since = default,
            CancellationToken cancellationToken = default
        )
        {
            var handle = new SubscriptionHandle(Guid.NewGuid());
            var handler = new DelegateAnonymousMessageObserver(handle, messageCallback, onSubscriptionEnd);
            var handlerRef = await Task.FromResult(grainFactory.CreateObjectReference<IAnonymousMessageObserver>(handler)).ConfigureAwait(false);
            anonymousObservers[handle.SubscriptionId] = (handler, handlerRef);
            var messageGrain = grainFactory.GetGrain<IAnonymousMessageGrain>(hubName);
            await messageGrain.SubscribeToMessages(handlerRef, since).ConfigureAwait(false);
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

        public async Task SubscribeToConnectionAsync(
            string connectionId,
            Func<AddressedMessage, MessageHandle, Task> messageCallback,
            Func<string, Task> onSubscriptionEnd,
            MessageHandle since = default,
            CancellationToken cancellationToken = default)
        {
            var handler = new DelegateClientMessageObserver(connectionId, messageCallback, onSubscriptionEnd);
            var handlerRef = await Task.FromResult(grainFactory.CreateObjectReference<IClientMessageObserver>(handler)).ConfigureAwait(false);
            clientObservers[connectionId] = (handler, handlerRef);
            var messageGrain = grainFactory.GetGrain<IClientGrain>($"{hubName}::{connectionId}");
            await messageGrain.SubscribeToMessages(handlerRef, since).ConfigureAwait(false);
        }

        public async Task UnsubscribeFromConnectionAsync(string connectionId, CancellationToken cancellationToken = default)
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
