(function (ROE) {
}(window.ROE = window.ROE || {}));


(function (obj) {
    //BDA.Console.setCategoryDefaultView('ROE.RealtimeResource', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file

    var CONST = {};
    // How often the timer refreshes (e.g. silver update)
    CONST.RefreshRate = 500; // in milliseconds

    var _timeOutInterval;

    var _timerTick = function _timerTick() {
        var now = Date.now();
        var now_endOfFunction, functionRunTimeInMS, nextTimeout;

        //
        // Add resource funcs that you want realtime updates for.
        //
        _updateUISilverValues();

        // Borrowed code from BDA-Timer to throttle refresh rate...
        now_endOfFunction = new Date();
        functionRunTimeInMS = (now_endOfFunction - now);
        nextTimeout = functionRunTimeInMS / 0.05; // we want the cost of tick to be < 5% of processing time. 
        nextTimeout = Math.max(ROE.isMobile ? 500 : 100, nextTimeout); // we have certain min timeouts - we dont want this to fire too often, no matter what
        nextTimeout = Math.ceil(nextTimeout / 500.0) * 500; // we want the time out to occur on 0.5 second mark - ie, 500,1000,1500, 2000
        nextTimeout = Math.min(nextTimeout, 5000); // max every 5 seconds tick

        _timeOutInterval = setTimeout(_timerTick, nextTimeout);
    }

    var _updateUISilverValues = function _updateUISilverValues() {
        // If you want silver to be updated automagically, add the "realtimeSilverUpdate"
        // e.g. <div class="... realtimeSilverUpdate" data-vid="0"></div>
        // Where you provide the id for the village in data-vid.
        $('.realtimeSilverUpdate').each(function (i, elem) {           
            var elemJQ = $(elem);
            if (elemJQ.attr("data-vid") != 0) {
                ROE.Villages.getVillage(elemJQ.attr("data-vid"),
                    // Callback after retrieving village.
                    function (village) {
                        // Incase village or coins is undefined, double check...
                        if (village && village.coins) {
                            // Update the silver, only if necessary
                            var elemSilver = parseInt(elemJQ.text().replace(',', ''));
                            if (elemSilver != village.coins) {
                                elemJQ.text(ROE.Utils.addThousandSeperator(village.coins));
                                // Turn red if 
                                if(elemJQ.attr("data-checkmax")) {
                                    elemJQ.toggleClass('maxed', village.coins >= village.coinsTresMax)
                                }
                            }
                        }
                    }
                );
            }
        });
    }


    var _init = function _init() {
        // The initial _timerTick call to get things started.
        setTimeout(function () { _timerTick(); }, CONST.RefreshRate);
    }

    // Initialize right away.
    _init();    

}(window.ROE.RealtimeResource = window.ROE.RealtimeResource || {}));