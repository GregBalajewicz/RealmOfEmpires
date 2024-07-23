(function () { }(window.ROE = window.ROE || {}));

(function (l) {
    BDA.Console.setCategoryDefaultView('roe-map', false);


    // variable names:
    // x,y - coordinates of square (landmark, village) from db, -15, 25
    // ep, lp - earthpx, landpx
    // lux, rdy  - left upper x, right down y
    // rx, ry - position of map surface in browser pxs, changing it, map viewport on which user looks
    // l - short names to namespace ROE.Landmark
    // w, h - width, height of viewport window
    // sw, sh - scaled width and height, s prefix always means some coords scaled

    l.troopInfoTemplate = {};//jquery object representing the template for troop info display 


    $.extend(l, {

        // l.el - surface dom
        // l.w, l.h - width, height of screen
        // l.px, l.py - db coord selected
        // l.rx, l.ry - px of surface offset
        // l.scale - scale { 1, 0.75, 0.5, 0.25 }

        earthpx: 504, // size of one earth in px for 1:1 zoom
        landpx: 84, // in px size of one square, landmark or village 1:1 zoom

        events: [],

        // collection where all canvases lies
        // x and y, here is made for quicker access, so they are duplication of raw list of earthes
        // с is constructed like 504_-504 (x_y), means top left point
        // while x[504] gives array of all on this line
        earth: { x: {}, c: {} },

        scale: 1, // initial scale 1:1

        // object storage of all things load on map
        clans: {},
        players: {},
        land: {},
        landcoords: {},
        villcoords: {},
        landtypes: {},
        villages: {},

        timer: {},

        // here you can push function that will execute after changing current landmark - function(x,y){}
        point_changed: []
    });

    var timer_mapGuiHover;

    var zoomLevel = 0;

    var _phrase = function (mes) {
        return $('#map .phrases [ph=' + mes + ']').html();
    }

    // Responsible for topicality of villages (not more than 5 minutes old). 
    // Check happens each 10 sec. villold() check that any of _screenarea().villcoords 
    //       is older than 5 minutes, then it executes update from ajax and refill().
    var _checkvills = function () {

        function villold() {
            var d5min;
            if (ROE.rt == "X") {
                d5min = new Date(new Date().setSeconds(new Date().getSeconds() - 30));
            } else {
                d5min = new Date(new Date().setMinutes(new Date().getMinutes() - 5));
            }
            var vc = _screenarea().villcoords;
            for (var i in vc) {
                if (vc[i] < d5min.getTime()) return true;
            }
            return false;
        }

        function getnew() {
            var villCoords = _screenarea().villcoords;

            ROE.Api.call("mapLand", { coords: _screenarea().res }, function (r) {

                _savelocalplcl(r);
                for (var vci in villCoords) {
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

                _connect();
                _savelocalvill();
                _refill();
                l.nomorepaint = false;
                _pointdb(true);
            });
        }

        getnew();

        setInterval(function () {
            if (villold() && !l.nomorepaint) {
                l.nomorepaint = true;
                getnew();
            }

        }, 1000 * 10 /*check each 10 sec*/);
    }

    var _screenarea = function () {
        var rx = -l.rx; var ry = l.ry;
        var h = l.h; var w = l.w;

        var lp = l.landpx;
        var fl = Math.floor;

        var rxl = fl(rx / lp); var ryl = fl(ry / lp);
        var dxl = fl((rx + w) / lp); var dyl = fl((ry - h) / lp);

        var lu = _areacoord(rxl, ryl, true);
        var ru = _areacoord(dxl, ryl, true);
        var rd = _areacoord(dxl, dyl, true);
        var ld = _areacoord(rxl, dyl, true);

        var scrlist = {};

        var xs = rd.x < lu.x ? rd.x : lu.x;
        var xb = rd.x < lu.x ? lu.x : rd.x;

        var ys = rd.y < lu.y ? rd.y : lu.y;
        var yb = rd.y < lu.y ? lu.y : rd.y;

        for (var x = xs; x <= xb; x += 6) {
            for (var y = ys; y <= yb; y += 6) {
                scrlist[x + '_' + y] = l.villcoords[x + '_' + y];
            }
        }

        return { lu: lu.r, ru: ru.r, rd: rd.r, ld: ld.r, res: lu.r + ',' + rd.r, villcoords: scrlist };
    }

    var _areacoord = function (x, y, ext) {
        var base = { x: 0, y: 1 };
        var sx = Math.floor((x - base.x) / 6);
        var sy = Math.floor((y - base.y) / 6);

        var bx = base.x + (sx * 6);
        var by = base.y + (sy * 6);

        return !ext ? bx + "_" + by : { x: bx, y: by, r: bx + "_" + by };
    }

    // resaves only villages that are loaded 5min ago, and dont touch other, 
    // before that there were several hundreds unneeded updates
    var _savelocalvill = function () {
        //return;
        var rvilla = [];

        function villByArea(area) {
            var varr = [];
            $.each(l.villages, function (i, v) {
                if (v.areacoord == area) { varr.push(v); }
            });
            return varr;
        }

        var d5min = new Date(new Date().setMinutes(new Date().getMinutes() - 5));

        var villToUpdate = [];
        $.each(l.villcoords, function (k, n) {
            if (n > d5min.getTime()) {
                villToUpdate = villToUpdate.concat(villByArea(k));
            }
        });

        $.each(villToUpdate, function (i, v) {
            if (!l.players[v.pid]) return;
            var ob = $.extend({}, v); delete ob.player; delete ob.painted; delete ob.created;
            ob.mine = ob.mine == true ? 1 : 0;

            rvilla.push(ob);
        });
        BDA.Database.Insert('Villages', rvilla, function () { });
    }

    var _savevill = function (v) {

        if (!ROE.Landmark.players[v.pid]) return;

        var ob = $.extend({}, v); delete ob.player; delete ob.painted; delete ob.created;
        BDA.Database.Insert('Villages', [ob], function () { BDA.Console.verbose("INSERT INTO Villages", arguments); });
    }

    var _savelocalplcl = function (r) {
        var l = ROE.Landmark;

        $.each(r.clans, function (i, n) {
            l.clans[n.id] = n;
        });

        BDA.Database.LocalSet('MapClans', JSON.stringify(l.clans));

        $.each(r.players, function (i, n) {
            l.players[n.id] = n;
        });

        BDA.Database.LocalSet('MapPlayers', JSON.stringify(l.players));

        l.events.push('savelocalplcl');
    }

    // function loads new data from server or from websql cache, cb executed when data comes
    var _moredata = function (param, cb) {

        l.events.push('moredata');

        //save this so we can use it later if needed. (used for recovering landtypes via api call)
        var originalCoords = param.coords;

        var dataMissing = false;

        // LandTypes - loaded once from server, then stored in localStorage, and restored to l.landtypes every map load
        if (BDA.Database.LocalGet('MapLandTypes') != null) {
            l.landtypes = JSON.parse(BDA.Database.LocalGet('MapLandTypes'));
        } else {
            param.hasAllLandTypes = true;
            dataMissing = true;
        }

        if ($.isEmptyObject(l.landtypes)) {
            param.hasAllLandTypes = true;
            dataMissing = true;
        }

        // Deciding which of Lands and Villages load from WebSql and which from Ajax

        // Landmarks and Villages – are stored in WebSql. 
        // While moredata asked for array of earth coordinates, 
        // function checks MapVillArea and MapLandArea if such landmarks were already once requested and load data from WebSql. 
        // WebSql used for 2 reasons – for offline access and because its quicker in general than Ajax. 
        // As _savelocalplcl(), the _savelocalvill() saves new information from Ajax to WebSql if area mentioned in MapVillArea is more than 5 min old.
        param.landcoords = [];

        var landcoordslocal = [];

        if (BDA.Database.LocalGet('MapLandArea') != null) {
            l.landcoords = JSON.parse(BDA.Database.LocalGet('MapLandArea'));
        } else {
            dataMissing = true;
        }

        if (BDA.Database.LocalGet('MapVillArea') != null) {
            l.villcoords = JSON.parse(BDA.Database.LocalGet('MapVillArea'));
        } else {
            dataMissing = true;
        }

        $.each(param.coords, function (i, n) {
            if (!l.landcoords[n]) {
                l.landcoords[n] = true;
            }
            param.landcoords.push(n);
        });


        var villcoordslocal = [];
        param.coords = $.grep(param.coords, function (a) {
            if (l.villcoords[a]) {
                villcoordslocal.push(a);
                return false;
            } else {
                l.villcoords[a] = new Date().getTime();
                return true;
            }
        });


        var landcoordslocal = param.landcoords; //copy it for local db use
        param.landcoords = param.landcoords.join(','); //joining them to a string for API call

        //still have an issue, so forcing mapland to get data based on originalCoords
        param.coords = originalCoords;
        param.coords = param.coords.join(',');  //joining them to a string for API call

        var mapLandServerCall, mapVillLocalDbCall, mapLandLocalDbCall;

        if (param.coords.length > 0 || param.landcoords.length > 0) {
            mapLandServerCall = ROE.Api.call("mapLand", param,
                function mapLandServer_Callback(r) {

                    if (r.landtypes) {
                        l.landtypes = r.landtypes;
                        BDA.Database.LocalSet('MapLandTypes', JSON.stringify(r.landtypes));
                    }

                    BDA.Database.LocalSet('MapLandArea', JSON.stringify(l.landcoords));
                    BDA.Database.LocalSet('MapVillArea', JSON.stringify(l.villcoords));

                    // count area of landmark by x, y
                    $.each(r.land, function (i, n) {
                        n.area = _areacoord(n.x, n.y);
                    });

                    var rlanda = $.map(r.land, function (v, k) {
                        return { x: v.x, y: v.y, area: v.area, image: v.image, id: v.x + "_" + v.y }; //create an id, based on xy, to help with DB actions
                    });

                    BDA.Database.Insert('Landmarks', rlanda, function () { });

                    $.each(r.land, function (i, n) {
                        if (!l.land[n.x + "_" + n.y]) {
                            n.image = l.landtypes[n.image].image;
                            l.land[n.x + "_" + n.y] = n;
                            l.land[n.x + "_" + n.y].painted = false;
                        }
                    });

                    _savelocalplcl(r);

                    $.each(r.villages, function (i, n) {
                        var ind = n.x + "_" + n.y;
                        var exist = l.villages[ind];
                        if (!l.players[n.pid]) return;

                        l.villages[ind] = n;
                        if (!exist) {
                            l.villages[ind].painted = false; // for refill to know which canvases need to be updated
                        }
                    });

                    _connect();

                    l.events.push('moredata: server loaded after connect');

                    _savelocalvill();

                }
            );

        }

        if (!dataMissing) {

            //this is when no local DB data is missing, we can make a paint call using what we have
            //dont have to wait for mapland CB

            if (landcoordslocal.length > 0 && !($.isEmptyObject(l.landtypes))) {

                mapLandLocalDbCall =
                    BDA.Database.Find("Landmarks", "area", landcoordslocal)
                        .done(function mapLandWebsql_Callback(a) {
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
            }

            if (villcoordslocal.length > 0) {
                mapVillLocalDbCall =
                    BDA.Database.Find("Villages", "areacoord", villcoordslocal)
                        .done(function mapVillWebsql_Callback(a) {
                            l.clans = JSON.parse(BDA.Database.LocalGet('MapClans'));
                            l.players = JSON.parse(BDA.Database.LocalGet('MapPlayers'));

                            $.each(a, function (i, n) {
                                var ind = n.x + "_" + n.y;
                                var exist = l.villages[ind];
                                if (!l.players[n.pid]) return;

                                l.villages[ind] = n;
                                if (!exist) {
                                    l.villages[ind].painted = false; // for images map to know when add them
                                }
                            });

                            _connect();

                            l.events.push('moredata: websql loaded after connect');
                        });
            }

            $.when(mapVillLocalDbCall, mapLandLocalDbCall).done(function () {

                cb();

                l.events.push('moredata: whole load');
                l.nomorepaint = false;

                // if something not loaded sometimes happens, than load again
                if (_update()) {
                    _paint();
                }

                if (!l.firstime) {
                    _checkvills();
                    l.firstime = true;
                }

            });

        }

        //when mapland API calls back and its resolved
        $.when(mapLandServerCall).done(function () {

            cb();

            l.events.push('moredata: whole load');
            l.nomorepaint = false;

            // if something not loaded sometimes happens, than load again
            if (_update()) {
                _paint();
            }

            if (!l.firstime) {
                _checkvills();
                l.firstime = true;
            }

        });



    }

    // checks if one of 4 points of viewport not on earth filled area
    // read about fill() method to know what is l.earth
    var _update = function (_rx, _ry) {
        var rx = _rx ? -_rx : -l.rx; var ry = _ry ? -_ry : -l.ry;
        var h = l.h; var w = l.w;
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
    }

    // called after update, if it needed, sets new area which need to be filled
    var _paint = function (_rx, _ry) {
        if (!l.nomorepaint) {
            var rx = _rx ? -_rx : -l.rx; var ry = _ry ? -_ry : -l.ry;
            var ep = l.earthpx;
            var lux = Math.floor(rx / ep) * ep;
            var luy = Math.floor(ry / ep) * ep;
            var rdx = Math.ceil((rx + l.w) / ep) * ep;
            var rdy = Math.ceil((ry + l.h) / ep) * ep;

            // this needed to make only one request to ajax, 
            // without it will do 10th ajaxes until data will be loaded
            l.nomorepaint = true;

            _fill(lux, luy, rdx, rdy);
        }
    }

    //_updatePaintCycle is another interval (1 sec) that was added because a bug of black parts of map sometimes. 
    // That black parts disappear after next small move and is connected to “l.nomorepaint” flag. 
    // It was added, to make moredata() sync. update() whenever asked tell true or false, 
    // if one of 4 sides of viewport is on black place, where there is no canvas. 
    // This check “if(update()) paint()” happens all the time someone moving through the map, 
    // so in this case l.nomorepaint means that no worries - loading is started, 
    // in the end of moredata() there is also check for update/paint. 
    // But if person stops moving a map in the middle of drawing canvases, 
    // and will be outside of zone that user requested, it will get black zone. 
    // For this _updatePaintCycle updates each second.
    var _updatePaintCycle = function () {
        l.timer.painting = setInterval(function () {
            if (l.update()) {
                l.paint();
            }
        }, 1000);
    }

    // gives x,y db coordinates, and also calls changing of current landmark (for updating village info on header)
    var _pointdb = function (force) {

        var lp = l.landpx;
        var p = {
            x: (parseInt($('#select').css('left')) / lp),
            y: -(parseInt($('#select').css('top')) / lp)
        };

        if ((p.x != l.px || p.y != l.py) || force) {
            $.each(l.point_changed, function (i, n) { n(p.x, p.y); });
        }

        l.px = p.x;
        l.py = p.y;

        return p;
    }

    var _pointpx = function () {
        return { pageX: ~ ~($(window).width() / 2), pageY: ~ ~($(window).height() / 2) - 20 };
    }

    var _gotopx = function (rx /*px*/, ry /*px*/) {
        var prop = 'translate(' + rx + 'px, ' + ry + 'px)';
        l.el.style[Modernizr.prefixed('transform')] = prop;
    }

    var _gotodb = function (x /*db*/, y /*db*/) {
        var lp = l.landpx;

        // db coords to px + offset for half screen and half land
        l.rx = - ~ ~((x * lp - (l.w / 2)) + (lp / 2));
        l.ry = - ~ ~((-y * lp - (l.h / 2)) + (lp / 2));

        _gotopx(l.rx, l.ry);

        if (_update()) {
            _paint();
        }
        _pointdb();
    }

    // Reloads whole map in other scale value centering on the same village that was selected. 
    // Before that to scale we use “translate: scale(0.5)” but it was hard for devices, 
    //   so now it is removing all old canvases (sets 1px width/height) and 
    //   with new position rx, ry + scale + new landpx, earthpx - it creates map newly.
    var _zoom = function (ds) {
        if (!l.moveFunc) return;

        if (l.scale <= 0.25 && ds < 0) { ROE.Frame.infoBar(_phrase('InfoMinZoom')); return };
        if (l.scale >= 1 && ds > 0) { ROE.Frame.infoBar(_phrase('InfoMaxZoom')); return };

        //needs to be refactored - basically converts center of map to a relative left/top of the main village, 
        //and then converts it to a relative coordinate like pointdb
        var lp = l.landpx;
        // click coord
        var cc = { left: -l.rx + Math.round(l.w / 2), top: -l.ry + Math.round(l.h / 2) };
        // get shift to tile size
        var modx = (cc.left % lp); var mody = (cc.top % lp);
        // apply shift, for - and + surface zone, different shift
        cc.left = cc.left - (modx < 0 ? lp + modx : modx);
        cc.top = cc.top - (mody < 0 ? lp + mody : mody);
        var centerCoord = { x: cc.left / lp, y: -cc.top / lp };

        var selectedDbCoord = _pointdb();
        l.scale += ds;

        if (l.scale > 1) { l.scale = 1; }
        else if (l.scale < .25) { l.scale = 0.25; }

        var lp = l.landpx = Math.ceil(84 * l.scale);
        l.earthpx = Math.ceil(504 * l.scale);

        // we found that removing canvases, causes errors
        // other way to low memory without removing, was making theire canvas zone small - 1x1px
        // so we remove all canvases while zooming, and create new one
        $('#surface .earth').hide().attr('width', '1px').attr('height', '1px');

        ROE.Landmark.earth = { x: {}, y: {}, c: {} };
        ROE.Landmark.canvas.list = {};

        BDA.Database.LocalSet('MapZoom', l.scale);
        // after zoom, you need the center be on the same place where it was
        l.rx = - ~ ~((lp * centerCoord.x) - (l.w / 2) + (lp / 2));
        l.ry = ~ ~((lp * centerCoord.y) + (l.h / 2) - (lp / 2));

        //// centring selection
        //rescaling selection
        var szone = $('#surface #select');
        szone.height(lp).width(lp);
        szone.css({ left: lp * selectedDbCoord.x, top: -lp * selectedDbCoord.y });

        l.moveFunc({ x: l.rx, y: l.ry });
        l.nomorepaint = false;

        if (_update()) {
            _paint();
        }
        _pointdb();
    }

    // village can be selected on a map with blue square, that gives you details on top and actions to do with it
    var _select = function (px, py, doCentering) {
        // if no args, then it will be center of screen
        if (!px) px = Math.round(l.w / 2);
        if (!py) py = Math.round(l.h / 2);

        var lp = l.landpx;

        // click coord
        var cc = { left: -l.rx + px, top: -l.ry + py };
        // get shift to tile size
        var modx = (cc.left % lp); var mody = (cc.top % lp);
        // apply shift, for - and + surface zone, different shift
        cc.left = cc.left - (modx < 0 ? lp + modx : modx);
        cc.top = cc.top - (mody < 0 ? lp + mody : mody);

        var szone = $('#surface #select');
        szone.height(lp).width(lp);

        // old selected place
        var szl = parseInt(szone.css('left'));
        var szt = parseInt(szone.css('top'));

        if (doCentering) {
            // if was clicked on the same point, old == new -> than center screen on that village
            if (szl == cc.left && szt == cc.top) {
                _clearInvisibleCanvases();
                _gotodb(szl / lp, -(szt / lp));
            }
        }

        szone.css(cc);
        _pointdb();


    }


    var _handleTroopCountInVillageChanged = function _handleTroopCountInVillageChanged(village) {
        ///<param name="village>instance of ROE.Village, and this is the village where the troop count changed</param>
        _landmarkChangePositionTroops(village, true);
        _populateHoverTroops(village, true);
    }

    var _start = function (x /*db*/, y /*db*/, scale) {

        var down = false;
        var click = 0;
        var tim;
        var templateAsStr = BDA.Templates.getRaw("map2", ROE.realmID);
        var template = $(templateAsStr);
        l.troopInfoTemplate = template.find(".unitsMapDisplay").remove();
        l.troopInfoLoadingTemplate = template.find(".unitsMapDisplay_loading").remove();


        // default the troops that are displayed on the HUD
        if (!BDA.Database.LocalGet('MapTroopType')) {
            BDA.Database.LocalSet('MapTroopType', 'all');
        }

        // see case 27703
        try {
            var mapt = BDA.Templates.populate(template[0].outerHTML, {});
        } catch (exception) {
            var roeex = new BDA.Exception('error in _start of map - template is empty', exception);
            roeex.data.add('templateAsStr', templateAsStr);
            roeex.data.addContextInfoObj(BDA.Templates.getContextInfo());
            BDA.latestException = roeex;
            throw roeex;
        }

        $('.TDContent').html(mapt);

        var mapElement = $('#map');
        l.el = $('#surface')[0];
        l.h = $('#mapwrap').height();
        l.w = mapElement.width();

        $(window).resize(function () {
            l.h = $('#mapwrap').height();
            l.w = $('#map').width();
        });


        ROE.Animation.initMapAnimations();

        // was region
        l.events = [];

        if (!scale) { scale = 1; }
        var sz = BDA.Database.LocalGet('MapZoom');
        if (sz) { scale = parseFloat(sz); }

        _clearLocal();

        var mp = BDA.Database.LocalGet('MapPromote');
        if (mp != null)
            l.promotion = JSON.parse(mp);

        $(document).bind('player.refresh', _mapPlayerRefreshTrigger);

        _mapPlayerRefreshTrigger();


        BDA.Broadcast.subscribe(mapElement, "InOutgoingDataChanged", function MapInOutgoingDataChanged() {
            _refill(true);
        });
        BDA.Broadcast.subscribe(mapElement, "VillageExtendedInfoUpdated", _handleTroopCountInVillageChanged);
        BDA.Broadcast.subscribe(mapElement, "VillageExtendedInfoInitiallyPopulated", _handleTroopCountInVillageChanged);
        BDA.Broadcast.subscribe(mapElement, "CurrentlySelectedVillageChanged", _handleSelectedVillageChangedEvent);

        l.scale = scale;
        var lp = l.landpx = Math.ceil(84 * scale);
        var ep = l.earthpx = Math.ceil(504 * scale);

        var h = l.h; var w = l.w;

        l.rx = - ~ ~((x * lp - (w / 2)) + (lp / 2));
        l.ry = - ~ ~((-y * lp - (h / 2)) + (lp / 2));

        var sx = ~ ~(x * lp - (w / 2));
        var sy = ~ ~(-y * lp - (h / 2));
        var lx = Math.floor(sx / ep) * ep;
        var ly = Math.floor(sy / ep) * ep;
        var rx = Math.ceil((sx + w) / ep) * ep;
        var ry = Math.ceil((sy + h) / ep) * ep;

        _gotopx(l.rx, l.ry);

        _fill(lx, ly, rx, ry);
        if (_update()) _paint();
        _updatePaintCycle();
        if (ROE.isMobile) {
            _refreshLandmarkVillages();
            _M_info();
        } else {
            _D2_info();
        }
        _select();
        _connect();
        //_pointdb(); // seems redundent as select calls it anyway

        // was region
        l.moveFunc = _map_flashmove;

        if (ROE.isMobile) {
            mapElement.bind('touchmove', function (e) {
                click++; _map_tm(e, e.originalEvent.targetTouches[0], _map_flashmove);
            });
            mapElement.bind('touchend', function (e) {
                o = null; click = 0;
            });
        }

        var offset = $('#form1').position();

        mapElement.click(function (e) {
            if (click < 5) { //prevents accidental clicks during drag
                ROE.UI.Sounds.click();
                ROE.Landmark.select(e.pageX - offset.left, e.pageY - offset.top, ROE.isMobile);
                var mapEventExistsUnderClick = ROE.MapEvents.checkPlayerMapEvents(l.px, l.py);
                if (ROE.isD2) {
                    var lp = l.landpx;
                    var p = {
                        x: (parseInt($('#select').css('left')) / lp),
                        y: -(parseInt($('#select').css('top')) / lp)
                    };
                    var v = l.villages[p.x + '_' + p.y];

                    //an issue here, if mapevent survives first click, player will never get the selectedV gui
                    //will fix later as for now we dont have events that persist -farhad Sep 21 2015 
                    if (v && !mapEventExistsUnderClick) {
                        _D2_SelectedMapGuiUpdate(v);
                        _mapGuiShowSelected(v);
                        ROE.Animation.vClick(v);
                    }
                }
            }
            click = 0;
        });

        if (ROE.isD2) {
            mapElement.dblclick(function (e) {
                ROE.UI.Sounds.click();
                ROE.Landmark.select(e.pageX - offset.left, e.pageY - offset.top, true);
            });
            mapElement.bind('mousemove', function (e) {
                if (down) { _map_tm(e, e, _map_flashmove); click++; }
                ROE.Landmark.hoverOverVillage({ x: e.pageX - offset.left, y: e.pageY - offset.top });
            });
            mapElement.bind('mousedown', function (e) {
                e.preventDefault(); down = true;
            });
            //bind window so that when user lets go of drag outside the map, it will actually trigger this event
            $(window).bind('mouseup', function (e) {
                /* e.preventDefault(); What is the purpose of this for mouseup?*/ down = false; o = null;
            });
            if (!ROE.Frame.isInIframe()) {
                mapElement.bind('mousewheel DOMMouseScroll', function (e) {
                    e.preventDefault();

                    if (ROE.LocalServerStorage.get('MapScrollZoom') == "OFF") { return; }

                    var currScale = Math.round(l.scale * 100) + zoomLevel;

                    //detail is for firefox, and wheelDelta is for chrome/IE
                    if (e.originalEvent.wheelDelta > 0 || e.originalEvent.detail < 0) {
                        if (currScale >= 100) { ROE.Frame.infoBar(_phrase('InfoMaxZoom')); }
                        else { zoomLevel += 25; currScale += 25; }
                    } else {
                        if (currScale <= 25) { ROE.Frame.infoBar(_phrase('InfoMinZoom')); }
                        else { zoomLevel -= 25; currScale -= 25; }
                    }

                    var zoomPanel = $('#zoomPanel');
                    if (zoomPanel.length) {
                        zoomPanel.find('.progress-indicator').first().css('width', currScale + '%');
                        zoomPanel.find('.zoomText').text('Zoom Level: ' + currScale + '%');
                    } else {
                        $('body').append('<div id="zoomPanel" class="zoomPanel progress-bg">' +
                                            '<div class="progress-container">' +
                                                '<div class="progress-indicator" id="progressBar" style="width:' + currScale + '%;">' +
                                                    '<div class="zoomText">Zoom Level: ' + currScale + '%</div>' +
                                                '</div>' +
                                            '</div>' +
                                          '</div>');
                    }

                    //waits 250ms to tell if user stopped scrolling, and then calls the database items to populate the map
                    clearTimeout($.data(this, 'timer'));
                    $.data(this, 'timer', setTimeout(function () {
                        if (zoomLevel != 0) ROE.Landmark.zoom(zoomLevel / 100);
                        $('.zoomPanel').remove();
                        zoomLevel = 0;
                    }, 500));
                });
            }

            $('#mapGuiSelected .mapGuiTroop').click(_troopPopupOpen);
            $('#mapGuiSelected, #mapGuiButtons').mouseover(function () {
                $('#mapGuiHover').stop().hide();
                $('#mapGuiSelected, #mapGuiButtons').stop().fadeIn(100);
            });


        }

        ROE.MapEvents.updateMapEventNotifications();

    }


    function _refreshLandmarkVillages() {
        //taken from getNew(), this is seperately called as a temp fix for mapHeader not showing in time
        var villCoords = _screenarea().villcoords;

        ROE.Api.call("mapLand", { coords: _screenarea().res }, function mapLandCallback(r) {

            _savelocalplcl(r);
            for (var vci in villCoords) {
                l.villcoords[vci] = new Date().getTime();
            }

            $.each(r.villages, function (i, n) {
                var ind = n.x + "_" + n.y;
                var exist = l.villages[ind];
                l.villages[ind] = n;
                if (!exist || exist.icon != n.icon || exist.flag != n.flag) {
                    l.villages[ind].painted = false;
                }
            });

            _connect();

            _savelocalvill();

            _refill();
            l.nomorepaint = false;
            _pointdb(true);
        });
    }

    function _handleSelectedVillageChangedEvent() {
        if (!l.v || ROE.SVID != l.v.id) {
            ROE.Villages.getVillage(ROE.SVID, function _handleSelectedVillageChangedEvent_gotVillage(village) {
                ROE.Landmark.gotodb(village.x, village.y);
                ROE.Landmark.select();
            });
        }
        _refill(true);
    }

    function _map_flashmove(rn) {
        ROE.Landmark.gotopx(rn.x, rn.y);
    }


    var o = null;
    var n = null;
    var t = null;

    function _map_tm(e, tt, moveFunc) {
        e.preventDefault();
        if (t) clearTimeout(t);

        if (o == null) {
            o = { x: tt.pageX, y: tt.pageY };
        } else {
            n = { x: tt.pageX, y: tt.pageY };

            var l = ROE.Landmark;
            var dx = n.x - o.x;
            var dy = n.y - o.y;
            var rn = { x: l.rx + dx, y: l.ry + dy };
            l.rx = rn.x;
            l.ry = rn.y;

            o = n;

            if (l.update(tt.pageLastX, tt.pageLastY)) {
                l.paint(tt.pageLastX, tt.pageLastY);
            }
            l.pointdb();

            moveFunc(rn);

            ROE.Animation.mapDrag(l.rx, l.ry);
            t = setTimeout(function () { o = null; }, 300);
        }
    }



    // While developing a map, at first all loaded once canvases were on a page, until you go to a vov, that clears it. 
    // This approach gives crashes to mobile app after of some moving through a map. 
    // So we decided to remove unneeded canvases, but that also gives us crashes. 
    // After all we found that only way to clear memory without crashing is make canvases 1px width and height.
    var _clearInvisibleCanvases = function () {
        BDA.Console.verbose("roe-map", "START ROE.Landmark.clear");

        if ($('#surface').length == 0) { return; }

        var w = l.w;
        var h = l.h;
        var tx = -l.rx;
        var ty = -l.ry;

        $.each(l.canvas.list, function (k, n) {
            var isdel = (n.ax < (tx - w)) || (n.ax > (tx + 2 * w)) || (n.ay < (ty - h)) || (n.ay > (ty + 2 * h));

            if (isdel) {
                BDA.Console.verbose("roe-map", "del " + k);
                n.can.hide().attr('width', '1px').attr('height', '1px');;
                delete ROE.Landmark.earth.c[n.ax + '_' + n.ay];
                delete ROE.Landmark.earth.x[n.ax][n.ay];
                delete ROE.Landmark.canvas.list[k];
            }
        });
        BDA.Console.verbose("roe-map", "END ROE.Landmark.clear");
    }

    // connecting by id to full circular referenced network, and do indexing,
    // for quicker search (village by x+y and by id)
    var _connect = function () {
        $.each(l.players, function (i, n) {
            if (n.CID) n.clan = l.clans[n.CID];
        });

        l.villages_byid = {};

        $.each(l.villages, function (i, n) {
            if (n.pid) n.player = l.players[n.pid];

            n.mine = n.pid == ROE.playerID;
            n.areacoord = _areacoord(n.x, n.y);

            l.villages_byid[n.id] = n;
        });
    }

    var _clearLocal = function () {
        l.earth = { x: {}, y: {}, c: {} };
        l.villages = {};
        l.players = {};
        l.clans = {};
        l.canvas.list = {};

        clearInterval(l.timer.painting);
    }

    var _close = function () {
        _clearLocal();
        $('#map').remove();
        $(document).unbind('player.refresh', _mapPlayerRefreshTrigger);
        l.el = null;
    }



    var _troopPopupOpen = function () {

        if ($(".troopSettingsPopup").length == 0) {
            ROE.UI.Sounds.click();
            var ph = _phrase;

            var popupbox = '<div class=troopSettingsPopup>' + ph('PopupTroopsDescription') + '<BR><BR>';
            popupbox += ' <label><input type="checkbox" option="all">' + ph('PopupTroopsAll') + '</label> <br/>';
            popupbox += ' <label><input type="checkbox" option="your-all">' + ph('PopupTroopsYourAll') + '</label> <br/>';
            popupbox += ' <label><input type="checkbox" option="your">' + ph('PopupTroopsYour') + '</label> <br/>';
            popupbox += ' <label><input type="checkbox" option="support"> ' + ph('PopupTroopsSupport') + '</label> <br/>';
            popupbox += '<input type="button" class="go" value="' + ph('PopupTroopsGo') + '" >';
            popupbox += '</div>';

            //call popup box template (title, width, vertAllign, bgcolor, content)
            if (ROE.isMobile) {
                ROE.Frame.popupInfo(ph("PopupTroopsTitle"), "230px", "center", "rgba(0,0,0,0.3)", popupbox, false);
            } else {
                ROE.Frame.popupInfo(ph("PopupTroopsTitle"), "330px", "center", "rgba(0,0,0,0.3)", popupbox, false);
            }
            var mtt = BDA.Database.LocalGet('MapTroopType');
            if (mtt) {
                var types = mtt.split(',');

                $.each(types, function (i, n) {
                    $(".troopSettingsPopup [option=" + n + "]")[0].checked = true;
                });
            }

            $(".troopSettingsPopup .go").click(function () {
                ROE.UI.Sounds.click();
                var tp = [];

                $('.troopSettingsPopup [option]:checked').each(function (i, n) {
                    tp.push($(n).attr('option'));
                })

                BDA.Database.LocalSet('MapTroopType', tp.join(','));

                ROE.Frame.popupInfoClose($(this));

                _pointdb(true);
            });
        }
    }

    // Consolidation region

    var _promoteListCallback = function (villages) {
        BDA.Database.LocalSet('MapPromote', JSON.stringify(villages));
        l.promotion = villages;
        _refill(true);
    }

    var _mapPlayerRefreshTrigger = function () {
        if (ROE.Realm.isConsolidationAttackFreezeActiveNow) {
            if (!l.timer.promote) {
                function villagePromoteList() {
                    if (ROE.Realm.isConsolidationAttackFreezeActiveNow) {
                        ROE.Api.call('village_promote_list', { normal: false },
                            function (data) {
                                _promoteListCallback(data);
                            }
                        );
                    } else {
                        clearInterval(l.timer.promote);
                    }
                }

                l.timer.promote = setInterval(villagePromoteList, 1000 * 60 * 5);
                villagePromoteList();
            }
        }

        if (ROE.Realm.isConsolidationCompleted) {
            // first time it gets IsConsolidationCompleted
            // asks for villages list - normal part, that exists after removal
            // than gets all villages from websql and which are not 
            if (!BDA.Database.LocalGet('MapPromoteDone')) {
                BDA.Database.Delete(['Villages', 'VillagesMy']).done(function () {
                    BDA.Database.LocalSet('MapPromoteDone', 'true');
                    _refresh();
                });
            }
        }
    }

    var _addConsolidationOptions = function (vid, container, helpArea) {
        var absorbed = _villageAbsorbed(vid);
        var promoted = _villagePromoted(vid);

        var promElem = $('<div class="promSet"></div>');
        if (!absorbed && !promoted) {
            promElem.append($('<a class="prom"></a>').click(function () { villPromoteAddRemove('add'); }));
            ROE.helper_AddHelpTextToElement($('a.prom', promElem), helpArea, 'Promote Village');
        } else if (promoted) {
            promElem.append($('<a class="unprom"></a>').click(function () { villPromoteAddRemove('remove'); }));
            ROE.helper_AddHelpTextToElement($('a.unprom', promElem), helpArea, 'Demote Village');
        } else {
            return;
        }
        promElem.append($('<a class="promInfo"></a>').click(_promotionPopupOpen));
        ROE.helper_AddHelpTextToElement($('a.promInfo', promElem), helpArea, 'What\'s this?');
        container.append(promElem);

        function villPromoteAddRemove(op) {
            ROE.Api.call('village_promote_' + op, { vid: vid }, function VillagePromotedSuc(data) {
                _promoteListCallback(data);
                _pointdb(true);
            });
        }
    }

    var _promotionPopupOpen = function () {
        if ($("#popupInfo .promotionPopup").length == 0) {
            ROE.UI.Sounds.click();
            var ph = _phrase;

            var popupbox = $('#promotionPopupTemplate').html();

            ROE.Frame.popupInfo(ph("PopupPromotionTitle"), "280px", "center", "rgba(0,0,0,0.3)", popupbox, false);

            var popup = $("#popupInfo .promotionPopup");

            BDA.Timer.initTimers();

            //         $(".promote, .unpromote", popup).hide();

            $(".okBtn", popup).show().click(function () { ROE.Frame.popupInfoClose($(this)); });
            //function villPromoteAddRemove(op, but) {
            //    ROE.Api.call('village_promote_' + op, { vid: l.v.id }, function VillagePromotedSuc(data) {
            //        _promoteListCallback(data);
            //        _pointdb(true);
            //        ROE.Frame.popupInfoClose(but);
            //    });
            //}

            //if (!_villageAbsorbed(l.v.id) && !_villagePromoted(l.v.id)) {
            //    $(".promote", popup).show().click(function () { villPromoteAddRemove('add', $(this)); });
            //}

            //if (_villageAbsorbed(l.v.id) || _villagePromoted(l.v.id)) {
            //    $(".unpromote", popup).show().click(function () { villPromoteAddRemove('remove', $(this)); });
            //}
        }
    }

    var _villageAbsorbed = function (id) {
        return $.grep(l.promotion.absorbed, function (n) { return n == id }).length > 0;
    }

    var _villagePromoted = function (id) {
        return $.grep(l.promotion.promoted, function (n) { return n == id }).length > 0;
    }

    var _refresh = function () {
        BDA.Database.LocalDel('MapVillArea');

        l.villages = {};
        l.players = {};
        l.clans = {};
        l.villcoords = {};

        // user is on map right now
        if (l.el) {
            var allCoordsLoaded = $.map($('.earth', l.el), function (n) { return $(n).attr('ind'); });

            _moredata({ coords: allCoordsLoaded }, function () { _pointdb(true); _refill(true); });
        }
    }

    var _getActionBarItems = function _getActionBarItems(doact, villageURL, v, helpArea) {

        doact.append(ROE.getVOVAnchor(villageURL, v.mine, v.id));
        doact.append(ROE.getProperAttackAnchor(v.id, v.x, v.y, false, helpArea));
        doact.append(ROE.getProperSupportAnchor(v.id, v.x, v.y, v.pid, false, helpArea));
        // if (ROE.loginMode !== "mobileonly") { doact.append(ROE.getSupportAnchor_OneClick(v.id, v.x, v.y, v.pid, false, helpArea)); }
        if (ROE.loginMode !== "mobileonly") { doact.append(ROE.getHQAnchor(v.id, v.mine, false, helpArea)); }
        if (ROE.loginMode !== "mobileonly") { doact.append(ROE.getRecruitAnchor(v.id, v.mine, false, helpArea)); }
        doact.append(ROE.getSendSilverAnchor(v.id, v.mine, v.x, v.y, v.pid, helpArea, false));
        doact.append(ROE.getSupportLookupAnchor(v.id, v.mine, false, helpArea, v.pid));

        if (v.mine) {
            //doact.append('<a href="#" onclick="ROE.Utils.RenameVillage(' + v.id + ', $(\'#vName\')[0], null, ROE.Utils.updateVillageName)" class="renV sfx2"></a>');
            doact.append('<a href="#" onclick="ROE.UI.Sounds.click(); ROE.Utils.PopupRenameVillage();" class="renV"></a>');
            ROE.helper_AddHelpTextToElement($('.doact a.renV'), helpArea, 'Rename this village');
        } else {
            doact.append($('<div class="na renV_na"></div>')); //inactive rename
        }

        doact.append($('<a class="highlight"></a>').click(function (e) { ROE.UI.Sounds.click(); ROE.Frame.popupHighlight(e); }));

        _setupVillageClaimButton(v, doact);
        _setupReportsButton(v, doact);
        _setupVillageNoteButton(v, doact);
        _setupVillageTargetButton(v, doact);

        if (v.mine) {
            //item rewards popup link
            doact.append('<a href="#" onclick="ROE.UI.Sounds.click(); ROE.Items2.showPopup();" class="itemsPopup"></a>');
            //consolidation promo stuff
            if (ROE.Realm.isConsolidationAttackFreezeActiveNow) {
                _addConsolidationOptions(v.id, doact, helpArea);
            }
        }

        //must be last entry in GUI
        _setupPresetButtons(v, doact);

    }

    var _populateD2MapGuiButtons = function _populateD2MapGuiButtons(v) {

        var villageURL = v.mine ? BuildURL('VillageOverview.aspx', ['svid=' + v.id]) : BuildURL('VillageOverviewOtherPopup.aspx', ['ovid=' + v.id]);
        var helpArea = $('<div class="help fontSilverFrLCmed"></div>');
        var mapGuiButtons = $('#mapGuiButtons').empty();

        mapGuiButtons.append(ROE.getVOVAnchor(villageURL, v.mine, v.id));
        mapGuiButtons.append(ROE.getProperAttackAnchor(v.id, v.x, v.y, false, helpArea));
        mapGuiButtons.append(ROE.getProperSupportAnchor(v.id, v.x, v.y, v.pid, false, helpArea));
        mapGuiButtons.append(ROE.getHQAnchor(v.id, v.mine, false, helpArea));
        mapGuiButtons.append(ROE.getRecruitAnchor(v.id, v.mine, false, helpArea));
        mapGuiButtons.append(ROE.getSendSilverAnchor(v.id, v.mine, v.x, v.y, v.pid, helpArea, false));
        mapGuiButtons.append(ROE.getSupportLookupAnchor(v.id, v.mine, false, helpArea, v.pid));
        mapGuiButtons.append($('<a class="highlight"></a>').click(function (e) { ROE.UI.Sounds.click(); ROE.Frame.popupHighlight(e); }));
        mapGuiButtons.append(ROE.getGiftsAnchor(v.id, v.mine, false, helpArea));

        ROE.helper_AddHelpTextToElement($('a.othervov', mapGuiButtons), helpArea, 'View village profile');
        ROE.helper_AddHelpTextToElement($('a.highlight', mapGuiButtons), helpArea, 'Change map shield colors');

        if (v.mine) {
            mapGuiButtons.append('<a href="#" onclick="ROE.UI.Sounds.click(); ROE.Utils.PopupRenameVillage(' + v.id + ',\'' + v.name + '\');" class="renV"></a>');
            ROE.helper_AddHelpTextToElement($('a.renV', mapGuiButtons), helpArea, 'Rename this village');

            if (ROE.Realm.isConsolidationAttackFreezeActiveNow) {
                _addConsolidationOptions(v.id, mapGuiButtons, helpArea);
            }

        } else {
            mapGuiButtons.append('<div class="na renV_na"></div>');
        }

        _setupVillageClaimButton(v, mapGuiButtons);
        _setupReportsButton(v, mapGuiButtons);
        _setupVillageNoteButton(v, mapGuiButtons);
        _setupVillageTargetButton(v, mapGuiButtons);

        _setupPresetButtons(v, mapGuiButtons);

        mapGuiButtons.append(helpArea);
    }

    //makes a village claim button for a given gui container
    function _setupVillageClaimButton(village, guiContainer) {

        guiContainer.append('<a class="claimVillage" data-claimstatus="' + village.claimedStatus + '" ></a>');

        if (ROE.Player.Clan) {

            if (village.claimedStatus_otherClan) {
                $('.claimVillage', guiContainer).addClass('purple');
            }

            $('.claimVillage', guiContainer).click(function () {
                if ($('.claimVillage', guiContainer).hasClass('busy')) {
                    return;
                }
                if (parseInt($(this).attr('data-claimstatus')) === 0) {
                    $(this).addClass('busy');
                    ROE.Api.call_claimVillage(village.id, _villageClaimCallback);
                } else if (parseInt($(this).attr('data-claimstatus')) === 1) {
                    $(this).addClass('busy');
                    ROE.Api.call_claimVillage_unclaim(village.id, _villageClaimCallback);
                } else {
                    ROE.Frame.infoBar('Village already claimed by clanmate');
                }

            }).hover(function () {
                var claimStatus = parseInt($(this).attr('data-claimstatus'));
                if (claimStatus === 0) {
                    if (village.claimedStatus_otherClan) {
                        $('.help', guiContainer).html('Village is claimed by Allied clan. Click to claim for yourself.');
                    } else {
                        $('.help', guiContainer).html('Village is unclaimed. Click to claim.');
                    }
                } else if (claimStatus === 1) {
                    $('.help', guiContainer).html('Village is claimed by you. Click to unclaim.');
                } else if (claimStatus === 2) {
                    $('.help', guiContainer).html('Village is already claimed by a clanmate.');
                }
            });

        } else {
            $('.claimVillage', guiContainer).addClass('na');
        }

        function _villageClaimCallback(data) {
            var resultingStatus = data.claimResults;
            l.villages_byid[data.villageID].claimedStatus = resultingStatus;
            $('.claimVillage', guiContainer).attr('data-claimstatus', resultingStatus).removeClass('busy');
            $('.help', guiContainer).empty();
            _refill(true);
            if (resultingStatus === 0) {
                ROE.Frame.infoBar('Claim removed');
            } else if (resultingStatus === 1) {
                ROE.Frame.infoBar('Village claimed!');
            } else if (resultingStatus === 2) {
                ROE.Frame.errorBar('Village already claimed by clanmate');
            } else if (resultingStatus === 3) {
                ROE.Frame.errorBar('Max number of allowed claims reaches');
            }
        }

    }

    function _setupReportsButton(village, guiContainer) {
        guiContainer.append('<a class="launchReports"></a>');

        $('.launchReports', guiContainer).click(function launchReportsClick() {
            ROE.Frame.popupReports('(' + village.x + ',' + village.y + ')');
        }).hover(function () {
            $('.help', guiContainer).html('See your reports on this village');
        });
    }

    function _setupVillageNoteButton(v, guiContainer) {
        guiContainer.append('<a class="editVillageNote"></a>');

        $('.editVillageNote', guiContainer).click(function editVillageNote() {
            ROE.Frame.popupVillageNote(v);
        }).hover(function () {
            $('.help', guiContainer).html('Edit village note');
        });
    }

    //sets up target support and target attack in a given map GUI container
    function _setupVillageTargetButton(v, guiContainer) {

        //target support
        guiContainer.append('<a class="targetSupport"></a>');
        var targetBtn = $('.targetSupport', guiContainer);
        targetBtn.click(function mapGuiOpenTarget() {
            _mapGuiHideSelected();
            //if target exists on village, open village target list, else open a new Support target UI
            if (ROE.Targets.returnPriorityTargetFromVillageID(v.id)) {
                ROE.Targets.open(v);
            } else {
                ROE.Targets.openNew(v, 1);
            }

        }).hover(function () {
            $('.help', guiContainer).html('Call for Support on this village.');
        });

        //target attack
        guiContainer.append('<a class="targetAttack"></a>');
        var targetBtn = $('.targetAttack', guiContainer);
        targetBtn.click(function mapGuiOpenTarget() {
            _mapGuiHideSelected();
            //if target exists on village, open village target list, else open a new Attack target UI
            if (ROE.Targets.returnPriorityTargetFromVillageID(v.id)) {
                ROE.Targets.open(v);
            } else {
                ROE.Targets.openNew(v, 2);
            }
        }).hover(function () {
            $('.help', guiContainer).html('Call for Attack on this village.');
        });

    }

    function _setupPresetButtons(v, guiContainer) {
        var mapGuiPresetPanel = $('<div class="mapGuiPresetPanel"></div>');
        ROE.PresetCommands.populateMapGuiPresetButtons(mapGuiPresetPanel, v);
        guiContainer.append(mapGuiPresetPanel);
    }

    //given a village object, find a target on the village
    //populate the targetsArea given based n target info
    function _setupVillageTargetArea(v, targetsArea) {

        //WARNING: target will have a referrence to player targets object, dont mess with its data.

        targetsArea.empty();

        //gets target information on a particular village {target: x, count: x}
        var villageTargetInfo = ROE.Targets.returnPriorityTargetFromVillageID(v.id);

        if (villageTargetInfo) {

            var target = villageTargetInfo.target;
            var count = villageTargetInfo.count;
            var targetContent = $('<div class="targetContent"></div>');
            var tIcon = $('<div class="tIcon type' + target.Type + '"></div>').appendTo(targetContent);

            ///

            //setup verbage
            var typeLabel = 'Act on';
            if (target.Type == 1) {
                typeLabel = 'Support';
            } else if (target.Type == 2) {
                typeLabel = 'Attack';
            }
            var verbage = '<div class="tVerbage">' + typeLabel + ' this village </div>';

            //setup target player assignment
            var assignedElement = "";
            if (target.AssingedToPlayerID) {
                var assignedElement = $('<div class="tAssigned">' + target.AssignedToPlayerName + ' </div>');
                //if assigned to you, wed need a special class
                if (target.AssingedToPlayerID == ROE.Player.id) {
                    targetContent.addClass('assignedToMe');
                }
            }

            //try to get settime, if still in the future, make a countdown element
            var setTime = ROE.Utils.cDateToJsDate(target.SetTime);
            var timerElement = "";
            if (setTime > Date.now()) {
                var timerElement = $('<div class="tSetTime">by <span class="tTimer countdown" data-finisheson="' +
                    setTime.valueOf() + '" data-refreshcall="ROE.Landmark.mapGuiTargetSetTimeout();" ></span></div>');
            }

            //setup target note
            var noteElement = "";
            if (target.Note) {
                var noteString = target.Note;
                if (noteString.length > 100) {
                    noteString = noteString.substring(0, 100) + '...';
                }
                noteElement = '<div class="tNote">Note: ' + noteString + '</div>';
            }


            //add all the bits
            targetContent.append(assignedElement, verbage, timerElement, noteElement);

            targetsArea.append(targetContent);
            targetsArea.show().unbind().click(function () {
                //ROE.Targets.openTargetOnVillageCords(v.x, v.y);
                _mapGuiHideSelected();
                ROE.Targets.open(v, target);
            });

            //at the end we add a live created-ago ticker
            var timeCreated = $('<div class="tCreated" data-time="' + ROE.Utils.cDateToJsDate(target.TimeCreated).valueOf() + '"></div>').appendTo(targetContent);
            BDA.Utils.formatTimeDifferenceLive(timeCreated);

            //if more than 1 target on village
            if (villageTargetInfo.count > 1) {
                var multipleTargets = $('<div class="tMoreTargets">multiple targets on village, click to see all</div>')
                multipleTargets.click(function (event) {
                    event.stopPropagation();
                    _mapGuiHideSelected();
                    ROE.Targets.open(v);
                });
                var multipleTargetsCounter = $('<div class="tMoreTargetsCounter">' + villageTargetInfo.count + '</div>');
                targetContent.append(multipleTargets, multipleTargetsCounter);
            }


        } else {
            targetsArea.hide();
        }

    }

    function _mapGuiTargetSetTimeout(element) {
        console.log('_mapGuiTargetSetTimeout', element);
    }


    function _landmarkChangePositionTroops(village, isLoad) {

        var types = [];
        var mapTroopsType = BDA.Database.LocalGet('MapTroopType');

        if (!mapTroopsType) {
            mapTroopsType = 'all';
            BDA.Database.LocalSet('MapTroopType', mapTroopsType);
        }

        types = mapTroopsType.split(',');

        var p = _pointdb();
        var v = l.villages[p.x + '_' + p.y];
        var finishedHTML;
        if (v && v.id == village.id) {

            if (!isLoad)
                ROE.Villages.ExtendedInfo_loadLatest(village.id);

            if (village._TroopsDictionary == undefined) {
                finishedHTML = BDA.Templates.populate(l.troopInfoLoadingTemplate[0].outerHTML, village._TroopsDictionary);
            } else {
                finishedHTML = BDA.Templates.populate(l.troopInfoTemplate[0].outerHTML, village._TroopsDictionary);
            }

            if (ROE.isD2) {
                var container = $('#mapGuiSelected .mapGuiTroop').html(finishedHTML).fadeIn(100);
                $('.unitHdr', container).hide();
                for (var i = 0; i < types.length; i++) {
                    $('[data-type=' + types[i] + ']', container).show();
                }
            } else {
                troop.html(finishedHTML).show().click(_troopPopupOpen);
                $('.unitHdr', troop).hide();
                for (var i = 0; i < types.length; i++) {
                    $('[data-type=' + types[i] + ']', troop).show();
                }
            }

        }
    }

    //populate troops for D2 hover GUI
    function _populateHoverTroops(village, cached) {

        var mapGuiHover = $('#mapGuiHover');

        //hovered over village data stored
        var v = mapGuiHover.data('v');

        if (!v || !v.mine) {
            //if no village or village is not ours, hide troops
            $('.mapGuiTroop', mapGuiHover).stop().hide();
            return;
        }

        if (v.id != village.id) {
            //discard data that comes in for a different village than what we are looking at
            return;
        }

        var types = [];
        var mapTroopsType = BDA.Database.LocalGet('MapTroopType');

        if (!mapTroopsType) {
            mapTroopsType = 'all';
            BDA.Database.LocalSet('MapTroopType', mapTroopsType);
        }

        types = mapTroopsType.split(',');

        var finishedHTML;

        if (!cached) {

            clearTimeout(timer_mapGuiHover);
            timer_mapGuiHover = setTimeout(function mapGuiHoverExtendedInfo_loadLatest() {
                ROE.Villages.ExtendedInfo_loadLatest(village.id);
            }, 500);

        }

        if (village._TroopsDictionary == undefined) {
            finishedHTML = '<div class="loading fontGrayFrLCmed">Loading troop information.</div>';
        } else {
            finishedHTML = BDA.Templates.populate(l.troopInfoTemplate[0].outerHTML, village._TroopsDictionary);
        }

        var troopContainer = $('.mapGuiTroop', mapGuiHover).html(finishedHTML).fadeIn(300);
        $('.unitHdr', troopContainer).hide();
        for (var i = 0; i < types.length; i++) {
            $('[data-type=' + types[i] + ']', troopContainer).show();
        }

    }


    var troop;

    // Executed on start() once sets l.point_changed = [function(x, y)]. 
    // When select() called, it calls pointdb() which checks if its other place village executes this event – point_changed. 
    // Note: current selected village, or land x,y can be found at l.px, l.p.y. 
    // point_changed is big, but in general it just show information from l.villages and ajax loading troops.
    var _M_info = function () {
        var hmap = $('header .hmap');
        var td = $('td', hmap);

        var name = $('.name', hmap);
        var player = $('.player', hmap);
        var clan = $('.clan', hmap);

        var doact = hmap.filter('.doact');
        troop = hmap.filter('.troop').hide();
        var info = hmap.filter('.info');

        info.unbind('click').bind('click', function (e) {
            e.preventDefault();
            ROE.UI.Sounds.click();
            $('.headernav').toggleClass("clicked");
        });

        l.point_changed = [function (x, y) {
            if ($('#map').length == 0) return;

            var v = l.villages[x + '_' + y];
            l.v = v;

            troop.hide();

            //clean up and add new targets area
            $('.hmap.targetsArea').remove();


            if (v) {
                var p = v.player;
                _info_tutorial(p);

                var c = p.clan;
                var villageURL = v.mine ? BuildURL('VillageOverview.aspx', ['svid=' + v.id]) : BuildURL('VillageOverviewOtherPopup.aspx', ['ovid=' + v.id]);

                //create and setup targetsArea 
                var targetsArea = $('<div class="hmap targetsArea"></div>').insertBefore(troop);
                _setupVillageTargetArea(v, targetsArea);


                info.html(_M_villageInfoBuilder(v));
                $('.player-avatar', info).click(function _userNameClick(e) {
                    e.preventDefault();
                    ROE.Frame.popupPlayerProfile($(this).parent().find('.pn').html());
                });

                doact.empty();

                var helpArea = doact.append('<div class="help sfx2" style="width: 300px"></div>').find('.help');
                var vovLink = hmap.find('#villName');

                ROE.getAnchorMode_IconOnly = true;

                _getActionBarItems(doact, villageURL, v, helpArea);

                if (v.mine) {

                    if (BDA.Database.LocalGet('MapTroopType')) {
                        ROE.Villages.getVillage(v.id, _landmarkChangePositionTroops);
                    } else {
                        troop.empty().show()
                            .append($('<span class="displayTroopsMsg" >' + _phrase('TroopsNoSelectedTypes') +
                                    '<img style="margin-left: 10px" src="https://static.realmofempires.com/images/icons/M_MoreSettings.png" height="14px" class="troopSettings" /></span>').click(_troopPopupOpen));
                    }
                }

                var inOutContainer = $('<div>').addClass('inOutContainer');
                inOutContainer.append(ROE.getIncomingToVillageAnchor(v.id, v.name, v.x, v.y, v.pid));
                inOutContainer.append(ROE.getIncomingToPlayerAnchor(v.pid, p.PN));
                inOutContainer.append(ROE.getOutgoingFromVillageAnchor(v.id, v.name, v.x, v.y, v.pid));
                inOutContainer.append(ROE.getOutgoingFromPlayerAnchor(v.pid, p.PN));
                doact.append(inOutContainer);

                $("header").show();
                if (v.mine && v.id != ROE.SVID) {
                    // fire event telling everyone that currently selected village has changed - this is guaranteed to be your village 
                    ROE.Frame.setCurrentlySelectedVillage(v.id, v.x, v.y, v.name);
                }

                // fire event telling everyone that selected village on map has changed - this may not be your village, could be any village on the map
                BDA.Broadcast.publish("MapSelectedVillageChanged", {
                    villageID: v.id, ownerPlayerID: v.player.id, Cord: { x: v.x, y: v.y }
                });

            } else {
                name.html('<br /><br /><br />');
                $('#map').attr('data-tutorial-rebel-selected', 'false');
                $("header").hide();
            }

        }];
    };

    var _M_villageInfoBuilder = function (v) {
        var p = v.player;
        var c = p.clan;
        var coords = ' (' + v.x + ',' + v.y + ')';
        var note = v.note;
        if (note && note.length && note != '0' && v.note.length >= 30) {
            note += '...';
        }
        return '<div class="head">' +
                (!ROE.isSpecialPlayer(v.pid) ? '<img class="player-avatar" src="' + ROE.Avatar.list[p.Av].imageUrlS + '" height="40px" style="float: left; margin: 2px 5px 0 2px" />' : '') +
                '<div class=onel><span class=area>[A' + v.area + ']</span>&nbsp;<span id="vName">' + v.name + '</span>&nbsp;<span>' + coords + '</span><span class=points>[' + BDA.Utils.formatNum(~~v.points) + ']</span>' +
                (note != '0' ? '<span class="note"> ' + note + '</span>' : '')
                + '</div> <div class=onel>' +
                (!ROE.isSpecialPlayer(v.pid) ? ('<span class="pn">' + p.PN + '</span> <span class=points>[' + ROE.Utils.formatShortNum(p.PP) + ']</span>') : '&nbsp;') +
                (p.Pe != '0' ? '<span class="note"> ' + p.Pe + '</span>' : '')
                + '</div> <div class=onel>' +
                (c ? c.CN + (c.CP != '0' ? '<span class=points> [' + ROE.Utils.formatShortNum(c.CP) + ']</span>' : '') : '&nbsp;</div>') +
                '</div><div class=do sfx2></div>';
    }

    function _info_tutorial(p) {
        if ($('#map').is('[data-tutorial-1move-start]')) {
            $('#map').attr('data-tutorial-1move', $('#map').is('[data-tutorial-1move]') ? 'false' : 'true');
        }
        if ($('#map').is('[data-tutorial-1move=false]')) {
            $('#map').attr('data-tutorial-rebel-selected', p.id == ROE.CONST.specialPlayer_Rebel);
        }
    }

    var _D2_info = function () {

        l.point_changed = [function (x, y) {

            ROE.getAnchorMode_IconOnly = true;
            var v = l.villages[x + '_' + y];
            l.v = v;
            if (v) {
                _info_tutorial(v.player);

                _D2_SelectedMapGuiUpdate(v);

                if (v.mine) {
                    ROE.Villages.getVillage(v.id, _landmarkChangePositionTroops);
                } else {
                    $('.mapGuiTroop', mapGuiSelected).stop().hide();
                }

                if (v.mine && v.id != ROE.SVID) {
                    // fire event telling everyone that currently selected village has changed - this is guaranteed to be your village 
                    ROE.Frame.busy('Travelling to Village...', 5000, $('#vovFrame .vovFrameContent'));
                    ROE.Frame.setCurrentlySelectedVillage(v.id, v.x, v.y, v.name);
                }

                // fire event telling everyone that selected village on map has changed - this may not be your village, could be any village on the map
                BDA.Broadcast.publish("MapSelectedVillageChanged", {
                    villageID: v.id, ownerPlayerID: v.player.id, Cord: { x: v.x, y: v.y }
                });

            } else {
                $('#map').attr('data-tutorial-rebel-selected', 'false');
                _mapGuiHideSelected();
            }
        }];
    };

    var _D2_SelectedMapGuiUpdate = function _D2_SelectedMapGuiUpdate(v) {

        var mapGuiSelected = $('#mapGuiSelected');

        var inOutContainer = $('<div class="inOutContainer">');
        inOutContainer.append(ROE.getIncomingToVillageAnchor(v.id, v.name, v.x, v.y, v.pid));
        inOutContainer.append(ROE.getIncomingToPlayerAnchor(v.pid, v.player.PN));
        inOutContainer.append(ROE.getOutgoingFromVillageAnchor(v.id, v.name, v.x, v.y, v.pid));
        inOutContainer.append(ROE.getOutgoingFromPlayerAnchor(v.pid, v.player.PN));
        $('.mapGuiInout', mapGuiSelected).html(inOutContainer);

        _populateD2MapGuiPlayerInfo(v, mapGuiSelected);
        _populateD2MapGuiButtons(v);
    }

    var _populateD2MapGuiPlayerInfo = function _populateD2MapGuiPlayerInfo(village, container) {

        var player = village.player;
        var clan = player.clan;
        var avatar = (!ROE.isSpecialPlayer(village.pid) ? ROE.Avatar.list[player.Av].imageUrlS : 'https://static.realmofempires.com/images/npc/Av_Rebel.png'); //not good, what about abandoned?

        container.find('.pAvatarPicture').css('background-image', 'url(\'' + avatar + '\')').css('cursor', 'pointer').unbind().click(function _userNameClick() {
            if (player.PN && !ROE.isSpecialPlayer(village.pid)) {
                ROE.UI.Sounds.click();
                ROE.Frame.popupPlayerProfile(player.PN);
            }
        });

        container.find('.pVillageInfo').html(village.name + ' (' + village.x + ',' + village.y + ')[' + BDA.Utils.formatNum(village.points) + ']');
        container.find('.pName').html(!ROE.isSpecialPlayer(village.pid) ? player.PN + '[' + ROE.Utils.formatShortNum(player.PP) + ']' : player.PN);
        container.find('.pClan').html((clan ? clan.CN + (clan.CP != '0' ? '[' + ROE.Utils.formatShortNum(clan.CP) + ']' : '') : ''));

        if (village.note && village.note.length && village.note != '0') {
            if (village.note.length >= 30) {
                container.find('.mapGuiNote').html(village.note + '...').show();
            } else {
                container.find('.mapGuiNote').html(village.note).show();
            }
        } else {
            container.find('.mapGuiNote').hide().html('');
        }

        _setupVillageTargetArea(village, container.find('.mapGuiTargets'));

    }

    var _mapGuiShowSelected = function _mapGuiShowSelected(v) {

        $('#mapGuiHover').stop().hide();

        var sel = $('#select');
        var selOffset = sel.offset();
        var mapGuiSelected = $('#mapGuiSelected').attr('vid', v.id);
        var mapGuiButtons = $('#mapGuiButtons');

        mapGuiSelected.stop().css({
            'left': selOffset.left - mapGuiSelected.width() / 2 + sel.width() / 2,
            'bottom': -(selOffset.top - $(window).height()) - 2
        }).show();/*.fadeIn(200)*/

        mapGuiButtons.stop().css({
            'left': selOffset.left - mapGuiButtons.width() / 2 + sel.width() / 2,
            'top': selOffset.top /*+ sel.height()*/ + 2
        }).show();/*.fadeIn(200)*/

    }

    var _mapGuiHideSelected = function _mapGuiHideSelected() {
        $('#mapGuiSelected, #mapGuiButtons').stop().fadeOut(50);
    }

    var _mapGuiHideHover = function _mapGuiHideHover() {
        $('#mapGuiHover').stop().hide();/*.fadeOut(200);*/

    }

    var _hoverOverVillage = function (pos) {

        var lp = l.landpx;
        var p = {
            x: Math.floor((pos.x - l.rx) / lp), y: Math.ceil((l.ry - pos.y) / lp)
        };
        var hp = p.x + '_' + p.y;

        if (hp == l.hp) {
            //this little bit fixes a small issue with map gui where sometimes hovering a close village after selection wasn't working.
            var vtemp = l.villages[hp];
            if (vtemp) {
                var guiSelectedVid = parseInt($('#mapGuiSelected').attr('vid'));
                if (guiSelectedVid) {
                    if (vtemp.id != guiSelectedVid) {
                        _mapGuiHideSelected();
                        $('#mapGuiHover').stop().fadeIn(200);
                    }
                }
            }
            return;
        };

        var v = l.villages[hp];
        l.hp = p.x + '_' + p.y;

        if (v) {
            $('#mapwrap').css('cursor', 'pointer');
            _mapGuiHideSelected();

            var lt = {
                left: p.x * lp + l.rx, top: -p.y * lp + l.ry
            };
            var mapGuiHover = $('#mapGuiHover');
            mapGuiHover.data('v', v);
            _populateD2MapGuiPlayerInfo(v, mapGuiHover);
            mapGuiHover.stop().fadeIn(200).css({
                'left': lt.left - mapGuiHover.width() / 2 + lp / 2 + 10, //the 10 is adjusting for the the thickness of side
                //'top': lt.top - mapGuiHover.height()
                'bottom': -(lt.top - $(window).height())
            });

            if (v.mine) {
                ROE.Villages.getVillage(v.id, _populateHoverTroops);
            } else {
                $('.mapGuiTroop', mapGuiHover).stop().hide();
            }

        } else {
            $('#mapwrap').css('cursor', 'default');
            _mapGuiHideHover();
            _mapGuiHideSelected();
        }
    }

    var _updateVillageName = function (newName, id) {
        BDA.Database.UpdateNoId('Villages', {
            id: 'id', fields: [{ 'id': id, 'name': newName }]
        }, function () {
        });

        var v = l.v;
        if (typeof v != 'undefined') {
            v.name = newName;
        }
    }

    bigimgs = {
    };



    //Handles Prelaoding and Painting to Canvas
    l.canvas = {
        list: {
        },
        // preload imgs, and cache it in bigimgs array
        // because of preloading for many canvases lying on dom, we dont care when ex
        preload: function (file, cb, data) {
            if (bigimgs[file]) {
                cb(bigimgs[file], data); return;
            }

            var img = new Image();
            img.onload = function (e) {
                bigimgs[e.target.src] = e.target;
                cb(e.target, data);
                l.canvas.preload_count--;
                if (l.canvas.preload_count == 0) {
                    $('#surface .earth.old').hide().attr('width', '1px').attr('height', '1px');
                }
            };
            img.src = file;
            this.preload_count++;
        },
        preload_count: 0,

        // Responsible for draw info on canvas. Draw happened in stages:
        // 1. Drawing of green background
        // 2. Going through each of 6x6 land and check if there is village or landmark for it.
        // 3. If village drawing, there are also highlights flags to draw, support/attack icons and consolidation images – which village is promoted and which absorbed.
        // Notes:
        // 1. To draw it converts global db coordinates into local canvas coordinate (xx, yy) in px starting point
        // 2. All images ever loaded stored inside bigimgs object in form of { “<url>”: Image() }
        // 3. preload() function returns Image from bigimgs or set new Image().onload event.
        paint: function (canvas, bx, by, cb) {
            var sf = l;
            var lc = sf.canvas;
            var lp = sf.landpx;
            var ep = sf.earthpx;
            var tdc = 6; // tile db coords size
            var ctx = canvas.getContext("2d");

            // bx, by - bottom left x, y (db coords) landmark, that is starting point for drawing 6x6 earth tile
            bx = ~ ~bx;
            by = ~ ~by;

            var path_mapdir = 'https://static.realmofempires.com/images/map/';

            var troopsmove = BDA.Database.LocalGet('TroopDirection') == 'outgoing' ? ROE.Troops.InOut2.getOutgoingData() : ROE.Troops.InOut2.getIncomingData();

            lc.preload(GLOBAL_MAP_BIGTILE, function (img) {
                ctx.drawImage(img, 0, 0, sf.earthpx, sf.earthpx);

                for (var mapX = bx; mapX < bx + tdc; mapX++) {
                    for (var mapY = by; mapY < by + tdc; mapY++) {
                        // this code looks awkward, but unfortunatelly for now I dont know how to collapse it
                        // that is because of how % works: 
                        // -7 % 6 -6 % 6  -5 % 6  -3 % 6  -1 % 6  0 % 6  3 % 6  7 % 6
                        //   -1      0      -5      -3      -1      0      3      1
                        // thats why we need to handle 0, - and + differently
                        var xx = 0;
                        var x1 = mapX % tdc;
                        if (mapX < 0) {
                            xx = (5 * lp) + ((x1 == 0 ? -tdc : x1) * lp + lp);
                        }
                        else {
                            xx = x1 * lp;
                        }

                        var yy = 0;
                        var y1 = (mapY - 1) % tdc;

                        if (mapY < 0) {
                            yy = -((y1 == 0 ? -tdc : y1) * lp + lp);
                        } else if (mapY == 0) {
                            yy = 0;
                        } else {
                            yy = (5 * lp) - (y1 * lp);
                        }

                        //check if a landmark exists, paint it
                        var n = l.land[mapX + '_' + mapY];
                        if (n) {

                            lc.preload(path_mapdir + n.image, function (img, d) {
                                d.ctx.drawImage(img, d.x, d.y, sf.landpx, sf.landpx);

                                var v = l.villages[d.mapX + '_' + d.mapY];
                                if (v) {
                                    _paintVillage(d.ctx, v, d.x, d.y);
                                } else {
                                    _checkPaintEvent(d.ctx, d.mapX, d.mapY);
                                }

                            }, {
                                ctx: ctx, x: xx, y: yy, mapX: mapX, mapY: mapY
                            });

                        } else {

                            //if no landmark, check for village
                            var v = l.villages[mapX + '_' + mapY];
                            if (v) {
                                _paintVillage(ctx, v, xx, yy);
                            } else {
                                _checkPaintEvent(ctx, mapX, mapY);
                            }

                        }
                    }
                }

            });

            //xx and yy are canvas pixel coords not map coords
            function _paintVillage(context, v, xx, yy) {

                lc.preload(BDA.Utils.GetMapVillageIconUrl(v.points, v.type), function PaintVillage(img, d) {
                    d.ctx.drawImage(img, d.x, d.y, sf.landpx, sf.landpx);

                    var flag1 = ROE.Map.Highlights.getflag(d.v);
                    flag1 = flag1 == null ? d.v.flag : flag1;

                    if (d.v.pid === ROE.playerID && d.v.id === ROE.SVID) {
                        flag1 = 'Shield2b_Black.png';
                    }

                    if (d.v.pid === ROE.playerID && l.promotion) {
                        if (_villagePromoted(d.v.id)) {
                            flag1 = 'Shield4_Yellow.png';
                        }
                        if (_villageAbsorbed(d.v.id)) {
                            flag1 = 'Shield4_Teal.png';
                        }
                    }

                    //figure out claim status icon
                    var claimStatusIcon = null;
                    if (d.v.claimedStatus === 1) {
                        claimStatusIcon = 'https://static.realmofempires.com/images/icons/claimeD_myTiny.png';
                    } else if (d.v.claimedStatus === 2) {
                        claimStatusIcon = 'https://static.realmofempires.com/images/icons/claimeD_otherTiny.png';
                    }

                    //if a village flag exists paint it
                    if (flag1) {
                        lc.preload(path_mapdir + flag1, function (img, d2) {
                            d2.ctx.drawImage(img, d2.x, d2.y, 18, 20);

                            //paint other ally claim
                            if (d.v.claimedStatus_otherClan == 1) {
                                lc.preload('https://static.realmofempires.com/images/icons/claimeD_otherAllied.png', function (img, d3) {
                                    d3.ctx.drawImage(img, d3.x + 11, d3.y + 9, 9, 9);
                                }, {
                                    ctx: d2.ctx, x: d2.x, y: d2.y
                                });
                            }

                            //paint your / clan mates claim
                            if (claimStatusIcon) {
                                lc.preload(claimStatusIcon, function (img, d3) {
                                    d3.ctx.drawImage(img, d3.x + 4, d3.y + 9, 9, 9);
                                }, {
                                    ctx: d2.ctx, x: d2.x, y: d2.y
                                });
                            }

                        }, {
                            ctx: d.ctx, x: d.x, y: d.y
                        });

                    } else {

                        //paint other ally claim
                        if (d.v.claimedStatus_otherClan == 1) {
                            lc.preload('https://static.realmofempires.com/images/icons/claimeD_otherAllied.png', function (img, d2) {
                                d2.ctx.drawImage(img, d2.x + 11, d2.y + 4, 9, 9);
                            }, {
                                ctx: d.ctx, x: d.x, y: d.y
                            });
                        }

                        //paint your / clan mates claim
                        if (claimStatusIcon) {
                            lc.preload(claimStatusIcon, function (img, d2) {
                                d2.ctx.drawImage(img, d2.x + 4, d2.y + 4, 9, 9);
                            }, {
                                ctx: d.ctx, x: d.x, y: d.y
                            });
                        }


                    }

                    var now = new Date().getTime();

                    try {
                        var troopsSup = $.grep(troopsmove.commands, function (c) {
                            return c.dvid == d.v.id && c.type == 0 && c.etime > now;
                        });
                        var troopsAtt = $.grep(troopsmove.commands, function (c) {
                            return c.dvid == d.v.id && c.type == 1 && c.etime > now;
                        });
                    } catch (exception) {
                        var roeex = new BDA.Exception('error in lc.preload', exception);
                        roeex.data.add('troopsmove.commands', troopsmove.commands);
                        BDA.latestException = roeex;
                        throw roeex;
                    }


                    if (troopsAtt.length > 0) {
                        lc.preload('https://static.realmofempires.com/images/attack.png',
                            function (img, d2) {
                                d2.ctx.drawImage(img, d2.x, d2.y, 12, 12)
                            }, { ctx: d.ctx, x: d.x + 20, y: d.y + 5 }
                        );
                    }

                    if (troopsSup.length > 0) {
                        lc.preload('https://static.realmofempires.com/images/support.png',
                            function (img, d2) {
                                d2.ctx.drawImage(img, d2.x, d2.y, 12, 12)
                            }, {
                                ctx: d.ctx, x: d.x + 15,
                                y: d.y
                            }
                        );
                    }


                    _checkPaintEvent(ctx, d.v.x, d.v.y);

                }, {
                    ctx: context, x: xx, y: yy, v: v
                });

            }

            function _checkPaintEvent(context, mapX, mapY) {

                var mapEvent;
                var creditsTotal = 0;
                var mapEvenetsHere = ROE.Player.MapEventsGrouped[mapX + "," + mapY];
                if (mapEvenetsHere) {

                    ////i think this is a translation of world X,Y cords, to individual canvas draw x,y
                    var tdc = 6; //tile cord db size? sure
                    var xx = 0;
                    var x1 = mapX % tdc;
                    if (mapX < 0) {
                        xx = (5 * lp) + ((x1 == 0 ? -tdc : x1) * lp + lp);
                    } else {
                        xx = x1 * lp;
                    }

                    var yy = 0;
                    var y1 = (mapY - 1) % tdc;
                    if (mapY < 0) {
                        yy = -((y1 == 0 ? -tdc : y1) * lp + lp);
                    } else if (mapY == 0) {
                        yy = 0;
                    } else {
                        yy = (5 * lp) - (y1 * lp);
                    }
                    ////


                    for (var i = 0; i < mapEvenetsHere.length; i++) {
                        mapEvent = mapEvenetsHere[i];
                        if (mapEvent.typeID == 1) { //credit farm event
                            creditsTotal += parseInt(mapEvent.data);
                        } else if (mapEvent.typeID == 2) { //loot camp event
                            lc.preload("https://static.realmofempires.com/images/map/Caravan01.png",
                                function paintEventLootCamp(img, cbData) {
                                    var size = Math.max(cbData.size * .6, 20);
                                    var iconX = cbData.x + (cbData.size / 2) - (size / 2);
                                    var iconY = cbData.y + (cbData.size / 2) - (size / 2);
                                    cbData.ctx.drawImage(img, iconX, iconY, size, size);
                                },
                                {
                                    ctx: context,
                                    x: xx,
                                    y: yy,
                                    size: l.landpx,
                                    scale: l.scale
                                }
                            );
                        } else if (mapEvent.typeID == 3) { //loot camp event
                            lc.preload("https://static.realmofempires.com/images/icons/M_Target.png",
                            function paintEventTarget(img, cbData) {
                                var size = Math.max(cbData.size * .6, 20);
                                var iconX = cbData.x + (cbData.size / 2) - (size / 2);
                                var iconY = cbData.y + (cbData.size / 2) - (size / 2);
                                cbData.ctx.drawImage(img, iconX, iconY, size, size);
                            },
                            {
                                ctx: context,
                                x: xx,
                                y: yy,
                                size: l.landpx,
                                scale: l.scale
                            }
                        );
                        }

                    }

                    //paints a credit-farmevent only if credits exist
                    if (creditsTotal > 0) {

                        var icon = 'https://static.realmofempires.com/images/icons/servantCarry_m.png';
                        if (ROE.Realm.creditFarmBonusDateEnds > (new Date()) && ROE.Realm.creditFarmBonusIcon) {
                            icon = ROE.Realm.creditFarmBonusIcon;
                        }

                        lc.preload(icon,
                        function paintEventCreditFarm(img, cbData) {
                            var farmFlagSize = Math.round(Math.max(cbData.size * .6, 20));
                            var farmIconX = cbData.x + (cbData.size / 2) - (farmFlagSize / 2);
                            var farmIconY = cbData.y + (cbData.size / 2) - (farmFlagSize / 2);
                            cbData.ctx.drawImage(img, farmIconX, farmIconY, farmFlagSize, farmFlagSize);
                            var textSize = 25 * cbData.scale;
                            cbData.ctx.fillStyle = "white";
                            cbData.ctx.font = textSize + "px georgia";
                            cbData.ctx.lineWidth = 3;
                            var dataTextX = farmIconX + farmFlagSize - textSize / 2;
                            var dataTextY = farmIconY + farmFlagSize - textSize / 4;
                            cbData.ctx.strokeText(cbData.creditsTotal, dataTextX, dataTextY);
                            cbData.ctx.fillText(cbData.creditsTotal, dataTextX, dataTextY);
                        },
                        {
                            ctx: context,
                            x: xx,
                            y: yy,
                            size: l.landpx,
                            scale: l.scale,
                            creditsTotal: creditsTotal
                        });
                    }



                }
            }

        }
    };

    //end of l.canvas



    //   refill(true) – repaints all canvases
    //   refill() – reload all canvases, which has villages that changed in some way, icon or flag
    //              Used by MapHighlights for example.
    var _refill = function (all) {
        var reloadCanvases = {
        };
        var scl = l.canvas;
        if (!all) {
            $.each(l.villages, function (i, n) {
                if (n.painted == false) {
                    var xy = n.areacoord.split('_');
                    reloadCanvases[n.areacoord] = {
                        x: xy[0], y: xy[1], xy: n.areacoord
                    };
                }
            });

            $.each(reloadCanvases, function (i, n) {
                if (scl.list[n.xy]) {
                    scl.paint(scl.list[n.xy].can[0], n.x, n.y);
                }
            });
        } else {
            $.each(scl.list, function (i, n) {
                var xy = i.split('_');
                scl.paint(n.can[0], xy[0], xy[1]);
            });
        }
    };

    // History Comment
    //// idea of this render is that we use bunch of canvases as squares 504 px x 504 px
    //// bug: unfortunatelly android and ios browsers crashing on big canvases and canvas collectivelly px2 around 2000px x 3000px
    //// to load map on canvas, in compare with imgs loading, we need to have all rendering pictures preloaded
    //// comparing with idea of canvas.toUrlData, here we dont care, about time when all of images loaded
    //// in our case, they painted on canvas after loading, 
    //// the only exceptions is first image of grass, cause we cant support z-index, and we need to paint in two steps
    //// for that I use callback in preload
    //// also one note for canvas.toUrlData - they support only own domain pictures (security reasons), need to be changed



    // Method creates canvases. Input parameter is a viewport zone left upper and right down. 
    // This coordinates are moduled at earthpx, so canvases are always in the same place in relation to div coordinates.
    // While creating canvases it also fill “l.earth” object. It consist of 2 indexes:
    // - by string “left_top” of canvas
    // - by left and inside of it object of all top
    // 
    // For sample on top: 
    // l.earth =
    // {
    //     c: { “0_0”: true, “504_0”: true, “1008_0”: true, “0_504”: true, “504_504”: true, “1008_504”: true },
    //     x: { “0”: { “0”: true, “504”: true }, “504”: { “0”: true, “504”: true }, “1008”: { “0”: true, “504”: true } }
    // }
    // While fill() executes it checks if earth already exist in an object and if not creates it.

    var _fill = function (lx, ly, rx, ry) {
        var s = $('#surface');
        var lp = l.landpx;
        var ep = l.earthpx;
        var tdc = 6; // tile db coords size
        var new_earth = [];
        var new_earth2 = {
        };

        // lx, ly, rx, ry - gives us 2 diagonal points from screen, in relation to 504 tile network, 
        // its including whole screen viewport always
        // so we go by each tile that could be there, if its not exists there, we put new, 
        // saving its x,y of bottom tile to newearth

        //so this removes garbage Canvases, just for D2 because in M itll break things maybe
        if (ROE.isD2) {
            s.find('canvas[width="1px"]').remove();
        }


        for (var ax = lx; ax <= rx; ax += ep) {
            for (var ay = ly; ay <= ry; ay += ep) {
                if (!l.earth.c[ax + '_' + ay]) {
                    l.earth.c[ax + '_' + ay] = true;
                    if (!l.earth.x[ax]) {
                        l.earth.x[ax] = {
                        };
                    }
                    l.earth.x[ax][ay] = true;

                    // index of canvas in db coords of bottom left tile
                    var ind = ((ax / ep) * tdc) + '_' + -((((ay + ep) / ep) * tdc) - 1);

                    // width and height attributes, cannot be put in css, not working
                    var can = $('<canvas class="earth" alt="landscape' + ax + '_' + ay + '" ind="' + ind + '" width="' + ep + 'px" height="' + ep + 'px" />').css({
                        left: ax, top: ay
                    });

                    new_earth.push(ind);
                    new_earth2[ind] = {
                        ax: ax, ay: ay, can: can
                    };

                    s.append(can);
                }
            }
        }

        $.extend(l.canvas.list, new_earth2);

        _moredata({
            coords: new_earth
        }, function () {
            for (var e in new_earth) {
                var nee = new_earth[e];
                if (typeof (nee) == 'string') {
                    var ne2 = new_earth2[nee];
                    var xy = nee.split('_');
                    l.canvas.paint(ne2.can[0], xy[0], xy[1], function () { }, new_earth, new_earth2);
                }
            }
        });
    }






    l.start = _start;
    l.gotopx = _gotopx;
    l.gotodb = _gotodb;
    l.close = _close;
    l.select = _select;
    l.update = _update;
    l.pointdb = _pointdb;
    l.paint = _paint;
    l.zoom = _zoom;
    l.refresh = _refresh;
    l.refill = _refill;
    l.savevill = _savevill;

    //l.handleSelectedVillageChanaged = _handleSelectedVillageChanaged;
    l.updateVillageName = _updateVillageName;
    l.hoverOverVillage = _hoverOverVillage;

    l.checkvills = _checkvills;

    l.mapGuiTargetSetTimeout = _mapGuiTargetSetTimeout;

}(window.ROE.Landmark = window.ROE.Landmark || {}));