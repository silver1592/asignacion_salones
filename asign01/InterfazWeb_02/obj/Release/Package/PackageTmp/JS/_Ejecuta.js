var ejecuta_hora_ini = null;
var ejecuta_hora_fin = null;

function Ejecuta() {
    var datos = GetDatosEjecuta();
    $("#resConsola").children().remove();

    EjecutaHora()
}

function EjecutaHora()
{
    //TODO: hacer que lance un evento al terminar que haga que se ejecute el que sigue
    //while (!siguiente);
    //siguiente = false;
    var datos = GetDatosEjecuta();
    var dt = JSON.stringify(datos);
    var _url = $(".direccion #Ejecuta").text();

    $.ajax({
        type: "POST",
        url: _url,
        contentType: "application/json; charset=utf-8",
        data: dt,
        dataType: "json",
        success: function (resultado) {
            $("#resConsola").append("<p>" + resultado + "</p>");
            ejecuta_hora_ini++;
            if (ejecuta_hora_ini <= ejecuta_hora_fin) {
                EjecutaHora();
            }
            else {
                ejecuta_hora_ini = null;
                ejecuta_hora_fin = null;
            }
        },
        error: function (jqXHR, exception) {
            $("#resConsola").append("<p><strong>" + exception + "-" + ErrorToString(jqXHR, exception) + "<strong></p>");
            ejecuta_hora_ini = null;
            ejecuta_hora_fin = null;
        }
    });
}

function GetDatosEjecuta()
{
    var ini;
    var fin;

    if (ejecuta_hora_ini === null)
    {
        ini = parseInt($("[name='hora_ini']").val());
        fin = parseInt($("[name='hora_fin']").val());
        ejecuta_hora_fin = fin;
        ejecuta_hora_ini = ini;
    }
    else
    {
        ini = ejecuta_hora_ini;
        fin = ejecuta_hora_fin;
    }

    var bEmpalmes = $("[name='emp']").is(":checked");

    var bPreasignacion = $("[name='pre']").is(":checked");
    var bOtrosSemestres = $("[name='pre_otros_sem']").is(":checked");

    var bAlgoritmo = $("[name='asig']").is(":checked");
    var iIndividuos = parseInt($("[name='alg_individuos']").val());
    var iGeneraciones = parseInt($("[name='alg_generaciones']").val());

    datos = {
        hora: ini,
        hora_fin:fin,
        empalmes: bEmpalmes,
        preasignacion: bPreasignacion,
        otrosSemestres: bOtrosSemestres,
        algoritmo: bAlgoritmo,
        individuo: iIndividuos,
        generacion: iGeneraciones
    }

    return datos;
}