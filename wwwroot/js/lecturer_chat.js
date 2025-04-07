document.addEventListener("DOMContentLoaded", async function () {
  // Get DOM elements
  const chatContainer = document.getElementById("chat-popup-container");
  const messageList = document.getElementById("message-list");
  const chatInput = document.getElementById("chat-input");
  const sendButton = document.getElementById("send-chat-message");
  const chatMessages = document.getElementById("chat-messages");
  const chatTitle = document.getElementById("chat-title");
  
  let currentConversationId = null;
  let currentReceiverId = null;
  let connection = null;

  // Initialize SignalR connection
  async function initSignalR() {
      try {
          connection = new signalR.HubConnectionBuilder()
              .withUrl("/chatHub")
              .withAutomaticReconnect()
              .build();

          connection.on("ReceiveMessage", function (message) {
              console.log("Message received:", message);
              if (message.conversationId === currentConversationId) {
                  appendMessage(message);
              } else {
                  loadActiveChats(); // Refresh chat list for new messages
              }
          });

          await connection.start();
          console.log("SignalR Connected.");
      } catch (err) {
          console.error("SignalR Connection Error: ", err);
          return false;
      }
  }

  await initSignalR();

  // Load active chats
  async function loadActiveChats() {
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

              chatDiv.addEventListener("click", () => openChat(chat.id, chat.studentId, chat.studentName));
              chatsList.appendChild(chatDiv);
          });
      } catch (error) {
          console.error("Error loading chats:", error);
          document.getElementById("chats-list").innerHTML = 
              '<div class="text-center text-danger p-3">Error loading chats</div>';
      }
  }

  // Load enrolled students
  async function loadEnrolledStudents() {
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

              studentDiv.addEventListener("click", () => startNewChat(student.id, student.name));
              studentsList.appendChild(studentDiv);
          });
      } catch (error) {
          console.error("Error loading students:", error);
          document.getElementById("students-list").innerHTML = 
              '<div class="text-center text-danger p-3">Error loading students</div>';
      }
  }

  // Open existing chat
  async function openChat(conversationId, studentId, studentName) {
      currentConversationId = conversationId;
      currentReceiverId = studentId;

      // Switch views
      document.getElementById("chats-list-view").style.display = "none";
      document.getElementById("chat-messages-view").style.display = "block";
      document.getElementById("current-chat-name").textContent = `Chat with ${studentName}`;

      // Enable input
      chatInput.disabled = false;
      sendButton.disabled = false;

      // Load messages
      await loadMessages(conversationId);
  }

  // Load messages for conversation
  async function loadMessages(conversationId) {
      try {
          const response = await fetch(`/Chat/GetMessages/${conversationId}`);
          const data = await response.json();
          console.log("Messages:", data);

          messageList.innerHTML = "";

          if (!data.messages || data.messages.length === 0) {
              messageList.innerHTML = '<div class="no-messages">No messages yet</div>';
              return;
          }

          data.messages.forEach(appendMessage);
          messageList.scrollTop = messageList.scrollHeight;
      } catch (error) {
          console.error("Error loading messages:", error);
          messageList.innerHTML = '<div class="error-message">Error loading messages</div>';
      }
  }

  // Append message to chat
  function appendMessage(message) {
      const messageDiv = document.createElement("div");
      messageDiv.className = `message ${message.senderId === currentReceiverId ? 'received' : 'sent'}`;
      
      const contentP = document.createElement("p");
      contentP.textContent = message.content;
      messageDiv.appendChild(contentP);
      
      const timeSpan = document.createElement("span");
      timeSpan.className = "message-time";
      const messageDate = new Date(message.sentAt);
      timeSpan.textContent = messageDate.toLocaleString();
      messageDiv.appendChild(timeSpan);
      
      messageList.appendChild(messageDiv);
      messageList.scrollTop = messageList.scrollHeight;
  }

  // Send message handler
  async function sendMessage() {
      if (!currentConversationId || !chatInput.value.trim()) {
          return;
      }

      try {
          const response = await fetch("/Chat/SendMessage", {
              method: "POST",
              headers: {
                  "Content-Type": "application/json",
              },
              body: JSON.stringify({
                  conversationId: currentConversationId,
                  content: chatInput.value.trim()
              })
          });

          if (!response.ok) {
              throw new Error("Failed to send message");
          }

          chatInput.value = "";
      } catch (error) {
          console.error("Error sending message:", error);
          alert("Failed to send message");
      }
  }

  // Event Listeners
  document.getElementById("open-chat-popup")?.addEventListener("click", function() {
      chatContainer.style.display = "block";
      document.getElementById("chats-list-view").style.display = "block";
      document.getElementById("chat-messages-view").style.display = "none";
      loadActiveChats();
  });

  document.getElementById("new-chat-toggle")?.addEventListener("click", function() {
      chatContainer.style.display = "block";
      document.getElementById("chats-list-view").style.display = "none";
      document.getElementById("new-chat-view").style.display = "block";
      loadEnrolledStudents();
  });

  document.getElementById("back-to-chats")?.addEventListener("click", function() {
      document.getElementById("chat-messages-view").style.display = "none";
      document.getElementById("chats-list-view").style.display = "block";
      currentConversationId = null;
      currentReceiverId = null;
      loadActiveChats();
  });

  document.getElementById("back-to-chats-new")?.addEventListener("click", function() {
      document.getElementById("new-chat-view").style.display = "none";
      document.getElementById("chats-list-view").style.display = "block";
      loadActiveChats();
  });

  sendButton?.addEventListener("click", sendMessage);

  chatInput?.addEventListener("keypress", function(event) {
      if (event.key === "Enter" && !event.shiftKey) {
          event.preventDefault();
          sendMessage();
      }
  });
});