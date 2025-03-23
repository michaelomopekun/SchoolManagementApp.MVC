using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Services
{
    public interface IChatService
    {
        Task<Conversation> CreateOneOnOneChat(int studentId, int lecturerId);
        Task<Conversation> CreateCourseGroupChat(int courseId, string name);
        Task<Message> SendMessage(int conversationId, int senderId, string content);
        Task<bool> MarkMessageAsRead(int messageId, int userId);
        Task<IEnumerable<Message>> GetConversationHistory(int conversationId, int page = 1);
        Task<IEnumerable<Conversation>> GetUserActiveChats(int userId);
    }
}