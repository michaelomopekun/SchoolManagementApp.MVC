
namespace SchoolManagementApp.MVC.Services
{
    public interface IChatService
    {
        Task<Conversation> CreateOneOnOneChat(int studentId, int lecturerId);
        Task<Conversation> CreateCourseGroupChat(int courseId, string name);
        Task<Message> SendMessage(int conversationId, int senderId, string content, int? replyToMessageId = null);
        Task<bool> MarkConversationAsRead(int conversationId, int userId);
        Task<IEnumerable<Conversation>> GetUserConversations(int userId);
        Task<IEnumerable<Message>> GetConversationMessages(int conversationId, int page = 1);
        Task<int> GetUnreadCount(int userId, int conversationId);
        Task UpdateTypingStatus(int conversationId, int userId, bool isTyping);
        Task<bool> DeleteMessage(int messageId, int userId);
        Task<bool> AddReaction(int messageId, int userId, string reaction);
    }
}