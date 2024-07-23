/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

// ensure ROE.UI object exists
(function (obj, $, undefined) {
} (window.ROE.UI = window.ROE.UI || {}, jQuery));

// ensure ROE.UI.GovTypeSelect object exists

//GovTypeSelect -> shoudl be GovTypeSelect
(function (obj, $, undefined) {

    var _init = function () {
        //todo : put this in some common function because we do this a lot
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'gov .popupBody').length > 0) {
            return;
        }

        if (ROE.isMobile) {
            popupModalOverlay("gov", "", 140, 5);
        }else {
            popupModalOverlay("gov", "", 140, 30);
        }
        
        var temp = BDA.Templates.getRawJQObj("GovSelection", ROE.realmID); // this template is not preloaded on purpose since its used only once

        //todo : put this in some common function because we do this a lot
        var popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'gov .popupBody').append(temp);
        //
        // bind events
        //
        $("#GovSelection .govType").click(_govTypeClick);
        $("#GovSelection .popupInfo_close").click(_closeclick);        
        $("#GovSelection .acceptButton").click(_acceptButtonClicked);
        $("#GovSelection .closeX").click( _closePopup);
        
    }

    var _init_DisplayOnly = function () {
        //todo : put this in some common function because we do this a lot
        if ($("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'gov .popupBody').length > 0) {
            return;
        }

        if (ROE.isMobile) {
            popupModalOverlay("gov", "", 140, 5);
        } else {
            popupModalOverlay("gov", "", 140, 30);
        }

        var temp = BDA.Templates.getRawJQObj("GovSelection", ROE.realmID); // this template is not preloaded on purpose since its used only once

        //todo : put this in some common function because we do this a lot
        var popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'gov .popupBody').append(temp);
        //
        // bind events
        //
        $("#GovSelection .govType").click(_govTypeClick);
        $("#GovSelection .popupInfo_close").click(_closeclick);
        $("#GovSelection .closeX").click(_closePopup);

        //
        // display only
        //
        $("#GovSelection .acceptButton").remove();
        $("#GovSelection .title").hide();
        $("#GovSelection .title.ViewOnly").show();


    }

    var _closePopup = function () {
        if (ROE.isMobile || ROE.isD2) { //no sounds in D1, it'll break
            ROE.UI.Sounds.clickMenuExit();
        }
        $('.selectGovType').hide(); // this is code for mobile only but it will do no harm in desktop
        closeMe();
    }
    

    var _acceptButtonClicked = function () {
        var govTypeID;

        if (confirm('Are you sure? It is not possible to change your Government Type later on in the game')) {
            govTypeID = $('#GovSelection .govType.selected').attr('data-govtype');
                
            // sanity check - if no gov type id, display message
            if (!govTypeID) {
                $('.phrases > div').hide();
                $('.phrases [ph=2]').fadeIn();
                return;
            }

            // display "saving" message
            $('.phrases > div').hide();
            $('.phrases [ph=1]').show();

            ROE.Api.call("GovTypeSelect", { 'govTypeID': govTypeID }, _callBack);
        }
    }

    var _closeclick = function (e) {
        $('#GovSelection .govType').removeClass('selected');
        e.stopPropagation();
    }

    var _govTypeClick = function (e) {
        if (!$(e.currentTarget).hasClass('selected')) {
            $('#GovSelection .govType').removeClass('selected');
            $(e.currentTarget).addClass('selected');
            $("#GovSelection .acceptButton").show().fadeIn();
        } else {
            $('#GovSelection .govType').removeClass('selected');

        }
        

    }
    var _callBack = function (data) {
        _closePopup();
    }
    //
    // interface
    //
    obj.init = _init;
    obj.init_DisplayOnly = _init_DisplayOnly;

}(window.ROE.UI.GovTypeSelect = window.ROE.UI.GovTypeSelect || {}, jQuery));