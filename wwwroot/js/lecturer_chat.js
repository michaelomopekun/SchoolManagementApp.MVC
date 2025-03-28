import Chat from "./chat.js";

document.addEventListener("DOMContentLoaded", function () {
  const chatContainer = document.getElementById("chat-popup-container");

  const messageList = document.getElementById("message-list");

  const chatInput = document.getElementById("chat-input");

  const sendButton = document.getElementById("send-chat-message");

  const lecturerDropdown = document.getElementById("lecturer-dropdown");

  const chat = new Chat(
    chatContainer,
    messageList,
    chatInput,
    sendButton,
    lecturerDropdown
  );

  // Open Chat Popup
  document.getElementById("open-chat-popup")?.addEventListener("click", () => {
    chat.openChatPopup();
  });

  // Close Chat Popup
  document.getElementById("close-chat-popup")?.addEventListener("click", () => {
    chat.closeChatPopup();
  });
});
