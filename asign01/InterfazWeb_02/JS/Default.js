Date.prototype.yyyymmdd = function () {
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();

    return [this.getFullYear(),
            (mm > 9 ? '' : '0') + mm,
            (dd > 9 ? '' : '0') + dd
    ].join('');
};

function Wait() {
    $('#div_carga').show();
}

function Continue() {
    $('#div_carga').hide();
}