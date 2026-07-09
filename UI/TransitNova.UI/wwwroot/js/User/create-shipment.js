(() => {
    const form = document.querySelector('[data-customer-create-form]');
    if (!form) return;

    const submitButton = form.querySelector('[data-create-submit-button]');
    const submitText = submitButton?.querySelector('[data-button-text]');
    const submitSpinner = submitButton?.querySelector('[data-button-spinner]');
    const alertPanel = document.querySelector('[data-create-alert]');
    const alertTitle = document.querySelector('[data-create-alert-title]');
    const alertMessage = document.querySelector('[data-create-alert-message]');
    const paymentOverlay = document.querySelector('[data-payment-overlay]');
    const invoiceModal = document.querySelector('[data-invoice-modal]');
    const invoiceCloseButtons = document.querySelectorAll('[data-invoice-close]');
    const shipmentDetailsLink = document.querySelector('[data-shipment-details-link]');
    const allInputs = [...form.querySelectorAll('input, textarea, select')];
    const validationInputs = [...form.querySelectorAll('[data-live-required], [data-live-email], [data-live-phone], [data-live-select], [data-live-city], [data-live-weight], [data-live-dimension], [data-live-pickup-date]')];
    const allControls = [...form.querySelectorAll('input, textarea, select, button')];
    const statusBadge = document.querySelector('[data-invoice-status-badge]');
    const invoiceIdField = document.querySelector('[data-invoice-id]');
    const paymentMethodField = document.querySelector('[data-invoice-payment-method]');
    const paymentIdField = document.querySelector('[data-invoice-payment-id]');
    const paidAtField = document.querySelector('[data-invoice-paid-at]');
    const shippingCostField = document.querySelector('[data-invoice-shipping-cost]');
    const commissionField = document.querySelector('[data-invoice-commission]');
    const amountField = document.querySelector('[data-invoice-amount]');
    const notesField = document.querySelector('[data-invoice-notes]');
    const serviceableCities = new Set(['1', '2', '3', '4', '5', '10', '11', '12', '20']);
    const originalButtonText = submitText?.textContent?.trim() || 'Create Shipment';
    const defaultFailureMessage = 'No invoice was generated. Please review the payment method and try again.';
    const defaultValidationMessage = 'Please review the highlighted fields and try again.';
    const originalBodyOverflow = document.body.style.overflow;
    const countrySelect = form.querySelector('[data-country-select]');
    const governmentSelect = form.querySelector('[data-government-select]');
    const citySelect = form.querySelector('[data-city-select]');

    let isSubmitting = false;
    let paymentOverlayOpen = false;
    let invoiceModalOpen = false;
    let lastFocusedElement = null;

    async function validateWithFetch(result) {
        const url = 'data:application/json,' + encodeURIComponent(JSON.stringify(result));
        const response = await fetch(url);
        return response.json();
    }

    function errorTarget(input) {
        return document.querySelector('[data-error-for="' + input.id + '"]');
    }

    function setState(input, message) {
        const target = errorTarget(input);
        const invalid = Boolean(message);
        input.classList.toggle('is-invalid', invalid);
        input.setAttribute('aria-invalid', invalid ? 'true' : 'false');
        if (target) {
            target.textContent = message || '';
            target.classList.toggle('is-visible', invalid);
        }
    }

    async function validateInput(input) {
        let message = '';
        const value = input.value.trim();

        if (input.hasAttribute('data-live-required') && value.length < 2) {
            message = input.getAttribute('data-live-required') + ' is required.';
        }

        if (!message && input.hasAttribute('data-live-email') && !/^\S+@\S+\.\S+$/.test(value)) {
            message = 'Use a valid receiver email.';
        }

        if (!message && input.hasAttribute('data-live-phone') && value.replace(/\D/g, '').length < 10) {
            message = 'Use a reachable phone number with at least 10 digits.';
        }

        if (!message && input.hasAttribute('data-live-select') && !value) {
            message = (input.getAttribute('data-live-select') || 'Selection') + ' is required.';
        }

        if (!message && input.hasAttribute('data-live-city')) {
            const number = Number(value);
            const check = await validateWithFetch({ ok: Number.isInteger(number) && number > 0, message: 'Choose a pickup city before continuing.' });
            if (!check.ok) message = check.message;
        }

        if (!message && input.hasAttribute('data-live-weight')) {
            const number = Number(value);
            const check = await validateWithFetch({ ok: number > 0 && number <= 35, message: 'Packages above 35 kg need a freight quote before submission.' });
            if (!check.ok) message = check.message;
        }

        if (!message && input.hasAttribute('data-live-dimension')) {
            const number = Number(value);
            const check = await validateWithFetch({ ok: number > 0 && number <= 160, message: 'Single-side dimensions must be 160 cm or less.' });
            if (!check.ok) message = check.message;
        }

        if (!message && input.hasAttribute('data-live-pickup-date') && value) {
            const selected = new Date(value);
            const check = await validateWithFetch({ ok: selected > new Date(), message: 'Pickup time must be in the future.' });
            if (!check.ok) message = check.message;
        }

        setState(input, message);
        return !message;
    }

    function clearFieldStates() {
        allInputs.forEach((input) => setState(input, ''));
    }

    function showFailure(title, message) {
        if (!alertPanel) return;
        alertPanel.classList.remove('hidden');
        if (alertTitle) alertTitle.textContent = title;
        if (alertMessage) alertMessage.textContent = message;
    }

    function hideFailure() {
        if (!alertPanel) return;
        alertPanel.classList.add('hidden');
        if (alertMessage) alertMessage.textContent = '';
    }

    function syncBodyScrollLock() {
        document.body.style.overflow = paymentOverlayOpen || invoiceModalOpen ? 'hidden' : originalBodyOverflow;
    }

    function togglePaymentOverlay(open) {
        if (!paymentOverlay) return;
        paymentOverlayOpen = open;
        paymentOverlay.classList.toggle('hidden', !open);
        syncBodyScrollLock();
    }

    function toggleInvoiceModal(open) {
        if (!invoiceModal) return;
        if (open) {
            lastFocusedElement = document.activeElement instanceof HTMLElement ? document.activeElement : null;
            shipmentDetailsLink?.focus();
        }
        invoiceModalOpen = open;
        invoiceModal.classList.toggle('hidden', !open);
        invoiceModal.setAttribute('aria-hidden', open ? 'false' : 'true');
        syncBodyScrollLock();
        if (!open) lastFocusedElement?.focus();
    }

    function setSubmittingState(isBusy) {
        allControls.forEach((control) => {
            control.disabled = isBusy;
        });

        submitButton?.classList.toggle('opacity-80', isBusy);
        submitButton?.classList.toggle('cursor-wait', isBusy);
        submitSpinner?.classList.toggle('hidden', !isBusy);
        if (submitText) submitText.textContent = isBusy ? 'Processing payment...' : originalButtonText;
    }

    function normalizeKey(key) {
        return (key || '').replaceAll('.', '_').replaceAll('[', '_').replaceAll(']', '');
    }

    function findInputByKey(key) {
        const normalizedKey = normalizeKey(key);
        return normalizedKey ? form.querySelector('#' + CSS.escape(normalizedKey)) : null;
    }

    function fillSelect(select, items, placeholder) {
        if (!select) return;
        select.innerHTML = '';
        const emptyOption = document.createElement('option');
        emptyOption.value = '';
        emptyOption.textContent = placeholder;
        select.appendChild(emptyOption);

        (items || []).forEach((item) => {
            const option = document.createElement('option');
            option.value = item.id;
            option.textContent = item.name;
            select.appendChild(option);
        });
    }

    async function loadOptions(url, target, placeholder) {
        if (!target) return;
        target.disabled = true;
        fillSelect(target, [], 'Loading...');

        try {
            const response = await fetch(url, {
                credentials: 'same-origin',
                headers: {
                    Accept: 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            const payload = await response.json();
            if (!response.ok) {
                fillSelect(target, [], payload?.message || placeholder);
                return;
            }

            fillSelect(target, payload.items || [], placeholder);
            target.disabled = false;
            setState(target, '');
        } catch {
            fillSelect(target, [], 'Unable to load options');
        }
    }

    function wireLocationCascade() {
        if (!countrySelect || !governmentSelect || !citySelect) return;

        countrySelect.addEventListener('change', () => {
            fillSelect(governmentSelect, [], 'Choose government');
            fillSelect(citySelect, [], 'Choose city');
            governmentSelect.disabled = true;
            citySelect.disabled = true;
            setState(governmentSelect, '');
            setState(citySelect, '');

            if (!countrySelect.value) return;
            const url = `${form.dataset.governmentsUrl}?countryId=${encodeURIComponent(countrySelect.value)}`;
            loadOptions(url, governmentSelect, 'Choose government');
        });

        governmentSelect.addEventListener('change', () => {
            fillSelect(citySelect, [], 'Choose city');
            citySelect.disabled = true;
            setState(citySelect, '');

            if (!governmentSelect.value) return;
            const url = `${form.dataset.citiesUrl}?governmentId=${encodeURIComponent(governmentSelect.value)}`;
            loadOptions(url, citySelect, 'Choose city');
        });
    }

    function applyValidationErrors(validationErrors) {
        clearFieldStates();
        let hasFieldErrors = false;
        const generalMessages = [];

        Object.entries(validationErrors || {}).forEach(([key, messages]) => {
            const normalizedMessages = Array.isArray(messages) ? messages.filter((message) => typeof message === 'string' && message.trim().length > 0) : [];
            if (!key) {
                generalMessages.push(...normalizedMessages);
                return;
            }

            const input = findInputByKey(key);
            if (!input) {
                generalMessages.push(...normalizedMessages);
                return;
            }

            hasFieldErrors = true;
            setState(input, normalizedMessages[0] || 'Invalid value.');
        });

        if (hasFieldErrors || generalMessages.length > 0) {
            showFailure('Please review the highlighted fields', generalMessages[0] || defaultValidationMessage);
        }

        form.querySelector('.is-invalid')?.focus();
    }

    function humanize(value) {
        return value
            ? String(value).replace(/([a-z])([A-Z])/g, '$1 $2').replace(/[_-]+/g, ' ').trim()
            : 'Not available';
    }

    function formatAmount(value) {
        const amount = Number(value);
        return Number.isFinite(amount)
            ? amount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })
            : '0.00';
    }

    function formatDateTime(value) {
        if (!value) return 'Pending confirmation';
        const date = new Date(value);
        return Number.isNaN(date.getTime())
            ? value
            : date.toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' });
    }

    function updateStatusBadge(status) {
        if (!statusBadge) return;
        const normalizedStatus = String(status || '').toLowerCase();
        statusBadge.textContent = humanize(status || 'Pending');
        statusBadge.className = 'rounded-full px-4 py-2 text-sm font-bold';
        if (normalizedStatus.includes('paid') || normalizedStatus.includes('success')) {
            statusBadge.classList.add('bg-[#F3F7F3]', 'text-[#14532D]');
            return;
        }
        if (normalizedStatus.includes('fail') || normalizedStatus.includes('declin')) {
            statusBadge.classList.add('bg-[#FDEBEC]', 'text-[#9F2F2D]');
            return;
        }
        statusBadge.classList.add('bg-[#F4F4F3]', 'text-[#444444]');
    }

    function populateInvoice(invoice) {
        if (!invoice) return;
        if (invoiceIdField) invoiceIdField.textContent = invoice.invoiceId || 'Not available';
        if (paymentIdField) paymentIdField.textContent = invoice.paymentId || 'Not available';
        if (paymentMethodField) paymentMethodField.textContent = humanize(invoice.paymentMethod);
        if (paidAtField) paidAtField.textContent = formatDateTime(invoice.paidAt);
        const currency = humanize(invoice.currency || '');
        if (shippingCostField) shippingCostField.textContent = `${formatAmount(invoice.shippingCost)} ${currency}`.trim();
        if (commissionField) commissionField.textContent = `${formatAmount(invoice.commission)} ${currency}`.trim();
        const totalAmount = invoice.totalAmount ?? invoice.amount;
        if (amountField) amountField.textContent = `${formatAmount(totalAmount)} ${currency}`.trim();
        if (notesField) notesField.textContent = invoice.notes || 'No additional notes.';
        updateStatusBadge(invoice.status);
    }

    async function submitCreateShipment() {
        if (isSubmitting) return;

        hideFailure();
        const results = await Promise.all(validationInputs.map(validateInput));
        if (results.some((valid) => !valid)) {
            showFailure('Please review the highlighted fields', defaultValidationMessage);
            form.querySelector('.is-invalid')?.focus();
            return;
        }

        const formData = new FormData(form);
        isSubmitting = true;
        setSubmittingState(true);
        togglePaymentOverlay(true);

        try {
            const response = await fetch(form.action || window.location.href, {
                method: form.method || 'POST',
                body: formData,
                headers: {
                    Accept: 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const payload = await response.json().catch(() => null);
            if (!response.ok || !payload?.isSuccess) {
                if (payload?.validationErrors) {
                    applyValidationErrors(payload.validationErrors);
                } else {
                    showFailure(payload?.title || "Payment couldn't be completed", payload?.message || defaultFailureMessage);
                }
                return;
            }

            if (!payload.invoice || !payload.shipmentDetailsUrl) {
                showFailure("Payment couldn't be completed", defaultFailureMessage);
                return;
            }

            hideFailure();
            clearFieldStates();
            populateInvoice(payload.invoice);
            if (shipmentDetailsLink) shipmentDetailsLink.href = payload.shipmentDetailsUrl;
            toggleInvoiceModal(true);
        } catch {
            showFailure("Payment couldn't be completed", defaultFailureMessage);
        } finally {
            isSubmitting = false;
            setSubmittingState(false);
            togglePaymentOverlay(false);
        }
    }

    wireLocationCascade();

    allInputs.forEach((input) => {
        input.addEventListener('blur', () => validateInput(input));
        input.addEventListener('change', () => validateInput(input));
        input.addEventListener('input', () => {
            if (!isSubmitting) hideFailure();
        });
    });

    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        await submitCreateShipment();
    });

    invoiceCloseButtons.forEach((button) => button.addEventListener('click', () => toggleInvoiceModal(false)));
    invoiceModal?.addEventListener('click', (event) => {
        if (event.target === invoiceModal) toggleInvoiceModal(false);
    });
    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' && invoiceModalOpen) toggleInvoiceModal(false);
    });
})();

