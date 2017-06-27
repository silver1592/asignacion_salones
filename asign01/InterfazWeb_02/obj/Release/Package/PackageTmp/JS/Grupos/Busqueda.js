var dias = [false,false,false,false,false,false];

function Busqueda()
{
    var cve = $("#cve_materia option:selected").val();
    var nGrupo = $("input[name='noGrupo']").val();
    var ini = $("input[name ='ini']").val();
    var fin = $("input[name='fin']").val();

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

    $("#lista").empty();

    //TODO: Llamada para la busqueda

    //TODO: Carga del resultado de busqueda

    //clv-chica-linda XD:197029
}