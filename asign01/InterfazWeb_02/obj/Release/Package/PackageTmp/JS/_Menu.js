function ActualizaOrigen()
{
    $.ajax({
        type: "POST",
        url: '@Url.Action("ExcelValid","Consultas",null,Request.Url.Scheme)',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultado) {
            $("#dataBaseName").text(resultado[1]);
            $("#dataBaseStatus").text(resultado[0]);
        },
        error: ErrorFunction
    });
}