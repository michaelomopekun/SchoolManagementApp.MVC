document.addEventListener("DOMContentLoaded", async function () 
{
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
  async function initializeSignalR() 
  {
    try 
    {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .withAutomaticReconnect()
            .build();

        // Set up message handler before starting connection
        connection.on("ReceiveMessage", async function(message) 
        {
            console.log("Message received:", message);

            if (message.conversationId === currentConversationId) 
            {
                await loadMessages(currentConversationId);
                messageList.scrollTop = messageList.scrollHeight;
            }
        });

        // Start the connection
        await connection.start();
        console.log("SignalR Connected.");
        return true;

    } 
    catch (err) 
    {
        console.error("SignalR Connection Error:", err);
        return false;
    }
}

// Initialize SignalR when the page loads
initializeSignalR().then(connected => 
{
    if (!connected) 
    {
        console.error("Failed to establish SignalR connection");
    }
});

  

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
  async function openChat(conversationId, studentId, studentName) 
  {
    try
    {
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

      if(connection)
      {
        await connection.invoke("JoinConversation", conversationId.toString());
        console.log("Joined conversation:", conversationId);
      }
      else
      {
        console.error("SignalR connection is  not established.");
      }

      console.log("Joined conversation:", conversationId);
    }
    catch(error)
    {
        console.error("Error opening chat:", error);
        alert("Failed to open chat. Please try again.");
    }

  }

  // Load messages for conversation
  async function loadMessages(conversationId) {
      try 
      {
          const response = await fetch(`/Chat/GetMessages/${conversationId}`);
          const data = await response.json();
          console.log("Messages:", data);

        //   if(!data.response === 404)
        //   {
        //     data.messages.sort((a,b) =>
        //         {
        //             return new Date(a.sentAt) - new Date(b.sentAt);
        //         });
        //   }

          messageList.innerHTML = "";

          if (!data.messages || data.messages.length === 0) {
              messageList.innerHTML = '<div class="no-messages">No messages yet</div>';
              return;
          }

          const sortedMessage = data.messages.sort((a,b) =>
            {
                return new Date(a.sentAt) - new Date(b.sentAt);
            });

          data.messages.forEach(appendMessage);
          messageList.scrollTop = messageList.scrollHeight;
      } catch (error) 
      {
          console.error("Error loading messages:", error);
          messageList.innerHTML = '<div class="error-message">Error loading messages</div>';
      }
  }


  // Append message to chat
  async function appendMessage(message) 
  {
      const messageDiv = document.createElement("div");
      const isSent = message.senderId === "self" || message.senderId !== currentReceiverId;
      messageDiv.className = `message ${isSent ? 'sent' : 'received'}`;
      
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
  async function sendMessage() 
  {

      if (!currentConversationId || !chatInput.value.trim()) 
        {
          return;
        }

        const messageContent = chatInput.value.trim();

      try 
      {
          const response = await fetch("/Chat/SendMessage", 
            {
              method: "POST",
              headers: {
                  "Content-Type": "application/json",
              },
              body: JSON.stringify({
                  conversationId: currentConversationId,
                  content: chatInput.value.trim()
              })
            });

          if (!response.ok) 
            {
              throw new Error("Failed to send message");
            }

            // const messageData =
            // {
            //     content: messageContent,
            //     sentAt: new Date().toISOString(),
            //     senderId: "self",
            //     receiverId: currentReceiverId,
            //     conversationId: currentConversationId
            // };

            // await appendMessage(messageData);

            chatInput.value = "";

            if(connection)
            {
                console.log("Broadcasting message via SignalR",
                    {
                        conversationId: currentConversationId,
                        content: messageContent
                    }
                );
                
                await connection.invoke("SendMessage", currentConversationId, messageContent);
                console.log("Message broardcasted via SignalR");
            }
            else
            {
                console.error("SignalR connection is not established.", connection.State);
                throw new Error("Chat connection lost. Please refresh the page.");
            }

            await loadMessages(currentConversationId);

            messageList.scrollTop = messageList.scrollHeight;

      }
      catch (error) 
      {
          console.error("Error sending message:", error);
          alert("Failed to send message" + error.message);
      }
  }



  // Event Listeners
  document.getElementById("open-chat-popup")?.addEventListener("click", function() 
  {
      chatContainer.style.display = "block";
      document.getElementById("chats-list-view").style.display = "block";
      document.getElementById("chat-messages-view").style.display = "none";
      loadActiveChats();
  });


  document.getElementById("new-chat-toggle")?.addEventListener("click", function() 
  {
      chatContainer.style.display = "block";
      document.getElementById("chats-list-view").style.display = "none";
      document.getElementById("new-chat-view").style.display = "block";
      loadEnrolledStudents();
  });


  document.getElementById("back-to-chats")?.addEventListener("click", function() 
  {
      document.getElementById("chat-messages-view").style.display = "none";
      document.getElementById("chats-list-view").style.display = "block";
      currentConversationId = null;
      currentReceiverId = null;
      loadActiveChats();
  });


  document.getElementById("back-to-chats-new")?.addEventListener("click", function() 
  {
      document.getElementById("new-chat-view").style.display = "none";
      document.getElementById("chats-list-view").style.display = "block";
      loadActiveChats();
  });


  sendButton?.addEventListener("click", sendMessage);


  chatInput?.addEventListener("keypress", function(event) 
  {
      if (event.key === "Enter" && !event.shiftKey) 
        {
          event.preventDefault();
          sendMessage();
        }
  });
});