


$(function () {
    MassFunct.init();
    saveDialog.init($('#save'), 'massUp', MassFunct.save, MassFunct.load);
});
MassFunct = {
    flags: {
        path: 'https://www.realmofempires.com/images/map/',
        list: {
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
        var buildingType = ROE.Realm.BuildingTypeByID(type);

        var parentTR = $(selectEl).parentsUntil("tr").parent();
        MassFunct.populateLevelsDD(buildingType, buildingType.MaxLevel, parentTR);
        //        var levelSelect = $('select.BuildingLevel', parentTR).first();

        //        levelSelect.empty();
        //        for (var l = 1; l < buildingType.MaxLevel; l++) {
        //            levelSelect.append($('<option value="' + l + '">Level ' + l + '</option> '));
        //        }
        //        levelSelect.append($('<option selected="true" value="' + buildingType.MaxLevel + '">Level ' + buildingType.MaxLevel + '</option> '));
    },

    populateLevelsDD: function (buildingType, levelToSelect, parentTR) {
        var levelSelect = parentTR.find('select.BuildingLevel').empty();
        for (var l = 1; l <= buildingType.MaxLevel; l++) {
            levelSelect.append($('<option ' + (levelToSelect === l ? 'Selected="True"' : '') + ' value="' + l + '">' + l + '</option> '));
        }

    },


    // hanlde the onclick of the delete icon to delete a row. 
    deleteRow: function (imgEl) {
        // get containing row
        var parentTR = $(imgEl).first().parent().parent();

        // blank out the text box
        $('input.keyword', parentTR).val("");

        if ($('#MassFunct tbody tr').length > 1) {
            // if more than one row, delete the clicked on row
            parentTR.fadeOut('slow', function () { $(this).remove(); });
        }
    },
    //Erase: an "indicator" of the process and  icons of "delete"
    deleteUpdate: function (imgEl) {
        // get containing  td
        var parentTD = $(imgEl).first().parent();
        var parentTDprev = $(imgEl).first().parent().prev();
        if (parentTD.length && parentTDprev.length) 
        {
            $(parentTD).children().remove();
            $(parentTDprev).children().remove();
        }
    },

    item: '<tr class="item"> \
              <td><img class="cancel" onclick="MassFunct.deleteRow(this);" src="https://static.realmofempires.com/images/cancel.png"/><TD><TD> Upgrade Building  <select class="BuildingTypeID" onchange="MassFunct.typeChange(this);"> \
                   </select></td> \
              <td>To level <select class="BuildingLevel"></select></td> \
              <td><input type="checkbox">Upgrade req?</input></td> \
          </tr>',

    init: function () {
        $('.village').each(function (i, n) {
            $(n).append($('<img class="flag favor" style="display: none" />').attr('id', 'vil_flag_' + $(n).attr('rel')));
        });

        MassFunct.form();
    },


    form: function () {

        //MassFunct.load();
        MassFunct.fill();

        $('#MassFunct tfoot .highlight').click(function () {
            MassFunct.clear(); MassFunct.fill(); MassFunct.save();
        });

        $('#MassFunct tfoot .addrow').click(function () {
            MassFunct.addrow();
        });

        // trigger the change event on selects so that we get proper autocomplete on the text boxes
        $('#MassFunct tbody select.BuildingTypeID').change();
    },

    addrow: function (obj) {
        ///<summary> obj is optional and will be passed in when loading from a save. if normally adding a new row, obj will be undefined</summary>
        //
        var startBTID = obj ? parseInt(obj.btID, 10) : ROE.Realm.Buildings[0].ID;
        var startLevel; //= obj ? parseInt(obj.level, 10) : -1;
        var startUpRequirements = obj ? obj.upReq : true;

        var item = $(MassFunct.item);
        var startBuildingType = undefined;
        for (var bt = 0; bt < ROE.Realm.Buildings.length; bt++) {
            if (startBTID === ROE.Realm.Buildings[bt].ID) { startBuildingType = ROE.Realm.Buildings[bt]; }

            item.find('select.BuildingTypeID').append($('<option ' + (startBTID === ROE.Realm.Buildings[bt].ID ? 'Selected="True"' : '')
                + ' value="' + ROE.Realm.Buildings[bt].ID + '">' + ROE.Realm.Buildings[bt].Name + '</option> '))
        }
        startLevel = obj ? parseInt(obj.level, 10) : startBuildingType.MaxLevel;

        item.find('[type=checkbox]').prop('checked', startUpRequirements);

        $('#MassFunct tbody').append(item);
        item.fadeIn();

        // to select the level, we save the level #, then trigger on change on the building type
        MassFunct.populateLevelsDD(startBuildingType, startLevel, item);

        return item;
    },
    fill: function () {
        $('#MassFunct tbody tr').each(function (i, n) {

            var type = $('.type', n).val();
            var keyword = $('.keyword', n).val();
            var flag = $('.flag-icon', n).attr('flag');

            if (keyword == "") return;
            // find clans and persons in array

        });
    },
    save: function () {
        ///<summary> this returns an array representing the rules specified. Can be used to save rule, or to pass to ajax to execute them</summary>
        var res = [];

        $('#MassFunct tbody tr').each(function (i, n) {
            if ($('input', n).val() != '') {
                res.push({ btID: $('select.BuildingTypeID', n).val(), level: $('select.BuildingLevel', n).val(), upReq: $('[type=checkbox]', n).prop('checked') });
            }
        });

        return res;
    },
    realm: function () {
        return $('#ctl00_lblCurRealm').text();
    },
    player: function () {
        return $('#ctl00_lblPID').text();
    },
    load: function (obj) {
        if (!obj) return;

        var list = obj;




        if (list == null || list.length == 0) { def(); return; }

        function def() {
            for (var fl = 0; fl < MassFunct.flags.def.length; fl++) {
                MassFunct.addrow(MassFunct.flags.def[fl]);
            }
        }



        $('#MassFunct tbody').empty();
        for (var el = 0; el < list.length; el++) {
            var item = MassFunct.addrow(list[el]);

        }
    },
    clear: function () {
        // clear my flags
        $('#mapActual .flag.favor').hide();
        // make visible old flags
        // cant use show - as than it is using display:inline, 
        // that causes to show elements with fbfFlags
        $('#mapActual .flag:not(.favor)').attr('style', '');
    },

    apply: function (id) {




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