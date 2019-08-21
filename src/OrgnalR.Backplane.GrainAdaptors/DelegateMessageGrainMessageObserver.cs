using System;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class DelegateMessageGrainMessageObserver : IAnonymousMessageObserver
    {
        private readonly SubscriptionHandle subscriptionHandle;
        private readonly Func<AnonymousMessage, Task> messageCallback;
        private readonly Func<SubscriptionHandle, Task> onSubscriptionEnded;

        public DelegateMessageGrainMessageObserver(SubscriptionHandle subscriptionHandle, Func<AnonymousMessage, Task> messageCallback, Func<SubscriptionHandle, Task> onSubscriptionEnded)
        {
            this.subscriptionHandle = subscriptionHandle ?? throw new ArgumentNullException(nameof(subscriptionHandle));
            this.messageCallback = messageCallback ?? throw new ArgumentNullException(nameof(messageCallback));
            this.onSubscriptionEnded = onSubscriptionEnded ?? throw new ArgumentNullException(nameof(onSubscriptionEnded));
        }

        public void ReceiveMessage(AnonymousMessage message)
        {
            messageCallback(message).Ignore();
        }

        public void SubscriptionEnded()
        {
            onSubscriptionEnded(subscriptionHandle).Ignore();
        }
    }
}