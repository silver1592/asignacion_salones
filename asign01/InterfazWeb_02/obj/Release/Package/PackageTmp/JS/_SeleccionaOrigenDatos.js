function changeExcel() {
    var excelName = $("#archivos option:selected").text();
    var _url = $(".direccion #changeExcel").text();

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
            error: ErrorFunction
        });
    }
}

function seleccionaOrigenDatos() {
    var excelName = $("#SeleccionExcel #archivos option:selected").text();
    var sheet = $("#SeleccionExcel #hojas option:selected").text();
    var operation = $("#SeleccionExcel input:checked").val();
    var semestre = $("#SeleccionExcel input[name='semestre']").val();
    var _url = $(".direccion #seleccionaOrigenDatos").text();

    if (operation == "e")
        var data = { excel: excelName, sheet: sheet, ciclo: semestre, bd: false }
    else
        var data = { excel: excelName, sheet: sheet, ciclo: semestre, bd: true }

    data = JSON.stringify(data);
    Wait();
    $.ajax({
        type: "POST",
        url: _url,
        contentType: "application/json; charset=utf-8",
        data: data,
        dataType: "json",
        success: function (resultado) {
            ActualizaOrigen();
        },
        error: ErrorFunction
    });
}