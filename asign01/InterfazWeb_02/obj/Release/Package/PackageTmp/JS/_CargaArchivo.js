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
        success: function (resultado) {
            if (resultado[0] != true) alert(resultado[1]);
        },
        error: ErrorFunction
    });
}