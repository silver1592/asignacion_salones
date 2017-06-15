var siguiente = true;

function Ejecuta() {
    var ini = parseInt($("[name='hora_ini']").val());
    var fin = parseInt($("[name='hora_fin']").val());

    var bEmpalmes = $("[name='emp']").is(":checked");

    var bPreasignacion = $("[name='pre']").is(":checked");
    var bOtrosSemestres = $("[name='pre_otros_sem']").is(":checked");

    var bAlgoritmo = $("[name='asig']").is(":checked");
    var iIndividuos = parseInt($("[name='alg_individuos']").val());
    var iGeneraciones = parseInt($("[name='alg_generaciones']").val());

    if (!bEmpalmes && !bPreasignacion && !bOtrosSemestres && !bAlgoritmo)
        return 0;

    var datos;

    for (var i = ini; i <= fin; i++) {
        datos = {
            hora : i,
            empalmes: bEmpalmes,
            preasignacion: bPreasignacion,
            otrosSemestres: bOtrosSemestres,
            algoritmo: bAlgoritmo,
            individuo: iIndividuos,
            generacion : iGeneraciones
        }

        //TODO: hacer que lance un evento al terminar que haga que se ejecute el que sigue
        //while (!siguiente);
        //siguiente = false;

        datos = JSON.stringify(datos);

        $.ajax({
            type: "POST",
            url: '/Ejecuta/EjecutaOperaciones',
            contentType: "application/json; charset=utf-8",
            data: datos,
            dataType: "json",
            success: function (resultado) {
                $("#resConsola").append("<p>" + resultado + "</p>");
                siguiente = true;
            },
            error: function (jqXHR, exception)
            {
                $("#resConsola").append("<p><strong>" + exception + "-" + ErrorToString(jqXHR) + "<strong></p>");
                siguiente = true;
            }
        });
    }
}