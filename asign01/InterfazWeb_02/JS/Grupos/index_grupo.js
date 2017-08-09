$().ready(function () {
    $("#salon").change(function () {
        $(this).parent().submit();
    });
    $("#grupo").hide();
});