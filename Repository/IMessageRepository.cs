using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> GetByIdAsync(int id);
        Task<IEnumerable<Message>> GetConversationMessagesAsync(int conversationId, int page = 1, int pageSize = 20);
        Task<Message> CreateAsync(Message message);
        Task UpdateAsync(Message message);
        Task<bool> DeleteAsync(int id);
        Task<int> GetUnreadCountAsync(int userId, int conversationId);
    }
}