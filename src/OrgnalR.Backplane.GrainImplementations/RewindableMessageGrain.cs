using System.Linq;
using System;
using System.Collections.Generic;
using OrgnalR.Backplane.GrainInterfaces;
using System.Threading.Tasks;
using Orleans;
using OrgnalR.Core;
using Orleans.Providers;
using OrgnalR.Core.Provider;

namespace OrgnalR.Backplane.GrainImplementations
{
    [StorageProvider(ProviderName = Constants.MESSAGE_STORAGE_PROVIDER)]
    public class RewindableMessageGrain<T> : Grain<RewindableMessageGrainState<T>>, IRewindableMessageGrain<T>
    {

        private long maxMessages;
        private long OldestMessageId => Math.Max(0, LatestMessageId - maxMessages);
        private long LatestMessageId => State.LatestMessageId;
        private Dictionary<long, RewindableMessageWrapper<T>> Messages => State.Messages;
        private bool dirty = false;


        public override Task OnActivateAsync()
        {
            var config = (OrgnalRSiloConfig?)ServiceProvider.GetService(typeof(OrgnalRSiloConfig));
            maxMessages = config?.MaxMessageRewind ?? 0;
            if (State == null)
            {
                State = new RewindableMessageGrainState<T>
                {
                    MessageGroup = Guid.NewGuid()
                };
            }
            RegisterTimer(WriteStateIfDirtyAsync, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            return base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await WriteStateIfDirtyAsync(null);
            await base.OnDeactivateAsync();
        }

        public Task<List<(T message, MessageHandle handle)>> GetMessagesSinceAsync(MessageHandle handle)
        {
            var messageIdExclusive = handle.MessageId;
            if (messageIdExclusive >= LatestMessageId || handle.MessageGroup != State.MessageGroup)
            {
                // It's possible we have been recreated on a new silo (and thus restarted our counter), so we simply return an empty value
                // This could happen if we use the in memory grain storage.  And if we are, then we do not need to be reliable
                return Task.FromResult(new List<(T, MessageHandle)>());
            }
            if (OldestMessageId > messageIdExclusive - 1)
            {
                throw new ArgumentOutOfRangeException($"Oldest message is: {OldestMessageId}");
            }
            return Task.FromResult(
                Util.LongRange(messageIdExclusive + 1, LatestMessageId - messageIdExclusive)
                .Select(id => (Messages[id].Message, new MessageHandle(id, State.MessageGroup)))
                .ToList()
            );
        }

        public Task<MessageHandle> PushMessageAsync(T message)
        {
            State.LatestMessageId++;
            Messages[LatestMessageId] = new RewindableMessageWrapper<T>
            {
                Message = message,
                SentAt = DateTimeOffset.UtcNow
            };
            Messages.Remove(LatestMessageId - maxMessages);
            dirty = true;
            return Task.FromResult(new MessageHandle(LatestMessageId, State.MessageGroup));
        }

        private Task WriteStateIfDirtyAsync(object? _)
        {
            if (!dirty)
            {
                return Task.CompletedTask;
            }
            return WriteStateAsync();
        }
    }
    public class RewindableMessageGrainState<T>
    {
        public Guid MessageGroup { get; set; }
        public long LatestMessageId { get; set; }
        public Dictionary<long, RewindableMessageWrapper<T>> Messages { get; set; } = new Dictionary<long, RewindableMessageWrapper<T>>();
    }

    public class RewindableMessageWrapper<T>
    {
        public DateTimeOffset SentAt { get; set; }
        public T Message { get; set; } = default!;
    }
}
