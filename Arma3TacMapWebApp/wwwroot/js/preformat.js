
class MessageTemplateUI {

    constructor(composeFormFields, composeText, defaultValues) {
        this.composeFormFields = composeFormFields;
        this.composeText = composeText;
        this.lastValues = defaultValues || {};
    }

    clear() {
        this.composeFormFields.innerHTML = '';
        this.composeText.readOnly = false;
        this.config = null;
    }

    getText() {
        if (!this.config) {
            return '';
        }
        var data = [];
        this.config.lines.forEach((line, lnum) => {
            var lineData = line.title ? line.title + ':' : '';
            line.fields.forEach((field, fnum) => {
                var id = 'l' + lnum + 'f' + fnum;
                var element = document.getElementById(id);

                switch (field.type) {
                    case 'checkbox':
                    case 'CheckBox':
                        if (element.checked) {
                            lineData = lineData + ' ' + field.title;
                            document.getElementById(id + '-box').classList.add('bg-primary', 'text-white');
                        } else {
                            document.getElementById(id + '-box').classList.remove('bg-primary', 'text-white');
                        }
                        break;
                    default:
                        var value = ('' + element.value).trim();
                        if (value && value.length > 0) {
                            lineData = lineData + ' ' + (field.title || '') + value;
                            document.getElementById(id + '-box').classList.add('bg-primary', 'text-white');
                        } else {
                            document.getElementById(id + '-box').classList.remove('bg-primary', 'text-white');
                        }
                        switch (field.type) {
                            case 'callsign':
                            case 'CallSign':
                            case 'frequency':
                            case 'Frequency':
                                this.lastValues[field.type.toLocaleLowerCase()] = value;
                                break;
                        }
                        break;
                }
            });
            data.push(lineData);
        });
        return data.join('\n');
    }

    updateComposeText() {
        if (this.config) {
            this.composeText.value = this.getText();
        }
    }

    setup(config) {
        this.config = config;
        this.composeText.readOnly = true;
        this.composeFormFields.innerHTML = '';

        const generatePreformated = this.updateComposeText.bind(this);

        config.lines.forEach((line, lnum) => {
            const fieldsDiv = document.createElement('div');
            fieldsDiv.className = 'form-inline';
            line.fields.forEach((field, fnum) => {
                const id = 'l' + lnum + 'f' + fnum;
                var width = '7em';
                if (line.fields.length == 1) {
                    width = '15em';
                }
                switch (field.type) {
                    case 'checkbox':
                    case 'CheckBox':
                        var inputGroup = document.createElement('div');
                        inputGroup.className = 'input-group input-group-sm mb-2 mr-sm-2';

                        var inputGroupPrepend = document.createElement('div');
                        inputGroupPrepend.className = 'input-group-prepend';

                        var inputGroupText = document.createElement('div');
                        inputGroupText.className = 'input-group-text';
                        inputGroupText.id = id + '-box';

                        var checkbox = document.createElement('input');
                        checkbox.type = 'checkbox';
                        checkbox.id = id;
                        checkbox.addEventListener('click', generatePreformated);

                        var label = document.createElement('label');
                        label.className = 'form-check-label ml-1';
                        label.htmlFor = id;
                        label.textContent = field.title || '';

                        inputGroupText.appendChild(checkbox);
                        inputGroupText.appendChild(label);
                        inputGroupPrepend.appendChild(inputGroupText);

                        var descriptionLabel = document.createElement('label');
                        descriptionLabel.className = 'form-control bg-light';
                        descriptionLabel.htmlFor = id;
                        descriptionLabel.textContent = field.description || '';

                        inputGroup.appendChild(inputGroupPrepend);
                        inputGroup.appendChild(descriptionLabel);
                        fieldsDiv.appendChild(inputGroup);
                        break;
                    default:
                        var attr = { id: id, placeholder: field.description, type: 'text' };
                        switch (field.type) {
                            case 'utm':
                            case 'Grid':
                                attr.value = '';
                                break;
                            case 'callsign':
                            case 'CallSign':
                                attr.value = this.lastValues['callsign'] || '';
                                break;
                            case 'frequency':
                            case 'Frequency':
                                attr.type = 'number';
                                attr.step = '0.025';
                                attr.value = this.lastValues['frequency'] || '';
                                break;
                            case 'number':
                            case 'Number':
                                attr.type = 'number';
                                break;
                            case 'DateTime':
                                attr.type = 'datetime-local';
                                break;
                        }
                        var inputGroup = document.createElement('div');
                        inputGroup.className = 'input-group input-group-sm mb-2 mr-sm-2';

                        var inputGroupPrepend = document.createElement('div');
                        inputGroupPrepend.className = 'input-group-prepend';

                        var inputGroupText = document.createElement('label');
                        inputGroupText.className = 'input-group-text';
                        inputGroupText.htmlFor = id;
                        inputGroupText.id = id + '-box';
                        inputGroupText.textContent = field.title || '';

                        var input = document.createElement('input');
                        input.className = 'form-control';
                        Object.assign(input, attr);
                        input.style.width = width;
                        input.addEventListener('change', generatePreformated);
                        input.addEventListener('keyup', generatePreformated);

                        inputGroupPrepend.appendChild(inputGroupText);
                        inputGroup.appendChild(inputGroupPrepend);
                        inputGroup.appendChild(input);
                        fieldsDiv.appendChild(inputGroup);
                        break;
                }
            });
            var colDiv = document.createElement('div');
            colDiv.className = 'col';
            colDiv.textContent = line.title ? line.title + ': ' + (line.description || '') : (line.description || '');
            colDiv.appendChild(fieldsDiv);
            this.composeFormFields.appendChild(colDiv);
        });

        this.updateComposeText();
    }
}

function showPerformated(config) {
    new MessageTemplateUI(document.getElementById('compose-form-fields'), document.getElementById('compose-text')).setup(config);
}
