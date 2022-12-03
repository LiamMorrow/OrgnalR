using System;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using OrgnalR.Core.State;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class GrainActorProvider
    {
        private readonly string hubName;
        private readonly IGrainFactory grainFactory;

        public GrainActorProvider(string hubName, IGrainFactory grainFactory)
        {
            this.hubName = hubName ?? throw new ArgumentNullException(nameof(hubName));
            this.grainFactory =
                grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        }

        public IMessageAcceptor GetAllActor()
        {
            return new GrainAllActor(
                hubName,
                grainFactory.GetGrain<IAnonymousMessageGrain>(hubName)
            );
        }

        public IMessageAcceptor GetClientActor(string connectionId)
        {
            return new GrainClientActor(
                hubName,
                grainFactory.GetGrain<IClientGrain>($"{hubName}::{connectionId}")
            );
        }

        public IGroupActor GetGroupActor(string groupName)
        {
            return new GrainGroupActor(
                hubName,
                grainFactory.GetGrain<IGroupActorGrain>($"{hubName}::{groupName}")
            );
        }

        public IUserActor GetUserActor(string userId)
        {
            return new GrainUserActor(
                hubName,
                grainFactory.GetGrain<IUserActorGrain>($"{hubName}::{userId}")
            );
        }
    }
}
