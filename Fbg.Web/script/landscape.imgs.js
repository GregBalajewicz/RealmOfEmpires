/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4.js" />
/// <reference path="interfaces.js" />
/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />
/// <reference path="ROE-utils.js" />
/// <reference path="roe-player.js" />
/// <reference path="landscape.common.js" />


// iOS devices default to useing images for map, not canvas. 
if (
    ROE.Landmark.forceUseMapType() === ROE.Landmark.Enum.MapTypes.imgs ||
    $.browser.idevice
    ) {

    ROE.Landmark.currentMapType = ROE.Landmark.Enum.MapTypes.imgs;

    ROE.Landmark.images = {
        villages: {}
    };

    ROE.Landmark.refill = function (all) {
        var s = ROE.Landmark.scale;
        $.each(ROE.Landmark.villages, function (i, n) {
            if (n.painted == false || all) {
                var vv = ROE.Landmark.images.villages[n.x + '_' + n.y];
                if (vv) {
                    vv.v.attr('src', BDA.Utils.GetMapVillageIconUrl(n.points, n.type));
                    vv.f.attr('src', 'https://static.realmofempires.com/images/map/' + n.flag).height(~~(20 / s)).width(~~(18 / s));
                }
            }
        });
    };

    // filling, goes for area, and checks if some earth is not exist, 
    // add it and after loading it filled with landmarks and villages, flags
    ROE.Landmark.fill = function (lx, ly, rx, ry) {
        var s = $('#surface');

        var new_earth = [];

        for (var ax = lx; ax <= rx; ax += this.earthpx) {
            for (var ay = ly; ay <= ry; ay += this.earthpx) {
                if (!this.earth.c[ax + '_' + ay]) {
                    this.earth.c[ax + '_' + ay] = true;
                    if (!this.earth.x[ax]) { this.earth.x[ax] = {}; }
                    this.earth.x[ax][ay] = true;

                    var ep = this.earthpx;
                    new_earth.push(((ax / ep) * 6) + '_' + -((((ay + ep) / ep) * 6) - 1));

                    s.append($('<img class="earth" src="https://static.realmofempires.com/images/map/bigtile.jpg" alt="landscape' + ax + '_' + ay + '" style="height: 504px; width: 504px" />').css({ left: ax, top: ay }));
                }
            }
        }

        this.moredata({ coords: new_earth }, function () {
            var l = ROE.Landmark;
            var sif = l.images.villages;
            var ls = l.scale;

            $.each(ROE.Landmark.land, function (i, n) {
                if (!n.painted) {
                    s.append($('<img class="land" coord="' + n.x + '_' + n.y + '" src="https://static.realmofempires.com/images/map/' + n.image + '" style="height: 84px; width: 84px; z-index: 2" />').css({ left: n.x * l.landpx, top: n.y * -l.landpx }));
                    n.painted = true;
                }
            });

            $.each(ROE.Landmark.villages, function (i, n) {
                if (!n.painted) {
                    var pos = { left: n.x * l.landpx, top: n.y * -l.landpx };
                    var v = $('<img class="village" coord="' + n.x + '_' + n.y + '" src="' + BDA.Utils.GetMapVillageIconUrl(n.points, n.type) + '" style="height: 84px; width: 84px; z-index: 2" />').css(pos);
                    sif[n.x + '_' + n.y] = { v: v };
                    s.append(v);

                    if (n.flag != null) {
                        var f = $('<img class="flag" coord="' + n.x + '_' + n.y + '" src="https://static.realmofempires.com/images/map/' + n.flag + '" style="height: 20px; width: 18px; z-index: 3" />').css(pos).height(~ ~(20 / ls)).width(~ ~(18 / ls));
                        sif[n.x + '_' + n.y].f = f;
                        s.append(f);
                    }
                    n.painted = true;
                }
            });
        });
    };
}