document.addEventListener("DOMContentLoaded", function () {
  const chatPopupContainer = document.getElementById("chat-popup-container");
  const openChatPopup = document.getElementById("open-chat-popup");
  const closeChatPopup = document.getElementById("close-chat-popup");
  const chatInput = document.getElementById("chat-input");
  const sendChatMessage = document.getElementById("send-chat-message");
  const viewFullChat = document.getElementById("view-full-chat");

  let connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

  let currentConversationId = null;
  let selectedLecturerId = null;

  connection.start().catch((err) => console.error(err));

  connection.on("ReceiveMessage", function (message) {
    appendMessage(message);
  });

  connection.on("UserTyping", function (userName) {
    showTypingIndicator(userName);
  });

  openChatPopup?.addEventListener("click", function () {
    chatPopupContainer.style.display = "block";
    loadRecentChats();
  });

  closeChatPopup?.addEventListener("click", function () {
    chatPopupContainer.style.display = "none";
  });

  chatInput?.addEventListener("keypress", function (e) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  });

  sendChatMessage?.addEventListener("click", sendMessage);

  async function joinConversation(conversationId) {
    if (currentConversationId) {
      await connection.invoke("LeaveConversation", currentConversationId);
    }
    currentConversationId = conversationId;
    await connection.invoke("JoinConversation", conversationId);
    await loadMessages(conversationId);
  }

  async function sendMessage() {
    const content = chatInput.value.trim();
    if (!content || !currentConversationId) return;

    try {
      await fetch("/Chat/SendMessage", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          conversationId: currentConversationId,
          content: content,
        }),
      });

      await connection.invoke("SendMessage", currentConversationId, content);
      chatInput.value = "";
    } catch (error) {
      console.error("Error sending message:", error);
    }
  }

  async function loadRecentChats() {
    try {
      const response = await fetch("/Chat/GetRecentChats", {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });

      if (!response.ok) {
        throw new Error("Failed to load chats");
      }

      const chats = await response.json();
      displayRecentChats(chats);
    } catch (error) {
      console.error("Error loading recent chats:", error);
    }
  }

  function displayRecentChats(chats) {
    const chatContent = document.querySelector("#chat-content");
    chatContent.innerHTML = chats
      .map(
        (chat) => `
        <div class="chat-item" onclick="joinConversation(${chat.id})">
            <div class="chat-title">${chat.name || "Direct Message"}</div>
            <div class="chat-preview">${
              chat.lastMessage?.content || "No messages yet"
            }</div>
        </div>
    `
      )
      .join("");
  }

  function appendMessage(message) {
    const messageDiv = document.createElement("div");
    messageDiv.className = `message ${
      message.isSentByMe ? "sent" : "received"
    }`;
    messageDiv.innerHTML = `
      <div class="message-content">${message.content}</div>
      <div class="message-time">${new Date(
        message.sentAt
      ).toLocaleTimeString()}</div>
    `;
    document.querySelector("#chat-content").appendChild(messageDiv);
    scrollToBottom();
  }

  function scrollToBottom() {
    const chatContent = document.querySelector("#chat-content");
    chatContent.scrollTop = chatContent.scrollHeight;
  }

  async function loadLecturers() {
    try {
      console.log("Loading lecturers...");
      const response = await fetch("/Chat/GetLecturers");

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log("Received lecturers:", data);

      if (data.error) {
        console.error("Server error:", data.error);
        return;
      }

      const lecturerDropdown = document.getElementById("lecturer-dropdown");
      if (!lecturerDropdown) {
        console.error("Lecturer dropdown element not found!");
        return;
      }

      // Clear existing options
      lecturerDropdown.innerHTML =
        '<option value="">Select lecturer to chat with</option>';

      if (Array.isArray(data) && data.length > 0) {
        data.forEach((lecturer) => {
          const option = document.createElement("option");
          option.value = lecturer.id;
          option.textContent = `${lecturer.name} - ${lecturer.courseName}`;
          lecturerDropdown.appendChild(option);
        });
      } else {
        console.log("No lecturers found or invalid data format");
        const option = document.createElement("option");
        option.disabled = true;
        option.textContent = "No lecturers available";
        lecturerDropdown.appendChild(option);
      }
    } catch (error) {
      console.error("Error loading lecturers:", error);
      const lecturerDropdown = document.getElementById("lecturer-dropdown");
      if (lecturerDropdown) {
        lecturerDropdown.innerHTML =
          '<option value="">Error loading lecturers</option>';
      }
    }
  }

  // Ensure the function is called when chat is opened
  document
    .getElementById("open-chat-popup")
    ?.addEventListener("click", function () {
      console.log("Chat popup opened");
      document.getElementById("chat-popup-container").style.display = "block";
      loadLecturers();
    });

  async function startChat(lecturerId) {
    try {
      const response = await fetch("/Chat/StartChat", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ lecturerId }),
      });

      if (!response.ok) throw new Error("Failed to start chat");

      const result = await response.json();
      if (result.conversationId) {
        currentConversationId = result.conversationId;
        await joinConversation(currentConversationId);
        loadMessages(currentConversationId);
      }
    } catch (error) {
      console.error("Error starting chat:", error);
    }
  }

  // Update the existing chat popup button click handler
  document
    .getElementById("open-chat-popup")
    .addEventListener("click", function () {
      if (!currentConversationId) {
        loadLecturers();
      }
      document.getElementById("chat-popup-container").style.display = "block";
    });
});

