document.getElementById('notificationBell').addEventListener('click', async function() {
    try {
        const response = await fetch('/api/Notification/MarkAllAsRead', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            credentials: 'include'
        });

        if (response.ok) {
            // Reset notification count badge
            const badge = document.getElementById('notification-count');
            if (badge) {
                badge.style.display = 'none';
                badge.textContent = '0';
            }

            // Update notification items visual state
            const notificationItems = document.querySelectorAll('.notification-item');
            notificationItems.forEach(item => {
                item.classList.remove('unread');
                item.classList.add('read');
            });
        } else {
            console.error('Failed to mark notifications as read');
        }
    } catch (error) {
        console.error('Error marking notifications as read:', error);
    }
});

// Function to update notification count
async function updateNotificationCount() {
    try {
        const response = await fetch('/api/Notification/UnreadCount');
        if (response.ok) {
            const count = await response.json();
            const badge = document.getElementById('notification-count');
            if (badge) {
                if (count > 0) {
                    badge.style.display = 'inline';
                    badge.textContent = count;
                } else {
                    badge.style.display = 'none';
                }
            }
        }
    } catch (error) {
        console.error('Error fetching notification count:', error);
    }
}

// Update count periodically
setInterval(updateNotificationCount, 30000); // Update every 30 seconds