//#SeleccionExcel input[name='file'] ----> Boton para subir archivo

$().ready(function () {
    //UX
    initialize();
    inicializaMenu();

    //Comportamiento del boton para subir archivo
    $("#SeleccionExcel input[name='archivo']").change(function () {
        $(this).parent().submit();
    })

    //Comportamiento de la seleccion del archivo
    $("#SeleccionExcel #archivos").change(function () {
        /* Bloque donde obtiene la informacion de las hojas */
        var excelName = $("#archivos option:selected").text();
        var _url = $(".direccion #changeExcel").text().trim();

        $("#hojas option").remove();
        if (excelName != "---------") {
            $.ajax({
                type: "POST",
                url: _url,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ file: excelName }),
                dataType: "json",
                success: function (resultado) {
                    $.each(resultado, function (index, item) {
                        $("#hojas").append($("<option>" + item + "</option>"))
                    });
                },
                error: ErrorFunction,
                beforeSend: Wait,
                complete: Continue,
            });
        }

        /* Boque donde asigna la url a los botones*/
        var urlDownloadButton = window.location.href + "Archivos/" + excelName;
        $("#SeleccionExcel #descarga").attr("href", urlDownloadButton);
    });

    $("#EliminaExcel").click(function () {
        //TODO:Cambiar el direccionamiento y que lea el archivo
        /*
        var _url = $(".direccion #elimina").text().trim();

        $.ajax({
            type: "GET",
            url: _url,
            contentType: "application/json; charset=utf-8",
            success: function (resultado) {
                $("#resConsola").prepend("<p>" + resultado + "</p>");
            },
            error: function (jqXHR, exception) {
                $("#resConsola").append("<p><strong>" + exception + "-" + ErrorToString(jqXHR, exception) + "<strong></p>");
            },
            beforeSend: Wait,
            complete: Continue,
        });*/
    });

    //Comportamiento del boton para cargar la base de datos
    $("#SeleccionExcel #uploadExcel").click(function () {
        var _url = $(".direccion #uploadExcelToDB").text().trim();

        var excelName = $("#SeleccionExcel #archivos option:selected").text();
        var sheet = $("#SeleccionExcel #hojas option:selected").text();
        var operation = $("#SeleccionExcel input:checked").val();
        var semestre = $("#SeleccionExcel input[name='semestre']").val();

        var data = { excel: excelName, sheet: sheet, ciclo: semestre }

        data = JSON.stringify(data);

        $.ajax({
            type: "POST",
            url: _url,
            contentType: "application/json; charset=utf-8",
            data: data,
            dataType: "json",
            beforeSend: Wait,
            complete: Continue,
            success: function (resultado) {
                if (resultado[0] != true) alert(resultado[1]);
            },
            error: ErrorFunction
        });
    });

    $("#EjecutaOperaciones").click(Ejecuta);

    $("#Exporta").click(function () {
        var d = new Date();
        var _excel = d.yyyymmdd() + ".xlsx"
        var _sheet = "Exportacion_"+d.getHours().toString() + "_" + d.getMinutes().toString();

        var datos = {excel: _excel,sheet:_sheet};
        var dt = JSON.stringify(datos);
        var _url = $(".direccion #Exporta").text().trim();

        $.ajax({
            type: "POST",
            url: _url,
            contentType: "application/json; charset=utf-8",
            data: dt,
            dataType: "json",
            success: function (resultado) {
                $("#resConsola").prepend("<p>" + resultado + "</p>"+"<spawn>Guardado en "+excel+"->"+sheet+"</spawn>");
            },
            error: function (jqXHR, exception) {
                $("#resConsola").append("<p><strong>" + exception + "-" + ErrorToString(jqXHR, exception) + "<strong></p>");
            },
            beforeSend: Wait,
            complete: Continue,
        });
    });

    ActualizaOrigen();

    $('#div_carga').hide();
    $("#SeleccionExcel input[name='archivo']").hide();
});

function initialize()
{
    $("#optionsTabs").tabs({ collapsible: true });
    $("#tabs").tabs({ collapsible: true });
}

function GetDatosExporta() {

    var d = new Date();

    excel = d.yyyymmdd() + ".xlsx"
    sheet = d.getHours().toString() + "_" + d.getMinutes().toString();

    datos = {
        excel: excel,
        hoja: sheet,
    }

    return datos;
}