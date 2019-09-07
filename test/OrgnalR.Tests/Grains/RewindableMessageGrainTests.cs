using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Protocol;
using OrgnalR.Backplane.GrainImplementations;
using OrgnalR.Core.Provider;
using Orleans.TestKit;
using Xunit;
using OrgnalR.Core;

namespace OrgnalR.Tests.Grains
{
    public class RewindableMessageGrainTests : TestKitBase
    {
        [Fact]
        public async Task GetMessageSinceReturnsAllMessagesIfInBounds()
        {
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig
            {
                MaxMessageRewind = 1
            });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(Guid.NewGuid().ToString());
            var handle = await grain.PushMessageAsync(new AnonymousMessage(new HashSet<string>(), new InvocationMessage("TestTarget1", new object[0])));
            var since = await grain.GetMessagesSinceAsync(handle);
            Assert.Empty(since);
            var secondMsg = new AnonymousMessage(new HashSet<string>(), new InvocationMessage("TestTarget2", new object[0]));
            var handle2 = await grain.PushMessageAsync(secondMsg);
            since = await grain.GetMessagesSinceAsync(handle2);
            Assert.Empty(since);
            since = await grain.GetMessagesSinceAsync(handle);
            Assert.NotEmpty(since);
            Assert.Equal(handle2, since.Single().handle);
            Assert.Equal(secondMsg, since.Single().message);
        }

        [Fact]
        public async Task GetMessageSinceThrowsWhenOutOfBounds()
        {
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig
            {
                MaxMessageRewind = 1
            });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(Guid.NewGuid().ToString());
            var handle = await grain.PushMessageAsync(new AnonymousMessage(new HashSet<string>(), new InvocationMessage("TestTarget1", new object[0])));
            await grain.PushMessageAsync(new AnonymousMessage(new HashSet<string>(), new InvocationMessage("TestTarget2", new object[0])));
            await grain.PushMessageAsync(new AnonymousMessage(new HashSet<string>(), new InvocationMessage("TestTarget3", new object[0])));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await grain.GetMessagesSinceAsync(handle);
            });
        }

        [Fact]
        public async Task GetMessageSinceReturnsEmptyWhenGroupChanges()
        {
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig
            {
                MaxMessageRewind = 1
            });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(Guid.NewGuid().ToString());
            var handle = await grain.PushMessageAsync(new AnonymousMessage(new HashSet<string>(), new InvocationMessage("TestTarget1", new object[0])));
            handle = new MessageHandle(handle.MessageId, Guid.NewGuid());
            Assert.Empty(await grain.GetMessagesSinceAsync(handle));
        }

        [Fact]
        public async Task GetMessageSinceReturnsEmptyWhenHandleNewer()
        {
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig
            {
                MaxMessageRewind = 1
            });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(Guid.NewGuid().ToString());
            var handle = await grain.PushMessageAsync(new AnonymousMessage(new HashSet<string>(), new InvocationMessage("TestTarget1", new object[0])));
            handle = new MessageHandle(handle.MessageId + 1, handle.MessageGroup);
            Assert.Empty(await grain.GetMessagesSinceAsync(handle));
        }
    }
}
