document.addEventListener('DOMContentLoaded', function () {
    const messagesContainer = document.querySelector('.messages');
    if (messagesContainer) {
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
});

document.getElementById('imageUpload').addEventListener('change', function () {
    const fileInput = this;
    const file = fileInput.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const preview = document.createElement('div'); // Create a container for the image and the remove button
            const img = document.createElement('img');
            const removeButton = document.createElement('button');

            // Set image attributes
            img.src = e.target.result;
            img.style.width = '100px'; // Thumbnail size
            img.style.height = '100px'; // Thumbnail size
            img.style.borderRadius = '5px'; // Rounded corners
            img.style.marginBottom = '10px'; // Space above the image

            // Set remove button attributes
            removeButton.innerHTML = '✖'; // X icon
            removeButton.classList.add('remove-image-button');
            removeButton.onclick = function () {
                preview.remove(); // Remove the image and button when clicked
                fileInput.value = ''; // Clear the file input to remove the attachment
            };

            // Create a container for the image and button
            preview.classList.add('image-preview'); // Class for future reference
            preview.style.position = 'relative'; // Enable positioning of the button
            preview.appendChild(img);
            preview.appendChild(removeButton);

            // Remove existing preview
            const existingPreview = document.querySelector('.image-preview');
            if (existingPreview) {
                existingPreview.remove();
            }

            document.querySelector('.messages').appendChild(preview);
        }
        reader.readAsDataURL(file);
    }
});

// Get the modal elements
var modal = document.getElementById("imageModal");
var modalImg = document.getElementById("modalImage");
var captionText = document.getElementById("caption");
var closeModal = document.getElementById("closeModal");

// Function to show the image in a modal
function showImage(img) {
    modal.style.display = "block";
    modalImg.src = img.src;
    captionText.innerHTML = img.alt || "Zoomed Image"; // Show alt text as caption if available
}

// Attach the click event to all images with the class 'message-image'
document.querySelectorAll('.message-image').forEach(img => {
    img.addEventListener('click', function () {
        showImage(this);
    });
});

// Close the modal when the close button is clicked
closeModal.onclick = function () {
    modal.style.display = "none";
}

// Close the modal when clicking anywhere outside the image content
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

$(document).ready(function () {
    var messageContent = document.getElementById('messageContent');
    var messageForm = document.querySelector('form.message-form');
    var sendButton = document.querySelector('.send-button');

    // Function to submit the form
    function sendMessage() {
        // Ensure that the form is correctly selected
        if (!messageForm) {
            console.error('Message form not found.');
            return;
        }

        // Create a FormData object using the plain JavaScript form element
        var formData = new FormData(messageForm);

        $.ajax({
            url: '/Message/SendMessage',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                // Handle success, e.g., update the message list
                $('#messageContent').val(''); // Clear the input
                const existingPreview = document.querySelector('.image-preview');
                if (existingPreview) {
                    existingPreview.remove(); // Remove the image preview element
                }
                scrollToBottom(); // Ensure the view scrolls to the bottom after sending a message
            },
            error: function (error) {
                console.error('Error sending message:', error);
            }
        });
    }

    // Click event for the send button
    sendButton.addEventListener('click', function (event) {
        event.preventDefault();
        sendMessage(); // Call sendMessage function
    });

    // Keydown event for the message content area
    messageContent.addEventListener('keydown', function (event) {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault(); // Prevent new line on Enter key
            sendMessage(); // Send message
            messageContent.focus(); // Ensure focus remains on the input
        }
    });

    // Set focus on message content area when loading
    messageContent.focus();
});

function hideConversation(conversationId) {
    // Find the conversation item by its ID
    var conversationItem = document.querySelector(`.conversation-item[data-id='${conversationId}']`);

    if (conversationItem) {
        // Hide the conversation item visually
        conversationItem.style.display = 'none';

        // Optionally, send a request to the server to hide the conversation
        fetch('/Message/HideConversation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ conversationId: conversationId })
        })
            .then(response => response.json())
            .then(data => {
                if (!data.success) {
                    console.error('Error hiding conversation:', data);
                    // Optionally: show the item again if there was an error
                    conversationItem.style.display = 'block';
                }
            })
            .catch(error => {
                console.error('Error:', error);
                // Optionally: show the item again if there was an error
                conversationItem.style.display = 'block';
            });
    } else {
        console.error('Conversation item not found for ID:', conversationId);
    }
}
function scrollToBottom() {
    const messageList = document.querySelector('.messages');
    messageList.scrollTop = messageList.scrollHeight;
}
$(document).ready(function () {
    scrollToBottom();
});

