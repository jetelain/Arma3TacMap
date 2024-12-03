var arma3TacMapLoaded = false;

var Arma3TacMap;
(function (Arma3TacMap) {

    ms.setStandard("APP6"); // We always use APP6 edition D

    var intl = new Intl.NumberFormat();

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

    // Keep up to date with Arma3TacMapLibrary.Maps.MapExporter
    var metisSupported = {
        entity: ["121100", "121102", "121103", "121104", "121105", "120500", "120600", "121000", "150600", "120501", "111000"],
        modifier1: ["98"],
        modifier2: ["51"],
        size: ["11", "12", "13", "14", "15", "16", "17", "18", "21", "22", "23", "24"],
        status: ["1"]
    };

    function getMetisCompatible(wanted, supported, fallback) {
        if (supported.includes(wanted)) {
            return wanted;
        }
        return fallback;
    }

    function getSymbolMetisCompatible() {
        var symbol = '100';
        symbol += $('#id2').val() || '0';
        symbol += '10';
        symbol += getMetisCompatible($('#status').val() || '0', $('#id2').val() != "4" ? metisSupported.status : [], '0');
        symbol += '0';
        symbol += getMetisCompatible($('#size').val() || '00', metisSupported.size, '00');
        symbol += getMetisCompatible($('#icon').val() || '000000', metisSupported.entity, '000000');
        symbol += getMetisCompatible($('#mod1').val() || '00', metisSupported.modifier1, '00');
        symbol += getMetisCompatible($('#mod2').val() || '00', metisSupported.modifier2, '00');
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

        var symMetis = new ms.Symbol(getSymbolMetisCompatible(), {
            size: 40,
            uniqueDesignation: config.uniqueDesignation
        });
        $('#symbolPreviewMetis').empty();
        $('#symbolPreviewMetis').append($('<img></img>')
            .attr({
                src: symMetis.asCanvas(window.devicePixelRatio).toDataURL(),
                width: symMetis.getSize().width,
                height: symMetis.getSize().height
            })
        );
        return symbol;
    }

    var basicColors = {
        "colorblack": "#000000",
        "colorgrey": "#7f7f7f",
        "colorred": "#e50000",
        "colorbrown": "#7f3f00",
        "colororange": "#d86600",
        "coloryellow": "#d8d800",
        "colorkhaki": "#7f9966",
        "colorgreen": "#00cc00",
        "colorblue": "#0000ff",
        "colorpink": "#ff4c66",
        "colorwhite": "#ffffff",
        "colorunknown": "#b29900",
        "colorblufor": "#004c99",
        "coloropfor": "#7f0000",
        "colorindependent": "#007f00",
        "colorcivilian": "#66007f"
    };

    function colorToCss(color) {
        return basicColors[(color || '').toLowerCase()] || '#000000';
    }


    var modalMarkerId;
    var modalMarker;
    var modalMarkerData;
    var clickPosition = null;
    var currentLine = null;
    var currentMeasure = null;
    var currentLayer = null;
    var currentMission = null;
    var missionSelection = null;
    var colorPicker;
    var lockAllButton;
    var noteEditor = null;

    var pointEditMarkers = [];

    function getCurrentLayerId() {
        return currentLayer ? currentLayer.id : null;
    }

    function previewMakerPointUpdate(marker, posList, map) {
        var markerData = marker.options.markerData;
        if (markerData.type == 'line') {
            marker.setLatLngs(posList);
        }
        else if (markerData.type == 'measure') {
            marker.setLatLngs(posList);
            computeDistanceAndShowTooltip(map, marker, posList, true);
        }
        else if (markerData.type == 'mission') {
            var result = generateMission(markerData.symbol, posList, markerData.config.size || '13');
            marker.setLatLngs(result.lines);
            if (result.labelsPoints) {
                for (var i = 0; i < result.labelsPoints.length; ++i) {
                    marker.labels[i].setLatLng(result.labelsPoints[i]);
                }
            }
        }
        else {
            marker.setLatLng(posList[0]);
        }
    }

    function saveMarkerPointsUpdate(marker, posList, backend) {
        var markerData = marker.options.markerData;
        markerData.pos = posList.flat();
        backend.moveMarker(marker.options.markerId, markerData);
    }

    function editMarkerPoints(marker, map, backend) {
        clearpointEditMakers();
        var posList = posToPoints(marker.options.markerData.pos);
        var icon = new L.DivIcon({
            className: 'edit-marker',
            html: '',
            iconAnchor: [8, 8],
            iconSize: [16, 16]
        });
        for (var i = 0; i < posList.length; ++i) {
            pointEditMarkers.push(
                L.marker(posList[i], { icon: icon, draggable: true, interactive: true, zIndexOffset: 1000, pointIndex: i })
                    .addTo(map)
                    .on('drag', function (e) {
                        var latlng = e.target.getLatLng();
                        posList[e.target.options.pointIndex] = [latlng.lat, latlng.lng];
                        previewMakerPointUpdate(marker, posList, map);
                    })
                    .on('dragend', function (e) {
                        saveMarkerPointsUpdate(marker, posList, backend);
                    })
            );
        }
    }

    function clearpointEditMakers() {
        if (pointEditMarkers.length > 0) {
            pointEditMarkers.forEach(m => m.remove());
        }
        pointEditMarkers = [];
    }

    function updateMarkerHandler(e, map, backend) {
        clearpointEditMakers();

        var marker = e.target;
        if (marker.isDisabled) {
            e.mapCanHandle = true;
            map.fire('click', e);
            return;
        }

        modalMarker = marker;
        modalMarkerId = marker.options.markerId;
        modalMarkerData = marker.options.markerData;

        if (e.originalEvent.altKey) {
            editMarkerPoints(marker, map, backend);
            return;
        }

        $('select.layers-dropdown').val('' + marker.options.markerLayer.id);
        $('select.layers-dropdown').selectpicker('refresh');

        if (modalMarkerData.type == 'mil') {
            setSymbol(modalMarkerData.symbol, modalMarkerData.config);
            $('#milsymbol-scale').val((modalMarkerData.scale ?? 1) * 100);
            $('#milsymbol').modal('show');
            $('#milsymbol-delete').show();
            $('#milsymbol-update').show();
            $('#milsymbol-insert').hide();
            $('#milsymbol-grid').text(Arma3Map.toGrid(e.latlng));

        } else if (modalMarkerData.type == 'basic') {
            $('#basic-type').val(modalMarkerData.symbol);
            $('#basic-color').val(modalMarkerData.config.color.toLowerCase());
            $('#basic-dir').val(modalMarkerData.config.dir);
            $('#basic-label').val(modalMarkerData.config.label);
            $('#basic-scale').val((modalMarkerData.scale ?? 1) * 100);
            $('select').selectpicker('render');

            $('#basicsymbol').modal('show');
            $('#basicsymbol-delete').show();
            $('#basicsymbol-update').show();
            $('#basicsymbol-insert').hide();
            $('#basicsymbol-grid').text(Arma3Map.toGrid(e.latlng));

        } else if (modalMarkerData.type == 'line') {
            $('#line-color').val(modalMarkerData.config.color.toLowerCase());
            $('select').selectpicker('render');
            $('#line').modal('show');
        } else if (modalMarkerData.type == 'measure') {
            $('#measure').modal('show');
        } else if (modalMarkerData.type == 'mission') {
            $('#mission-edit-color').val(modalMarkerData.config.color.toLowerCase());
            $('select').selectpicker('render');
            $('#mission-edit').modal('show');

        } else if (modalMarkerData.type == 'note') {

            initNoteEditor(modalMarkerData.config.content);

            $('#note-position').val(modalMarkerData.config.position);
            $('select').selectpicker('render');

            $('#note-dialog').modal('show');
            $('#note-delete').show();
            $('#note-update').show();
            $('#note-insert').hide();
            $('#note-grid').text(Arma3Map.toGrid(e.latlng));
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

        $('#milsymbol-layer').val('' + getCurrentLayerId());
        $('#milsymbol-layer').selectpicker('refresh');
    };

    function insertOrbat(latlng) {
        clickPosition = latlng;
        $('#orbat').modal('show');
        $('#orbat-grid').text(Arma3Map.toGrid(latlng));
    };
    function insertBasicSymbol(latlng) {
        clickPosition = latlng;
        $('#basicsymbol').modal('show');
        $('#basicsymbol-delete').hide();
        $('#basicsymbol-update').hide();
        $('#basicsymbol-insert').show();
        $('#basicsymbol-grid').text(Arma3Map.toGrid(latlng));

        $('#basicsymbol-layer').val('' + getCurrentLayerId());
        $('#basicsymbol-layer').selectpicker('refresh');
    };

    function milsymbolMarkerTool(backend) {

        $('#milsymbol-insert').on('click', function () {
            var symbol = getSymbol();
            var symbolConfig = getSymbolConfig();

            backend.addMarker(Number($('#milsymbol-layer').val()), {
                type: 'mil',
                symbol: symbol,
                config: symbolConfig,
                scale: Number($('#milsymbol-scale').val()) / 100,
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
            modalMarkerData.scale = Number($('#milsymbol-scale').val()) / 100;
            backend.updateMarkerToLayer(modalMarkerId, Number($('#milsymbol-layer').val()), modalMarkerData);
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

    function orbatMarkerTool(backend) {
        $('.orbat-btn').on('click', function () {
            
            backend.addMarker(getCurrentLayerId(), {
                type: 'mil',
                symbol: $(this).attr('data-milsymbol'),
                config: { uniqueDesignation: $(this).attr('data-unique-designation') },
                pos: [clickPosition.lat, clickPosition.lng]
            });
            $('#orbat').modal('hide');
        });

        $('#orbat-custom').on('click', function () {
            insertMilSymbol(clickPosition);
            $('#orbat').modal('hide');
        });
    }

    function basicsymbolMarkerTool(backend) {

        $('#basicsymbol-insert').on('click', function () {
            backend.addMarker(Number($('#basicsymbol-layer').val()), {
                type: 'basic',
                symbol: $('#basic-type').val(),
                config: { color: $('#basic-color').val(), label: $('#basic-label').val(), dir: $('#basic-dir').val() },
                scale: Number($('#basic-scale').val()) / 100,
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
            modalMarkerData.scale = Number($('#basic-scale').val()) / 100;
            backend.updateMarkerToLayer(modalMarkerId, Number($('#basicsymbol-layer').val()), modalMarkerData);
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
            backend.updateMarkerToLayer(modalMarkerId, Number($('#line-layer').val()), modalMarkerData);
            $('#line').modal('hide');
        });
    }

    function measureMarkerTool(backend) {
        $('#measure-delete').on('click', function () {
            backend.removeMarker(modalMarkerId);
            $('#measure').modal('hide');
        });

    }

    function missionTool(map, backend) {

        $('#mission-edit-delete').on('click', function () {
            backend.removeMarker(modalMarkerId);
            $('#mission-edit').modal('hide');
        });

        $('#mission-edit-update').on('click', function () {
            modalMarkerData.config.color = $('#mission-edit-color').val();
            backend.updateMarkerToLayer(modalMarkerId, Number($('#mission-layer').val()), modalMarkerData);
            $('#mission-edit').modal('hide');
        });

        $('.mission-btn').on('click', function () {
            missionSelection = {
                mission: $(this).attr('data-mission'),
                size: $('#mission-selector-size').val(),
                color: $('#mission-selector-color').val()
            };
            $('#mission-selector').modal('hide');
            return false;
        });

        $('#mission-selector').on('hidden.bs.modal', function (event) {
            if (!missionSelection) {
                selectTool(map, 0);
            }
        });
    }

    function noteTool(map, backend) {
        $('#note-insert').on('click', function () {

            backend.addMarker(Number($('#note-layer').val()), {
                type: 'note',
                config: { content: noteEditor.getContent(), position: $('#note-position').val() },
                pos: [clickPosition.lat, clickPosition.lng]
            });
            $('#note-dialog').modal('hide');
        });

        $('#note-delete').on('click', function () {
            backend.removeMarker(modalMarkerId);
            $('#note-dialog').modal('hide');
        });

        $('#note-update').on('click', function () {
            modalMarkerData.config = { content: noteEditor.getContent(), position: $('#note-position').val() };
            backend.updateMarkerToLayer(modalMarkerId, Number($('#note-layer').val()), modalMarkerData);
            $('#note-dialog').modal('hide');
        });

        $(document).on('focusin', function (e) {
            if ($(e.target).closest(".tox-tinymce, .tox-tinymce-aux, .moxman-window, .tam-assetmanager-root").length) {
                e.stopImmediatePropagation();
            }
        });
    }

    function layersModal(backend) {
        function getPhase() {
            return $('#layer-phase-mode-all').is(':checked') ? null : Number($('#layer-phase-number').val());
        }

        $('#layers-add').on('click', function () {
            $('#layer-label').val('');
            $('#layer-phase-mode-all').prop("checked", true);
            $('#layer-phase-mode-specific').prop("checked", false);
            $('#layer-phase-number').val("0");
            $('#layer-insert').show();
            $('#layer-update').hide();
            $('#layer').modal('show');
            return false;
        });

        $('#layer-insert').on('click', function () {
            backend.addLayer({ label: $('#layer-label').val(), phase: getPhase() });
            $('#layer').modal('hide');
            return false;
        });

        $('#layer-update').on('click', function () {
            backend.updateLayer(currentLayer.id, { label: $('#layer-label').val(), phase: getPhase() });
            $('#layer').modal('hide');
            return false;
        });

        $('#layer-delete-confirm').on('click', function () {
            backend.removeLayer(currentLayer.id);
            $('#layer-delete').modal('hide');
            return false;
        });
    }

    function insertLine(latlng, map, append, backend) {
        var point = latlng;

        if (!currentLine) {
            currentLine = L.polyline([point, point], { color: colorToCss(colorPicker.val()), weight: 3, interactive: false }).addTo(map);
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
            backend.addMarker(getCurrentLayerId(), {
                type: 'line',
                symbol: 'line',
                config: { color: colorPicker.val() },
                pos: data.map(function (e) { return [e.lat, e.lng]; }).flat()
            });
        }
    }


    function insertMeasure(latlng, map, backend) {
        if (!currentMeasure) {
            currentMeasure = L.polyline([latlng, latlng], { color: '#000000', weight: 1.3, dashArray: '4', interactive: false }).addTo(map);
        }
        else {
            var data = currentMeasure.getLatLngs();
            currentMeasure.remove();
            currentMeasure = null;
            backend.addMarker(getCurrentLayerId(), {
                type: 'measure',
                symbol: 'measure',
                config: {},
                pos: data.map(function (e) { return [e.lat, e.lng]; }).flat()
            });
        }
    }

    function generateMission(mission, points, size) {
        var def = MilMissions.missions[mission];
        if (def) {
            var result = { labels: def.labels};
            var sizeMeters = 1000;
            switch (size) {
                case '12': sizeMeters = 25; break;
                case '13': sizeMeters = 50; break;
                case '14': sizeMeters = 250; break;
            }
            if (def.points > 1 && points.length < 2) {
                result.lines = points;
                if (def.labels) {
                    result.labelsPoints = [points[0], points[0]]; // we have up to 2 labels
                }
                return result;
            }
            if (def.points == 4 && points.length < 4) {
                points = MilMissions.complete4Points(points, sizeMeters);
            }
            result.lines = def.generate(points, sizeMeters);
            if (def.labels) {
                result.labelsPoints = def.generateLabels(points, sizeMeters, result.lines);
            }
            return result;
        }
        return {lines:[]};
    }

    function missionLabels(marker, result, target) {
        if (!marker.labels) {
            marker.labels = result.labels.map(function (label, i) {
                return L.marker(result.labelsPoints[i], {
                    icon: new L.DivIcon({
                        className: 'mission-text',
                        html: $('<div></div>').text(label).html(),
                        iconAnchor: [8, 8],
                        iconSize: [16, 16]
                    }),
                    interactive: marker.options.interactive
                }).addTo(target).on('click', function (ev) {
                    ev.sourceTarget = marker;
                    ev.target = marker;
                    marker.fire('click', ev);
                });
            });
            marker.on('remove', function (ev) {
                ev.target.labels.forEach(function (l) { l.remove(); });
            });
        } else {
            for (var i = 0; i < result.labelsPoints.length; ++i) {
                marker.labels[i].setLatLng(result.labelsPoints[i]);
            }
        }
    }

    function insertMission(latlng, map, backend) {

        if (!missionSelection) {
            $('#mission-selector').modal('show');
            return;
        }
        if (!missionSelection.points) {
            missionSelection.points = [[latlng.lat, latlng.lng]];
        } else {
            missionSelection.points.push([latlng.lat, latlng.lng]);
        }
        if (MilMissions.missions[missionSelection.mission].points == missionSelection.points.length) {
            if (currentMission) {
                currentMission.remove();
                currentMission = null;
            }
            backend.addMarker(getCurrentLayerId(), {
                type: 'mission',
                symbol: missionSelection.mission,
                config: { size: missionSelection.size, color: missionSelection.color },
                pos: missionSelection.points.flat()
            });
            missionSelection = null;
            selectTool(map, 0);
        } else {
            var result = generateMission(missionSelection.mission, missionSelection.points, missionSelection.size);
            if (!currentMission) {
                var color = colorToCss(missionSelection.color);
                currentMission = L.polyline(result.lines, { smoothFactor: 0.5, color: color, weight: 3, interactive: false }).addTo(map);
            } else {
                currentMission.setLatLngs(result.lines);
            }
            if (result.labels) {
                missionLabels(currentMission, result, map);
            }
        }
    }

    function insertNote(latlng, map, backend) {
        $('#note-delete').hide();
        $('#note-update').hide();
        $('#note-insert').show();
        $('#note-grid').text(Arma3Map.toGrid(latlng));
        $('#note-dialog').modal('show');

        $('#note-layer').val('' + getCurrentLayerId());
        $('#note-layer').selectpicker('refresh');

        initNoteEditor('');
    }

    function initNoteEditor(data) {
        if (!noteEditor) {
            tinymce.init({
                selector: '#note-content',
                plugins: 'image fullscreen autolink link',
                content_css: '/lib/bootstrap/dist/css/bootstrap.min.css,/css/site.css,/css/arma3TacMap.css',
                toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | outdent indent | image link | fullscreen',
                height: "350",
                image_dimensions: false,
                init_instance_callback: function (editor) {
                    editor.setContent(data);
                    noteEditor = editor;
                }
            });
        }
        else {
            noteEditor.setContent(data);
        }
    }

    function generateIcon(markerData) {

        var size = 32;
        if (markerData.scale) {
            size = Number(markerData.scale) * size;
        }
        if (markerData.type == 'mil') {
            var symbolConfig = $.extend({ size: size }, markerData.config);
            var sym = new ms.Symbol(markerData.symbol, symbolConfig);
            return L.icon({
                iconUrl: sym.asCanvas(window.devicePixelRatio).toDataURL(),
                iconSize: [sym.getSize().width, sym.getSize().height],
                iconAnchor: [sym.getAnchor().x, sym.getAnchor().y]
            });
        }

        var url = '/img/markers/' + markerData.config.color + '/' + markerData.symbol + '.png';

        if ((markerData.config.label && markerData.config.label.length > 0) || markerData.config.dir) {

            var img = $('<img src="' + url + '" width="' + size + '" height="' + size + '" />');
            if (markerData.config.dir) {
                img.css('transform', 'rotate(' + (Number(markerData.config.dir) * 360 / 6400) + 'deg)')
            }

            var iconHtml = $('<div></div>').append(
                $('<div></div>')
                    .addClass('text-marker-content')
                    .css('color', colorToCss(markerData.config.color))
                    .text(markerData.config.label)
                    .prepend(img))
                .html();

            return new L.DivIcon({
                className: 'text-marker',
                html: iconHtml,
                iconAnchor: [size / 2, size/2]
            });
        }

        return L.icon({ iconUrl: url, iconSize: [size, size], iconAnchor: [size / 2, size/2] });
    }

    function posToPoints(pos) {
        var points = [];
        for (var i = 0; i < pos.length; i += 2) {
            points.push([pos[i], pos[i + 1]]);
        }
        return points;
    }

    function addOrUpdateMarker(map, markers, marker, canEdit, backend, opacity, layer) {
        var markerId = marker.id;
        var markerData = marker.data;
        var existing = markers[markerId];

        if (markerData.type == 'line') {
            var posList = posToPoints(markerData.pos);
            var color = colorToCss(markerData.config.color);
            if (existing) {
                existing.setLatLngs(posList);
                existing.setStyle({ color: color });
                existing.options.markerData = markerData;
            } else {
                var mapMarker = L.polyline(posList, { color: color, weight: 3, interactive: canEdit, markerId: markerId, markerData: markerData, opacity: opacity['line'] ?? 1.0 }).addTo(layer.group);
                if (canEdit) {
                    mapMarker.on('click', e => updateMarkerHandler(e,map,backend));
                }
                markers[markerId] = existing = mapMarker;
            }
        }
        else if (markerData.type == 'measure') {
            var posList = posToPoints(markerData.pos);
            if (existing) {
                existing.setLatLngs(posList);
                computeDistanceAndShowTooltip(map, existing, posList, canEdit, markerId, markerData);
                existing.options.markerData = markerData;
            } else {
                var mapMarker = L.polyline(posList, { color: '#000000', weight: 1.3, dashArray: '4', interactive: canEdit, markerId: markerId, markerData: markerData, opacity: opacity['measure'] ?? 1.0 }).addTo(layer.group);
                computeDistanceAndShowTooltip(map, mapMarker, posList, canEdit, markerId, markerData);
                if (canEdit) {
                    mapMarker.on('click', e => updateMarkerHandler(e, map, backend));
                }
                markers[markerId] = existing = mapMarker;
            }
        }
        else if (markerData.type == 'mission') {
            var posList = posToPoints(markerData.pos);
            var color = colorToCss(markerData.config.color);
            var result = generateMission(markerData.symbol, posList, markerData.config.size || '13');
            if (existing) {
                existing.setLatLngs(result.lines);
                existing.setStyle({ color: color });
                existing.options.markerData = markerData;
            } else {
                var mapMarker = L.polyline(result.lines, { smoothFactor: 0.5, color: color, weight: 3, interactive: canEdit, markerId: markerId, markerData: markerData, opacity: opacity['line'] ?? 1.0 }).addTo(layer.group);
                markers[markerId] = existing = mapMarker;
                if (canEdit) {
                    mapMarker.on('click', e => updateMarkerHandler(e, map, backend));
                }
            }
            if (result.labels) {
                missionLabels(existing, result, layer.group);
            }
        }
        else if (markerData.type == 'note') {
            if (existing) {
                existing.setLatLng(markerData.pos);
                existing.setContent( markerData.config.content);
                existing.options.direction = markerData.config.position;
                existing.options.markerData = markerData;
                existing.update();
            } else {
                var options = { content: markerData.config.content, direction: markerData.config.position, permanent: true, interactive: true, markerId: markerId, markerData: markerData, className: 'stickyNote' };
                var mapMarker = L.tooltip(markerData.pos, options).addTo(layer.group);
                if (canEdit) {
                    mapMarker.on('click', e => updateMarkerHandler(e, map, backend));
                }
                markers[markerId] = existing = mapMarker;
            }
        }
        else {
            var icon = generateIcon(markerData);
            if (existing) {
                existing.setIcon(icon);
                existing.setLatLng(markerData.pos);
                existing.options.markerData = markerData;
            }
            else {
                var mapMarker =
                    L.marker(markerData.pos, { icon: icon, draggable: canEdit, interactive: canEdit, markerId: markerId, markerData: markerData, opacity: opacity[markerData.type] ?? 1.0 })
                        .addTo(layer.group);
                if (canEdit) {
                    mapMarker
                        .on('click', e => updateMarkerHandler(e, map, backend))
                        .on('dragend', function (e) {
                            var marker = e.target;
                            var markerId = marker.options.markerId;
                            var markerData = marker.options.markerData;
                            markerData.pos = [marker.getLatLng().lat, marker.getLatLng().lng];
                            backend.moveMarker(markerId, markerData);
                        });
                }
                markers[markerId] = existing = mapMarker;
            }
        }

        if (existing.options.markerLayer != layer) {
            
            if (existing.options.markerLayer) {
                existing.removeFrom(existing.options.markerLayer.group);
                existing.addTo(layer.group);
            }
            existing.options.markerLayer = layer;
        }

        updateMarkerState(layer, existing);

    }

    function isFastSvg() {
        // Only Chrome and Safari are fast enough to animate SVG tiles
        return (window.chrome && window.navigator.vendor == "Google Inc.") || navigator.vendor.indexOf("pple") != -1;
    }

    function changeOpacity(tileLayer, delta) {
        var current = tileLayer.options.opacity ?? 1;
        var opacity = Math.min(Math.max(0, current + delta), 1);
        tileLayer.setOpacity(opacity);
        $('#bg-opacity').text(Math.round(opacity * 100) + ' %');
    }

    function initMapArea(mapInfos, endpoint, center, fullScreen) {
        var map = L.map('map', {
            minZoom: mapInfos.minZoom,
            maxZoom: mapInfos.maxZoom +1 ,
            crs: mapInfos.CRS,
            zoomSnap: fullScreen ? 0.01 : 0.2,
            zoomControl: !fullScreen,
            zoomDelta: 0.2,
            zoomAnimation: !mapInfos.isSVG || isFastSvg(),
            fadeAnimation: !fullScreen
        });

        var tileLayer = L.tileLayer((endpoint || 'https://jetelain.github.io/Arma3Map') + mapInfos.tilePattern, {
                attribution: mapInfos.attribution,
                tileSize: mapInfos.tileSize,
                maxNativeZoom: mapInfos.maxZoom
            })
            .on('load', function () { arma3TacMapLoaded = true })
            .addTo(map);

        if (fullScreen) {
            map.fitBounds([[0, 0], [mapInfos.worldSize, mapInfos.worldSize]]);
        } else if (center) {
            map.setView(center, mapInfos.maxZoom);
        } else {
            map.setView(mapInfos.center, mapInfos.defaultZoom);
        }

        var graticuleZooms = [];
        if (mapInfos.maxZoom > 4) {
            graticuleZooms.push({ start: 0, end: mapInfos.maxZoom - 4, interval: 10000 });
            graticuleZooms.push({ start: mapInfos.maxZoom - 4, end: 10, interval: 1000 });
        } else {
            graticuleZooms.push({ start: 0, end: 10, interval: 1000 });
        }
        L.latlngGraticule(
            {
                color: mapInfos.isSVG ? '#0071D6' : '#444',
                zoomInterval: graticuleZooms
            }).addTo(map);

        if (!fullScreen) {
            L.control.scale({ maxWidth: 200, imperial: false }).addTo(map);
            L.control.gridMousePosition().addTo(map);
        }

        $('#bg-opacity-minus').on('click', function () { changeOpacity(tileLayer, -0.1); });
        $('#bg-opacity-plus').on('click', function () { changeOpacity(tileLayer, +0.1); });

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

        if (currentTool == 6) {
            missionSelection = null;
            $('#mission-selector').modal('show');
        }
    };

    function createColorPicker(gameJson) {

        if (gameJson && gameJson.colors) {
            basicColors = {};
            gameJson.colors.forEach(color => { basicColors[color.name.toLowerCase()] = color.hexadecimal; });
        }
        
        var previousColor = Object.getOwnPropertyNames(basicColors)[0];
        var colorSelect = $('<select class="btn-maptool" data-container="body" id="color-tool" data-style="game-bg-' + previousColor + '"></select>');
        Object.getOwnPropertyNames(basicColors).forEach(function (color) {
            colorSelect.append($('<option></option>').attr({
                value: color,
                'data-content': "<div class='game-bg-" + color + "' style='width:20px; height:20px;'/>"
            }).addClass('game-bg-' + color));
        });
        colorSelect.on('change', function () {
            colorSelect.selectpicker('setStyle', 'game-bg-' + previousColor, 'remove');
            colorSelect.selectpicker('setStyle', 'game-bg-' + colorSelect.val(), 'add');
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
    }

    var Backend =
    {
        SignalR: {
            create: function (mapid, hub) {
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl(hub || "/MapHub")
                    .withAutomaticReconnect()
                    .build();
                this.mapid = mapid;
            },

            connect: function (callbacks) {
                this.connection.on("AddOrUpdateMarker", function (marker, isReadOnly) {
                    callbacks.addOrUpdateMarker(marker, isReadOnly === true);
                });
                this.connection.on("AddOrUpdateLayer", function (layer) {
                    callbacks.addOrUpdateLayer(layer);
                });
                this.connection.on("RemoveMarker", function (marker) {
                    callbacks.removeMarker(marker);
                });
                this.connection.on("RemoveLayer", function (layer) {
                    callbacks.removeLayer(layer);
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
            addMarker: function (layerId, markerData) {
                this.connection.invoke("AddMarkerToLayer", layerId, markerData);
            },
            removeMarker: function (markerId) {
                this.connection.invoke("RemoveMarker", markerId);
            },
            updateMarkerToLayer: function (markerId, layerId, markerData) {
                this.connection.invoke("UpdateMarkerToLayer", markerId, layerId, markerData);
            },
            moveMarker: function (markerId, markerData) {
                this.connection.invoke("MoveMarker", markerId, markerData);
            },
            addLayer: function (layerData) {
                this.connection.invoke("AddLayer", layerData);
            },
            updateLayer: function (layerId, layerData) {
                this.connection.invoke("UpdateLayer", layerId, layerData);
            },
            removeLayer: function (layerId) {
                this.connection.invoke("RemoveLayer", layerId);
            },
            pointMap: function (pos) {
                this.connection.invoke("PointMap", pos);
            },
            endPointMap: function () {
                this.connection.invoke("EndPointMap");
            },
            close: function () {
                this.connection.stop();
            }
        }
    };

    function setupEditTools(map, backend, gameJson) {

        tools = [
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 0); }, content: '<i class="far fa-hand-paper"></i>' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 1); }, content: '<i class="far fa-hand-pointer"></i>' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 2); }, content: '<img height="16" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEsAAAAzCAYAAADfP/VGAAACTElEQVRoge3ZsUsCcRTA8TdZaurZmJG2RtASURBI0BRELVG0BBFYi0tDkQSOQf0BTTW1FU01tQhBtNSWi8NFFNSc82soT9PTu/N+7/d+d+fwQLg70C8f7n73EwAg0hvbAxEAwN5Yjt4Sqz+WQC2VDvwkhkYwFI2ZxwrHk8aB1MQUbl7eY1HHQM7q6RVqw5lfPHGtNZaWSmPu5glHZ+aMaNn8IfsXlzkHL984uZ4zfv/44hrmSxXzWLWL5veOAqesUVM4kcSl4zMs6oiFcrVzrKKOgVFmpmn38d04bitWEJS109Q4jmL5UZmVJlex/KTMjiYhsbyszIkmYbG8qKxZ0/LJue1rhcTygrJuNZHEUlmZG02ksVRSJkITeSwVlInSJC0WhzLRmqTGkqmMQhNLLEpllJrYYlEoo9bEHkuEMlmalIjlRplMTUrFcqKMQ5Nysewo49KkbCwzZbM7+6yalI5lpgwAsC8aY9GkfKzmexP3O6aysczuTarsZCgTy+pJp8JOhhKxnDzpOJWxxup23cSljC2WiHWTbGXSY4lehctUJjUW5SpchjIpsWS901ErI4/F8U5HpYwsFvcOAYUyklgq7BBQKBMai1sTtTJhsf5p0gbZdwgolLmOpaomCmWuYnlBk0hlXcXymiZRyhzH8rImt8psx/KLJjfKbMXyo6ZulHWM5XdN7Wb79tlUWdtYQdJkV9nGxV1rrFB0wDhpbGEF86UKFsrVQM7W9QNmprPN/zTVY/XGcl5rsXQA+AKAt7/PvanPBwB8AoD+A4WfYoYlQx+dAAAAAElFTkSuQmCC" />' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 3); }, content: '●' }).addTo(map), // maybe ⬤ ?
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 4); }, content: '╱' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 5); }, content: '<i class="fas fa-ruler"></i>' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 6); }, content: '<i class="fas fa-plus-circle"></i>' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 7); }, content: '<i class="fas fa-sticky-note"></i>' }).addTo(map),
        ];

        colorPicker = createColorPicker(gameJson);
        L.control.overlayDiv({ content: colorPicker.get(0), position: 'topleft' }).addTo(map);

        milsymbolMarkerTool(backend);
        basicsymbolMarkerTool(backend);
        lineMarkerTool(backend);
        measureMarkerTool(backend);
        missionTool(map, backend);
        noteTool(map, backend);
        layersModal(backend);

        var hasOrbat = $('#orbat').length > 0;
        if (hasOrbat) {
            orbatMarkerTool(backend);
        }
        map.on('click', function (e) {
            clickPosition = e.latlng;
            if ((e.originalEvent.target.localName == "div" && !e.originalEvent.target.classList.contains("leaflet-tooltip")) || e.mapCanHandle) {
                clearpointEditMakers();
                if (currentTool == 2) {
                    if (hasOrbat) {
                        insertOrbat(e.latlng);
                    } else {
                        insertMilSymbol(e.latlng);
                    }
                }
                else if (currentTool == 3) {
                    insertBasicSymbol(e.latlng);
                }
                else if (currentTool == 4) {
                    insertLine(e.latlng, map, e.originalEvent.ctrlKey || e.originalEvent.shiftKey, backend);
                }
                else if (currentTool == 5) {
                    insertMeasure(e.latlng, map, backend);
                }
                else if (currentTool == 6) {
                    insertMission(e.latlng, map, backend);
                }
                else if (currentTool == 7) {
                    insertNote(e.latlng, map, backend);
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
            if (currentMission) {
                var points = missionSelection.points.slice();
                points.push([e.latlng.lat, e.latlng.lng]);
                var result = generateMission(missionSelection.mission,points,missionSelection.size);
                currentMission.setLatLngs(result.lines);
                if (result.labels) {
                    missionLabels(currentMission, result, null);
                }
            }
            if (currentMeasure) {
                var posList = currentMeasure.getLatLngs();
                posList[1] = e.latlng;
                currentMeasure.setLatLngs(posList);
                computeDistanceAndShowTooltip(map, currentMeasure, posList, false);
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
        map.on('contextmenu', function (e) {
            if (currentMeasure) { // un clic droit lors d'une mesure la supprime.
                currentMeasure.remove();
                currentMeasure = null;
            }
            if (currentLine) {
                var data = currentLine.getLatLngs().slice(0, -1);
                if (data.length > 1) {
                    currentLine.setLatLngs(data);
                } else {
                    currentLine.remove();
                    currentLine = null;
                }
            }
        });

        $('select').selectpicker();

        $('.modal-edit-points').on('click', _ => editMarkerPoints(modalMarker, map, backend));

        selectTool(map, 0);
    }

    function getHeading(p1, p2) {
        var dx = p2.lat - p1.lat;
        var dy = p2.lng - p1.lng;
        var heading = Math.round(Math.atan2(dy, dx) * 3200 / Math.PI);
        if (heading < 0) {
            heading = 6400 + heading;
        }
        return heading;
    }

    function computeDistanceAndShowTooltip(map, line, posList, interactive, markerId, markerData) {
        var distance = map.distance(posList[0], posList[1]).toFixed();
        var heading = getHeading(L.latLng(posList[0]), L.latLng(posList[1]));
        var formatedDistance = '<i class="fas fa-arrows-alt-h"></i> ' + intl.format(distance) + ' m<br/><i class="fas fa-compass"></i> ' + intl.format(heading) + ' mil';
        if (line.getTooltip()) {
            line.unbindTooltip();
        }
        line.bindTooltip(formatedDistance, { direction: 'center', permanent: true, interactive: interactive, opacity: 0.8, markerId: markerId, markerData: markerData });
    }

    function setupSearch(map, mapInfos, markers) {
        L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topright', click: function () { $('#search').modal('show'); search(map, mapInfos, markers); }, content: '<i class="fas fa-search"></i>' }).addTo(map);
        $('#search-term').on('keyup', function () { search(map, mapInfos, markers); });
    }

    function getOrCreateLayer(map, id, layers) {
        var layer = layers[id];
        if (!layer) {
            layers[id] = layer = { id: id, group: L.layerGroup().addTo(map) };
            updateLockAllState(layers);
        }
        return layer;
    }

    function updateMarkerState(layer, marker) {
        if (layer === currentLayer && !layer.isLocked) {
            if (marker.dragging) marker.dragging.enable();
            if (marker._icon) $(marker._icon).removeClass('disabled');
            if (marker._path) $(marker._path).removeClass('disabled');
            if (marker._tooltip) $(marker._tooltip._container).removeClass('disabled');
            marker.isDisabled = false;
        } else {
            if (marker.dragging) marker.dragging.disable();
            if (marker._icon) $(marker._icon).addClass('disabled');
            if (marker._path) $(marker._path).addClass('disabled');
            if (marker._tooltip) $(marker._tooltip._container).addClass('disabled');
            marker.isDisabled = true;
        }
    }

    function updateLayerState(layer) {
        layer.group.eachLayer(function (marker) {
            updateMarkerState(layer, marker);
        });
    }

    function updateAllLayersState(layers) {
        Object.getOwnPropertyNames(layers).forEach(function (id) {
            updateLayerState(layers[id]);
        });
    }

    function updateLockAllState(layers) {
        if (lockAllButton) {
            var allLocked = Object.getOwnPropertyNames(layers).every(id => layers[id].isLocked);
            if (allLocked) {
                lockAllButton.setClass('btn-primary');
                lockAllButton.j().find('i.fas').attr('class', 'fas fa-lock');
            } else {
                lockAllButton.setClass('btn-outline-secondary');
                lockAllButton.j().find('i.fas').attr('class', 'fas fa-lock-open');
            }
        }
    }

    function setCurrentLayer(layer, layers) {
        if (currentLayer !== layer) {
            if (currentLayer) {
                currentLayer.listItem.removeClass('active');
            }
            currentLayer = layer;
            currentLayer.listItem.addClass('active');
            updateAllLayersState(layers);
        }
    }

    function setLayerVisibility(layer, isVisible) {
        var i = $(layer.listItem).find('.layers-item-display').find('i.fas');
        layer.isHidden = !isVisible;
        if (layer.isHidden) {
            i.attr('class', 'fas fa-eye-slash');
            layer.group.remove();
        } else {
            i.attr('class', 'fas fa-eye');
            map.addLayer(layer.group);
        }
        layer.group.eachLayer(function (marker) {
            updateMarkerState(layer, marker);
        });
    }

    function updatePhaseSelectorActive(layers) {
        let allLayers = Object.getOwnPropertyNames(layers).map(id => layers[id]);
        getAllPhases(layers).forEach(phase => {
            let phaseItem = $('#phase-num-' + phase);
            let active = allLayers.every(l =>
                (!!l.isHidden) === !(
                    l.data.phase == phase || l.data.phase === null || l.data.phase === undefined
                )
            );
            if (active) {
                phaseItem.addClass('active');
            } else {
                phaseItem.removeClass('active');
            }
        });
    }

    function getAllPhases(layers) {
        var set = new Set(Object.getOwnPropertyNames(layers).map(id => layers[id].data.phase));
        set.delete(null);
        set.delete(undefined);
        return Array.from(set.values()).sort();
    }

    function setCurrentPhase(layers, phase) {
        let allLayers = Object.getOwnPropertyNames(layers).map(id => layers[id]);
        allLayers.forEach(layer => {
            setLayerVisibility(layer, layer.data.phase == phase || layer.data.phase === null || layer.data.phase === undefined);
        });
        updatePhaseSelectorActive(layers);
        let wantedLayer = allLayers.find(l => l.data.phase == phase);
        if (wantedLayer) {
            setCurrentLayer(wantedLayer, layers);
        }
    }

    function setupLayerUI(map, layer, layers) {
        layer.listItem.on('click', function () { setCurrentLayer(layer, layers); });

        layer.listItem.find('.layers-item-display').on('click', function () {
            setLayerVisibility(layer, layer.isHidden);
            updatePhaseSelectorActive(layers);
            return false;
        });

        layer.listItem.find('.layers-item-lock').on('click', function () {
            var i = $(this).find('i.fas');
            layer.isLocked = !layer.isLocked;
            if (layer.isLocked) {
                i.attr('class', 'fas fa-lock');
            } else {
                i.attr('class', 'fas fa-lock-open');
            }
            updateLayerState(layer);
            updateLockAllState(layers);
            return false;
        });

        layer.listItem.find('.layers-item-edit').on('click', function () {
            setCurrentLayer(layer, layers);
            $('#layer-label').val(layer.data.label);
            var isAll = layer.data.phase === null || layer.data.phase === undefined;
            $('#layer-phase-mode-all').prop("checked", isAll);
            $('#layer-phase-mode-specific').prop("checked", !isAll);
            $('#layer-phase-number').val(!isAll ? String(layer.data.phase) : "0");
            $('#layer-insert').hide();
            $('#layer-update').show();
            $('#layer').modal('show');
            return false;
        });

        layer.listItem.find('.layers-item-delete').on('click', function () {
            setCurrentLayer(layer, layers);
            $('#layer-delete-label').text(layer.data.label);
            $('#layer-delete').modal('show');
            return false;
        });
    }

    function updatePhaseSelectorItems(layers, phaseTemplate) {
        let phases = getAllPhases(layers);
        if (phases.length == 0) {
            $('#phase-selector').hide();
            $('.phase-item').remove();
        } else {
            let phaseItems = [];
            $('.phase-item').each((index, element) => {
                let phaseItem = $(element);
                let phase = Number(phaseItem.attr('data-phase'));
                if (!(phases.includes(phase))) {
                    phaseItem.remove();
                } else {
                    phaseItems.push({ number: phase, item: phaseItem });
                }
            });
            phases.forEach(phase => {
                let id = 'phase-num-' + phase;
                let phaseItem = $('#' + id);
                if (phaseItem.length == 0) {
                    phaseItem = phaseTemplate.clone().attr('id', id).attr('data-phase', phase);
                    phaseItem.find('a')
                        .on('click', (e) => { setCurrentPhase(layers, phase); e.preventDefault(); })
                        .text(String(phase));
                    let before = phaseItems.find(other => other.number > phase);
                    if (before) {
                        phaseItem.insertBefore(before.item);
                    } else {
                        $('#phase-selector').append(phaseItem);
                    }
                }
            });
            $('#phase-selector').show();
        }
        updatePhaseSelectorActive(layers);
    }

    function addOrUpdateLayer(map, layers, layerJson, layerTemplate, phaseTemplate) {
        var layer = getOrCreateLayer(map, layerJson.id, layers);
        layer.data = layerJson.data;
        layer.isDefaultLayer = layerJson.isDefaultLayer;
        if (!layer.listItem) {
            if (!layerJson.isDefaultLayer) {
                layer.listItem = layerTemplate.clone();
                $('#layers-list').append(layer.listItem);
                $('select.layers-dropdown').append($('<option />').attr('value', '' + layerJson.id));
            }
            else {
                layer.listItem = $('#layers-default');
            }
            setupLayerUI(map, layer, layers);
        }
        if (!layerJson.isDefaultLayer) {
            layer.listItem.find('.layers-item-label').text(layerJson.data.label);
            $('select.layers-dropdown option[value=' + layerJson.id + ']').text(layerJson.data.label);
            $('select.layers-dropdown').selectpicker('refresh');
        } else if (!currentLayer) {
            currentLayer = layer;
        }

        updatePhaseSelectorItems(layers, phaseTemplate);

        // TODO: Sort $('#layers-list')
        // TODO: Sort $('select.layers-dropdown')
    }

    function connect(map, backend, markers, pointing, canEdit, opacity, layers) {

        var pointingIcon = new L.icon({ iconUrl: '/img/pointmap.png', iconSize: [16, 16], iconAnchor: [8, 8] });

        var layerTemplate = $('#layers-template').remove();
        layerTemplate.removeAttr('id');

        var phaseTemplate = $('#phases-template').remove();
        phaseTemplate.removeAttr('id');

        backend.connect({
            addOrUpdateMarker: function (marker, isReadOnly) {
                addOrUpdateMarker(map, markers, marker, canEdit && !isReadOnly, backend, opacity, getOrCreateLayer(map, marker.layerId, layers));
            },
            addOrUpdateLayer: function (layerJson) {
                addOrUpdateLayer(map, layers, layerJson, layerTemplate, phaseTemplate);
            },
            removeMarker: function (marker) {
                var existing = markers[marker.id];
                if (existing) {
                    existing.remove();
                    existing.removeFrom(getOrCreateLayer(map, marker.layerId, layers).group);
                    delete markers[marker.id];
                }
            },
            removeLayer: function (layer) {
                var existing = layers[layer.id];
                if (existing) {
                    existing.group.remove();
                    if (existing.listItem) existing.listItem.remove();
                    delete layers[layer.id];
                    if (existing === currentLayer) {
                        setCurrentLayer(Object.getOwnPropertyNames(layers).map(n => layers[n])[0], layers);
                    }
                    updateLockAllState(layers);
                }
            },
            pointMap: function (id, pos) {
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
    }

    function setupLayerToggle(map) {
        var isLayersToolboxVisible = false;
        var layersBtn = L.control.overlayButton({
            baseClassName: 'btn btn-maptool', position: 'topright', click: function () {
                isLayersToolboxVisible = !isLayersToolboxVisible;
                if (isLayersToolboxVisible) {
                    $('#layers-col').attr('class', 'col tacmap-sidebar pl-2');
                    layersBtn.setClass('btn-primary');
                    history.replaceState({}, '', '#showLayers');
                }
                else {
                    $('#layers-col').attr('class', 'd-none');
                    layersBtn.setClass('btn-outline-secondary');
                    history.replaceState({}, '', '#');
                }
            }, content: '<i class="fas fa-layer-group"></i>'
        }).addTo(map);

        if (location.hash.indexOf('showLayers') != -1) {
            isLayersToolboxVisible = true;
            $('#layers-col').attr('class', 'col tacmap-sidebar pl-2');
            layersBtn.setClass('btn-primary');
        }
    }

    function setupLockAll(map, layers) {
        lockAllButton = L.control.overlayButton({
            baseClassName: 'btn btn-maptool', position: 'topright', click: function () {
                var layersList = Object.getOwnPropertyNames(layers).map(id => layers[id]);
                var allWasLocked = layersList.every(l => l.isLocked);
                layersList.forEach(l => { l.isLocked = !allWasLocked; });
                updateAllLayersState(layers);
                updateLockAllState(layers);
                if (!allWasLocked) {
                    $('.layers-item-lock i.fas').attr('class', 'fas fa-lock');
                } else {
                    $('.layers-item-lock i.fas').attr('class', 'fas fa-lock-open');
                }
            }, content: '<i class="fas fa-lock-open"></i>'
        }).addTo(map);
    }

    var defaultOpacity = { 'mil': 1.0, 'basic': 1.0, 'line': 1.0, 'measure': 1.0 };

    /**
     * Displays, in real time, a tacmap on a preconfigured Leaflet map.
     * @param {L.map} map
     * @param {string} hub
     * @param {any} mapId
     * @param {any} opacity
     */
    Arma3TacMap.connnectReadOnlyMap = function (map, hub, mapId, opacity) {
        var markers = {};
        var pointing = {};
        var backend = Backend.SignalR;
        var opacity = opacity ?? defaultOpacity;
        backend.create(mapId, hub);
        connect(map, backend, markers, pointing, false, opacity, {});

        return {
            close: function () {
                backend.close();
                Object.getOwnPropertyNames(markers).forEach(id => {
                    markers[id].remove();
                });
                Object.getOwnPropertyNames(pointing).forEach(id => {
                    pointing[id].remove();
                });
            }
        };
    };


    function getMapInfos(config) {
        var worldName = config.worldName;
        var view = config.view;

        if (view == 'topo' && Arma3Map.TopoMaps) {
            var topo = Arma3Map.TopoMaps[worldName];
            if (topo) {
                topo.CRS = MGRS_CRS(topo.crsFactor.x, topo.crsFactor.y, topo.tileSize);
                topo.center = [topo.center.y, topo.center.x];
                topo.defaultZoom = 1;
                topo.attribution = topo.credits;
                topo.cities = topo.cities || [];
                topo.tilePattern = '/topo/' + topo.worldName + '/' + topo.tilePattern;
                topo.isSVG = true;
                return topo;
            }
        }
        return Arma3Map.Maps[worldName || 'altis'] || Arma3Map.Maps.altis;
    }

    Arma3TacMap.initLiveMap = function (config) {
        $(function () {
            var mapId = config.mapId;
            var canEdit = !config.isReadOnly;

            var mapInfos = getMapInfos(config);

            var map = initMapArea(mapInfos, config.endpoint);

            setupLayerToggle(map);

            var markers = {};
            var layers = {};
            var pointing = {};
            var backend = Backend.SignalR;

            setupLockAll(map,layers);

            backend.create(mapId, config.hub);

            if ($('#share').length) {
                L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topright', click: function () { $('#share').modal('show'); }, content: '<i class="fas fa-share-square"></i>' }).addTo(map);
            }
            setupSearch(map, mapInfos, markers);

            connect(map, backend, markers, pointing, canEdit, defaultOpacity, layers);

            if (canEdit) {
                setupEditTools(map, backend, config.game);
            }
        });
    };


    Arma3TacMap.initStaticMap = function (config) {
        $(function () {
            var mapInfos = getMapInfos(config);
            var markers = {};

            var map = initMapArea(mapInfos, config.endpoint, config.center, config.fullScreen);

            if ($('#search').length) {
                setupSearch(map, mapInfos, markers);
            }

            var uniqueLayer = { id: -1, group: L.layerGroup().addTo(map) };
            Object.getOwnPropertyNames(config.markers).forEach(function (id) {
                addOrUpdateMarker(map, markers, { id: id, data: config.markers[id] }, false, null, defaultOpacity, uniqueLayer);
            });
        });
    }

})(Arma3TacMap = Arma3TacMap || {});

