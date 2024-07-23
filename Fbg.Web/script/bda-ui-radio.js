/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="bda-val.js" />

(function (obj, $, undefined) {

    obj.init = function (element, radioSelectionChanged, names, checkedIndex, options) {
        /// <summary> 
        /// element : where you wnat the UI to be appened to 
        /// radioSelectionChanged : pass in method if you want to be called when user clicks and changed the selected radio button. signature is  "function (chcekedIndex)" 
        /// checkedIndex : index of the radio that shoudl be selected at start
        /// </summary>
        BDA.Val.validate(element, "element empty", true, function (p) { return p.length > 0 });

        var settings = $.extend({
            'orientation': 'horizontal'
        }, options);


        var clicked = function (event) {
            var toggle = $(event.currentTarget);
            if (!toggle.hasClass('on')) {
                container.find('.bda-ui-radio').removeClass("on");
                toggle.addClass("on");
                if (radioSelectionChanged) {
                    radioSelectionChanged(parseInt(toggle.attr("data-index"), 10));
                }
            }
        }

        var container = element;
        var state = 0;
        var radioGroup = $('<span class="bda-ui-radiogroup"></span>');
        var template = $('<span class="bda-ui-radio clickable" data-index="0"><img class=bda-ui-radioimg src=https://static.realmofempires.com/images/misc/RadioBoxCheck.png></img><span class="bda-ui-text"></span></span>');
        if (settings.orientation === 'horizontal') {
            template.addClass("horizontal");
        }
        var name;
        var oneRadio;

        for (var i = 0, name; name = names[i]; ++i) {
            oneRadio = template.clone();
            oneRadio.attr("data-index", i);
            oneRadio.find('.bda-ui-text').text(name);
            if (checkedIndex === i) {
                oneRadio.addClass("on");
            }

            radioGroup.append(oneRadio);
        }

        radioGroup.delegate(".bda-ui-radio", "click", clicked);

        container.append(radioGroup);

        return {
            getCheckedIndex: function () {
                /// <summary>returns the index of the option that is checked
                /// NOTE - this function was never tested!!
                /// </summary>
                if (radioGroup.find(".bda-ui-radio.on").length > 0) {
                    return parseInt(radioGroup.find(".bda-ui-radio.on").attr("data-index"), 10);
                }
            },

            checkItem: function (index) {
                /// <summary>makes the radio button identified by index checked, rest unchecked
                /// NOTE - this function was never tested!!
                /// </summary>
                radioGroup.find('.bda-ui-radio').removeClass("on");
                radioGroup.find('.bda-ui-radio').find('[data-index=' + index + ']').addClass("on");
            }
        };
    }

} (window.BDA.UI.Radio = window.BDA.UI.Radio || {}, jQuery));
