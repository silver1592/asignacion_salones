$().ready(function () {
    inicializaMenu();
    ActualizaOrigen();

    $("#salon").change(function () {
        var salon = $("#salon option:selected").text().trim();

        var url = $(".direccion #buscaSalon").text().trim();
        var dt = JSON.stringify({ cve_salon: salon});

        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json; charset=utf-8",
            data: dt,
            success: function (resultado) {
                $("#salonList").empty();
                $("#salonList").append(resultado);
            },
            beforeSend: Wait,
            complete: Continue,
            error: function (jqXHR, exception) {
                alert(exception);
            }
        });
    })

    $("#empalmes").click(function () {
        var url = $(".direccion #buscaSalon").text().trim();
        var dt = JSON.stringify({ cve_salon: "empalmes" });

        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json; charset=utf-8",
            data: dt,
            success: function (resultado) {
                $("#salonList").empty();
                $("#salonList").append(resultado);
            },
            beforeSend: Wait,
            complete: Continue,
            error: function (jqXHR, exception) {
                alert(exception);
            }
        });
    });
});