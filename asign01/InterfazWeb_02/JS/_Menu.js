var selSemestre = "";

function ActualizaOrigen()
{
    var _url = $(".direccion #ActualizaOrigen").text().trim();
    var datos = { semestre: selSemestre }
    var dt = JSON.stringify(datos);
    $.ajax({
        type: "POST",
        url: _url,
        contentType: "application/json; charset=utf-8",
        data: dt,
        dataType: "json",
        success: function (resultado) {
            $("#dataBaseStatus").text(resultado);
        },
        error: ErrorFunction
    });
}

function inicializaMenu()
{
    $("#selectSemestre").change(function () {
        $("#selectSemestre option:selected").each(function () {
            selSemestre = $(this).text();
        });
        ActualizaOrigen();
    })

    $("#selectSemestre option:selected").each(function () {
        selSemestre = $(this).text();
    });
}