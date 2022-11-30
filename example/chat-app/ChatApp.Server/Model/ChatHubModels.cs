namespace ChatApp.Server.Model;

[GenerateSerializer]
public record JoinChatRequest(string ChatName);

[GenerateSerializer]
public record SendMessageRequest(string ChatName, string SenderName, string Message);
