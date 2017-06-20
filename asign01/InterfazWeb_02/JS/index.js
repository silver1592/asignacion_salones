$().ready(function () {
    //UX
    initialize();

    //Eventos
    $("#archivos").change(changeExcel);
    $("#SeleccionExcel #setSession").click(seleccionaOrigenDatos);
    $("#SeleccionExcel #uploadExcel").click(uploadExcelToDB);
    $("#EjecutaOperaciones").click(Ejecuta);

    ActualizaOrigen();
});

function initialize()
{
    $("#optionsTabs").tabs({ collapsible: true });
    $("#tabs").tabs({ collapsible: true });
    
}

function Wait()
{
    //$('html,body').css('cursor', 'wait');
}

function Continue()
{
    //$('html,body').css('cursor', 'default');
}