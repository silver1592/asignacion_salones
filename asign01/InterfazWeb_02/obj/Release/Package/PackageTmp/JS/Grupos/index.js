﻿$().ready(function () {
    $("#buscar_grupos").click(Busqueda);

    inicializaMenu();
    ActualizaOrigen();

    $('#lista table').tablesorter();
});