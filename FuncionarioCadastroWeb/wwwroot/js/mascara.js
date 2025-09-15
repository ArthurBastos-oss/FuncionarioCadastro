$(function () {
    // Aplica máscara no CPF
    if ($('#CPF').length) {
        $('#CPF').mask('000.000.000-00');
    }

    // Aplica máscara no CEP
    if ($('#CEP').length) {
        $('#CEP').mask('00000-000');
    }

    // Mascara dinâmica para CTPS
    $('#CTPS-Tipo').on('change', function () {
        let tipo = $(this).val();
        $('#CTPS').unmask(); // remove máscara antiga

        if (tipo === "CPF") {
            $('#CTPS').mask('000.000.000-00');
        } else if (tipo === "CTPS") {
            $('#CTPS').mask('0000000/0000');
        }
    });

    // Se o campo CTPS já tiver valor ao carregar a página
    let tipoInicial = $('#CTPS-Tipo').val();
    if (tipoInicial) {
        $('#CTPS-Tipo').trigger('change');
    }
});
