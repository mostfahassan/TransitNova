(() => {
    const form = document.querySelector('[data-bundle-checkout-form]');
    if (!form) return;

    const submitButton = form.querySelector('[data-bundle-submit-button]');
    const submitText = submitButton?.querySelector('[data-button-text]');
    const submitSpinner = submitButton?.querySelector('[data-button-spinner]');
    const alertPanel = document.querySelector('[data-bundle-alert]');
    const alertTitle = document.querySelector('[data-bundle-alert-title]');
    const alertMessage = document.querySelector('[data-bundle-alert-message]');
    const paymentOverlay = document.querySelector('[data-bundle-payment-overlay]');
    const invoiceModal = document.querySelector('[data-bundle-invoice-modal]');
    const invoiceCloseButtons = document.querySelectorAll('[data-bundle-invoice-close]');
    const allControls = [...form.querySelectorAll('input, textarea, select, button')];
    const statusBadge = document.querySelector('[data-bundle-invoice-status-badge]');
    const invoiceIdField = document.querySelector('[data-bundle-invoice-id]');
    const paymentIdField = document.querySelector('[data-bundle-invoice-payment-id]');
    const paymentMethodField = document.querySelector('[data-bundle-invoice-payment-method]');
    const bundleNameField = document.querySelector('[data-bundle-invoice-name]');
    const customerField = document.querySelector('[data-bundle-invoice-customer]');
    const subscribedAtField = document.querySelector('[data-bundle-invoice-subscribed-at]');
    const endDateField = document.querySelector('[data-bundle-invoice-end-date]');
    const commissionField = document.querySelector('[data-bundle-invoice-commission]');
    const bundlePriceField = document.querySelector('[data-bundle-invoice-price]');
    const amountField = document.querySelector('[data-bundle-invoice-amount]');
    const notesField = document.querySelector('[data-bundle-invoice-notes]');
    const reportForm = document.querySelector('[data-bundle-report-form]');
    const reportPaymentIdInput = document.querySelector('[data-bundle-report-payment-id]');
    const reportButton = document.querySelector('[data-bundle-report-button]');
    const reportSpinner = document.querySelector('[data-bundle-report-spinner]');
    const reportText = document.querySelector('[data-bundle-report-text]');
    const reportFeedback = document.querySelector('[data-bundle-report-feedback]');
    const originalButtonText = submitText?.textContent?.trim() || 'Pay and subscribe';
    const originalReportText = reportText?.textContent?.trim() || 'Print Invoice Report';
    const originalBodyOverflow = document.body.style.overflow;

    let isSubmitting = false;
    let isRequestingReport = false;
    let paymentOverlayOpen = false;
    let invoiceModalOpen = false;
    let lastFocusedElement = null;

    function pick(source, camelName, pascalName) {
        if (!source) return undefined;
        if (Object.prototype.hasOwnProperty.call(source, camelName)) return source[camelName];
        if (Object.prototype.hasOwnProperty.call(source, pascalName)) return source[pascalName];
        return undefined;
    }

    function formatMoney(value, currency) {
        const number = Number(value || 0);
        return number.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' ' + (currency || 'EGP');
    }

    function formatDate(value) {
        if (!value) return 'Pending confirmation';
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) return 'Pending confirmation';
        return date.toLocaleString('en-US', {
            year: 'numeric',
            month: 'short',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    function showFailure(title, message) {
        if (!alertPanel) return;
        alertPanel.classList.remove('hidden');
        if (alertTitle) alertTitle.textContent = title || 'Payment could not be completed';
        if (alertMessage) alertMessage.textContent = message || 'The request could not be completed.';
    }

    function hideFailure() {
        if (!alertPanel) return;
        alertPanel.classList.add('hidden');
        if (alertMessage) alertMessage.textContent = '';
    }

    function setReportFeedback(message, isSuccess) {
        if (!reportFeedback) return;
        if (!message) {
            reportFeedback.classList.add('hidden');
            reportFeedback.textContent = '';
            reportFeedback.classList.remove('text-[#14532D]', 'text-[#9F2F2D]');
            return;
        }

        reportFeedback.classList.remove('hidden');
        reportFeedback.textContent = message;
        reportFeedback.classList.toggle('text-[#14532D]', !!isSuccess);
        reportFeedback.classList.toggle('text-[#9F2F2D]', !isSuccess);
    }

    function syncBodyScrollLock() {
        document.body.style.overflow = paymentOverlayOpen || invoiceModalOpen ? 'hidden' : originalBodyOverflow;
    }

    function togglePaymentOverlay(open) {
        if (!paymentOverlay) return;
        paymentOverlayOpen = open;
        paymentOverlay.classList.toggle('hidden', !open);
        paymentOverlay.setAttribute('aria-hidden', open ? 'false' : 'true');
        syncBodyScrollLock();
    }

    function toggleInvoiceModal(open) {
        if (!invoiceModal) return;
        if (open) lastFocusedElement = document.activeElement instanceof HTMLElement ? document.activeElement : null;
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

    function setReportSubmittingState(isBusy) {
        if (!reportButton) return;
        reportButton.disabled = isBusy || !reportPaymentIdInput?.value;
        reportButton.classList.toggle('opacity-80', isBusy);
        reportButton.classList.toggle('cursor-wait', isBusy);
        reportSpinner?.classList.toggle('hidden', !isBusy);
        if (reportText) reportText.textContent = isBusy ? 'Requesting report...' : originalReportText;
    }

    function fillInvoice(invoice) {
        const status = pick(invoice, 'status', 'Status') || 'Paid';
        const currency = pick(invoice, 'currency', 'Currency') || 'EGP';
        if (statusBadge) statusBadge.textContent = status;
        if (invoiceIdField) invoiceIdField.textContent = pick(invoice, 'invoiceId', 'InvoiceId') || 'Not available';
        if (paymentIdField) paymentIdField.textContent = pick(invoice, 'paymentId', 'PaymentId') || 'Not available';
        if (paymentMethodField) paymentMethodField.textContent = pick(invoice, 'paymentMethod', 'PaymentMethod') || 'Not available';
        if (bundleNameField) bundleNameField.textContent = pick(invoice, 'bundleName', 'BundleName') || 'Not available';
        if (customerField) customerField.textContent = pick(invoice, 'fullName', 'FullName') || 'Customer account';
        if (subscribedAtField) subscribedAtField.textContent = formatDate(pick(invoice, 'subscribedAt', 'SubscribedAt'));
        if (endDateField) endDateField.textContent = formatDate(pick(invoice, 'endDate', 'EndDate'));
        if (commissionField) commissionField.textContent = formatMoney(pick(invoice, 'commission', 'Commission'), currency);
        if (bundlePriceField) bundlePriceField.textContent = formatMoney(pick(invoice, 'bundlePrice', 'BundlePrice'), currency);
        if (amountField) amountField.textContent = formatMoney(pick(invoice, 'totalAmount', 'TotalAmount'), currency);
        if (notesField) notesField.textContent = pick(invoice, 'notes', 'Notes') || 'No additional notes.';
        if (reportPaymentIdInput) reportPaymentIdInput.value = pick(invoice, 'paymentId', 'PaymentId') || '';
        setReportFeedback('', true);
        setReportSubmittingState(false);
    }

    invoiceCloseButtons.forEach((button) => {
        button.addEventListener('click', () => toggleInvoiceModal(false));
    });

    invoiceModal?.addEventListener('click', (event) => {
        if (event.target === invoiceModal) toggleInvoiceModal(false);
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' && invoiceModalOpen) toggleInvoiceModal(false);
    });

    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        if (isSubmitting) return;

        isSubmitting = true;
        hideFailure();
        setReportFeedback('', true);
        setSubmittingState(true);
        togglePaymentOverlay(true);

        try {
            const formData = new FormData(form);
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                credentials: 'same-origin',
                headers: {
                    Accept: 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const payload = await response.json().catch(() => null);
            if (!response.ok || !payload?.isSuccess) {
                showFailure(payload?.title, payload?.message);
                return;
            }

            fillInvoice(payload.invoice || payload.Invoice);
            toggleInvoiceModal(true);
        } catch {
            showFailure('Payment could not be completed', 'The payment service is temporarily unavailable. Try again after the API is running.');
        } finally {
            togglePaymentOverlay(false);
            setSubmittingState(false);
            isSubmitting = false;
        }
    });

    reportForm?.addEventListener('submit', async (event) => {
        event.preventDefault();
        if (isRequestingReport || !reportPaymentIdInput?.value) return;

        isRequestingReport = true;
        setReportSubmittingState(true);
        setReportFeedback('', true);

        try {
            const response = await fetch(reportForm.action, {
                method: 'POST',
                body: new FormData(reportForm),
                credentials: 'same-origin',
                headers: {
                    Accept: 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const payload = await response.json().catch(() => null);
            if (!response.ok || !payload?.isSuccess) {
                setReportFeedback(payload?.message || 'The bundle invoice report request could not be completed.', false);
                return;
            }

            setReportFeedback(payload.message || 'Bundle invoice report requested successfully.', true);
        } catch {
            setReportFeedback('The report service is temporarily unavailable. Try again after the API is running.', false);
        } finally {
            setReportSubmittingState(false);
            isRequestingReport = false;
        }
    });
})();