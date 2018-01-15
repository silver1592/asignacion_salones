var ejecuta_hora_ini = null;
var ejecuta_hora_fin = null;
var excel = null;
var sheet = null;

function Ejecuta() {
    $("#resConsola").children().remove();
    $("#resConsola").prepend($("<p>Iniciando Ejecucion</p>"));

    var hora = GetHora();
    var operaciones = GetOperaciones();
    var ciclo = selSemestre; //Variable definida en _Menu.js

    EjecutaHora(hora,operaciones,ciclo,excel,sheet)
}

function EjecutaHora(_hora, _operaciones, _ciclo, _excel, _hoja)
{
    var _url = $(".direccion #Ejecuta").text().trim();
    var datos = {
        hora: _hora,
        operaciones: _operaciones,
        ciclo: _ciclo,
        excel: _excel,
        hoja:_hoja
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
                EjecutaHora();
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