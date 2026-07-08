(() => {
    const sidebar = document.querySelector('[data-carrier-sidebar]');
    const toggle = document.querySelector('[data-carrier-sidebar-toggle]');
    toggle?.addEventListener('click', () => sidebar?.classList.toggle('is-open'));

    const profileButton = document.querySelector('[data-carrier-profile-button]');
    const profileMenu = document.querySelector('[data-carrier-profile-menu]');

    if (profileButton && profileMenu) {
        const closeProfileMenu = () => {
            profileMenu.classList.add('hidden');
            profileButton.setAttribute('aria-expanded', 'false');
        };

        profileButton.addEventListener('click', (event) => {
            event.stopPropagation();
            const willOpen = profileMenu.classList.contains('hidden');
            profileMenu.classList.toggle('hidden');
            profileButton.setAttribute('aria-expanded', willOpen ? 'true' : 'false');
        });

        document.addEventListener('click', (event) => {
            if (!profileButton.contains(event.target) && !profileMenu.contains(event.target)) {
                closeProfileMenu();
            }
        });

        document.addEventListener('keydown', (event) => {
            if (event.key === 'Escape') closeProfileMenu();
        });
    }

    document.querySelectorAll('[data-loading-form]').forEach((form) => {
        form.addEventListener('submit', () => {
            const button = form.querySelector('button[type="submit"]');
            const text = button?.querySelector('[data-button-text]');
            const spinner = button?.querySelector('[data-button-spinner]');
            if (!button || button.dataset.loading === 'true') return;
            button.dataset.loading = 'true';
            button.setAttribute('disabled', 'disabled');
            spinner?.classList.remove('hidden');
            if (text && form instanceof HTMLElement) text.textContent = form.dataset.loadingText || 'Working...';
        });
    });
})();
