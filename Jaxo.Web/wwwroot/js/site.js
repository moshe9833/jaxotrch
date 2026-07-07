// Each section title's small ring flies up into the logo's "o" when the
// section scrolls past the nav, and returns when you scroll back down.
(function () {
    var logoO = document.querySelector(".jx-logo-o");
    var nav = document.querySelector(".jx-nav");
    var eyebrows = Array.prototype.slice.call(document.querySelectorAll(".jx-eyebrow"));
    if (!logoO || !nav || !eyebrows.length) return;
    var reduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    function fly(eb) {
        if (reduced) return;
        var r = eb.getBoundingClientRect();
        var t = logoO.getBoundingClientRect();
        var size = 10;
        var sx = r.left + 1;
        var sy = r.top + r.height / 2 - size / 2;
        var el = document.createElement("span");
        el.className = "jx-fly-ring";
        el.style.left = sx + "px";
        el.style.top = sy + "px";
        if (eb.closest(".jx-dark")) el.style.borderColor = "#fff";
        el.style.borderTopColor = "#2EE6A8";
        document.body.appendChild(el);
        var dx = t.left + t.width / 2 - (sx + size / 2);
        var dy = t.top + t.height / 2 - (sy + size / 2);
        var s = Math.max(t.width / size, 1);
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                el.style.transform = "translate(" + dx + "px, " + dy + "px) scale(" + s + ") rotate(45deg)";
                el.style.borderColor = "#0B1E3F";
                el.style.borderTopColor = "#2EE6A8";
                el.style.opacity = "0";
            });
        });
        el.addEventListener("transitionend", function done() {
            el.removeEventListener("transitionend", done);
            el.remove();
            logoO.classList.remove("jx-docked");
            logoO.getBoundingClientRect(); // restart the spin animation
            logoO.classList.add("jx-docked");
            setTimeout(function () { logoO.classList.remove("jx-docked"); }, 650);
        });
    }

    function check(initial) {
        var navBottom = nav.getBoundingClientRect().bottom;
        eyebrows.forEach(function (eb) {
            var crossed = eb.getBoundingClientRect().top < navBottom + 4;
            if (crossed && !eb.classList.contains("jx-flown")) {
                eb.classList.add("jx-flown");
                if (!initial) fly(eb);
            } else if (!crossed && eb.classList.contains("jx-flown")) {
                eb.classList.remove("jx-flown");
            }
        });
    }

    var ticking = false;
    window.addEventListener("scroll", function () {
        if (ticking) return;
        ticking = true;
        requestAnimationFrame(function () { ticking = false; check(false); });
    }, { passive: true });
    check(true);
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
