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
        private readonly IGroupActorProvider groupActorProvider;
        private readonly IUserActorProvider userActorProvider;
        private readonly IMessageObservable messageObservable;
        private readonly IMessageObserver messageObserver;
        private SubscriptionHandle? allSubscriptionHandle;

        private OrleansHubLifetimeManager(
            IGroupActorProvider groupActorProvider,
            IUserActorProvider userActorProvider,
            IMessageObservable messageObservable,
            IMessageObserver messageObserver
            )
        {
            this.groupActorProvider = groupActorProvider ?? throw new ArgumentNullException(nameof(groupActorProvider));
            this.userActorProvider = userActorProvider ?? throw new ArgumentNullException(nameof(userActorProvider));
            this.messageObservable = messageObservable ?? throw new ArgumentNullException(nameof(messageObservable));
            this.messageObserver = messageObserver ?? throw new ArgumentNullException(nameof(messageObserver));
        }

        public static async Task<OrleansHubLifetimeManager<THub>> CreateAsync(
            IGroupActorProvider groupActorProvider,
            IUserActorProvider userActorProvider,
            IMessageObservable messageObservable,
            IMessageObserver messageObserver,
            CancellationToken cancellationToken = default
            )
        {
            var manager = new OrleansHubLifetimeManager<THub>(groupActorProvider, userActorProvider, messageObservable, messageObserver);
            manager.allSubscriptionHandle = await messageObservable.SubscribeToAllAsync(manager.OnAnonymousMessageReceived, cancellationToken);
            return manager;
        }

        public override async Task OnConnectedAsync(HubConnectionContext connection)
        {
            hubConnectionStore.Add(connection);
            if (connection.UserIdentifier != null)
            {
                await userActorProvider.GetUserActor(connection.UserIdentifier)
                    .AddToUserAsync(connection.ConnectionId);
            }
            await messageObservable.SubscribeToConnectionAsync(connection.ConnectionId, OnAddressedMessageReceived);
        }

        public override async Task OnDisconnectedAsync(HubConnectionContext connection)
        {
            hubConnectionStore.Remove(connection);
            if (connection.UserIdentifier != null)
            {
                await userActorProvider.GetUserActor(connection.UserIdentifier)
                    .RemoveFromUserAsync(connection.ConnectionId);
            }
            await messageObservable.UnsubscribeFromSpecificAsync(connection.ConnectionId);
        }

        public override Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            var group = groupActorProvider.GetGroupActor(groupName);
            return group.AddToGroupAsync(connectionId, cancellationToken);
        }

        public override Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            var group = groupActorProvider.GetGroupActor(groupName);
            return group.RemoveFromGroupAsync(connectionId, cancellationToken);
        }

        public override Task SendAllAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            return messageObserver.SendAllMessageAsync(new AnonymousMessage(EmptySet<string>.Instance, new InvocationMessage(methodName, args)), cancellationToken);
        }

        public override Task SendAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
        {
            return messageObserver.SendAllMessageAsync(new AnonymousMessage(excludedConnectionIds.ToSet(), new InvocationMessage(methodName, args)), cancellationToken);
        }

        public override Task SendConnectionAsync(string connectionId, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            return SendConnectionsAsync(new SingletonList<string>(connectionId), methodName, args, cancellationToken);
        }

        public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            var toAwait = new List<Task>();
            foreach (var connectionId in connectionIds)
            {
                var local = hubConnectionStore[connectionId];
                var msg = new AddressedMessage(connectionId, new InvocationMessage(methodName, args));
                if (local != null)
                {
                    toAwait.Add(OnAddressedMessageReceived(msg));
                }
                else
                {
                    toAwait.Add(messageObserver.SendAddressedMessageAsync(msg));
                }
            }
            return Task.WhenAll(toAwait);
        }

        public override Task SendGroupAsync(string groupName, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            var group = groupActorProvider.GetGroupActor(groupName);
            return group.AcceptMessageAsync(new AnonymousMessage(EmptySet<string>.Instance, new InvocationMessage(methodName, args)), cancellationToken);
        }

        public override Task SendGroupExceptAsync(string groupName, string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
        {
            var group = groupActorProvider.GetGroupActor(groupName);
            return group.AcceptMessageAsync(new AnonymousMessage(excludedConnectionIds.ToSet(), new InvocationMessage(methodName, args)), cancellationToken);
        }

        public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                groupNames.Select(group => SendGroupAsync(group, methodName, args, cancellationToken))
            );
        }

        public override Task SendUserAsync(string userId, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            var user = userActorProvider.GetUserActor(userId);
            return user.AcceptMessageAsync(new AnonymousMessage(EmptySet<string>.Instance, new InvocationMessage(methodName, args)), cancellationToken);
        }

        public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                userIds.Select(userId => SendUserAsync(userId, methodName, args, cancellationToken))
            );
        }

        private Task OnAddressedMessageReceived(AddressedMessage arg)
        {
            var conn = hubConnectionStore[arg.ConnectionId];
            if (conn == null) return Task.CompletedTask;
            if (conn.ConnectionAborted.IsCancellationRequested) return Task.CompletedTask;
            return conn.WriteAsync(arg.Payload).AsTask();
        }

        private Task OnAnonymousMessageReceived(AnonymousMessage arg)
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
