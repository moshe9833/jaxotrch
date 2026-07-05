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
