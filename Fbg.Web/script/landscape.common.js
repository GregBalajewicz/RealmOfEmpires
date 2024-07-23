$.browser.idevice = /iPhone/.test(navigator.userAgent); // mobile safari (ipod, ipad, iphone)
$.browser.android = /Android/.test(navigator.userAgent); // android
$.browser.mobile = $.browser.idevice || $.browser.android; // one of it

function prefix() {
    if ($.browser.msie) return '-ms';
    if ($.browser.mozilla) return '-moz';
    return '-webkit';
}

// variable names:
// x,y - coordinates of square (landmark, village) from db, -15, 25
// ep, lp - earthpx, landpx
// lux, rdy  - left upper x, right down y
// rx, ry - position of map surface in browser pxs, changing it, map viewport on which user looks
// l, s - short names to namespaces ROE.Landmark and ROE.Landmark.surface
// w, h - width, height of viewport window
// sw, sh - scaled width and height, s prefix always means some coords scaled

ROE.Landmark = {


    Enum: { MapTypes: { imgs: "imgs", canvas: "canvas"} }, // public enums

    forceUseMapType: function () { return $.cookie("dev_forceUseMapType") }, // to force the system to use one map type instead of the default for the device. one of Enum.MapTypes 
    currentMapType: undefined, // current map type used by the system. one of Enum.MapTypes. this is set by the actual code that is loaded / being executed

    earthpx: 504, // size of one earth in px
    landpx: 84, // in px size of one square, landmark or village




    earth: { x: {}, y: {}, c: {} },
    // collection where all earths lies
    // x and y, here is made for quicker access, so they are duplication of raw list of earthes
    // с is constructed like 504_-504 (x_y), means top left point
    // while x[504] gives array of all on this line
    scale: 1, // initial scale 1:1

    // called after update, if it needed, sets new area which need to be filled
    paint: function () {
        var l = ROE.Landmark;
        if (!l.nomorepaint) {
            var rx = - ~ ~l.rx; var ry = - ~ ~l.ry;
            var h = l.h; var w = l.w;

            rx = rx / l.scale; ry = ry / l.scale;
            w = w / l.scale; h = h / l.scale;

            var ep = l.earthpx;
            var lp = l.landpx;
            var lux = Math.floor(rx / ep) * ep;
            var luy = Math.floor(ry / ep) * ep;
            var rdx = Math.ceil((rx + w) / ep) * ep;
            var rdy = Math.ceil((ry + h) / ep) * ep;

            // this needed to make only one request to ajax, 
            // without it will do 10th ajaxes until data will be loaded
            l.nomorepaint = true;

            l.fill(lux, luy, rdx, rdy);
        }
    },

    // initiates update of whole map villages, if any of loaded images is 5 minutes old loaded
    checkvills: function () {
        var l = ROE.Landmark;
        
        function villold() {
            var d5min = new Date(new Date().setMinutes(new Date().getMinutes() - 5));
            var vc = l.screenarea().villcoords;
            for (var i in vc) {
                if (vc[i] < d5min.getTime()) return true;
            }
            return false;
        }

        function getnew() {
            ROE.Api.call("mapLand", { coords: l.screenarea().res }, function (r) {

                l.savelocalplcl(r);

                for (var vci in l.villcoords) {
                    l.villcoords[vci] = new Date().getTime();
                }

                $.each(r.villages, function (i, n) {
                    var ind = n.x + "_" + n.y;
                    var exist = l.villages[ind];
                    l.villages[ind] = n;
                    if (!exist || exist.icon != n.icon || exist.flag != n.flag) {
                        l.villages[ind].painted = false; // for images map to know when update them
                    }
                });

                l.connect();

                l.savelocalvill();

                l.refill();

                l.nomorepaint = false;
            });
        }

        getnew();

        setInterval(function () {
            if (villold() && !l.nomorepaint) {
                l.nomorepaint = true;
                getnew();
            }

        }, 1000 * 10 /*check each 10 sec*/);
    },

    screenarea: function () {
        var l = ROE.Landmark;
        var rx = - ~ ~l.rx; var ry = ~ ~l.ry;
        var h = l.h / l.scale; var w = l.w / l.scale;
        rx = rx / l.scale; ry = ry / l.scale;

        var lp = l.landpx;
        var fl = Math.floor;

        var rxl = fl(rx / lp); var ryl = fl(ry / lp);
        var dxl = fl((rx + w) / lp); var dyl = fl((ry - h) / lp);

        var lu = l.areacoord(rxl, ryl, true);
        var ru = l.areacoord(dxl, ryl, true);
        var rd = l.areacoord(dxl, dyl, true);
        var ld = l.areacoord(rxl, dyl, true);

        var scrlist = {};

        var xs = rd.x < lu.x ? rd.x : lu.x;
        var xb = rd.x < lu.x ? lu.x : rd.x;

        var ys = rd.y < lu.y ? rd.y : lu.y;
        var yb = rd.y < lu.y ? lu.y : rd.y;
        
        for(var x = xs; x <= xb; x += 6) {
            for(var y = ys; y <= yb; y += 6) {
                scrlist[x + '_' + y] = l.villcoords[x + '_' + y];
            }
        }

        return { lu: lu.r, ru: ru.r, rd: rd.r, ld: ld.r, res: lu.r + ',' + rd.r, villcoords: scrlist };
    },

    areacoord: function (x, y, ext) {
        var base = { x: 0, y: 1 };
        var sx = Math.floor((x - base.x) / 6);
        var sy = Math.floor((y - base.y) / 6);
        
        var bx = base.x + (sx * 6);
        var by = base.y + (sy * 6);

        return !ext ? bx + "_" + by : { x: bx, y: by, r: bx + "_" + by };
    },

    savelocalvill: function () {
        var l = ROE.Landmark;

        var rvilla = $.map(l.villages, function (v, k) {
            v.areacoord = l.areacoord(v.x, v.y);
            var ob = $.extend({}, v); delete ob.player; delete ob.painted; delete ob.created;
            ob.mine = ob.mine == true ? 1 : 0; return ob;
        });

        BDA.Database.Insert('Villages', rvilla, function () { });
    },

    savevill: function (v) {
        var ob = $.extend({}, v); delete ob.player; delete ob.painted; delete ob.created;
        BDA.Database.Insert('Villages', [ob], function () { console.log(arguments); });
    },

    savelocalplcl: function (r) {
        var l = ROE.Landmark;

        $.each(r.clans, function (i, n) {
            l.clans[n.id] = n;
        });

        BDA.Database.LocalSet('MapClans', JSON.stringify(l.clans));

        $.each(r.players, function (i, n) {
            l.players[n.id] = n;
        });

        BDA.Database.LocalSet('MapPlayers', JSON.stringify(l.players));
    },

    // function loads new data from server or from websql cache, cb executed when data comes
    moredata: function (param, cb) {

        var l = ROE.Landmark;

        if (BDA.Database.LocalGet('MapLandTypes') != null) {
            l.landtypes = JSON.parse(BDA.Database.LocalGet('MapLandTypes'));
        }

        if ($.isEmptyObject(l.landtypes))
            param.hasAllLandTypes = true;

        param.landcoords = [];

        var landcoordslocal = [];

        if (BDA.Database.LocalGet('MapLandArea') != null) {
            l.landcoords = JSON.parse(BDA.Database.LocalGet('MapLandArea'));
        }

        if (BDA.Database.LocalGet('MapVillArea') != null) {
            l.villcoords = JSON.parse(BDA.Database.LocalGet('MapVillArea'));
        }

        $.each(param.coords, function (i, n) {
            if (!l.landcoords[n]) {
                l.landcoords[n] = true;
                param.landcoords.push(n);
            } else {
                landcoordslocal.push('"' + n + '"');
            }
        });

        var villcoordslocal = [];

        param.coords = $.grep(param.coords, function (a) {
            if (l.villcoords[a]) { villcoordslocal.push('"' + a + '"'); return false; } else { l.villcoords[a] = new Date().getTime(); return true; }
        });

        param.landcoords = param.landcoords.join(',');
        param.coords = param.coords.join(',');

        var mapLandServerCall, mapVillLocalDbCall;

        if (param.coords.length > 0 || param.landcoords.length > 0) {
            mapLandServerCall = ROE.Api.call("mapLand", param,
                function (r) {
                    var l = ROE.Landmark;

                    if (r.realmid != ROE.realmID) { return; }

                    if (r.landtypes) {
                        l.landtypes = r.landtypes;
                        BDA.Database.LocalSet('MapLandTypes', JSON.stringify(r.landtypes));
                    }

                    BDA.Database.LocalSet('MapLandArea', JSON.stringify(l.landcoords));
                    BDA.Database.LocalSet('MapVillArea', JSON.stringify(l.villcoords));

                    // count area of landmark by x, y
                    $.each(r.land, function (i, n) {
                        n.area = l.areacoord(n.x, n.y);
                    });

                    var rlanda = $.map(r.land, function (v, k) { return { x: v.x, y: v.y, area: v.area, image: v.image }; });

                    BDA.Database.Insert('Landmarks', rlanda, function () { });

                    $.each(r.land, function (i, n) {
                        if (!l.land[n.x + "_" + n.y]) {
                            n.image = l.landtypes[n.image].image;
                            l.land[n.x + "_" + n.y] = n;
                            l.land[n.x + "_" + n.y].painted = false; // for images map to know when add them
                        }
                    });

                    l.savelocalplcl(r);

                    $.each(r.villages, function (i, n) {
                        var ind = n.x + "_" + n.y;
                        var exist = l.villages[ind];
                        l.villages[ind] = n;
                        if (!exist) {
                            l.villages[ind].painted = false; // for images map to know when add them
                        }
                    });

                    l.connect();

                    l.savelocalvill();
                }
            );
        }

        if (villcoordslocal.length > 0) {
            var mapVillLocalDbCall =
                //BDA.Database.GetDatabase().query(['select * from Villages_' + BDA.Database.id() + ' where areacoord in (' + villcoordslocal.join(',') + ')'])
                BDA.Database.Find("Villages", "areacoord", villcoordslocal)
                    .done(function (a) {
                        var l = ROE.Landmark;

                        l.clans = JSON.parse(BDA.Database.LocalGet('MapClans'));
                        l.players = JSON.parse(BDA.Database.LocalGet('MapPlayers'));

                        $.each(a, function (i, n) {
                            var ind = n.x + "_" + n.y;
                            var exist = l.villages[ind];
                            l.villages[ind] = n;
                            if (!exist) {
                                l.villages[ind].painted = false; // for images map to know when add them
                            }
                        });

                        l.connect();
                    });
        }

        var mapLandLocalDbCall =
            //BDA.Database.GetDatabase().query(['select * from Landmarks_' + BDA.Database.id() + ' where area in (' + landcoordslocal.join(',') + ')'])
            BDA.Database.Find("Landmarks", "area", landcoordslocal)
                .done(function (a) {
                    var l = ROE.Landmark;

                    $.each(a, function (i, n) {
                        if (!l.land[n.x + "_" + n.y]) {
                            // new object because looks like "a" consist of readonly (somehow) objects
                            var n1 = $.extend({}, n);
                            n1.image = l.landtypes[n1.image].image;

                            l.land[n.x + "_" + n.y] = n1;
                            l.land[n.x + "_" + n.y].painted = false; // for images map to know when add them
                        }
                    });
                });

        $.when(mapLandServerCall, mapVillLocalDbCall, mapLandLocalDbCall).done(function () {

            cb();

            var l = ROE.Landmark;
            l.nomorepaint = false;

            // if something not loaded sometimes happens, than load again
            if (l.update()) {
                l.paint();
            }

            if (!l.firstime) {
                l.checkvills();
                l.firstime = true;
            }

            l.px = null; l.py = null; // this code needed for first occureness of the map and header,
            l.pointdb();                        // because if execute before, no info about village loaded
        });
    },

    // checks if one of 4 points of viewport not on earth filled area
    update: function () {
        var l = ROE.Landmark;
        var rx = - ~ ~l.rx; var ry = - ~ ~l.ry;
        var h = l.h / l.scale; var w = l.w / l.scale;
        rx = rx / l.scale; ry = ry / l.scale;
        var ep = l.earthpx;
        var ea = l.earth;

        var lux = Math.floor(rx / ep) * ep;
        var luy = Math.floor(ry / ep) * ep;

        var rux = Math.floor((rx + w) / ep) * ep;
        var ruy = Math.floor(ry / ep) * ep;

        var rdx = Math.floor((rx + w) / ep) * ep;
        var rdy = Math.floor((ry + h) / ep) * ep;

        var ldx = Math.floor(rx / ep) * ep;
        var ldy = Math.floor((ry + h) / ep) * ep;

        if (!(ea.x[lux] && ea.x[lux][luy]) ||
            !(ea.x[rux] && ea.x[rux][ruy]) ||
            !(ea.x[ldx] && ea.x[ldx][ldy]) ||
            !(ea.x[rdx] && ea.x[rdx][rdy])) {
            return true;
        }

        return false;
    },

    // gives x,y db coordinates, and also calls changing of current landmark (for updating village info on header)
    pointdb: function () {
        var rx = - ~ ~this.rx; var ry = - ~ ~this.ry;
        var lp = this.landpx * this.scale;
        var h = this.h; var w = this.w;
        var p = { x: Math.floor((rx + ~ ~(w / 2)) / lp), y: (-Math.floor((ry + ~ ~(h / 2)) / lp)) };

        if (p.x != this.px || p.y != this.py) $.each(this.point_changed, function (i, n) { n(p.x, p.y); });
        this.px = p.x; this.py = p.y;

        return p;
    },

    zoom: function (ds) {

        var l = ROE.Landmark;

        if (!l.moveFunc) return;
        if (l.scale < 0.7 && ds < 0) { ROE.Frame.infoBar("Max zoom-out achieved"); return };
        if (l.scale >= 1 && ds > 0) { ROE.Frame.infoBar("Max zoom-in achieved"); return };
        var xy = l.pointdb();
        l.scale += ds;
        var lp = l.landpx * l.scale;

        BDA.Database.LocalSet('MapZoom', l.scale);

        var zoomScale = parseInt(l.scale*5)-1;
        //$("#roeOptions .zoomlevel").text(zoomScale);

        l.rx = - ~ ~((lp * xy.x) - (l.w / 2) + (lp / 2));
        l.ry = ~ ~((lp * xy.y) + (l.h / 2) - (lp / 2));

        l.refill(true);

        l.moveFunc({ x: l.rx, y: l.ry });

        if (l.update()) {
            l.paint();
        }
        l.pointdb();
    },

    pointpx: function () {
        return { pageX: ~ ~($(window).width() / 2), pageY: ~ ~($(window).height() / 2) - 20 };
    },

    // here you can push function that will execute after changing current landmark - function(x,y){}
    point_changed: [],

    // x,y - coords from db
    // scale - 0.5, 1
    start: function (x, y, scale) {

        if (!scale) { scale = 1; }
        var sz = BDA.Database.LocalGet('MapZoom');
        if (sz) { scale = parseFloat(sz); }

        this.earth = { x: {}, y: {}, c: {} };
        this.villages = {};
        this.players = {};
        this.clans = {};
        this.canvas.list = {};


        BDA.Broadcast.subscribe($('#map'), "InOutgoingDataChanged", function MapInOutgoingDataChanged() {
            ROE.Landmark.refill(true);
        });
        //$('#map .getInOutDataChangedEvent').bind('InOutgoingDataChanged', function MapInOutgoingDataChanged() {
        //    ROE.Landmark.refill(true);
        //});

        var l = ROE.Landmark;

        l.scale = scale;

        l.firsttime = false;

        var lp = l.landpx;
        var lps = lp * scale;
        var ep = l.earthpx;

        var h = l.h; var w = l.w;
        var sw = w / scale; var sh = h / scale;

        var sx = ~ ~(x * lp - (sw / 2));
        var sy = ~ ~(-y * lp - (sh / 2));

        var sx2 = ~ ~(x * lps - (w / 2));
        var sy2 = ~ ~(-y * lps - (h / 2));

        var lx = Math.floor(sx / ep) * ep;
        var ly = Math.floor(sy / ep) * ep;
        var rx = Math.ceil((sx + sw) / ep) * ep;
        var ry = Math.ceil((sy + sh) / ep) * ep;

        var xx = (-sx2 - (lps / 2)); var yy = -sy2 - (lps / 2);

        l.rx = xx; l.ry = yy;

        l.gotopx(xx, yy);

        l.fill(lx, ly, rx, ry);
        if (l.update()) l.paint();
        l.info();
    },

    gotopx: function (x /*px*/, y /*px*/) {
        if ($.browser.idevice) {
            this.el.setAttribute('style', prefix() + '-transform: translate3d(' + x + 'px, ' + y + 'px, 0px) scale3d(' + this.scale + ', ' + this.scale + ', 1);')
        } else {
            this.el.setAttribute('style', prefix() + '-transform: translate(' + x + 'px, ' + y + 'px) scale(' + this.scale + ');')
        }
    },

    gotodb: function(x /*db x*/, y /*db y*/) {
        var l = this;
        var lp = l.landpx;
        var lps = lp * l.scale;

        var sx2 = ~ ~(x * lps - (l.w / 2));
        var sy2 = ~ ~(-y * lps - (l.h / 2));

        var xx = (-sx2 - (lps / 2)); var yy = -sy2 - (lps / 2);

        this.gotopx(xx, yy);

        l.rx = xx; l.ry = yy;

        if (l.update()) {
            l.paint();
        }
        l.pointdb();
    },
    

    // connecting by id to full circular referenced network, and do indexing, for quicker search (village by x+y and by id)
    connect: function () {
        var l = ROE.Landmark;

        $.each(l.players, function (i, n) {
            if (n.CID) n.clan = l.clans[n.CID];
        });

        l.villages_byid = {};

        $.each(l.villages, function (i, n) {
            if (n.pid) n.player = l.players[n.pid];

            n.mine = n.pid == ROE.playerID;

            l.villages_byid[n.id] = n;
        });
    },

    close: function () {

    },

    // object storage of all things load on map
    clans: {},
    players: {},
    land: {},
    landcoords: {},
    villcoords: {},
    landtypes: {},
    villages: {},

    timer: {}
};

