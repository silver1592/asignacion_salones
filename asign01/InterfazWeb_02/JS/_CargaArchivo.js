function uploadExcelToDB() {
    var _url = $(".direccion #uploadExcelToDB").text();
    $.ajax({
        type: "POST",
        url: _url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultado) {
            if (resultado[0] != true) alert(resultado[1]);
        },
        error: ErrorFunction
    });
}