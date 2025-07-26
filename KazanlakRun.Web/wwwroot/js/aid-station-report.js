document.addEventListener('DOMContentLoaded', function() {
    var input = document.getElementById('filterInput');
    var container = document.getElementById('stationsContainer');
    var blocks = Array.from(container.querySelectorAll('.station-block'));
    var firstBtn = document.getElementById('firstBtn');
    var prevBtn = document.getElementById('prevBtn');
    var nextBtn = document.getElementById('nextBtn');
    var lastBtn = document.getElementById('lastBtn');
    var pageInfo = document.getElementById('pageInfo');
    var current = 0;

    function renderPage()
    {
        blocks.forEach((b, i) => {
            b.style.display = (i === current) ? '' : 'none';
        });
        pageInfo.textContent = (current + 1) + ' / ' + blocks.length;
        firstBtn.disabled = (current === 0);
        prevBtn.disabled = (current === 0);
        nextBtn.disabled = (current === blocks.length - 1);
        lastBtn.disabled = (current === blocks.length - 1);
    }

    function filterGoods(e)
    {
        if (e && e.key === 'Enter') e.preventDefault();
        var filter = input.value.trim().toLowerCase();
        blocks.forEach(function(block) {
            var rows = block.querySelectorAll('tbody tr');
            var anyVisible = false;
            rows.forEach(function(row) {
                var text = row.cells[0].textContent.trim().toLowerCase();
                if (text.includes(filter))
                {
                    row.style.display = '';
                    anyVisible = true;
                }
                else
                {
                    row.style.display = 'none';
                }
            });
            block.style.display = anyVisible ? '' : 'none';
        });
        blocks.forEach((b, i) => {
            if (b.style.display === '') { current = i; return false; }
        });
        renderPage();
    }

    firstBtn.addEventListener('click', () => { current = 0; renderPage(); });
    prevBtn.addEventListener('click', () => { current--; renderPage(); });
    nextBtn.addEventListener('click', () => { current++; renderPage(); });
    lastBtn.addEventListener('click', () => { current = blocks.length - 1; renderPage(); });
    input.addEventListener('input', filterGoods);
    input.addEventListener('keydown', filterGoods);
    renderPage();
});
