/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui.js"/>

(function (BDA) {
}(window.BDA = window.BDA || {}));

(function (UI) {
}(window.BDA.UI = window.BDA.UI || {}));

(function (Touch) {
    if (!Modernizr.touch)
        return;

    var CONST = {};

    CONST.Threshold = {
        touchMove: 8,
        click: 24
    };

    var _touchMove;

    var _x0,
        _y0;

    var _fastClick;

    $(document).ready(function () {
        document.addEventListener("touchstart", onTouchStart, true);
        document.addEventListener("touchend", onTouchEnd, true);
        document.addEventListener("touchcancel", onTouchCancel, true);
        document.addEventListener("touchmove", onTouchMove, true);
        
        setFastClickNeeded();

    });

    // _fastClick is set to true if needed
    function setFastClickNeeded() {
        _fastClick = _checkIfFastClickNeeded();
    }

    function _checkIfFastClickNeeded() {
        var chromeVersion;
        var deviceIsAndroid;

        // Devices that don't support touch don't need fast click
        if (typeof window.ontouchstart === 'undefined') {
            return false;
        }
        // Chrome version - zero for other browsers
        chromeVersion = +(/Chrome\/([0-9]+)/.exec(navigator.userAgent) || [, 0])[1];

        if (chromeVersion) {
            // Check if device is Android
            deviceIsAndroid = navigator.userAgent.indexOf('Android') > 0;

            if (deviceIsAndroid) {
                metaViewport = document.querySelector('meta[name=viewport]');

                if (metaViewport) {
                    // Chrome on Android with user-scalable="no" doesn't need fast click
                    if (metaViewport.content.indexOf('user-scalable=no') !== -1) {
                        return false;
                    }

                    // Chrome 32 and above with width=device-width or less don't need fast click
                    if (chromeVersion > 31 && document.documentElement.scrollWidth <= window.outerWidth) {
                        return false;
                    }
                }

              
            } else {
                // Chrome desktop doesn't need fast click
                return false;
            }
        }

        return true;

    }
    
    function map(touchEvent, type) {
        /// <summary>
        /// Dispatches a MouseEvent event of the specified type "type", with properties mapped from the specified TouchEvent "touchEvent".
        /// </summary>
        /// <param name="touchEvent" type="TouchEvent">The TouchEvent to map.</param>
        /// <param name="type" type="String">The type of event to dispatch.</param>

        var touch;
        if (touchEvent.changedTouches) {
            // iOS

            if (!touchEvent.changedTouches.length) {
                return;
            }

            touch = touchEvent.changedTouches[0];
        } else {
            // Android

            if (!touchEvent.touches.length) {
                return;
            }

            touch = touchEvent.touches[0];
        }

        var mouseEvent = document.createEvent("MouseEvent");
        mouseEvent.initMouseEvent(
            type, // type
            true, // canBubble
            true, // cancelable
            window, // view
            1, // detail
            touch.screenX, // screenX
            touch.screenY, // screenY
            touch.clientX, // clientX
            touch.clientY, // clientY
            false, // ctrlKey
            false, // altKey
            false, // shiftKey
            false, // metaKey
            0, // button
            null // relatedTarget
        );

        touchEvent.target.dispatchEvent(mouseEvent);
    }

    function onTouchStart(event) {
        /// <summary>
        /// Maps the specified "TouchEvent" event to "mouseover", "mousemove", and "mousedown".
        /// </summary>
        /// <param name="event" type="TouchEvent">The TouchEvent.</param>
       
        if (event.touches.length > 1) {
            return;
        }

        _touchMove = false;

        _x0 = event.clientX;
        _y0 = event.clientY;

        map(event, "mouseover");
        map(event, "mousemove");
        map(event, "mousedown");
    }

    function onTouchEnd(event) {
        /// <summary>
        /// Maps the specified "TouchEvent" event to "mouseup", "mouseout", and "click".
        /// </summary>
        /// <param name="event" type="TouchEvent">The TouchEvent.</param>
        
        if (event.touches.length > 1) {
            return;
        }

        map(event, "mouseup");
        map(event, "mouseout");

        if (_touchMove)
            return;

        _x0 = event.clientX;
        _y0 = event.clientY;
        
        if (_fastClick) {
            window.setTimeout(function () {               
                document.removeEventListener("click", onClick, true);
                map(event, "click");
                document.addEventListener("click", onClick, true);
            }, 0);
        }
    }

    function onTouchCancel(event) {
        /// <summary>
        /// Treats the specified "TouchEvent" event as a "touchend" TouchEvent.
        /// </summary>
        /// <param name="event" type="TouchEvent">The TouchEvent.</param>
        
        onTouchEnd(event);
    }

    function onTouchMove(event) {
        /// <summary>
        /// Maps the specified "TouchEvent" event to "mousemove".
        /// </summary>
        /// <param name="event" type="TouchEvent">The TouchEvent.</param>
        
        if (event.touches.length > 1) {
            return;
        }

        /*
        // Threshold; possibly unnecessary.
        if (Math.sqrt( ((event.clientX - _x0) * (event.clientX - _x0)) + ((event.clientY - _y0) * (event.clientY - y0)) )
            <= CONST.Threshold.touchMove) {
            return;
        }
        */

        _touchMove = true;

        map(event, "mousemove");
    }

    function onClick(event) {
        /// <summary>
        /// Prevents the default behaviour for the specified MouseEvent "event".
        /// </summary>
        /// <param name="event" type="MouseEvent">The MouseEvent.</param>

        // https://developers.google.com/mobile/articles/fast_buttons

        /*
        // Threshold; possibly unnecessary.
        if (Math.sqrt( ((event.clientX - _x0) * (event.clientX - _x0)) + ((event.clientY - _y0) * (event.clientY - y0)) )
            > CONST.Threshold.click) {
            return;
        }
        */

        document.removeEventListener("click", onClick, true);

        event.preventDefault();
        event.stopPropagation();
    }
}(window.BDA.UI.Touch = window.BDA.UI.Touch || {}));
