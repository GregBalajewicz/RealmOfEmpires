/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />

BDA.Utils = {
    monthes: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
};

BDA.Utils.timeLeft = function (timeInMS) {
    var ret = {};
    var now = new Date();

    ret.timeleft = timeInMS - now;

    ret.totalSecondsLeft = ret.timeleft / 1000;

    ret.h = Math.floor(ret.timeleft / (1000 * 60 * 60));
    ret.timeleft -= ret.h * (1000 * 60 * 60);

    ret.m = Math.floor(ret.timeleft / (1000 * 60));
    ret.timeleft -= ret.m * (1000 * 60);

    ret.s = Math.floor(ret.timeleft / 1000);
    ret.timeleft -= ret.s * 1000;

    return ret;
};

// str in a form of '/Date(1352901753330)/' return Date
BDA.Utils.fromMsJsonDate = function (str) {
    if (str) {
        return new Date(parseInt(str.slice(6, 19)));
    } else {
        var roeex = new BDA.Exception("Error in fromMsJsonDate");
        roeex.data.add('str', str);
        BDA.latestException = roeex;
        BDA.Console.error(roeex);
        throw roeex;
    }
}

BDA.Utils.formatEventTime = function (time, showToday, showMonthes, showYear) {
    var text;

    var midnight = new Date();
    midnight.setUTCHours(23);
    midnight.setUTCMinutes(59);
    midnight.setUTCSeconds(59);

    var d = new Date();
    var today = new Date(d.getFullYear(), d.getMonth(), d.getDate());

    var midnightTomorrow = new Date();
    midnightTomorrow.setUTCHours(47);
    midnightTomorrow.setUTCMinutes(59);
    midnightTomorrow.setUTCSeconds(59);

    if (time < midnight && time > today) {
        if (showToday && !showMonthes)
            text = BDA.Dict.today_at + padDigits(time.getUTCHours()) + ":" + padDigits(time.getUTCMinutes()) + ":" + padDigits(time.getUTCSeconds());
        else 
            text = padDigits(time.getUTCHours()) + ":" + padDigits(time.getUTCMinutes()) + ":" + padDigits(time.getUTCSeconds());
    }
    else if (time < midnightTomorrow && time > midnight) {
        text = BDA.Dict.tomorrow_at + padDigits(time.getUTCHours()) + ":" + padDigits(time.getUTCMinutes()) + ":" + padDigits(time.getUTCSeconds());
    }
    
    if (time > midnightTomorrow || time < today || (showMonthes && showToday)) {
        text = BDA.Utils.monthes[time.getUTCMonth()] + ' ' + padDigits(time.getUTCDate()) + (showYear ? ' ' + padDigits(time.getUTCFullYear() - 2000) : '') + ' ' + padDigits(time.getUTCHours()) + ":" + padDigits(time.getUTCMinutes()) + ":" + padDigits(time.getUTCSeconds());
    }

    return text;

};

//out puts string in format: Jan 01 10:00:59
BDA.Utils.formatEventTimeSimpleUTC = function (dateObject) {
    return BDA.Utils.monthes[dateObject.getUTCMonth()] + ' ' + //month
    padDigits(dateObject.getUTCDate()) + ' ' + //day
    //(padDigits(dateObject.getUTCFullYear() - 2000)) + ' ' + //year
    padDigits(dateObject.getUTCHours()) + ":" + padDigits(dateObject.getUTCMinutes()) + ":" + padDigits(dateObject.getUTCSeconds());
}

//represent time in the past as: 10d 5hr ago  
//and time in future as: in 10d 5hr

