document.addEventListener('DOMContentLoaded', function () {
    const toggleButton = document.querySelector('[data-collapse-toggle]');
    const menu = document.getElementById('navbar-default');

    if (toggleButton && menu) {
        toggleButton.addEventListener('click', function () {
            menu.classList.toggle('hidden');

            // Toggle aria-expanded for accessibility
            const expanded = this.getAttribute('aria-expanded') === 'true';
            this.setAttribute('aria-expanded', !expanded);
        });
    }
});

//Script from Claude AI 