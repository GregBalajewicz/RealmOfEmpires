/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui.js"/>

(function (BDA) {
}(window.BDA = window.BDA || {}));

(function (UI) {
}(window.BDA.UI = window.BDA.UI || {}));

(function (Transition) {
    var CONST = {};

    CONST.Transition = {
        // When using CSS transitions, CSS properties that were just updated are
        // not correctly handled by transitions; a delay must be added for new
        // values to be accounted for.
        delay: 20 // ms
    };

    CONST.CssStyle = {
        transition: Modernizr.prefixed("transition")
    };

    CONST.DomEvent = {
        transition: {
            "WebkitTransition": "webkitTransitionEnd",
            "MozTransition": "transitionend",
            "OTransition": "oTransitionEnd otransitionend",
            "msTransition": "MSTransitionEnd",
            "transition": "transitionend webkitTransitionEnd" //NOTE having 2 names because on android 4.1, Modernizr.prefixed("transition") returns "transition" yet the event name is  webkitTransitionEnd. this is a hack for now
        }[CONST.CssStyle.transition]
    };

    CONST.CssClass = {
        transition: "transition",
        instantFrom: "instantFrom",
        instantTo: "instantTo",
        crossFadeFrom: "crossFadeFrom",
        crossFadeTo: "crossFadeTo",
        slideLeftFrom: "slideLeftFrom",
        slideLeftTo: "slideLeftTo",
        slideRightFrom: "slideRightFrom",
        slideRightTo: "slideRightTo",
        slideUpFrom: "slideUpFrom",
        slideUpTo: "slideUpTo",
        slideDownFrom: "slideDownFrom",
        slideDownTo: "slideDownTo"
    }

    CONST.CssClass.cssClass = [
        CONST.CssClass.transition,
        CONST.CssClass.instantFrom,
        CONST.CssClass.instantTo,
        CONST.CssClass.crossFadeFrom,
        CONST.CssClass.crossFadeTo,
        CONST.CssClass.slideLeftFrom,
        CONST.CssClass.slideLeftTo,
        CONST.CssClass.slideRightFrom,
        CONST.CssClass.slideRightTo,
        CONST.CssClass.slideUpFrom,
        CONST.CssClass.slideUpTo,
        CONST.CssClass.slideDownFrom,
        CONST.CssClass.slideDownTo
    ].join(" ");

    function instant(to, from, f) {
        /// <summary>
        /// Transitions from the specified element "from" to the specified element "to", instantly.
        /// </summary>
        /// <param name="to" type="jQuery">The element to transition to.</param>
        /// <param name="from" type="jQuery">The element to transition from.</param>
        /// <param name="f" type="function">[Optional] The function to call when the transition ends.</param>

        transition(to, CONST.CssClass.instantTo, from, CONST.CssClass.instantFrom, f);
    }

    function crossFade(to, from, f) {
        /// <summary>
        /// Transitions from the specified element "from" to the specified element "to", by cross-fading.
        /// </summary>
        /// <param name="to" type="jQuery">The element to transition to.</param>
        /// <param name="from" type="jQuery">The element to transition from.</param>
        /// <param name="f" type="function">[Optional] The function to call when the transition ends.</param>

        transition(to, CONST.CssClass.crossFadeTo, from, CONST.CssClass.crossFadeFrom, f);
    }
    
    function slideLeft(to, from, f) {
        /// <summary>
        /// Transitions from the specified element "from" to the specified element "to", by sliding left.
        /// </summary>
        /// <param name="to" type="jQuery">The element to transition to.</param>
        /// <param name="from" type="jQuery">The element to transition from.</param>
        /// <param name="f" type="function">[Optional] The function to call when the transition ends.</param>

        transition(to, CONST.CssClass.slideLeftTo, from, CONST.CssClass.slideLeftFrom, f);
    }

    function slideRight(to, from, f) {
        /// <summary>
        /// Transitions from the specified element "from" to the specified element "to", by sliding right.
        /// </summary>
        /// <param name="to" type="jQuery">The element to transition to.</param>
        /// <param name="from" type="jQuery">The element to transition from.</param>
        /// <param name="f" type="function">[Optional] The function to call when the transition ends.</param>

        transition(to, CONST.CssClass.slideRightTo, from, CONST.CssClass.slideRightFrom, f);
    }

    function slideUp(to, from, f) {
        /// <summary>
        /// Transitions from the specified element "from" to the specified element "to", by sliding up.
        /// </summary>
        /// <param name="to" type="jQuery">The element to transition to.</param>
        /// <param name="from" type="jQuery">The element to transition from.</param>
        /// <param name="f" type="function">[Optional] The function to call when the transition ends.</param>

        transition(to, CONST.CssClass.slideUpTo, from, CONST.CssClass.slideUpFrom, f);
    }

    function slideDown(to, from, f) {
        /// <summary>
        /// Transitions from the specified element "from" to the specified element "to", by sliding down.
        /// </summary>
        /// <param name="to" type="jQuery">The element to transition to.</param>
        /// <param name="from" type="jQuery">The element to transition from.</param>
        /// <param name="f" type="function">[Optional] The function to call when the transition ends.</param>

        transition(to, CONST.CssClass.slideDownTo, from, CONST.CssClass.slideDownFrom, f);
    }

    function transition(to, transitionTo, from, transitionFrom, f) {
        /// <summary>
        /// Transitions from the specified element "from" to the specified element "to", using the specified transition function "transitionTo" and the specified transition function "transitionFrom", respectively. If CSS transitions are not supported, the elements will be instantly swapped.
        /// </summary>
        /// <param name="to" type="jQuery">The element to transition to.</param>
        /// <param name="from" type="jQuery">The element to transition from.</param>
        /// <param name="transitionTo" type="function">The transition function to use for the specified element "to".</param>
        /// <param name="transitionFrom" type="function">The transition function to use for the specified element "from".</param>
        /// <param name="f" type="function">[Optional] The function to call when the transition ends.</param>

        from
            .removeClass(CONST.CssClass.cssClass)
            .addClass(transitionFrom || CONST.CssClass.slideLeftFrom);

        to
            .removeClass(CONST.CssClass.cssClass)
            .addClass(transitionTo || CONST.CssClass.slideLeftTo);

        if (Modernizr.csstransitions) {
            window.setTimeout(function () {
                var busy = 0;

                if ((transitionFrom != CONST.CssClass.instantFrom)
                    && (transitionFrom != CONST.CssClass.instantTo)) {
                    ++busy;
                    from.one(CONST.DomEvent.transition, function (event) {
                        --busy;
                        if (!busy) {
                            f && f();
                        }
                    });
                }

                if ((transitionTo != CONST.CssClass.instantFrom)
                    && (transitionTo != CONST.CssClass.instantTo)) {
                    ++busy;
                    to.one(CONST.DomEvent.transition, function (event) {
                        --busy;
                        if (!busy) {
                            f && f();
                        }
                    });
                }

                from.addClass(CONST.CssClass.transition);
                to.addClass(CONST.CssClass.transition);

                if (!busy) {
                    f && f();
                }

            }, CONST.Transition.delay);
        } else {
            from.addClass(CONST.CssClass.transition);
            to.addClass(CONST.CssClass.transition);

            f && f();
        }
    }

    Transition.instant = instant;

    Transition.crossFade = crossFade;

    Transition.slideLeft = slideLeft;
    Transition.slideRight = slideRight;
    Transition.slideUp = slideUp;
    Transition.slideDown = slideDown;
}(window.BDA.UI.Transition = window.BDA.UI.Transition || {}));
