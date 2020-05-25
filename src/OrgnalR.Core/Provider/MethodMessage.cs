namespace OrgnalR.Core.Provider
{
    public class MethodMessage
    {
        public string MethodName { get; }

        public object[] Args { get; }

        public MethodMessage(string methodName, object[] args)
        {
            MethodName = methodName;
            Args = args;
        }
    }
}
