import { ChatService } from "./ChatService.js";

export class StudentChat extends ChatService
{
    constructor()
    {
        super();
        // this.loadLecturers = this.loadLecturers.bind(this);
        // this.sendMessage = this.sendMessage.bind(this);
        // this.initializeEventListeners = this.initializeEventListeners.bind(this);
        this.initializeEventListeners();
    }


  // Load Lecturers


  initializeEventListeners()
  {
    // Handle recipient selection
    this.lecturerDropdown?.addEventListener("change", async (e)=> 
    {
        const lecturerId = parseInt(e.target.value, 10);

        this.currentReceiverId = lecturerId;

        if (!lecturerId || isNaN(lecturerId)) 
        {
            console.warn("Invalid recipient selected:", e.target.value);
            return;
        }

        try 
        {
            console.log("Starting chat with recipient:", lecturerId);

            const response = await fetch("/Chat/StartChat", 
                {
                    method: "POST",
                    headers: 
                    {
                        "Content-Type": "application/json",
                        "Accept": "application/json"
                    },
                    body: JSON.stringify(
                        { 
                        lecturerId: lecturerId,
                        isPrivateChat: true
                        }),
                    credentials: 'include' // Include cookies in the request
            });

            if (!response.ok) 
            {
                const errorData = await response.json();
                throw new Error(errorData.error || `Failed to start chat: ${response.status}`);
            }

            const data = await response.json();
            console.log("Chat started successfully:", data);

            this.currentConversationId = data.conversationId;
            this.chatInput.disabled = false;
            this.sendButton.disabled = false;
            
            this.chatMessages.style.display = "block";
            const selectedOption = document.getElementById("lecturer-dropdown").options[document.getElementById("lecturer-dropdown").selectedIndex];
            document.getElementById("chat-title").textContent = `Chat with ${selectedOption.text}`;

            //debugging
            console.log(`conversationId: ${this.currentConversationId}`);
            console.log(`receiverId: ${this.currentReceiverId}`);

            await this.loadMessages(this.currentConversationId);

            await this.connection.invoke("JoinConversation", this.currentConversationId.toString());
        } 
        catch (error) 
        {
            console.error("Error starting chat:", error);
            alert(`Failed to start chat: ${error.message}`);
        }
});



  // Send Message
  this.sendButton?.addEventListener("click", ()=> this.sendMessage());

    // Send Message on Enter Key Press
  this.chatInput?.addEventListener("keypress", (event)=> 
  {
      if (event.key === "Enter" && !event.shiftKey)
    {
          event.preventDefault();
          this.sendMessage();
        //   await loadMessages();
      }
  });

  // Show Chat Popup
  document.getElementById("open-chat-popup")?.addEventListener("click", ()=> 
  {
    document.getElementById("chat-popup-container").style.display = "block";
      this.loadLecturers();
        
  });
  //   close chat popup
  document.getElementById("close-chat-popup")?.addEventListener("click", ()=> 
  {
    document.getElementById("chat-popup-container").style.display = "none";
  });

  }

}

document.addEventListener("DOMContentLoaded", ()=> 
{
    const studentChat = new StudentChat();
})