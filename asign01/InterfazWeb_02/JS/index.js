$().ready(function () {
    $("#importacionTabs").tabs({ collapsible: true });
    $("#tabs").tabs({ collapsible: true });
    $("#archivos").change(changeExcel);
});

function changeExcel() {
    var excelName = $("#archivos option:selected").text();

    $("#hojas option").remove();
    if (excelName != "---------") {
        $.ajax({
            type: "POST",
            url: '/Consultas/PaginasExcel',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ file: excelName }),
            dataType: "json",
            success: function (resultado) {
                $.each(resultado, function (index, item) {
                    $("#hojas").append($("<option>" + item + "</option>"))
                });
            },
            error: function (jqXHR, exception) {
                var msg = '';
                if (jqXHR.status === 0) {
                    msg = 'Not connect.\n Verify Network.';
                } else if (jqXHR.status == 404) {
                    msg = 'Requested page not found. [404]';
                } else if (jqXHR.status == 500) {
                    msg = 'Internal Server Error [500].';
                } else if (exception === 'parsererror') {
                    msg = 'Requested JSON parse failed.';
                } else if (exception === 'timeout') {
                    msg = 'Time out error.';
                } else if (exception === 'abort') {
                    msg = 'Ajax request aborted.';
                } else {
                    msg = 'Uncaught Error.\n' + jqXHR.responseText;
                }
                alert(msg);
            }
        });
    }
}