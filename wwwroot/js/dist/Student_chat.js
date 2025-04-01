// (function () {
//   'use strict';

//   class ChatService {
//     static async loadLecturers() {
//       const response = await fetch("Chat/GetLecturers");

//       console.log(
//         "Fetching Lecturers from Endpoint, response): ",
//         response.json()
//       );

//       if (!response.ok) {
//         throw new Error("Failed to load lecturers");
//       }

//       return response.json();
//     }

//     static async startChat(lecturerId) {
//       const response = await fetch("/Chat/StartChat", {
//         method: "POST",
//         headers: {
//           "Content-Type": "application/json",
//         },
//         body: JSON.stringify({ lecturerId }),
//       });

//       if (!response.ok) {
//         throw new Error("Failed to start chat");
//       }

//       return response.json();
//     }

//     static async loadMessages(conversationId) {
//       const response = await fetch(`/Chat/GetMessages/${conversationId}`);

//       if (!response.ok) {
//         throw new Error("Failed to load messages");
//       }

//       return response.json();
//     }

//     static async sendMessage(conversationId, content) {
//       const response = await fetch("/Chat/SendMessage", {
//         method: "POST",
//         headers: {
//           "Content-Type": "application/json",
//         },
//         body: JSON.stringify({ conversationId, content, replyToMessage: null }),
//       });

//       if (!response.ok) {
//         throw new Error("Failed to send message");
//       }

//       return response.json();
//     }
//   }

//   class Chat {
//     constructor(chatContainer, messageList, chatInput, sendButton) {
//       this.chatContainer = chatContainer;

//       this.messageList = messageList;

//       this.chatInput = chatInput;

//       this.sendButton = sendButton;

//       this.lecturerDropdown = lecturerDropdown;

//       this.conversationId = null;

//       this.isSendingMessage = false;

//       this.initEventListeners();

//       this.initSignalR();
//     }


//     openChatPopup() 
//     {

//       console.log("Opening chat popup...");

//       if (this.chatContainer) 
//       {
//         this.chatContainer.style.display = "block";

//         if (this.lecturerDropdown) 
//         {
//           this.loadLecturers(this.lecturerDropdown);
//         } 
//         else 
//         {
//           console.error("Lecturer dropdown not initialized");
//         }
//       } 
//       else 
//       {
//         console.error("Chat container not initialized");
//       }

//     }


//     closeChatPopup() 
//     {
//       this.chatContainer.style.display = "none";
//     }


//     initSignalR() 
//     {

//       this.connection = new signalR.HubConnectionBuilder()
//         .withUrl("/chatHub")
//         .withAutomaticReconnect()
//         .build();

//       this.connection
//         .start()
//         .then(() => console.log("SignalR connected"))
//         .catch((err) => console.error("SignalR Connection Error:", err));

//       this.connection.on("ReceiveMessage", (message) => 
//       {
//         if (message.conversationId === this.conversationId) 
//         {
//           this.appendMessage(message);
//         }
//       });

//     }

//     async loadLecturers(lecturerDropdown) 
//     {

//       try 
//       {
//         const lecturers = await ChatService.loadLecturers();

//         lecturerDropdown.innerHTML =
//           '<option value=""> Select a Lecturer </option>';

//         lecturers.forEach((lecturer) => {
//           const option = document.createElement("option");

//           option.value = lecturer.id;
//           option.textContent = `${lecturer.name}`;

//           lecturerDropdown.appendChild(option);

//           console.log(lecturers);
//         });
//       } 
//       catch (err) 
//       {
//         console.error("Error loading lecturer:", err);
//       }

//     }


//     async startChat(lecturerId) 
//     {

//       try 
//       {
//         console.log("Starting chat with lecturer:", lecturerId);
//         const data = await ChatService.startChat(lecturerId);

//         if (!data.success) 
//         {
//           throw new Error(data.error || "Failed to start chat");
//         }

//         this.conversationId = data.conversationId;
//         this.chatInput.disabled = false;
//         this.sendButton.disabled = false;

//         this.chatMessages.style.display = "block";

//         // Update the chat title with the lecturer's name
//         if (this.chatTitle && this.lecturerDropdown) 
//         {

//           const selectedOption =
//             this.lecturerDropdown.options[this.lecturerDropdown.selectedIndex];
//           this.chatTitle.textContent = `Chat with ${selectedOption.text}`;

