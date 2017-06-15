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
    $("#importacionTabs").tabs({ collapsible: true });
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

function ErrorFunction(jqXHR, exception) {
    Continue();
    
    alert(ErrorToString(jqXHR));
}

function ErrorToString(jqXHR)
{
    var msg = '';
    if (jqXHR.status === 0) {
        msg = 'Not connect.\n Verify Network.';
    } else if (jqXHR.status == 404) {
        msg = 'Requested page not found. [404]';
    } else if (jqXHR.status == 500) {
        msg = 'Internal Server Error [500].';
    } else if (exception === 'parsererror') {
        msg = 'Requested JSON parse failed.';
    } else if (exception === 'timeout') {
        msg = 'Time out error.';
    } else if (exception === 'abort') {
        msg = 'Ajax request aborted.';
    } else {
        msg = 'Uncaught Error.\n' + jqXHR.responseText;
    }

    return msg;
}