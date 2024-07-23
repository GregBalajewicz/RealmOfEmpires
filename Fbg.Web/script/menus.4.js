var shadowOpts = { offset : 3, opacity : 0.2 };
var IDMapping = { // this is to speed up DOM lookups. LookByID is 1000s of times faster..
    'buildingsTable' : 'ctl00_ContentPlaceHolder1_tblBuildings'
}
var genericMenuConfig = {
	moveAwayDelay	: 700,	// time in miliseconds!
	timer			: null,	// used internally.
	maxHeight       : 400,
	menus			: 	{
		'fake'	:	[
		    { select : '.header_bar a.reports, .header_bar a.mail, .header_bar a.clan', menuType : 'genericMenuFake' }
		],
		'helpMenu' : [
    		{ select : '.header_bar a.help', menuType : 'genericMenuStart', menuAlign : 'right' , stillClickable : true},
    		{ type: 'markup', 'markup': "<div class='menuHeader' style='background-image : url(https://static.realmofempires.com/images/navIcons/Help2.gif); width : 220px;'><span>Help</span></div>" },
		    { title : 'Beginner Tutorial', url : 'startTutorial.aspx', type : 'item', divider :  true},
			{ title: 'FAQ - Frequently Asked Questions', url: 'http://roe.wikia.com/wiki/FAQ', type: 'item', opts: { target: "_blank"} },
			{ title : 'Buildings', url : 'Help.aspx', type : 'item' },
			{ title : 'Units', url : 'Help.aspx?HPN=Unit', type : 'item' },
			{title: 'Forum (Discussion board)', url: 'https://www.realmofempires.com/throneroom.aspx', type: 'item', opts: { target: "_black" }, divider: true }, 
			{ title: 'Make a Suggestion', url: 'https://roe.uservoice.com/forums/96839-general', type: 'item', opts: { target: "_blank"} },
			{ title: 'Contact Support', url: 'ContactSupport.aspx', type: 'item', opts: { target: "_blank" } },
			{ title : 'About Us',
			    url: 'https://www.facebook.com/pages/Realm-Of-Empires/310594832310078', 
			  type : 'item', 
			  opts : { target : "_new" }, 
			  divider : true 
			},
			{ title : 'Terms Of Use', url : 'tou.aspx', type : 'item', opts : { target : "_new"} },
			{ title : 'Credits', url : 'about_credits.aspx', type : 'item', opts : { target : "_new"} }
		],
		'ranking' : [
		    { select : '.header_bar a.ranking', menuType : 'genericMenuStart', menuAlign : 'right', stillClickable : true },
            { type: 'markup', 'markup': "<div class='menuHeader' style='background-image : url(https://static.realmofempires.com/images/navIcons/Ranking2.gif);'><span>Ranking</span></div>" },
		    { type : 'item', title : 'Player Ranking', url : 'stats.aspx' , divider :  true },
		    { type : 'item', title : 'Friend Network Ranking', url : 'playersFriendRanking.aspx' },
		    { type : 'item', title : 'Clan Ranking', url : 'clanranking.aspx' },
		    { type : 'item', title : 'Player Titles', url : 'TitlesRanking.aspx' },
		    { type : 'item', title : 'Global Stats', url : 'GlobalStats.aspx' },
		    { type : 'item', title : 'Friends of Realm Of Empires', url : 'pfDonors.aspx' }
		],
		'invite' : [
		    { select : '.header_bar a.invite', menuType : 'genericMenuStart', menuAlign : 'right', stillClickable : true }          
		],
		'settings' : [
		    { select : '.header_bar a.settings', menuType : 'genericMenuStart', menuAlign : 'right' , stillClickable : true},
            { type: 'markup', 'markup': "<div class='menuHeader' style='background-image : url(https://static.realmofempires.com/images/navIcons/Settings2.gif);'><span>Tools & Options</span></div>" },
		    { type : 'item', title : 'Unlock Features', url : 'pfBenefits.aspx', divider :  true },
		    { type : 'item', title : 'Kingdom Stewardship', url : 'AccountStewards.aspx'},
		    { type : 'item', title : 'Options', url : 'PlayerOptions.aspx'},
		    { type : 'item', title : 'Polls', url : 'Polls.aspx'},
		    { type : 'item', title : 'Switch Realm', url : 'logoutOfRealm.aspx', divider :  true }
		],
		'mail' : [
		    { select : '.header_bar a.mail', menuType : 'genericMenuStart', menuAlign : 'right' , stillClickable : true},
            { type: 'markup', 'markup': "<div class='menuHeader' style='background-image : url(https://static.realmofempires.com/images/navIcons/mail2.gif);'><span>Messages</span></div>" },
		    { type : 'item', title : 'Inbox', url : 'messages.aspx?fid=-1&va=false&pageindex=-1', divider :  true },
		    { type : 'item', title : 'Compose Message', url : 'messages_create.aspx'},
		    { type : 'item', title : 'Last View', url : 'messages.aspx'}
		],
		'reports' : [
		    { select : '.header_bar a.reports', menuType : 'genericMenuStart', menuAlign : 'right' , stillClickable : true},
            { type: 'markup', 'markup': "<div class='menuHeader' style='background-image : url(https://static.realmofempires.com/images/navIcons/reports2.gif);'><span>Reports</span></div>" },
		    { type : 'item', title : 'Inbox', url : 'Reports.aspx?fid=-1&va=false&reptid=-1&nosearch=1&pageindex=-1', divider :  true },
		    { type : 'item', title : 'Last View', url : 'Reports.aspx'}
		],
		'clan' : [
		    { select : '.header_bar a.clan', menuType : 'genericMenuStart', menuAlign : 'right' , stillClickable : true},
            { type: 'markup', 'markup': "<div class='menuHeader' style='background-image : url(https://static.realmofempires.com/images/navIcons/clan2.gif);'><span>My Clan</span></div>" },
		    { type : 'item', title : 'Overview', url : 'ClanOverview.aspx', divider :  true },
		    { type : 'item', title : 'Public Message', url : 'ClanPublicProfile.aspx'},
		    { type : 'item', title : 'Invitations', url : 'ClanInvitations.aspx'},
		    { type : 'item', title : 'Members', url : 'ClanMembers.aspx'},
		    { type : 'item', title : 'Forum', url : 'ClanForum.aspx'},
		    { type : 'item', title : 'Diplomacy', url : 'ClanDiplomacy.aspx'},
		    { type : 'item', title : 'Events', url : 'ClanEvents.aspx'}		    		    
		],
		'manage' : [
		    { select : "td.TroopsHeader a.jsTroopsManage, a.jsTroopsMenu", menuType : 'genericMenuStart', menuAlign : 'right' , stillClickable : true},
            { type: 'markup', 'markup': "<div class='menuHeader' style='background-image : url(https://static.realmofempires.com/images/units/infantry.png);'><span>Manage Troops</span></div>" },
		    { type : 'item', url : "CommandTroops.aspx", title : "Command", tooltip : "Attack or support other villages", divider :  true },
            { type : 'item', url : "VillageUnitRecruit.aspx", title : "Recruit", tooltip : "Recruit more troops" },
            { type : 'item', url : "TroopMovementsIn.aspx?vid=-1", title : "Incoming", tooltip : "List of all inbound troops"},
            { type : 'item', url : "TroopMovementsOut.aspx?vid=-1", title : "Outgoing", tooltip : "List of all your outbound troops"},
            { type : 'item', url : "UnitsAbroad.aspx?PS=0", title : "My troops abroad", tooltip : "My troops in (supporting) other villages"},
            { type : 'item', url : "UnitsSupporting.aspx?vid=-1", title : "Support", tooltip : "Troops from other villages supporting my village"},
            { type : 'item', url : "BattleSimulator.aspx", title : "Battle Simulator", tooltip : "Allows you to simulate a battle", divider :  true}
		],
		
		'TA' : [
		    { select : 'a.jsTAMenu', menuType : 'genericMenuStart', stillClickable : true },
		    { type : 'item', url : "UnitsAbroad.aspx?PS=1", title : "See this village's troops supporting other villages", tooltip : "" },
            { type : 'item', url : "UnitsAbroad.aspx?PS=0", title : "See all my troops supporting other villages", tooltip : "" }
		],
		'Sup' : [
		    { select : 'a.jsSupMenu', menuType : 'genericMenuStart', stillClickable : true },
		    { type : 'item', url : "UnitsSupporting.aspx", title : "See Support I have at this village", tooltip : "" },
            { type : 'item', url : "UnitsSupporting.aspx?vid=-1", title : "See Support I have at all my villages", tooltip : "" }
		],
		'Outgoing' : [
		    { select : 'a.jsOutgoingMenu', menuType : 'genericMenuStart', stillClickable : true },
		    { type : 'item', url : "TroopMovementsOut.aspx", title : "See outbound troops from this village", tooltip : "" },
            { type : 'item', url : "TroopMovementsOut.aspx?vid=-1", title : "See outbound troops from all my villages", tooltip : "" }
		],
		'VddMenu' : [
		    { select : 'a.jsVddMenu', menuType : 'genericMenuStart' },
		    { type : 'item', url : "MyVillages.aspx", title : "Select a Village", tooltip : "", opts : { OnClick : "return popupVillageSelectionList(this);"  }},
            { type : 'item', url : "VillageSummary.aspx?b=1", title : "Summary: Buildings", tooltip : "", divider :  true },
            { type : 'item', url : "VillageSummary.aspx?u=1", title : "Summary: Units", tooltip : "" },
            { type : 'item', url : "VillageMassRecruit.aspx?u=1", title: "Mass Recruit", tooltip: "", divider: true },
            { type : 'item', url : "VillageMassUpgrade.aspx?b=1", title: "Mass Upgrade", tooltip: "" },
            { type : 'item', url: "VillageMassUpgradeB.aspx?b=1", title: "Mass Upgrade 2 (Beta)", tooltip: "" },
            { type : 'item', url: "VillageMassDowngrade.aspx?b=1", title: "Mass Downgrade", tooltip: "" },
            { type : 'item', url : "VillageMassDisband.aspx?u=1", title : "Mass Disband", tooltip : "" },
            { type : 'item', url : "VillageMassChestBuy.aspx?b=1", title : "Mass Chest Buy", tooltip : "" },
            { type : 'item', url : "Tags.aspx", title : "Group Villages", tooltip : "" , divider :  true},
            { type : 'item', url : "VillageMassTag.aspx", title: "Mass Tag/UnTag", tooltip: "" }
		],
		'Incoming' : [
		    { select : 'a.jsIncomingMenu', menuType : 'genericMenuStart', stillClickable : true },
		    { type : 'item', url : "TroopMovementsIn.aspx", title : "See inbound troops to this village", tooltip : "" },
            { type : 'item', url : "TroopMovementsIn.aspx?vid=-1", title : "See inbound troops to all my villages", tooltip : "" }
		]	
	}
	/* Other options for menu items 
	   * type can be 'item' or 'markup'
	     markup is freeform HTML basicly, item will just be a nice link
	   * divider can be true or false, just ture or false, not strigns eg "true"
	     This will make the top of the item a divider / seperator
	   * extra. Extra data it include in a <small> after the item, like the points on the village switch */
	   
	/* example code to enable a menu:
	  $(".header_bar a.ranking")
	    .addClass('genericMenuStart')
	    .attr('menu', 'ranking')
	    .attr('menuAlign', 'right'); */
};

