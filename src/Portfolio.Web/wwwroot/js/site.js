(function () {
    'use strict';

    var STORAGE_KEY = 'portfolio-theme';
    var root = document.documentElement;

    function applyTheme(theme) {
        root.setAttribute('data-bs-theme', theme);
        var icon = document.getElementById('themeIcon');
        if (icon) {
            icon.innerHTML = theme === 'dark' ? '&#9728;&#65039;' : '&#127769;';
        }
    }

    applyTheme(localStorage.getItem(STORAGE_KEY) || 'light');

    var toggle = document.getElementById('themeToggle');
    if (toggle) {
        toggle.addEventListener('click', function () {
            var next = root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
            applyTheme(next);
            localStorage.setItem(STORAGE_KEY, next);
        });
    }
})();
