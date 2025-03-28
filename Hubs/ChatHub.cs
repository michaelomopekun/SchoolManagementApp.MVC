using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using SchoolManagementApp.MVC.Services;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public async Task SendMessage(int conversationId, string content)
    {
        try
        {
            // Only broadcast to others in the group, don't save
            await Clients.OthersInGroup(conversationId.ToString()).SendAsync("ReceiveMessage", new
            {
                conversationId = conversationId,
                content = content,
                sentAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting message in conversation {ConversationId}", conversationId);
            throw new HubException("Failed to broadcast message");
        }
    }

    public async Task JoinConversation(string conversationId)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            await Clients.Group(conversationId).SendAsync("UserJoined", Context.User.Identity?.Name);
            
            _logger.LogInformation("User {UserName} joined conversation {ConversationId}", 
                Context.User.Identity?.Name, conversationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining conversation {ConversationId}", conversationId);
            throw new HubException("Failed to join conversation");
        }
    }

    public async Task LeaveConversation(string conversationId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
            await Clients.Group(conversationId).SendAsync("UserLeft", Context.User.Identity?.Name);
            
            _logger.LogInformation("User {UserName} left conversation {ConversationId}", 
                Context.User.Identity?.Name, conversationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving conversation {ConversationId}", conversationId);
            throw new HubException("Failed to leave conversation");
        }
    }

    public async Task UserIsTyping(int conversationId)
    {
        var userName = Context.User.Identity?.Name;
        await Clients.Group(conversationId.ToString())
            .SendAsync("UserTyping", userName);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error");
        }
        await base.OnDisconnectedAsync(exception);
    }
}