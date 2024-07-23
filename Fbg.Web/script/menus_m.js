/// <reference path="roe-vov.js" />
/*
allows iframe popup to send something back to parent. used by some popups
*/
function ReceiveIframeInput (name, value) 
{
	input = $('.jsiframeInput[rel="' + name + '"]')[0];
	
	if (input.tagName == "INPUT")
		$(input).attr('value', value);
	if (input.tagName == "TEXTAREA")
		$(input).text(value);
		
	return true;
}


function closeIFrame (name)
{
    if (name == "*")
    {
        $('iframe').parents('.iFrameDiv').remove();
    } else {
        // we're just going to hide the iframe/div.. for now.
        var id = 'popupIframe_' + name;
        $('#' + id)
            .attr('keep', 'no')
            .remove();
    }

    return true;   
}

BDA.Console.setCategoryDefaultView("Silver", true);
function closeIFrame(name, parentDivAlso) {
    if (name == "*") {
        $('iframe').parents('.iFrameDiv').remove();
    } else {
        // we're just going to hide the iframe/div.. for now.
        var id = 'popupIframe_' + name;
        $('#' + id)
            .attr('keep', 'no')
            .hide();
        if (parentDivAlso)
            $('#' + id).remove();
    }
    
    /*     When closing SilverTransport popup, refresh currently selected village. We could apply this to other popups as needed, however,
        this is a bit of a hack, hopefully we get rid of all of these closeIframes and popupIframes in teh future and use one clean system. -farhad 

        //disabled for now, letting QB and QR handle their own refresh functions through Footer .js
        //otherwise, if we need a solution for all silver transport closures, we need to activate this again, but it doesnt work too well!
    */
    if (name == "SilverTransport") {
        BDA.Console.verbose('Silver', 'SilverTransport Closed, calling ROE.Villages.ExtendedInfo_loadLatest(ROE.SVID);' );
        ROE.Villages.ExtendedInfo_loadLatest(ROE.SVID);
    }
    

    return true;
}



function popupIFrame(url, name, title, height, width, x, y, func, iconImgUrl) {
    
    // construct a unique-ish id
    var id = 'popupIframe_' + name;

    var iconUrl = iconImgUrl;
    if (!iconUrl) {
        iconUrl = "https://static.realmofempires.com/images/icons/M_Stable.png";
    }

    // create the div, if it doesn't exist!
    if ($('#' + id).length > 0) {
        $('#' + id)
            .show()                // display the (posiably) hidden iframe div
            .attr('keep', 'yes');   // flag it as active
    } else {
        $("body").append(''
                   + '<div class="iFrameDiv" id="' + id + '" >'
                     + '<img id="background" src="https://static.realmofempires.com/images/misc/SplashScreenMuted.jpg" class="stretch" alt="">'
                       + '<div class="IFrameDivTitle" >'
                           + '<section class="themeM-panel header clearfix">'
                               + '<div class="bg">'
                                   + '<div class="corner-tl"></div>'
                                   + '<div class="corner-br"></div>'
                                   + '<div class="stripe"></div>'
                               + '</div>'
                               + '<div class="fg">'
                                   + '<div class="themeM-icon scale-large">'
                                       + '<div class="bg"></div>'
                                       + '<div class="fg">'
                                           + '<img src="' + iconUrl + '" alt="" /><br />'
                                       + '</div>'
                                   + '</div>'
                                   + '<div class="label">'
                                       + '<span>' + title + '</span><br />'
                                   + '</div>'
                                   + '<div class="level">'
                                       + '<span></span><br />'
                                   + '</div>'
                                   + '<div class="action close pressedEffect type2" onclick="ROE.UI.Sounds.click(); return !' + (func ? func : 'closeIFrame') + '(\'' + name + '\', true);"></div>'
                               + '</div>'
                           + '</section>'
                       + '</div>'
                       + '<div style="position: absolute; left: 0px; top: 50px; right: 0px; bottom: 0px; width: auto; height: auto;">'
                           + '<iframe name="' + name + '" border="0" onload="this.style.visibility = \'visible\'; "    style="visibility:hidden; height:100%; width:100%; border:none;" src="' + url + '" frameborder="0"></iframe>'
                       + '</div>'
                   + '</div>'
               );

        $('#' + id)
            .show()
            .attr('keep', 'yes')    // so that it's not instantly cleaned up.
            .mouseover(function () { $(this).attr('keep', 'yes'); }); // flag as active when it's hovered









    }

    return true;
}

