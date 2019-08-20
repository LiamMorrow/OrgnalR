using OrgnalR.Core.State;

namespace OrgnalR.Core.Provider
{
    public interface IGroupActorProvider
    {
        IGroupActor GetGroupActor(string groupId);
    }
}