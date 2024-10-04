// customClock.js

function updateTime() {
    const now = new Date();
    const formattedTime = now.toLocaleString('vi-VN', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
    });
    document.getElementById('currentTime').innerHTML = formattedTime;
}

// Update time every second
setInterval(updateTime, 1000);

// Initialize time on page load
updateTime();