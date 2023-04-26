var MilMissions;
(function (MilMissions) {

    function normalVector(a, b) {
        var dx = b[0] - a[0];
        var dy = b[1] - a[1];
        var l = Math.sqrt((dx * dx) + (dy * dy));
        return [dx / l, dy / l];
    }

    function simpleArrow(a, b, arrowScale, n) {
        if (!n) n = normalVector(a, b);
        var x1 = [n[0] * -0.7071068 + n[1] * 0.7071068, n[0] * -0.7071068 + n[1] * -0.7071068];  // +3/4 PI
        var x2 = [n[0] * -0.7071068 + n[1] * -0.7071068, n[0] * 0.70710682 + n[1] * -0.7071068]; // -3/4 PI
        return [
            [a, b],
            [b, [b[0] + (x1[0] * arrowScale), b[1] + (x1[1] * arrowScale)]],
            [b, [b[0] + (x2[0] * arrowScale), b[1] + (x2[1] * arrowScale)]]
        ];
    }

    function brokenArrow(a, b, arrowScale, reverse) {
        var c = [(b[0] + a[0]) / 2, (b[1] + a[1]) / 2];
        var n = normalVector(a, b);
        var x3, x4;
        if (reverse) {
            x3 = [n[0] * 0.9238795 + n[1] * -0.3826835, n[0] * 0.3826835 + n[1] * 0.9238795];   // -1/8 PI 
            x4 = [n[0] * -0.9238795 + n[1] * 0.3826835, n[0] * -0.3826835 + n[1] * -0.9238795]; // +7/8 PI
        } else {
            x3 = [n[0] * 0.9238795 + n[1] * 0.3826835, n[0] * -0.3826835 + n[1] * 0.9238795];   // +1/8 PI
            x4 = [n[0] * -0.9238795 + n[1] * -0.3826835, n[0] * 0.3826835 + n[1] * -0.9238795]; // -7/8 PI
        }
        var c1 = [c[0] + (x3[0] * arrowScale * 2), c[1] + (x3[1] * arrowScale * 2)];
        var c2 = [c[0] + (x4[0] * arrowScale * 2), c[1] + (x4[1] * arrowScale * 2)];
        return [
            [a, c1],
            [c1, c2]
        ].concat(simpleArrow(c2, b, arrowScale, n));
    }
    function upperLabels(a, b, scale) {
        var n = normalVector(a, b);
        var x2 = [n[0] * 0.7071068 + n[1] * 0.7071068, n[0] * -0.7071068 + n[1] * 0.7071068]; // 1/4 PI
        var x1 = [n[0] * -0.7071068 + n[1] * 0.7071068, n[0] * -0.70710682 + n[1] * -0.7071068]; // 3/4 PI
        return [[a[0] + (x1[0] * scale * 2), a[1] + (x1[1] * scale * 2)],
        [b[0] + (x2[0] * scale * 2), b[1] + (x2[1] * scale * 2)]];
    }

    function centerLabelForBroken(a, b, scale, lines) {
        var n = normalVector(a, b);
        var p = lines[0][1];
        return [[p[0] + (n[1] * scale), p[1] + (n[0] * scale)]];
    }

    function centerLabelForRegular(a, b, scale) {
        var c = [(b[0] + a[0]) / 2, (b[1] + a[1]) / 2];
        var n = normalVector(a, b);
        return [[c[0] + (n[1] * scale), c[1] + (n[0] * scale)]];
    }

    function curvedLineL(a, b, clipDistance) {
        var dx = b[0] - a[0];
        var dy = b[1] - a[1];
        var distance = Math.sqrt((dx * dx) + (dy * dy));
        var vectM45 = [(dx / distance) * 0.7071068 + (dy / distance) * -0.7071068, (dx / distance) * 0.7071068 + (dy / distance) * 0.7071068]; // -1/4 PI
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
                    line.push([a[0] + (px / Math.sqrt(pdistsq) * clipDistance), a[1] + (py / Math.sqrt(pdistsq) * clipDistance)]);
                }
                line.push(p);
            }
        }
        return line;
    }

    function curvedLineR(a, b, clipDistance) {
        var dx = b[0] - a[0];
        var dy = b[1] - a[1];
        var distance = Math.sqrt((dx * dx) + (dy * dy));
        var vectM45 = [(dx / distance) * 0.7071068 + (dy / distance) * 0.7071068, (dx / distance) * -0.7071068 + (dy / distance) * 0.7071068]; // +1/4 PI
        var radius = distance / Math.sqrt(2);
        var center = [a[0] + (vectM45[0] * radius), a[1] + (vectM45[1] * radius)];
        var baseAngle = Math.atan2(dy, dx) + (1 * Math.PI / 4);
        var line = [];
        var clip = clipDistance * clipDistance;
        for (var angle = Math.PI / 2; angle >= 0; angle -= Math.PI / 40) {
            var p = [center[0] + (Math.cos(angle + baseAngle) * radius), center[1] + (Math.sin(angle + baseAngle) * radius)];
            var px = p[0] - a[0];
            var py = p[1] - a[1];
            var pdistsq = px * px + py * py;
            if (pdistsq >= clip) {
                if (line.length == 0) {
                    line.push([a[0] + (px / Math.sqrt(pdistsq) * clipDistance), a[1] + (py / Math.sqrt(pdistsq) * clipDistance)]);
                }
                line.push(p);
            }
        }
        return line;
    }

    MilMissions.missions = {
        toSurvey: {
            points: 4,
            generate:
                function (points, scale) {
                    return brokenArrow(points[0], points[2], scale / 3)
                        .concat(brokenArrow(points[1], points[3], scale / 3, true));
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
                    return brokenArrow(points[0], points[2], scale / 3)
                        .concat(brokenArrow(points[1], points[3], scale / 3, true));
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
                    var line = curvedLineL(a, b, scale * 2);

                    // Arrow to target (last segment of curved line)
                    if (line.length > 2) {
                        var arrow = simpleArrow(line[line.length - 2], line[line.length - 1], scale);
                        arrow.splice(0, 1);
                        return [circle, line].concat(arrow);
                    }
                    return [circle];
                },
        },
        toSeizeR: {
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
                    var line = curvedLineR(a, b, scale * 2);

                    // Arrow to target (last segment of curved line)
                    if (line.length > 2) {
                        var arrow = simpleArrow(line[line.length - 2], line[line.length - 1], scale);
                        arrow.splice(0, 1);
                        return [circle, line].concat(arrow);
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
                function (points, scale) {
                    var a = points[0];
                    var b = points[1];
                    var n = normalVector(a, b);
                    var x1 = [b[0] - (n[0] * scale * 3), b[1] - (n[1] * scale * 3)];
                    var x2 = [b[0] + (n[0] * scale * 2), b[1] + (n[1] * scale * 2)];

                    var p1 = [a[0] + (n[1] * scale * 1.5), a[1] - (n[0] * scale * 1.5)];
                    var p2 = [x1[0] + (n[1] * scale * 1.5), x1[1] - (n[0] * scale * 1.5)];
                    var p3 = [x1[0] + (n[1] * scale * 3), x1[1] - (n[0] * scale * 3)];
                    var p5 = [x1[0] - (n[1] * scale * 3), x1[1] + (n[0] * scale * 3)];
                    var p6 = [x1[0] - (n[1] * scale * 1.5), x1[1] + (n[0] * scale * 1.5)];
                    var p7 = [a[0] - (n[1] * scale * 1.5), a[1] + (n[0] * scale * 1.5)];


                    var z1 = [x2[0] + (n[1] * scale * 3), x2[1] - (n[0] * scale * 3)];
                    var z2 = [x2[0] - (n[1] * scale * 3), x2[1] + (n[0] * scale * 3)];

                    return [[p1, p2, p3, b, p5, p6, p7]]
                        .concat(brokenArrow([((2 * b[0]) + p3[0]) / 3, ((2 * b[1]) + p3[1]) / 3], z1, scale / 3, true))
                        .concat(brokenArrow([((2 * b[0]) + p5[0]) / 3, ((2 * b[1]) + p5[1]) / 3], z2, scale / 3));
                },
        },
        toDestroy: {
            points: 1,
            generate:
                function (points, scale) {
                    var c = points[0];
                    return [
                        [
                            [c[0] - (scale * 2), c[1] - (scale * 2)],
                            [c[0] - (scale * 0.75), c[1] - (scale * 0.75)]
                        ],
                        [
                            [c[0] + (scale * 2), c[1] - (scale * 2)],
                            [c[0] + (scale * 0.75), c[1] - (scale * 0.75)]
                        ],
                        [
                            [c[0] - (scale * 2), c[1] + (scale * 2)],
                            [c[0] - (scale * 0.75), c[1] + (scale * 0.75)]
                        ],
                        [
                            [c[0] + (scale * 2), c[1] + (scale * 2)],
                            [c[0] + (scale * 0.75), c[1] + (scale * 0.75)]
                        ]
                    ];
                },
            labels: ['D'],
            generateLabels:
                function (points, scale) {
                    return points;
                },
        },
        toDefend: {
            points: 1,
            generate:
                function (points, scale) {
                    var c = points[0];

                    var m = 18 * Math.PI / 16;
                    var points = [];
                    var lines = [points];
                    for (var angle = -6 * Math.PI / 16; angle <= m; angle += Math.PI / 8) {
                        var p1 = [c[0] + (Math.cos(angle) * scale * 2), c[1] + (Math.sin(angle) * scale * 2)];
                        var p2 = [c[0] + (Math.cos(angle) * scale * 2.5), c[1] + (Math.sin(angle) * scale * 2.5)];
                        points.push(p1);
                        lines.push([p1, p2]);
                    }
                    var mx = 21 * Math.PI / 16;
                    points.push([c[0] + (Math.cos(mx) * scale * 2), c[1] + (Math.sin(mx) * scale * 2)]);

                    var arrow = simpleArrow(points[points.length - 2], points[points.length - 1], scale / 2);

                    return lines.concat(arrow);
                },
            labels: ['R'],
            generateLabels:
                function (points, scale) {
                    var c = points[0];
                    return [[c[0], c[1] + (scale * 2.25)]];
                },
        }
    };

    MilMissions.complete4Points = function (points, sizeMeters) {
        points = points.slice();
        var n = normalVector(points[0], points[1]);
        var x1 = [n[0] * -0.7071068 + n[1] * -0.7071068, n[0] * 0.7071068 + n[1] * -0.7071068]; // -3/4 PI
        var x2 = [n[0] * 0.7071068 + n[1] * -0.7071068, n[0] * 0.70710682 + n[1] * 0.7071068]; // -1/4 PI
        if (points.length < 3) {
            points.push([points[0][0] + (x1[0] * sizeMeters * 4), points[0][1] + (x1[1] * sizeMeters * 4)]);
        }
        points.push([points[1][0] + (x2[0] * sizeMeters * 4), points[1][1] + (x2[1] * sizeMeters * 4)]);
        return points;
    }

})(MilMissions = MilMissions || {});