(() => {
    const button = document.querySelector('[data-customer-profile-button]');
    const menu = document.querySelector('[data-customer-profile-menu]');

    if (button && menu) {
        button.addEventListener('click', () => menu.classList.toggle('hidden'));
        document.addEventListener('click', (event) => {
            if (!button.contains(event.target) && !menu.contains(event.target)) {
                menu.classList.add('hidden');
            }
        });
    }

    document.querySelectorAll('[data-loading-form]').forEach((form) => {
        form.addEventListener('submit', () => {
            const submit = form.querySelector('[type="submit"]');
            if (!submit) return;
            const text = submit.querySelector('[data-button-text]');
            const spinner = submit.querySelector('[data-button-spinner]');
            submit.disabled = true;
            submit.classList.add('opacity-80', 'cursor-wait');
            if (text) text.textContent = submit.getAttribute('data-loading-text') || 'Processing...';
            if (spinner) spinner.classList.remove('hidden');
        });
    });
})();