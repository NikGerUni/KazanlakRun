
document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('FilterText');
    if (!input) return;

    input.addEventListener('input', () => {
        const filter = input.value;
        const url = new URL(window.location.href);
        url.searchParams.set('filter', filter);
        url.searchParams.set('page', '1');
        window.location.href = url.toString();
    });
});



