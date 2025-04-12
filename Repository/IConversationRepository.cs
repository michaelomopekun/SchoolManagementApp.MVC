using System.Threading.Tasks;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Repositories
{
    public interface IConversationRepository
    {
        Task<Conversation> GetByIdAsync(int id);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId);
        Task<Conversation> CreateAsync(Conversation conversation);
        Task UpdateAsync(Conversation conversation);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Conversation>> GetCourseConversationsAsync(int courseId);
        Task<Conversation> GetConversationBetweenUsersAsync(int user1Id, int user2Id);
        Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(int conversationId);
    }
}