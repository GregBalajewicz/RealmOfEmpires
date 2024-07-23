/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

(function (obj, $, undefined) {

    var _currentPurchaseCallback = null;
    var _amazonIAPInitialized = false;
    var _amazonUserID = null;

    var _initializeAmazon = function (callback) {
        amzn_wa.enableApiTester(amzn_wa_tester);
        if (amzn_wa.IAP == null) {
            BDA.Console.error('could not load amazon web api for in-app purchases');
        } else {
            amzn_wa.IAP.registerObserver({
                onSdkAvailable: function (response) {
                    BDA.Console.verbose('PFP', 'onSdkAvailable:' + response);
                    //if (response.isSandboxMode) {
                    //    // from Amazon IAP ButtonClicker test app:

                    //    // In a production application this should trigger either
                    //    // shutting down IAP functionality or redirecting to some
                    //    // page explaining that you should purchase this application
                    //    // from the Amazon Appstore.
                    //    //
                    //    // Not checking can leave your application in a state that
                    //    // is vulnerable to attacks.
                    //}
                    amzn_wa.IAP.getUserId();
                },
                //
                // set config_misc.config/AmazonRVSSandbox="true" to verify against local RVSSandbox
                //
                onPurchaseResponse: function (response) {
                    BDA.Console.verbose('PFP', 'onPurchaseResponse:' + response);
                    BDA.Console.verbose('PFP', JSON.stringify(response, null, 4));
                    //if (response.receipt) {
                    //} else {
                    //}
                    if (response.purchaseRequestStatus === amzn_wa.IAP.PurchaseStatus.SUCCESSFUL) {
                        _call_amazonPurchase(response.userId, response.receipt.purchaseToken, _currentPurchaseCallback);
                    }
                },
                onPurchaseUpdatesResponse: function (response) {
                    BDA.Console.verbose('PFP', 'onPurchaseUpdatesResponse:' + response);
                },
                onGetUserIdResponse: function (response) {
                    BDA.Console.verbose('PFP', 'onGetUserIdResponse:' + response);
                    _amazonUserID = response.userId;
                    _amazonIAPInitialized = true;
                    if (callback) {
                        callback();
                    }
                }
            });
            BDA.Console.verbose('PFP', 'amazon initialized');
        }
    };

    var _initialize = function () {
        switch (ROE.isDevice) {
            case ROE.Device.CONSTS.Amazon: return _initializeAmazon(null);
        }
    };
        
    $(_initialize);
        
    var _sendRoeRequest =  function (url) {
        var iframe = document.createElement('IFRAME');
        iframe.setAttribute('src', 'roe://' + url);
        document.documentElement.appendChild(iframe);
        iframe.parentNode.removeChild(iframe);
        iframe = null;
    }

    var _sendSFXRoeRequest = function (id) {
        // don't use ROE.Utils.sendRoeRequest() for SFX to ensure speed
        window.location = 'roe://playSFX?sfx=' + id;
    }

    var _sendSFXRoeRequest2 = function (id) {
        var iframe = document.createElement('IFRAME');
        iframe.setAttribute('src', 'roe://playSFX?sfx=' + id);
        document.documentElement.appendChild(iframe);
        iframe.parentNode.removeChild(iframe);
        iframe = null;
    }

    var _loggedOn = function () {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            try {
                ROEDroid.loggedOn2(ROE.deviceUseOnly);
            }
            catch (x) {
                ROEDroid.loggedOn();
            }
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('loggedon?d=' + ROE.deviceUseOnly);
        }
    }

    var _playMusic = function (play) {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            ROEDroid.playMusic(play);
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('playMusic?play=' + play);
        } else {
            localStorage.setItem('isMusicPlaying', play);

            if (play) {
                if (ROE.Audio && BetaFeatures.status('AudioTools') == 'ON') {
                    //ROE.Audio.oldHooksPlay('music1');
                    ROE.Audio.event('playMusic1');
                } else {
                ROE.UI.Sounds.playHTML5Music();
                }
                
            } else {
                if (ROE.Audio && BetaFeatures.status('AudioTools') == 'ON') {
                    //ROE.Audio.oldHooksStop('music1');
                    ROE.Audio.event('stopMusic1');
            } else {
                ROE.UI.Sounds.stopHTML5Music();
            }
                
        }



        }
    }

    var _isMusicPlaying = function () {
        var playing = false;
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            playing = ROEDroid.isMusicPlaying();
        } else {
            if (localStorage.isMusicPlaying != null) {
                return  localStorage.getItem('isMusicPlaying') === 'true';
            } else {
                localStorage.setItem('isMusicPlaying', false); //apparently localstorage cant store bool, it converts to strings?
                return false;
            }

        }
        return playing;
    }

    var _setMusicPlaying = function (callback, callbackstring) {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            callback(ROE.Device.isMusicPlaying());
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('setmusicplaying?js=' + callbackstring);
        } else {
            callback(ROE.Device.isMusicPlaying());
        }
    }

    var _setSFXOn = function (callback, callbackstring) {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            callback(ROE.Device.isSFXOn());
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('setsfxon?js=' + callbackstring);
        } else {
            callback(ROE.Device.isSFXOn());
        }
    }

    var _sfx = function (on) {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            ROEDroid.sfx(on);
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('sfx?on=' + on);
        } else {           
            localStorage.setItem('isSfxOn', on);
        }
    }

    var _isSFXOn = function () {
        var on = false;
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            on = ROEDroid.isSFXOn();
        } else {
            if (localStorage.isSfxOn != null) {
                return localStorage.getItem('isSfxOn');
            }
            else {
                localStorage.setItem('isSfxOn', false); //apparently localstorage cant store bool, it converts to strings?
                return false;
            }
            
        }
        return on;
    }

    var _playSFX = function (id, suppressedId) {
        if (typeof ROEDroid != 'undefined' && !suppressedId) {
            //if ((ROE.isDevice == ROE.Device.CONSTS.Android) && !_suppressedSFX[id]) {
            BDA.Console.verbose('sfx', 'sound#' + id);
            ROEDroid.playSFX(id);
        }
        else if ((ROE.isDevice == ROE.Device.CONSTS.iOS) && !suppressedId) {
            BDA.Console.verbose('sfx', 'sound#' + id);
            ROE.Device.sendSFXRoeRequest(id);
        } else {
            if (ROE.Audio && BetaFeatures.status('AudioTools') == 'ON') {
                ROE.Audio.event(id);
            } else {
            ROE.UI.Sounds.playHtml5SFX(id);
        }
            
        }
    }

    var _getAmazonUserID = function () {
        return _amazonUserID;
    };

    var _purchase = function (productID, callback, callbackstring) {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            ROEDroid.purchase(productID);
            callback();
        }
        // purchase productID and then call ROE.Frame.refreshHeader();
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('purchase?productid=' + productID + '&js=' + callbackstring);
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.Amazon) {
            BDA.Console.verbose('PFP', 'buy init :' + productID);
            // Amazon IAPs do not support separate callbacks on each purchase -- there
            // can only be one global purchase response callback. So, we put the callback
            // into a variable that gets called from the global purchase response callback,
            // meaning that there can only be one at a time. This shouldn't cause any
            // problems as the entire frame is set to busy whenever you try to purchase
            // anything. But it's something to keep in mind.
            if (_amazonIAPInitialized) {
                _currentPurchaseCallback = function () {
                    _currentPurchaseCallback = null;
                    callback();
                };
                BDA.Console.verbose('PFP', 'call amzn_wa.IAP.purchaseItem');
                amzn_wa.IAP.purchaseItem(productID);
                BDA.Console.verbose('PFP', 'call amzn_wa.IAP.purchaseItem DONE');
            } else {
                BDA.Console.error('amazon IAP interface not yet initialized');
            }
        } else if (!ROE.isDevice) {
            var amazonProductID = productID.replace('debug', 'amazon');
            alert('Simulating amazon purchase of ' + amazonProductID);
            var debugPurchaseTokens = {
                'com.realmofempires.debug.100servants': 'eyJ0eXBlIjoiQ09OU1VNQUJMRSIsInNrdSI6ImNvbS5yZWFsbW9mZW1waXJlcy5hbWF6b24uMTAwc2VydmFudHMifQ',
                'com.realmofempires.debug.1000servants': 'eyJ0eXBlIjoiQ09OU1VNQUJMRSIsInNrdSI6ImNvbS5yZWFsbW9mZW1waXJlcy5hbWF6b24uMTAwMHNlcnZhbnRzIn0'
            }
            var debugPurchaseUserId = 'Jacko';
            if (!(productID in debugPurchaseTokens)) {
                alert('error: no tokens generated for ' + amazonProductID + '. aborting.');
                return;
            }

            _call_amazonPurchase(debugPurchaseUserId, debugPurchaseTokens[productID], function () {
                alert('purchase complete.');
                callback();
            });
        }
    }


    var _showvideoad = function () {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
           
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('showrewardvideoad?uid=' + ROE.userID);
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.Amazon) {
           
        } else if (!ROE.isDevice) {
            
        }
    }

    var _rateApp = function () {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            ROEDroid.rateApp();
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            //ROE.Device.sendRoeRequest('rateapp');
            var appstoreurl = 'http://itunes.apple.com/app/id596477295';
            var iframe = document.createElement('IFRAME');
            iframe.setAttribute('src', appstoreurl);
            document.documentElement.appendChild(iframe);
            iframe.parentNode.removeChild(iframe);
            iframe = null;
        } else if (ROE.isDevice == ROE.Device.CONSTS.Amazon) {
            window.location.href = "amzn://apps/android?asin=B00GCMQNSS";
        }
    }

    var _getLatestApp = function () {
        _rateApp();
    }

    var _refresh = function () {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            ROEDroid.refresh();
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('refresh');
        }
    }

    var _reset = function () {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            ROEDroid.reset();
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('reset');
        }
    }

    var _actRec = function (email) {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {
            ROEDroid.actrec(encodeURIComponent(email));
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('actrec?eml=' + encodeURIComponent(email));
        }
    }

    var _getAppVersion = function () {           
        //e.g. ROEAPP=1.4       
        var matches = (/\bROEAPP=([1-9].[0-9]*)\b/i)
            .exec(navigator.userAgent);
        if (!matches) {
            appVer = 1.0;
        }
        else {
            appVer = matches[1];
        }
        return appVer;
    }

    var _supportsActRec = function () {
        var ver = parseFloat(_getAppVersion());
        return ver >= 1.4;
    }

    var apiLocationPrefix = '';

    var _call = function (functionName, param, suc, err, async, timeoutInMS, type) {
        ///<summary>Call any ROE REST API call</summary>
        /// <param name="functionName">name of the function. must be specififed</param>
        if (!functionName) { throw "functionName required"; }
        BDA.Console.verbose('roe.device', 'call ' + functionName + ' args[' + JSON.stringify(param) + ']');
        return ajax(ROE.Api.apiLocationPrefix + "deviceapi.aspx?fn=" + functionName, param,
            function (data) { // success function call back 
                BDA.Console.verbose('roe.device', 'call ' + functionName + '-returned'); BDA.Console.verbose('roe.device', data);
                suc(data);
            },
            function (jqXHR, textStatus, errorThrown) { // error function call back
                BDA.Console.error('roe.device', 'call %fn%-failed : %textStatus%, %errorThrown%'.format({ textStatus: textStatus, errorThrown: errorThrown, fn: functionName }));
                if (err) { err(jqXHR, textStatus, errorThrown) }
            } ,
            async, timeoutInMS, type);
    }

    var _logerror = function (jsonSerializedMessage) {
        $.ajax({
            url: ROE.Api.apiLocationPrefix + "api.aspx?fn=logE"
          , dataType: 'string'
          , data: { msg: jsonSerializedMessage }
          , success: function () { }
          , async: true
          , type: "POST"
        });
    }

    var _call_amazonPurchase = function (amazonId, purchaseToken, callback) { _call("amazpurch", { auid: amazonId, data: purchaseToken }, callback); }

    var _amazonStart_callback = function () {
        var d = new Date();
        var tzOffset = d.getTimezoneOffset() / 60;
        window.location = 'chooserealmmobile.aspx?uid=' + _amazonUserID + '&lt=mob&tz=' + tzOffset;
    }

    var _amazonStart = function () {
        _initializeAmazon(_amazonStart_callback);
    }


    var _checkVAA = function () {
        if (ROE.isDevice == ROE.Device.CONSTS.Android) {

        }
        else if (ROE.isDevice == ROE.Device.CONSTS.iOS) {
            ROE.Device.sendRoeRequest('IsVideoAdAvaialble?uid=' + ROE.userID);
        }
        else if (ROE.isDevice == ROE.Device.CONSTS.Amazon) {

        } else if (!ROE.isDevice) {

        }
    }

    obj.CONSTS = {};
    obj.CONSTS.Other = 0;
    obj.CONSTS.Android = 1;
    obj.CONSTS.iOS = 2;
    obj.CONSTS.BB = 3;
    obj.CONSTS.WP = 4;
    obj.CONSTS.Amazon = 5;
    obj.sendRoeRequest = _sendRoeRequest;
    obj.sendSFXRoeRequest = _sendSFXRoeRequest;
    obj.sendSFXRoeRequest2 = _sendSFXRoeRequest2;
    obj.loggedOn = _loggedOn;
    obj.playMusic = _playMusic;
    obj.isMusicPlaying = _isMusicPlaying;
    obj.setMusicPlaying = _setMusicPlaying;
    obj.setSFXOn = _setSFXOn;
    obj.sfx = _sfx;
    obj.isSFXOn = _isSFXOn;
    obj.playSFX = _playSFX;
    obj.purchase = _purchase;
    obj.rateApp = _rateApp;
    obj.refresh = _refresh;
    obj.reset = _reset;
    obj.actRec = _actRec;
    obj.supportsActRec = _supportsActRec;
    obj.getAppVersion = _getAppVersion;
    obj.getLatestApp = _getLatestApp;
    obj.call_amazonPurchase = _call_amazonPurchase;
    obj.amazonStart = _amazonStart;
    obj.getAmazonUserId = _getAmazonUserID;
    obj.showVideoAd = _showvideoad;
    obj.checkVAA = _checkVAA;

}(window.ROE.Device = window.ROE.Device || {}, jQuery));