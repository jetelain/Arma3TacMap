function applySymbolSet() {
    var id = '0003';
    var symbolset = $('#set').val();

    $('#size').empty();
    $.each(echelonMobilityTowedarray(symbolset), function (name, value) {
        var sym = new ms.Symbol(id + symbolset + '00' + value.code + '0000000000', { size: 16 });
        var labelHtml = '<img class="mil-icon" src="' + sym.asCanvas(window.devicePixelRatio).toDataURL() + '" width="' + sym.getSize().width + '" height="' + sym.getSize().height + '"> ' + value.name;
        $('#size').append($('<option></option>').attr({ value: value.code, 'data-content': labelHtml }).text(value.name));
    });

    var data = milstd.app6d[symbolset];

    var grps = {};
    $('#icon').empty();
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

            $('#icon').append($('<option></option>').attr({ 'data-divider': 'true' }));
        }
        $('#icon').append($('<option></option>').attr({ value: value.code, 'data-content': labelHtml }).text(labelText));
    });

    $('#mod1').empty();
    $.each(data['modifier 1'], function (name, value) {
        var sym = new ms.Symbol(id + symbolset + '0000000000' + value.code + '00', { size: 16 });
        var labelHtml = '<img class="mil-icon" src="' + sym.asCanvas(window.devicePixelRatio).toDataURL() + '" width="' + sym.getSize().width + '" height="' + sym.getSize().height + '"> ' + value.modifier;
        $('#mod1').append($('<option></option>').attr({ value: value.code, 'data-content': labelHtml }).text(value.modifier));
    });

    $('#mod2').empty();
    $.each(data['modifier 2'], function (name, value) {
        var sym = new ms.Symbol(id + symbolset + '000000000000' + value.code, { size: 16 });
        var labelHtml = '<img class="mil-icon" src="' + sym.asCanvas(window.devicePixelRatio).toDataURL() + '" width="' + sym.getSize().width + '" height="' + sym.getSize().height + '"> ' + value.modifier;
        $('#mod2').append($('<option></option>').attr({ value: value.code, 'data-content': labelHtml }).text(value.modifier));
    });

    $('#size').selectpicker('refresh');
    $('#icon').selectpicker('refresh');
    $('#mod1').selectpicker('refresh');
    $('#mod2').selectpicker('refresh');
}
function getSymbol() {
    var symbol = '100';
    symbol += $('#id2').val() || '0';
    symbol += $('#set').val() || '00';
    symbol += $('#status').val() || '0';
    symbol += $('#hq').val() || '0';
    symbol += $('#size').val() || '00';
    symbol += $('#icon').val() || '000000';
    symbol += $('#mod1').val() || '00';
    symbol += $('#mod2').val() || '00';
    return symbol;
}

function addDegreesToConfig(config, name) {
    var value = $('#' + name).val();
    if (value !== '') {
        config[name] = String(Number(value) * 360 / 6400);
    }
}
function addToConfig(config, name) {
    var value = $('#' + name).val();
    if (value !== '') {
        config[name] = value;
    }
}

function getSymbolConfig() {

    var config = {};
    addToConfig(config, 'uniqueDesignation');
    addDegreesToConfig(config, 'direction');
    addToConfig(config, 'higherFormation');
    addToConfig(config, 'additionalInformation');
    addToConfig(config, 'reinforcedReduced');
    return config;
}

function setSymbol(symbol, config) {

    $('#id2').val(symbol.substr(3, 1));
    $('#set').val(symbol.substr(4, 2));
    applySymbolSet();

    $('#status').val(symbol.substr(6, 1));
    $('#hq').val(symbol.substr(7, 1));
    $('#size').val(symbol.substr(8, 2));
    $('#icon').val(symbol.substr(10, 6));
    $('#mod1').val(symbol.substr(16, 2));
    $('#mod2').val(symbol.substr(18, 2));

    $('#uniqueDesignation').val(config.uniqueDesignation || '');
    $('#higherFormation').val(config.higherFormation || '');
    $('#additionalInformation').val(config.additionalInformation || '');
    $('#reinforcedReduced').val(config.reinforcedReduced || '');
    $('#direction').val(config.direction !== undefined ? (Number(config.direction) * 6400 / 360) : '');

    applySymbol();

    $('select').selectpicker('render');
}

