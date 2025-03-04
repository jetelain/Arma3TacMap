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

interface FieldValueHeper {
    getCurrentLocation?(): string;
    getCurrentFrequency?(): string;
    getCurrentDatetime?(): string;
    getCallSign?(): string;

    promptLocation?(currentValue: string): Promise<string>;
    promptFrequency?(currentValue: string): Promise<string>;
}
interface MessageData {
    text: string;
    attachments: MessageAttachment[];
}

interface MessageAttachment {
    type: string;
    label: string;
    markerPosition?: number[];
    position?: number[];
    positionPrecision?: number[];
}

class MessageTemplateUI {

    config: MessageTemplateConfig;
    composeText: HTMLTextAreaElement;
    composeFormFields: HTMLElement;
    lastValues: any;
    helper?: FieldValueHeper;
    bootstrapVersion: number = 4;

    constructor(composeFormFields: HTMLElement, composeText: HTMLTextAreaElement, helper?: FieldValueHeper, defaultValues?: any) {
        this.composeFormFields = composeFormFields;
        this.composeText = composeText;
        this.lastValues = defaultValues || {};
        this.helper = helper;
    }

    clear(): void {
        this.composeFormFields.innerHTML = '';
        this.composeText.readOnly = false;
        this.config = null;
    }

    getText(): string {
        return this.getData().text;
    }

    getAttachments(): MessageAttachment[] {
        return this.getData().attachments;
    }

