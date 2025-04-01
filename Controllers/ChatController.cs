using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Repository;
using SchoolManagementApp.MVC.Services;

public class ChatController : Controller
{

    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public ChatController(IChatService chatService, ILogger<ChatController> logger, IEnrollmentRepository enrollmentRepository)
    {
        _chatService = chatService;
        _logger = logger;
        _enrollmentRepository = enrollmentRepository;
    }

    private async Task<int> GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogError("User Claim not found");

            // throw new Exception("User not found");
        }

        return int.Parse(userId);
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = await GetUserId();
            var conversations = await _chatService.GetUserConversations(userId);
            return View(conversations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user conversations");
            TempData["Error"] = "Failed to get user conversations";
            return RedirectToAction("Index", "Home");
        }
    }

    public async Task<IActionResult> Conversation(int id)
    {
        try
        {
            var userId = await GetUserId();
            var conversation = await _chatService.GetConversationMessages(id);

            // if (!conversation.Participants.Any(p => p.UserId == userId))
            // {
            //     _logger.LogWarning("User {UserId} is not authorized to delete message {MessageId}", userId, messageId);
            //     return Forbid();
            // }

            await _chatService.MarkConversationAsRead(id, userId);
            return View(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get conversation messages");
            TempData["Error"] = "Failed to get conversation messages";
            return RedirectToAction(nameof(Index));
            
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var userId = await GetUserId();
            // Save message to database
            var message = await _chatService.SendMessage(
                request.ConversationId,
                userId,
                request.Content,
                request.ReplyToMessageId
            );

            return Json(new { 
                success = true, 
                message = new {
                    id = message.Id,
                    content = message.Content,
                    senderId = message.SenderId,
                    sentAt = message.SentAt,
                    conversationId = message.ConversationId
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message");
            return Json(new { success = false, error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMessage(int messageId)
    {
        try
        {
            var userId = await GetUserId();
            var result = await _chatService.DeleteMessage(messageId, userId);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete message");
            TempData["Error"] = "Failed to delete message";

            return Json(new { success = false });
        }
    }

    [HttpPost]
    public async Task<IActionResult> StartChat([FromBody] StartChatRequest request)
    {
        _logger.LogInformation("StartChat called with lecturerId: {LecturerId}", request.LecturerId);

        try
        {
            if (request.LecturerId <= 0)
            {
                _logger.LogWarning("Invalid lecturer ID received: {LecturerId}", request.LecturerId);
                return BadRequest(new { success = false, error = "Invalid lecturer ID" });
            }

            var userId = await GetUserId();
            _logger.LogInformation("Starting chat between user {UserId} and lecturer {LecturerId}", 
                userId, request.LecturerId);

            var conversation = await _chatService.CreateOneOnOneChat(userId, request.LecturerId);

            return Json(new { 
                success = true, 
                conversationId = conversation.Id,
                lecturerId = request.LecturerId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start chat with lecturer {LecturerId}", request.LecturerId);
            return BadRequest(new { 
                success = false, 
                error = ex.Message 
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> StartGroupChat(int courseId, string name)
    {
        try
        {
            if (courseId <= 0 || string.IsNullOrEmpty(name))
            {
                _logger.LogError("Invalid input - CourseId: {CourseId}, Name: {Name}", courseId, name);
                TempData["Error"] = "Invalid input for group chat.";
                return RedirectToAction(nameof(Index));
            }

            var conversation = await _chatService.CreateCourseGroupChat(courseId, name);
            return RedirectToAction(nameof(Conversation), new {id = conversation.Id});
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Error Starting GroupChat for Course {courseId}");
            TempData["Error"] = "Failed to Start Group Chat";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetUnreadMessageCount(int conversationId)
    {
        var userId = await GetUserId();
        var count = await _chatService.GetUnreadCount(userId, conversationId);
        return Json(new {count});
    }

    [HttpGet]
    public async Task<IActionResult> GetRecentChats()
    {
        try
        {
            var userId = await GetUserId();
            var conversations = await _chatService.GetUserConversations(userId);
            return Json(conversations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get recent chats for user");
            return Json(new { error = "Failed to load recent chats" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLecturers()
    {
        try
        {
            var studentId = await GetUserId();
            var lecturers = await _enrollmentRepository.GetEnrolledCourseLecturersAsync(studentId);

            var lecturerList = lecturers.Select(x => new
            {
                Id = x.Id,
                Name = x.Username,
                // CourseName = x.CourseName
            });

            return Json(lecturerList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get lecturers for student {StudentId}", await GetUserId());
            return Json(new { error = "Failed to load lecturers" });
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetMessages(int conversationId) {
        var messages = await _chatService.GetConversationMessages(conversationId, page: 1);
        return Json(messages);
    }



    public class SendMessageRequest
    {
        public int ConversationId { get; set; }
        public string Content { get; set; }
        public int? ReplyToMessageId { get; set; }
    }

    public class StartChatRequest
    {
        public int LecturerId { get; set; }
    }

}