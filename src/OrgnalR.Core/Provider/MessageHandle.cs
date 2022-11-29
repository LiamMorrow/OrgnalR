using System;
using Orleans;

namespace OrgnalR.Core.Provider
{
    [GenerateSerializer]
    public readonly struct MessageHandle
    {
        /// <summary>
        /// Represents an always increasing message id within the group.
        /// Consumers can rely on this to generally always increase for resubscription, as long as the MessageGroup is unchanged.
        /// When reliable storage is used in the silo, the MessageGroup should never change, however if memory storage is used, it may
        /// </summary>
        [Id(0)]
        public long MessageId { get; }

        /// <summary>
        /// MessageGroup is used to notify subscribers that message id has been reset.
        /// When the group changes, the MessageId cannot be relied on to always be increasing
        /// </summary>
        [Id(1)]
        public Guid MessageGroup { get; }

        public MessageHandle(long messageId, Guid messageGroup)
        {
            MessageId = messageId;
            MessageGroup = messageGroup;
        }

        public static bool operator ==(MessageHandle left, MessageHandle right)
        {
            return left.MessageId == right.MessageId && left.MessageGroup == right.MessageGroup;
        }

        public static bool operator !=(MessageHandle left, MessageHandle right)
        {
            return left.MessageId != right.MessageId || left.MessageGroup != right.MessageGroup;
        }

        public override int GetHashCode()
        {
            return MessageId.GetHashCode() ^ MessageGroup.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is MessageHandle other)
            {
                return other == this;
            }
            return false;
        }
    }
}
