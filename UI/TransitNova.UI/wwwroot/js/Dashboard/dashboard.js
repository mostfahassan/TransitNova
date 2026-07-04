(function () {
    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    const sidebar = document.querySelector("[data-dashboard-sidebar]");
    const overlay = document.querySelector("[data-dashboard-sidebar-overlay]");
    const toggles = document.querySelectorAll("[data-dashboard-sidebar-toggle]");

    const setSidebarOpen = function (isOpen) {
        if (!sidebar || !overlay) {
            return;
        }

        sidebar.classList.toggle("is-open", isOpen);
        overlay.classList.toggle("is-open", isOpen);
        overlay.classList.toggle("hidden", !isOpen);
        toggles.forEach(function (toggle) {
            toggle.setAttribute("aria-expanded", String(isOpen));
        });
    };

    toggles.forEach(function (toggle) {
        toggle.addEventListener("click", function () {
            const isOpen = toggle.getAttribute("aria-expanded") !== "true";
            setSidebarOpen(isOpen);
        });
    });

    if (overlay) {
        overlay.addEventListener("click", function () {
            setSidebarOpen(false);
        });
    }

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            setSidebarOpen(false);
        }
    });

    const revealItems = document.querySelectorAll("[data-dashboard-reveal]");
    if (prefersReducedMotion || !("IntersectionObserver" in window)) {
        revealItems.forEach(function (item) {
            item.classList.add("is-visible");
        });
    } else {
        const observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) {
                    return;
                }

                entry.target.classList.add("is-visible");
                observer.unobserve(entry.target);
            });
        }, { threshold: 0.12, rootMargin: "0px 0px -8% 0px" });

        revealItems.forEach(function (item, index) {
            item.style.setProperty("--reveal-delay", `${Math.min(index * 55, 260)}ms`);
            observer.observe(item);
        });
    }

    const counters = document.querySelectorAll("[data-dashboard-count]");
    const animateCounter = function (element) {
        const target = Number(element.getAttribute("data-dashboard-count"));
        if (!Number.isFinite(target)) {
            return;
        }

        const prefix = element.getAttribute("data-count-prefix") || "";
        const suffix = element.getAttribute("data-count-suffix") || "";
        const decimals = Number(element.getAttribute("data-count-decimals") || "0");
        const duration = 900;
        const startedAt = performance.now();
        const formatter = new Intl.NumberFormat("en-US", {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        });

        const tick = function (now) {
            const progress = Math.min((now - startedAt) / duration, 1);
            const eased = 1 - Math.pow(1 - progress, 3);
            element.textContent = `${prefix}${formatter.format(target * eased)}${suffix}`;

            if (progress < 1) {
                requestAnimationFrame(tick);
            }
        };

        requestAnimationFrame(tick);
    };

    if (!prefersReducedMotion && counters.length > 0 && "IntersectionObserver" in window) {
        const counterObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) {
                    return;
                }

                animateCounter(entry.target);
                counterObserver.unobserve(entry.target);
            });
        }, { threshold: 0.75 });

        counters.forEach(function (counter) {
            counterObserver.observe(counter);
        });
    }
}());
