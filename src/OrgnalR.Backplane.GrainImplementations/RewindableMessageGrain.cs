using System.Linq;
using System;
using System.Collections.Generic;
using OrgnalR.Backplane.GrainInterfaces;
using System.Threading.Tasks;
using Orleans;
using OrgnalR.Core;
using Orleans.Providers;
using OrgnalR.Core.Provider;
using OrgnalR.Core.Data;

namespace OrgnalR.Backplane.GrainImplementations
{
    [StorageProvider(ProviderName = Constants.MESSAGE_STORAGE_PROVIDER)]
    public class RewindableMessageGrain<T> : Grain<RewindableMessageGrainState<T>>, IRewindableMessageGrain<T>
    {

        private int maxMessages;
        private CircularBuffer<RewindableMessageWrapper<T>> messageBuffer = null!;
        private long OldestMessageId => messageBuffer.Front().MessageId;
        private long LatestMessageId => State.LatestMessageId;
        private bool dirty = false;


        public override Task OnActivateAsync()
        {
            var config = (OrgnalRSiloConfig?)ServiceProvider?.GetService(typeof(OrgnalRSiloConfig));
            maxMessages = config?.MaxMessageRewind ?? 0;
            if (State == null || State.Messages == null)
            {
                State = new RewindableMessageGrainState<T>
                {
                    MessageGroup = Guid.NewGuid(),
                    Messages = new RewindableMessageWrapper<T>[0]
                };
            }
            messageBuffer = new CircularBuffer<RewindableMessageWrapper<T>>(maxMessages, State.Messages);
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
            // If the oldest message is for example 2, and we want all messages since 1, we can still service that, so we add 1
            if (OldestMessageId > messageIdExclusive + 1)
            {
                throw new ArgumentOutOfRangeException($"Oldest message is: {OldestMessageId}");
            }
            var messages = messageBuffer.SkipWhile(x => x.MessageId <= messageIdExclusive).ToList();
            return Task.FromResult(
                messages
                    .Select(msg => (msg.Message, new MessageHandle(msg.MessageId, State.MessageGroup)))
                    .ToList()
            );
        }

        public Task<MessageHandle> PushMessageAsync(T message)
        {
            State.LatestMessageId++;
            messageBuffer.PushBack(
                new RewindableMessageWrapper<T>
                {
                    Message = message,
                    SentAt = DateTimeOffset.UtcNow,
                    MessageId = State.LatestMessageId
                }
            );
            dirty = true;
            return Task.FromResult(new MessageHandle(LatestMessageId, State.MessageGroup));
        }

        private Task WriteStateIfDirtyAsync(object? _)
        {
            if (!dirty)
            {
                return Task.CompletedTask;
            }
            State.Messages = messageBuffer.ToArray();
            return WriteStateAsync();
        }
    }
    public class RewindableMessageGrainState<T>
    {
        public Guid MessageGroup { get; set; }
        public long LatestMessageId { get; set; }
        public RewindableMessageWrapper<T>[] Messages { get; set; } = null!;
    }

    public class RewindableMessageWrapper<T>
    {
        public long MessageId { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public T Message { get; set; } = default!;
    }
}
