$().ready(function () {
    //UX
    initialize();
    inicializaMenu();



    //Eventos
    $("#archivos").change(changeExcel);
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