/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />
/// <reference path="troops.3.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

// ensure ROE.Troops object exists
(function (obj, $, undefined) {
} (window.ROE.Troops = window.ROE.Troops || {}, jQuery));

// ensure ROE.Troops.InOutTroopsFilter object exists
(function (obj, $, undefined) {

    obj.init = function (element, toggleClickedCallback, options) {
        /// <summary> 
        /// element : where you wnat the UI to be appened to 
        /// toggleClickedCallback : pass in method if you want to be called when user clicks on any togles. signature is  "function (type, currentValue) "
        /// </summary>

        var settings = $.extend({
            showReturning: true
        }, options);



        var clicked = function (event) {
            var toggle = $(event.currentTarget);
            var curState = toggle.attr('data-state');
            var newState = curState == "on" ? "off" : "on"
            var newStateID = newState == "on" ? 1 : 0
            var type = toggle.attr('data-rel');
            state[type] = newStateID;
            toggle.toggleClass("on");
            toggle.attr('data-state', newState);
            if (toggleClickedCB) {
                toggleClickedCB(type, newStateID);
            }

            ROE.Frame.infoBar(_onOffPhrases(type, newState));
        }

        var container = element;
        var toggleClickedCB = toggleClickedCallback;
        var state = { attack: 1, support: 1, returning: 1, hidden: 0 };

        var template = $(BDA.Templates.getRaw("InOutTroopsFilter", ROE.realmID));

        if (!settings.showReturning) {
            template.find('.toggle.returning').remove();
        }

        container.append(template);


        container.find(".templ_inOutFilter .toggle").click(clicked);

        return {
            getState: function () {
                /// <summary>currentVale : 1 - toggle just been turned on, meaning show those commands | 0 - means the toggle has just been turned off
                /// </summary>
                return state;
            }
        };


        function _onOffPhrases(type, newState) {
            return $('.templ_inOutFilter .phrases [ph=%type%_%newState%]'.format({type:type, newState : newState})).html();
        }


    }

} (window.ROE.Troops.InOutTroopsFilter = window.ROE.Troops.InOutTroopsFilter || {}, jQuery));
