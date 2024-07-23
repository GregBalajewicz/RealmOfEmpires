/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui-scrolling.js"/>
/// <reference path="bda-ui-transition.js"/>
/// <reference path="roe-api.js"/>
/// <reference path="roe-player.js" />
/// <reference path="countdown.js"/>
/// <reference path="roe-vov.js" />
/// <reference path="roe-ui.sounds.js" />
(function (ROE) {
} (window.ROE = window.ROE || {}));

(function (obj) {
    BDA.Console.setCategoryDefaultView('ROE.QB', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file
    var Timer = window.BDA.Timer,
        Scrolling = window.BDA.UI.Scrolling,
        Transition = window.BDA.UI.Transition,
        Utils = window.BDA.Utils;
    
    var Api = window.ROE.Api,
        Entities = window.ROE.Entities,
        Frame = window.ROE.Frame,
        Player = window.ROE.Player;

    var CONST = { popupName: "QuickBuild" },
        CACHE = {};
    
    var storedScroll = 0;
    var _UIDisplayMode; // "Standard" || "Compact"
    var pageViewMode = "upgrade"; // this is the view mode, can be: "upgrade" | "downgrade"
    var pageActivity = "idle"; //this is the village activity, can be: "idle" | "upgrade" | "downgrade"
    var firstEventId = "0"; //this is the first working event, be it upgrade or downgrade
    var _villageIDPassedFromMap;
    var isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = false;

    CONST.Selector = {      
        upgDowngContainer: '.themeM-view .upgDowngContainer',
        upgradeArea: '.themeM-view .upgradeArea',
        downgradeArea: '.themeM-view .downgradeArea',
        resourceFooter: ".resourceFooter.footer1",
        cannotUpgradePanel: '.cannotUpgradePanel',
        currentBuildTimeTimer: '.currentBuildTimeTimer',
        totalBuildTimeTimer: '.totalBuildTimeTimer',
        switchModeArea: '.switchModeArea',
        switchModeBtn: '.switchModeArea .switchModeBtn',
        switchModeToUpgrade: '.switchModeArea .switchModeBtn.toUpgrade',
        switchModeToUpgrade: '.switchModeArea .switchModeBtn.toDowngrade',
        qUpgradeSummaryArea: '.qUpgradeSummaryArea',
        qUpgradeSlideToQueue: '.qUpgradeSummaryArea .slideToQueuePage',
        qDowngradeSummaryArea: '.qDowngradeSummaryArea',
        qDowngradeSlideToQueue: '.qDowngradeSummaryArea .slideToQueuePage',
        templates: {
            buildingRow: '.upgradeArea .buildingRow.template',
            downgradeRow: '.downgradeArea .buildingRow.template',
            queueRow: '.queuedItemRow.template'
        },
        buildingRow: {
            row: '.buildingRow',
            name: '.buildingName',
            requirements: '.buildingRequirements'
        },
        Q: {
            queuedItemsArea: '.queuedItemsArea',
            bigQContainer: '.queueContainer',
            queuedItemRow: '.queuedItemRow',
            qTime: '.qTime',
            cancelAll:'.cancelAll'
        },
        upgradePad: {
            header: '.upgradeNumpadHeader',
            holder: '.upgradeNumpadHolder',
            template: '.upgradeNumpadTemplate',
            item: '.upgradeItemNum',
            headerBuildingIcon: '.buildingIcon',
            headerBuildingName: '.buildingName',
            headerBuildingLevel: '.buildingLevel',
            box: '.upgradeNumpadBox',
            buildingUpgradeText: '.buildingUpgradeText'
        },
        UI: {
            smallArrowDown: '.smallArrowDown',
            smallArrowUp: '.smallArrowUp'
        },
        SpeedUps: {
            openSpeedUpPanel:'.buildTimeArea .speedUpButton'
        }
    };



    CONST.CssClass = {
        countdown: "countdown",
        expandedRow: 'expanded',
        statusClasses: {
            statusLack: "statusLack",
            statusLackSilver: "statusLack statusLackSilver",
            statusLackFood: "statusLack statusLackFood",
            statusUnsatisfied: "statusUnsatisfied",
            statusMaxed: "statusMaxed",
            statusBuild: "statusBuild",
            canDowngrade: "canDowngrade",
            cantDowngrade: "cantDowngrade"
        },
        UI: {
            smallArrowDown: 'smallArrowDown',
            smallArrowUp: 'smallArrowUp',
            smallRoundButtonLight: 'smallRoundButtonLight'
        },
        requirementItem: 'requirementItem',
        reqNotMet:'reqNotMet'
    };

    CACHE.Selector = { };
    var DATA = {};
    var _templates = {};
    var _villageID;

    var _showQuickBuildPopup = function (launchedFrom, selectedVID) {
        storedScroll = 0;
        pageViewMode = "upgrade";
        _UIDisplayMode = ROE.LocalServerStorage.get('UIDisplayMode') || 'Standard';
        if (ROE.isMobile) { _UIDisplayMode = 'Standard'; } //force standard template for M
        ROE.QuickBuild.changesMade_RequireVOVReload = false;
        ROE.QuickBuild.launchedFrom = launchedFrom;     //if launchedFrom is == "vov" in M it reloads VOV upon close when changes are made 
        if (selectedVID) {
            _villageIDPassedFromMap = selectedVID;
        } else {
            _villageIDPassedFromMap = null;
        }
        _load();
    }

    function _load() {        

        if (!CACHE.template) {
            Frame.busy('Loading Blueprints...', 5000, $('#quickBuildDialog'));
            var temp;
            if (ROE.isMobile || _UIDisplayMode == 'Standard') {
                temp = BDA.Templates.getRawJQObj("QuickBuildTempl", ROE.realmID);
            } else {
                temp = BDA.Templates.getRawJQObj("QuickBuildTempl_d2", ROE.realmID);
            }
            Frame.free($('#quickBuildDialog'));
            CACHE['template'] = temp;
        }

        CONST.Selector.defaultPage = "#quickBuildDialog .themeM-view.default";
        CONST.Selector.queuePage = "#quickBuildDialog .themeM-view.queuepage";

        _templates["main"] = CACHE['template'].clone();
        _templates["buildingRow"] = _templates["main"].find(CONST.Selector.templates.buildingRow);
        _templates["downgradeRow"] = _templates["main"].find(CONST.Selector.templates.downgradeRow);
        _templates["queueRow"] = _templates["main"].find(CONST.Selector.templates.queueRow);
        
        $('#quickBuildDialog').dialog('open');
        $('.speedUpOptionsPopup.quickbuild').remove();
        _sync();
    }      


    function _sync(optionalVillage) {
        Frame.busy(_randomPhrase('MainLoadingMsg'), 10000, $('#quickBuildDialog'));
        ROE.vov.pauseAnimation();
        if (_villageIDPassedFromMap) {
            _villageID = _villageIDPassedFromMap;
        } else {
            _villageID = ROE.SVID;
        }
        ROE.QuickBuild.villageID = _villageID;
        if (optionalVillage) {
            _sync_onDataSuccess(optionalVillage);
        } else {
            ROE.Villages.getVillage(_villageID, _sync_onDataSuccess, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
        }
    }

    function _syncForSelectedVillage(selectedVillageID) {
        Frame.busy(_randomPhrase('MainLoadingMsg'), 10000, $('#quickBuildDialog'));
        $('.speedUpOptionsPopup.quickbuild').remove();
        _villageID = ROE.QuickBuild.villageID = selectedVillageID;
        ROE.Villages.getVillage(_villageID, _sync_onDataSuccess, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
    }

    var _village;
    function _sync_onDataSuccess(village) {
        Frame.free($('#quickBuildDialog'));
        _village = village;
        DATA = village.upgrade;
        _populate();
        //ROE.Frame.refreshQuestCount();
    }

    function _sync_onDataFailure(data) {}

    function _switchMode(switching) {
        if (switching == "toUpgrade") {
            if (pageViewMode == "upgrade") {
                return;
            }
            _setViewUpgrade();
        } else if (switching == "toDowngrade") {
            if (pageViewMode == "downgrade") {
                return;
            }
            _setViewDowngrade();
        }
        $(CONST.Selector.upgDowngContainer).scrollTop(2000);
        ROE.UI.Sounds.click();
    }

    function _setViewUpgrade() {
        pageViewMode = "upgrade";
        $(CONST.Selector.switchModeArea).removeClass('downgradeActive').addClass('upgradeActive');
        $(CONST.Selector.qDowngradeSummaryArea).hide();
        $(CONST.Selector.downgradeArea).hide();
        $(CONST.Selector.qUpgradeSummaryArea).show();
        $(CONST.Selector.upgradeArea).show();
        $('#popup_QuickBuild .IFrameDivTitle .label').text('Quick Build');
    }

    function _setViewDowngrade() {
        pageViewMode = "downgrade";
        $(CONST.Selector.switchModeArea).removeClass('upgradeActive').addClass('downgradeActive');
        $(CONST.Selector.qUpgradeSummaryArea).hide();
        $(CONST.Selector.upgradeArea).hide();
        $(CONST.Selector.qDowngradeSummaryArea).show();
        $(CONST.Selector.downgradeArea).show();
        $('#popup_QuickBuild .IFrameDivTitle .label').text('Downgrade');
    }

    function _populate() {
        //console.log('_populate DATA: ',DATA);
        var popupContent = $('#quickBuildDialog');
        BDA.Broadcast.subscribe(popupContent, "CurrentlySelectedVillageChanged", _handleCurrentlySelectedVillageChangedEvent);
        BDA.Broadcast.subscribe(popupContent, "VillageExtendedInfoUpdated", _handleVillageExtendedInfoUpdatedOrPopulated);

        var content = _templates.main.clone();
        var queueElement = '<div class="slideToQueuePage BtnDSm2n fontSilverFrSClrg grayout" onclick="ROE.QuickBuild.slideToQueuePage($(this));">' + content.find('#QuickBuildPhrases').find('div[ph=\'Queue\']').html() + '<span class="smallArrowRight"></span></div>';

        var qUpgradeSummaryArea = content.find(CONST.Selector.qUpgradeSummaryArea);
        var qUpgradeItemsArea = qUpgradeSummaryArea.find(CONST.Selector.Q.queuedItemsArea).empty();
        var qDowngradeSummaryArea = content.find(CONST.Selector.qDowngradeSummaryArea);
        var qDowngradeItemsArea = qDowngradeSummaryArea.find(CONST.Selector.Q.queuedItemsArea).empty();
        var qContainer = content.find(CONST.Selector.Q.bigQContainer).empty();
        pageActivity = "idle"; 

        if (_UIDisplayMode == 'Compact') {
            var switchModeElement = content.find('.switchModeArea');
            switchModeElement.parent().after(switchModeElement);
        }

        //SETUP UPGRADE Q
        if (DATA.Q.UpgradeEvents.length > 0) {
            pageActivity = "upgrade";
                      
            //only if its not completed do we setup the following timers, prevents timer craziness
            var completionTime = DATA.Q.UpgradeEvents[0].completionTime;
            if(completionTime > (new Date()).getTime()){
                qUpgradeSummaryArea.find(CONST.Selector.currentBuildTimeTimer).attr('data-finisheson', completionTime).addClass(CONST.CssClass.countdown);
                qUpgradeSummaryArea.find(CONST.Selector.totalBuildTimeTimer).attr('data-finisheson', DATA.Q.finishesOn).addClass(CONST.CssClass.countdown);
            }
                      
            var qiIconUrl, qiName, qiToLevel, qiCompletion, qiDuration, qiTime, qiEventId, qiIsInQ,
                qiBuildingId, queueItemRowData, aNewQueueRow;
            for (var e = 0; e < DATA.Q.UpgradeEvents.length; e++) {
                qiIconUrl = Entities.BuildingTypes[DATA.Q.UpgradeEvents[e].bid].IconUrl_ThemeM;
                qiName = Entities.BuildingTypes[DATA.Q.UpgradeEvents[e].bid].Name;
                qiToLevel = DATA.Q.UpgradeEvents[e].upgradeToLevel;      
                qiCompletion = Utils.timeLeft(DATA.Q.UpgradeEvents[e].completionTime);
                qiDuration = ROE.Utils.msToTime(DATA.Q.UpgradeEvents[e].upgradeDuration);
                qiTime = (e == 0 ? qiCompletion.h + ":" + qiCompletion.m + ":" + qiCompletion.s : qiDuration);
                qiEventId = DATA.Q.UpgradeEvents[e].ID;
                qiIsInQ = DATA.Q.UpgradeEvents[e].qEntryID != null ? "isQ" : "notQ";
                qiBuildingId = DATA.Q.UpgradeEvents[e].bid;
                queueItemRowData = {
                    qLvl: qiToLevel,
                    qIcon: qiIconUrl,
                    qName: qiName,
                    qTime: qiTime,
                    qEventId: qiEventId,
                    qIsInQ: qiIsInQ,
                    qBID: qiBuildingId,
                    qType: "upgrade",
                    actionColumn: "Building"
                }
                aNewQueueRow = $(BDA.Templates.populate(_templates.queueRow[0].outerHTML, queueItemRowData)).removeClass('template');
                if (e == 0) {
                    //only countdown the first item in the queue page
                    aNewQueueRow.find(CONST.Selector.Q.qTime).addClass(CONST.CssClass.countdown);
                    //add a speedup option for first row only
                    aNewQueueRow.append('<div class="qSpeedUp upgrade smallRoundButtonDark" onclick="ROE.QuickBuild.showSpeedUpOptions($(this));"><span></span></div>');
                    firstEventId = qiEventId;
                }
                qContainer.append(aNewQueueRow);

                if ((_UIDisplayMode == "Compact" && e < 8) || e < 3) { //only show top 3 queues in the main page summary area for mobile or up to 8 queues in web
                    qUpgradeItemsArea.append('<span class="queuedItem fontWhiteNumbers" style="background-image:url(\'' + qiIconUrl + '\');">(' + qiToLevel + ')</span>');
                }
            }

            if (_UIDisplayMode == "Compact") { //add the queue button into the upgradeitems area for web
                if (DATA.Q.UpgradeEvents.length > 8) {
                    qUpgradeItemsArea.append('<span class="queuedMore");">...</span>');
                }
                qUpgradeItemsArea.append(queueElement);
            } else if (DATA.Q.UpgradeEvents.length > 3) { //add a ... in q preview if more than 3 items in Q
                qUpgradeItemsArea.append('<span class="queuedMore");">...</span>');
                
            }
            qUpgradeSummaryArea.find(CONST.Selector.SpeedUps.openSpeedUpPanel).removeClass('grayout');
            qDowngradeItemsArea.prepend('<span class="fontGoldFrLClrg" style="position: relative;top: 9px; float: left;">Upgrade In Progress</span>');
            content.find(CONST.Selector.qUpgradeSlideToQueue).removeClass('grayout');          
            content.find('.queueWrapper .queueHeader .qName').html('Building');
        } else {
            if (_UIDisplayMode == "Compact") { //add the queue button into the upgradeitems area for web
                qUpgradeItemsArea.append(queueElement);
            }
            qUpgradeSummaryArea.find(CONST.Selector.SpeedUps.openSpeedUpPanel).addClass('grayout');
            content.find(CONST.Selector.qUpgradeSlideToQueue).addClass('grayout');
            qUpgradeSummaryArea.find(CONST.Selector.currentBuildTimeTimer).removeAttr('data-finisheson').removeClass('countdown').text("00:00:00");
            qUpgradeSummaryArea.find(CONST.Selector.totalBuildTimeTimer).removeAttr('data-finisheson').removeClass('countdown').text("00:00:00");
        }

        //SETUP DOWNGRADE Q
        var buildingStateInfo = {};
        if ((DATA.DowngradeQ.length > 0)) {
            pageActivity = "downgrade";
            //var t = Utils.timeLeft(DATA.DowngradeQ[0].completionTime);
            qDowngradeSummaryArea.find(CONST.Selector.currentBuildTimeTimer).attr('data-finisheson', DATA.DowngradeQ[0].completionTime).addClass(CONST.CssClass.countdown);

            var qiIconUrl, qiName, qiToLevel, qiCompletion, qiDuration, qiTime, qiEventId, qiIsInQ,
                qiBuildingId, queueItemRowData, aNewQueueRow;

            ///Due to lacking Downgrade API info, this object will assist in tracking building levels in Q
            for (var b = 0; b < DATA.Buildings.length; b++) {
                buildingStateInfo[DATA.Buildings[b].buildingID] = { levelState: DATA.Buildings[b].curLevel }
            }

            for (var e = 0; e < DATA.DowngradeQ.length; e++) {
                qiBuildingId = DATA.DowngradeQ[e].buildingID;
                qiIconUrl = Entities.BuildingTypes[qiBuildingId].IconUrl_ThemeM;
                qiName = Entities.BuildingTypes[qiBuildingId].Name;
                qiDuration = "00:00:00"; //missing API information
                qiCompletion = Utils.timeLeft(DATA.DowngradeQ[e].completionTime);
                qiTime = (e == 0 ? qiCompletion.h + ":" + qiCompletion.m + ":" + qiCompletion.s : "Waiting...");
                qiEventId = DATA.DowngradeQ[e].eventID != null ? DATA.DowngradeQ[e].eventID : DATA.DowngradeQ[e].qEntryID;
                qiIsInQ = DATA.DowngradeQ[e].qEntryID != null ? "isQ" : "notQ";
                qiDownToLevel = --buildingStateInfo[qiBuildingId].levelState;

                queueItemRowData = {
                    qIcon: qiIconUrl,
                    qName: qiName,
                    qTime: qiTime,
                    qEventId: qiEventId,
                    qIsInQ: qiIsInQ,
                    qBID: qiBuildingId,
                    qLvl: qiDownToLevel,
                    qType: "downgrade"
                }
                aNewQueueRow = $(BDA.Templates.populate(_templates.queueRow[0].outerHTML, queueItemRowData)).removeClass('template');
                if (e == 0) {
                    //only countdown the first item in the queue page
                    aNewQueueRow.find(CONST.Selector.Q.qTime).addClass(CONST.CssClass.countdown);
                    //add a speedup option for first row only
                    aNewQueueRow.append('<div class="qSpeedUp downgrade smallRoundButtonDark" onclick="ROE.QuickBuild.showSpeedUpOptions($(this));"><span></span></div>');
                    firstEventId = qiEventId;
                }
                qContainer.append(aNewQueueRow);

                if ((_UIDisplayMode == "Compact" && e < 8) || e < 3) { //only show top 3 queues in the main page summary area for mobile or up to 8 queues for web
                    qDowngradeItemsArea.append('<span class="queuedItem fontWhiteNumbers" style="background-image:url(\'' + qiIconUrl + '\');">(' + qiDownToLevel + ')</span>');
                }
            }

            if (_UIDisplayMode == "Compact") { //add queue button to downgrade items section
                qDowngradeItemsArea.append(queueElement);
            }

            qDowngradeSummaryArea.find(CONST.Selector.SpeedUps.openSpeedUpPanel).removeClass('grayout');
            qUpgradeItemsArea.prepend('<span class="fontGoldFrLClrg" style="position: relative;top: 9px; float: left;">Downgrade In Progress</span>');
            content.find(CONST.Selector.qDowngradeSlideToQueue).removeClass('grayout');
            content.find('.queueWrapper .queueHeader .qName').html('Downgrading'); 
        }
        else {
            if (_UIDisplayMode == "Compact") {
                qDowngradeItemsArea.append(queueElement);
            }
            qDowngradeSummaryArea.find(CONST.Selector.SpeedUps.openSpeedUpPanel).addClass('grayout');
            content.find(CONST.Selector.qDowngradeSlideToQueue).addClass('grayout');
            qDowngradeSummaryArea.find(CONST.Selector.currentBuildTimeTimer).removeAttr('data-finisheson').removeClass('countdown').text("00:00:00");

        }   

        var upgradeArea = content.find(CONST.Selector.upgradeArea).empty();
        if (_UIDisplayMode == "Compact") {
            upgradeArea.append(
                '<div id="upgHeader" class="buildingRow">' +
                    '<div class="buildingsIcon"></div>' +
                    '<div class="buildingName fontGoldFrLClrg"></div>' +
                    '<div class="buildingResources silverResource fontGoldFrLClrg"><span class="bResourceIcon bSilver"></span></div>' +
                    '<div class="buildingResources foodResource fontGoldFrLClrg"><span class="bResourceIcon bFood"></span></div>' +
                    '<div class="buildingResources timeResource fontGoldFrLClrg"><span class="bResourceIcon bTime"></span></div>' +
                    '<div class="upgradeLevel fontGoldFrLClrg">Upgrade to Level</div>' +
                '</div>');
        }

        var downgradeArea = content.find(CONST.Selector.downgradeArea).empty();

        var thisBuilding, buildingID, buildingEntity, buildingName, buildingIcon, curLevel, upToLevel, nextLevel, downStatusObject, canDownCode,
            nextLevel_max, nextLevelSilver, nextLevelFood, nextLevelTime, canUpCode, requirements, statusObject, rowData, aSampleRow, btnMoreStatus;
        
        //setup build/upgrade building rows
        for (var i = 0; i < DATA.Buildings.length; i++) {
            thisBuilding = DATA.Buildings[i];
            buildingID = thisBuilding.buildingID;
            buildingEntity = Entities.BuildingTypes[buildingID];
            buildingName = buildingEntity.Name;
            buildingIcon = buildingEntity.IconUrl_ThemeM;
            curLevel = thisBuilding.curLevel;
            upToLevel = "";
            downToLevel = "";
            if (buildingStateInfo[buildingID] && buildingStateInfo[buildingID].levelState != curLevel) {
                downToLevel = "->"+buildingStateInfo[buildingID].levelState;
            }
            nextLevel = 0;
            if (thisBuilding.Upgrade.nextLevel) {
                nextLevel = thisBuilding.Upgrade.nextLevel.levelNum;
                nextLevelSilver = ROE.Utils.addThousandSeperator(thisBuilding.Upgrade.nextLevel.cost);
                nextLevelFood = ROE.Utils.addThousandSeperator(thisBuilding.Upgrade.nextLevel.food);
                nextLevelTime = ROE.Utils.msToTime(thisBuilding.Upgrade.nextLevel.time);
                if ((nextLevel - 1) > curLevel) { upToLevel = "->" + (nextLevel - 1); }
            }
            nextLevel_max = 0;
            if (thisBuilding.Upgrade.nextLevel_max) {
                nextLevel_max = thisBuilding.Upgrade.nextLevel_max.levelNum;
            }
            if (nextLevel == nextLevel_max) {
                btnMoreStatus = 'grayout';
            } else {
                btnMoreStatus = '';
            }
            canUpCode = thisBuilding.Upgrade.canUpgrade;
            canDownCode = thisBuilding.Upgrade.canDowngrade;
            statusObject = determineStatus(canUpCode, canDownCode);
            requirements = thisBuilding.Upgrade.unsatisfiedRequirementsIfAny;
            
            rowData = {
                upStatus: statusObject.upClass,
                upCode: canUpCode,
                downStatus: statusObject.downClass,
                downCode: canDownCode,
                villageId: _villageID,
                buildingId: buildingID,
                iconUrl: buildingIcon,
                name: buildingName,
                currLevel: curLevel,
                upToLevel: upToLevel,
                nextLevel: nextLevel,
                nextLevelMax: nextLevel_max,
                nextCost: nextLevelSilver,
                nextFood: nextLevelFood,
                nextTime: nextLevelTime,
                btnMoregrayout: btnMoreStatus,
                downToLevel: downToLevel
            };
            
            aSampleRow = $(BDA.Templates.populate(_templates.buildingRow[0].outerHTML, rowData)).removeClass('template');

            var reqData, reqName, reqDiv;
            if ( requirements.length > 0) {
                for (var j = 0; j < requirements.length; j++) {
                    reqData = requirements[j];
                    reqName = Entities.BuildingTypes[reqData.btid].Name;
                    reqDiv = $('<div>').addClass(CONST.CssClass.requirementItem)
                        .addClass(CONST.CssClass.reqNotMet).html(reqName + " lvl " + reqData.level);
                    aSampleRow.find(CONST.Selector.buildingRow.requirements).append(reqDiv);
                }
            }
            
            //Populate extra levels for web only
            if (_UIDisplayMode == "Compact") {
                var upgrades = aSampleRow.find('.upgradeLevel');
                var levelCount = nextLevel + 1;
                var incrementer = 1;
                //algorithm that increases levels upgrade btns display
                for (var count = 1; levelCount < nextLevel_max; count++) {

                    //this will force a max of 8 items shown so it doesnt grow too large (next level, and max level upgrades always shown)
                    if (count > 7) { break; } 

                    //always show first 5 (first one not in this loop, and one loop tick after incrementer incremented)
                    if (count > 3) {
                        //at every 5 divisble, incremenet by 5s
                        if (levelCount % 5 == 0) { incrementer = 5; }
                    }
                    upgrades.append('<a class="fontSilverNumbersLrg" href="javascript:ROE.QuickBuild.doUpgradeBuilding(\'' + buildingID + '\',\'' + levelCount + '\',\'0\');">' + levelCount + '</a>');
                    levelCount += incrementer;
                }
                if (nextLevel != nextLevel_max && nextLevel_max != 0) {
                    upgrades.append('<a class="fontSilverNumbersLrg" href="javascript:ROE.QuickBuild.doUpgradeBuilding(\'' + buildingID + '\',\'' + nextLevel_max + '\',\'0\');">' + nextLevel_max + '</a>');
                }
            }

            upgradeArea.append(aSampleRow);
            
            //setup downgrade buildings in the same loop
            aSampleRow = $(BDA.Templates.populate(_templates.downgradeRow[0].outerHTML, rowData)).removeClass('template');
            downgradeArea.append(aSampleRow);
        }

        content = $(content);
        if (BetaFeatures.status('QB:ShowMaxedBuildings') == 'OFF') {
            content.find('.' + CONST.CssClass.statusClasses.statusMaxed).remove();
        }
        content.find(CONST.Selector.cannotUpgradePanel).remove();

        if (!ROE.isVPRealm) {
            content.find('.speedUpButton, .qSpeedUp').remove();
        }

        popupContent.html(content);
        
        ROE.FooterWidget.init(content.find(CONST.Selector.resourceFooter), _village, "ROE.QuickBuild.reInitContent();");

        if (pageViewMode == "downgrade") {
            _setViewDowngrade();
        } else {
            _setViewUpgrade();
        }

        $(CONST.Selector.upgDowngContainer).scrollTop(storedScroll);
        Timer.initTimers();

    }

    function determineStatus(upCode, downCode) {
        var status = {
            upClass: null,
            downClass: null
        };

        ///canupgrade values
        ///          (7) No_DowngradesInProgress   - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (6) No_UnsatisfiedReq         - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (3) No_BuildingFullyUpgraded  - nextLevel and unsatisfiedRequirementsIfAny are NOT valid
        ///          (1) No_LackFood               - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (2) No_LackSilver             - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (5) No_Busy                   - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (4) No_LockedFeature          - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (0) Yes                       - nextLevel and unsatisfiedRequirementsIfAny are valid
        switch (upCode) {
            case 7:
                status.upClass = CONST.CssClass.statusClasses.statusLack;
                break;
            case 6:
                status.upClass = CONST.CssClass.statusClasses.statusUnsatisfied;
                break;
            case 3:
                status.upClass = CONST.CssClass.statusClasses.statusMaxed;
                break;
            case 1:
                status.upClass = CONST.CssClass.statusClasses.statusLackFood;
                break;
            case 2:
                status.upClass = CONST.CssClass.statusClasses.statusLackSilver;
                break;
            case 5:
                status.upClass = CONST.CssClass.statusClasses.statusLack;
                break;
            case 4:
                status.upClass = CONST.CssClass.statusClasses.statusLack;
                break;
            case 0:
                status.upClass = CONST.CssClass.statusClasses.statusBuild;
                break;
        }

        /// 0: yes
        /// 1: No_UpgradesInProgress
        /// 2: No_NoSuchBuildingInVillage
        /// 3: No_BuildingBeingDowngradedToMinLevel
        /// 4: No_LockedFeature
        /// 5: No_StwardsCannotDoThis
        switch (downCode) {
            case 0:
                status.downClass = CONST.CssClass.statusClasses.canDowngrade;
                break;
            case 1:
                status.downClass = CONST.CssClass.statusClasses.cantDowngrade;
                break;
            case 2:
                status.downClass = CONST.CssClass.statusClasses.cantDowngrade;
                break;
            case 3:
                status.downClass = CONST.CssClass.statusClasses.cantDowngrade;
                break;
            case 4:
                status.downClass = CONST.CssClass.statusClasses.cantDowngrade;
                break;
            case 5:
                status.downClass = CONST.CssClass.statusClasses.cantDowngrade;
                break;
            default:
                status.downClass = CONST.CssClass.statusClasses.cantDowngrade;
                break;
        }
        
        return status;
    }

    function _doCancel(btn) {
        ROE.UI.Sounds.click();
        var eventID = btn.attr('data-eventId');
        var isQ = (btn.attr('data-isq') == "isQ");
        var qType = btn.attr('data-qType');
        Frame.busy('Canceling...', 5000, $('#quickBuildDialog'));
        if (qType == "upgrade") {
            ROE.Api.call_upgrade_cancel2(_villageID, eventID, isQ, _cancelUpgradeReturn);
        } else {
            ROE.Api.call_downgrade_cancel(_villageID, eventID, isQ, _cancelDowngradeReturn);
        }
    }

    function _cancelUpgradeReturn(data) {
        Frame.free($('#quickBuildDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.id) {
            return;
        }

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        ROE.QuickBuild.changesMade_RequireVOVReload = true;
        ROE.Villages.__populateExtendedData(data.id, _village, data);    
        _sync(_village);
        $(CONST.Selector.defaultPage).removeClass('slideLeftTo').addClass('slideLeftFrom');
        $(CONST.Selector.queuePage).removeClass('slideRightFrom').addClass('slideRightTo');
    }

    function _cancelDowngradeReturn(data) {
        Frame.free($('#quickBuildDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.id) {
            return;
        }

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        ROE.QuickBuild.changesMade_RequireVOVReload = true;
        ROE.Villages.__populateExtendedData(data.id, _village, data);
        _sync(_village);
        $(CONST.Selector.defaultPage).removeClass('slideLeftTo').addClass('slideLeftFrom');
        $(CONST.Selector.queuePage).removeClass('slideRightFrom').addClass('slideRightTo');
    }

    function _cancelAll() {
        ROE.UI.Sounds.click();

        var btn = $('#quickbuild_popup .queuepage .cancelAll ');
        if (!btn.hasClass('areyousure')) {
            btn.attr('data-label', btn.html());
            btn.addClass('areyousure').html("Sure?");
            window.setTimeout(function () {
                btn.removeClass('areyousure').html(btn.attr('data-label'));
            }, 2500);
            return;
        }

        var qButtonsReversed = $($('#quickbuild_popup .queuepage .queueContainer .qCancel').get().reverse());
        var qStuffArray = [];
        var currIndex = 0;
  
        qButtonsReversed.each(function () {

            var eventID = $(this).attr('data-eventId');
            var isQ = ($(this).attr('data-isq') == "isQ");
            var qType = $(this).attr('data-qType');

            qStuffArray.push({
                villageID: _villageID,
                eventID: eventID,
                isQ: isQ,
                qType: qType
            });

        });

        _cancelAllStep();

        function _cancelAllStep() {

            if (currIndex >= qStuffArray.length) {
                return;
            }
            
            var aQ = qStuffArray[currIndex];
            Frame.busy('Canceling Queue item ' + (currIndex + 1) + '/' + qStuffArray.length, 5000, $('#quickBuildDialog'));

            if (aQ.qType == "upgrade") {
                ROE.Api.call_upgrade_cancel2(aQ.villageID, aQ.eventID, aQ.isQ, _cancelAllStepReturn);
            } else {
                ROE.Api.call_downgrade_cancel(aQ.villageID, aQ.eventID, aQ.isQ, _cancelAllStepReturn);
            }

        }

        function _cancelAllStepReturn(data) {

            Frame.free($('#quickBuildDialog'));

            //if village changed before call came back, discard data
            if (_villageID != data.Village.id) {             
                return;
            }

            isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
            ROE.QuickBuild.changesMade_RequireVOVReload = true;
            ROE.Villages.__populateExtendedData(data.id, _village, data);

            currIndex += 1;
            if (currIndex < qStuffArray.length) {
                _cancelAllStep(currIndex);
            } else {
                _allCanceled(data);
            }
            
        }

        function _allCanceled(data) {
 
            //if village changed before call came back, discard data
            if (_villageID != data.Village.id) {
                return;
            }

            isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
            ROE.QuickBuild.changesMade_RequireVOVReload = true;
            ROE.Villages.__populateExtendedData(data.id, _village, data);
            _sync(_village);

        }

    }


    function _upgradeNumpad(row) {
        var statusCode = row.attr('data-statusCode');
        if (row.attr('data-statusCode') != "0") {
            showFailPanel("upgrade", statusCode);
            return;
        }
        //if nextlevel is nextlevelmax, dont need upgrade numpad
        if (row.find('.buildingUpgradeBtnMore').hasClass('grayout')) { return; }
        ROE.UI.Sounds.click();
        var buildingId = row.attr('data-buildingId');
        var buildingObj = Entities.BuildingTypes[buildingId];
        var buildingIcon = buildingObj.IconUrl_ThemeM;
        var buildingName = buildingObj.Name;
        var buildingCurLevel = parseInt(row.attr('data-currLevel'));
        var startingNum = parseInt(row.attr('data-nextLevel'));
        var maxUpgradePossibleNow = parseInt(row.attr('data-nextLevelMax'));
        var highestBuildingLevel = parseInt(buildingObj.MaxLevel);
        var numPadTemplate = $(CONST.Selector.upgradePad.template);
        var nHeader = numPadTemplate.find(CONST.Selector.upgradePad.header);
        nHeader.find(CONST.Selector.upgradePad.headerBuildingName).html(buildingName);
        if (buildingCurLevel == 0) {
            nHeader.find(CONST.Selector.upgradePad.headerBuildingLevel).hide();
        } else {
            nHeader.find(CONST.Selector.upgradePad.headerBuildingLevel).html("Level " + buildingCurLevel).show();
        }
        nHeader.find(CONST.Selector.upgradePad.buildingUpgradeText).html("Upgrade<br>To Level");

        var numBox = numPadTemplate.find(CONST.Selector.upgradePad.box).empty();
        var upgItem;
        for (var i = startingNum; i < highestBuildingLevel + 1; i++) {
            upgItem = $('<div>').addClass('upgradeItemNum smallRoundButtonLight fontSilverNumbersLrg').attr('data-upgradeTo', i).html(i);
            if (i > maxUpgradePossibleNow) {
                upgItem.addClass('impossibleUpgrade');
            } else {
                //the reason its done this way is because the .html() later doesnt carry over bindings.
                upgItem.attr('onclick', "ROE.QuickBuild.doUpgradeBuilding(" + buildingId + "," + i + ",'0'); $('.upgradeToNumpadPopup.quickbuild').remove();");
            }
            numBox.append(upgItem);
        }

        ROE.Frame.simplePopopOverlay(buildingIcon, buildingName, numPadTemplate.html(), 'upgradeToNumpadPopup quickbuild', $('#quickBuildDialog').parent());

    }

    function _doUpgradeBuilding(buildingId, levelToUpgradeTo, statusCode) {
        ROE.UI.Sounds.click();
        if (statusCode != "0") {
            showFailPanel("upgrade", statusCode);
            return;
        }
        Frame.busy('Upgrading...', 5000, $('#quickBuildDialog'));
        storedScroll = $(CONST.Selector.upgDowngContainer).scrollTop();
        ROE.Api.call_upgrade_doupgrade2(_villageID, buildingId, levelToUpgradeTo, _doUpgradeBuildingReturn);
    }

    function _doUpgradeBuildingReturn(data) {
        Frame.free($('#quickBuildDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.id) {
            return;
        }
        
        if (data.hasOwnProperty('cannotUpgrade')) {
            showFailPanel("upgrade", data.cannotUpgrade);
            //ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village); //<-- if failed, nothing changed, why do this at all?
        } else {
            ROE.QuickBuild.changesMade_RequireVOVReload = true;
            isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
            ROE.Villages.__populateExtendedData(data.id, _village, data);
            _sync(_village);
        }
        
    }

    function _doDowngradeBuilding(buildingId, statusCode) {
        ROE.UI.Sounds.click();
        if (statusCode != "0") {
            showFailPanel("downgrade", statusCode);
            return;
        }
        storedScroll = $(CONST.Selector.upgDowngContainer).scrollTop();
        Frame.busy('Downgrading...', 5000, $('#quickBuildDialog'));
        ROE.Api.call_downgrade_dodowngrade(_villageID, buildingId, _doDowngradeReturn);
    }

    function _doDowngradeReturn(data) {
        Frame.free($('#quickBuildDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.id) {
            return;
        }

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        ROE.QuickBuild.changesMade_RequireVOVReload = true;
        ROE.Villages.__populateExtendedData(data.id, _village, data);
        _sync(_village);
    }

    function showFailPanel(actionType, statusCode) {
        statusCode = statusCode + ""; //ensure string
        if (actionType == "upgrade") { //Cant Upgrade reasons
            switch (statusCode) {
                case "7":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantUpgrade_7');
                    break;
                case "6":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantUpgrade_6');
                    break;
                case "3":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantUpgrade_3');
                    break;
                case "1":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantUpgrade_1');
                    break;
                case "2":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantUpgrade_2');
                    break;
                case "5":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantUpgrade_5');
                    break;
                case "4":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantUpgrade_4');
                    break;
                default:
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantUpgrade_def');
                    break;
            }
        } else { //Cant Downgrade reasons
            switch (statusCode) {
                case "1":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantDowngrade_1');
                    break;
                case "2":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantDowngrade_2');
                    break;
                case "3":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantDowngrade_3');
                    break;
                case "4":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantDowngrade_4');
                    break;
                case "5":
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantDowngrade_5');
                    break;
                default:
                    reasonText = ROE.Utils.phrase('QuickBuildPhrases', 'qb_cantDowngrade_Default');
                    break;
            }
        }
        var div = $('<div>').addClass('cannotUpgradePanel').html('<span>' + reasonText + '</span>')
        .click(function () { $(this).remove(); }).appendTo('#quickbuild_popup');
    }
    
    function _showSpeedUpOptions(btn) {
        if (btn.hasClass('grayout')) { return; }
        ROE.UI.Sounds.click();

        var icon = "https://static.realmofempires.com/images/icons/m_hatspells.png";
        var speedUpTemplate = $('.speedUpTemplate').clone();

        if (btn.hasClass('upgrade')) {

            _populateSpeedUpContentUpgrade(speedUpTemplate);

        } else {

            var bid = DATA.DowngradeQ[0].buildingID;
            var icon = ROE.Entities.BuildingTypes[bid].IconUrl_ThemeM;
            speedUpTemplate.find('.sIcon').css('background-image', "url('" + icon + "')");
            speedUpTemplate.find('.sToLevel').text(''); //insufficient downgrade info
            speedUpTemplate.find('.sServantCount').html(ROE.Player.credits);

            var speedUpContainer = speedUpTemplate.find('.sContent');
            var optionTemplate = speedUpTemplate.find('.sOption').remove();
            var time = $(CONST.Selector.qDowngradeSummaryArea).find(CONST.Selector.currentBuildTimeTimer).text();
            var countDownSpeedup = speedUpTemplate.find('.sTimer').addClass('countdown').html(time).attr("data-finisheson", DATA.DowngradeQ[0].completionTime);
            //.attr("data-totalms", DATA.DowngradeQ[0].upgradeDuration) <-- since downgrade is missing this, progress bar wont work.
            var newOption, cost, count = 1;

            ///temporary untill API improves
            speedUpTemplate.find('.sProgressContainer').addClass('downgradeTempFix');

            newOption = optionTemplate.clone();
            newOption.find('.sName').html(BDA.Dict['spell_name_speedupDowngradenow']);
            newOption.find('.sCut').html(BDA.Dict['qb_SpeedUpDowngrade_CutFinishNow']);
            newOption.find('.sServants').html(BDA.Dict['qb_SpeedUpDowngrade_ServantFinishNow']);
            newOption.find('.speedUpButton').attr('onclick', 'ROE.QuickBuild.doSpeedUp();');
            speedUpContainer.append(newOption);

        }

        ROE.Frame.simplePopopOverlay(icon, 'Cast Spell', speedUpTemplate.html(), 'speedUpOptionsPopup quickbuild', $('#quickBuildDialog').parent());
        Timer.initTimers();
    }

    function _populateSpeedUpContentUpgrade(speedUpTemplate) {

        var firstQItem = DATA.Q.UpgradeEvents[0];
        var icon = ROE.Entities.BuildingTypes[firstQItem.bid].IconUrl_ThemeM;
        speedUpTemplate.find('.sIcon').css('background-image', "url('" + icon + "')");
        speedUpTemplate.find('.sToLevel').text('To ' + firstQItem.upgradeToLevel);
        speedUpTemplate.find('.sServantCount').html(ROE.Player.credits);

        var speedUpContainer = speedUpTemplate.find('.sContent');
        var optionTemplate = speedUpTemplate.find('.sOption').first();
        speedUpTemplate.find('.sOption').remove();

        var t = Utils.timeLeft(firstQItem.completionTime);
        var time = t.h + ":" + t.m + ":" + t.s;
        var countDownSpeedup = speedUpTemplate.find('.sTimer').addClass(CONST.CssClass.countdown).attr("data-totalms", firstQItem.upgradeDuration).attr('data-finisheson', firstQItem.completionTime);
        var timeLeft = ROE.Utils.convertCountdownDisplayToTimeLeft(time);
        var speedUpsAvail = ROE.Utils.UpgradeSpeedUp.calculateFinishNow(timeLeft.leftMin, _village.upgrade.villageinfo_minSpeedUpAllowed);
        var finishNowCost = speedUpsAvail.cost;


        //the speedup rewards panel
        $('.speedUpItemRewards', speedUpTemplate).remove();
        if (ROE.Items2.existsItemOfType('buildingspeedup')) {
            $('.sHeader', speedUpTemplate).after($('<div class="speedUpItemRewards fontSilverFrLCmed" ' +
                'onclick="ROE.Items2.showPopup(' + _villageID + ',\'buildingspeedup\'); $(\'.speedUpOptionsPopup\').remove();">' +
                'Use Speedup Rewards</div>'));
        }

        //the not enough speeds ups left in the day panel
        $('.notEnoughSpeedUp', speedUpTemplate).remove();
        var speedupRemainingText;
        if (timeLeft.leftMin > _village.upgrade.villageinfo_minSpeedUpAllowed) {
            if (_village.upgrade.villageinfo_minSpeedUpAllowed > 0) {
                speedupRemainingText = _village.upgrade.villageinfo_minSpeedUpAllowed + ' minutes of speedup left today.';
            } else {
                speedupRemainingText = 'No more speedups remaining today.';
            }
            $('.sHeader', speedUpTemplate).after('<div class="notEnoughSpeedUp fontSilverFrLCmed">' +
                speedupRemainingText + '</br>Resets at 04:00 AM UTC</div>');
        }

        var newOption, cost, count = 1;
        if (speedUpsAvail.allowFinishNow) {
            //FinishNow speedup option 
            newOption = optionTemplate.clone();
            newOption.find('.sName').html(BDA.Dict['spell_name_speedupFinishnow']);
            newOption.find('.sCut').html(BDA.Dict['qb_SpeedUp_CutFinishNow']);
            newOption.find('.sServants').html(finishNowCost + " " + BDA.Dict['qb_SpeedUp_ServantFinishNow']);
            newOption.find('.speedUpButton').attr('data-minToCut', 9999).attr('onclick', 'ROE.QuickBuild.doSpeedUp($(this));');
            speedUpContainer.append(newOption);
        }

        //then display 3 additional most expensive options cheaper than FinishNow
        for (var i = 0, sp; sp = ROE.UpgradeSpeedUps_timeCuts[i]; i++) {
            if (count > 3) { break; } //only show a total of 3 options
            cost = ROE.Utils.UpgradeSpeedUp.costOfTimeCut(sp);
            if (cost < finishNowCost) {
                count++;
                newOption = optionTemplate.clone();
                newOption.find('.sName').html(BDA.Dict['spell_name_speedup' + sp]);
                newOption.find('.sCut').html(BDA.Dict['qb_SpeedUp_Cut' + sp]);
                newOption.find('.sServants').html(BDA.Dict['qb_SpeedUp_Servant' + sp]);
                newOption.find('.speedUpButton').attr('data-minToCut', sp).attr('onclick', 'ROE.QuickBuild.doSpeedUp($(this));');
                speedUpContainer.append(newOption);
            }
        }

    }

    function updateSpeedUpPopup(from, data) {
        ROE.UI.Sounds.clickSpell();
        var speedUpPopup = $('.speedUpOptionsPopup.quickbuild');
        speedUpPopup.find('.sBusy').hide();
        if (from == "upgrade") {                       
            if (data.Village.upgrade.Q.UpgradeEvents[0] && firstEventId == data.Village.upgrade.Q.UpgradeEvents[0].ID) {              
                DATA = data.Village.upgrade;
                _populateSpeedUpContentUpgrade(speedUpPopup);
            } else {
                speedUpPopup.find('.sContent').empty();
                speedUpPopup.find('.sProgressBar').addClass('finished');
                speedUpPopup.find('.sTimer').hide();
                speedUpPopup.find('.sToLevel').text('Completed');
                speedUpPopup.find('.sServantCount').html(ROE.Player.credits);

                //if more items in Q, make a button to load in the next item for speedup
                if (data.Village.upgrade.Q.UpgradeEvents.length > 0) {
                    var nextItemBtn = $('<div>').html('Next').addClass("nextInQueue BtnBLg1 fontSilverFrSClrg").click(function () {
                        speedUpPopup.remove();
                        $("#quickbuild_popup .speedUpButton.upgrade").click();
                    });
                    speedUpPopup.find('.sContent').append(nextItemBtn);
                }
            }
        } else if (from == "downgrade") {
            ///temporary untill API improves
            speedUpPopup.remove();
            Frame.infoBar(ROE.Utils.phrase('QuickBuildPhrases', 'SpeedUpDowngradeSuccess'));
        }
    }

    function _speedUpCompleted() {
        $('.speedUpOptionsPopup.quickbuild').remove();
    }

    function _doSpeedUp(btn) {       
        ROE.UI.Sounds.click();
        $('.speedUpOptionsPopup.quickbuild').find('.sBusy').show();
        if (pageActivity == "upgrade") {
            var minToCut = btn.attr('data-mintocut');
            ROE.Api.call_upgrade_speedupupgrade2(_villageID, null, firstEventId, minToCut, _doSpeedUpUpgradeReturn);
        } else if (pageActivity == "downgrade") {
            ROE.Api.call_downgrade_speedup(_villageID, firstEventId, _doSpeedUpDowngradeReturn);
        }        
    }

    function _doSpeedUpUpgradeReturn(data) {
        Frame.free($('#quickBuildDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.Village.id) {
            return;
        }

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        if (data.cutSuccessful) {
            ROE.Frame.refreshPFHeader(data.playersCreditsNow);
            ROE.QuickBuild.changesMade_RequireVOVReload = true;
            updateSpeedUpPopup("upgrade", data); //works in M, redundant in D2 untill issue noted below is addressed.
            ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village);
        } else {
            $('.speedUpOptionsPopup.quickbuild').remove();
            Frame.infoBar(ROE.Utils.phrase('QuickBuildPhrases', 'SpeedUpCastFail'));
            ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village);
        }       
        
        //this needs another visit at some point, as synching this way is wiping out the speedUp popup -farhad
        //possible options were discussed, do synch onpopup close, or pop the popup after populate, or stick the popup somewhere that doesnt get wiped out, 
        //or best and most expensive option, restructure the popup in a manner that the popup doesnt get wiped in a populate.
        _sync(_village);

    }

    function _doSpeedUpDowngradeReturn(data) {
        Frame.free($('#quickBuildDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.Village.id) {
            return;
        }

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        if (data.cutSuccessful) {
            ROE.Player.credits = data.playersCreditsNow;
            ROE.QuickBuild.changesMade_RequireVOVReload = true;
            updateSpeedUpPopup("downgrade", data);
            ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village);
        } else {
            $('.speedUpOptionsPopup.quickbuild').remove();
            Frame.infoBar(ROE.Utils.phrase('QuickBuildPhrases', 'SpeedUpDowngradeFail'));
            ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village);
        }            
        _sync(_village);
    }

    //the freespeedup function for when X amount of time remaining.
    function _freeUpgradeReady() {

        //disabled for RX, also disabled in backend
        if (ROE.rt == "X") { return; }

        var popupContent = $('#quickBuildDialog');
        $('.speedUpButton', popupContent).hide(); //hide the standard speedup button
        $('.speedUpFreeButton', popupContent).fadeIn(); //show the free button
        $('.qSpeedUp', popupContent).hide(); //hide the Q view speedup button
        $('.speedUpOptionsPopup.quickbuild').remove(); //remove speeduppop if currently open
    }

    function _doSpeedUpFree() {
        Frame.busy('Completing the building...', 5000, $('#quickBuildDialog'));
        ROE.Api.call_upgrade_speedupupgradefree(_villageID, firstEventId, _doSpeedUpUpgradeReturn);
    }

    

    var _reInitContentWithDifferentVillage = function (v) {
        $('.speedUpOptionsPopup.quickbuild').remove();
        if (v) {
            _sync(v);
        } else {
            _villageID = ROE.SVID;
            _sync();
        }
    }

    var _reInitContent = function () {
        _sync();
    }

    var _reInitContent2 = function () {
        //for calling with weird clospopup functions...
        ROE.QuickBuild.reInitContent();
        closeModalIFrame('SilverTransport', true);
    }

    var _showBuildingPagePopup = function (buildingId) {
        ROE.Building2.showBuildingPagePopup(buildingId);
    }

    var _slideToQueuePage = function (btn) {
        if (btn.hasClass('grayout')) { return; }
        ROE.UI.Sounds.click();
        $(CONST.Selector.defaultPage).removeClass('slideLeftTo').addClass('slideLeftFrom');
        $(CONST.Selector.queuePage).removeClass('slideRightFrom').addClass('slideRightTo');
    }

    var _slideBackToDefaultPage = function () {
        ROE.UI.Sounds.click();
        $(CONST.Selector.defaultPage).removeClass('slideLeftFrom').addClass('slideLeftTo');
        $(CONST.Selector.queuePage).removeClass('slideRightTo').addClass('slideRightFrom');
    }

    function _expandRow(btn) {
        ROE.UI.Sounds.click();
        var row = btn.parent().toggleClass(CONST.CssClass.expandedRow);
        if (row.hasClass(CONST.CssClass.expandedRow)) {
            row.find(CONST.Selector.UI.smallArrowDown)
                .removeClass(CONST.CssClass.UI.smallArrowDown).addClass(CONST.CssClass.UI.smallArrowUp);
        } else {
            row.find(CONST.Selector.UI.smallArrowUp)
                .removeClass(CONST.CssClass.UI.smallArrowUp).addClass(CONST.CssClass.UI.smallArrowDown);
        }
    }



    function _handleCurrentlySelectedVillageChangedEvent() {
        if ($("#quickBuildDialog").dialog("isOpen") === true && _villageID != ROE.SVID) {
            BDA.Console.verbose('ROE.QB', "switching to VID:%vid%".format({ vid: ROE.SVID }));
            _villageIDPassedFromMap = null;
            _syncForSelectedVillage(ROE.SVID);  
        }
    }


    function _handleVillageExtendedInfoUpdatedOrPopulated(village) {
        if (!isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent) {
            // only handle this event if we are not ignoring the next one. 
            //  why do we do this? because this event fires after user notiated events in this UI, which reloads it self already. 
            //  so no point reloading it twice. 
            if ((ROE.Frame.isPopupOpen("#quickBuildDialog") || ROE.Frame.isPopupOpen("#popup_QuickBuild")) && _villageID == village.id) {
                if (village.changes.buildings
                || village.changes.buildingWork
                || village.changes.coins
                || village.changes.popRem
                || village.changes.name) {
                    BDA.Console.verbose('ROE.QB', "VID:%vid% - reloading".format({ vid: village.id }));
                    _sync(village);
                } else {
                    BDA.Console.verbose('ROE.QB', "VID:%vid% - no changes".format({ vid: village.id }));
                }                
            }
        }
        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = false;
    }

    //randomly picks from a set of messages
    function _randomPhrase(msg) {
        return ROE.Utils.phrase('QuickBuildPhrases', 'MainLoadingMsg1');
        //switch(msg){
        //    case 'MainLoadingMsg':
        //        var arr = [
        //            ROE.Utils.phrase('QuickBuildPhrases', 'MainLoadingMsg1'),
        //            ROE.Utils.phrase('QuickBuildPhrases', 'MainLoadingMsg2'),
        //            ROE.Utils.phrase('QuickBuildPhrases', 'MainLoadingMsg3'),
        //            ROE.Utils.phrase('QuickBuildPhrases', 'MainLoadingMsg4'),
        //            ROE.Utils.phrase('QuickBuildPhrases', 'MainLoadingMsg5')
        //        ]
        //        return arr[Math.floor(Math.random() * myArray.length)];
        //}
    }

    //D2 only
    function _UIDisplayModeChanged() {
        _UIDisplayMode = ROE.LocalServerStorage.get('UIDisplayMode');
        CACHE.template = null;
        $("#quickBuildDialog").dialog({ width: _UIDisplayMode == 'Standard' ? 360 : 525 });
        if ($("#quickBuildDialog").dialog("isOpen") === true) {
            _load();
        }
        
    }

    
    obj.speedUpCompleted = _speedUpCompleted;
    obj.showSpeedUpOptions = _showSpeedUpOptions;
    obj.doSpeedUp = _doSpeedUp;
    obj.doCancel = _doCancel;
    obj.cancelAll = _cancelAll;
    obj.doDowngradeBuilding = _doDowngradeBuilding;
    obj.doUpgradeBuilding = _doUpgradeBuilding;
    obj.expandRow = _expandRow;
    obj.upgradeNumpad = _upgradeNumpad;
    obj.slideToQueuePage = _slideToQueuePage;
    obj.slideBackToDefaultPage = _slideBackToDefaultPage;
    obj.showPopup = _showQuickBuildPopup;
    obj.reInitContent = _reInitContent;
    obj.reInitContent2 = _reInitContent2;
    obj.reInitContentWithDifferentVillage = _reInitContentWithDifferentVillage;
    obj.switchMode = _switchMode;
    obj.changesMade_RequireVOVReload = false;
    obj.launchedFrom = "vov";
    obj.villageID = _villageID;
    obj.showBuildingPagePopup = _showBuildingPagePopup;
    obj.freeUpgradeReady = _freeUpgradeReady;
    obj.doSpeedUpFree = _doSpeedUpFree;
    obj.UIDisplayModeChanged = _UIDisplayModeChanged;

}(window.ROE.QuickBuild = window.ROE.QuickBuild || {}));
