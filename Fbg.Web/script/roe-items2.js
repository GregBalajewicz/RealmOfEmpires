"use strict";

(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {
    BDA.Console.setCategoryDefaultView('ROE.I2', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file
   
    var Api = window.ROE.Api,
        Entities = window.ROE.Entities,
        Frame = window.ROE.Frame,
        Player = window.ROE.Player;

    var CONST = { popupName: "items2" },
        CACHE = {};


    CONST.Selector = {
        dialog: '#items2Dialog'
    };



    CONST.CssClass = {
        countdown: "countdown",
    };

    CACHE.Selector = {};
    var DATA = {};
    var _templates = {};
    var _villageID;
    var _village;
    var _container;

    var _filterCatType; //show only items of this type
    var _postAutoSelectCat; //if not null, it will pick this category after load

    var _showItemsPopup = function (selectedVID,postSelectCat) {

        if (selectedVID && typeof (selectedVID) == "number") {
            _villageID = selectedVID;
        } else {
            _villageID = ROE.SVID;
        }

        if (postSelectCat) { _postAutoSelectCat = postSelectCat; }
      
        _container = $(CONST.Selector.dialog).dialog('open');

        BDA.Broadcast.subscribe(_container, "CurrentlySelectedVillageChanged", _handleCurrentlySelectedVillageChangedEvent); 
        BDA.Broadcast.subscribe(_container, "VillageExtendedInfoUpdated", _handleVillageExtendedInfoUpdatedOrPopulated);

        _load();
    }

    function _load() {

        if (!CACHE.template) {
            Frame.busy('Loading Blueprints...', 5000, $(CONST.Selector.dialog));
            var temp;
            if (ROE.isMobile ) {
                temp = BDA.Templates.getRawJQObj("Items2", ROE.realmID);
            } else {
                temp = BDA.Templates.getRawJQObj("Items2", ROE.realmID);
            }
            Frame.free($(CONST.Selector.dialog));
            CACHE['template'] = temp;
        }

        _templates["main"] = CACHE['template'].clone();
    
        _sync();

    }


    function _sync(optionalVillage) {
        Frame.busy("Getting Village Info...", 10000, $(CONST.Selector.dialog));
        if (optionalVillage) {
            _sync_onDataSuccess(optionalVillage);
        } else {
            ROE.Villages.getVillage(_villageID, _sync_onDataSuccess, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
        }
    }

    function _sync_onDataSuccess(village) {
        Frame.free($(CONST.Selector.dialog));
        _village = village;
        _villageID = _village.id;
        _loadItems();
    }
   

    function _loadItems(forceGetLatest) {
        Frame.busy("Getting your rewards...", 10000, $(CONST.Selector.dialog));
        var getLatest = forceGetLatest ? '1':'0';
        Api.call_items2_myitemgroups(getLatest, _callback_items2_myitemgroups);
    }

    function _callback_items2_myitemgroups(data) {
        //console.log('_callback_items2_myitemgroups', data);
        Frame.free($(CONST.Selector.dialog));
        ROE.Player.itemGroups = data;
        _populate();

        if (_postAutoSelectCat) {
            _postPickCat();
        }
    }

    function _sync_onDataFailure(data) { }

    function _populate() {


        if (!$('#items2Dialog').dialog('isOpen')) {
            return;
        }

        var freshPop = true; //is this a fresh populate, or existing

        //will hold reward categories
        var catArray = [];

        if ($('.item',_container).length) { //Populate: Existing
           
            freshPop = false;

            var content = $('#items2', _container);
            var itemContainer = $('.itemsList', content);
            $('.tooMany', itemContainer).remove();

            var itemGroup, firstItem, itemElement;
            for (var i = 0; itemGroup = ROE.Player.itemGroups[i]; i++) {
                
                firstItem = itemGroup._firstItem;
                itemElement = $('.item[data-groupID="' + itemGroup.groupID + '"]', itemContainer);

                
                if (itemElement.length == 0) { //if element doesnt exist, create it
                    itemElement = $('<div class="item ' + firstItem.Type + '" data-groupID="' + itemGroup.groupID + '" style="background-image:url(' + _getItemIcon(firstItem) + ');" >' +
                        '<div class="text fontGoldFrLCmed ">' + firstItem.Text + '</div><div class="c">x' + itemGroup.count + '</div></div>');
                    itemElement.data('itemGroup', itemGroup);
                    itemContainer.append(itemElement);
                } else { //if element exists just update the data and count text
                    itemElement.data('itemGroup', itemGroup);
                    _updateElementCount(itemElement);
                }

                if (catArray.indexOf(firstItem.Type) == -1) {
                    catArray.push(firstItem.Type);
                }
                
            }

        } else { //Populate: Fresh
            
            freshPop = true;

            var content = _templates.main.clone();
            var itemContainer = $('.itemsList', content);
            
            var itemGroup, firstItem, itemElement;
            if (ROE.Player.itemGroups.length > 0) {
                for (var i = 0; itemGroup = ROE.Player.itemGroups[i]; i++) {

                    firstItem = itemGroup._firstItem;
                    itemElement = $('<div class="item ' + firstItem.Type + '" data-groupID="' + itemGroup.groupID + '" style="background-image:url(' + _getItemIcon(firstItem) + ');" >' +
                        '<div class="text fontGoldFrLCmed ">' + firstItem.Text + '</div><div class="c">x' + itemGroup.count + '</div></div>');
                    itemElement.data('itemGroup', itemGroup);
                    itemContainer.append(itemElement);

                    if (catArray.indexOf(firstItem.Type) == -1) {
                        catArray.push(firstItem.Type);
                    }
                }
            }

            content.on('click', '.item', _doUseItem);

            $('.reload', content).click(function () { _loadItems(true); });

            //$('.settings', content).click(function () { _openSettings(); });

            _container.html(content);
        }

        var categoryListElement = $('.categoryList').empty();
        var categoryItem;
        for (var i = 0; i < catArray.length; i++) {
            categoryItem = $('<div class="categoryItem" style="background-image:url(' + _getCategoryIcon(catArray[i]) + ')" data-type="' + catArray[i] + '">').click(function () { _catClick($(this)) });
            categoryListElement.append(categoryItem);
        }
           
        _villageEffectOnItems();

        if (freshPop) {
            _filterCatType = catArray[0];
        }

        _filterCatUpdate();
    }

    function _getItemIcon(item) {
        var iconUrl = "";

        if (item.Type == "silver") {
            iconUrl = "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png";
        } else if (item.Type == "troops") {
            iconUrl = ROE.Entities.UnitTypes[item.UnitType.unitTypeID].IconUrl_ThemeM;
        } else if (item.Type == "pfd") {
            iconUrl = ROE.PFPckgs[item.PFPackageID].icon_ActiveL;
        } else if (item.Type == "buildingspeedup") {
            iconUrl = "https://static.realmofempires.com/images/icons/Q_Upgrade2.png";
        } else if (item.Type == "researchspeedup") {
            iconUrl = "https://static.realmofempires.com/images/icons/M_ResearchList.png";
        }
        return iconUrl;
    }

    function _getCategoryIcon(type) {
        var iconUrl = "";

        if (type == "silver") {
            iconUrl = "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png";
        } else if (type == "troops") {
            iconUrl = "https://static.realmofempires.com/images/icons/m_attacks.png";
        } else if (type == "pfd") {
            iconUrl = "https://static.realmofempires.com/images/icons/m_hatspells.png";
        } else if (type == "buildingspeedup") {
            iconUrl = "https://static.realmofempires.com/images/icons/Q_Upgrade2.png";
        } else if (type == "researchspeedup") {
            iconUrl = "https://static.realmofempires.com/images/icons/M_ResearchList.png";
        } else {
            iconUrl = "https://static.realmofempires.com/images/icons/M_Rucksack.png";
        }

        return iconUrl;
    }

    function _postPickCat() {
        _filterCatType = _postAutoSelectCat;
        _postAutoSelectCat = null;
        _filterCatUpdate();
    }

    function _catClick(catElement) {
        _filterCatType = catElement.attr('data-type');
        _filterCatUpdate();     
    }

    function _filterCatUpdate() {

        $('.categoryItem',_container).removeClass('selected');
        $('.categoryItem[data-type="' + _filterCatType + '"]',_container).addClass('selected');

        $('.catNotice').remove();
        switch (_filterCatType) {
            case "researchspeedup":
                $('.itemsList', _container).prepend('<div class="catNotice">If item\'s speed up time is more than current research in progress, the extra time will be lost.</div>');
                break;
            case "buildingspeedup":
                $('.itemsList', _container).prepend('<div class="catNotice">Item is used on the first building in queue, in your selected village. If speed up time is more than build time, the extra time is lost, and will not be applied to next queue.</div>');
                break;
        }
        

        $('.item', _container).each(function () {
            if ($(this).hasClass(_filterCatType)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });

    }

    //changes to item populate that are affected by the village of choice
    function _villageEffectOnItems() {

        var spaceCoin = _village.coinsTresMax - _village.coins;
        var spaceFood = _village.popRemaining;
        var existingItem, itemCant, cantMsg, firstItem, itemGroup;

        for (var i = 0; i < ROE.Player.itemGroups.length; i++) {

            itemGroup = ROE.Player.itemGroups[i];
            if (itemGroup.itemIDs.length) { //only attempt work on non empty itemGroups
                firstItem = itemGroup._firstItem;
                existingItem = $('.item[data-groupID="' + itemGroup.groupID + '"]', _container);
                if (existingItem.length) {
                    itemCant = false;

                    if (firstItem.Type == "silver" && firstItem.Amount > spaceCoin) {
                        itemCant = true;
                        cantMsg = "Treasury Full";
                    } else if (firstItem.Type == "troops" && firstItem.Amount * ROE.Entities.UnitTypes[firstItem.UnitType.unitTypeID].Pop > spaceFood) {
                        itemCant = true;
                        cantMsg = "Farms at Capacity";
                    }
                    if (itemCant) {
                        if (!existingItem.hasClass('cant')) {
                            existingItem.addClass('cant').append('<div class="cantMask">' + cantMsg + '</div>');
                        }
                    } else {
                        existingItem.removeClass('cant').find('.cantMask').remove();
                    }
                }
            }
        }

        $('.header .villageInfo', _container).html(_village.name + ' (' + _village.x + ',' + _village.y + ')');
        $('.header .villageIcon', _container).css('background-image', 'url(' + BDA.Utils.GetMapVillageIconUrl(_village.points, _village.villagetypeid) + ')');
        
    }
 
    function _doUseItem(event) {
    
        var itemElement = $(event.currentTarget);
        if (itemElement.hasClass('cant')) {
            return;
        }

        var itemGroupData = itemElement.data('itemGroup');
        var groupID = itemGroupData.groupID;

        if (itemGroupData.itemIDs.length < 1) {
            return;
        }

        var itemID = itemGroupData.itemIDs.pop(); //pop out last item ID from array
        _updateElementCount(itemElement); //update the count rightaway, will re update when call comes back

        
        if (itemGroupData.busyCount) {
            itemGroupData.busyCount++;
        } else {
            itemElement.append('<div class="busy"></div>');
            itemGroupData.busyCount = 1;
        }

        ROE.Api.call_items2_use(_villageID, itemID, groupID, _useItemReturn);
        _container.find('.reload').hide();
    }
 
    function _useItemReturn(data) {

        var affectedItem = $('.item[data-groupID="' + data.groupID + '"]', _container);
        if (affectedItem.length) {

            var itemGroupData = affectedItem.data('itemGroup');

            if (data.wasUsed) {

                if (data.itemType == "pfd") {
                    //ROE.Player.credits = data.servants;//updates qty of servants available
                    ROE.Player.PFPckgs.list = data.PFPckgs; //updates the list of packages
                    ROE.Frame.refreshPFHeader();
                } else if (data.itemType == "researchspeedup") {
                    var researchPanel = $('#research');
                    if (researchPanel.length) {

                        //throtle research synch call so that mad-clicking item use does not general huge number of calls                    
                        clearTimeout(researchPanel.data('tmo'));
                        var tmo = setTimeout(function () {
                            ROE.Research.synch();
                            ROE.Player.refresh();
                        },1000);   
                        researchPanel.data('tmo', tmo);
                    }         
                } else {
                    ROE.Villages.ExtendedInfo_loadLatest(_village.id, null);
                }

                ROE.Frame.refreshQuestCount();

            } else { //if not used

                //re add the itemID to the list of IDs
                var itemID = itemGroupData.itemIDs.push(data.itemID);

                _updateElementCount(affectedItem);
                _villageEffectOnItems();
            }

            //give animated feedback text
            _usedFeedback(affectedItem, data.wasUsed);

            //handle the busy gif: if item is still processing or not
            itemGroupData.busyCount--;
            if (itemGroupData.busyCount < 1) {
                $('.busy', affectedItem).remove();
                if (!_anythingStillProcessing()) {
                    _container.find('.reload').show();
                }
            }

        }
        
    }

    //decrements or increments itemelement count text, based on item data (itemIDs length)
    function _updateElementCount(itemElement) {

        var itemGroupData = itemElement.data('itemGroup');
        var newCount = itemGroupData.itemIDs.length;
        if (newCount > 0) {
            itemElement.removeClass('empty');
            $('.c', itemElement).html('x' + newCount);
        } else {
            itemElement.addClass('cant empty');
            $('.c', itemElement).html('0');
        }

    }

    //after itemuse callback, add feedback based on wasused or not
    function _usedFeedback(itemGroupElement, wasUsed) {

        if (wasUsed) {
            var feedbackElement = $('<div class="feedback used">1 used</div>');
        } else {
            var feedbackElement = $('<div class="feedback notused">1 failed</div>');
        }

        itemGroupElement.append(feedbackElement);
        feedbackElement.stop().animate({ bottom: '100%', opacity: 0 }, 3000, "easeOutSine", function () {
            $(this).remove();
        });

    }

    //checks to see if there are any outstand calls still, buy just checking for busy element inside the group elements
    function _anythingStillProcessing() {
        var stillBusy = $('.item .busy').length > 0 ? true : false;
        return stillBusy;
    }

    //
    var _reInitContent = function (v) {
        //V can be null
        _sync(v);
    }




    function _handleCurrentlySelectedVillageChangedEvent() {
        if ($(CONST.Selector.dialog).dialog("isOpen") === true && _villageID != ROE.SVID) {
            Frame.busy('Village change...', 10000, $(CONST.Selector.dialog));
            _villageID = ROE.SVID;
            //_sync(); //we dont call synch because that route calls a loaditems as well, we dont need that for a villchange -farhad
            _cleanUp();
            ROE.Villages.getVillage(_villageID, _villchangeCB, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
        }

    }

    function _villchangeCB(village) {
        Frame.free($(CONST.Selector.dialog));
        _village = village;
        _villageID = _village.id;
        _populate();
    }

    function _cleanUp() {
        $('.item.used', _container).remove();
        $('.item.failed', _container).removeClass('failed').find('.failedMask').remove();
    }


    function _handleVillageExtendedInfoUpdatedOrPopulated(village) {
        
        if ($(CONST.Selector.dialog).dialog("isOpen") === true && _villageID == village.id) {
            if ( village.changes.coins || village.changes.popRem || village.changes.name) {
                _villchangeCB(village);
            }
        }

    }


    function _openSettings() {

        ROE.Frame.simplePopopOverlay('https://static.realmofempires.com/images/icons/M_FilterB.png', 'Settings', '', 'itemSettings', _container);

        var content = $('.simplePopupOverlay.itemSettings .pContent');

        var settingsPanel = $('.settingsPanel.template', _container).clone();
        settingsPanel.removeClass('template');

        content.append(settingsPanel);
        
    }


    ///ITEM related public utils

    //checks if player has an item of certain type in his rewards collection
    function _existsItemOfType(type) {
        if (!ROE.Player.itemGroups || ROE.Player.itemGroups.length == 0) {
            return false;
        }

        for (var i = 0, itemGroup; itemGroup = ROE.Player.itemGroups[i]; i++) {
            if (itemGroup._firstItem.Type == type) {
                return true;
            }
        }
        return false;
    }

    function _existsItemByGroupID(groupID) {

        if (!ROE.Player.itemGroups || ROE.Player.itemGroups.length == 0) {
            return false;
        }

        for (var i = 0, itemGroup; itemGroup = ROE.Player.itemGroups[i]; i++) {
            if (itemGroup.groupID == groupID) {
                return true;
            }
        }
        return false;

    }


    //updates Player items (if given new items), and repopulates module (if module is open)
    function _update(itemGroups) {

        if (itemGroups) {
            ROE.Player.itemGroups = itemGroups;
            if ($(CONST.Selector.dialog).dialog("isOpen")) {
                _populate();
            }
        } else if ($(CONST.Selector.dialog).dialog("isOpen")) {
            _load();
        }


        
    }



    
    obj.showPopup = _showItemsPopup;
    obj.reInitContent = _reInitContent;

    obj.existsItemOfType = _existsItemOfType;
    obj.existsItemByGroupID = _existsItemByGroupID;
    obj.update = _update;
    obj.loadItems = _loadItems;
    
}(window.ROE.Items2 = window.ROE.Items2 || {}));




