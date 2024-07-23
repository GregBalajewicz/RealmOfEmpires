var vovNutTest = 0;
(function (obj) {
    BDA.Console.setCategoryDefaultView('ROE.Frame', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file

    var infoTimeout = 0;
    var Enum = {}; // private enums
    Enum.Views = { vov: "vov", map: "map", research: "research" };
    obj.Enum = Enum;

    /*
     {
        id : number         // id of this widget
        widget : <object>   // reference to this widget
        cssclass : string      // class 
        pinned : bool       // pinned or not indicator
    }
    */
    var _InOutWidgets = [{}, {}];
    var _view = Enum.Views.map;
    var _newReportsNotified = false;
    var CONST = {};
    CONST.Selector = {
        mainButtonPanel: {
            mail: ".ui-buttonpanel-main .mail",
            reports: ".ui-buttonpanel-main .reports",
            clan: ".ui-buttonpanel-main .clan",
            inOutTroops: ".ui-buttonpanel-main .inOutTroops",
            settings: ".ui-buttonpanel-main .settings",
            rankings: ".ui-buttonpanel-main .rankings",
        },
        sidePane: {
            battleSim: "#linkBattleSim"
        },
        uiButtons: {
            linkQuests: "#linkQuests",
            socialInvite: "#socialInvite"
        }
        , incomingAlert_Empire: ".inout-alert .incomming-alert"
        , outgoingAlert_Empire: ".inout-alert .outgoing-alert"
        , currentVillage: {
            incomingAlert: ".vov-inout-alert.inc"
            , outgoingAlert: ".vov-inout-alert.out"
        }
    };

    function _populateOrRefreshFrame_CommonStuff() {

        /*  // the animated progress bar looks good on its own, the text flashing doesnt, leaving it out for now - farhad april 2016   
        var previousPoints = parseInt($(".hdrPoints").text());
        if (BDA.Utils.formatNum(ROE.Player.Ranking.points > previousPoints )) {
            var ptsDiv = $(".hdrPoints").text(BDA.Utils.formatNum(ROE.Player.Ranking.points));
            ptsDiv.removeClass('effect-credits-positive').addClass('effect-credits-positive')
            .on("webkitAnimationEnd oanimationend msAnimationEnd animationend", function () {
                ptsDiv.removeClass('effect-credits-positive');
            });
        }else{
            $(".hdrPoints").text(BDA.Utils.formatNum(ROE.Player.Ranking.points));
        }
        */

        $(".hdrPoints").text(BDA.Utils.formatNum(ROE.Player.Ranking.points));
        $(".player-ranking-progress .progress-indicator").css("width", String(ROE.Player.Ranking.titleProgress) + "%");
        $(".player-ranking .player-level").html("lvl " + ROE.Player.Ranking.titleLvl);
        $(".player-ranking .player-title").html(ROE.Player.Ranking.title);

        $('.ui-buttonpanel-main .reports').toggleClass('newReport effect-pulse', ROE.Player.Indicators.report);
        $('.ui-buttonpanel-main .mail').toggleClass('newMail effect-pulse', ROE.Player.Indicators.mail);
        $('.ui-buttonpanel-main .clan').toggleClass('newClan effect-pulse', ROE.Player.Indicators.clanForum);


        if (ROE.Player.beginnerProtected) {
            $('.vovAlert.beginerProtectionTimer').show();
            $('.vovAlert.beginerProtectionTimer .protCountdown').addClass('countdown').attr('data-finisheson', ROE.Player.beginnerProtection);
        } else {
            $('.vovAlert.beginerProtectionTimer').hide();
            $('.vovAlert.beginerProtectionTimer .protCountdown').removeClass('countdown').removeAttr('data-finisheson');
        }

        ROE.Frame.handleSleeModeIcon();
        ROE.Frame.handleResearchIcon();
        ROE.Frame.handleDailyReward();
        ROE.Frame.handleTargetsListIcon();
        _handleQuestIcon();

        if (ROE.Player.Ranking.levelUp) {
            ROE.Player.Ranking.levelUp = false;
            _showLevelUpPopup();
        }

        if (ROE.Player.Ranking.titleLvl > 2) {
            $("#playOnMinfo").show();
            $("#socialInvite").show();
        }

        initTimers();
    }

    var _populateFrame = function () {
        _populateOrRefreshFrame_CommonStuff();

        ROE.Player.numberOfVillages < 2 ? $('.popVillageList').hide() : $('.popVillageList').show();

        //$("#hdrPlrLvl").text(ROE.Utils.addThousandSeperator(ROE.Player.Ranking.titleLvlR));         what is this?
        $(".player-ranking .player-name").html(ROE.Player.name);
        $(".subpanel-left .player-avatar-frame").attr('data-playername', ROE.Player.name);

        _refreshPFHeader(ROE.Player.credits);
        _handleRXRealm();

    };

    //special things to do in D2 frame for RX realm
    var _handleRXRealm = function _handleRXRealm() {
        if (ROE.rt == "X") {
        }
    }

    var _reloadVillageInfo = function (village) {
        $("#vovFrame").attr('data-vid', village.id);
        $("#vovFrame .villName").text(village.name);
        $("#vovFrame .villCords").text("(" + village.x + "," + village.y + ")");
        $("#vovFrame .villPoints").text(/*"[" +*/ village.points/* + "]"*/);
        $("#currentVillageInfo .currCoins").text(" " + BDA.Utils.formatNum(village.coins)).toggleClass('maxed', village.coins >= village.coinsTresMax).attr("data-vid", village.id);
        $("#currentVillageInfo .maxCoins").text("/" + BDA.Utils.formatNum(village.coinsTresMax));
        $("#currentVillageInfo .foodWrapper").toggleClass('maxed', !village.Buildings[8].Upgrade.nextLevel);
        $("#currentVillageInfo .currFood").text(BDA.Utils.formatNum(village.popRemaining));
        $("#currentVillageInfo .loyaltyWrapper").toggleClass('under100', village.yey < 100);
        $("#currentVillageInfo .currLoyalty").text(village.yey);

        var villageType = ROE.Entities.VillageTypes[village.villagetypeid];
        villageType.IsBonus ? $("#vovFrame .bonusIcon").css('background-image', "url('" + villageType.LargeIconUrl + "')").show() : $("#vovFrame .bonusIcon").hide();
        
        //Update VoV Troops
        var troopsRow = $("#currentVillageInfo .troopsPanel .troopRow").empty();       
        var troopList = village.TroopList;
        if (troopList) {
            for (var t = 0; t < troopList.length; t++) {
                troopsRow.append('<td class="troop"><span class="tN" style="background-image:url(\'' +
                    ROE.Entities.UnitTypes[troopList[t].id].IconUrl_ThemeM + '\')">' + troopList[t].YourUnitsCurrentlyInVillageCount + '</span></td>');
            }
        }

        //hide / show VoV Action buttons based on player number of villages
        if (ROE.Player.numberOfVillages < 2) {
            $("#vovFrame .actionButton.qb, #vovFrame .actionButton.qr, #vovFrame .actionButton.st").hide();
            $("#vovFrame .actionButton.bu").show();
        } else {
            $("#vovFrame .actionButton.qb, #vovFrame .actionButton.qr, #vovFrame .actionButton.st").show();
            $("#vovFrame .actionButton.bu").hide();
        }

        //update VoV live tooltips
        ROE.Tooltips.vovCoinWrapperLive = ROE.Tooltips.vovCoinWrapperBase.format({
            vcwt1: BDA.Utils.formatNum(village.coins),
            vcwt2: BDA.Utils.formatNum(village.coinsTresMax),
            vcwt3: BDA.Utils.formatNum(Math.round(village.coinsPH))
        });

        ROE.Tooltips.vovFoodWrapperLive = ROE.Tooltips.vovFoodWrapperBase.format({
            vft1: BDA.Utils.formatNum(village.popMax),
            vft2: BDA.Utils.formatNum(village.popCur),
            vft3: BDA.Utils.formatNum(village.popRemaining)
        });
      
        ROE.MapEvents.checkAddVOVCaravan(); //mainly to show caravans if any
    };

    var _reloadFrame = function (callBack) {
        ///<summary>reloads all the info that is on the vov header. optionally, pass a call back to be called when done.</summary>
        ROE.Player.load(function () {
            _populateFrame();
            if (callBack) {
                callBack();
            }
        });
    };

    var _timedRefreshFrame = function () {
        ///<summary>this does not refresh everything, just some select info</summar>
        ROE.Player.refresh(_timedRefreshFrameCallBack, _timedRefreshFrameCallBack_timeout, _timedRefreshFrameCallBack_error);

        setTimeout(_timedRefreshFrame, ROE.CONST.headerRefreshRate); // what if refresh fails... do we make the refresh longer?
    };
    
    var _timedRefreshFrameCallBack = function () {
        // since we know playerrefresh now completed OK, hide any errors/ warnings the coudl have resulted from playerrefresh call
        $('.networkerrors_slow').hide();
        $('.networkerrors_noconnection').hide();

        _populateOrRefreshFrame_CommonStuff()

        if (ROE.Player.Indicators.report) {
           
            BDA.Broadcast.publish("NewReports");
            
            // Turn on beta features to test this notification feature:
            // Only want to broadcast the first time, otherwise will keep being called.
            if (!_newReportsNotified && BetaFeatures.status('DesktopNotifications') == 'ON') {
                BDA.Client.pushLocalNotify("Realm of Empires", "New reports available.", "https://static.realmofempires.com/images/icons/m_Reports.png");
            }
            // Set this to true
            _newReportsNotified = true;
        }

        if (ROE.Player.Indicators.mail) {
            BDA.Console.verbose('ROE.Frame', 'new mail indicated');
            BDA.Broadcast.publish("NewMail");
        }

        if (ROE.Player.chooseGovType) {
            $('.selectGovType').show();
            $('.inviteFriendsReward').hide(); //since the buttons share space
        }

        if (ROE.Realm.isSaleActive) {
            $('.ui-panel.ui-panel-main .buyServants').addClass("sale");
        } else {
            $('.ui-panel.ui-panel-main .buyServants').removeClass("sale");
        }
        /* not in D2 for now
        if (!ROE.Player.hasOffer2) {
            $(".Offer2").remove();
        } else {
            $(".Offer2").show();
        }
        */
        //ROE.Realm.friendRewardBonusUntil = 1379312000000;
        var inviteRewardIcon = $('.inviteFriendsReward');
        if (ROE.Realm.friendRewardBonusUntil > (new Date)) {
            inviteRewardIcon.addClass('promo');
            if($('.counter', inviteRewardIcon).length == 0){
                inviteRewardIcon.append('<span class="counter countdown" data-finisheson="' + ROE.Realm.friendRewardBonusUntil + '"></span>');
            }          
        } else {
            inviteRewardIcon.removeClass('promo').find('.countdown').remove();
        }


        //recovery email
        if (!ROE.Player.recEmailSet && ROE.Player.recEmailState < 2 && ROE.Player.Ranking.titleLvl > 2 ) {
            $(".emailEntryIcon").show();
        }
        else {
            $(".emailEntryIcon").hide();
        }

        ROE.Frame._checkIfReloadNeededDueToUpdate();

        ROE.Frame._fireActivityEvent('D2');

    };

    function _timedRefreshFrameCallBack_timeout() {
        $('.networkerrors_slow').fadeIn();
    }
    function _timedRefreshFrameCallBack_error() {
        $('.networkerrors_noconnection').fadeIn();
        $('.networkerrors_slow').hide();
    }

    var _showLevelUpPopup = function () {
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'levelUp').length !== 0) {
            return; // already showing
        }
       
        ROE.UI.Sounds.clickVictory();
        ROE.Api.call("acceptnewtitle", {}, function () {

            ROE.Frame.reloadFrame();
            ROE.Player.Ranking.levelUp = false;
        });
                
        var newTitle = ROE.Titles[ROE.Player.Ranking.titleID() + 1];
        var nextTitle = ROE.Titles[ROE.Player.Ranking.titleID() + 2];
        var dataForTemplate = {};
        dataForTemplate.newTitleName = newTitle.name;
        dataForTemplate.nextTitleName = nextTitle.name;
        dataForTemplate.xp = newTitle.xp;
        dataForTemplate.titleID = newTitle.id;
        dataForTemplate.realmID = ROE.realmID;
        dataForTemplate.realm_NewTitle = "";
        dataForTemplate.giftName_NewTitle = "";
        dataForTemplate.giftImg_NewTitle = "";
        
        if (newTitle.unlockedRealmName.length !== 0) {
            dataForTemplate.realm_NewTitle = newTitle.unlockedRealmName[0];
        }

        // track title acceptance event
        mixpanel.track('Title', { 'TitleID': newTitle.id, 'RealmID': ROE.realmID });
        
        var itemImage = new Array;
        itemImage["Silver"] = "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png";
        itemImage["Infantry"] = "https://static.realmofempires.com/images/units/Infantry_m.png";
        itemImage["Light Cavalry"] = "https://static.realmofempires.com/images/units/Cavalry_M.png";
        itemImage["Spy"] = "https://static.realmofempires.com/images/units/Spy_m.png";
        itemImage["Knight"] = "https://static.realmofempires.com/images/units/Knight_m.png";
        itemImage["Ram"] = "https://static.realmofempires.com/images/units/ram_m.png";
        itemImage["Trebuchet"] = "https://static.realmofempires.com/images/units/Treb_m.png";

        
        var popupTemp = BDA.Templates.getRawJQObj("LevelUpNew", ROE.realmID).html();
        var content = BDA.Templates.populate(popupTemp, dataForTemplate);
        
        if (newTitle.unlockedGift.length !== 0) {
            
            var images = '<span class="itemtext2">';
            var contentArray = content.split(images);
            
            for (var i = 0; i < newTitle.unlockedGift.length; i++) {
                
                images += "<img src='" + itemImage[newTitle.unlockedGift[i].title] + "' >";
            }
            var content = contentArray[0] + images + contentArray[1];
        }                

        ROE.Frame.simplePopopOverlay("https://static.realmofempires.com/images/icons/M_chalice.png", "LEVEL UP!", content, 'levelUpOverlay d2centered');

        if (newTitle.unlockedGift.length === 0) {
            $("#popup_levelUp .iteminfo").hide();
        }
        else {
            $("#popup_levelUp .general").hide();
        }

        if (newTitle.unlockedRealmName.length === 0) {
            $("#popup_levelUp .realminfo").hide();
        }

        $("#popup_levelUp .itemstore IMG").click(function () {

            ROE.Frame.popupGifts();
            $('.simplePopupOverlay').remove();
        });
        
        $("#popup_levelUp .globalxp").html("You've gained " + newTitle.xp + " global experience.");
        
        $("#popup_levelUp .questclaim").click(function () {
            ROE.UI.Sounds.click();
            $('.levelUpOverlay').remove();
            _showIframeOpenDialog('#questsDialog', 'Quests.aspx');
        });
        
    };

    var _showVillList = function () {
        /// <summary>
        /// Show the UI with village list
        ///</summary> 
        ROE.UI.Sounds.click();
        ROE.UI.VillageList.init($('#villageListDialog').dialog('open'));       
    };

    // DEAD CODE ?
    //var _closePopup = function () {
    //    $('.iFrameDiv .header .action.close').click();
    //};

    var _showNextVOV = function (direction, optionalCallBackFn, travelToText) {
        /// <summary>shows previous or next village in the list</summary>
        /// <param name="direction" type="string">prev | next</param>
        /// <param name="optionalCallBackFn" type="fucntion"> optional call back function once the new village is switched - note that this will fire before the actual VOV repaints it self</param>
        /// <param name="travelToText" type="string">OPTIONAL. by default, text is "Traveling to" or you can override it</param>

        if (ROE.Player.numberOfVillages < 2) { return; } 
        ROE.Frame.busy('Preparing village list.', 5000, $('#vovFrame .vovFrameContent'));
        travelToText = travelToText || "Traveling to"; // default text if not specified
        ROE.Villages.getVillages(function getVillagesCallBack(villageList) {
            ROE.Frame.free($('#vovFrame .vovFrameContent'));
            for (var i = 0; i < villageList.length; i++) {
                if (villageList[i].id == ROE.SVID) {
                    var j = i + (direction === 'prev' ? -1 : 1);
                    if (j >= villageList.length) {
                        j = 0;
                    } else if (j < 0) {
                        j = villageList.length - 1;
                    }
                    ROE.Frame.busy(travelToText + ' ' + villageList[j].name, 5000, $('#vovFrame .vovFrameContent'));
                    ROE.Frame.setCurrentlySelectedVillage(villageList[j].id, villageList[j].x, villageList[j].y, villageList[j].name);
                    break;
                }
            }
        });
    };

    function _showThisVOV2(village) {
       
        if (village.id != ROE.SVID) {
            // this call back is not for the current village. Perhaps the selected village has changed before the call back came back, so ignore this. 
            ROE.Frame.free($('#vovFrame .vovFrameContent'));
            return;
        }

        ///<summary> this function assumes that the current view is VOV</summary>        
        BDA.Console.verbose('ROE.Frame', 'Showing village :%vid%'.format({ vid: ROE.SVID }));

        _updateInOutgoingForSelectedVillage();
        _populateVOV(village.VOV);
        _reloadVillageInfo(village);     
    };

    function _visitPrevVillage() {
        _showNextVOV('prev');
    }

    function _visitNextVillage() {
        _showNextVOV('next');
    }

    function _centerThisVillage(x, y) {
        ROE.Landmark.gotodb(x, y);
        ROE.Landmark.select();
        //_refill(true);
    }

    function _boostYay() {

        //if boost spell still on cooldown
        if (ROE.Player.approvalBoostAllowedWhen > BDA.Utils.getUTCNow()) {
            _errorBar('Cant boost again untill: ' + BDA.Utils.formatEventTime(new Date(ROE.Player.approvalBoostAllowedWhen)));
            return;
        }

        if (ROE.Player.credits < 200) {
            ROE.UI.Sounds.clickMenuExit();
            _infoBar('Need 200 servants to cast this spell.');
            return;
        }

        if (confirm("Spend 200 servants to boost the approval to 100% in this village?")) {
            ROE.UI.Sounds.clickSpell();
            ROE.Frame.busy('Rallying the town!', 5000, $('#vovFrame #vovFrameContent'));
            ROE.Api.call_boostLoyalty(ROE.SVID, _callback_onBoostLoyalty);
        }
    }

    function _callback_onBoostLoyalty(data) {
        ROE.Frame.free($('#vovFrame .vovFrameContent'));
        if (data.boostSuccessful) {
            _reloadFrame();
            ROE.Villages.getVillage(ROE.SVID, function _callback_onBoostLoyaltyGetVillage(v) {
                ROE.Villages.__populateExtendedData(v.id, v, data.Village);
            }, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
        } else {

            if (data.ifNotBoostedThenisDueToCoolDown) {
                //fail due to cooldown
                _errorBar('Loyalty boost failed: still on cooldown.');
            } else {
                //generic fail
                _errorBar('Boost failed.');
            }
            
        }
    }

    var _showServantsPopup = function () {
        /// <summary>
        /// Show the UI with servants and related info 
        ///</summary>
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'servants').length === 0) {
            ROE.UI.Sounds.click();
            var popupContent = $("#pfDialog").dialog('open');
            var settings = {
                mode: "popup", // popup || inline
                fill: popupContent, //for popup mode, popup content must be passed, for inline mode any element can be passed
                featureId: null //for inline mode only, display one PF
            }
            ROE.UI.PFs.init(settings, _refreshPFHeader); //initializes the widget
        }
    };

    var _showPFStatus = function () {
        var element = $('.pfStatus').empty();
        var row = '<div data-pfpackageid="%id%" class="pfItemWrap hoverScale helpTooltip" data-toolTipID="pfDesc%id%"><div class="pfItem" style="background-image:url(%iconUrl%);"></div></div>';
        var pfObj, rowPopulated, isActive, expDate;
        for (var pf in ROE.PFPckgs) {
            pfObj = ROE.PFPckgs[pf];          
            if (!pfObj.Id) { continue; }
            isActive = ROE.Player.PFPckgs.isActive(pfObj.Id);
            rowPopulated = BDA.Templates.populate(row, {
                id: pfObj.Id,
                title: pfObj.desc,
                iconUrl: isActive ? pfObj.icon_ActiveL : pfObj.icon_NotActiveL
            });
            if (isActive && ROE.Player.PFPckgs.list[pfObj.Id]) {
                expDate = BDA.Utils.timeLeft(new Date(ROE.Player.PFPckgs.list[pfObj.Id].ExpiresOn));
                rowPopulated = $(rowPopulated).append($('<div class="countdown" format="long2"  data-refreshCall="ROE.Frame.refreshPFHeader();">' + expDate.h + ":" + expDate.m + ":" + expDate.s + '</div>'));
            }
            element.append(rowPopulated);
        }
        //populate spell tooltips. This is done only once, so not included in the loop above.
        if (!element.hasClass('init')) {
            element.addClass('init');
            for (var pf in ROE.PFPckgs) {
                pfObj = ROE.PFPckgs[pf];
                if (!pfObj.Id) { continue; }
                ROE.Tooltips['pfDesc' + pfObj.Id] = pfObj.desc;
            }
        }
    };

    var _refreshPFHeader = function (newCredits) {

        if (typeof (newCredits) != 'undefined') {
            //get old count from attr and not ROE.Player.credits because it could get updated from player.load or elsewhere
            var oldCredits = $(".playerCredits").attr('data-credits');
            ROE.Player.credits = newCredits;
            $(".playerCredits").attr('data-credits', newCredits).text(' ' + BDA.Utils.formatNum(newCredits));
            if (newCredits > oldCredits) {
                _flashCreditCount();
            }        
        }

        _showPFStatus();
    };

    var _flashCreditCount = function () {
        var creditsElement = $(".ui-panel-main .subpanel-right .playerCredits");
        creditsElement.removeClass('effect-credits-positive').addClass('effect-credits-positive')
            .on("webkitAnimationEnd oanimationend msAnimationEnd animationend", function () {
                creditsElement.removeClass('effect-credits-positive');
            });
    }

    //explains map notifications
    var _mapNotifWhy = function _mapNotifWhy(id) {
        if(id == 1){ //servant farm
            var title = "Map Event Notification";
            var content = $('<div class="mapNotifWhyDialog">');
            var info = $('<div class="info">').html('Attacking Rebel or Abandoned villages gives you a chance to rescue some servants.<br><br>' +
                'Once you\'ve rescued a servant, you can click and collect them from the map. They never expire.<br><br>' + 'Now go and find your rescued servants!');
            content.append(info);
            _popGeneric(title, content, 320, 230);
        }
    }

    var _switchToVoV = function () {
        BDA.Console.error("Not implemented, should not be use used in D2");
    };

    var _switchToMap = function (x, y) {
        ///<summary>
        ///     call this to force the frame to switch to the map view centered at the x and y 
        ///     passed in. 
        ///     It is safe to call this from anywhere and this takes care of everything - but make sure to close your own popup or window if applicable
        ///</summary>      

        ///this way seems to be quicker, also matches the 'mapIt' way from illage overview .js
        ROE.Landmark.gotodb(x, y);
        ROE.Landmark.select();

    };

    var _switchToView = function (view, customfunctionToLoadView) {
        BDA.Console.error("Not implemented, should not be use used in D2");
    };

    var _load_map = function () {
        ///<summary>loads the map centerd on the player's currently selected village</summary>        
        _fnLoad_mapByCoords(ROE.CSV.x, ROE.CSV.y);
    };

    var _fnLoad_mapByCoords = function (x, y) {
        ROE.Landmark.start(x, y, 1);
    };


    var _populateVOV = function (village) {
        ROE.vov._initializeNewVOV();
        ROE.vov.init(village);
        ROE.Frame.free($('#vovFrame .vovFrameContent'));
    };

    var _reloadView = function (callback) {
        BDA.Console.error("Not implemented, should not be use used in D2");
    };
  
   
    var _popupMail = function () {       
        ROE.UI.Sounds.click();

        var youveGotMail = $('#linkMail').hasClass('newMail');
        // Immediately check if we need to remove the new mail style because
        // the user is checking the mail right now, so nothing else new ...
        if (youveGotMail)
            $('.ui-buttonpanel-main .mail').toggleClass('newMail effect-pulse', false);

        ROE.Mail.init($('#mailDialog'), youveGotMail);
        $('#mailDialog').dialog('open');
    };

    var _popupMailPrefilled = function (to, subject, message) {
      
        var youveGotMail = $('#linkMail').hasClass('newMail');
        if (youveGotMail)
            $('.ui-buttonpanel-main .mail').toggleClass('newMail effect-pulse', false);

        ROE.Mail.init($('#mailDialog'), youveGotMail, { to: to, subject: subject, message: message });
        $('#mailDialog').dialog('open');

    }


    var _popupHighlight = function (e) {
        e.preventDefault();
        ROE.Map.Highlights.init($('#mapHighlightsDialog'));
        $('#mapHighlightsDialog').dialog('open');               
        return false;
    };

    var _popupMapSummary = function (e) {
        e.preventDefault();
        ROE.Map.Summary.init($('#mapSummaryDialog'));
        $('#mapSummaryDialog').dialog('open');     
        return false;
    };

    var _showBuildPopup = function () {
        ROE.UI.Sounds.click();        
        $('#buildDialog').html('<div class="BuildPopup"></div>').dialog('open');
        ROE.Build.init($('#buildDialog .BuildPopup'));
    };

    var _showSilverTransportPopup = function (villageId, isMine) {
        ROE.UI.Sounds.click();
        ROE.SilverTransport.init($('#transportSilverDialog').dialog('open'), villageId, isMine);
    };


    var _popupInOutSummary = function (directionID /*1|0*/
        ) {        
        ROE.Map.Summary.init($('#mapSummaryDialog'));
        var dialog = directionID == ROE.Troops.InOut2.Enum.Directions.incoming ? $('#incomingSummarDialog') : $('#outgoingSummarDialog');
        
        ROE.Troops.InOutSummary.init(directionID, dialog);
        dialog.dialog('open');
        return false;
    };

    var _popupClan = function (clanid, forumPostID, section) {        
        var frameSrc = "ClanOverview.aspx"; // Default will be clan overview page
        if (clanid === '0' || !clanid) {
            if (section) {
                switch (section) {
                    case "overview":               
                        frameSrc = 'ClanOverview.aspx';
                        break;
                    case "forumpost":
                        frameSrc = 'ShowThread.aspx?ID=' + forumPostID;
                        break;                    
                }            
            }
        } else {
            frameSrc = 'ClanPublicProfile.aspx?clanid=' + clanid;
        }
        _showIframeOpenDialog('#clanDialog', frameSrc);
        return false;
    };

    var _popupStats = function (section) {
        
        var frameSrc = "stats.aspx";
        if (section) {
            switch (section) {
                case "clanranking":
                    frameSrc = "ClanRanking.aspx";
                    break;
            }
        } 
        _showIframeOpenDialog('#rankingsDialog', frameSrc);
        return false;
    }

    /*
    var _popupAvatar = function () {
        ROE.Avatar.init($('#avatarDialog').dialog('open'));
    }
    */

    function _showIframeOpenDialog(dialogSelector, frameSrc, dialogTitle) {
        ROE.Frame.base_showIframeOpenDialog(dialogSelector, frameSrc, dialogTitle);
    }
    
    var _popupReports = function (optionalSearchString) {
        if (localStorage.ReportPopup == 'false') return true; // old
        ROE.UI.Sounds.click();
        ROE.Reports.init($('#reportsDialog'), null, optionalSearchString);
        _checkedReports(); //set flags to false, must be after initiating reports popup
        $('#reportsDialog').dialog('open');
    };

    var _checkedReports = function () {
        $('.ui-buttonpanel-main .reports').toggleClass('newReport effect-pulse', false);
        _newReportsNotified = false;
        ROE.Player.Indicators.report = false;
    }

    var _popupPlayerProfile = function (name) {
        ROE.PlayerNew.init($('#playerProfileDialog'), name);
        $('#playerProfileDialog').dialog('open');
        return false;
    };

    var _popupVillageProfile = function (id) {       
        ROE.UI.VillageOverview.init($('#villageProfileDialog'), id);
        $('#villageProfileDialog').dialog('open');
        return false;      
    };

    var _popupAttacks = function (vID, cordX, cordY) {
        _popupCommandTroops(1, vID, cordX, cordY, ROE.SVID);
    };

    var _popupSupport = function (vID, cordX, cordY) {
        _popupCommandTroops(0, vID, cordX, cordY, ROE.SVID);

    };

    var _popupVillageNote = function (village) {

        _popGeneric('Village Note', "<div class='editVillageNotePopup'></div>", 400, 400);

        var content = $('.editVillageNotePopup.genericDialogContent');
        
        var textarea = $('<textarea class="textAreaInput" />')
        content.append(textarea);

        var saveButton = $('<div class="BtnDSm2n saveNote">Save</div>').click(function () {
            ROE.UI.Sounds.click();
            ROE.Frame.busy('Saving', 5000, content);

            var noteText = $('.textAreaInput', content).val();
            ROE.Api.call('village_other_save_note', { note: noteText, vid: village.id }, function (t) {

                // If village_byid is available we'll update it so the map view has the latest note data.
                if (ROE.Landmark.villages_byid) {

                    mapVillageObj = ROE.Landmark.villages_byid[village.id];
                    if (mapVillageObj) { // the landmarks may not have this village loaded, so this check is necessary
                        //why substring here? because in qMapBySquares.sql the info is substringed as well, so this would match the display
                        mapVillageObj.note = noteText.toString().substring(0, 30); 
                        ROE.Landmark.savevill(mapVillageObj);
                        ROE.Landmark.refill(true);
                        ROE.Landmark.pointdb(true);
                    }
                }

                ROE.Frame.free(content);
            });
            $('#genericDialog').dialog('close');
        });

        content.append(saveButton);

        ROE.Frame.busy('Getting village info...', 5000, content);
        ROE.Api.call('othervillageinfo', { vid: village.id }, function (vcb) {
            ROE.Frame.free(content);
            textarea.val(ROE.Utils.toTextarea(vcb.note));
        });
    }

    function _popupCommandTroops(attackType, vID, cordX, cordY, currentVillageID) {
        var pageType = ["Support", "Attack"];

        $('#CommandTroopsPopup').dialog('option', 'title', pageType[attackType]);
        $('#CommandTroopsPopup').dialog('open');
        ROE.CommandTroops.init(attackType, vID, cordX, cordY, currentVillageID);
        
        return false;
    };

    function _popupQuickCommand(AttackType, TARGETVID, COORX, COORY) {
        var AttackTypeName = ["War Room: Support", "War Room: Attack"]; //support label / attack label
        var AttackTypeIcon = ["https://static.realmofempires.com/images/icons/M_Support.png",
            "https://static.realmofempires.com/images/icons/m_attacks.png"]; //support icon / attack icon

        $('#quickCommandDialog').dialog('option', 'title', AttackTypeName[AttackType]);
        $('#quickCommandDialog').dialog('open');
        ROE.QuickCommand.init($('#quickCommandDialog'), AttackType, TARGETVID, COORX, COORY);
        
        if (AttackType == 1) {
            ROE.Tutorial.startIfNotAlreadyRan(1, 'warRoomDesktop');
        }
        return false; //supress A click 
    };

    var _popupGifts = function (optionalVillageID) {
        ROE.UI.Sounds.click(); 
        ROE.Gifts.init($('#giftsDialog').dialog('open'), optionalVillageID);
    };

    var _popupGiftSend = function () {
        _showIframeOpenDialog('#giftSendDialog', 'Gift_Send.aspx');
    }

    var _popupQuests = function () {
        _showIframeOpenDialog('#questsDialog', 'Quests.aspx');
    }

    var _settingsPopup = function (indexOfDefaultTab) {
        ROE.Settings.init($('#settingsDialog'), indexOfDefaultTab);
        $('#settingsDialog').dialog('open');
    };

    var _popupAccount = function (e) {
        ROE.UI.Sounds.click();
        ROE.Account.init($('#accountDialog'));
        $('#accountDialog').dialog('open');
        return false;
    };


    var _popupInOut_togglePin = function _popupInOut_togglePin(id, btn) {
        var widgetObj = _popupInOut_findByID(id);
        if (widgetObj != undefined) {
            widgetObj.pinned = widgetObj.pinned ? false : true;
            if (widgetObj.pinned) {
                btn.addClass('pinned');
                _infoBar("pinned. window will not be reused");
            } else {
                btn.removeClass('pinned');
                _infoBar("un pinned. window will be reused");
            }
            
        }
    }

    var _popupInOut_getNextID = function _popupInOut_getNextID() {
        var maxID = 0;
        var listOfWidgets;
        for (var item in listOfWidgets) {
            if (listOfWidgets.hasOwnProperty(item)) {
                maxID = Math.max(maxID, listOfWidgets[item].id);
            }
        }
        for (var directions = 0; directions <= 1; directions++) {
            for (var item in _InOutWidgets[directions]) {
                if (_InOutWidgets[directions].hasOwnProperty(item)) {
                    listOfWidgets = _InOutWidgets[directions];
                    for (var item2 in listOfWidgets) {
                        if (listOfWidgets.hasOwnProperty(item2)) {
                            maxID = Math.max(maxID, listOfWidgets[item2].id);
                        }
                    }
                }
            }
        }


        return maxID + 1;
    }

    var _popupInOut_findFirstUnpinned = function _popupInOut_findFirstUnpinned(listOfWidgets) {
        /// <summary>find the first unpinned popup / widget in the list given</summary>
        for (var item in listOfWidgets) {
            if (listOfWidgets.hasOwnProperty(item)) {
                if (!listOfWidgets[item].pinned) {
                    return listOfWidgets[item];
                }
            }
        }
        return undefined;
    }

    var _popupInOut_findByID = function _popupInOut_findByID(idToFind) {
        /// <summary>find the widget with this ID, no matter what direction</summary>
        var maxID = 0;
        for (var directions = 0; directions <= 1; directions++) {
            for (var item in _InOutWidgets[directions]) {
                if (_InOutWidgets[directions].hasOwnProperty(item)) {
                    if (_InOutWidgets[directions][item].id == idToFind)
                        return _InOutWidgets[directions][item];
                    }
            }
        }
        return undefined;
    }

    var _popupInOut = function (direction, toFromFilterObj) {
        /// <summary></summary>
        /// <param name="toFromFilterObj" type="Object">OPTIONAL! </param>
        /// <param name="direction" type="String">OPTIONAL! "in" | "out". "in" is default</param>
        var directionID;
        var nextID;
        var widgetCacheObj;
        var template;
        var container;
        
        template = BDA.Templates.getRawJQObj("InOutTroopsPopup2_d2", ROE.realmID);
        directionID = direction === "in" ? ROE.Troops.InOut2.Enum.Directions.incoming : ROE.Troops.InOut2.Enum.Directions.outgoing;

        /*              
        - if (there a popup, for this direction, that is not pinned) 
          - reuse this popup - set the proper filter and show it. 
        - else (no popup at all, or it is pinned) 
          - create a new popup, and show it 
        */
        widgetCacheObj = _popupInOut_findFirstUnpinned(_InOutWidgets[directionID]);
        if (widgetCacheObj != undefined) {
            /*              
            - if (there a popup, for this direction, that is not pinned) 
              - reuse this popup - set the proper filter and show it. 
            */

            $('.' + widgetCacheObj.cssclass).dialog('open');
            widgetCacheObj.widget.ToFromFilter(toFromFilterObj);

        } else {
            /*              
            - else (no popup at all, or it is pinned) 
              - create a new popup, and show it 
            */
            nextID = _popupInOut_getNextID(); // get next available ID
            widgetCacheObj = { id: nextID, pinned: false };
            widgetCacheObj.cssclass = 'InOutPopup_%directionID%_%nextID%'.format({ nextID: nextID, directionID: directionID });
            widgetCacheObj.widget = ROE.Troops.InOutWidget.init(directionID, template.find('.incomingContainer'), undefined, template.find('.incoming.reload'), widgetCacheObj.id);
            widgetCacheObj.widget.ToFromFilter(toFromFilterObj);
            _InOutWidgets[directionID][widgetCacheObj.id] = widgetCacheObj;

            container = $("<div class='%cssclass%' />".format(widgetCacheObj));
            container.addClass('popupDialogs inOutDialogs').append(template);
                        
            $('document').append(container);
            container.dialog({
                autoOpen: false,
                title: ROE.Troops.InOut2.CONST.directionNameByDirection[directionID],
                position: { at: "left top", of: window },
                width: 760,
                height: 450,
                open: function () {
                    _commonDialogOpenFunction($(this));
                },
                create: function () {
                    _addDialogStylingElements($(this));
                },
                close: function () {
                    _commonDialogCloseFunction($(this));
                }

            });

            $('<div>').addClass('pinButton').click(function handlePinClick() {
                ROE.UI.Sounds.click();
                ROE.Frame.popupInOut_togglePin(widgetCacheObj.id, $(this));
            }).appendTo(container.parent().find('.ui-dialog-titlebar'));

            container.dialog('open');
            //ROE.Troops.InOutSummary.init(direction == 'in' ? 0 : 1, container.find('.filter'))
        }
       
        return false;
    };

    var _popupInOut_filterSpecificWidget_toVillage = function _popupInOut_filterSpecificWidget_toVillage(widgetID, villageID, villageName, villageXCord, villageYCord, toOrFrom) {
        ///<param name="toOrFrom">expected either 'from' or 'to'</param>
        var widget = _popupInOut_findByID(widgetID);
        var village;
        var toFromFilter;

        if (widget) {
            village = new ROE.Class.Village(villageID, villageName, villageXCord, villageYCord);
            if (toOrFrom === 'to') {
                toFromFilter = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, village); // to this village
            } else {
                toFromFilter = new ROE.Troops.InOutWidget.toFromFilter(village); // from this village
            }
            widget.widget.ToFromFilter(toFromFilter);
        }

    }

    //var _togleIncoming = function (incomingAtt) {
    //    //if (incomingAtt.NumAttacks > 0) {
    //    //    $(".ui-buttonpanel-main .inOutTroops").parent().addClass('incoming');
    //    //    $(".ui-buttonpanel-main .countdown").text(incomingAtt.FirstAttackArrivalIn.Days * 24 + incomingAtt.FirstAttackArrivalIn.Hours +
    //    //        ':' + padDigits(incomingAtt.FirstAttackArrivalIn.Minutes) +
    //    //        ':' + padDigits(incomingAtt.FirstAttackArrivalIn.Seconds));
    //    //    initTimers(); // this will initiate any "countdown" timers - is this a good place for this ? why reload ALL timersm, why not just this one ??
    //    //} else {
    //    //    $(".ui-buttonpanel-main .inOutTroops").parent().removeClass('incoming');
    //    //    $(".ui-buttonpanel-main .countdown").text('');
    //    //}

    //};

    //var _incomingAttackCountdownAtZero = function () {
    //    //_InOutgoingChanged(ROE.Troops.InOut2.Directions.incoming);
    //    //$(".ui-buttonpanel-main .inOutTroops").parent().removeClass('incoming');
    //    //$(".ui-buttonpanel-main .countdown").text('');
    //};

    var _enableView = function (enable) {
    };   

    //popup box template (boxtitle, boxwidth, box vertical pos (YYpx, top/center/bottom), bgcolor, content, show/hide closing X)
    function _popupInfo(title, width, Ypos, bgcolor, boxcontent, noX) {


        $("#popupInfo").addClass("fader");

        var boxes = $(".popupInfo_box").length;
        
        var boxtemplate = $("#popupInfoTemp > DIV").attr("id", "PopupInfoBox_" + boxes);

        boxtemplate.clone().appendTo("#popupInfo");
        
        if (noX === true) { $("#PopupInfoBox_" + boxes + " DIV.popupInfo_header").hide(); }
        else { $("#PopupInfoBox_" + boxes + " DIV.popupInfo_header").show(); }

        $("#PopupInfoBox_" + boxes + " DIV.popupInfo_header >SPAN").html(title);
        $("#PopupInfoBox_" + boxes + " DIV.popupInfo_content").html(boxcontent);
        $("#PopupInfoBox_" + boxes).css({ "width": width }).addClass("popupOpenBox");
        if (typeof bgcolor !== "undefined") {
            $("#PopupInfoBox_" + boxes).css({ "background": bgcolor });
        }
        
        var BoxHeight = $("#PopupInfoBox_" + boxes).outerHeight();
        var DocHeight = $(window).height();
        
        var BoxY;
        switch (Ypos) {
            case "center":
                BoxY = (DocHeight / 2) - (BoxHeight / 2);
                $("#PopupInfoBox_" + boxes).css("margin-top", BoxY);
                break;
            case "top":
                $("#PopupInfoBox_" + boxes).css("top", "10px");
                break;
            case "bottom":
                BoxY = (DocHeight) - (BoxHeight) - 5; //extra 5 for the border
                $("#PopupInfoBox_" + boxes).css("margin-top", BoxY);
                break;
            default: $("#PopupInfoBox_" + boxes).css("top", Ypos);
        }

        //empty template container
        $("#popupInfoTemp .popupInfo_content").html("");
        $("#popupInfoTemp .popupInfo_header SPAN").html("");
        $("#popupInfoTemp .popupInfo_box").removeAttr('id');


        $("#PopupInfoBox_" + boxes + " DIV.popupInfo_close").click(function () {            
            
            //do only for incoming/outcoming troops popup
            if ($('.directionBox >DIV').hasClass('directionToggle')) {

                var direction = $('.directionBox .directionToggle').attr("data-dir");
                var olddirection = BDA.Database.LocalGet('TroopDirection');
                if (direction != olddirection) {
                    BDA.Database.LocalSet('TroopDirection', direction);
                    ROE.Landmark.refill(true);
                }                
            }

            $("#PopupInfoBox_" + boxes).addClass("popupInfo_boxClose");

            setTimeout(function () {

                $("#PopupInfoBox_" + boxes).remove();
                var openboxes = $(".popupOpenBox").length;

                //hide main container if no open box left
                if (openboxes === 0) { $("#popupInfo").removeClass("fader"); }
                
            }, 200);

         });   
    }

    function _popupInfoClose(e) {


        $(e).parents(".popupOpenBox").addClass("popupInfo_boxClose");
        setTimeout(function () {

            $(e).parents(".popupOpenBox").remove();

            var openboxes = $(".popupOpenBox").length;
            //hide main container if no open box left
            if (openboxes === 0) { $("#popupInfo").removeClass("fader"); }

        }, 300);


    }

    function _popupBattleSim(reportId, as, who) {
        if (reportId) {            
            _showIframeOpenDialog('#battleSimDialog', 'battlesimulator.aspx?recID=' + reportId + '&as=' + as + '&who=' + who);
        } else {
            _showIframeOpenDialog('#battleSimDialog', 'battlesimulator.aspx');
        }
    }
    function _popupStewardship() {
        _showIframeOpenDialog('#kingdomStewardship', 'accountstewards.aspx');
    }
    function _popupTou() {
        _showIframeOpenDialog('#touDialog', 'tou.aspx');
    }
    function _popupContactSupport() {
        _showIframeOpenDialog('#supportDialog', 'ContactSupport.aspx');
    }

    function _errorBar(infotext) {
        _showBar('error', infotext)
    }
    function _infoBar(infotext) {
        _showBar('info', infotext)
    }

    function _showBar(type, infotext) {
        if (infotext == undefined) {
            BDA.Console.log("infotext passed to _showBar is undefined ");
            return;
        }

        //close any other open infobar
        $(".infobarDiv").remove();

        $("body").append("<DIV class='infobarDiv sfx2 %type% ' ><span>%infotext%</span><div class=infobarimg ><img src='https://static.realmofempires.com/images/icons/M_X.png' ></div></DIV>".format({ infotext: infotext, type: type }));
        
        clearTimeout(infoTimeout);

        infoTimeout = setTimeout(function () {
            _infoBarClose();
        }, 4300);

        $(".infobarDiv").click(function () { _infoBarClose(); });
    }

    function _infoBarClose() {
        $(".infobarDiv").fadeOut("slow", function () { $(".infobarDiv").remove(); });
    }

    function _busy(overrideText, timeOutForTooLongMessage, container) {
        ROE.Frame.base_busy(overrideText, timeOutForTooLongMessage, container);
    }

    function _free(container) {
        ROE.Frame.base_free(container);
        }

    function _isBusy() {
        return ROE.Frame.base_isBusy();
        }

    function _reloadTheWindow() {
        window.location.reload();
    }

   
    var _purchase = function (productID) {
        ////
        ////This is not used in D2 ATM
        ////
        //BDA.Console.verbose('PFP', 'in ROE.Frame.purchase' + productID);
        //ROE.Frame.busy("Working. Please wait for a confirmation popup", 7000);

        //// this is like this on purpose ... avoiding eval()
        //ROE.Device.purchase(productID, ROE.Frame.reloadFrame, 'ROE.Frame.reloadFrame()');
        
        //setTimeout(function () { 
        //    ROE.Frame.free();
        //}, 5000);
    };

    var _rateApp = function () {
        ROE.Device.rateApp();
    };

    var tim; 

    var _swipe = function (direction) {
        ////
        ////This is not used in D2 ATM
        ////


        ///// <summary></summary>
        ///// <param name="direction" type="string">left | right</param>

        //switch (_view) {
        //    case Enum.Views.vov:                
        //        if (ROE.Frame.IsPopupOpened()) {
        //            if ($(ROE.Frame.CONSTS.PopupIDs.building).length > 0) { //building popup swipe handling                       
        //                ROE.UI.Sounds.clickActionSwipe();
        //                _showNextVOV(direction === 'left' ? 'next' : 'prev',
        //                    function showNextCallback() { ROE.Building.reInitContentWithDifferentVillage(); },
        //                    'Switching to');
        //            } else if ($(ROE.Frame.CONSTS.PopupIDs.quickbuild).length > 0) { //quickbuild popup swipe handling
        //                ROE.UI.Sounds.clickActionSwipe();
        //                _showNextVOV(direction === 'left' ? 'next' : 'prev',
        //                    function showNextCallback() { ROE.QuickBuild.reInitContentWithDifferentVillage(); },
        //                    'Switching to');
        //            } else if ($(ROE.Frame.CONSTS.PopupIDs.quickrecruit).length > 0) { //quickrecruit popup swipe handling
        //                ROE.UI.Sounds.clickActionSwipe();
        //                _showNextVOV(direction === 'left' ? 'next' : 'prev',
        //                    function showNextCallback() { ROE.QuickRecruit.reInitContentWithDifferentVillage(); },
        //                    'Switching to');
        //            } else { //any other popup-open swipe handling
        //                //do nothing
        //            }
        //        }
        //        else {
        //            ROE.UI.Sounds.clickActionSwipe();
        //            _showNextVOV(direction === 'left' ? 'next' : 'prev');
        //        }
        //        break;
        //    case Enum.Views.map:
        //        break;
        //    case Enum.Views.research:
        //        break;
        //}

    };

    var _swipeLeft = function () {
        //
        //This is not used in D2 ATM
        //
    //    if (!_isBusy()) {
    //    _swipe('left');
    //}
    };

    var _swipeRight = function () {
        //
        //This is not used in D2 ATM
        //
    //    if (!_isBusy()) {
    //        _swipe('right');
    //}
    };

    var _swipeUp = function () {
    //    //
    //    //This is not used in D2 ATM
    //    //
    //    if (!_isBusy()) {
    //    switch (_view) {
    //        case Enum.Views.map:
    //            break;
    //        case Enum.Views.research:
    //            break;
    //    }
    //}
    };

    var _swipeDown = function () {
    //    //
    //    //This is not used in D2 ATM
    //    //
    //    if (!_isBusy()) {
    //    switch (_view) {
    //        case Enum.Views.map:
    //            break;
    //        case Enum.Views.research:
    //            break;
    //    }
    //}
    };


    var _refresh = function () { //DEAD CODE MOST LIKELY
        if (!ROE.Frame.IsPopupOpened()) {
            _reloadFrame();
        }
    };

    var _pinchZoom = function (action) {
        // zoom is +
        // pinch is -;
        // alert('pinchZoom:' + action);
    };

    var _backButton = function () {
        //alert('backbutton');
    };

    var _IsPopupOpened = function () {
        /// <summary>in D2, this is always false</summary>       
        return false;
    };
    

    /*The following two functions address a Android 4.4 keyoard-hiding-inputs issue */
    function _inputFocused() { }

    function _inputBlured() { }


    function _someInOutgoingCountdownFinished() {
        //_updateInOutgoingForSelectedVillage(ROE.Troops.InOut2.Enum.Directions.incoming);
        _InOutgoingChanged(ROE.Troops.InOut2.Enum.Directions.incoming);

    }

    function _updateInOutgoingForSelectedVillage(direction) {
        var incoming;
        var outgoing;
        var incomingDOM = $(CONST.Selector.currentVillage.incomingAlert);
        var outgoingDOM = $(CONST.Selector.currentVillage.outgoingAlert);

        if (!direction || direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
            incoming = ROE.Troops.InOut2.getIncomingWarning(ROE.SVID);
            if (incoming.count > 0) {
                incomingDOM.addClass('active').html('%count% incoming in <span class=countdown2 data-finishesOn="%time%" refresh=false>%timeleft.h%:%timeleft.m%:%timeleft.s%</span>'.format({ count: incoming.count, timeleft: BDA.Utils.timeLeft(incoming.earliestLandingTime), time: incoming.earliestLandingTime }));
                initTimers();
            } else {
                incomingDOM.removeClass('active').html('');
            }
        }

        if (!direction || direction == ROE.Troops.InOut2.Enum.Directions.outgoing) {
            outgoing = ROE.Troops.InOut2.getOutgoingWarning(ROE.SVID);
            if (outgoing.count > 0) {
                outgoingDOM.addClass('active').html('%count% outgoing'.format(outgoing));
                initTimers();
            } else {
                outgoingDOM.removeClass('active').html('');
            }
        }

    }

    
    function _InOutgoingChanged(direction) {
        /// <summary>handle the event that tells that that list of incoming or outgoing has changed</summary>       

        var incomingToEmpire;
        var outgoingToEmpire;
        var incomingDOM = $(CONST.Selector.incomingAlert_Empire);
        var outgoingDOM = $(CONST.Selector.outgoingAlert_Empire);

        if (direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
            incomingToEmpire = ROE.Troops.InOut2.getIncomingWarning();
            if (incomingToEmpire.count > 0) {
                incomingDOM.html('%count% in <span data-refreshCallDelay="1" class=countdown2 data-finishesOn="%time%" data-refreshCall="ROE.Frame.someInOutgoingCountdownFinished();">%timeleft.h%:%timeleft.m%:%timeleft.s%</span>'.format({ count: incomingToEmpire.count, timeleft: BDA.Utils.timeLeft(incomingToEmpire.earliestLandingTime), time: incomingToEmpire.earliestLandingTime }));
                incomingDOM.addClass('active');
                incomingDOM.parent().addClass('incomingActive');
                initTimers();
            } else {
                incomingDOM.html('');
                incomingDOM.removeClass('active');
                incomingDOM.parent().removeClass('incomingActive');
            }
        } else if (direction == ROE.Troops.InOut2.Enum.Directions.outgoing) {
            var outgoingToEmpire = ROE.Troops.InOut2.getOutgoingWarning();
            if (outgoingToEmpire.count > 0) {
                outgoingDOM.html('%count% outgoing'.format(outgoingToEmpire));
                outgoingDOM.addClass('active');
                initTimers();
            } else {
                outgoingDOM.html('');
                outgoingDOM.removeClass('active');
            }
        }

        _updateInOutgoingForSelectedVillage(direction);
    }

    var _isInIframe = function () {
        try {
            return window.self !== window.top;
        } catch (e) {
            return true;
        }
    }

    var _viewFullscreen = function () {       
        if (_isInIframe()) {
            $.cookie('fullscreen', "1");
            top.location = self.location;
        }
    }
    var _viewInFrame = function (link) {
        if (!_isInIframe()) {
            $.cookie('fullscreen', "0");
            top.location = $(link).attr("data-url");
        }
    }
   
    function _init() { //NOTE: this function is intended to be ideantical in D2 and M 
        ///<summary>initilizes the UI for the first time</summary>
        /*
        Init process is as follows:

            1. reload the header; this, once returned, will give me player info
                2. [_init_getVillage] then try to get the currently selected village 
                    3. [_init_gotVillage] if we got the village, then just move on and populate the UI
                      (3b) if the village we got is null, that could mean that we no longer own the ROE.SVID village
                      in which case, we get the list of villages
                        4. [_init_takeFirstVillage] here we take the first village from the list and go with this one. 

            note that 3b shoudl not happen as the ROE.SVID coming from the server should be correct however it is still possible that the village happens
            to get lost just after the server released the page
        */

        ROE.Frame.busy("Starting the windmill...");

        ROE.Frame.reloadFrame(
            _init_getVillage
            );
    }
    function _init_getVillage() { //NOTE: this function is intended to be ideantical in D2 and M 
        // here we try to get the currently selected village; 
        //  by doing : ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists
        //  and by the fact that this is happening on initial load (so village list cannot hace extended data populated) 
        //  we can ensure that there will be an api call made to try to get this village. 
        //  So, if this village is no longer mine, the call with return with a undefined village. 
        //  
        ROE.Villages.getVillage(ROE.SVID, _init_gotVillage, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists)
    }
    function _init_gotVillage(village) { //NOTE: this function is intended to be ideantical in D2 and M 
        if (village == undefined) {
            //
            // village is undefined. 
            // 
            ROE.Villages.getVillages(_init_takeFirstVillage)
        } else {
            ROE.Frame.setCurrentlySelectedVillage(village.id, village.x, village.y, village.name)
            _init_allDataReady(village);
        }        
    }
    function _init_takeFirstVillage(villageList) { //NOTE: this function is intended to be ideantical in D2 and M 
        //
        // get the currently selected village
        //
        if (villageList.length < 1) {
            // no villages, shoudl take player back to 'register' screen, so we just kick them out to chooserealm
            window.location = "chooseRealm.aspx";
        } else {
            _init_gotVillage(villageList[0]);
        }
    }
    function _init_allDataReady(village) {
        var toolTipTimer = null;
        var windowResizeTimer = null;

        // setup some event hanlding
        $(CONST.Selector.mainButtonPanel.mail).click(_popupMail);
        $(CONST.Selector.mainButtonPanel.reports).click(function () { ROE.Frame.popupReports() });
        $(CONST.Selector.mainButtonPanel.clan).click(function () { _showIframeOpenDialog('#clanDialog', 'clanOverview.aspx'); });
        $(CONST.Selector.mainButtonPanel.inOutTroops).click(function handleInOutButtonClick() { ROE.Frame.popupInOut('in'); });
        $(CONST.Selector.mainButtonPanel.settings).click(function () { ROE.Frame.settingsPopup(1); });
        $(CONST.Selector.mainButtonPanel.rankings).click(function () { ROE.UI.Sounds.click(); ROE.Frame.popupStats(); });
        $(CONST.Selector.incomingAlert_Empire).click(function handleInOutButtonClick() { ROE.Frame.popupInOut('in'); });
        $(CONST.Selector.outgoingAlert_Empire).click(function handleInOutButtonClick() { ROE.Frame.popupInOut('out'); });
        $(CONST.Selector.uiButtons.linkQuests).click(function () { ROE.UI.Sounds.click(); ROE.Frame.popupQuests(); });
        $(CONST.Selector.uiButtons.socialInvite).click(function () { ROE.UI.Sounds.click(); _showIframeOpenDialog('#socialInviteDialog', 'Invite.aspx'); });
        $(CONST.Selector.sidePane.battleSim).click(function () { _popupBattleSim(); });


        //mouseover vovFrame, dialogs, or UI panel will hide mapGui panels
        $("#vovFrame, .ui-panel-main, #roeOptions, #mainCommPanel, #mainChatFrame").mouseover(function () { $('.mapGuiPanel').stop().hide(); });

        $('#vovFrame .toggleSize').click(function () { ROE.UI.Sounds.click(); $('#vovFrame').toggleClass('minimized'); });
        $("#vovFrame .popVillageList").click(ROE.Frame.showVillList);
        $("#vovFrame .navToPrev").click(function () { ROE.UI.Sounds.click(); ROE.Frame.visitPrevVillage() });
        $("#vovFrame .navToNext").click(function () { ROE.UI.Sounds.click(); ROE.Frame.visitNextVillage() });
        $("#vovFrame .navToPrev, #vovFrame .navToNext").toggleClass('grayout', ROE.playersNumVillages < 2);
        $("#vovFrame .centerVillage").click(function () { ROE.UI.Sounds.click(); ROE.Frame.centerThisVillage(ROE.CSV.x, ROE.CSV.y); });
        $("#vovFrame .loyaltyWrapper").click(_boostYay);

        $("#viewSwitch_res").click(function () { ROE.UI.Sounds.click(); ROE.Research.showResearchPopup(); });
        $(".dailyReward").click(function () { ROE.UI.Sounds.click(); ROE.DailyReward.showPopup(); });
        $("#sleepMode").click(function handleSleepModeClick() { ROE.UI.Sounds.click(); ROE.SleepMode.showPopup('d2'); });
        $(".pfStatus").click(_showServantsPopup);
        $("#linkItems").click(function () { ROE.UI.Sounds.click(); ROE.Items2.showPopup(); });
        $("#launchRaidsPopup").click(function () { ROE.UI.Sounds.click(); ROE.Raids.showPopup(); });

        $("#launchTargetsListSupport").click(function () { ROE.UI.Sounds.click(); ROE.Targets.launchTargetsList(1); });
        $("#launchTargetsListAttack").click(function () { ROE.UI.Sounds.click(); ROE.Targets.launchTargetsList(2); });

        if (ROE.Player.numberOfVillages > 1) {
            $('#linkMassActions').parent().show().click(function () {
                ROE.MassActions.showPopup();
            });
        } else {
            $('#linkMassActions').parent().hide();
        }
        

        //D2 massfeatures opening in dialogIframe
        $("#massFeaturesDialog .massFeature").click(function () {
            _popGeneric($(this).attr('data-title'), "", 600, 500);
            _showIframeOpenDialog('#genericDialog', $(this).attr('data-href'));
        });
        
        BDA.Broadcast.subscribe($("body"), "InOutgoingDataChanged", _InOutgoingChanged);

        //setup tooltip interaction
        $("body").delegate(".helpTooltip", "mouseover", function () {
            var element = $(this);
            toolTipTimer = setTimeout(function () { _showTooltip(element); }, 800);
        });
        $("body").delegate(".helpTooltip", "mouseout", function () {
            $('.toolTipBox').stop().fadeOut(200, function () { $(this).remove(); });
            window.clearTimeout(toolTipTimer);
        });

        $(".vov-inout-alert.inc").click(function () {
            ROE.Villages.getVillage(ROE.SVID,
                function vovInAlertClick(village) {
                    var filter = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, new ROE.Class.Village(village.id, village.name, village.x, village.y));
                    ROE.Frame.popupInOut('in', filter);
                })
        });

        $(".vov-inout-alert.out").click(function () {
            ROE.Villages.getVillage(ROE.SVID, function vovInAlertClick(village) {
                var filter = new ROE.Troops.InOutWidget.toFromFilter(new ROE.Class.Village(village.id, village.name, village.x, village.y));
                ROE.Frame.popupInOut('out', filter);
            });
        });

        $(window).bind('resize', function () {
            windowResizeTimer && clearTimeout(windowResizeTimer);
            windowResizeTimer = setTimeout(_onWindowResize, 100);
        });

        _initMusicButton();
        _setupDialogs();
        _populateVOV(village.VOV);
        _load_map();
        _initAudio();         
        _setupHotkeys();
        ROE.Clock.init();
        _free();
        _reloadVillageInfo(village);
        _onWindowResize();

        BDA.Broadcast.subscribe($('html'), "CurrentlySelectedVillageChanged", _handleCurrentlySelectedVillageChangedEvent);
        BDA.Broadcast.subscribe($('html'), "VillageExtendedInfoUpdated", _handleVillageExtendedInfoUpdatedOrPopulated);

        setTimeout(_timedRefreshFrame, ROE.CONST.headerRefreshRate);

        if ($('#playOnMinfo').length) {
            setInterval(function () {
                var mInfoIcon = $('#playOnMinfo .innerIcon').fadeOut(300, function () {
                    var nextState = parseInt(mInfoIcon.attr('data-iconState')) + 1;
                    if (nextState > 3) { nextState = 1; }
                    mInfoIcon.attr('data-iconState', nextState).fadeIn(300);
                });
            }, 2500);
        }
 
        ROE.Player.refresh();

        ROE.Tutorial.startIfRequested(1, 'mainDesktop');
        


        //start advisor
        ROE.Advisor.init();


        //Google Chrome check, if not chrome suggest chrome
        if (!((navigator.userAgent.toLowerCase().indexOf('chrome') > -1) && (navigator.vendor.toLowerCase().indexOf("google") > -1))) {
            //if hasnt been dont-show-anymored by user
            if (!window.localStorage['dontshowgetchrome']) {
                var getChromePanel = $('<div class="getChrome">' +
                    '<div>Non Chrome browser detected.</div>' +
                    '<div>While we support as many browsers as possible, RoE runs best on Chrome.</div>' +
                    '<div><a href="https://www.google.com/chrome/browser/desktop/" target="_blank">Give it a try here.</a> It may greatly enhance your experience.</div>' +
                    '<div class="noshow">Dont show this any more <input type="checkbox" class="noMoreGetChrome" /></div>' +
                '</div>');
                var closeButton = $('<div class="close">X</div>').click(function () {
                    if ($('.noMoreGetChrome').is(':checked')) {
                        window.localStorage['dontshowgetchrome'] = true;
                    }
                    getChromePanel.remove();
                });
                getChromePanel.append(closeButton);
                $('#PromoWrapper').append(getChromePanel);
            }
        }

        //NEW CHAT
        if (!ROE.Player.isSteward) {
            $('body').append('<div id="chatBarContainer" class="gameChatBarContainer"></div>');
            ROE.Chat2.startHub(ROE.realmID, ROE.playerID, $('#chatBarContainer'));
        }

        //Try to let this be the last thing done, to help it get the best offset.
        //gets the server to local time difference and populates ROE.timeOffset with it.
        ROE.Utils.getServerTimeOffset();


      // ROE.Frame.showVillList();
    }

    function _onWindowResize() {
        //var w = $(window).width();
        var h = $(window).height();
        $('body').toggleClass('slimView', h < 750);
    }

    function _initMusicButton() {
        if (ROE.Device.isMusicPlaying() == true) {
            $('#freeFloatIconsArea .setMusic').show();
            $('#freeFloatIconsArea .setMusic').click(function () { ROE.Frame.playMusic(false); $('#freeFloatIconsArea .setMusic').fadeOut(); });        
        }
    }

    function _showTooltip(element) {
        //ROE.Tooltips data comes from roe.tooltips.js.aspx.resx
        var text = ROE.Tooltips[element.attr('data-toolTipID')];      
        var toolTipBox = $('<div>').addClass('toolTipBox').html(text).hide().appendTo('body').position({
            my: "center top",
            at: "center bottom+5",
            of: element,
            collision: "flipfit"
        }).fadeIn();
    }

    function _handleCurrentlySelectedVillageChangedEvent() {
        BDA.Console.verbose('ROE.Frame', 'CSV changed to :%vid%'.format({ vid: ROE.SVID }));
        ROE.Villages.getVillage(ROE.SVID, _showThisVOV2, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);       
    }

    function _handleVillageExtendedInfoUpdatedOrPopulated(village) {
        if (village.id == ROE.SVID) {
            if (
                village.changes.coins
                || village.changes.popRem
                || village.changes.points
                || village.changes.name
                || village.changes.type
                || village.changes.yey
                || village.changes.areTranspAvail
                ) {
                BDA.Console.verbose('VillChanges', "VID:%vid% [in frame] - has changes, updating UI".format({ vid: village.id }));
                _reloadVillageInfo(village);
            } else {
                BDA.Console.verbose('VillChanges', "VID:%vid% [in frame] - no changes".format({ vid: village.id }));
            }
        }
    }

    function _getClanLeaderNamesList(clanId, callback) {
        ROE.Api.call_getClanMemberList(clanId, function populateTheClanMessage(r) {
            var clanLeaders = {};
            $.each(r.Roles, function( index, value ) {
                if(value.RoleID == 0 || value.RoleID == 3)
                    clanLeaders[value.PlayerID] = true;
            });
            
            var leadersList = "";
            $.each(r.Members, function (index, value) {
                if (clanLeaders[value.PlayerID])
                    leadersList += value.Name + ",";
            });

            callback(leadersList);          
       });
    }

    function _launchClanInviteRequestMessage(clanId) {
       
        var subject = "A Request to Join The Clan";
        var msg = "Hello Clan Leaders,\n\n"
        msg += "I am looking to join your clan. Please send me an invite so that I may pledge my sword to you.\n\n"
        msg += "Sincerely,\n"+ROE.Player.name
        // TODO: Consider adding a link to the ClanInvitations section to make easier for the person
        // receiving the email. Maybe even pre-populate the invite input box when you click on the link.
        
        _getClanLeaderNamesList(clanId, function onGetClanLeadersSuccess(leadersList) {
            ROE.Frame.popupMailPrefilled(leadersList, subject, msg)
        });
    }

    function _initAudio() {
        // Check to see if audio toggles have been set    
        if (!localStorage.isSfxOn) {
            localStorage.setItem('isSfxOn', "true");
        }

        if (!localStorage.isMusicPlaying) {
            localStorage.setItem('isMusicPlaying', "true");
            ROE.UI.Sounds.playHTML5Music();
        } else {
            if (localStorage.getItem('isMusicPlaying') == "true")
                ROE.UI.Sounds.playHTML5Music();
        }
    }





    function _spawnFlyRewardIcon(data) {

        var claimDiv = $('<div class="questRewardFly">');

        $('body').append(claimDiv);

        claimDiv.css({
            left: $(window).width() / 2 - claimDiv.width() /2,
            top: $(window).height() / 2 - claimDiv.height() /2
        });

        var rewardsIcon = $('#linkItems');

        if (rewardsIcon.length) {

            var rewardsIconPos = rewardsIcon.offset();

            claimDiv.animate({ top: '-=30px' }, 500, "easeOutSine")
                .animate({ top: rewardsIconPos.top, left: rewardsIconPos.left, width:'44px', height:'44px' }, 1200, "easeInSine", function () {
                    $(this).remove();
                });

        } else {
            claimDiv.animate({ top: '-=30px' }, 1500, "easeOutSine").animate({ top: '-=5px', opacity: 0 }, 300, "easeOutSine", function () {
                $(this).remove();
            });
        }
    }

    //When a completed quest "Get Reward" in Quest.aspx is clicked
    //its called from ScriptManager.RegisterStartupScript in quests.aspx.cs(standard) and quests.ascx.cs(mastery)
    function _questRewardAcceptCB(data) {
        //console.log('_questRewardAcceptCB data:', data);

        //send a big reward icon flying towards rewards icon
        ROE.Frame.spawnFlyRewardIcon();

        //update rewards (gets latest)
        ROE.Items2.update();

        //decrement quest completed count 
        ROE.Player.questsCompl = data.completedQuests;

        //update player credits and UI
        _refreshPFHeader(data.credits);

        //update quest icon UI
        _handleQuestIcon();
        
        //call for latest count, to make sure all is synched.
        //_refreshQuestCount();
    }

    var _refreshQuestTimeout; //used for throttling calls
    function _refreshQuestCount() {
        
        clearTimeout(_refreshQuestTimeout);

        _refreshQuestTimeout = setTimeout(function () {
            //console.log('REFRESH-CALL PRE COUNT:', ROE.Player.questsCompl);
            ROE.Api.call_quest_getcompletedcount(function getcompletedcountCB(data) {
                //console.log('REFRESH-CALL POST COUNT:', data);
                ROE.Player.questsCompl = data.questsCompletedCount;
                _handleQuestIcon();
            });
        }, 800);

    }

    function _handleQuestIcon(){

        if (ROE.Player.questsCompl > 0) {
            // if new completed quest, and quest window opened, refresh that window
            if (ROE.Player.questsCompl > $('#linkQuests').attr('data-completedQuestCount')) {
                if ($('#questsDialog .dialogIframe').length) {
                    //$('#questsDialog iframe').attr("src", $('#questsDialog iframe').attr("src"));
                    var questsPage = $('.dialogIframe').contents();
                    $('#pageReloader', questsPage).show();
                }
            }
            
            ROE.Frame.iconNeedsAttention($('#linkQuests').addClass('hasQuests').attr('data-completedQuestCount', ROE.Player.questsCompl), true);
            $('#linkQuests_completedCount').text(ROE.Player.questsCompl);
        }else{
            ROE.Frame.iconNeedsAttention($('#linkQuests').removeClass('hasQuests').attr('data-completedQuestCount', ROE.Player.questsCompl), false);
            $('#linkQuests_completedCount').text('');
        }

    }


    //we can add more hotkeys easily!
    function _setupHotkeys() {

        //TURNED THIS OFF FOR NOW
        //it is just causing too much headache, needs to become smarter and not activate when certain elements are in focus

        /*
        $(document).keydown(function (e) {
            switch (e.which) {
                case $.ui.keyCode.LEFT:
                    if(e.ctrlKey){
                        _visitPrevVillage();
                    }                   
                    break;

                case $.ui.keyCode.UP:
                    break;

                case $.ui.keyCode.RIGHT:_commonDialogCloseFunction($(this));
                    if (e.ctrlKey) {
                        _visitNextVillage();
                    }
                    break;

                case $.ui.keyCode.DOWN:
                    break;

                default: return;
            }
            
            //this may cause problems, for example preventing standard hotkeys from working, but could also be a good thing, must test
            //e.preventDefault(); 
        });*/
    }

    var _setupDialogs = function _setupDialogs() {
        var _UIDisplayMode = ROE.LocalServerStorage.get('UIDisplayMode') || 'Standard';

        $('#genericDialog').dialog({
            autoOpen: false,
            modal: true,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#rxInfoDialog').dialog({
            autoOpen: false,
            title: 'Tournament Realm Info',
            position: { at: "center center", of: window },
            width: 737,
            height: 550,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#playOnMDialog').dialog({
            autoOpen: false,
            title: 'Realm of Empires Mobile App',
            position: { at: "center center", of: window },
            width: 700,
            height: 500,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#inviteFriendsRewardDialog').dialog({
            autoOpen: false,
            title: 'Invite-a-Friend Reward',
            position: { at: "center center", of: window },
            width: 700,
            height: 500,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#buildDialog').dialog({
            autoOpen: false,
            title: 'Build Guide',
            position: { my: "center", at: "center", of: window },
            width: 360,
            height: 610,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#buildingDialog').dialog({
            autoOpen: false,
            position: { my: "center", at: "center", of: window },
            width: 360,
            height: 560,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#massFeaturesDialog').dialog({
            autoOpen: false,
            title: 'Mass Features',
            position: { my: "center center", at: "center center", of: window },
            width: 360,
            height: 200,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#quickBuildDialog').dialog({
            autoOpen: false,
            title: 'Quick Build',
            position: { at: "right center", of: window },
            width: _UIDisplayMode == 'Standard' ? 360 : 525,
            height: 555,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#quickRecruitDialog').dialog({
            autoOpen: false,
            title: 'Quick Recruit',
            position: { at: "left center", of: window },
            width: _UIDisplayMode == 'Standard' ? 375 : 432,
            height: 590,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#researchDialog').dialog({
            autoOpen: false,
            title: 'Research',
            position: { at: "center center", of: window },
            width: 860,
            height: 600,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#mapHighlightsDialog').dialog({
            autoOpen: false,
            title: 'Map Highlights',
            position: { at: "center center", of: window },
            width: 360,
            height: 600,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#mapSummaryDialog').dialog({
            autoOpen: false,
            title: 'Map Summary',
            position: { at: "center center", of: window },
            width: 360,
            height: 600,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#reportsDialog').dialog({
            autoOpen: false,
            title: 'Reports',
            position: { at: "center center", of: window },
            width: 665,
            height: 560,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#mailDialog').dialog({
            autoOpen: false,
            title: 'Mail',
            position: { at: "center center", of: window },
            width: 800,
            height: 560,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#playerProfileDialog').dialog({
            autoOpen: false,
            title: 'Player Profile',
            resizable: false,
            position: { at: "center center", of: window },
            width: 360,
            height: 620,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#villageProfileDialog').dialog({
            autoOpen: false,
            title: 'Village Profile',
            resizable: false,
            position: { at: "right center", of: window },
            width: 360,
            height: 620,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#transportSilverDialog').dialog({
            autoOpen: false,
            modal: true,
            title: 'Transport Silver',
            position: { at: "center center", of: window },
            resizable: false,
            width: 500,
            height: 620,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                ROE.Villages.ExtendedInfo_loadLatest(ROE.SVID);
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#clanDialog').dialog({
            autoOpen: false,
            title: 'Clan',
            position: { at: "left center", of: window },
            width: 800,
            height: 620,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#battleSimDialog').dialog({
            autoOpen: false,
            title: 'Battle Simulator',
            position: { at: "center center", of: window },
            width: 800,
            height: 520,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#questsDialog').dialog({
            autoOpen: false,
            title: 'Quests',
            position: { at: "center center", of: window },
            width: 800,
            height: 610,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#rankingsDialog').dialog({
            autoOpen: false,
            title: 'Player Rankings and Statistics',
            position: { at: "center center", of: window },
            width: 840,
            height: 610,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        /*
        $('#avatarDialog').dialog({
            autoOpen: false,
            modal: true,
            title: 'Avatar',
            position: { at: "center center", of: window },
            width: 840,
            height: 610,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        */

        $('#supportLookupDialog').dialog({
            autoOpen: false,
            title: 'Support Lookup',
            position: { at: "center center", of: window },
            width: 766,
            height: 620,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#giftSendDialog').dialog({
            autoOpen: false,
            modal: true,
            title: 'Send Gifts to Friends',
            resizable: false,
            width: 460,
            height: 620,
            position: { at: "center center", of: window },
            open: function () {
                //_openFullScreen($(this));
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#socialInviteDialog').dialog({
            autoOpen: false,
            title: 'Facebook Invite',
            resizable: false,
            draggable: false,
            position: { at: "center center", of: window },
            open: function () {
                _openFullScreen($(this));
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#sleepModeDialog').dialog({
            autoOpen: false,
            resizable: false,
            title: 'Sleep Mode',
            position: { at: "center center", of: window },
            width: 360,
            height: 620,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#CommandTroopsPopup').dialog({
            autoOpen: false,
            title: 'Command',
            position: { at: "left center", of: window },
            width: 510,
            height: 600,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).find('.AttacksPopup').empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#quickCommandDialog').dialog({
            autoOpen: false,
            title: 'Quick Command',
            resizable: true,
            position: { at: "left bottom", of: window },
            width: 680,
            height: 400,
            open: function () {
                _commonDialogOpenFunction($(this));
                //ROE.QuickCommand.paint();
            },
            create: function () {
                _addDialogStylingElements($(this));
                $(this).parent().find('.ui-dialog-titlebar').append($('<div>').addClass('pinButton').click(ROE.QuickCommand.handlePinButtonClick));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });
        $('#kingdomStewardship').dialog({
            autoOpen: false,
            title: 'Kingdom Stewardship',
            position: { at: "center center", of: window },
            width: 900,
            height: 700,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#settingsDialog').dialog({
            autoOpen: false,
            title: 'Settings',
            position: { at: "center center", of: window },
            width: 360,
            height: 630,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#accountDialog').dialog({
            autoOpen: false,
            modal: true,
            title: 'Account Security',
            position: { at: "center center", of: window },
            width: 360,
            height: 385,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#supportDialog').dialog({
            autoOpen: false,
            title: 'Terms of Use',
            position: { at: "center center", of: window },
            width: 900,
            height: 700,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#touDialog').dialog({
            autoOpen: false,
            title: 'Terms of Use',
            position: { at: "center center", of: window },
            width: 900,
            height: 700,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#giftsDialog').dialog({
            autoOpen: false,
            modal: true,
            title: 'The Bazaar and Inventory',
            position: { at: "center center", of: window },
            width: 360,
            height: 620,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        $('#pfDialog').dialog({
            autoOpen: false,
            title: 'Premium Features',
            position: { at: "center center", of: window },
            width: 450,
            height: 670,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            }
        });
        $('#villageListDialog').dialog({
            autoOpen: false,
            title: 'Village List',
            position: { at: "left top", of: window },
            width: 500,
            height: 600,
            open: function () {
                _responsiveDialog($(this));
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
                $(this).parent().append('<a class="feedback fontGoldFrLCsm" ' +
                    'href="https://roebeta.uservoice.com/forums/273298-beta-ui-feedback/category/159789-window-village-list" target="uv">' +
                    'Got ideas how to make village list better?</a>');
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            resize: function () {
                _responsiveDialog($(this));
            }
        });

        $('#incomingSummarDialog').dialog({
            autoOpen: false,
            title: 'Incoming Summary',
            position: { at: "center center", of: window },
            width: 387,
            height: 600,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });

        $('#outgoingSummarDialog').dialog({
            autoOpen: false,
            title: 'Outgoing Summary',
            position: { at: "center center", of: window },
            width: 387,
            height: 600,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });

        $('#items2Dialog').dialog({
            autoOpen: false,
            modal: false,
            title: 'Rewards',
            position: { at: "center center", of: window },
            width: 540,
            height: 465,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#targetsDialog').dialog({
            autoOpen: false,
            modal: false,
            title: 'Targets',
            position: {
                at: "10% center", of: window
            },
            width: 320,
            minWidth: 320,
            height: 465,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        $('#presetsDialog').dialog({
            autoOpen: false,
            modal: false,
            title: 'Combat Presets',
            position: {
                at: "right center", of: window
            },
            width: 600,
            height: 500,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });
        



    }

    function _responsiveDialog(dialog) {
        var id = dialog.attr('id');
        var w = dialog.width();
        switch (id) {
            case "villageListDialog":
                if (w > 600) {
                    dialog.attr('sizeRange', 'l');
                } else if (w > 450) {
                    dialog.attr('sizeRange', 'm');
                } else {
                    dialog.attr('sizeRange', 's');
                }
                break;
            default:
                break;
        }
    }

    //create, and open a disposable dialog
    //it checks if exists or already open, when opening
    //on close, the dialog is destroyed, conserves memory and cleans DOM
    ///
    ///settings.ID - sets dialog ID, default 'Generic'
    ///settings.title - sets dialog title, default ''
    ///settings.content - sets dialog content, default ''
    ///settings.width - sets dialog width, default 350
    ///settings.height - sets dialog height, default auto
    ///settings.modal - sets dialog modal, default true
    ///settings.contentCustomClass - gives dialog's content a custom class

    function _popDisposable(settings) {

        settings.ID = settings.ID || 'genericDialog';
        settings.title = settings.title || '';
        settings.content = settings.content || '<div></div>';
        settings.width = settings.width || 350;
        settings.height = settings.height || 'auto';
        settings.modal = (settings.modal === false ? false : true);
        settings.contentCustomClass = settings.contentCustomClass || '';

        var dialog = $('#'+settings.ID).empty();

        if (dialog.length < 1) {
            dialog = $('<div id="' + settings.ID + '" class="popupDialogs"></div>').appendTo('body');
        }

        dialog.dialog({
            autoOpen: false,
            title: settings.title,
            width: settings.width,
            height: settings.height,
            modal: settings.modal,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                dialog.dialog('destroy');
                dialog.remove();
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        dialog.dialog('open');

        var dialogContent = $(settings.content).addClass(settings.contentCustomClass).appendTo(dialog);
        
    }

    //populate and open the reusable dialog
    function _popGeneric(title, content, w, h) {
        /* DEPRECATED 
        var genericDialog = $('#genericDialog').empty();
        w = w || 350;
        h = h || 'auto';
        genericDialog.append($(content).addClass('genericDialogContent'));
        genericDialog.dialog("option", "title", title)
        genericDialog.dialog("option", "width", w);
        genericDialog.dialog("option", "height", h);
        genericDialog.dialog('open');
        */

        _popDisposable({
            ID: 'genericDialog',
            title: title,
            content: content,
            width: w,
            height: h,
            modal: true,
            contentCustomClass: 'genericDialogContent'
        });

    }

    //use as to add onOpen functionality to all dialogs at once
    function _commonDialogOpenFunction(dialog) {
        //this allows dialogs to be dragged off screen
        //NOTE: if this change is reverted, change ui-dialog position back to absolute form fixed, in frame d2 css
        if (dialog.parent().hasClass('ui-draggable')) {
            dialog.dialog("widget").draggable("option", "containment", "none");
        }        
    }

    //use as to add onClose functionality to all dialogs at once
    function _commonDialogCloseFunction(dialog) {
        //this fixes the issue of a dialog opened from a modal dialog not putting mask back.
        var mask = $('.ui-widget-overlay.ui-front');
        if (mask.length) {
            mask.css("z-index", mask.css("z-index") - 1);
        }
    }

    //this forces a dialog to always open fullscreen
    function _openFullScreen(dialog) {
        var maxH = $(window).height();
        var maxW = $(window).width();
        dialog.dialog({ width: maxW, height: maxH });
    }
    
    //this adds styling to all dialogs, also a good place to add oncreate event binding
    function _addDialogStylingElements(dialog) {
        var titleBar = dialog.parent().find('.ui-dialog-titlebar');
        var titleBarElements = "<div class='roeStyleTitle'><div class='headerL'></div><div class='headerM'></div><div class='headerR'></div><div class='headerOverlay'></div></div>";
        var dialogContentElements = "<div class='roeStyleContent'><div class='contentBody'></div><div class='contentLT'></div><div class='contentLM'></div><div class='contentLB'></div><div class='contentBottom'></div><div class='contentRT'></div><div class='contentRM'></div><div class='contentRB'></div></div>";
        dialog.parent().prepend(dialogContentElements);
        titleBar.prepend(titleBarElements);
        $(".ui-dialog-titlebar-close", titleBar).click(function () { ROE.UI.Sounds.clickMenuExit(); });
        dialog.bind('dialogdragstop', function () { _dialogDragEndSnapBack(this); });
        dialog.parent().mouseover(function () { $('.mapGuiPanel').stop().hide(); });
    }

    function _dialogDragEndSnapBack(D) {
        //this prevents dialogtop being unreachable beyond top of window
        var DialogParent = $(D).parent();        
        if (DialogParent.position().top < 0) {
            DialogParent.animate({ top: "0px" }, 500, function () {
                //saves the last position to dialog widget, as css moving alone doesn't
                $(D).dialog('option', 'position', [$(this).offset().left, $(this).offset().top]);
            });
            
        }
    }

  
    function _isPopupOpen(selector) {

        var isOpen = false;
        var jqElement = $(selector);

        if (jqElement.hasClass('ui-dialog')) {
            isOpen = $(selector).dialog("isOpen");
        } else {
            isOpen = jqElement.length > 0;
        }

        return isOpen;
    }

    function _showBuyCredits() {
        if (ROE.Player.recEmail_isKLogin) {
            window.location.assign("pfCredits_kongregate2.aspx");
        }
        else {
            //window.location.assign("pfCredits.aspx");
            window.open('pfCredits.aspx', '_blank');
        }
    }

    //
    // PUBLIC INTERFACE
    //
    obj.isPopupOpen = _isPopupOpen;
    obj.addDialogStylingElements = _addDialogStylingElements;
    obj.reloadFrame = _reloadFrame;
    obj.showVillList = _showVillList;
    obj.showServantsPopup = _showServantsPopup;
    obj.showBuildPopup = _showBuildPopup;
    obj.showSilverTransportPopup = _showSilverTransportPopup;
    obj.switchToView = _switchToView;
    obj.switchToMap = _switchToMap;
    obj.reloadView = _reloadView;
    obj.popupBattleSim = _popupBattleSim;
    obj.popupMail = _popupMail;
    obj.popupMailPrefilled = _popupMailPrefilled;
    obj.popupReports = _popupReports;
    obj.checkedReports = _checkedReports;
    obj.popupPlayerProfile = _popupPlayerProfile;
    obj.popupVillageProfile = _popupVillageProfile;
    obj.popupHighlight = _popupHighlight;
    obj.popupMapSummary = _popupMapSummary;
    obj.popupAttacks = _popupAttacks;
    obj.popupSupport = _popupSupport;
    obj.popupCommandTroops = _popupCommandTroops;
    obj.popupGifts = _popupGifts;
    obj.popupGiftSend = _popupGiftSend;
    obj.popupVillageNote = _popupVillageNote;
    obj.popupInOut = _popupInOut;
    obj.popupInOut_togglePin = _popupInOut_togglePin;
    obj.popupInOut_filterSpecificWidget_toVillage = _popupInOut_filterSpecificWidget_toVillage;
    obj.enableView = _enableView;
    obj.busy = _busy;
    obj.busyFail = ROE.Frame.base_fail;
    obj.free = _free;
    obj.isBusy = _isBusy;
    obj.reloadTheWindow = _reloadTheWindow;
    obj.purchase = _purchase;
    obj.rateApp = _rateApp;
    obj.visitPrevVillage = _visitPrevVillage;
    obj.visitNextVillage = _visitNextVillage;
    obj.centerThisVillage = _centerThisVillage;
    obj.swipeLeft = _swipeLeft;
    obj.swipeRight = _swipeRight;
    obj.swipeUp = _swipeUp;
    obj.swipeDown = _swipeDown;
    obj.refresh = _refresh;
    obj.pinchZoom = _pinchZoom;
    obj.backButton = _backButton;
    obj.popupInfo = _popupInfo;
    obj.popupInfoClose = _popupInfoClose;
    obj.infoBar = _infoBar;
    obj.errorBar = _errorBar;
    obj.infoBarClose = _infoBarClose;
    obj.settingsPopup = _settingsPopup;
    obj.popupQuests = _popupQuests;
    obj.popupAccount = _popupAccount;
    obj.CurrentView = function _currentView() { return _view; };
    obj.popupClan = _popupClan;
    obj.popupStats = _popupStats;
    //obj.popupAvatar = _popupAvatar;
    obj.IsPopupOpened = _IsPopupOpened;
    obj.popupQuickCommand = _popupQuickCommand;
    obj.popupStewardship = _popupStewardship;
    obj.inputFocused = _inputFocused;
    obj.inputBlured = _inputBlured;
    obj.init = _init;
    obj.someInOutgoingCountdownFinished = _someInOutgoingCountdownFinished
    obj.refreshPFHeader = _refreshPFHeader;
    obj.popupInOutSummary = _popupInOutSummary;
    obj.popupTou = _popupTou;
    obj.popupContactSupport = _popupContactSupport
    obj.showIframeOpenDialog = _showIframeOpenDialog;
    obj.isInIframe = _isInIframe;
    obj.ViewInFrame = _viewInFrame;
    obj.isInIframe = _isInIframe;
    obj.viewFullscreen = _viewFullscreen;
    obj.launchClanInviteRequestMessage = _launchClanInviteRequestMessage;
    obj.showBuyCredits = _showBuyCredits;
    obj.flashCreditCount = _flashCreditCount;
    obj.mapNotifWhy = _mapNotifWhy;
    obj.popGeneric = _popGeneric;
    obj.popDisposable = _popDisposable;
    obj.questRewardAcceptCB = _questRewardAcceptCB;
    obj.spawnFlyRewardIcon = _spawnFlyRewardIcon;
    obj.refreshQuestCount = _refreshQuestCount;

}(window.ROE.Frame = window.ROE.Frame || {}));


//this shim fixes issue with jquery UI dialog modal mask -farhad Oct 29 2014
jQuery.ui.dialog.prototype._moveToTop = function (event, silent) {
    var moved = false,
        zIndicies = this.uiDialog.siblings(".ui-front:visible").map(function () {
            return +$(this).css("z-index");
        }).get(),
        zIndexMax = Math.max.apply(null, zIndicies);

    $('.ui-widget-overlay').css("z-index", zIndexMax);
    if (zIndexMax >= +this.uiDialog.css("z-index")) {
        this.uiDialog.css("z-index", zIndexMax + 1);
        moved = true;
    }

    if (moved && !silent) {
        this._trigger("focus", event);
    }
    return moved;
}

//this adds some extra code to jquery UI Dialog Open, to tackle the problem of an already open dialog not calling its open function
//purpose of this was to recover dialogs that were dragged out of view. If we decide to contain them again inside the viewport, we can remove all of this - farhad Oct 27, 2014
$.ui.dialog.prototype._originalOpen = $.ui.dialog.prototype.open;
$.ui.dialog.prototype.open = function () {

    $.ui.dialog.prototype._originalOpen.apply(this, arguments);

    var D = this.uiDialog;
    var P = D.position();
    var w = D.width();
    var h = D.height();
    var limit = 100;
    var right = $(window).width();
    var bottom = $(window).height();

    //if dialog is out of viewport, put it back in the center
    if (P.left < limit - w || P.left + limit > right || P.top < limit - h || P.top + limit > bottom) {
        D.position({
            my: "center",
            at: "center",
            of: window,
            using: function (position) {
                $(this).animate(position, 300, function () {
                    //D.dialog('option', 'position', [$(this).offset().left, $(this).offset().top]);
                });
            }
        });
        
    }

};