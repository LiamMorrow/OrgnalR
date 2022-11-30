using Microsoft.Extensions.ObjectPool;
using OrgnalR.Core.Provider;
using Orleans.Serialization;

namespace OrgnalR.Backplane.GrainAdaptors;

public class OrleansMessageArgsSerializer : IMessageArgsSerializer
{
    private readonly Serializer serializer;

    public OrleansMessageArgsSerializer(Orleans.Serialization.Serializer serializer)
    {
        this.serializer = serializer;
    }

    public object?[] Deserialize(byte[] serialized)
    {
        return serializer.Deserialize<object?[]>(serialized);
    }

    public byte[] Serialize(object?[] args)
    {
        return serializer.SerializeToArray(args);
    }
}
