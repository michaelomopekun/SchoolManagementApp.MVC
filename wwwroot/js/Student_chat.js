import Chat from "./chat.js";

document.addEventListener("DOMContentLoaded", function () {
  // Get DOM elements
  const chatContainer = document.getElementById("chat-popup-container");
  const messageList = document.getElementById("message-list");
  const chatInput = document.getElementById("chat-input");
  const sendButton = document.getElementById("send-chat-message");
  const lecturerDropdown = document.getElementById("lecturer-dropdown");

  // Debug DOM elements
  console.log("Chat container:", chatContainer);
  console.log("Lecturer dropdown:", lecturerDropdown);

  if (!chatContainer || !lecturerDropdown) {
    console.error("Chat container or lecturer dropdown not found in the DOM.");
    return;
  }

  // Initialize chat
  const chat = new Chat(
    chatContainer,
    messageList,
    chatInput,
    sendButton,
    lecturerDropdown
  );

  // Handle chat button click
  const openChatButton = document.getElementById("open-chat-popup");
  if (openChatButton) {
    openChatButton.addEventListener("click", (event) => {
      console.log("Chat button clicked");
      event.preventDefault();
      chat.openChatPopup();
    });
  } else {
    console.error("Open chat button not found");
  }

  // Handle close button click
  const closeChatButton = document.getElementById("close-chat-popup");
  if (closeChatButton) {
    closeChatButton.addEventListener("click", (event) => {
      console.log("Close button clicked");
      event.preventDefault();
      chat.closeChatPopup();
    });
  } else {
    console.error("Close chat button not found");
  }

  // Handle lecturer selection
  lecturerDropdown.addEventListener("change", (event) => {
    const lecturerId = parseInt(event.target.value, 10);
    if (lecturerId) {
      chat.startChat(lecturerId);
    }
  });
});
