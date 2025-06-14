// Notification system
class NotificationManager {
    constructor() {
        this.container = this.createContainer();
        this.init();
    }

    init() {
        // Handle TempData messages
        this.handleTempDataMessages();
        
        // Handle form submission feedback
        this.handleFormFeedback();
    }

    createContainer() {
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }
        return container;
    }

    handleTempDataMessages() {
        // Check for success messages
        const successMessage = document.querySelector('[data-success-message]');
        if (successMessage) {
            this.show(successMessage.dataset.successMessage, 'success');
            successMessage.remove();
        }

        // Check for error messages
        const errorMessage = document.querySelector('[data-error-message]');
        if (errorMessage) {
            this.show(errorMessage.dataset.errorMessage, 'error');
            errorMessage.remove();
        }

        // Check for info messages
        const infoMessage = document.querySelector('[data-info-message]');
        if (infoMessage) {
            this.show(infoMessage.dataset.infoMessage, 'info');
            infoMessage.remove();
        }
    }

    handleFormFeedback() {
        // Handle AJAX form submissions
        document.addEventListener('submit', async (e) => {
            const form = e.target;
            if (!form.dataset.ajax) return;

            e.preventDefault();
            
            try {
                const formData = new FormData(form);
                const response = await fetch(form.action, {
                    method: form.method,
                    body: formData,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                if (response.ok) {
                    this.show('Operation completed successfully!', 'success');
                    if (form.dataset.redirectUrl) {
                        setTimeout(() => {
                            window.location.href = form.dataset.redirectUrl;
                        }, 1500);
                    }
                } else {
                    this.show('An error occurred. Please try again.', 'error');
                }
            } catch (error) {
                this.show('Network error. Please check your connection.', 'error');
            }
        });
    }

    show(message, type = 'info', duration = 5000) {
        const toast = this.createToast(message, type);
        this.container.appendChild(toast);

        // Animate in
        setTimeout(() => {
            toast.classList.add('show');
        }, 100);

        // Auto dismiss
        setTimeout(() => {
            this.dismiss(toast);
        }, duration);

        return toast;
    }

    createToast(message, type) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        
        const icons = {
            success: '✓',
            error: '✕',
            warning: '⚠',
            info: 'ℹ'
        };

        toast.innerHTML = `
            <div class="toast-header">
                <span class="toast-icon">${icons[type] || icons.info}</span>
                <strong class="toast-title">${this.getTitle(type)}</strong>
                <button type="button" class="btn-close" aria-label="Close">×</button>
            </div>
            <div class="toast-body">${message}</div>
        `;

        // Handle close button
        const closeBtn = toast.querySelector('.btn-close');
        closeBtn.addEventListener('click', () => this.dismiss(toast));

        return toast;
    }

    dismiss(toast) {
        toast.classList.add('hide');
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }

    getTitle(type) {
        const titles = {
            success: 'Success',
            error: 'Error',
            warning: 'Warning',
            info: 'Information'
        };
        return titles[type] || titles.info;
    }
}

// Initialize notification manager
document.addEventListener('DOMContentLoaded', () => {
    window.notificationManager = new NotificationManager();
});