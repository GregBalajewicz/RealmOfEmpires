(function (ROE) {
}(window.ROE = window.ROE || {}));


(function (obj) {

    var _container;     
    var dailylimitShow = 200;
    var _sendGiftEnabled = false;
    var CONST = {
        MAX_GIFTS: 999
    };
    var _village; // the village into which we want to get the items

    var _itemMax = 0; //max of this item you can get
    var _itemOwned = 0; //how many of current item you own
    var _itemsSelected = 0; //number of items currently selected to get
    var _selectItemIndex; //current item is selected index from ROE.Entities.Items

    var _silverPFSpecialUpdate = false; //a rough way of doing a special populate for silver pf update

    var giftcount;
    var giftID;
    var giftUnitCost;
    var giftType;
    var giftUnitID;
    var giftUnitPop;
    var giftImg = new Array(
     "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png",
     "https://static.realmofempires.com/images/units/Infantry_m.png",
     "https://static.realmofempires.com/images/units/Cavalry_M.png",
     "https://static.realmofempires.com/images/units/Knight_m.png",
    "https://static.realmofempires.com/images/units/Spy_m.png",
    "https://static.realmofempires.com/images/units/ram_m.png",
    "https://static.realmofempires.com/images/units/Treb_m.png");

    var _expectedCost;

    CONST.Selectors = {
        giftsPopup: "", // Set in init
        giftUnits: ".giftUnits",
        maxGifts: ".maxGifts",
        giftAddMax: ".giftAddMax",
        sendItemToFriend: ".sendItemToFriend",
        sendGiftsToggleBtn: ".sendGiftsToggleBtn",
        phrase_SendToFriends: '#gifts_popup .phrases [ph=13]',
        phrase_TurnOffSendToFriends: '#gifts_popup .phrases [ph=14]'

    };

    CONST.CSSClass = {
        invalidNum: "invalidNum"
    };

    obj.init = function (container, optionalVillageID) {
        _container = container;
                      
        // Reset SEND enabled when player views popup
        _sendGiftEnabled = false;
       
        if (ROE.isMobile) {           
            _container.append(BDA.Templates.getRawJQObj("GiftsPopup",  ROE.realmID));
        } else {            
            _container.append(BDA.Templates.getRawJQObj("GiftsPopup_d2",  ROE.realmID));
        }

        ROE.Frame.busy('Preparing village...', 5000, _container); // getting the village object may require API call  so this is why we do this
        ROE.Villages.getVillage(optionalVillageID ? optionalVillageID : ROE.SVID
            , rowPopulated
            , ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
    }
    

    function rowPopulated(village) {
        ROE.Frame.free(_container);

        _village = village; 
        giftItems = { };

        var content = "";
        var dailyGiftText = "";
        var dailyGiftLeft = 0;
        var PlayerTitle = ROE.Player.Ranking.titleID();
        var Transition = window.BDA.UI.Transition;
        var itemlength = ROE.Entities.Items.length;
    
        var rowTemp = $(".giftsIconSheet");
        rowTemp.remove();

        var rowTempNot = $(".giftsIconSheetNot");
        rowTempNot.remove();

        SentGifts = new Array;
        GiftsCost = new Array;
        GiftsPay = new Array;            

        var pageWidth = $(window).width() + "px";
           
        _giftUpdater();   
        _createList();
           
        function _giftUpdater() {             

            //localstorage cache does not exist
            if (!BDA.Database.LocalGet('GiftsListLoaded')) {
                ROE.Frame.busy('Getting Items...', 5000, _container);
                ROE.Api.call("items_getMyItems", { vid: _village.id},  _mygiftlist);
            } else {
                //load available gifts from localstorage cache
                _loadCache();
                ROE.Frame.busy('Updating Items...', 5000, _container);
                ROE.Api.call("items_getMyItems", { vid: _village.id },  _mygiftupdate);
            }               
                
        }

        function _loadCache() {
            var temp = JSON.parse(BDA.Database.LocalGet('GiftsListLoaded'));
            hourlySilver = temp.hourlySilverProdForSilverGift;
            dailyGiftLeft = temp.todaysDailyLimit;
            $.each(temp.myItems, function (key, value) {
                SentGifts[value.giftID] = value.count;
                GiftsCost[value.giftID] = value.costInServants;
                GiftsPay[value.giftID] = value.payoutMultiplyer;
            });
        }
            
        function _mygiftlist(response) {
            ROE.Frame.free(_container);
               
            var temp = response.myGiftInfo;
            hourlySilver = temp.hourlySilverProdForSilverGift;            
            dailyGiftLeft = temp.todaysDailyLimit;

            $.each(temp.myItems, function (key, value) {

                var giftid = value.giftID;
                var giftvalue = value.count;
                var giftloc = $("#Gift_" + giftid + " .availableText");

                SentGifts[giftid] = giftvalue;
                GiftsCost[giftid] = value.costInServants;
                GiftsPay[giftid] = value.payoutMultiplyer;
                    
                if (value.count > 0) { giftloc.text("(" + value.count +  ")"); }
                else { giftloc.text(""); }

                $("#Gift_" + giftid).attr("data-giftcnt", giftvalue);
            });
                
            BDA.Database.LocalSet('GiftsListLoaded', JSON.stringify (temp));

            if (dailyGiftLeft > dailylimitShow) {  $("#giftdaily").removeClass("giftdailyShow"); }
            else { $("#giftdaily").addClass("giftdailyShow"); $("#giftdaily > span").text(dailyGiftLeft); }

            _updateInfoPanel(temp);

        }


        function _mygiftupdate(response) {
            ROE.Frame.free(_container);

            var temp = response.myGiftInfo;
            hourlySilver = temp.hourlySilverProdForSilverGift;
            dailyGiftLeft = temp.todaysDailyLimit;
                
            if (dailyGiftLeft >= dailylimitShow) {  $("#giftdaily").removeClass("giftdailyShow"); }
            else { $("#giftdaily").addClass("giftdailyShow");  $("#giftdaily > span").text(dailyGiftLeft); }
                
            $.each(temp.myItems, function (key, value) {

                var giftid = value.giftID;
                var giftvalue = value.count;
                var giftloc = $("#Gift_" + giftid + " .availableText");
                var oldvalue = giftloc.text();

                if (oldvalue != giftvalue) {
                    if (value.count > 0) { giftloc.text("(" + value.count  + ")").addClass("ShowNewGift"); }
                    else { giftloc.text(""); }

                    $("#Gift_" + giftid).attr("data-giftcnt", giftvalue);
                }

                SentGifts[value.giftID] = value.count;
                GiftsCost[value.giftID] = value.costInServants;
                GiftsPay[value.giftID] = value.payoutMultiplyer;
                    
            });
                
            BDA.Database.LocalSet('GiftsListLoaded', JSON.stringify(temp));

            //if there is a result code, we came here from an itempurchase callback
            var rcode = response.resultCode;
            if (rcode != null && typeof (rcode) != "undefined") {
                _itemsSelectedChange(0);

                if (rcode == 0) {

                    ROE.Frame.busy('Synching...', 5000, _container);
                    ROE.Villages.ExtendedInfo_loadLatest(_village.id, _syncReady);

                    var numberOfItemsBought = response.resultDetails.numberOfItemsBought;
                    var numberOfOwnedGiftsUsed = response.resultDetails.numberOfOwnedGiftsUsed;
                    var totalBuy = numberOfItemsBought + numberOfOwnedGiftsUsed;
                    var giftGetText = " +" + BDA.Utils.formatNum(parseInt(giftGet * totalBuy)) + " " + giftUnitName + "!";

                    if (numberOfOwnedGiftsUsed > 0) {
                        giftcount -= parseInt(numberOfOwnedGiftsUsed);
                    }

                    BDA.UI.Transition.slideRight($("#gifts_popup .giftList"), $("#gifts_popup .giftInfo"), function () { }); //_mygiftupdate

                    var youGotDiv = $('<div class="fontGoldFrLCXlrg youGotGifts"></div>').hide().html('You got ' + giftGetText).click(function () { $(this).remove(); });
                    $(_container).append(youGotDiv);
                    youGotDiv.slideDown(700);

                } else {
                    _reportAjaxError(rcode);
                }

                ROE.Frame.refreshQuestCount();

            } else if (_silverPFSpecialUpdate) {
                //quick n dirty way to make sure the spell shows latest silver related changes, this whole module needs a redo
                _silverPFSpecialUpdate = false;
                $('#Gift_1', _container).click();
            }

            if (response.credits) {               
                ROE.Frame.refreshPFHeader(response.credits);
            }

            _updateInfoPanel(temp);

        }
        
        //upadte the more information panel with dynamic info
        function _updateInfoPanel(giftInfo) {
            var infoPanel = $('.infoPanel', _container);
            if (infoPanel.length) {
                var str = infoPanel.html();
                var str = str.replace("%dailyLimit%", giftInfo.dailyLimit);
                var str = str.replace("%dailyLimitCarryOverDays%", giftInfo.dailyLimitCarryOverDays);
                infoPanel.html(str);
            }
        }

        function _createList() {

            for (i = 0; i < itemlength; i++) {
                    
                giftItems.index = i;
                giftItems.name = ROE.Entities.Items[i].title;
                giftItems.id = ROE.Entities.Items[i].id;
                giftItems.img = giftImg[i];
                    
                giftItemsTitle = ROE.Entities.Items[i].requiredTitleID;
                giftItems.reqtitle = ROE.Titles[giftItemsTitle].name;
                giftItems.available = SentGifts[ giftItems.id ];
                if (giftItems.available == undefined ||  giftItems.available == 0) { giftItems.availabletext = ""; }
                else { giftItems.availabletext = "(" +  giftItems.available + ")"; }
                    
                if (PlayerTitle >= giftItemsTitle) { content +=  BDA.Templates.populate(rowTemp.prop('outerHTML'), giftItems); }
                else { content += BDA.Templates.populate(rowTempNot.prop ('outerHTML'), giftItems); }

                if(i != itemlength-1)
                    content += "<div class='themeM-squiggle-t'></div>";
            }
            //fillup page created from template
            $("#gifts_popup .giftList .giftContainer").append(content);
            $('.infoBtn, .infoPanel', _container).click(_toggleInfo);
            $(".giftsIconSheet").click(_ItemRowClick);
            $(".slideBackToDefaultPage").click(_giftback);
            $(".giftInfoAdd").click(_addRemoveItems);
            $('.maxButtons .maxFree', _container).click(function () {
                if ($(this).hasClass('grayout')) { return; }
                _directInput(_itemOwned);
            });
            $('.maxButtons .max', _container).click(function () {
                if ($(this).hasClass('grayout')) { return; }
                _directInput(_itemMax);
            });
            $(".getItemsButton", _container).click(_getItems);

        }

        function _ItemRowClick() {

            ROE.UI.Sounds.clickActionOpen();
            ROE.Frame.busy('Preparing item details...', 5000, _container);
            BDA.UI.Transition.slideLeft($("#gifts_popup .giftInfo"), $("#gifts_popup .giftList"), _giveFocusToGiftsInputBox);

            $('.youGotGifts', _container).remove();

            var giftElement = $(this);
            _selectItemIndex = giftElement.attr("data-giftindex");            
            giftcount = parseInt($(this).attr("data-giftcnt"));
            giftID = ROE.Entities.Items[_selectItemIndex].id;
            giftType = ROE.Entities.Items[_selectItemIndex].type;
            giftUnitCost = GiftsCost[giftID];
            giftUnitID = ROE.Entities.Items[_selectItemIndex].unitTypeID;
            _itemOwned = SentGifts[giftID];


            var giftpayout = ROE.Entities.Items[_selectItemIndex].payout;
            var giftMultiply = GiftsPay[giftID];
            
            var UnitPopulat = new Array;
            var village = _village;
            var troops;
            for (j = 0; j < ROE.Entities.UnitTypes.SortedList.length; j++) {
                troops = village.TroopByID(ROE.Entities.UnitTypes.SortedList[j]);
                UnitPopulat[troops.id] = troops.YourUnitsTotalCount;
            }

            switch (giftType) {

                case "silver":
                    giftGet = parseInt(hourlySilver * giftpayout * giftMultiply);
                    giftUnitName = giftType;
                    giftUnitImage = "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png";
                    giftUnitPop = 1;
                    giftPopul = _village.coins;
                    break;

                case "troops":
                    giftGet = (giftpayout * giftMultiply);
                    giftUnitName = ROE.Entities.UnitTypes[giftUnitID].Name;
                    giftUnitImage = ROE.Entities.UnitTypes[giftUnitID].Image;
                    giftUnitPop = ROE.Entities.UnitTypes[giftUnitID].Pop;
                    giftPopul = UnitPopulat[giftUnitID];
                    break;

                default: alert("ERROR:Unknown item type");

            }

            ROE.Villages.ExtendedInfo_loadLatest(_village.id, _syncReady);
            
        }

        function _syncReady(village) {
            ROE.Frame.free(_container);
            _village = village;
            _sync();
        }

        function _sync() {

            var coins = _village.coins;
            var coinsMax = _village.coinsTresMax;
            var pop = _village.popCur;
            var popMax = _village.popMax;
            var servants = ROE.Player.credits;
            var maxByServants = parseInt(servants / giftUnitCost) + SentGifts[giftID];

            if (giftType == "silver") {
                maxByPopulation = parseInt((coinsMax - coins) / (giftUnitPop * giftGet));
            }
            else {
                maxByPopulation = parseInt((popMax - pop) / (giftUnitPop * giftGet));
            }

            MaxAvailable = Math.min(dailyGiftLeft, maxByPopulation, maxByServants);
            if (MaxAvailable < 0) { MaxAvailable = 0; }
            _itemMax = MaxAvailable;

            $("#giftError",_container).html("");

            if (MaxAvailable <= 0) {

                if (maxByPopulation == 0) {
                    var r = 9;
                    if (giftType == "silver") { var r = 8; }
                }
                if (maxByServants == 0) { var r = 10; }

                if (dailyGiftLeft == 0) { var r = 1; }

                _reportAjaxError(r);

                if (r == 10) {
                    var BuyText = '<div>'+$("#giftError").text()+'</div>';
                    BuyText += '<a class="sfx2 buymore customButtomBG"  onclick="ROE.Frame.showBuyCredits()">' + $('#gifts_popup .phrases  [ph=12]').text() + "</a>";
                    $("#giftError").html(BuyText);
                }
            }

            _ItemPagePopulate();
        }



        //the new bazzar populate function
        function _ItemPagePopulate() {
           
            _itemsSelectedChange(0);

            $('.itemIcon', _container).css('background-image', 'url(' + giftImg[_selectItemIndex] + ')');
            $('.youOwnNum', _container).html(_itemOwned);
            $('.isWorthNum', _container).html(giftGet);
            $('.maxButtons .maxFree', _container).html('Max Free: ' + Math.min(_itemOwned, _itemMax)).toggleClass('grayout', _itemMax < 1 || _itemOwned < 1);
            $('.maxButtons .max', _container).html('Max: ' + _itemMax).toggleClass('grayout', _itemMax < 1);
            $('.giftInfoAdd', _container).toggleClass('grayout', _itemMax < 1);
            $(".panelSilverPF", _container).hide();

            //setup and instantiate a Silver premium feature
            if (giftType == "silver" && ROE.isVPRealm && !(ROE.Player.PFPckgs.isActive(22))) {
                $(".panelSilverPF", _container).show()
                var settings = {
                    mode: "inline", 
                    fill: ($(".panelSilverPF .pfContainer", _container)), //for popup mode, popup content must be passed, for inline mode any element can be passed
                    featureId: 22 
                }
                ROE.UI.PFs.init(settings, _silverPFActivateCallback);
            }

        }

        function _silverPFActivateCallback(data) {
            _silverPFSpecialUpdate = true;
            _giftUpdater();
            ROE.Frame.refreshPFHeader(data);
        }


        //activate gift info screen back button
        function _giftback() {               
            ROE.UI.Sounds.clickActionOpen();
            BDA.UI.Transition.slideRight($("#gifts_popup .giftList"), $("#gifts_popup .giftInfo"), _giftUpdater);
            $("#giftError").html("").removeClass("buysuccess");
        }

        // mobile click
        function _addRemoveItems() {
            if ($(this).hasClass('grayout')) { return; }
            ROE.UI.Sounds.click();
            var change = parseInt($(this).attr("data-giftadd"));
            _itemsSelectedChange(_itemsSelected + change);
        }

        function _directInput(num) {
            ROE.UI.Sounds.click();
            _itemsSelectedChange(num);
        }

        function _itemsSelectedChange(num) {
            if (num < 1) { num = 0; }
            else if (num > _itemMax) { num = _itemMax; }
            _itemsSelected = num;
            _updateSummary();
        }

        function _updateSummary() {
            if (ROE.isMobile) {
                var inputText = _itemsSelected == 0 ? "no" : _itemsSelected;
                $('.itemsSelected', _container).html(inputText);
            } else {
                $(CONST.Selectors.giftUnits).val(_itemsSelected);
            }

            $('.getItemsButton', _container).toggleClass('grayout', _itemsSelected == 0);

            _expectedCost = parseInt(giftUnitCost * (_itemsSelected - giftcount));
            if (_expectedCost < 0) { _expectedCost = 0; }

            var name = giftUnitName;
            var freeTotal = Math.min(_itemsSelected, giftcount) * giftGet; //amount of stuff we get for free
            var paidTotal = Math.max(_itemsSelected - giftcount, 0) * giftGet; //amount of stuff we get from servants
            var allTotal = giftGet * _itemsSelected; //total amount of stuff
            

            $('.summaryL2', _container).html(BDA.Utils.formatNum(freeTotal) + ' ' + name + ' for free and,').toggleClass('grayout', freeTotal < 1);
            $('.summaryL3', _container).html(BDA.Utils.formatNum(paidTotal) + ' ' + name + ' for ' + _expectedCost + ' servants.').toggleClass('grayout', paidTotal < 1);
            $('.summaryL4', _container).html('Total: ' + BDA.Utils.formatNum(allTotal) + ' ' + name).toggleClass('grayout', allTotal < 1);

        }
        
        //get/activate the selected items
        function _getItems() {
            if (_itemsSelected > 0 && _itemMax > 0) {
                $("#giftError").html("");
                ROE.UI.Sounds.clickSpell();             
                ROE.Frame.busy('Getting your stuff!<BR>Be patient, it\'s  a lot of stuff!', 30000,_container);
                ROE.Api.call("Items_Buy", { cnt: _itemsSelected, expectedCost: _expectedCost, gid: giftID, vid: _village.id }, _mygiftupdate);
            }
        }


        function _reportAjaxError(r) {
            $("#giftError", _container).html($('#gifts_popup .phrases  [ph=' + r + ']', _container).html());
        }





        //d2
        function _giveFocusToGiftsInputBox() {
            $(CONST.Selectors.giftUnits).focus();
        }

        function _clearGiftUnitsInputBox() {
            $(CONST.Selectors.giftUnits).val("").removeClass(CONST.CSSClass.invalidNum);
              
        }

        function _selectedItemsChangeFromInput() {
            var inputElement = $(CONST.Selectors.giftUnits);
            var value = parseInt(inputElement.val());
            if (isNaN(value) || value == "") {
                value = 0;
                inputElement.val(0);
            }
            _directInput(value);
        }

        function _toggleSendButtons() {
            _sendGiftEnabled = !_sendGiftEnabled;

            if (_sendGiftEnabled) {
                $(CONST.Selectors.sendItemToFriend).show();
                $(CONST.Selectors.sendGiftsToggleBtn, _container).html ($(CONST.Selectors.phrase_TurnOffSendToFriends, _container).html());
            } else {
                $(CONST.Selectors.sendItemToFriend).hide();
                $(CONST.Selectors.sendGiftsToggleBtn, _container).html ($(CONST.Selectors.phrase_SendToFriends, _container).html());
            }
        }

        function _toggleInfo() {
           $('.infoPanel', _container).toggle();
        }
           
        obj.selectedItemsChangeFromInput = _selectedItemsChangeFromInput;
        obj.toggleSendButtons = _toggleSendButtons;
    }

   


}(window.ROE.Gifts = window.ROE.Gifts || {}));