function applySymbol() {
    var symbol = getSymbol();

    $('#symbolNumber').text(symbol);

    var config = getSymbolConfig();

    config.size = 70;

    var sym = new ms.Symbol(symbol, config);

    $('#symbolPreview').empty();

    $('#symbolPreview').append($('<img></img>')
        .attr({
            src: sym.asCanvas(window.devicePixelRatio).toDataURL(),
            width: sym.getSize().width,
            height: sym.getSize().height

        })
    );

    return symbol;
}

var basicColors = { "ColorBlack": "000000", "ColorGrey": "7F7F7F", "ColorRed": "E50000", "ColorBrown": "7F3F00", "ColorOrange": "D86600", "ColorYellow": "D8D800", "ColorKhaki": "7F9966", "ColorGreen": "00CC00", "ColorBlue": "0000FF", "ColorPink": "FF4C66", "ColorWhite": "FFFFFF", "ColorUNKNOWN": "B29900", "colorBLUFOR": "004C99", "colorOPFOR": "7F0000", "colorIndependent": "007F00", "colorCivilian": "66007F" };

var modalMarkerId;
var modalMarkerData;
var clickPosition = null;
var currentLine = null;
var colorPicker;

function updateMarkerHandler(e) {
    var marker = e.sourceTarget;
    modalMarkerId = marker.options.markerId;
    modalMarkerData = marker.options.markerData;

    if (modalMarkerData.type == 'mil') {
        setSymbol(modalMarkerData.symbol, modalMarkerData.config);
        $('#milsymbol').modal('show');
        $('#milsymbol-delete').show();
        $('#milsymbol-update').show();
        $('#milsymbol-insert').hide();
        $('#milsymbol-grid').text(Arma3Map.toGrid(e.latlng));

    } else if (modalMarkerData.type == 'basic') {
        $('#basic-type').val(modalMarkerData.symbol);
        $('#basic-color').val(modalMarkerData.config.color);
        $('#basic-dir').val(modalMarkerData.config.dir);
        $('#basic-label').val(modalMarkerData.config.label);
        $('select').selectpicker('render');

        $('#basicsymbol').modal('show');
        $('#basicsymbol-delete').show();
        $('#basicsymbol-update').show();
        $('#basicsymbol-insert').hide();
        $('#basicsymbol-grid').text(Arma3Map.toGrid(e.latlng));

    } else if (modalMarkerData.type == 'line') {
        $('#line-color').val(modalMarkerData.config.color);
        $('select').selectpicker('render');
        $('#line').modal('show');
    }

};
function insertMilSymbol(latlng) {
    clickPosition = latlng;
    setSymbol(getSymbol(), {}); // Garde le même symbole, mais réinitialise les annotations
    $('#milsymbol').modal('show');
    $('#milsymbol-delete').hide();
    $('#milsymbol-update').hide();
    $('#milsymbol-insert').show();
    $('#milsymbol-grid').text(Arma3Map.toGrid(latlng));
};
function insertBasicSymbol(latlng) {
    clickPosition = latlng;
    $('#basicsymbol').modal('show');
    $('#basicsymbol-delete').hide();
    $('#basicsymbol-update').hide();
    $('#basicsymbol-insert').show();
    $('#basicsymbol-grid').text(Arma3Map.toGrid(latlng));
};

function milsymbolMarkerTool(backend) {

    $('#milsymbol-insert').on('click', function () {
        var symbol = getSymbol();
        var symbolConfig = getSymbolConfig();

        backend.addMarker({
            type: 'mil',
            symbol: symbol,
            config: symbolConfig,
            pos: [clickPosition.lat, clickPosition.lng]
        });

        $('#milsymbol').modal('hide');
    });

    $('#milsymbol-delete').on('click', function () {
        backend.removeMarker(modalMarkerId);
        $('#milsymbol').modal('hide');
    });

    $('#milsymbol-update').on('click', function () {
        modalMarkerData.symbol = getSymbol();
        modalMarkerData.config = getSymbolConfig();
        backend.updateMarker(modalMarkerId, modalMarkerData);
        $('#milsymbol').modal('hide');
    });

    $.each(milstd.app6d, function (name, value) {
        $('#set').append($('<option></option>').attr({ value: value.symbolset }).text(value.name));
    });
    $('#set').selectpicker("refresh");
    $('#set').val("10");
    applySymbolSet();

    $('#set').change(applySymbolSet);
    applySymbol();

    $('select').change(applySymbol);
    $('input').change(applySymbol);
}

