
document.addEventListener("DOMContentLoaded", async () => {
    const notificationList = document.getElementById("notification-list");
    const notificationCount = document.getElementById("notification-count");
    const markAllAsReadBtn = document.getElementById("markAllAsRead");

    async function loadNotifications() {
        try {
            const response = await fetch("/api/Notification/GetNotifications");
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const notifications = await response.json();
            console.log("Notifications loaded:", notifications); // Debug log

            if (notifications && notifications.length > 0) {
                notificationList.innerHTML = "";
                notifications.forEach(notification => {
                    const listItem = document.createElement("li");
                    listItem.className = `notification-item dropdown-item ${notification.isRead ? 'read' : 'unread'}`;
                    listItem.dataset.notificationId = notification.id;
                    listItem.innerHTML = `
                        <div class="d-flex justify-content-between align-items-center">
                            <span class="notification-message">${notification.message}</span>
                            <small class="text-muted ms-2">${new Date(notification.generatedDate).toLocaleString()}</small>
                        </div>
                    `;

                    // Add click handler for individual notification
                    listItem.addEventListener('click', async (e) => {
                        e.preventDefault();
                        e.stopPropagation();

                        if (!notification.isRead) {
                            try {
                                const response = await fetch(`/api/Notification/MarkAsRead/${notification.id}`, {
                                    method: 'POST',
                                    headers: {
                                        'X-Requested-With': 'XMLHttpRequest'
                                    }
                                });

                                if (!response.ok) {
                                    throw new Error(`HTTP error! status: ${response.status}`);
                                }

                                // Update UI
                                listItem.classList.remove('unread');
                                listItem.classList.add('read');
                                notification.isRead = true;

                                updateNotificationCount(notifications);
                            } catch (error) {
                                console.error("Error marking notification as read:", error);
                            }
                        }
                    });

                    notificationList.appendChild(listItem);
                });

                updateNotificationCount(notifications);
            } else {
                notificationList.innerHTML = '<li class="dropdown-item text-muted">No notifications</li>';
                notificationCount.style.display = "none";
                markAllAsReadBtn.style.display = "none";
            }
        } catch (error) {
            console.error("Error fetching notifications:", error);
            notificationList.innerHTML = '<li class="dropdown-item text-danger">Error loading notifications</li>';
        }
    }

    function updateNotificationCount(notifications) {
        const unreadCount = notifications.filter(n => !n.isRead).length;
        if (unreadCount > 0) {
            notificationCount.innerText = unreadCount;
            notificationCount.style.display = "inline";
            markAllAsReadBtn.style.display = "block";
        } else {
            notificationCount.style.display = "none";
            markAllAsReadBtn.style.display = "none";
        }
    }

    // Add event listener for Mark All as Read button
    markAllAsReadBtn.addEventListener('click', async (e) => {
        e.preventDefault();
        e.stopPropagation();

        try {
            const response = await fetch("/api/Notification/MarkAllAsRead", {
                method: "POST",
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // Refresh notifications after marking all as read
            await loadNotifications();
        } catch (error) {
            console.error("Error marking all notifications as read:", error);
        }
    });

    // Load notifications initially
    await loadNotifications();

    // Refresh notifications every 30 seconds
    setInterval(loadNotifications, 30000);
});



document.getElementById("notificationBell").addEventListener("click", async () => {
    const notificationCount = document.getElementById("notification-count");

    try {
        await fetch("/api/Notification/MarkAllAsRead", { method: "POST" });
        notificationCount.innerText = "0";
        notificationCount.style.display = "none";
    } catch (error) {
        console.error("Error marking notifications as read:", error);
    }
});
