



$(function () {
    BDA.Console.setCategoryDefaultView("ROE.Player.refresh", false);
    BDA.Console.setCategoryDefaultView("roe.api", false);
    BDA.Console.setCategoryDefaultView("BDA.Templates", false);
    BDA.Console.setCategoryDefaultView("MixPanel", false);

    //
    // preload templates
    //
    // all templates must exist or entire preload will fail 
    BDA.Templates.preload_asynch("map2,QuickRecruitTempl,VillageOverview,BuildingTempl2,GiftsPopup", ROE.realmID, undefined, true);


    $(".footernav .togle").click(function () { ROE.Frame.fnTogleView(); });
    $("#viewSwitch_res").click(function () { ROE.Research.showResearchPopup(); });

    $(".hvov.hdrVInfo").click(ROE.Frame.showVillList);
    $("header #hdrPF").click(ROE.Frame.showServantsPopup);
    $("header .pfStatus").click(ROE.Frame.showServantsPopup);
    $("header #hdrCoins").click(ROE.Frame.showResPopup);
    $("header #hdrLoyalty").click(ROE.Frame.showResPopup);
    $("header #hdrFood").click(ROE.Frame.showResPopup);
    $("header #hdrTroops").click(ROE.Frame.showTroopPopup);
    $("#hdrBuild").click(function (event) {
        event.preventDefault();

        ROE.Frame.showBuildPopup();
    });
    $(".footernav .more").click(ROE.Frame.toggleOptionsPopup);

    $(".footernav").delegate(".next", "click", function (event) {
        var p = $(this).closest(".group");
        BDA.UI.Transition.slideLeft(p.next(), p);
    });
    $(".footernav").delegate(".prev", "click", function (event) {
        var p = $(this).closest(".group");
        BDA.UI.Transition.slideRight(p.prev(), p);
    });


    $("footer .mail").click(function (e) {
        e.preventDefault();

        ROE.Frame.popupMail();

        return false;
    });   // for now, we want this to open in a new window on all platforms

    $("#sleepMode").click(ROE.SleepMode.showPopup);
    $("#itemRewardsIcon").click(ROE.Items2.showPopup);
    $("#launchRaidsPopup").click(function () { ROE.Raids.showPopup(); });
    $("footer .reports").click(function () { ROE.Frame.popupReports(); }); // for now, we want this to open in a new window on all platforms
    $("footer .items").click(function () { ROE.Frame.popupGifts(); return false; });
    $("#roeOptions .notifications").click(ROE.Frame.settingsPopup);
    $("footer .clan").click(function () { ROE.Frame.popupClan(0); });

    if (!ROE.NoPopupBrowser) {
        $("#linkBattleSim").click(function () { return !popupModalIFrame2('battlesimulator.aspx', 'BattleSimulator', 'Battle Simulator', 'https://static.realmofempires.com/images/icons/M_BattleSim.png') });
        $("#linkRanking").click(function () { return !popupModalIFrame2('stats.aspx', 'Ranking', 'Ranking', 'https://static.realmofempires.com/images/icons/m_ranking.png') });
        $("header .pInfo a").click(function () { ROE.Frame.popupPlayerProfile($(this).find('.hdrPName').html()); /*return !popupModalIFrame2('PlayerPopup.aspx?pid=' + ROE.playerID, 'player', 'player info', 'https://static.realmofempires.com/images/icons/m_ranking.png') */ }); // for now, we want this to open in a new window on all platforms
        $("#roeOptions .settings.button > a").click(function () { return !popupModalIFrame2('PlayerOptions.aspx', 'popup', 'Tools & Options', 'https://static.realmofempires.com/images/icons/M_Settings.png'); });
        $(".inviteFriendsReward").click(function () { return !popupModalIFrame2('friendreward.aspx', 'InviteFriends', 'Invite Friends', 'https://static.realmofempires.com/images/icons/m_ranking.png') });
    } else {
        $("header .pInfo a").click(function () { ROE.Frame.popupPlayerProfile($(this).find('.hdrPName').html()); /* return !window.open('PlayerPopup.aspx?pid=' + ROE.playerID) */ });
        // Stats has been modified to work as a popup so it needs to be triggered this way.
        // Otherwise player links won't work on the page.
        $("#linkRanking a").click(function () { return !popupModalIFrame2('stats.aspx', 'Ranking', 'Ranking', 'https://static.realmofempires.com/images/icons/m_ranking.png') });
    }

    $("#RXIcon").click(function () { return !popupModalIFrame2('RXInfo.aspx', 'RXInfo', 'RX Info', 'https://static.realmofempires.com/images/icons/m_ranking.png') });
    $("footer .footernav .inOutTroops").click(function () { ROE.UI.Sounds.click(); ROE.Frame.popupInOut(); });


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
    // temp code for now 
    //
    $(".mobAppTryGift").click(
        function () {
            var content = $('<div class=rateAppPopup ><CENTER><span class=title>Thank you for playing <BR>Realm of Empires mobile!</span><BR> <img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" ><BR>Expect many new features (like the new alerts) on a regular basis!<BR><BR>Please help us grow by giving us a 5-STAR RATING. <BR><img class="rateit sfx2" src="https://static.realmofempires.com/images/icons/m_5star.png" ><BR><BR>Please accept a <span style=color:white;>GIFT of 50 SERVANTS</span> as a thank you for your support!<BR><BR><div class="rateit rateitbut sfx2"></div><div class="nothanks sfx2">[not now, thanks]</div><img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" ></center></div>');

            content.find('.rateit').click(function () {

                ROE.Frame.popupInfoClose($(this));

                $('.mobAppTryGift').fadeOut();
                ROE.Api.call("mobileapprate", {}, function () {
                    ROE.Frame.reloadFrame();//update servants
                });
                ROE.Frame.rateApp();

            });

            content.find('.nothanks').click(function () { ROE.Frame.popupInfoClose($(this)); });


            ROE.Frame.popupInfo('', '300px', 'center', undefined, content, true);
        }
    );

    $(".Offer2").click(ROE.Frame.showBuyCredits);


    if (ROE.Entities.Ages != null) {

        $("#currentAge").show()
            .css("background", "url('https://static.realmofempires.com/images/icons/Age" + ROE.Entities.Ages.Age.CurrentAge.AgeNumber + ".png') no-repeat center")
        //.click(ROE.RealmAges.showPopup);
        .click(function () {
            ROE.Frame.popGeneric('Age information', '', 320, 320);
            ROE.Frame.showIframeOpenDialog('#genericDialog', 'currentAgeInfo.aspx');
        });
    }

    //
    // temp code - for now, for ios,we display the 2 step intro to the new alert system 
    //
    /*
    if (!localStorage.AlertMinTutorialSeen && ROE.Device.type === ROE.Device.CONSTS.iOS) {
        localStorage.AlertMinTutorialSeen = true;
        ROE.Tutorial.start(0, 'newAlertSystemNotification')
    }
    */


    $('#roeOptions .settings').bind("click", function () {

        //$("#roeOptions .settingsBar").toggleClass("showsup0");
        $("#roeOptions .moretools").toggleClass("showsup");


    });

    $('#roeOptions .movetoCoord').bind("click", function () {
        var popupbox;
        if ($(".movetoCoordBox").length == 0) {

            if (ROE.Browser.android) {
                /// http://stackoverflow.com/questions/23205511/what-models-of-samsung-smartphones-have-missing-period-for-html5-input-type-num
                popupbox = '<div class=movetoCoordBox  > X <input type="text" class="x" >';
                popupbox += ' Y <input type="text" class="y" >';
                popupbox += '<BR><BR><span class="go customButtomBG sfx2" >GO</span>';
                popupbox += '</div>';
            } else {
                popupbox = '<div class=movetoCoordBox  > X <input type="number" class="x" >';
                popupbox += ' Y <input type="number" class="y" >';
                popupbox += '<BR><BR><span class="go customButtomBG sfx2" >GO</span>';
                popupbox += '</div>';
            }

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

        var direction = BDA.Database.LocalGet('TroopDirection');
        if (direction == "incoming") { var tempdir = "outgoing"; }
        else { var tempdir = "incoming"; }
        //set current interface settings
        setDirection(tempdir);


        $('.directionBox .directionToggle').click(function () {

            var direction = $('.directionBox .directionToggle').attr("data-dir");
            var BGimg = imgOut;
            var newDirection = "incoming";

            //change interface settings
            setDirection(direction);

            //set toggle, as opposite of current settings            
            if (direction == "incoming") { var newDirection = "outgoing"; }
            if (newDirection == "incoming") { var BGimg = imgIn; }
            //set Incom/Outgo image on MoreButton
            $('#roeOptions-map .direction').attr("data-troops", direction).css("background-image", "url(" + BGimg + ")");

        });




        function setDirection(direction) {

            switch (direction) {
                case "incoming":
                    var datatext = outgoingtext;
                    var dataImg = imgOut;
                    break;
                case "outgoing":
                    var datatext = incomingtext;
                    var dataImg = imgIn;
                    break;

                default: var datatext = incomingtext;
                    var dataImg = imgIn;
            }

            $('.directionBox .directionToggle').attr("data-dir", datatext);
            $('.directionBox .dirtext').html(datatext + " " + troopstext);
            $('.directionBox .directionToggle IMG:first-child').attr("src", dataImg);
            $('.directionBox .dirlegends SPAN').html(datatext);
            $('.directionBox .dirinfo SPAN').html(datatext);
        }

    });

    $('.emailEntryIcon').bind("click", function () {
        ROE.Frame.popupAccount();
    });

    ROE.Device.loggedOn();

    ///buffer sounds for Amazon and etc
    if (ROE.isDevice == ROE.Device.CONSTS.Amazon /*|| ROE.isDevice == 0*/) {
        bufferHTML5Audio();
        //ROE.UI.Sounds.playHTML5Music(1);
    }

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
        //ROE.UI.Sounds.bufferedMusic[1] = new Audio("https://static.realmofempires.com/sfx/m_01A_underscore_loop.ogg");        
    }

    function orientationchange(event) {
        $('#screenOrientationMask').remove();
        if (window.orientation == 90 || window.orientation == -90) {
            $('<div>').attr('id', 'screenOrientationMask')
                .append('<p>Please rotate the device into portrait position.</p>')
                .append('<img src="https://static.realmofempires.com/images/misc/orientChangeLock.png"/>')
                .appendTo('body');
        }
    }

    window.addEventListener("orientationchange", orientationchange, false);

});