using ChatApp.GrainInterfaces;
using ChatApp.GrainInterfaces.Model;
using Orleans.Runtime;

namespace ChatApp.Silo;

public class ChatGrain : IGrainBase, IChatGrain
{
    public IGrainContext GrainContext { get; }

    private readonly List<ChatMessage> messages = new();

    public ChatGrain(IGrainContext context)
    {
        GrainContext = context;
    }

    public Task SendMessageAsync(ChatMessage message)
    {
        messages.Add(message);
        return Task.CompletedTask;
    }

    public Task<List<ChatMessage>> GetAllMessagesAsync()
    {
        return Task.FromResult(messages);
    }
}
