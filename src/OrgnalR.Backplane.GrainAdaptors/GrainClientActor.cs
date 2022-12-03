using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core;
using OrgnalR.Core.Provider;
using OrgnalR.Core.State;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class GrainClientActor : IMessageAcceptor
    {
        private readonly string hubName;
        private readonly IClientGrain clientGrain;

        public GrainClientActor(string hubName, IClientGrain clientGrain)
        {
            this.hubName = hubName;
            this.clientGrain = clientGrain;
        }

        public Task AcceptMessageAsync(
            AnonymousMessage message,
            CancellationToken cancellationToken = default
        )
        {
            message = new AnonymousMessage(
                message.Excluding.Select(x => $"{hubName}::{x}").ToSet(),
                message.Payload
            );
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }

            return clientGrain.AcceptMessageAsync(message.Payload, token.Token);
        }
    }
}
