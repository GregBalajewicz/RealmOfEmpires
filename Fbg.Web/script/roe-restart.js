/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

(function (obj, $, undefined) {


    var _showPopup = function () {
        
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'Restart .popupBody').length > 0) {
            return;
        }

        popupModalOverlay('Restart', '', 0, 0);


        // this template is not preloaded on purpose since its used only once
        var temp = BDA.Templates.getRawJQObj('Restart', ROE.realmID);

        var popupContent = $('#' + ROE.Frame.CONSTS.popupNameIDPrefix + 'Restart .popupBody').append(temp);

        
        $('#Restart .restartCost').text(ROE.Player.restartCost);
        $('#Restart .playerServants').text(ROE.Player.credits);

        if (ROE.Player.restartCost > ROE.Player.credits) {
            $("#Restart .hireservant").css("visibility","visible");
        }
        //
        // bind events
        //        
        $('#popup_Restart .titleClose').click(_close);
        $('#Restart .restartOK').click(_restartClick);
        //
    }

    var _restartClick = function (e) {
        if (confirm(_phrases(1))) {
            ROE.Api.call_restart(_sync_onRestartSuccess);
        }
    }

    var _sync_onRestartSuccess = function (data) {
        if (data.NotEnoughServants) {
            $('#Restart .notEnoughServants').removeClass('ui-helper-hidden');
        }
        else if (data.noRespawns) {
            $('#Restart .noRespawns').removeClass('ui-helper-hidden');
        }
        else if (data.Restarted) {
            closeMe();
            window.location = data.Redirect;
        }

    }

    var _close = function (data) {
        closeMe();
    }

    function _phrases(id) {
        return $('#Restart .phrases [ph=' + id + ']').html();
    }

    obj.showPopup = _showPopup;

}(window.ROE.Restart = window.ROE.Restart || {}, jQuery));