

document.addEventListener("DOMContentLoaded", function () 
{
  const chatContainer = document.getElementById("chat-popup-container");

  const lecturerDropdown = document.getElementById("lecturer-dropdown");
  
  const chatInput = document.getElementById("chat-input");
  
  const sendButton = document.getElementById("send-chat-message");
  
  const messageList = document.getElementById("message-list");
  
  let currentConversationId = null;
  let currentReceiverId = null;
  let isSendingMessage = false;
  let connection = null;

  // Initialize SignalR
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





  // Load Lecturers
  async function loadLecturers() 
  {
    try 
    {
        console.log("Fetching recipients...");
        const response = await fetch("/Chat/GetLecturers");
        console.log("Response status:", response.status);

        if (!response.ok) 
        {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log("Raw response:", data);

        // Handle both array and wrapped formats
        const lecturers = Array.isArray(data) ? data : (data.$values || []);
        console.log("Processed lecturers:", lecturers);

        lecturerDropdown.innerHTML = '<option value="">Select a recipient</option>';
        
        if (lecturers.length === 0) 
        {
            lecturerDropdown.innerHTML += '<option value="" disabled>No recipients available</option>';
            return;
        }

        lecturers.forEach(lecturer => 
        {
            const option = document.createElement("option");
            option.value = lecturer.id;
            option.textContent = `${lecturer.name || 'Unknown'}`;
            lecturerDropdown.appendChild(option);
        });
    } 
    catch (error) 
    {
        console.error("Error loading recipients:", error);
        lecturerDropdown.innerHTML = '<option value="">Error loading recipients</option>';
    }
}

  // Handle recipient selection
  lecturerDropdown?.addEventListener("change", async function () 
  {
    const lecturerId = parseInt(this.value, 10);

    currentReceiverId = lecturerId;

    if (!lecturerId || isNaN(lecturerId)) 
    {
        console.warn("Invalid recipient selected:", this.value);
        return;
    }

    try 
    {
        console.log("Starting chat with recipient:", lecturerId);

        const response = await fetch("/Chat/StartChat", 
            {
                method: "POST",
                headers: 
                {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                body: JSON.stringify(
                    { 
                    lecturerId: lecturerId,
                    isPrivateChat: true
                    }),
                credentials: 'include' // Include cookies in the request
        });

        if (!response.ok) 
        {
            const errorData = await response.json();
            throw new Error(errorData.error || `Failed to start chat: ${response.status}`);
        }

        const data = await response.json();
        console.log("Chat started successfully:", data);

        currentConversationId = data.conversationId;
        chatInput.disabled = false;
        sendButton.disabled = false;
        
        document.getElementById("chat-messages").style.display = "block";
        const selectedOption = lecturerDropdown.options[lecturerDropdown.selectedIndex];
        document.getElementById("chat-title").textContent = `Chat with ${selectedOption.text}`;

        //debugging
        console.log(`conversationId: ${currentConversationId}`);
        console.log(`receiverId: ${currentReceiverId}`);

        await loadMessages(currentConversationId);

        await connection.invoke("JoinConversation", currentConversationId.toString());
    } 
    catch (error) 
    {
        console.error("Error starting chat:", error);
        alert(`Failed to start chat: ${error.message}`);
    }
});




    // Load messages for the current conversation
    async function loadMessages(conversationId) 
    {
        if (!conversationId) 
        {
            console.error("No conversation ID provided");
            return;
        }
    
        try 
        {
            console.log("Loading messages for conversation:", conversationId);
    
            const response = await fetch(`/Chat/GetMessages/${conversationId}`, 
            {
                method: 'GET',
                headers: 
                {
                    "Accept": "application/json",
                    "Content-Type": "application/json"
                },
                credentials: 'include'
            });
    
            console.log("Response status:", response.status);

            if(response.status === 500)
            {
                console.log("internal error");
                messageList.innerHTML = `<div class="no-message">Internal error 500</div>`;
                return;
            }
    
            if (!response.ok)
            {
                if(response.status == 404)
                {
                    console.warn("No messages found for this conversation.");
                    messageList.innerHTML = '<div class="no-messages">No messages yet</div>';
                    return;
                }
                const errorData = await response.text();
                console.error("Error response:", errorData);
                throw new Error(errorData.error || `HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            console.log("Raw messages response:", data);

            const sortedMessages = data.messages.sort((a,b) =>
            {
                return new Date(a.sentAt) - new Date(b.sentAt);
            });
    
            messageList.innerHTML = "";

            const fragment = document.createDocumentFragment();
    
            // Handle direct array format
            const messages = data.messages || [];
            console.log("Processed messages:", messages);
    
            if (!data.messages || data.messages.length === 0)
            {
                messageList.innerHTML = '<div class="no-messages">No messages yet</div>';
                return;
            }
    
            messages.forEach(message => 
            {
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
        
                fragment.appendChild(messageDiv);

            });

            messageList.appendChild(fragment);
    
            messageList.scrollTop = messageList.scrollHeight;

        } 
        catch (error) 
        {
            console.error("Error loading messages:", error);
            messageList.innerHTML = '<div class="error-message">Error loading messages</div>';
        }
    }


    function appendMessage(message)
    {
        const messageDiv = document.createElement("div");

        messageDiv.className = `message ${message.senderId === currentReceiverId ? 'received' : 'sent'}`;

        const contentP = document.createElement("p");
        
        contentP.textContent = message.content;

        messageDiv.appendChild(contentP);

        // Add timestamp for new messages
        const timeSpan = document.createElement("span");

        timeSpan.className = "message-time";
        
        const messageDate = new Date(message.sentAt);
        
        timeSpan.textContent = messageDate.toLocaleTimeString();
        
        messageDiv.appendChild(timeSpan);

        messageList.appendChild(messageDiv);

        messageList.scrollTop = messageList.scrollHeight; 
    }




  // Send Message
  sendButton?.addEventListener("click", sendMessage);
  chatInput?.addEventListener("keypress", function (e) 
  {
      if (e.key === "Enter" && !e.shiftKey) 
    {
          e.preventDefault();
          sendMessage();
        //   await loadMessages();
      }
  });

  async function sendMessage() 
  {
      if (isSendingMessage || !currentConversationId || !chatInput.value.trim()) 
      {
          return;
      }

      isSendingMessage = true;
      const content = chatInput.value.trim();

      try 
      {
          const response = await fetch("/Chat/SendMessage", 
          {
              method: "POST",
              headers: {
                  "Content-Type": "application/json",
              },
              body: JSON.stringify(
                {
                  conversationId: currentConversationId,
                  content: content,
                  receiverId: currentReceiverId
                }),
           });

          if (!response.ok) 
          {
              throw new Error("Failed to send message");
          }

          chatInput.value = "";

          await loadMessages(currentConversationId);

          await connection.invoke("SendMessage", currentConversationId, content);

          messageList.scrollTop = messageList.scrollHeight;

      } 
      catch (error) 
      { 
          console.error("Error sending message:", error);
      } 
      finally 
      { 
          isSendingMessage = false;
      } 
  }

  // Show/Hide Chat Popup
  document.getElementById("open-chat-popup")?.addEventListener("click", function () 
  {
      chatContainer.style.display = "block";
      loadLecturers();
  });

  document.getElementById("close-chat-popup")?.addEventListener("click", function () 
  {
      chatContainer.style.display = "none";
  });

});