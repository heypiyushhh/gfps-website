// Client-side interactions for Green Field Public School (public site)

document.addEventListener("DOMContentLoaded", () => {

    // =====================================================
    // 1. STICKY NAVBAR — scroll-based opacity & shadow
    // Source reference: Stitch Home screen inline JS
    // On scroll > 20px: add 'navbar-scrolled' class
    // which changes background from rgba(255,255,255,0.7) → rgba(255,255,255,0.92)
    // and adds a shadow — all handled in site.scss via #navbar.navbar-scrolled {}
    // =====================================================
    const navbar = document.getElementById("navbar");
    if (navbar) {
        const handleScroll = () => {
            if (window.scrollY > 20) {
                navbar.classList.add("navbar-scrolled");
            } else {
                navbar.classList.remove("navbar-scrolled");
            }
        };
        // Run once on page load (in case user refreshes mid-page)
        handleScroll();
        window.addEventListener("scroll", handleScroll, { passive: true });
    }

    // =====================================================
    // 2. INTERSECTION OBSERVER — scroll reveal animations
    // Source: animate-fade-up class gets 'visible' on intersection
    // =====================================================
    const observerOptions = {
        root: null,
        rootMargin: "0px",
        threshold: 0.1
    };

    const revealObserver = new IntersectionObserver((entries, obs) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add("visible");
                obs.unobserve(entry.target); // Trigger only once
            }
        });
    }, observerOptions);

    document.querySelectorAll(".animate-fade-up").forEach(el => {
        revealObserver.observe(el);
    });

    // =====================================================
    // 3. MOBILE MENU — keyboard accessibility (Escape key)
    // =====================================================
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            const mobileMenu = document.getElementById("mobileMenu");
            if (mobileMenu && mobileMenu.classList.contains("show")) {
                // Collapse Bootstrap collapse
                const bsCollapse = bootstrap.Collapse.getInstance(mobileMenu);
                if (bsCollapse) bsCollapse.hide();
            }
        }
    });

    // =====================================================
    // 4. GALLERY FILTER TABS (if present on page)
    // =====================================================
    const filterBtns = document.querySelectorAll("[data-gallery-filter]");
    if (filterBtns.length > 0) {
        filterBtns.forEach(btn => {
            btn.addEventListener("click", () => {
                const filter = btn.getAttribute("data-gallery-filter");
                filterBtns.forEach(b => b.classList.remove("active", "btn-primary"));
                filterBtns.forEach(b => b.classList.add("btn-outline-secondary"));
                btn.classList.add("active", "btn-primary");
                btn.classList.remove("btn-outline-secondary");

                document.querySelectorAll("[data-gallery-category]").forEach(item => {
                    if (filter === "all" || item.getAttribute("data-gallery-category") === filter) {
                        item.style.display = "";
                    } else {
                        item.style.display = "none";
                    }
                });
            });
        });
    }

});
