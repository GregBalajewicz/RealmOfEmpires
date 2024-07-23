"use strict";

(function (obj) {
    BDA.Console.setCategoryDefaultView('ROE.UI.VillageList', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file

    var CONST = {};
    CONST.Enum = {
       
    };

    CONST.CssClass = {
        fullyPopulated : "jsFullyPopulated"
    };

    CONST.Selectors = {
       
        villageList: function () { return ROE.isMobile ? "#villageListPopup .villageList" : "#villageListPopup .villageList tbody" }
        ,InVillageList : {
            oneVillage: "#villageListPopup .villageList .villlist_itemlist"
            , oneVillageByID: function (villageID) { return ".villlist_itemlist[data-villistid=%vid%]".format(({vid: villageID})) } // only applicable to _container.find(...)
        }
        , InOneVillageRow : {
            food : ".food"
        }

    }

    var _template;
    var _templateOneVillage;
    var _templateFilterSettings;
    var _container;
    var _filterSettings = {
        troopsA: localStorage.getItem('villageList_settings_troopsA') === "ON",
        troopsY: localStorage.getItem('villageList_settings_troopsY') === "ON",
        troopsC: localStorage.getItem('villageList_settings_troopsC') === "ON",
        troopsS: localStorage.getItem('villageList_settings_troopsS') === "ON",
        buildings: localStorage.getItem('villageList_settings_buildings') === "ON",
        buildingBusyTime: localStorage.getItem('villageList_settings_buildingBusyTime') === "ON",
        villageNameSearchString: '',
        recruitingTroopNumbers:localStorage.getItem('villageList_settings_recruitingTroopNumbers') === "ON",
    }
    
    function _init(container) {
        ROE.Frame.busy("Reading scrolls, checking maps...", undefined, container);
        _container = container;

        BDA.Broadcast.subscribe(_container, "VillageExtendedInfoUpdated", _handleVillageExtendedInfoUpdatedOrPopulated);
        BDA.Broadcast.subscribe(_container, "VillageExtendedInfoInitiallyPopulated", _handleVillageExtendedInfoUpdatedOrPopulated);
        BDA.Broadcast.subscribe(_container, "CurrentlySelectedVillageChanged", _handleSelectedVillageChangedEvent);

        // we should really handle this events so that we could display the incoming indicators however, this event does not say which village to change, so we'd need to 
        //  rebuilt the entire list and we do not want to do this for now
        //BDA.Broadcast.subscribe(_container, "InOutgoingDataChanged", _InOutgoingChanged);

        ROE.Villages.getVillages(function (villageList) { _displayVillageList(container, villageList); });
    }

    function _handleSelectedVillageChangedEvent() {
        if (_container.dialog("isOpen") === true) {
            $(CONST.Selectors.InVillageList.oneVillage).removeClass('selected');
            $(CONST.Selectors.InVillageList.oneVillageByID(ROE.SVID)).addClass('selected');
        }
    }

    function _displayVillageList(container, villageList) {

        if (!_template) {
            if (ROE.isMobile) {
                _template = BDA.Templates.getRawJQObj("VillageList_m", ROE.realmID);
            } else {
                _template = BDA.Templates.getRawJQObj("VillageList_d2", ROE.realmID);
            }
            _templateOneVillage = _template.find(CONST.Selectors.InVillageList.oneVillage).remove();
            _templateFilterSettings = _template.find('.filterandsettings').remove();
        }

        var contentHTML;
        var VillageId = ROE.SVID;
        var len = villageList.length;
        var body = "";

        for (var i = 0, vill; (vill = villageList[i]) ; ++i) {

            if (_filterSettings.villageNameSearchString) {
                if (vill.name.toLowerCase().indexOf(_filterSettings.villageNameSearchString.toLowerCase()) == -1) {
                    continue;
                }                
            }

            // if the object does not have extended properties, then trigger getting them
            if (!vill.areExtendedPropertiesAvailable && !ROE.isMobile) {
                ROE.Villages.ExtendedInfo_loadLatest(vill.id);
            }

            body += _getOneVillageRowPopulated(vill);
            
        }

        if (len == 1) {
            var getmorevillage = "To get more villages build a Palace and recruit governors needed to conquer other villages.";
            body += "<div class=getmorevill >" + getmorevillage + "</div>";
            body += "<div class='villseparator'><img src='https://static.realmofempires.com/images/misc/m_listbar.png'></div>";
        }

        contentHTML = _template;
        contentHTML.find(CONST.Selectors.villageList()).html(body);
        container.html(contentHTML);
        container.find('.villlist_refresh').click(ROE.UI.VillageList.refreshVillList);
        container.find('.filtersCmdBtn').click(_handleFilterButtonClick);
        container.find('.massfeature').click(_handleMassFeatureClick);
        container.find('.filterListBtn').click(_searchByName);
        container.append(_templateFilterSettings.clone());

        container.find('.villageList').on('click', '.villListName', _handleVillageNameClicked);
        container.find('.villageList').on('click', '.jsV', _handleVillageClicked);



        //setup filterSettings UI Checkboxes
        BDA.UI.CheckBox.init($(".filterandsettings .troopsA", container), function (state) { _updateFilterSettings('troopsA', state) },
            "Show A: All troops in this village now", _filterSettings.troopsA);
        BDA.UI.CheckBox.init($(".filterandsettings .troopsY", container), function (state) { _updateFilterSettings('troopsY', state) },
            "Show Y: Your troops from this village", _filterSettings.troopsY);
        BDA.UI.CheckBox.init($(".filterandsettings .troopsC", container), function (state) { _updateFilterSettings('troopsC', state) },
            "Show C: Your troops from this village, Currently home", _filterSettings.troopsC);
        BDA.UI.CheckBox.init($(".filterandsettings .troopsS", container), function (state) { _updateFilterSettings('troopsS', state) },
            "Show S: Support troops at this village", _filterSettings.troopsS);
        BDA.UI.CheckBox.init($(".filterandsettings .buildings", container), function (state) { _updateFilterSettings('buildings', state) },
            "Show building details", _filterSettings.buildings);
        BDA.UI.CheckBox.init($(".filterandsettings .buildingBusyTime", container), function (state) { _updateFilterSettings('buildingBusyTime', state) },
            "include busy time", _filterSettings.buildingBusyTime);
        BDA.UI.CheckBox.init($(".filterandsettings .recruitingTroopNumbers", container), function (state) { _updateFilterSettings('recruitingTroopNumbers', state) },
            "include recruiting troop numbers", _filterSettings.recruitingTroopNumbers);


        _affectFilterSettings();

        //TODO: implement scrolling to selected village
        if (ROE.isMobile) {

            ROE.Frame.smartLoadInit({
                containerSelector: '#villageListPopup .villageList',
                itemSelector: '.villlist_itemlist',
                itemsMadeVisible: {
                    keyAttribute: 'data-villistid',
                    callback: _handleItemsMadeVisibleEvent
                },
                loadSize: 10
            });

        }
        

        // If the user presses enter within the filter input,
        // then do the same action as hitting the filter button.
        container.find('.filterSearchString').keypress(function _keyPressFilterReportsInput(e) {
            if (!e) e = window.event;
            var keyCode = e.keyCode || e.which;
            if (keyCode == '13') {
                // Enter pressed
                _searchByName();
                return false;
            }                      
        });

        ROE.Frame.free(container);


        //TABLE HACK: to solve scrollable table head puzzle
        //we clone table, and use the clone as a dynamic header mask
        if (ROE.isD2) {

            //rebuild and clone table after every build
            var villageListHeader = $('.villageListHeader', container).empty().append($('.villageList table', container).clone());
            var villageListsWrapper = $('.villageListsWrapper', container).prepend(villageListHeader);

            //get the height of the village list table header, so we can both apply it as masking of header,
            //we absolute position this table on top of the original table like a mask
            _d2HeaderMagicReflow();

            //to keep header Y position properly at all scroll times
            villageListsWrapper.on('scroll', function () {
                villageListHeader.css('top', villageListsWrapper.scrollTop() + 'px');
            });
        }

        container.find('.dataHeaderRow th').click(_tableHeadClicked);


        //remove old mass features
        container.find('.massfeatures').remove();
        
        
    }

    //get the height of the village list table header, so we can both apply it as masking of header,
    //we absolute position this table on top of the original table like a mask
    function _d2HeaderMagicReflow() {
        if (ROE.isD2) { //D2 only for now
            //timeout to make sure its after css reflow
            window.setTimeout(function () {
                $('.villageListHeader', _container).css('height', $('.villageList thead', _container).height());
                $('.villageListHeader', _container).css('top', $('.villageListsWrapper', _container).scrollTop() + 'px');
            }, 100);
        }
    }

    function _searchByName(event) {
        var filterPhrase = $('#villageListPopup .filterSearchString').val();
        // Has to be an actual phrase to continue
        if (typeof (filterPhrase) === "undefined" || filterPhrase === null) {
            filterPhrase = "";
        }

        if (filterPhrase != _filterSettings.villageNameSearchString) {
            _filterSettings.villageNameSearchString = filterPhrase;
            ROE.Villages.getVillages(function (villageList) { _displayVillageList(_container, villageList); });
        }
    }
    

    function _handleMassFeatureClick(event) {
        //D2 massfeatures opening in dialogIframe
        ROE.Frame.popGeneric($(this).attr('data-title'), "", 600, 500);
        ROE.Frame.showIframeOpenDialog('#genericDialog', $(this).attr('data-href'));
    }

    function _handleFilterButtonClick() {

        /*
            only need the content passed at minimum, the rest are optional -farhad

            !!!NOTE: if no 'appendTo' is passed, the quickPopup is constructued and appened to body
            which means the content will then reside in it, in the body, from then on
            for best practices use an ID for the content selector
            example: instead of $(".QuickCommandPopup .filterandsettings"), itd be best to just do $("#quickCommandSettings")
            -farhad
        */

        ROE.Frame.quickPopup({
            content: _container.find(".filterandsettings"),
            appendTo: _container,
            title: 'Filter and Settings',
            icon: 'https://static.realmofempires.com/images/icons/M_FilterB.png',
            customQuickPopupContainerClass: 'villageListFilterSettings',
            closeFunction: function () { /*something cool*/ }
        });

    }



    //when column head clicked, sort rows by that columns data
    function _tableHeadClicked() {

        var $tbody = $('.villageList table tbody', _container);
        var dataHeader = $(this);
        var ColumnIndex = dataHeader.index();

        //setup per column orderby, default to desc
        var _orderBy = 'desc';
        if (dataHeader.hasClass('asc')) {
            dataHeader.removeClass('asc').addClass('desc');
            _orderBy = 'desc';
        } else if (dataHeader.hasClass('desc')) {
            dataHeader.removeClass('desc').addClass('asc');
            _orderBy = 'asc';
        } else {
            $('.villageListHeader th.asc').removeClass('asc');
            $('.villageListHeader th.desc').removeClass('desc');
            dataHeader.addClass('desc');
            _orderBy = 'desc';
        }

        //sort the TRs based on the selected column's data
        $tbody.find('tr').sort(function (a, b) {
            var tda = $(a).find('td:eq(' + ColumnIndex + ')').text().trim();
            var tdb = $(b).find('td:eq(' + ColumnIndex + ')').text().trim();

            //take out commas, then parseint and if valid, use that value to compare (numbers sort better than strings)
            tda = (tda == "-" ? "0" : tda);
            var tdaParsed = parseInt(tda.replace(new RegExp(",", "g"), ""));
            if (!isNaN(tdaParsed)) { tda = tdaParsed; }

            tdb = (tdb == "-" ? "0" : tdb);
            var tdbParsed = parseInt(tdb.replace(new RegExp(",", "g"), ""));
            if (!isNaN(tdbParsed)) { tdb = tdbParsed; }

            //ascending or descening sort
            if (_orderBy == 'asc') {
                return tda > tdb ? 1 : tda < tdb ? -1 : 0;
            } else {
                return tda < tdb ? 1 : tda > tdb ? -1 : 0;
            }

        }).appendTo($tbody);

        //toggle order for next time
        if (_orderBy == 'asc') {
            _orderBy = 'desc';
        } else {
            _orderBy = 'asc';
        }

    }
    
    function _handleItemsMadeVisibleEvent(arrayOfVillageIDs) {
        for (var i = 0; i < arrayOfVillageIDs.length; i++) {
            if (!$(CONST.Selectors.InVillageList.oneVillageByID(arrayOfVillageIDs[i])).hasClass(CONST.CssClass.fullyPopulated)) {
                ROE.Villages.ExtendedInfo_loadLatest(arrayOfVillageIDs[i]);
            }
        }
    }

    function _handleVillageExtendedInfoUpdatedOrPopulated(vill) {
        if ($("#villageListDialog").dialog("isOpen") === true) {
            BDA.Console.log('ROE.UI.VillageList', "in _handleVillageExtendedInfoUpdatedOrPopulated, vid:" + vill.id);
            var onVillageListRow = _container.find(CONST.Selectors.InVillageList.oneVillageByID(vill.id));
            onVillageListRow.html($(_getOneVillageRowPopulated(vill)).html());
            onVillageListRow.addClass(CONST.CssClass.fullyPopulated);
        }
    }

    function _getOneVillageRowPopulated(vill) {
        var villageType = ROE.Entities.VillageTypes[vill.villagetypeid];
        var data;
        data = {
            villageImageUrl: BDA.Utils.GetMapVillageIconUrl(vill.points, vill.villagetypeid)
            , villageName: vill.name
            , x: vill.x
            , y: vill.y
            , id: vill.id
            , currentVillageClass: ROE.SVID == vill.id ? "selected" : ""
            , points: ROE.Utils.formatNum(vill.points)
            , yey: vill.yey
            , yeyClass: vill.yey < 100 ? "under100" : ""
            , popRemaining: vill.popRemaining == undefined ? "<span class=notYetLoaded>?</span>" : ROE.Utils.formatNum(vill.popRemaining)
            , coins: vill.coins == undefined ? "<span class=notYetLoaded>?</span>" : ROE.Utils.formatNum(vill.coins)
            , coinsClass: vill.coins >= vill.coinsTresMax ? "full" : ""
            , bonusType: villageType.IsBonus ? villageType.LargeIconUrl : ""
            , foodMaxed: _isFarmAtLastLevel(vill) ? "maxed" : ""
            , incomingImages: _getImcomingIndicators(vill)
            //, isRecruiting: _isRecruiting(vill) ? "isRecruiting" : ""
            , troops: {}
            , buildings: {}
        }


        // troops in village
        var troopInfo;
        for (var i = 0, unitTypeID; unitTypeID = ROE.Entities.UnitTypes.SortedList[i]; i++) {
                                                                                                                  
            if (vill.areExtendedPropertiesAvailable) {
                troopInfo = vill.TroopByID(unitTypeID);
                data.troops['id' + unitTypeID] = {
                    YourUnitsCurrentlyInVillageCount: BDA.Utils.formatNum(troopInfo.YourUnitsCurrentlyInVillageCount),
                    YourUnitsCurrentlyInVillageCount_font: troopInfo.YourUnitsCurrentlyInVillageCount == 0 && troopInfo.CurrentlyRecruiting == 0 ? "zero" : "",
                    YourUnitsTotalCount: BDA.Utils.formatNum(troopInfo.YourUnitsTotalCount),
                    YourUnitsTotalCount_font: troopInfo.YourUnitsTotalCount == 0 ? "zero" : "",
                    TotalNowInVillageCount: BDA.Utils.formatNum(troopInfo.TotalNowInVillageCount),
                    TotalNowInVillageCount_font: troopInfo.TotalNowInVillageCount == 0 ? "zero" : "",
                    SupportCount: BDA.Utils.formatNum(troopInfo.SupportCount),
                    SupportCount_font: troopInfo.SupportCount == 0 ? "zero" : "",
                };
                if (troopInfo.CurrentlyRecruiting) {
                    data.troops['id' + unitTypeID].CurrentlyRecruiting = "(" + troopInfo.CurrentlyRecruiting + ")";
                }
            } else {
                data.troops['id' + unitTypeID] = {
                    YourUnitsCurrentlyInVillageCount: "<span class=notYetLoaded>?</span>",
                    YourUnitsTotalCount: "<span class=notYetLoaded>?</span>",
                    TotalNowInVillageCount: "<span class=notYetLoaded>?</span>",
                    SupportCount: "<span class=notYetLoaded>?</span>",
                };
            }
        }

        // buildings in village
        var thisBuilding, maxLevel, curLevel, nextLevel, displayLevel;
        for (var i = 0, bid; bid = ROE.Entities.BuildingTypes.SortedList[i]; i++) {
           
            if (vill.areExtendedPropertiesAvailable) {
                
                thisBuilding = vill.Buildings[bid];
                maxLevel = ROE.Entities.BuildingTypes[bid].MaxLevel;
                curLevel = thisBuilding.curLevel;
                displayLevel = curLevel;

                if (thisBuilding.Upgrade.nextLevel) {
                    nextLevel = thisBuilding.Upgrade.nextLevel.levelNum;
                    if ((nextLevel - 1) > curLevel) {
                        displayLevel = "<span class='upgrading'>" + (nextLevel - 1) + "</span>" ;
                    }
                } else if (curLevel != maxLevel){
                    displayLevel = "<span class='upgrading'>" + maxLevel + "</span>" ;
                }

                data.buildings['id' + bid] = {
                    lvl: displayLevel == 0 ? "-" :displayLevel ,
                    lvl_font: displayLevel == 0 ? "zero" : "",
                };




                var busyTimeInMS=0;

                //if (bid == 3
                //    && vill.upgrade.Q.totalUpgradeTime > 0) // TODO use constant
                //{
                //    data.buildings['id' + bid].upgradeTimeDisplay = '<span class=buildingBusyTime>(%t%h)</span>'.format({ t: Math.round((vill.upgrade.Q.totalUpgradeTime / 1000 / 60 / 60) * 10) / 10 });
                //} else {
                busyTimeInMS = _calculateBuildingBusyTime(vill, bid);
                if (busyTimeInMS > 0) {
                    data.buildings['id' + bid].busyTimeDisplay = '<span class=buildingBusyTime>(%t%h)</span>'.format({ t: Math.round((busyTimeInMS / 1000 / 60 / 60) * 10) / 10 });
                }
                //}

                //if (bid == 1) {
                //    data.buildings['id' + bid].work = Math.round((vill.upgrade.Q.totalUpgradeTime / 1000 / 60 / 60) * 10) / 10;
                //}
                
            } else {
                data.buildings['id' + bid] = { lvl: "<span class=notYetLoaded>?</span>" };
            }
        }
        

        //if (vill.upgrade) {
        //    data['isBuilding'] = vill.upgrade.Q.UpgradeEvents.length ? "isBuilding" : "";
        //}

        return BDA.Templates.populate(_templateOneVillage[0].outerHTML, data);
    }
    
    function _calculateBuildingBusyTime(village, bid) {// should be part of village object
        var buildingRecruitInfo;
        var recruitQ;
        var busyTimeInMS=0;

        if (bid == 3) {
            busyTimeInMS = Math.max(0, village.upgrade.Q.totalUpgradeTime)
        } else {

            buildingRecruitInfo = $.grep(village.recruit.recruitInfo.recruitInfoList, function (e) { return e.buildingTypeID == bid; });
            if (buildingRecruitInfo.length == 1) {
                recruitQ = buildingRecruitInfo[0].q;
                busyTimeInMS = 0;
                if (recruitQ.length > 0) {
                    busyTimeInMS += new Date(recruitQ[0].timeLeft) - new Date();
                    for (var i = 1; i < recruitQ.length; i++) {
                        busyTimeInMS += recruitQ[i].time;
                    }
                }
            }
        }

        return busyTimeInMS;
    }


    function _getImcomingIndicators(v) {
        var list = ROE.Troops.InOut2.getIncomingMiniSummary(v.id);
        var indicators = ROE.Utils.getImcomingMiniIndicatorsHTML(list, 2);
        var indicatorsTooltip = ROE.Utils.getImcomingMiniIndicatorsHTML(list, 100);
        return indicatorsTooltip;
    }


    function _isFarmAtLastLevel(v){
        if (v.areExtendedPropertiesAvailable) {
            if (v.Buildings[8].Upgrade.nextLevel == null) {
                return true;
            }
        }

        return false;
    }

    //function _isRecruiting(v) {
    //    if (v.recruit) {
    //        var recruitInfoList = v.recruit.recruitInfo.recruitInfoList;
    //        for (var i = 0; i < recruitInfoList.length; i++){
    //            if (recruitInfoList[i].q.length) { return true; }
    //        }
    //    }
    //    return false;
    //}


    function _handleVillageClicked(event) {
        if (ROE.isMobile) { return; }
       // if (ROE.isMobile) {
            var villageItem = $(event.currentTarget);
            _switchVillage(villageItem);
        //}
    }
    function _handleVillageNameClicked(event) {
        if (ROE.isMobile) { return; }
        var villageItem = $(event.currentTarget);
        villageItem = $(villageItem.parents('.villlist_itemlist')[0]);
       
        _switchVillage(villageItem);
      //  event.stopPropagation();
    }

    function _switchVillage(villageItem) {
        var id, x, y;
        id = parseInt(villageItem.attr('data-villistid'), 10);
        x = parseInt(villageItem.attr('x'), 10);
        y = parseInt(villageItem.attr('y'), 10);
        ROE.Frame.setCurrentlySelectedVillage(id, x, y);
        if (ROE.isMobile) {
            $('#villageListDialog').dialog('close');
        }
    }



    var _refreshVillList = function () {
        ROE.Frame.busy("Yes Sire, checking maps again!", undefined, _container);
        ROE.Villages.clearCache();
        ROE.Villages.getVillages(function (villageList) { _displayVillageList(_container, villageList); });
    };

    function _affectFilterSettings() {
        var villagePopup = $('#villageListPopup');
        //toggle classfails, do with ifs
        if (_filterSettings.troopsA) {
            villagePopup.addClass('showTroopsA');
        } else {
            villagePopup.removeClass('showTroopsA');
        }

        if (_filterSettings.troopsY) {
            villagePopup.addClass('showTroopsY');
        } else {
            villagePopup.removeClass('showTroopsY');
        }

        if (_filterSettings.troopsC) {
            villagePopup.addClass('showTroopsC');
        } else {
            villagePopup.removeClass('showTroopsC');
        }

        if (_filterSettings.troopsS) {
            villagePopup.addClass('showTroopsS');
        } else {
            villagePopup.removeClass('showTroopsS');
        }
        if (_filterSettings.buildings) {
            villagePopup.addClass('showBuildings');
        } else {
            villagePopup.removeClass('showBuildings');
        }
        if (_filterSettings.buildingBusyTime) {
            villagePopup.addClass('showBuildingBusyTime');
        } else {
            villagePopup.removeClass('showBuildingBusyTime');
        }
        if (_filterSettings.recruitingTroopNumbers) {
            villagePopup.addClass('showrecruitingTroopNumbers');
        } else {
            villagePopup.removeClass('showrecruitingTroopNumbers');
        }

        _d2HeaderMagicReflow();

    }

    function _updateFilterSettings(setting, state) {
        _filterSettings[setting] = state; 
        localStorage.setItem('villageList_settings_' + setting, (state ? "ON" : "OFF"));
        _affectFilterSettings();
    }

    obj.refreshVillList = _refreshVillList;
    obj.handleVillageClicked = _handleVillageClicked;
    obj.init = _init;

} (window.ROE.UI.VillageList = window.ROE.UI.VillageList || {}));