/*

TESTING a hide/show version of the popup and not remove/add

*/
function closeModalPopup_HIDE(name, parentDivAlso) {
    $('.popup_modal').remove();

    return closePopup_HIDE(name, parentDivAlso);
}
function closePopup_HIDE(name) {
    if (name == "*") {
        $('iframe').parents('.iFrameDiv').remove();
    } else {
        // we're just going to hide the iframe/div.. for now.
        var id = ROE.Frame.CONSTS.popupNameIDPrefix + name;
        $('#' + id).hide();
    }



    ROE.Frame.enableView(true);

    return true;
}
function closeModalPopupAndReloadHeader_HIDE(name, parentDivAlso) {
    ROE.Frame.reloadFrame();
    ROE.vov.unpauseAnimation();
    return closeModalPopup_HIDE(name, parentDivAlso);
}





function closeModalIFrameAndReloadHeader(name, parentDivAlso) {
    ROE.Frame.reloadFrame();

    return closeModalIFrame(name, parentDivAlso);
}

function closeModalPopupAndReloadHeader(name, parentDivAlso) {   
    ROE.Frame.reloadFrame();
    ROE.vov.unpauseAnimation();
    if ($('#popup_QuickBuild').length > 0) {
        ROE.QuickBuild.reInitContent();
    }
    if ($('#popup_QuickRecruit').length > 0) {
        ROE.QuickRecruit.reInitContent();
    }
    return closeModalPopup(name, parentDivAlso);
}

function closeModalPopupAndReloadHeaderAndView(name, parentDivAlso)  {
    ROE.Frame.reloadFrame();
    ROE.Frame.reloadView();
    ROE.vov.unpauseAnimation();
    return closeModalPopup(name, parentDivAlso);
}

function closeBuildingPage(name, parentDivAlso) {
    ROE.Frame.reloadFrame();
    if (ROE.Building2.changesMade_RequireVOVReload) {
        ROE.Frame.reloadView();
    }
    ROE.vov.unpauseAnimation();
    return closeModalPopup(name, parentDivAlso);
}

function closeQuickBuild(name, parentDivAlso) {
    ROE.Frame.reloadFrame();
    if (ROE.QuickBuild.changesMade_RequireVOVReload && ROE.QuickBuild.launchedFrom == "vov") {
        ROE.Frame.reloadView();
    }
    ROE.vov.unpauseAnimation();
    return closeModalPopup(name, parentDivAlso);
}

function closeQuickRecruit(name, parentDivAlso) {
    ROE.Frame.reloadFrame();
    if (ROE.QuickRecruit.changesMade_RequireVOVReload && ROE.QuickRecruit.launchedFrom == "vov") {
        ROE.Frame.reloadView();
    }
    ROE.vov.unpauseAnimation();
    return closeModalPopup(name, parentDivAlso);
}

function closeResearch(name, parentDivAlso) {
    ROE.Frame.reloadFrame();
    ROE.vov.unpauseAnimation();
    return closeModalPopup(name, parentDivAlso);
}


function closeModalIFrame(name, parentDivAlso) {
    $('.popupIFrame_modal').remove();
    ROE.vov.unpauseAnimation();
    return closeIFrame(name, parentDivAlso);
}


function popupModalIFrame2(url, name, title, iconImgUrl, closeFunct ) {
    return popupModalIFrame(url, name, title, undefined, undefined, undefined, undefined, closeFunct, iconImgUrl);
}

function popupModalIFrame(url, name, title, height, width, x, y, f, iconImgUrl) {

    ROE.Frame.enableView(false);
    ROE.vov.pauseAnimation();
    $('body').append('<div class="popupIFrame_modal" style="height: 100%; width: 100%; position: fixed; left: 0pt; top: 0pt; z-index: 2999; opacity: 0.5; filter: alpha(opacity=50); background-color: black;"></div>');


    height = height ? height : ROE.Frame.CONSTS.popupDefaultHeight;
    width = width ? width : ROE.Frame.CONSTS.popupDefaultWidth;
    x = x ? x : ROE.Frame.CONSTS.popupDefaultX;
    y = y ? y : ROE.Frame.CONSTS.popupDefaultY;

    f = f ? f : 'closeModalIFrame';

    return popupIFrame(url, name, title, height, width, x, y, f, iconImgUrl);
}


