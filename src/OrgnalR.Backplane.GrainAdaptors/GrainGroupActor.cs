using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using OrgnalR.Core.State;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class GrainGroupActor : IGroupActor
    {
        private readonly IGroupActorGrain groupActorGrain;

        public GrainGroupActor(IGroupActorGrain groupActorGrain)
        {
            this.groupActorGrain = groupActorGrain;
        }

        public Task AcceptMessageAsync(AnonymousMessage targetedMessage, CancellationToken cancellationToken = default)
        {
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }

            return groupActorGrain.AcceptMessageAsync(targetedMessage, token.Token);
        }

        public Task AddToGroupAsync(string connectionId, CancellationToken cancellationToken = default)
        {
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }

            return groupActorGrain.AddToGroupAsync(connectionId, token.Token);
        }

        public Task RemoveFromGroupAsync(string connectionId, CancellationToken cancellationToken = default)
        {
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }

            return groupActorGrain.RemoveFromGroupAsync(connectionId, token.Token);
        }
    }
}