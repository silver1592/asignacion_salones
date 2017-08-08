//#SeleccionExcel input[name='file'] ----> Boton para subir archivo

$().ready(function () {
    //UX
    initialize();
    inicializaMenu();

    //Comportamiento del boton para subir archivo
    $("#SeleccionExcel input[name='file']").change(function () {
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
        var urlDownloadButton = window.location.href + "/Archivos/" + excelName;
        $("#SeleccionExcel #descarga").attr("href", urlDownloadButton);
    });

    //Comportamiento del boton para cargar la base de datos
    $("#SeleccionExcel #uploadExcel").click(uploadExcelToDB);

    $("#EjecutaOperaciones").click(Ejecuta);

    ActualizaOrigen();

    $('#div_carga').hide();
});

function initialize()
{
    $("#optionsTabs").tabs({ collapsible: true });
    $("#tabs").tabs({ collapsible: true });
}