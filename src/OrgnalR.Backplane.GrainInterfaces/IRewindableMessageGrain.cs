using System.Collections.Generic;
using System.Threading.Tasks;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainInterfaces
{
    public interface IRewindableMessageGrain<T> : IGrainWithStringKey
    {

        Task<List<(T message, MessageHandle handle)>> GetMessagesSinceAsync(MessageHandle lastHandle);
        Task<MessageHandle> PushMessageAsync(T message);
    }
}
