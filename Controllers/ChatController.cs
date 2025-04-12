using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Repository;
using SchoolManagementApp.MVC.Services;

[Authorize]
[Route("Chat")]
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

    [HttpPost("SendMessage")]
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

    [HttpPost("DeleteMessage")]
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

    [HttpPost("StartChat")]
    public async Task<IActionResult> StartChat([FromBody] StartChatRequest request)
    {
        if (request == null)
        {
            _logger.LogWarning("StartChatRequest is null");
            return BadRequest(new { success = false, error = "Invalid request" });
        }

        var UserId = await GetUserId();

        if (UserId <= 0)
        {
            _logger.LogWarning("User ID is invalid: {UserId}", UserId);
            return Unauthorized(new { success = false, error = "Invalid user ID" });
        }

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

    [HttpPost("GetUnreadMessageCount/{conversationId}")]
    public async Task<IActionResult> GetUnreadMessageCount(int conversationId)
    {
        var userId = await GetUserId();
        var count = await _chatService.GetUnreadCount(userId, conversationId);
        return Json(new {count});
    }

    [HttpGet("GetRecentChats")]
    public async Task<IActionResult> GetRecentChats()
    {
        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var userId = await GetUserId();

            if (userId <= 0)
            {
                _logger.LogWarning("User ID is invalid: {UserId}", userId);
                return BadRequest(new { success = false, error = "Invalid user ID" });
            }

            _logger.LogInformation("GetRecentChats called with userId: {UserId}", userId);

            var conversations = await _chatService.GetUserConversations(userId);

            if (conversations == null || !conversations.Any())
            {
                _logger.LogWarning("No recent chats found for user {UserId}", userId);
                return Json(new { success = false, message = "No recent chats found" });
            }

            _logger.LogInformation("Conversations found for user {UserId}: {ConversationsCount}", userId, conversations.Count());

            // Create a simplified object to avoid circular references
            var chatList = conversations.Select(c => new
            {
                id = c.Id,
                studentId = c.Participants?.FirstOrDefault(p => p.UserId != userId)?.UserId,
                studentName = c.Participants?.FirstOrDefault(p => p.UserId != userId)?.User?.Username ?? "Unknown User",
                lastMessage = c.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Content ?? "",
                lastMessageTime = c.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault()?.SentAt.ToString("yyyy-MM-ddTHH:mm:ss") ?? c.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                unreadCount = c.Messages?.Count(m => !m.IsRead && m.SenderId != userId) ?? 0
            }).ToList();

            _logger.LogInformation("Found {Count} chats for user {UserId}", chatList.Count, userId);

            return Json(chatList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get recent chats for user");
            return Json(500,new { error = "Failed to load recent chats" });
        }
    }

    [HttpGet("GetLecturers")]
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


    [HttpGet("GetMessages/{conversationId}")]
    public async Task<IActionResult> GetMessages([FromRoute] int conversationId)
    {
        try
        {
            _logger.LogInformation("Fetching messages for conversation {ConversationId}", conversationId);

            var userId = await GetUserId();
            var messages = await _chatService.GetConversationMessages(conversationId);

            if (messages == null || !messages.Any())
            {
                _logger.LogWarning("No messages found for conversation {ConversationId}", conversationId);
                return Ok(new List<Message>()); // Return empty list directly
            }

            var messageList = messages.Select(m => new
            {
                id = m.Id,
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                content = m.Content,
                sentAt = m.SentAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                status = m.Status
            }).ToList();

            return Json( new {messages = messageList}); // Return messages directly
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messages for conversation {ConversationId}", conversationId);
            return StatusCode(500, new { error = "Failed to load messages" });
        }
    }

    [HttpGet("GetLecturersStudents")]
    public async Task<IActionResult> GetLecturersStudents()
    {
        var LecturerId = await GetUserId();

        if (LecturerId <= 0)
        {
            _logger.LogWarning("Lecturer ID is invalid: {LecturerId}", LecturerId);
            return BadRequest(new { success = false, error = "Invalid lecturer ID" });
        }

        _logger.LogInformation("GetLecturersStudents called with lecturerId: {LecturerId}", LecturerId);

        var students = await _enrollmentRepository.GetLecturersStudentsAsync(LecturerId);

        var studentList = students.Select(x => new
        {
            Id = x.Id,
            Name = x.User.Username
        });

        return Json(studentList);
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