using Orleans;

namespace OrgnalR.Core.Provider
{
    [GenerateSerializer]
    public class MethodMessage
    {
        [Id(0)]
        public string MethodName { get; }

        [Id(1)]
        public object?[] Args { get; }

        public MethodMessage(string methodName, object?[] args)
        {
            MethodName = methodName;
            Args = args;
        }
    }
}
