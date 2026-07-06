(function () {
    const forms = document.querySelectorAll("[data-account-form]");

    const fieldMessages = {
        required: "This field is required.",
        email: "Enter a valid email address.",
        match: "The confirmation value does not match.",
        minLength: "Use at least {0} characters.",
        select: "Choose an option before continuing."
    };

    const getFieldName = function (field) {
        return field.getAttribute("name") || field.getAttribute("id") || "";
    };

    const findErrorElement = function (form, field) {
        const name = getFieldName(field);
        return form.querySelector(`[data-account-error-for='${CSS.escape(name)}']`) || form.querySelector(`[data-valmsg-for='${CSS.escape(name)}']`);
    };

    const setFieldError = function (form, field, message) {
        const errorElement = findErrorElement(form, field);
        field.setAttribute("aria-invalid", message ? "true" : "false");

        if (errorElement) {
            errorElement.textContent = message || "";
        }
    };

    const validateField = function (form, field) {
        if (field.disabled || field.type === "hidden") {
            setFieldError(form, field, "");
            return true;
        }

        const value = field.value.trim();
        let message = "";

        if (field.hasAttribute("required") && !value) {
            message = field.tagName === "SELECT" ? fieldMessages.select : fieldMessages.required;
        }

        if (!message && field.type === "email" && value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
            message = fieldMessages.email;
        }

        const minLength = Number(field.getAttribute("data-min-length"));
        if (!message && Number.isFinite(minLength) && minLength > 0 && value.length < minLength) {
            message = fieldMessages.minLength.replace("{0}", minLength);
        }

        const matchSelector = field.getAttribute("data-match");
        if (!message && matchSelector) {
            const matchField = form.querySelector(matchSelector);
            if (matchField && value !== matchField.value.trim()) {
                message = fieldMessages.match;
            }
        }

        setFieldError(form, field, message);
        return !message;
    };

    const validateForm = function (form) {
        const fields = form.querySelectorAll("input, select, textarea");
        let isValid = true;

        fields.forEach(function (field) {
            if (!validateField(form, field)) {
                isValid = false;
            }
        });

        return isValid;
    };

    const clearServerErrors = function (form) {
        form.querySelectorAll("[data-account-error-for]").forEach(function (element) {
            element.textContent = "";
        });

        form.querySelectorAll("[aria-invalid='true']").forEach(function (field) {
            field.setAttribute("aria-invalid", "false");
        });
    };

    const showAlert = function (form, message, type) {
        const alert = form.querySelector("[data-form-alert]");
        if (!alert) {
            return;
        }

        if (!message) {
            alert.hidden = true;
            alert.textContent = "";
            return;
        }

        alert.hidden = false;
        alert.textContent = message;
        alert.classList.toggle("account-alert-error", type !== "success");
        alert.classList.toggle("account-alert-success", type === "success");
    };

    const setLoading = function (form, isLoading) {
        const button = form.querySelector("[data-submit-button]");
        if (!button) {
            return;
        }

        if (!button.dataset.defaultText) {
            button.dataset.defaultText = button.textContent.trim();
        }

        button.disabled = isLoading;
        button.classList.toggle("is-loading", isLoading);
        button.textContent = isLoading ? (button.dataset.loadingText || "Working...") : button.dataset.defaultText;
    };

    const applyServerErrors = function (form, payload) {
        const errors = payload.errors || {};
        let firstMessage = payload.message || "Please review the highlighted fields.";

        Object.keys(errors).forEach(function (key) {
            const messages = Array.isArray(errors[key]) ? errors[key] : [String(errors[key])];
            const message = messages.filter(Boolean).join(" ");

            if (!key) {
                firstMessage = message || firstMessage;
                return;
            }

            const field = form.querySelector(`[name='${CSS.escape(key)}']`);
            if (field) {
                setFieldError(form, field, message);
            }
        });

        showAlert(form, firstMessage, "error");
    };

    const submitAjaxForm = async function (form) {
        clearServerErrors(form);
        showAlert(form, "", "error");

        if (!validateForm(form)) {
            showAlert(form, "Fix the highlighted fields before submitting.", "error");
            return;
        }

        setLoading(form, true);

        try {
            const response = await fetch(form.action, {
                method: form.method || "POST",
                body: new FormData(form),
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                }
            });

            const contentType = response.headers.get("content-type") || "";
            let payload = {};

            if (contentType.includes("application/json")) {
                payload = await response.json();
            } else if (!response.ok) {
                const responseText = await response.text();
                const preview = responseText.replace(/<[^>]*>/g, " ").replace(/\s+/g, " ").trim().slice(0, 220);
                payload = {
                    isSuccess: false,
                    message: preview ? `Request failed (HTTP ${response.status}). ${preview}` : `Request failed (HTTP ${response.status}).`
                };
            }

            if (!response.ok || payload.isSuccess === false) {
                applyServerErrors(form, payload);
                return;
            }

            if (payload.message) {
                showAlert(form, payload.message, "success");
            }

            if (payload.redirectUrl) {
                window.location.assign(payload.redirectUrl);
            }
        } catch (error) {
            showAlert(form, "Connection failed. Please try again.", "error");
        } finally {
            setLoading(form, false);
        }
    };

    forms.forEach(function (form) {
        form.querySelectorAll("input, select, textarea").forEach(function (field) {
            field.addEventListener("blur", function () {
                validateField(form, field);
            });

            field.addEventListener("input", function () {
                if (field.getAttribute("aria-invalid") === "true") {
                    validateField(form, field);
                }

                const dependentSelector = field.getAttribute("data-revalidates");
                if (dependentSelector) {
                    const dependent = form.querySelector(dependentSelector);
                    if (dependent && dependent.value) {
                        validateField(form, dependent);
                    }
                }
            });

            field.addEventListener("change", function () {
                validateField(form, field);
            });
        });

        form.addEventListener("submit", function (event) {
            if (!form.hasAttribute("data-ajax-form")) {
                return;
            }

            event.preventDefault();
            submitAjaxForm(form);
        });
    });

    const fillSelect = function (select, items, placeholder) {
        select.innerHTML = "";

        const emptyOption = document.createElement("option");
        emptyOption.value = "";
        emptyOption.textContent = placeholder;
        select.appendChild(emptyOption);

        items.forEach(function (item) {
            const option = document.createElement("option");
            option.value = item.id;
            option.textContent = item.name;
            select.appendChild(option);
        });
    };

    const loadOptions = async function (url, target, placeholder) {
        target.disabled = true;
        fillSelect(target, [], "Loading...");

        try {
            const response = await fetch(url, {
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                }
            });

            const payload = await response.json();
            if (!response.ok) {
                fillSelect(target, [], payload.message || placeholder);
                return;
            }

            fillSelect(target, payload.items || [], placeholder);
            target.disabled = false;
        } catch (error) {
            fillSelect(target, [], "Unable to load options");
        }
    };

    const registerForm = document.querySelector("[data-register-form]");

    if (registerForm) {
        const countrySelect = registerForm.querySelector("[data-country-select]");
        const governmentSelect = registerForm.querySelector("[data-government-select]");
        const citySelect = registerForm.querySelector("[data-city-select]");

        if (countrySelect && governmentSelect && citySelect) {
            countrySelect.addEventListener("change", function () {
                fillSelect(governmentSelect, [], "Choose government");
                fillSelect(citySelect, [], "Choose city");
                governmentSelect.disabled = true;
                citySelect.disabled = true;

                if (!countrySelect.value) {
                    return;
                }

                const url = `${registerForm.dataset.governmentsUrl}?countryId=${encodeURIComponent(countrySelect.value)}`;
                loadOptions(url, governmentSelect, "Choose government");
            });

            governmentSelect.addEventListener("change", function () {
                fillSelect(citySelect, [], "Choose city");
                citySelect.disabled = true;

                if (!governmentSelect.value) {
                    return;
                }

                const url = `${registerForm.dataset.citiesUrl}?governmentId=${encodeURIComponent(governmentSelect.value)}`;
                loadOptions(url, citySelect, "Choose city");
            });
        }
    }
}());
