$(document).ready(function() {
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
            success: function(response) {
                // Handle success, e.g., update the message list
                $('#messageContent').val(''); // Clear the input
                scrollToBottom(); // Ensure the view scrolls to the bottom after sending a message
            },
            error: function(error) {
                console.error('Error sending message:', error);
            }
        });
    }

    // Click event for the send button
    sendButton.addEventListener('click', function(event) {
        event.preventDefault();
        sendMessage(); // Call sendMessage function
    });

    // Keydown event for the message content area
    messageContent.addEventListener('keydown', function(event) {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault(); // Prevent new line on Enter key
            sendMessage(); // Send message
            messageContent.focus(); // Ensure focus remains on the input
        }
    });

    // Set focus on message content area when loading
    messageContent.focus();
});
