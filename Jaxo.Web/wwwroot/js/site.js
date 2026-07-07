// The hero ring flies up and docks into the logo's "o" as you scroll.
// Scroll-linked (not time-based), so it scrubs naturally in both directions.
(function () {
    var ring = document.querySelector(".jx-hero-ring");
    var hero = document.querySelector(".jx-hero");
    var logoO = document.querySelector(".jx-logo-o");
    var nav = document.querySelector(".jx-nav");
    if (!ring || !hero || !logoO || !nav) return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

    var base = null;

    function measure() {
        ring.style.transform = "";
        var rr = ring.getBoundingClientRect();
        var hr = hero.getBoundingClientRect();
        base = { cx: rr.left + rr.width / 2 - hr.left, cy: rr.top + rr.height / 2 - hr.top, w: rr.width };
    }

    function update() {
        if (getComputedStyle(ring).display === "none") {
            ring.style.transform = "";
            ring.style.opacity = "";
            return;
        }
        if (!base) measure();
        var travel = Math.max(hero.offsetHeight - nav.offsetHeight, 1);
        var p = Math.min(Math.max(window.scrollY / travel, 0), 1);
        if (p <= 0) {
            ring.style.transform = "";
            ring.style.opacity = "";
            logoO.classList.remove("jx-docked");
            return;
        }
        var hr = hero.getBoundingClientRect();
        var t = logoO.getBoundingClientRect();
        var baseCx = hr.left + base.cx;
        var baseCy = hr.top + base.cy;
        var dx = (t.left + t.width / 2 - baseCx) * p * p; // slide toward the logo late, once the ring is small
        var dy = (t.top + t.height / 2 - baseCy) * p;
        var s = 1 + (t.width / base.w - 1) * p;
        ring.style.transform = "translate(" + dx + "px, " + dy + "px) translateY(-50%) scale(" + s + ")";
        ring.style.opacity = p > 0.85 ? String(Math.max(1 - (p - 0.85) / 0.15, 0)) : "";
        if (p >= 0.95) logoO.classList.add("jx-docked");
        else if (p < 0.5) logoO.classList.remove("jx-docked");
    }

    var ticking = false;
    function onScroll() {
        if (ticking) return;
        ticking = true;
        requestAnimationFrame(function () { ticking = false; update(); });
    }

    window.addEventListener("scroll", onScroll, { passive: true });
    window.addEventListener("resize", function () { base = null; onScroll(); });
    update();
})();

// Draw the process-step arcs when they scroll into view.
(function () {
    var svgs = document.querySelectorAll(".jx-step svg");
    if (!svgs.length || !("IntersectionObserver" in window)) return;
    svgs.forEach(function (svg) { svg.classList.add("jx-anim"); });
    var io = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add("jx-drawn");
                io.unobserve(entry.target);
            }
        });
    }, { threshold: 0.4 });
    svgs.forEach(function (svg) { io.observe(svg); });
})();
