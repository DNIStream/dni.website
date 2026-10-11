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

  var MESSAGES = {
    captcha: 'Please complete the "I am human" check before sending.',
    sending: "Sending…",
    sent: "Thanks — your message is on its way. We'll be in touch.",
    failed: "Something went wrong. Please try again.",
    offline: "Network error — please try again in a moment.",
  };

  // Without JavaScript the form posts natively to the form service.
  function initContactForm() {
    var form = document.querySelector("form.contact-form");
    if (!form || !window.fetch) return;
    var status = form.querySelector(".form-status");
    var button = form.querySelector('button[type="submit"]');
    var sending = false;

    function show(text, kind) {
      status.textContent = text;
      status.className = "form-status" + (kind ? " is-" + kind : "");
    }

    form.addEventListener("submit", function (event) {
      event.preventDefault();
      if (sending) return;
      var token = form.querySelector('[name="h-captcha-response"]');
      if (!token || !token.value) {
        show(MESSAGES.captcha, "error");
        var captcha = form.querySelector(".h-captcha");
        if (captcha) captcha.scrollIntoView({ block: "center" });
        return;
      }
      sending = true;
      button.disabled = true;
      show(MESSAGES.sending);

      fetch(form.action, { method: "POST", body: new FormData(form), headers: { Accept: "application/json" } })
        .then(function (response) {
          return response
            .json()
            .catch(function () {
              return {};
            })
            .then(function (data) {
              if (response.ok && data.success) {
                form.reset();
                show(MESSAGES.sent, "ok");
                if (window.hcaptcha) window.hcaptcha.reset();
              } else {
                show(data.message || MESSAGES.failed, "error");
              }
            });
        })
        .catch(function () {
          show(MESSAGES.offline, "error");
        })
        .then(function () {
          sending = false;
          button.disabled = false;
        });
    });
  }

  initNavToggle();
  initArtworkFallback();
  initContactForm();
})();
