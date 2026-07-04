(function () {
    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    const header = document.querySelector("[data-landing-header]");
    const mobileToggle = document.querySelector("[data-mobile-nav-toggle]");
    const mobileNav = document.querySelector("[data-mobile-nav]");

    const setMobileNav = function (isOpen) {
        if (!mobileToggle || !mobileNav) {
            return;
        }

        mobileToggle.setAttribute("aria-expanded", String(isOpen));
        mobileNav.classList.toggle("hidden", !isOpen);
        mobileNav.classList.toggle("is-open", isOpen);
        document.body.classList.toggle("landing-nav-open", isOpen);
    };

    if (mobileToggle && mobileNav) {
        mobileToggle.addEventListener("click", function () {
            const isOpen = mobileToggle.getAttribute("aria-expanded") !== "true";
            setMobileNav(isOpen);
        });

        mobileNav.querySelectorAll("a").forEach(function (link) {
            link.addEventListener("click", function () {
                setMobileNav(false);
            });
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                setMobileNav(false);
            }
        });

        document.addEventListener("click", function (event) {
            if (mobileNav.classList.contains("hidden")) {
                return;
            }

            const clickedInside = mobileNav.contains(event.target) || mobileToggle.contains(event.target);
            if (!clickedInside) {
                setMobileNav(false);
            }
        });
    }

    const heroSection = document.querySelector("#product");

    if (header && heroSection && "IntersectionObserver" in window) {
        const headerObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                header.classList.toggle("is-scrolled", !entry.isIntersecting);
            });
        }, {
            rootMargin: "-110px 0px 0px 0px",
            threshold: 0
        });

        headerObserver.observe(heroSection);
    }

    const revealItems = document.querySelectorAll("[data-reveal]");

    if (prefersReducedMotion || !("IntersectionObserver" in window)) {
        revealItems.forEach(function (item) {
            item.classList.add("is-visible");
        });
    } else {
        const revealObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) {
                    return;
                }

                entry.target.classList.add("is-visible");
                revealObserver.unobserve(entry.target);
            });
        }, {
            rootMargin: "0px 0px -12% 0px",
            threshold: 0.12
        });

        revealItems.forEach(function (item, index) {
            item.style.setProperty("--reveal-delay", `${Math.min(index * 55, 260)}ms`);
            revealObserver.observe(item);
        });
    }

    const navLinks = Array.from(document.querySelectorAll("a[href^='#']"));
    const sections = navLinks
        .map(function (link) {
            return document.querySelector(link.getAttribute("href"));
        })
        .filter(Boolean);

    if (sections.length > 0 && "IntersectionObserver" in window) {
        const sectionObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) {
                    return;
                }

                navLinks.forEach(function (link) {
                    link.classList.toggle("is-active", link.getAttribute("href") === `#${entry.target.id}`);
                });
            });
        }, {
            rootMargin: "-36% 0px -58% 0px",
            threshold: 0
        });

        sections.forEach(function (section) {
            sectionObserver.observe(section);
        });
    }

    const counters = document.querySelectorAll("[data-counter]");

    const animateCounter = function (element) {
        const target = Number(element.getAttribute("data-counter"));
        if (!Number.isFinite(target)) {
            return;
        }

        const duration = 900;
        const startedAt = performance.now();
        const formatter = new Intl.NumberFormat("en-US");

        const tick = function (now) {
            const progress = Math.min((now - startedAt) / duration, 1);
            const eased = 1 - Math.pow(1 - progress, 3);
            element.textContent = formatter.format(Math.round(target * eased));

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
        }, {
            threshold: 0.7
        });

        counters.forEach(function (counter) {
            counterObserver.observe(counter);
        });
    }
}());
