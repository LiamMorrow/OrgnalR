using ChatApp.GrainInterfaces.Model;

namespace ChatApp.GrainInterfaces;

public interface IChatGrain : IGrainWithStringKey
{
    Task SendMessageAsync(ChatMessage message);

    Task<List<ChatMessage>> GetAllMessagesAsync();
}
