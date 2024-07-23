/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

(function (obj) {

    var _village;
    var _showPopup = function () {
        
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'bonus .popupBody').length > 0) {
            return;
        }

        ROE.Villages.getVillage(ROE.SVID, _populate, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
    }

    var _populate = function (village) {
        _village = village;

        popupModalOverlay("bonus", "", 10, 0);

        var temp = BDA.Templates.getRawJQObj("BonusVillageChange", ROE.realmID);
        var popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'bonus .popupBody').append(temp);

        $(".currentBonusTypes .villName").text(ROE.Entities.VillageTypes[village.villagetypeid].Name);

        ROE.Api.call("getbonustypes", { vid: _village.id }, _updateDefaultpage);

        if (ROE.isD2) {
            $('#popup_bonus').position({
                my: "center center",
                at: "center center",
                of: window
            });
        }
    }


    var _toggleDesc = function (e) {
        
        var clicked = $(e.currentTarget);  
        $("#BonusVillageSelection .bonusDisplay").removeClass("highlight");

        if (!clicked.parent().hasClass('showit')) {
            $("#BonusVillageSelection .bonusTypes").removeClass('showit');
            clicked.parent().addClass('showit');
            clicked.addClass("highlight");
        } else {
            $("#BonusVillageSelection .bonusTypes").removeClass('showit');
        }
    }


    var _bonusChange = function (e) {
        
        if ($(e.currentTarget).hasClass("GO")) {

            
            $(e.currentTarget).removeClass("GO");
            bonusid = $(e.currentTarget).attr("data-typeid");
            
            if (ROE.isMobile) {
                ROE.Frame.infoBar(_phrases(1));
                ROE.UI.Sounds.click();
            }
            else if (ROE.isD2) {
                ROE.UI.Sounds.click();
                ROE.Frame.busy('Changing village bonus...', 5000, $('#popup_bonus'));
            } else {
                $("#ctl00_UpdateProgress2").show();
                $("#ctl00_UpdateProgress2 DIV").css("z-index", "10000");
            }
            ROE.Api.call("setbonustype", { vid: _village.id, bt: bonusid }, _bonusSaved);

        }
    }
    

    var _bonusSaved = function (response) {

        if (response.Changed) {
            
            if (ROE.isMobile || ROE.isD2) {
                ROE.Frame.infoBar(_phrases(3));

                ROE.Villages.ExtendedInfo_loadLatest(_village.id);

                //need update the MAP!!!
                ROE.Landmark.refresh();
            }
            else {
                //desktop reload page
                window.location.reload();
            }
            _closePopup();
        }
        else {
            //error
            if (ROE.isMobile) {
                ROE.Frame.errorBar(_phrases(6));
            }
            else {
                ROE.Frame.errorBar(_phrases(6));

                //  $("#ctl00_UpdateProgress2").hide();
                //  $("#popup_bonus .bonusswitch").text(_phrases(6)).css("color","red");
            }
            ROE.Frame.free($('#popup_bonus'));
        }

        $("#BonusVillageSelection .bonusConvert").addClass("GO");
    }


    var _closePopup = function () {
        $('#BonusVillageSelection').hide();
        $("#ctl00_UpdateProgress2").hide();
        closeMe();
    }
   

    function _updateDefaultpage(response) {
        
        var tempBonusList = "";
        var conversion = 20;
        var currentBonusType = response.CurrentBonusType;
        var currentServants = response.CurrentServants;
        var BonusTypes = response.BonusTypes;
        var costError = "";

        if (response.BonusChange && (_village.villagetypeid <= 7 || ROE.realmID < 95) ) {
            
            for (i = 0; i < BonusTypes.length; i++) {

                var bonusID = BonusTypes[i].ID;
                var bonusIMG = BonusTypes[i].ImgUrl;
                var bonusName = BonusTypes[i].Name;
                var bonusCost = BonusTypes[i].Cost;
                var bonusNextCost = BonusTypes[i].NextCost;
               
                costError = "";

                if (currentBonusType != bonusID) {

                    if (currentServants < bonusCost) { costError = "costError"; }

                    tempBonusList += '<div class="bonusTypes" data-typeid=' + bonusID + '>';
                    tempBonusList += '<img class="bonusDisplay sfx2" src="' + bonusIMG + '" ><div class=bonusName>' + bonusName + "</div>";
                    tempBonusList += '<div class="bonusDesc"><div class="villCost">' + _phrases(4) + ': ' + bonusCost + ' ' + _phrases(2) + '</div>';
                    
                    if (costError == "") {
                        tempBonusList += '<div class="nextCost" >' + _phrases(9) + ' ' + +bonusNextCost + ' ' + _phrases(2) + '</div>';
                        tempBonusList += '<div class="bonusConvert customButtomBG GO"  data-typeid=' + bonusID + ' >' + _phrases(5) + '</div>';
                    }
                    else {

                        tempBonusList += '<div class="nextCost costError" >' + _phrases(7) + '</div>';
                        tempBonusList += '<br><div class="hireServant customButtomBG GO"  >' + _phrases(10) + '</div>';
                    }
                     tempBonusList += '</div></div>';
                }
            };
        }
        else {
            $("#BonusVillageSelection .bonusswitch").text(_phrases(0));
        }

        $("#BonusVillageSelection .bonusList").empty().append(tempBonusList);
        $("#BonusVillageSelection .bonusDesc").show();

        //
        // bind events
        //
        $("#BonusVillageSelection .bonusConvert").click(_bonusChange);
        $("#BonusVillageSelection .hireServant").click(_hireServant);
        $("#BonusVillageSelection .closeX").click(_closePopup);
        $("#BonusVillageSelection .bonusDisplay").click(_toggleDesc);

        if (ROE.isD2) {
            $('#popup_bonus').position({
                my: "center center",
                at: "center center",
                of: window
            });
        }

    }


    function _hireServant() {
        _closePopup();

        ROE.Frame.showBuyCredits();
    }


    function _phrases(id) {
        return $('#BonusVillageSelection .phrases [ph=' + id + ']').html();
    }


    obj.showPopup = _showPopup;

}(window.ROE.BonusVillageChange = window.ROE.BonusVillageChange || {}, jQuery));