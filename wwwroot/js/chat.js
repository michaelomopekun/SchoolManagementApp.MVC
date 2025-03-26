document.addEventListener("DOMContentLoaded", function () {
  // DOM Elements
  const chatContainer = document.getElementById("chat-popup-container");
  const lecturerDropdown = document.getElementById("lecturer-dropdown");
  const chatInput = document.getElementById("chat-input");
  const sendButton = document.getElementById("send-chat-message");
  const messageList = document.getElementById("message-list");

  // State Variables
  let currentConversationId = null;
  let isSendingMessage = false; // Prevent multiple simultaneous sends

  // Initialize SignalR for real-time communication
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

  connection
    .start()
    .catch((err) => console.error("SignalR Connection Error:", err));

  // 1. Open Chat Popup
  document
    .getElementById("open-chat-popup")
    ?.addEventListener("click", function () {
      chatContainer.style.display = "block"; // Show the chat popup
      loadLecturers(); // Load the list of lecturers
    });

  // 2. Close Chat Popup
  document
    .getElementById("close-chat-popup")
    ?.addEventListener("click", function () {
      chatContainer.style.display = "none"; // Hide the chat popup
    });

  // 3. Load Lecturers
  async function loadLecturers() {
    try {
      const response = await fetch("/Chat/GetLecturers"); // Fetch lecturers from the server
      const lecturers = await response.json();

      // Populate the dropdown with lecturers
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

  // 4. Handle Lecturer Selection
  lecturerDropdown?.addEventListener("change", async function () {
    const lecturerId = parseInt(this.value, 10); // Get the selected lecturer ID
    console.log("Selected lecturer ID:", lecturerId);

    if (!lecturerId || isNaN(lecturerId)) {
      console.warn("Invalid lecturer ID selected");
      return;
    }

    try {
      console.log("Starting chat with lecturer:", lecturerId);

      // Start a new chat or fetch an existing conversation
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

      // Update the current conversation ID
      currentConversationId = data.conversationId;

      // Enable the chat input and send button
      chatInput.disabled = false;
      sendButton.disabled = false;

      // Show the chat messages container
      document.getElementById("chat-messages").style.display = "block";

      // Update the chat title with the lecturer's name
      const selectedOption =
        lecturerDropdown.options[lecturerDropdown.selectedIndex];
      document.getElementById(
        "chat-title"
      ).textContent = `Chat with ${selectedOption.text}`;

      // Load existing messages for the conversation
      await loadMessages(currentConversationId);

      // Join the SignalR group for this conversation
      await connection.invoke(
        "JoinConversation",
        currentConversationId.toString()
      );
    } catch (error) {
      console.error("Error starting chat:", error);
      alert("Failed to start chat: " + error.message);
    }
  });

  // 5. Load Messages for a Conversation
  async function loadMessages(conversationId) {
    try {
      const response = await fetch(`/Chat/GetMessages/${conversationId}`);

      if (response.status === 404) {
        console.warn("No messages found for this conversation");
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
      messageList.innerHTML = ""; // Clear the message list
      messages.forEach(appendMessage); // Append each message to the chat
    } catch (error) {
      console.error("Error loading messages:", error);
    }
  }

  // 6. Send a Message
  async function sendMessage() {
    if (isSendingMessage || !currentConversationId || !chatInput.value.trim()) {
      return; // Prevent sending if already sending or input is empty
    }

    isSendingMessage = true;
    const content = chatInput.value.trim(); // Get the message content

    try {
      // Save the message via the controller
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
        // Clear the input and show the message locally
        chatInput.value = "";
        appendMessage(data.message);

        // Broadcast the message via SignalR to other users
        await connection.invoke("SendMessage", currentConversationId, content);
      }
    } catch (error) {
      console.error("Error sending message:", error);
      alert("Failed to send message: " + error.message);
    } finally {
      isSendingMessage = false; // Reset the sending state
    }
  }

  // 7. Append a Message to the Chat
  function appendMessage(message) {
    const userId = chatContainer.dataset.userId; // Get the current user's ID
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
    messageList.appendChild(messageDiv); // Add the message to the list
    messageList.scrollTop = messageList.scrollHeight; // Scroll to the bottom
  }

  // 8. Handle Real-Time Messages via SignalR
  connection.on("ReceiveMessage", function (message) {
    if (message.conversationId === currentConversationId) {
      // Only append messages from other users
      if (message.senderId !== parseInt(chatContainer.dataset.userId)) {
        appendMessage(message);
      }
    }
  });

  // Add event listeners for sending messages
  sendButton?.addEventListener("click", sendMessage);
  chatInput?.addEventListener("keypress", function (e) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  });
});