function basicsymbolMarkerTool(backend) {

    $('#basicsymbol-insert').on('click', function () {
        backend.addMarker({
            type: 'basic',
            symbol: $('#basic-type').val(),
            config: { color: $('#basic-color').val(), label: $('#basic-label').val(), dir: $('#basic-dir').val() },
            pos: [clickPosition.lat, clickPosition.lng]
        });
        $('#basicsymbol').modal('hide');
    });

    $('#basicsymbol-delete').on('click', function () {
        backend.removeMarker(modalMarkerId);
        $('#basicsymbol').modal('hide');
    });

    $('#basicsymbol-update').on('click', function () {
        modalMarkerData.symbol = $('#basic-type').val();
        modalMarkerData.config = { color: $('#basic-color').val(), label: $('#basic-label').val(), dir: $('#basic-dir').val() };
        backend.updateMarker(modalMarkerId, modalMarkerData);
        $('#basicsymbol').modal('hide');
    });
}

function lineMarkerTool(backend) {

    $('#line-delete').on('click', function () {
        backend.removeMarker(modalMarkerId);
        $('#line').modal('hide');
    });

    $('#line-update').on('click', function () {
        modalMarkerData.config = { color: $('#line-color').val() };
        backend.updateMarker(modalMarkerId, modalMarkerData);
        $('#line').modal('hide');
    });
}

function insertLine(latlng, map, append, backend) {
    var point = latlng;

    if (!currentLine) {
        currentLine = L.polyline([point, point], { color: '#' + basicColors[colorPicker.val()], weight: 3, interactive: false }).addTo(map);
    } else if (append) {
        var data = currentLine.getLatLngs();
        data[data.length - 1] = point;
        data.push(point);
        currentLine.setLatLngs(data);
    } else {
        var data = currentLine.getLatLngs();
        data[data.length - 1] = point;
        currentLine.remove();
        currentLine = null;
        backend.addMarker({
            type: 'line',
            symbol: 'line',
            config: { color: colorPicker.val() },
            pos: data.map(function (e) { return [e.lat, e.lng]; }).flat()
        });
    }
}

function generateIcon(markerData) {

    if (markerData.type == 'mil') {
        var symbolConfig = $.extend({ size: 32 }, markerData.config);
        var sym = new ms.Symbol(markerData.symbol, symbolConfig);
        return L.icon({
            iconUrl: sym.asCanvas(window.devicePixelRatio).toDataURL(),
            iconSize: [sym.getSize().width, sym.getSize().height],
            iconAnchor: [sym.getAnchor().x, sym.getAnchor().y]
        });
    }

    var url = '/img/markers/' + markerData.config.color + '/' + markerData.symbol + '.png';

    if ((markerData.config.label && markerData.config.label.length > 0) || markerData.config.dir) {

        var img = $('<img src="' + url + '" width="32" height="32" />');
        if (markerData.config.dir) {
            img.css('transform', 'rotate(' + (Number(markerData.config.dir) * 360 / 6400) + 'deg)')
        }

        var iconHtml = $('<div></div>').append(
            $('<div></div>')
                .addClass('text-marker-content')
                .css('color', '#' + basicColors[markerData.config.color])
                .text(markerData.config.label)
                .prepend(img))
            .html();

        return new L.DivIcon({
            className: 'text-marker',
            html: iconHtml,
            iconAnchor: [16, 16]
        });
    }

    return L.icon({ iconUrl: url, iconSize: [32, 32], iconAnchor: [16, 16] });
}

