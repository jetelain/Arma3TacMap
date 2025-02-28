document.addEventListener("DOMContentLoaded", function () {

    let fields = {
        uniqueDesignation: document.getElementById('UniqueDesignation')
    };

    Object.getOwnPropertyNames(fields).forEach(name => {
        fields[name].addEventListener('change', function () {
            PmadMilsymbolSelector.getInstance('FriendSidc').updatePreview();
        });
    });

    PmadMilsymbolSelector.setOptions("FriendSidc", {
        getSymbolOptions: function () {
            let options = {};
            Object.getOwnPropertyNames(fields).forEach(name => {
                let value = fields[name].value;
                if (value !== '') {
                    options[name] = value;
                }
            });
            return options;
        },
        symbolUpdatedCallback: function (sidc, optionsWithDegrees, symbol) {
            if (sidc.substring(3, 4) != "3" || sidc.substring(6, 7) != "0") {
                // Force std id to "3" and status to "0" as required by backend
                PmadMilsymbolSelector.getInstance('FriendSidc').setValue(
                    sidc.substring(0, 3) + "3" + sidc.substring(4, 6) + "0" + sidc.substring(7)
                );
            }
        }
    });

});