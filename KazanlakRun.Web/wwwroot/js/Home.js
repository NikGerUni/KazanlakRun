document.addEventListener('DOMContentLoaded', function () {
    const target = new Date('May 31, 2026 10:00:00').getTime();
    const countdownEl = document.getElementById('countdown');
    if (!countdownEl) return;

    let timer;

    function showZeros() {
        countdownEl.innerText = '0d 0h 0m 0s';
    }

    function updateTimer() {
        const now = Date.now();
        const diff = target - now;

        if (diff <= 0) {
            showZeros();
            clearInterval(timer);
            return;
        }

        const days = Math.floor(diff / (1000 * 60 * 60 * 24));
        const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((diff % (1000 * 60)) / 1000);

        countdownEl.innerText = `${days}d ${hours}h ${minutes}m ${seconds}s`;
    }

    updateTimer();
    timer = setInterval(updateTimer, 1000);
});
