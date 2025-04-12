import { ChatService } from "./ChatService.js";

export class StudentChat extends ChatService
{
  constructor()
  {
      super();
      this.isSendingMessage = false;
      this.initializeEventListeners();
  }


  initializeEventListeners()
  {
    // Send Message
    this.sendButton?.addEventListener("click", async ()=> 
    {
        if(this.isSendingMessage) return;
        this.isSendingMessage = true;
        try
        {
          await this.sendMessage();
          this.loadActiveChats();
        }
        finally
        {
          this.isSendingMessage = false;
        }
    });

    // Send Message on Enter Key Press
    this.chatInput?.addEventListener("keypress", async (event)=> 
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

    // Show Chat Popup
    document.getElementById("open-chat-popup")?.addEventListener("click", ()=> 
    {
      document.getElementById("chat-popup-container").style.display = "block";
      document.getElementById("chats-list-view").style.display = "block";
      document.getElementById("chat-messages-view").style.display = "none";
      this.loadActiveChats();
    });
    
    //   close chat popup
    document.getElementById("close-chat-popup")?.addEventListener("click", ()=> 
    {
      document.getElementById("chat-popup-container").style.display = "none";
    });


    // open New Chat Popup
    document.getElementById("new-chat-toggle")?.addEventListener("click", () => 
    {
      document.getElementById("chat-popup-container").style.display = "block";
      document.getElementById("chats-list-view").style.display = "none";
      document.getElementById("new-chat-view").style.display = "block";
      this.loadLecturers();
    });


    // back to chat list view
    document.getElementById("back-to-chats")?.addEventListener("click", () => 
    {
      document.getElementById("chat-messages-view").style.display = "none";
      document.getElementById("chats-list-view").style.display = "block";
      this.currentConversationId = null;
      this.currentReceiverId = null;
      this.loadActiveChats();
    });


    document.getElementById("back-to-chats-new")?.addEventListener("click", () => 
    {
      document.getElementById("new-chat-view").style.display = "none";
      document.getElementById("chats-list-view").style.display = "block";
      this.loadActiveChats();
    });

  // Close New Chat Popup
    document.getElementById("close-newchat-popup")?.addEventListener("click", () => 
    {
      document.getElementById("new-chat-view").style.display = "none";
      document.getElementById("chat-popup-container").style.display = "none";
      this.currentConversationId = null;
      this.currentReceiverId = null;
    });


  }

}

document.addEventListener("DOMContentLoaded", ()=> 
{
  const studentChat = new StudentChat();
})