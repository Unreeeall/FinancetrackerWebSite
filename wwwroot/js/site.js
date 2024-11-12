// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleMenu() {
    const menuIcon = document.querySelector('.menu-icon');
    const menu = document.getElementById('menu');

    // Toggle active class for animation
    menuIcon.classList.toggle('active');

    // Show/hide menu with animation
    if (menu.classList.contains('show')) {
        menu.classList.remove('show');
    } else {
        menu.classList.add('show');
    }
}
