using OrgnalR.Core.State;

namespace OrgnalR.Core.Provider
{
    public interface IUserActorProvider
    {
        IUserActor GetUserActor(string userId);
    }
}
