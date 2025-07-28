(function () {
    function getCookie(name) {
        var match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
        return match ? decodeURIComponent(match[2]) : null;
    }
    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + encodeURIComponent(value) + expires + "; path=/";
    }

    document.addEventListener('DOMContentLoaded', function () {
        var filterInput = document.getElementById('filterInput');
        var pageSizeInput = document.getElementById('pageSizeInput');
        var table = document.getElementById('reportTable');
        var rows = Array.from(table.tBodies[0].rows);
        var firstBtn = document.getElementById('firstBtn');
        var prevBtn = document.getElementById('prevBtn');
        var nextBtn = document.getElementById('nextBtn');
        var lastBtn = document.getElementById('lastBtn');
        var pageInfo = document.getElementById('pageInfo');

        var defaultSize = window.goodsForDeliveryDefaultPageSize || 10;
        var savedSize = getCookie('GoodsForDeliveryPageSize');
        pageSizeInput.value = (savedSize && !isNaN(savedSize))
            ? savedSize
            : defaultSize;

        var currentPage = 0;
        var pageSize = parseInt(pageSizeInput.value) || defaultSize;
        var totalPages = Math.ceil(rows.length / pageSize);

        function renderTable() {
            var filter = filterInput.value.trim().toLowerCase();
            var filteredRows = rows.filter(r =>
                Array.from(r.cells).some(c =>
                    c.textContent.trim().toLowerCase().includes(filter)
                )
            );

            pageSize = parseInt(pageSizeInput.value) || defaultSize;
            totalPages = Math.ceil(filteredRows.length / pageSize) || 1;
            if (currentPage >= totalPages) currentPage = totalPages - 1;

            setCookie('GoodsForDeliveryPageSize', pageSize, 365);

            rows.forEach(r => r.style.display = 'none');
            var start = currentPage * pageSize;
            var end = start + pageSize;
            filteredRows.slice(start, end).forEach(r => r.style.display = '');

            pageInfo.textContent = (currentPage + 1) + ' / ' + totalPages;
            firstBtn.disabled = currentPage === 0;
            prevBtn.disabled = currentPage === 0;
            nextBtn.disabled = currentPage === totalPages - 1;
            lastBtn.disabled = currentPage === totalPages - 1;
        }

        ['input', 'keyup', 'change'].forEach(evt =>
            filterInput.addEventListener(evt, () => { currentPage = 0; renderTable(); })
        );
        pageSizeInput.addEventListener('change', () => { currentPage = 0; renderTable(); });
        firstBtn.addEventListener('click', () => { currentPage = 0; renderTable(); });
        prevBtn.addEventListener('click', () => { if (currentPage > 0) currentPage--; renderTable(); });
        nextBtn.addEventListener('click', () => { if (currentPage < totalPages - 1) currentPage++; renderTable(); });
        lastBtn.addEventListener('click', () => { currentPage = totalPages - 1; renderTable(); });

        renderTable();
    });
})();