////FOr now we assume date object given is expressed in UTC, we can make it a param later
BDA.Utils.formatTimeDifference = function (dateObject) {

    var outPutString = "";

    //get value of now and account for sych time variation
    var valueNow = new Date().valueOf() + ROE.timeOffset;

    //get value of time given
    var valueGiven = dateObject.valueOf();

    /* if we are comparing 2 dates, do we need to assume UTC or otherwise?
     because dates get expressed in local time by default

    console.log('now:', new Date());
    console.log('passed:', dateObject);

    //if we assume given date object is UTC, we need to account ofr local offset
    var assumeUTC = true;
    if (assumeUTC) {
        var timezoneoffsetms = ((new Date().getTimezoneOffset()) * 60000);
        valueGiven = valueGiven - timezoneoffsetms;
    }
    */

    //get the difference between value of given time and now
    var dateDifference = valueGiven - valueNow;

    //we get the absolute value of difference, since we assume its all in past
    var dateDifferenceLeft = Math.abs(dateDifference);

    //number of days ago, then we remove it from total difference
    var diffDays = Math.floor(dateDifferenceLeft / 1000 / 60 / 60 / 24);
    dateDifferenceLeft = dateDifferenceLeft - (diffDays * 24 * 60 * 60 * 1000);

    //number of hours ago, then we remove it from total difference
    var diffHours = Math.floor(dateDifferenceLeft / 1000 / 60 / 60);
    dateDifferenceLeft = dateDifferenceLeft - (diffHours * 60 * 60 * 1000);

    //number of minutes ago, then we remove it from total difference
    var diffMins = Math.floor(dateDifferenceLeft / 1000 / 60);
    dateDifferenceLeft = dateDifferenceLeft - (diffMins * 60 * 1000);

    //number of seconds ago
    var diffSecs = Math.floor(dateDifferenceLeft / 1000);

    //PAST
    //if now is more than given time, given time is in the past
    if (valueNow > valueGiven) {

        //now prepare a string, with at most 2 of the above stuff
        if (diffDays > 0) {
            outPutString = diffDays + "d " + (diffHours ? diffHours + "h" : "") + " ago";
        } else if (diffHours > 0) {
            outPutString = diffHours + "h " + (diffMins ? diffMins + "m" : "") + " ago";
        } else if (diffMins > 0) {
            outPutString = diffMins + "m " + (diffSecs ? diffSecs + "s" : "") + " ago";
        } else {
            outPutString = diffSecs + "s ago";
        }

    } else { //FUTURE

        //now prepare a string, with at most 2 of the above stuff
        if (diffDays > 0) {
            outPutString = "in " + diffDays + "d " + (diffHours ? diffHours + "h" : "");
        } else if (diffHours > 0) {
            outPutString = "in " + diffHours + "h " + (diffMins ? diffMins + "m" : "");
        } else if (diffMins > 0) {
            outPutString = "in " + diffMins + "m " + (diffSecs ? diffSecs + "s" : "");
        } else {
            outPutString = "in " + diffSecs + "s";
        }

    }

    return outPutString;

}



///applies BDA.Utils.formatTimeDifference to an element and keeps it updated periodically as long as the element exists
BDA.Utils.formatTimeDifferenceLive = function (element) {

    function updateTick() {

        //if JQ element exists, AND it exists in DOM no Memory
        if (element.length && $.contains(document.documentElement, element[0])) {
            
            //get the time valye of element, and start updating difference 
            var dateValue = parseInt(element.attr("data-time"));
            element.html(BDA.Utils.formatTimeDifference(new Date(dateValue)));
            
            //periodically recall teh function to update
            window.setTimeout(function () {
                updateTick();
            }, 5000);

        } else {
            //terminates when dom element doesnt exist anymore
        }

    }

    //start ticking
    updateTick();



}



BDA.Utils.formatNum = function (num) {
       
    var n = String(num);    

    var i = n.length % 3;
    if (i == 0) {
        i += 3;
    }

    var s = n.substring(0, i);

    while (i < n.length) {
        s += ("," + n.substring(i, (i += 3)));
    }

    return s;
}

BDA.Utils.formatShortNum = function (num) {
    ///<summary>any numbers 1000 or greater formatted as k, m, b, etc to maximum 999.9q(uadrillion) e.g. '12345' = '12.3k'</summary>
    ///<param name="num" >number or numerical string.  e.g. 12345, "12,345" </param>
    var shortNum = num;
    try {
        if (typeof num == "string") {
            shortNum = parseInt(num.replace(/,/g, ''));
        }
        if (shortNum >= 1000) {
            var exp = Math.floor(Math.log(shortNum) / Math.log(1000));
            var symbols = 'kmbtq';
            shortNum = '' + Math.round(shortNum * 10 / Math.pow(1000, exp)) / 10 + symbols.charAt(exp - 1);
        }
    }
    catch (e) {
        //swallow all errors and reset to num
        shortNum = num;
    }
    return shortNum;
};

