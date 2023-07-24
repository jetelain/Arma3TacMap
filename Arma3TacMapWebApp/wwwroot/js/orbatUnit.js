function applySymbol() {
    applySymbolOne('3', '0', 'NatoSymbolFriendlyImageBase64', 'PreviewFriendly');
    applySymbolOne('6', '0', 'NatoSymbolHostileImageBase64', 'PreviewHostile');
    applySymbolOne('6', '1', 'NatoSymbolHostileAssumedImageBase64');
}

function applySymbolOne(c, e, hidden, area) {
    var symbol = '100' + c + '10' + e;
    symbol += $('#NatoSymbolHQ').val() || '0';
    symbol += $('#NatoSymbolSize').val() || '00';
    symbol += $('#NatoSymbolIcon').val() || '000000';
    symbol += $('#NatoSymbolMod1').val() || '00';
    symbol += $('#NatoSymbolMod2').val() || '00';
    var config = {
        uniqueDesignation: $('#UniqueDesignation').val(),
        size: 70
    };
    var sym = new ms.Symbol(symbol, config);
    var src = sym.asCanvas(window.devicePixelRatio).toDataURL();
    if (area) {
        $('#' + area).empty().append($('<img></img>')
            .attr({
                src: src,
                width: sym.getSize().width,
                height: sym.getSize().height
            })
        );
    }
    $('#' + hidden).val(src);
    return symbol;
}


$(function () {
    var id = '0003';
    var symbolset = '10';

    ms.setStandard("APP6"); // We always use APP6 edition D

    $('#NatoSymbolSize').empty();
    $.each(echelonMobilityTowedarray(symbolset), function (name, value) {
        var sym = new ms.Symbol(id + symbolset + '00' + value.code + '0000000000', { size: 16 });
        var labelHtml = '<img class="mil-icon" src="' + sym.asCanvas(window.devicePixelRatio).toDataURL() + '" width="' + sym.getSize().width + '" height="' + sym.getSize().height + '"> ' + value.name;
        $('#NatoSymbolSize').append($('<option></option>').attr({ value: value.code, 'data-content': labelHtml }).text(value.name));
    });

    var data = milstd.app6d[symbolset];

    $('#NatoSymbolIcon').empty();
    $.each(data['main icon'], function (name, value) {
        var sym = new ms.Symbol(id + symbolset + '0000' + value.code + '0000', { size: 16 });
        var labelHtml = '<img class="mil-icon" src="' + sym.asCanvas(window.devicePixelRatio).toDataURL() + '" width="' + sym.getSize().width + '" height="' + sym.getSize().height + '"> ';
        var labelText = '';
        if (value['entity type']) {
            if (value['entity subtype']) {
                labelHtml += '<span class="font-weight-light text-muted">' + value.entity + ' - ' + value['entity type'] + '</span> - <strong>' + value['entity subtype'] + '</strong>';
                labelText = value.entity + ' - ' + value['entity type'] + ' - ' + value['entity subtype'];
            } else {
                labelHtml += '<span class="font-weight-light text-muted">' + value.entity + '</span> - <strong>' + value['entity type'] + '</strong>';
                labelText = value.entity + ' - ' + value['entity subtype'];
            }
        }
        else {
            labelHtml += '<strong>' + value.entity + '</strong>';
            labelText = value.entity;

            $('#NatoSymbolIcon').append($('<option></option>').attr({ 'data-divider': 'true' }));
        }
        $('#NatoSymbolIcon').append($('<option></option>').attr({ value: value.code, 'data-content': labelHtml }).text(labelText));
    });

    $('#NatoSymbolMod1').empty();
    $.each(data['modifier 1'], function (name, value) {
        var sym = new ms.Symbol(id + symbolset + '0000000000' + value.code + '00', { size: 16 });
        var labelHtml = '<img class="mil-icon" src="' + sym.asCanvas(window.devicePixelRatio).toDataURL() + '" width="' + sym.getSize().width + '" height="' + sym.getSize().height + '"> ' + value.modifier;
        $('#NatoSymbolMod1').append($('<option></option>').attr({ value: value.code, 'data-content': labelHtml }).text(value.modifier));
    });

    $('#NatoSymbolMod2').empty();
    $.each(data['modifier 2'], function (name, value) {
        var sym = new ms.Symbol(id + symbolset + '000000000000' + value.code, { size: 16 });
        var labelHtml = '<img class="mil-icon" src="' + sym.asCanvas(window.devicePixelRatio).toDataURL() + '" width="' + sym.getSize().width + '" height="' + sym.getSize().height + '"> ' + value.modifier;
        $('#NatoSymbolMod2').append($('<option></option>').attr({ value: value.code, 'data-content': labelHtml }).text(value.modifier));
    });

    // data-selected
    $('#NatoSymbolSize').val($('#NatoSymbolSize').attr('data-selected'));
    $('#NatoSymbolIcon').val($('#NatoSymbolIcon').attr('data-selected'));
    $('#NatoSymbolMod1').val($('#NatoSymbolMod1').attr('data-selected'));
    $('#NatoSymbolMod2').val($('#NatoSymbolMod2').attr('data-selected'));
    $('#NatoSymbolHQ').val($('#NatoSymbolHQ').attr('data-selected'));

    $('#NatoSymbolSize').selectpicker();
    $('#NatoSymbolIcon').selectpicker();
    $('#NatoSymbolMod1').selectpicker();
    $('#NatoSymbolMod2').selectpicker();
    $('#NatoSymbolHQ').selectpicker();

    $('#NatoSymbolSize').on('change',applySymbol);
    $('#NatoSymbolIcon').on('change',applySymbol);
    $('#NatoSymbolMod1').on('change',applySymbol);
    $('#NatoSymbolMod2').on('change',applySymbol);
    $('#NatoSymbolHQ').on('change', applySymbol);
    $('#UniqueDesignation').on('change', applySymbol);
    applySymbol();
});