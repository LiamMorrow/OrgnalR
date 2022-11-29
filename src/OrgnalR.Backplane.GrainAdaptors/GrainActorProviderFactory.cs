using OrgnalR.Core.Provider;
using OrgnalR.Core.State;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors;


public class GrainActorProviderFactory : IActorProviderFactory
{
    private readonly IGrainFactory grainFactory;

    public GrainActorProviderFactory(IGrainFactory grainFactory)
    {
        this.grainFactory = grainFactory;
    }

    public IGroupActor GetGroupActor(string hubName, string groupName)
    {
        return new GrainActorProvider(hubName, grainFactory).GetGroupActor(groupName);
    }

    public IUserActor GetUserActor(string hubName, string userId)
    {
        return new GrainActorProvider(hubName, grainFactory).GetUserActor(userId);
    }
}
