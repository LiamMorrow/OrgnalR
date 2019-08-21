using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using OrgnalR.Core.State;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class GrainUserActor : IUserActor
    {
        private readonly IUserActorGrain userActorGrain;

        public GrainUserActor(IUserActorGrain userActorGrain)
        {
            this.userActorGrain = userActorGrain;
        }

        public Task AcceptMessageAsync(AnonymousMessage targetedMessage, CancellationToken cancellationToken = default)
        {
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }

            return userActorGrain.AcceptMessageAsync(targetedMessage, token.Token);
        }

        public Task AddToUserAsync(string connectionId, CancellationToken cancellationToken = default)
        {
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }

            return userActorGrain.AddToUserAsync(connectionId, token.Token);
        }

        public Task RemoveFromUserAsync(string connectionId, CancellationToken cancellationToken = default)
        {
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }

            return userActorGrain.RemoveFromUserAsync(connectionId, token.Token);
        }
    }
}