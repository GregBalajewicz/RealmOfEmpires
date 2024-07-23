

//
// Do not use this. use getProperSupportAnchor
//
ROE.getSupportAnchor = function (vID, cordX, cordY, ownerPlayerID, bLocateNearLink, helpArea) {
    if (!ROE.isSpecialPlayer_Rebel(ownerPlayerID)) {


        var theAnchor = "<a href=# class='sup' onclick='ROE.UI.Sounds.click(); ROE.Frame.popupSupport(" + vID + "," + cordX + "," + cordY + ")' ></a>";

        theAnchor = $(theAnchor);
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Support this village');
        return theAnchor;


    }
    else {
        return $('<div>').addClass('na').addClass('sup_na');
    }
};

// NEW SUPPORT
ROE.getProperSupportAnchor = function (vID, cordX, cordY, ownerPlayerID, bLocateNearLink, helpArea) {
    if (ROE.playersNumVillages > 1) {
        return ROE.getSupportAnchor_OneClick(vID, cordX, cordY, ownerPlayerID, bLocateNearLink, helpArea);
    } else {
        return ROE.getSupportAnchor(vID, cordX, cordY, ownerPlayerID, bLocateNearLink, helpArea);
    }

};


 
//
// get proper attack anchor
//
ROE.getProperAttackAnchor = function (vID, cordX, cordY, bLocateNearLink, helpArea) {
    if (ROE.playersNumVillages > 1) {
        return ROE.getAttackAnchor_OneClick(vID, cordX, cordY, bLocateNearLink, helpArea);
    } else {
        return ROE.getAttackAnchor(vID, cordX, cordY, bLocateNearLink, helpArea);
    }
};


//
// Do not use this. use getProperAttackAnchor
//
ROE.getAttackAnchor = function (vID, cordX, cordY, bLocateNearLink, helpArea) {
    var theAnchor = "<a href=# class='att' onclick='ROE.UI.Sounds.click();ROE.Frame.popupAttacks(" + vID + "," + cordX + "," + cordY + ")' ></a>";

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Attack this village');
    return theAnchor;
};
//
// get proper one click support anchor
//
ROE.getSupportAnchor_OneClick = function (vID, cordX, cordY, ownerPlayerID, bLocateNearLink, helpArea) {
    //enabling support icon for single village players, as a quest depends on it for new players -farhad jan 19 2015
    //if(ROE.playersNumVillages > 1 && !ROE.isSpecialPlayer_Rebel(ownerPlayerID)) {
    if (!ROE.isSpecialPlayer_Rebel(ownerPlayerID)) {

        var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Support - Quick')
                , ''
                    , {
                        'onclick': "ROE.UI.Sounds.click(); return ROE.Frame.popupQuickCommand(%AttackType%, %TARGETVID%, %COORX%, %COORY%)".format({ AttackType: 0, TARGETVID: vID, COORX: cordX, COORY: cordY })

                    , 'class': 'sup sfx2', 'target': '_blank'
                    });

        theAnchor = $(theAnchor);
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Support this village');
        return theAnchor;
    }
    else {
        return $('<div>').addClass('na').addClass('sup_na');
    }
};
//
// get proper one click attack anchor
//
ROE.getAttackAnchor_OneClick = function (vID, cordX, cordY, bLocateNearLink, helpArea) {

    var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Attack - Quick')
            , ''
                , {
                    'onclick': "ROE.UI.Sounds.click(); return ROE.Frame.popupQuickCommand(%AttackType%, %TARGETVID%, %COORX%, %COORY%)".format({ AttackType: 1, TARGETVID: vID, COORX: cordX, COORY: cordY })
                , 'class': 'att sfx2', 'target': '_blank'
                });
    theAnchor = $(theAnchor);

    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Attack this village');
    return theAnchor;

};

ROE.getIncomingToVillageAnchor = function (villageID, villageName, villageX, villageY, ownerPlayerID, helpArea) {
    /// <summary>shows my troops incoming (aka outgoing) to this village </summary>
    /// <param name="vID" type="Object"></param>
    /// <param name="ownerPlayerID" type="Object"></param>

    var theAnchor;
    if (ownerPlayerID !== ROE.playerID) {
        theAnchor = "<a class='toVillage sfx2' onclick='ROE.UI.Sounds.click(); RoeObjectClick.handleInOutFilteredPopupClick(this)' data-direction='out' data-VID='%vid%' data-VName='%vn%' data-Vx='%x%' data-Vy='%y%' data-pid='%ownerPlayerID%'></a>".format({ vid: villageID, vn: villageName, x: villageX, y: villageY, ownerPlayerID: ownerPlayerID });
    } else {
        theAnchor = "<a class='toVillage sfx2' onclick='ROE.UI.Sounds.click(); RoeObjectClick.handleInOutFilteredPopupClick(this)' data-direction='in' data-VID='%vid%' data-VName='%vn%' data-Vx='%x%' data-Vy='%y%' data-pid='%ownerPlayerID%'></a>".format({ vid: villageID, vn: villageName, x: villageX, y: villageY, ownerPlayerID: ownerPlayerID });
    }

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Incoming to this village');
    return theAnchor;
}

