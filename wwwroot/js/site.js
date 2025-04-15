// Theme Management
document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('themeToggle');
    const html = document.documentElement;
    const themeStorageKey = 'userTheme';
    
    // Function to apply theme
    function applyTheme(theme) {
        html.setAttribute('data-theme', theme);
        localStorage.setItem(themeStorageKey, theme);
        if (themeToggle) {
            themeToggle.checked = theme === 'dark';
            // Update moon/sun icon
            const icon = themeToggle.nextElementSibling.querySelector('i');
            if (icon) {
                icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
            }
        }
    }
    
    // Load saved theme or use default
    const savedTheme = localStorage.getItem(themeStorageKey);
    if (savedTheme) {
        applyTheme(savedTheme);
    } else {
        // Check for system preference
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        applyTheme(prefersDark ? 'dark' : 'light');
    }
    
    // Handle theme toggle
    if (themeToggle) {
        themeToggle.addEventListener('change', function() {
            applyTheme(this.checked ? 'dark' : 'light');
        });
    }
    
    // Listen for system theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
        if (!localStorage.getItem(themeStorageKey)) {
            applyTheme(e.matches ? 'dark' : 'light');
        }
    });
});

// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
