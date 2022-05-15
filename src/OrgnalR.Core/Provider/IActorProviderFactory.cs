using OrgnalR.Core.State;

namespace OrgnalR.Core.Provider;

public interface IActorProviderFactory
{
    IGroupActor GetGroupActor(string hubName, string groupName);
    IUserActor GetUserActor(string hubName, string userId);
}