ROE.getOutgoingFromVillageAnchor = function (villageID, villageName, villageX, villageY, ownerPlayerID, helpArea) {
    /// <summary>show troops incoming at me from this village</summary>
    /// <param name="vID" type="Object"></param>
    /// <param name="ownerPlayerID" type="Object"></param>

    var theAnchor;
    if (!ROE.isSpecialPlayer_Rebel(ownerPlayerID)) {
        if (ownerPlayerID !== ROE.playerID) {
            theAnchor = "<a class='fromVillage sfx2' onclick='ROE.UI.Sounds.click(); RoeObjectClick.handleInOutFilteredPopupClick(this)' data-direction='in' data-VID='%vid%' data-VName='%vn%' data-Vx='%x%' data-Vy='%y%' data-pid='%ownerPlayerID%'></a>".format({ vid: villageID, vn: villageName, x: villageX, y: villageY, ownerPlayerID: ownerPlayerID });
        } else {
            theAnchor = "<a class='fromVillage sfx2' onclick='ROE.UI.Sounds.click(); RoeObjectClick.handleInOutFilteredPopupClick(this)' data-direction='out' data-VID='%vid%' data-VName='%vn%' data-Vx='%x%' data-Vy='%y%' data-pid='%ownerPlayerID%'></a>".format({ vid: villageID, vn: villageName, x: villageX, y: villageY, ownerPlayerID: ownerPlayerID });
        }
    }
    else {
        theAnchor = '<a class="fromVillage notAvailable"></a>';
    }

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Outgoing from this village');
    return theAnchor;
}

ROE.getIncomingToPlayerAnchor = function (ownerPlayerID, ownerPlayerIDName) {
    /// <summary>show my troops incoming (aka outgoing) to this player, to any of their villages</summary>
    /// <param name="ownerPlayerID" type="Object"></param>

    if (!ROE.isSpecialPlayer_Rebel(ownerPlayerID) && ownerPlayerID !== ROE.playerID) {
        var theAnchor = "<a class='toPlayer sfx2' onclick='ROE.UI.Sounds.click(); RoeObjectClick.handleInOutFilteredPopupClick(this)' data-direction='out' data-PID='%id%' data-pname='%name%'></a>".format({ id: ownerPlayerID, name: ownerPlayerIDName });

        theAnchor = $(theAnchor);
        return theAnchor;
    }
    else {
        return $('<a class="toPlayer notAvailable" ></a>');
    }
}

ROE.getOutgoingFromPlayerAnchor = function (ownerPlayerID, ownerPlayerIDName) {
    /// <summary>show troops incoming at me from this player</summary>
    /// <param name="ownerPlayerID" type="Object"></param>

    if (!ROE.isSpecialPlayer_Rebel(ownerPlayerID) && ownerPlayerID !== ROE.playerID) {
        var theAnchor = "<a class='fromPlayer sfx2' onclick='ROE.UI.Sounds.click(); RoeObjectClick.handleInOutFilteredPopupClick(this)' data-direction='in' data-PID='%id%' data-pname='%name%'></a>".format({ id: ownerPlayerID, name: ownerPlayerIDName });

        theAnchor = $(theAnchor);
        return theAnchor;
    }
    else {
        return $('<a class="fromPlayer notAvailable"></a>');
    }
}

