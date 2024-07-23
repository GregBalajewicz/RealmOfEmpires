favorite = {

    flags: {
        path: 'https://static.realmofempires.com/images/map/',

        list: {
            
            "x1": 'Shield2_Black.png',
            "x2": 'Shield2_Blue.png',
            "x3": 'Shield2_Green.png',
            "x4": 'Shield2_Orange.png',
            "x5": 'Shield2_Purple.png',
            "x6": 'Shield2_Red.png',
            "x7": 'Shield2_White.png',
            "x8": 'Shield2_Yellow.png',

            "xb1": 'Shield2b_Black.png',
            "xb2": 'Shield2b_Blue.png',
            "xb3": 'Shield2b_Green.png',
            "xb4": 'Shield2b_Orange.png',
            "xb5": 'Shield2b_Purple.png',
            "xb6": 'Shield2b_Red.png',
            "xb7": 'Shield2b_Teal.png',
            "xb8": 'Shield2b_Yellow.png',

            "xc1": 'Shield2c_Black.png',
            "xc2": 'Shield2c_Blue.png',
            "xc3": 'Shield2c_Green.png',
            "xc4": 'Shield2c_Orange.png',
            "xc5": 'Shield2c_Purple.png',
            "xc6": 'Shield2c_Red.png',
            "xc7": 'Shield2c_Teal.png',
            "xc8": 'Shield2c_Yellow.png',

            "z1": 'Shield3_Black.png',
            "z2": 'Shield3_Blue.png',
            "z3": 'Shield3_Green.png',
            "z4": 'Shield3_Orange.png',
            "z5": 'Shield3_Purple.png',
            "z6": 'Shield3_Red.png',
            "z7": 'Shield3_White.png',
            "z8": 'Shield3_Yellow.png',

            "zb1": 'Shield3b_Black.png',
            "zb2": 'Shield3b_Blue.png',
            "zb3": 'Shield3b_Green.png',
            "zb4": 'Shield3b_Orange.png',
            "zb5": 'Shield3b_Purple.png',
            "zb6": 'Shield3b_Red.png',
            "zb7": 'Shield3b_Teal.png',
            "zb8": 'Shield3b_Yellow.png',

            //"zc1": 'Shield3c_Black.png',
            "zc2": 'Shield3c_Blue.png',
            "zc3": 'Shield3c_Green.png',
            "zc4": 'Shield3c_Orange.png',
            "zc5": 'Shield3c_Purple.png',
            "zc6": 'Shield3c_Red.png',
            "zc7": 'Shield3c_Teal.png',
            "zc8": 'Shield3c_Yellow.png',
            
            "y1": 'Shield4_Black.png',
            "y2": 'Shield4_Blue.png',
            "y3": 'Shield4_Green.png',
            "y4": 'Shield4_Orange.png',
            "y5": 'Shield4_Purple.png',
            "y6": 'Shield4_Red.png',
            "y7": 'Shield4_White.png',
            "y8": 'Shield4_Yellow.png',

            "yb1": 'Shield4b_Black.png',
            "yb2": 'Shield4b_Blue.png',
            "yb3": 'Shield4b_Green.png',
            "yb4": 'Shield4b_Orange.png',
            "yb5": 'Shield4b_Purple.png',
            "yb6": 'Shield4b_Red.png',
            "yb7": 'Shield4b_Teal.png',
            "yb8": 'Shield4b_Yellow.png',

            //"yc1": 'Shield4c_Black.png',
            "yc2": 'Shield4c_Blue.png',
            "yc3": 'Shield4c_Green.png',
            "yc4": 'Shield4c_Orange.png',
            "yc5": 'Shield4c_Purple.png',
            "yc6": 'Shield4c_Red.png',
            "yc7": 'Shield4c_Teal.png',
            "yc8": 'Shield4c_Yellow.png',

            "teal2": 'flag_Teal2.png',
            "teal1": 'flag_Teal1.png',
            "yellow2": 'flag_Yellow2.png',
            "red2": 'flag_Red2.png',
            "purple2": 'flag_Purple2.png',
            "purple1": 'flag_Purple1.png',
            "pink1": 'flag_Pink1.png',
            "pink2": 'flag_Pink2.png',
            "orange1": 'flag_Orange1.png',
            "orange2": 'flag_Orange2.png',
            "green2": 'flag_Green2.png'
        },


        def: ["orange1"]
    },


    //
    // handle the highlight type selection change to init the text box properly
    //
    typeChange: function (selectEl) {
        var type = $(selectEl).val();
        var parentTR = $(selectEl).first().parents("tr");
        var textBox = $('input.keyword', parentTR).first();

        switch (type) {
            case "players": favorite.autocomplete_player(textBox); break;
            case "clans": favorite.autocomplete_clan(textBox); break;
            case "village-tag": favorite.autocomplete_tags(textBox); break;
            case "village-note":
            case "player-note": textBox.autocomplete({ disabled: true }); break;
        }

    },

    // helper for clan autocomplete box
    autocomplete_clan: function (textBox) {
        textBox.autocomplete({
            minLength: 1,
            source: "NamesAjax.aspx?what=clans"
        })
    },

    // helper for player autocomplete box
    autocomplete_player: function (textBox) {
        textBox.autocomplete({
            minLength: 3,
            source: "NamesAjax.aspx?what=players",
            disabled: false
        })
    },

    // helpers for tags autocomplete
    tagsArrayForAutoComplete: [],
    autocomplete_tags: function (textBox) {
        if (favorite.tagsArrayForAutoComplete.length < 1) {
            for (tag in tags) {
                favorite.tagsArrayForAutoComplete.push(tags[tag].n);
            }
        }

        textBox.autocomplete({
            minLength: 1,
            source: favorite.tagsArrayForAutoComplete
        })
    },

    // hanlde the onclick of the delete icon to delete a row. 
    deleteRow: function (imgEl) {
        // get containing row
        var parentTR = $(imgEl).first().parent().parent();

        // blank out the text box
        $('input.keyword', parentTR).val("");

        if ($('#favorite tbody tr').length > 1) {
            // if more than one row, delete the clicked on row
            parentTR.fadeOut('slow', function () { $(this).remove(); });
        }

        $('#favorite tfoot .highlight').click();
    },

    item: '<tr class="item"> \
              <td><img class="cancel" onclick="favorite.deleteRow(this);" src="https://static.realmofempires.com/images/cancel.png"/><select class="type" onchange="favorite.typeChange(this);"> \
                     <option value="clans">Clan</option> \
                     <option value="players">Player</option> \
                     <option value="player-note">Player Note</option> \
                     <option value="village-note">Village Note</option> \
                     <option value="village-tag">Village Tag</option> \
                   </select></td> \
              <td><input class="keyword" /></td> \
              <td><img class="flag-icon" /></td> \
          </tr>',

    init: function () {
        $('.village').each(function (i, n) {
            $(n).append($('<img class="flag favor" style="display: none" />').attr('id', 'vil_flag_' + $(n).attr('rel')));
        });

        favorite.form();
    },


    form: function () {
        var groups = [8, 8, 8,8, 8, 7,8, 8, 7,11];
        var counter = 0;
        var inGroup = 0;

        for (var fl in this.flags.list) {
        //var fl;
        //for (var i = this.flags.length-1; i <= 0; i--) {
            
            if (counter >= groups[inGroup]) {
                inGroup++;
                counter = 0;
                $('#favorite .popup').append($('<BR>'));
            }

            $('#favorite .popup').append(
                $('<img />').attr('src', favorite.flags.path + this.flags.list[fl])
                            .attr('who', fl)
                            .click(function () {
                                var caller = $('#favorite .popup').attr('caller');
                                var flag = $(this).attr('who');

                                $('#favorite .flag-icon:eq(' + caller + ')')
                                    .attr('flag', flag)
                                    .attr('src', favorite.flags.path + favorite.flags.list[flag]);

                                $('#favorite .popup').hide();
                                $('#favorite tfoot .highlight').click(); //trigger re-higlight
                            })
            );

            counter++;
        }

        // testing data - DELETE IT
        //        $('tbody tr input:eq(0)', tbl).val('Bengis');
        //        $('tbody tr input:eq(1)', tbl).val('RFI Roll For Initiative');
        //        $('tbody tr select:eq(0)', tbl).val('players');
        // testing data - DELETE IT

        favorite.load();
        favorite.fill();

        $('#favorite tfoot .highlight').click(function () {
            favorite.clear(); favorite.fill(); favorite.save();
        });

        $('#favorite tfoot .addrow').click(function () {
            favorite.addrow();
        });

        // trigger the change event on selects so that we get proper autocomplete on the text boxes
        $('#favorite tbody select.type').change();
    },
    addrow: function (fl) {
        if (!fl) {
            var selected = {};

            $('#favorite tbody tr').each(function (i, n) {
                selected[$('.flag-icon', n).attr('flag')] = 1;
            });

            for (var f in this.flags.list) {
                if (!selected[f]) { fl = f; }
            }
            if (!fl) { fl = this.flags.def[0]; }
        }

        var item = $(favorite.item);
        item.find('.flag-icon')
                .attr('flag', fl)
                .attr('src', this.flags.path + this.flags.list[fl])
                .click(function () {
                    var caller = $('#favorite .item .flag-icon').index(this);
                    $('#favorite .popup').show().attr('caller', caller);
                });

        $('#favorite tbody').append(item);
        item.fadeIn();

        $('#favorite tbody select.type').change();

        return item;
    },
    fill: function () {
        $('#favorite tbody tr').each(function (i, n) {

            var type = $('.type', n).val();
            var keyword = $('.keyword', n).val();
            var flag = $('.flag-icon', n).attr('flag');

            if (keyword == "") return;
            // find clans and persons in array
            var vils = favorite.search(type, keyword);

            for (var vi = 0; vi < vils.length; vi++) {
                favorite.apply(vils[vi].id, favorite.flags.path + favorite.flags.list[flag]);
            }
        });
    },
    save: function () {
        var res = [];

        $('#favorite tbody tr').each(function (i, n) {
            if ($('input', n).val() != '') {
                res.push({ type: $('select', n).val(), keyword: $('input', n).val(), flag: $('.flag-icon', n).attr('flag') });
            }
        });

        var json = $.toJSON(res);

        for (var i = 0; i < Math.floor(json.length / 4000) + 1; i++) {
            $.cookie('favorite.' + ROE.realmID + '.' + ROE.playerID + '.' + i, json.substr(i, 4000), { expires: 300 });
        }

        for (var i = 0; i < 10; i++) {
            $.cookie('favorite.' + ROE.realmID + '.' + i, null);
        }

        //$.cookie('favorite.' + this.realm(), json);
    },
    load: function () {
        var cook = '', cook_str = '', i = 0;

        while (cook != null) {
            cook = $.cookie('favorite.' + ROE.realmID + '.' + ROE.playerID + '.' + i);
            if (cook == null) cook = $.cookie('favorite.' + ROE.realmID + '.' + i);

            i++;
            if (cook != null) cook_str += cook;
        }

        if (cook_str == '') { def(); return; }

        var list = $.evalJSON(cook_str);

        if (list == null || list.length == 0) { def(); return; }

        function def() {
            for (var fl = 0; fl < favorite.flags.def.length; fl++) {
                favorite.addrow(favorite.flags.def[fl]);
            }
        }

        for (var el = 0; el < list.length; el++) {
            var item = this.addrow(list[el].flag);

            $('.type', item).val(list[el].type);
            $('.keyword', item).val(list[el].keyword);
        }
    },
    clear: function () {
        // clear my flags
        $('#mapActual .flag.favor').hide();
        // make visible old flags
        // cant use show - as than it is using display:inline, 
        // that causes to show elements with fbfFlags
        $('#mapActual .flag:not(.favor) .flag:not(.sleep)').attr('style', '');
    },

    apply: function (id, url) {

        var flag = document.getElementById('vil_flag_' + id);
        flag.src = url;
        flag.style.display = 'block';

        var allimgs = flag.parentNode.childNodes;

        for (var i = 0; i < allimgs.length; i++) {
            var img = allimgs[i]; var c = img.className;
            if (c != "" && c != "flag favor" && c != "flag att" && c != "flag sup" && c !="flag sleep")
                img.style.display = 'none';
        }
    },

    search: function (type, name) {

        var vils = []; var obj = [];

        var name = $.trim(name);

        switch (type) {
            case "players": obj_search(type, "PN", comp_nocase, name); break;
            case "clans": obj_search(type, "CN", comp_nocase, name); break;
            case "player-note": obj_search("players", "Pe", comp_contains, name); break;
            case "village-tag": tag_search(name); return obj;
            case "village-note": obj_search("villages", "Ve", comp_contains, name); return obj;
        }

        function comp_nocase(f, s) { return f.toLowerCase() == s.toLowerCase(); }
        function comp_contains(f, s) { return f.toLowerCase().indexOf(s.toLowerCase()) != -1; }

        function obj_search(type, field, compfunc, name) {
            for (var it in window[type]) {
                if (compfunc(window[type][it][field], name)) {
                    obj.push(window[type][it]);
                }
            }
        }

        function tag_search(name) {
            //
            // find tag ID from tag name
            // 
            var tagID = -1;
            for (var tagDef in tags) {
                if (comp_nocase(tags[tagDef].n, name)) {
                    tagID = tags[tagDef].id;
                    break;
                }
            }
            if (tagID !== -1) {
                for (var vil in villages) {
                    for (var tag in villages[vil].tags) {
                        if (villages[vil].tags[tag] === tagID) {
                            obj.push(villages[vil]);
                        }
                    }
                }
            }
        }

        for (var i = 0; i < obj.length; i++) {
            if (type == "clans") {
                for (var it in window.players) {
                    if (window.players[it].CID == obj[i].id) {
                        bypid(window.players[it].id);
                    }
                }
            }
            if (type == "players" || type == "player-note") {
                bypid(obj[i].id);
            }
        }

        function bypid(id) {
            for (var vi in window.villages) {
                if (window.villages[vi].PID == id)
                    vils.push(window.villages[vi]);
            }
        }

        return vils;
    }
}

