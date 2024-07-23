/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

(function (obj, $, undefined) {

    var _CACHE = {};
    var _sel = -1;
    var itemImg = new Array(
        "",
        "https://static.realmofempires.com/images/icons/sackOfSilver_m.png",
        "https://static.realmofempires.com/images/units/Infantry_m.png",
        "https://static.realmofempires.com/images/units/Cavalry_M.png",
        "https://static.realmofempires.com/images/units/Knight_m.png",
        "https://static.realmofempires.com/images/units/Spy_m.png",
        "https://static.realmofempires.com/images/icons/M_PF_NP.png",
        "https://static.realmofempires.com/images/icons/M_PF_Silver.png",
        "https://static.realmofempires.com/images/icons/servantCarry_m.png",
        "https://static.realmofempires.com/images/units/ram_m.png",
        "https://static.realmofempires.com/images/units/Treb_m.png"
        );

    var _showPopup = function () {
        _sel = -1;
        _CACHE = {};
        var popupContent;

        if (ROE.isD2) {
            //since daily reward is used infrequently, we use a disposable dialog method
            popupContent = $("<div id='popup_DailyReward' class='popupDialogs'/>");
            popupContent.appendTo('document');
            popupContent.dialog({
                autoOpen: true,
                modal: true,
                title: "King's Fortune",
                position: { at: "center center", of: window },
                width: 360,
                height: 500,
                create: function () {
                    ROE.Frame.addDialogStylingElements($(this));
                },
                close: function () {
                    $(this).dialog('destroy');
                    if (_sel != -1) { _rewardHideRefresh(); }
                }
            });
        } else {
            if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'DailyReward .popupBody').length) { return; }
            popupModalOverlay('DailyReward', '', 0, 0);
            popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'DailyReward .popupBody');
        }

        // this template is not preloaded on purpose since its used only once
        var temp = BDA.Templates.getRawJQObj('DailyReward', ROE.realmID);
        var rewardList = "";
        var rewardBox = 9;
        var TR = 0;
        for (var i = 0; i < rewardBox; i++) {

            if (TR == 0) { rewardList += '<TR>'; }
            rewardList += '<td><section class="rewardListItem"><div class="rewardType" data-rewardtype=' + (i + 1) + ' >';
            rewardList += '<div class="figure cardfront"><div class="rewardTypeImg sfx2 ButtonTouch" ></div>';
            rewardList += '<span class="rewardTypeDesc"></span></div>';
            rewardList += '<div class="figure cardback"><div class="rewardTypeImg" ></div>';
            rewardList += '<span class="rewardTypeDesc"></span></div>';
            rewardList += '</section></td>';

            TR++;
            if (TR == 3) { rewardList += '</TR>'; TR = 0; }
        }

        popupContent.append(temp);

        if (typeof ROE.Player != "undefined" && ROE.Player.dailyRewardLevel) {
            var lvl = Math.min(ROE.Player.dailyRewardLevel, 14);
            lvl
            $("#RewardSelection .rewardDays").html("Your King's fortune is level: " + lvl);
            $("#RewardSelection .fortuneBadge").css({ 'background-position': '0px -' + ((lvl - 1) * 60) + 'px' });
        } else {
            $("#RewardSelection .rewardDays").hide();
            $("#RewardSelection .fortuneBadge").hide();
        }

        $("#RewardSelection .rewardTypes").append(rewardList);

        //
        // bind events
        //        
        $('#popup_DailyReward .titleClose').click(_close);
        $('#RewardSelection .rewardType').click(_dailyRewardClick);

        if (ROE.Player.dailyRewardAvail === false) {
            $('.rewardDesc', popupContent).hide();
            $('.rewardDays', popupContent).css({'top':'20px'});
            $('table.rewardTypes', popupContent).hide();
            $('.notAvail', popupContent)
                .html('<p style="text-align:center;">Please wait: <span class="countdown"  data-finisheson="' + ROE.Player.dailyRewardNext + '"></span> for your next King\'s Fortune!</p>').show();
            //data-refreshcall="ROE.DailyReward.showPopup();"
        }
    }

    var _dailyRewardClick = function (e) {
        
        if (_sel == -1) {
            ROE.Frame.busy('Granting your King\'s fortune!', 10000, $('#popup_DailyReward.popupDialogs'));
            $(e.currentTarget).addClass('selected');
            _sel = $(e.currentTarget).attr('data-rewardtype');
            ROE.Api.call_getMyDailyReward(_sel, _sync_onGetMyDailyRewardSuccess);
        }
    }

    var _sync_onGetMyDailyRewardSuccess = function (data) {
        ROE.Frame.free($('#popup_DailyReward.popupDialogs'));
        _CACHE.data = {};
        _CACHE.data = data;
        _sel = data.Sel;
        
        $('#RewardSelection .rewardListItem').css("cursor","default");
        
        var rewardTypes = $('#RewardSelection .rewardListItem .cardback');
        var PlayerGot = "";
        for (var i = 0; i < rewardTypes.length; i++) {

            var img = data.Rewards[i].ImageUrl;
            var rtype = data.Rewards[i].Type;
            var desc = data.Rewards[i].Desc;
            var title = data.Rewards[i].Title;
            var rewardType = $('.rewardType[data-rewardtype="' + (i + 1) + '"]'); //$(rewardTypes[i]);

            if ((_sel - 1) != i) {
                rewardType.addClass("greyscale");
            }
            else {
                if (title == "Cornucopia") { PlayerGot = data.PlayerGot; }
                else { PlayerGot = desc + " " + title; }

                if (ROE.isMobile || ROE.isD2) {
                    var bazaarIconElement = $('.actionButton.bz').length ? $('.actionButton.bz') : $('#linkItem'); //M or D2 icons
                    ROE.Frame.spawnFlyIcon({
                        parentElement: null,
                        startPos: { top: rewardType.offset().top + rewardType.height() / 2, left: rewardType.offset().left + rewardType.width() / 2 },
                        iconUrl: itemImg[rtype],
                        endPos: { top: bazaarIconElement.offset().top, left: bazaarIconElement.offset().left }
                    });
                }

            }

            rewardType.find('.rewardTypeImg').css('background-image', "url('" + itemImg[rtype] + "')");
            rewardType.find('span').text(desc + " " + title);
            rewardType.onclick = undefined;
        }
               
        $('#RewardSelection .rewardDesc').text(_phrases(1));        
        $('#RewardSelection .rewardDays').html(_phrases(2));
        $('#RewardSelection .rewardDays SPAN').html(PlayerGot);

        
        if (ROE.isMobile) {
            ROE.UI.Sounds.clickSpell();
            ROE.Frame.reloadFrame();
        }

        $("#RewardSelection .rewardType").addClass("flipped");
        if (GetIEVersion() > 0) {
            $("#RewardSelection .rewardTypeDesc").addClass("ieFixDescFlip");
        }

        $('#RewardSelection .rewardOK').css("display", "inline-block").click(_showRewardNextDay);
        $('#RewardSelection .inventory').css("display", "inline-block").click(function () { _close(); ROE.Frame.popupGifts(); });
    }

    var _showRewardNextDay = function () {

        $('#RewardSelection .rewardDesc').text(_phrases(5));
        $('#RewardSelection .rewardDays').text(_phrases(4));

        var rewardTypes = $('#RewardSelection .rewardListItem .cardfront');

        for (var i = 0; i < rewardTypes.length; i++) {

            var desc = _CACHE.data.NextDayRewards[i].Desc;
            var title = _CACHE.data.NextDayRewards[i].Title;
            var type = _CACHE.data.NextDayRewards[i].Type;
            var rewardType = $(rewardTypes[i]);

            rewardType.find('.rewardTypeImg').css('background-image', 'url(' + itemImg[type] + ')').removeClass("sfx2 ButtonTouch");
            rewardType.find('span').text(desc + " " + title);
            rewardType.unbind('click');
            rewardType.removeClass('selected');
        }

        $("#RewardSelection .rewardType").removeClass("flipped");
        $('#RewardSelection .rewardOK').click(_close);
    }

    var _close = function (data) {
        if (ROE.isD2) {
            $("#popup_DailyReward").dialog('close');
        } else if (ROE.isMobile) {
            closeMe();
            if (_sel != -1) {
                _rewardHideRefresh();
            }
        } else { //D1 compatibility
            closeMe();
            if (_sel != -1) {
                $('.dailyReward').remove();
            }          
        }

    }

    function _rewardHideRefresh() {
        ROE.Player.dailyRewardAvail = false;
        var rewardIcon = $(".dailyReward").hide();
        $('.counter', rewardIcon).removeClass("countdown").attr("data-finisheson", '');
        ROE.Player.refresh(ROE.Frame.handleDailyReward);
    }

    function _nextRewardReady() {
        ROE.Player.dailyRewardAvail = true;
        var rewardIcon = $(".dailyReward").css('background-image', "url('https://static.realmofempires.com/images/icons/dailyBonus.png')").removeClass("grayout").show();
        ROE.Frame.iconNeedsAttention(rewardIcon, true);
        $('.counter', rewardIcon).removeClass("countdown").attr("data-finisheson", '').hide();
    }

    function _nextRewardCounting() {
        var rewardIcon = $(".dailyReward").css('background-image', "url('https://static.realmofempires.com/images/icons/M_PF_NP.png')").addClass("grayout").show();
        ROE.Frame.iconNeedsAttention(rewardIcon, false);
        $('.counter', rewardIcon).attr('data-refreshcall', 'ROE.DailyReward.nextRewardReady();')
            .attr("data-finisheson", ROE.Player.dailyRewardNext).addClass("countdown").show();
    }

    function _phrases(id) {
        return $('#RewardSelection .phrases [ph=' + id + ']').html();
    }

    function GetIEVersion() {
        var sAgent = window.navigator.userAgent;
        var Idx = sAgent.indexOf("MSIE");

        // If IE, return version number.
        if (Idx > 0)
            return parseInt(sAgent.substring(Idx + 5, sAgent.indexOf(".", Idx)));

            // If IE 11 then look for Updated user agent string.
        else if (!!navigator.userAgent.match(/Trident\/7\./))
            return 11;

        else
            return 0; //It is not IE
    }

    obj.showPopup = _showPopup;
    obj.nextRewardReady = _nextRewardReady;
    obj.nextRewardCounting = _nextRewardCounting;
    
}(window.ROE.DailyReward = window.ROE.DailyReward || {}, jQuery));