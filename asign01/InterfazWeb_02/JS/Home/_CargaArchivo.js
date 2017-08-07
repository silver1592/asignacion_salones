function uploadExcelToDB() {
    var _url = $(".direccion #uploadExcelToDB").text().trim();

    var excelName = $("#SeleccionExcel #archivos option:selected").text();
    var sheet = $("#SeleccionExcel #hojas option:selected").text();
    var operation = $("#SeleccionExcel input:checked").val();
    var semestre = $("#SeleccionExcel input[name='semestre']").val();

    var data = { excel: excelName, sheet: sheet, ciclo: semestre }

    data = JSON.stringify(data);

    $.ajax({
        type: "POST",
        url: _url,
        contentType: "application/json; charset=utf-8",
        data: data,
        dataType: "json",
        beforeSend: Wait,
        complete: Continue,
        success: function (resultado) {
            if (resultado[0] != true) alert(resultado[1]);
        },
        error: ErrorFunction
    });
}

function changeExcel() {
    var excelName = $("#archivos option:selected").text();
    var _url = $(".direccion #changeExcel").text().trim();

    $("#hojas option").remove();
    if (excelName != "---------") {
        $.ajax({
            type: "POST",
            url: _url,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ file: excelName }),
            dataType: "json",
            success: function (resultado) {
                $.each(resultado, function (index, item) {
                    $("#hojas").append($("<option>" + item + "</option>"))
                });
            },
            error: ErrorFunction,
            beforeSend: Wait,
            complete: Continue,
        });
    }
}