/// <reference path="roe-utils.js" />

// ensure ROE object exists
(function (obj, $, undefined) {
}(window.BDA = window.BDA || {}, jQuery));

//
// left here for compatilibity. just an alias for BDA.Timer.initTimers()
//
function initTimers() {
    ///<summary>repreciated. use BDA.Timer.initTimers() instead</summary>
    BDA.Timer.initTimers();
}

var windowReloading = false; // making this global since anotehr function uses it

(function (obj, $, undefined) {
    var timers = new Array();
    var loopTimers = 1; // Set to one if the timers are to keep going.

    var _initTimers = function _initTimers() {
        // kill the stack
        timers = timers.slice(0, 0);

        var now = new Date();

        var o = 0;
        $('.countdown, .Countdown, #lblCountdown').each(
          function () {
              if ($(this).text().trim() != '') {
                  time = $(this).text().split(':', 3);
                  for (i = 0; i < 3; i++) { time[i] = parseInt(time[i], 10); }

                  finish = new Date();

                  finish.setHours(finish.getHours() + time[0]);
                  finish.setMinutes(finish.getMinutes() + time[1]);
                  finish.setSeconds(finish.getSeconds() + time[2]);

                  var left = finish - now;
                  var totalSecondsTemp;
                  totalSecondsTemp = left / 1000;


                  if ((finish - now) > 0) {
                      var name = 'timer' + o++;

                      if ($(this).attr('refresh') == "false")
                          timers.push({
                              'name': name,
                              'finish': finish,
                              refresh: false,
                              newFormating: $(this).attr('format') == 'long' ? true : false,
                              formating_percentage: $(this).attr('format') == 'percent' ? true : false,
                              redir: $(this).attr('redir') ? $(this).attr('redir') : null,
                              redirSilent: $(this).attr('redirSilent') ? $(this).attr('redirSilent') : null,
                              doneMessage: $(this).attr('doneMessage') ? $(this).attr('doneMessage') : null,
                              progressBar: $('div.jsCountDownProgressBar'),
                              totalSeconds: totalSecondsTemp
                          });
                      else
                          timers.push({
                              'name': name,
                              'finish': finish,
                              refresh: true,
                              newFormating: $(this).attr('id') == 'lblCountdown' ? true : false,
                              redir: $(this).attr('redir') ? $(this).attr('redir') : null,
                              functToCallOnRefresh: $(this).attr('data-refreshCall') ? $(this).attr('data-refreshCall') : "windowReload();"
                          });

                      $(this).attr('id', name).addClass('countdownReal');
                  } else if ((finish - now) > -4000) {
                      if ($(this).attr('refresh') != "false") {
                          $(this).text("Finished").addClass('CountdownDone');
                          setTimeout("$('.CountdownDone').text('Refreshing');", 2000);
                          var functToCallOnRefresh = $(this).attr('data-refreshCall');
                          setTimeout(functToCallOnRefresh ? functToCallOnRefresh : "windowReload();", 4000);
                      }
                  } else {
                      if ($(this).attr('refresh') == "false")
                          $(this).text('0:00:00');
                      else
                          $(this).empty().append('<a href="/" onclick="windowReload(); return false;">Overdue!</a>').addClass('help').attr('rel', 'jOverdue');
                  }
              }
          }
        );
        //DumperAlert(timers);

        // arrange the count down
        _timerTick();
    }
    var _timerTick = function _timerTick() {
        // Current time
        now = new Date();

        // Check each timer
        $(timers).each(
            function (i) {
                var left = this.finish - now;

                if (left < 0) {
                    // if the timer is finished, remove the id attr and reload the window
                    if ($("#" + this.name).length > 0) {
                        if (this.redir) {
                            $("#" + this.name).text("Redirecting..").attr('id', this.name + "Done").addClass('CountdownDone');
                            window.location = this.redir;
                        } else if (this.redirSilent) {
                            $("#" + this.name).text("finished!").attr('id', this.name + "Done").addClass('CountdownDone');
                            window.location = this.redirSilent;
                        } else if (this.doneMessage) {
                            $("#" + this.name).text(this.doneMessage).attr('id', this.name + "Done").addClass('CountdownDone');
                        } else if (this.refresh) {
                            $("#" + this.name).text("Finished").attr('id', this.name + "Done").addClass('CountdownDone');
                            setTimeout("$('.CountdownDone').text('Refreshing');", 2000);
                            setTimeout(this.functToCallOnRefresh, 4000);
                        } else {
                            $("#" + this.name).text("00:00:00").attr('id', this.name + "Done").addClass('CountdownDone');
                        }
                    }
                } else {
                    var totalSecondsLeft = 6000;
                    totalSecondsLeft = left / 1000;

                    hours = Math.floor(left / (1000 * 60 * 60));
                    left -= hours * (1000 * 60 * 60);

                    mins = Math.floor(left / (1000 * 60));
                    left -= mins * (1000 * 60);

                    secs = Math.floor(left / 1000);
                    left -= secs * 1000;

                    var percentageWidth = Math.ceil(((this.totalSeconds - totalSecondsLeft) / this.totalSeconds) * 100);
                    if (percentageWidth > 100) { percentageWidth = 100; }

                    if (this.formating_percentage) {
                        $("#" + this.name).text((percentageWidth) + "%");
                    }
                    else if (this.newFormating) {
                        $("#" + this.name).text(
                            (hours > 0 ? hours + ":" : "") +
                            (mins > 0 || hours > 0 ? pad(mins) + ":" : "") + pad(secs)
                            // hour and minute text removed, added leading zero for new design
                        );
                    } else {

                                              

                        $("#" + this.name).text(hours + ':' + localPadDigits(mins) + ':' + localPadDigits(secs));
                    }

                    if (this.progressBar) {
                        this.progressBar.css({ width: (percentageWidth) + "%" });
                    }

                }
            }
        );
        setTimeout(function () { _timerTick(); }, 100);
    }

    function pad(n) {
        return (n < 10) ? ("0" + n) : n;
    }

    function localPadDigits(num) {
        s = num.toString();
        if (s.length == 1) {
            return "0" + s;
        }
        return s;
    }

    obj.initTimers = _initTimers;

}(window.BDA.Timer = window.BDA.Timer || {}, jQuery));
