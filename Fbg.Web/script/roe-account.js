/* script for StatsPopup template */
(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {
    // holds the dom for stats template
    var _container;
    var _accountEmail;
   
    var CACHE = {};
    var CONST = {};
    var Api = window.ROE.Api,
       Frame = window.ROE.Frame;

    CACHE.uiTemplate = null;
    CONST.phrases = {
        SubmitResponseError: ""
        , SubmitResponseEmailTaken: ""
        , SubmitResponseOK: ""
    }

    obj.init = function (container) {       
        _container = container;
        _accountEmail = "";

        Frame.busy(undefined, undefined, _container);
        
        if (CACHE.uiTemplate) {
            // Ignore...
        } else {
            var temp;
            if (ROE.isMobile) {
                temp = BDA.Templates.getRawJQObj("ManageAccountTempl", ROE.realmID);
            } else {
                temp = BDA.Templates.getRawJQObj("ManageAccountTempl", ROE.realmID);
            }                       
            CACHE.uiTemplate = temp;
        }

        _container.empty().append(CACHE.uiTemplate);

        // Store the phrases.
        CONST.phrases.SubmitResponseError = ROE.Utils.phrase('AccountPhrases', 'SubmitResponseError');
        CONST.phrases.SubmitResponseEmailTaken = ROE.Utils.phrase('AccountPhrases', 'SubmitResponseEmailTaken');
        CONST.phrases.SubmitResponseOK = ROE.Utils.phrase('AccountPhrases', 'SubmitResponseOK');

        Frame.free(_container);

        // for device login, ask them to create tactica 
        if (ROE.Player.recEmail_isDeviceLogin == 1 &&  ROE.isDevice != ROE.Device.CONSTS.Amazon) {
            _switchPage(".registeredViaMobile");
        } else {
            // Pre-populate with existing email
            $("#accountRecoveryEmail", _container).val(ROE.Player.recEmail);

            // for tactica accounts, we do not allow changing email  
            if (ROE.Player.recEmail_isTacticaLogin == 1) {
                $("#accountRecoveryEmail", _container).prop("readonly", true);
            }
            _switchPage(".default");          
        }
   }

    // Call this to check if the provided email is valid.
    var _recoveryEmail_check = function _recoveryEmail_check() {
        _accountEmail = $("#accountRecoveryEmail", _container).val();

        if (ROE.Utils.isValidEmailFmt(_accountEmail)) {
            $('.errMsg', _container).hide();

            _recoveryEmail_confirm();
        } else {
            $('.errMsg', _container).show();
        }
    }

    // Call this to ask the user if they are absolutely sure this
    // is the email they want to us and is correct.
    var _recoveryEmail_confirm = function _recoveryEmail_confirm() {
        $('.currEmailAccount', _container).text(_accountEmail);

        // for tactica accounts, we do not allow changing email so just go right away to sending the confirmation email. 
        if (ROE.Player.recEmail_isTacticaLogin == 1) {
            _recoveryEmail_update(_accountEmail);
        }

        _switchPage('.confirmPage', '.default');
    }

    // If the email is not correct or user wants to cancel
    var _recoveryEmail_handleCancel = function _recoveryEmail_handleCancel() {
        _switchPage('.default');
    }

    // If the email is correct, ready to submit
    var _recoveryEmail_handleAccept = function _recoveryEmail_handleAccept() {
        //_switchPage('.default');

        _recoveryEmail_update(_accountEmail)
    }
    
    // Call this to actually update the email on the server. There will
    // be a response callback providing info on the status.
    var _recoveryEmail_update = function _recoveryEmail_update(email) {       
        Frame.busy(undefined, undefined, _container);

        ROE.Api.call("recoveryemail", { e: email }, _recoveryEmail_updateCallback);
    }

    // Called when the server responds regarding recovery email
    var _recoveryEmail_updateCallback = function _recoveryEmail_updateCallback(r) {       

        // Response: OK_nofailure OR FAIL_emailNotDifferentThanCurrent
        var response = CONST.phrases.SubmitResponseOK;
        // Check if FAIL_emailAlredyInUse
        if (r.v2_wasEmailUpdated_updateFailureReasonCode == "FAIL_emailAlredyInUse") {
            response = CONST.phrases.SubmitResponseEmailTaken;
        }        
                
        $('.responseMsg', _container).html(response);

        _switchPage('.responsePage');

        Frame.free(_container);
    }

    var _switchPage = function _switchPage(pageToOpen, pageToClose) {
        if (pageToClose) {
            $(pageToClose, _container).hide();
        } else {
            // Hide all if not specified.
            $('.accountPages', _container).hide();
        }
        $(pageToOpen, _container).show();
    }
    
    obj.recoveryEmail_check         = _recoveryEmail_check;
    obj.recoveryEmail_handleCancel  = _recoveryEmail_handleCancel;
    obj.recoveryEmail_handleBack    = _recoveryEmail_handleCancel;
    obj.recoveryEmail_handleAccept  = _recoveryEmail_handleAccept;

}(window.ROE.Account = window.ROE.Account || {}));