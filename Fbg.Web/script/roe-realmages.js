/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

(function (obj, $, undefined) {

    
    var _showPopup = function (hideCallback) {
        
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'RealmAges .popupBody').length > 0) {
            return;
        }
        

        var popupTemp = BDA.Templates.getRawJQObj("RealmAges", ROE.realmID);
        
        var titles = new Array("You are in Age ", "What's next?", "The distant future:");
        var roman = new Array("","I.","II.","III.","IV.","V.");
        var ageItems = {};
        var Ages = ROE.Entities.Ages.Age.Ages;
        var CurrentAge = ROE.Entities.Ages.Age.CurrentAge.AgeNumber;
        var content = "";
        var j = 0;
        var ageImg = "https://static.realmofempires.com/images/icons/Age";
        var rowTemp = popupTemp.find(".AgeSheet");
        rowTemp.remove();

        for (var i = 0; i < Ages.length; i++) {

            ageItems.id = Ages[i].AgeNumber;

            if (ageItems.id >= CurrentAge) {

                ageItems.currentimg = "";
                ageItems.title = titles[j];
                ageItems.img = ageImg + ageItems.id + ".png";

                if (ageItems.id == CurrentAge) {
                    ageItems.img = ageImg + ageItems.id + "L.png";
                    ageItems.currentimg = "realmAgeImgCurrent";
                    ageItems.title = titles[j] + roman[ageItems.id];
                }
                
                ageItems.info = Ages[i].Info;
                ageItems.date = "<BR>";

                if (typeof Ages[i - 1] != "undefined" && ageItems.id > CurrentAge) {

                    ageItems.date = _timeleft(parseInt(Ages[i - 1].Until.slice(6, 19)));
                }

                content += BDA.Templates.populate(rowTemp.prop('outerHTML'), ageItems);
                
                j++;
                if (j > 2) { break;}//show max 3 ages
                if (ageItems.id < Ages.length) { content += "<div class='ageSeparator' ></div>"; }
            }
        }

        popupTemp = popupTemp.append(content);
        

        ROE.Frame.simplePopopOverlay("https://static.realmofempires.com/images/icons/Age" + CurrentAge + ".png", "Realm Age", popupTemp.html());

        if (!ROE.isMobile) {
            $(".pContainer").addClass("desktop");
        }
    }        


    function _timeleft (timeInMS) {
        
        var now = new Date();
        var ret = "Starts in ";
        var timeleft = timeInMS - now;
        
        var reth = Math.floor(timeleft / (1000 * 60 * 60 * 24));
        ret += reth + "d, ";
        timeleft -= reth * (1000 * 60 * 60 * 24);

        var reth = Math.floor(timeleft / (1000 * 60 * 60));
        ret += reth + "h, ";
        timeleft -= reth * (1000 * 60 * 60);

        var retm = Math.floor(timeleft / (1000 * 60));
        ret += retm + "m";
        timeleft -= retm * (1000 * 60);
        
        return ret;
    };
    obj.showPopup = _showPopup;

}(window.ROE.RealmAges = window.ROE.RealmAges || {}, jQuery));