using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrgnalR.Backplane.GrainImplementations;
using OrgnalR.Core.Provider;
using Orleans.TestKit;
using Xunit;
using OrgnalR.Core;

namespace OrgnalR.Tests.Grains
{
    public class RewindableMessageGrainTests : TestKitBase
    {
        [Fact(Skip = "TestKit is not updated to orleans 7")]
        public async Task GetMessageSinceReturnsAllMessagesIfInBounds()
        {
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig { MaxMessageRewind = 1, });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(
                Guid.NewGuid().ToString()
            );
            var handle = await grain.PushMessageAsync(
                new AnonymousMessage(
                    new HashSet<string>(),
                    new MethodMessage("TestTarget1", new object[0])
                )
            );
            var since = await grain.GetMessagesSinceAsync(handle);
            Assert.Empty(since);
            var secondMsg = new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget2", new object[0])
            );
            var handle2 = await grain.PushMessageAsync(secondMsg);
            since = await grain.GetMessagesSinceAsync(handle2);
            Assert.Empty(since);
            since = await grain.GetMessagesSinceAsync(handle);
            Assert.NotEmpty(since);
            Assert.Equal(handle2, since.Single().handle);
            Assert.Equal(secondMsg, since.Single().message);
        }

        [Fact(Skip = "TestKit is not updated to orleans 7")]
        public async Task GetMessageSinceReturnsAllMessagesIfInBoundsLargerSet()
        {
            var maxRewind = 10;
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig { MaxMessageRewind = maxRewind });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(
                Guid.NewGuid().ToString()
            );
            var handles = Enumerable
                .Range(0, 20)
                .Select(
                    i =>
                        grain.PushMessageAsync(
                            new AnonymousMessage(
                                new HashSet<string>(),
                                new MethodMessage(i.ToString(), new object[0])
                            )
                        )
                )
                .Select(x => x.Result)
                .ToList();

            for (int i = 0; i < handles.Count; i++)
            {
                if (i + 1 < maxRewind)
                {
                    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                        async () => await grain.GetMessagesSinceAsync(handles[i])
                    );
                }
                else
                {
                    var since = await grain.GetMessagesSinceAsync(handles[i]);
                    var expectedSinceHandles = handles
                        .SkipWhile(x => x.MessageId <= handles[i].MessageId)
                        .ToList();
                    Assert.Equal(expectedSinceHandles, since.Select(x => x.handle).ToList());
                    if (i != handles.Count - 1)
                    {
                        Assert.NotEmpty(since);
                    }
                }
            }
        }

        [Fact(Skip = "TestKit is not updated to orleans 7")]
        public async Task GetMessageSinceThrowsWhenOutOfBounds()
        {
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig { MaxMessageRewind = 1 });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(
                Guid.NewGuid().ToString()
            );
            var handle = await grain.PushMessageAsync(
                new AnonymousMessage(
                    new HashSet<string>(),
                    new MethodMessage("TestTarget1", new object[0])
                )
            );
            await grain.PushMessageAsync(
                new AnonymousMessage(
                    new HashSet<string>(),
                    new MethodMessage("TestTarget2", new object[0])
                )
            );
            await grain.PushMessageAsync(
                new AnonymousMessage(
                    new HashSet<string>(),
                    new MethodMessage("TestTarget3", new object[0])
                )
            );
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await grain.GetMessagesSinceAsync(handle);
            });
        }

        [Fact(Skip = "TestKit is not updated to orleans 7")]
        public async Task GetMessageSinceReturnsEmptyWhenGroupChanges()
        {
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig { MaxMessageRewind = 1 });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(
                Guid.NewGuid().ToString()
            );
            var handle = await grain.PushMessageAsync(
                new AnonymousMessage(
                    new HashSet<string>(),
                    new MethodMessage("TestTarget1", new object[0])
                )
            );
            handle = new MessageHandle(handle.MessageId, Guid.NewGuid());
            Assert.Empty(await grain.GetMessagesSinceAsync(handle));
        }

        [Fact(Skip = "TestKit is not updated to orleans 7")]
        public async Task GetMessageSinceReturnsEmptyWhenHandleNewer()
        {
            Silo.ServiceProvider.AddService(new OrgnalRSiloConfig { MaxMessageRewind = 1 });
            var grain = await Silo.CreateGrainAsync<RewindableMessageGrain<AnonymousMessage>>(
                Guid.NewGuid().ToString()
            );
            var handle = await grain.PushMessageAsync(
                new AnonymousMessage(
                    new HashSet<string>(),
                    new MethodMessage("TestTarget1", new object[0])
                )
            );
            handle = new MessageHandle(handle.MessageId + 1, handle.MessageGroup);
            Assert.Empty(await grain.GetMessagesSinceAsync(handle));
        }
    }
}
