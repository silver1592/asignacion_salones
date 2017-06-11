$().ready(function () {
    //UX
    initialize();

    //Eventos
    $("#archivos").change(changeExcel);
    $("#SeleccionExcel #setSession").click(seleccionaOrigenDatos);
    $("#SeleccionExcel #uploadExcel").click(uploadExcelToDB);
});

function initialize()
{
    $("#importacionTabs").tabs({ collapsible: true });
    $("#tabs").tabs({ collapsible: true });
}

function changeExcel() {
    var excelName = $("#archivos option:selected").text();

    $("#hojas option").remove();
    if (excelName != "---------") {
        $.ajax({
            type: "POST",
            url: '/Consultas/PaginasExcel',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ file: excelName }),
            dataType: "json",
            success: function (resultado) {
                $.each(resultado, function (index, item) {
                    $("#hojas").append($("<option>" + item + "</option>"))
                });
            },
            error: ErrorFunction
        });
    }
}

function seleccionaOrigenDatos()
{
    var excelName = $("#SeleccionExcel #archivos option:selected").text();
    var sheet = $("#SeleccionExcel #hojas option:selected").text();
    var operation = $("#SeleccionExcel input:checked").val();
    var semestre = $("#SeleccionExcel input[name='semestre']").val();

    if(operation == "e")
        var data = { excel: excelName, sheet: sheet, ciclo:semestre , bd: false }
    else
        var data = { excel: excelName, sheet: sheet, ciclo:semestre, bd: true }

    data = JSON.stringify(data);
    $.ajax({
        type: "POST",
        url: '/Importacion/SetSession_OrigenDatos',
        contentType: "application/json; charset=utf-8",
        data: data,
        dataType: "json",
        success: function (resultado) {
            validaConexion();
            alert("Operacion realizada");
        },
        error: ErrorFunction
    });
}

function uploadExcelToDB()
{
    data = JSON.stringify(data);
    $.ajax({
        type: "POST",
        url: '/Importacion/CargaExcel_BD',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultado) {
            if (resultado[0] != true) alert(resultado[1]);
        },
        error: ErrorFunction
    });
}

function ErrorFunction (jqXHR, exception) {
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
    alert(msg);
}