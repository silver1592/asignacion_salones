var dias = [false,false,false,false,false,false];

function Busqueda()
{
    var ini = $("input[name ='ini']").val();
    var fin = $("input[name='fin']").val();

    dias = [false,false,false,false,false,false];
    $("input[name='dias']:checked").each(function () {
        switch ($(this).val()) {
            case 'L': dias[0] = true;
                break;
            case 'M': dias[1] = true;
                break;
            case 'Mi': dias[2] = true;
                break;
            case 'J': dias[3] = true;
                break;
            case 'V': dias[4] = true;
                break;
            case 'S': dias[5] = true;
                break;
        }
    });

    var _dias="";
    for (var i = 0; i < 6; i++)
        if (dias[i])
            _dias += "1";
        else
            _dias += "0";

    $("#lista").empty();

    var url = $(".direccion #buscaGrupos").text().trim();
    var dt = JSON.stringify({ ini: ini, fin: fin, dias: _dias });

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: dt,
        success: function (resultado) {
            $("#lista").append(resultado);
        },
        error: function (jqXHR, exception) {
            alert(exception);
        }
    });
}