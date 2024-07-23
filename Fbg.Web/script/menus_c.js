var genericMenuConfig = {
	moveAwayDelay	: 700,	// time in miliseconds!
	timer			: null,	// used internally.
	maxHeight       : 400
    }
function genericMenusClear() {
    // code for fading
    //if ($(".genericMenu[keep!='yes']").attr('fade') == 'none')
    //	$(".genericMenu[keep!='yes']").attr('fade', 'fast').fadeOut('fast', function () { $(this).attr('fade', 'done'); });
    $(".genericMenu[keep!='yes'], .ui_menu[keep!='yes']").hide();

    $(".genericMenu, .ui_menu").attr('keep', 'no');

    // cancel the timer (we were just called..)
    //console.log("Clear any non-keep UI menus, calling genericMenuTimer to cancel timers");
    genericMenuTimer(null);
}
function genericMenusClearNow(id) {
    $(".genericMenu, .ui_menu, .ui_menu_fast").attr('keep', 'no');
    $('#' + id).attr('keep', 'yes');

    genericMenusClear();
}

function genericMenuTimer(command) {
    //console.log("genericMenuTimer Called (%s)", command);
    if (!command) {
        // no command, call just wants to cancel a timer
        if (genericMenuConfig.timer) {
            clearTimeout(genericMenuConfig.timer);
            genericMenuConfig.timer = null;
        } else {
        }
    } else {
        // we have a command,
        if (!genericMenuConfig.timer) {
            // no timer set, so we can set one!
            genericMenuConfig.timer = setTimeout(command, genericMenuConfig.moveAwayDelay);
        }
    }
}

function initFakeSelects() {
    $('div.jsFakeSelect').each(
        function () {

            try {
                var options = $(this).find('.jsOptions');
                var master = $(this).find('.jsMaster');
                var triger = $(this).find('.jsTriger');

                $(triger)
                .click(function () { genericMenusClearNow(); $(this).parent().find('.jsOptions').show(); })
                .mouseover(function () { genericMenusClearNow(); $(this).parent().find('.jsOptions').show(); })
                .mouseout(function () { genericMenuTimer('genericMenusClear();'); });

                p = $(master).position();
                if ($(master).hasClass('includeMe')) {
                    p.height = 0;
                    $(master).clone().prependTo(options);
                } else {
                    p.height = $(master).height();
                }

                $(options)
                .css({ 'left': p.left + 'px', 'top': (p.top + p.height) + 'px' })
                .mouseover(function () { $(this).attr('keep', 'yes'); })
                .mouseout(function () { $(this).attr('keep', 'no'); genericMenuTimer('genericMenusClear();'); })
            } catch (e) { }
        });
    return;
}

function jaxReplace() {
    $('.jaxHide')
        .hide()
        .each(
            function () {
                link = $('<a href="#">' + $(this).attr('rel') + '</a>').click(
                    function () { $(this).prev().slideDown().end().remove(); return false; }
                );
                $(this).after(link);
            }
        );
    $('.jaxHideFader')
        .hide()
        .each(
            function () {
                link = $('<a href="#">' + $(this).attr('rel') + '</a>').click(
                    function () { $(this).prev().fadeIn().end().remove(); return false; }
                );
                $(this).after(link);
            }
        );

    $('.rplClanRename').hide().after('<a href="/" onclick="$(this).remove(); $(\'.rplClanRename\').slideDown(); return false;">'+BDA.Dict.Rename+'</a>');
}

function popupOverlay (name, title, x, y, func) {

    // construct a unique-ish id
    var id = ROE.Frame.CONSTS.popupNameIDPrefix + name;

    // create the div, if it doesn't exist!
    if ($('#' + id).length > 0) {
        $('#' + id)
            .show()                // display the (posiably) hidden iframe div
            .attr('keep', 'yes');   // flag it as active
    } else {
        $('body').append('<div id="' + id + '" class="popupOverlayContainer" style="position:absolute;z-index: 4100;"></div>');
        $('#' + id)
            .css({ top: y + 'px', left: x + 'px' })
            .append('<div class=popupBody  ></div>')
            .append('<script> function closeMe() { return !' + func + '(\'' + name + '\', true);}</script>')
            .fadeIn('slow');
            
    }

    return true;
}


function popupModalOverlay(name, title, x, y, f) {

    $('body').append('<div class="popup_modal" style="height: 100%; width: 100%; position: fixed; left: 0pt; top: 0pt; z-index: 4000; opacity: 0.5; filter: alpha(opacity=50); background-color: black;"></div>');

    x = x ? x : ROE.Frame.CONSTS.popupDefaultX;
    y = y ? y : ROE.Frame.CONSTS.popupDefaultY;
    f = f ? f : 'closeModalOverlay';

    return popupOverlay(name, title, x, y, f);
}

function closeModalOverlay(name) {
    $('.popup_modal').remove();

    return closeOverla(name);
}

function closeOverla(name) {
    
    // we're just going to hide the iframe/div.. for now.
    var id = ROE.Frame.CONSTS.popupNameIDPrefix + name;
    $('#' + id)
        .fadeOut(400, function () {
            $(this).remove(); 
        });     
    return true;
}



function closeModalPopup(name, parentDivAlso) {
    $('.popup_modal').remove();
    ROE.vov.unpauseAnimation();
    return closePopup(name, parentDivAlso);
}

function closePopup(name) {
    if (name == "*") {
        $('iframe').parents('.iFrameDiv').remove();
    } else {
        // we're just going to hide the iframe/div.. for now.
        var id = ROE.Frame.CONSTS.popupNameIDPrefix + name;
        $('#' + id)
            .attr('keep', 'no')
            .remove();
    }

    ROE.Frame.enableView(true);


    return true;
}

function popupIFrameFlip(url, name, title, height, width, from) {
    var CSSSTYLE_TRANSITION = Modernizr.prefixed("transition");
    var DOMEVENT_TRANSITION = {
        "WebkitTransition": "webkitTransitionEnd",
        "MozTransition": "transitionend",
        "OTransition": "oTransitionEnd otransitionend",
        "msTransition": "MSTransitionEnd",
        "transition": "transitionend"
    }[CSSSTYLE_TRANSITION];

    var id = "popupIFrameFlip_" + name;

    var p = $("#" + id);
    if (p.length) {
        p.remove();
    }

    p = $('<div style="position: absolute;"></div>')
        .attr("id", id);

    p.append(
        $('<a style="display: block; height: 44px;" href="#" class="back">Back</a>')
            .click(function (event) {
                if (Modernizr.csstransitions) {
                    p.one(DOMEVENT_TRANSITION, function (event) {
                        p.remove();
                    });
                    p.removeClass("transition");

                    $(from).removeClass("transition");
                } else {
                    p.fadeOut("", function () {
                        p.remove();
                    });

                    $(from).fadeIn();
                }
            })
    );


    var o = $('<iframe frameborder="0" marginwidth="0" marginheight="0"></iframe>')
        .attr("src", url)
        .attr("name", name)
        .css("height", height - 44)
        .css("width", width)
        .css("border", "0px");

    p.append(o);

    if ($(from).length)
        $(from).after(p);
    else
        $("body").append(p);

    if (Modernizr.csstransitions) {
        $(from).addClass("leftFrom");
        p.addClass("leftTo");

        window.setTimeout(function () {
            $(from).one(DOMEVENT_TRANSITION, function (event) {
            });
            $(from).addClass("transition");

            p.addClass("transition");
        }, 20);
    } else {
        $(from).fadeOut("", function () {
        });

        p.fadeIn();
    }

    return true;
}