function addOrUpdateMarker(map, markers, marker, canEdit, backend) {
    var markerId = marker.id;
    var markerData = marker.data;
    var existing = markers[markerId];

    if (markerData.type == 'line') {
        var posList = [];
        for (var i = 0; i < markerData.pos.length; i += 2) {
            posList.push([markerData.pos[i], markerData.pos[i + 1]]);
        }
        var color = '#' + (basicColors[markerData.config.color] || '000000');
        if (existing) {
            existing.setLatLngs(posList);
            existing.setStyle({ color: color });
            existing.options.markerData = markerData;
        } else {
            var mapMarker = L.polyline(posList, { color: color, weight: 3, interactive: canEdit, markerId: markerId, markerData: markerData }).addTo(map);
            if (canEdit) {
                mapMarker.on('click', updateMarkerHandler);
            }
            markers[markerId] = mapMarker;
        }
    } else {
        var icon = generateIcon(markerData);
        if (existing) {
            existing.setIcon(icon);
            existing.setLatLng(markerData.pos);
            existing.options.markerData = markerData;
        }
        else {
            var mapMarker =
                L.marker(markerData.pos, { icon: icon, draggable: canEdit, interactive: canEdit, markerId: markerId, markerData: markerData })
                    .addTo(map);
            if (canEdit) {
                mapMarker
                    .on('click', updateMarkerHandler)
                    .on('dragend', function (e) {
                        var marker = e.target;
                        var markerId = marker.options.markerId;
                        var markerData = marker.options.markerData;
                        markerData.pos = [marker.getLatLng().lat, marker.getLatLng().lng];
                        backend.moveMarker(markerId, markerData);
                    });
            }
            markers[markerId] = mapMarker;
        }
    }
}

function initMapArea(mapInfos, endpoint, center) {
    var map = L.map('map', {
        minZoom: mapInfos.minZoom,
        maxZoom: mapInfos.maxZoom,
        crs: mapInfos.CRS
    });
    L.tileLayer((endpoint || 'https://jetelain.github.io/Arma3Map') + mapInfos.tilePattern, {
        attribution: mapInfos.attribution,
        tileSize: mapInfos.tileSize
    }).addTo(map);

    if (center) { 
        map.setView(center, mapInfos.maxZoom);
    } else {
        map.setView(mapInfos.center, mapInfos.defaultZoom);
    }

    L.latlngGraticule({ zoomInterval: [{ start: 0, end: 10, interval: 1000 }] }).addTo(map);
    L.control.scale({ maxWidth: 200, imperial: false }).addTo(map);
    L.control.gridMousePosition().addTo(map);
    return map;
}

var tools;
var currentTool = 0;

function selectTool(map, num) {
    tools[currentTool].setClass('btn-outline-secondary');
    tools[num].setClass('btn-primary');
    currentTool = num;

    $('.leaflet-container').css('cursor', currentTool == 0 ? '' : 'crosshair');

    colorPicker.selectpicker(currentTool == 4 ? 'show' : 'hide');

    if (currentTool == 1) {
        map.dragging.disable();
    } else {
        map.dragging.enable();
    }
};

function createColorPicker() {
    var previousColor = Object.getOwnPropertyNames(basicColors)[0];
    var colorSelect = $('<select class="btn-maptool" data-container="body" id="color-tool" data-style="arma3-' + previousColor + '"></select>');
    Object.getOwnPropertyNames(basicColors).forEach(function (color) {
        colorSelect.append($('<option></option>').attr({
            value: color,
            'data-content': "<div class='arma3-" + color + "' style='width:20px; height:20px;'/>"
        }).addClass('arma3-' + color));
    });
    colorSelect.on('change', function () {
        colorSelect.selectpicker('setStyle', 'arma3-' + previousColor, 'remove');
        colorSelect.selectpicker('setStyle', 'arma3-' + colorSelect.val(), 'add');
        previousColor = colorSelect.val();
    });
    return colorSelect;
}

function search(map, mapInfos, markers) {

    var terms = new RegExp($('#search-term').val(), 'i');

    var resultEntries = [];

    mapInfos.cities.forEach(function (city) {
        if (terms.test(city.name)) {
            resultEntries.push({
                name: city.name,
                latLng: [city.y, city.x],
                icon: '/img/home-solid.svg'
            });
        }
    });

    Object.getOwnPropertyNames(markers).forEach(function (id) {
        var marker = markers[id];
        var markerData = marker.options.markerData;
        if (markerData.config.label && terms.test(markerData.config.label)) {
            resultEntries.push({
                name: markerData.config.label,
                latLng: marker.getLatLng(),
                icon: ''
            });
        }
        else if (
            (markerData.config.additionalInformation && terms.test(markerData.config.additionalInformation))
            || (markerData.config.higherFormation && terms.test(markerData.config.higherFormation))) {
            resultEntries.push({
                name: (markerData.config.additionalInformation || '') + ' ' + (markerData.config.higherFormation || ''),
                latLng: marker.getLatLng(),
                icon: marker.getIcon().options.iconUrl
            });
        }
    });

    resultEntries.sort(function (a, b) {
        return a.name.localeCompare(b.name);
    });

    var results = $('#search-results').empty();
    resultEntries.forEach(function (resultData) {
        var entry = $('<li class="media"><div class="mr-1" style="width:30px;overflow:hidden;"><img height="20" /></div><div class="media-body"><a href="#"></a></div></li>');
        entry.find('img').attr('src', resultData.icon);
        entry.find('a').text(resultData.name).on('click', function () { $('#search').modal('hide'); map.setView(resultData.latLng, mapInfos.maxZoom); return false; });
        results.append(entry);
    });

    console.log(resultEntries);
}

