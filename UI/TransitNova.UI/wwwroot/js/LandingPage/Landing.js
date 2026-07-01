(function () {
    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    const header = document.querySelector("[data-landing-header]");
    const navToggle = document.querySelector("[data-nav-toggle]");
    const navMenu = document.querySelector("[data-nav-menu]");

    const closeMenu = function () {
        if (!navMenu || !navToggle) {
            return;
        }

        navMenu.classList.remove("is-open");
        navToggle.setAttribute("aria-expanded", "false");
    };

    if (navToggle && navMenu) {
        navToggle.addEventListener("click", function () {
            const isOpen = navMenu.classList.toggle("is-open");
            navToggle.setAttribute("aria-expanded", String(isOpen));
        });

        navMenu.querySelectorAll("a").forEach(function (link) {
            link.addEventListener("click", closeMenu);
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                closeMenu();
            }
        });

        document.addEventListener("click", function (event) {
            if (!navMenu.classList.contains("is-open")) {
                return;
            }

            const clickedInside = navMenu.contains(event.target) || navToggle.contains(event.target);
            if (!clickedInside) {
                closeMenu();
            }
        });
    }

    const revealItems = document.querySelectorAll("[data-reveal]");

    if (prefersReducedMotion || !("IntersectionObserver" in window)) {
        revealItems.forEach(function (item) {
            item.classList.add("is-visible");
        });
    } else {
        const revealObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    revealObserver.unobserve(entry.target);
                }
            });
        }, {
            rootMargin: "0px 0px -10% 0px",
            threshold: 0.12
        });

        revealItems.forEach(function (item, index) {
            item.style.setProperty("--reveal-delay", `${Math.min(index * 45, 240)}ms`);
            revealObserver.observe(item);
        });
    }

    const hero = document.querySelector(".hero-section");

    if (header && hero && "IntersectionObserver" in window) {
        const headerObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                header.classList.toggle("is-scrolled", !entry.isIntersecting);
            });
        }, {
            rootMargin: "-120px 0px 0px 0px",
            threshold: 0
        });

        headerObserver.observe(hero);
    }

    const navLinks = Array.from(document.querySelectorAll(".nav-menu a[href^='#']"));
    const sections = navLinks
        .map(function (link) {
            const id = link.getAttribute("href");
            return id ? document.querySelector(id) : null;
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
            rootMargin: "-35% 0px -55% 0px",
            threshold: 0
        });

        sections.forEach(function (section) {
            sectionObserver.observe(section);
        });
    }

    const numberItems = document.querySelectorAll(".number-card strong");

    const animateNumber = function (element) {
        const original = element.textContent.trim();
        const match = original.match(/^(\d+)(.*)$/);

        if (!match) {
            return;
        }

        const target = Number(match[1]);
        const suffix = match[2] || "";
        const duration = 950;
        const startedAt = performance.now();

        const tick = function (now) {
            const progress = Math.min((now - startedAt) / duration, 1);
            const eased = 1 - Math.pow(1 - progress, 3);
            element.textContent = `${Math.round(target * eased)}${suffix}`;

            if (progress < 1) {
                requestAnimationFrame(tick);
            } else {
                element.textContent = original;
            }
        };

        requestAnimationFrame(tick);
    };

    if (!prefersReducedMotion && numberItems.length > 0 && "IntersectionObserver" in window) {
        const numberObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    animateNumber(entry.target);
                    numberObserver.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.6
        });

        numberItems.forEach(function (item) {
            numberObserver.observe(item);
        });
    }
}());