function popupModalIFrameOrWinOpen(url, name, title, height, width, x, y, f) {
    ///<summary>same as popupModalIFrame except that this will do a window.open if this is a non-popup browser</summary>
    if (!ROE.NoPopupBrowser) {
        return !popupModalIFrame(url, name, title, height, width, x,y,f);
    } else {
        window.open(url);
        return true;
    }
}

function popupQuickSilverTransport (link, bPositionHeightByLink, height, width, defaultX, defaultY) 
{
    /// <param name="defaultX" >optiona</param>
    /// <param name="defaultY" > optional </param>
    var url = $(link).attr('href');
    var x = defaultX | 10;
    var y = defaultY | 20;
    if (bPositionHeightByLink) {
        y = $(link).offset().top;
    }
    height = height || 470;
    width = width || 350;
    return !popupModalIFrame(url, 'QuickSilverTransport', "Silver Transport", height, width, x, y);
}

function popupToOtherPlayersVillageSilverTransport (link, bPositionHeightByLink, height, width, defaultX, defaultY) 
{
    /// <param name="defaultX" >optiona</param>
    /// <param name="defaultY" > optional </param>
    var url = $(link).attr('href');
    var x = defaultX | 20;
    var y = defaultY | 40;
    if (bPositionHeightByLink) {
        y = $(link).offset().top;
    }
    height = height || 450;
    width = width || 350;
    return !popupModalIFrame(url, 'ToOtherPlayersVillageSilverTran', "Quick Silver Transport", height, width, x, y);
}

// ToOtherPlayersVillageSilverTransport  - TOPVST
function popupTOPVST(link, windowame)
{
    var url = $(link).attr('href');
    var p = $(link).position();    
    return !popupIFrame(url, windowname, "Quick Silver Transport", 450, 350, p.left, p.top + 10);
}

function popupVillageSelectionList (link)
{
    var url = $(link).attr('href');
    var p = $(link).position();
    return !popupIFrame(url, 'VillageSelectionList', "Village Selection", 500, 450, p.left, p.top + 10);
}


function popupQuickSendTroops(link, targetVid, type, bPositionHeightByLink, height, width) {
    /// <param name="bPositionHeightByLink" > optional. if false, will position window at a fix location 20,40. if true, will position the window at the same height as the link</param>
    var url = $(link).attr('href');
    var x = 20;
    var y = 40;
    height = height || 450;
    width = width || 380;
    switch(bPositionHeightByLink)
    {
          case 1:
              y = $(link).offset().top;
              break;
          case 2:
              x = ($('div.TDContent').outerWidth() - width)/2;
              y = ($('div.TDContent').outerHeight() - height)/2;
              break;
    }
    return !popupModalIFrame(url, 'QuickSend' + targetVid + Math.floor(Math.random() * 1000), "Quick " + type, height, width, x, y);
}



// extend the Array prototype
if (Array.prototype.contains == null)
        Array.prototype.contains = function (t)
        {
                var i;
                for (i = 0; i < this.length; i++)
                        if (t.toLowerCase() == this[i].toLowerCase())
                                return true;

                return false;
        }



// enable the cose button
$(function () { $('#popUpClose a, #popUpClose button').click(function () { window.close(); }); });

function popupWindow (url, name)
{
    popupWindow2(url, name, 450,500);
}

function popupWindow2 (url, name, height, width)
{
    try {
        this.newWindow = window.open(url, name, 'height='+height+',width='+width+',resizeable=yes,scrollbars=yes,toolbar=no,menubar=no,location=no,status=no,directories=no');
    } catch (e) {
        alert('Popup blocked! Make sure to allow JavaScript popups!');
        return null;
    }
    return this.newWindow;
}


