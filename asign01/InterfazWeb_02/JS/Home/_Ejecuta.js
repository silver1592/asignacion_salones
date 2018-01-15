var ejecuta_hora_ini = null;
var ejecuta_hora_fin = null;
var excel = null;
var sheet = null;
var operaciones = null;
var cicloEjecucion = null;

function Ejecuta() {
    $("#resConsola").children().remove();
    $("#resConsola").prepend($("<p>Iniciando Ejecucion</p>"));

    ejecuta_hora_ini = GetHora();
    operaciones = GetOperaciones();
    cicloEjecucion = selSemestre; //Variable definida en _Menu.js

    EjecutaHora()
}

function EjecutaHora()
{
    var _url = $(".direccion #Ejecuta").text().trim();
    var datos = {
        hora: ejecuta_hora_ini,
        operaciones: operaciones,
        ciclo: cicloEjecucion,
        excel: excel,
        hoja:sheet
    }
    var dt = JSON.stringify(datos);

    $.ajax({
        type: "POST",
        url: _url,
        contentType: "application/json; charset=utf-8",
        data: dt,
        dataType: "json",
        success: function (resultado) {
            $("#resConsola").prepend("<p>" + resultado + "</p>");
            ejecuta_hora_ini++;
            if (ejecuta_hora_ini < ejecuta_hora_fin) {
                EjecutaHora(ejecuta_hora_ini);
            }
            else {
                ejecuta_hora_ini = null;
                ejecuta_hora_fin = null;
                $("#resConsola").prepend("<p>Asignacion terminada</p>");
                alert("Operacion terminada");
            }
        },
        error: function (jqXHR, exception) {
            $("#resConsola").append("<p><strong>" + exception + "-" + ErrorToString(jqXHR, exception) + "<strong></p>");
            ejecuta_hora_ini = null;
            ejecuta_hora_fin = null;
        }
    });
}

function GetHora()
{
    var ini;
    var fin;

    if (ejecuta_hora_ini === null) {
        ini = parseInt($("[name='hora_ini']").val());
        fin = parseInt($("[name='hora_fin']").val());
        ejecuta_hora_fin = fin;
        ejecuta_hora_ini = ini;

        var d = new Date();

        excel = d.yyyymmdd() + ".xlsx"
        sheet = d.getHours().toString() + "_" + d.getMinutes().toString();
    }
    else {
        ini = ejecuta_hora_ini;
        fin = ejecuta_hora_fin;
    }

    return ini;

}

function GetOperaciones()
{
    var operaciones = [];
    if ($("[name='emp']").is(":checked"))
        operaciones.push(2);
    if ($("[name='pre']").is(":checked"))
        operaciones.push(3);
    if ($("[name='pre_otros_sem']").is(":checked"))
        operaciones.push(4);
    if ($("[name='asig']").is(":checked"))
        operaciones.push(1);
    
    return operaciones;
}