BDA.Utils.formatShortTxt = function (txt, len) {
    ///<summary>shortens long text and appends ellipses</summary>
    ///<param name="txt" >the text to shorten</param>
    ///<param name="num" >the length of the return text</param>
    var shortTxt = txt;
    if (!len) {
        len = 15;
    }
    try {
        if (shortTxt.length > len) {
            shortTxt = shortTxt.substr(0, len - 3) + '...';
        }
    }
    catch (e) {
        //swallow all errors and reset to num
        shortTxt = txt;
    }
    return shortTxt;
};

BDA.Utils.padDigits = function padDigits(num) {
    s = num.toString();
    if (s.length == 1) {
        return "0" + s;
    }
    return s;
};



BDA.Utils.GetDateForApiCall = function GetDateForApiCall(date) {
    ///<summary>
    /// This convert the date to a known string so that it can be easily converted to a .net DateTime object
    ///</summary>
    var dd = date.getDate();//yields day
    var MM = date.getMonth() + 1;//yields month
    var yyyy = date.getFullYear(); //yields year
    var HH = date.getHours();//yields hours 
    var mm = date.getMinutes();//yields minutes
    var ss = date.getSeconds();//yields seconds          
    var ms = date.getMilliseconds();//yields MS
    ms = ms.toString();
    if (ms.length == 1) {
        ms = "00" + ms;
    }
    else if (ms.length == 2) {
        ms = "0" + ms;
    }
    return MM + "/" + dd + "/" + yyyy + " " + HH + ':' + mm + ':' + ss + ":" + ms;
};

BDA.Utils.getUTCNow = function getUTCNow() {
    var now = new Date();
    var time = now.getTime();
    var offset = now.getTimezoneOffset();
    offset = offset * 60000;
    return time - offset;
}



String.prototype.format = function (data) {
    return this.replace(/%([a-zA-Z0-9_.]*)%/g,
                function (m, key) {
                    var ks = key.split('.');
                    var el = null;
                    $.each(ks, function (i, n) {
                        el = (!el ? data : el).hasOwnProperty(n) ? (!el ? data : el)[n] : "";
                    })
                    return el;
                }
           );
}

// Capitalize the first letter of a string
String.prototype.capitalizeFirst = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
}

// Array Remove - By John Resig (MIT Licensed)
Array.prototype.remove = function (from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};

BDA.Utils.last = function(t){
    var el;
    for (var i in t) { el = t[i]; }
    return el;
}

BDA.Utils.first = function (t) {
    for (var i in t) { return t[i]; }
}

BDA.Utils.GetMapVillageIconUrl = function (points, villageTypeID) {
   
    var imgurl = "";

    // get the icons for this village type
    var icons = ROE.MapIcons[villageTypeID];

    // if we dont have icons for this village type, get the icons for default village type
    if (!icons)
    {        
        icons = ROE.MapIcons[0]; 
    }
    
    // find proper icon based on points
    var key;
    for (key in icons)
    {
        if (points <= icons[key].MaxVillagePoints)
        {
            return icons[key].IconUrl
        }
    }
}


BDA.Utils.convertCountDownStrToFinishOnDate = function (countdown) {
    ///<summar> takes string like "1:44:13" (1 hour, 44 min 13 seconds left countdown ) and converts it to the date that the counter will finish on. 
    /// example. current time: 1pm, counter "1:44:13", so returned will be date representing 1pm + "1:44:13" = 2:44:13 pm
    ///</summary>
    BDA.Val.required(countdown, "countdown must be specified and type string", true);

    var time = countdown.split(':', 3);
    for (i = 0; i < 3; i++) { time[i] = parseInt(time[i], 10); }

    var finish = new Date();

    finish.setHours(finish.getHours() + time[0]);
    finish.setMinutes(finish.getMinutes() + time[1]);
    finish.setSeconds(finish.getSeconds() + time[2]);

    return finish;

}


