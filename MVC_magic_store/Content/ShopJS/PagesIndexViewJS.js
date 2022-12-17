$(function () {
    /* confirm page deletion */
    $("a.delete").click(function () {
        if (!confirm("Confirm page deletion")) return false;
    });
    /* change cursor when drag and drop */
    $("tr:not(.home)").mousedown(function () {
        $(this).css("cursor", "move");
    });
    $("tr:not(.home)").mouseup(function () {
        $(this).css("cursor", "default");
    });
    /* Sorting script */

    $("table#pages tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            var url = "/Admin/Pages/ReorderPages";
            $.post(url, ids, function (data) {
            });
        }
    });
});