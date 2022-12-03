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
    public class GrainAllActor : IMessageAcceptor
    {
        private readonly string hubName;
        private readonly IAnonymousMessageGrain anonymousMessageGrain;

        public GrainAllActor(string hubName, IAnonymousMessageGrain anonymousMessageGrain)
        {
            this.hubName = hubName;
            this.anonymousMessageGrain = anonymousMessageGrain;
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

            return anonymousMessageGrain.AcceptMessageAsync(message, token.Token);
        }
    }
}