var Backend =
{
    SignalR: {
        create: function (mapid) {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/MapHub")
                .withAutomaticReconnect()
                .build();
            this.mapid = mapid;
        },

        connect: function (callbacks) {
            this.connection.on("AddOrUpdateMarker", function (marker, isReadOnly) {
                callbacks.addOrUpdateMarker(marker, isReadOnly === true);
            });
            this.connection.on("RemoveMarker", function (marker) {
                callbacks.removeMarker(marker);
            });
            this.connection.on("PointMap", function (id, pos) {
                callbacks.pointMap(id, pos);
            });
            this.connection.on("EndPointMap", function (id) {
                callbacks.endPointMap(id);
            });
            var _this = this;
            this.connection.start()
                .then(function () {
                    _this.connection.invoke("Hello", _this.mapid);
                }).catch(callbacks.connectionLost);
            this.connection.onclose(callbacks.connectionLost);
        },
        addMarker: function (markerData) {
            this.connection.invoke("AddMarker", markerData);
        },
        removeMarker: function (markerId) {
            this.connection.invoke("RemoveMarker", markerId);
        },
        updateMarker: function (markerId, markerData) {
            this.connection.invoke("UpdateMarker", markerId, markerData);
        },
        moveMarker: function (markerId, markerData) {
            this.connection.invoke("MoveMarker", markerId, markerData);
        },
        pointMap: function (pos) {
            this.connection.invoke("PointMap", pos);
        },
        endPointMap: function () {
            this.connection.invoke("EndPointMap");
        }
    }
};

function setupEditTools(map, backend) {

    tools = [
        L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 0); }, content: '<i class="far fa-hand-paper"></i>' }).addTo(map),
        L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 1); }, content: '<i class="far fa-hand-pointer"></i>' }).addTo(map),
        L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 2); }, content: '<img height="16" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEsAAAAzCAYAAADfP/VGAAACTElEQVRoge3ZsUsCcRTA8TdZaurZmJG2RtASURBI0BRELVG0BBFYi0tDkQSOQf0BTTW1FU01tQhBtNSWi8NFFNSc82soT9PTu/N+7/d+d+fwQLg70C8f7n73EwAg0hvbAxEAwN5Yjt4Sqz+WQC2VDvwkhkYwFI2ZxwrHk8aB1MQUbl7eY1HHQM7q6RVqw5lfPHGtNZaWSmPu5glHZ+aMaNn8IfsXlzkHL984uZ4zfv/44hrmSxXzWLWL5veOAqesUVM4kcSl4zMs6oiFcrVzrKKOgVFmpmn38d04bitWEJS109Q4jmL5UZmVJlex/KTMjiYhsbyszIkmYbG8qKxZ0/LJue1rhcTygrJuNZHEUlmZG02ksVRSJkITeSwVlInSJC0WhzLRmqTGkqmMQhNLLEpllJrYYlEoo9bEHkuEMlmalIjlRplMTUrFcqKMQ5Nysewo49KkbCwzZbM7+6yalI5lpgwAsC8aY9GkfKzmexP3O6aysczuTarsZCgTy+pJp8JOhhKxnDzpOJWxxup23cSljC2WiHWTbGXSY4lehctUJjUW5SpchjIpsWS901ErI4/F8U5HpYwsFvcOAYUyklgq7BBQKBMai1sTtTJhsf5p0gbZdwgolLmOpaomCmWuYnlBk0hlXcXymiZRyhzH8rImt8psx/KLJjfKbMXyo6ZulHWM5XdN7Wb79tlUWdtYQdJkV9nGxV1rrFB0wDhpbGEF86UKFsrVQM7W9QNmprPN/zTVY/XGcl5rsXQA+AKAt7/PvanPBwB8AoD+A4WfYoYlQx+dAAAAAElFTkSuQmCC" />' }).addTo(map),
        L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 3); }, content: '●' }).addTo(map),
        L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 4); }, content: '╱' }).addTo(map)
    ];

    colorPicker = createColorPicker();
    L.control.overlayDiv({ content: colorPicker.get(0), position: 'topleft' }).addTo(map);

    milsymbolMarkerTool(backend);

    basicsymbolMarkerTool(backend);

    lineMarkerTool(backend);

    map.on('click', function (e) {
        clickPosition = e.latlng;
        if (e.originalEvent.target.localName == "div") {
            if (currentTool == 2) {
                insertMilSymbol(e.latlng);
            }
            else if (currentTool == 3) {
                insertBasicSymbol(e.latlng);
            }
            else if (currentTool == 4) {
                insertLine(e.latlng, map, e.originalEvent.ctrlKey, backend);
            }
        }
    });

    var isPointing = false;

    map.on('mousemove', function (e) {
        if (isPointing) {
            backend.pointMap([e.latlng.lat, e.latlng.lng]);
        }
        if (currentLine) {
            var data = currentLine.getLatLngs();
            data[data.length - 1] = [e.latlng.lat, e.latlng.lng];
            currentLine.setLatLngs(data);
        }
    });

    map.on('mousedown', function (e) {
        if (currentTool == 1) {
            isPointing = true;
            backend.pointMap([e.latlng.lat, e.latlng.lng]);
        }
    });
    map.on('mouseup mouseout', function (e) {
        if (isPointing) {
            isPointing = false;
            backend.endPointMap();
        }
    });
    map.on('contextmenu', function (e) { });

    $('select').selectpicker();

    selectTool(map, 0);
}

