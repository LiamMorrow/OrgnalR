using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using OrgnalR.Core.Provider;
using Xunit;

namespace OrgnalR.Tests.Core.Internal;

public record MyMethodRequest(string Message);

public interface ITestClient
{
    Task MyMethod(string arg1, int arg2, MyMethodRequest arg3);
    Task<int> MyMethodWithAReturnValue();
}

public class TypedClientBuilderTests
{
    [Fact]
    public async Task GetsAStronglyTypedClient()
    {
        var clientProxy = new Mock<IClientProxy>();
        var client = TypedClientBuilder<ITestClient>.Build(clientProxy.Object);

        var arg1 = "MyArg1";
        var arg2 = 30;
        var arg3 = new MyMethodRequest("Message");
        await client.MyMethod(arg1, arg2, arg3);

        clientProxy.Verify(
            x =>
                x.SendCoreAsync(
                    nameof(ITestClient.MyMethod),
                    new object[] { arg1, arg2, arg3 },
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task ThrowsInvalidOperationExceptionForMethodsWithReturnValues()
    {
        var clientProxy = new Mock<IClientProxy>();
        var client = TypedClientBuilder<ITestClient>.Build(clientProxy.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(client.MyMethodWithAReturnValue);
    }
}
