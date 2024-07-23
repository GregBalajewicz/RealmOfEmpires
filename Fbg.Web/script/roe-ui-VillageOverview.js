(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {
    var _container;
    var _v;
    var _vid;
    
    var Frame = window.ROE.Frame;

    var CACHE = {};
    CACHE.villagePopupTemplate=null;
    CACHE.notePopupTemplate = null;
    CACHE.noteText = "";
    
    obj.init = function (container, vid) {
        
        if (!CACHE.villagePopupTemplate) {
            Frame.busy(undefined, undefined, container);
            var temp = BDA.Templates.getRawJQObj("VillagePopup", ROE.realmID);
           
            Frame.free(container);
            CACHE.villagePopupTemplate = temp;
            CACHE.notePopupTemplate = $('.template.notePopup', CACHE.villagePopupTemplate).clone().removeClass('template');
        }
        _container = container.empty().append(CACHE.villagePopupTemplate.clone());
        
        var l = ROE.Landmark;
        var v = l.villages_byid ? l.villages_byid[vid] : null;
        _v = v;
        _vid = vid;
        
        ROE.Api.call('othervillageinfo', { vid: vid }, function (ob) {
            _populate(ob);
        });

        Frame.busy(undefined, undefined, _container);

       
    }


    function _targetUpdate(response) {

        var TargetPlayer = response.OwnerName;
        var TargetPlayerId = response.OwnerPlayerID;
        var VillageName = response.VillageName;
        var coordX = response.coords.X;
        var coordY = response.coords.Y;
        var TargetVClanID = response.clanID;
        var VillagePoint = response.Points; 
        var clanName = response.clanName;
        var imageUrl = BDA.Utils.GetMapVillageIconUrl(VillagePoint, response.ID);


        $("#village_popup .villageimage IMG").attr("src", imageUrl);
        $("#village_popup .villagename SPAN:first-child").html(VillageName);
        $("#village_popup .villagename SPAN.coord").html("(" + coordX + "," + coordY + ")");
        $("#village_popup .villpoint SPAN").html(VillagePoint);
        $("#village_popup .villowner DIV").html(TargetPlayer);

        //if (TargetPlayerId != ROE.CONST.specialPlayer_Rebel) {
        if(!ROE.isSpecialPlayer(TargetPlayerId)) {
            $("#village_popup .villowner DIV").click(function () { ROE.Frame.popupPlayerProfile(TargetPlayer); });
        }
        else { $("#village_popup .villowner DIV").removeClass("ButtonTouch sfx2"); }
        
        if (TargetVClanID != 0) {

            $("#village_popup .villclan").css("display", "block");
            $("#village_popup .villclan DIV").html(clanName);
            $("#village_popup .villclan DIV").click(function () { ROE.Frame.popupClan(TargetVClanID); });

        }
        
        ROE.Frame.free(_container);
    }

   
    function _populate(v) {

        $('.attack', _container).click(function () {
            if (ROE.playersNumVillages > 1) {                
                ROE.Frame.popupQuickCommand(1, v.id, v.x, v.y);
            } else {
                ROE.Frame.popupAttacks(v.id, v.x, v.y);
            }
        });

        if (!ROE.isSpecialPlayer_Rebel(v.pid)) {
            $('.support', _container).click(function () {
                if (ROE.playersNumVillages > 1) {
                    ROE.Frame.popupQuickCommand(0, v.id, v.x, v.y);
                } else {
                    ROE.Frame.popupSupport(v.id, v.x, v.y);
                }                
            }).addClass("sfx2 ButtonTouch").removeClass("faded");

            ROE.getAnchorMode_IconOnly = false;
            var sendlink = ROE.getSendSilverAnchor(v.id, v.mine, v.x, v.y, v.pid, undefined, false);
            
            var tempStr = $('.sendtab .sends', _container).text(); // store temporarily as it will be removed in next line
            $('.sendtab', _container).empty().append(sendlink);
            $('.sendtab .sends', _container).addClass("sfx2 ButtonTouch").removeClass("faded").text(tempStr);
            

        }

        _container.find(".actions .supportlookup").html($(ROE.getSupportLookupAnchor(v.id, v.pid == ROE.playerID, true, undefined, v.pid)).text(_container.find(".actions .supportlookup").text()));
        _container.find(".actions .incoming").html($(ROE.getIncomingToVillageAnchor(v.id, v.name, v.x, v.y, v.pid, undefined)).text(_container.find(".actions .incoming").text()));
        _container.find(".actions .outgoing").html($(ROE.getOutgoingFromVillageAnchor(v.id, v.name, v.x, v.y, v.pid, undefined)).text(_container.find(".actions .outgoing").text()));
        _container.find(".actions .items").html($(ROE.getGiftsAnchor(v.id, v.pid == ROE.playerID, true, undefined)).text(_container.find(".actions .items").text()));
        _container.find(".actions .qr").html($(ROE.getRecruitAnchor(v.id, v.pid == ROE.playerID, true, undefined)).text(_container.find(".actions .qr").text()));
        _container.find(".actions .qb").html($(ROE.getHQAnchor(v.id, v.pid == ROE.playerID, true, undefined)).text(_container.find(".actions .qb").text()));

        if (!ROE.isMobile) {
            $(".ButtonTouch", _container).removeClass("ButtonTouch");
        }

        $('.mapit', _container).click(function () {
            if (ROE.Frame.CurrentView() != ROE.Frame.Enum.Views.map) {
                ROE.Frame.switchToView(ROE.Frame.Enum.Views.map);
            }
            ROE.Landmark.gotodb(v.x, v.y);
            ROE.Landmark.select();

            //we call it manually here to hurry up incase we have ported to a new section of the map with no village info yet
            ROE.Landmark.checkvills(); 

            if (ROE.isMobile) {
                // trigger click of the X element in header of and and all popups, so that the map can be shown
                //  because this could close many popups, we are suspending sounds
                ROE.UI.Sounds.supressAllUntil();
                $(".iFrameDiv").find(".header .action.close").click();
                ROE.UI.Sounds.supressAllUntil_resume();
            }
        });

       // $('.name', _container).html(v.name + ' (' + v.x + ', ' + v.y + ')');
        // $('.points', _container).html(v.points);
       
        CACHE.noteText = v.note == '0' ? '' : v.note;
        if (CACHE.noteText == "")
            $('.note .currentNote', _container).html('<em>' + $('#VillagePopupPhrases [ph=NoNotes]', _container).html() + '</em>');
        else
            $('.note .currentNote', _container).html(CACHE.noteText);
       
        //var note = v.note == '0' ? '' : v.note;
        //$('.note .view', _container).html(note);
        //$('.note .text', _container).val(ROE.Utils.toTextarea(note));

        if (ROE.isSpecialPlayer_Rebel(v.pid)) {
            $('.rebel', _container).show();
        } else if (ROE.isSpecialPlayer_Rebel(v.pid)) {
            $('.abandoned', _container).show();
        } else if (v.player) {
            $('tbody.tplayer', _container).show();
            $('.player', _container).html('<a onclick="ROE.Frame.popupPlayerProfile(\'' + v.player.PN + '\')">' + v.player.PN + '</a>');
            if (v.player.clan) {
                $('.clan', _container).html('<a onclick="">' + v.player.clan.CN + '</a>');
            }
        }


        _targetUpdate(v);
    }

    
    function _notePopup_show() {
       
        ROE.Frame.simplePopopOverlay('https://static.realmofempires.com/images/icons/M_ResearchList.png', "Village Notes", CACHE.notePopupTemplate[0].outerHTML, 'village_notePopup', $('#villageProfileDialog'));
        $('.village_notePopup .editNotesTextarea').val(ROE.Utils.toTextarea(CACHE.noteText));
    }

   
    function _notePopup_save() {
        var mapVillageObj;
        ROE.UI.Sounds.click();

        ROE.Frame.busy(undefined, undefined, _container);
        CACHE.noteText = $('.village_notePopup .editNotesTextarea').val();
        ROE.Api.call('village_other_save_note', { note: CACHE.noteText, vid: _vid }, function (t) {
         
            // If village_byid is available we'll update it so the map
            // view has the latest note data.
            if (ROE.Landmark.villages_byid) {
                
                mapVillageObj = ROE.Landmark.villages_byid[_vid];
                if (mapVillageObj) { // the landmarks may not have this village loaded, so this check is necessary
                    mapVillageObj.note = CACHE.noteText;
                    ROE.Landmark.savevill(_v);
                    ROE.Landmark.refill(true);
                    ROE.Landmark.pointdb(true);
                }
            }

            ROE.Frame.free(_container);
        });
   
        // Refresh the note.
       
        //$('.note .currentNote', _container).html(CACHE.noteText);
        if (CACHE.noteText == "")
            $('.note .currentNote', _container).html('<em>' + $('#VillagePopupPhrases [ph=NoNotes]', _container).html() + '</em>');
        else
            $('.note .currentNote', _container).html(CACHE.noteText);

    }

   
    // Expose the notePopup_edit function   
    obj.notePopup_save = _notePopup_save;
    obj.notePopup_show = _notePopup_show;

}(window.ROE.UI.VillageOverview = window.ROE.UI.VillageOverview || {}));