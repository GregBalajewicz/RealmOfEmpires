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
    BDA.Console.setCategoryDefaultView('ROE.BP', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file
    var Timer = window.BDA.Timer,
        Scrolling = window.BDA.UI.Scrolling,
        Transition = window.BDA.UI.Transition,
        Utils = window.BDA.Utils;
    
    var Api = window.ROE.Api,
        Entities = window.ROE.Entities,
        Frame = window.ROE.Frame,
        Player = window.ROE.Player

    var CONST = { popupName: "Building2" },
        CACHE = {};

    CONST.Selector = {
        defaultPage: ".themeM-view.default",
        alternate: ".themeM-view.alternate",
        countDown : ".countdown",
        resourceFooter: ".resourceFooter.footer1",

        templates: {
            standardRecPanel: '.standardRecPanel.template',
            queueRow: '.queuedItemRow.template',
            descBox: '.unitDescriptionBox.template'
        },

        Q: {
            queuedItemsArea: '.queuedItemsArea',
            bigQContainer: '.queueContainer',
            queuedItemRow: '.queuedItemRow',
            qTime: '.qTime'
        }
       
    }; 

    CONST.CssClass = {
        template: "template",
        upgradeTemplate: "upgradeTemplate",
        disabled: "disabled",
        expanded: "expanded",
        stateConstruction: "state-upgrade-construction",
        stateBuild: "state-upgrade-build",
        stateBuilt: "state-upgrade-built",
        countdownTimer: "countdown",
        styleWarning: "style-warning"
    };

    CONST.DataAttr = {
        countdownTimer: "data-refreshCall",
        eventID: "data-eventID",
        speedUpMin : "data-min",
        unitID : "data-uid"
    };


    CONST.Enum = {};

    CONST.Enum.CanUpgrade = {
        Yes: 0,
        No_LackFood: 1,
        No_LackSilver: 2,
        No_BuildingFullyUpgraded: 3,
        No_LockedFeature: 4,
        No_Busy: 5,
        No_UnsatisfiedReq: 6,
        No_DowngradesInProgress: 7
    };

    CONST.Enum.CanRecruit = {
        Yes: 0,
        No_RecruitmentBuildingNotBuilt: 1,
        No_RequirementsNotSatisfied: 2,
        No_LackSilver: 3,
        No_LackFood: 4
    };

    CONST.Enum.BuildingType = {
        Palace: "9"
    };

    CACHE.Selector = {
        //
    };

    CACHE.Data = {
        upgrade: null,
        recruit: null
    };

    // remembers the raw template (jquery object) for this particular building
    CACHE.template = {};

    var DATA = {}; //this will hold all data from api return
    var _template_Page; // jquery object
    var _buildingID;
    var _villageID;
    var _village;
    var _buildingEventID; //used for canceling, and speeding up
    var _template_SpeedUps; // jquery object holding the template for speedups
    var _slideView = "default"; //to keep track of what slide was being viewed before refresh
    var _container;
    var isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = false;

    var _showBuildingPagePopup = function (buildingID) {
        /// <summary>
        /// Show the building UI popup 
        ///</summary>
        _buildingID = parseInt(buildingID, 10);
        //if quickbuild is open, take villageID from quickbuild.
        if ($('#popup_QuickBuild').length > 0) {
            _villageID = ROE.QuickBuild.villageID;
        } else {
            _villageID = ROE.SVID;
        }
        _slideView = "default";
        ROE.UI.Sounds.clickBuildingEnter();
        ROE.vov.pauseAnimation();
        _load();
    }

    function _load() {        

        if (!CACHE.template[_buildingID]) {
            Frame.busy('Getting Building Blueprints...', 3000, $('#buildingDialog'));
            var temp = BDA.Templates.getRawJQObj("BuildingTempl2", ROE.realmID);
            
            // remove panels that are not for this building
            temp.find('.bldSpecific:not([data-bid="' + _buildingID + '"])').remove();
            temp.find('.oneUnitHelp:not([data-bid="' + _buildingID + '"])').remove();
            temp.find('.oneBuildingHelp:not([data-bid="' + _buildingID + '"])').remove();

            CACHE.template[_buildingID] = temp;
            CACHE.template["descBox"] = temp.find(CONST.Selector.templates.descBox);
            CACHE.template["queueRow"] = temp.find(CONST.Selector.templates.queueRow);

            Frame.free($('#buildingDialog'));
        }

        _template_Page = CACHE.template[_buildingID];
        _template_SpeedUps = _template_Page.find(CONST.Selector.speedUpPanel);

        var iconImgUrl = buildingPopupIconUrl(_buildingID);
        var buildingName = Entities.BuildingTypes[_buildingID].Name;
        if (ROE.isMobile) {
            popupModalPopup(CONST.popupName, buildingName, undefined, undefined, undefined, undefined, 'closeBuildingPage', iconImgUrl);
            _container = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + CONST.popupName + ' .popupBody').css({ 'background-image': 'url("' + buildingBackground(_buildingID) + '")' });
        } else {
            _container = $('#buildingDialog').css({ 'background-image': 'url("' + buildingBackground(_buildingID) + '")' });
            BDA.Broadcast.subscribe(_container, "CurrentlySelectedVillageChanged", _handleMapSelectedVillageChangedEvent);            
            $('#buildingDialog').dialog('option', 'title', buildingName).dialog('open');
        }

        BDA.Broadcast.subscribe(_container, "VillageExtendedInfoUpdated", _handleVillageExtendedInfoUpdatedOrPopulated);
        $('.speedUpOptionsPopup.building2').remove();
        _sync();
    }

    function _sync(optionalVillage) {
        Frame.busy('Entering Building...', 10000, $('#buildingDialog'));
        BDA.Console.verbose('ROE.BP', "_synch(%v%)".format({ v: optionalVillage }));
        if (optionalVillage) {
            _sync_onDataSuccess(optionalVillage);
        } else {
            ROE.Villages.getVillage(_villageID, _sync_onDataSuccess, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
        }
    }

    function _syncForSelectedVillage(selectedVillageID) {
        Frame.busy('Entering Building...', 10000, $('#buildingDialog'));
        $('.speedUpOptionsPopup.building2').remove();
        _villageID = selectedVillageID;
        ROE.Villages.getVillage(_villageID, _sync_onDataSuccess, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
    }


    function _sync_onDataSuccess(village) {
        Frame.free($('#buildingDialog'));
        _villageID = village.id;
        _village = village;

        //restructure village data for buildingpages
        DATA = {};        
        DATA.upgradeInfo = _deriveUpgradeData(village.upgrade.Buildings);
        DATA.recruitInfo = {};
        DATA.recruitInfo.unitRecruitInfo = _deriveRecruitData(village.recruit.recruitInfo.recruitInfoList);
        DATA.govInfo = village.recruit.recruitInfo.govInfo;
        DATA.Q = village.upgrade.Q;

        _populate();
        ROE.Frame.refreshQuestCount();
    }

    function _deriveRecruitData(recruitInfoList) {
        for (var i = 0; i < recruitInfoList.length; i++) {
            if (recruitInfoList[i].buildingTypeID == _buildingID) {
                return recruitInfoList[i].unitsRecruited;
            }
        }
    }

    function _deriveUpgradeData(buildings) {
        for (var i = 0; i < buildings.length; i++) {
            if (buildings[i].buildingID == _buildingID) {
                return buildings[i];
            }
        }
    }

    function _sync_onDataFailure(data) { }

    function _populate() {
        //console.log('_populate', DATA);

        var refinedData = {};
        var preparedRequirments = ""; //this will hold all requirement items, and appended later
        var reqText, reqBuildingId;
        var buildingEntity = Entities.BuildingTypes[_buildingID];
        var currentActiveAlternatePage = null;
       
        if (_slideView == "default") {
            refinedData.slideClassDefault = "";
            refinedData.slideClassAlternate = "slideRightFrom";
        } else if (_slideView == "alternate") {
            refinedData.slideClassDefault = "slideLeftFrom";
            refinedData.slideClassAlternate = "slideRightTo";
            //record which alternate page was active, to make active after resetting wiping dom.
            currentActiveAlternatePage = $('.alternateSubPage.active', _container).attr('data-alt');
        }

        refinedData.buildingIcon = buildingEntity.IconUrl_ThemeM;
        refinedData.backgroundimageurl = buildingBackground(_buildingID);       
        refinedData.curLevel = DATA.upgradeInfo.curLevel;
        if (!ROE.isMobile) { $('#buildingDialog').dialog('option', 'title', 'Level ' + DATA.upgradeInfo.curLevel + ' ' + buildingEntity.Name); }
        refinedData.canUpgrade = DATA.upgradeInfo.Upgrade.canUpgrade;
        if (refinedData.canUpgrade != 0) { refinedData.actionButtonGrayout = 'cant'; }

        ///SETUP UPGRADE-TO-NEXT SECTION AND REQUIREMENTS
        if (DATA.upgradeInfo.Upgrade.nextLevel) {

            refinedData.upgradeNextCost = ROE.Utils.addThousandSeperator(DATA.upgradeInfo.Upgrade.nextLevel.cost);
            refinedData.currentSilver = ROE.Utils.addThousandSeperator(_village.coins);
            refinedData.villageID = _village.id; // For realtime silver updates.

            //If the cost of upgrade is bigger than your tresury max
            if (DATA.upgradeInfo.Upgrade.nextLevel.cost > _village.coinsTresMax) {
                
                preparedRequirments += '<div class="requirementHeader fontSilverFrLCmed">Build cost is more than your treasury can hold!</div>';
                preparedRequirments += addRequirement("buildTres");
                preparedRequirments += addRequirement("tresResearch");
                
            } else {

                //Requirements: Lack of Silver
                if (DATA.upgradeInfo.Upgrade.nextLevel.cost > _village.coins) {
                    refinedData.enoughSilver = "reqNotMet";
                    preparedRequirments += '<div class="requirementHeader fontSilverFrLCmed">Insufficient silver, here are some options to help:</div>';
                    if (ROE.Items2.existsItemOfType('silver')) { preparedRequirments += addRequirement("silverRewards"); }
                    if (ROE.Player.questsCompl > 0) { preparedRequirments += addRequirement("silverQuests"); }
                    preparedRequirments += addRequirement("silverBazaar");
                    preparedRequirments += addRequirement("silverLootRebels");
                    preparedRequirments += addRequirement("silverUseSpells");
                    preparedRequirments += addRequirement("silverResearch");
                    if (_buildingID != 5) { preparedRequirments += addRequirement("silverBuildSM"); }
                } else {
                    refinedData.enoughSilver = "reqMet";
                }

            }

            

            refinedData.upgradeNextFood = ROE.Utils.addThousandSeperator(DATA.upgradeInfo.Upgrade.nextLevel.food);
            refinedData.currentFood = ROE.Utils.addThousandSeperator(_village.popRemaining);
            if (DATA.upgradeInfo.Upgrade.nextLevel.food > _village.popRemaining) {
                refinedData.enoughFood = "reqNotMet";
                preparedRequirments += '<div class="requirementHeader fontSilverFrLCmed">Insufficient food, here are some options to help:</div>';
                preparedRequirments += addRequirement("foodBuildFarm");
                preparedRequirments += addRequirement("foodResearch");
                //possibly add info about farm research
            } else {
                refinedData.enoughFood = "reqMet";
            }

            refinedData.upgradeNextTime = ROE.Utils.msToTime(DATA.upgradeInfo.Upgrade.nextLevel.time);
            refinedData.upgradeNextToLevel = DATA.upgradeInfo.Upgrade.nextLevel.levelNum;

            //Building requirements
            for (var r = 0; r < DATA.upgradeInfo.Upgrade.unsatisfiedRequirementsIfAny.length; r++) {
                reqBuildingId = DATA.upgradeInfo.Upgrade.unsatisfiedRequirementsIfAny[r].btid;
                reqText = "Level (<span>" + DATA.upgradeInfo.Upgrade.unsatisfiedRequirementsIfAny[r].level + "</span>)";
                preparedRequirments += addRequirement("reqBuilding",reqText,reqBuildingId );
            }

            //Locked feature: To Q more than 2 needs Nobility package
            if (refinedData.canUpgrade == CONST.Enum.CanUpgrade.No_LockedFeature /* 4 */) {
                reqText = "Enable Nobility Package to queue more than 2 buildings.";
                preparedRequirments += addRequirement("upgradeInProgress", reqText, undefined);
            }

            //Blocked by Other Upgrading
            if (refinedData.canUpgrade == 5 ) {
                reqText = "Upgrade In Progress";
                preparedRequirments += addRequirement("upgradeInProgress", reqText, undefined);
            }

            //Blocked by Downgrading
            if (refinedData.canUpgrade == 7 ) {
                reqText = "Downgrade In Progress";
                preparedRequirments += addRequirement("downgradeInProgress", reqText, undefined);
            }
        }

        ///SETUP UPGRADING SECTION
        var firstQItem = DATA.Q.UpgradeEvents[0];
        if (firstQItem && firstQItem.bid == _buildingID) {
            _buildingEventID = firstQItem.eventID;
            if (firstQItem.completionTime > Date.now()) {
                refinedData.upgradeState = 'upgrading';
                refinedData.countDown = CONST.CssClass.countdownTimer;
                refinedData.bFinishesOn = firstQItem.completionTime;
                refinedData.upgradingToLevel = DATA.upgradeInfo.curLevel + 1;
                refinedData.maxDuration = firstQItem.upgradeDuration;
            }
        } else {
            _buildingEventID = null;
            refinedData.upgradeState = 'idle';           
        }

        ///SETUP Q SECTION
        var queuedPreviewItems = '';
        var fullQContainer = $('<div>');
        if (DATA.Q.UpgradeEvents.length > 0) {
            refinedData.showQPreview = "active";
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

                try {
                    aNewQueueRow = $(BDA.Templates.populate(CACHE.template.queueRow[0].outerHTML, queueItemRowData)).removeClass('template');
                } catch (ex) {
                    var roeex = new BDA.Exception("BuildingPages2: case 26614, somehow queueRow isnt defined? ", ex);
                    roeex.data.add('buildingID', _buildingID);
                    roeex.data.add('qiBuildingId', qiBuildingId);
                    roeex.data.add('qiEventId', qiEventId);
                    roeex.data.add('qiIsInQ', qiIsInQ);
                    roeex.data.add('CACHE.template', CACHE.template);
                    roeex.data.add('CACHE.template.queueRow[0].outerHTML', CACHE.template.queueRow[0].outerHTML);
                    BDA.latestException = roeex;
                    throw roeex;
                }
                

                if (e == 0) {
                    //only add countdown and speedUp to the first item in full Q page
                    aNewQueueRow.find(CONST.Selector.Q.qTime).addClass(CONST.CssClass.countdownTimer);
                    aNewQueueRow.append('<div class="qSpeedUp upgrade smallRoundButtonDark" onclick="ROE.Building2.showSpeedUpOptions($(this));"><span></span></div>');
                }               

                fullQContainer.append(aNewQueueRow);
                
                if (e < 3) { //only show top 3 queues in the main page summary area
                    queuedPreviewItems += '<span class="queuedItem fontWhiteNumbers" style="background-image:url(\'' + qiIconUrl + '\');">(' + qiToLevel + ')</span>';
                }

            }

            //add a ... in q preview if more than 3 items in Q
            if (DATA.Q.UpgradeEvents.length > 3) {
                queuedPreviewItems += '<span class="queuedMore");">...</span>';
            }

        }

        ///SETUP UNIT DETAIL SECTION
        var preparedRecruitInfoButtons = $('<div>');
        var preparedRecruitInfoDesc = $('<div>');

        if (DATA.recruitInfo.unitRecruitInfo.length > 0) {
            refinedData.troopDetails = "active";
            for (var u = 0; u < DATA.recruitInfo.unitRecruitInfo.length; u++) {
                _createUnitDetail(DATA.recruitInfo.unitRecruitInfo[u], preparedRecruitInfoButtons, preparedRecruitInfoDesc);
            }
        }

        ///SETUP EFFICIENCY PANEL
        refinedData.effectPanel = {};
        refinedData.effectPanel.totalPerc = DATA.upgradeInfo.EffectInfo.total_perc;
        if (DATA.upgradeInfo.EffectInfo.level_perc) {
            refinedData.effectPanel.showLevelPerc = "show";
            refinedData.effectPanel.curLevel = DATA.upgradeInfo.curLevel;
            refinedData.effectPanel.maxLevel = buildingEntity.MaxLevel;
            refinedData.effectPanel.levelPerc = (DATA.upgradeInfo.curLevel / buildingEntity.MaxLevel) * 100;
        }
        if (DATA.upgradeInfo.EffectInfo.res_perc) {
            refinedData.effectPanel.showResPerc = "show";
            refinedData.effectPanel.resPercCur = Math.floor(DATA.upgradeInfo.EffectInfo.res_effect);
            refinedData.effectPanel.resPercMax = Math.floor(DATA.upgradeInfo.EffectInfo.res_max);
            refinedData.effectPanel.resPerc = DATA.upgradeInfo.EffectInfo.res_perc;
        }
        
        ///SETUP BUILDING SPECIFIC ACTIONS
        switch (_buildingID) {
            case 1: //barracks
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_perc;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_perc + "% faster training";
                }               
                refinedData.showPanelSpecial = "active";
                refinedData.showPanelTraining = "active";
                break;
            case 2: //Stable
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_perc;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_perc + "% faster training";
                }
                refinedData.showPanelSpecial = "active";
                refinedData.showPanelTraining = "active";
                break;
            case 3: //headquarters
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_perc;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_perc + "% faster construction";
                }
                break;
            case 4: //wall
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_perc;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_perc + "% better defense";
                }
                if (DATA.upgradeInfo.EffectInfo.levelW_perc) {
                    refinedData.effectPanel.showLevelPerc = "show";
                    refinedData.effectPanel.levelPerc = DATA.upgradeInfo.EffectInfo.levelW_perc;
                }
                if (DATA.upgradeInfo.EffectInfo.levelT_perc) {
                    refinedData.effectPanel.showMiscPerc = "show";
                    refinedData.effectPanel.miscPerc = DATA.upgradeInfo.EffectInfo.levelT_perc;
                    refinedData.effectPanel.miscLabel = "From Tower Level";
                }
                break;
            case 5: //silvermine
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_effect;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_effect + " silver per hour";
                }
                break;
            case 6: //treasury
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_effect;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_effect + " silver capacity";
                }
                var timeTillFull = DATA.upgradeInfo.EffectInfo.timeTillTreasuryFull;
                if (timeTillFull == "0") {
                    refinedData.treasVar1Display = "none";
                    refinedData.treasVar2Display = "block";
                } else {
                    refinedData.effectValTwo = "<span class='countdown' refresh=\"false\">" + DATA.upgradeInfo.EffectInfo.timeTillTreasuryFull + "</span>";
                    refinedData.treasVar1Display = "block";
                    refinedData.treasVar2Display = "none";
                }
                break;
            case 7: //towers
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_perc;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_perc + "% better defense";
                }
                if (DATA.upgradeInfo.EffectInfo.levelT_perc) {
                    refinedData.effectPanel.showLevelPerc = "show";
                    refinedData.effectPanel.levelPerc = DATA.upgradeInfo.EffectInfo.levelT_perc;
                }
                if (DATA.upgradeInfo.EffectInfo.levelW_perc) {
                    refinedData.effectPanel.showMiscPerc = "show";
                    refinedData.effectPanel.miscPerc = DATA.upgradeInfo.EffectInfo.levelW_perc;
                    refinedData.effectPanel.miscLabel = "From Wall Level";
                }
                break;
            case 8: //farmland
                refinedData.effectValOne = _village.popRemaining;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_effect + " food produced";
                }
                break;
            case 9: //palace
                refinedData.effectValOne = 100 - DATA.upgradeInfo.EffectInfo.total_perc;
                refinedData.showPanelSpecial = "active";
                refinedData.showPanelTraining = "active";
                break;
            case 10: //siege workshop
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_perc;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_perc + "% faster training";
                }
                refinedData.showPanelSpecial = "active";
                refinedData.showPanelTraining = "active";
                break;
            case 11: //trading post
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_effect;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_effect + " per transport";
                }
                break;
            case 12: //tavern
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_perc;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_perc + "% faster training";
                }
                refinedData.showPanelSpecial = "active";
                refinedData.showPanelTraining = "active";
                break;
            case 13: //hiding spot
                refinedData.effectValOne = DATA.upgradeInfo.EffectInfo.total_effect;
                if (DATA.upgradeInfo.EffectInfoNextLevel) {
                    refinedData.upgradeNextBonus = DATA.upgradeInfo.EffectInfoNextLevel.total_effect + " silver hidden";
                }
                break;
            default:
                break;

        }


        var content = BDA.Templates.populate(_template_Page[0].outerHTML, refinedData);
        content = $(content);
        content.find('.actionRequirements').append(preparedRequirments); 
        content.find('.qPreviewPanel .queuedItemsArea').append(queuedPreviewItems);
        content.find('.pageFullQueue .queueContainer').append(fullQContainer);
        content.find('.unitInformationSection.buttonsSection').append(preparedRecruitInfoButtons.children());
        content.find('.unitInformationSection.descriptionsSection').append(preparedRecruitInfoDesc.children());

        if (!ROE.isVPRealm) {
            content.find('.speedUpButton, .qSpeedUp').remove();
        }

        _container.html(content);

        //keep the alternate page active if it was previously in use
        $('.alternateSubPage[data-alt="' + currentActiveAlternatePage + '"]', _container).addClass('active');

        ROE.FooterWidget.init(content.find(CONST.Selector.resourceFooter), _village, "ROE.Building2.reInitContent();");
      
        Timer.initTimers();

        if (ROE.isDevice == ROE.Device.CONSTS.Amazon) {
            var wrapper = $('<div>').addClass("html5SwipeWrapper inBuildingPage");
            var floater = $('<div>').addClass("html5SwipeFloater");
            wrapper.append(floater);
            _container.append(wrapper);
            var wrapperWidth = wrapper.width();
            var floaterWidth = floater.width();
            wrapper.scrollLeft(floaterWidth / 2 - wrapperWidth / 2)
                .scroll(function () {
                    window.clearTimeout(window.ROE.Building2['scrollTimeout']);
                    var scroll = wrapper.scrollLeft();
                    var scrollMax = floater.width() - wrapper.width();
                    if (scroll == 0) {
                        window.ROE.Frame.swipeRight();
                    } else if (scroll >= scrollMax) {
                        window.ROE.Frame.swipeLeft();
                    } else {
                        window.ROE.Building2['scrollTimeout'] = window.setTimeout(function () {
                            var scrollCenter = floater.width() / 2 - wrapper.width() / 2;
                            wrapper.stop().animate({ 'scrollLeft': scrollCenter }, 50, "easeInCirc");
                        }, 200);
                    }
                });
        }
        
    }


    function addRequirement(type, reqText, buildingId) {
        var reqItemIcon, reqLabel, reqIcon, onclick;
        var reqHow = reqText || "";
        reqLabel = "";

        switch (type) {
            case "upgradeInProgress":
                reqLabel = "Blocked: ";
                reqItemIcon = "https://static.realmofempires.com/images/misc/M_SmNotEnough.png";
                reqIcon = '';
                break;
            case "downgradeInProgress":
                reqLabel = "Blocked: ";
                reqItemIcon = "https://static.realmofempires.com/images/misc/M_SmNotEnough.png";
                reqIcon = '';
                break;             
            case "silverRewards":
                reqItemIcon = "https://static.realmofempires.com/images/icons/M_Reward.png";
                onclick = '"ROE.Items2.showPopup(' + _villageID + ',\'silver\');"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Use your reward items.";
                break;
            case "silverQuests":
                reqItemIcon = "https://static.realmofempires.com/images/icons/m_Quests2.png";
                onclick = '"ROE.Frame.popupQuests();"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Claim completed quest rewards.";
                break;
            case "silverBuildSM":               
                reqItemIcon = "https://static.realmofempires.com/images/BuildingIcons/m_mine.png";
                onclick = '"ROE.Building2.goToRequirementBuilding($(this),5);"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Upgrade Silver Mine.";
                break;
            case "silverResearch":
                reqItemIcon = "https://static.realmofempires.com/images/icons/M_ResearchList.png";
                onclick = '"ROE.Research.showResearchPopup(\'b5\');"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Improve Mining research.";
                break;
            case "silverBazaar":
                reqItemIcon = "https://static.realmofempires.com/images/icons/giftsS.png";
                onclick = '"ROE.Frame.popupGifts(' + _villageID + ');"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Use silver gifts.";
                break;
            case "silverLootRebels":
                reqItemIcon = "https://static.realmofempires.com/images/map/freb.png";
                onclick = '"ROE.Building2.rebelLootPopup();"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Loot rebel villages.";
                break;
            case "silverUseSpells":
                reqItemIcon = "https://static.realmofempires.com/images/icons/m_hatspells.png";
                onclick = '"ROE.Frame.showServantsPopup();"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Cast Elven efficiency.";
                break;
            case "foodBuildFarm":
                reqItemIcon = "https://static.realmofempires.com/images/BuildingIcons/m_farm2.png";
                onclick = '"ROE.Building2.goToRequirementBuilding($(this),8);"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Upgrade Farm Land";
                break;
            case "foodResearch":
                reqItemIcon = "https://static.realmofempires.com/images/icons/M_ResearchList.png";
                onclick = '"ROE.Research.showResearchPopup(\'b8\');"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Improve Farming research";
                break;
            case "buildTres":
                reqItemIcon = "https://static.realmofempires.com/images/BuildingIcons/m_treasury.png";
                onclick = '"ROE.Building2.goToRequirementBuilding($(this),6);"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Upgrade Treasury building";
                break;
            case "tresResearch":
                reqItemIcon = "https://static.realmofempires.com/images/icons/M_ResearchList.png";
                onclick = '"ROE.Research.showResearchPopup(\'b6\');"';
                reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                reqHow = "Improve Treasury research";
                break;
            default:
                if (ROE.Entities.BuildingTypes[buildingId]) {
                    reqLabel = "Required Building: ";
                    reqItemIcon = ROE.Entities.BuildingTypes[buildingId].IconUrl_ThemeM;
                    onclick = '"ROE.Building2.goToRequirementBuilding($(this),' + buildingId + ');"';
                    reqIcon = '<div class="reqItemIcon smallRoundButtonDark" onclick=' + onclick + '><div style="background-image:url(\'' + reqItemIcon + '\')"></div></div>';
                }
                break;
        }

        return '<div class="reqItemWrapper"><div class="requirementItem fontSilverFrLCmed"><div class="reqItemLabel">' + reqLabel + '</div>' +
        reqIcon + '<span class="reqItemDetail">' + reqHow + '</span></div></div>';
        
    }

    function _createUnitDetail(recUnit,buttonsPanel,descPanel) {
        var unitId = recUnit.uid;
        var unitEntity = Entities.UnitTypes[unitId];
        var unitPerCostSilver = recUnit.unitCost;
        var unitPerCostFood = unitEntity.Pop;
        var unitPerCostTime = ROE.Utils.msToTime(recUnit.time);
        buttonsPanel.append( '<div class="unitDetailBtn fontGoldFrLClrg" data-unitId="' + unitId + '" onclick="ROE.Building2.selectUnitDetail($(this));">' +
            '<span class="unitDetailName">' + unitEntity.Name + '</span>' +
            '<div class="smallRoundButtonDark" ><span style="background-image:url(' + unitEntity.IconUrl_ThemeM + ');"></span></div></div>');
        var detailData = {
            descUID: unitId,
            descIcon: unitEntity.IconUrl_ThemeM,
            descText: unitEntity.description,
            descSilver: unitPerCostSilver,
            descFood: unitPerCostFood,
            descTime: unitPerCostTime,
            descAttack: unitEntity.AttackStrength,
            descDef: {
                11: unitEntity.DefenceStrengths[11],
                icon11: Entities.UnitTypes[11].IconUrl_ThemeM,
                2: unitEntity.DefenceStrengths[2],
                icon2: Entities.UnitTypes[2].IconUrl_ThemeM,
                5: unitEntity.DefenceStrengths[5],
                icon5: Entities.UnitTypes[5].IconUrl_ThemeM,
                6: unitEntity.DefenceStrengths[6],
                icon6: Entities.UnitTypes[6].IconUrl_ThemeM
            },
            descMovement: unitEntity.Speed
        }
        if (unitId == "10") {
            detailData['descSpecial'] = "gov";
            detailData.descSilver = DATA.govInfo.chestCostForNextGoverner;
        }
        descPanel.append($(BDA.Templates.populate(CACHE.template.descBox[0].outerHTML, detailData)).removeClass('template'));
    }

    function _selectUnitDetail(detailButton) {
        var container = $('#building_popup2');
        var alreadySelected = detailButton.hasClass('selected');
        container.find('.unitDetailBtn').removeClass('selected');
        container.find('.unitDescriptionBox').removeClass('selected');
        if (alreadySelected) { return; }
        var unitId = detailButton.attr('data-unitId');
        container.find('.unitDescriptionBox[data-unitId="' + unitId + '"]').addClass('selected');
        detailButton.addClass('selected');
    }

    function _doUpgradeBuilding(btn, levelToUpgradeTo) {
        if (btn.hasClass('cant')) {
            $('#building_popup2 .actionRequirements').toggleClass('displayed');
            return;
        }
        ROE.UI.Sounds.click();
        //ROE.Audio.event('buildingUpgrade');
        ROE.Building2.changesMade_RequireVOVReload = true;
        Frame.busy('Upgrading...', 5000, $('#buildingDialog'));
        ROE.Api.call_upgrade_doupgrade2(_villageID, _buildingID, levelToUpgradeTo, _doUpgradeOrCancelBuildingCallBack);
    }

    //for canceling currently upgrading building
    function _doCancelUpgrade() {
        ROE.UI.Sounds.click();
        ROE.Building2.changesMade_RequireVOVReload = true;
        Frame.busy('Canceling...', 5000, $('#buildingDialog'));
        ROE.Api.call_upgrade_cancel2(_villageID, _buildingEventID, false, _doUpgradeOrCancelBuildingCallBack);
    }

    //for canceling from full Q oage
    function _doCancel(btn) {
        ROE.UI.Sounds.click();
        var eventID = btn.attr('data-eventId');
        var isQ = (btn.attr('data-isq') == "isQ");
        var qType = btn.attr('data-qType');
        Frame.busy('Canceling...', 5000, $('#buildingDialog'));
        ROE.Api.call_upgrade_cancel2(_villageID, eventID, isQ, _doUpgradeOrCancelBuildingCallBack);
    }

    function _doUpgradeOrCancelBuildingCallBack(data) {
        Frame.free($('#buildingDialog'));
        BDA.Console.verbose('ROE.BP', "_doUpgradeOrCancelBuildingCallBack");

        //if village changed before call came back, discard data
        if (_villageID != data.Village.id) {
            return;
        }

        if (data.hasOwnProperty('cannotUpgrade')) {
            //ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village); //<-- if failed, nothing changed, why do this at all?
            alert('unhandled: building-upgrade unsuccessful.');
        } else {
            ROE.Building2.changesMade_RequireVOVReload = true;
            isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
            ROE.Villages.__populateExtendedData(data.id, _village, data);
            _sync(_village);
        }
    }

    function _upgradeProgressComplete() {
        BDA.Console.verbose('ROE.BP', "_upgradeProgressComplete");
        ROE.Villages.ExtendedInfo_loadLatest(_villageID, _sync_onDataSuccess);
    }



    /* SPEEDUP FUNCTIONS */

    function _showSpeedUpOptions(btn) {
        if (btn.hasClass('grayout')) { return; }
        ROE.UI.Sounds.click();
        var icon = "https://static.realmofempires.com/images/icons/m_hatspells.png";
        var speedUpTemplate = $('.speedUpTemplate').clone();
         _populateSpeedUpContentUpgrade(speedUpTemplate);
         ROE.Frame.simplePopopOverlay(icon, 'Cast Spell', speedUpTemplate.html(), 'speedUpOptionsPopup building2', $('#buildingDialog').parent());
        Timer.initTimers();
    }

    function _populateSpeedUpContentUpgrade(speedUpTemplate) {

        var firstQItem = DATA.Q.UpgradeEvents[0];
        var icon = ROE.Entities.BuildingTypes[firstQItem.bid].IconUrl_ThemeM;
        speedUpTemplate.find('.sIcon').css('background-image', "url('" + icon + "')");
        speedUpTemplate.find('.sToLevel').text('To ' + (DATA.upgradeInfo.curLevel + 1));
        speedUpTemplate.find('.sServantCount').html(ROE.Player.credits);

        var speedUpContainer = speedUpTemplate.find('.sContent');
        var optionTemplate = speedUpTemplate.find('.sOption').first();
        speedUpTemplate.find('.sOption').remove();
        var t = Utils.timeLeft(firstQItem.completionTime);
        var time = t.h + ":" + t.m + ":" + t.s;
        var countDownSpeedup = speedUpTemplate.find('.sTimer').addClass(CONST.CssClass.countdownTimer)
            .attr('data-finisheson', firstQItem.completionTime).attr("data-totalMS", firstQItem.upgradeDuration);
        var timeLeft = ROE.Utils.convertCountdownDisplayToTimeLeft(time);
        var speedUpsAvail = ROE.Utils.UpgradeSpeedUp.calculateFinishNow(timeLeft.leftMin, _village.upgrade.villageinfo_minSpeedUpAllowed);
        var finishNowCost =  speedUpsAvail.cost ; 

        //the speedup rewards panel
        $('.speedUpItemRewards', speedUpTemplate).remove();
        if (ROE.Items2.existsItemOfType('buildingspeedup')) {
            $('.sHeader', speedUpTemplate).after($('<div class="speedUpItemRewards fontSilverFrLCmed" ' +
                'onclick="ROE.Items2.showPopup(null,\'buildingspeedup\'); $(\'.speedUpOptionsPopup\').remove();">' +
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
            $('.sHeader', speedUpTemplate)
                .after('<div class="notEnoughSpeedUp fontSilverFrLCmed">' + speedupRemainingText + '</br>Resets at 04:00 AM UTC</div>');
        }

        var newOption, cost, count = 1;
        if (speedUpsAvail.allowFinishNow) {
            //FinishNow speedup option 
            newOption = optionTemplate.clone();
            newOption.find('.sName').html(BDA.Dict['spell_name_speedupFinishnow']);
            newOption.find('.sCut').html(BDA.Dict['qb_SpeedUp_CutFinishNow']);
            newOption.find('.sServants').html(finishNowCost + " " + BDA.Dict['qb_SpeedUp_ServantFinishNow']);
            newOption.find('.speedUpButton').attr('data-minToCut', 9999).attr('onclick', 'ROE.Building2.doSpeedUp($(this));');
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
                newOption.find('.speedUpButton').attr('data-minToCut', sp).attr('onclick', 'ROE.Building2.doSpeedUp($(this));');
                speedUpContainer.append(newOption);
            }
        }
    }

    function _doSpeedUp(btn) {
        ROE.UI.Sounds.click();
        $('.speedUpOptionsPopup.building2').find('.sBusy').show();
        var minToCut = btn.attr('data-mintocut');
        var eventID = DATA.Q.UpgradeEvents[0].eventID;
        ROE.Api.call_upgrade_speedupupgrade2(_villageID, null, eventID, minToCut, _doSpeedUpUpgradeReturn);
       
    }

    function _doSpeedUpUpgradeReturn(data) {
        Frame.free($('#buildingDialog'));
        ROE.Player.refresh(); // update points. 

        //if village changed before call came back, discard data
        if (_villageID != data.Village.Village.id) {
            return;
        }

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        if (data.cutSuccessful) {
            ROE.Frame.refreshPFHeader(data.playersCreditsNow);
            ROE.Building2.changesMade_RequireVOVReload = true;
            updateSpeedUpPopup("upgrade", data); //works in M, redundant in D2 untill issue noted below is addressed.
            ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village);
            _sync(_village);
        } else {
            $('.speedUpOptionsPopup.building2').remove();
            Frame.infoBar(ROE.Utils.phrase('BuildingPhrases', 'SpeedUpCastFail'));
            //ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village);
        }

        //_sync(_village);

    }

    function updateSpeedUpPopup(from, data) {
        ROE.UI.Sounds.clickSpell();
        var speedUpPopup = $('.speedUpOptionsPopup.building2');
        speedUpPopup.find('.sBusy').hide();

        if (data.Village.upgrade.Q.UpgradeEvents[0] && _buildingEventID == data.Village.upgrade.Q.UpgradeEvents[0].ID) {
            DATA.Q.UpgradeEvents = data.Village.upgrade.Q.UpgradeEvents;
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
                    $("#building_popup2 .speedUpButton").click();
                });
                speedUpPopup.find('.sContent').append(nextItemBtn);
            }
        }
    }

    function _speedUpCompleted() {
        $('.speedUpOptionsPopup.building2').remove();
    }

    function _doSpeedUpFree(eId) {
        //take in a passed in eventId OR the current upgrading building's event ID
        var eventId = eId || _buildingEventID;
        if (!eventId) { return; }
        Frame.busy('Completing the building...', 5000, $('#buildingDialog'));
        ROE.Api.call_upgrade_speedupupgradefree(_villageID, eventId, _doSpeedUpFreeReturn);
    }

    function _doSpeedUpFreeReturn(data) {
        Frame.free($('#buildingDialog'));
        ROE.Player.refresh(); // update points. 

        //if village changed before call came back, discard data
        if (_villageID != data.Village.Village.id) {
            return;
        }

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        if (data.cutSuccessful) {
            $('.sProgressContainer', _container).addClass('freeFinished').append('<div class="finishedText">Finished</div>');
            $('.progressPanel .sTimeBar,.progressPanel .cancelUpgrade', _container).remove();
            $('.speedUpFreeButton', _container).hide();

            //a bit of time to let the player enjoy his actions
            setTimeout(function () {
                ROE.Frame.refreshPFHeader(data.playersCreditsNow);
                ROE.Building2.changesMade_RequireVOVReload = true;
                ROE.Villages.__populateExtendedData(data.Village.id, _village, data.Village);
                _sync(_village);
            }, 1000);

        } else {
            $('.speedUpOptionsPopup.building2').remove();
            Frame.infoBar('Free speedup unsuccessfull.');
        }
    }

    //the freespeedup function for when X amount of time remaining.
    function _freeUpgradeReady() {

        //disabled for RX, also disabled in backend
        if (ROE.rt == "X") { return; }

        $('.speedUpButton', _container).hide(); //hide the standard speedup button
        $('.speedUpFreeButton', _container).fadeIn(); //show the free button
        $('.speedUpOptionsPopup.building2').remove(); //remove speeduppop if currently open
        $('.qSpeedUp', _container).hide(); //hide the Q view speedup button
    }



    /* END OF SPEEDUP FUNCTIONS*/

    function buildingBackground(buildingId) {
        var id = buildingId,
            bgImage = "";
        switch (id) {
            case 1://barracks
                bgImage = "https://static.realmofempires.com/images/backgrounds/M_BG_Barracks.jpg";
                break;
            case 2://Stable
                bgImage = "https://static.realmofempires.com/images/backgrounds/M_BG_Stable.jpg";
                break;
            case 3://headquarters
                bgImage = "https://static.realmofempires.com/images/misc/M_BG_HQ.jpg";
                break;
            case 4://wall
                bgImage = "https://static.realmofempires.com/images/misc/M_BG_Wall.jpg";
                break;
            case 5://silvermine
                bgImage = "https://static.realmofempires.com/images/misc/M_BG_Mine.jpg";
                break;
            case 6://treasury
                bgImage = "https://static.realmofempires.com/images/misc/M_BG_Treasury.jpg";
                break;
            case 7://defensive towers
                bgImage = "https://static.realmofempires.com/images/misc/M_BG_Tower.jpg";
                break;
            case 8://farmland
                bgImage = "https://static.realmofempires.com/images/backgrounds/M_BG_Farm.jpg";
                break;
            case 9://palace
                bgImage = "https://static.realmofempires.com/images/backgrounds/M_BG_Palace.jpg";
                break;
            case 10://siege workshop
                bgImage = "https://static.realmofempires.com/images/backgrounds/M_BG_SeigeWorkshop.jpg";
                break;
            case 11:// trading post
                bgImage = "https://static.realmofempires.com/images/backgrounds/M_BG_TradingPost.jpg";
                break;
            case 12://tavern
                bgImage = "https://static.realmofempires.com/images/backgrounds/M_BG_Tavern.jpg";
                break;
            case 13://hiding spot
                bgImage = "https://static.realmofempires.com/images/misc/M_BG_HidingSpot.jpg";
                break;
        }
        return bgImage;
    }

    function buildingPopupIconUrl(buildingId) {
        return ROE.Entities.BuildingTypes[buildingId].IconUrl_ThemeM;
    }

    var _reInitContent = function () {
        ///<summary> when called, this will make the necessary api calls to reinitialize the popup</summary>
        _slideView = "default";
        _sync();
    }

    var _reInitContentWithDifferentVillage = function (v) {
        ///<summary> when called, this will make the necessary api calls to reinitialize the popup, for a different village</summary>
        
        //switching villages should wipe the speedup panel
        $('.speedUpOptionsPopup.building2').remove();

        if (v) {
            _sync(v);
        } else {
            _villageID = ROE.SVID;
            _sync();
        }       
    }

    var _slideToAlternateView = function (page) {
        _slideView = "alternate";
        ROE.UI.Sounds.click();
        _container.find(CONST.Selector.defaultPage).removeClass('slideLeftTo').addClass('slideLeftFrom');
        _container.find(CONST.Selector.alternate).removeClass('slideRightFrom').addClass('slideRightTo').find('.alternateSubPage ').removeClass('active');//.hide();
        _container.find(page).addClass('active');
    }

    var _slideBackToDefaultView = function () {
        _slideView = "default";
        ROE.UI.Sounds.click();
        _container.find(CONST.Selector.defaultPage).removeClass('slideLeftFrom').addClass('slideLeftTo');
        _container.find(CONST.Selector.alternate).removeClass('slideRightTo').addClass('slideRightFrom');
    }

    var _goToRequirementBuilding = function (btn, buildingId) {
        if (btn.hasClass('reqMet')) { return; }
        $('#popup_Building2 .IFrameDivTitle .action.close').click();
        _showBuildingPagePopup(buildingId);
    }

    function _handleMapSelectedVillageChangedEvent() {
        if ($("#buildingDialog").dialog("isOpen") === true && _villageID != ROE.SVID) {
            _syncForSelectedVillage(ROE.SVID);
        }
    }

    function _handleVillageExtendedInfoUpdatedOrPopulated(village) {
        if (!isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent) {
            if ((ROE.Frame.isPopupOpen("#buildingDialog") || ROE.Frame.isPopupOpen("#popup_Building2")) && _villageID == village.id) {
                if (village.changes.buildings
                || village.changes.buildingWork
                || village.changes.coins
                || village.changes.popRem
                || village.changes.name) {
                        _sync(village);
                }
            }
        }
        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = false;
    }

    function _rebelLootPopup() {
        ROE.UI.Sounds.click();
        var icon = "https://static.realmofempires.com/images/map/freb.png";
        var content = '<div class="fontGoldFrLClrg" style="text-align: justify; padding: 30px;">' +
            'You can attack and loot Rebel villages for their silver. Beware, Rebels will defend themselves. You need a strong force to overcome them, or you will lose your forces.' +
            '</div>';
        ROE.Frame.simplePopopOverlay(icon, 'Looting Rebels', content, 'lootRebels', $('#buildingDialog').parent());
    }


    obj.doUpgradeBuilding = _doUpgradeBuilding;
    obj.doCancelUpgrade = _doCancelUpgrade;
    obj.doCancel = _doCancel;
    obj.upgradeProgressComplete = _upgradeProgressComplete;
    obj.showSpeedUpOptions = _showSpeedUpOptions;
    obj.doSpeedUp = _doSpeedUp;
    obj.doSpeedUpFree = _doSpeedUpFree;
    obj.freeUpgradeReady = _freeUpgradeReady;
    obj.speedUpCompleted = _speedUpCompleted;
    obj.slideToAlternateView = _slideToAlternateView;
    obj.slideBackToDefaultView = _slideBackToDefaultView;
    obj.showBuildingPagePopup = _showBuildingPagePopup;
    obj.reInitContent = _reInitContent;
    obj.reInitContentWithDifferentVillage = _reInitContentWithDifferentVillage;
    obj.changesMade_RequireVOVReload = false;
    obj.goToRequirementBuilding = _goToRequirementBuilding;
    obj.selectUnitDetail = _selectUnitDetail;
    obj.rebelLootPopup = _rebelLootPopup;

}(window.ROE.Building2 = window.ROE.Building2 || {}));



//the reason this is done after, is because phrases themselves need to be in teh dom for this function to work
//$('#building_popup .buildDescription').html(ROE.Utils.phrase('BuildingPhrases', 'desc_bid' + _buildingID));
