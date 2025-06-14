// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
console.log('TWeb application loaded successfully')

// Auto-hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function() {
    const alerts = document.querySelectorAll('.alert-dismissible')
    alerts.forEach(function(alert) {
        setTimeout(function() {
            const bsAlert = new bootstrap.Alert(alert)
            bsAlert.close()
        }, 5000)
    })
})

// Handle logout
function logout() {
    const form = document.createElement('form')
    form.method = 'POST'
    form.action = '/Account/Logout'
    
    const token = document.querySelector('input[name="__RequestVerificationToken"]')
    if (token) {
        const hiddenToken = document.createElement('input')
        hiddenToken.type = 'hidden'
        hiddenToken.name = '__RequestVerificationToken'
        hiddenToken.value = token.value
        form.appendChild(hiddenToken)
    }
    
    document.body.appendChild(form)
    form.submit()
}
