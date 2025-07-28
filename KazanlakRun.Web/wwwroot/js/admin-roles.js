(function () {
    let rowTemplate = "";

    window.markDeleted = function (btn) {
        const $row = $(btn).closest("tr");
        $row.find(".is-deleted").val("true");
        $row.hide();
    };

    $(function () {
        const $addBtn = $("#addRow");
        const $tbody = $("#tableBody");

        $addBtn.prop("disabled", true);

        console.log("Loading row template...");
        $.get("/Admin/Role/RowTemplate")
            .done(function (html) {
                console.log("Row template loaded.");
                rowTemplate = html;

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
            });

        $.validator.unobtrusive.parse($tbody);

        const $alert = $("#success-alert");
        if ($alert.length) {
            setTimeout(() => {
                $alert.fadeOut();  // или $alert.slideUp();
            }, 3000);
        }
    });
})();
