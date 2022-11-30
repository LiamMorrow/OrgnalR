namespace ChatApp.GrainInterfaces.Model;

[GenerateSerializer]
public record ChatMessage(string SenderName, string Message);