$(function () {
    var chat = $.connection.chatHub;

    console.log(myUserId, conversationId)

    // Enable logging for debugging
    $.connection.hub.logging = true;

    chat.client.receiveMessage = function (convId, userId, senderName, senderAvatar,
        content, imagePath, sentAt, senderGender) {
        if (convId == conversationId) {
            addMessageToChat(convId, userId, senderName, senderAvatar, content,
                imagePath, sentAt, senderGender);
        }
    };

    // Start the connection
    $.connection.hub.start()
        .done(function () {
            console.log('Connected to SignalR hub');
            if (conversationId) {
                chat.server.joinConversation(conversationId)
                    .done(function () {
                        console.log('Joined conversation:', conversationId);
                    })
                    .fail(function (error) {
                        console.error('Failed to join conversation:', error);
                    });
            }
        })
        .fail(function (error) {
            console.error('Could not connect to SignalR hub:', error);
        });

    // Handle connection state changes
    $.connection.hub.stateChanged(function (change) {
        if (change.newState == $.signalR.connectionState.reconnecting) {
            console.log('Reconnecting...');
        } else if (change.newState == $.signalR.connectionState.connected) {
            console.log('Reconnected');
            if (conversationId) {
                chat.server.joinConversation(conversationId);
            }
        }
    });

    // Handle disconnection
    $.connection.hub.disconnected(function () {
        console.log('Disconnected. Attempting to reconnect...');
        setTimeout(function () {
            $.connection.hub.start();
        }, 5000);
    });

    function addMessageToChat(conversationId, userId, senderName, senderAvatar,
        content, imagePath, sentAt, senderGender) {
        const messageList = document.querySelector('.message-list');
        const messageItem = document.createElement('li');
        const color = getNameColor(userId, senderGender);
        const isOwnMessage = userId == myUserId;

        messageItem.className = `message-item ${ isOwnMessage ? "sent" : "received" }`;

        let messageHTML = `
            <img src="${senderAvatar}" alt="${senderName}'s Avatar" class="user-avatar" />
            <div style="display: flex; align-items: center;">
                <div style="flex: 1;">
                    <div class="message-info">
                        <a href="/Profile?maNhanVien=${userId})" style="color:${color}"><strong>${userId === myUserId ? senderName : senderName || "Admin"}</strong></a>
                        &nbsp;&nbsp;
                        <span class="message-time" style="font-size:12px; padding-left:10px; color:#aaa">${formatDate(new Date(sentAt))}</span>
                    </div>
                    <div class="message-content">
                        <span class="message-Content">${content ? escapeHtml(content) : ''}</span>
                        ${imagePath ? `<img src="${imagePath}" class="message-image" alt="Attached Image" />` : ''}
                    </div>
                </div>
            </div>
        `;

        messageItem.innerHTML = messageHTML;
        messageList.appendChild(messageItem);
        messageList.scrollTop = messageList.scrollHeight;
    }

    // Helper function to format date
    function formatDate(date) {
        const now = new Date();
        const timePart = date.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });

        if (date.toDateString() === now.toDateString()) {
            return "Today at " + timePart;
        } else if (date.toDateString() === new Date(now - 86400000).toDateString()) {
            return "Yesterday at " + timePart;
        } else {
            return date.toLocaleDateString() + " " + timePart;
        }
    }

    // Helper function to escape HTML
    function escapeHtml(text) {
        return text.replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    // Function to determine name color based on user and gender
    function getNameColor(senderId, gender) {

        if (senderId == "admin") {
            return "#8c32db"; // Color for admin    
        }
        if (senderId == myUserId) {
            return "#db6d3d"; // Color for self messages (orange)
        }
        
        switch (gender) {
            case 0: // Female
                return "pink";
            case 1: // Male
                return "#5a75ed";
            default:
                return "#74de62"; // Default color for other genders
        }
    }
});
$.connection.hub.disconnectTimeout = 30000; // 30 seconds
$.connection.hub.keepAliveInterval = 15000; // 15 seconds

/*
function loadGlobalChat() {
    $.ajax({
        url: '/Message/Index',
        type: 'GET',
        success: function (data) {
            $('.chat-container').html(data);
            scrollToBottom();
        },
        error: function (error) {
            console.error("Error loading global chat: ", error);
        }

    });
}

function loadConversation(conversationId) {
    $.ajax({
        url: '/Message/Index',
        type: 'GET',
        data: { conversationId: conversationId },
        success: function (data) {
            // Update the chat container with the response data
            $('.chat-container').html(data); // Make sure to use the correct selector
            scrollToBottom();
        },
        error: function (error) {
            console.error("Error loading conversation: ", error);
        }
    });
    scrollToBottom();
}

function startConversation(participantId) {
    $.ajax({
        url: '/Message/StartConversation',
        type: 'GET',
        data: { participantId: participantId },
        success: function (data) {
            // Update the message list with the response data
            $('.chat-container').html(data); // Ensure the correct selector is used
            scrollToBottom();
        },
        error: function (error) {
            console.error("Error starting conversation: ", error);
        }
    });
}
*/