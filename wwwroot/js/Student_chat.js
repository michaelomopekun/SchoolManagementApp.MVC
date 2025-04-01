import Chat from "./chat.js";

document.addEventListener("DOMContentLoaded", async function () {
  // Get DOM elements
  const chatContainer = document.getElementById("chat-popup-container");
  const messageList = document.getElementById("message-list");
  const chatInput = document.getElementById("chat-input");
  const sendButton = document.getElementById("send-chat-message");
  const lecturerDropdown = document.getElementById("lecturer-dropdown");
  const chatTitle = document.getElementById("chat-title");
  const chatMessages = document.getElementById("chat-messages");

  // Debug DOM elements
  console.log("Chat container:", chatContainer);
  console.log("Lecturer dropdown:", lecturerDropdown);

  if (!chatContainer || !lecturerDropdown) {
    console.error("Required DOM elements are missing.");
    return;
  }

  // Initialize chat
  const chat = new Chat(
    chatContainer,
    messageList,
    chatInput,
    sendButton,
    lecturerDropdown,
    chatTitle,
    chatMessages
  );

  await chat.initSignalR();
  console.log("SignalR initialized successfully.");


  document
  .getElementById("open-chat-popup")
  ?.addEventListener("click", async function () {
    chatContainer.style.display = "block"; // Show the chat popup

    await chat.loadLecturers(lecturerDropdown);
    console.log("Lecturers loaded successfully.");
  });


  document
  .getElementById("close-chat-popup")
  ?.addEventListener("click", function () {
    chatContainer.style.display = "none"; // Hide the chat popup
  });

  

  // Handle lecturer selection
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
      await chat.loadMessage(currentConversationId);

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


});