var vovNutTest = 0;
(function (obj) {
    BDA.Console.setCategoryDefaultView('ROE.Frame', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file

    var infoTimeout = 0;
    var Enum = {}; // private enums
    Enum.Views = { vov: "vov", map: "map" };

    obj.Enum = Enum;

    var deviceHeight = $(window).height();
    var head = document.getElementsByTagName('head')[0];
    var style = "<STYLE>\n #background { height: %DeviceHeight%px !important; } \n .iFrameDiv > .popupBody { height: %popupBodyHeigh%px !important; }</STYLE>".format({ DeviceHeight: deviceHeight, popupBodyHeigh: deviceHeight - 44 });
    var InOutWidget_incomingList; // refrence to the incoming list of commands widget
    var InOutWidget_outgoingList; // refrence to the outgoing list of commands widget
    $(head).append(style);


    var _view = Enum.Views.vov;
    var CONST = {};
    CONST.Selector = {
        currentVillage: {
            incomingAlert: ".vov-inout-alert.inc"
            , outgoingAlert: ".vov-inout-alert.out"
        }
    };



    function _populateOrRefreshFrame_CommonStuff() {

        $("header .hdrPoints").text(ROE.Utils.formatShortNum(ROE.Player.Ranking.points));
        $("header #hdrPlrLvl").text(ROE.Utils.addThousandSeperator(ROE.Player.Ranking.titleLvlR));
        $(".hvov.pInfo .progress-indicator").css("width", String(ROE.Player.Ranking.titleProgress) + "%");

        //_togleIncoming(ROE.Player.IncomingAtt);
        _handleQuestIcon();

        $('.footernav .reports').toggleClass('newReport effect-pulse', ROE.Player.Indicators.report);
        $('.footernav .mail').toggleClass('newMail effect-pulse', ROE.Player.Indicators.mail);
        $('.footernav .clan').toggleClass('newClan effect-pulse', ROE.Player.Indicators.clanForum);

        if (ROE.Player.Ranking.levelUp) {
            ROE.Player.Ranking.levelUp = false;
            _showLevelUpPopup();
        }

        ROE.Frame.handleSleeModeIcon();
        ROE.Frame.handleResearchIcon();
        ROE.Frame.handleDailyReward();

        //replace Build btn with QuickBuild button for players with more than One Vill
        if ($("#openQuickBuildBtn").length > 0 && ROE.Player.numberOfVillages > 1) {
            $('#hdrBuild').hide();
            $('#openQuickBuildBtn').show();
        }

        if (ROE.Player.numberOfVillages > 1) {
            $('#openQuickRecruitBtn').show();
        }

        _handleVA();
    }

    function _handleVA() {
        if (ROE.Player.va == 2) {
            ROE.Device.checkVAA();
            $("#showvideoad").hide();
        }
        else if (ROE.Player.va == 1) {
            $("#showvideoad").fadeIn();
        } else {
            $("#showvideoad").hide();
        }
       
    }

    var _populateFrame = function () {
        _populateOrRefreshFrame_CommonStuff();
        _refreshPFHeader(ROE.Player.credits);
    };

    var _reloadVillageInfo = function (village) {
        try {
            $("#villName").text(village.name);
            $("#villCoins").text(ROE.Utils.formatShortNum(village.coins));
            $("#villFood").text(ROE.Utils.formatShortNum(village.popRemaining));
            $("#hdrLoyalty").text(village.yey < 100 ? "!" : "");
            if (village.areTranspAvail) {
                $("#hdrCoins img").attr("src", "https://static.realmofempires.com/images/icons/M_silver_sml_glow.png");
                $("#hdrCoins img").addClass("effect-pulse");
            } else {
                $("#hdrCoins img").attr("src", "https://static.realmofempires.com/images/icons/M_silver_sml.png");
                $("#hdrCoins img").removeClass("effect-pulse");
            }
            $(".troopsStrength").text(ROE.Utils.formatShortNum(ROE.Utils.getTroopStrength(village).att));
            $(".troopsStrengthDef").text(ROE.Utils.formatShortNum(ROE.Utils.getTroopStrength(village).def));
            $("#villCords").text("(" + village.x + "," + village.y + ")");
        } catch (e) {
            var roeex = new BDA.Exception("erorr in frame._reloadVillageInfo", e);
            roeex.data.add('village', village);
            BDA.latestException = roeex;
            BDA.Console.error(roeex);
            throw roeex;
        }
    };

    var _reloadFrame = function (callBack) {
        ///<summary>reloads all the info that is on the vov header. optionally, pass a call back to be called when done.</summary>
        ROE.Player.load(function () {
            _populateFrame();
            ROE.Villages.getVillage(ROE.SVID, _reloadVillageInfo, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
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


    function _timedRefreshFrameCallBack_timeout() {
        $('.networkerrors_slow').fadeIn();
    }
    function _timedRefreshFrameCallBack_error() {
        $('.networkerrors_noconnection').fadeIn();
        $('.networkerrors_slow').hide();
    }


    var _timedRefreshFrameCallBack = function () {
        // since we know playerrefresh now completed OK, hide any errors/ warnings the coudl have resulted from playerrefresh call
        $('.networkerrors_slow').hide();
        $('.networkerrors_noconnection').hide();

        _populateOrRefreshFrame_CommonStuff()


        if (ROE.Player.Indicators.report) {
            BDA.Broadcast.publish("NewReports");
        }
        if (ROE.Player.Indicators.mail) {
            BDA.Console.verbose('ROE.Frame', 'new mail indicated');
            BDA.Broadcast.publish("NewMail");
        }

        if (ROE.Player.chooseGovType) {
            $('.selectGovType').show();
            $('.inviteFriendsReward').hide(); //since the buttons share space
        }

        var inviteRewardIcon = $('.inviteFriendsReward');
        if (ROE.Realm.friendRewardBonusUntil > (new Date)) {
            inviteRewardIcon.addClass('promo');
            if ($('.counter', inviteRewardIcon).length == 0) {
                inviteRewardIcon.append('<span class="counter countdown" data-finisheson="' + ROE.Realm.friendRewardBonusUntil + '"></span>');
            }
        } else {
            inviteRewardIcon.removeClass('promo').find('.countdown').remove();
        }


        //recovery email

        if ((ROE.Player.recEmail_isDeviceLogin || (!ROE.Player.recEmailSet && ROE.Player.recEmailState < 2)) && ROE.Player.Ranking.titleLvl > 1) {
            $(".emailEntryIcon").show();
        }
        else {
            $(".emailEntryIcon").hide();
        }

        ROE.Frame._checkIfReloadNeededDueToUpdate();

        ROE.Frame._fireActivityEvent('M');

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

        ROE.Frame.simplePopopOverlay("https://static.realmofempires.com/images/icons/M_chalice.png", "LEVEL UP!", content, 'levelUpOverlay');

        if (newTitle.unlockedGift.length === 0) {
            $("#popup_levelUp .iteminfo").hide();
        } else {
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
            $('.levelUpOverlay').remove();
            //simulating a click on the quest button is lazy, but less risky on M
            $('#linkQuests')[0].click();
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

        travelToText = travelToText || "Traveling To"; // default text if not specified
        ROE.Frame.busy();
        ROE.Frame.base_busyText(travelToText);
        ROE.Villages.getVillages(function getVillagesCallBack(villageList) {
            for (var i = 0; i < villageList.length; i++) {
                if (villageList[i].id == ROE.SVID) {
                    var j = i + (direction === 'prev' ? -1 : 1);
                    if (j >= villageList.length) {
                        j = 0;
                    } else if (j < 0) {
                        j = villageList.length - 1;
                    }
                    ROE.Frame.base_busyText(travelToText + ' ' + villageList[j].name);

                    // important note; typically, we should not be calling _showThisVOV since this would get called anyway as part of the 
                    //  event that fires after calling setCurrentlySelectedVillage. However, we call _ShowThisVOV so that we get a call back when the call 
                    //  finishes so that we can remove the buys overlay. There will be no double calls due to this since the event that fires after 
                    //  setCurrentlySelectedVillage will not do anything since the village loaded is already the right one. 
                    ROE.Frame.setCurrentlySelectedVillage(villageList[j].id, villageList[j].x, villageList[j].y, villageList[j].name);

                    ROE.Villages.getVillage(villageList[j].id,
                        function (village) {
                            _showThisVOV2(village);
                            if (typeof optionalCallBackFn === 'function') { optionalCallBackFn(village); }
                            ROE.Frame.free();
                        }
                        , ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);

                    break;
                }
            }
        });
    };

    var _showThisVOV2 = function (village) {
        BDA.Console.verbose('ROE.Frame', 'Showing village :%vid%'.format({ vid: ROE.SVID }));

        _updateInOutgoingForSelectedVillage();

        _populateVOV(village.VOV);
        _reloadVillageInfo(village);
        ROE.MapEvents.checkAddVOVCaravan(); //mainly to show caravans if any
        _free();
    };

    var _showServantsPopup = function () {
        /// <summary>
        /// Show the UI with servants and related info 
        ///</summary>
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'servants').length === 0) {
            ROE.UI.Sounds.click();
            popupModalPopup('servants', BDA.Dict.PremiumFeatures, null, null, null, null, null, "https://static.realmofempires.com/images/icons/M_servants.png");
            var popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'servants' + ' .popupBody');
            var settings = {
                mode: "popup", // popup || inline
                fill: popupContent, //for popup mode, popup content must be passed, for inline mode any element can be passed
                featureId: null //for inline mode only, display one PF
            }
            ROE.UI.PFs.init(settings, _refreshPFHeader); //initializes the widget
        }
    };

    var _showPFStatus = function () {
        var len = ROE.PFPckgs.Order.length;
        var element = $('.pfStatus');
        var tempTable = $('<div>');
        var row = '<div data-pfpackageid=%id% class="%class%"></div>';
        for (var i = 0; i < len; i++)//populates contents of rows
        {
            rowPopulated = BDA.Templates.populate(row, {
                id: ROE.PFPckgs.Order[i],
                'class': (ROE.Player.PFPckgs.isActive(ROE.PFPckgs.Order[i])) ? (((BDA.Utils.timeLeft(new Date(ROE.Player.PFPckgs.list[ROE.PFPckgs.Order[i]].ExpiresOn))).h < 6) ? 'active-warn' : 'active') : 'not-active'
            });
            tempTable.append(rowPopulated);
        }
        element.html(tempTable.find('div'));
    };

    var _refreshPFHeader = function (newCredits) {

        if (typeof (newCredits) != 'undefined') {
            ROE.Player.credits = newCredits;
            $("header .playerCredits").attr('data-credits', newCredits).text(' ' + BDA.Utils.formatNum(newCredits));
        }

        _showPFStatus();
    };

    //explains map notifications
    var _mapNotifWhy = function _mapNotifWhy(id) {
        if (id == 1) { //servant farm
            var title = "Map Event Notification";
            var content = $('<div class="mapNotifWhyDialog">');
            var info = $('<div class="info">').html('Attacking Rebel or Abandoned villages gives you a chance to rescue some servants.<br><br>' +
                'Once you\'ve rescued a servant, you can click and collect them from the map. They never expire.<br><br>' + 'Now go and find your rescued servants!');
            content.append(info);
            _popGeneric(title, content);
        }
    }

    var _showTroopPopup_toggleTabs = function (that) {
        if (that == "vilres_allTroop") {//will check which tab currently selected
            if ($('.troopInformation_popup #vilres_allTroop').attr('class') != 'selected') {
                $('.troopInformation_popup #vilres_allTroop').addClass('selected');
                $('.troopInformation_popup #vilres_myTroop').removeClass('selected');
                $('.troopInformation_popup #vilres_support').removeClass('selected');
                _showTroopPopup_toggleTabs_UpdateTroops('all');
                $(".troopInformation_popup .vilres_mytroopbottomtabs").hide();
            }
        } else if (that == "vilres_myTroop") {
            if ($('.troopInformation_popup #vilres_myTroop').attr('class') != 'selected') {
                $('.troopInformation_popup #vilres_myTroop').addClass('selected');
                $('.troopInformation_popup #vilres_allTroop').removeClass('selected');
                $('.troopInformation_popup #vilres_support').removeClass('selected');
                $('.troopInformation_popup #vilres_myAllTroop').addClass('selected');
                $('.troopInformation_popup #vilres_myInVilTroop').removeClass('selected');
                _showTroopPopup_toggleTabs_UpdateTroops('my');
                $(".troopInformation_popup .vilres_mytroopbottomtabs").show();
            }
        } else if (that == "vilres_support") {
            if ($('.troopInformation_popup #vilres_support').attr('class') != 'selected') {
                $('.troopInformation_popup #vilres_support').addClass('selected');
                $('.troopInformation_popup #vilres_allTroop').removeClass('selected');
                $('.troopInformation_popup #vilres_myTroop').removeClass('selected');
                _showTroopPopup_toggleTabs_UpdateTroops('support');
                $(".troopInformation_popup .vilres_mytroopbottomtabs").hide();
            }
        } else {
            if (that == "vilres_myAllTroop") {
                if ($('.troopInformation_popup #vilres_myAllTroop').attr('class') != 'selected') {
                    $('.troopInformation_popup #vilres_myAllTroop').addClass('selected');
                    $('.troopInformation_popup #vilres_myInVilTroop').removeClass('selected');
                    _showTroopPopup_toggleTabs_UpdateTroops('my');
                }
            } else if (that == "vilres_myInVilTroop") {
                if ($('.troopInformation_popup #vilres_myInVilTroop').attr('class') != 'selected') {
                    $('.troopInformation_popup #vilres_myInVilTroop').addClass('selected');
                    $('.troopInformation_popup #vilres_myAllTroop').removeClass('selected');
                    _showTroopPopup_toggleTabs_UpdateTroops('myInVillage');
                }
            }
        }
        //end check tab currently selected
    };

    function _showTroopPopup_toggleTabs_UpdateTroops(whichTR) {
        var popup = $('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'troop' + ' .popupBody');

        // helper function to get the right troop 
        var getTroopCount = function (curTroopObj) {
            if (whichTR == 'all') {
                return curTroopObj.TotalNowInVillageCount;
            } else if (whichTR == 'my') {
                return curTroopObj.YourUnitsTotalCount;
            } else if (whichTR == 'support') {
                return curTroopObj.SupportCount;
            } else if (whichTR == 'myInVillage') {
                return curTroopObj.YourUnitsCurrentlyInVillageCount;
            }
        };

        if (whichTR == 'support') {
            popup.find('.detailedSupport').show();
        } else {
            popup.find('.detailedSupport').hide();
        }

        popup.find('.vilres_unitcountnumb').each(
            function (index, elem) {
                var curTroopObj = _showTroopPopup_village.TroopByID($(this).attr('data-troopid'));
                $(this).text(ROE.Utils.addThousandSeperator(getTroopCount(curTroopObj))).fadeIn();
            }
        );
    }

    var _showTroopPopup = function () {
        ROE.Villages.getVillage(ROE.SVID
            , _showTroopPopup_gotVillage
            , ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);

    }

    var _showTroopPopup_village; // used only by the troopsPopup; its a hack - showTroopsPopup should be its own module
    var _showTroopPopup_gotVillage = function (village) {
        _showTroopPopup_village = village;

        if ($('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'troop').length === 0) {

            popupModalPopup('troop', 'Troop Info', undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeader, "https://static.realmofempires.com/images/misc/M_TroopInfo.png");
            ROE.UI.Sounds.click();
            var data = {};
            data.id = ROE.SVID;
            data.troops = {};
            var len = ROE.Entities.UnitTypes.SortedList.length;
            var unitType;
            for (var i = 0; i < len; i++) {
                unitType = ROE.Entities.UnitTypes[ROE.Entities.UnitTypes.SortedList[i]];
                data.troops['id' + unitType.ID] = {};
                data.troops['id' + unitType.ID].YourUnitsTotalCount = ROE.Utils.addThousandSeperator(village.TroopByID(unitType.ID).TotalNowInVillageCount);

            }
            content = $(BDA.Templates.get('TroopInformation', ROE.realmID, data));

            var popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'troop' + ' .popupBody');

            var getStrength = ROE.Utils.getTroopStrength(village);
            content.find('.troopInformation_popup .attackStrength').text(ROE.Utils.addThousandSeperator(getStrength.att));
            content.find('.troopInformation_popup .defenceStrength').text(ROE.Utils.addThousandSeperator(getStrength.def));

            popupContent.html(popupContent.html() + content.html());

            BDA.Broadcast.subscribe(popupContent, "VillageExtendedInfoUpdated", _showTroopPopup_refreshed);
            BDA.Broadcast.subscribe(popupContent, "VillageExtendedInfoInitiallyPopulated", _showTroopPopup_refreshed);
            ROE.Villages.ExtendedInfo_loadLatest(village.id); // ensure that we got the latest troop counts

        }
    };

    var _showTroopPopup_refreshed = function (village) {
        if (village.id === ROE.SVID) {
            var popup = $('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'troop' + ' .popupBody');
            var len = $('.troopInformation_popup .vilres_unitcountnumb').length;
            for (var i = 0; i < len; i++) {
                var curSpanTroopId = $('.troopInformation_popup .vilres_unitcountnumb')[i].getAttribute('data-troopid');
                var curTroopObj = village.TroopByID(curSpanTroopId);
                popup.find('#id' + curSpanTroopId).text(ROE.Utils.addThousandSeperator(curTroopObj.TotalNowInVillageCount));
            }
        }
    };

    var _showResPopup = function () {
        if ($('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'Resources').length === 0) {
            ROE.UI.Sounds.click();
            ROE.VillageResources.showResourcesPopup();
        }
    };

    var _showBuildPopup = function () {
        if ($('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'build').length === 0) {
            ROE.UI.Sounds.click();
            popupModalPopup("build", "Build", undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeader, "https://static.realmofempires.com/images/icons/m_build.png");
            var popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'build .popupBody').append($('<div class="BuildPopup"></div>'));
            ROE.Build.init(popupContent.find('.BuildPopup'));
        }
    };

    var _showSilverTransportPopup = function (villageId, isMine) {
        ROE.UI.Sounds.click();
        ROE.SilverTransport.init($('#transportSilverDialog').dialog('open'), villageId, isMine);
    };

    var _switchToVoV = function switchToVoV(newVID) {
        ///<summary>
        ///     call this to force the frame to switch to the VOV view 
        ///     It is safe to call this from anywhere and this takes care of everything - but make sure to close your own popup or window if applicable
        ///</summary>
        _switchToView(Enum.Views.vov);
        _busy();
        
        ROE.Frame.setCurrentlySelectedVillage(newVID);

        ROE.Villages.getVillage(ROE.SVID,
            function (village) {
                ROE.CSV = { id: village.id, x: village.x, y: village.y };
                _showThisVOV2(village);
                _free();
            }
            , ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);

    };

    var _switchToMap = function (x, y) {
        ///<summary>
        ///     call this to force the frame to switch to the map view centered at the x and y 
        ///     passed in. 
        ///     It is safe to call this from anywhere and this takes care of everything - but make sure to close your own popup or window if applicable
        ///</summary>
        _switchToView(Enum.Views.map, function () { _fnLoad_mapByCoords(x, y); });
    };

    var _fnTogleView = function () {
        switch (_view) {
            case Enum.Views.vov:
                return _switchToView(Enum.Views.map);
            case Enum.Views.map:
                return _switchToView(Enum.Views.vov);
                //case Enum.Views.research:
                //return _switchToView(Enum.Views.vov);
            default:
                return _switchToView(Enum.Views.vov);
        }
    };

    var _switchToView = function (view, customfunctionToLoadView) {
        ///<summary>customfunctionToLoadView is optional - pass it if you want to override the default bahaviour</summary>
        _optionsPopup(false); // close options if opened

        if (view != Enum.Views.map && _view == Enum.Views.map) {
            ROE.Landmark.close();
        }

        var functionToLoadView = null;
        switch (view) {
            case Enum.Views.map:
                _view = Enum.Views.map;
                $("body").removeClass('view-vov').addClass('view-map').removeClass('view-res');
                $(".footernav .togle").addClass("vovmode");
                $("div.TDContent.mainView").css("top", "0px");
                $("header").hide();
                $("header > .headernav").show();
                $("header > .research").remove();
                $("footer > .research").remove();
                if (localStorage.settings_showChatOnMap == "true") {
                    $('#chatContainer').show();
                } else {
                    $('#chatContainer').hide();
                }

                functionToLoadView = _load_map;
                break;
            case Enum.Views.vov:
            default:
                _view = Enum.Views.vov;
                $("body").addClass('view-vov').removeClass('view-map').removeClass('view-res');
                $(".footernav .togle").removeClass("vovmode");
                $("div.TDContent.mainView").css("top", "");
                $("header > .headernav").show();
                $("header > .research").remove();
                $("footer > .research").remove();
                $("header").show();
                $('.vovBuildPnl .building').attr("href", ""); //TEMP FIX - because we are still using the OLD Build panel that has A tags, we want to quickly remove the HREFs because sometimes on iphone, they would fire instead of onclicks added when the panel is displayed
                $('#chatContainer').show();
                functionToLoadView =
                    function _switchToView_vovload() {
                        ROE.Villages.getVillage(ROE.SVID
                            , function _switchToView_vovload_gotvillage(village) {
                                _showThisVOV2(village);
                            }
                            , ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
                    }
                break;
        }

        $('.hvov, .hmap').addClass('forceHide').hide();
        $('.h' + view).removeClass('forceHide').show();

        if (customfunctionToLoadView) {
            customfunctionToLoadView();
        } else {
            functionToLoadView();
        }

        return false;
    };

    var _toggleChatOnMapView = function () {
        var currentSetting = localStorage.settings_showChatOnMap;

        if (!currentSetting || currentSetting == "false") {
            localStorage.settings_showChatOnMap = "true";
            $('#chatContainer').show();
        } else {
            localStorage.settings_showChatOnMap = "false";
            $('#chatContainer').hide();
        }

    }

    function _spawnFlyRewardIcon(data) {

        var claimDiv = $('<div class="questRewardFly">');

        $('body').append(claimDiv);

        claimDiv.css({
            left: $(window).width() / 2 - claimDiv.width() / 2,
            top: $(window).height() / 2 - claimDiv.height() / 2
        });

        claimDiv.animate({ top: '-=30px' }, 800, "easeOutSine").animate({ top: '110px', left: '1px', width: 44, height: 44, opacity: .2 }, 700, "easeOutSine", function () {
            $(this).remove();
        });

    }

    //When a completed quest "Get Reward" in Quest.aspx is clicked
    function _questRewardAcceptCB(data) {

        //send a big reward icon flying towards rewards icon
        ROE.Frame.spawnFlyRewardIcon();

        //update rewards (gets latest)
        //ROE.Items2.update();

        //decrement quest completed count 
        ROE.Player.questsCompl = data.completedQuests;

        //update player credits and UI
        _refreshPFHeader(data.credits);

        //update quest icon UI
        _handleQuestIcon();

        //call for latest count, to make sure all is synched.
        //_refreshQuestCount();

        ROE.Frame.questRewardAccepted_ReloadUI();
    }

    var _refreshQuestTimeout; //used for throttling calls
    function _refreshQuestCount() {
        clearTimeout(_refreshQuestTimeout);
        _refreshQuestTimeout = setTimeout(function () {
            ROE.Api.call_quest_getcompletedcount(function getcompletedcountCB(data) {
                ROE.Player.questsCompl = data.questsCompletedCount;
                _handleQuestIcon();
            });
        }, 800);
    }

    function _handleQuestIcon() {
        $("header .linkQuests img")[0].src = ROE.Player.questsCompl > 0 ? "https://static.realmofempires.com/images/icons/m_Quests2.png" : "https://static.realmofempires.com/images/icons/m_Quests.png";
        $('#linkQuests_completedCount').text(ROE.Player.questsCompl > 0 ? ROE.Player.questsCompl : '');
        ROE.Frame.iconNeedsAttention($("header .linkQuests img"), ROE.Player.questsCompl > 0);
        if (ROE.Player.questsCompl > $('#linkQuests_completedCount').attr('data-completedQuestCount')) {
            $('iframe[src="Quests.aspx"]').contents().find('#pageReloader').show();
        }
        $('#linkQuests_completedCount').attr('data-completedQuestCount', ROE.Player.questsCompl);
    }


    var _load_map = function () {
        ///<summary>loads the map centerd on the player's currently selected village</summary>
        _fnLoad_mapByCoords(ROE.CSV.x, ROE.CSV.y);
    };

    var _fnLoad_mapByCoords = function (x, y) {
        ROE.Landmark.start(x, y, 1);
    };


    var _populateVOV = function (village) {
        if (_view == Enum.Views.vov) { // if we are still on the VOV after this call
            ROE.vov._initializeNewVOV();
            ROE.vov.init(village);
        }
    };

    var _reloadView = function (callback) {
        $('.hvov, .hmap').addClass('forceHide').hide();
        $('.h' + _view).removeClass('forceHide').show();

        switch (_view) {
            case Enum.Views.vov:
                ROE.Villages.getVillage(ROE.SVID,
                     function (village) {
                         _showThisVOV2(village);
                         if (callback) { callback(); }
                     }
                     , ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
                break;
            case Enum.Views.map:
                _load_map(); if (callback) { callback(); }
                break;
            case Enum.Views.research:
                _load_research(); if (callback) { callback(); }
                break;
        }
        return false;
    };

    var _toggleOptionsPopup = function () {
        ROE.UI.Sounds.click();
        if ($("#roeOptions:visible").length > 0) {
            _optionsPopup(false);
        } else {
            _optionsPopup(true);
        }


    };

    var _optionsPopup = function (show) {

        if (!show) {
            if ($("#roeOptions:visible").length > 0) {
                $("#roeOptions").hide();
            }
        } else {
            _enableView(false);

            $("#roeOptions .viewSpecific").hide();
            $("#roeOptions #roeOptions-vov").show();
            if (_view == Enum.Views.map) {

                var zoomScale = parseInt(ROE.Landmark.scale * 5) - 1;
                //$("#roeOptions .zoomlevel").text(zoomScale);
                $("#roeOptions #roeOptions-" + Enum.Views.map).show();
            }

            $("#roeOptions").show();

        }
    };

    var _popupMail = function () {
        if ($('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'mail').length === 0) {
            ROE.UI.Sounds.click();
            ROE.Mail.init(_createPopup('mail', 'Mail'), $('#linkMail').hasClass('newMail'));
        }
    };

    var _popupMailPrefilled = function (to, subject, message) {

        var youveGotMail = $('#linkMail').hasClass('newMail');

        if ($('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'mail').length === 0) {
            ROE.UI.Sounds.click();
            ROE.Mail.init(_createPopup('mail', 'Mail'), youveGotMail, { to: to, subject: subject, message: message });
        }

    }


    var _createPopup = function (name, title, img) {
        var uname = name[0].toUpperCase() + name.substr(1, name.length);

        if ($('.' + uname + 'Popup').length > 0) {
            closeModalPopup(name, true);
        }

        var staticurl = "https://static.realmofempires.com/images/";
        var imgurl = !img ? staticurl + "icons/m_" + name + ".png" : staticurl + img;
        popupModalPopup(name, '', undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeader, imgurl);

        content = $('<div class="' + uname + 'Popup"></div>');
        popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + name);
        $('.popupBody', popupContent).append(content);
        $('.themeM-panel.header .label', popupContent).html(title);

        return popupContent.find('.' + uname + 'Popup');
    };

    var _popupHighlight = function (e) {
        e.preventDefault();
        ROE.Map.Highlights.init(_createPopup('highlight', 'Map Highlights', 'icons/highlight2.png'));
        return false;
    };

    var _popupMapSummary = function (e) {
        e.preventDefault();
        ROE.Map.Summary.init(_createPopup('mapsummary', 'Summary', 'icons/m_Mail.png'));
        return false;
    };

    var _popupClan = function (clanid, forumPostID, section) {
        if ($('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'clan').length === 0) {
            if (clanid === '0' || !clanid) {
                clanid = 0;// if not in clan
                if (ROE.Player.Clan) { clanid = ROE.Player.Clan.id; }
            }
            popupModalPopup('clan', "Clan", undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeader, "https://static.realmofempires.com/images/icons/m_Clan.png");
            content = $('<div class="ClanPopup" data-clanid=' + clanid + ' ></div>');
            popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'clan .popupBody').append(content);
            ROE.Clan.init(popupContent.find('.ClanPopup'), forumPostID);
        }
        return false;
    };

    /*var _popupStats = function (e) {
        ROE.UI.Sounds.click();
        ROE.Stats.init($('#statsDialog').dialog('open'));
        return false;
    };*/

    var _popupAccount = function (e) {
        ROE.UI.Sounds.click();
        ROE.Account.init($('#accountDialog').dialog('open'));
        return false;
    };

    var _popupReports = function (optionalSearchString) {
        if (localStorage.ReportPopup == 'false') return true; // old
        ROE.UI.Sounds.click();
        ROE.Reports.init(_createPopup('reports', BDA.Dict.reportTitle), null, optionalSearchString);
        return false;
    };

    var _popupPlayerProfile = function (name) {

        ROE.PlayerNew.init(_createPopup('playerNew', 'Player Profile', 'misc/M_TroopInfo.png'), name);
    };

    var _popupVillageProfile = function (id) {

        ROE.UI.VillageOverview.init(_createPopup('village', 'Village Profile', 'icons/M_VillList.png'), id);
    };

    // DEAD CODE ?
    //var _goToVillage = function (vid, pid) {
    //    if (ROE.isMe(pid)) {
    //        ROE.Frame.switchToVoV(vid);
    //        $('.iFrameDiv .IFrameDivTitle .action.close').click(); // close the popup. this code is generic and shoudl work for any popup
    //    } else {
    //        ROE.Frame.popupVillageProfile(vid);
    //    }
    //};

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
        var AttackTypeIcon = ["https://static.realmofempires.com/images/icons/M_Support.png", "https://static.realmofempires.com/images/icons/m_attacks.png"]; //support icon / attack icon
        //remove old attack popup content
        if ($(".AttacksPopup").length > 0) { $(".AttacksPopup").remove(); }

        popupModalPopup('attacks', pageType[attackType], undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeader, AttackTypeIcon[attackType]);
        var content = $('<div class="AttacksPopup" ></div>');
        $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'attacks .popupBody').append(content);
        //$("#" + ROE.Frame.CONSTS.popupNameIDPrefix + "attacks .AttacksPopup").append(BDA.Templates.getRawJQObj("CommandTroopsPopup", ROE.realmID));

        ROE.CommandTroops.init(attackType, vID, cordX, cordY, currentVillageID);
    };

    var _popupQuickCommand = function (AttackType, TARGETVID, COORX, COORY) {
        var AttackTypeName = ["War Room: Support", "War Room: Attack"]; //support label / attack label
        var AttackTypeIcon = ["https://static.realmofempires.com/images/icons/M_Support.png",
            "https://static.realmofempires.com/images/icons/m_attacks.png"]; //support icon / attack icon

        $('#quickCommandDialog').dialog('option', 'title', AttackTypeName[AttackType]);
        $('#quickCommandDialog').dialog('open');
        ROE.QuickCommand.init($('#quickCommandDialog'), AttackType, TARGETVID, COORX, COORY);     

        // for attacks and first time entry, display the tutorial
        if (AttackType == 1) {
            ROE.Tutorial.startIfNotAlreadyRan(1, 'warRoomMobile');
        }

        return false; //supress A click 
    };

    var _popupGifts = function (optionalVillageID) {
        ROE.UI.Sounds.click();
        ROE.Gifts.init($('#giftsDialog').dialog('open'), optionalVillageID);
        return false;
    };

    var _popupQuests = function _popupQuests() {
        return !popupModalIFrame2('Quests.aspx', 'popup', 'Quests', 'https://static.realmofempires.com/images/icons/M_Quests.png', closeModalIFrameAndReloadHeader);
    }

    var _settingsPopup = function (e) {


        popupModalPopup('settings', 'Settings', undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeader, "https://static.realmofempires.com/images/icons/M_Siege.png");

        content = $('<div class="SettingsPopup"></div>');


        popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'settings .popupBody').append(content);

        ROE.Settings.init(popupContent.find('.SettingsPopup'), 1);

        return false;
    };

    _popupInOut_initContent = function (container) {
        var content;
        var popupContent;
        var outgoingContent;

        template = BDA.Templates.getRawJQObj("InOutTroopsPopup2", ROE.realmID);

        // MUST ADD - toFromFilterObj passing. 
        InOutWidget_incomingList = ROE.Troops.InOutWidget.init(0, template.find('.incomingContainer'), undefined, template.find('.incoming.reload'));
        InOutWidget_outgoingList = ROE.Troops.InOutWidget.init(1, template.find('.outgoingContainer'), undefined, template.find('.outgoing.reload'));
        ROE.Troops.InOutSummary.init(0, template.find('.incomingContainer .summary'))
        ROE.Troops.InOutSummary.init(1, template.find('.outgoingContainer .summary'))

        container.append(template);

        // init the tabs, init the first tab as default
        //
        // TODO - this does not setup just this popup's tabs, but ALL tabs in entire app. so could attach multiple events to tabs. this must be redone. 
        tabs_init();
    };

    var _popupInOut = function (direction, toFromFilterObj) {
        /// <summary></summary>
        /// <param name="toFromFilterObj" type="Object">OPTIONAL! </param>
        /// <param name="direction" type="String">OPTIONAL! "in" | "out". "in" is default</param>

        ROE.vov.pauseAnimation();
        var content;
        var popupContent;
        var selector = "#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'incoming';
        if ($(selector).length === 0) {

            popupModalPopup('incoming', BDA.Dict.InOut,
                undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeader_HIDE, "https://static.realmofempires.com/images/misc/M_IncomingOutgoing.png");

            content = $('<div class=InOutPopup></div>');
            popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'incoming' + ' .popupBody');
            popupContent.append(content);

            _popupInOut_initContent(popupContent.find('.InOutPopup'));
            initTimers(); // make sure the countdown timers are taken care of. althought the widget calls it too, the first time it opens, it is not yet added to the dom so we do it again here. 
        } else {
            // handle the direction and filter here now 
            $(selector).show();
        }

        if (direction === 'out') {
            // direction is OUT
            if (toFromFilterObj) {
                InOutWidget_outgoingList.ToFromFilter(toFromFilterObj);
            }
            ROE.Troops.InOutWidget.inoutPopupSwitchMode('toOut');
        } else {
            // direction is IN
            if (toFromFilterObj) {
                InOutWidget_incomingList.ToFromFilter(toFromFilterObj);
            }
            ROE.Troops.InOutWidget.inoutPopupSwitchMode('toInc');
        }
        return false;
    };

    //var _togleIncoming = function (incomingAtt) {
    //    //if (incomingAtt.numAttacks > 0) {
    //    //    $("footer .footernav .inOutTroops").addClass('incoming');

    //    //    $("#lblCountdown")
    //    //        .attr("data-finishesOn", incomingAtt.firstAttackArrivalTime)
    //    //        .addClass('countdown');
    //    //    $("footer .footernav .inOutTroops .incomingCount").text("[%num%]".format({ num: incomingAtt.numAttacks}));
    //    //} else {
    //    //    $("footer .footernav .inOutTroops").removeClass('incoming');
    //    //    $("#lblCountdown").text('').removeClass('countdown');
    //    //}

    //};

    //var _incomingAttackCountdownAtZero = function () {
    //    //$("footer .footernav .inOutTroops").removeClass('incoming');
    //    //$("#lblCountdown").text('').removeClass('coutdown');
    //    //$("footer .footernav .inOutTroops .incomingCount").text('');
    //};

    var _enableView = function (enable) {
        //        //if (ROE.isDevice == ROE.Device.CONSTS.Android) {
        //        switch (_view) {
        //            case Enum.Views.vov:
        //                BDA.Console.log("VOV", enable);
        //                if (enable) {
        //                    $('.graphicalVOV a.building').show();
        //                }
        //                else {
        //                    $('.graphicalVOV a.building').hide();
        //                }
        //                break;
        //        }
        //        //}
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
            default: $("#PopupInfoBox_" + boxes).css("margin-top", Ypos);
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

    function _busy(overrideText, timeOutForTooLongMessage) {
        ROE.Frame.base_busy(overrideText, timeOutForTooLongMessage, null);
    }

  
    function _free() {
        ROE.Frame.base_free(null);
    }

    function _isBusy() {
        return ROE.Frame.base_isBusy();
    }

    function _reloadTheWindow() {
        window.location.reload();
    }

    var _purchase = function (productID) {
        BDA.Console.verbose('PFP', 'in ROE.Frame.purchase' + productID);
        ROE.Frame.busy("Working. Please wait for a confirmation popup", 7000);

        // this is like this on purpose ... avoiding eval()
        ROE.Device.purchase(productID, ROE.Frame.reloadFrame, 'ROE.Frame.reloadFrame()');

        setTimeout(function () {
            ROE.Frame.free();
        }, 5000);
    };

    var _rateApp = function () {
        ROE.Device.rateApp();
    };

    var tim;

    var _swipe = function (direction) {
        /// <summary></summary>
        /// <param name="direction" type="string">left | right</param>

        //disable swipe for some popups
        if ($('#popup_Research').length || $('#popup_SilverTransport').length) { return; }

        switch (_view) {
            case Enum.Views.vov:
                if (ROE.Frame.IsPopupOpened()) {
                    if ($('#popup_Building2').length > 0) { //building popup swipe handling                       
                        ROE.UI.Sounds.clickActionSwipe();
                        _showNextVOV(direction === 'left' ? 'next' : 'prev', ROE.Building2.reInitContentWithDifferentVillage, 'Switching to');
                    } else if ($('#quickBuildDialog').dialog('isOpen')) { //quickbuild popup swipe handling
                        ROE.UI.Sounds.clickActionSwipe();
                        _showNextVOV(direction === 'left' ? 'next' : 'prev', ROE.QuickBuild.reInitContentWithDifferentVillage, 'Switching to');
                    } else if ($('#quickRecruitDialog').dialog('isOpen') ) { //quickrecruit popup swipe handling
                        ROE.UI.Sounds.clickActionSwipe();
                        _showNextVOV(direction === 'left' ? 'next' : 'prev', ROE.QuickRecruit.reInitContentWithDifferentVillage, 'Switching to');
                    } else {
                        //do nothing
                    }
                } else {
                    ROE.UI.Sounds.clickActionSwipe();
                    _showNextVOV(direction === 'left' ? 'next' : 'prev');
                }
                break;
            case Enum.Views.map:
                break;
        }
    };

    // swipeleft app gesture
    var _swipeLeft = function () {
        if (!_isBusy()) {
            _swipe('left');
        }
    };

    // swiperight app gesture
    var _swipeRight = function () {
        if (!_isBusy()) {
            _swipe('right');
        }
    };

    // swipedown app gesture
    var _swipeUp = function () {
        if (!_isBusy()) {
            switch (_view) {
                case Enum.Views.map:
                    break;
                case Enum.Views.research:
                    break;
            }
        }
    };

    // swipedown app gesture
    var _swipeDown = function () {
        if (!_isBusy()) {
            switch (_view) {
                case Enum.Views.map:
                    break;
                case Enum.Views.research:
                    break;
            }
        }
    };

    // refresh called from app
    var _refresh = function () {
        if (!ROE.Frame.IsPopupOpened()) {
            _reloadFrame();
        }
    };

    // doubletap app gesture
    var _doubleTap = function () {
        //alert('doubleTap');
        switch (_view) {
            case Enum.Views.vov:
                break;
            case Enum.Views.map:
                break;
            case Enum.Views.research:
                break;
        }
    };

    // pinchzoom app gesture
    var _pinchZoom = function (action) {
        // zoom is +
        // pinch is -;
        // alert('pinchZoom:' + action);
    };

    // android back button called from app
    var _backButton = function () {
        //alert('backbutton');
    };

    var _IsPopupOpened = function () {
        /// <summary>tells you if some popup is up - like a building popup, premium features, village list etc etc</summary>       
        return $(".iFrameDiv:visible").length !== 0 || $("#genericDialog:visible").length !== 0 
        || $(".ui-dialog:visible").length !== 0;
    };

    /*The following two functions address a Android 4.4 keyoard-hiding-inputs issue */
    function _inputFocused() {
        if (ROE.Browser.androidApiLevel >= 19) {
            $('.paddinator2000').css({ 'height': '250px' });
        }
    }

    function _inputBlured() {
        if (ROE.Browser.androidApiLevel >= 19) {
            $('.paddinator2000').css({ 'height': '0px' });
        }
    }

    function _someInOutgoingCountdownFinished() {
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
        // <summary>handle the event that tells that that list of incoming or outgoing has changed</summary>       

        var incomingToEmpire;
        //var outgoingToEmpire;
        var incomingDOM = $(CONST.Selector.incomingAlert_Empire);
        //var outgoingDOM = $(CONST.Selector.outgoingAlert_Empire);

        if (direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
            incomingToEmpire = ROE.Troops.InOut2.getIncomingWarning();
            if (incomingToEmpire.count > 0) {
                incomingDOM.html('%count% in <span refresh="false" class=countdown>%timeleft.h%:%timeleft.m%:%timeleft.s%</span>'.format({ count: incomingToEmpire.count, timeleft: BDA.Utils.timeLeft(incomingToEmpire.earliestLandingTime) }));
                incomingDOM.addClass('active');
                incomingDOM.parent().addClass('incomingActive');


                $("footer .footernav .inOutTroops").addClass('incoming');

                $("#lblCountdown")
                    .attr("data-finishesOn", incomingToEmpire.earliestLandingTime)
                    .addClass('countdown');
                $("footer .footernav .inOutTroops .incomingCount").text("[%num%]".format({ num: incomingToEmpire.count }));

            } else {
                $("footer .footernav .inOutTroops").removeClass('incoming');
                $("#lblCountdown").text('').removeClass('countdown');
                $("footer .footernav .inOutTroops .incomingCount").text('');
            }




        } else if (direction == ROE.Troops.InOut2.Enum.Directions.outgoing) {
            //var outgoingToEmpire = ROE.Troops.InOut2.getOutgoingWarning();
            //if (outgoingToEmpire.count > 0) {
            //    outgoingDOM.html('%count% outgoing'.format(outgoingToEmpire));
            //    outgoingDOM.addClass('active');
            //    initTimers();
            //} else {
            //    outgoingDOM.html('');
            //    outgoingDOM.removeClass('active');
            //}
        }

        _updateInOutgoingForSelectedVillage(direction);
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

        ROE.Frame.busy("Starting the windmill...", 20000);

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
        ROE.Frame.reloadView();
        $(".dailyReward").click(function () { ROE.DailyReward.showPopup(); });

        $(".vov-inout-alert.inc").click(function () {
            _busy();
            ROE.Villages.getVillage(ROE.SVID,
                function vovInAlertClick(village) {
                    var filter = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, new ROE.Class.Village(village.id, village.name, village.x, village.y));
                    ROE.Frame.popupInOut('in', filter);
                    _free();
                })
        });
        $(".vov-inout-alert.out").click(function () {
            _busy();
            ROE.Villages.getVillage(ROE.SVID,
                function vovInAlertClick(village) {
                    var filter = new ROE.Troops.InOutWidget.toFromFilter(new ROE.Class.Village(village.id, village.name, village.x, village.y));
                    ROE.Frame.popupInOut('out', filter);
                    _free();
                })
        });

        _setupDialogs();
        ROE.Clock.init();

        BDA.Broadcast.subscribe($('html'), "CurrentlySelectedVillageChanged", _handleCurrentlySelectedVillageChangedEvent);
        BDA.Broadcast.subscribe($('html'), "VillageExtendedInfoUpdated", _handleVillageExtendedInfoUpdatedOrPopulated);
        BDA.Broadcast.subscribe($("body"), "InOutgoingDataChanged", _InOutgoingChanged);

        setTimeout(_timedRefreshFrame, ROE.CONST.headerRefreshRate);
        ROE.Frame.free();
        ROE.Player.refresh();

        if (ROE.Player.numberOfVillages > 1) {
            $('#linkMassActions').show().click(function () {
                ROE.MassActions.showPopup();
            });
        } else {
            $('#linkMassActions').hide();
        }

        //NEW CHAT
        $('body').append('<div id="chatBarContainer" class="gameChatBarContainer"></div>');
        ROE.Chat2.startHub(ROE.realmID, ROE.playerID, $('#chatBarContainer'));

        if ((navigator.userAgent.match(/iPhone/i)) || (navigator.userAgent.match(/iPod/i))) {
            //if iPhone then dont do the chat input movement, as the app already squeeshes screen
            //it became apparent that the squish was causing new issues, so we do the following -farhad
      
            $("body").delegate(".chatWindow .chatInput", "focus", function () {
                setTimeout(function () {
                    $('#chatContainer').css({
                        top: 'auto',
                        height: window.innerHeight
                    });
                }, 100);                
            });
            $("body").delegate(".chatWindow .chatInput", "blur", function () {
                //the timeout allows interaction with child elements before blur
                setTimeout(function () {
                    $('#chatContainer').css({
                        top: '0px',
                        height: 'auto'
                    });
                }, 100);
            });
        } else {
            //android keyboard overlays the input, so we need to move it up
            $("body").delegate(".chatWindow .chatInput", "focus", function () {
                var chatWindowMessages = $(this).parents('.chatWindow').addClass('inputFocused').find('.messages');
                chatWindowMessages.scrollTop(chatWindowMessages[0].scrollHeight);
            });
            $("body").delegate(".chatWindow .chatInput", "blur", function () {
                var chatInput = $(this);
                //the timeout allows interaction with child elements before blur
                setTimeout(function () {
                    chatInput.parents('.chatWindow').removeClass('inputFocused');
                }, 100);
            });
        }

        ROE.Tutorial.startIfNotAlreadyRan(1, 'mainMobile');
        ROE.Utils.getServerTimeOffset(); //gets the server to local time difference and populates ROE.timeOffset with it.
    }




    function _handleCurrentlySelectedVillageChangedEvent() {
        if (_view == Enum.Views.vov) {
            BDA.Console.verbose('ROE.Frame', 'CSV changed to :%vid%'.format({ vid: ROE.SVID }));
            _busy();
            ROE.Villages.getVillage(ROE.SVID, _showThisVOV2
                , ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);

        }
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
            $.each(r.Roles, function (index, value) {
                if (value.RoleID == 0 || value.RoleID == 3)
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
        msg += "Sincerely,\n" + ROE.Player.name
        // TODO: Consider adding a link to the ClanInvitations section to make easier for the person
        // receiving the email. Maybe even pre-populate the invite input box when you click on the link.

        _getClanLeaderNamesList(clanId, function onGetClanLeadersSuccess(leadersList) {
            console.log(leadersList);
            ROE.Frame.popupMailPrefilled(leadersList, subject, msg)
        });
    }



    var _setupDialogs = function _setupDialogs() {
        var maxH = $(window).height();
        var maxW = $(window).width();

        $('#genericDialog').dialog({
            autoOpen: false,
            modal: true,
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });


        $('#quickCommandDialog').dialog({
            autoOpen: false,
            modal: false,
            title: 'Quick Command',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
                $(this).parent().find('.ui-dialog-titlebar').append($('<div>').addClass('pinButton').click(ROE.QuickCommand.handlePinButtonClick));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
                ROE.QuickCommand.paint();
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });

        $('#villageListDialog').dialog({
            autoOpen: false,
            title: 'Village List',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        /*$('#statsDialog').dialog({
            autoOpen: false,
            title: 'Stats',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });*/

        $('#accountDialog').dialog({
            autoOpen: false,
            title: 'Account Security',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        $('#items2Dialog').dialog({
            autoOpen: false,
            title: 'Rewards',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        $('#quickBuildDialog').dialog({
            autoOpen: false,
            title: 'Quick Build',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        $('#quickRecruitDialog').dialog({
            autoOpen: false,
            title: 'Quick Recruit',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        $('#transportSilverDialog').dialog({
            autoOpen: false,
            title: 'Transport Silver',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        $('#supportLookupDialog').dialog({
            autoOpen: false,
            title: 'Support Lookup',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        $('#giftsDialog').dialog({
            autoOpen: false,
            title: 'Bazaar',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        $('#targetsDialog').dialog({
            autoOpen: false,
            title: 'Targets',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

        $('#presetsDialog').dialog({
            autoOpen: false,
            title: 'Combat Presets',
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                $(this).empty();
            }
        });

    }

    //use to add common onCreate functionality to all dialogs at once
    function _addDialogStylingElements(dialog) {
        dialog.parent().find('.ui-dialog-titlebar-close').addClass('sfxMenuExit');
    }

    //use as to add onOpen functionality to all dialogs at once
    function _commonDialogOpenFunction(dialog) {

        //the following is trying to tackle the android 5 flicker issue -farhad Feb 18 2015
        //every dialog that opens, tracks and hides popups that are open underneath, and restores them on close
        var thingsHiddenIds = [];
        $('.iFrameDiv').each(function () {
            var popup = $(this);
            if (popup.is(":visible")) {
                //(&& !popup.hasClass('hiddenByDialog'))
                //popup.addClass('hiddenByDialog'); //hiding by css class isnt solving the android flicker issue
                popup.hide(); //on raw inline hiding seems to be working...
                thingsHiddenIds.push(popup.attr('id')); //keep track of ID for later onClose restoration
            }
        });
        dialog.data('thingsHiddenIds', thingsHiddenIds);
    }

    //use as to add onClose functionality to all dialogs at once
    function _commonDialogCloseFunction(dialog) {
        //restore popups closed by this dialog only (was for android flicker issue)
        var thingsHiddenIds = dialog.data('thingsHiddenIds');
        for (var i = 0; i < thingsHiddenIds.length; i++) {
            $('#' + thingsHiddenIds[i]).show();
        }
    }

    //create, and open a disposable dialog
    //it checks if exists or already open, when opening
    //on close, the dialog is destroyed, conserves memory and cleans DOM
    ///
    ///settings.ID - sets dialog ID, default 'Generic'
    ///settings.title - sets dialog title, default ''
    ///settings.content - sets dialog content, default ''
    ///settings.width - //unlike D2 this is overriden by system
    ///settings.height - //unlike D2 this is overriden by system
    ///settings.modal - //unlike D2 all dialogs on M are modal
    ///settings.contentCustomClass - gives dialog's content a custom class

    function _popDisposable(settings) {

        settings.ID = settings.ID || 'genericDialog';
        settings.title = settings.title || '';
        settings.content = settings.content || '<div></div>';
        settings.contentCustomClass = settings.contentCustomClass || '';

        var dialog = $('#' + settings.ID).empty();

        if (dialog.length < 1) {
            dialog = $('<div id="' + settings.ID + '" class="popupDialogs"></div>').appendTo('body');
        }

        var maxH = $(window).height();
        var maxW = $(window).width();

        dialog.dialog({
            title: settings.title,
            autoOpen: false,
            modal: true,
            resizable: false,
            draggable: false,
            position: [0, 0],
            width: maxW,
            height: maxH,
            create: function () {
                _addDialogStylingElements($(this));
            },
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                dialog.dialog('destroy');
                dialog.remove();
            }
        });

        dialog.dialog('open');

        var dialogContent = $(settings.content).addClass(settings.contentCustomClass).appendTo(dialog);

    }

    //populate and open the reusable dialog
    function _popGeneric(title, content/*, w, h*/) {
        /* DEPRECATED 
        //h / w -- dialogs will always open fullscreen on M -farhad
        var genericDialog = $('#genericDialog').empty();
        genericDialog.append($(content).addClass('genericDialogContent'));
        genericDialog.dialog("option", "title", title)
        genericDialog.dialog('open');
        */

        _popDisposable({
            ID: 'genericDialog',
            title: title,
            content: content,
            contentCustomClass: 'genericDialogContent'
        });

    }

    function _showIframeOpenDialog(dialogSelector, frameSrc, dialogTitle) {
        ROE.Frame.base_showIframeOpenDialog(dialogSelector, frameSrc, dialogTitle);
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

        ROE.Credits.showPopup();

    }

    //
    // PUBLIC INTERFACE
    //
    obj.isPopupOpen = _isPopupOpen;
    obj.reloadFrame = _reloadFrame;
    obj.showVillList = _showVillList;
    obj.showServantsPopup = _showServantsPopup;
    obj.showTroopPopup_toggleTabs = _showTroopPopup_toggleTabs;
    obj.showTroopPopup = _showTroopPopup;
    obj.showResPopup = _showResPopup;
    obj.showBuildPopup = _showBuildPopup;
    obj.showSilverTransportPopup = _showSilverTransportPopup;
    obj.switchToVoV = _switchToVoV;
    obj.fnTogleView = _fnTogleView;
    obj.switchToView = _switchToView;
    obj.switchToMap = _switchToMap;
    obj.reloadView = _reloadView;
    obj.popupAccount = _popupAccount;
    //obj.popupStats = _popupStats;
    obj.popupMail = _popupMail;
    obj.popupMailPrefilled = _popupMailPrefilled;
    obj.popupReports = _popupReports;
    obj.popupPlayerProfile = _popupPlayerProfile;
    obj.popupVillageProfile = _popupVillageProfile;
    obj.popupHighlight = _popupHighlight;
    obj.popupMapSummary = _popupMapSummary;
    obj.popupAttacks = _popupAttacks;
    obj.popupSupport = _popupSupport;
    obj.popupVillageNote = _popupVillageNote;
    obj.popupCommandTroops = _popupCommandTroops;
    obj.popupGifts = _popupGifts;
    obj.popupInOut = _popupInOut;
    obj.toggleOptionsPopup = _toggleOptionsPopup;
    obj.enableView = _enableView;
    obj.busy = _busy;
    obj.busyFail = ROE.Frame.base_fail;
    obj.free = _free;
    obj.isBusy = _isBusy;
    obj.reloadTheWindow = _reloadTheWindow;
    obj.purchase = _purchase;
    obj.rateApp = _rateApp;
    obj.swipeLeft = _swipeLeft;
    obj.swipeRight = _swipeRight;
    obj.swipeUp = _swipeUp;
    obj.swipeDown = _swipeDown;
    obj.refresh = _refresh;
    obj.doubleTap = _doubleTap;
    obj.pinchZoom = _pinchZoom;
    obj.backButton = _backButton;
    obj.popupInfo = _popupInfo;
    obj.popupInfoClose = _popupInfoClose;
    obj.infoBar = _infoBar;
    obj.errorBar = _errorBar;
    obj.infoBarClose = _infoBarClose;
    obj.settingsPopup = _settingsPopup;
    obj.popupQuests = _popupQuests;
    obj.CurrentView = function _currentView() { return _view; };
    obj.popupClan = _popupClan;
    obj.IsPopupOpened = _IsPopupOpened;
    obj.popupQuickCommand = _popupQuickCommand;
    obj.inputFocused = _inputFocused;
    obj.inputBlured = _inputBlured;
    obj.init = _init;
    obj.someInOutgoingCountdownFinished = _someInOutgoingCountdownFinished;
    obj.launchClanInviteRequestMessage = _launchClanInviteRequestMessage;
    obj.showBuyCredits = _showBuyCredits;
    obj.refreshPFHeader = _refreshPFHeader;
    obj.mapNotifWhy = _mapNotifWhy;
    obj.popGeneric = _popGeneric;
    obj.popDisposable = _popDisposable;
    obj.showIframeOpenDialog = _showIframeOpenDialog;
    obj.toggleChatOnMapView = _toggleChatOnMapView;
    obj.spawnFlyRewardIcon = _spawnFlyRewardIcon;
    obj.questRewardAcceptCB = _questRewardAcceptCB;
    obj.refreshQuestCount = _refreshQuestCount;

}(window.ROE.Frame = window.ROE.Frame || {}));
