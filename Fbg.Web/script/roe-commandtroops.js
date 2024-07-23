"use strict";

(function (ROE) {
}(window.ROE = window.ROE || {}));


(function (obj) {

    var _targetPlayerId;
    var AttackType;
    var _originVillage;
    var _targetVID;
    var _targetXCord;
    var _targetYCord;
    var targetinfo;
    var AttackError;
    var replycode;
    var _fromVillageID;
    var rowTemp;
    var mainButtonSrc;
    var targetDistance;
    var isGovPresent;
    var attackItems;

    function _busy()
    {
        ROE.Frame.busy(undefined, undefined, $(".AttacksPopup"));
    }
    function _free() {
        ROE.Frame.free($(".AttacksPopup"));
    }


    var _init = function (attackType, targetVID, targetX, targetY, fromVID, troopsInfo) {

        // This is not a good way of doing this. We should really get a container, instead of doing $(".attack_popup"), but this is done this way for compatibility
        $(".AttacksPopup").empty().append(BDA.Templates.getRawJQObj("CommandTroopsPopup", ROE.realmID));

        _busy();

        rowTemp = $("#attack_popup .attackIconSheet");
        rowTemp.remove();

        targetinfo = "";
        AttackError = 0;
        replycode = "";

        AttackType = attackType;
        _targetVID = targetVID;
        _targetXCord = targetX;
        _targetYCord = targetY;
        _fromVillageID = fromVID;

        if (AttackType != 1) {
            $("#attack_popup #background IMG").attr("src", "https://static.realmofempires.com/images/misc/M_BG_Generic.jpg");
            mainButtonSrc = "M_Btn_Support";
        }
        else {
            mainButtonSrc = "M_Btn_Attack";
        }


        $("#attack_popup #doAttackButton").css("background-image", "url('https://static.realmofempires.com/images/buttons/" + mainButtonSrc + "3.png')");

        $("#attack_popup").delegate(".attackMax", "click", _addmaxunit);
        $("#attack_popup").delegate(".attackOne", "click", _addoneunit);
        $("#attack_popup").delegate("input", "keyup", _traveltime);
        $("#attack_popup").delegate("input", "change", _traveltime);

        $('#attack_popup').delegate(".savePreset", "click", _savePreset);

        $("#attack_popup .unitDieWhy").click(function (event) { event.stopPropagation(); ROE.UI.Sounds.click(); $("#unitDieInfo").toggleClass("targetDieInfo"); });
        $("#attack_popup .unitDesertWhy").click(function (event) { event.stopPropagation(); ROE.UI.Sounds.click(); $("#unitDesertInfo").toggleClass("targetDieInfo"); });


        $('#attack_popup .outgoing').show();

        BDA.Broadcast.subscribe($("#attack_popup"), "VillageExtendedInfoUpdated", _handleTroopCountInVillageChanged);
        BDA.Broadcast.subscribe($("#attack_popup"), "PlayerMoraleChanged", _updateMorale);

        if (ROE.isD2) {
            //BDA.Broadcast.subscribe($("#attack_popup"), "CurrentlySelectedVillageChanged", _handleSelectedVillageChangedEvent);
            BDA.Broadcast.subscribe($("#attack_popup"), "MapSelectedVillageChanged", _handleMapSelectedVillageChanged);
        }

        ROE.Villages.getVillage(fromVID, _populate, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
    }

    function _populate(village) {

        _originVillage = village;

        attackItems = {};
        attackItems.update = 0;

        var unitlist = village._TroopsList;
        var content = "";
        var unitsInVillage = false;
        var BuildingSelector = new Array();
        var TargetIMG = new Array();
        var DafaultTarget = new Array();
        var attackItemsid;
        var buildingID;
        attackItems.UnitTypePop = new Array();

        //reset - hack for now untill this page gets a nice rework
        targetinfo = "";
        $("#attack_popup .attackUnitList").empty();


        for (var i = 0; i < unitlist.length; i++) {

            var unitID = unitlist[i].id;
            attackItems.id = unitID;
            attackItems.max = unitlist[i].YourUnitsCurrentlyInVillageCount;
            attackItems.name = ROE.Entities.UnitTypes[attackItems.id].Name;
            attackItems.img = ROE.Entities.UnitTypes[attackItems.id].IconUrl_ThemeM;
            attackItems.speed = ROE.Entities.UnitTypes[attackItems.id].Speed;
            attackItems.strength = ROE.Entities.UnitTypes[attackItems.id].AttackStrength;
            attackItems.spy = ROE.Entities.UnitTypes[attackItems.id].SpyAbility;
            var TBuldingArray = ROE.Entities.UnitTypes[attackItems.id].AttackableBuildingIDs;
            attackItems.UnitTypePop[unitID] = ROE.Entities.UnitTypes[attackItems.id].Pop;

            //make Unit Target Selection Options
            if (TBuldingArray.length > 0) {

                BuildingSelector[attackItems.id] = "";
                var defaultTarget = 1;

                $.each(TBuldingArray, function (k, i) {

                    var buildingName = ROE.Entities.BuildingTypes[i].Name;
                    var buildingID = ROE.Entities.BuildingTypes[i].ID;
                    var buildingIMG = ROE.Entities.BuildingTypes[i].IconUrl_ThemeM;

                    TargetIMG[buildingID] = buildingIMG;

                    //if Ram then default selected is Walls
                    if (attackItems.id == 7) { DafaultTarget[attackItems.id] = 4; defaultTarget = 4; }

                    //if trebuche then default selected is Towers
                    if (attackItems.id == 8) { DafaultTarget[attackItems.id] = 7; defaultTarget = 7; }

                    BuildingSelector[attackItems.id] = BuildingSelector[attackItems.id] + "<DIV class='selectTarget sfx2'  data-targetunitid=" + attackItems.id + " data-targetid=" + buildingID + " > <IMG src='" + buildingIMG + "' > " + buildingName + "</DIV>";


                });

                attackItems.target = defaultTarget;
            }

            if (attackItems.max > 0) { attackItems.show = ""; var unitsInVillage = true; }
            else { attackItems.show = "unithide"; }

            content += BDA.Templates.populate(rowTemp.prop('outerHTML'), attackItems);
        }

        $("#attack_popup .attackUnitList").append(content);

        //target selection only for attack screen
        if (AttackType == 1) {

            //create Special Units Target Selection
            for (var i = 0; i < BuildingSelector.length; i++) {

                if (typeof BuildingSelector[i] != "undefined") {

                    $("#attackTarget_" + i).css("visibility", "visible");
                    $("#attack_popup .ATarget_" + i).append(BuildingSelector[i]);
                }
            }
        }

        if (!unitsInVillage) { $("#attack_popup .attackUnitList").append("<div class=attackButton style='width:100%;padding:20px 0;'>No Unit in the Village!</div>"); }


        var TargetText = '<span class="originvillagename fontGoldFrLClrg">' + village.name + "</span> (" + village.x + "," + village.y + ")";
        $(".currentVillage > span").html(TargetText);

        // direction 0 -current, 1 -target
        _populateTargetInfo(1, _targetXCord, _targetYCord, _targetVID);

        //var helpArea = $('<div>');
        //var chURL = changeTargetURL(_fromVillageID, _targetXCord, _targetYCord, helpArea);
        //var changeFromText = $(".changeFrom").html();

       

        var targetDistanceX = (village.x - _targetXCord);
        var targetDistanceY = (village.y - _targetYCord);
        targetDistance = Math.abs(Math.sqrt((targetDistanceX * targetDistanceX) + (targetDistanceY * targetDistanceY))).toFixed(2);

        $("#attack_popup #attackDistance >SPAN").html(targetDistance);

       

        //set default targets
        for (attackItemsid in DafaultTarget) {
            buildingID = DafaultTarget[attackItemsid];

            $("#attack_popup #attackTarget_" + attackItemsid + " >IMG").css("background-image", "url(" + TargetIMG[buildingID] + ")");
        }

        //target popup show
        $("#attack_popup .attackTarget").bind("click", function () {

            var targetUnitID = $(this).attr("data-unitid");
            var popupbox = $(".ATarget_" + targetUnitID).clone().css("display", "block").html();

            //call popup box template (title, width, vertAllign, bgcolor, content)
            ROE.Frame.popupInfo("Select Target Building", "230px", "center", "rgba(0,0,0,0.5)", popupbox);

            //create target click entry
            $(".selectTarget").bind("click", function () {

                var targetID = $(this).attr("data-targetid");
                var targetunitid = $(this).attr("data-targetunitid");

                $("#attack_popup #attackTarget_" + targetunitid + " >IMG").css("background-image", "url(" + TargetIMG[targetID] + ")");
                $("#attack_popup .ATarget_" + targetunitid).attr("data-targetid", targetID);

                //close this popup box
                ROE.Frame.popupInfoClose($(this));
            });
        });

        _updateMorale();

        _free();

        //update target village info
        ROE.Frame.busy('Getting Village Info...', 5000, $(".attackInfo"));
        ROE.Api.call("othervillageinfo", { vid: _targetVID }, _targetUpdate);


    }


    function _populateTargetInfo(direction, TargetX, TargetY, TargetID) {

        var TargetVillage = ROE.Landmark.villages[TargetX + "_" + TargetY];

        //if no landmark object yet
        if (typeof TargetVillage == "undefined") { ROE.Api.call("othervillageinfo", { vid: TargetID }, _othervillageinfo_callback) }
        else {
            var TargetVName = TargetVillage.name;
            var TargetMine = TargetVillage.mine;
            var TargetPlayer = TargetVillage.player.PN;
            var TargetText = '<span class="targetvillagename fontGoldFrLClrg">' + TargetVName + "</span> (" + TargetX + "," + TargetY + ")";

            if (direction == 1) {
                $("#attack_popup .targetVillage > span").html(TargetText);
                _targetPlayerId = TargetVillage.pid;
            }
            else { $("#attack_popup .currentVillage > SPAN").html(TargetText); }

            $('#attack_popup .outgoing .showingMsg').html("Showing incoming to %name%(%x%,%y%)".format({ name: TargetVName, x: _targetXCord, y: _targetYCord }));
        }
    }


    function _othervillageinfo_callback(response) {

        attackItems.update = 1;
        var TargetVName = response.VillageName;
        var TargetX = response.coords.X;
        var TargetY = response.coords.Y;
        var TargetText = '<span class="targetvillagename fontGoldFrLClrg">' + TargetVName + "</span> (" + TargetX + "," + TargetY + ")";

        if (_targetXCord == TargetX && _targetYCord == TargetY) {
            $("#attack_popup .targetVillage > span").html(TargetText);
        }
        else {
            $("#attack_popup .currentVillage > SPAN").html(TargetText);
        }

        $('#attack_popup .outgoing .showingMsg').html("Showing incoming to %name%(%x%,%y%)".format({ name: TargetVName, x: _targetXCord, y: _targetYCord }));

    }


    // previously known as : function _attackUnitUpdater() {
    function _handleTroopCountInVillageChanged(village) {
        var unitId;
        var newMax;

        if ($("#attack_popup").length > 0) {

            if (village.id == _fromVillageID) {
                $("#attackupdater").addClass("attackupdaterOut");

                var unitlist = village._TroopsList;

                for (var i = 0; i < unitlist.length; i++) {

                    unitId = unitlist[i].id;
                    newMax = unitlist[i].YourUnitsCurrentlyInVillageCount;

                    var thisUnit = $("#attack_popup #AttackUnit_" + unitId + " .attackMax > SPAN");
                    var oldMAX = thisUnit.html();

                    // for now, we do nothing here since the code has changed such that this message comes up 
                    //    unnecessarily. we might put it in later on, but not for now.
                    //if (oldMAX != newMax) {
                    //    //ROE.Frame.errorBar(_targetWarning(32));
                    //}

                    thisUnit.html(newMax);

                    if (newMax > 0 || oldMAX > 0) { $("#attack_popup #AttackUnit_" + unitId).fadeIn("1000"); }
                }
            }
        }
    }

    function _handleSelectedVillageChangedEvent() {
        /*
        console.log('commandtroops, CSV change.');
        ROE.Villages.getVillage(ROE.SVID, function _handleSelectedVillageChangedEvent_gotVillage(village) {
            console.log('commandtroops, got vill:', village);
            ROE.Api.call("othervillageinfo", { vid: village.id }, _targetUpdate);
        });*/
    }

    function _handleMapSelectedVillageChanged(t) {
        //ROE.Api.call("othervillageinfo", { vid: t.villageID }, _targetUpdate);
        if ($("#CommandTroopsPopup").dialog("isOpen") === true) {
            _init(AttackType, t.villageID, t.Cord.x, t.Cord.y, _fromVillageID, undefined);
        }
    }

    function _updateMorale() {
        ROE.PlayerMorale.display($('#attack_popup .commandMorale'));
    }
    

    function _targetUpdate(response) {

        if ($("#CommandTroopsPopup").dialog("isOpen") === false) {
            return; //if callback comes back but dialog is closed
        }

        var BeginnerProtectionEndsDate = response.BeginnerProtectionEndsDate;
        var IsCapitalVillage = response.IsCapitalVillage;
        var isCapitalVillage_ProtectionEndsInDays = response.IsCapitalVillage_ProtectionEndsInDays;
        var TargetPlayer = response.OwnerName;
        var InSleepModeUntil = response.InSleepModeUntil;
        var IsInVacationMode = response.IsInVacationMode;
        var IsInVacationModeUntill = response.IsInVacationModeUntill;
        var IsInWeekendMode = response.IsInWeekendMode;
        var IsInWeekendModeUntill = response.IsInWeekendModeUntill;
        var IsUnderBeginnerProtection = response.IsUnderBeginnerProtection;
        var TargetVName = response.VillageName;
        var TargetVClanID = response.clanID;
        var TargetIsAlly = response.clanDiplomacy_isAlly;
        var TargetIsNAP = response.clanDiplomacy_isNap;
        var TargetPoints = response.OwnerPoints;
        var PlayerPoints = ROE.Player.Ranking.points;
        _targetPlayerId = response.OwnerPlayerID;
        AttackError = 0;

        if (ROE.Player.Clan == null) { var PlayerClanID = -1; }
        else { var PlayerClanID = ROE.Player.Clan.id; }

        var oldTargetText = $("#attack_popup .targetVillage > span").html();

        //default MAX rebel target distance
        var targetMaxRebelDistance = 22;

        if (targetDistance >= targetMaxRebelDistance && _targetPlayerId == ROE.CONST.specialPlayer_Rebel) { targetinfo += _targetWarning(5) + "<BR>"; AttackError = 1; }

        if ((_fromVillageID == _targetVID || _targetPlayerId == ROE.playerID) && AttackType == 1) { targetinfo += _targetWarning(15) + "<BR>"; }

        if (TargetVClanID == PlayerClanID && AttackType == 1 && _targetPlayerId != ROE.playerID) { targetinfo += _targetWarning('TargetClanIsYours') + "<BR>"; }

        var TargetText = '<span class="targetvillagename fontGoldFrLClrg">' + TargetVName + "</span> (" + _targetXCord + "," + _targetYCord + ")";

        $("#attack_popup .targetVillage > span").html(TargetText);

        if (oldTargetText != TargetText && attackItems.update == 0) {
            $("#attack_popup .targetVillage > span").addClass("attackwarning");
            targetinfo += _targetWarning(16) + "<BR>";
        }

        if (InSleepModeUntil > 0 && AttackType == 1) {
            var sleepModeDate = new Date(InSleepModeUntil).toUTCString();
            targetinfo += _targetWarning(18) + " " + sleepModeDate + "<BR>";
            AttackError = 1;
        }

        if (IsInVacationMode && AttackType == 1) {
            var vacationExpireDate = new Date(IsInVacationModeUntill).toUTCString();
            targetinfo += _targetWarning(13) + " until " + vacationExpireDate + "<BR>";
            AttackError = 1;
        }

        if (IsInWeekendMode && AttackType == 1) {
            var wmExpireDate = new Date(IsInWeekendModeUntill).toUTCString();
            targetinfo += "In Weekend Mode until " + wmExpireDate + "<BR>";
            AttackError = 1;
        }


        if (IsUnderBeginnerProtection && AttackType == 1) {

            targetinfo += _targetWarning(17) + " " + BeginnerProtectionEndsDate + "<BR>";
            AttackError = 1;
        }
        if (TargetIsAlly && AttackType == 1) { targetinfo += _targetWarning(19) + "<BR>"; }
        if (TargetIsNAP && AttackType == 1) { targetinfo += _targetWarning(20) + "<BR>"; }
        if (!IsUnderBeginnerProtection && IsCapitalVillage && AttackType == 1) {
            targetinfo += _targetWarning(23).format({ d: isCapitalVillage_ProtectionEndsInDays, s: isCapitalVillage_ProtectionEndsInDays > 1 ? 's' : '' }) + "<BR>";
        }

        //Low Morale warning
        if (AttackType == 1 && ROE.PlayerMoraleSettings.isActiveOnThisRealm && ROE.Player.morale < ROE.PlayerMoraleSettings.minMorale_Normal) {
            targetinfo += "Your troops have low morale.<BR>";
/*
            if (localStorage.commandtroops_confirmlowmorale == "true") {
                targetinfo += "Your troops have low morale.<BR>";
            } else {
                AttackError = 1;
                var moraleWarning = "<div class='moraleWarningWrapper'>" +
                    "<div class='mw moraleWarningMsg'>Attacking while in low morale will have penalties, click on the Morale Bar at the top of this widow for details.</div>" +
                    "<div class='mw moraleWarningDisable'>Disable this warning,<br>Allow attack anyway.</div>" +
                "</div>";
                $("#attack_popup #doAttackButton").after(moraleWarning);
                $("#attack_popup .moraleWarningDisable").click(function () {
                    $("#attack_popup .moraleWarningWrapper").remove();
                    localStorage.commandtroops_confirmlowmorale = true;
                    _populate(_originVillage);
                });

            }
*/

        }

        $("#attack_popup #targetInfo SPAN").html(targetinfo);
        if (targetinfo != "") { $("#attack_popup #targetInfo").fadeIn(); }

        _traveltime();

        var handiCap = CalcBattleHandicap(PlayerPoints, TargetPoints, _targetPlayerId);
        if (AttackType == 1 && parseInt(handiCap) > 0) {
            $("#attack_popup #attackHandicap").show();
            $("#attack_popup #attackHandicap >SPAN").html(handiCap);
        } else {
            $("#attack_popup #attackHandicap").hide();
        }

        //
        //  display incoming / outgoing
        // this must be done only once!!
        //

        var inOutgoing;
        var filter;
        $('#attack_popup .outgoing .templateContent').remove();

        if (_targetPlayerId == ROE.playerID) {

            inOutgoing = ROE.Troops.InOutWidget.init(ROE.Troops.InOut2.Enum.Directions.incoming, $('#attack_popup .outgoing'), undefined, undefined, undefined, { displayFilter: false, hideToColumn : true, hideArrivalInColumn : true });
            filter = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, new ROE.Class.Village(_targetVID));
            inOutgoing.ToFromFilter(filter);
        } else {
            inOutgoing = ROE.Troops.InOutWidget.init(ROE.Troops.InOut2.Enum.Directions.outgoing, $('#attack_popup .outgoing'), undefined, undefined, undefined, { displayFilter: false, hideToColumn: true, hideArrivalInColumn: true });
            filter = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, new ROE.Class.Village(_targetVID));
            inOutgoing.ToFromFilter(filter);
        }

        ROE.Frame.free($(".attackInfo"));

    }


    function _saveattack() {

        ROE.UI.Sounds.click();

        var attackUnitList = []; //[{utID:6,sendCount:1,targetBuildingTypeID:1}]
        var unitlist = ROE.Entities.UnitTypes.SortedList;
        var total = 0;

        for (var i = 0; i < unitlist.length; i++) {

            var unitId = ROE.Entities.UnitTypes[unitlist[i]].ID;
            var unitCount = parseInt($("#attack_popup #AttackUnit_" + unitId + " input").val() * 1);
            var unitMax = parseInt($("#attack_popup #AttackUnit_" + unitId + " .attackMax > SPAN").html());
            var unitTarget = parseInt(($("#attack_popup .ATarget_" + unitId).attr("data-targetid")) * 1);

            if (unitTarget == 0) { unitTarget = 1; }

            total = total + unitCount;

            if (unitCount > 0) { attackUnitList.push({ utID: unitId, sendCount: unitCount, targetBuildingTypeID: unitTarget }); }

        }

        if (total > 0 && AttackError == 0) {

            _busy();

            //put a placeholder animation
            //temp ANIMATIONS D2 only
            if (!ROE.isMobile) {
                ROE.Animation.awaitingTroopCommand({
                    ovid: _originVillage.id,
                    ovx: _originVillage.x,
                    ovy: _originVillage.y,
                    dvid: _targetVID,
                    dvx: _targetXCord,
                    dvy: _targetYCord,
                    cmdtype: AttackType
                });
            }


            //send attack 

            ROE.Api.call_cmd_execute(_fromVillageID, _targetVID, AttackType, $.toJSON(attackUnitList), _saveAttackReply);
            ROE.Villages.ExtendedInfo_loadLatest(_fromVillageID);

            //make a quick unit update till API works
            for (var i = 0; i < attackUnitList.length; i++) {

                var utID = attackUnitList[i].utID;
                var count = attackUnitList[i].sendCount;
                var unitCount = $("#attack_popup .attackMax[data-unitid=" + utID + "] SPAN").text();
                var left = unitCount - count;

                $("#attack_popup #AttackUnit_" + utID + " input").val("");
                $("#attack_popup .attackMax[data-unitid=" + utID + "] SPAN").text(left);

            }

        }
        else { _reportError('noTroops'); }

    }

    function _savePreset() {

        var btn = $("#attack_popup .savePreset");
        if (!(btn.hasClass("grayout"))) {

            ROE.UI.Sounds.click();

            /*
            if (!btn.hasClass('areyousure')) {
                btn.attr('data-label', btn.html());
                btn.addClass('areyousure').html("Sure?");
                window.setTimeout(function () {
                    btn.removeClass('areyousure').html(btn.attr('data-label'));
                }, 2500);
                return;
            }
            */


            var attackUnitList = []; //[{utID:6,sendCount:1,targetBuildingTypeID:1}]
            var unitlist = ROE.Entities.UnitTypes.SortedList;
            var total = 0;

            for (var i = 0; i < unitlist.length; i++) {

                var unitId = ROE.Entities.UnitTypes[unitlist[i]].ID;
                var unitCount = parseInt($("#attack_popup #AttackUnit_" + unitId + " input").val() * 1);
                var unitMax = parseInt($("#attack_popup #AttackUnit_" + unitId + " .attackMax > SPAN").html());
                var unitTarget = parseInt(($("#attack_popup .ATarget_" + unitId).attr("data-targetid")) * 1);

                if (unitTarget == 0) { unitTarget = 1; }

                total = total + unitCount;

                if (unitCount > 0) { attackUnitList.push({ utID: unitId, sendCount: unitCount, targetBuildingTypeID: unitTarget }); }

            }

            var newPreset = {
                type: 'csv',
                customText: 'New Preset',
                troops: attackUnitList,
                commandType: AttackType
            }

            ROE.PresetCommands.externalNewPreset(newPreset);

            btn.hide(); //so they cant accidentally spam it
        }
    }

    function _saveAttackReply(reply) {
        var replycode = reply.canCommand;

        $("#attack_popup #targetInfo").hide();

        if (replycode != 0) { //error report

            if (replycode < 14) {
                _reportError(replycode);
            } else if (replycode == 14) {
                ROE.Frame.errorBar("Target is in WeekendMode. Can not attack.");
            } else if (replycode == 15) {
                ROE.Frame.errorBar("Cannot command troops without subscription. Enable Subscription to be able to command your troops.");
            } else if (replycode == 16) {
                ROE.Frame.errorBar("Cannot support other players on this realm");
            }

            _free();
        }
        else { //success, troops sent
            ROE.Player.refresh(); //triggers reload of incoming/outgoing immediatelly 

            if (AttackType == 1) { var infotext = _targetWarning("0b"); }
            else { var infotext = _targetWarning("0a"); }

            ROE.Frame.infoBar(infotext);

            _attackBtnDisable();

            //empty
            $("#attack_popup .attackButton input").val("");
            $("#attack_popup .desertFactor >SPAN").html("");
            $("#attack_popup .desertFactor").hide();
            $("#attack_popup #attackTraveltime > SPAN").html("00:00:00");
            $("#attack_popup #attackLandingtime .landdate").html(_targetWarning(30));
            $("#attack_popup #attackLandingtime .landtime").removeClass("Time").removeAttr("id").html("00:00:00");
            $("#attack_popup #unitDie").hide();
            $("#attack_popup #unitDieInfo").removeClass("targetDieInfo");
            $("#attack_popup #Desertion").hide();
            $("#attack_popup #unitDesertInfo").removeClass("targetDieInfo");
            $("#attack_popup #unitDesert").hide();
        }
        //
        // update morale?
        if (ROE.Player.morale != reply.playerMoraleAfterCmd) {
            ROE.Player.morale = reply.playerMoraleAfterCmd;
            BDA.Broadcast.publish("PlayerMoraleChanged");
        }
        _free();
    }


    function _addmaxunit() {

        var unitid = $(this).attr("data-unitid");
        var unitmax = parseInt($(this).children().text());

        $("#attack_popup #AttackUnit_" + unitid + " input").val(unitmax);

        _traveltime();
    }


    function _addoneunit() {

        var unitid = $(this).attr("data-unitid");
        $("#attack_popup #AttackUnit_" + unitid + " input").val(1);

        _traveltime();
    }


    function _traveltime() {

        var armySpeed = 999999;
        var totalStrength = 0;
        var total = 0;
        var totalSpy = 0; //this appears to be a total spy strength or something
        var WillDie = 0;
        var UnitTotalByPop = 0;
        isGovPresent = false;
        var spyOnly = false;
        var unitlist = ROE.Entities.UnitTypes.SortedList;
        var totalSpyUnitCount = 0; //number of spy units total

        for (var i = 0; i < unitlist.length; i++) {

            var unitId = ROE.Entities.UnitTypes[unitlist[i]].ID;
            var unitCount = parseInt($("#attack_popup #AttackUnit_" + unitId + " input").val() * 1);
            var unitSpeed = parseInt($("#attack_popup #AttackUnit_" + unitId).attr("data-unitspeed"));
            var unitMax = parseInt($("#attack_popup #AttackUnit_" + unitId + " .attackMax > SPAN").text());
            var unitSpy = parseInt($("#attack_popup #AttackUnit_" + unitId).attr("data-unitspy"));
            var unitStrenth = parseInt($("#attack_popup #AttackUnit_" + unitId + " .attackTarget").attr("data-unitstr"));

            totalStrength += unitCount * unitStrenth;
            totalSpy += unitCount * unitSpy;


            //validate unit entry
            if (unitCount > unitMax || unitCount < 0) { $("#attack_popup #AttackUnit_" + unitId + " input").addClass("attackwarning2"); unitCount = -99999999999999; }
            else { $("#attack_popup #AttackUnit_" + unitId + " input").removeClass("attackwarning2"); }

            if (unitCount > 0 && unitSpeed < armySpeed) { armySpeed = unitSpeed; }
            if (unitCount > 0 && unitId == 10) { isGovPresent = true; }
            if (unitId == 12) { totalSpyUnitCount += unitCount; }


            total += unitCount;

            UnitTotalByPop += (unitCount * attackItems.UnitTypePop[unitId]);
        }

        if (total > 0 && totalSpyUnitCount == total) { spyOnly = true; }

        var travelTimeSecs = parseInt((targetDistance / armySpeed) * 3600);

        //troop speed modifiers only affect Attack
        if (AttackType == 1) {

            //if noGovPresent, is special player, and rebel rush active, then 20x less travel time
            if (!isGovPresent && ROE.isSpecialPlayer(_targetPlayerId) && ROE.Player.PFPckgs.isActive(32)) {
                travelTimeSecs = travelTimeSecs / 20;
            }

            //if a morale realm, affect morale onto speed (spy only attacks, and attacks with Govs, dont get affected by speed mod)
            if (ROE.PlayerMoraleSettings.isActiveOnThisRealm && !spyOnly && !isGovPresent && ROE.isSpecialPlayer(_targetPlayerId)) {
                travelTimeSecs = travelTimeSecs / ROE.PlayerMoraleSettings.Effects[ROE.Player.morale].moveSpeed;
            }

        }

        var travelHour = ("0" + parseInt(travelTimeSecs / 3600)).slice(-2);
        var travelMin = ("0" + parseInt((travelTimeSecs - (3600 * travelHour)) / 60)).slice(-2);
        var travelSec = ("0" + parseInt(travelTimeSecs - (3600 * travelHour) - (60 * travelMin))).slice(-2);

        $("#attack_popup #attackTraveltime > SPAN").html(travelHour + ":" + travelMin + ":" + travelSec);        
        $("#attack_popup #targetError SPAN").text("");
        $("#attack_popup #targetError").hide();

        //set up Arriving Time Counter
        if (total > 0) {
            $("#attack_popup #attackLandingtime .landtime").addClass("Time").attr('data-showtoday', true).attr('data-offset', travelTimeSecs * 1000);
        } else {
            $("#attack_popup #attackLandingtime .landtime").removeClass("Time").removeAttr("id").html("00:00:00");
        }

        ROE.Clock.reClock();

        /// give warning if only rams and/or treb and/or lord is being sent.
        /// total strength zero and Not spies
        if ((totalStrength <= 0 && total > 0) && totalSpy == 0 && AttackType == 1) {
            $("#attack_popup #unitDie").addClass("targetError").show();

        }
        else { $("#attack_popup #unitDie").hide(); $("#attack_popup #unitDieInfo").removeClass("targetDieInfo"); }

        if (AttackType == 1) {
            // get desert factor
            var Desertion = GetUnitDesertion(targetDistance, UnitTotalByPop, _targetPlayerId);

            var totalDeserter = 0;


            for (var i = 0; i < unitlist.length; i++) {

                var unitId = ROE.Entities.UnitTypes[unitlist[i]].ID;
                var unitCount = parseInt($("#AttackUnit_" + unitId + " input").val() * 1);

                var deserter = Math.round(unitCount * Desertion);

                totalDeserter += deserter;

                if (deserter == 0) {
                    $("#attack_popup #AttackUnit_" + unitId + " .desertFactor >SPAN").html("");
                    $("#attack_popup #AttackUnit_" + unitId + " .desertFactor").hide();
                }
                else {
                    $("#attack_popup #AttackUnit_" + unitId + " .desertFactor >SPAN").html(deserter);
                    $("#attack_popup #AttackUnit_" + unitId + " .desertFactor").show();
                }
            }

            var DesertRate = Math.round((totalDeserter / total) * 100);

            if (parseInt(DesertRate) > 0) {

                $("#attack_popup #Desertion").show();
                $("#attack_popup #Desertion >SPAN").html(DesertRate + "%");
            }
            else {
                $("#attack_popup #Desertion").hide();
                $("#attack_popup #unitDesertInfo").removeClass("targetDieInfo");
                $("#attack_popup #unitDesert").hide();
            }

            if (totalDeserter == total && total > 0) {
                $("#attack_popup #unitDesert").addClass("targetError").show();
                $("#attack_popup #unitDesert span:first-child").html(_targetWarning(21));

            }
            else {

                if (totalDeserter > 0) {
                    $("#attack_popup #unitDesert").addClass("targetError").show();
                    $("#attack_popup #unitDesert span:first-child").html(_targetWarning(22));
                }

            }

        }

        $("#attack_popup #targetInfo SPAN").html(targetinfo);
        if (targetinfo != "") { $("#targetInfo").fadeIn(); }

        //LOW MORALE WARNING
        $("#attack_popup .moraleWarningWrapper").remove();
        if (AttackType == 1 && ROE.PlayerMoraleSettings.isActiveOnThisRealm &&
            ROE.Player.morale < ROE.PlayerMoraleSettings.minMorale_Normal && localStorage.commandtroops_confirmlowmorale != "true") {
            AttackError = 1;
            var moraleWarning = "<div class='moraleWarningWrapper'>" +
                "<div class='mw moraleWarningMsg'>Attacking while in low morale will have penalties, click on the Morale Bar at the top of this widow for details.</div>" +
                "<div class='mw moraleWarningDisable'>Disable this warning,<br>Allow attack anyway.</div>" +
            "</div>";
            $("#attack_popup #doAttackButton").after(moraleWarning);
            $("#attack_popup .moraleWarningDisable").click(function () {
                $("#attack_popup .moraleWarningWrapper").remove();
                localStorage.commandtroops_confirmlowmorale = true;
                _traveltime();
            });
        }

        if (total > 0) {
            $('#attack_popup .savePreset').show();
        } else {
            $('#attack_popup .savePreset').hide();
        }

        if (total > 0 && AttackError == 0) {
            _attackButtonEnable();
        } else {
            _attackBtnDisable();
        }

    }

    function GetUnitDesertion(targetDistance, unitsByPopulation, _targetPlayerId) {

        var UnitDesertionScalingFactor = ROE.CONST.UnitDesertionScalingFactor;
        var DesertionMinDistance = ROE.CONST.UnitDesertionMinDistance;
        var UnitDesertionMaxPopulation = ROE.CONST.UnitDesertionMaxPopulation;
        var isSpecialPlayer = ROE.isSpecialPlayer(_targetPlayerId);

        if (AttackType == 1 && !isGovPresent
                   && targetDistance > DesertionMinDistance
                   && unitsByPopulation <= UnitDesertionMaxPopulation
                   && !isSpecialPlayer
                   && UnitDesertionScalingFactor > 0) {

            var a = Math.pow(targetDistance - DesertionMinDistance, 2) * UnitDesertionScalingFactor;
            var b = Math.pow(unitsByPopulation, 2);

            var Desertion = (1 - (1 / (a / b + 1)));

        }
        else { var Desertion = 0; }

        return Desertion;
    }

    function CalcBattleHandicap(attackersPoints, defendersPoints, _targetPlayerId) {

        var actualHandicap = 0;
        var Param_MaxHandicap = ROE.CONST.Handicap_MaxHandicap;
        var Param_StartRatio = ROE.CONST.Handicap_StartRatio;
        var Param_Steepness = ROE.CONST.Handicap_Steepness;
        var isSpecialPlayer = ROE.isSpecialPlayer(_targetPlayerId);

        if (ROE.CONST.IsBattleHandicapActive) {

            if (isSpecialPlayer) {
                defendersPoints = 100000000; //hack, no handicap on those, so setting to ridiculously large number
            }

            if (attackersPoints <= 0 || defendersPoints <= 0) {

                var actualHandicap = 0;
            }
            else {
                var ratio = (attackersPoints / defendersPoints);

                if (ratio <= Param_StartRatio) {
                    var actualHandicap = 0;
                }
                else {
                    //double logValB = 2 * Math.Log10(ratio - 3);

                    //actualHandicap = 0.5 - 0.5 * Math.Pow(4, -0.25 * ((logValB * logValB)));
                    var logValB = 2 * Math.log(ratio - Param_StartRatio + 1) / Math.LN10;

                    var actualHandicap = Param_MaxHandicap - Param_MaxHandicap * Math.pow(Param_StartRatio, -0.25 * Param_Steepness * ((logValB * logValB)));

                }
            }
        } 

        var actualHandicap = (actualHandicap * 100).toFixed(1) + "%";

        return actualHandicap;
    }

    function _attackBtnDisable() {
        $("#attack_popup #doAttackButton")
            .removeClass("pressedEffect")
            .css("background-image", "url('https://static.realmofempires.com/images/buttons/" + mainButtonSrc + "3.png')")
            .unbind();
    }

    function _attackButtonEnable() {
        $("#attack_popup #doAttackButton")
            .addClass("pressedEffect")
            .css("background-image", "url('https://static.realmofempires.com/images/buttons/" + mainButtonSrc + ".png')")
            .click(_saveattack);
    }

    function _reportError(r) {
        $("#attack_popup #targetError SPAN").html($('#attack_popup .phrases [ph=' + r + ']').html());
        $("#attack_popup #targetError").fadeIn();
    }

    function _targetWarning(r) {
        return $('#attack_popup .phrases [ph=' + r + ']').html();
    }

    obj.init = _init;

}(window.ROE.CommandTroops = window.ROE.CommandTroops || {}));