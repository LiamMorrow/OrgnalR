namespace ChatApp.Server.Model;

public record JoinChatRequest(string ChatName);

public record SendMessageRequest(string ChatName, string SenderName, string Message);