    getData(): MessageData {
        if (!this.config) {
            return { text : '', attachments: [] };
        }
        let attachements: MessageAttachment[] = [];
        let data = [];
        this.config.lines.forEach((line, lnum) => {
            let lineData = line.title ? line.title + ':' : '';
            line.fields.forEach((field, fnum) => {
                let id = 'l' + lnum + 'f' + fnum;
                let element = document.getElementById(id) as HTMLInputElement;

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
                            case 'Grid':
                            case 'GridNoMarker':
                                var attachement = this.createGridAttachment(value, field.type == 'Grid');
                                if (attachement) {
                                    attachements.push(attachement);
                                }
                                break;
                        }
                        break;
                }
            });
            data.push(lineData);
        });
        return { text: data.join('\n'), attachments: attachements };
    }

    parsePosition(value: string): { position: number[], precision:number[] } {
        let test = /^[ ]*([0-9]{5})[ ]*-[ ]*([0-9]{5})[ ]*$/.exec(value);
        if (test) {
            return { position: [parseInt(test[1]), parseInt(test[2])], precision: [1, 1] };
        }
        test = /^[ ]*([0-9]{4})[ ]*-[ ]*([0-9]{4})[ ]*$/.exec(value);
        if (test) {
            return { position: [parseInt(test[1]), parseInt(test[2])], precision: [10, 10] };
        }
        test = /^[ ]*([0-9]{3})[ ]*-[ ]*([0-9]{3})[ ]*$/.exec(value);
        if (test) {
            return { position: [parseInt(test[1]), parseInt(test[2])], precision: [100, 100] };
        }
        return null;
    }

    createGridAttachment(value: string, isMarker: boolean): MessageAttachment {
        let parsed = this.parsePosition(value);
        if (!parsed) {
            return null;
        }
        return {
            type: isMarker ? 'Marker' : 'Grid',
            label: value,
            markerPosition: [parsed.position[0] + (parsed.precision[0] / 2), parsed.position[1] + (parsed.precision[1] / 2)],
            position: parsed.position,
            positionPrecision: parsed.precision
        };
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

        this.generateHelperButtons(field, inputGroup, input);

        return inputGroup;
    }

    generateHelperButtons(field: MessageFieldTemplate, inputGroup: HTMLDivElement, input: HTMLInputElement) {
        if (this.helper) {
            let inputGroupAppend = inputGroup;
            if (this.bootstrapVersion < 5) {
                inputGroupAppend =  document.createElement('div');
                inputGroupAppend.className = 'input-group-append';
            }
            switch (field.type) {
                case 'utm':
                case 'Grid':
                case 'GridNoMarker':
                    if (this.helper.getCurrentLocation) {
                        var locationButton = document.createElement('button');
                        locationButton.className = 'btn btn-outline-secondary';
                        locationButton.textContent = 'Here';
                        locationButton.addEventListener('click', () => {
                            input.value = this.helper.getCurrentLocation();
                            input.dispatchEvent(new Event('change'));
                        });
                        inputGroupAppend.appendChild(locationButton);
                    }
                    if (this.helper.promptLocation) {
                        var locationButton = document.createElement('button');
                        locationButton.className = 'btn btn-outline-secondary';
                        locationButton.textContent = 'Find';
                        locationButton.addEventListener('click', async () => {
                            var currentValue = input.value;
                            var newValue = await this.helper.promptLocation(currentValue);
                            if (newValue) {
                                input.value = newValue;
                                input.dispatchEvent(new Event('change'));
                            }
                        });
                        inputGroupAppend.appendChild(locationButton);
                    }
                    break;
                case 'frequency':
                case 'Frequency':
                    if (this.helper.getCurrentFrequency) {
                        var frequencyButton = document.createElement('button');
                        frequencyButton.className = 'btn btn-outline-secondary';
                        frequencyButton.textContent = 'Current';
                        frequencyButton.addEventListener('click', () => {
                            input.value = this.helper.getCurrentFrequency();
                            input.dispatchEvent(new Event('change'));
                        });
                        inputGroupAppend.appendChild(frequencyButton);
                    }
                    if (this.helper.promptFrequency) {
                        var frequencyButton = document.createElement('button');
                        frequencyButton.className = 'btn btn-outline-secondary';
                        frequencyButton.textContent = 'List';
                        frequencyButton.addEventListener('click', async () => {
                            var currentValue = input.value;
                            var newValue = await this.helper.promptFrequency(currentValue);
                            if (newValue) {
                                input.value = newValue;
                                input.dispatchEvent(new Event('change'));
                            }
                        });
                        inputGroupAppend.appendChild(frequencyButton);
                    }
                    break;
                case 'DateTime':
                    if (this.helper.getCurrentDatetime) {
                        var datetimeButton = document.createElement('button');
                        datetimeButton.className = 'btn btn-outline-secondary';
                        datetimeButton.textContent = 'Now';
                        datetimeButton.addEventListener('click', () => {
                            input.value = this.helper.getCurrentDatetime();
                            input.dispatchEvent(new Event('change'));
                        });
                        inputGroupAppend.appendChild(datetimeButton);
                    }
                    break;
            }
            if (this.bootstrapVersion < 5 && inputGroupAppend.children.length > 0) {
                inputGroup.appendChild(inputGroupAppend);
            }
        }
    }

    private setupInputTypeSpecific(input: HTMLInputElement, field: MessageFieldTemplate): void {
        switch (field.type) {
            case 'utm':
            case 'Grid':
            case 'GridNoMarker':
                input.type = 'text';
                input.value = (this.helper && this.helper.getCurrentLocation) ? this.helper.getCurrentLocation() : '';
                break;
            case 'callsign':
            case 'CallSign':
                input.type = 'text';
                input.value = this.lastValues['callsign'] || ((this.helper && this.helper.getCallSign) ? this.helper.getCallSign() : '');
                break;
            case 'frequency':
            case 'Frequency':
                input.type = 'number';
                input.step = '0.025';
                input.value = this.lastValues['frequency'] || ((this.helper && this.helper.getCurrentFrequency) ? this.helper.getCurrentFrequency() : '');
                break;
            case 'number':
            case 'Number':
                input.type = 'number';
                break;
            case 'DateTime':
                input.type = 'datetime-local';
                input.value = (this.helper && this.helper.getCurrentDatetime) ? this.helper.getCurrentDatetime() : '';
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
