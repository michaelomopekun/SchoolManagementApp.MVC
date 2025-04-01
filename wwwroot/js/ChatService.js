export default class ChatService {
  static async loadLecturers() {
    const response = await fetch("Chat/GetLecturers");

    console.log(
      "Fetching Lecturers from Endpoint, response): ",
      response.json()
    );

    if (!response.ok) {
      throw new Error("Failed to load lecturers");
    }

    return response.json();
  }

  static async startChat(lecturerId) {
    const response = await fetch("/Chat/StartChat", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ lecturerId }),
    });

    if (!response.ok) {
      throw new Error("Failed to start chat");
    }

    return response.json();
  }

  static async loadMessages(conversationId) {
    const response = await fetch(`/Chat/GetMessages/${conversationId}`);

    if (!response.ok) {
      throw new Error("Failed to load messages");
    }

    return response.json();
  }

  static async sendMessage(conversationId, content) {
    const response = await fetch("/Chat/SendMessage", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ conversationId, content, replyToMessage: null }),
    });

    if (!response.ok) {
      throw new Error("Failed to send message");
    }

    return response.json();
  }
}
