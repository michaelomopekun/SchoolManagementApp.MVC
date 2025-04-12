using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SchoolManagementApp.MVC.Models;
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
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User.Identity?.Name;

            _logger.LogInformation("User {UserName} with {userId} is sending message to conversation {ConversationId}", userName, userId, conversationId);

            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                throw new HubException("User not authenticated");
            }

            var messageData = new
            {
                conversationId = conversationId,
                content = content,
                sentAt = DateTime.UtcNow,
                senderId = userId,
                senderName = userName,
                isRead = false,
                status = "Sent"
            };

            // Only broadcast to others in the group
            _logger.LogInformation("ready to Broadcasting message to conversation {ConversationId}", conversationId);

            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", messageData);

            // broadcast to other participants in the conversation to update/reload their chat list
            var otherParticipants = await _chatService.GetConversationParticipant(conversationId);
            foreach (var participant in otherParticipants.Where(p => p.UserId.ToString() != userId))
            {
                await Clients.User(participant.UserId.ToString()).SendAsync("UpdateChatList");
                 _logger.LogInformation("Notifying user {UserId} to update chat list", participant.UserId);
            }
            _logger.LogInformation("Message sent from {UserName} in conversation {ConversationId}: {Content}", userName, conversationId, content);

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