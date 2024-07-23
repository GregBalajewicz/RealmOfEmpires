/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="bda-val.js" />

(function (obj, $, undefined) {

    obj.init = function (element, toggleClickedCallback, name, startingState) {
        /// <summary> 
        /// element : where you wnat the UI to be appened to 
        /// toggleClickedCallback : pass in method if you want to be called when user clicks on the check box. signature is  "function (currentValue)" where currentValue 
        //      is an INT of either 1 or 0 
        /// startingState :  1 or 0 (meaning : on or off) 
        /// </summary>
        BDA.Val.validate(element, "element empty", true, function (p) { return p.length > 0 });

        var clicked = function (event) {
            var toggle = $(event.currentTarget);
            var curState = toggle.attr('data-state');
            var newState = curState == "1" ? "0" : "1"
            toggle.toggleClass("on");
            toggle.attr('data-state', newState);
            if (toggleClickedCB) {
                toggleClickedCB(parseInt(newState, 10));
            }
        }

        var container = element;
        var toggleClickedCB = toggleClickedCallback;

        var template = $('<span class="bda-ui-checkmark clickable on" data-state="1"><img class=bda-ui-checkmarkimg src=https://static.realmofempires.com/images/check_small.png></img><span class=bda-ui-text>' + name + '</span></span>');
        if (!startingState) {
            template.removeClass("on");
            template.attr("data-state", "0");
        }

        template.click(clicked);

        container.append(template);

        return {
            getState: function () {
                /// <summary>returns int : 1 means its on, 0 means its off
                /// </summary>
                return parseInt(template.find(".bda-ui-checkmark").attr("data-state"), 10);
            },

            click: function () {
                template.click();
            }
        };
    }

} (window.BDA.UI.CheckBox = window.BDA.UI.CheckBox || {}, jQuery));
