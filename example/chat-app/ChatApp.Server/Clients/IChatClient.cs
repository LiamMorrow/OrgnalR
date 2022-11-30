using ChatApp.GrainInterfaces.Model;

namespace ChatApp.Server.Clients;

public interface IChatClient
{
    Task NewMessage(SendMessageRequest message);
}