function popupWindowFromLink(link, sWindowTitle, bPositionHeightByLink, height, width) {
    /// <summary>popup iframe window from url in the link</summary>
    /// <param name="link" >required. Anchor element</param>
    /// <param name="sWindowTitle" >optional</param>
    /// <param name="bPositionHeightByLink" > optional. if false, will position window at a fix location 20,40. if true, will position the window at the same height as the link</param>
    /// <param name="height" >optiona - height of the window</param>
    /// <param name="width" > optional - width of the window</param>
    /// <returns type="Number">false is all was successful - can be used in onclick=return popupWindowFromLink to cancel default action</returns>

    var url = $(link).attr('href');
    var x = 20;
    var y = 40;
    if (bPositionHeightByLink) {
        y = $(link).offset().top;
    }
    height = height || 500;
    width = width || 680;
    return !popupModalIFrame(url, 'popupWindowFromLink' + Math.floor(Math.random() * 1000).toString(), sWindowTitle.toString(), height, width, x, y);
    // hack: the "+ Math.floor(Math.random() * 1000).toString()" was added for chrome. chrome did not load the popup the second time, returns some weird error. 
}



function buildQuery (query)
{
    var queryString = null;
    for (i = 0; i < query.length; i++) {
        if (queryString)
            queryString = queryString + '&' + query[i].name + "=" + query[i].value;
        else
            queryString = query[i].name + "=" + query[i].value;
    }
    return queryString;
}

function breakQuery (query)
{
    if (!query)
        return null;
    
    try {
        var newQuery = new Array();
        
        var q = query.split(/&/);
        for (i=0; i < q.length; i++) {
            pair = q[i].split(/=/);
            if (pair[1])
                newQuery.push( { 'name' : pair[0], 'value' : pair[1] } );
        }
        return newQuery;
    } catch (e) {
        //DumperAlert([{ 'e': e }, 'breakQuery']);
        alert(e);
    }
    return null;
}

function stripQuery (queryString, toStrip)
{
    newQuery = new Array();

    if (!queryString)
        return newQuery;
        
    for (i=0; i < queryString.length; i++) {
        mustKeep = true;
        for (o=0; o < toStrip.length; o++) {
            if (toStrip[o].toLowerCase() == queryString[i].name.toLowerCase())
                mustKeep = false;
        }
        if (mustKeep) {
            newQuery.push(queryString[i]);
        }
    }
    return newQuery;
}

function BuildURL (path, data)
{
    var url = path;
    if (data.length > 0) {
        url = url + '?' + data.join('&');
    }
    return url;
}
function BuildAnchor (text, url, opts)
{
    attr = "";
    if (opts)
        for (x in opts) {
            attr += ' ' + x + '="' + opts[x] + '"';
        }
    return '<a href="' + url + '"' + attr + '>' + text + '</a>';
}


function InitStripeTable()
{
   
}

$(
    function ()
    {
        $('.stripeTable tr.popupHighlight').hover(
            function () { $(this).addClass('stripeTablePopupHover'); },
            function () { $(this).removeClass('stripeTablePopupHover'); }
        );
    }
);



function reloadParent () {

    if (opener) {
        // we're executing from a popup window
        opener.windowReload();
        window.close();
    } else {
        // executing from an IFrame
        // Just close the ifrme (we have to know the name): window.parent.closeIFrame('QuickSilverTransport');
        window.parent.windowReload();
    }
       
    return false;
}
function loadInParent (element)
{
    var url = element.href ? element.href : element;

    if (opener) {
        // in a popup window
        opener.window.location.href = url;
        window.close();
    } else {
        window.parent.location.href = url;
    }
    
    return false;
}
function selectVillageInParent (link)
{
    var id = $(link).attr('href').match(/svid=(\d+)/)[1];
    
    window.parent.selectVillage(id);
    
    return false;
}
function bench (string, start)
{
    var now = new Date();
    $('body').append('<p>' + string + ": " + (now - start) + "</p>");
    
    return new Date();
}

function genericModalPopup(link, winName)
{
    var url = $(link).attr('href');
    return !popupModalIFrame(url, 'popup', winName, 1000, 680, 20, 40);
}

function closeAndReload(n, p) {
    ROE.Frame.enableView(true);
    closeModalIFrame(n, p); ROE.Frame.reloadFrame();
}

function popupResearch(fullQuerry)
{
    return !popupModalIFrame("research.aspx" + (fullQuerry ? fullQuerry :""), "Research", '', 500, 680, 20, 40, 'closeAndReload');
}
function popupPlayerOverview(url, targetPlayerName)
{
    return !popupModalIFrame(url, targetPlayerName.replace(/\./gi, '_') + Math.floor(Math.random() * 1000), '', 500, 680, 20, 40);
}

