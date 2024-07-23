/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />
/// <reference path="troops.3.js" />


(function (obj, $, undefined) {
} (window.TroopFuncts = window.TroopFuncts || {}, jQuery));


(function (obj, $, undefined) {
    var _rowClick = function (event) {
        // if A was not clicked, display options
        if (event.target.tagName !== "A" && TroopFuncts.Recall.isRecallingSome === false) {
            var tr = $(event.currentTarget);

            if (tr.find('.actionsPanel:visible').length > 0) {
                tr.parent().find(".ut").remove();
                tr.find('.actionsPanel').slideUp('fast');
                return;
            } if (tr.find('.actionsPanel').length > 0) {
                tr.parent().find('.actionsPanel:visible').slideUp('fast');
                tr.find('.actionsPanel').slideDown();
                tr.parent().find(".ut").remove();
            } else {
                tr.parent().find(".ut").remove();
                tr.parent().find('.actionsPanel').slideUp('fast');
            }

            var menu = $('<div class="actionsPanel"></div>');

            menu.append(tr.find("td.action a"));

            tr.find("td[uid]").each(function () {
                $(this).prepend('<div class="ut ut' + $(this).attr("uid") + '">');
            });

            tr.append(menu);


            menu.css({
                top: tr.position().top + tr.height(),
                left: tr.width() / 3
            })
                    .slideDown();


            event.preventDefault();
        }
    }


    $(document).ready
            (

                function () {

                    $('.jsTroopsSupporting, .jsTroopsAbroad').delegate('tbody tr', 'click', _rowClick);
                }

            );
} (window.TroopFuncts.Mob = window.TroopFuncts.Mob || {}, jQuery));

