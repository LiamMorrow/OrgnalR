using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OrgnalR.Core.Provider;

namespace OrgnalR.Backplane
{
    public class OrleansHubLifetimeManager<THub> : HubLifetimeManager<THub> where THub : Hub
    {
        private readonly IGroupActorProvider groupContainerProvider;

        public OrleansHubLifetimeManager(IGroupActorProvider groupContainerProvider)
        {
            this.groupContainerProvider = groupContainerProvider ?? throw new ArgumentNullException(nameof(groupContainerProvider));
        }


        public override Task OnConnectedAsync(HubConnectionContext connection)
        {
            throw new NotImplementedException();
        }

        public override Task OnDisconnectedAsync(HubConnectionContext connection)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
    }
}