BDA.Utils.attentionGrabber = function attentionGrabber(elPos) {
    ///<sumary>
    /// elPos is an object of type { width, height, top, left}, which you can get from ROE.Utils.getElementCoords($(...))
    ///</summary>

    var win = $(window);
    var tuto = $('#tutorial');
    $('.grabber').stop().remove();

    var gT = $('<div class="grabber gT">').appendTo(tuto);
    var gR = $('<div class="grabber gR">').appendTo(tuto);
    var gB = $('<div class="grabber gB">').appendTo(tuto);
    var gL = $('<div class="grabber gL">').appendTo(tuto);

    var speed = 800;
    var easing = "easeOutSine";
    var finalOpacity = .5;

    gT.css({ left: elPos.left + elPos.width / 2 - gT.width() / 2, top: -gT.height() });
    gR.css({ left: win.width(), top: elPos.top + elPos.height / 2 - gR.height() /2 });
    gB.css({ left: elPos.left + elPos.width / 2 - gB.width() / 2, top: win.height() });
    gL.css({ left: -gL.width(), top: elPos.top + elPos.height / 2 - gL.height() /2 });

    gT.animate({ top: elPos.top - gT.height(), opacity: finalOpacity }, speed, easing, function () {
        $('.grabber').stop().animate({ opacity: 0 }, 500, "easeInSine", function () {
            $('.grabber').stop().remove();
        });
    });
    gR.animate({ left: elPos.left + elPos.width, opacity: finalOpacity }, speed, easing, function () { });
    gB.animate({ top: elPos.top + elPos.height, opacity: finalOpacity }, speed, easing, function () { });
    gL.animate({ left: elPos.left - gL.width(), opacity: finalOpacity }, speed, easing, function () { });
}



