//ROE.FrameUtils = {};





ROE.Frame.handleTargetsListIcon = function handleTargetsListIcon() {
    ROE.Targets.updateTargetsListLaunchIcon($('#launchTargetsListSupport'));
    ROE.Targets.updateTargetsListLaunchIcon($('#launchTargetsListAttack'));
}

ROE.Frame.handleDailyReward = function handleDailyReward() {
    if (ROE.Player.dailyRewardAvail) {
        ROE.DailyReward.nextRewardReady();
    } else {
        ROE.DailyReward.nextRewardCounting();
    }
}

ROE.Frame.handleResearchIcon = function handleResearchIcon() {
    var numberImg;
    var researchButton;
    var researchSpan;
    var counter ;
    var ctext ;
    researchButton = $("#viewSwitch_res");
    researchSpan = $("#viewSwitch_res SPAN");

    // Research, if active on this realm
    if (researchButton.length > 0) {
        if (ROE.Player.MyResearch.numOfIdleResearchers > 0) {
            //
            // if research not active, put counter of how many are available
            if (researchSpan.attr("data-numOfIdleResearchers") != ROE.Player.MyResearch.numOfIdleResearchers) {
                researchButton.removeClass("researchersBusy");
                ROE.Frame.iconNeedsAttention(researchButton, true);
                numberImg = "<img src=https://static.realmofempires.com/images/fonts/num_B%numOfIdleResearchers%.png />".format(ROE.Player.MyResearch);
                researchSpan.html(numberImg);
                researchSpan.attr("data-numOfIdleResearchers", ROE.Player.MyResearch.numOfIdleResearchers);
                researchSpan.attr("data-researcherIdleIn", "");
                researchSpan.removeClass("countdown");
                researchSpan.attr("data-finisheson", '');
            }
        } else if (ROE.Player.MyResearch.researcherIdleIn > 0) {
            //
            // if resarch is active, put a counter
            if (researchSpan.attr("data-researcherIdleIn") != ROE.Player.MyResearch.researcherIdleIn) {
                researchButton.addClass("researchersBusy");
                ROE.Frame.iconNeedsAttention(researchButton, false);
                researchSpan.attr("data-numOfIdleResearchers", "");
                researchSpan.attr("data-researcherIdleIn", ROE.Player.MyResearch.researcherIdleIn);
                researchSpan.attr("data-finisheson", ROE.Player.MyResearch.researcherIdleIn);
                researchSpan.addClass("countdown");
                initTimers();
            }
        }
    }
}

ROE.Frame.researchersIdleInCountdownAtZero = function researchersIdleInCountdownAtZero() {
    // just remove the counter and such 
    $("#viewSwitch_res").removeClass("researchersBusy");
    $("#viewSwitch_res SPAN").html("");
    $("#viewSwitch_res SPAN").removeClass("countdown");

    // what is this? see case 27499
    ROE.Villages.ExtendedInfo_loadLatest(ROE.SVID);
}

ROE.Frame.handleSleeModeIcon = function handleSleeModeIcon() {
    //sleepmode checks
    if (ROE.Player.SleepinRealm.IsAvailableOnThisRealm) {

        var sleepAvailable = true;

        if (ROE.Player.SleepModeCountdown.indexOf("-") == -1 && ROE.Player.sleepModeOn < 0) {
            $("#sleepMode").addClass("deactive").show();
            $("#sleepMode SPAN").addClass("countdown");
            $("#sleepMode SPAN").text(ROE.Player.SleepModeCountdown);
            sleepAvailable = false;
        }

        if (ROE.Player.sleepModeOn > 0 && $("#sleepMode SPAN").text() === "") {
            $("#sleepMode").removeClass("deactive").css("opacity", 1).show();
            $("#sleepMode SPAN").addClass("countdown");
            $("#sleepMode SPAN").attr("data-finisheson", ROE.Player.sleepModeOn);
            sleepAvailable = false;
        }

        if (sleepAvailable) {
            $("#sleepMode").removeClass("deactive").css("opacity", 1).show();
        }
    }
}

ROE.Frame.setCurrentlySelectedVillage = function setCurrentlySelectedVillage(id, x, y) {
    ROE.CSV = { id: id, x: x, y: y}; // we set this each time on purpose so that the first load works 

    if (ROE.SVID != id) {
        ROE.SVID = id
        $.cookie("svid", ROE.SVID);
        BDA.Broadcast.publish("CurrentlySelectedVillageChanged");
    }
}

