(() => {
    const profileButton = document.querySelector('[data-ops-profile-button]');
    const profileMenu = document.querySelector('[data-ops-profile-menu]');

    if (profileButton && profileMenu) {
        profileButton.addEventListener('click', () => {
            profileMenu.classList.toggle('hidden');
        });

        document.addEventListener('click', (event) => {
            if (!profileButton.contains(event.target) && !profileMenu.contains(event.target)) {
                profileMenu.classList.add('hidden');
            }
        });
    }

    document.querySelectorAll('[data-confirm]').forEach((element) => {
        element.addEventListener('submit', (event) => {
            const message = element.getAttribute('data-confirm') || 'Confirm this operation?';
            if (!window.confirm(message)) {
                event.preventDefault();
            }
        });
    });
})();
