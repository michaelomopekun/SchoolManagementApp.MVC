import ChatService from "./ChatService.js";

export default class Chat {
  constructor(chatContainer, messageList, chatInput, sendButton) {
    this.chatContainer = chatContainer;

    this.messageList = messageList;

    this.chatInput = chatInput;

    this.sendButton = sendButton;

    this.lecturerDropdown = lecturerDropdown;

    this.conversationId = null;

    this.isSendingMessage = false;

    this.initEventListeners();

    this.initSignalR();
  }

  openChatPopup() {
    console.log("Opening chat popup...");
    if (this.chatContainer) {
      this.chatContainer.style.display = "block";
      if (this.lecturerDropdown) {
        this.loadLecturers(this.lecturerDropdown);
      } else {
        console.error("Lecturer dropdown not initialized");
      }
    } else {
      console.error("Chat container not initialized");
    }
  }

  closeChatPopup() {
    this.chatContainer.style.display = "none";
  }

  initSignalR() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("/chatHub")
      .withAutomaticReconnect()
      .build();

    this.connection
      .start()
      .catch((err) => console.error("SignalR Connection Error:", err));

    this.connection.on("ReceiveMessage", (message) => {
      if (message.conversationId === this.conversationId) {
        this.appendMessage(message);
      }
    });
  }

  async loadLecturers(lecturerDropdown) {
    try {
      const lecturers = await ChatService.loadLecturers();

      lecturerDropdown.innerHTML =
        '<option value=""> Select a Lecturer </option>';

      lecturers.forEach((lecturer) => {
        const option = document.createElement("option");

        option.value = lecturer.id;
        option.textContent = `${lecturer.name}`;

        lecturerDropdown.appendChild(option);
      });
    } catch (err) {
      console.error("Error loading lecturer:", err);
    }
  }

  async startChat(lecturerId) {
    try {
      const data = await ChatService.startChat(lecturerId);

      this.currentConversationId = data.conversationId;

      this.chatInput.disabled = false;

      this.sendButton.disabled = false;

      await this.loadMessages();
    } catch (err) {
      console.error("Error starting chat:", error);
    }
  }

  async loadMessage() {
    try {
      const messages = await ChatService.loadMessages(
        this.currentConversationId
      );

      this.messageList.innerHTML = "";

      messages.forEach((message) => this.appendMessage(message));
    } catch (err) {
      console.error("Error loading messages: ", err);
    }
  }

  async sendMessage() {
    if (this.isSendingMessage || !this.chatInput.value.trim()) return;

    this.isSendingMessage = true;

    const content = this.chatInput.value.trim();

    try {
      const data = await ChatServices.sendMessage(
        this.currentConversationId,
        content
      );

      this.appendMessage(data.message);

      this.chatInput.value = "";
    } catch (err) {
      console.error("Error sending message: ", err);
    } finally {
      this.isSendingMessage = false;
    }
  }

  appendMessage(message) {
    const userId = this.chatContainer.dataset.userId;

    const messageDiv = document.createElement("div");

    messageDiv.className = `message ${
      message.senderId === parseInt(userId) ? "send" : "received"
    }`;

    messageDiv.innerHTML = `<div class = "message-content"> ${
      message.content
    } </div>
                            <div class = "message-time"> ${new Date(
                              message.sentAt
                            ).toLocaleTimeString()} </div>`;

    this.messageList.appendChild(messageDiv);

    this.messageList.scrollTop = this.messageList.scrollHeight;
  }

  initEventListeners() {
    this.sendButton.addEventListener("click", () => this.sendMessage());

    this.chatInput.addEventListener("keypress", (e) => {
      if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        this.sendMessage();
      }
    });
  }
}