ROE.Frame._fireActivityEvent = function _fireActivityEvent(ui) {
    //
    // fire "activity" Mixpanel event, once every 24 hours
    //
    var now = new Date();
    var mixPanelLastActivityTimeStamp;

    if (localStorage.getItem("MixPanelLastActivityTimeStamp") == null) {
        localStorage.setItem("MixPanelLastActivityTimeStamp", now)
    } else {
        mixPanelLastActivityTimeStamp = new Date(localStorage.getItem("MixPanelLastActivityTimeStamp"));
        if (now - mixPanelLastActivityTimeStamp > 86400000) {
            if (ROE.collectAnalyticsOnThisRealm) {
                mixpanel.track('activity', { 'RealmID': ROE.realmID, 'UI' : ui });
                BDA.Console.verbose('MixPanel', "fire:activity");
                localStorage.setItem("MixPanelLastActivityTimeStamp", now)
            }
        }
    }
}


ROE.Frame._refreshDueToNewViersion_PostponedTimeStamp = null;
ROE.Frame._checkIfReloadNeededDueToUpdate = function _checkIfReloadNeededDueToUpdate() {
    //
    // check if need to reload due to new update
    //
    if ((localStorage.getItem("CurrentBuildID") == null || isNaN(parseFloat(localStorage.getItem("CurrentBuildID"))))) {
        localStorage.setItem("CurrentBuildID", ROE.CurrentBuildID);
    }
    else if (parseFloat(localStorage.getItem("CurrentBuildID")) != parseFloat(ROE.CurrentBuildID)) {
        //
        // if person clicked later, then do not show it for 60 seconds
        if (ROE.Frame._refreshDueToNewViersion_PostponedTimeStamp != null) {
            now = new Date();
            diff = now - ROE.Frame._refreshDueToNewViersion_PostponedTimeStamp;
            if (diff / 1000 < 60) {
                return;
            }
        }

        var msg = BDA.Dict.NewVersionRefresh_msg;
        if (ROE.CurrentBuildIDWhatsNew != "") {
            msg = msg + BDA.Dict.NewVersionRefresh_msg2 + ROE.CurrentBuildIDWhatsNew;
        }

        ROE.Frame.Confirm(
            msg,
            BDA.Dict.NewVersionRefresh_refresh,
            BDA.Dict.NewVersionRefresh_later,
            "",            
            function _refreshDueToNewVersion_GO() {
                ROE.Frame.reloadTheWindow();
                localStorage.setItem("CurrentBuildID", ROE.CurrentBuildID);
            },
            undefined,
            function _refreshDueToNewVersion_Postpone() {
                ROE.Frame._refreshDueToNewViersion_PostponedTimeStamp = new Date();
            },
            true);
    }
}


ROE.Frame.playMusic = function playMusic(play) {
    ROE.Device.playMusic(play);
};

ROE.Frame.sfx = function sfx(on) {
    ROE.Device.sfx(on);
};

ROE.Frame.setupSoundOptions = function setupSoundOptions() {
    // this looks weird but it is correct -- avoiding an Eval() call
    ROE.Device.setMusicPlaying(ROE.Frame.setMusicPlaying, 'ROE.Frame.setMusicPlaying');
    ROE.Device.setSFXOn(ROE.Frame.setSFXOn, 'ROE.Frame.setSFXOn');
};

ROE.Frame.setMusicPlaying = function setMusicPlaying(playing) {
    if (typeof (playing) == "string") { //this ensures value coming from localStorage turns bool
        if (playing == "true") { playing = true; }
        else { playing = false; }
    }
    if ($(".soundContainer .setMusic").length > 0) {
        if (playing) {
            $(".soundContainer .setMusic").css("background-image", "URL('" + ROE.Frame.CONSTS.musicON + "')").addClass("on");
        }
        else {
            $(".soundContainer .setMusic").css("background-image", "URL('" + ROE.Frame.CONSTS.musicOFF + "')").removeClass("on");
        }
    }
};

ROE.Frame.setSFXOn = function setSFXOn(sfxOn) {
    if (typeof (sfxOn) == "string") { //this ensures value coming from localStorage turns bool
        if (sfxOn == "true") { sfxOn = true; }
        else { sfxOn = false; }
    }
    if ($(".soundContainer .setSound").length > 0) {
        if (sfxOn) {
            $(".soundContainer .setSound").css("background-image", "URL('" + ROE.Frame.CONSTS.soundON + "')").addClass("on");
        }
        else {
            $(".soundContainer .setSound").css("background-image", "URL('" + ROE.Frame.CONSTS.soundOFF + "')").removeClass("on");
        }
    }
};


