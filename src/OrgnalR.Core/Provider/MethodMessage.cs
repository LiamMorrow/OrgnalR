using Orleans;

namespace OrgnalR.Core.Provider
{
    [GenerateSerializer]
    public class MethodMessage
    {
        public string MethodName { get; }

        public object?[] Args { get; }

        public MethodMessage(string methodName, object?[] args)
        {
            MethodName = methodName;
            Args = args;
        }
    }
}
