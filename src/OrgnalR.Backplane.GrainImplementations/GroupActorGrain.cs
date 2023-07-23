using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core;
using OrgnalR.Core.Provider;
using Orleans;
using Orleans.Providers;

namespace OrgnalR.Backplane.GrainImplementations
{
    [StorageProvider(ProviderName = Constants.GROUP_STORAGE_PROVIDER)]
    public class GroupActorGrain : Grain<GroupActorGrainState>, IGroupActorGrain
    {
        private bool dirty = false;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            RegisterTimer(
                WriteStateIfDirty,
                string.Empty, // state is not used
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30)
            );
            return base.OnActivateAsync(cancellationToken);
        }

        public override async Task OnDeactivateAsync(
            DeactivationReason reason,
            CancellationToken cancellationToken
        )
        {
            await WriteStateIfDirty(null);
            await base.OnDeactivateAsync(reason, cancellationToken);
        }

        private Task WriteStateIfDirty(object? _)
        {
            if (dirty)
                return Task.CompletedTask;
            return WriteStateAsync();
        }

        public Task AcceptMessageAsync(
            AnonymousMessage message,
            GrainCancellationToken cancellationToken
        )
        {
            return Task.WhenAll(
                    State.ConnectionIds
                        .Where(connId => !message.Excluding.Contains(connId))
                        .Select(connId => GrainFactory.GetGrain<IClientGrain>(connId))
                        .Select(
                            client => client.AcceptMessageAsync(message.Payload, cancellationToken)
                        )
                )
                .WithCancellation(cancellationToken.CancellationToken);
        }

        public Task AddToGroupAsync(string connectionId, GrainCancellationToken cancellationToken)
        {
            dirty = State.ConnectionIds.Add(connectionId) || dirty;
            return Task.CompletedTask;
        }

        public Task RemoveFromGroupAsync(
            string connectionId,
            GrainCancellationToken cancellationToken
        )
        {
            dirty = State.ConnectionIds.Remove(connectionId) || dirty;
            return Task.CompletedTask;
        }
    }

    [GenerateSerializer]
    public class GroupActorGrainState
    {
        [Id(0)]
        public ISet<string> ConnectionIds { get; set; } = new HashSet<string>();
    }
}
