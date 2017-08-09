function ActualizaGrupos(cve_full, salon) {
    var url = $(".direccion #ActualizaGrupo").text().trim();
    var dt = JSON.stringify({ cve_full: cve_full, salon: salon});

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: dt,
        success: function (resultado) {
            if (!resultado['res'])
                alert("No se pudo realizar el cambio");
            else
                alert("Cambio realizado")
        },
        beforeSend: Wait,
        complete: Continue,
        error: function (jqXHR, exception) {
            alert("No se pudo realizar el cambio");
        }
    });
}