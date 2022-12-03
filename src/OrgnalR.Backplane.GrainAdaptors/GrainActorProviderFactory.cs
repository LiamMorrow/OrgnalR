using OrgnalR.Core.Provider;
using OrgnalR.Core.State;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors;

public class GrainActorProviderFactory : IActorProviderFactory
{
    private readonly IGrainFactoryProvider grainFactoryProvider;

    public GrainActorProviderFactory(IGrainFactoryProvider grainFactoryProvider)
    {
        this.grainFactoryProvider = grainFactoryProvider;
    }

    public IMessageAcceptor GetAllActor(string hubName)
    {
        return new GrainActorProvider(
            hubName,
            grainFactoryProvider.GetGrainFactory()
        ).GetAllActor();
    }

    public IMessageAcceptor GetClientActor(string hubName, string connectionId)
    {
        return new GrainActorProvider(
            hubName,
            grainFactoryProvider.GetGrainFactory()
        ).GetClientActor(connectionId);
    }

    public IGroupActor GetGroupActor(string hubName, string groupName)
    {
        return new GrainActorProvider(
            hubName,
            grainFactoryProvider.GetGrainFactory()
        ).GetGroupActor(groupName);
    }

    public IUserActor GetUserActor(string hubName, string userId)
    {
        return new GrainActorProvider(hubName, grainFactoryProvider.GetGrainFactory()).GetUserActor(
            userId
        );
    }
}
