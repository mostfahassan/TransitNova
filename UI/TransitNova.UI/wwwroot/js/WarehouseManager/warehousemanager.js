(() => {
    const sidebar = document.querySelector('[data-warehouse-sidebar]');
    const sidebarButton = document.querySelector('[data-warehouse-sidebar-toggle]');
    const profileButton = document.querySelector('[data-warehouse-profile-button]');
    const profileMenu = document.querySelector('[data-warehouse-profile-menu]');
    const modalBackdrop = document.querySelector('[data-warehouse-modal-backdrop]');

    if (sidebar && sidebarButton) {
        sidebarButton.addEventListener('click', () => sidebar.classList.toggle('is-open'));
    }

    if (profileButton && profileMenu) {
        profileButton.addEventListener('click', () => profileMenu.classList.toggle('hidden'));
        document.addEventListener('click', (event) => {
            if (!profileButton.contains(event.target) && !profileMenu.contains(event.target)) {
                profileMenu.classList.add('hidden');
            }
        });
    }

    const closeSlideOvers = () => {
        document.querySelectorAll('[data-warehouse-slide-over]').forEach((panel) => panel.classList.remove('is-open'));
        modalBackdrop?.classList.remove('is-open');
    };

    document.querySelectorAll('[data-open-warehouse-panel]').forEach((button) => {
        button.addEventListener('click', () => {
            const target = document.querySelector(button.getAttribute('data-open-warehouse-panel'));
            if (!target) return;
            modalBackdrop?.classList.add('is-open');
            target.classList.add('is-open');
            target.querySelector('input, select, textarea, button')?.focus();
        });
    });

    document.querySelectorAll('[data-close-warehouse-panel]').forEach((button) => button.addEventListener('click', closeSlideOvers));
    modalBackdrop?.addEventListener('click', closeSlideOvers);

    document.querySelectorAll('[data-warehouse-ajax-form]').forEach((form) => {
        form.addEventListener('submit', async (event) => {
            event.preventDefault();
            const submit = form.querySelector('[type="submit"]');
            const status = form.querySelector('[data-ajax-status]');
            const defaultText = submit?.textContent?.trim() || 'Save';

            if (submit) {
                submit.disabled = true;
                submit.innerHTML = '<span class="warehouse-spinner"></span><span>Saving...</span>';
            }

            try {
                const response = await fetch(form.action, {
                    method: form.method || 'POST',
                    body: new FormData(form),
                    credentials: 'same-origin',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });

                if (!response.ok) {
                    if (status) status.textContent = `Request failed (${response.status}).`;
                    return;
                }

                if (status) status.textContent = 'Saved successfully.';
                setTimeout(() => window.location.reload(), 350);
            } catch {
                if (status) status.textContent = 'Connection failed. Try again.';
            } finally {
                if (submit) {
                    submit.disabled = false;
                    submit.textContent = defaultText;
                }
            }
        });
    });

    document.querySelectorAll('[data-warehouse-loading-form]').forEach((form) => {
        form.addEventListener('submit', () => {
            const submit = form.querySelector('[type="submit"]');
            if (!submit) return;
            submit.disabled = true;
            submit.classList.add('opacity-80', 'cursor-wait');
        });
    });
})();