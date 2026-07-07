(() => {
    const sidebar = document.querySelector('[data-carrier-sidebar]');
    const toggle = document.querySelector('[data-carrier-sidebar-toggle]');
    toggle?.addEventListener('click', () => sidebar?.classList.toggle('is-open'));

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