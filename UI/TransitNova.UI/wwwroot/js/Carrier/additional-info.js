(() => {
    const form = document.querySelector('[data-carrier-additional-info-form]');
    if (!form) return;

    const alert = form.querySelector('[data-carrier-form-alert]');
    const submitButton = form.querySelector('[data-carrier-submit-button]');
    const submitText = submitButton?.querySelector('[data-button-text]');
    const submitSpinner = submitButton?.querySelector('[data-button-spinner]');
    const fields = [...form.querySelectorAll('input, select')].filter((field) => field.type !== 'hidden');
    const originalButtonText = submitText?.textContent?.trim() || 'Save info';

    function normalizeKey(key) {
        const parts = String(key || '').split('.');
        return parts[parts.length - 1].replaceAll('[', '_').replaceAll(']', '');
    }

    function errorElement(field) {
        const name = field.getAttribute('name') || field.id;
        return form.querySelector(`[data-carrier-error-for='${CSS.escape(normalizeKey(name))}']`);
    }

    function setFieldError(field, message) {
        field.classList.toggle('is-invalid', Boolean(message));
        field.setAttribute('aria-invalid', message ? 'true' : 'false');
        const target = errorElement(field);
        if (target) target.textContent = message || '';
    }

    function showAlert(message, type = 'error') {
        if (!alert) return;
        alert.textContent = message || '';
        alert.classList.toggle('hidden', !message);
        alert.classList.toggle('border-[#cfe8d7]', type === 'success');
        alert.classList.toggle('bg-[#EAF4ED]', type === 'success');
        alert.classList.toggle('text-[#28623c]', type === 'success');
        alert.classList.toggle('border-[#f2caca]', type !== 'success');
        alert.classList.toggle('bg-[#FDEBEA]', type !== 'success');
        alert.classList.toggle('text-[#9f2f2d]', type !== 'success');
    }

    function clearErrors() {
        fields.forEach((field) => setFieldError(field, ''));
        showAlert('');
    }

    function validateField(field) {
        const value = field.value.trim();
        let message = '';

        if (field.required && !value) {
            message = field.tagName === 'SELECT' ? 'Choose an option before continuing.' : 'This field is required.';
        }

        if (!message && field.type === 'number') {
            const number = Number(value);
            const min = Number(field.min);
            if (!Number.isFinite(number)) message = 'Enter a valid number.';
            if (!message && Number.isFinite(min) && number < min) message = `Value must be at least ${min}.`;
        }

        if (!message && field.type === 'date') {
            const selected = new Date(`${value}T00:00:00`);
            const now = new Date();
            if (!value || Number.isNaN(selected.getTime())) message = 'Choose a valid date.';
            if (!message && selected <= now) message = 'Start date must be in the future.';
        }

        setFieldError(field, message);
        return !message;
    }

    function validateForm() {
        let valid = true;
        fields.forEach((field) => {
            if (!validateField(field)) valid = false;
        });
        return valid;
    }

    function setLoading(isLoading) {
        fields.forEach((field) => field.disabled = isLoading);
        submitButton.disabled = isLoading;
        submitButton.classList.toggle('cursor-wait', isLoading);
        submitSpinner?.classList.toggle('hidden', !isLoading);
        if (submitText) submitText.textContent = isLoading ? (submitButton.dataset.loadingText || 'Saving...') : originalButtonText;
    }

    function findField(key) {
        const normalized = normalizeKey(key);
        return fields.find((field) => normalizeKey(field.getAttribute('name') || field.id) === normalized);
    }

    function applyServerErrors(payload) {
        let firstMessage = payload?.message || 'Please review the highlighted fields.';
        const errors = payload?.errors || {};

        Object.entries(errors).forEach(([key, messages]) => {
            const text = Array.isArray(messages) ? messages.filter(Boolean).join(' ') : String(messages || '');
            if (!key) {
                firstMessage = text || firstMessage;
                return;
            }

            const field = findField(key);
            if (field) setFieldError(field, text || 'Invalid value.');
            else firstMessage = text || firstMessage;
        });

        showAlert(firstMessage, 'error');
        form.querySelector('.is-invalid')?.focus();
    }

    fields.forEach((field) => {
        field.addEventListener('blur', () => validateField(field));
        field.addEventListener('change', () => validateField(field));
        field.addEventListener('input', () => {
            if (field.getAttribute('aria-invalid') === 'true') validateField(field);
            showAlert('');
        });
    });

    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        clearErrors();

        if (!validateForm()) {
            showAlert('Fix the highlighted fields before submitting.', 'error');
            form.querySelector('.is-invalid')?.focus();
            return;
        }

        setLoading(true);
        try {
            const response = await fetch(form.action, {
                method: form.method || 'POST',
                body: new FormData(form),
                credentials: 'same-origin',
                headers: {
                    Accept: 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const payload = await response.json().catch(() => null);
            if (!response.ok || payload?.isSuccess === false) {
                applyServerErrors(payload || { message: `Request failed (HTTP ${response.status}).` });
                return;
            }

            showAlert(payload?.message || 'Carrier profile completed successfully.', 'success');
            window.setTimeout(() => {
                window.location.assign(payload?.redirectUrl || '/CarrierArea/Dashboard/Index');
            }, 900);
        } catch {
            showAlert('Connection failed. Please try again.', 'error');
        } finally {
            setLoading(false);
        }
    });
})();