// executed ONCE, sets callback function for header filling while moving on map
ROE.Landmark.info = function () {
    var hmap = $('header .hmap');
    var td = $('td', hmap);

    var name = $('.name', hmap);
    var player = $('.player', hmap);
    var clan = $('.clan', hmap);

    var doact = hmap.filter('.doact');
    var troop = hmap.filter('.troop').hide();
    var info = hmap.filter('.info');

    info.unbind('click').bind('click', function (e) {
        e.preventDefault();
        ROE.UI.Sounds.click();
        $('.headernav').toggleClass("clicked");
    });
    
    this.point_changed.push(function (x, y) {
        var v = ROE.Landmark.villages[x + '_' + y];

        ROE.Landmark.v = v;
        
        if (ROE.Landmark.timer.troops) {
            clearTimeout(ROE.Landmark.timer.troops);
        }

        troop.hide();

        if (v) {
            var p = v.player;

            // this is Rebel village
            $('#map').attr('data-tutorial-rebel-selected', p.id == ROE.CONST.specialPlayer_Rebel);

            var c = p.clan;
            var coords = ' (' + v.x + ',' + v.y + ')';
            var villageURL = v.mine ? BuildURL('VillageOverview.aspx', ['svid=' + v.id]) : BuildURL('VillageOverviewOtherPopup.aspx', ['ovid=' + v.id]);

            info.html('<div class="head">' +
                         (!ROE.isSpecialPlayer(v.pid) ? '<img class="player-avatar" src="' + ROE.Avatars[v.player.Av].img + '" height="40px" style="float: left; margin: 2px 5px 2px 2px" />' : '') +
                         '<div class=onel><span class=area>[A' + v.area + ']</span>&nbsp;<span id="vName">' + v.name + '</span>&nbsp;<span>' + coords + '</span><span class=points>[' + BDA.Utils.formatNum(~~v.points) + ']</span>' +
                          (v.note != '0' ? '<span class="note"> ' + v.note + '</span>' : '')
                            + '</div> <div class=onel>' +
                          (!ROE.isSpecialPlayer(v.pid) ? ('<span class="pn">' + p.PN + '</span> <span class=points>[' + ROE.Utils.formatShortNum(p.PP) + ']</span>') : '&nbsp;') +
                          (p.Pe != '0' ? '<span class="note"> ' + p.Pe + '</span>' : '')
                            + '</div> <div class=onel>' +
                          (c ? c.CN + (c.CP != '0' ? '<span class=points> [' + ROE.Utils.formatShortNum(c.CP) + ']</span>' : '') : '&nbsp;</div>') +
                          '</div><div class=do sfx2></div>'
                          );

            $('.player-avatar', info).click(function _userNameClick(e) {
                e.preventDefault();
                ROE.Frame.popupPlayerProfile($(this).parent().find('.pn').html());
            });

            doact.empty();

            var helpArea = doact.append('<div class="help sfx2" style="width: 300px"></div>').find('.help');
            var vovLink = hmap.find('#villName');

            ROE.getAnchorMode_IconOnly = true;

            doact.append(ROE.getVOVAnchor(villageURL, v.mine, v.id));
            doact.append(ROE.getAttackAnchor(v.id, v.x, v.y, false, helpArea));
            //doact.append(ROE.getAttackAnchor_OneClick(v.id, v.x, v.y, false, helpArea));
            doact.append(ROE.getSupportAnchor(v.id, v.x, v.y, v.pid, false, helpArea));
           // if (ROE.loginMode !== "mobileonly") { doact.append(ROE.getSupportAnchor_OneClick(v.id, v.x, v.y, v.pid, false, helpArea)); }
            if (ROE.loginMode !== "mobileonly") { doact.append(ROE.getHQAnchor(v.id, v.mine, false, helpArea)); }
            if (ROE.loginMode !== "mobileonly") { doact.append(ROE.getRecruitAnchor(v.id, v.mine, false, helpArea)); }
            doact.append(ROE.getSendSilverAnchor(v.id, v.mine, v.x, v.y, v.pid, helpArea, false));
            doact.append(ROE.getSupportLookupAnchor(v.id, v.mine, false, helpArea, v.pid));

            if (v.mine) {
                doact.append('<a href="#" onclick="ROE.Utils.RenameVillage(' + v.id + ', $(\'#vName\')[0], null, ROE.Utils.updateVillageName)" class="renV sfx2"></a>');
                ROE.helper_AddHelpTextToElement($('.doact a.renV'), helpArea, 'Rename this village');

                ROE.Landmark.timer.troops = setTimeout(function LandmarkChangePositionTroopsTimer() {
                    if (!BDA.Database.LocalGet('MapTroopType'))
                        BDA.Database.LocalSet('MapTroopType', 'all');

                    var p = ROE.Landmark.pointdb();
                    var v = ROE.Landmark.villages[p.x + '_' + p.y];

                    Troops.GetTroopsInVillageByType(v.id, BDA.Database.LocalGet('MapTroopType'),
                        function GetTroopsInVillageByType_AjaxCallback(f) {
                            var pp = ROE.Landmark.pointdb();
                            var vv = ROE.Landmark.villages[pp.x + '_' + pp.y];

                            if (f != '' && vv != undefined) {
                                var obj = eval(f);
                                if (obj.VID == vv.id) {
                                    troop.html(obj.htmltable).show();
                                }
                            }
                        }
                    )
                }, 1000);

            } else {
                var empty = $('<div>').addClass('na').addClass('renV_na');
                doact.append(empty);
            }

            // wait while will be made completelly
            if (localStorage.HighlightPopup == 'true')
                doact.append($('<a class="highlight">H</a>').click(ROE.Frame.popupHighlight));

            if (!$("header").is(":visible"))
                $("header").show();

        } else {
            name.html('<br /><br /><br />');

            $('#map').attr('data-tutorial-rebel-selected', 'false');

            if ($("header").is(":visible"))
                $("header").hide();
        }
    });
};

ROE.Landmark.updateVillageName = function (newName) {
    if (typeof ROE.Landmark.v != 'undefined') {
        BDA.Database.UpdateNoId('Villages', { id: 'id', fields: [{ 'id': ROE.Landmark.v.id, 'name': newName }] }, function () { });
        ROE.Landmark.villages[ROE.Landmark.v.x + '_' + ROE.Landmark.v.y].name = newName;
    }
}