//         }

//         // Load existing messages
//         await this.loadMessages(this.conversationId);

//         // Join the SignalR group for this conversation
//         await this.connection.invoke( "JoinConversation", this.conversationId.toString() );
//       } 
//       catch (error) 
//       {
//         console.error("Error starting chat:", error);
//         alert("Failed to start chat: " + error.message);
//       }

//     }


//     async loadMessage()
//      {

//       try 
//       {
//         const messages = await ChatService.loadMessages( this.currentConversationId );

//         console.log("Loaded messages:", messages);

//         this.messageList.innerHTML = "";

//         messages.forEach((message) => this.appendMessage(message));
//       } 
//       catch (err) 
//       {
//         console.error("Error loading messages: ", err);
//       }

//     }


//     async sendMessage() 
//     {

//       if (this.isSendingMessage || !this.chatInput.value.trim()) return;

//       this.isSendingMessage = true;

//       const content = this.chatInput.value.trim();

//       try 
//       {
//         const data = await ChatServices.sendMessage
//         (

//           this.currentConversationId,
//           content
        
//         );

//         this.appendMessage(data.message);

//         this.chatInput.value = "";
//       } 
//       catch (err) 
//       {
//         console.error("Error sending message: ", err);
//       } 
//       finally 
//       {
//         this.isSendingMessage = false;
//       }

//     }


//     appendMessage(message) 
//     {

//       const userId = this.chatContainer.dataset.userId;

//       const messageDiv = document.createElement("div");

//       messageDiv.className = `message ${ message.senderId === parseInt(userId) ? "send" : "received" }`;

//       messageDiv.innerHTML = `<div class = "message-content"> ${ message.content } </div>
//                             <div class = "message-time"> ${new Date( message.sentAt ).toLocaleTimeString()} </div>`;

//       this.messageList.appendChild(messageDiv);

//       this.messageList.scrollTop = this.messageList.scrollHeight;

//     }


//     initEventListeners() 
//     {

//       this.sendButton.addEventListener("click", () => this.sendMessage());

//       this.chatInput.addEventListener("keypress", function (e) 
//       {

//         if (e.key === "Enter" && !e.shiftKey) 
//           {
            
//           e.preventDefault();
//           this.sendMessage();
        
//         }

//       });
      
//     }
//   }

//   document.addEventListener("DOMContentLoaded", function () {
//     // Get DOM elements
//     const chatContainer = document.getElementById("chat-popup-container");
//     const messageList = document.getElementById("message-list");
//     const chatInput = document.getElementById("chat-input");
//     const sendButton = document.getElementById("send-chat-message");
//     const lecturerDropdown = document.getElementById("lecturer-dropdown");
//     const chatTitle = document.getElementById("chat-title");
//     const chatMessages = document.getElementById("chat-messages");

//     // Debug DOM elements
//     console.log("Chat container:", chatContainer);
//     console.log("Lecturer dropdown:", lecturerDropdown);

//     if (!chatContainer || !lecturerDropdown) {
//       console.error("Required DOM elements are missing.");
//       return;
//     }

//     // Initialize chat
//     const chat = new Chat(
//       chatContainer,
//       messageList,
//       chatInput,
//       sendButton,
//       lecturerDropdown,
//       chatTitle,
//       chatMessages
//     );

//     // Handle chat button click
//     try {
//       document
//         .getElementById("open-chat-popup")
//         ?.addEventListener("click", () => chat.openChatPopup());
//     } catch (error) {
//       console.error("Error adding event listener to open chat button:", error);
//     }

//     // Handle close button click
//     // try {
//     //   document
//     //     .getElementById("close-chat-popup")
//     //     ?.addEventListener("click", () => chat.closeChatPopup());
//     // } catch (error) {
//     //   console.error("Error adding event listener to close chat button:", error);
//     // }

//     // Handle lecturer selection
//     lecturerDropdown?.addEventListener("change", async function() {
//       const lecturerId = parseInt(this.value, 10);
//       console.log("Selected lecturer ID:", lecturerId);

//       if (!lecturerId || isNaN(lecturerId)) {
//         console.warn("Invalid lecturer ID selected");
//         return;
//       }

//       await chat.startChat(lecturerId);
//     });
//   });

// })();
// //# sourceMappingURL=Student_chat.js.map



// document.addEventListener("DOMContentLoaded", function () {

// }