function popupPlayerOverviewByID(link, pid) {
    var url = $(link).attr('href');

    //
    // originally was calling popupModalIFrameOrWinOpen so that the popup opens a proper popup in iOS
    //  and a new window on android. but this complicates the code inside the pages that load inside the iframe and since 
    //  we will probably have all code inside native apps, we are instead electing to do all iFrame popups in new window. 
    //
    //return !popupModalIFrameOrWinOpen(url, pid.toString() + Math.floor(Math.random() * 1000), '');
    window.open(url);
    return false;
}

function popupOtherVilageOverview(url, targetVID) {

    //
    // originally was calling popupModalIFrameOrWinOpen so that the popup opens a proper popup in iOS
    //  and a new window on android. but this complicates the code inside the pages that load inside the iframe and since 
    //  we will probably have all code inside native apps, we are instead electing to do all iFrame popups in new window. 
    //
    //return !popupModalIFrameOrWinOpen(url, targetVID + Math.floor(Math.random() * 1000), '');
    window.open(url);
    return true;

}

function popupVilageOverview2(link, targetVID, bPositionHeightByLink, height, width) {
    var url = $(link).attr('href');
    var x = 20;
    var y = 40;
    if (bPositionHeightByLink) {
        y = $(link).offset().top;
    }
    height = height || 500;
    width = width || 680;

    return !popupModalIFrame(url, targetVID + Math.floor(Math.random() * 1000), '', height, width, x, y);
}

function popupModalPopup(name, title, height, width, x, y, f, iconImgUrl) {

    ROE.vov.pauseAnimation();

    $('body').append('<div class="popup_modal" style="height: 100%; width: 100%; position: fixed; left: 0pt; top: 0pt; z-index: 2999; opacity: 0.5; filter: alpha(opacity=50); background-color: black;"></div>');

    height = height ? height : ROE.Frame.CONSTS.popupDefaultHeight;
    width = width ? width : ROE.Frame.CONSTS.popupDefaultWidth;
    x = x ? x : ROE.Frame.CONSTS.popupDefaultX;
    y = y ? y : ROE.Frame.CONSTS.popupDefaultY;
    f = f ? f : 'closeModalPopup';

    ROE.Frame.enableView(false);

    

    return popupPopup(name, title, height, width, x, y, f, iconImgUrl);
}


function popupPopup(name, title, height, width, x, y, func, iconImgUrl) {
    // construct a unique-ish id
    var id = ROE.Frame.CONSTS.popupNameIDPrefix + name;
    var iconUrl = iconImgUrl;
    if (!iconUrl) {
        iconUrl = "https://static.realmofempires.com/images/icons/M_servants.png";
    }

    // create the div, if it doesn't exist!
    if ($('#' + id).length > 0) {
        $('#' + id)
            .show()                // display the (posiably) hidden iframe div
            .attr('keep', 'yes');   // flag it as active
    } else {


        $("body").append(''
            + '<div class="iFrameDiv" id="' + id + '" >'
                + '<div class="IFrameDivTitle" >'
                    + '<section class="themeM-panel header clearfix">'
                        + '<div class="bg">'
                            //+ '<div class="corner-tl"></div>'
                            //+ '<div class="corner-br"></div>'
                            + '<div class="stripe"></div>'
                        + '</div>'
                        + '<div class="fg">'
                            + '<div class="themeM-icon scale-large">'
                                + '<div class="bg"></div>'
                                + '<div class="fg">'
                                    + '<img src=' + iconUrl + ' alt="" /><br />'
                                + '</div>'
                            + '</div>'
                            + '<div class="label">'
                                + '<span>' + title + '</span><br />'
                            + '</div>'
                            + '<div class="level">'
                                + '<span></span><br />'
                            + '</div>'
                            + '<div class="action close  pressedEffect type2" onclick="ROE.Frame.infoBarClose(); ROE.UI.Sounds.clickMenuExit(); return !' + (func ? func : 'closeIFrame') + '(\'' + name + '\', true);"></div>'
                        + '</div>'
                    + '</section>'
                + '</div>'
                + '<div class="popupBody" ></div>'
            + '</div>'
        );


        $('#' + id)
            .show()
            .attr('keep', 'yes')    // so that it's not instantly cleaned up.
            .mouseover(function () { $(this).attr('keep', 'yes'); }); // flag as active when it's hovered
    }

    return true;
}
