export class ChatService {
  constructor() {
      // Initialize properties
      this.chatContainer = document.getElementById("chat-popup-container");
      this.messageList = document.getElementById("message-list");
      this.chatInput = document.getElementById("chat-input");
      this.sendButton = document.getElementById("send-chat-message");
      this.chatMessages = document.getElementById("chat-messages");
      this.chatTitle = document.getElementById("chat-title");
      this.currentConversationId = null;
      this.currentReceiverId = null;
      this.connection = null;

      // Bind methods to preserve 'this' context
      this.sendMessage = this.sendMessage.bind(this);
      this.loadMessages = this.loadMessages.bind(this);
      this.appendMessage = this.appendMessage.bind(this);
      this.openChat = this.openChat.bind(this);
      this.loadActiveChats = this.loadActiveChats.bind(this);
      this.loadEnrolledStudents = this.loadEnrolledStudents.bind(this);
      this.initializeEventListeners = this.initializeEventListeners.bind(this);

      // Initialize SignalR and event listeners
      this.initializeSignalR().then(connected => {
          if (connected) {
              this.initializeEventListeners();
          } else {
              console.error("Failed to establish SignalR connection");
          }
      });
  }

  async initializeSignalR() {
      try {
          this.connection = new signalR.HubConnectionBuilder()
              .withUrl("/chatHub")
              .withAutomaticReconnect()
              .build();

          // Set up message handler before starting connection
          this.connection.on("ReceiveMessage", async (message) => {
              console.log("Message received:", message);
              if (message.conversationId === this.currentConversationId) {
                  await this.loadMessages(this.currentConversationId);
                  this.messageList.scrollTop = this.messageList.scrollHeight;
              }
          });

          await this.connection.start();
          console.log("SignalR Connected.");
          return true;
      } catch (err) {
          console.error("SignalR Connection Error:", err);
          return false;
      }
  }


  async loadActiveChats() {
    try {
        const response = await fetch("/Chat/GetRecentChats");
        const data = await response.json();
        console.log("Active chats:", data);

        const chatsList = document.getElementById("chats-list");
        chatsList.innerHTML = "";

        if (!data || data.length === 0) {
            chatsList.innerHTML = '<div class="text-center p-3">No active chats</div>';
            return;
        }

        data.forEach(chat => {
            const chatDiv = document.createElement("div");
            chatDiv.className = "chat-item";
            chatDiv.innerHTML = `
                <div class="chat-item-content">
                    <span class="chat-name">${chat.studentName}</span>
                    <div class="chat-preview">${chat.lastMessage || 'No messages'}</div>
                    <span class="chat-time">${new Date(chat.lastMessageTime).toLocaleString()}</span>
                </div>
                ${chat.unreadCount > 0 ? 
                    `<span class="unread-badge">${chat.unreadCount}</span>` : 
                    ''}
            `;

            chatDiv.addEventListener("click", () => this.openChat(chat.id, chat.studentId, chat.studentName));
            chatsList.appendChild(chatDiv);
        });
    } catch (error) {
        console.error("Error loading chats:", error);
        document.getElementById("chats-list").innerHTML = 
            '<div class="text-center text-danger p-3">Error loading chats</div>';
    }
}


async loadEnrolledStudents() {
  try {
      const response = await fetch("/Chat/GetLecturersStudents");
      const data = await response.json();
      console.log("Enrolled students:", data);

      const studentsList = document.getElementById("students-list");
      studentsList.innerHTML = "";

      if (!data || data.length === 0) {
          studentsList.innerHTML = '<div class="text-center p-3">No students found</div>';
          return;
      }

      data.forEach(student => {
          const studentDiv = document.createElement("div");
          studentDiv.className = "student-item";
          studentDiv.innerHTML = `
              <div class="student-info">
                  <div class="student-name">${student.name}</div>
                  <div class="student-course">${student.courseName}</div>
              </div>
          `;

          studentDiv.addEventListener("click", () => this.startNewChat(student.id, student.name));
          studentsList.appendChild(studentDiv);
      });
  } catch (error) {
      console.error("Error loading students:", error);
      document.getElementById("students-list").innerHTML = 
          '<div class="text-center text-danger p-3">Error loading students</div>';
  }
}


async openChat(conversationId, studentId, studentName) {
  try {
      this.currentConversationId = conversationId;
      this.currentReceiverId = studentId;

      document.getElementById("chats-list-view").style.display = "none";
      document.getElementById("chat-messages-view").style.display = "block";
      document.getElementById("current-chat-name").textContent = `Chat with ${studentName}`;

      this.chatInput.disabled = false;
      this.sendButton.disabled = false;

      await this.loadMessages(conversationId);

      if (this.connection?.state === "Connected") {
          await this.connection.invoke("JoinConversation", conversationId.toString());
          console.log("Joined conversation:", conversationId);
      } else {
          throw new Error("SignalR connection not established");
      }
  } catch (error) {
      console.error("Error opening chat:", error);
      alert("Failed to open chat. Please try again.");
  }
}


async loadMessages(conversationId) {
  try {
      const response = await fetch(`/Chat/GetMessages/${conversationId}`);
      const data = await response.json();
      
      this.messageList.innerHTML = "";
      if (!data.messages || data.messages.length === 0) {
          this.messageList.innerHTML = '<div class="no-messages">No messages yet</div>';
          return;
      }

      const sortedMessages = data.messages.sort((a,b) => 
          new Date(a.sentAt) - new Date(b.sentAt)
      );

      sortedMessages.forEach(message => this.appendMessage(message));
      this.messageList.scrollTop = this.messageList.scrollHeight;
  } catch (error) {
      console.error("Error loading messages:", error);
      this.messageList.innerHTML = '<div class="error-message">Error loading messages</div>';
  }
}


async sendMessage() {
  if (!this.currentConversationId || !this.chatInput.value.trim()) {
      return;
  }

  const messageContent = this.chatInput.value.trim();

  try {
      const response = await fetch("/Chat/SendMessage", {
          method: "POST",
          headers: {
              "Content-Type": "application/json",
          },
          body: JSON.stringify({
              conversationId: this.currentConversationId,
              content: messageContent
          })
      });

      if (!response.ok) {
          throw new Error("Failed to send message");
      }

      this.chatInput.value = "";

      if (this.connection?.state === "Connected") {
          await this.connection.invoke("SendMessage", this.currentConversationId, messageContent);
          await this.loadMessages(this.currentConversationId);
      } else {
          throw new Error("Chat connection lost. Please refresh the page.");
      }
  } catch (error) {
      console.error("Error sending message:", error);
      alert("Failed to send message: " + error.message);
  }
}


appendMessage(message) {
  const messageDiv = document.createElement("div");
  const isSent = message.senderId === "self" || message.senderId !== this.currentReceiverId;
  messageDiv.className = `message ${isSent ? 'sent' : 'received'}`;
  
  const contentP = document.createElement("p");
  contentP.textContent = message.content;
  messageDiv.appendChild(contentP);
  
  const timeSpan = document.createElement("span");
  timeSpan.className = "message-time";
  const messageDate = new Date(message.sentAt);
  timeSpan.textContent = messageDate.toLocaleString();
  messageDiv.appendChild(timeSpan);
  
  this.messageList.appendChild(messageDiv);
}

}