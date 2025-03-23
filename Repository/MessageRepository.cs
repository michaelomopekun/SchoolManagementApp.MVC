using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Repositories;

public class MessageRepository : IMessageRepository
{

    private readonly SchoolManagementAppDbContext _context;
    private readonly ILogger<MessageRepository> _logger;

    public MessageRepository(SchoolManagementAppDbContext context, ILogger<MessageRepository> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<Message> CreateAsync(Message message)
    {
        try
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            _logger.LogInformation("-----------------[Message] Created new message with ID: {MessageId}-----------------", message.Id);
            return message;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "-------------Error creating message-----------------");
            throw;
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        if (id == 0)
        {
            _logger.LogError("-----------------Invalid message ID: {Id}-----------------", id);
            throw new ArgumentException("Invalid message ID", nameof(id));
        }

        try
        {
            var message = _context.Messages.Find(id);
            if (message == null)
            {
                _logger.LogWarning("-----------------Message not found with ID: {Id}-----------------", id);
                return Task.FromResult(false);
            }

            _context.Messages.Remove(message);
            _context.SaveChanges();

            _logger.LogInformation("-----------------Deleted message with ID: {Id}-----------------", id);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "-----------------Error deleting message with ID: {Id}-----------------", id);
            throw;
        }
        // throw new NotImplementedException();
    }

    public async Task<Message> GetByIdAsync(int id)
    {
        if (id == 0)
        {
            _logger.LogError("-----------------Invalid message ID: {Id}-----------------", id);
            throw new ArgumentException("Invalid message ID", nameof(id));
        }

        try
        {
            var message = await _context.Messages
                .Include(message => message.Sender)
                .Include(message => message.Conversation)
                .Include(message => message.Reactions)
                .Include(message => message.Attachment)
                .FirstOrDefaultAsync(message => message.Id == id && !message.IsDeleted);

            if (message == null)
            {
                _logger.LogWarning("-----------------Message not found with ID: {Id}-----------------", id);
                return null;
            }

            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "-----------------Error getting message with ID: {Id}-----------------", id);
            throw;
        }
    }

    public async Task<IEnumerable<Message>> GetConversationMessagesAsync(int conversationId, int page = 1, int pageSize = 20)
    {
        if (conversationId == 0)
        {
            _logger.LogError("-----------------Invalid conversation ID: {ConversationId}-----------------", conversationId);
            throw new ArgumentException("Invalid conversation ID", nameof(conversationId));
        }

        try
        {
            var conversation = await _context.Messages
                .AsNoTracking()
                .Include(message => message.Sender)
                // .Include(message => message.Conversation)
                .Include(message => message.Reactions)
                .Include(message => message.Attachment)
                .Where(message => message.ConversationId == conversationId && !message.IsDeleted)
                .OrderByDescending(message => message.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return conversation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "-----------------Error getting messages for conversation with ID: {ConversationId}-----------------", conversationId);
            throw;
        }
    }

    public async Task<int> GetUnreadCountAsync(int userId, int conversationId)
    {
        if (userId <= 0 || conversationId <= 0)
        {
            _logger.LogError("-----------------Invalid user ID: {UserId} or conversation ID: {ConversationId}-----------------", userId, conversationId);
            throw new ArgumentException("Invalid user ID or conversation ID", nameof(userId));
        }

        try
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(participant => participant.UserId == userId && participant.ConversationId == conversationId);

            if (participant == null)
            {
                _logger.LogWarning("-----------------User with ID: {UserId} is not a participant in conversation with ID: {ConversationId}-----------------", userId, conversationId);
                return 0;
            }

            var conversationUnreadMessage = await _context.Messages
                .CountAsync(message => message.ConversationId == conversationId &&
                                !message.IsDeleted &&
                                message.SentAt > participant.LastReadAt);

            return conversationUnreadMessage;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "-----------------Error Getting Participants unread messages-----------------");
            throw;
        }
    }

    public async Task UpdateAsync(Message message)
    {
        if (message == null)
        {
            _logger.LogError("[Message] Attempted to update null message");
            throw new ArgumentNullException(nameof(message));
        }

        try
        {
            message.IsEdited = true;
            message.EditedAt = DateTime.UtcNow;
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();

            _logger.LogInformation("[Message] Updated message with ID: {MessageId}", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "[Message] Error updating message with ID: {MessageId}", message.Id);
            throw;
        }
    }
}
