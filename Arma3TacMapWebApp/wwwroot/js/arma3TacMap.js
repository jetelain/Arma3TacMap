var arma3TacMapLoaded = false;

var Arma3TacMap;
(function (Arma3TacMap) {

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
    var currentMeasure = null;
    var currentLayer = null;
    var currentMission = null;
    var missionSelection = null;
    var colorPicker;

    function getCurrentLayerId() {
        return currentLayer ? currentLayer.id : null;
    }

    function updateMarkerHandler(e) {
        var marker = e.sourceTarget;
        if (marker.isDisabled) {
            return;
        }
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
        } else if (modalMarkerData.type == 'measure') {
            $('#measure').modal('show');
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

    function orbatMarkerTool(backend) {
        $('.orbat-btn').on('click', function () {
            
            backend.addMarker({
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

    function measureMarkerTool(backend) {
        $('#measure-delete').on('click', function () {
            backend.removeMarker(modalMarkerId);
            $('#measure').modal('hide');
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


    function insertMeasure(latlng, map, backend) {
        if (!currentMeasure) {
            currentMeasure = L.polyline([latlng, latlng], { color: '#000000', weight: 1.3, dashArray: '4', interactive: false }).addTo(map);
        }
        else {
            var data = currentMeasure.getLatLngs();
            currentMeasure.remove();
            currentMeasure = null;
            backend.addMarker({
                type: 'measure',
                symbol: 'measure',
                config: {},
                pos: data.map(function (e) { return [e.lat, e.lng]; }).flat()
            });
        }
    }

    function normalVector(a, b) {
        var dx = b[0] - a[0];
        var dy = b[1] - a[1];
        var l = Math.sqrt((dx * dx) + (dy * dy));
        return [dx / l, dy / l];
    }

    function simpleArrow(a, b, arrowScale, n) {
        if ( !n ) n = normalVector(a, b);
        var x1 = [n[0] * -0.7071068 + n[1] * 0.7071068, n[0] * -0.7071068 + n[1] * -0.7071068];  // +3/4 PI
        var x2 = [n[0] * -0.7071068 + n[1] * -0.7071068, n[0] * 0.70710682 + n[1] * -0.7071068]; // -3/4 PI
        return [
            [a, b],
            [b, [b[0] + (x1[0] * arrowScale), b[1] + (x1[1] * arrowScale)]],
            [b, [b[0] + (x2[0] * arrowScale), b[1] + (x2[1] * arrowScale)]]
        ];
    }

    function brokenArrow(a, b, arrowScale) {
        var c = [(b[0] + a[0]) / 2, (b[1] + a[1]) / 2];
        var n = normalVector(a, b);
        var x3 = [n[0] * 0.9238795 + n[1] * 0.3826835, n[0] * -0.3826835 + n[1] * 0.9238795];   // +1/8 PI
        var x4 = [n[0] * -0.9238795 + n[1] * -0.3826835, n[0] * 0.3826835 + n[1] * -0.9238795]; // -1/8 PI
        var c1 = [c[0] + (x3[0] * arrowScale * 2), c[1] + (x3[1] * arrowScale * 2)];
        var c2 = [c[0] + (x4[0] * arrowScale * 2), c[1] + (x4[1] * arrowScale * 2)];
        return [
            [a, c1],
            [c1, c2]
        ].concat(simpleArrow(c2, b, arrowScale, n));
    }

    function innerBox(a, b, scale) {
        var n = normalVector(a, b);
        n = [n[0] * scale, n[1] * scale];
        var p1 = [a[0] + n[0] - n[1], a[1] + n[1] + n[0]];
        var p4 = [a[0] + n[0] + n[1], a[1] + n[1] - n[0]];
        var p2 = [b[0] - n[0] - n[1], b[1] - n[1] + n[0]];
        var p3 = [b[0] - n[0] + n[1], b[1] - n[1] - n[0]];
        return [
            [p1, p2, p3, p4, p1]
        ];
    }
    function upperLabels(a, b, scale) {
        var n = normalVector(a, b);
        var x2 = [n[0] * 0.7071068 + n[1] * 0.7071068, n[0] * -0.7071068 + n[1] * 0.7071068]; // 1/4 TI
        var x1 = [n[0] * -0.7071068 + n[1] * 0.7071068, n[0] * -0.70710682 + n[1] * -0.7071068]; // 3/4 PI
        return [[a[0] + (x1[0] * scale * 2), a[1] + (x1[1] * scale * 2)],
                [b[0] + (x2[0] * scale * 2), b[1] + (x2[1] * scale * 2)]];
    }

    function centerLabelForBroken(a, b, scale, lines) {
        var n = normalVector(a, b);
        var p = lines[0][1];
        return [[p[0] + (n[1] * scale), p[1] + (n[0] * scale)]];
    }

    function centerLabelForRegular(a, b, scale, lines) {
        var c = [(b[0] + a[0]) / 2, (b[1] + a[1]) / 2];
        var n = normalVector(a, b);
        return [[c[0] + (n[1] * scale),c[1] + (n[0] * scale)]];
    }

    function curvedLine(a, b, clipDistance) {
        var dx = b[0] - a[0];
        var dy = b[1] - a[1];
        var distance = Math.sqrt((dx * dx) + (dy * dy));
        var vectM45 = [(dx / distance) * 0.7071068 + (dy / distance) * -0.7071068, (dx / distance) * 0.7071068 + (dy / distance) * 0.7071068];
        var radius = distance / Math.sqrt(2);
        var center = [a[0] + (vectM45[0] * radius), a[1] + (vectM45[1] * radius)];
        var baseAngle = Math.atan2(dy, dx) + (5 * Math.PI / 4);
        var line = [];
        var clip = clipDistance * clipDistance;
        for (var angle = 0; angle <= Math.PI / 2; angle += Math.PI / 40) {
            var p = [center[0] + (Math.cos(angle + baseAngle) * radius), center[1] + (Math.sin(angle + baseAngle) * radius)];
            var px = p[0] - a[0];
            var py = p[1] - a[1];
            var pdistsq = px * px + py * py;
            if (pdistsq >= clip) {
                if (line.length == 0) {
                    line.push([a[0] + (px / Math.sqrt(pdistsq) * clipDistance),a[1] + (py / Math.sqrt(pdistsq) * clipDistance)]);
                }
                line.push(p);
            }
        }
        return line;
    }

    var missionsDefiniton = {
        toSurvey: {
            points: 4,
            generate:
                function (points, scale) {
                    return innerBox(points[0], points[1], scale / 3)
                        .concat(
                            brokenArrow(points[0], points[2], scale / 3),
                            brokenArrow(points[1], points[3], scale / 3));;
                },
            labels: ['S', 'S'],
            generateLabels:
                function (points, scale) {
                    return upperLabels(points[0], points[1], scale / 2);
                },
        },
        toCover: {
            points: 4,
            generate:
                function (points, scale) {
                    return innerBox(points[0], points[1], scale / 3)
                        .concat(
                            brokenArrow(points[0], points[2], scale / 3),
                            brokenArrow(points[1], points[3], scale / 3));;
                },
            labels: ['C', 'C'],
            generateLabels:
                function (points, scale) {
                    return upperLabels(points[0], points[1], scale / 2);
                },
        },
        toSupportByFire: {
            points: 4,
            generate:
                function (points, scale) {
                    var n = normalVector(points[0], points[1]);
                    var x2 = [n[0] * 0.7071068 + n[1] * 0.7071068, n[0] * -0.7071068 + n[1] * 0.7071068]; // 1/4 TI
                    var x1 = [n[0] * -0.7071068 + n[1] * 0.7071068, n[0] * -0.70710682 + n[1] * -0.7071068]; // 3/4 PI
                    var xa = [points[0][0] + (x1[0] * scale * 2), points[0][1] + (x1[1] * scale * 2)];
                    var xb = [points[1][0] + (x2[0] * scale * 2), points[1][1] + (x2[1] * scale * 2)];
                    return [
                        [points[0], points[1]],
                        [xa, points[0]],
                        [xb, points[1]]]
                        .concat(
                            simpleArrow(points[0], points[2], scale),
                            simpleArrow(points[1], points[3], scale));
                },
        },
        toRecce: {
            points: 2,
            generate:
                function (points, scale) {
                    return brokenArrow(points[0], points[1], scale);
                },
            labels: ['RECO'],
            generateLabels:
                function (points, scale, lines) {
                    return centerLabelForBroken(points[0], points[1], scale / 2, lines);
                },
        },
        toScout: {
            points: 2,
            generate:
                function (points, scale) {
                    return brokenArrow(points[0], points[1], scale);
                },
            labels: ['ECL'],
            generateLabels:
                function (points, scale, lines) {
                    return centerLabelForBroken(points[0], points[1], scale / 2, lines);
                },
        },
        toSeize: {
            points: 2,
            generate:
                function (points, scale) {
                    var a = points[0];
                    var b = points[1];
                    // Circle around start position
                    var circle = [];
                    for (var angle = 0; angle <= 2 * Math.PI; angle += Math.PI / 20) {
                        circle.push([a[0] + (Math.cos(angle) * scale * 2), a[1] + (Math.sin(angle) * scale * 2)]);
                    }
                    // Curved line from start to target
                    var line = curvedLine(a, b, scale * 2);

                    // Arrow to target (last segment of curved line)
                    if (line.length > 2) {
                        var arrow = simpleArrow(line[line.length - 2], line[line.length - 1], scale);
                        arrow.splice(0, 1);
                        return [circle, line, arrow];
                    }
                    return [circle];
                },
        },
        toSupport: {
            points: 2,
            generate: /* Simple arrow */
                function (points, scale) {
                    return simpleArrow(points[0], points[1], scale);
                },
            labels: ['APP'],
            generateLabels:
                function (points, scale) {
                    return centerLabelForRegular(points[0], points[1], scale / 2);
                },
        },
        toMakeAndIdentifyContact: {
            points: 2,
            generate:
                function () {
                    return [];
                },
        },
        toDestroy: {
            points: 1,
            generate:
                function () {
                    return [];
                },
        },
        toDefend: {
            points: 1,
            generate:
                function () {
                    return [];
                },
        }
    };

    function generateMission(mission, points, size) {
        var def = missionsDefiniton[mission];
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
                points = points.slice();
                var n = normalVector(points[0], points[1]);
                var x1 = [n[0] * -0.7071068 + n[1] * -0.7071068, n[0] * 0.7071068 + n[1] * -0.7071068]; // -3/4 PI
                var x2 = [n[0] * 0.7071068 + n[1] * -0.7071068, n[0] * 0.70710682 + n[1] * 0.7071068]; // -1/4 PI
                if (points.length < 3) {
                    points.push([points[0][0] + (x1[0] * sizeMeters * 2), points[0][1] + (x1[1] * sizeMeters * 2)]);
                }
                points.push([points[1][0] + (x2[0] * sizeMeters * 2), points[1][1] + (x2[1] * sizeMeters * 2)]);
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
                    })
                }).addTo(target);
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
        if (missionsDefiniton[missionSelection.mission].points == missionSelection.points.length) {
            if (currentMission) {
                currentMission.remove();
                currentMission = null;
            }
            backend.addMarker({
                type: 'mission',
                symbol: missionSelection.mission,
                config: { size: missionSelection.size },
                pos: missionSelection.points.flat()
            });
            missionSelection = null;
        } else {
            var result = generateMission(missionSelection.mission, missionSelection.points, missionSelection.size);
            if (!currentMission) {
                currentMission = L.polyline(result.lines, { color: '#000', weight: 3, interactive: false }).addTo(map);
            } else {
                currentMission.setLatLngs(result.lines);
            }
            if (result.labels) {
                missionLabels(currentMission, result, map);
            }
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

    function addOrUpdateMarker(map, markers, marker, canEdit, backend, opacity, layer) {
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
                var mapMarker = L.polyline(posList, { color: color, weight: 3, interactive: canEdit, markerId: markerId, markerData: markerData, opacity: opacity['line'] ?? 1.0 }).addTo(layer.group);
                if (canEdit) {
                    mapMarker.on('click', updateMarkerHandler);
                }
                markers[markerId] = existing = mapMarker;
            }
        }
        else if (markerData.type == 'measure') {
            var posList = [[markerData.pos[0], markerData.pos[1]],
            [markerData.pos[2], markerData.pos[3]]];

            if (existing) {
                existing.setLatLngs(posList);
                computeDistanceAndShowTooltip(map, existing, posList, canEdit);
                existing.options.markerData = markerData;
            } else {
                var mapMarker = L.polyline(posList, { color: '#000000', weight: 1.3, dashArray: '4', interactive: canEdit, markerId: markerId, markerData: markerData, opacity: opacity['measure'] ?? 1.0 }).addTo(layer.group);
                computeDistanceAndShowTooltip(map, mapMarker, posList, canEdit);
                if (canEdit) {
                    mapMarker.on('click', updateMarkerHandler);
                }
                markers[markerId] = existing = mapMarker;
            }
        }
        else if (markerData.type == 'mission') {
            var points = [];
            for (var i = 0; i < markerData.pos.length; i += 2) {
                points.push([markerData.pos[i], markerData.pos[i + 1]]);
            }
            var result = generateMission(markerData.symbol, points, markerData.config['size'] || '13');
            if (existing) {
                existing.setLatLngs(result.lines);
                existing.options.markerData = markerData;
            } else {
                var mapMarker = L.polyline(result.lines, { color: '#000000', weight: 3, interactive: canEdit, markerId: markerId, markerData: markerData, opacity: opacity['line'] ?? 1.0 }).addTo(layer.group);
                markers[markerId] = existing = mapMarker;
            }
            if (result.labels) {
                missionLabels(existing, result, layer.group);
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
                        .on('click', updateMarkerHandler)
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


        updateMarkerState(layer, existing);

    }

    function initMapArea(mapInfos, endpoint, center, fullScreen) {
        var map = L.map('map', {
            minZoom: mapInfos.minZoom,
            maxZoom: mapInfos.maxZoom,
            crs: mapInfos.CRS,
            zoomSnap: fullScreen ? 0.01 : 1,
            zoomControl: !fullScreen
        });
        L.tileLayer((endpoint || 'https://jetelain.github.io/Arma3Map') + mapInfos.tilePattern, {
            attribution: mapInfos.attribution,
            tileSize: mapInfos.tileSize
        })
            .on('load', function (event) { arma3TacMapLoaded = true })
            .addTo(map);

        if (fullScreen) {
            map.fitBounds([[0, 0], [mapInfos.worldSize, mapInfos.worldSize]]);
        } else if (center) {
            map.setView(center, mapInfos.maxZoom);
        } else {
            map.setView(mapInfos.center, mapInfos.defaultZoom);
        }
        L.latlngGraticule({ zoomInterval: [{ start: 0, end: 10, interval: 1000 }] }).addTo(map);
        if (!fullScreen) {
            L.control.scale({ maxWidth: 200, imperial: false }).addTo(map);
            L.control.gridMousePosition().addTo(map);
        }
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
            addMarker: function (markerData) {
                this.connection.invoke("AddMarkerToLayer", getCurrentLayerId(), markerData); // XXX: Move layerId to caller ?
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

    function setupEditTools(map, backend) {

        tools = [
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 0); }, content: '<i class="far fa-hand-paper"></i>' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 1); }, content: '<i class="far fa-hand-pointer"></i>' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 2); }, content: '<img height="16" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEsAAAAzCAYAAADfP/VGAAACTElEQVRoge3ZsUsCcRTA8TdZaurZmJG2RtASURBI0BRELVG0BBFYi0tDkQSOQf0BTTW1FU01tQhBtNSWi8NFFNSc82soT9PTu/N+7/d+d+fwQLg70C8f7n73EwAg0hvbAxEAwN5Yjt4Sqz+WQC2VDvwkhkYwFI2ZxwrHk8aB1MQUbl7eY1HHQM7q6RVqw5lfPHGtNZaWSmPu5glHZ+aMaNn8IfsXlzkHL984uZ4zfv/44hrmSxXzWLWL5veOAqesUVM4kcSl4zMs6oiFcrVzrKKOgVFmpmn38d04bitWEJS109Q4jmL5UZmVJlex/KTMjiYhsbyszIkmYbG8qKxZ0/LJue1rhcTygrJuNZHEUlmZG02ksVRSJkITeSwVlInSJC0WhzLRmqTGkqmMQhNLLEpllJrYYlEoo9bEHkuEMlmalIjlRplMTUrFcqKMQ5Nysewo49KkbCwzZbM7+6yalI5lpgwAsC8aY9GkfKzmexP3O6aysczuTarsZCgTy+pJp8JOhhKxnDzpOJWxxup23cSljC2WiHWTbGXSY4lehctUJjUW5SpchjIpsWS901ErI4/F8U5HpYwsFvcOAYUyklgq7BBQKBMai1sTtTJhsf5p0gbZdwgolLmOpaomCmWuYnlBk0hlXcXymiZRyhzH8rImt8psx/KLJjfKbMXyo6ZulHWM5XdN7Wb79tlUWdtYQdJkV9nGxV1rrFB0wDhpbGEF86UKFsrVQM7W9QNmprPN/zTVY/XGcl5rsXQA+AKAt7/PvanPBwB8AoD+A4WfYoYlQx+dAAAAAElFTkSuQmCC" />' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 3); }, content: '●' }).addTo(map), // maybe ⬤ ?
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 4); }, content: '╱' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 5); }, content: '<i class="fas fa-ruler"></i>' }).addTo(map),
            L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topleft', click: function () { selectTool(map, 6); }, content: 'Mission' }).addTo(map)
        ];

        colorPicker = createColorPicker();
        L.control.overlayDiv({ content: colorPicker.get(0), position: 'topleft' }).addTo(map);

        milsymbolMarkerTool(backend);

        basicsymbolMarkerTool(backend);

        lineMarkerTool(backend);

        measureMarkerTool(backend);

        var hasOrbat = $('#orbat').length > 0;
        if (hasOrbat) {
            orbatMarkerTool(backend);
        }
        map.on('click', function (e) {
            clickPosition = e.latlng;
            if (e.originalEvent.target.localName == "div") {
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
                    insertLine(e.latlng, map, e.originalEvent.ctrlKey, backend);
                }
                else if (currentTool == 5) {
                    insertMeasure(e.latlng, map, backend);
                }
                else if (currentTool == 6) {
                    insertMission(e.latlng, map, backend);
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
        });

        $('select').selectpicker();

        selectTool(map, 0);

        $('#layers-add').on('click', function () {
            $('#layer-label').val('');
            $('#layer-insert').show();
            $('#layer-update').hide();
            $('#layer').modal('show');
            return false;
        });

        $('#layer-insert').on('click', function () {
            backend.addLayer({ label: $('#layer-label').val() });
            $('#layer').modal('hide');
            return false;
        });

        $('#layer-update').on('click', function () {
            backend.updateLayer(currentLayer.id, { label: $('#layer-label').val() });
            $('#layer').modal('hide');
            return false;
        });

        $('#layer-delete-confirm').on('click', function () {
            backend.removeLayer(currentLayer.id);
            $('#layer-delete').modal('hide');
            return false;
        });

        $('.mission-btn').on('click', function () {
            missionSelection = {
                mission: $(this).attr('data-mission'),
                size: $('#mission-selector-size').val()
            };
            $('#mission-selector').modal('hide');
            return false;
        });

    }

    function computeDistanceAndShowTooltip(map, line, posList, interactive) {
        var distance = map.distance(posList[0], posList[1]).toFixed();
        var formatedDistance = new Intl.NumberFormat().format(distance) + ' m';

        if (line.getTooltip()) {
            line.unbindTooltip();
        }
        line.bindTooltip(formatedDistance, { direction: 'center', permanent: true, interactive: interactive, opacity: 0.8 });
    }


    function setupSearch(map, mapInfos, markers) {
        L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topright', click: function () { $('#search').modal('show'); search(map, mapInfos, markers); }, content: '<i class="fas fa-search"></i>' }).addTo(map);
        $('#search-term').on('keyup', function () { search(map, mapInfos, markers); });
    }

    function getOrCreateLayer(map, id, layers) {
        var layer = layers[id];
        if (!layer) {
            layers[id] = layer = { id: id, group: L.layerGroup().addTo(map) };
        }
        return layer;
    }

    function updateMarkerState(layer, marker) {
        if (layer === currentLayer) {
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

    function setCurrentLayer(layer, layers) {
        if (currentLayer !== layer) {
            if (currentLayer) {
                currentLayer.listItem.removeClass('active');
            }
            currentLayer = layer;
            currentLayer.listItem.addClass('active');

            Object.getOwnPropertyNames(layers).forEach(function (id) {
                var other = layers[id];
                other.group.eachLayer(function (marker) {
                    updateMarkerState(other, marker);
                });
            });
        }
    }

    function setupLayerUI(map, layer, layers) {
        layer.listItem.on('click', function () { setCurrentLayer(layer, layers); });

        layer.listItem.find('.layers-item-display').on('click', function () {
            var i = $(this).find('i.fas');
            layer.isHidden = !layer.isHidden;
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
            return false;
        });

        layer.listItem.find('.layers-item-edit').on('click', function () {
            setCurrentLayer(layer, layers);
            $('#layer-label').val(layer.data.label);
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

    function addOrUpdateLayer(map, layers, layerJson, layerTemplate) {
        var layer = getOrCreateLayer(map, layerJson.id, layers);
        layer.data = layerJson.data;
        layer.isDefaultLayer = layerJson.isDefaultLayer;
        if (!layer.listItem) {
            if (!layerJson.isDefaultLayer) {
                layer.listItem = layerTemplate.clone();
                $('#layers-list').append(layer.listItem);
            }
            else {
                layer.listItem = $('#layers-default');
            }
            setupLayerUI(map, layer, layers);
        }
        if (!layerJson.isDefaultLayer) {
            layer.listItem.find('.layers-item-label').text(layerJson.data.label);
        } else if (!currentLayer) {
            currentLayer = layer;
        }
    }

    function connect(map, backend, markers, pointing, canEdit, opacity, layers) {

        var pointingIcon = new L.icon({ iconUrl: '/img/pointmap.png', iconSize: [16, 16], iconAnchor: [8, 8] });

        var layerTemplate = $('#layers-template').remove();
        layerTemplate.removeAttr('id');

        backend.connect({
            addOrUpdateMarker: function (marker, isReadOnly) {
                addOrUpdateMarker(map, markers, marker, canEdit && !isReadOnly, backend, opacity, getOrCreateLayer(map, marker.layerId, layers));
            },
            addOrUpdateLayer: function (layerJson) {
                addOrUpdateLayer(map, layers, layerJson, layerTemplate);
            },
            removeMarker: function (marker) {
                var existing = markers[marker.id];
                if (existing) {
                    existing.remove();
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


    Arma3TacMap.initLiveMap = function (config) {
        $(function () {
            var worldName = config.worldName;
            var mapId = config.mapId;
            var canEdit = !config.isReadOnly;


            var mapInfos = Arma3Map.Maps[worldName || 'altis'] || Arma3Map.Maps.altis;

            var map = initMapArea(mapInfos, config.endpoint);

            setupLayerToggle(map);

            if ($('#share').length) {
                L.control.overlayButton({ baseClassName: 'btn btn-maptool', position: 'topright', click: function () { $('#share').modal('show'); }, content: '<i class="fas fa-share-square"></i>' }).addTo(map);
            }

            var markers = {};
            var layers = {};
            var pointing = {};
            var backend = Backend.SignalR;
            backend.create(mapId, config.hub);

            setupSearch(map, mapInfos, markers);

            connect(map, backend, markers, pointing, canEdit, defaultOpacity, layers);

            if (canEdit) {
                setupEditTools(map, backend);
            }
        });
    };


    Arma3TacMap.initStaticMap = function (config) {
        $(function () {
            var worldName = config.worldName;
            var mapInfos = Arma3Map.Maps[worldName || 'altis'] || Arma3Map.Maps.altis;
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

