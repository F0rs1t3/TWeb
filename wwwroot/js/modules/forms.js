// Form validation and enhancement
class FormValidator {
    constructor(formSelector) {
        this.form = document.querySelector(formSelector);
        this.init();
    }

    init() {
        if (!this.form) return;
        
        this.setupValidation();
        this.setupFileUpload();
        this.setupPasswordStrength();
    }

    setupValidation() {
        const inputs = this.form.querySelectorAll('input, select, textarea');
        
        inputs.forEach(input => {
            input.addEventListener('blur', () => this.validateField(input));
            input.addEventListener('input', () => this.clearErrors(input));
        });

        this.form.addEventListener('submit', (e) => {
            if (!this.validateForm()) {
                e.preventDefault();
            }
        });
    }

    validateField(field) {
        const value = field.value.trim();
        const fieldName = field.name;
        let isValid = true;
        let errorMessage = '';

        // Required field validation
        if (field.hasAttribute('required') && !value) {
            isValid = false;
            errorMessage = `${this.getFieldLabel(field)} is required.`;
        }

        // Email validation
        if (field.type === 'email' && value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(value)) {
                isValid = false;
                errorMessage = 'Please enter a valid email address.';
            }
        }

        // Password validation
        if (field.type === 'password' && fieldName === 'Password' && value) {
            if (value.length < 6) {
                isValid = false;
                errorMessage = 'Password must be at least 6 characters long.';
            }
        }

        // Confirm password validation
        if (fieldName === 'ConfirmPassword' && value) {
            const passwordField = this.form.querySelector('input[name="Password"]');
            if (passwordField && value !== passwordField.value) {
                isValid = false;
                errorMessage = 'Passwords do not match.';
            }
        }

        // Number validation
        if (field.type === 'number' && value) {
            const min = field.getAttribute('min');
            const max = field.getAttribute('max');
            const numValue = parseInt(value);

            if (min && numValue < parseInt(min)) {
                isValid = false;
                errorMessage = `Value must be at least ${min}.`;
            }
            if (max && numValue > parseInt(max)) {
                isValid = false;
                errorMessage = `Value must not exceed ${max}.`;
            }
        }

        this.showFieldValidation(field, isValid, errorMessage);
        return isValid;
    }

    validateForm() {
        const inputs = this.form.querySelectorAll('input[required], select[required], textarea[required]');
        let isFormValid = true;

        inputs.forEach(input => {
            if (!this.validateField(input)) {
                isFormValid = false;
            }
        });

        return isFormValid;
    }

    showFieldValidation(field, isValid, errorMessage) {
        const fieldContainer = field.closest('.form-group') || field.parentElement;
        const existingError = fieldContainer.querySelector('.invalid-feedback');

        // Remove existing validation classes and messages
        field.classList.remove('is-valid', 'is-invalid');
        if (existingError) {
            existingError.remove();
        }

        if (!isValid && errorMessage) {
            field.classList.add('is-invalid');
            const errorDiv = document.createElement('div');
            errorDiv.className = 'invalid-feedback';
            errorDiv.textContent = errorMessage;
            fieldContainer.appendChild(errorDiv);
        } else if (field.value.trim()) {
            field.classList.add('is-valid');
        }
    }

    clearErrors(field) {
        field.classList.remove('is-invalid');
        const fieldContainer = field.closest('.form-group') || field.parentElement;
        const existingError = fieldContainer.querySelector('.invalid-feedback');
        if (existingError) {
            existingError.remove();
        }
    }

    getFieldLabel(field) {
        const label = this.form.querySelector(`label[for="${field.id}"]`);
        return label ? label.textContent.replace('*', '').trim() : field.name;
    }

    setupFileUpload() {
        const fileInputs = this.form.querySelectorAll('input[type="file"]');
        
        fileInputs.forEach(input => {
            const container = input.closest('.form-group');
            if (!container) return;

            // Create custom file upload UI
            const customUpload = document.createElement('div');
            customUpload.className = 'file-upload-container';
            customUpload.innerHTML = `
                <label for="${input.id}" class="file-upload-label">
                    <span class="file-upload-text">Click to select a file or drag and drop</span>
                </label>
                <div class="file-preview" style="display: none;"></div>
            `;

            input.style.display = 'none';
            container.appendChild(customUpload);

            const label = customUpload.querySelector('.file-upload-label');
            const preview = customUpload.querySelector('.file-preview');

            // Handle file selection
            input.addEventListener('change', (e) => {
                const file = e.target.files[0];
                if (file) {
                    this.showFilePreview(file, preview, label);
                }
            });

            // Handle drag and drop
            label.addEventListener('dragover', (e) => {
                e.preventDefault();
                label.classList.add('drag-over');
            });

            label.addEventListener('dragleave', () => {
                label.classList.remove('drag-over');
            });

            label.addEventListener('drop', (e) => {
                e.preventDefault();
                label.classList.remove('drag-over');
                
                const files = e.dataTransfer.files;
                if (files.length > 0) {
                    input.files = files;
                    this.showFilePreview(files[0], preview, label);
                }
            });
        });
    }

    showFilePreview(file, previewContainer, label) {
        const fileName = file.name;
        const fileSize = (file.size / 1024 / 1024).toFixed(2);
        
        // Update label text
        label.querySelector('.file-upload-text').textContent = `Selected: ${fileName} (${fileSize} MB)`;
        
        // Show image preview if it's an image
        if (file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = (e) => {
                previewContainer.innerHTML = `
                    <img src="${e.target.result}" alt="Preview" style="max-width: 200px; max-height: 200px; margin-top: 10px; border-radius: 4px;">
                `;
                previewContainer.style.display = 'block';
            };
            reader.readAsDataURL(file);
        }
    }

    setupPasswordStrength() {
        const passwordField = this.form.querySelector('input[name="Password"]');
        if (!passwordField) return;

        const strengthIndicator = document.createElement('div');
        strengthIndicator.className = 'password-strength';
        strengthIndicator.innerHTML = `
            <div class="strength-bar">
                <div class="strength-fill"></div>
            </div>
            <div class="strength-text">Password strength: <span class="strength-level">Weak</span></div>
        `;

        passwordField.parentElement.appendChild(strengthIndicator);

        passwordField.addEventListener('input', () => {
            const strength = this.calculatePasswordStrength(passwordField.value);
            this.updatePasswordStrength(strengthIndicator, strength);
        });
    }

    calculatePasswordStrength(password) {
        let score = 0;
        
        if (password.length >= 8) score += 1;
        if (password.length >= 12) score += 1;
        if (/[a-z]/.test(password)) score += 1;
        if (/[A-Z]/.test(password)) score += 1;
        if (/[0-9]/.test(password)) score += 1;
        if (/[^A-Za-z0-9]/.test(password)) score += 1;

        return Math.min(score, 5);
    }

    updatePasswordStrength(indicator, strength) {
        const fill = indicator.querySelector('.strength-fill');
        const text = indicator.querySelector('.strength-level');
        
        const levels = ['Very Weak', 'Weak', 'Fair', 'Good', 'Strong'];
        const colors = ['#dc3545', '#fd7e14', '#ffc107', '#20c997', '#28a745'];
        
        const level = Math.max(0, strength - 1);
        const percentage = (strength / 5) * 100;
        
        fill.style.width = `${percentage}%`;
        fill.style.backgroundColor = colors[level] || colors[0];
        text.textContent = levels[level] || levels[0];
        text.style.color = colors[level] || colors[0];
    }
}

// Initialize form validation when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Initialize validation for all forms
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        new FormValidator(`#${form.id}`);
    });
});