//
// get proper vov anchor
//
ROE.getVOVAnchor = function (villageNameLink, helpArea) {
    ///<param name="helpArea" >a qjuery div obj that represents the area where the help text will go. if passed undefined, it will be ignore.</param>
    //
    villageNameLink = $(villageNameLink).first(); // get the actual A incase it is not a jquerry object

    var target = villageNameLink.attr('target');
    target = target || "_self";

    var href = villageNameLink.attr('href');

    var onclick = villageNameLink.attr('click');
    onclick = onclick || villageNameLink.attr('onclick');

    var theAnchor = BuildAnchor(ROE.helper_GetAnchorText("Village Overview"), href, { 'onclick': (onclick || ''), 'class': 'myvov', 'target': target });

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Go to Village Overview');
    return theAnchor;
};

//
// get proper attack anchor
//
ROE.getAttackAnchor = function (vID, cordX, cordY, bLocateNearLink, helpArea) {
    var url = BuildURL('CommandTroopsPopup.aspx', ['x=' + cordX, 'y=' + cordY, 'cmd=1']);

    var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Attack')
            , url
            , { 'onclick': ROE.isInPopup ? "" : "return popupQuickSendTroops(this," + vID + ", 'Attack', " + bLocateNearLink + " );"
                    , 'class': 'att'
            });

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Attack this village');
    return theAnchor;

};

//
// get proper one click attack anchor
//
ROE.getAttackAnchor_OneClick = function (vID, cordX, cordY, bLocateNearLink, helpArea) {
    if (ROE.playersNumVillages > 1) {
        var url = BuildURL('MyVillages.aspx'
                    , ['x=' + cordX.toString()
                    , 'y=' + cordY.toString()
                    , 'cmd=1'
                    , 'oneclick=1'
                    , 'ru=CommandTroopsPopup.aspx%3fx%3d' + cordX.toString() + '%26y%3d' + cordY.toString() + '%26cmd%3d1']);

        var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Attack - Quick')
                , url
                    , { 'onclick': ROE.isInPopup ? "" : "return popupQuickSendTroops(this," + vID.toString() + ",'Attack', " + bLocateNearLink + " );", 'class': 'attq' });
        theAnchor = $(theAnchor);

        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Quick-Attack this village');
        return theAnchor;

    }
    else {
        return $('<div>').addClass('na').addClass('attq_na');
    }
};


ROE.getSupportAnchor = function (vID, cordX, cordY, ownerPlayerID, bLocateNearLink, helpArea) {
    if (!ROE.isSpecialPlayer_Rebel(ownerPlayerID)) {
        var url = BuildURL('CommandTroopsPopup.aspx', ['x=' + cordX.toString(), 'y=' + cordY.toString(), 'cmd=0']);

        var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Support')
                , url
                    , { 'onclick': ROE.isInPopup ? "" : "return popupQuickSendTroops(this," + vID.toString() + ", 'Support', " + bLocateNearLink + " );", 'class': 'sup' });

        theAnchor = $(theAnchor);
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Support this village');
        return theAnchor;
    }
    else {
        return $('<div>').addClass('na').addClass('sup_na');
    }
};

//
// get proper one click support anchor
//
ROE.getSupportAnchor_OneClick = function (vID, cordX, cordY, ownerPlayerID, bLocateNearLink, helpArea) {
    if (ROE.playersNumVillages > 1 && !ROE.isSpecialPlayer_Rebel(ownerPlayerID)) {
        var url = BuildURL('MyVillages.aspx'
                    , ['x=' + cordX.toString()
                    , 'y=' + cordY.toString()
                    , 'cmd=0'
                    , 'oneclick=1'
                    , 'ru=CommandTroopsPopup.aspx%3fx%3d' + cordX.toString() + '%26y%3d' + cordY.toString() + '%26cmd%3d0']);

        var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Support - Quick')
                , url
                    , { 'onclick': ROE.isInPopup ? "" : "return popupQuickSendTroops(this," + vID.toString() + ",'Quick Support', " + bLocateNearLink + " );", 'class': 'supq' });

        theAnchor = $(theAnchor);
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Quick-Support this village');
        return theAnchor;
    }
    else {
        return $('<div>').addClass('na').addClass('supq_na');
    }
};

