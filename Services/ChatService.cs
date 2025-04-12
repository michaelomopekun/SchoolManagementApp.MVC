using SchoolManagementApp.MVC.Repositories;
using SchoolManagementApp.MVC.Services;

public class ChatService : IChatService
{

    private readonly ILogger<ChatService> _logger;
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private const string DELETED_MESSAGE_TEXT = "*This message has been deleted*";

    public ChatService(ILogger<ChatService> logger, IMessageRepository messageRepository, IConversationRepository conversationRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
    }



    public Task<bool> AddReaction(int messageId, int userId, string reaction)
    {
        throw new NotImplementedException();
    }

    public Task<Conversation> CreateCourseGroupChat(int courseId, string name)
    {
        try
        {
            var conversation = new Conversation
            {
                Type = ConversationType.CourseGroupChat,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                CourseId = courseId,
                name = name
            };

            return _conversationRepository.CreateAsync(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Chat] Failed to create course group chat for course {CourseId}", courseId);
            throw;
        }
    }

    public async Task<Conversation> CreateOneOnOneChat(int studentId, int lecturerId)
    {
        // lecturerId = 3;
        if(lecturerId <= 0)
        {
            _logger.LogError("Invalid lecturerId {LecturerId}",lecturerId);
            
            // return BadRequest(new { success = false, error = "Invalid lecturer ID" });
        }

        try
        {
            var existingConversation = await _conversationRepository.GetConversationBetweenUsersAsync(studentId, lecturerId);
            if (existingConversation != null)
            {
                return existingConversation;
            }

            var conversation = new Conversation
            {

                Type = ConversationType.StudentLecturerChat,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Participants = new List<ConversationParticipant>
                {
                    new ConversationParticipant { UserId = studentId },
                    new ConversationParticipant { UserId = lecturerId }
                }
            };

            return await _conversationRepository.CreateAsync(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Chat] Failed to create chat between student {StudentId} and lecturer {LecturerId}",
                studentId, lecturerId);
            throw;
        }
    }

    public async Task<bool> DeleteMessage(int messageId, int userId)
    {
        try
        {
            if (messageId <= 0 || userId <= 0)
            {
                _logger.LogError("[Chat] Invalid input - MessageId: {MessageId}, UserId: {UserId}", messageId, userId);
                throw new ArgumentException("Invalid message ID or user ID");
            }

            var message = await _messageRepository.GetByIdAsync(messageId);

            if (message == null)
            {
                _logger.LogWarning("[Chat] Message {MessageId} not found", messageId);
                return false;
            }

            if (message.SenderId != userId)
            {
                var conversation = await _conversationRepository.GetByIdAsync(message.ConversationId);
                var participant = conversation?.Participants.FirstOrDefault(p => p.UserId == userId);

                if (participant == null)
                {
                    _logger.LogWarning("[Chat] User {UserId} is not a participant in conversation {ConversationId}", userId, message.ConversationId);
                    return false;
                }
            }

            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow;
            message.DeletedBy = userId.ToString();
            message.Content = DELETED_MESSAGE_TEXT;

            await _messageRepository.UpdateAsync(message);

            _logger.LogInformation("[Chat] Message {MessageId} deleted by user {UserId}",
                messageId, userId);

            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Chat] Failed to delete message {MessageId}", messageId);
            throw;
        }

    }

    public async Task<IEnumerable<Message>> GetConversationMessages(int conversationId, int page = 1)
    {
        try
        {
            return await _messageRepository.GetConversationMessagesAsync(conversationId, page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Chat] Failed to get message History for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    public async Task<int> GetUnreadCount(int userId, int conversationId)
    {
        try
        {
            if (userId <= 0 || conversationId <= 0)
            {
                _logger.LogError("[Chat] Invalid input - UserId: {UserId}, ConversationId: {ConversationId}",
                    userId, conversationId);
                throw new ArgumentException("Invalid user ID or conversation ID");
            }

            var unreadCount = await _messageRepository.GetUnreadCountAsync(userId, conversationId);

            _logger.LogInformation("[Chat] Retrieved unread count for user {UserId} in conversation {ConversationId}: {Count}",
                userId, conversationId, unreadCount);

            return unreadCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Chat] Failed to get unread count for user {UserId} in conversation {ConversationId}",
                userId, conversationId);
            throw;
        }
    }

