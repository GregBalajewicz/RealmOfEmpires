/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

(function (obj, $, undefined) {

    var _popupContent;
    var _template;

    var _showPopup = function _showPopup(launchMode) {
    
        _template = BDA.Templates.getRawJQObj('SleepMode', ROE.realmID);

        if (launchMode && launchMode == "d2") {          
            _popupContent = $('#sleepModeDialog').dialog('open');
            _template.find('.title').remove();
        } else {
            if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'SleepMode .popupBody').length > 0) { return; }
            popupModalOverlay('SleepMode', '', 0, 0);
            _popupContent = $('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'SleepMode .popupBody');
        }

        _populate();
    }

    var _populate = function _populate(){
 
        _popupContent.html(_template.clone());

        var sleepAvailable = true;       
        if (ROE.Player.sleepModeOn > 0) {
            var counter = $("#sleepMode SPAN").text();
            $('#SleepModeBox .set1 .countdown').text(counter);
            $('#SleepModeBox .set1').addClass("ON");
            sleepAvailable = false;
        }

        if (ROE.Player.SleepModeCountdown.indexOf("-") == -1 && sleepAvailable) {           
            $('#SleepModeBox .set2 .countdown').text(ROE.Player.SleepModeCountdown);
            $('#SleepModeBox .set2').addClass("ON");
            sleepAvailable = false;
        }

        if (sleepAvailable) {
            $('#SleepModeBox .set3').addClass("ON");
        }

        var str = $('#SleepModeBox .ON .text').html();
        var str = str.replace("{0}", ROE.Player.SleepinRealm.Duration);
        var str = str.replace("{1}", ROE.Player.SleepinRealm.TimeTillActive);
        $('#SleepModeBox .ON .text').html(str);

        var str = $('#SleepModeBox .SleepHelpBox').html();
        var str = str.replace("{0}", ROE.Player.SleepinRealm.Duration);
        var str = str.replace("{1}", ROE.Player.SleepinRealm.TimeTillActive);
        var str = str.replace("{2}", ROE.Player.SleepinRealm.AavailableOnceEveryXHours);
        $('#SleepModeBox .SleepHelpBox').html(str);

        if (ROE.Player.SleepinRealm.TimeTillActive < 1) {
            var str = _phrases(2);
            var str = str.replace("{1}", Math.floor(ROE.Player.SleepinRealm.TimeTillActive * 60));
        }
        else {
            var str = _phrases(1);
            var str = str.replace("{1}", ROE.Player.SleepinRealm.TimeTillActive);
        }
      
        $('#SleepModeBox .activateSleep').text(str);
        $('#popup_SleepMode .titleClose').click(_close);
        $('.infoBtnSleep, .SleepHelpBox', _popupContent).click(function () { $('.SleepHelpBox', _popupContent).toggle(); });

        //if WM is to kick in within certain time 
        //then dont allow SM
        /*
        if (ROE.Player.WeekendModeInRealm.Allowed && ROE.Player.WeekendModeStatus.requested) {
            var certainTimeFromNow = (new Date()).valueOf() + (16 * 3600 * 1000); //16 hours is hardcoded in sleep activate api
            if (ROE.Player.WeekendModeStatus.takesEffectOnMilli < certainTimeFromNow) {
                $('.setContainer.sleepContainer')
                    .html('<div class="fontGoldFrLCXlrg" >Sleep Mode</div><div style="color: #E63131;">Cant use Sleep Mode within 16 hours of Weekend Mode activation.</div>');
            }
        }
        */

        $("#SleepModeBox .activateSleep").click(function (e) {

            e.stopPropagation();


            var btn = $(this);

            if (btn.hasClass('busy')) { return; }

            if (!btn.hasClass('areyousure')) {
                btn.attr('data-text', btn.text());
                btn.addClass('areyousure').html("Are you sure?");
                window.setTimeout(function () {
                    if (btn.hasClass('busy')) { return; }
                    btn.removeClass('areyousure').html(btn.attr('data-text'));
                }, 2500);
                return;
            }

            btn.removeClass('areyousure').addClass('busy').html("Requesting...");


            ROE.Api.call("activatesleepmode", {}, _activatesleepmodeCB);

            ROE.Frame.busy('Feeling sleepy...', 5000, $('#sleepModeDialog'));

        });

        function _activatesleepmodeCB(ret) {

            ROE.Frame.free($('#sleepModeDialog'));

            var sleepActivateBtn = $("#SleepModeBox .activateSleep");
            sleepActivateBtn.removeClass('busy').html(sleepActivateBtn.attr('data-text'));

            if (ret.activate) {
                ROE.Player.sleepModeOn = 1;

                setTimeout(function () {

                    //start counter
                    var ctext = parseInt(ROE.Player.SleepinRealm.TimeTillActive * 3600);
                    var chour = Math.floor(ctext / (60 * 60));
                    ctext -= chour * (60 * 60);
                    var cmin = Math.floor(ctext / 60);
                    ctext -= cmin * 60;
                    var csec = Math.floor(ctext);
                    var counterText = BDA.Utils.padDigits(chour) + ":" + BDA.Utils.padDigits(cmin) + ":" + BDA.Utils.padDigits(csec);

                    $("#sleepMode SPAN").text(counterText).removeClass("deactive").css("opacity", 1).show().addClass("countdown");

                    _populate();

                    initTimers();

                }, 300);
            } else {
                //somehow SM didnt get activated
            }
        }


        //Setup Vacation Mode

        if (ROE.Player.VacationInRealm.Allowed) {

            $('.infoBtnVacation, .VacationHelpBox', _popupContent).click(function () { $('.VacationHelpBox', _popupContent).toggle(); });
            var str = $('#SleepModeBox .VacationHelpBox').html();
            var str = str.replace("%delay%", ROE.Player.VacationInRealm.ActivationDelayDays);
            var str = str.replace("%PerUseMinimum%", ROE.Player.VacationInRealm.PerUseMinimum);

            //to make per use max more communicable, we write it out capped based on actual days left
            var daysLeft = Math.max(0, ROE.Player.VacationStatus.daysMax - ROE.Player.VacationStatus.daysUsed);
            var str = str.replace("%PerUseMaximum%", Math.min(daysLeft, ROE.Player.VacationInRealm.PerUseMaximum));
            var str = str.replace("%reactivation%", ROE.Player.VacationInRealm.ReactivationDelayDays);
            $('#SleepModeBox .VacationHelpBox').html(str);

            if (!ROE.Player.VacationStatus.requested) {

                $('#SleepModeBox .set4', _popupContent).show();

                var elementVacationDaysTotal = $('#SleepModeBox .set4 .vacationDaysTotal', _popupContent);
                elementVacationDaysTotal.html(elementVacationDaysTotal.html().replace("%vacationDaysTotal%", ROE.Player.VacationStatus.daysMax));

                var elementVacationDaysLeft = $('#SleepModeBox .set4 .vacationDaysLeft', _popupContent);
                

                //if no vacation days left
                if (daysLeft < 1) { 
                    $('#SleepModeBox .activateVacation', _popupContent).hide();
                    elementVacationDaysLeft.html('No available vacation days.').css('color', '#E63131');
                } else {

                    $('#SleepModeBox .activateVacation', _popupContent).click(_activateVacation);
                    elementVacationDaysLeft.html(elementVacationDaysLeft.html().replace("%vacationDaysLeft%", daysLeft));

                    //check reactivation time
                    var lastEndDate = new Date(ROE.Player.VacationStatus.lastEndOnOnMilli);
                    var reActivateAllowedDate = lastEndDate.setDate(lastEndDate.getDate() + ROE.Player.VacationInRealm.ReactivationDelayDays)
                    if (reActivateAllowedDate > (new Date())) {
                        $('#SleepModeBox .activateVacation', _popupContent).hide();
                        var reactivateDateElement = $('#SleepModeBox .reactivateDate', _popupContent).show();
                        reactivateDateElement.html(reactivateDateElement.html().replace("%reactivateAllowedDate%", BDA.Utils.formatEventTimeSimpleUTC(new Date(reActivateAllowedDate))));
                    }

                }

            } else {

                $('#SleepModeBox .set5', _popupContent).show();

                var elementVacationRequestedOn = $('#SleepModeBox .set5 .vacationRequestedOn', _popupContent);
                var dateFormattedRequestedOn = BDA.Utils.formatEventTimeSimpleUTC(new Date(ROE.Player.VacationStatus.requestedOnMilli));
                elementVacationRequestedOn.html(elementVacationRequestedOn.html().replace("%vacationRequestedOn%", dateFormattedRequestedOn));

                var elementVacationTakesEffectOn = $('#SleepModeBox .set5 .vacationTakesEffectOn', _popupContent);
                var dateFormattedTakesEffectOn = BDA.Utils.formatEventTimeSimpleUTC(new Date(ROE.Player.VacationStatus.takesEffectOnMilli));
                elementVacationTakesEffectOn.html(elementVacationTakesEffectOn.html().replace("%vacationTakesEffectOn%", dateFormattedTakesEffectOn));

                var elementVacationEndsOn = $('#SleepModeBox .set5 .vacationEndsOn', _popupContent);
                var dateFormattedEndsOn = BDA.Utils.formatEventTimeSimpleUTC(new Date(ROE.Player.VacationStatus.endsOnMilli));
                elementVacationEndsOn.html(elementVacationEndsOn.html().replace("%vacationEndsOn%", dateFormattedEndsOn));


            }

        } else {
            $('#SleepModeBox .vacationContainer', _popupContent).remove();
        }

        _setupWeekendMode();

        initTimers();

        ROE.Frame.free($('#sleepModeDialog'));
    }



    var _activateVacation = function _activateVacation() {

        var btn = $('#SleepModeBox .activateVacation', _popupContent);

        if (btn.hasClass('busy')) { return; }

        if (!btn.hasClass('areyousure')) {
            btn.attr('data-text', btn.text());
            btn.addClass('areyousure').html("Are you sure?");
            window.setTimeout(function () {
                if (btn.hasClass('busy')) { return; }
                btn.removeClass('areyousure').html(btn.attr('data-text'));
            }, 2500);
            return;
        }

        btn.removeClass('areyousure').addClass('busy').html("Requesting...");

        var duration = $('#vacationDuration').val();
        ROE.Api.call_activateVacation(duration, _activateVacationReturn);
        ROE.Frame.busy('Vacation time!', 5000, $('#sleepModeDialog'));
    }

    var _activateVacationReturn = function _activateVacationReturn(data) {
        ROE.Frame.free($('#sleepModeDialog'));
        ROE.Player.VacationStatus = data.status;
        _populate();
    }







    //WEEKEND MODE SECTION
    function _getContainer() {

        if ($('#sleepModeDialog').length) {
            return $('#sleepModeDialog');
        } else {
            return $('#popup_SleepMode');
        }
    }

    function _setupWeekendMode(){
    
        if (ROE.Player.WeekendModeInRealm.Allowed) {


            var str = $('#SleepModeBox .WeekendModeHelpBox').html();
            var str = str.replace("%wmDurationDays%", ROE.Player.WeekendModeInRealm.RealmBaseDays);
            var str = str.replace("%wmActivationDelay%", ROE.Player.WeekendModeInRealm.ActivationDelayMinimumHours);
            var str = str.replace("%wmReactivationDelayDays%", ROE.Player.WeekendModeInRealm.ReactivationDelayDays);
            $('#SleepModeBox .WeekendModeHelpBox').html(str);

            //WM info box click
            $('.infoBtnWeekendMode, .WeekendModeHelpBox', _popupContent).click(function () { $('.WeekendModeHelpBox', _popupContent).toggle(); });

            //WM setup popup launch
            $('#SleepModeBox .openSetupWM', _popupContent).click(_openSetupWM);

            //WM setup poup close
            $('#SleepModeBox .wmRequestPanel .close', _popupContent).click(function () {
                $('#SleepModeBox .wmRequestPanel', _popupContent).hide();
            });

            //WM request click to API call
            $('#SleepModeBox .requestWM', _popupContent).click(_requestWeekendMode);


            //get UTC components of the current set time
            var activationHours = ROE.Player.WeekendModeInRealm.ActivationDelayMinimumHours;
            var hoursInMilliseconds = 3600000 * activationHours;
            var currentDateTimeValue = Date.now().valueOf() + (hoursInMilliseconds);
            var nowDate = new Date(currentDateTimeValue);
            var utcYear = nowDate.getUTCFullYear();
            var utcMonth = nowDate.getUTCMonth();
            var utcDayOfMonth = nowDate.getUTCDate();
            var utcHour = nowDate.getUTCHours();
            var utcMinute = nowDate.getUTCMinutes();
            var utcSecond = nowDate.getUTCSeconds();

            //set current time into the input and make into a timpicker
            //timepicker source: http://timepicker.co/
            $('.wmTimePicker', _popupContent).timepicker({
                //timeFormat: 'hh:mm:ss p',
                timeFormat: 'HH:mm',
                defaultTime: utcHour + ':' + (utcMinute+2) + ':' + utcSecond, //we add 2 more minutes to padd for minimum time from now
                dropdown: false,
                scrollbar: false,
                change: function (time) {
                    //console.log('timepicker change', time);
                }
            });

            // for day month year, we use a jquery date picker
            //add 1 to month because its 0 indexed
            $('.wmDatePicker', _popupContent).val((utcMonth + 1) + '/' + utcDayOfMonth + '/' + utcYear).datepicker({
                dateFormat: 'mm/dd/yy'
            });


            //if Weekend Mode not yet requested
            if (!ROE.Player.WeekendModeStatus.requested) {

                $('#SleepModeBox .setWM-NR', _popupContent).show();

                //check reactivation time
                var lastEndDate = new Date(ROE.Player.WeekendModeStatus.lastEndOnOnMilli);
                var reActivateAllowedDate = lastEndDate.setDate(lastEndDate.getDate() + ROE.Player.WeekendModeInRealm.ReactivationDelayDays)

                //if reactivation not allowed
                if (reActivateAllowedDate > (new Date())) {
                    $('#SleepModeBox .setWM-NR .openSetupWM', _popupContent).hide();
                    var reactivateDateElement = $('#SleepModeBox .setWM-NR .wmReactivateDate', _popupContent).show();
                    var reActivateAllowedDateFormatted = BDA.Utils.formatEventTimeSimpleUTC(new Date(reActivateAllowedDate));
                    reactivateDateElement.html(reactivateDateElement.html().replace("%wmReactivateAllowedDate%", reActivateAllowedDateFormatted));

                //if reactivation allowed
                } else {
                    var reactivateDateElement = $('#SleepModeBox .setWM-NR .wmReactivateDate', _popupContent).hide();

                    //format the request panel, and show the request panel toggle-button
                    var requestPanel = $('#SleepModeBox .wmRequestPanel', _popupContent);
                    var wmDurationDaysPanel = $('.wmDurationDays',requestPanel);
                    wmDurationDaysPanel.html(wmDurationDaysPanel.html().replace("%wmDurationDays%", ROE.Player.WeekendModeInRealm.RealmBaseDays));
                    var wmActivationDelayPanel = $('.wmActivationDelay',requestPanel);
                    wmActivationDelayPanel.html(wmActivationDelayPanel.html().replace("%wmActivationDelay%", ROE.Player.WeekendModeInRealm.ActivationDelayMinimumHours));
                    $('#SleepModeBox .setWM-NR .openSetupWM', _popupContent).show();
                }

            //if a request is in, but not yet active
            } else {
               
                $('#SleepModeBox .setWM-R', _popupContent).show();

                var elementWMTakesEffectOn = $('#SleepModeBox .setWM-R .wmTakesEffectOn', _popupContent);
                var dateFormattedTakesEffectOn = BDA.Utils.formatEventTimeSimpleUTC(new Date(ROE.Player.WeekendModeStatus.takesEffectOnMilli));
                elementWMTakesEffectOn.html(elementWMTakesEffectOn.html().replace("%wmTakesEffectOn%", dateFormattedTakesEffectOn));

                var elementWMEndsOn = $('#SleepModeBox .setWM-R .wmEndsOn', _popupContent);
                var dateFormattedEndsOn = BDA.Utils.formatEventTimeSimpleUTC(new Date(ROE.Player.WeekendModeStatus.endsOnMilli));
                elementWMEndsOn.html(elementWMEndsOn.html().replace("%wmEndsOn%", dateFormattedEndsOn));
            }

        } else {
            $('#SleepModeBox .weekendModeContainer', _popupContent).remove();
        }

    }

    function _openSetupWM() {
        $('#SleepModeBox .wmRequestPanel').show();
    }

    //Request WM - send time and date params.
    function _requestWeekendMode() {

        var _container = _getContainer();

        /* put together a date time and send it for the API */
        var timePickerVal = $('.wmTimePicker', _container).val();
        var datePickerVal = $('.wmDatePicker', _container).val();

        //construct a date object from the two inputs, (which is assumed to be displaying in UTC)
        var constructedDateTimeRaw = Date.parse(datePickerVal + ' ' + timePickerVal);
        //the date object is made into local time so we convert to UTC
        var timezoneoffsetms = ((new Date().getTimezoneOffset()) * 60000);
        constructedDateTimeRaw = constructedDateTimeRaw - timezoneoffsetms;

        //add realm activation delay hours, as miliseconds, to current date, to get earliest time WM can be activated
        var minimumActivationDateRaw = new Date().valueOf() + (ROE.Player.WeekendModeInRealm.ActivationDelayMinimumHours * 3600000);
    
        //console.log('constructedDateTimeRaw', constructedDateTimeRaw, new Date(constructedDateTimeRaw).toUTCString());
        //console.log('minimumActivationDate', minimumActivationDateRaw, new Date(minimumActivationDateRaw).toUTCString());

        //if input time is later than minium activation time
        if (constructedDateTimeRaw > minimumActivationDateRaw) {
            ROE.Frame.busy('Weekend Time!', 5000, _container);
            // desiredActivationDate is the UTC date the person wants to enter into WM, 
            ROE.Api.call_weekendModeRequest(constructedDateTimeRaw, _requestWeekendModeCB);
        } else {
            //the input time was earlier than is valid, we correct it, and send error msg

            //get UTC components of the minimumActivationDateRaw
            var nowDate = new Date(minimumActivationDateRaw);
            var utcYear = nowDate.getUTCFullYear();
            var utcMonth = nowDate.getUTCMonth();
            var utcDayOfMonth = nowDate.getUTCDate();
            var utcHour = nowDate.getUTCHours();
            var utcMinute = nowDate.getUTCMinutes();
            var utcSecond = nowDate.getUTCSeconds();
            
            //input the minimum time (+2 minutes for grace)
            $('.wmTimePicker', _popupContent).val(utcHour + ':' + (utcMinute + 2) + ':' + utcSecond);
            $('.wmDatePicker', _popupContent).val((utcMonth + 1) + '/' + utcDayOfMonth + '/' + utcYear);

            ROE.Frame.errorBar('Can not enter Weekend Mode before: ' +
                ((utcMonth + 1) + '/' + utcDayOfMonth + '/' + utcYear) + " " +
                (utcHour + ':' + utcMinute + ':' + utcSecond) + " Game Time.");

        }
        
        //for testing we can get raw local datetime
        //var constructedDateTimeRaw = Date.now();
        //console.log(constructedDateTimeRaw, new Date(constructedDateTimeRaw).toUTCString());
 
    }

    function _requestWeekendModeCB(data) {

        ROE.Frame.free(_getContainer());

        if (data && data.result) {
            ROE.Player.WeekendModeStatus = data.status;
            _populate();
        } else {
            //ROE.Frame.errorBar('Weekend Mode request failed. Possible Sleep Mode cooldown, check More Info.');
            ROE.Frame.errorBar('Weekend Mode request failed.');
        }

    }

    /* 
    //were not making Cancel API calls normally, itll be done in the WM Active aspx page
    function _cancelWeekendMode() {
        ROE.Api.call_weekendModeCancel({}, _cancelWeekendModeCB);
    }
    function _cancelWeekendModeCB(data) {
        console.log('_cancelWeekendModeCB', data);
    }
    */

    //end of WM code

    
    var _close = function (data) {
        closeMe();
    }

    function _phrases(id) {
        return $('#SleepModeBox .phrases [ph=' + id + ']').html();
    }

    obj.showPopup = _showPopup;

}(window.ROE.SleepMode = window.ROE.SleepMode || {}, jQuery));