//var console = { log : function () {} }; // comment this to enable console loggin in FF/Firebug!

var ui_id_helper = 1;
var destroyTimer;
var PreLoadImages = new Array();

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
/*popup unlock a PF popup*/
function popupUnlock (link)
{    
    var url = $(link).attr('href');
    var w = popupWindow2(url, 'unlock', 500, 500);
    
    return false;}

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
    return true;
}

function popupIFrame(url, name, title, height, width, x, y, func) {

    // construct a unique-ish id
    var id = 'popupIframe_' + name;

    // create the div, if it doesn't exist!
    if ($('#' + id).length > 0) {
        $('#' + id)
            .show()                // display the (posiably) hidden iframe div
            .attr('keep', 'yes');   // flag it as active
    } else {
        $('body').append('<div id="' + id + '" class="iFrameDiv" style="position:absolute; height:' + (height + 18) + 'px; width:' + width + 'px;z-index: 3000;"></div>');
        $('#' + id)
            .css({ top: y + 'px', left: x + 'px' })
            .append('<div class="IFrameDivTitle" style="background:none repeat scroll 0 0 #292116;height:18px;text-align:left;width:100%;"><span class="title" style="color:#C39037;float:left;font-size:14px;font-weight:bold;padding:0 2px;">' + title + '</span><img style="background:none repeat scroll 0 0 transparent;border:medium none;cursor:pointer;display:inline;float:right;margin:0;" id=imgIframeClose onclick="return !' + (func ? func : 'closeIFrame') + '(\'' + name + '\', true);" src="https://static.realmofempires.com/images/delete.gif" /></div>')
            .append('<iframe name="' + name + '" border="0" style="height:' + height + 'px; width:' + width + 'px; border:none;" src="' + url + '" frameborder="0"></iframe>')
            .show()
            .attr('keep', 'yes')    // so that it's not instantly cleaned up.
            .mouseover(function() { $(this).attr('keep', 'yes'); }); // flag as active when it's hovered
    }

    return true;
}


