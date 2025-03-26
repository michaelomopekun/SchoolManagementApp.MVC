document.addEventListener("DOMContentLoaded", function () {
  const chatContainer = document.getElementById("chat-popup-container");
  const lecturerDropdown = document.getElementById("lecturer-dropdown");
  const chatInput = document.getElementById("chat-input");
  const sendButton = document.getElementById("send-chat-message");
  const messageList = document.getElementById("message-list");

  let currentConversationId = null;
  let isSendingMessage = false; // Prevent multiple simultaneous sends

  // Initialize SignalR
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

  connection
    .start()
    .catch((err) => console.error("SignalR Connection Error:", err));

  // Load lecturers when chat is opened
  document
    .getElementById("open-chat-popup")
    ?.addEventListener("click", function () {
      chatContainer.style.display = "block";
      loadLecturers();
    });

  // Close chat popup
  document
    .getElementById("close-chat-popup")
    ?.addEventListener("click", function () {
      chatContainer.style.display = "none";
    });

  // Handle lecturer selection
  lecturerDropdown?.addEventListener("change", async function () {
    const lecturerId = parseInt(this.value, 10);
    console.log("Selected lecturer ID:", lecturerId);

    if (!lecturerId || isNaN(lecturerId)) {
      console.warn("Invalid lecturer ID selected");
      return;
    }

    try {
      console.log("Starting chat with lecturer:", lecturerId);

      const response = await fetch("/Chat/StartChat", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ lecturerId: lecturerId }),
      });

      const data = await response.json();
      console.log("Start chat response:", data);

      if (!data.success) {
        throw new Error(data.error || "Failed to start chat");
      }

      currentConversationId = data.conversationId;
      chatInput.disabled = false;
      sendButton.disabled = false;
      document.getElementById("chat-messages").style.display = "block";

      // Update chat title with lecturer name
      const selectedOption =
        lecturerDropdown.options[lecturerDropdown.selectedIndex];
      document.getElementById(
        "chat-title"
      ).textContent = `Chat with ${selectedOption.text}`;

      // Load existing messages if any
      await loadMessages(currentConversationId);
    } catch (error) {
      console.error("Error starting chat:", error);
      alert("Failed to start chat: " + error.message);
    }
  });

  // Send message
  sendButton?.removeEventListener("click", sendMessage); // Ensure no duplicate listeners
  sendButton?.addEventListener("click", sendMessage);

  chatInput?.removeEventListener("keypress", handleKeyPress); // Ensure no duplicate listeners
  chatInput?.addEventListener("keypress", handleKeyPress);

  function handleKeyPress(e) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  }

  async function sendMessage() {
    if (isSendingMessage || !currentConversationId || !chatInput.value.trim()) {
      return;
    }

    isSendingMessage = true;
    const content = chatInput.value.trim();

    try {
      // First, save the message via the controller
      const response = await fetch("/Chat/SendMessage", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          conversationId: currentConversationId,
          content: content,
          replyToMessage: null,
        }),
      });

      if (!response.ok) {
        throw new Error("Failed to send message");
      }

      const data = await response.json();
      if (data.success) {
        // Clear input and show message locally
        chatInput.value = "";
        appendMessage(data.message);

        // Then broadcast via SignalR to other users
        await connection.invoke("SendMessage", currentConversationId, content);
      }
    } catch (error) {
      console.error("Error sending message:", error);
      alert("Failed to send message: " + error.message);
    } finally {
      isSendingMessage = false;
    }
  }

  // Append message to chat
  function appendMessage(message) {
    const userId = chatContainer.dataset.userId;
    const messageDiv = document.createElement("div");
    messageDiv.className = `message ${
      message.senderId === parseInt(userId) ? "sent" : "received"
    }`;
    messageDiv.innerHTML = `
            <div class="message-content">${message.content}</div>
            <div class="message-time">${new Date(
              message.sentAt
            ).toLocaleTimeString()}</div>
        `;
    messageList.appendChild(messageDiv);
    messageList.scrollTop = messageList.scrollHeight;
  }

  // SignalR message receiver
  connection.on("ReceiveMessage", function (message) {
    if (message.conversationId === currentConversationId) {
      // Only append messages from other users
      if (message.senderId !== parseInt(chatContainer.dataset.userId)) {
        appendMessage(message);
      }
    }
  });

  async function loadLecturers() {
    try {
      const response = await fetch("/Chat/GetLecturers");
      const lecturers = await response.json();

      lecturerDropdown.innerHTML =
        '<option value="">Select a lecturer</option>';
      lecturers.forEach((lecturer) => {
        const option = document.createElement("option");
        option.value = lecturer.id;
        option.textContent = `${lecturer.name} - ${lecturer.courseName}`;
        lecturerDropdown.appendChild(option);
      });
    } catch (error) {
      console.error("Error loading lecturers:", error);
    }
  }

  async function loadMessages(conversationId) {
    try {
      const response = await fetch(`/Chat/GetMessages/${conversationId}`);

      if (response.status === 404) {
        const errorData = await response.json();
        console.warn("No messages found:", errorData.error);
        messageList.innerHTML =
          '<div class="no-messages">No messages in this conversation</div>';
        return;
      }

      if (!response.ok) {
        const errorData = await response.json();
        console.error("Failed to load messages:", errorData);
        throw new Error(errorData.error || "Failed to load messages");
      }

      const messages = await response.json();
      messageList.innerHTML = "";
      messages.forEach(appendMessage);
    } catch (error) {
      console.error("Error loading messages:", error);
    }
  }
});