ROE.Frame.Confirm = function confirm(confirmtext, yestext, notext, bgcolor, yes_callback, yes_param, no_callback, preventDoublePopup) {
    /// <summary></summary>
    /// <param name="confirmtext" type="Object"></param>
    /// <param name="yestext" type="Object"></param>
    /// <param name="notext" type="Object">you may enter '' if you do not want the NO button to be dispalyed</param>
    /// <param name="bgcolor" type="Object"></param>
    /// <param name="yes_callback" type="function"></param>
    /// <param name="yes_param" type="Object"></param>
    /// <param name="no_callback" type="function">Optional call back for when no/second button is pressed</param>
    /// <param name="preventDoublePopup" type=Boolean>set to true if you want to make sure that only one popup is shown. if confirm popup is showing, and this is set to true, the popup will not be shown and you call will simply abort</param>

    if (preventDoublePopup === true && $(".confirmbox").length == 1) { //box already open    
        return;
    }

    var popupbox = "<DIV class=confirmbox >" + confirmtext;
    popupbox += "<DIV><SPAN class='confirmbutton sfx2 customButtomBG' data-confirnbutton='1' >" + yestext + "</SPAN>";
    if (notext) {
        popupbox += "<SPAN class='confirmbutton sfx2 customButtomBG' data-confirnbutton='2' >" + notext + "</SPAN></DIV></DIV>";
    }

    ROE.Frame.popupInfo("", "280px", "center", bgcolor, popupbox, true);

    $(".confirmbutton").click(function () {
        //close this popup box
        ROE.Frame.popupInfoClose($(this));
        var n = $(this).attr("data-confirnbutton");

        switch (n) {
            case "1":
                yes_callback(yes_param);
                break;
            case "2":
                if (typeof (no_callback) == "function") {
                    no_callback();
                }
                break;
            default:
                //none
        }

        //if ($(this).attr("data-confirnbutton")==) { callback(param); }

    });

};



