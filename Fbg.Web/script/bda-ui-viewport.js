/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui.js"/>

(function (BDA) {
}(window.BDA = window.BDA || {}));

(function (UI) {
}(window.BDA.UI = window.BDA.UI || {}));

(function (Viewport) {
    var CONST = {},
        CACHE = {};

    CONST.Id = {
        blockMouse: "blockMouse"
    };

    CACHE.Selected = {
        blockMouse: null
    };

    var _loaded;

    $(document).ready(function () {
        CACHE.Selected.blockMouse = $('<div></div>')
            .attr("id", CONST.Id.blockMouse)
            .hide()
            .appendTo("body");

        $(window).one("load", function (event) {
            _loaded = true;
        });
    });

    function centerToViewportX(o) {
        /// <summary>
        /// Attempts to center the specified element "o" along the viewport's x-axis, while keeping the element's bounds within the viewport's bounds. Uses the Facebook JavaScript API if the viewport is contained within a Facebook Canvas page.
        /// </summary>
        /// <param name="o" type="jQuery">The element to center.</param>

        if (FB && FB.Canvas) {
            FB.Canvas.getPageInfo(function (fb) {
                _centerToViewportX(o, fb.scrollLeft, fb.clientWidth);
            });
        }
        else {
            _centerToViewportX(o, $(window).scrollLeft(), $(window).width());
        }
    }

    function centerToViewportY(o) {
        /// <summary>
        /// Attempts to center the specified element "o" along the viewport's y-axis, while keeping the element's bounds within the viewport's bounds. Uses the Facebook JavaScript API if the viewport is contained within a Facebook Canvas page.
        /// </summary>
        /// <param name="o" type="jQuery">The element to center.</param>

        if (FB && FB.Canvas) {
            FB.Canvas.getPageInfo(function (fb) {
                _centerToViewportY(o, fb.scrollTop, fb.clientHeight);
            });
        }
        else {
            _centerToViewportY(o, $(window).scrollTop(), $(window).height());
        }
    }

    function centerToViewport(o) {
        /// <summary>
        /// Attempts to center the specified element "o" to the viewport, while keeping the element's bounds within the viewport's bounds. Uses the Facebook JavaScript API if the viewport is contained within a Facebook Canvas page.
        /// </summary>
        /// <param name="o" type="jQuery">The element to center.</param>

        if (FB && FB.Canvas) {
            FB.Canvas.getPageInfo(function (fb) {
                _centerToViewport(o, fb.scrollLeft, fb.clientWidth, fb.scrollTop, fb.clientHeight);
            });
        }
        else {
            _centerToViewport(o, $(window).scrollLeft(), $(window).width(), $(window).scrollTop(), $(window).height());
        }
    }

    function _centerToViewportX(o, viewportX, viewportW) {
        /// <summary>
        /// Attempts to center the specified element "o" along the viewport's x-axis, while keeping the element's bounds within the viewport's bounds.
        /// </summary>
        /// <param name="o" type="jQuery">The element to center.</param>
        /// <param name="viewportX" type="Number">The viewport's scroll offset along the x-axis.</param>
        /// <param name="viewportW" type="Number">The viewport's width.</param>

        var w = o.width();
        
        var minX = 0,
            maxX = $(document).width() - w,
            x = viewportX + ((viewportW - w) * 0.5);

        o.offset({
            left: Math.max(minX, Math.min(x, maxX)),
            top: o.offset().top
        });
    }

    function _centerToViewportY(o, viewportY, viewportH) {
        /// <summary>
        /// Attempts to center the specified element "o" along the viewport's y-axis, while keeping the element's bounds within the viewport's bounds.
        /// </summary>
        /// <param name="o" type="jQuery">The element to center.</param>
        /// <param name="viewportY" type="Number">The viewport's scroll offset along the y-axis.</param>
        /// <param name="viewportH" type="Number">The viewport's height.</param>

        var h = o.height();

        var minY = 0,
            maxY = $(document).height() - h,
            y = viewportY + ((viewportH - h) * 0.5);

        o.offset({
            left: o.offset().left,
            top: Math.max(minY, Math.min(y, maxY))
        });
    }

    function _centerToViewport(o, viewportX, viewportW, viewportY, viewportH) {
        /// <summary>
        /// Attempts to center the specified element "o" to the viewport, while keeping the element's bounds within the viewport's bounds.
        /// </summary>
        /// <param name="o" type="jQuery">The element to center.</param>
        /// <param name="viewportX" type="Number">The viewport's scroll offset along the x-axis.</param>
        /// <param name="viewportW" type="Number">The viewport's width.</param>
        /// <param name="viewportY" type="Number">The viewport's scroll offset along the y-axis.</param>
        /// <param name="viewportH" type="Number">The viewport's height.</param>

        var w = o.width(),
            h = o.height();

        var minX = 0,
            maxX = $(document).width() - w,
            x = viewportX + ((viewportW - w) * 0.5),
            minY = 0,
            maxY = $(document).height() - h,
            y = viewportY + ((viewportH - h) * 0.5);

        o.offset({
            left: Math.max(minX, Math.min(x, maxX)),
            top: Math.max(minY, Math.min(y, maxY))
        });
    }

    function isLandscape() {
        /// <summary>
        /// Gets whether the viewport is considered landscape.
        /// </summary>
        /// <returns type="Boolean">True if the viewport's orientation or dimensions are landscape, false otherwise.</returns>
        
        return (window.orientation != undefined)
            ? ((window.orientation == -90) || (window.orientation == 90))
            : ($(window).width() > $(window).height());
    }

    function isPortrait() {
        /// <summary>
        /// Gets whether the viewport is considered portrait.
        /// </summary>
        /// <returns type="Boolean">True if the viewport's orientation or dimensions are portrait, false otherwise.</returns>

        return (window.orientation != undefined)
            ? ((window.orientation == 0) || (window.orientation == 180))
            : ($(window).width() < $(window).height());
    }

    function hideChrome() {
        /// <summary>
        /// Attempts to hide the browser's chrome/UI, (e.g.: the address bar on mobile).
        /// </summary>
        
        var doc = $(document),
            win = $(window);

        function f() {
            if (window.location.hash) {
                return;
            }

            var y = (navigator.platform == "iPod" || navigator.platform == "iPhone" || navigator.platform == "iPad")
                ? 60
                : 56;

            var h = win.height() + y;

            if (doc.height() < h) {
                $("body").height(h);
            }

            window.setTimeout(function () {
                win.scrollTop(1);
            }, 20);
        }

        if (!_loaded) {
            win.one("load", function (event) {
                if (win.scrollTop()) {
                    return;
                }

                f();
            });
        }
        else {
            f();
        }

        win.bind("orientationchange", function (event) {
            f();
        });
    }

    function blockMouse() {
        /// <summary>
        /// Causes the viewport to block mouse events for elements contained within the documentElement.
        /// </summary>

        CACHE.Selected.blockMouse.show();
    }
    
    function allowMouse() {
        /// <summary>
        /// Causes the viewport to allow mouse events for elements contained within the documentElement.
        /// </summary>

        CACHE.Selected.blockMouse.hide();
    }

    Viewport.centerToViewportX = centerToViewportX;
    Viewport.centerToViewportY = centerToViewportY;
    Viewport.centerToViewport = centerToViewport;

    Viewport.isLandscape = isLandscape;
    Viewport.isPortrait = isPortrait;
    
    Viewport.hideChrome = hideChrome;
    
    Viewport.blockMouse = blockMouse;
    Viewport.allowMouse = allowMouse;
}(window.BDA.UI.Viewport = window.BDA.UI.Viewport || {}));