function setupSeach(map, mapInfos, markers) {
    L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topright', click: function () { $('#search').modal('show'); search(map, mapInfos, markers); }, content: '<i class="fas fa-search"></i>' }).addTo(map);
    $('#search-term').on('keyup', function () { search(map, mapInfos, markers); });
}

function initLiveMap(config) {
    $(function () {

        var worldName = config.worldName;
        var mapId = config.mapId;
        var canEdit = !config.isReadOnly;

        var mapInfos = Arma3Map.Maps[worldName || 'altis'] || Arma3Map.Maps.altis;

        var map = initMapArea(mapInfos, config.endpoint);

        var markers = {};
        var pointing = {};
        var backend = Backend.SignalR;
        backend.create(mapId);

        if ($('#share').length) {
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topright', click: function () { $('#share').modal('show'); }, content: '<i class="fas fa-share-square"></i>' }).addTo(map);
        }

        setupSeach(map, mapInfos, markers);

                //L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topright', click: function () { }, content: '<i class="fas fa-lock-open"></i>' }).addTo(map);

        var pointingIcon = new L.icon({ iconUrl: '/img/pointmap.png', iconSize: [16, 16], iconAnchor: [8, 8] });

        backend.connect({
            addOrUpdateMarker: function (marker, isReadOnly) {
                addOrUpdateMarker(map, markers, marker, canEdit && !isReadOnly, backend);
            },
            removeMarker: function (marker) {
                var existing = markers[marker.id];
                if (existing) {
                    existing.remove();
                }
            },
            pointMap: function (id, pos) {
                console.log("PointMap", id, pos);
                var existing = pointing[id];
                if (existing) {
                    existing.setLatLng(pos);
                }
                else {
                    pointing[id] = L.marker(pos, { icon: pointingIcon, draggable: false, interactive: false }).addTo(map);
                }
            },
            endPointMap: function (id) {
                var existing = pointing[id];
                if (existing) {
                    existing.remove();
                    delete pointing[id];
                }
            },
            connectionLost: function (e) {
                if (e) {
                    console.error(e);
                    $('#connectionlost').show();
                }
            }
        });

        if (canEdit) {
            setupEditTools(map, backend);
        }

    });
}

function initStaticMap(config) {
    $(function () {

        var worldName = config.worldName;
        var mapInfos = Arma3Map.Maps[worldName || 'altis'] || Arma3Map.Maps.altis;
        var markers = {};

        var map = initMapArea(mapInfos, config.endpoint, config.center);

        setupSeach(map, mapInfos, markers);

        Object.getOwnPropertyNames(config.markers).forEach(function (id) {
            addOrUpdateMarker(map, markers, { id: id, data: config.markers[id] }, false, null);
        });
    });
}
