(function () {
    let rowTemplate = "";

    // make markDeleted global
    window.markDeleted = function (btn) {
        const $row = $(btn).closest("tr");
        $row.find(".is-deleted").val("true");
        $row.hide();
    };

    $(function () {
        const $addBtn = $("#addRow");
        const $tbody = $("#tableBody");

        // 1) Деактивираме бутона докато чакаме шаблона
        $addBtn.prop("disabled", true);

        // 2) Зареждаме шаблона през AJAX
        console.log("Loading row template...");
        $.get("/Admin/Role/RowTemplate")
            .done(function (html) {
                console.log("Row template loaded.");
                rowTemplate = html;

                // 3) Активираме бутона и закачаме handler
                $addBtn.prop("disabled", false)
                    .on("click", function () {
                        const idx = $tbody.children("tr").length;
                        const newRow = rowTemplate.replace(/__index__/g, idx);
                        $tbody.append(newRow);
                        $.validator.unobtrusive.parse($tbody.children().last());
                    });
            })
            .fail(function () {
                console.error("Failed to load /Admin/Role/RowTemplate");
                // може да покажете уведомление за потребителя
            });

        // 4) Инициираме client‑side валидация за вече съществуващите редове
        $.validator.unobtrusive.parse($tbody);

        // 5) Плавно скриване на success alert (ако съществува)
        const $alert = $("#success-alert");
        if ($alert.length) {
            setTimeout(() => {
                $alert.fadeOut();  // или $alert.slideUp();
            }, 3000);
        }
    });
})();