public async Task<IEnumerable<Conversation>> GetUserConversations(int userId)
{
    try
    {
        if (userId <= 0)
        {
            _logger.LogError("[Chat] Invalid userId: {UserId}", userId);
            throw new ArgumentException("Invalid user ID");
        }

        var conversations = await _conversationRepository.GetUserConversationsAsync(userId);
        
        if (conversations == null)
        {
            _logger.LogWarning("[Chat] No conversations found for user {UserId}", userId);
            return new List<Conversation>();
        }

        foreach(var conversation in conversations)
        {
            if (conversation.Participants != null)
            {
                var otherParticipant = conversation.Participants
                    .FirstOrDefault(p => p.UserId != userId && p.User != null);

                conversation.name = otherParticipant?.User?.Username ?? "Unknown User";
                
                // Add last message and unread count
                var lastMessage = conversation.Messages?
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();
                    
                conversation.LastMessageAt = lastMessage?.SentAt ?? conversation.CreatedAt;
                conversation.LastMessage = lastMessage?.Content ?? "";
                conversation.UnreadCount = conversation.Messages?
                    .Count(m => !m.IsRead && m.SenderId != userId) ?? 0;
            }
            else
            {
                _logger.LogWarning("[Chat] Conversation {ConversationId} has no participants", 
                    conversation.Id);
                conversation.name = "Unknown User";
            }
        }

        // Order by most recent message
        return conversations
            .OrderByDescending(c => c.LastMessageAt)
            .ToList();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "[Chat] Failed to get conversations for user {UserId}", userId);
        throw;
    }
}

    public async Task<bool> MarkConversationAsRead(int conversationId, int userId)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            if (conversation == null)
            {
                _logger.LogWarning("[Chat] Conversation {ConversationId} not found", conversationId);
                return false;
            }

            var participant = conversation.Participants.FirstOrDefault(p => p.UserId == userId);

            if (participant == null)
            {
                _logger.LogWarning("[Chat] User {UserId} is not a participant in conversation {ConversationId}", userId, conversationId);
                return false;
            }

            var lastMessage = conversation.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

            if (lastMessage != null)
            {
                participant.LastReadMessageId = lastMessage.Id;
                participant.LastReadAt = DateTime.UtcNow;
                await _conversationRepository.UpdateAsync(conversation);
            }

            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Chat] Failed to mark conversation {ConversationId} as read", conversationId);
            throw;
        }
    }

    public async Task<Message> SendMessage(int conversationId, int senderId, string content, int? replyToMessageId = null)
    {
        try
        {
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                SentAt = DateTime.UtcNow,
                Status = MessageStatus.Sent,
                ReplyToMessageId = replyToMessageId
            };

            var savedMessage = await _messageRepository.CreateAsync(message);

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            if (conversation != null)
            {
                conversation.LastMessageAt = DateTime.UtcNow;
                conversation.MessageCount++;
                await _conversationRepository.UpdateAsync(conversation);
            }

            return savedMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Chat] Failed to send message in conversation {ConversationId}", conversationId);
            throw;
        }
    }


    public async Task<IEnumerable<ConversationParticipant>> GetConversationParticipant(int conversationId)
    {
        if(conversationId <= 0)
        {
            _logger.LogError("[Chat] Invalid conversationId {ConversationId}", conversationId);
            throw new ArgumentException("Invalid conversation ID");
        }

        try
        {
            var ConversationParticipant = await _conversationRepository.GetConversationParticipantsAsync(conversationId);

            if (ConversationParticipant == null)
            {
                _logger.LogWarning("[Chat] No participants found for conversation {ConversationId}", conversationId);
                return new List<ConversationParticipant>();
            }

            return ConversationParticipant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Chat] Failed to get participants for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    public Task UpdateTypingStatus(int conversationId, int userId, bool isTyping)
    {
        throw new NotImplementedException();
    }
}