function closeModalIFrame(name, parentDivAlso) {
    $('.popupIFrame_modal').remove();

    return closeIFrame(name, parentDivAlso);
}

function popupModalIFrame(url, name, title, height, width, x, y, f) {

    $('body').append('<div class="popupIFrame_modal" style="height: 100%; width: 100%; position: fixed; left: 0pt; top: 0pt; z-index: 2999; opacity: 0.5; filter: alpha(opacity=50); background-color: black;"></div>');

    return popupIFrame(url, name, title, height, width, x, y, f ? f : 'closeModalIFrame');
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
    return !popupModalIFrame(url, 'QuickSilverTransport', "Convenient Silver Transport", height, width, x, y);
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
function popupTOPVST(link, windowname)
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
    if (bPositionHeightByLink) {
        y = $(link).offset().top;
    }
    height = height || 550;
    width = width || 680;
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

// preloader code
jQuery.preloadImages = function()
{
    for(var i = 0; i<arguments.length; i++)
    {
        PreLoadImages[i] = new Image();
		PreLoadImages[i].number = i;
		// PreLoadImages[i].onload = function () { $('#load_' + this.number).text('loaded'); };
		PreLoadImages[i].src = arguments[i];
    }
}

// click menus for the top icons
$(
    function () {
        // preload some images
        $.preloadImages(
        // images for map borders
            'https://static.realmofempires.com/images/map/MapBorderTL2.png',
            'https://static.realmofempires.com/images/map/MapBorderT2.png',
            'https://static.realmofempires.com/images/map/MapBorderTR2.png',
            'https://static.realmofempires.com/images/map/MapBorderR2.png',
            'https://static.realmofempires.com/images/map/MapBorderBR2.png',
            'https://static.realmofempires.com/images/map/MapBorderB2.png',
            'https://static.realmofempires.com/images/map/MapBorderBL2.png',
            'https://static.realmofempires.com/images/map/MapBorderL2.png',
        // menu icons
            'https://static.realmofempires.com/images/navIcons/18.gif',
        // village icons
            'https://static.realmofempires.com/images/map/village1a.png',
            'https://static.realmofempires.com/images/map/village1b.png',
            'https://static.realmofempires.com/images/map/village2a.png',
            'https://static.realmofempires.com/images/map/village2b.png',
            'https://static.realmofempires.com/images/map/village3a.png',
            'https://static.realmofempires.com/images/map/village3b.png',
            'https://static.realmofempires.com/images/map/village4a.png',
            'https://static.realmofempires.com/images/map/village4b.png',
            'https://static.realmofempires.com/images/map/village5a.png',
            'https://static.realmofempires.com/images/map/village5b.png',
            'https://static.realmofempires.com/images/map/village6a.png',
            'https://static.realmofempires.com/images/map/village6b.png',
        // menu buttons
            'https://static.realmofempires.com/images/navIcons/Reports1.gif',
            'https://static.realmofempires.com/images/navIcons/Reports2.gif',
            'https://static.realmofempires.com/images/navIcons/Reports3.gif',
            'https://static.realmofempires.com/images/navIcons/Reports4.gif',
            'https://static.realmofempires.com/images/navIcons/Mail1.gif',
            'https://static.realmofempires.com/images/navIcons/Mail2.gif',
            'https://static.realmofempires.com/images/navIcons/Mail3.gif',
            'https://static.realmofempires.com/images/navIcons/Mail4.gif',
            'https://static.realmofempires.com/images/navIcons/Clan1.gif',
            'https://static.realmofempires.com/images/navIcons/Clan2.gif',
            'https://static.realmofempires.com/images/navIcons/Clan3.gif',
            'https://static.realmofempires.com/images/navIcons/Clan4.gif',
            'https://static.realmofempires.com/images/navIcons/Invite1.gif',
            'https://static.realmofempires.com/images/navIcons/Invite2.gif',
            'https://static.realmofempires.com/images/navIcons/Ranking1.gif',
            'https://static.realmofempires.com/images/navIcons/Ranking2.gif',
            'https://static.realmofempires.com/images/navIcons/Help1.gif',
            'https://static.realmofempires.com/images/navIcons/Help2.gif',
            'https://static.realmofempires.com/images/navIcons/Settings1.gif',
            'https://static.realmofempires.com/images/navIcons/Settings2.gif');


        // used to store menus
        $('body').append('<div id="allMenus"></div>');


        // we need to do stuff the user would ntice first
        genericMenusInit(); // new menus
       
        // Init the help system 
        initHelpLoadDB();

        // init all drop down expanders
        initDropDownExpanders();

       

        // init village Next/Prev
        NextPrevVillage();

        // add hover over effect for get more silver icon
        if ($('.getMoreSilverIcon').length > 0) {
            $('.getMoreSilverIcon')
                .hover(
                    function () { $(this).attr('src', 'https://static.realmofempires.com/images/GetMore3.gif'); },
                    function () { $(this).attr('src', 'https://static.realmofempires.com/images/GetMore2.gif'); }
                );
        }
    }
);

function initDropDownExpanders ()
{
    $('.expandRelated')
        .click( function () { $(".expandable[name='" + $(this).attr('name') + "']").toggle(); } )
        .css( { cursor : 'pointer' });
}

function initFakeSelects ()
{
    $('div.jsFakeSelect').each(
        function () {

            try {
                var options = $(this).find('.jsOptions');
                var master = $(this).find('.jsMaster');
                var triger = $(this).find('.jsTriger');

                $(triger)
                .click(function () { genericMenusClearNow(); $(this).parent().find('.jsOptions').show(); })
                .mouseover(function () { genericMenusClearNow(); $(this).parent().find('.jsOptions').show(); })
                .mouseout(function () { genericMenuTimer('genericMenusClear();'); });

                p = $(master).position();
                if ($(master).hasClass('includeMe')) {
                    p.height = 0;
                    $(master).clone().prependTo(options);
                } else {
                    p.height = $(master).height();
                }

                $(options)
                .css({ 'left': p.left + 'px', 'top': (p.top + p.height) + 'px' })
                .mouseover(function () { $(this).attr('keep', 'yes'); })
                .mouseout(function () { $(this).attr('keep', 'no'); genericMenuTimer('genericMenusClear();'); })
            }  catch (e) { }
        });
    return;
}

function jaxReplace ()
{
    $('.jaxHide')
        .hide()
        .each(
            function () 
            {
                link = $('<a href="#">' + $(this).attr('rel') + '</a>').click(
                    function () { $(this).prev().slideDown().end().remove();return false; }
                );
                $(this).after(link);
            }
        );
    $('.jaxHideFader')
        .hide()
        .each(
            function () 
            {
                link = $('<a href="#">' + $(this).attr('rel') + '</a>').click(
                    function () { $(this).prev().fadeIn().end().remove();return false; }
                );
                $(this).after(link);
            }
        );
        
    $('.rplClanRename').hide().after('<a href="/" onclick="$(this).remove(); $(\'.rplClanRename\').slideDown(); return false;">Rename Clan</a>');
}



function ui_warn (message)
{
    if ($('#ui_warn').length == 0)
        $('body').append('<div id="ui_warn"></div>');
    
    $('#ui_warn').append('<div class="ui_warn"></div>');
    $(message).each(
        function () {
            $('#ui_warn .ui_warn:last').append('<p>' + this + '</p>');
        }
    );
    $('#ui_warn .ui_warn:last').append('<a href="#" onclick="return ui_warn_dismis(this.parentNode);">Dismiss</a>');
}

function ui_warn_dismis (node)
{
    $(node).fadeOut('fast', function () 
                    {
                        $(this).remove();
                        if ($('#ui_warn').children().length == 0)
                            $('#ui_warn').remove();
                    });
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



function selectVillageURL (newSvid)
{
    url = 'VillageOverview.aspx?svid=';
    // newQuery will hold the values that ARE to be propergated
    newQuery = null;
    try {
        // grab the users current URL
        current = window.location.toString().split(/\?/);

        // break off the query
        url = current[0];
        query = current[1];

        // break apart the query, into name=value pairs
        query = breakQuery(query);

        // some URIs need certin paramitors stripped from thier query_string.
        if (url.match(/(UnitsSupporting.aspx|UnitsAbroad.aspx)/))
            newQuery = stripQuery(query, ['SiVID', 'SdVID', 'svid']);
        else
            newQuery = stripQuery(query, ['svid']);
    } catch (e) {
        ReportException('window.location', e.message, 'menus.js');
    }

    // Find the .aspx filename
    url = url.match(/([a-zA-Z_]+\.aspx)$/);
    if (url) {
        url = url[0];
    } else {
        url = '';
    }
    // For some URLs the village switch doesn't make much sense
    if (url.match(/createmail\.aspx/)) // URL/Query filter for 814
        newQuery = stripQuery(newQuery, ['recid']);

    newQuery = stripQuery(newQuery, ['svid']);
    newQuery.push( { 'name' : 'svid', 'value' : newSvid });
    
    return url + '?' + buildQuery(newQuery);
}

function selectVillage (newSvid)
{
    var url = selectVillageURL(newSvid);
    window.location = url;
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
    $('.stripeTable tr.highlight').hover(
        function () { $(this).addClass('stripeTableHover'); },
        function () { $(this).removeClass('stripeTableHover'); }
    );
    $('.stripeTable tr.highlight_d1').hover(
        function () { $(this).addClass('stripeTableHover');$(this).next().addClass('stripeTableHover'); },
        function () { $(this).removeClass('stripeTableHover');$(this).next().removeClass('stripeTableHover'); }
    );
    $('.stripeTable tr.highlight_u1').hover(
        function () { $(this).addClass('stripeTableHover');$(this).prev().addClass('stripeTableHover'); },
        function () { $(this).removeClass('stripeTableHover');$(this).prev().removeClass('stripeTableHover'); }
    );
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


function ReportException (name, message, file)
{
    alert(message);// DumperAlert([name, message, file]);
    $.ajax(
        {
            'type'  :   'POST',
            'url'   :   '/ReportException'
            //'data'  :   { 'name' : name, 'message' : 'message', file }
        }
    );
}

function NextPrevVillage ()
{
    if (!window.myVillages)
        return; // we're not on a standard load, so exit.
        
    // prepareMyVillagesArray() adds .url attributes to the village list
    prepareMyVillagesArray();
    
    // find the id of the current village, try cookies first, then parse the URL.
    if (!(svid = $.cookie('svid')))
        svid = parseInt(window.location.href.match(/svid=(\d+)/)[1], 10);
    
    if (myVillages[0].id == svid)
        $('.jPrevVillage').children('img').attr('src', 'https://static.realmofempires.com/images/leftArrow0.png');
    else
        $('.jPrevVillage').attr('href', myVillages[0].url);
    
    // find the last avalaible village
    last = myVillages.pop();
    if (!last)
        last = myVillages.pop();
        
    if (last.id == svid)
        $('.jNextVillage').children('img').attr('src', 'https://static.realmofempires.com/images/rightArrow0.png');
    else
        $('.jNextVillage').attr('href', last.url);
}

function genericMenusInit ()
{
    // for debugging
    //console.log('Init generic menus');

    // the "click off", and mouse away
    //$('body')/*.mouseover(bodyHover)*/.click(genericMenusClear);
    $(document).click(function (event) {
        if ($(event.target).closest("div.jsFakeSelect").length)
            return;

        genericMenusClear();
    });
	
	for (x in genericMenuConfig.menus) {
	    config = genericMenuConfig.menus[x][0];
	    $(config.select)
	        .addClass(config.menuType)
	        .attr('menu', x)
	        .attr('menuAlign', config.menuAlign);
	    if (config.stillClickable)
	        $(config.select).addClass('stillClickable');
	}

	
	
	
	$('.genericMenuStart')
	    .mouseout(genericMenuUnKeep)// function () {  genericMenuTimer('genericMenusClear();'); } )
	    .mouseover(genericMenu)
	    .click( function () { if ($(this).hasClass('stillClickable')) { return true; } return false; });
	    //.mousemove(genericMenu); //.click(genericMenu);
	$('.genericMenuClicker')
	    .mouseout(genericMenuUnKeep)
	    .click(genericMenu);
	$('.genericMenuFake')
	    .mouseover(genericMenusClear);
}

function genericMenuConvert (id)
{   
	$('#genericMenu_' + id)
	    .mouseover( 
	        function (){ 
	            //console.log("Mouse over on the menu! setting keep=yes");
	            $(this).attr('keep', 'yes');
	        })
	    .mouseout( 
	        function () { 
	            //console.log("Mouse Out on menu! Calling genericMenuTimer to set a MenusClear() timeout");
	            $(this).attr('keep', 'no');
	            genericMenuTimer('genericMenusClear();'); 
	        } );
}

function genericMenuUnKeep ()
{
    id = 'genericMenu_' + $(this).attr('menu');
    //console.log('Unkeep for %s!', id);
    
    $('#' + id).attr('keep', 'no'); // mark the menu as "Non-Keep"
    genericMenuTimer('genericMenusClear();'); // clear all non-keep menus
}

function bodyHover ()
{
	genericMenuTimer('genericMenusClear();');
}

function genericMenuTimer (command)
{
    //console.log("genericMenuTimer Called (%s)", command);
	if (!command) {
		// no command, call just wants to cancel a timer
		if (genericMenuConfig.timer) {
			clearTimeout(genericMenuConfig.timer);
			genericMenuConfig.timer = null;
		} else{
		}
	} else {
		// we have a command,
		if (!genericMenuConfig.timer) {
			// no timer set, so we can set one!
			genericMenuConfig.timer = setTimeout(command, genericMenuConfig.moveAwayDelay);
		}
	}
}

function genericMenusClearNow (id)
{
	$(".genericMenu, .ui_menu, .ui_menu_fast").attr('keep', 'no');
	$('#' + id).attr('keep', 'yes');
	
	genericMenusClear();
}

function genericMenusClearFast ()
{
    //alert("Found : " + $(".ui_menu_fast[keep!='yes']").length);
    $(".ui_menu_fast[keep!='yes']").hide();
    $(".ui_menu_fast").attr('keep', 'no');
    setTimeout(genericMenusClearFast_b, 10);
}
function genericMenusClearFast_b ()
{
    //alert("Found : " + $(".ui_menu_fast[keep!='yes']").length);
    $(".ui_menu_fast[keep!='yes']").hide();
}
function genericMenusClear ()
{
    // code for fading
    //if ($(".genericMenu[keep!='yes']").attr('fade') == 'none')
    //	$(".genericMenu[keep!='yes']").attr('fade', 'fast').fadeOut('fast', function () { $(this).attr('fade', 'done'); });
	$(".genericMenu[keep!='yes'], .ui_menu[keep!='yes']").hide();
	
	$(".genericMenu, .ui_menu").attr('keep', 'no');
	
	// cancel the timer (we were just called..)
	//console.log("Clear any non-keep UI menus, calling genericMenuTimer to cancel timers");
	genericMenuTimer(null);
}

function genericMenu(e) {

    var menu;
    //alert('genericMenu()');

    //console.log('genericMenu called. ClientX: %d, ClientY : %d', e.clientX, e.pageY);

    if (!(id = 'genericMenu_' + $(this).attr('menu'))) return;
    genericMenusClearNow(id);

    // find the right position
    p = $(this).position();
    p.o = $(this).height();
    p.r = $(this).width() - 2; //for some reason on IE, when vertical scroll bar is present, showed tools menu was slighly off screen to the right


    // check we have a cache, if we don't we need the build a menu
    if ($('#' + id).length == 0) { genericMenuCreate(id); }

    // mark the menu as "to keep", then show the menu.
    menu = $('#' + id)
    menu.attr('keep', 'yes').attr('fade', 'none').show();

    // make sure the ul stays the same width!
    $('ul', menu).css('width', $('#' + id + ' ul').width() + 'px');

    // check size constriants
    if ($('#' + id + ' ul').height() > genericMenuConfig.maxHeight)
        $('#' + id + ' .genericMenuInner').css({ width: ($('#' + id + ' ul').width() + 25) + 'px', height: genericMenuConfig.maxHeight + "px", overflow: 'auto' });

    // find the width of the menu and the document, and if needed calculate a new menu x co-ord.
    docWidth = $(document).width();
    menuWidth = menu.width();
    if ($(this).attr('menuAlign') == "right") {
        p.left = (p.left + p.r) - menuWidth;
    } else {
        if (p.left + menuWidth > docWidth) // IF the menu + it's left position is greater than the right and side we need to move the menu
            p.left = docWidth - (menuWidth + 2);
    }
    // reposition the menu
    if (menu.attr('fix') == 'done')
        return false;



    if (menu.attr('fix') == 'yes') {
        menu.css({ top: (p.top + p.o) + 'px', left: p.left + 'px', width: menu.width() + 'px' });
        menu.attr('fix', 'done');
    } else {
        menu.css({ top: (p.top + p.o) + 'px', left: p.left + 'px' });
    }
    return false;
}
function genericMenuCreate (id)
{
	var name = id.match(/_(.+)/)[1];
	var markup = genericMenuMarkup(name);
	
  	$('body').append('<div class="ui_menu genericMenu" style="display : none;" id="' + id + '"><div class="genericMenuInner"><ul>' + markup + '</ul></div></div>');
	
	
	// apply mouse overs 
	$('#' + id)
	    .mouseover( 
	        function (){ 
	            //console.log("Mouse over on the menu! setting keep=yes");
	            $(this).attr('keep', 'yes');
	        })
	    .mouseout( 
	        function () { 
	            //console.log("Mouse Out on menu! Calling genericMenuTimer to set a MenusClear() timeout");
	            $(this).attr('keep', 'no');
	            genericMenuTimer('genericMenusClear();'); 
	        } );
	    //.click( function () { return false; } );
}

function genericMenuData (name)
{
	// DumperAlert(genericMenuConfig.menus[name]);
	name = name.replace(/\d+$/, '');
	if (genericMenuConfig.menus[name])
		return genericMenuConfig.menus[name];
}
function genericMenuData_Help ()
{
    rid = $(".header_bar a.help").attr('href').match(/rid=(\d+)/)[1];
    menu = [
    		{ title : 'Beginner Tutorial', url : BuildURL('startTutorial.aspx', []), type : 'item'},
			{ title : 'Buildings', url : BuildURL('Help.aspx', ['rid=' + rid]), type : 'item' },
			{ title : 'Units', url : BuildURL('Help.aspx', ['rid=' + rid, 'HPN=Unit']), type : 'item' },
			{ title : 'Forum (Discussion board)', url : 'https://www.facebook.com/board.php?uid=10471770557', type : 'item', opts : { target : "_black" }, divider : true },
			{ title : 'Suggestions', url : 'https://www.facebook.com/edittopic.php?uid=10471770557&action=8', type : 'item', opts : { target : "_blank" } },
			{ title : 'Support / Report a bug', url : 'https://www.facebook.com/edittopic.php?uid=10471770557&action=8', type : 'item', opts : { target : "_blank" } },
			{ title : 'About Us', 
			  url : 'http://www.facebook.com/apps/application.php?id=10471770557', 
			  type : 'item', 
			  opts : { target : "_new" }, 
			  divider : true 
			},
			{ title : 'Terms Of Use', url : 'tou.aspx', type : 'item', opts : { target : "_new"} }
    ];
    return menu;
}

function genericMenuMarkup (name)
{
	if (name == "villages") {
		data = genericMenuData_Villages();
	/* replaced with click menu div } else if (name == "silver") {
	    data = []; */
	} else if (name == "help") {
	    data = genericMenuData_Help();
	} else {
		data = genericMenuData(name);
	}
	//DumperAlert(data);
	markup = '';
	
	// compile the data into markup
	for (i=0; i<data.length; i++) {
		extra = '';
		
		// for stuff like class="" and rel='x'
		if (data[i].opts)
			for (opt in data[i].opts)
				extra += ' ' + opt + '="' + data[i].opts[opt] + '"';

		// work out what to do with the item
		if (data[i].type == "item") {
			if (data[i].divider == true)
				markup += '<li class="divider">';
			else
				markup += '<li>';
				
			markup += '<a href="' + data[i].url + '"' + extra + '><span class="title">' + data[i].title + '</span>&nbsp;';
			if (data[i].extra)
				markup += '<small>' + data[i].extra + '</small>';
			
			markup += '</a></li>';
		} else if (data[i].type == "title") {
			markup += '<li id="' + itemID + '"><a><span class="title">' + this.title + '</span>&nbsp;</a></li>';
		} else if (data[i].type == "divider") {
			markup +='<div class="divider"><hr /></div>';
		} else if (data[i].type == "markup") {
			markup += '<li class="markup">' + data[i].markup + '</li>';
		}
	}
	
	return markup;
}

// function to make the village menu code
function genericMenuData_Villages ()
{
	menudata = [];
	
	// Summery is on the villages menu, no mather how many village you have
	menudata.push({ 'type' : 'markup', 'markup' : '<span class="summary">Summary: <a class="normal" href="VillageSummary_buildings.aspx">Buildings</a> | <a class="normal" href="VillageSummary_Units.aspx">Units</a></span>' });
	
	// defualt URL, failing all else..
	url = 'VillageOverview.aspx?svid=';
	
	// newQuery will hold the values that ARE to be propergated
    newQuery = null;

    try {
        // grab the users current URL
        current = window.location.toString().split(/\?/);

        // break off the query
        url = current[0];
        query = current[1];

        // break apart the query, into name=value pairs
        query = breakQuery(query);

        // some URIs need certin paramitors stripped from thier query_string.
        if (url.match(/(UnitsSupporting.aspx|UnitsAbroad.aspx)/))
            newQuery = stripQuery(query, ['SiVID', 'SdVID', 'svid']);
        else
            newQuery = stripQuery(query, ['svid']);
    } catch (e) {
        ReportException('window.location', e.message, 'menus.js');
    }
    
    // Find the .aspx filename
    url = url.match(/([a-zA-Z_]+\.aspx)$/);
    if (url) {
        url = url[0];
    } else {
        url = '';
    }
    // For some URLs the village switch doesn't make much sense
    if (url.match(/createmail\.aspx/)) // URL/Query filter for 814
        newQuery = stripQuery(newQuery, ['recid']);
	
	// for loop over the myVillages array
	for (v=0; v< myVillages.length; v++)
	{
		// skip if i isn't valid
		if (!myVillages[v])
		    continue;
		
		if (!myVillages[v].url) {
		    // build a Query
		    newQuery = stripQuery(newQuery, ['svid']);
            newQuery.push( { 'name' : 'svid', 'value' : myVillages[v].id });

		    // build a URL
		    myVillages[v].url = url + '?' + buildQuery(newQuery);
		}
		try {
		    menudata.push( { type 	: 'item', 
					         title	: myVillages[v].name + ' (' + myVillages[v].X + ',' + myVillages[v].Y + ')',
					         extra	: 'Points: ' + addCommas(myVillages[v].P) + ', Silver: ' + addCommas(myVillages[v].S),
					         url	: myVillages[v].url,
					         divider: v == 0 ? true : false});
	    } catch (e) {
	    }
	}
	// format : { 'id':7596,'name':'Alex village','X':'69','Y':'-12','P':'63','S':'20793'},
	//DumperAlert(menudata);
	return menudata;
}

function prepareMyVillagesArray ()
{
	// defualt URL, failing all else..
	url = 'VillageOverview.aspx?svid=';
	
	// newQuery will hold the values that ARE to be propergated
    newQuery = null;

    try {
        current = window.location.toString().split(/\?/);
        url = current[0];
        query = current[1];
        query = breakQuery(query);

        // some URIs need certin paramitors stripped from thier query_string.
        if (url.match(/(UnitsSupporting.aspx|UnitsAbroad.aspx)/))
            newQuery = stripQuery(query, ['SiVID', 'SdVID', 'svid']);
        else
            newQuery = stripQuery(query, ['svid']);
    } catch (e) {
        ReportException('window.location', e.message, 'menus.js');
    }
    
    // Find the .aspx filename
    url = url.match(/([a-zA-Z_]+\.aspx)$/);
    if (url) {
        url = url[0];
    } else {
        url = '';
    }
    // For some URLs the village switch doesn't make much sense
    if (url.match(/createmail\.aspx/)) // URL/Query filter for 814
        newQuery = stripQuery(newQuery, ['recid']);
	
	// for loop over the myVillages array
	for (v=0; v< myVillages.length; v++)
	{
		// skip if i isn't valid
		if (!myVillages[v])
		    continue;
		
		if (!myVillages[v].url) {
		    // build a Query
		    newQuery = stripQuery(newQuery, ['svid']);
            newQuery.push( { 'name' : 'svid', 'value' : myVillages[v].id });

		    // build a URL
		    myVillages[v].url = url + '?' + buildQuery(newQuery);
		}
	}
}

function reloadParent ()
{
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
    closeModalIFrame(n, p); windowReload();
}

function popupResearch(fullQuerry)
{
    return !popupModalIFrame("research.aspx" + (fullQuerry ? fullQuerry :""), "Research", '', 500, 680, 20, 40, 'closeAndReload');
}
function popupPlayerOverview(url, targetPlayerName)
{
    return !popupModalIFrame(url, targetPlayerName.replace(/\./gi, '_') + Math.floor(Math.random() * 1000), '', 500, 680, 20, 40);
}

function popupPlayerOverviewByID(link, pid, bPositionHeightByLink, height, width) {
    var url = $(link).attr('href');
    var x = 20;
    var y = 40;
    if (bPositionHeightByLink) {
        y = $(link).offset().top;
    }
    height = height || 500;
    width = width || 680;
    return !popupModalIFrame(url, pid.toString() + Math.floor(Math.random() * 1000), '', height, width, x, y);
}

function popupVilageOverview(url, targetVID)
{
    return !popupModalIFrame(url, targetVID + Math.floor(Math.random() * 1000), '', 500, 680, 20, 40);
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


function popupAvatarSelect()
{
    return !popupModalIFrame("Avatar_Select.aspx", "Selectyouravatar", '', 500, 680, 20, 40);
}

function popupModalPopup(name, title, height, width, x, y, f) {

    $('body').append('<div class="popup_modal" style="height: 100%; width: 100%; position: fixed; left: 0pt; top: 0pt; z-index: 2999; opacity: 0.5; filter: alpha(opacity=50); background-color: black;"></div>');

    height = height ? height : ROE.Frame.CONSTS.popupDefaultHeight;
    width = width ? width : ROE.Frame.CONSTS.popupDefaultWidth;
    x = x ? x : ROE.Frame.CONSTS.popupDefaultX;
    y = y ? y : ROE.Frame.CONSTS.popupDefaultY;
    f = f ? f : 'closeModalPopup';

    return popupPopup(name, title, height, width, x, y, f);
}


function popupPopup(name, title, height, width, x, y, func) {

    // construct a unique-ish id
    var id = ROE.Frame.CONSTS.popupNameIDPrefix + name;

    // create the div, if it doesn't exist!
    if ($('#' + id).length > 0) {
        $('#' + id)
            .show()                // display the (posiably) hidden iframe div
            .attr('keep', 'yes');   // flag it as active
    } else {
        $('body').append('<div id="' + id + '" class="iFrameDiv" style="position:absolute; height:' + (height + 18) + 'px; width:' + width + 'px;z-index: 3000;"></div>');
        $('#' + id)
            .css({ top: y + 'px', left: x + 'px' })
            .append('<div class="IFrameDivTitle" style="background:none repeat scroll 0 0 #292116;height:18px;text-align:left;width:100%;"><span class="title" style="color:#C39037;float:left;font-size:14px;font-weight:bold;padding:0 2px;">' + title + '</span><img style="background:none repeat scroll 0 0 transparent;border:medium none;cursor:pointer;display:inline;float:right;margin:0;" id=imgIframeClose onclick="return !' + (func ? func : 'closeIFrame') + '(\'' + name + '\', true);" src="https://static.realmofempires.com/images/delete.gif" /></div>')
            .append('<div class="popupBody" ></div>')
            .show()
            .attr('keep', 'yes')    // so that it's not instantly cleaned up.
            .mouseover(function () { $(this).attr('keep', 'yes'); }); // flag as active when it's hovered
    }

    return true;
}