document.addEventListener("DOMContentLoaded", function () {

    let fields = {
        uniqueDesignation: document.getElementById('uniqueDesignation'),
        direction: document.getElementById('direction'),
        higherFormation: document.getElementById('higherFormation'),
        additionalInformation: document.getElementById('additionalInformation'),
        reinforcedReduced: document.getElementById('reinforcedReduced'),
        commonIdentifier: document.getElementById('commonIdentifier')
    };

    Object.getOwnPropertyNames(fields).forEach(name => {
        fields[name].addEventListener('change', function () {
            PmadMilsymbolSelector.getInstance('Symbol').updatePreview();
        });
    });

    let lastSidc = null;
    let lastParams = null;
    let lastSymbol = null;
    PmadMilsymbolSelector.setOptions("Symbol", {
        getSymbolOptions: function () {
            let options = {};
            Object.getOwnPropertyNames(fields).forEach(name => {
                let value = fields[name].value;
                if (value !== '') {
                    if (name == 'direction') {
                        value = String(Number(value) * 360 / 6400);
                    }
                    options[name] = value;
                }
            });
            return options;
        },
        symbolUpdatedCallback: function (sidc, optionsWithDegrees, symbol) {
            let optionsWithMils = Object.assign({}, optionsWithDegrees);
            if (optionsWithMils.direction) {
                optionsWithMils.direction = String(Math.round(Number(optionsWithMils.direction) * 6400 / 360));
            }
            let params = new URLSearchParams(optionsWithMils).toString();
            if (params.length > 0) {
                params = "?" + params;
            }
            if (lastSidc == null) {
                // Initial state
                history.replaceState({ sidc: sidc, options: optionsWithMils }, "", "/Symbols/" + sidc + params);
                console.log("replaceState", sidc);
            }
            else if (lastSidc != sidc) {
                // Value has changed
                history.pushState({ sidc: sidc, options: optionsWithMils }, "", "/Symbols/" + sidc + params);
                console.log("pushState", sidc);
            }
            else if (lastParams != params) {
                // replace or push ? (if push, we need to handle popstate)
                history.replaceState({ sidc: sidc, options: optionsWithMils }, "", "/Symbols/" + sidc + params);
                console.log("replaceState", sidc);
            }
            lastSymbol = symbol;
            lastSidc = sidc;
            lastParams = params;
        }
    });

    window.addEventListener("popstate", (event) => {
        if (event.state.sidc) {
            lastSidc = event.state.sidc;
            console.log("popState", lastSidc);
            PmadMilsymbolSelector.getInstance('Symbol').setValue(lastSidc);
        }
    });

    document.getElementById("download-svg").addEventListener("click", function () {
        let svg = lastSymbol.asSVG();
        let blob = new Blob([svg], { type: "image/svg+xml" });
        let url = URL.createObjectURL(blob);
        let a = document.createElement("a");
        a.href = url;
        a.download = lastSidc + ".svg";
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    });

    document.getElementById("download-png").addEventListener("click", function () {
        let canvas = lastSymbol.asCanvas(2);
        canvas.toBlob(function (blob) {
            let url = URL.createObjectURL(blob);
            let a = document.createElement("a");
            a.href = url;
            a.download = lastSidc + ".png";
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
        });
    });
});