/// <reference path="BDA-utils.js" />


ROE.Utils = {

    //
    // link : the A that represents the name of the village that should change
    // donePreparingCallback : optional callback function when this method is finished changing the UI to edit the village name
    // doneSavedCallback : optional call back that is called after the save button is cliced AND after the ajax call to rename the village successfully returns
    //
    RenameVillage: function (vID, link, donePreparingCallback, doneSavedCallback) {

        //only one element
        if (!$("input#ROEUtils_RenameVilage").length) {
            link = $(link);
            var textBox = $('<input type="text" class="TextBox" maxlength="' + ROE.CONST.VillageNameMaxLength.toString() + '" id="ROEUtils_RenameVilage"></input>');
            var button = $('<input type="submit" class="inputbutton sfx2"  value=' + BDA.Dict.Save + ' />');

            var textBox = $('<input type="text" class="TextBox" maxlength="' + ROE.CONST.VillageNameMaxLength.toString() + '" id="ROEUtils_RenameVilage"></input>');

            var vilName;
            if (link.text().match(ROE.CONST.regex_VillageCordinates) != null) {
                vilName = link.text().replace(link.text().match(ROE.CONST.regex_VillageCordinates)[0], ""); //crude but works - find the cords in village display, then remove them and take the rem string which should be village name
            }
            else {
                vilName = link.text();
            }
            villCoord = $(".vilres_nameloc_txt").next().text();
            $(".vilres_nameloc_txt").next().text("");

            textBox.val(vilName);
            button.click(function () { ROE.Utils.RenameVillage_Save(vID, link, textBox, button, doneSavedCallback); return false; });

            link.after(button);
            link.after(textBox);

            link.hide();

            textBox.focus().select();          

            if (typeof donePreparingCallback === 'function') { donePreparingCallback(); }

        }
    },

    PopupRenameVillage: function (vid,vname) {

        var popupbox = '<div class=renameVillagePopup>';
        var villageID = vid || ROE.SVID;
        var currentName = vname || $('#vName').html(); //'#vName is in M's map village select hud  

        popupbox += 'Name:<input class="name" style="width: 115px; margin-left: 3px;"><input type="button" class="go sfx2" value="save"  >';
        popupbox += '</div>';

        //call popup box template (title, width, vertAllign, bgcolor, content)
        var $popupbox = $(popupbox);
        ROE.Frame.popupInfo("Rename Village ", "230px", "115px", "rgba(0,0,0,0.3)", popupbox);
        
        $('.renameVillagePopup .name').val(currentName);

        $(".renameVillagePopup .go").click(function () {
            villCoord = '';

            ROE.Utils.RenameVillage_Save(
                villageID,
                $('#vName'),
                $('.renameVillagePopup .name'),
                $('.renameVillagePopup .go'),
                ROE.Utils.updateVillageName
            )

            ROE.Frame.popupInfoClose($('.renameVillagePopup'));
        });

        if (ROE.isD2) {
            $('.popupInfo_box.popupOpenBox').position({
                my: "center center",
                at: "center center",
                of: window
            });
        }
    },

    //
    //
    // link : expecting a jquery object
    // textBox : expecting a jquery object
    // button : expecting a jquery object
    //
    RenameVillage_Save: function (vid, link, textBox, button, doneSavedCallback) {

        var villageName = textBox.val();
        villageName = villageName.trim();

        if (villageName.length < 1) {
            alert('Empty name not allowed');
            return;
        }

        //helper "**)[1(23][]*()&".match("^[a-zA-Z0-9)(*[\\]._]{1,25}$");
        var regex = "^[a-zA-Z0-9%)(*[\\]._\\,\\-\\^\\#\\@\\|\\+\\~\\!\\{\\} ]{1,25}$";
        if (!villageName.match(regex)) {
            alert('Allowed characters: A to Z, 0 to 9, . _ - ^ [] () {} % * # @ ! | ~ + , and space.');
            return;
        }

        button.unbind();
        button.attr("disabled", "true");
        var busyImage = $('<img src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />')
        button.after(busyImage);
        ajax('utilsajax.aspx', { func: 'remv', vid: vid.toString(), vn: villageName }, ROE.Utils.RenameVillage_Saved(link, textBox, button, busyImage, doneSavedCallback));

    },

    RenameVillage_Saved: function (link, textBox, button, busyImage, doneSavedCallback) {
        return function (result) {

            if (link.text().match(ROE.CONST.regex_VillageCordinates) != null) {
                link.text(result.name + ' ' + link.text().match(ROE.CONST.regex_VillageCordinates)[0]);
            }
            else {
                link.text(result.name);
            }

            busyImage.fadeOut('fast', function () { $(this).remove(); });
            textBox.fadeOut('fast', function () { $(this).remove(); });
            button.fadeOut('fast', function () {
                link.css('display', 'inline').fadeIn('fast');
                $(this).remove();
                $(".vilres_nameloc_txt").next().text(villCoord);
            });

            if (ROE.isM || ROE.isD2) {
                ROE.Villages.ExtendedInfo_loadLatest(result.id);
                ROE.Frame.refreshQuestCount();
            }


            if (typeof doneSavedCallback === 'function') { doneSavedCallback(result); }
        }
    }

    , updateVillageName: function (res) {
        ROE.Landmark.updateVillageName(res.name, res.id);
    }

    , addThousandSeperator: function (nStr) {
        ///<summary> pass in '1000.0' get '1,000.0'</summar>
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


    , formatShortNum: function (num) {
        ///<summary>depreciated. use the BDA.Utils version</summary>
        return BDA.Utils.formatShortNum(num);
    }

    , formatShortTxt: function (txt, len) {
        ///<summary>depreciated. use the BDA.Utils version</summary>
        return BDA.Utils.formatShortTxt(txt, len);
    }

    , padDigits: function padDigits(num) {
        ///<summary>depreciated. use the BDA.Utils version</summary>
        return BDA.Utils.padDigits(num);
    }

    , toTextarea: function (str) {
        return str.replace(/<br \/>/g, '\n');
    }
    , toHtml: function (str) {
        return str.replace(/\n/g, '<br />');
    }

    , formatNum: function (num) {
        ///<summary>use this to properly format a number for display - formats are different when app is in mobile and when it is not</summary>
        ///<param name="num" >number or numerical string.  e.g. 12345, "12,345" </param>
        if (ROE.isMobile) {
            return BDA.Utils.formatShortNum(num);
        }
        else {
            return BDA.Utils.formatNum(num); // "12111" -> "12,111"
        }
    }


    , convertCountdownDisplayToTimeLeft: function (countdownString) {
        ///<summary> pass in "hh:mm:ss" and get a time left info </summary>
        var time = countdownString.split(':', 3);
        for (i = 0; i < 3; i++) {
            time[i] = parseInt(time[i], 10);
        }
        var now = new Date();
        var finish = new Date();

        finish.setHours(finish.getHours() + time[0]);
        finish.setMinutes(finish.getMinutes() + time[1]);
        finish.setSeconds(finish.getSeconds() + time[2]);

        var left = finish - now;

        return { leftMS: left, leftSec: left / 1000, leftMin: left / (1000 * 60) }
    }

    //
    //
    // deal with calculating possible upgrade speed up options to offer to the player
    //
    //
    , UpgradeSpeedUp: {
        costOfTimeCut: function (minutesToCut) {
            var time
            for (var i = 0, time; time = ROE.UpgradeSpeedUps_timeCuts[i]; ++i) {
                if (time == minutesToCut) {
                    return ROE.UpgradeSpeedUps_timeCutCosts[i];
                }
            }
            return 0;
        }


        , calculateFinishNow: function (minutesLeft, minutesCanSpeedUp) {
            ///<summary>returns an object with "cost" (total cost for 'finish now') but only valid if allowFinishNow==true
            /// "allowFinishNow" tells you if finishNow shoudl be available (curently, only reason it is not, is because player runs out of daily allowance)
            /// and "listOfTimeCuts" (array of seperate time cuts to display) </summary>
            var returnVal = { cost: 0, listOfTimeCuts: [], allowFinishNow: true };

            // if totaal time left more than what we can do now
            if (minutesLeft > minutesCanSpeedUp)
            {
                minutesLeft = minutesCanSpeedUp;
                returnVal.allowFinishNow = false;
            }

            ROE.Utils.UpgradeSpeedUp._calculateFinishNow(minutesLeft, 0, returnVal);

            return returnVal;

        }

        , _calculateFinishNow: function (minutesLeft, loc, returnVal) {
            // minutesCanSpeedUp - daily limit allowance left for today
            var nextLowerCut;
            var nextLowerCost;

            if (loc < ROE.UpgradeSpeedUps_timeCuts.length - 1) {
                nextLowerCut = ROE.UpgradeSpeedUps_timeCuts[loc + 1];
                nextLowerCost = ROE.UpgradeSpeedUps_timeCutCosts[loc + 1];
            }
            else {
                nextLowerCut = 0;
                nextLowerCost = 1;
            }

            while (minutesLeft > nextLowerCut
                && (Math.ceil(minutesLeft) / nextLowerCut * nextLowerCost) >= ROE.UpgradeSpeedUps_timeCutCosts[loc]
                //&& ROE.UpgradeSpeedUps_timeCuts[loc] <= minutesCanSpeedUp 
                ) {
                returnVal.listOfTimeCuts.push(ROE.UpgradeSpeedUps_timeCuts[loc]);
                minutesLeft -= ROE.UpgradeSpeedUps_timeCuts[loc];
                returnVal.cost += ROE.UpgradeSpeedUps_timeCutCosts[loc];
            }

            if (loc < ROE.UpgradeSpeedUps_timeCutCosts.length - 1) {
                ROE.Utils.UpgradeSpeedUp._calculateFinishNow(minutesLeft, loc + 1, returnVal);
            }

        }

    }

    , isValidEmailFmt: function (email) {
        var re = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$/;
        return re.test(email.toLowerCase());
    }

    , phrase: function (phrasesId,phrase) {
        return $('#' + phrasesId + ' [ph=' + phrase + ']').html();
    }


    , CalculateDistanceBetweenVillages : function(originX, originY, destinationX, destinationY)
    {
        var x, y;
        x = Math.abs(originX - destinationX);
        y = Math.abs(originY - destinationY);

        x = x * x;
        y = y * y;

        return  Math.sqrt(x + y);
    }

    , getTroopStrength : function getTroopStrength (village) {
        ///<summary>returns {att:X, def: Y}</summary>
        var ret = { att: 0, def: 0 };
        for (var i = 0, unit; (unit = village.TroopList[i]) ; ++i) {
            var unitType = ROE.Entities.UnitTypes[unit.id];
            ret.att += unitType.AttackStrength * unit.YourUnitsCurrentlyInVillageCount;
            ret.def += unitType.AvgDefenseStrength * (unit.SupportCount + unit.YourUnitsCurrentlyInVillageCount);
        }
        return ret;
    }

    , getImcomingMiniIndicatorsHTML: function getImcomingMiniIndicatorsHTML(list, maxIndicators, doNotDo9plus) {        
        var imagesHtml = "";
        // find what type of incoming is landing first, and then check if another type is landing next
        if (list.length > 0) {
            for (var i = 0; i < list.length; i++) {               
                imagesHtml += '<span class="incomingMiniIndicator indicatorType%type% %hidden% indicatorCountCharCount%charCount%">%count%</span>'.format(
                    {
                        type: list[i].type == ROE.Troops.InOut2.Enum.CommandTypes.attack ? "Attack" : "Support",
                        hidden: list[i].isHidden ? "indicatorHidden" : "",
                        count: doNotDo9plus ? list[i].count : (list[i].count < 10 ? list[i].count : "9+")
                    });

                if (i >= maxIndicators - 1) {
                    // we already got the max indicators
                    if (list.length > i + 1) {
                        imagesHtml += "<span class='incomingMiniIndicator indicatorMore'>..</span>";
                    }
                    break;
                }
            }
        }
        return imagesHtml;
    }

    , msToTime: function msToTime(ms) {
        var seconds = parseInt((ms / 1000) % 60)
            , minutes = parseInt((ms / (1000 * 60)) % 60)
            , hours = parseInt((ms / (1000 * 60 * 60)) % 24);
        hours = (hours < 10) ? "0" + hours : hours;
        minutes = (minutes < 10) ? "0" + minutes : minutes;
        seconds = (seconds < 10) ? "0" + seconds : seconds;
        return hours + ":" + minutes + ":" + seconds;
    }

     , getServerTimeOffset: function getServerTimeOffset() {

         var nextCallTimer = 0;
         var localTimeCall = (new Date()).valueOf();
         ROE.Api.call_getservertimeoffset(localTimeCall, offsetReturn);

         function offsetReturn(data) {

             var serverNow = data.serverNow; //server time at reception
             var serverDifference = data.serverDifference; //local / server difference at reception
             var localTimeAnswer = (new Date()).valueOf(); //local time at answer
             var answerDifference = serverNow - localTimeAnswer; //local / server difference at answer
             var networkOffset = Math.round((serverDifference + answerDifference) / 2); //average the offsets to factor in roundtrip delays

             //the difference between the time differences
             var variation = Math.abs(serverDifference - answerDifference);
            
             //set offset if its the best (least) variation found so far
             if (!ROE.timeOffset || variation < ROE.timeOffsetVariation /* networkOffset < ROE.timeOffset*/) {
                 //BDA.Console.log('New ROE.timeOffset:' + networkOffset);
                 ROE.timeOffset = networkOffset;
                 ROE.timeOffsetVariation = variation;
             }

             //BDA.Console.log('variation: ' + variation + ' _ networkOffset:' + networkOffset);

             //keep checking for a better variance, less and less often, down to once a minute              
             var timer = window['getServerTimeOffsetTimer'] || 5000;
             timer = timer * 2;
             timer = Math.min(timer, 60000);
             window.setTimeout(function () {
                 ROE.Utils.getServerTimeOffset();
             }, timer);
             window['getServerTimeOffsetTimer'] = timer;
             //BDA.Console.log('variation was too much, trying again.');
         }

     }

    , getElementCoords: function TutorialGetElementCoords(elemSelector) {

        var elem = $(elemSelector);
        if (elem.length < 1) {
            //it means the selector given was wrong or item doesnt exist in DOM at this moment
            //this shouldnt normally happen, if it does, something different has gone wrong, handle gracefully
            return { width: 0, height: 0, top: 0, left: 0 };
        }

        var elw = elem.outerWidth();
        var elh = elem.outerHeight();
        var elp = elem.offset();
        var elt = Math.ceil(elp.top);
        var ell = Math.ceil(elp.left);

        // offset works great, but dont count cases of transition scale, that is on vov
        if (elem.parents('.mainView .vovMain').length > 0) {
            elw = Math.ceil(elw * 0.67);
            elh = Math.ceil(elh * 0.67);
        }

        if (!ROE.isMobile && elem.parents('.vovMain').length > 0) {
            var ps = elem.parents('.vovMain').offset();
            var fo = $('#form1').offset();
            ps.top -= fo.top;
            ps.left -= fo.left;
            elw = Math.ceil(elw * 0.8);
            elh = Math.ceil(elh * 0.8);
            elt = Math.ceil(elt);
            ell = Math.ceil(ell);
        }

        return { width: elw, height: elh, top: elt, left: ell };
    }

    , attentionGrabber: function attentionGrabber(jQueryElement, numTimes) {
        BDA.Utils.attentionGrabber(ROE.Utils.getElementCoords(jQueryElement));
        if (numTimes > 1) {
            setTimeout(function () { ROE.Utils.attentionGrabber(jQueryElement, numTimes - 1) }, 1000);
        }
    }

    , cDateToJsDate: function cDateToJsDate(cDateString) {
        return new Date(parseInt(cDateString.substr(6)));
    }
}
