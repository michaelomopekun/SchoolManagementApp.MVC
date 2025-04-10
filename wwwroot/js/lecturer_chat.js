import { ChatService } from "./ChatService.js";

export class LecturerChat extends ChatService
{
    constructor()
    {
        super();
        this.initializeEventListeners();
    }



    initializeEventListeners() 
    {
        document.getElementById("open-chat-popup")?.addEventListener("click", () => {
            this.chatContainer.style.display = "block";
            document.getElementById("chats-list-view").style.display = "block";
            document.getElementById("chat-messages-view").style.display = "none";
            this.loadActiveChats();
        });

        document.getElementById("new-chat-toggle")?.addEventListener("click", () => {
            this.chatContainer.style.display = "block";
            document.getElementById("chats-list-view").style.display = "none";
            document.getElementById("new-chat-view").style.display = "block";
            this.loadEnrolledStudents();
        });

        document.getElementById("back-to-chats")?.addEventListener("click", () => {
            document.getElementById("chat-messages-view").style.display = "none";
            document.getElementById("chats-list-view").style.display = "block";
            this.currentConversationId = null;
            this.currentReceiverId = null;
            this.loadActiveChats();
        });

        document.getElementById("back-to-chats-new")?.addEventListener("click", () => {
            document.getElementById("new-chat-view").style.display = "none";
            document.getElementById("chats-list-view").style.display = "block";
            this.loadActiveChats();
        });

        // this.sendButton?.addEventListener("click", () => this.sendMessage());

        this.chatInput?.addEventListener("keypress", (event) => {
            if (event.key === "Enter" && !event.shiftKey) {
                event.preventDefault();
                this.sendMessage();
            }
        });
    }

}

// Initialize the chat when the page loads
document.addEventListener("DOMContentLoaded", () => {
    const chat = new LecturerChat();
});