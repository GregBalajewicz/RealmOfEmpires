/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="bda-val.js" />


(function (ROE) {
}(window.ROE = window.ROE || {}));


(function (ROE) {
}(window.ROE.UI = window.ROE.UI || {}));

(function (obj, $, undefined) {
    var _suppressedSFX = [];
    var _suppressedSFXUntil = []; //{suspended:boolean, soundedonce:boolean}
    var _suppressAllSounds = false;
    BDA.Console.setCategoryDefaultView('sfx', false); //this assumes BDA.Console is defined BEFORE this file

    

    var _playSFX = function (id) {
        if (_suppressAllSounds) {
            return;
        }
        if (_suppressedSFXUntil[id] !== undefined && _suppressedSFXUntil[id].suspended === true) {
            if (_suppressedSFXUntil[id].soundedonce === true) {
                return; // supress the sound
            }
            _suppressedSFXUntil[id].soundedonce = true; // we ignore the fact that the sound may not actually play because is is suspended once via _suppressedSFX
        }
        ROE.Device.playSFX(id, _suppressedSFX[id]);
        BDA.Console.verbose('sfx', 'play sound#' + id);
        _suppressedSFX[id] = false;
    };

    var _suppressNextSFX = function (id) {
        BDA.Console.verbose('sfx', 'suppress sound#' + id);
        _suppressedSFX[id] = true;
    }

    var _supressAllButFirstUntil = function (id) {
        BDA.Console.verbose('sfx', 'suppress all but first, sound#' + id);
        _suppressedSFXUntil[id] = { suspended: true, soundedonce: false};
    }

    var _supressAllButFirstUntil_resume = function (id) {
        BDA.Console.verbose('sfx', '(resume) suppress all but first, sound#' + id);
        _suppressedSFXUntil[id] = { suspended: false };
    }

    var _supressAllUntil = function () {
        ///<summary>suppresses all sounds untill resume is called </summary>
        BDA.Console.verbose('sfx', 'suppress all sounds');
        _suppressAllSounds = true;
    }

    var _supressAllUntil_resume = function (id) {
        ///<summary>reverses the suppresses all sounds </summary>
        BDA.Console.verbose('sfx', '(resume) suppress all sounds');
        _suppressAllSounds = false;
    }


    var _playHtml5SFX = function (id) {
       if (!localStorage.isSfxOn || localStorage.getItem('isSfxOn') == "false") {
            return;
         }
        var snd = obj.bufferedSFX[id];
        if (snd) {
           
            snd.play();
        }

    }

    var _playHTML5Music = function (id) {
        // Kill this if Amazon Kindle
        if (ROE.isDevice == ROE.Device.CONSTS.Amazon) {
            return;
        }
        if (!localStorage.isMusicPlaying || localStorage.getItem('isMusicPlaying') == "false") {            
            return;
        }
        if (typeof (id) == "undefined") {
            id = 1;
        }
       
        obj.currentMusic = obj.bufferedMusic[id];
        obj.currentMusic.loop = true;        
        obj.currentMusic.play();
    }

    var _stopHTML5Music = function () {        
        obj.currentMusic.pause();
        obj.currentMusic.currentTime = 0;
    }

    obj.bufferedSFX = {}; ///will hold buffered SFX for HTML5 method
    obj.bufferedMusic = {}; ///will hold buffered Muisc for HTML5 method
    obj.playHtml5SFX = _playHtml5SFX; ///HTML5 method of playing SFX
    obj.playHTML5Music = _playHTML5Music; ///HTML5 method of playing Muisc
    obj.stopHTML5Music = _stopHTML5Music; ///stop the music
    obj.playSFX = _playSFX;
    obj.click = function () { _playSFX(ROE.UI.Sounds.CONSTS.ActionClick) };
    obj.suppressNext_click = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.ActionClick) };
    obj.clickDefeat = function () { _playSFX(ROE.UI.Sounds.CONSTS.EventDefeat) };
    obj.suppressNext_clickDefeat = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.EventDefeat) };
    obj.clickLevelUp = function () { _playSFX(ROE.UI.Sounds.CONSTS.EventLevelUp) };
    obj.suppressNext_clickLevelUp = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.EventLevelUp) };
    obj.clickReward = function () { _playSFX(ROE.UI.Sounds.CONSTS.EventReward) };
    obj.suppressNext_clickReward = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.EventReward) };
    obj.clickSpell = function () { _playSFX(ROE.UI.Sounds.CONSTS.EventSpell) };
    obj.suppressNext_clickSpell = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.EventSpell) };
    obj.clickVictory = function () { _playSFX(ROE.UI.Sounds.CONSTS.EventVictory) };
    obj.suppressNext_clickVictory = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.Victory) };
    obj.clickMenuExit = function () { _playSFX(ROE.UI.Sounds.CONSTS.MenuExit) };
    obj.suppressNext_clickMenuExit = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.MenuExit) };
    obj.clickBuildingEnter = function () { _playSFX(ROE.UI.Sounds.CONSTS.BuildingEnter) };
    obj.suppressNext_clickBuildingEnter = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.BuildingEnter) };
    obj.clickActionSwipe = function () { _playSFX(ROE.UI.Sounds.CONSTS.ActionSwipe) };
    obj.suppressNext_clickActionSwipe = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.ActionSwipe) };
    obj.clickActionScroll = function () { _playSFX(ROE.UI.Sounds.CONSTS.ActionScroll) };
    obj.suppressNext_clickActionScroll = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.ActionScroll) };
    obj.clickActionOpen = function () { _playSFX(ROE.UI.Sounds.CONSTS.ActionOpen) };
    obj.suppressNext_clickActionOpen = function () { _suppressNextSFX(ROE.UI.Sounds.CONSTS.ActionOpen) };

    obj.supressAllButFirstUntil = _supressAllButFirstUntil;
    obj.supressAllButFirstUntil_resume = _supressAllButFirstUntil_resume;

    obj.supressAllUntil = _supressAllUntil;
    obj.supressAllUntil_resume = _supressAllUntil_resume;

    obj.CONSTS = {};
    // sfx_ui_action_click - clicking anywhere that doesn't have another sound
    // class: sfx2
    obj.CONSTS.ActionClick = 1;
    // sfx_event_defeat - When report is opened with lost village
    // class: sfxDefeat
    obj.CONSTS.EventDefeat = 2;
    // sfx_event_levelup - When accepting new title, possibly when going up global rank?
    // class: sfxLevelUp
    obj.CONSTS.EventLevelUp = 3;
    // sfx_event_reward - Claiming a quest reward
    // class: sfxReward
    obj.CONSTS.EventReward = 4;
    // sfx_event_spell - Activating any spell
    // class: sfxSpell
    obj.CONSTS.EventSpell = 5;
    // sfx_event_victory - Opening report that captures village
    // class: sfxVictory
    obj.CONSTS.EventVictory = 6;
    // sfx_menu_exit - X or any cancel button
    // class: sfxMenuExit
    obj.CONSTS.MenuExit = 7;
    // sfx_building_enter - VoV buildings
    // class: sfxBuildingEnter
    obj.CONSTS.BuildingEnter = 8;
    // sfx_action_swipe
    // class: sfxSwipe
    obj.CONSTS.ActionSwipe = 9;
    // sfx_action_scroll
    // class: sfxScroll
    obj.CONSTS.ActionScroll = 10;
    // sfx_action_open
    // class: sfxOpen
    obj.CONSTS.ActionOpen = 11;

}(window.ROE.UI.Sounds = window.ROE.UI.Sounds || {}, jQuery));
