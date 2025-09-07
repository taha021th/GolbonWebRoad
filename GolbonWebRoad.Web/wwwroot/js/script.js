// Wait for the DOM to be fully loaded before running scripts
document.addEventListener("DOMContentLoaded", function () {

    // Initialize AOS (Animate on Scroll)
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800, // Animation duration
            once: true,    // Animate elements only once
        });
    }

    // Theme Switcher Logic
    (function themeSwitcher() {
        const themeToggleBtn = document.getElementById('theme-toggle');
        const body = document.body;

        if (!themeToggleBtn) return;

        // 1. Check localStorage when the page loads
        const currentTheme = localStorage.getItem('theme');
        if (currentTheme === 'light') {
            body.classList.add('light-mode');
        }

        // 2. Add click event to the toggle button
        themeToggleBtn.addEventListener('click', function () {
            body.classList.toggle('light-mode');

            // 3. Save the user's choice in localStorage
            if (body.classList.contains('light-mode')) {
                localStorage.setItem('theme', 'light');
            } else {
                localStorage.setItem('theme', 'dark');
            }
        });
    })();

    // Initialize Swiper sliders if they exist on the page
    if (typeof Swiper !== 'undefined') {
        // Category Slider
        if (document.querySelector('.category-swiper')) {
            const categorySwiper = new Swiper('.category-swiper', {
                loop: true,
                spaceBetween: 30,
                slidesPerView: 1,
                breakpoints: { 640: { slidesPerView: 2, spaceBetween: 20 }, 768: { slidesPerView: 3, spaceBetween: 30 }, 1024: { slidesPerView: 4, spaceBetween: 30 } },
                pagination: { el: '.category-swiper .swiper-pagination', clickable: true },
                navigation: { nextEl: '.category-swiper .swiper-button-next', prevEl: '.category-swiper .swiper-button-prev' }
            });
        }

        // Product Slider
        if (document.querySelector('.product-swiper')) {
            const productSwiper = new Swiper('.product-swiper', {
                loop: true,
                spaceBetween: 30,
                slidesPerView: 1,
                breakpoints: { 640: { slidesPerView: 2, spaceBetween: 20 }, 768: { slidesPerView: 3, spaceBetween: 30 }, 1024: { slidesPerView: 4, spaceBetween: 30 } },
                pagination: { el: '.product-swiper .swiper-pagination', clickable: true },
                navigation: { nextEl: '.product-swiper .swiper-button-next', prevEl: '.product-swiper .swiper-button-prev' }
            });
        }
    }

    // Deal of the Day Countdown Timer
    (function startCountdown() {
        const countdownTimerEl = document.getElementById("countdown-timer");
        if (!countdownTimerEl) return;

        const today = new Date();
        const tomorrow = new Date(today);
        tomorrow.setDate(tomorrow.getDate() + 1);
        tomorrow.setHours(0, 0, 0, 0);
        const countDownDate = tomorrow.getTime();

        const timer = setInterval(function () {
            const now = new Date().getTime();
            const distance = countDownDate - now;

            const days = Math.floor(distance / (1000 * 60 * 60 * 24));
            const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            document.getElementById("days").innerText = days;
            document.getElementById("hours").innerText = hours;
            document.getElementById("minutes").innerText = minutes;
            document.getElementById("seconds").innerText = seconds;

            if (distance < 0) {
                clearInterval(timer);
                countdownTimerEl.innerHTML = "<p>تخفیف به پایان رسید!</p>";
            }
        }, 1000);
    })();
});

