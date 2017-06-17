function ActualizaOrigen()
{
    var _url = $(".direccion #ActualizaOrigen").text();
    $.ajax({
        type: "POST",
        url: _url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultado) {
            $("#dataBaseName").text(resultado[1]);
            $("#dataBaseStatus").text(resultado[0]);
        },
        error: ErrorFunction
    });
}