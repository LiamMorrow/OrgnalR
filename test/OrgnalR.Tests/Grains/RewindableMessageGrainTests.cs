using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core;
using OrgnalR.Core.Provider;
using OrgnalR.Silo;
using Orleans.Hosting;
using Orleans.TestingHost;
using Xunit;

namespace OrgnalR.Tests.Grains;

public class TestSiloConfigurationsMax1 : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddOrgnalRWithMemoryGrainStorage();
        siloBuilder.ConfigureServices(services =>
        {
            services.AddTransient(config => new OrgnalRSiloConfig { MaxMessageRewind = 1 });
        });
    }
}

public class TestSiloConfigurationsMax10 : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddOrgnalRWithMemoryGrainStorage();
        siloBuilder.ConfigureServices(services =>
        {
            services.AddTransient(config => new OrgnalRSiloConfig { MaxMessageRewind = 10 });
        });
    }
}

public class RewindableMessageGrainTests
{
    public TestCluster? Cluster { get; set; }

    [Fact]
    public async Task GetMessageSinceReturnsAllMessagesIfInBounds()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax1>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();
        var grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        var handle = await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget1", Array.Empty<byte>())
            )
        );
        var since = await grain.GetMessagesSinceAsync(handle);
        Assert.Empty(since);
        var secondMsg = new AnonymousMessage(
            new HashSet<string>(),
            new MethodMessage("TestTarget2", new byte[] { 1, 2, 3 })
        );
        var handle2 = await grain.PushMessageAsync(secondMsg);
        since = await grain.GetMessagesSinceAsync(handle2);
        Assert.Empty(since);
        since = await grain.GetMessagesSinceAsync(handle);
        Assert.NotEmpty(since);
        Assert.Equal(handle2, since.Single().handle);
        Assert.Equal(
            secondMsg.Payload.SerializedArgs,
            since.Single().message.Payload.SerializedArgs
        );
    }

    [Fact]
    public async Task GetMessageSinceReturnsAllMessagesIfInBoundsLargerSet()
    {
        var maxRewind = 10;
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax10>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();

        var grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        var handles = Enumerable
            .Range(0, 20)
            .Select(
                i =>
                    grain.PushMessageAsync(
                        new AnonymousMessage(
                            new HashSet<string>(),
                            new MethodMessage(i.ToString(), Array.Empty<byte>())
                        )
                    )
            )
            .Select(x => x.Result)
            .ToList();
        for (var i = 0; i < handles.Count; i++)
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
                    Assert.NotEmpty(since);
            }
    }

    [Fact]
    public async Task GetMessageSinceThrowsWhenOutOfBounds()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax1>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();
        var grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        var handle = await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget1", Array.Empty<byte>())
            )
        );
        await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget2", Array.Empty<byte>())
            )
        );
        await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget3", Array.Empty<byte>())
            )
        );
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await grain.GetMessagesSinceAsync(handle);
        });
    }

    [Fact]
    public async Task GetMessageSinceReturnsEmptyWhenGroupChanges()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax1>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();
        var grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        var handle = await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget1", Array.Empty<byte>())
            )
        );
        handle = new MessageHandle(handle.MessageId, Guid.NewGuid());
        Assert.Empty(await grain.GetMessagesSinceAsync(handle));
    }

    [Fact]
    public async Task GetMessageSinceReturnsEmptyWhenHandleNewer()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax1>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();
        var grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        var handle = await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget1", Array.Empty<byte>())
            )
        );
        handle = new MessageHandle(handle.MessageId + 1, handle.MessageGroup);
        Assert.Empty(await grain.GetMessagesSinceAsync(handle));
    }
}
