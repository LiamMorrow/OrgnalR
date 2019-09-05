using System.Collections.Generic;
using System.Threading.Tasks;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainInterfaces
{
    public interface IRewindableMessageGrain<T> : IGrainWithStringKey
    {
        /// <summary>
        /// Will get all messages since <paramref name="lastHandle"/>, exclusive
        /// </summary>
        /// <param name="lastHandle">The handle to get messages since, exclusive</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the message buffer does not go back as far as the requested message</exception>
        Task<List<(T message, MessageHandle handle)>> GetMessagesSinceAsync(MessageHandle lastHandle);
        Task<MessageHandle> PushMessageAsync(T message);
    }
}