//
// get Support anchor
//
ROE.getSupportLookupAnchor = function (vID, isMine, bLocateNearLink, helpArea, ownerPlayerID) {
    //after allowing support sending with one village, there is need for withdrawing support easily -farhad jan 20 2015
    //if (ROE.playersNumVillages > 1 && !ROE.isSpecialPlayer_Rebel(ownerPlayerID)) {   
   
    bLocateNearLink = bLocateNearLink || false;
    var url;
    var theAnchor
        
    if (isMine) {
        url = BuildURL('UnitsSupportingPopup.aspx', ['vid=' + vID.toString()]);
    }
    else {
        url = BuildURL('UnitsAbroadPopupAt.aspx', ['sdvid=' + vID.toString()]);
    }

    theAnchor = $('<a>').addClass('supl').attr('href', '#').click(function (e) {
        ROE.UI.Sounds.click();
        ROE.Frame.showIframeOpenDialog($('#supportLookupDialog'), url, 'Support Lookup');
        /*
        e.preventDefault();
        $('#supportLookupDialog').html('<div style="position: absolute; left: 0px; top: 0px; right: 0px; bottom: 0px; width: auto; height: auto; overflow:hidden;">' +
        '<iframe id="supportLookupDialogIframe" border="0"  style="height:100%; width:100%; border:none;" src="' + url + '" frameborder="0"></iframe>' +
        '</div>').dialog('open');
        return false;
        */
    });

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Support and Troops Abroad');
    return theAnchor;
   
};
//
// get proper QR anchor
//

ROE.getRecruitAnchor = function (vID, isMine, bLocateNearLink, helpArea) {
    if (isMine && ROE.playersNumVillages > 1) {
        var theAnchor = $('<a>').addClass('sfx2 recr').attr('href', '#').attr('onclick', 'ROE.UI.Sounds.click(); return ROE.QuickRecruit.showPopup("map","' + vID + '");');
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Recruit or disband troops');
        return theAnchor;
    }
    else {
        return $('<div>').addClass('na').addClass('recr_na');
    }
};

//
// get proper map it anchor
//
ROE.getSendSilverAnchor = function (vID, isMine, cordX, cordY, ownerPlayerID, helpArea, bLocateNearLink, locX, locY) {
    /// <param name="locX" >optiona</param>
    /// <param name="locY" > optional </param>  
    if (!ROE.isSpecialPlayer(ownerPlayerID)) {
        var theAnchor = $('<a>').addClass('sfx2 sends').attr('href', '#')
        .attr('onclick', 'ROE.UI.Sounds.click(); return ROE.Frame.showSilverTransportPopup(' + vID.toString() + "," + isMine + ');');
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Send silver to this village');
            return theAnchor;
    } else {
        return $('<div>').addClass('na').addClass('sends_na');
    }
};


//
// get proper map it anchor
//
ROE.getMapItAnchor = function (cordX, cordY, helpArea) {
    var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Map-It'), '#', { 'class': 'map sfx2', 'onclick': 'ROE.Frame.switchToMap(%x%, %y%)'.format({x:cordX, y:cordY}) });

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Center map on this village');
    return theAnchor;
};

ROE.getHQAnchor = function (vID, isMine, bLocateNearLink, helpArea) {
    if (isMine && ROE.playersNumVillages > 1) {
     
        var theAnchor = $('<a>').addClass('sfx2 hq').attr('href', '#').attr('onclick', 'ROE.UI.Sounds.click(); return ROE.QuickBuild.showPopup("map","' + vID + '");');
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Upgrade buildings at this village');
        return theAnchor;

    } else {
        return $('<div>').addClass('na').addClass('hq_na');
    }
};

ROE.getGiftsAnchor = function (vID, isMine, bLocateNearLink, helpArea) {
    if (isMine) {
        var theAnchor = $('<a>').addClass('sfx2 gifts').attr('href', '#').attr('onclick', 'ROE.UI.Sounds.click(); return ROE.Frame.popupGifts(' + vID + ');');
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Get items in this village');
        return theAnchor;
    } else {
        return $('<div>').addClass('na').addClass('gifts_na');
    }
};

ROE.Browser.idevice = /iPhone/.test(navigator.userAgent) || /iPad/.test(navigator.userAgent) || /iPod/.test(navigator.userAgent); // mobile safari (ipod, ipad, iphone)
ROE.Browser.android = /Android/.test(navigator.userAgent); // android
ROE.Browser.isAndroidOriDevice = ROE.Browser.idevice || ROE.Browser.android;

ROE.Browser.NoPopupBrowser = false;
//if (navigator.userAgent.match(/Android/i) && !navigator.userAgent.match(/AmazonWebAppPlatform/i)) {
//    ROE.Browser.NoPopupBrowser = true;
//}
ROE.NoPopupBrowser = ROE.Browser.NoPopupBrowser; // leaving  ROE.NoPopupBrowser for compatibility, for now. 

ROE.Browser.getAndroidApiLevel = function () {
    var matches = (/\bApiLevel=([1-9][0-9]*)\b/i)
        .exec(navigator.userAgent);

    if (!matches) {
        return 0;
    }

    return matches[1];
}

ROE.Browser.androidApiLevel = ROE.Browser.getAndroidApiLevel(); // get from navigator.userAgent here