//custom time picker
!function (e) { "object" == typeof module && "object" == typeof module.exports ? e(require("jquery"), window, document) : "undefined" != typeof jQuery && e(jQuery, window, document) }(function (e, t, i, n) {
    !function () { function t(e, t, i) { return new Array(i + 1 - e.length).join(t) + e } function n() { if (1 === arguments.length) { var t = arguments[0]; return "string" == typeof t && (t = e.fn.timepicker.parseTime(t)), new Date(0, 0, 0, t.getHours(), t.getMinutes(), t.getSeconds()) } return 3 === arguments.length ? new Date(0, 0, 0, arguments[0], arguments[1], arguments[2]) : 2 === arguments.length ? new Date(0, 0, 0, arguments[0], arguments[1], 0) : new Date(0, 0, 0) } e.TimePicker = function () { var t = this; t.container = e(".ui-timepicker-container"), t.ui = t.container.find(".ui-timepicker"), 0 === t.container.length && (t.container = e("<div></div>").addClass("ui-timepicker-container").addClass("ui-timepicker-hidden ui-helper-hidden").appendTo("body").hide(), t.ui = e("<div></div>").addClass("ui-timepicker").addClass("ui-widget ui-widget-content ui-menu").addClass("ui-corner-all").appendTo(t.container), t.viewport = e("<ul></ul>").addClass("ui-timepicker-viewport").appendTo(t.ui), e.fn.jquery >= "1.4.2" && t.ui.delegate("a", "mouseenter.timepicker", function () { t.activate(!1, e(this).parent()) }).delegate("a", "mouseleave.timepicker", function () { t.deactivate(!1) }).delegate("a", "click.timepicker", function (i) { i.preventDefault(), t.select(!1, e(this).parent()) })) }, e.TimePicker.count = 0, e.TimePicker.instance = function () { return e.TimePicker._instance || (e.TimePicker._instance = new e.TimePicker), e.TimePicker._instance }, e.TimePicker.prototype = { keyCode: { ALT: 18, BLOQ_MAYUS: 20, CTRL: 17, DOWN: 40, END: 35, ENTER: 13, HOME: 36, LEFT: 37, NUMPAD_ENTER: 108, PAGE_DOWN: 34, PAGE_UP: 33, RIGHT: 39, SHIFT: 16, TAB: 9, UP: 38 }, _items: function (t, i) { var r, a, o = this, s = e("<ul></ul>"), c = null; for (-1 === t.options.timeFormat.indexOf("m") && t.options.interval % 60 !== 0 && (t.options.interval = 60 * Math.max(Math.round(t.options.interval / 60), 1)), r = i ? n(i) : t.options.startTime ? n(t.options.startTime) : n(t.options.startHour, t.options.startMinutes), a = new Date(r.getTime() + 864e5) ; a > r;) o._isValidTime(t, r) && (c = e("<li>").addClass("ui-menu-item").appendTo(s), e("<a>").addClass("ui-corner-all").text(e.fn.timepicker.formatTime(t.options.timeFormat, r)).appendTo(c), c.data("time-value", r)), r = new Date(r.getTime() + 60 * t.options.interval * 1e3); return s.children() }, _isValidTime: function (e, t) { var i = null, r = null; return t = n(t), null !== e.options.minTime ? i = n(e.options.minTime) : null === e.options.minHour && null === e.options.minMinutes || (i = n(e.options.minHour, e.options.minMinutes)), null !== e.options.maxTime ? r = n(e.options.maxTime) : null === e.options.maxHour && null === e.options.maxMinutes || (r = n(e.options.maxHour, e.options.maxMinutes)), null !== i && null !== r ? t >= i && r >= t : null !== i ? t >= i : null !== r ? r >= t : !0 }, _hasScroll: function () { var e = "undefined" != typeof this.ui.prop ? "prop" : "attr"; return this.ui.height() < this.ui[e]("scrollHeight") }, _move: function (e, t, i) { var n = this; if (n.closed() && n.open(e), !n.active) return void n.activate(e, n.viewport.children(i)); var r = n.active[t + "All"](".ui-menu-item").eq(0); r.length ? n.activate(e, r) : n.activate(e, n.viewport.children(i)) }, register: function (t, i) { var n = this, r = {}; r.element = e(t), r.element.data("TimePicker") || (r.options = e.metadata ? e.extend({}, i, r.element.metadata()) : e.extend({}, i), r.widget = n, e.extend(r, { next: function () { return n.next(r) }, previous: function () { return n.previous(r) }, first: function () { return n.first(r) }, last: function () { return n.last(r) }, selected: function () { return n.selected(r) }, open: function () { return n.open(r) }, close: function () { return n.close(r) }, closed: function () { return n.closed(r) }, destroy: function () { return n.destroy(r) }, parse: function (e) { return n.parse(r, e) }, format: function (e, t) { return n.format(r, e, t) }, getTime: function () { return n.getTime(r) }, setTime: function (e, t) { return n.setTime(r, e, t) }, option: function (e, t) { return n.option(r, e, t) } }), n._setDefaultTime(r), n._addInputEventsHandlers(r), r.element.data("TimePicker", r)) }, _setDefaultTime: function (t) { "now" === t.options.defaultTime ? t.setTime(n(new Date)) : t.options.defaultTime && t.options.defaultTime.getFullYear ? t.setTime(n(t.options.defaultTime)) : t.options.defaultTime && t.setTime(e.fn.timepicker.parseTime(t.options.defaultTime)) }, _addInputEventsHandlers: function (t) { var i = this; t.element.bind("keydown.timepicker", function (e) { switch (e.which || e.keyCode) { case i.keyCode.ENTER: case i.keyCode.NUMPAD_ENTER: e.preventDefault(), i.closed() ? t.element.trigger("change.timepicker") : i.select(t, i.active); break; case i.keyCode.UP: t.previous(); break; case i.keyCode.DOWN: t.next(); break; default: i.closed() || t.close(!0) } }).bind("focus.timepicker", function () { t.open() }).bind("blur.timepicker", function () { setTimeout(function () { t.element.data("timepicker-user-clicked-outside") && t.close() }) }).bind("change.timepicker", function () { t.closed() && t.setTime(e.fn.timepicker.parseTime(t.element.val())) }) }, select: function (t, i) { var n = this, r = t === !1 ? n.instance : t; n.setTime(r, e.fn.timepicker.parseTime(i.children("a").text())), n.close(r, !0) }, activate: function (e, t) { var i = this, n = e === !1 ? i.instance : e; if (n === i.instance) { if (i.deactivate(), i._hasScroll()) { var r = t.offset().top - i.ui.offset().top, a = i.ui.scrollTop(), o = i.ui.height(); 0 > r ? i.ui.scrollTop(a + r) : r >= o && i.ui.scrollTop(a + r - o + t.height()) } i.active = t.eq(0).children("a").addClass("ui-state-hover").attr("id", "ui-active-item").end() } }, deactivate: function () { var e = this; e.active && (e.active.children("a").removeClass("ui-state-hover").removeAttr("id"), e.active = null) }, next: function (e) { return (this.closed() || this.instance === e) && this._move(e, "next", ".ui-menu-item:first"), e.element }, previous: function (e) { return (this.closed() || this.instance === e) && this._move(e, "prev", ".ui-menu-item:last"), e.element }, first: function (e) { return this.instance === e ? this.active && 0 === this.active.prevAll(".ui-menu-item").length : !1 }, last: function (e) { return this.instance === e ? this.active && 0 === this.active.nextAll(".ui-menu-item").length : !1 }, selected: function (e) { return this.instance === e && this.active ? this.active : null }, open: function (t) { var n = this, r = t.getTime(), a = t.options.dynamic && r; if (!t.options.dropdown) return t.element; switch (t.element.data("timepicker-event-namespace", Math.random()), e(i).bind("click.timepicker-" + t.element.data("timepicker-event-namespace"), function (e) { t.element.get(0) === e.target ? t.element.data("timepicker-user-clicked-outside", !1) : t.element.data("timepicker-user-clicked-outside", !0).blur() }), (t.rebuild || !t.items || a) && (t.items = n._items(t, a ? r : null)), (t.rebuild || n.instance !== t || a) && (e.fn.jquery < "1.4.2" ? (n.viewport.children().remove(), n.viewport.append(t.items), n.viewport.find("a").bind("mouseover.timepicker", function () { n.activate(t, e(this).parent()) }).bind("mouseout.timepicker", function () { n.deactivate(t) }).bind("click.timepicker", function (i) { i.preventDefault(), n.select(t, e(this).parent()) })) : (n.viewport.children().detach(), n.viewport.append(t.items))), t.rebuild = !1, n.container.removeClass("ui-helper-hidden ui-timepicker-hidden ui-timepicker-standard ui-timepicker-corners").show(), t.options.theme) { case "standard": n.container.addClass("ui-timepicker-standard"); break; case "standard-rounded-corners": n.container.addClass("ui-timepicker-standard ui-timepicker-corners") } n.container.hasClass("ui-timepicker-no-scrollbar") || t.options.scrollbar || (n.container.addClass("ui-timepicker-no-scrollbar"), n.viewport.css({ paddingRight: 40 })); var o = n.container.outerHeight() - n.container.height(), s = t.options.zindex ? t.options.zindex : t.element.offsetParent().css("z-index"), c = t.element.offset(); n.container.css({ top: c.top + t.element.outerHeight(), left: c.left }), n.container.show(), n.container.css({ left: t.element.offset().left, height: n.ui.outerHeight() + o, width: t.element.outerWidth(), zIndex: s, cursor: "default" }); var u = n.container.width() - (n.ui.outerWidth() - n.ui.width()); return n.ui.css({ width: u }), n.viewport.css({ width: u }), t.items.css({ width: u }), n.instance = t, r ? t.items.each(function () { var i, a = e(this); return i = e.fn.jquery < "1.4.2" ? e.fn.timepicker.parseTime(a.find("a").text()) : a.data("time-value"), i.getTime() === r.getTime() ? (n.activate(t, a), !1) : !0 }) : n.deactivate(t), t.element }, close: function (t) { var n = this; return n.instance === t && (n.container.addClass("ui-helper-hidden ui-timepicker-hidden").hide(), n.ui.scrollTop(0), n.ui.children().removeClass("ui-state-hover")), e(i).unbind("click.timepicker-" + t.element.data("timepicker-event-namespace")), t.element }, closed: function () { return this.ui.is(":hidden") }, destroy: function (e) { var t = this; return t.close(e, !0), e.element.unbind(".timepicker").data("TimePicker", null) }, parse: function (t, i) { return e.fn.timepicker.parseTime(i) }, format: function (t, i, n) { return n = n || t.options.timeFormat, e.fn.timepicker.formatTime(n, i) }, getTime: function (t) { var i = this, n = e.fn.timepicker.parseTime(t.element.val()); return n instanceof Date && !i._isValidTime(t, n) ? null : n instanceof Date && t.selectedTime ? t.format(n) === t.format(t.selectedTime) ? t.selectedTime : n : n instanceof Date ? n : null }, setTime: function (t, i, r) { var a = this, o = t.selectedTime; if ("string" == typeof i && (i = t.parse(i)), i && i.getMinutes && a._isValidTime(t, i)) { if (i = n(i), t.selectedTime = i, t.element.val(t.format(i, t.options.timeFormat)), r) return t } else t.selectedTime = null; return null === o && null === t.selectedTime || (t.element.trigger("time-change", [i]), e.isFunction(t.options.change) && t.options.change.apply(t.element, [i])), t.element }, option: function (t, i, n) { if ("undefined" == typeof n) return t.options[i]; var r, a, o = t.getTime(); "string" == typeof i ? (r = {}, r[i] = n) : r = i, a = ["minHour", "minMinutes", "minTime", "maxHour", "maxMinutes", "maxTime", "startHour", "startMinutes", "startTime", "timeFormat", "interval", "dropdown"], e.each(r, function (i) { t.options[i] = r[i], t.rebuild = t.rebuild || e.inArray(i, a) > -1 }), t.rebuild && t.setTime(o) } }, e.TimePicker.defaults = { timeFormat: "hh:mm p", minHour: null, minMinutes: null, minTime: null, maxHour: null, maxMinutes: null, maxTime: null, startHour: null, startMinutes: null, startTime: null, interval: 30, dynamic: !0, theme: "standard", zindex: null, dropdown: !0, scrollbar: !1, change: function () { } }, e.TimePicker.methods = { chainable: ["next", "previous", "open", "close", "destroy", "setTime"] }, e.fn.timepicker = function (t) { if ("string" == typeof t) { var i, n, r = Array.prototype.slice.call(arguments, 1); return i = "option" === t && arguments.length > 2 ? "each" : -1 !== e.inArray(t, e.TimePicker.methods.chainable) ? "each" : "map", n = this[i](function () { var i = e(this), n = i.data("TimePicker"); return "object" == typeof n ? n[t].apply(n, r) : void 0 }), "map" === i && 1 === this.length ? e.makeArray(n).shift() : "map" === i ? e.makeArray(n) : n } if (1 === this.length && this.data("TimePicker")) return this.data("TimePicker"); var a = e.extend({}, e.TimePicker.defaults, t); return this.each(function () { e.TimePicker.instance().register(this, a) }) }, e.fn.timepicker.formatTime = function (e, i) { var n = i.getHours(), r = n % 12, a = i.getMinutes(), o = i.getSeconds(), s = { hh: t((0 === r ? 12 : r).toString(), "0", 2), HH: t(n.toString(), "0", 2), mm: t(a.toString(), "0", 2), ss: t(o.toString(), "0", 2), h: 0 === r ? 12 : r, H: n, m: a, s: o, p: n > 11 ? "PM" : "AM" }, c = e, u = ""; for (u in s) s.hasOwnProperty(u) && (c = c.replace(new RegExp(u, "g"), s[u])); return c = c.replace(new RegExp("a", "g"), n > 11 ? "pm" : "am") }, e.fn.timepicker.parseTime = function () { var t = [[/^(\d+)$/, "$1"], [/^:(\d)$/, "$10"], [/^:(\d+)/, "$1"], [/^(\d):([7-9])$/, "0$10$2"], [/^(\d):(\d\d)$/, "$1$2"], [/^(\d):(\d{1,})$/, "0$1$20"], [/^(\d\d):([7-9])$/, "$10$2"], [/^(\d\d):(\d)$/, "$1$20"], [/^(\d\d):(\d*)$/, "$1$2"], [/^(\d{3,}):(\d)$/, "$10$2"], [/^(\d{3,}):(\d{2,})/, "$1$2"], [/^(\d):(\d):(\d)$/, "0$10$20$3"], [/^(\d{1,2}):(\d):(\d\d)/, "$10$2$3"]], i = t.length; return function (r) { var a = n(new Date), o = !1, s = !1, c = !1, u = !1, l = !1; if ("undefined" == typeof r || !r.toLowerCase) return null; r = r.toLowerCase(), o = /a/.test(r), s = o ? !1 : /p/.test(r), r = r.replace(/[^0-9:]/g, "").replace(/:+/g, ":"); for (var m = 0; i > m; m += 1) if (t[m][0].test(r)) { r = r.replace(t[m][0], t[m][1]); break } return r = r.replace(/:/g, ""), 1 === r.length ? c = r : 2 === r.length ? c = r : 3 === r.length || 5 === r.length ? (c = r.substr(0, 1), u = r.substr(1, 2), l = r.substr(3, 2)) : (4 === r.length || r.length > 5) && (c = r.substr(0, 2), u = r.substr(2, 2), l = r.substr(4, 2)), r.length > 0 && r.length < 5 && (r.length < 3 && (u = 0), l = 0), c === !1 || u === !1 || l === !1 ? !1 : (c = parseInt(c, 10), u = parseInt(u, 10), l = parseInt(l, 10), o && 12 === c ? c = 0 : s && 12 > c && (c += 12), c > 24 ? r.length >= 6 ? e.fn.timepicker.parseTime(r.substr(0, 5)) : e.fn.timepicker.parseTime(r + "0" + (o ? "a" : "") + (s ? "p" : "")) : (a.setHours(c, u, l), a)) } }() }()
});


