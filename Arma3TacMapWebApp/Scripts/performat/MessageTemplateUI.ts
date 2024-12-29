interface MessageLineTemplate {
    title: string;
    description: string;
    fields: MessageFieldTemplate[];
}

interface MessageFieldTemplate {
    title: string;
    description: string;
    type: string;
}

interface MessageTemplateConfig {
    lines: MessageLineTemplate[];
}

class MessageTemplateUI {

    config: MessageTemplateConfig;
    composeText: HTMLTextAreaElement;
    composeFormFields: HTMLElement;
    lastValues: any;

    constructor(composeFormFields: HTMLElement, composeText: HTMLTextAreaElement, defaultValues?: any) {
        this.composeFormFields = composeFormFields;
        this.composeText = composeText;
        this.lastValues = defaultValues || {};
    }

    clear(): void {
        this.composeFormFields.innerHTML = '';
        this.composeText.readOnly = false;
        this.config = null;
    }

    getText(): string {
        if (!this.config) {
            return '';
        }
        var data = [];
        this.config.lines.forEach((line, lnum) => {
            var lineData = line.title ? line.title + ':' : '';
            line.fields.forEach((field, fnum) => {
                var id = 'l' + lnum + 'f' + fnum;
                var element = document.getElementById(id) as HTMLInputElement;

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

    updateComposeText(): void {
        if (this.config) {
            this.composeText.value = this.getText();
        }
    }

    setup(config: MessageTemplateConfig): void {
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
                        fieldsDiv.appendChild(this.generateCheckBox(id, generatePreformated, field));
                        break;
                    default:
                        fieldsDiv.appendChild(this.generateInput(id, generatePreformated, field, width));
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

    private generateCheckBox(id: string, generatePreformated: any, field: MessageFieldTemplate): HTMLElement {
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
        return inputGroup;
    }

    private generateInput(id: string, generatePreformated: any, field: MessageFieldTemplate, width: string): HTMLElement {
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
        input.id = id;
        input.placeholder = field.description;
        this.setupInputTypeSpecific(input, field);
        input.style.width = width;
        input.addEventListener('change', generatePreformated);
        input.addEventListener('keyup', generatePreformated);

        inputGroupPrepend.appendChild(inputGroupText);
        inputGroup.appendChild(inputGroupPrepend);
        inputGroup.appendChild(input);
        return inputGroup;
    }

    private setupInputTypeSpecific(input: HTMLInputElement, field: MessageFieldTemplate): void {

        switch (field.type) {
            case 'utm':
            case 'Grid':
                input.type = 'text';
                input.value = '';
                break;
            case 'callsign':
            case 'CallSign':
                input.type = 'text';
                input.value = this.lastValues['callsign'] || '';
                break;
            case 'frequency':
            case 'Frequency':
                input.type = 'number';
                input.step = '0.025';
                input.value = this.lastValues['frequency'] || '';
                break;
            case 'number':
            case 'Number':
                input.type = 'number';
                break;
            case 'DateTime':
                input.type = 'datetime-local';
                break;
            default:
                input.type = 'text';
                break;
        }
    }
}

function showPerformated(config: MessageTemplateConfig) {
    new MessageTemplateUI(document.getElementById('compose-form-fields'), document.getElementById('compose-text') as HTMLTextAreaElement).setup(config);
}
