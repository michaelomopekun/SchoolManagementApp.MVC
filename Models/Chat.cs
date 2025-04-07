using SchoolManagementApp.MVC.Models;

public class Conversation
{
    public int Id { get; set; }

    public ConversationType Type { get; set; }

    public string? name { get; set; }

    public int? CourseId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public string? Topic { get; set; }

    public virtual Course? Course { get; set; }

    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    public string? LastMessage { get; set; }

    public int UnreadCount { get; set; }

    public int MessageCount { get; set; } = 0;

    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();

}


public class ConversationParticipant
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastReadAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;

    public bool IsActive { get; set; }

    public DateTime DeletedAt { get; set; }

    public bool IsTyping { get; set; } = false;

    public DateTime LastTypingAt { get; set; }

    public bool IsMuted { get; set; } = false;

    public DateTime MutedUntil { get; set; }

    public int? LastReadMessageId { get; set; }

    public Message? LastReadMessage { get; set; }

    public bool HasReadMessage(int messageId)
    => LastReadMessageId.HasValue && LastReadMessageId.Value >= messageId;

    public virtual Conversation? Conversation { get; set; }

    public virtual User? User { get; set; }

}



public class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string? Content { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; } 
    public DateTime ReadAt { get; set; } 
    public MessageStatus Status { get; set; } = MessageStatus.Sent;

    public bool IsDeleted { get; set; } = false;

    public DateTime DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public int? ReplyToMessageId { get; set; }

    public bool IsEdited { get; set; }

    public DateTime EditedAt { get; set; }


    public Conversation? Conversation { get; set; }

    public virtual User? Sender { get; set; }

    public virtual Message? ReplyToMessage { get; set; }
    public int? AttachmentId { get; set; }

    public virtual MessageAttachment? Attachment { get; set; }

    public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();

    // public ICollection<MessageReadStatus> ReadStatuses { get; set; } = new List<MessageReadStatus>();


}




// public class MessageReadStatus
// {
//     public int Id { get; set; }

//     public int MessageId { get; set; }

//     public int UserId { get; set; }

//     public DateTime ReadAt { get; set; }

//     public virtual Message? Message { get; set; }

//     public virtual User? User { get; set; }

// }



public class MessageReaction
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public int UserId { get; set; }

    public string? Reaction { get; set; }

    public DateTime ReactedAt { get; set; }

    public virtual Message? Message { get; set; }

    public virtual User? User { get; set; }

}


public class MessageAttachment
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public string? Url { get; set; }

    public string? Type { get; set; }

    public string? Name { get; set; }

    public string? Size { get; set; }

    public virtual Message? Message { get; set; }
}


public enum MessageStatus
{
    Sent,

    Delivered,
    
    Read,
    
    Failed,
    
    Deleted

}

public enum ConversationType
{
    StudentLecturerChat = 1,

    CourseGroupChat = 2,

    CourseAnnouncement = 3,

    AcademicQuery = 4,

    OfficeHoursConsultation = 5

}