import { ChatService } from "./ChatService.js";

export class LecturerChat extends ChatService
{
    constructor()
    {
        super();
        this.initializeEventListeners();
    }


    // Initialize event listeners
    initializeEventListeners() 
    {
        // Show chat popup
        document.getElementById("open-chat-popup")?.addEventListener("click", () => {
            document.getElementById("chat-popup-container").style.display = "block";
            document.getElementById("chats-list-view").style.display = "block";
            document.getElementById("chat-messages-view").style.display = "none";
            this.loadActiveChats();
        });

        // show new chat view
        document.getElementById("new-chat-toggle")?.addEventListener("click", () => {
            document.getElementById("chat-popup-container").style.display = "block";
            document.getElementById("chats-list-view").style.display = "none";
            document.getElementById("new-chat-view").style.display = "block";
            this.loadEnrolledStudents();
        });

        // close chat popup
        document.getElementById("close-chat-popup")?.addEventListener("click", ()=> 
            {
              document.getElementById("chat-popup-container").style.display = "none";
        });

        // back to chat list button
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

        // close new chat popup
        const closeButtons = document.getElementsByClassName("close-btn");

        Array.from(closeButtons).forEach(button => 
        {
          button?.addEventListener("click", () => 
          {
            document.getElementsById("new-chat-view").style.display = "none";
            document.getElementsById("chat-popup-container").style.display = "none";
            this.currentConversationId = null;
            this.currentReceiverId = null;
          });
      
        });

        // send message button click event
        this.sendButton?.addEventListener("click", async () => 
        {
            if(this.isSendingMessage) return;
            this.isSendingMessage = true;
            try
            {
                await this.sendMessage();
                await this.loadActiveChats();
            }
            finally
            {
                this.isSendingMessage = false;
            }

        });

        // send message on enter key press
        this.chatInput?.addEventListener("keypress", async (event) => 
            {

                if (event.key === "Enter" && !event.shiftKey) 
                {
                    if(this.isSendingMessage) return;
                    this.isSendingMessage = true;
                    try
                    {
                        await this.sendMessage();
                    }
                    finally
                    {
                        this.isSendingMessage = false;
                    }
                }
        });
    }

}

// Initialize the chat when the page loads
document.addEventListener("DOMContentLoaded", () => {
    const chat = new LecturerChat();
});