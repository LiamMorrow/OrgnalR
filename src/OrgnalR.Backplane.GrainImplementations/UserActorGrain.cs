using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core;
using OrgnalR.Core.Provider;
using Orleans;
using Orleans.Providers;

namespace OrgnalR.Backplane.GrainImplementations
{
    [StorageProvider(ProviderName = Constants.USER_STORAGE_PROVIDER)]
    public class UserActorGrain : Grain<UserActorGrainState>, IUserActorGrain
    {
        private bool dirty = false;

        public override Task OnActivateAsync()
        {
            RegisterTimer(WriteStateIfDirty, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            return base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await WriteStateIfDirty(null);
            await base.OnDeactivateAsync();
        }

        private Task WriteStateIfDirty(object? _)
        {
            if (dirty) return Task.CompletedTask;
            return WriteStateAsync();
        }

        public Task AcceptMessageAsync(AnonymousMessage message, GrainCancellationToken cancellationToken)
        {
            return Task.WhenAll(
                 State.ConnectionIds
                 .Where(connId => !message.Excluding.Contains(connId))
                 .Select(connId => GrainFactory.GetGrain<IClientGrain>(connId))
                 .Select(client => client.AcceptMessageAsync(message.Payload, cancellationToken))
             ).WithCancellation(cancellationToken.CancellationToken);
        }

        public Task AddToUserAsync(string connectionId, GrainCancellationToken cancellationToken)
        {
            dirty = State.ConnectionIds.Add(connectionId) || dirty;
            return Task.CompletedTask;
        }

        public Task RemoveFromUserAsync(string connectionId, GrainCancellationToken cancellationToken)
        {
            dirty = State.ConnectionIds.Remove(connectionId) || dirty;
            return Task.CompletedTask;
        }

    }

    public class UserActorGrainState
    {
        public ISet<string> ConnectionIds { get; set; } = new HashSet<string>();
    }
}
