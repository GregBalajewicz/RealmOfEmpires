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
    var Timer = window.BDA.Timer,
        Scrolling = window.BDA.UI.Scrolling,
        Transition = window.BDA.UI.Transition,
        Utils = window.BDA.Utils;
    
    var Api = window.ROE.Api,
        Entities = window.ROE.Entities,
        Frame = window.ROE.Frame,
        Player = window.ROE.Player;

    var popupIcon = "https://static.realmofempires.com/images/icons/M_QRecruit.png";
    var CONST = { popupName: "QuickRecruit" },
        CACHE = {};
    
    var _UIDisplayMode; // "Standard" || "Compact"
    var _storedScroll = 0;
    var _pageViewMode = "recruit"; // this is the view mode, can be: "recruit" | "disband"
    var lastQueueBuildingId = null;
    var isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = false;

    CONST.Selector = {      
        summaryItemHolder: ".recruitSummary .unitsSummaryHolder",
        recruitPanelContainer: ".recruitPanelContainer",
        disbandContainer: '.recruitPanelContainer .disbandContainer',
        recruitPanel: '.recruitPanelContainer .recruitPanel',
        recruitAllRow: '.recruitPanelContainer .recruitAllRow',
        resourceFooter: ".resourceFooter.footer1",
        slideToQueuePage: '.slideToQueuePage',
        frameTitleLabel: '#popup_QuickRecruit .IFrameDivTitle .label',
        frameTitleIcon: '#popup_QuickRecruit .IFrameDivTitle .themeM-icon img',
        popupRecruitInput: '.quickRecruitInput.recruit.quickrecruit',
        popupDisbandInput: '.quickRecruitInput.disband.quickrecruit',
        popupContentRecruitInput: '.quickRecruitInput.recruit.quickrecruit .pContent',
        popupContentDisbandInput: '.quickRecruitInput.disband.quickrecruit .pContent',
        popupContentMassChestBuy: '.massChestPopup.quickrecruit .pContent',
        massbuyLog: '.massChestPopup.quickrecruit .pContent .massBuyLog',
        switchModeArea:'.switchModeArea',
        Q: {
            queueContainer: ".queueContainer",
            queuedItemRow: ".queuedItemRow",
            queuedItemsArea: '.queuedItemsArea',
            qTime: '.qTime'
        },
        templates: {
            recruitPanel: '.recruitPanel.template',
            unitSection: '.unitSection.template',
            disbandUnitSection:'.disbandUnitSection.template',
            massChestSection: '.massChestSection.template',
            unitSectionGov: '.unitSectionGov.template',
            queuedItemRow: '.queuedItemRow.template',
            switchModeArea: '.switchModeArea.template',
            recruitAllRow: '.recruitAllRow.template',
            chestBuy: '.chestSection.template'
        }
    }; 

    CONST.CssClass = {
        countdown: "countdown"
    }
    CACHE.Selector = { };
    DATA = { };

    var _templates = {};
    var _buildingID;
    var _village;
    var _villageID;
    var _villageIDPassedFromMap;
    var _villageList; ///list of all villages for mass chest buy
    var _massChestIconUrl = "https://static.realmofempires.com/images/icons/M_Icon_MultiChest.png";
    var _villtoMassBuyIndex = 0;
    var _totalChestsBought = 0;

    var _showThisPopup = function (launchedFrom, selectedVID) {
        _storedScroll = 0;
        _pageViewMode = "recruit";
        _UIDisplayMode = ROE.LocalServerStorage.get('UIDisplayMode') || 'Standard';
        if (ROE.isMobile) { _UIDisplayMode = 'Standard'; } //force standard template for M
        ROE.QuickRecruit.changesMade_RequireVOVReload = false;
        ROE.QuickRecruit.launchedFrom = launchedFrom;    //if launchedFrom is == "vov" in M it reloads VOV upon close when changes are made 
        if (selectedVID) {
            _villageIDPassedFromMap = selectedVID;
        } else {
            _villageIDPassedFromMap = null;
        }
        _load();
    }

    function _load() {        

        if (!CACHE.template) {
            Frame.busy('Loading Recruitment Plans...', 10000, $('#quickRecruitDialog'));
            var temp;
            if (ROE.isMobile || _UIDisplayMode == 'Standard') {
                temp = BDA.Templates.getRawJQObj("QuickRecruitTempl", ROE.realmID);
            } else {
                temp = BDA.Templates.getRawJQObj("QuickRecruitTempl_d2", ROE.realmID);
            }
            Frame.free($('#quickRecruitDialog'));
            CACHE['template'] = temp;
        }

        CONST.Selector.defaultPage = "#quickRecruitDialog .themeM-view.default";
        CONST.Selector.queuePage = "#quickRecruitDialog .themeM-view.queuepage";

        _templates["main"] = CACHE['template'].clone();
        _templates["recruitPanel"] = _templates["main"].find(CONST.Selector.templates.recruitPanel);
        _templates["unitSection"] = _templates["main"].find(CONST.Selector.templates.unitSection);
        _templates["disbandUnitSection"] = _templates["main"].find(CONST.Selector.templates.disbandUnitSection);
        _templates["unitSectionGov"] = _templates["main"].find(CONST.Selector.templates.unitSectionGov);
        _templates["massChestSection"] = _templates["main"].find(CONST.Selector.templates.massChestSection);
        _templates["queuedItemRow"] = _templates["main"].find(CONST.Selector.templates.queuedItemRow);
        _templates["switchModeArea"] = _templates["main"].find(CONST.Selector.templates.switchModeArea);
        _templates["recruitAllRow"] = _templates["main"].find(CONST.Selector.templates.recruitAllRow);
        _templates["chestBuy"] = _templates["main"].find(CONST.Selector.templates.chestBuy);
        
        $('#quickRecruitDialog').dialog('open');
        _sync();
    }      


    function _sync(optionalVillage) {
        Frame.busy('Looking for runaway conscripts...', 10000, $('#quickRecruitDialog'));
        ROE.vov.pauseAnimation();
        if (_villageIDPassedFromMap) {
            _villageID = _villageIDPassedFromMap;
        } else {
            _villageID = ROE.SVID;
        }
        ROE.QuickRecruit.villageID = _villageID;
        if (optionalVillage) {
            _sync_onDataSuccess(optionalVillage);
        } else {
            ROE.Villages.getVillage(_villageID, _sync_onDataSuccess, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
        }
    }

    function _syncForSelectedVillage(selectedVillageID) {
        Frame.busy('Looking for runaway conscripts...', 10000, $('#quickRecruitDialog'));
        _villageID = ROE.QuickRecruit.villageID = selectedVillageID;
        ROE.Villages.getVillage(_villageID, _sync_onDataSuccess, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
    }

    function _sync_onDataSuccess(village) {
        Frame.free($('#quickRecruitDialog'));
        _village = village;
        DATA = village.recruit;
        _populate();       
    }

    function _sync_onDataFailure(data) {
    }

    function _populate() {
        //console.log('_populate', _village);

        _storedScroll = $(CONST.Selector.recruitPanelContainer).scrollTop();
        var popupContent = $('#quickRecruitDialog');
        BDA.Broadcast.subscribe(popupContent, "CurrentlySelectedVillageChanged", _handleCurrentlySelectedVillageChangedEvent);
        BDA.Broadcast.subscribe(popupContent, "VillageExtendedInfoUpdated", _handleVillageExtendedInfoUpdatedOrPopulated);

        var content = _templates.main;
        var qContainer = content.find(CONST.Selector.Q.queueContainer).empty();
        var recruitInfoArray = DATA.recruitInfo.recruitInfoList;
        var buildingRecruitInfo, recruitables, recruitPanelDiv, buildingId, buildingEntity, buildingName,
            buildingRecruitPanel, panelData, unitSection, recUnit, recMax, rec25, rec50, unitId, unitEntity, unitData,
            qPreviewArea, aNewQueueRow, buildingQ, quId, quEntity, qIcon, qCount, qData, canRecCode;
        var summaryItemHolder = content.find(CONST.Selector.summaryItemHolder).empty();
        var recruitPanelContainer = content.find(CONST.Selector.recruitPanelContainer).empty();
        content.find(CONST.Selector.switchModeArea).remove(); //remove switchmodearea 
        var disbandContainer = $('<div>').addClass('disbandContainer');
        recruitPanelContainer.append(disbandContainer);

        for (var b = 0; b < recruitInfoArray.length; b++) {
            
            buildingRecruitInfo = recruitInfoArray[b];           
            recruitables = buildingRecruitInfo.unitsRecruited;
            //skip buildings with no unit production
            if (recruitables.length < 1) { continue; }
                      
            buildingId = buildingRecruitInfo.buildingTypeID;
            buildingEntity = Entities.BuildingTypes[buildingId];
            buildingName = buildingEntity.Name;

            if (recruitables[0].canRecruit == "1") {
                recruitPanelContainer.append('<div class="recruitPanel fontSilverFrLClrg unbuilt" data-buildingid="' + buildingId + '"><div class="grayband"></div><div class="lockImage"></div><div class="buildingName">' + buildingName + '</div><div class="buildingState">Unbuilt</div></div>');
                continue;
            }

            panelData = {
                pId: buildingId,
                pName: buildingName,
                pIcon: buildingEntity.IconUrl_ThemeM
            }

            buildingRecruitPanel = $(BDA.Templates.populate(_templates.recruitPanel[0].outerHTML, panelData)).removeClass('template');
            


            if (buildingId == "9") {
                //console.log('govInfo:', DATA.recruitInfo.govInfo);

                var govData, chestData;
                var govInfo = DATA.recruitInfo.govInfo;
                var chestSilverCost = govInfo.chestCost;
                var currentChests = ROE.Player.chestCount;
                var costOfNextGov = govInfo.chestCostForNextGoverner;
                var govFoodCost = govInfo.foodNeeded;
                var govRecruitTime = govInfo.recruitTime;
                var maxChestsBuyable = govInfo.maxChests;
                var chestIcon = govInfo.chestIconThemeM;
                var isPalaceBuilt = govInfo.isPalaceBuilt;

                ///Add a Gov section
                recUnit = recruitables[0];
                unitId = recUnit.uid;
                unitEntity = Entities.UnitTypes[unitId];
                canRecCode = recUnit.canRecruit;
                govData = {
                    uId: unitId,
                    uName: unitEntity.Name,
                    uIcon: unitEntity.IconUrl_ThemeM,
                    uCanRec: canRecCode,
                    uCurrentCount: ROE.Utils.addThousandSeperator(recUnit.currentCount),
                    costOfNextGov: costOfNextGov,
                    enoughChests: costOfNextGov > currentChests ? "notEnoughChests" : "enoughChests",
                    uGovFoodCost: govFoodCost,
                    enoughFood: _village.popRemaining < govFoodCost ? "notEnoughFood" : "enoughFood"
                }

                ///Add to summary section
                _addThisUnitToSummary(summaryItemHolder, unitEntity.IconUrl_ThemeM, ROE.Utils.addThousandSeperator(recUnit.currentCount));
                ///Add to recruit section
                $(BDA.Templates.populate(_templates.unitSectionGov[0].outerHTML, govData)).removeClass('template').appendTo(buildingRecruitPanel);

                ///Govs can NOT be disbanded. backend will ignore the command.
                //$(BDA.Templates.populate(_templates.disbandUnitSection[0].outerHTML, govData)).removeClass('template').appendTo(disbandContainer);

                ///Add a Chest section
                recMax = isPalaceBuilt ? maxChestsBuyable : 0; //if no palace built, show 0 chests on buttons
                rec25 = Math.ceil(recMax * 0.25);
                rec50 = Math.ceil(recMax * 0.5);
                chestData = {
                    uId: "chest",
                    uName: 'Chests',
                    uIcon: chestIcon,
                    uCanRec: isPalaceBuilt ? '0' : 'noPalace',
                    uCurrentCount: currentChests,
                    uRec25: ROE.Utils.addThousandSeperator(rec25),
                    uRec50: ROE.Utils.addThousandSeperator(rec50),
                    uRecMax: ROE.Utils.addThousandSeperator(recMax)
                }
                if (_UIDisplayMode == 'Compact') {
                    $(BDA.Templates.populate(_templates.chestBuy[0].outerHTML, chestData)).removeClass('template').addClass('unitSection').appendTo(buildingRecruitPanel);
                } else {
                    $(BDA.Templates.populate(_templates.unitSection[0].outerHTML, chestData)).removeClass('template').appendTo(buildingRecruitPanel);
                }

                ///Add Mass Chest Buy section
                chestData = {
                    uIcon: _massChestIconUrl
                }
                $(BDA.Templates.populate(_templates.massChestSection[0].outerHTML, chestData)).removeClass('template').appendTo(buildingRecruitPanel);

            } else {

                for (var u = 0; u < recruitables.length; u++) {
                    recUnit = recruitables[u];
                    recMax = recUnit.maxToRecruit;
                    rec25 = Math.ceil(recMax * 0.25);
                    rec50 = Math.ceil(recMax * 0.5);
                    unitId = recUnit.uid;
                    unitEntity = Entities.UnitTypes[unitId];
                    canRecCode = recUnit.canRecruit;

                    unitData = {
                        uId: unitId,
                        uName: unitEntity.Name,
                        uIcon: unitEntity.IconUrl_ThemeM,
                        uCanRec: canRecCode,
                        uCurrentCount: ROE.Utils.addThousandSeperator(recUnit.currentCount),
                        uRec25: ROE.Utils.addThousandSeperator(rec25),
                        uRec50: ROE.Utils.addThousandSeperator(rec50),
                        uRecMax: ROE.Utils.addThousandSeperator(recMax),
                    }

                    if (canRecCode == '2') {
                        if (recUnit.unsatisfiedReq.length) {
                            unitData.lockedDueTo = 'missingBuildingLevel';
                            unitData.missingBuilding = "Requires " + ROE.Entities.BuildingTypes[recUnit.unsatisfiedReq[0].btid].Name + "(" + recUnit.unsatisfiedReq[0].level+")";
                        } else if (recUnit.unsatisfiedReqRes.length) {
                            unitData.lockedDueTo = 'missingResearch';
                        }
                    }

                    ///Add to summary section
                    _addThisUnitToSummary(summaryItemHolder, unitEntity.IconUrl_ThemeM, ROE.Utils.addThousandSeperator(recUnit.currentCount));
                    ///Add to recruit section
                    $(BDA.Templates.populate(_templates.unitSection[0].outerHTML, unitData)).removeClass('template').appendTo(buildingRecruitPanel);

                    ///Add to disband section if any of such units currently home
                    unitData.uYourUnitsCurrentlyInVillageCount = _village._TroopsDictionary['id' + unitId].YourUnitsCurrentlyInVillageCount;
                    if (unitData.uYourUnitsCurrentlyInVillageCount > 0) {
                        unitData.uYourUnitsCurrentlyInVillageCount = ROE.Utils.addThousandSeperator(unitData.uYourUnitsCurrentlyInVillageCount);
                        $(BDA.Templates.populate(_templates.disbandUnitSection[0].outerHTML, unitData)).removeClass('template').appendTo(disbandContainer);
                    }

                }
            }


            
            //SETUP BUILDINGS RECRUIT Q
            buildingQ = buildingRecruitInfo.q;
            qPreviewArea = buildingRecruitPanel.find(CONST.Selector.Q.queuedItemsArea);

            if (buildingQ.length < 1) {
                buildingRecruitPanel.find(CONST.Selector.slideToQueuePage).addClass('grayout');
            } else {
                for (var qI = 0; qI < buildingQ.length; qI++) {
                    quId = buildingQ[qI].uid;
                    quEntity = Entities.UnitTypes[quId];
                    qIcon = quEntity.IconUrl_ThemeM;
                    qCount = ROE.Utils.addThousandSeperator(buildingQ[qI].count);
                    if(buildingQ[qI].timeLeft){
                        qTime = Utils.timeLeft(buildingQ[qI].timeLeft);
                        qTime = qTime.h + ":" + qTime.m + ":" + qTime.s;
                    }else{
                        qTime = buildingQ[qI].timeFormatted; 
                    }
                    qData = {
                        quId: quId,
                        qCount: qCount,
                        qIcon: qIcon,
                        qName: quEntity.Name,
                        qBID: buildingId,
                        qEventId: buildingQ[qI].qEntryID,
                        qTime: qTime
                    }

                    

                    try {
                        aNewQueueRow = $(BDA.Templates.populate(_templates.queuedItemRow[0].outerHTML, qData)).removeClass('template').appendTo(qContainer);
                    } catch (ex) {
                        var roeex = new BDA.Exception("QR: case 26614, somehow queueRow isnt defined? ", ex);
                        roeex.data.add('qData', qData);
                        roeex.data.add('quId', quId);
                        roeex.data.add('_templates', _templates);
                        roeex.data.add('_templates.queuedItemRow[0].outerHTML', _templates.queuedItemRow[0].outerHTML);
                        BDA.latestException = roeex;
                        throw roeex;
                    }


                    if (qI == 0) {
                        //only countdown the first item in the queue page
                        aNewQueueRow.find(CONST.Selector.Q.qTime).addClass(CONST.CssClass.countdown);
                    }
                    if ((_UIDisplayMode == 'Compact' && qI < 5) || qI < 3) {
                        qPreviewArea.append('<span class="queuedItem fontWhiteNumbers" style="background-image:url(\'' + qIcon + '\');">' + qCount + '</span>');
                    }
                }

                if ((_UIDisplayMode == 'Standard' && buildingQ.length > 3) || buildingQ.length > 5) {
                    qPreviewArea.append('<span class="queuedMore");">...</span>');
                }
            }

            recruitPanelContainer.append(buildingRecruitPanel);
        }

        //grayout all recruit buttons that have 0 for display
        recruitPanelContainer.find('.unitSection').find('.unitRecruitCommands').find('.rcNum').each(function () {
            if (parseInt(this.innerHTML) == 0) {
                $(this).parent().addClass('grayout');
            }
        });

        //Recruit All, appended before Palace
        if (_UIDisplayMode == 'Compact') {
            $('.recruitPanel[data-buildingid="9"]', recruitPanelContainer).before($(BDA.Templates.populate(_templates.recruitAllRow[0].outerHTML, {})).removeClass('template'));
            recruitPanelContainer.after($(BDA.Templates.populate(_templates.switchModeArea[0].outerHTML, {})).removeClass('template'));
        } else {
            recruitPanelContainer.append($(BDA.Templates.populate(_templates.switchModeArea[0].outerHTML, {})).removeClass('template'));
        }

        content = $(content);
        popupContent.html(content);

        ROE.FooterWidget.init(content.find(CONST.Selector.resourceFooter), _village, "ROE.QuickRecruit.reInitContent();");

        if (_pageViewMode == "disband") {
            _setViewDisband();
        } else {
            _setViewRecruit();
        }

        $(CONST.Selector.recruitPanelContainer).scrollTop(_storedScroll);
        _filterQueueForBuilding();
        Timer.initTimers();
    }

    var _addThisUnitToSummary = function (container, icon, count) {
        container.append('<div class="summaryItem" style="background-image:url(\'' + icon + '\')"><span class="count">' + count + '</span></div>');
    }

    var _reInitContentWithDifferentVillage = function (v) {
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

    //when entering queue page, must show only the items for a particular building ID
    var _filterQueueForBuilding = function () {
        $(CONST.Selector.queuePage).find(CONST.Selector.Q.queuedItemRow).hide().each(function () {
            if ($(this).find('.qCancel').attr('data-bid') == lastQueueBuildingId) {
                $(this).show();
            }
        });
    }

    var _slideToQueuePage = function (btn) {
        if (btn.hasClass('grayout')) { return; }        
        lastQueueBuildingId = btn.attr('data-buildingid');
        _filterQueueForBuilding();
        $(CONST.Selector.frameTitleLabel).text(Entities.BuildingTypes[lastQueueBuildingId].Name + " Queue");
        $(CONST.Selector.frameTitleIcon).attr('src', Entities.BuildingTypes[lastQueueBuildingId].IconUrl_ThemeM);
        $(CONST.Selector.defaultPage).removeClass('slideLeftTo').addClass('slideLeftFrom');
        $(CONST.Selector.queuePage).removeClass('slideRightFrom').addClass('slideRightTo');
    }

    var _slideBackToDefaultPage = function () {
        $(CONST.Selector.frameTitleLabel).text(ROE.Utils.phrase('QuickRecruitPhrases', 'QuickRecruit'));
        $(CONST.Selector.frameTitleIcon).attr('src', popupIcon);
        $(CONST.Selector.defaultPage).removeClass('slideLeftFrom').addClass('slideLeftTo');
        $(CONST.Selector.queuePage).removeClass('slideRightTo').addClass('slideRightFrom');
    }


    ///RECRUITMENT FUNCTIONS

    function _openRecruitInputField(btn) {
        ROE.UI.Sounds.click();
        var panel = btn.parentsUntil('.unitSection').parent();
        var canCode = panel.attr('data-ucanrec');
        if (canCode != "0") { return; }
        var unitId = panel.attr('data-unitid');
        var uName = panel.attr('data-unitName');
        var uIcon = panel.attr('data-unitIcon');
        var uRecMax = panel.attr('data-uRecMax');
        Frame.simplePopopOverlay(uIcon, uName, "", "quickRecruitInput recruit quickrecruit", $('#quickRecruitDialog').parent());

        var popupContent = $(CONST.Selector.popupContentRecruitInput);
        $('<div class="recruitIcon" style="background-image:url(\'' + uIcon + '\')">').appendTo(popupContent);
        $('<div class="recruitName fontSilverFrSClrg" >').html(uName).appendTo(popupContent);
        $('<div class="recruitmaxLabel fontGoldFrLClrg">').html(ROE.Utils.phrase('QuickRecruitPhrases', 'Max')).appendTo(popupContent);
        $('<div class="recruitmaxNumber fontSilverNumbersSm">').html(uRecMax).appendTo(popupContent);
        var input = $('<input class="recruitAmount" type="number" min="0" >').appendTo(popupContent);
        var recruitButton = $('<div>').addClass('recruitAmountBtn BtnBSm2 fontSilverFrSClrg');

        if(unitId == "chest"){
            recruitButton.html(ROE.Utils.phrase('QuickRecruitPhrases', "Buy")).click(function () {
                ROE.UI.Sounds.click();
                var amount = parseInt(input.val().replace(/,/g, ''));
                if (isNaN(amount)) {
                    input.val(0);
                    return;
                } else if (amount < 0) {
                    // No cashing out on chest stocks!
                    input.val(0);
                    return;
                } else if (amount > uRecMax) {
                    input.val(uRecMax);
                    return;
                }
                $(CONST.Selector.popupRecruitInput).remove();
                Frame.busy(ROE.Utils.phrase('QuickRecruitPhrases', 'FillingUpChests'), 7000, $('#quickRecruitDialog'));
                ROE.Api.call_gov_buychest2(_villageID, amount, 1, _syncDoRecruitTroops_onDataSuccess);
            });
        }else{
            recruitButton.html(ROE.Utils.phrase('QuickRecruitPhrases', 'Recruit')).click(function () {
                ROE.UI.Sounds.click();
                var amount = parseInt(input.val().replace(/,/g, ''));
                if (isNaN(amount)) {
                    input.val(0);
                    return;
                } else if (amount > uRecMax) {
                    input.val(uRecMax);
                    return;
                }
                var recruitCommands = [{ uid: unitId, count: amount }];
                $(CONST.Selector.popupRecruitInput).remove();
                Frame.busy(ROE.Utils.phrase('QuickRecruitPhrases', 'Recruiting'), 7000, $('#quickRecruitDialog'));
                ROE.Api.call_recruit_dorecruit2(_villageID, $.toJSON(recruitCommands), _syncDoRecruitTroops_onDataSuccess);
            });
        }
        
        recruitButton.appendTo(popupContent);
        input.select().focus();

    }

    function _recruitAll(btn) {
        var recruitCommands = [];
        btn.parent().siblings('.recruitPanel[data-buildingid!="9"]').children('.unitSection').each(function () {
            var canCode = $(this).attr('data-ucanrec');
            if (canCode != "0") { return; }
            var unitId = $(this).attr('data-unitid');
            var amount = parseInt($(this).children('.unitRecruitCommands').children('.recruitAmount').val().replace(/,/g, ''));
            if (!isNaN(amount) && amount > 0) {
                recruitCommands.push({ uid: unitId, count: amount });
            }
        });
        if (recruitCommands.length > 0) {
            Frame.busy(ROE.Utils.phrase('QuickRecruitPhrases', 'Recruiting'), 7000, $('#quickRecruitDialog'));
            ROE.Api.call_recruit_dorecruit2(_villageID, $.toJSON(recruitCommands), _syncDoRecruitTroops_onDataSuccess);
        }
    }

    function _doRecruit(btn) {
        ROE.UI.Sounds.click();
        var canCode = btn.parentsUntil('.unitSection').parent().attr('data-ucanrec');
        if (canCode != "0") { return; }   
        var unitId = btn.parentsUntil('.unitSection').parent().attr('data-unitid');
        var amount = 0;
        if (btn.hasClass('chestBuy')) {
            var input = btn.siblings('.recruitAmount');
            var uRecMax = parseInt(btn.siblings('.BtnDSm2n').find('.rcNum').html().replace(/,/g, ''));
            amount = parseInt(input.val().replace(/,/g, ''));
            if (isNaN(amount)) {
                input.val(0);
                return;
            } else if (amount > uRecMax) {
                input.val(uRecMax);
                return;
            }
        } else {
            amount = parseInt(btn.find('.rcNum').html().replace(/,/g, ''));
        }
        if (unitId == "chest") {
            Frame.busy(ROE.Utils.phrase('QuickRecruitPhrases', 'FillingUpChests'), 7000, $('#quickRecruitDialog'));
            ROE.Api.call_gov_buychest2(_villageID, amount, 1, _syncDoRecruitTroops_onDataSuccess);
        } else {           
            Frame.busy(ROE.Utils.phrase('QuickRecruitPhrases', 'Recruiting'), 7000, $('#quickRecruitDialog'));
            var recruitCommands = [{ uid: unitId, count: amount }];
            ROE.Api.call_recruit_dorecruit2(_villageID, $.toJSON(recruitCommands), _syncDoRecruitTroops_onDataSuccess);
        }

    }

    function _doRecruitSingleGov(btn) {
        ROE.UI.Sounds.click();
        var parent = btn.parentsUntil('.unitSectionGov').parent();
        var notEnoughChests = parent.find('.unitRecruitCommands').hasClass('notEnoughChests');
        var canCode = parent.attr('data-ucanrec');
        if (notEnoughChests || canCode != "0") { return; }
        var unitId = parent.attr('data-unitid');
        Frame.busy(ROE.Utils.phrase('QuickRecruitPhrases', 'PreparingGovernor'), 7000, $('#quickRecruitDialog'));
        var recruitCommands = [{ uid: unitId, count: 1 }];
        ROE.Api.call_recruit_dorecruit2(_villageID, $.toJSON(recruitCommands), _syncDoRecruitTroops_onDataSuccess);
    }

    function _syncDoRecruitTroops_onDataSuccess(data) {
        Frame.free($('#quickRecruitDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.id) {
            return;
        }

        ROE.Player.chestCount = data.recruit.recruitInfo.govInfo.availableChests;

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;        
        if (data.hasOwnProperty('cannotUpgrade')) { //what is the QR fail indicator here?
            alert('QR Rec Fail not handled.');
        } else {
            ROE.QuickRecruit.changesMade_RequireVOVReload = true;
            ROE.Villages.__populateExtendedData(data.id, _village, data);

            _sync(_village);
        }
    }

    function _doCancelRecruit(btn) {
        ROE.UI.Sounds.click();
        var eventID = btn.attr('data-eventId');
        var buildingID = btn.attr('data-bid');
        Frame.busy('Canceling...', 10000, $('#quickRecruitDialog'));
        ROE.Api.call_recruit_cancel2(_villageID, buildingID, eventID, _cancelRecruitReturn);
    }

    function _cancelRecruitReturn(data) {
        Frame.free($('#quickRecruitDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.id) {
            return;
        }

        ROE.QuickRecruit.changesMade_RequireVOVReload = true;
        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;

        //for canceling govs, need updated chest count.
        ROE.Player.chestCount = data.recruit.recruitInfo.govInfo.availableChests;

        ROE.Villages.__populateExtendedData(data.id, _village, data);
        _sync(_village);
    }


    //// MASS CHEST BUY SECTION 

    function _massChestPopup() {
        Frame.busy(ROE.Utils.phrase('QuickRecruitPhrases', 'PreparingVillageList'), 9000, $('#quickRecruitDialog'));
        Frame.simplePopopOverlay(_massChestIconUrl, "Mass Chest Buy", "<p>" + ROE.Utils.phrase('QuickRecruitPhrases', 'PreparingVillageList') + ".</p>", "massChestPopup quickrecruit", $('#quickRecruitDialog').parent());
        ROE.Villages.getVillages(_listIsReady);
    }

    function _listIsReady(listOfVillages) {      
        _villageList = listOfVillages;
        var popupContent = $(CONST.Selector.popupContentMassChestBuy);
        popupContent.html("<p class='note fontGoldFrLCmed'>Input an amount of silver to keep in each village, or leave 0 to buy Max chests in all villages.</p>");
        var keepCoinsInput = $('<input type="text" class="keepCoins">').val(0).appendTo(popupContent);
        var startMassBuyBtn = $('<div>').addClass('startMassBuyBtn BtnBLg1 fontSilverFrSClrg').html(ROE.Utils.phrase('QuickRecruitPhrases', 'StartMassBuy'))
        .click(function () { _startMassBuy(popupContent); }).appendTo(popupContent);
        Frame.free($('#quickRecruitDialog'));
    }


    function _startMassBuy(popupContent) {

        var silverToLeave = parseInt($('.keepCoins', popupContent).val());
        if (isNaN(silverToLeave) || silverToLeave < 0) {
            $('.massChestPopup .keepCoins').val(0);
            return;
        }
        if (silverToLeave > 999999999) {
            $('.massChestPopup .keepCoins').val(999999999);
            return;
        }
        $(".simplePopupOverlay.massChestPopup").data('silverToLeave', silverToLeave);

        popupContent.html('<div class="vProgressBar"><div class="vProgress" style="width:0%;"></div><div class="vProgressMessage fontGoldFrLCmed"></div></div>');
        popupContent.append('<div class="sectionDivider"></div><div class="massBuyLog"></div><div class="sectionDivider"></div>');

        var stopMassBuyBtn = $('<div>').addClass('stopMassBuyBtn BtnBSm2 fontSilverFrSClrg').html(ROE.Utils.phrase('QuickRecruitPhrases', 'Stop'))
            .click(function () { _stopMassBuy(popupContent); }).appendTo(popupContent);

        _villtoMassBuyIndex = 0;
        _totalChestsBought = 0;
        _massBuyInNextVillage();
        
    }

    function _stopMassBuy(popupContent) {
        popupContent.addClass('stop').find('.stopMassBuyBtn').remove();
        popupContent.append('<p class="stoppedNote fontGoldFrLClrg">'+ROE.Utils.phrase('QuickRecruitPhrases', 'StoppingMassChestBuy')+'</p>');
    }

    function _massBuyFinished(popupContent) {
        popupContent.find('.stopMassBuyBtn').remove();
        popupContent.append('<p class="fontGoldFrLClrg">' + _totalChestsBought + ' Chests bought from ' + _villageList.length + ' Villages.</p>');
        popupContent.find('.vProgressMessage').html('<p>'+ROE.Utils.phrase('QuickRecruitPhrases', 'MassBuyCompleted')+'</p>');
        ROE.Villages.getVillage(_village.id, _sync, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
    }

    function _massBuyInNextVillage() {
        
        var popupContent = $(CONST.Selector.popupContentMassChestBuy);
        var progressPercent = Math.ceil(_villtoMassBuyIndex / _villageList.length * 100);
        popupContent.find('.vProgress').css('width', progressPercent + '%');

        ///this means the popup was closed
        if (popupContent.length < 1) {        
            ROE.Villages.getVillage(_village.id, _sync, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
            return;
        }

        ///this means user hit stop button
        if(popupContent.hasClass('stop')){        
            popupContent.find('.stoppedNote').html(ROE.Utils.phrase('QuickRecruitPhrases', 'MassChestBuyStopped'));
            ROE.Villages.getVillage(_village.id, _sync, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
            return;
        }

        ///this means all villages were visited
        if (_villtoMassBuyIndex >= _villageList.length) {           
            _massBuyFinished(popupContent);
            return;
        }

        var nextVillObj = _villageList[_villtoMassBuyIndex];
        var vId = nextVillObj.id;
        var vName = nextVillObj.name;

        popupContent.find('.vProgressMessage').html('<p>' + ROE.Utils.phrase('QuickRecruitPhrases', 'BuyingChestsIn') + ' ' + vName + '</p>');
        
        var silverToLeave = $(".simplePopupOverlay.massChestPopup").data('silverToLeave');
        if (silverToLeave == 0) {
            ROE.Api.call_gov_buychest2(vId, undefined, 1, _massBuySuccessReturn);
        } else {
            ROE.Api.call_gov_buychest_leavesilver(vId, silverToLeave, 1, _massBuySuccessReturn);
        }
        
    }

    function _massBuySuccessReturn(data) {
        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        var visitedVillageObj = _villageList[_villtoMassBuyIndex];
        ROE.Villages.__populateExtendedData(data.id, visitedVillageObj, data);
        var vId = visitedVillageObj.id;
        var vName = visitedVillageObj.name;
        var chestsBefore = ROE.Player.chestCount;
        var chestsAfter = data.recruit.recruitInfo.govInfo.availableChests;       
        var dif = chestsAfter - chestsBefore;
        _totalChestsBought += dif;
        var log = $(CONST.Selector.massbuyLog);
        log.append('<p class="fontSilverFrSCmed">' + vName + ' <span class="fontSilverNumbersMed">+' + dif + '</span> (' + _totalChestsBought + ') </p>');
        log.scrollTop(99999);
        ROE.Player.chestCount = chestsAfter;
        _villtoMassBuyIndex++;
        _massBuyInNextVillage();
    }

    //// END OF MASS CHEST BUY SECTION


    function _switchMode(toMode) {
        var switchModeArea = $(CONST.Selector.switchModeArea);
        if( toMode == "toRecruit"){
            if (switchModeArea.hasClass('leftActive')) { return; }
            _setViewRecruit();
        } else if (toMode == "toDisband") {
            if (switchModeArea.hasClass('rightActive')) { return; }
            _setViewDisband();
        }
        $(CONST.Selector.recruitPanelContainer).scrollTop(99999);
        ROE.UI.Sounds.click();
    }

    function _setViewRecruit() {
        _pageViewMode = "recruit";
        $(CONST.Selector.switchModeArea).removeClass('rightActive').addClass('leftActive');
        $(CONST.Selector.disbandContainer).hide();
        $(CONST.Selector.recruitPanel).show();
        $(CONST.Selector.recruitAllRow).show();
        $(CONST.Selector.frameTitleLabel).text(ROE.Utils.phrase('QuickRecruitPhrases', 'QuickRecruit'));
    }

    function _setViewDisband() {
        _pageViewMode = "disband";
        $(CONST.Selector.switchModeArea).removeClass('leftActive').addClass('rightActive');
        $(CONST.Selector.recruitPanel).hide();
        $(CONST.Selector.recruitAllRow).hide();
        $(CONST.Selector.disbandContainer).show();
        $(CONST.Selector.frameTitleLabel).text(ROE.Utils.phrase('QuickRecruitPhrases', 'Disband'));
    }

    /* DISBAND SECTION */

    function _openDisbandInputField(btn) {

        ROE.UI.Sounds.click();
        var panel = btn.parentsUntil('.disbandUnitSection').parent();
        //var canCode = panel.attr('data-ucanrec');
        //if (canCode != "0") { return; }
        var unitId = panel.attr('data-unitid');
        var uName = panel.attr('data-unitName');
        var uIcon = panel.attr('data-unitIcon');
        var uCurrentCount = panel.attr('data-uYourUnitsCurrentlyInVillageCount');
        Frame.simplePopopOverlay(uIcon, 'Disband ' + uName, "", "quickRecruitInput disband quickrecruit", $('#quickRecruitDialog').parent());

        var popupContent = $(CONST.Selector.popupContentDisbandInput);
        $('<div class="recruitIcon" style="background-image:url(\'' + uIcon + '\')">').appendTo(popupContent);
        $('<div class="recruitName fontSilverFrSClrg" >').html(uName).appendTo(popupContent);
        $('<div class="recruitmaxLabel fontGoldFrLClrg">').html(ROE.Utils.phrase('QuickRecruitPhrases', 'Max')).appendTo(popupContent);
        $('<div class="recruitmaxNumber fontSilverNumbersSm">').html(uCurrentCount).appendTo(popupContent);
        var input = $('<input class="recruitAmount" type="number" min="0" >').appendTo(popupContent);
        var recruitButton = $('<div>').addClass('recruitAmountBtn BtnBSm2 fontSilverFrSClrg');

        recruitButton.html(ROE.Utils.phrase('QuickRecruitPhrases', 'Disband')).click(function () {
            ROE.UI.Sounds.click();
            var amount = parseInt(input.val().replace(/,/g, ''));
            if (isNaN(amount)) {
                input.val(0);
                return;
            } else if (amount > uCurrentCount) {
                input.val(uCurrentCount);
                return;
            }
            var recruitCommands = [{ uid: unitId, count: amount }];           
            _doDisband(recruitCommands);           
        });

        recruitButton.appendTo(popupContent);
        input.select().focus();
    }

    function _maxDisband(btn) {
        ROE.UI.Sounds.click();

        var unitId = btn.parentsUntil('.disbandUnitSection').parent().attr('data-unitid');
        var amount = parseInt(btn.find('.rcNum').html().replace(/,/g, ''));
        if (amount < 1) { return; }
        var recruitCommands = [{ uid: unitId, count: amount }];

        _doDisband(recruitCommands);
    }

    function _doDisband(recruitCommands) {

        ROE.Frame.Confirm("Disbanding is permanent. Are you sure?", 'Yeap', 'Nope', 'rgba(0,0,0,0.8)', function () {
            $(CONST.Selector.popupDisbandInput).remove();
            Frame.busy(ROE.Utils.phrase('QuickRecruitPhrases', 'Disbanding'), 7000, $('#quickRecruitDialog'));
            ROE.Api.call_disband(_villageID, $.toJSON(recruitCommands), _doDisband_onDataSuccess);
        }, undefined, undefined, true);

    }

    function _doDisband_onDataSuccess(data) {
        Frame.free($('#quickRecruitDialog'));

        //if village changed before call came back, discard data
        if (_villageID != data.Village.id) {
            return;
        }

        isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
        ROE.QuickRecruit.changesMade_RequireVOVReload = true;
        ROE.Villages.__populateExtendedData(data.id, _village, data);
        _sync(_village);
    }



    /* END OF DISBAND SECTION */




    function _handleCurrentlySelectedVillageChangedEvent() {
        if ($("#quickRecruitDialog").dialog("isOpen") === true && _villageID != ROE.SVID) {
            _villageIDPassedFromMap = null;
            _syncForSelectedVillage(ROE.SVID);
        }
    }


    function _handleVillageExtendedInfoUpdatedOrPopulated(village) {
        if (!isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent) {
            if ((ROE.Frame.isPopupOpen("#quickRecruitDialog") || ROE.Frame.isPopupOpen("#popup_QuickRecruit")) && _villageID == village.id) {
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

    function _UIDisplayModeChanged() {       
        _UIDisplayMode = ROE.LocalServerStorage.get('UIDisplayMode');
        CACHE.template = null;
        $("#quickRecruitDialog").dialog({ width: _UIDisplayMode == 'Standard' ? 375 : 432 });
        if ($("#quickRecruitDialog").dialog("isOpen") === true) {
            _load();
        }
    }

    obj.maxDisband = _maxDisband;
    obj.doDisband = _doDisband;
    obj.openDisbandInputField = _openDisbandInputField;
    obj.switchMode = _switchMode;
    obj.massChestPopup = _massChestPopup;
    obj.doCancelRecruit = _doCancelRecruit;
    obj.recruitAll = _recruitAll;
    obj.doRecruit = _doRecruit;
    obj.doRecruitSingleGov = _doRecruitSingleGov;
    obj.openRecruitInputField = _openRecruitInputField;
    obj.slideToQueuePage = _slideToQueuePage;
    obj.slideBackToDefaultPage = _slideBackToDefaultPage;
    obj.showPopup = _showThisPopup;
    obj.reInitContent = _reInitContent;
    obj.reInitContentWithDifferentVillage = _reInitContentWithDifferentVillage;
    obj.changesMade_RequireVOVReload = false;
    obj.UIDisplayModeChanged = _UIDisplayModeChanged;
    
}(window.ROE.QuickRecruit = window.ROE.QuickRecruit || {}));



/*      
    Can rec Codes:
    Yes: 0,
    No_RecruitmentBuildingNotBuilt: 1,
    No_RequirementsNotSatisfied: 2,
    No_LackSilver: 3,
    No_LackFood: 4
*/