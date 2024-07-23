



$(function () {
    BDA.Console.setCategoryDefaultView("ROE.Player.refresh", false);
    BDA.Console.setCategoryDefaultView("roe.api", false);
    BDA.Console.setCategoryDefaultView("BDA.Templates", false);
    BDA.Console.setCategoryDefaultView("MixPanel", false);

    //
    // preload templates
    //
    // all templates must exist or entire preload will fail 
    BDA.Templates.preload_asynch("map2,QuickRecruitTempl,VillageOverview,BuildingTempl2,GiftsPopup_d2", ROE.realmID, undefined, true);
    ROE.Frame.init();

    //
    // handle button pressed/clicked effect
    //
    var pressedEffect_addEffect = function (e) {
        $(e.currentTarget).addClass('pressed');
    }
    var pressedEffect_removeEffect = function (e) {
        $(e.target).removeClass('pressed');
    }

    $(document).delegate(".pressedEffect", "touchstart", pressedEffect_addEffect);
    $(document).delegate(".pressedEffect", "touchcanel", pressedEffect_removeEffect);
    $(document).delegate(".pressedEffect", "touchend", pressedEffect_removeEffect);
    $(document).delegate(".pressedEffect", "touchmove", pressedEffect_removeEffect);
    //
    // handle sounds
    //
    $(document).delegate(".sfx2", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.click();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_click();
        }
    });
    $(document).delegate(".sfxDefeat", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickDefeat();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickDefeat();
        }
    });
    $(document).delegate(".sfxLevelUp", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickLevelUp();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickLevelUp();
        }
    });
    $(document).delegate(".sfxReward", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickReward();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickReward();
        }
    });
    $(document).delegate(".sfxSpell", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickSpell();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickSpell();
        }
    });
    $(document).delegate(".sfxVictory", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickVictory();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickVictory();
        }
    });
    $(document).delegate(".sfxMenuExit", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickMenuExit();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickMenuExit();
        }
    });
    $(document).delegate(".sfxBuildingEnter", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickBuildingEnter();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickBuildingEnter();
        }
    });
    $(document).delegate(".sfxSwipe", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickActionSwipe();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickActionSwipe();
        }
    });
    $(document).delegate(".sfxScroll", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickActionScroll();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickActionScroll();
        }
    });
    $(document).delegate(".sfxOpen", "click", function (event) {
        // BDA.Console.verbose('', 'sound on' + event.eventTarget);
        ROE.UI.Sounds.clickActionOpen();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_clickActionOpen();
        }
    });
    // add touchevent to VoV main buttons
    $(document).delegate(".ButtonTouch", "touchstart", function (event) {
        $(event.currentTarget).addClass('ButtonTouchEffect');
    });

    $(document).delegate(".ButtonTouch", "touchend touchcanel touchmove", function (event) {
        setTimeout(function () { $(event.currentTarget).removeClass('ButtonTouchEffect'); }, 500);
    });

    //add BarTouchEffect
    $(document).delegate(".BarTouch", "touchstart", function (event) {
        $(event.currentTarget).addClass('BarTouchEffect');
    });
    $(document).delegate(".BarTouch", "touchend touchcanel touchmove", function (event) {
        setTimeout(function () { $(event.currentTarget).removeClass('BarTouchEffect'); }, 500);
    });


    //add RowTouchEffect
    $(document).delegate(".RowTouch", "touchstart", function (event) {
        $(event.currentTarget).addClass('RowTouchEffect');
    });
    $(document).delegate(".RowTouch", "touchend touchcanel touchmove", function (event) {
        setTimeout(function () { $(event.currentTarget).removeClass('RowTouchEffect'); }, 500);
    });



    //
    // rate us
    //
    $(".mobAppTryGift").click(
        function () {
            var content_step1;
            if (window.parent) {
                content_step1 = $('<div class=rateAppPopup ><CENTER><span class=title>Thank you for playing <BR>Realm of Empires!</span><BR> <img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" ><BR>We are new on Kongregate so we\'d appreciate your support.<BR><BR>Please help us grow by giving us a 5-STAR RATING. <BR><img class="rateit sfx2" src="https://static.realmofempires.com/images/icons/m_5star.png" ><BR><BR>Please accept a <span style=color:white;>GIFT of 50 SERVANTS</span> as a thank you for your support!<BR><BR><BR><img src="https://static.realmofempires.com/images/icons/questiong.png" style="height: 20px;">Have a question? <a class=askquestion>ask here</a><BR><img src="https://static.realmofempires.com/images/icons/exclamationg.png" style="height: 20px;">Got a cool idea? <a class=makesuggestion>suggest it here<a><BR><BR><div class="rateit rateitbut sfx2"></div><div class="nothanks sfx2">[not now, thanks]</div><img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" ></center></div>');
            } else {
                content_step1 = $('<div class=rateAppPopup ><CENTER><span class=title>Thank you for playing <BR>Realm of Empires!</span><BR> <img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" ><BR>We are new on Kongregate so we\'d appreciate your support.<BR><BR>Please help us grow by giving us a 5-STAR RATING. <BR><img class="rateit sfx2" src="https://static.realmofempires.com/images/icons/m_5star.png" ><BR><BR>Please accept a <span style=color:white;>GIFT of 50 SERVANTS</span> as a thank you for your support!<BR><BR><BR><img src="https://static.realmofempires.com/images/icons/questiong.png" style="height: 20px;">Have a question? <a class=askquestion>ask here</a><BR><img src="https://static.realmofempires.com/images/icons/exclamationg.png" style="height: 20px;">Got a cool idea? <a class=makesuggestion>suggest it here<a><BR><BR><div class="rateit rateitbut sfx2"></div><div class="nothanks sfx2">[not now, thanks]</div><img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" ><a class=askquestion>ask here<a></center></div>');
            }

            var content_step2 = $('<div class=rateAppPopup ><CENTER><span class=title>Thank you for helping us grow!</span><BR> <BR>Scroll to the botton of the window, and look for YOUR RATING where you can rate us.<BR><BR>5-STAR RATINGS help us grow and are much appreciated!<BR><BR><div class="donebut sfx2">OK, I am done!</div><div class="nothanks sfx2">[changed my mind, maybe later]</div></center></div>');

            // rate it button
            content_step1.find('.rateit').click(function () {
                if (window.parent != window) {
                    // replacement content with step 2 
                    ROE.Frame.popupInfoClose($(this));
                    ROE.Frame.popupInfo('', '300px', 'center', undefined, content_step2, true);
                } else {
                    // not in-frame, pop in frame, and reload this popup
                    
                    localStorage.setItem("popupRateUs", "yes"); // request to popup the rate us popup once loaded
                    window.location = "http://www.kongregate.com/games/BDAEntertainment/realm-of-empires";
                }
            });
            // done button
            content_step2.find('.donebut').click(function () {
                ROE.Frame.popupInfoClose($(this));

                $('.mobAppTryGift').fadeOut();
                ROE.Api.call("mobileapprate", {}, function () {
                    ROE.Frame.reloadFrame();//update servants
                });
                ROE.Frame.rateApp();

            });

            //no thanks 
            content_step1.find('.nothanks').click(function () { ROE.Frame.popupInfoClose($(this)); });
            content_step2.find('.nothanks').click(function () { ROE.Frame.popupInfoClose($(this)); });

            content_step1.find('.askquestion').click(function () { ROE.Frame.popupInfoClose($(this)); ROE.Utils.attentionGrabber($('#supportq'),2) });
            content_step1.find('.makesuggestion').click(function () { ROE.Frame.popupInfoClose($(this)); ROE.Utils.attentionGrabber($('#ideas'),2) });

            if (localStorage.getItem("popupRateUs") == "yes") {
                localStorage.removeItem("popupRateUs"); 
                ROE.Frame.popupInfo('', '400px', 'center', undefined, content_step2, true);
            }
            else {
                ROE.Frame.popupInfo('', '400px', 'center', undefined, content_step1, true);
            }
        }
    );


    //
    // rate us
    //
    $(".mobAppTryGift_AG").click(
        function () {
            var content_step1;
                content_step1 = $('<div class=rateAppPopup ><CENTER><span class=title>Thank you for playing <BR>Realm of Empires!</span><BR> <img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" ><BR>We are new on Armor Games so we\'d appreciate your support.<BR><BR>Please help us grow by giving us thumbs up / "like this". <BR><BR>Please accept a <span style=color:white;>GIFT of 50 SERVANTS</span> as a thank you for your support!<BR><BR><BR><img src="https://static.realmofempires.com/images/icons/questiong.png" style="height: 20px;">Have a question? <a class=askquestion>ask here</a><BR><img src="https://static.realmofempires.com/images/icons/exclamationg.png" style="height: 20px;">Got a cool idea? <a class=makesuggestion>suggest it here<a><BR><BR><div class="rateit rateitbut sfx2"></div><div class="nothanks sfx2">[not now, thanks]</div><img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" ></center></div>');

            var content_step2 = $('<div class=rateAppPopup ><CENTER><span class=title>Thank you for helping us grow!</span><BR> <BR>Scroll to the botton of the window, and look thumbs up / like it button.<BR><BR>Positive ratings help us grow and are much appreciated!<BR><BR><div class="donebut sfx2">OK, I am done!</div><div class="nothanks sfx2">[changed my mind, maybe later]</div></center></div>');

            // rate it button
            content_step1.find('.rateit').click(function () {
                    // replacement content with step 2 
                    ROE.Frame.popupInfoClose($(this));
                    ROE.Frame.popupInfo('', '300px', 'center', undefined, content_step2, true);
            });
            // done button
            content_step2.find('.donebut').click(function () {
                ROE.Frame.popupInfoClose($(this));

                $('.mobAppTryGift').fadeOut();
                ROE.Api.call("mobileapprate", {}, function () {
                    ROE.Frame.reloadFrame();//update servants
                });
                ROE.Frame.rateApp();

            });

            //no thanks 
            content_step1.find('.nothanks').click(function () { ROE.Frame.popupInfoClose($(this)); });
            content_step2.find('.nothanks').click(function () { ROE.Frame.popupInfoClose($(this)); });

            content_step1.find('.askquestion').click(function () { ROE.Frame.popupInfoClose($(this)); ROE.Utils.attentionGrabber($('#supportq'), 2) });
            content_step1.find('.makesuggestion').click(function () { ROE.Frame.popupInfoClose($(this)); ROE.Utils.attentionGrabber($('#ideas'), 2) });

            if (localStorage.getItem("popupRateUs") == "yes") {
                localStorage.removeItem("popupRateUs");
                ROE.Frame.popupInfo('', '400px', 'center', undefined, content_step2, true);
            }
            else {
                ROE.Frame.popupInfo('', '400px', 'center', undefined, content_step1, true);
            }
        }
    );

    //
    // popup rate us if requested to do so
    if (localStorage.getItem("popupRateUs") == "yes") {
        //localStorage.removeItem("popupRateUs"); -- do not do this. it is done in click of popup 
        $(".mobAppTryGift").click();
    }


    $(".Offer2").click(ROE.Frame.showBuyCredits);


    //if (ROE.Entities.Ages != null) {

    //    $("#currentAge")
    //        .css("background-image", "url('https://static.realmofempires.com/images/icons/Age" + ROE.Entities.Ages.Age.CurrentAge.AgeNumber + ".png')")
    //        .click(ROE.RealmAges.showPopup).show();
    //}


    $('#roeOptions .movetoCoord').bind("click", function () {

        if ($(".movetoCoordBox").length == 0) {

            var popupbox = '<div class=movetoCoordBox  > X <input type="number" class="x" >';
            popupbox += ' Y <input type="number" class="y" >';
            popupbox += '<BR><BR><span class="go customButtomBG sfx2" >GO</span>';
            popupbox += '</div>';

            //call popup box template (title, width, vertAllign, bgcolor, content)
            ROE.Frame.popupInfo("Go To Map Coordinates ", "230px", "center", "rgba(0,0,0,0.3)", popupbox, false);

            $(".movetoCoordBox .go").bind("click", function () {

                var X = parseInt($('.movetoCoordBox .x').val());
                var Y = parseInt($('.movetoCoordBox .y').val());

                if (!isNaN(X) && !isNaN(Y)) {
                    ROE.Landmark.gotodb(X, Y);
                    ROE.Landmark.select();

                    //we call it manually here to hurry up incase we have ported to a new section of the map with no village info yet
                    ROE.Landmark.checkvills();
                }

                ROE.Frame.popupInfoClose($(this));
            });
        }

    });


    $('#roeOptions .direction').bind("click", function () {
        var direction = BDA.Database.LocalGet('TroopDirection') || "outgoing";

        var imgIn = "https://static.realmofempires.com/images/icons/M_MoreInc.png";
        var imgOut = "https://static.realmofempires.com/images/icons/M_MoreOut.png";

        var incomingtext = "incoming";
        var outgoingtext = "outgoing";
        var troopstext = "troops";
        var directiontext1 = 'You will now see <span>incoming</span> troops to your villages on the map.';
        var directiontext2 = 'Look for this icons on villages:'
        var directiontext3 = 'Show <span>incoming</span> attacks';
        var directiontext4 = 'Show <span>incoming</span> support';
        var directiontitle = "Show Troops Movement";
        var clicktext = "Click to toggle movement.";


        var popupbox = '<div class=directionBox  >';
        popupbox += '<div class="directionToggle sfx2" data-dir=' + direction + ' ><img src="' + imgIn + '"  >';
        popupbox += '<img src="https://static.realmofempires.com/images/misc/M_ArrowL.png" ><div >' + clicktext + '</div></div>';
        popupbox += '<div class=dirtext >' + incomingtext + " " + troopstext + '</div>';
        popupbox += '<div class=dirinfo >' + directiontext1 + '</div>';
        popupbox += '<div class=dirinfo2 >' + directiontext2 + '</div>';
        popupbox += '<div class=dirlegends ><img src="https://static.realmofempires.com/images/attack.png"> <div>' + directiontext3 + '</div></div>';
        popupbox += '<div class=dirlegends ><img src="https://static.realmofempires.com/images/support.png"> <div>' + directiontext4 + '</div></div>';

        //call popup box template (title, width, vertAllign, bgcolor, content)
        ROE.Frame.popupInfo(directiontitle, "230px", "center", "rgba(0,0,0,0.3)", popupbox, false);

        setDirection(direction);

        $('.directionBox .directionToggle').click(function () {
            var directionSet = $(this).attr("data-dir");
            directionSet == "incoming" ? directionSet = "outgoing" : directionSet = "incoming";
            setDirection(directionSet);
        });

        function setDirection(direction) {

            switch (direction) {
                case "outgoing":
                    var datatext = outgoingtext;
                    var dataImg = imgOut;
                    break;
                case "incoming":
                    var datatext = incomingtext;
                    var dataImg = imgIn;
                    break;
                default: var datatext = outgoingtext;
                    var dataImg = imgOut;
            }

            $('.directionBox .directionToggle').attr("data-dir", datatext);
            $('.directionBox .dirtext').html(datatext + " " + troopstext);
            $('.directionBox .directionToggle IMG:first-child').attr("src", dataImg);
            $('.directionBox .dirlegends SPAN').html(datatext);
            $('.directionBox .dirinfo SPAN').html(datatext);
            $('#roeOptions .direction').attr("data-troops", direction);
        }

    });


    $('.emailEntryIcon').bind("click", function () {
        ROE.Frame.popupAccount();
    });

    $('.inviteFriendsReward').bind("click", function () {
        ROE.Frame.showIframeOpenDialog('#inviteFriendsRewardDialog', 'FriendReward.aspx');
    });
    $('#playOnMinfo').bind("click", function () {
        ROE.Frame.showIframeOpenDialog('#playOnMDialog', 'playonM.aspx');
    });
    $('#RXIcon').bind("click", function () {
        ROE.Frame.showIframeOpenDialog('#rxInfoDialog', 'RXinfo.aspx');
    });



    ///buffer sounds for Amazon and etc
    bufferHTML5Audio();

    function bufferHTML5Audio() {

        ROE.UI.Sounds.bufferedSFX[1] = new Audio("https://static.realmofempires.com/sfx/sfx_ui_action_click.ogg");
        ROE.UI.Sounds.bufferedSFX[2] = new Audio("https://static.realmofempires.com/sfx/sfx_event_defeat.ogg");
        ROE.UI.Sounds.bufferedSFX[3] = new Audio("https://static.realmofempires.com/sfx/sfx_event_levelup.ogg");
        ROE.UI.Sounds.bufferedSFX[4] = new Audio("https://static.realmofempires.com/sfx/sfx_event_reward.ogg");
        ROE.UI.Sounds.bufferedSFX[5] = new Audio("https://static.realmofempires.com/sfx/sfx_event_spell.ogg");
        ROE.UI.Sounds.bufferedSFX[6] = new Audio("https://static.realmofempires.com/sfx/sfx_event_victory.ogg");
        ROE.UI.Sounds.bufferedSFX[7] = new Audio("https://static.realmofempires.com/sfx/sfx_menu_exit.ogg");
        ROE.UI.Sounds.bufferedSFX[8] = new Audio("https://static.realmofempires.com/sfx/sfx_building_enter.ogg");
        ROE.UI.Sounds.bufferedSFX[9] = new Audio("https://static.realmofempires.com/sfx/sfx_ui_action_swipe.ogg");
        ROE.UI.Sounds.bufferedSFX[10] = new Audio("https://static.realmofempires.com/sfx/sfx_ui_action_scroll.ogg");
        ROE.UI.Sounds.bufferedSFX[11] = new Audio("https://static.realmofempires.com/sfx/sfx_ui_action_open.ogg");

        ROE.UI.Sounds.bufferedMusic[1] = new Audio("https://static.realmofempires.com/sfx/m_01A_underscore_loop.ogg");

    }

});