$(function() {
    favorite.init();

    $('.jsToggleObjectsTree').click(function(e) {

        var expcol = {
            expand: 'https://static.realmofempires.com/images/expand_button.gif',
            collapse: 'https://static.realmofempires.com/images/collapse_button.gif'
        };

        e.preventDefault();

        var ot = $('<ul></ul>');

        for (var cli in window.clans) {
            var clan = window.clans[cli];


            var cname = '<a href="ClanPublicProfile.aspx?clanid=' + clan.id + '">' + clan.CN + '</a>';
            ot.append($('<li></li>').html(cname + ' <span style="color:#D0D5D9;">[' + clan.CP + ']</span>').append(players(clan.id)));
        }

        ot.append($('<li>Not in clan</li>').append(players('')));

        var rab = $('<li>Rebels and Abandoned<ul><li class="rebels"><img class="collapse" src="' + expcol.expand + '" />Rebels</li><li class="abandoned"><img class="collapse" src="' + expcol.expand + '" />Abandoned</li></ul></li>');
        rab.find('.rebels').append(villages(ROE.CONST.specialPlayer_Rebel)).find('ul').css('display', 'none');
        rab.find('.abandoned').append(villages(ROE.CONST.specialPlayer_Abandoned)).find('ul').css('display', 'none');

        ot.append(rab);

        // clan num or 0
        function players(clan) {
            var li = $('<ul></ul>');
            for (var pli in window.players) {
                var player = window.players[pli];

                if (player.CID != clan || ROE.isSpecialPlayer(player.id)) continue;

                var pname = '<a onclick="return popupOtherVoV(this, \'' + player.id + '\', \'' + player.PN + '\');" href="PlayerPopup.aspx?pid=' + player.id + '">' + player.PN + '</a>';
                //todo: move to css
                var ppoint = '<span style="color:#D0D5D9;">[' + player.PP + ']</span>';
                //todo: move to css
                var padd = '<a pn="' + player.PN + '" class="torecip" style="padding: 2px 5px 2px 27px; background: url(https://static.realmofempires.com/images/OldMail.PNG) no-repeat scroll 0 center transparent" type="add" href="#">add to message recipients</a>';
                var pcol = '<img class="collapse" src="' + expcol.expand + '" />';

                var player_dom_data = [pcol, pname, ppoint, padd];
                if (player.Pe != 0) player_dom_data.push('<i>' + player.Pe + '</i>');

                var player_dom = $('<span>' + player_dom_data.join(' ') + '</span>');

                $('.torecip', player_dom).click(function(e) {

                    var s = $(this);
                    var ot = s.attr('type');
                    var nt = ot == 'remove' ? 'add' : 'remove';
                    s.attr('type', nt);
                    s.html(nt == 'remove' ? 'remove from message recipients' : 'add to message recipients');

                    var m = $('.objects-tree .message');
                    var pl = m.data('players');
                    if (ot == 'add') {
                        pl.push(s.attr('pn'));
                    } else {
                        pl.splice($.inArray(s.attr('pn'), pl), 1);
                    }

                    var pl_names = m.data('players');

                    m.html(pl_names == '' ? '' : m.attr('template').replace('{0}', pl_names.join(', ')));
                    m.attr('href', 'messages_create.aspx?To=' + pl_names.join(','));
                    //todo: move to css
                    m.css('font-size', '130%');

                    e.preventDefault();
                });

                // used .css('display', 'none') cause in jquery 1.2.3 we have a chrome not working with hide while adding elements
                li.append($('<li></li>').append(player_dom).append(villages(player.id))).find('ul').css('display', 'none');
            }
            return li;
        }

        // player
        function villages(player) {
            var li = $('<ul></ul>');
            for (var vli in window.villages) {
                var village = window.villages[vli];

                if (village.PID != player) continue;

                var vtitle = village.n + ' (' + village.X + ',' + village.Y + ')';
                var vname = '<a onclick="return popupOtherVoV(this,\'' + village.id + '\',\'' + vtitle + '\' );" href="VillageOverviewOtherPopup.aspx?ovid=' + village.id + '">' + vtitle + '</a>';

                li.append($('<li></li>').html(vname + (village.Ve == 0 ? '' : '<i>' + village.Ve + '</i>')));
            }
            return li;
        }

        $('.collapse', ot).click(function(e) {
            var ul = $(this).parents('li:first').find('ul');

            if ($(this).attr('src') == expcol.expand) {
                $(this).attr('src', expcol.collapse); ul.show();
            } else {
                $(this).attr('src', expcol.expand); ul.hide();
            }
        });

        var message_link = '<a class="message" href="#" template="Click here to message {0}"></a>';

        $('.objects-tree').empty()
            .append($(message_link).data('players', []))
            .append(ot)
            .append($(message_link).data('players', []));
    });


    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function() { window.isPostBack = true; });
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(
        function() {
            if (window.isPostBack)
                favorite.init();
        }
    );
});