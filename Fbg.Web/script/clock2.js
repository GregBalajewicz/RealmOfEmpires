

(function (obj) {

    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];


    var Clocks = new Array();

    //This INIT, is and MUST only be called once, per D2 and M frame, it the inits an interval that fires twice a second.
    function _init() {
        _reClock();
        setInterval(_tick, 500);
    }

    function _tick() {

        var midnight = new Date();
        midnight.setUTCHours(23);
        midnight.setUTCMinutes(59);
        midnight.setUTCSeconds(59);

        var midnightTomorrow = new Date();
        midnightTomorrow.setUTCHours(47);
        midnightTomorrow.setUTCMinutes(59);
        midnightTomorrow.setUTCSeconds(59);

        $(Clocks).each(function () {
            var now = new Date().valueOf() + ROE.timeOffset;
            var time = new Date(now);
            time.setUTCSeconds(time.getUTCSeconds() + (this.offset / 1000));

            if (time < midnight) {
                if (this.showToday) {
                    text = "today at " + BDA.Utils.padDigits(time.getUTCHours()) + ":" + BDA.Utils.padDigits(time.getUTCMinutes()) + ":" + BDA.Utils.padDigits(time.getUTCSeconds());
                } else {
                    text =
                        (this.TimeAndDate ?  (months[ time.getUTCMonth()] + " " + time.getUTCDate() + " ") : "") + 
                        BDA.Utils.padDigits(time.getUTCHours()) + ":" + BDA.Utils.padDigits(time.getUTCMinutes()) + ":" + BDA.Utils.padDigits(time.getUTCSeconds());
                }
            //} else if (time < midnightTomorrow) {
            //    text = "tomorow at " + BDA.Utils.padDigits(time.getUTCHours()) + ":" + BDA.Utils.padDigits(time.getUTCMinutes()) + ":" + BDA.Utils.padDigits(time.getUTCSeconds());
            } else {
                text = months[ time.getUTCMonth()] + " " + time.getUTCDate() + " " + BDA.Utils.padDigits(time.getUTCHours()) + ":" + BDA.Utils.padDigits(time.getUTCMinutes()) + ":" + BDA.Utils.padDigits(time.getUTCSeconds());
            }

            this.element.text(text);
        });
    }

    function _reClock() {      
        Clocks.length = 0; //reset the clocks array       
        $('.Time').each(function (i) {
            var element = $(this);
            if (element.attr('data-offset')) {
                Clocks.push({
                    'offset': element.attr('data-offset'),
                    'showToday': element.attr('data-showtoday') == "true",
                    'element': element,
                    'TimeAndDate' : element.hasClass("TimeAndDate")
                });
            }
        });
    }

    obj.init = _init;
    obj.reClock = _reClock;

}(window.ROE.Clock = window.ROE.Clock || {}));





//sigghhhh.... will deal with this later... parts of the app use this global function, hard to clean up -farhad
function padDigits(num) {
    s = num.toString();
    if (s.length == 1) {
        return "0" + s;
    }
    return s;
};