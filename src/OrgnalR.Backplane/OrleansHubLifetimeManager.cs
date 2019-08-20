using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using OrgnalR.Core;
using OrgnalR.Core.Data;
using OrgnalR.Core.Provider;

namespace OrgnalR.Backplane
{
    public class OrleansHubLifetimeManager<THub> : HubLifetimeManager<THub>, IDisposable where THub : Hub
    {
        private bool disposed;
        private readonly HubConnectionStore hubConnectionStore = new HubConnectionStore();
        private readonly IGroupActorProvider groupContainerProvider;
        private readonly IMessageObservable messageObservable;
        private readonly IMessageObserver messageObserver;
        private SubscriptionHandle? allSubscriptionHandle;

        private OrleansHubLifetimeManager(
            IGroupActorProvider groupContainerProvider,
            IMessageObservable messageObservable,
            IMessageObserver messageObserver
            )
        {
            this.groupContainerProvider = groupContainerProvider ?? throw new ArgumentNullException(nameof(groupContainerProvider));
            this.messageObservable = messageObservable ?? throw new ArgumentNullException(nameof(messageObservable));
            this.messageObserver = messageObserver ?? throw new ArgumentNullException(nameof(messageObserver));
        }

        public static async Task<OrleansHubLifetimeManager<THub>> CreateAsync(
            IGroupActorProvider groupContainerProvider,
            IMessageObservable messageObservable,
            IMessageObserver messageObserver,
            CancellationToken cancellationToken = default
            )
        {
            var manager = new OrleansHubLifetimeManager<THub>(groupContainerProvider, messageObservable, messageObserver);
            manager.allSubscriptionHandle = await messageObservable.SubscribeToAllAsync(manager.OnAllMessageReceived, cancellationToken);
            return manager;
        }

        public override Task OnConnectedAsync(HubConnectionContext connection)
        {
            hubConnectionStore.Add(connection);
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(HubConnectionContext connection)
        {
            hubConnectionStore.Remove(connection);
            return Task.CompletedTask;
        }

        public override Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            var group = groupContainerProvider.GetGroupActor(groupName);
            return group.AddToGroupAsync(connectionId, cancellationToken);
        }

        public override Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            var group = groupContainerProvider.GetGroupActor(groupName);
            return group.RemoveFromGroupAsync(connectionId, cancellationToken);
        }

        public override Task SendAllAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            return messageObserver.SendAllMessageAsync(new AllMessage(EmptySet<string>.Instance, new InvocationMessage(methodName, args)), cancellationToken);
        }

        public override Task SendAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task SendConnectionAsync(string connectionId, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task SendGroupAsync(string groupName, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task SendGroupExceptAsync(string groupName, string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task SendUserAsync(string userId, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private Task OnAllMessageReceived(AllMessage arg)
        {
            var toAwait = new List<ValueTask>();
            foreach (var conn in hubConnectionStore)
            {
                if (arg.Excluding.Contains(conn.ConnectionId)) continue;
                if (conn.ConnectionAborted.IsCancellationRequested) continue;
                toAwait.Add(conn.WriteAsync(arg.Payload));
            }
            return Task.WhenAll(toAwait.Where(vt => !vt.IsCompleted).Select(vt => vt.AsTask()));
        }

        public async ValueTask DisposeAsync()
        {
            if (disposed) return;
            if (allSubscriptionHandle != null)
            {
                await messageObservable.UnsubscribeFromAllAsync(allSubscriptionHandle);
                allSubscriptionHandle = null;
            }
            disposed = true;
        }

        [Obsolete("Use DisposeAsync instead")]
        public void Dispose()
        {
            if (disposed) return;
            DisposeAsync().AsTask().Wait();
        }
    }
}