document.addEventListener("DOMContentLoaded", function () {
  const chatPopupContainer = document.getElementById("chat-popup-container");
  const lecturerSelection = document.getElementById("lecturer-selection");
  const chatMessages = document.getElementById("chat-messages");
  const lecturerSearch = document.getElementById("lecturer-search");
  const lecturerList = document.getElementById("lecturer-list");

  // Load lecturers when chat is opened
  document
    .getElementById("open-chat-popup")
    .addEventListener("click", function () {
      chatPopupContainer.style.display = "block";
      loadLecturers();
    });

  async function loadLecturers() {
    try {
      const response = await fetch("/Chat/GetLecturers");
      const lecturers = await response.json();

      lecturerList.innerHTML = lecturers
        .map(
          (lecturer) => `
              <div class="lecturer-item" onclick="startChat(${lecturer.id})">
                  <div class="name">${lecturer.name}</div>
                  <div class="course">${lecturer.course}</div>
              </div>
          `
        )
        .join("");
    } catch (error) {
      console.error("Error loading lecturers:", error);
    }
  }

  document.addEventListener("DOMContentLoaded", function () {
    const chatPopup = document.getElementById("chat-popup-container");
    const lecturerDropdown = document.getElementById("lecturer-dropdown");
    const chatInput = document.getElementById("chat-input");
    const sendButton = document.getElementById("send-chat-message");
    let currentConversationId = null;

    // Load lecturers when chat is opened
    document
      .getElementById("open-chat-popup")
      ?.addEventListener("click", function () {
        chatPopup.style.display = "block";
        loadLecturers();
      });

    async function loadLecturers() {
      try {
        const response = await fetch("/Chat/GetLecturers");
        const lecturers = await response.json();

        if (lecturers.error) {
          console.error(lecturers.error);
          return;
        }

        lecturerDropdown.innerHTML = `
                <option value="">Select lecturer to chat with</option>
                ${lecturers
                  .map(
                    (l) => `
                    <option value="${l.id}"> ${l.name} }</option>
                `
                  )
                  .join("")}
            `;
      } catch (error) {
        console.error("Error loading lecturers:", error);
      }
    }

    // Handle lecturer selection
    lecturerDropdown.addEventListener("change", async function () {
      const lecturerId = this.value;
      if (!lecturerId) {
        chatInput.disabled = true;
        sendButton.disabled = true;
        return;
      }

      try {
        const response = await fetch("/Chat/StartChat", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ lecturerId: parseInt(lecturerId) }),
        });

        const result = await response.json();
        if (result.error) {
          console.error(result.error);
          return;
        }

        currentConversationId = result.id;
        chatInput.disabled = false;
        sendButton.disabled = false;
        document.getElementById("chat-messages").style.display = "block";

        // Load existing messages
        await loadMessages(currentConversationId);
      } catch (error) {
        console.error("Error starting chat:", error);
      }
    });
  });
});
