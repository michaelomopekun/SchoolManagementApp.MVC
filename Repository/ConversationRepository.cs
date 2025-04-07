using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly SchoolManagementAppDbContext _context;
    private readonly ILogger<ConversationRepository> _logger;

    public ConversationRepository(SchoolManagementAppDbContext context, ILogger<ConversationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }



    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        try
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "-------------Error creating conversation-----------------");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogError("---------------Invalid conversation ID: {Id}--------------", id);
            throw new ArgumentException("Invalid conversation ID", nameof(id));
        }

        try
        {
            var conversation = await _context.Conversations.FindAsync(id);
            if (conversation == null)
            {
                _logger.LogWarning("--------------Conversation not found with ID: {Id}--------------", id);
                return false;
            }

            conversation.IsDeleted = true;
            conversation.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("--------------Soft deleted conversation with ID: {Id}--------------", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "--------------Error deleting conversation with ID: {Id}--------------", id);
            throw;
        }
    }

    public async Task<Conversation> GetByIdAsync(int id)
    {
        if (id == 0)
        {
            _logger.LogError("---------------Conversation Id is null-----------------");
            throw new ArgumentNullException(nameof(id));
        }

        try
        {
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (conversation == null)
            {
                _logger.LogWarning("--------------Conversation not found with ID: {Id}--------------", id);
                return null;
            }

            return conversation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "--------------Error getting conversation with ID: {Id}--------------", id);
            throw;
        }
    }

    public async Task<IEnumerable<Conversation>> GetCourseConversationsAsync(int courseId)
    {
        if (courseId == 0)
        {
            _logger.LogError("---------------Course Id is null-----------------");
            throw new ArgumentNullException(nameof(courseId));
        }
        try
        {
            var Conversations = await _context.Conversations
                .Include(c => c.Course)
                .Include(c => c.Participants)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(u => u.CourseId == courseId && !u.IsDeleted)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            if (Conversations == null)
            {
                _logger.LogError("---------------_logger.LogError([Conversation] Operation failed - Conversation is null);-----------------");
                throw new ArgumentNullException(nameof(Conversations));
            }

            return Conversations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "--------------Error getting course conversations with ID: {Id}--------------", courseId);
            throw;
        }
    }

    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId)
    {
        if (userId == 0)
        {
            _logger.LogError("---------------User Id is null-----------------");
            throw new ArgumentNullException(nameof(userId));
        }

        try
        {
            _logger.LogInformation("--------------Getting user conversations with ID: {Id}--------------", userId);

            var Conversations = await _context.Conversations
                .AsNoTracking()
                .Include(c => c.Participants)
                .Include(c => c.Course)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(u => u.Participants.Any(u => u.UserId == userId) && !u.IsDeleted)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            _logger.LogInformation("--------------User conversations retrieved with ID: {Id}--------------", Conversations.FirstOrDefault()?.Id);

            if (Conversations == null)
            {
                _logger.LogError("---------------Conversations is null-----------------");
                throw new ArgumentNullException(nameof(Conversations));
            }

            return Conversations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "--------------Error getting user conversations with ID: {Id}--------------", userId);
            throw;
        }
    }

    public async Task UpdateAsync(Conversation conversation)
    {
        if (conversation == null)
        {
            _logger.LogError("---------------Conversation is null-----------------");
            throw new ArgumentNullException(nameof(conversation));
        }

        try
        {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
            _logger.LogInformation("--------------Conversation updated with ID: {Id}--------------", conversation.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "--------------Error updating conversation with ID: {Id}--------------", conversation.Id);
            throw;
        }

    }

            public async Task<Conversation?> GetConversationBetweenUsersAsync(int user1Id, int user2Id)
        {
            if(user1Id != 0 && user2Id != 0){
            return await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => 
                    c.Type == ConversationType.StudentLecturerChat &&
                    c.Participants.Any(p => p.UserId == user1Id) &&
                    c.Participants.Any(p => p.UserId == user2Id));
            }
            return null;
        }
}