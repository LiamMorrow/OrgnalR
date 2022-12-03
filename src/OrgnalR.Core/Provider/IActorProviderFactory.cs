using OrgnalR.Core.State;

namespace OrgnalR.Core.Provider;

public interface IActorProviderFactory
{
    IMessageAcceptor GetAllActor(string hubName);
    IMessageAcceptor GetClientActor(string hubName, string connectionId);
    IGroupActor GetGroupActor(string hubName, string groupName);
    IUserActor GetUserActor(string hubName, string userId);
}
