function uploadExcelToDB() {
    Wait();
    $.ajax({
        type: "POST",
        url: '/Importacion/CargaExcel_BD',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultado) {
            if (resultado[0] != true) alert(resultado[1]);
        },
        error: ErrorFunction
    });
}