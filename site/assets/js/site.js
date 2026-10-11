(function () {
  "use strict";

  function initNavToggle() {
    var toggle = document.querySelector(".nav-toggle");
    var nav = document.getElementById("site-nav");
    if (!toggle || !nav) return;
    toggle.addEventListener("click", function () {
      var open = toggle.getAttribute("aria-expanded") !== "true";
      toggle.setAttribute("aria-expanded", String(open));
      nav.classList.toggle("is-open", open);
    });
    document.addEventListener("keydown", function (event) {
      if (event.key === "Escape" && nav.classList.contains("is-open")) {
        toggle.setAttribute("aria-expanded", "false");
        nav.classList.remove("is-open");
        toggle.focus();
      }
    });
  }

  // Artwork comes from the podcast host. When it fails, hide it so the placeholder
  // behind it stays visible instead of a broken image.
  function hideFailed(img) {
    img.classList.add("is-failed");
  }

  function initArtworkFallback() {
    document.addEventListener(
      "error",
      function (event) {
        var target = event.target;
        if (target && target.matches && target.matches("img.artwork__img")) hideFailed(target);
      },
      true,
    );
    var images = document.querySelectorAll("img.artwork__img");
    for (var i = 0; i < images.length; i++) {
      if (images[i].complete && images[i].naturalWidth === 0 && images[i].currentSrc) hideFailed(images[i]);
    }
  }

  initNavToggle();
  initArtworkFallback();
})();
