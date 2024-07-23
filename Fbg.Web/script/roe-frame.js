/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4.js" />
/// <reference path="interfaces.js" />
/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />
/// <reference path="ROE_c.js" />
/// <reference path="troops.3.js" />

(function (obj) {
    obj.CONSTS = {};
    obj.CONSTS.popupNameIDPrefix = "popup_";
    obj.CONSTS.popupDefaultX = 1;
    obj.CONSTS.popupDefaultY = 1;
    obj.CONSTS.popupDefaultWidth = 318;
    obj.CONSTS.popupDefaultHeight = 478;
    obj.CONSTS.musicON = "https://static.realmofempires.com/images/icons/M_MoreMusicOn.png";
    obj.CONSTS.musicOFF = "https://static.realmofempires.com/images/icons/M_MoreMusicOff.png";
    obj.CONSTS.soundON = "https://static.realmofempires.com/images/icons/M_MoreSoundOn.png";
    obj.CONSTS.soundOFF = "https://static.realmofempires.com/images/icons/M_MoreSoundOff.png";

    // Tells you the IDs of popups. This can be used to identify what kind of a popu is displayed
    //
    // like for the building popup : <div class="iFrameDiv" id="popup_Building" keep="yes">
    //
    // EXAMPLE: crappy 
    //  if ($('#popup_Building').length > 0) { /*now you know the building popup is opened*/ }
    // USING CONSTS:
    //  if ($(ROE.Frame.CONST.PopupIDs.building).length > 0) { /*now you know the building popup is opened*/ }
    //
    obj.CONSTS.PopupIDs = {}
    obj.CONSTS.PopupIDs.building = "#popup_Building";
    obj.CONSTS.PopupIDs.quickbuild = "#popup_QuickBuild";
    obj.CONSTS.PopupIDs.quickrecruit = "#popup_QuickRecruit";

    function _simplePopopOverlay(iconUrl, popupTitle, htmlContent, specialCssClass, selectorToAppendTo) {
        var popup = $('<div>').addClass('simplePopupOverlay').html(
                '<div class="pContainer">' +
                    '<div class="pHeader">' +
                        '<div class="stripe"></div>' +
                        '<div class="iconBg"></div>' +
                        '<div class="iconFg" style="background-image:url(\'' + iconUrl + '\')"></div>' +
                        '<div class="title">' + popupTitle + '</div>' +
                        '<div class="pClose sfxMenuExit" onclick="' +
                            '$(this).closest(\'.simplePopupOverlay\').remove();' +
                        '"></div>' +
                    '</div>' +
                    '<div class="bar"></div>' +
                    '<div class="pContent">' +
                        htmlContent +
                    '</div>' +
                    '<div class="bar"></div>' +
                '</div>');

        if (typeof (specialCssClass) != "undefined") {
            popup.addClass(specialCssClass);
        }

        if (typeof (selectorToAppendTo) != "undefined" && selectorToAppendTo.length > 0) {
            popup.appendTo(selectorToAppendTo);
        } else {
            popup.addClass('appendedToBody').appendTo('body');
        }
    }

    //settings.content: the element to make the popup frame around
    //settings.appendTo: where to stick the whole thing to, default -> body
    function _quickPopup(settings) {
        
        //must have a content element
        if (!settings.content) { return; }

        var content = $(settings.content).show();
        var appendTo = $(settings.appendTo || 'body');
        var icon = settings.icon || "";
        var title = settings.title || "";
        
        //if turned into quick popup once, just show the quickpopup and return.
        if (content.hasClass('qpContent')) {
            content.parentsUntil('.quickPopup').show().parent().show();
            content.parentsUntil('.quickPopup').find('.iconFg').css({ 'background-image': 'url(' + icon + ')' });
            return;
        }
        

          
        var quickPopup = $('<div>').addClass('quickPopup').html(
            '<div class="pContainer">' +
                '<div class="pHeader">' +
                    '<div class="stripe"></div>' +
                    '<div class="iconBg"></div>' +
                    '<div class="iconFg" style="background-image:url(\'' + icon + '\')"></div>' +
                    '<div class="title">' + title + '</div>' +
                    '<div class="pClose sfxMenuExit"></div>' +
                '</div>' +
                '<div class="pContent">' +
                '</div>' +
            '</div>');

        //add a special class
        if (settings.customQuickPopupContainerClass) {
            quickPopup.addClass(settings.customQuickPopupContainerClass);
        }

        //if no icon, cleanup the icon portion
        if (!icon) {
            $('.iconBg, .iconFg', quickPopup).hide();
        }

        //if custom func, bind that too
        if (settings.closeFunction) {
            $('.pClose', quickPopup).click(settings.closeFunction);
        }


        //add close binding
        $('.pClose', quickPopup).click(function () {
            quickPopup.hide();
        });



        //append the passed content into the quickPopup content area
        $('.pContent', quickPopup).append(content.addClass('qpContent'));

        quickPopup.appendTo(appendTo);

        
    }



    var _busyCount = 0;
    var _busyTimeoutVar;

    function _base_busy(overrideText, timeOutForTooLongMessage, container) {
        ///<summary>put a busy message</summary>
        ///<param name="timeOutForTooLongMessage" type="int">OPTIONAL - timeout, in MS, when we to display message telling player that somethins is wrong on long api calls. do not specify for default</param>
        ///<param name="overrideText" type="string">OPTIONAL - overrride the message</param>
        ///<param name="container" type="domElement">OPTIONAL - directs to a different method of frame busy</param>

        if (container) {
            _busyContainer(overrideText, timeOutForTooLongMessage, container);
            return;
        }

        window.clearTimeout(_busyTimeoutVar);
        $('#busy-refresh').hide();
        if (++_busyCount) {
            if ($("#busy:visible").length === 0) {
                $("#busy").show();
            }
            _busyTimeoutVar = window.setTimeout(_busyTimeout, timeOutForTooLongMessage || 5000);

            if (overrideText) {
                _busyText(overrideText);
            }
        }

    }

    function _base_free(container) {

        if (container) {
            _freeContainer(container);
            return;
        }
        if (_busyCount === 0 || (! --_busyCount)) {
            $("#busy").hide();
            $('#busy-msg-default').show();
            $('#busy-msg-custom').hide();
        }

    }

    function _busyTimeout() {
        $('#busy-refresh').show();
    }

    function _busyText(msg) {
        ///<summary>override busy message text</summary>
        $('#busy-msg-default').hide();
        $('#busy-msg-custom').html(msg);
        $('#busy-msg-custom').show();
    }

    function _busyFail() {
        window.clearTimeout(_busyTimeoutVar);
        $('#busy-refresh .crashMsg').html("Error!<br>Something went wrong on the server.<br>Click to reload.");
        $('#busy-refresh').show();
    }

    function _base_isBusy() {
        return (_busyCount !== 0);
    }


    function _busyContainer(overrideText, timeOutForTooLongMessage, container) {
        container = $(container).addClass('blurredContainer');
        var busyMask = container.find('.containerBusyMask');
        overrideText = overrideText || 'Loading, Please wait.';
        if (busyMask.length) {
            busyMask.attr('data-count', parseInt(busyMask.attr('data-count')) + 1);
            busyMask.find('.busyMaskText').html(overrideText);
        } else {
            $('<div class="containerBusyMask" data-count="1"><div class="busyMaskText">' + overrideText + '</div></div>').appendTo(container);
        }
    }

    function _freeContainer(container) {
        container = $(container);
        var busyMask = container.find('.containerBusyMask');
        if (busyMask.length) {
            busyMask.attr('data-count', parseInt(busyMask.attr('data-count')) - 1);
            if (parseInt(busyMask.attr('data-count')) < 1) {
                busyMask.remove();
                container.removeClass('blurredContainer');
            }
        } else {
            container.removeClass('blurredContainer');
        }
    }





    function _busy(overrideText, timeOutForTooLongMessage) {
        ///<summary>put a busy message</summary>
        ///<param name="timeOutForTooLongMessage" type="int">OPTIONAL - timeout, in MS, when we to display message telling player that somethins is wrong on long api calls. do not specify for default</param>
        ///<param name="overrideText" type="string">OPTIONAL - overrride the message</param>

        window.clearTimeout(_busyTimeoutVar);
        $('#busy-refresh').hide();
        if (++_busyCount) {
            if ($("#busy:visible").length === 0) {
                $("#busy").show();
            }
            _busyTimeoutVar = window.setTimeout(_busyTimeout, timeOutForTooLongMessage || 10000);

            if (overrideText) {
                _busyText(overrideText);
            }
        }
    }

    function _busyText(msg) {
        ///<summary>override busy message text</summary>
        $('#busy-msg-default').hide();
        $('#busy-msg-custom').html(msg);
        $('#busy-msg-custom').show();
    }

    function _busyFail() {
        window.clearTimeout(_busyTimeoutVar);
        $('#busy-refresh .crashMsg').html("Error!<br>Something went wrong on the server.<br>Tap to refresh.");
        $('#busy-refresh').show();
    }

    function _free() {
        if (_busyCount === 0 || (! --_busyCount)) {
            $("#busy").hide();
            $('#busy-msg-default').show();
            $('#busy-msg-custom').hide();
        }
    }

    function _base_isBusy() {
        return (_busyCount !== 0);
    }



    function _base_showIframeOpenDialog(dialogSelector, frameSrc, dialogTitle) {

        var dialog = $(dialogSelector);
        if (dialogTitle) {
            dialog.dialog('option', 'title', dialogTitle);
        }
        dialog.html('<div style="position: absolute; left: 0px; top: 0px; right: 0px; bottom: 0px; width: auto; height: auto; overflow:hidden;">' +
        '<iframe class="dialogIframe" border="0"  style="height:100%; width:100%; border:none;" src="' + frameSrc + '" frameborder="0"></iframe>' +
        '</div>').dialog('open');
        _base_busy('Loading ...', 5000, dialog);
        $('.dialogIframe', dialog).load(function () {
            _busyCount = 0;
            _base_free(dialog);
        });

    }


    //data properties ->
    //parentElement: if no parent, goes to body
    //startPos: start position, if none, goes center
    //iconUrl: must have an icon,
    //endPos: end position, if none, animates up and fades
    //w: width of icon, defaults to 44
    //h: height of icon, defaults to 44
    function _spawnFlyIcon(data) {

        var parentElement = data.appendTo || $('body');
        var startPos = data.startPos || { top: $(window).height() / 2, left: $(window).width() / 2 }
        var iconUrl = data.iconUrl || "";
        var width = data.w || 44;
        var height = data.h || 44;
        var endPos = data.endPos || { top: $(window).height() / 2, left: $(window).width() / 2 };

        var claimDiv = $('<div class="spawnIconFly" style="width:'+width+'px; height:'+height+'px;">').appendTo(parentElement);

        claimDiv.css({
            left: startPos.left - claimDiv.width() / 2,
            top: startPos.top - claimDiv.height() / 2,
            'background-image': 'url(' + iconUrl + ')'
        });

        if (endPos) {

            claimDiv.animate({ top: '-=30px' }, 500, "easeOutSine")
                .animate({ top: endPos.top, left: endPos.left, width: '44px', height: '44px' }, 1200, "easeInSine", function () {
                    $(this).remove();
                });

        } else {

            claimDiv.animate({ top: '-=30px' }, 1500, "easeOutSine").animate({ top: '-=5px', opacity: 0 }, 300, "easeOutSine", function () {
                $(this).remove();
            });
        }

    }


    obj.simplePopopOverlay = _simplePopopOverlay;
    obj.quickPopup = _quickPopup;
    obj.busyCount = _busyCount;
    obj.base_isBusy = _base_isBusy;
    obj.base_busy = _base_busy;
    obj.base_free = _base_free;
    obj.base_fail = _busyFail;
    obj.busy = _base_busy;
    obj.free = _base_free;
    obj.base_busyText = _busyText;
    obj.base_showIframeOpenDialog = _base_showIframeOpenDialog;
    obj.spawnFlyIcon = _spawnFlyIcon;

} (window.ROE.Frame = window.ROE.Frame || {}));


