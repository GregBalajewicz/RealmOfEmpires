// http://stackoverflow.com/questions/951791/javascript-global-error-handling
//  - http://dev.opera.com/articles/view/better-error-handling-with-window-onerror/
//  - http://msdn.microsoft.com/en-us/library/ms976144.aspx
var errorHandlingV = 4;
window.onerror = function (message, url, linenumber, nn, t) {
    try {
        var str = '';
        str += '<div style="border:1px solid black;">';
        str += '<BR><B>message:</b> ' + message;
        str += '<BR><B>url:</b> ' + url;
        str += '<BR><B>linenumber:</b> ' + linenumber;

        var ex = BDA.latestException;
        BDA.latestException = {};
        var errorInHtml = DisplayException(ex);
        var errorInJSON = $.toJSON({ message: message, url: url, linenumber: linenumber, ex: ex })

        // create an extended object to log
        var errorExtendedObj = { message: message, url: url, linenumber: linenumber, ex: ex };

        errorExtendedObj.userAgent = window.navigator.userAgent;
        errorExtendedObj.platform = window.navigator.platform;
        
        try {
            errorExtendedObj.PlayerID = ROE.playerID;
            errorExtendedObj.RealmID = ROE.realmID;
            errorExtendedObj.SVID = ROE.SVID;
            errorExtendedObj.Player_Name = ROE.Player.name;
            errorExtendedObj.PopupsOpened = $.map($('.IFrameDivTitle .fg .label'), function (n) { return $(n).text(); });
            errorExtendedObj.FrameView = ROE.Frame.CurrentView();
            errorExtendedObj.buildID = ROE.CurrentBuildID;
            errorExtendedObj.isD2 = ROE.isD2;
            errorExtendedObj.isM = ROE.isMobile;
            errorExtendedObj.isDevice = ROE.isDevice;
            errorExtendedObj.loginMode = ROE.loginMode;
            errorExtendedObj.dbv = ROE.localDBVersion;
        } catch (ex2) {
            errorExtendedObj.ERROR = "error while gathering extended info"
        }

        // cookies
        try {
            errorExtendedObj.Cookies = {};
            $.each(document.cookie.split(/; */), function () {
                var splitCookie = this.split('=');               
                errorExtendedObj.Cookies[splitCookie[0]] = splitCookie[1].substring(0, 50);
            });
        } catch (ex2) {
            errorExtendedObj.ERROR_COOKIES = "error while gathering cookies"
        }
        // local storage
        try {
            errorExtendedObj.LS = {};
            for (var i = 0; i < localStorage.length; i++) {
                errorExtendedObj.LS[localStorage.key(i)] = localStorage.getItem(localStorage.key(i)).substring(0, 50);
            }
        } catch (ex2) {
            errorExtendedObj.ERROR_LS = "error while gathering localstorage"
        }
        // entities 
        try {
            errorExtendedObj.ROE_Entities = (ROE.Entities) ? ROE.Entities.toString() : "undefined";
            errorExtendedObj.ROE_PFPckgs = (ROE.PFPckgs) ? ROE.PFPckgs.toString() : "undefined";
            errorExtendedObj.ROE_Avatars = (ROE.Avatars) ? ROE.Avatars.toString() : "undefined";
            errorExtendedObj.ROE_Titles = (ROE.Titles) ? ROE.Titles.length : "undefined";
            errorExtendedObj.ROE_MapIcons = (ROE.MapIcons) ? ROE.MapIcons.toString() : "undefined";
            errorExtendedObj.ROE_Build = (ROE.Build) ? ROE.Build.toString() : "undefined";
        } catch (ex2) {
            errorExtendedObj.ERROR_entities = "error while gathering entity info"
        }
        
        var errorExtendedInJSON = $.toJSON(errorExtendedObj)
        
        str += errorInHtml;
        str += '</div>';

        BDA.Console.error(errorInJSON);
        BDA.Console.error(str);
       

        // it would be better to do : 
        //  ErrorHandling_DispatchLogErrorToApi(errorExtendedObj);
        // then on API side, do :
        // json_serializer.Deserialize<List<Object>>(msg);
        // however, for compatibility, we do this
        ErrorHandling_DispatchLogErrorToApi({ errormsg: errorExtendedInJSON, count:1 });

        return BDA.CONSTS.trapErrors;
    } catch (ExceptionInErrorHandler) {
        //alert("oops! error in error handling code...well that's embarassing!");
        // silent error
        $('body').append('unhandled erorr. Please contact support. Message:' + ExceptionInErrorHandler.message);
    }
}

