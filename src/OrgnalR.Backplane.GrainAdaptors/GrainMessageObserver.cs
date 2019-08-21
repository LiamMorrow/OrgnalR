using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using OrgnalR.Core;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class GrainMessageObserver : IMessageObserver
    {
        private readonly string hubName;
        private readonly IGrainFactory grainFactory;

        public GrainMessageObserver(
            string hubName,
            IGrainFactory grainFactory
        )
        {
            this.hubName = hubName ?? throw new System.ArgumentNullException(nameof(hubName));
            this.grainFactory = grainFactory ?? throw new System.ArgumentNullException(nameof(grainFactory));
        }

        public Task SendAddressedMessageAsync(AddressedMessage msg, CancellationToken cancellationToken = default)
        {
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }
            return grainFactory.GetGrain<IClientGrain>($"{hubName}::{msg.ConnectionId}").AcceptMessageAsync(msg.Payload, token.Token);
        }

        public Task SendAllMessageAsync(AnonymousMessage allMessage, CancellationToken cancellationToken = default)
        {
            var token = new GrainCancellationTokenSource();
            if (cancellationToken != default)
            {
                cancellationToken.Register(() => token.Cancel());
            }
            var hubNamespacesAllMessage = new AnonymousMessage(
                allMessage.Excluding.Select(id => $"{hubName}::{id}").ToSet(),
                allMessage.Payload
                );
            return grainFactory.GetGrain<IAnonymousMessageGrain>(hubName).AcceptMessageAsync(allMessage, token.Token);
        }
    }
}