ROE.Frame.smartLoadInit = function smartLoadInit(settings) {
    ///<summary>
    ///
    ///settings.containerSelector -- DOM css selector for the scrolling container
    ///settings.itemSelector -- DOM css selector for the items in the list
    ///settings.initialSize -- number of items shown initially *optional
    ///settings.startIndex -- where in the list to begin *optional
    ///settings.loadSize -- how many to load at a time *optional
    ///settings.itemsMadeVisible -- *optional; object with following two properties that if specified, both must be specified: 
    ///settings.itemsMadeVisible.keyAttribute -- the key attribute used to identify the actual item represented with list item. 
    ///  IE, if in village list, it would be the villageID; ie, represents the village that is related to the row in the village list
    ///settings.itemsMadeVisible.callback -- the call back function to call when items are made visible
    ///
    ///</summary>

    var itemsShowInitially;
    if (!settings.containerSelector) {
        alert('smartLoad failed - no container');
        return;
    }

    if (!settings.itemSelector) {
        alert('smartLoad failed - no itemSelector');
        return;
    }

    //setup defaults
    settings.initialSize = settings.initialSize || 20;
    settings.startIndex = settings.startIndex || 0;
    settings.loadSize = settings.loadSize || 10;

    var container = $(settings.containerSelector);
    var items = $(settings.itemSelector, container).hide();

    itemsShowInitially = items.slice(settings.startIndex, settings.startIndex + settings.initialSize).show();

    container.data('curIndex', settings.startIndex);
    container.data('loadSize', settings.loadSize);
    container.data('lastIndex', settings.startIndex + settings.initialSize);

    handle_itemsMadeVisible(itemsShowInitially);

    var _throttleTimer;

    // PRO TIP: This event will stack if you keep calling smart load on the same container.
    // If the container changes often, make sure you unbind scroll before calling smart load
    // init again on the same container to avoid this.
    container.scroll(function () {               
        clearTimeout(_throttleTimer);

        var c = $(this);
        if (c.data('skip')) {
            c.data('skip', false);
            return;
        }

        var h = c.height();
        var sTop = c.scrollTop();
        var sHeight = this.scrollHeight;

        _throttleTimer = setTimeout(function smartLoadTick() {

            //If scrolled near bottom load more downwards
            if ((sHeight - sTop) - h < 90) {               
                _loadMoreDown();

            //else if scrolled all the way top load more upwards
            }else if (sTop == 0) {
                _loadMoreUp();
            }

            function _loadMoreDown() {                
                var items = $(settings.itemSelector, container);

                //Set new range
                var newStartShownIndex = container.data('curIndex') + settings.loadSize;
                var newLastShownIndex = container.data('lastIndex') + settings.loadSize;

                //if new range is out of bottom bounds, reached bottom of list
                if (!items[newLastShownIndex]) {
                    newLastShownIndex = items.last().index() + 1;
                    newStartShownIndex = newLastShownIndex - settings.initialSize;
                    newStartShownIndex < 0 ? newStartShownIndex = 0 : newStartShownIndex;
                }

                //hide cut and show added
                var itemsCut = items.slice(container.data('curIndex'), newStartShownIndex).hide();
                var itemsAdded = items.slice(container.data('lastIndex'), newLastShownIndex).show();

                //save the range to container data
                container.data('curIndex', newStartShownIndex);
                container.data('lastIndex', newLastShownIndex);

                //adjusts scroll for seamlessness
                var newscroll = sTop - items.first().height() * itemsAdded.length;
                if (newscroll < 10) { newscroll = 10; }

                //skip the next scroll event, and call a scroll
                container.data('skip', true);
                c.scrollTop(newscroll);

                handle_itemsMadeVisible(itemsAdded);
            }

            function _loadMoreUp() {               
                var items = $(settings.itemSelector, container);

                //Set new range
                var newStartShownIndex = container.data('curIndex') - settings.loadSize;
                var newLastShownIndex = container.data('lastIndex') - settings.loadSize;

                //if new range is out of bottom bounds
                if (newStartShownIndex < 0) {
                    newStartShownIndex = 0;
                    newLastShownIndex = settings.initialSize;
                }

                //hide cut and show added
                var itemsCut = items.slice(newLastShownIndex,container.data('lastIndex')).hide();
                var itemsAdded = items.slice(newStartShownIndex, container.data('curIndex')).show();

                //save the range to container data
                container.data('curIndex', newStartShownIndex);
                container.data('lastIndex', newLastShownIndex);

                //adjusts scroll for seamlessness
                var newscroll = sTop + items.first().height() * itemsAdded.length;
                if (newscroll >= h) { newscroll = h - 10; }

                //skip the next scroll event, and call a scroll
                container.data('skip', true);
                c.scrollTop(newscroll);

                handle_itemsMadeVisible(itemsAdded);
            }
    
        //throttle timer
        }, 200);
        
    });

    function handle_itemsMadeVisible(itemsShown) {
        // if client wants a call back when items are made visible
        if (settings.itemsMadeVisible) {
            var itemsMadeVisible_IDs = [];
            itemsShown.each(function (i) {
                itemsMadeVisible_IDs[i] = $(this).attr(settings.itemsMadeVisible.keyAttribute);
            });
            // why the timeout? so that control does not get held up when the call back is processing; we want the scroll code to end, and let the client do it stuff 
            setTimeout(function settings_itemsMadeVisible_callback() { settings.itemsMadeVisible.callback(itemsMadeVisible_IDs) }, 1);
        }
    }
}




ROE.Frame.questRewardAccepted_ReloadUI = function questRewardAccepted_ReloadUI() {
    // this gets called to refresh the coins and servants after a quest's reward is accepted. 
    //  well actually, it gets called slighly before the reward is accepted hence this delay.
    //  its silly but will work till quest system is rewritten 
    setTimeout(
        function questRewardAccepted_ReloadUI_delayload()
        {
            ROE.Villages.ExtendedInfo_loadLatest(ROE.SVID);
            ROE.Frame.reloadFrame();
        }
        , 1000); 
}



ROE.Frame.iconNeedsAttention = function iconNeedsAttention(element, state) {
    if (ROE.isMobile) {
        ROE.Frame.effect_pulseConstant(element, state);
    } else {
        ROE.Frame.effect_bounceLoud(element, state);
    }
}

//element is the button or icon to make bounce, and state is on/off state
ROE.Frame.effect_bounceLoud = function effect_bounceLoud(element, state) {

    var effectClass = 'effect-bounce-loud';

    //no bounce in RX
    if(ROE.rt == "X"){
        element.removeClass(effectClass);
        return;
    }

    if (state && ROE.Player.numberOfVillages < 2) {
        //the timeout is to de-synch the icon bounce css-animations
        setTimeout(function () { element.addClass(effectClass) }, Math.random() * 2000);
    } else {
        element.removeClass(effectClass);
    }

}

//constant pulse
ROE.Frame.effect_pulseConstant = function effect_pulseConstant(element, state) {

    var effectClass = 'effect-pulse-constant';

    //no bounce in RX
    if(ROE.rt == "X"){
        element.removeClass(effectClass);
        return;
    }

    if (state && ROE.Player.numberOfVillages < 2) {
        element.addClass(effectClass);
    } else {
        element.removeClass(effectClass);
    }

}