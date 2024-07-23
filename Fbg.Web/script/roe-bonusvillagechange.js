/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

(function (obj) {

    var _showPopup = function () {
        
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'bonus .popupBody').length > 0) {
            return;
        }

        if (ROE.isMobile) {
            popupModalOverlay("bonus", "", 10,0);
            currentID = ROE.Player.CurVill.id;
            var currentTypeName = ROE.Player.CurVill.villageTypeName;
        } else {
            popupModalOverlay("bonus", "", (document.width / 2) - 150, 10);
            currentID = svid;
            var currentTypeName = $("#ctl00_ContentPlaceHolder1_Label1 SPAN").text();
        }

        var temp = BDA.Templates.getRawJQObj("BonusVillageChange", ROE.realmID);
        var popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'bonus .popupBody').append(temp);

        $(".currentBonusTypes .villName").text(currentTypeName);

        ROE.Api.call("getbonustypes", { vid: currentID }, _updateDefaultpage);
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
            villName = $(".bonusTypes[data-typeid=" + bonusid + "] .bonusName").html();
            
            if (ROE.isMobile) {
                ROE.Frame.infoBar(_phrases(1));
                ROE.UI.Sounds.click();
            }
            else {
                $("#ctl00_UpdateProgress2").show();
                $("#ctl00_UpdateProgress2 DIV").css("z-index", "10000");
            }
            ROE.Api.call("setbonustype", { vid: currentID, bt: bonusid }, _bonusSaved);

        }
    }
    

    var _bonusSaved = function (response) {

        if (response.Changed) {
            
            if (ROE.isMobile) {
                ROE.Frame.infoBar(_phrases(3));
                $(".bonusVillage .yellowLabel").text(_phrases(8) + " " + villName).css("color", "rgb(22, 255, 66)");

                ROE.Frame.reloadHeder();

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
                ROE.Frame.infoBar(_phrases(6));
            }
            else {
                $("#ctl00_UpdateProgress2").hide();
                $("#popup_bonus .bonusswitch").text(_phrases(6)).css("color","red");
            }
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

        if (response.BonusChange) {
            
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
    }


    function _hireServant() {
        
        _closePopup();

        if (ROE.isMobile) {
            ROE.Credits.showPopup();
        }
        else {
            window.location.assign("pfCredits.aspx");
        }
    }


    function _phrases(id) {
        return $('#BonusVillageSelection .phrases [ph=' + id + ']').html();
    }


    obj.showPopup = _showPopup;

}(window.ROE.BonusVillageChange = window.ROE.BonusVillageChange || {}, jQuery));