var ErrorHandling_timeout = null;
var ErrorHandling_errorQ = new Array();
var ErrorHandling_numErrorsSinceLastSend = 0;
function ErrorHandling_DispatchLogErrorToApi(err) {
    /// <summary>add error to Q and fired timer that will actually log it</summary>
    /// <param name="errorExtendedObj" type="Object"></param>
    ErrorHandling_numErrorsSinceLastSend++;
    if (ErrorHandling_errorQ.length > 0) {
        // if we got one error pending, check if this error is a repeat of the previous one.
        if (ErrorHandling_errorQ[ErrorHandling_errorQ.length - 1].errormsg === err.errormsg) {
            ErrorHandling_errorQ[ErrorHandling_errorQ.length - 1].count++;
            err = null;
        }
    }
    
    if (err) {
        ErrorHandling_errorQ.push(err);
    }
    if (ErrorHandling_timeout == null) {
        ErrorHandling_timeout = setTimeout(ErrorHandling_LogErrorToApi, ErrorHandling_GetSendRequency());
        ErrorHandling_numErrorsSinceLastSend = 0;
    }
}

function ErrorHandling_GetSendRequency() {
    if (ErrorHandling_numErrorsSinceLastSend < 10) {
        return 2000;
    } else if (ErrorHandling_numErrorsSinceLastSend < 100) {
        return 5000;
    } else {
        return 20000;
    }
    
}
function ErrorHandling_LogErrorToApi() {
    ROE.Api.logerror($.toJSON(ErrorHandling_errorQ));
   // ROE.Api.logerror(ErrorHandling_errorQ);
    
    ErrorHandling_timeout = null;
    ErrorHandling_errorQ = new Array();
}

function DisplayException(ex, level) {    
    var str='';
    var level = ( level ? level : 0 ) + 1;
    if (ex) {
        str += '<div style="border:1px solid black; margin-left:' + (level * 5) + 'px;">';
        if (ex instanceof BDA.Exception) {
            str += '<B>Message:</b> ' + ex.message;
            str += DisplayExceptionData(ex.data._data)
            str += DisplayException(ex.innerException, level);
        } else if (ex instanceof Error) {
            str += '<B>Message:</b> ' + ex.message;
            if (ex.type) { str += '<BR><B>type:</b> ' + ex.type; }
            if (ex.lineNumber) { str += '<BR><B>lineNumber:</b> ' + ex.lineNumber; }
            if (ex.line) { str += '<BR><B>line:</b> ' + ex.line; }
            if (ex.sourceId) { str += '<BR><B>sourceId:</b> ' + ex.sourceId; }
            if (ex.sourceURL) { str += '<BR><B>sourceURL:</b> ' + ex.sourceURL; }
            if (ex.arguments) {
                for (var i = 0, oneitem; oneitem = ex.arguments[i]; ++i) {
                    str += '<BR><B>arguments[' + i + ']:</b> ' + oneitem;
                }
            }
            if (ex.stack) { str += '<BR><B>stack:</b> <pre>' + ex.stack + '</pre>'; }
            if (!(ex.lineNumber || ex.line||ex.stack) ) {
                str += '<BR>No good info so printing out all of object properties<BR>';
                str += DisplayGenericObject(ex);
            }
        } else {

            str += DisplayGenericObject(ex);
        }
        str += '</div>';
    }
    return str;

}

function DisplayExceptionData(data) {
    var str = '';
    for (var i = 0, oneitem; oneitem = data[i]; ++i) {
        str += '<li><B>' + oneitem.name + ':</b> ' + (oneitem.val ? oneitem.val.toString() : 'undefined') + '</li>';
    }

    return str;
}

function DisplayGenericObject(object) {
    var str='';
    str += "<B>object.ToString()</B>: " + object.toString() ;
    for (item in object) {
        if (object.hasOwnProperty(item)) {
            str += "<BR><B>" + item + "</B>: " + object[item];
        }
    }

    return str;

}

var scriptErrors = 0;

function ScriptErrorHandle(e) {
    //if (scriptErrors < 2) {
    //    document.write($(e.target).clone()[0].outerHTML);
    //    scriptErrors++;
    //} else {
    //    window.location.href = "error2.aspx?scriptfail=" + e.srcElement.src;
    //}
}