ROE.getHQAnchor = function (vID, isMine, bLocateNearLink, helpArea) {
    if (isMine && ROE.playersNumVillages > 1) {
        var url = BuildURL('VillageBuildingsPopup.aspx', ['svid=' + vID]);

        var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Quick Upgrade')
            , url
                , { 'onclick': ROE.isInPopup ? "" : "return popupWindowFromLink(this,'Quick Upgrade'," + bLocateNearLink + ");", 'class': 'hq' });

        theAnchor = $(theAnchor);
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Upgrade buildings at this village');
        return theAnchor;

    } else {
        return $('<div>').addClass('na').addClass('hq_na');
    }
};
//
// get proper vov anchor
//
ROE.getRecruitAnchor = function (vID, isMine, bLocateNearLink, helpArea) {
    if (isMine && ROE.playersNumVillages > 1) {
        bLocateNearLink = bLocateNearLink || false;
        var url = BuildURL('VillageUnitRecruitPopup.aspx'
                , ['svid=' + vID.toString()]);

        var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Quick Recruit (Disband)')
            , url
                , { 'onclick': ROE.isInPopup ? "" : "return popupWindowFromLink(this,'Quick Recruit'," + bLocateNearLink + ");", 'class': 'recr' });

        theAnchor = $(theAnchor);
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Recruit or disband troops at this village');
        return theAnchor;

    }
    else {
        return $('<div>').addClass('na').addClass('recr_na');
    }
};

//
// get Support anchor
//
ROE.getSupportLookupAnchor = function (vID, isMine, bLocateNearLink, helpArea, ownerPlayerID) {
    if (ROE.playersNumVillages > 1 && !ROE.isSpecialPlayer_Rebel(ownerPlayerID)) {
        bLocateNearLink = bLocateNearLink || false;
        var url;
        var theAnchor

        if (isMine) {
            url = BuildURL('UnitsSupportingPopup.aspx'
                , ['vid=' + vID.toString()]);
        }
        else {
            url = BuildURL('UnitsAbroadPopupAt.aspx'
                , ['sdvid=' + vID.toString()]);
        }
        theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Support Lookup')
            , url
                , { 'onclick': ROE.isInPopup ? "" : "return popupWindowFromLink(this,'Support Lookup'," + bLocateNearLink + ");", 'class': 'supl' });

        theAnchor = $(theAnchor);
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Support and Troops Abroad look up');
        return theAnchor;
    }
    else {
        return $('<div>').addClass('na').addClass('supl_na');
    }
};




//
// get proper map it anchor
//
ROE.getSendSilverAnchor = function (vID, isMine, cordX, cordY, ownerPlayerID, helpArea, bLocateNearLink, locX, locY) {
    /// <param name="locX" >optiona</param>
    /// <param name="locY" > optional </param>

    if (!ROE.isSpecialPlayer(ownerPlayerID)) {
        var theAnchor;
        if (isMine) {
            theAnchor = '<a href="QuickTransportCoins.aspx?svid=' + vID.toString() + '" onclick="return popupQuickSilverTransport(this, ' + bLocateNearLink + ',null,null,' + locX + ',' + locY + ');" class=sends >' + ROE.helper_GetAnchorText("Send Silver") + '</a>';
        } else {
            //quickSendCoins = "~/QuickSendSilver.aspx?" + CONSTS.QuerryString.VillageID + "={0}&"+ CONSTS.QuerryString.XCord + "={1}&" + CONSTS.QuerryString.YCord + "={2}";
            theAnchor = '<a href="QuickSendSilver.aspx?vid=' + vID.toString() + '&x=' + cordX.toString() + '&y=' + cordY.toString() + '" onclick="return popupToOtherPlayersVillageSilverTransport(this, ' + bLocateNearLink + ',null,null,' + locX + ',' + locY + ');" class=sends>' + ROE.helper_GetAnchorText("Send Silver") + '</a>';
        }

        theAnchor = $(theAnchor);
        ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Quickly send silver to this village');
        return theAnchor;
    } else {
        return $('<div>').addClass('na').addClass('sends_na');
    }

};


//
// get proper map it anchor
//
ROE.getMapItAnchor = function (cordX, cordY, helpArea) {
    var theAnchor = BuildAnchor(ROE.helper_GetAnchorText('Map-It'), BuildURL('Map.aspx', ['x=' + cordX.toString(), 'y=' + cordY.toString()]), { 'class': 'map', 'target': (ROE.isInPopup ? '_parent' : '_self') });

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Center map on this village');
    return theAnchor;
};