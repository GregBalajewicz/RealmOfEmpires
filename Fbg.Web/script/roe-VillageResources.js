/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui-gesture-swipe.js"/>
/// <reference path="bda-ui-scrolling.js"/>
/// <reference path="bda-ui-transition.js"/>
/// <reference path="roe-api.js"/>
/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />
/// <reference path="roe-player.js" />
/// <reference path="roe-frame.js" />
/// <reference path="roe-frame_m.js" />
/// <reference path="countdown.js"/>

(function (ROE) {
} (window.ROE = window.ROE || {}));

(function (obj) {
    var Utils = window.BDA.Utils;
    
    var Api = window.ROE.Api,
        Entities = window.ROE.Entities,
        Frame = window.ROE.Frame,
        Player = window.ROE.Player;
        
    var CONST = {},
        CACHE = {};
        CONST.Enum = {};

    var CONST = { popupName: "Resources" },
        CACHE = {};

    CONST.Selector = {
        panelVillage: ".Village",
        panelResources: ".themeM-panel.resources",
        progressIndicator: ".progress-indicator",
        bonusVillage: ".bonusVillage",
        boostLoyalty: ".boostLoyalty",
        boostLoyaltyBlock: ".boostLoyaltyBlock",
        boostLoyaltyClick: ".boostLoyaltyClick",
        notEnoughServants: ".notEnoughServants",        
        label: ".label",
        title: ".title",
        nameChangeIcon: ".nameCngIcon",
        quickSilverTransport: ".quickTransport",
        bonusVillChange:".bonusVillChange"
    };

    CONST.CssClass = {
        progressIndicator: "progress-indicator",
        redProgressIndicator: "red-progress-indicator"
    };
   

    var _template_Page; // jquery object
    var _popupContent; 
    var _village;

    function _showResourcesPopup() {

        ROE.Frame.busy();
        _template_Page = $(BDA.Templates.getRaw("Resources", ROE.realmID));
        ROE.Frame.free();

        popupModalPopup(CONST.popupName, "RESOURCES", undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeaderAndView, "https://static.realmofempires.com/images/misc/M_VillageResources.png");
        BDA.Broadcast.subscribe($(CONST.Selector.panelVillage), "VillageExtendedInfoUpdated", _populate);

        // populate with cached data; this shoudl return immediatelly, since this object shoudl be available immediatelly. 
        ROE.Villages.getVillage(ROE.SVID, _populate, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);

        // trigger refresh of the village; this will trigger VillageExtendedInfoInitiallyPopulated event and thus _populate will be called again and thus refreshed with the latest info
        ROE.Villages.ExtendedInfo_loadLatest(ROE.SVID);
    }


    function _populate(village) {
        _village = village;
        var data = {};
        var content;        

        _popupContent= $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + CONST.popupName + ' .popupBody');

        data.village_name = _village.name;
        data.village_id = _village.id;
        data.village_x = _village.x;
        data.village_y = _village.y;
        data.village_loyalty = _village.yey;
        data.village_coins = ROE.Utils.addThousandSeperator(_village.coins);
        data.village_coinsPerHour = ROE.Utils.addThousandSeperator(Math.round(_village.coinsPH));
        data.village_tresMax = ROE.Utils.addThousandSeperator(_village.coinsTresMax);
        data.village_food = ROE.Utils.addThousandSeperator(_village.popCur);
        data.village_maxFood = ROE.Utils.addThousandSeperator(_village.popMax);
        data.village_foodRem = ROE.Utils.addThousandSeperator(_village.popMax - _village.popCur);
        data.village_bonusVillage = ROE.Entities.VillageTypes[_village.villagetypeid].Name;
        
        
        content = $(BDA.Templates.populate(_template_Page[0].outerHTML, data));
        
        if (!ROE.Entities.VillageTypes[_village.villagetypeid].IsBonus) {
            content.find(CONST.Selector.bonusVillage)
                    .remove();
        }
        else {
            if (ROE.bonusVillChange) { content.find(CONST.Selector.bonusVillChange).attr("onclick", "ROE.BonusVillageChange.showPopup()"); }
            else { content.find(CONST.Selector.bonusVillChange).remove(); }
        }
        var progress = content.find(CONST.Selector.panelVillage).find(CONST.Selector.progressIndicator);
        if (_village.yey < 100) {
            progress.removeClass(CONST.CssClass.progressIndicator);
            progress.addClass(CONST.CssClass.redProgressIndicator);
            
            content.find(CONST.Selector.panelVillage)
                    .find(CONST.Selector.boostLoyaltyClick)
                        .attr("onclick", "ROE.VillageResources.onBoostLoyalty()");
            
        }
        else {
            content.find(CONST.Selector.panelVillage)
                .find(CONST.Selector.boostLoyalty)
                    .remove();
        }
        progress.css("width", _village.yey + "%")
        
        content.find(CONST.Selector.nameChangeIcon).attr("onclick", "ROE.Utils.RenameVillage(" + ROE.SVID + ", $(\'.vilres_nameloc_txt\')[0], null, ROE.Utils.updateVillageName)");

        // configure quick transport button
        if (_village.areTranspAvail) {
            content.find(CONST.Selector.quickSilverTransport).show();
        }
        
        _popupContent.html(content);

   
     
    }
       

    function addCommas(nStr) {
        nStr += '';
        var x = nStr.split('.');
        var x1 = x[0];
        var x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    }


    function _onBoostLoyalty() {

        if (ROE.Player.approvalBoostAllowedWhen > BDA.Utils.getUTCNow()) {
            ROE.Frame.errorBar('Cant boost again untill: ' + BDA.Utils.formatEventTime(new Date(ROE.Player.approvalBoostAllowedWhen)));
            return;
        }
        
        if (_village.yey < 100) {

            if (ROE.Player.credits < 200) {
                ROE.UI.Sounds.clickMenuExit();
                _popupContent.find(CONST.Selector.boostLoyaltyBlock)
                    .hide();
                _popupContent.find(CONST.Selector.notEnoughServants)
                    .show();
                return;
            }
            
            if (confirm("Spend 200 servants to boost the approval to 100% in this village?")) {
                ROE.UI.Sounds.clickSpell();
                Frame.busy();
                ROE.Api.call_boostLoyalty(ROE.SVID, _callback_onBoostLoyalty);
            }
        }
    }

    function _callback_onBoostLoyalty (data) {

        Frame.free();
        ROE.Player.credits = data.creditsLeft;

        if (data.boostSuccessful) {
            ROE.Villages.__populateExtendedData(_village.id, _village, data.Village, _populate);
        } else {

            if (data.ifNotBoostedThenisDueToCoolDown) {
                //fail due to cooldown
                ROE.Frame.errorBar('Loyalty boost failed: still on cooldown.');
            } else {
                //generic fail
                ROE.Frame.errorBar('Boost failed.');
            }

            
        }
    }

    obj.showResourcesPopup = _showResourcesPopup;
    obj.onBoostLoyalty = _onBoostLoyalty;

} (window.ROE.VillageResources = window.ROE.VillageResources || {}));
