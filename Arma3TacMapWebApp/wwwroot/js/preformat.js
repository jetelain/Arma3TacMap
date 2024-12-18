var currentPreformated = { id: null };
var preformatedConfig = [];
var lastValues = {};
function closePerformated() {
    $('#compose-form-fields').empty();
    $('#compose-text').prop('readonly', false);
    currentPreformated = { id: null };
}
function generatePreformated() {
    if (currentPreformated.config) {
        var data = [];
        currentPreformated.config.lines.forEach((line, lnum) => {
            var lineData = line.title ? line.title + ':' : '';
            line.fields.forEach((field, fnum) => {
                var id = 'l' + lnum + 'f' + fnum;

                switch (field.type) {
                    case 'checkbox':
                    case 'CheckBox':
                        if ($('#' + id).is(':checked')) {
                            lineData = lineData + ' ' + field.title;
                            $('#' + id + '-box').addClass('bg-primary text-white');
                        }
                        else {
                            $('#' + id + '-box').removeClass('bg-primary text-white');
                        }
                        break;
                    default:
                        var value = ('' + $('#' + id).val()).trim();
                        if (value && value.length > 0) {
                            lineData = lineData + ' ' + (field.title || '') + value;
                            $('#' + id + '-box').addClass('bg-primary text-white');
                        }
                        else {
                            $('#' + id + '-box').removeClass('bg-primary text-white');
                        }
                        if (field.type == 'callsign' || field.type == 'frequency') {
                            lastValues[field.type.toLocaleLowerCase()] = value;
                        }
                        break;
                }
            });
            data.push(lineData);
        });
        $('#compose-text').val(data.join('\n'));
    }
}

function showPerformated(config) {
    $('#compose-text').prop('readonly', true);
    currentPreformated = { config: config };
    if (currentPreformated.config) {
        currentPreformated.config.lines.forEach((line, lnum) => {
            var fieldsDiv = $('<div class="form-inline" />');
            line.fields.forEach((field, fnum) => {
                var id = 'l' + lnum + 'f' + fnum;
                var width = '7em';
                if (line.fields.length == 1) {
                    width = '15em';
                }
                switch (field.type) {
                    case 'checkbox':
                    case 'CheckBox':
                        fieldsDiv.append($('<div class="input-group input-group-sm mb-2 mr-sm-2">')
                            .append($('<div class="input-group-prepend">').append($('<div class="input-group-text">').attr({ id: id + '-box' })
                                .append($('<input type="checkbox" />').attr({ id: id })).on('click', generatePreformated)
                                .append($('<label class="form-check-label ml-1" />').attr({ for: id }).text(field.title || ''))
                            ))
                            .append($('<label class="form-control bg-light" />').attr({ for: id }).text(field.description || '')));
                        break;
                    default:
                        var attr = { id: id, placeholder: field.description, type: 'text' };
                        switch (field.type) {
                            case 'utm':
                            case 'Grid':
                                attr.value = $('#position').text().trim();
                                break;
                            case 'callsign':
                            case 'CallSign':
                                attr.value = lastValues['callsign'] || '';
                                break;
                            case 'frequency':
                            case 'Frequency':
                                attr.type = 'number';
                                attr.step = '0.025';
                                attr.value = lastValues['frequency'] || '45.000';
                                break;
                            case 'number':
                            case 'Number':
                                attr.type = 'number';
                                break;
                            case 'DateTime':
                                attr.type = 'datetime-local';
                                break;
                        }
                        fieldsDiv.append($('<div class="input-group input-group-sm mb-2 mr-sm-2">')
                            .append($('<div class="input-group-prepend">').append($('<label class="input-group-text">').attr({ for: id, id: id + '-box' }).text(field.title || '')))
                            .append($('<input type="text" class="form-control" />').attr(attr).css({ width: width })
                                .on('change', generatePreformated)
                                .on('keyup', generatePreformated)));
                        break;
                }
            });
            $('#compose-form-fields')
                .append($('<div class="col" />').text(line.title ? line.title + ': ' + (line.description || '') : (line.description || ''))
                    .append(fieldsDiv));
        });
        generatePreformated();
    } else {
        var content = $('<div class="mb-2" />');
        preformatedConfig.forEach(config => {
            content.append($('<a class="btn btn-sm btn-primary mr-2"></a>').text(config.title).on('click', function () {
                showPerformated(config.id);
            }));
        });
        $('#compose-form-fields').append(content);
    }
}
