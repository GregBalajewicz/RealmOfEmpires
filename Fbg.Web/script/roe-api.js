
ROE.Api = {
    apiLocationPrefix: ''

    // Set this value to true, and all API calls that you make will fail. This
    // is useful for checking to see if your failure conditions are robust.
    // The error callback will be called with all-nulls as its arguments, so
    // be sure to handle that.
    , forceApiFailures: false

    , call: function (functionName, param, suc, err, async, timeoutInMS, type) {
        ///<summary>Call any ROE REST API call</summary>
        /// <param name="functionName">name of the function. must be specififed</param>
        // Note: Passing *null* as the error callback means that there is *no* error
        // callback. Passing *undefined* as the error callback is equivalent to not
        // specifying an error callback, and thus, will use the *default* error callback.

        if (!functionName) { throw "functionName required"; }
        BDA.Console.verbose('roe.api', 'call ' + functionName + ' args[' + JSON.stringify(param) + ']');

        // if caller has not specified an error function, we give it a default one
        // We ONLY check for undefined here; if it's null, leave it as null.
        if (err === undefined) {
            err = function DefaultErrorCallback() {
                // if frame busy is active, then we fail it, cause clearly the user is waiting for something, and call went wrong. 
                if (ROE.Frame.isBusy()) {
                    ROE.Frame.busyFail();
                }
            };
        }

        if (ROE.Api.forceApiFailures) {
            BDA.Console.error('roe.api', 'call %fn%-failed : debug forced failure'.format({ fn: functionName }));
            if (err) { err(null, null, null); }
            return;
        }

        return ajax(ROE.Api.apiLocationPrefix + "api.aspx?fn=" + functionName, param,
            function (data) { // success function call back 
                BDA.Console.verbose('roe.api', 'call ' + functionName + '-returned'); BDA.Console.verbose('roe.api', data);
                if (suc) {
                suc(data);
                }
            },
            function (jqXHR, textStatus, errorThrown) { // error function call back                
                if (err) {
                    err(jqXHR, textStatus, errorThrown)
                } else {
                    if (textStatus != 'timeout') {
                        var roeex = new BDA.Exception('api call %fn%-failed : %textStatus%, %errorThrown%'.format({ textStatus: textStatus, errorThrown: errorThrown, fn: functionName }));
                        BDA.latestException = roeex;
                        throw roeex;
                    }
                }
            } ,
            async, timeoutInMS, type);
    }
    , logerror: function (jsonSerializedMessage) {
        $.ajax({
            url: ROE.Api.apiLocationPrefix + "api.aspx?fn=logE"
          , dataType: 'string'
          , data: { msg: jsonSerializedMessage }
          , success: function () { }
          , async: true
          , type: "POST"
        });
    }

    , call_incomingTroops: function (callback, err) { ROE.Api.call("Incoming", {}, callback, err); }
    , call_getPlayerPFPackages: function (callback, err) { ROE.Api.call("PF_GetPlayerPFPackages", {}, callback, err); }
    , call_playerRefresh: function (svid, ts, callback, err) { ROE.Api.call("playerRef", { vid: svid, lastHandledVillagesCacheTimeStamp : ts }, callback, err, true, 10000); }
    , call_playerFull: function (svid, callback) { ROE.Api.call("playerFull", { vid: svid }, callback); }
    , call_activatePFPackage: function (pckgid, callback) { ROE.Api.call("pf_activatepackage", { pckgid: pckgid }, callback); }
    , call_researchDo: function (researchItemID, villageID) { ROE.Api.call("research_do", { rid: researchItemID, vid: villageID }, callback); }
    , call_research_catchup: function (callback) { ROE.Api.call("research_catchup", {}, callback); }
    , call_boostLoyalty: function (villageID, callback) { ROE.Api.call("boostapproval", { vid: villageID }, callback); }
    , call_upgradeAll_getinfo: function (villageID, callback) { ROE.Api.call("upgrade_all_getupgradeinfo", { vid: villageID }, callback); }
    , call_upgrade_getinfo: function (villageID, buildingID, callback) { ROE.Api.call("upgrade_getupgradeinfo", { vid: villageID, bid: buildingID }, callback); }
    , call_recruit_getinfo: function (villageID, buildingID, callback) { ROE.Api.call("recruit_getrecruitinfo", { vid: villageID, bid: buildingID }, callback); }
    , call_upgrade_doupgrade: function (villageID, buildingID, callback) { ROE.Api.call("upgrade_doupgrade", { vid: villageID, bid: buildingID }, callback); }
    , call_upgrade_doupgrade2: function (villageID, buildingID, levelToUpgradeTo, callback) { ROE.Api.call("upgrade_doupgrade2", { vid: villageID, bid: buildingID, levelToUpgradeTo: levelToUpgradeTo }, callback); }
    , call_upgrade_cancel: function (villageID, buildingID, eventID, isQ, callback) { ROE.Api.call("upgrade_cancelupgrade", { vid: villageID, bid: buildingID, eid: eventID, isq: isQ }, callback); }
    , call_upgrade_cancel2: function (villageID, eventID, isQ, callback) { ROE.Api.call("upgrade_cancelupgrade2", { vid: villageID, eid: eventID, isq: isQ }, callback); }
    , call_upgrade_speedupupgrade: function (villageID, buildingID, eventID, timeInMin, callback) { ROE.Api.call("upgrade_speedupupgrade", { vid: villageID, bid: buildingID, eid: eventID, min: timeInMin }, callback); }
    , call_upgrade_speedupupgrade2: function (villageID, buildingID, eventID, timeInMin, callback) { ROE.Api.call("upgrade_speedupupgrade2", { vid: villageID, bid: null, eid: eventID, min: timeInMin }, callback); }
    , call_upgrade_speedupupgradefree: function (villageID, eventID, callback) { ROE.Api.call("upgrade_speedupupgradefree", { vid: villageID, eid: eventID }, callback); }
    , call_recruit_dorecruit: function (villageID, buildingID, recruitCommands, callback) { ROE.Api.call("recruit_dorecruit", { vid: villageID, bid: buildingID, recruitCommands: recruitCommands }, callback); }
    , call_recruit_dorecruit2: function (villageID, recruitCommands, callback) { ROE.Api.call("recruit_dorecruit2", { vid: villageID, recruitCommands: recruitCommands }, callback); }
    , call_recruit_cancel: function (villageID, buildingID, eventID, callback) { ROE.Api.call("recruit_cancel", { vid: villageID, bid: buildingID, eid: eventID }, callback); }
    , call_recruit_cancel2: function (villageID, buildingID, eventID, callback) { ROE.Api.call("recruit_cancel2", { vid: villageID, bid: buildingID, eid: eventID }, callback); }
    , call_recruit_all_recruitinfo: function (villageID, callback) { ROE.Api.call("recruit_all_recruitinfo", { vid: villageID }, callback); }
    , call_disband: function (villageID, recruitCommands, callback) { ROE.Api.call("disband", { vid: villageID, recruitCommands: recruitCommands }, callback); }
    , call_downgrade_dodowngrade: function (villageID, buildingID, callback) { ROE.Api.call("downgrade_dodowngrade", { vid: villageID, bid: buildingID }, callback); }
    , call_downgrade_cancel: function (villageID, eventID, isQ, callback) { ROE.Api.call("downgrade_cancel", { vid: villageID, eid: eventID, isq: isQ }, callback); }
    , call_downgrade_speedup: function (villageID, eventID, callback) { ROE.Api.call("downgrade_speedup", { vid: villageID, eid: eventID }, callback); }
    , call_gov_getinfo: function (villageID, callback) { ROE.Api.call("gov_getinfo", { vid: villageID }, callback); }
    , call_gov_buychest: function (villageID, count, callback) { ROE.Api.call("gov_buychest", { vid: villageID, count: count }, callback); }
    , call_gov_buychest2: function (villageID, count, returnRecruitInfoInsteadOfJustGovInfo, callback) { ROE.Api.call("gov_buychest", { vid: villageID, count: count, returnRecruitInfoInsteadOfJustGovInfo: returnRecruitInfoInsteadOfJustGovInfo }, callback); }
    , call_gov_buychest_leavesilver: function (villageID, silverToLeave, returnRecruitInfoInsteadOfJustGovInfo, callback) { ROE.Api.call("gov_buychest_leavesilver", { vid: villageID, silverToLeave: silverToLeave, returnRecruitInfoInsteadOfJustGovInfo: returnRecruitInfoInsteadOfJustGovInfo }, callback); }
    , call_gov_cancelrecruit: function (villageID, callback) { ROE.Api.call("gov_cancelrecruit", { vid: villageID }, callback); }
    , call_gov_dorecruit: function (villageID, callback) { ROE.Api.call("gov_dorecruit", { vid: villageID }, callback); }
    , call_getCreditPackages: function (callback) { ROE.Api.call("getcreditpackages", { dvc: ROE.isDevice }, callback); }
    , call_getMyDailyReward: function (selected, callback) { ROE.Api.call("getmydailyreward", { sel: selected }, callback); }
    , call_getClanPublicProfile: function (clanID, callback) { ROE.Api.call("getclanpublicprofile", { cid: clanID }, callback); }
    , call_getClanMemberList: function (clanID, callback) { ROE.Api.call("getclanmemberlist", { cid: clanID }, callback); }
    , call_getClanDiplomacy: function (clanID, callback) { ROE.Api.call("getclandiplomacy", { cid: clanID }, callback); }
    , call_getClanInvites: function (playerID, callback) { ROE.Api.call("getclaninvites", { pid: playerID }, callback); }
    , call_recoveryEmail: function (email, callback) { ROE.Api.call("recoveryemail", { e: email }, callback); }
    , call_getBonusTypes: function (villageID, callback) { ROE.Api.call("getbonustypes", { vid: villageID }, callback); }
    , call_setBonusType: function (villageID, bonusType, callback) { ROE.Api.call("setbonustype", { vid: villageID, bt: bonusType }, callback); }
    , call_restart: function (callback) { ROE.Api.call("restart", {}, callback); }
    , call_activateVacation: function (duration, callback) { ROE.Api.call("activatevacation", { duration: duration }, callback); }
    , call_troopMove_cancel: function (callback, eventID) { ROE.Api.call("troopMovement_cancel", { eventID: eventID }, callback); }
    , call_troopMove_toggleHide: function (callback, eventID) { ROE.Api.call("troopmovement_togglehide", { eventID: eventID }, callback); }
    , call_troopMove_getDetails: function (callback, eventID) { ROE.Api.call("troopmovement_getdetails", { eventID: eventID }, callback); }
    , call_report_miscreport: function (reportID, callback) { ROE.Api.call("report_getmiscreport", { recordid: reportID }, callback); }
    , call_report_battlereport: function (reportID, callback) { ROE.Api.call("report_getbattlereport", { recordid: reportID }, callback); }
    , call_report_supportattackedreport: function (reportID, callback) { ROE.Api.call("report_getsupportattackedreport", { recordid: reportID }, callback); }
    , call_report_forward: function (reportID, playerName, callback) { ROE.Api.call("report_forward", { record: reportID, player: playerName }, callback); }
    // api solution for transfering credits
    , call_transfer_credits: function (amount, transferToPlayerName, callback) { ROE.Api.call("transfercredits", { amount: amount, pname: transferToPlayerName }, callback); }
    , call_get_max_transfer_credits: function (callback) { ROE.Api.call("maxtransfercredits", {}, callback); }
    , call_silvertransport_getnearestvillages: function (targetVillageId, minAmount, xNumVillages, callback) { ROE.Api.call("silvertransport_getnearestvillages", { vid: targetVillageId, minAmount: minAmount, xNumVillages: xNumVillages }, callback); }
    , call_silvertransport_getnearestvillagesforeign: function (targetVillageId, callback) { ROE.Api.call("silvertransport_getnearestvillagesforeign", { vid: targetVillageId }, callback); }    
    , call_silvertransport_getmaxsilverfromnearestvillages: function (targetVillageId, minAmount, xNumVillages, callback) { ROE.Api.call("silvertransport_getmaxsilverfromnearestvillages", { vid: targetVillageId, minAmount: minAmount, xNumVillages: xNumVillages }, callback); }
    //, call_silvertransport_sendamount: function (vidFrom, vidTo, amount, callback) { ROE.Api.call("silvertransport_sendamount", { vidFrom: vidFrom, vidTo: vidTo, amount: amount }, callback); }    
    , call_silvertransport_sendamount: function (vidFrom, vidTo, vToX, vToY, amount, isMine, callback) { ROE.Api.call("silvertransport_sendamount", { vidFrom: vidFrom, vidTo: vidTo, vToX: vToX, vToY: vToY, amount: amount, isMine: isMine }, callback); }
    , call_getservertimeoffset: function (localTimeCall, callback) { ROE.Api.call("getservertimeoffset", { localTimeCall: localTimeCall }, callback); }
    , call_othervillageinfo: function (vid, callback) { ROE.Api.call("othervillageinfo", { vid: vid }, callback); }
    //, call_quest_completed: function (questTag, callback) { ROE.Api.call("quest_completed", { qtag: questTag }, callback); } // Too risky
    , call_playermapevent_activate: function (eventID, typeID, xCord, yCord, callback) { ROE.Api.call("playermapevent_activate", { eventID: eventID, typeID: typeID, xCord: xCord, yCord: yCord }, callback); }
    , call_tr_userinfo: function (urlParams, callback) { ROE.Api.call("tr_userinfo", { uid: urlParams.uid, pid: urlParams.pid, rid: urlParams.rid, viewerpid: urlParams.viewerpid }, callback); }
    , call_tr_saveplayerlistsetting: function (pid, displayStatus, callback) { ROE.Api.call("tr_saveplayerlistsetting", { pid: pid, displayStatus: displayStatus }, callback); }
    , call_tr_renameuser: function (newName, callback) { ROE.Api.call("tr_renameuser", { newName: newName }, callback); }
    , call_tr_realmleaderboard: function (rid, callback) { ROE.Api.call("tr_realmleaderboard", { rid: rid }, callback); }
    , call_tr_setlike: function (uid, pid, callback) { ROE.Api.call("tr_setlike", { uid: uid, pid: pid }, callback); }
    , call_tr_togglechatvip: function (status, callback) { ROE.Api.call("tr_togglechatvip", { status: status }, callback); }
    , call_user_setavatarid: function (avatarid, callback) { ROE.Api.call("user_setavatarid", { avatarid: avatarid }, callback); }
    , call_user_setsex: function (sex, callback) { ROE.Api.call("user_setsex", { sex: sex }, callback); }
    , call_avatars_getall: function (callback) { ROE.Api.call("avatars_getall", {}, callback); }
    , call_avatars_purchase: function (avatarid, callback) { ROE.Api.call("avatars_purchase", { avatarid: avatarid }, callback); }
    , call_claimVillage: function (vid, callback) { ROE.Api.call("villageclaim", { vid: vid}, callback); }
    , call_claimVillage_unclaim: function (vid, callback) { ROE.Api.call("villageclaim_unclaim", { vid: vid }, callback); }
    , call_items2_myitems: function (callback) { ROE.Api.call("items2_getmyitems", null, callback); }
    , call_items2_myitemgroups: function (getLatest, callback) { ROE.Api.call("items2_getmyitemgroups", { getLatest: getLatest }, callback); }
    , call_items2_use: function (vid, iid, groupID, callback) { ROE.Api.call("call_items2_use", { vid: vid, itemid: iid, groupID: groupID }, callback); }

    //quest related API
    , call_quest_getcompletedcount: function (callback) { ROE.Api.call("quest_getcompletedcount", {}, callback); }
    , call_next_recommended_quest: function (callback) { ROE.Api.call("next_recommended_quest", {}, callback); }
    , call_quest_getrewardforcompleted: function (questTag, callback) { ROE.Api.call("quest_getrewardforcompleted", { questTag: questTag}, callback); }

    , call_playermapevent_caravan_cardreveal: function (eventID, callback) { ROE.Api.call("playermapevent_caravan_cardreveal", { eventID: eventID }, callback); }
    , call_playermapevent_caravan_cardpick: function (cardindex, eventID, callback) { ROE.Api.call("playermapevent_caravan_cardpick", { cardindex: cardindex, eventID: eventID }, callback); }
    , call_playermapevent_caravan_cardreroll: function (eventID, callback) { ROE.Api.call("playermapevent_caravan_cardreroll", { eventID: eventID }, callback); }
    , call_playermapevent_caravan_catchup: function (eventID, callback) { ROE.Api.call("playermapevent_caravan_catchup", { eventID: eventID }, callback); }
    , call_player_setavatarid: function (avatarid, callback) { ROE.Api.call("player_setavatarid", { avatarid: avatarid }, callback); }
    , call_player_setsex: function (sex, callback) { ROE.Api.call("player_setsex", { sex: sex }, callback); }
    , call_cmd_execute: function (fromVillageID, targetVID, attackType, attackUnitList, callback) { ROE.Api.call("cmd_execute", { ovid: fromVillageID, tvid: targetVID, cmdtype: attackType, units: attackUnitList, v: 2 }, callback); }

    , call_definedtargets_add: function (vid, type, settime, note, expiresInXDays, assignedTo, callback) { ROE.Api.call("definedtargets_add", { vid: vid, type: type, settime: settime, note: note, expiresInXDays: expiresInXDays, assignedTo: assignedTo }, callback); }
    , call_definedtargets_edit: function (definedtargetid, settime, note, expiresInXDays, assignedTo, callback) { ROE.Api.call("definedtargets_edit", { definedtargetid: definedtargetid, settime: settime, note: note, expiresInXDays: expiresInXDays, assignedTo: assignedTo }, callback); }
    , call_definedtargets_delete: function (definedtargetid, callback) { ROE.Api.call("definedtargets_delete", { definedtargetid: definedtargetid }, callback); }
    , call_definedtargetresponse_editdeladd: function (definedtargetid, responsetypeid, response, callback) { ROE.Api.call("definedtargetresponse_editdeladd", { definedtargetid: definedtargetid, responsetypeid: responsetypeid, response: response }, callback); }
    
    , call_weekendModeRequest: function (desiredActivationDate, callback) { ROE.Api.call("weekendmoderequest", { desiredActivationDate: desiredActivationDate }, callback); }
    , call_weekendModeCancel: function (callback) { ROE.Api.call("weekendmodecancel", { }, callback); }
    , call_vaar: function (callback) { ROE.Api.call("vaar", {}, callback); }

    //raids api
    , call_raids_getPlayerRaids: function (callback) { ROE.Api.call("raids_getplayerraids", {}, callback); }
    , call_raids_sendRaid: function (ovid, raidid, attackUnitList, callback) { ROE.Api.call("raids_sendraid", { ovid: ovid, raidid: raidid, units: attackUnitList }, callback); }
    //, call_raids_getraidplayermovements: function (raidid, callback) { ROE.Api.call("raids_getraidplayermovements", { raidid: raidid }, callback); }
    , call_raids_getraiddetails: function (raidid, callback) { ROE.Api.call("raids_getraiddetails", { raidid: raidid }, callback); }
    , call_raids_acceptRewards: function (raidid, callback) { ROE.Api.call("raids_acceptrewards", { raidid: raidid }, callback); }
};