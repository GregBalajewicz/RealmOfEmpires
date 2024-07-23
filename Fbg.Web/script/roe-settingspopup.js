(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {

    var CONST = {
        imgON: "https://static.realmofempires.com/images/misc/yesGreen.PNG",
        imgOFF: "https://static.realmofempires.com/images/icons/M_IcoCancel.png"
    }    

    var _container;
    var template;
    var temp_remember_indexOfDefaultTab;

    obj.init = function (container, indexOfDefaultTab) {
          
        ROE.Frame.busy('Loading...',5000,$('#settingsDialog'));
        _container = container;
        template = $(BDA.Templates.getRawJQObj("SettingsPopup", ROE.realmID));
        if (indexOfDefaultTab !== 1 && indexOfDefaultTab !== 0) { indexOfDefaultTab = 1; }
        temp_remember_indexOfDefaultTab = indexOfDefaultTab;
        //populate Alerts page from API
        ROE.Api.call("notifsetting_get", {}, _alertsPopulate);


    }

    function _alertsPopulate(response) {

        var masterStateID;
        var notify = {};
        var content = "";

        var rowTemp = template.find(".templ_settingpopup .notifyItem");
        //rowTemp.remove();

        template.find(".templ_settingpopup .notificationMasterSwitch SPAN").html(ROE.realmID);

        if (response.notificationsActiveOnRealm) {
            template.find(".templ_settingpopup .notificationMasterSwitch IMG").attr("src", CONST.imgON);
            var masterStateID = 1; //CR define all varables at top, and only once. Start with small letter 
        }
        else {
            template.find(".templ_settingpopup .notificationMasterSwitch IMG").attr("src", CONST.imgOFF);
            var masterStateID = 0;
        }
                

        var notifyer = response.NotfifcationSettings;

        for (i = 0; i < notifyer.length; i++) {

            notify.activeStateID = notifyer[i].activeStateID;
            notify.desc = notifyer[i].desc;
            notify.name = notifyer[i].name;
            notify.notificationID = notifyer[i].notificationID;
            notify.soundSettingID = notifyer[i].soundSettingID;
            notify.muteSettingID = notifyer[i].MuteAtNightSettingID;

            if (notify.activeStateID == 1) {
                notify.IMG0 = CONST.imgON;
            }
            else {
                notify.IMG0 = CONST.imgOFF;
            }

            if (notify.muteSettingID == 1) {
                notify.IMG1 = CONST.imgON;
            }
            else {
                notify.IMG1 = CONST.imgOFF;
            }

            if (notify.soundSettingID == 1) {
                notify.IMG2 = CONST.imgON;
            }
            else {
                notify.IMG2 = CONST.imgOFF;
            }

            notify.cssClass = "notifyactive";
            notify.cssMainClass = "";

            if (notify.activeStateID == 0) { notify.cssClass = "NotifyBlockOff"; notify.cssMainClass = "NotifyBlockOff"; } //CR do the style in standards - everything on its own line

            content += BDA.Templates.populate(rowTemp.prop('outerHTML'), notify);
            
        }
       
        template.find(".templ_settingpopup .notifyList").html(content);

        template.find(".templ_settingpopup .notificationMasterSwitch").attr("data-notifyTypeStatus", masterStateID);



        if (masterStateID == 0) { template.find(".templ_settingpopup .notifyList").addClass("NotifyBlockOff"); }
               
        template.find(".templ_settingpopup").css("visibility", "visible");        

        _container.append(template);
        _notifyUpdate(response);
        
        if (ROE.playersNumVillages == 1 && !ROE.Player.isSteward) {
            $(".templ_settingpopup .soundContainer .restart").show().click(function () {
                ROE.UI.Sounds.click();
                return ROE.Restart.showPopup();
            });
        } else {
            $(".templ_settingpopup .soundContainer .restart").hide();
        }

        $(".templ_settingpopup .notificationMasterSwitch").click(_notifyInfo);
        $(".templ_settingpopup .notifyButton").click(_notifyInfo);
        $(".templ_settingpopup .oneLine").click(_selectedTab);
        $(".templ_settingpopup .soundContainer .setMusic").click(_setMusic);
        $(".templ_settingpopup .soundContainer .setSound").click(_setSound);
        $(".templ_settingpopup .setUIDisplayMode").click(_clickUIDisplayMode);
        $(".templ_settingpopup .toggleMapScrollZoom").click(_clickToggleMapScrollZoom);
        $(".templ_settingpopup .resetLT").click(_resetLoginType);
        

        ROE.Frame.free($('#settingsDialog'));

        //set current music/sfx settings
        ROE.Frame.setupSoundOptions();

        //sets UI Mode
        _populateUIDisplayMode(ROE.LocalServerStorage.get('UIDisplayMode') || 'Standard');

        _populateMapScrollZoom(ROE.LocalServerStorage.get('MapScrollZoom') || 'ON');

        // default tab
        $(".templ_settingpopup .oneLine")[temp_remember_indexOfDefaultTab].click();
    }


    function _notifyInfo(event) {
        
        var clickable = $(event.currentTarget).hasClass("notifyactive");
        var masterSwitch = $(".templ_settingpopup .notificationMasterSwitch").attr("data-notifyTypeStatus");
        
        if (clickable) {

            ROE.UI.Sounds.click();

            var notifyID = $(this).attr("data-notifyID");
            var notifyList = new Array;
            var notifyTypeStatus;
            var MasterStatus = true;
            var infoText = 1;

            //notification master switch change
            if (notifyID == 0) {

                $(".templ_settingpopup .notify_save").addClass("savingOut");
                
                notifyTypeStatus = parseInt($(this).attr("data-notifyTypeStatus"),10);

                if (notifyTypeStatus == 1) {
                    infoText = 2;
                    MasterStatus = false;
                } 

                ROE.Api.call("notifsetting_setrealmwideactive", { onOrOff: MasterStatus }, _notifyUpdate);

                ROE.Frame.infoBar(_getInfoText(infoText));
            }
            else { //sub notification changes
                
                //double check master switch ON
                if (masterSwitch == 1) {

                    $(".templ_settingpopup .notify_save").addClass("savingOut");

                    var nid = $(this).parent().attr("data-notifyID");
                    var notifyTypeID = $(this).attr("data-notifyTypeID");
                    var notifyTypeStatus = parseInt($(this).attr("data-notifyTypeStatus"),10);
                    var infoText = 3;
                    
                    if (notifyTypeID == "activeStateID") { var infoText = 3; }
                    if (notifyTypeID == "soundSettingID") { var infoText = 5; }
                    if (notifyTypeID == "muteSettingID") { var infoText = 7; }

                    if (notifyTypeStatus == 1) { infoText++; }

                    //status switch (0/1)
                    var notifyTypeStatus = 1 - notifyTypeStatus;

                    notifyList[notifyTypeID] = notifyTypeStatus;

                    $(this).siblings().each(function () {

                        var notifyTypeID = $(this).attr("data-notifyTypeID");
                        var notifyTypeStatus = $(this).attr("data-notifyTypeStatus");
                        notifyList[notifyTypeID] = notifyTypeStatus;

                    });
                    
                    ROE.Api.call("notifsetting_update", { nid: nid, v: notifyList["muteSettingID"], s: notifyList["soundSettingID"], a: notifyList["activeStateID"] }, _notifyUpdate);
                    
                    ROE.Frame.infoBar(_getInfoText(infoText));
                }
            }
        }
    }


    function _notifyUpdate(response) {

        var masterStateID;
        var notifyIMG;
        var notifyer = response.NotfifcationSettings;

        if (response.notificationsActiveOnRealm) {
            masterStateID = 1;
            notifyIMG = CONST.imgON;
            $(".templ_settingpopup .notifyList").removeClass("NotifyBlockOff");
        }
        else {
            masterStateID = 0;
            notifyIMG = CONST.imgOFF;
            $(".templ_settingpopup .notifyList").addClass("NotifyBlockOff");
        }

        $(".templ_settingpopup .notificationMasterSwitch").attr("data-notifyTypeStatus", masterStateID);
        $(".templ_settingpopup .notificationMasterSwitch IMG").attr("src", notifyIMG);
 
        for (i = 0; i < notifyer.length; i++) {

            var notificationID = notifyer[i].notificationID;
            var soundSettingID = notifyer[i].soundSettingID;
            var muteSettingID = notifyer[i].MuteAtNightSettingID;
            var activeStateID = notifyer[i].activeStateID;

            if (activeStateID == 1) { var notifyIMG = CONST.imgON; _notifyStatusChange(notificationID, "activeStateID", notifyIMG, activeStateID, activeStateID); }
            else { var notifyIMG = CONST.imgOFF; _notifyStatusChange(notificationID, "activeStateID", notifyIMG, activeStateID, activeStateID); }

            if (muteSettingID == 1) { var notifyIMG = CONST.imgON; _notifyStatusChange(notificationID, "muteSettingID", notifyIMG, muteSettingID, activeStateID); }
            else { var notifyIMG = CONST.imgOFF; _notifyStatusChange(notificationID, "muteSettingID", notifyIMG, muteSettingID, activeStateID); }

            if (soundSettingID == 1) {
                var notifyIMG = CONST.imgON; _notifyStatusChange(notificationID, "soundSettingID", notifyIMG, soundSettingID, activeStateID);
            }
            else {
                var notifyIMG = CONST.imgOFF; _notifyStatusChange(notificationID, "soundSettingID", notifyIMG, soundSettingID, activeStateID);

                //if NotifSounds set off, then fade out & diactivate "Mute at Night" option
                $("div[data-notifyID='" + notificationID + "'] li[data-notifyTypeID='muteSettingID']").addClass("NotifyBlockOff").removeClass("notifyactive");
            }

        }

        $(".templ_settingpopup .notify_save").removeClass("savingOut");
    }


    function _notifyStatusChange(notificationID, notitype, notifyIMG, activeStateID, mainStateID) {

        var notifyClass = "";

        if (mainStateID == 0 && notitype != "activeStateID") {
            var notifyClass = "notifyactive";
        }

        if (mainStateID == 0) {
            $("div[data-notifyID='" + notificationID + "'] .notifytext").addClass("NotifyBlockOff");
            $("div[data-notifyID='" + notificationID + "'] LI:not(:first)").addClass("NotifyBlockOff");
        }
        else {
            $("div[data-notifyID='" + notificationID + "'] .notifytext").removeClass("NotifyBlockOff");
            $("div[data-notifyID='" + notificationID + "'] LI").removeClass("NotifyBlockOff");
        }

        $("div[data-notifyID='" + notificationID + "'] li[data-notifyTypeID='" + notitype + "'] IMG").attr("src", notifyIMG);
        $("div[data-notifyID='" + notificationID + "'] li[data-notifyTypeID='" + notitype + "']").addClass("notifyactive").removeClass(notifyClass).attr("data-notifyTypeStatus", activeStateID);

    }


    function _getInfoText(r) {
        return $('.templ_settingpopup .phrases [ph=' + r + ']').html();
    }

    //CR "handleSelectedTab"
    // functions => verbs
    // variables => nouns 
    function _selectedTab(event) {

        $(".templ_settingpopup .oneLine").removeClass('selected');
        $(event.currentTarget).addClass('selected');

        var container = $(event.currentTarget).attr("data-container");
        
        $(".templ_settingpopup .tabContent ").removeClass('tabs-in').addClass('tabs-out');
        $(".templ_settingpopup .tabContent." + container).removeClass('tabs-out').addClass('tabs-in');
    }


    function _setMusic() {

        ROE.UI.Sounds.click();

        var musicImg = $('.templ_settingpopup .soundContainer .setMusic');

        //change the music on/off flag
        musicImg.toggleClass('on');
        
        if (musicImg.hasClass('on')) {
            musicImg.css("background-image", "URL('" + ROE.Frame.CONSTS.musicON + "')");
            ROE.Frame.playMusic(true);
            var statusText = _getInfoText(11);
        }
        else {
            musicImg.css("background-image", "URL('" + ROE.Frame.CONSTS.musicOFF + "')");
            ROE.Frame.playMusic(false);
            var statusText = _getInfoText(12);
        }

        ROE.Frame.infoBar(_getInfoText(9) + " " + statusText);
    }


    function _setSound() {

        ROE.UI.Sounds.click();

        var soundImg = $('.templ_settingpopup .soundContainer .setSound');

        soundImg.toggleClass('on');

        if (soundImg.hasClass('on')) {
            soundImg.css("background-image", "URL('" + ROE.Frame.CONSTS.soundON + "')");
            ROE.Frame.sfx(true);
            var statusText = _getInfoText(11);
        }
        else {
            soundImg.css("background-image", "URL('" + ROE.Frame.CONSTS.soundOFF + "')");
            ROE.Frame.sfx(false);
            var statusText = _getInfoText(12);
        }

        ROE.Frame.infoBar(_getInfoText(10) + " " + statusText);
    }
    
    
    ///SET UI MODE FUNCTIONS
    function _clickUIDisplayMode() {
        ROE.UI.Sounds.click();
        var mode = ROE.LocalServerStorage.get('UIDisplayMode') || 'Standard';
        mode = (mode == 'Standard' ? 'Compact' : 'Standard');
        ROE.Frame.busy('Saving...', 5000, $('#settingsDialog'));
        ROE.LocalServerStorage.set('UIDisplayMode', mode, _setUIDisplayModeCallback);
    }

    function _setUIDisplayModeCallback(data) {
        ROE.Frame.free($('#settingsDialog'));
        _populateUIDisplayMode(ROE.LocalServerStorage.get('UIDisplayMode'));
        ROE.QuickRecruit.UIDisplayModeChanged();
        ROE.QuickBuild.UIDisplayModeChanged();
    }
    
    function _populateUIDisplayMode(mode) {       
        var setUIDisplayModeBtn = $('.templ_settingpopup .soundContainer .setUIDisplayMode').attr('data-mode',mode);
        $("SPAN", setUIDisplayModeBtn).html('UI Display: ' + mode);
    }
    ///

    ///SET MAP MOUSEWHEEL SCROLL ZOOM FUNCTIONS
    function _clickToggleMapScrollZoom() {
        ROE.UI.Sounds.click();
        var mode = ROE.LocalServerStorage.get('MapScrollZoom') || 'ON';
        mode = (mode == 'ON' ? 'OFF' : 'ON');
        ROE.Frame.busy('Saving...', 5000, $('#settingsDialog'));
        ROE.LocalServerStorage.set('MapScrollZoom', mode, _setMapScrollZoomCB);
    }
    function _setMapScrollZoomCB(data) {
        ROE.Frame.free($('#settingsDialog'));
        _populateMapScrollZoom(ROE.LocalServerStorage.get('MapScrollZoom'));
    }

    function _populateMapScrollZoom(mode) {
        var setUIDisplayModeBtn = $('.templ_settingpopup .soundContainer .toggleMapScrollZoom').attr('data-mode', mode);
        $("SPAN", setUIDisplayModeBtn).html('Map MouseWheel Zoom: ' + mode);
    }
    ///
    
    function _resetLoginType() {
        ROE.UI.Sounds.click();
        if (confirm('Are you sure? Use this only when directed by support')) {
            if (confirm('DOUBLE CHECKING - Click OK only if instructed by support to do this.')) {
                //localStorage.clear();
                window.location = 'login_resetLoginType.aspx';
            }
        }
    }

} (window.ROE.Settings = window.ROE.Settings || {}) );
