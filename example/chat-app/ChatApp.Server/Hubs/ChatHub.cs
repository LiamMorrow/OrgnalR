using System.Text.RegularExpressions;
using ChatApp.GrainInterfaces;
using ChatApp.GrainInterfaces.Model;
using ChatApp.Server.Clients;
using ChatApp.Server.Model;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Server.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly IClusterClient clusterClient;

    public ChatHub(IClusterClient clusterClient)
    {
        this.clusterClient = clusterClient;
    }

    public async Task<List<ChatMessage>> JoinChat(JoinChatRequest request)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, request.ChatName);
        return await clusterClient.GetGrain<IChatGrain>(request.ChatName).GetAllMessagesAsync();
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        await clusterClient
            .GetGrain<IChatGrain>(request.ChatName)
            .SendMessageAsync(new ChatMessage(request.SenderName, request.Message));
        await Clients.Group(request.ChatName).NewMessage(request);
    }
}
