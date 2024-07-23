"use strict";
/// <reference path="roe-utils.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.BDA = window.BDA || {}, jQuery));

//
// left here for compatilibity. just an alias for BDA.Timer.initTimers()
//
function initTimers() {
    ///<summary>repreciated. use BDA.Timer.initTimers() instead</summary>
}

//global server-local time offset
ROE.timeOffset = 0;

(function (obj, $, undefined) {
    BDA.Console.setCategoryDefaultView('BDA.Timer-perf', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file
    BDA.Console.setCategoryDefaultView('BDA.Timer', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file


    var _timeOutInterval;
    var _timerTick = function _timerTick ()
    {
        /*
        a lot of work was done to try to optimize this. 
        see case 27357, tagged "codecomment"
        */

        BDA.Console.verbose('BDA.Timer-perf', 'Tick');

        var now = new Date().valueOf();
        var nowServerSynched = now + ROE.timeOffset;
        var now_endOfFunction;
        var functionRunTimeInMS;
        var nextTimeout;
        var left;
        var oneTimer;
        var totalSecondsLeft;
        var percentageWidth;
        var timerPercentageWidth;
        var functToCallOnRefresh;
        var functToCallOnRefreshDelay;
        var textWhenFinished;
        var formating;
        var newTimerText;
        var days, hours, hours_withoutdays, mins, secs;
        var progressBar;
        var totalSeconds;

        var finishesOn;

        var eventPointTime; //indicates at certain milliseconds left before finishesOn, do something from "attr('data-eptime')"
        var eventPointFunc; //what to do if eventPointTime exists and is reached from "attr('data-eptime')"

        //var countdownHtmlElement;
        $('.countdown2, .Countdown, .countdown').each(
            function processOneTimer() {
                oneTimer = $(this);

                finishesOn = oneTimer.attr('data-finishesOn');

                if (!finishesOn) {
                    // we dont have finishesOn, meaning that it could be malformed html or old timer
                    if (oneTimer.text() != "") {
                        // lets try to extract the finishesON from the countdown currently displayed
                        BDA.Console.verbose('BDA.Timer', 'no data-finishesOn, trying to read the text');
                        finishesOn = BDA.Utils.convertCountDownStrToFinishOnDate(oneTimer.text());
                        oneTimer.attr('data-finishesOn', finishesOn.getTime());

                        // timer is bad, kill it. 
                        if (oneTimer.attr('data-finishesOn') == "NaN") {
                            oneTimer.removeClass('countdown2 Countdown countdown');
                            return;
                        }
                    } else {
                        // timer is bad, kill it. 
                        oneTimer.removeClass('countdown2 Countdown countdown');
                        return;
                    }
                }

                if (this == null) {
                    return;
                }

                left = finishesOn - nowServerSynched;
                

                if (left < 0) {
                    //
                    // if the timer is finished, remove the id attr and reload the window
                    //                    
                    functToCallOnRefresh =  oneTimer.attr('data-refreshCall') ? oneTimer.attr('data-refreshCall') : '';
                    functToCallOnRefreshDelay =  oneTimer.attr('data-refreshCallDelay') ? parseInt(oneTimer.attr('data-refreshCallDelay'), 10) : 4000;
                    textWhenFinished = oneTimer.attr('data-textWhenFinished') ? oneTimer.attr('data-textWhenFinished') : "Finished";

                    oneTimer.removeClass('countdown2 Countdown countdown').text(textWhenFinished);

                    if (left <= -4000) {

                        if (functToCallOnRefresh) {
                            setTimeout(functToCallOnRefresh, 1);
                        }

                    } else {
                       
                        if (functToCallOnRefresh) {
                            setTimeout(functToCallOnRefresh, functToCallOnRefreshDelay);
                            BDA.Console.verbose('BDA.Timer', "Tick : timer finished.");
                        }

                        progressBar = getProgressBar(oneTimer);
                        if (progressBar) {
                            progressBar.css({ width: "100%" });
                        }
                    }
                    
                } else {
                    //
                    // timer not done, update it. 
                    //
                    totalSecondsLeft = 6000;
                    totalSecondsLeft = left / 1000;

                    days = Math.floor(left / (1000 * 60 * 60 *24));
                    //left -= hours * (1000 * 60 * 60);

                    hours = Math.floor(left / (1000 * 60 * 60));
                    hours_withoutdays = Math.floor((left - days * (1000 * 60 * 60 *24)) / (1000 * 60 * 60));
                    left -= hours * (1000 * 60 * 60);

                    mins = Math.floor(left / (1000 * 60));
                    left -= mins * (1000 * 60);

                    secs = Math.floor(left / 1000);
                    left -= secs * 1000;

                    

                    formating = oneTimer.attr('format');
                    if (formating == 'long') {
                        newTimerText =
                            (hours > 0 ? hours + " hours, " : "") +
                            (mins > 0 || hours > 0 ? mins + " minutes, " : "") +
                            secs + " seconds";
                    } else if (formating == 'long2') {
                        newTimerText =
                            (days > 0 ? days + "d " : "") +
                            (hours_withoutdays > 0 ? hours_withoutdays + "h " : "");


                        if (days < 1) {
                            newTimerText +=
                           (mins > 0 ? mins + "m " : "");
                        }

                        if (newTimerText == "") {
                            newTimerText =
                               (secs > 0 ? secs + "s " : "");
                        }
                    }
                    else {
                        newTimerText = padDigits(hours) + ':' + padDigits(mins) + ':' + padDigits(secs);
                    }

                    // if timer's time has changes, update it 
                    if (newTimerText !== oneTimer.text()) {
                        oneTimer.text(newTimerText);

                        progressBar = getProgressBar(oneTimer);
                        if (progressBar) {
                            totalSeconds = oneTimer.attr('data-totalMS') /1000;
                            percentageWidth = Math.ceil(((totalSeconds - totalSecondsLeft) / totalSeconds) * 100);
                            timerPercentageWidth = Math.ceil(((totalSeconds - totalSecondsLeft) / totalSeconds) * 100);
                            if (percentageWidth > 100) { percentageWidth = 100; };
                            progressBar.css({ width: (timerPercentageWidth) + "%" });
                        }

                    }

                    //this part is used for when a timer needs to do something at a point in time (X milliseconds left to completion)
                    eventPointTime = parseInt(oneTimer.attr('data-eptime'));
                    if (!isNaN(eventPointTime)) {               
                        var timeLeft = finishesOn - nowServerSynched;
                        if (timeLeft < eventPointTime) {
                            var epfunc = oneTimer.attr('data-epfunc');
                            oneTimer.removeAttr('data-eptime').removeAttr('data-epfunc');
                            if (!epfunc) { return; }
                            setTimeout(epfunc, 1);
                        }                 
                    }
                }
            }
        );
        //
        // calc function run time, and hence the next tick time
        //
        now_endOfFunction = new Date();
        functionRunTimeInMS = (now_endOfFunction - now);
        nextTimeout = functionRunTimeInMS / 0.05; // we want the cost of tick to be < 5% of processing time. 
        nextTimeout = Math.max(ROE.isMobile ? 500 : 100, nextTimeout); // we have certain min timeouts - we dont want this to fire too often, no matter what
        nextTimeout = Math.ceil(nextTimeout / 500.0) * 500; // we want the time out to occur on 0.5 second mark - ie, 500,1000,1500, 2000
        nextTimeout = Math.min(nextTimeout, 5000); // max every 5 seconds tick

        _timeOutInterval = setTimeout(_timerTick, nextTimeout);

        BDA.Console.verbose('BDA.Timer-perf', 'Tick END :' + functionRunTimeInMS + ', next tick:' + nextTimeout);
    }

    function getProgressBar(oneTimer) {
        var idOfProgressBar;
        var progressBar;

        // check if the timer has a progress bar, if so, update it
        idOfProgressBar = oneTimer.attr("data-progressid");
        if (idOfProgressBar) {
            progressBar = $('.countdownProgressBar.id_%id%'.format({ id: idOfProgressBar }));
            if (progressBar.length > 0) { // sanity check 
                return progressBar;
            } else {
                oneTimer.removeAttr("data-progressid"); // actual progress bar not found, so remove this for performance
                return undefined;
            }
        }
    }

    setTimeout(function () { _timerTick(); }, 1000);

    obj.initTimers = function () { };


} (window.BDA.Timer = window.BDA.Timer || {}, jQuery));
