<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="Main" %>

<%@ Register Src="Controls/VillageHeaderInfo.ascx" TagName="VillageHeaderInfo" TagPrefix="uc2" %>
<!doctype html>
<!--[if lt IE 7]><html class="no-js lt-ie10 lt-ie8 lt-ie7" lang="en"><![endif]-->
<!--[if lt IE 8]><html class="no-js lt-ie10 lt-ie8" lang="en"><![endif]-->
<!--[if lt IE 10]><html class="no-js lt-ie10" lang="en"><![endif]-->
<!--[if gte IE 10]><!-->
<html class="no-js" lang="en">
<!--<![endif]-->
<head id="Head1" runat="server">
    <title><%= RSc("GameName") %></title>
    <link rel="shortcut icon" href="https://static.realmofempires.com/images/icons/favicon.ico"/>
    <meta charset="utf-8" />

    <script>

        // IE Check
        function isIE()
        {
            var ua = window.navigator.userAgent.toLowerCase();

            //var ua = window.navigator.userAgent;
            var msie = ua.indexOf('msie ');
            var trident = ua.indexOf('trident/');

            if (msie > 0)
            {
                // IE 10 or older => return version number
                return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
            } else if (trident > 0)
            {
                // IE 11 (or newer) => return version number
                var rv = ua.indexOf('rv:');
                return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
            }

            // other browser
            return -1;
        }

        var ieVer = isIE();
        if (ieVer != -1)
        {
            //alert("Our appologies, but while we are in the early Beta version, Internet Explorer is not supported. Chrome, Firefox and Safari are supported, and are available for free download");
            if (ieVer < 10)
            {
                // Kick the user to old version if less than 10
                window.location.href = "chooserealmd2.aspx?logout=1";
            }
        }



        <%if (String.IsNullOrEmpty(FacebookConfig.DisconnectedFromFacebookUserID)) {%>
        /*function inIframe () {
            try {
                return window.self !== window.top;
            } catch (e) {
                return true;
            }
        }
        if (inIframe()){
            window.parent.location = self.location;
        }*/
        <%}%>


    </script>
    <%
        //a list of beta features.
        List<String> BetaFeatures = new List<String> { "QB:ShowMaxedBuildings", "AdvancedReportList", "DesktopNotifications","AudioTools" };
        // if you want to remove a beta feature, and have it be active by default, list is in DepreciatedBetaFeatures
        List<String> DepreciatedBetaFeatures = new List<String> { };
        dynamic BetaCookies = new System.Dynamic.ExpandoObject();
        var dict = (IDictionary<string, object>)BetaCookies;
        BetaFeatures.ForEach(delegate(String name)
        {
            HttpCookie cookie = Request.Cookies[name];
            if (cookie != null)
            {
                dict[name] = cookie.Value;
            }
        });       
        DepreciatedBetaFeatures.ForEach(delegate(String name)
        {
            dict[name] = "ON";
        });       
    %>
    <% 
        bool indev = Request.Cookies["indev"] == null ? Config.InDev : Request.Cookies["indev"].Value == "true";
    %>

    

    
    <script type="text/javascript">

        var BetaFeatures = {

            //add your feature name to this array, remember to update the C# list above as well! :)
            FeaturesArray: ["QB:ShowMaxedBuildings", "AdvancedReportList", "DesktopNotifications", "AudioTools"],

            initNullCheck: function ()
            {
                var cookieValue;
                for (var i = 0; i < BetaFeatures.FeaturesArray.length; i++)
                {
                    cookieValue = $.cookie(BetaFeatures.FeaturesArray[i]);
                    if (cookieValue == "" || cookieValue != "ON")
                    {
                        $.cookie(BetaFeatures.FeaturesArray[i], 'OFF');
                    }
                }
            },

            //use BetaFeatures.status('featurename'); to check if feature is on or off in cookies
            //for example: if(BetaFeatures.status('quickBuild') == 'ON'){ /*do smething here*/ }
            status: function (featureName)
            {
                if ($.inArray(featureName, BetaFeatures.FeaturesArray) > -1)
                {
                    return $.cookie(featureName);
                } else
                {
                    return "ON"; //if feature not listed in array, return ON
                }
            },

            displayPanel: function ()
            {
                featurePanel = $('#betaFeaturePanel');
                featurePanel.find('.itemRow').remove();
                var itemRow, status, featureName;
                for (var i = 0; i < BetaFeatures.FeaturesArray.length; i++)
                {
                    featureName = BetaFeatures.FeaturesArray[i];
                    status = BetaFeatures.status(featureName);
                    itemRow = $('<div>').addClass('itemRow ' + status).html(featureName + " is " + status)
                        .attr('data-feature', featureName).attr('data-status', status)
                        .click(function ()
                        {
                            if ($(this).attr('data-status') == "ON")
                            {
                                $(this).removeClass("ON").addClass("OFF").attr('data-status', "OFF");
                            } else
                            {
                                $(this).removeClass("OFF").addClass("ON").attr('data-status', "ON");
                            }
                            $(this).html($(this).attr('data-feature') + " is " + $(this).attr('data-status'));
                            $.cookie($(this).attr('data-feature'), $(this).attr('data-status'));
                            $('#betaFeaturePanel').addClass('refreshOnClose');
                        });
                    itemRow.appendTo(featurePanel);
                }
                featurePanel.show();
            },

            hidePanel: function ()
            {
                if ($('#betaFeaturePanel').hasClass('refreshOnClose'))
                {
                    ROE.Frame.reloadTheWindow();
                } else
                {
                    $('#betaFeaturePanel').hide();
                }

            },

        }

        BetaFeatures.initNullCheck();




    </script>

    <%=Fbg.Common.WebHelper.FileList.js(Fbg.Common.WebHelper.FileList.FOR.mainPageDesktop2, false, indev, false, BetaCookies, IsTesterRoleOrHigher) %>


    <script type="text/javascript">
        try
        {
            <asp:Literal runat=server ID="lblJSONStruct"></asp:Literal>   
            <asp:Literal runat="server" ID="roeentities" EnableViewState=false></asp:Literal>
        } catch (e)
        {
            var roeex = new BDA.Exception("Error creating ROE.Entities:" + e.message);
            BDA.latestException = roeex;
            BDA.Console.error(roeex);
            throw roeex;
        }
    </script>


    <%=Fbg.Common.WebHelper.FileList.css(Fbg.Common.WebHelper.FileList.FOR.mainPageDesktop2, false, indev, BetaCookies) %>



    <link type="text/json" rel="help" href="static/help/j_Global.json.aspx" />


   

    <!-- FONT LINK REFERENCE SECTION-->
    <link href='https://fonts.googleapis.com/css?family=IM+Fell+French+Canon' rel='stylesheet' type='text/css'>
    <link href='https://fonts.googleapis.com/css?family=IM+Fell+French+Canon+SC' rel='stylesheet' type='text/css'>
    <link href='https://fonts.googleapis.com/css?family=Playfair+Display' rel='stylesheet' type='text/css'>
    <link href='https://fonts.googleapis.com/css?family=Droid+Sans+Mono' rel='stylesheet' type='text/css'>


</head>

<body class="view-vov desktop">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
            <Services>
                <asp:ServiceReference Path="Troops.asmx" />
            </Services>
        </asp:ScriptManager>
        <uc2:VillageHeaderInfo ID="ctrlVillageHeaderInfo" runat="server" Visible="false" />
        
        <div class="TDContent mainView"></div>

        <div id="vovFrame" data-scale="1">

            <!-- ROE-Advisor -->
            <div id="advisorMiniPanelArea" class="hide"></div>
            <div id="advisorLaunchIcon" class="zero" data-toolTipID="advisorLaunchIcon"></div>


            <!-- VOV Header Styling-->
            <div class="vovFrameHeader">
                <div class="headerL"></div>
                <div class="headerM"></div>
                <div class="headerR"></div>
                <div class="headerOverlay"></div>
                <div class="shield"></div>
                <div class="nameCordWrapper helpTooltip" data-toolTipID="vovNameCoordsPoints" onclick="ROE.UI.Sounds.click(); ROE.Utils.PopupRenameVillage();">
                    <span class="villName fontGoldFrLCmed"></span>
                    <span class="villCords fontDarkGoldNumbersSm"></span>
                    <span class="villPoints fontDarkGoldNumbersSm"></span>
                </div>
                <div class="toggleSize helpTooltip" data-toolTipID="vovToggleSize"></div>
            </div>

            <!-- VOV Body Styling-->
            <div class="vovFrameStyleBody">
                <div class="contentBody"></div>
                <div class="contentLT"></div>
                <div class="contentLM"></div>
                <div class="contentLB"></div>
                <div class="contentBottom"></div>
                <div class="contentRT"></div>
                <div class="contentRM"></div>
                <div class="contentRB"></div>
            </div>

            <!-- VOV Content Wrapper-->
            <div class="vovFrameContent">

                <!-- VOV-->
                <div id="currentVOV"></div>


                <!-- VOV Foooter Box-->
                <div id="currentVillageInfo">
                    <div class="navToPrev spLargeArrowL ButtonTouch helpTooltip" data-toolTipID="vovBtnPrevVill"></div>
                    <div class="navToNext spLargeArrowR ButtonTouch helpTooltip" data-toolTipID="vovBtnNextVill"></div>
                    <div class="popVillageList ButtonTouch helpTooltip" data-toolTipID="vovBtnVillList"></div>
                    <div class="centerVillage ButtonTouch helpTooltip" data-toolTipID="vovBtnCenterVill"></div>

                    <div class="coinsWrapper helpTooltip" data-toolTipID="vovCoinWrapperLive">
                        <div class="currCoins fontSilverNumbersSm realtimeSilverUpdate" data-vid="0" data-checkmax="1"></div>
                        <div class="maxCoins fontGrayNumbersSm"></div>
                    </div>
                    <div class="foodWrapper helpTooltip" data-toolTipID="vovFoodWrapperLive">
                        <div class="currFood fontSilverNumbersSm"></div>
                    </div>
                    <div class="loyaltyWrapper helpTooltip" data-toolTipID="vovLoyaltyWrapper">
                        <div class="currLoyalty fontSilverNumbersSm"></div>
                    </div>

                    <div class="troopsPanel fontGrayNumbersSm">
                        <table>
                            <tr class="troopRow">
                                <td class="troop"><span class="tN">0</span></td>
                                <td class="troop"><span class="tN">0</span></td>
                                <td class="troop"><span class="tN">0</span></td>
                                <td class="troop"><span class="tN">0</span></td>
                                <td class="troop"><span class="tN">0</span></td>
                                <td class="troop"><span class="tN">0</span></td>
                                <td class="troop"><span class="tN">0</span></td>
                                <td class="troop"><span class="tN">0</span></td>
                            </tr>
                        </table>

                    </div>

                </div>

            </div>
                
            <!-- VOV Alerts Area -->
            <div class="vovAlertsBox">
                <div class="vovAlert vov-inout-alert inc sfx2"></div>
                <div class="vovAlert vov-inout-alert out sfx2"></div>
                <div class="vovAlert beginerProtectionTimer helpTooltip" data-toolTipID="beginerProtection">New player protection: <span class="protCountdown countdown"></span></div>
            </div>

            <!-- VOV Action Buttons -->
            <div id="vovActionButtons">
                <div class="actionButton bz helpTooltip" data-toolTipID="vovBtnBazaar" onclick="ROE.Frame.popupGifts(ROE.SVID);" ></div>
                <div class="actionButton bu helpTooltip" data-toolTipID="vovBtnBuild" onclick="ROE.Frame.showBuildPopup();">
                    <div class="qbBtnCountdown"></div>
                </div>
                <div class="actionButton st helpTooltip" data-toolTipID="vovBtnTransport" onclick="ROE.UI.Sounds.click(); ROE.Frame.showSilverTransportPopup(ROE.SVID,true);"></div>
                
                <div class="actionButton qb helpTooltip" data-toolTipID="vovBtnQuickBuild" onclick="ROE.UI.Sounds.click(); ROE.QuickBuild.showPopup('vov',ROE.SVID);">
                    <div class="qbBtnCountdown"></div>
                </div>
                <div class="actionButton qr helpTooltip" data-toolTipID="vovBtnQuickRecruit" onclick="ROE.UI.Sounds.click(); ROE.QuickRecruit.showPopup('vov',ROE.SVID);"></div>                   
                <div class="actionButton bonusIcon helpTooltip" data-toolTipID="vovBtnBonusIcon" onclick="ROE.BonusVillageChange.showPopup();"></div>
                
            </div>



        </div>
                
        <div class="ui-panel ui-panel-main">
            <div class="ui-buttonpanel-main">
                <div class="inout-alert">
                    <div class="outgoing-alert helpTooltip sfx2" data-toolTipID="outgoingTroops"></div>
                    <div class="incomming-alert helpTooltip sfx2" data-toolTipID="incommingTroops"></div>
                </div>                
                <div class="buttonWrapper"><div class="inOutTroops button ButtonTouch helpTooltip sfx2" data-toolTipID="incommingTroops"></div></div>
                <div class="buttonWrapper"><div id="linkReports" class="reports button ButtonTouch sfx2 helpTooltip" data-toolTipID="linkReports"></div></div>
                <div class="buttonWrapper"><div id="linkMail" class="mail button ButtonTouch sfx2 helpTooltip" data-toolTipID="linkMail"></div></div>
                <div class="buttonWrapper"><div id="linkClan" class="clan button ButtonTouch sfx2 helpTooltip" data-toolTipID="linkClan"></div></div>
                <div class="buttonWrapper"><div id="linkRankings" class="rankings button ButtonTouch sfx2 helpTooltip" data-toolTipID="linkRankings"></div></div>
                <div class="buttonWrapper"><div id="linkMassActions" class="massActions button ButtonTouch sfx2 helpTooltip" data-toolTipID="linkMassActions"></div></div>
                <div class="buttonWrapper"><div id="linkSettings" class="settings button ButtonTouch sfx2 helpTooltip" data-toolTipID="linkSettings"></div></div>
            </div>
            <div class="subpanel-left">
                <div class="player-avatar-frame helpTooltip"  data-toolTipID="playerProfile" data-playerName="" onclick="ROE.UI.Sounds.click(); ROE.Frame.popupPlayerProfile($(this).attr('data-playerName'));">
                    <div class="player-avatar-picture" style="background-image:url('<%= FbgPlayer.Avatar.ImageUrlS %>');"></div>
                </div>
                <div runat="server" id="hdrPName" class="player-name hdrPName fontSilverFrLCmed"></div>
                <div class="player-ranking"><span class="player-level" style="color: #808080;"></span> <span class="player-title fontSilverFrLCmed"></span></div>
                <div class="player-ranking-progress fontSilverNumbersSm">
                    <div class="progress-bg">
                        <div class="progress-container">
                            <div class="progress-indicator"></div>
                        </div>
                        <span runat="server" id="Span1" class="hdrPoints"></span>
                    </div>
                </div>
                <a href="throneroom.aspx" target="_blank" class="toTheForums hoverScale helpTooltip sfx2" data-toolTipID="throneLink"></a>
            </div>
            <div class="subpanel-mid">
                 <div class="pfStatus"></div>
            </div>
            <div class="subpanel-right">
                <a href="#" class="buyServants sfx2 hoverScale helpTooltip" data-toolTipID="buyServants" onclick="ROE.Frame.showBuyCredits();" ></a>
                <div class="playerCredits hdrFont helpTooltip" data-toolTipID="creditCount"></div>
            </div>
        </div>
        


        <%if (IsTesterRoleOrHigher)
          { %>
       
        <%} %>




        <div id="roeOptions">


            <%if (IsTesterRoleOrHigher)
              { %>

            <%} %>
            <%if (!(FbgUser.LoginType == Fbg.Common.UserLoginType.FB || FbgUser.LoginType == Fbg.Common.UserLoginType.FB_inferred)) {%>
            <%} %>
            <%if (FbgUser.LoginType == Fbg.Common.UserLoginType.FB || FbgUser.LoginType == Fbg.Common.UserLoginType.FB_inferred) {%>
            <%} %>
            <div class="section toggleAnimationSettings hoverScale sfx2 helpTooltip" data-toolTipID="toggleAnimationSettings" onclick="ROE.Animation.openSettingsUI();"></div>
            <div class="section zoom out hoverScale sfx2 helpTooltip" data-toolTipID="zoomOut" onclick="ROE.Landmark.zoom(-0.25);"></div>
            <div class="section zoom in hoverScale sfx2 helpTooltip" data-toolTipID="zoomIn" onclick="ROE.Landmark.zoom(0.25);"></div>
            <div class="section movetoCoord hoverScale sfx2 helpTooltip" data-toolTipID="mapToCords"></div>
            <div class="section direction hoverScale sfx2 helpTooltip" data-toolTipID="setDirection" data-troops=""></div>
            <div class="section mapsummary hoverScale sfx2 helpTooltip" data-toolTipID="mapSummary" onclick="ROE.Frame.popupMapSummary(event)"></div>
            <div id="linkBattleSim" class="section battleSim hoverScale helpTooltip" data-toolTipID="linkBattleSim">
                <a target="_blank" href="battlesimulator.aspx" class="sfx2"></a>
            </div>
           
        </div>


        <div id="tutorial">
            <div class="dialog" style="display: none;">
                <section>

                    <div class="tutbody">
                        <div class="textwrite"></div>

                        <a class="next BtnBSm2 sfx2" href="#" style="visibility: visible;">Next</a>
                        <a class="skip BtnBSm2 sfx2" href="#" style="display: none;">Skip</a>

                        <div class="dontShow"><input type="checkbox" style="vertical-align: -2px;"><span></span></div>

                        <a class="tutclose" href="#" onclick="if(confirm('My Liege! You are almost done! The people depend on you - I beg you, finish this tutorial. STILL QUIT?')) { ROE.Tutorial.ended(); return true; } return false;"></a>
                        <div style="clear: both;"></div>
                    </div>

                </section>
            </div>

            <div class="pointer">
                <div class="pointer-circle">&nbsp;</div>
                <img src="https://static.realmofempires.com/images/misc/AdvArrowStraight.png" class="pointer-img" />
            </div>
        </div>



        <div id="mainCommPanel">

            <div id="networkerrors">
                <div class="error networkerrors_slow">Detected slow internet connection. <a class="Networkerrors_dismiss" onclick="$('.networkerrors_slow').fadeOut();">dismiss</a></div>
                <div class="error networkerrors_noconnection">You appear to have lost internet connection. Trying again... <a class="Networkerrors_dismiss" onclick="$('.networkerrors_noconnection').fadeOut();">dismiss</a></div>
            </div>

            <div id="freeFloatWrapper">
                <div class="ffLeftCap"></div>               
                <!-- Icons above chat go here -->
                <div id="freeFloatIconsArea">

                    <div id="launchRaidsPopup" class="ButtonTouch sfx2 freeFloatIcon helpTooltip" data-toolTipID="floatRaids" ></div>
                    <div id="launchTargetsListSupport" data-type="1" class="ButtonTouch sfx2 freeFloatIcon launchTargetsListIcon helpTooltip" data-toolTipID="launchTargetsListSupport" style="display: none;"></div> 
                    <div id="launchTargetsListAttack"  data-type="2" class="ButtonTouch sfx2 freeFloatIcon launchTargetsListIcon helpTooltip" data-toolTipID="launchTargetsListAttack" style="display: none;"></div> 
                    <div id="linkItems" class="ButtonTouch sfx2 freeFloatIcon helpTooltip" data-toolTipID="floatRewards" data-completedQuestCount=""><span id="Span2"></span></div>
                    <%if (!FbgPlayer.Realm.IsTemporaryTournamentRealm) { %>
                    <div id="linkQuests" class="ButtonTouch sfx2 freeFloatIcon helpTooltip" data-toolTipID="floatQuests" data-completedQuestCount=""><span id="linkQuests_completedCount"></span></div>
                    <%} %>
                    <%if (FbgPlayer.Realm.NewDailyReward) { %>
                    <div class="dailyReward freeFloatIcon helpTooltip" data-toolTipID="floatDailyreward" style="display: none; background-image: url('https://static.realmofempires.com/images/icons/dailyBonus.png');background-size: 100% 100%;"><span class="counter"></span></div>
                    <% }%> 
                    <% if (FbgPlayer.Realm.Research.IsResearchActive 
                           && FbgPlayer.Realm.RealmType != "CLASSIC" // HACK-should be based on if MORE research items are available  
                          && !FbgPlayer.Realm.IsTemporaryTournamentRealm)                          
                       { %> 

                    <div id="viewSwitch_res" class="freeFloatIcon ButtonTouch helpTooltip" data-toolTipID="floatResearch"><span class="counter" data-refreshcall="ROE.Frame.researchersIdleInCountdownAtZero();"></span></div>
                    <% }%>     
                    <div class="selectGovType effect-bounce-loud freeFloatIcon helpTooltip" data-toolTipID="floatGovselect" style="display: none; background-image: url('https://static.realmofempires.com/images/icons/choosegov.png');background-size: 100% 100%;" onclick="ROE.UI.GovTypeSelect.init();"></div>
                    <% if (!FbgPlayer.BeginnerProtection_IsNowActive || (FbgPlayer.BeginnerProtection_ExpiresInDays < 2))                          
                       { %>                
                    <div id="sleepMode" class="ButtonTouch sfx2 freeFloatIcon helpTooltip" data-toolTipID="floatSleepmode"  ><span class="counter" data-refreshCallDelay="1" data-refreshcall="windowReload()" ></span></div>                         
                    <%} %>
                    <%if (FbgPlayer.Realm.IsTemporaryTournamentRealm) { %>
                    <div id="RXIcon" class="freeFloatIcon helpTooltip" data-toolTipID="rxCountdown" style="background-image: url('https://static.realmofempires.com/images/icons/RXicon44x44.png');background-size: 100% 100%;"><span class="counter countdown" data-finisheson="<%=Fbg.Bll.Api.ApiHelper.SerializeDate(FbgPlayer.Realm.ClosingOn)%>"></span></div>
                    <%} %>
                    <%if (!FbgPlayer.Realm.IsTemporaryTournamentRealm) { %>
                    <div id="emailEntryIcon" class="freeFloatIcon emailEntryIcon" style="display:none;"></div>    
                    <% }%>
                    <%if (FbgUser.HasFlag(Fbg.Bll.User.Flags.HideMobileIconFromMainUI) == null
                          && !IsLoggedInAsSteward 
                          && !FbgPlayer.Realm.IsTemporaryTournamentRealm)                          
                      { %>
                        <div id="playOnMinfo" class="freeFloatIcon" style="display:none;"><div class="innerIcon" data-iconState="1"></div></div>
                    <%} %>    
                    <% if (LoginModeHelper.isFB(Request) && !FbgPlayer.Realm.IsTemporaryTournamentRealm) { %>
                    <div id="socialInvite" class="ButtonTouch sfx2 freeFloatIcon helpTooltip" data-toolTipID="floatSocialInvite" style="display:none;"></div>
                    <% }%> 
                    <%if (AllowFriendReward){%><div class="freeFloatIcon inviteFriendsReward"></div><%} %>
                    <div runat="server" id="mobAppTryGift" visible="false" class="mobAppTryGift effect-pulse freeFloatIcon helpTooltip" data-toolTipID="floatTrygifts" style="background-image: url('https://static.realmofempires.com/images/icons/M_FreeServants.png');"></div>
                    <div runat="server" id="mobAppTryGift_AG" visible="false" class="mobAppTryGift_AG effect-pulse freeFloatIcon helpTooltip" data-toolTipID="floatTrygifts" style="background-image: url('https://static.realmofempires.com/images/icons/M_FreeServants.png');"></div>
                    <div runat="server" id="Offer2" visible="false" class="Offer2 freeFloatIcon helpTooltip" data-toolTipID="floatFreeservants" style="display: none; background-image: url('https://static.realmofempires.com/images/icons/M_FreeServants.png');"></div>
                    <div class="setMusic ButtonTouch sfx2 freeFloatIcon " style="background-image: url(https://static.realmofempires.com/images/icons/M_MoreMusicOn.png);display:none"></div>
                </div>
                <div class="ffRightCap"></div>
            </div>
        </div>

        <div class="blockscreens">
            <div class="whole"></div>
            <div class="part part1" style="top: 0px; right: 0px;"></div>
            <div class="part part2" style="bottom: 0px;"></div>
            <div class="part part3" style="top: 0px; bottom: 0px; right: 0px;"></div>
            <div class="part part4" style="top: 0px; right: 0px;"></div>
        </div>

        <div id="busy" style="display: none;">
            <div id="busy-content">
                <img src="https://static.realmofempires.com/images/misc/ajax-loader1.gif" /><br />
                <span id="busy-msg-default">working...</span>
                <span id="busy-msg-custom" style="display: none;"></span><br />
                <a id="busy-refresh" href="#"><br><br>        
                    <div class="crashMsg" onclick="ROE.Frame.reloadTheWindow();">Oops! This is taking a while...<br>Tap to refresh if you feel it's stuck!</div>
                    <%if (IsTesterRoleOrHigher) { %><% }%> 
                </a>            
            </div>
        </div>

        <div id="popupInfo"></div>

        <div id="popupInfoTemp">
            <div class="popupInfo_box">
                <div class="popupInfo_header"><span></span>
                    <div class="popupInfo_close pressedEffect sfxMenuExit"></div>
                </div>
                <div class="popupInfo_content"></div>
            </div>
        </div>


        <script type="text/javascript">
    
            $(function () {
                if (!BDA.Database.LocalGet('TroopDirection')) { BDA.Database.LocalSet('TroopDirection', 'outgoing'); }
                $('#roeOptions .direction').attr("data-troops", BDA.Database.LocalGet('TroopDirection'));
            });

            
        </script>

        <!-- Game UI Main Frame  -->
        <div class="mainFrame border borderTop"></div>
        <div class="mainFrame border borderRight"></div>
        <div class="mainFrame border borderBottom"></div>
        <div class="mainFrame border borderLeft"></div>
        <div class="mainFrame corner cornerTL"></div>
        <div class="mainFrame corner cornerTR"></div>
        <div class="mainFrame corner cornerBR"></div>
        <div class="mainFrame corner cornerBL"></div>

        <!-- Game UI Footer -->
        <div class="mainFooter">

            <div class="realmsArea">

                <!-- Log Out Button-->
                <a class="switchRealm helpTooltip" data-toolTipID="switchRealm" href="LogoutOfRealm.aspx?isM=1" target="_self"></a>            
                
                <!-- Current Realm info + Ages area-->
                <div class="currentRealmContainer">
                    <div class="currentRealm">Current Realm <span class="currentRealmNumber"><asp:Label ID="lblCurRealm" runat="server" Visible="false" /></span></div>
                    <div id="agesArea" runat="server" class="ButtonTouch sfx2 helpTooltip" data-tooltipid="realmAge" 
                        onclick="ROE.Frame.popGeneric('Age information','',840,650); ROE.Frame.showIframeOpenDialog('#genericDialog','currentAgeInfo.aspx');">
                        <span style="float: left;">is in</span>
                        <div runat="server" id="currentAge" class="ageIcon"></div>
                        <span id="nextAgeArea" runat="server">
                            <span runat="server" id="timeTillNextAge" class="countdown" format="long2" style="float: left;"></span>
                            <span style="float: left; margin-left: 4px;">to</span>
                            <div runat="server" id="nextAge" class="ageIcon"></div>
                        </span>
                    </div>
                </div>

            </div>

            <!-- AoC Area -->
            <asp:Panel id="nextAOC" runat="server" CssClass="nextAOC" visible="false">                    
                <div class="ButtonTouch sfx2 helpTooltip" data-tooltipid="nextAOC" 
                    onclick="ROE.Frame.popGeneric('Age of Cities information','',840,650); ROE.Frame.showIframeOpenDialog('#genericDialog','currentAgeInfo_AOC.aspx');">
                    <asp:Label ID="lblNextAOC" runat="server" Text="AoC in "></asp:Label><span runat="server" id="timeTillAOC" class="countdown" format="long2"> </span>                         
                </div>             
            </asp:Panel>

            <!-- Realm switching links -->
            <div class="listedRealms"><span runat="server" id="spanRealmSwitch"></span></div>

            <!-- Admin Button-->
            <%if (IsTesterRoleOrHigher) { %><%} %>
            
            <!-- Kingdom Stewardship Button -->
            <%if (FbgPlayer.Realm.StewardshipType != Fbg.Bll.Realm.StewardshipTypes.NoStewardShip) { %> <a class="stewBtn fontGoldFrLCmed" href="#" onclick="ROE.Frame.popupStewardship();">Stewardship</a><%} %>
            
            <!-- Stew Exit -->
            <a href="LogoutOfRealm.aspx" class="fontGoldFrLCmed" style="margin-left: 10px;" runat=server id=SwitchRealm></a>            

            <!-- Footer Right Section -->
            <div class="footerRightSection">
                <asp:LinkButton ID="LinkButton1" OnCommand="btnSwitchToDesktop_onCommand" class="switchToOldDesktop helpTooltip fontGoldFrLCmed" data-toolTipID="switchToOldD" runat="server">Old UI</asp:LinkButton>
                <div class="socialMediaContainer">
                    <a class="socialMediaBtn TWI" href="https://twitter.com/RealmOfEmpires" target="_blank"></a>
                    <a class="socialMediaBtn FB" href="https://www.facebook.com/Realm-Of-Empires-310594832310078" target="_blank"></a>
                    <a class="socialMediaBtn YTB" href="https://www.youtube.com/channel/UCSTzvWwBUPTZmA3-RFxBApQ" target="_blank"></a>
                </div>
                <span id="hdrServerTime" class="serverTime Time TimeAndDate help fontSilverNumbersMed helpTooltip" data-tooltipid="serverTime" data-offset="0" data-showtoday="false"></span>
            </div>  
                                    
        </div>

    </form>

    <div id="preloadimages" style="display: none;">
        <img src='https://static.realmofempires.com/images/misc/M_MoreColumn.jpg' />
        <img src='https://static.realmofempires.com/images/misc/M_MoreGarg.png' />
        <img src='https://static.realmofempires.com/images/icons/m_switchrealm.png' />
        <img src='https://static.realmofempires.com/images/icons/M_Rucksack.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreSettings.png' />
        <img src='https://static.realmofempires.com/images/icons/m_Ranking.png' />
        <img src='https://static.realmofempires.com/images/icons/m_battleSim.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreSleep.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreMusicOn.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreMusicOff.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreSoundOff.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreSoundOn.png' />
        <img src='https://static.realmofempires.com/images/misc/M_MoreTime.png' />
        <img src='https://static.realmofempires.com/images/misc/yesGreen.PNG' />
        <img src='https://static.realmofempires.com/images/icons/M_IcoCancel.png' />
        <img src='https://static.realmofempires.com/images/misc/m_listbar.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreCoord.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreSummary.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreZoomIn.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreZoomout.png' />
        <img src='https://static.realmofempires.com/images/misc/M_MoreZoomBG.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreInc.png' />
        <img src='https://static.realmofempires.com/images/icons/M_MoreOut.png' />
    </div>

    <!--<div id="avatarDialog" class="popupDialogs"></div>-->
    <div id="genericDialog" class="popupDialogs"></div>
    <div id="rxInfoDialog" class="popupDialogs"></div>
    <div id="playOnMDialog" class="popupDialogs"></div>
    <div id="inviteFriendsRewardDialog" class="popupDialogs"></div>
    <div id="buildDialog" class="popupDialogs"></div>
    <div id="buildingDialog" class="popupDialogs"></div>
    <div id="quickBuildDialog" class="popupDialogs"></div>
    <div id="quickRecruitDialog" class="popupDialogs"></div>
    <div id="researchDialog" class="popupDialogs"></div>
    <div id="mapHighlightsDialog" class="popupDialogs"></div>
    <div id="mapSummaryDialog" class="popupDialogs MapsummaryPopup"></div>
    <div id="reportsDialog" class="popupDialogs"></div>
    <div id="mailDialog" class="popupDialogs"></div>
    <div id="playerProfileDialog" class="popupDialogs PlayerNewPopup"></div>
    <div id="villageProfileDialog" class="popupDialogs"></div>
    <div id="transportSilverDialog" class="popupDialogs"></div>
    <div id="clanDialog" class="popupDialogs"></div>
    <div id="battleSimDialog" class="popupDialogs"></div>
    <div id="kingdomStewardship" class="popupDialogs"></div>
    <div id="supportDialog" class="popupDialogs"></div>
    <div id="touDialog" class="popupDialogs"></div>
    <div id="questsDialog" class="popupDialogs"></div>
    <div id="settingsDialog" class="popupDialogs"></div>
    <div id="rankingsDialog" class="popupDialogs"></div>  
    <div id="supportLookupDialog" class="popupDialogs"></div>
    <div id="sleepModeDialog" class="popupDialogs"></div>
    <div id="giftsDialog" class="popupDialogs"></div>
    <div id="pfDialog" class="popupDialogs"></div>
    <div id="villageListDialog" class="popupDialogs"></div>
    <div id="CommandTroopsPopup" class="popupDialogs "><div class="AttacksPopup"></div></div>
    <div id="quickCommandDialog" class="popupDialogs"><div class="QuickCommandPopup"></div></div>
    <div id="giftSendDialog" class="popupDialogs"></div>
    <div id="socialInviteDialog" class="popupDialogs"></div>
  
    <div id="incomingSummarDialog" class="popupDialogs"></div>
    <div id="outgoingSummarDialog" class="popupDialogs"></div>
    <div id="accountDialog" class="popupDialogs"></div>
    <div id="items2Dialog" class="popupDialogs"></div>
    <div id="targetsDialog" class="popupDialogs"></div>
    <div id="presetsDialog" class="popupDialogs"></div>

    <!-- Map Select / Hover Panel GUI -->
    <div id="mapGuiHover" class="mapGuiPanel" style="display:none;">
        <div class="mapGuiSubPanel mapGuiPlayer fontSilverFrLCmed">
            <div class="pAvatarFrame"><div class="pAvatarPicture"></div></div>
            <div class="pInfo pVillageInfo"></div>
            <div class="pInfo pName"></div>
            <div class="pInfo pClan"></div>
        </div>
        <div class="mapGuiSubPanel mapGuiNote fontSilverFrLCmed" style="display:none;"></div>
        <div class="mapGuiSubPanel mapGuiTargets fontSilverFrLCmed" style="display:none;"></div>
        <div class="mapGuiSubPanel mapGuiTroop fontGrayNumbersSm" style="display:none;"></div>
    </div>

    <div id="mapGuiSelected" class="mapGuiPanel" style="display:none;">
        <div class="mapGuiSubPanel mapGuiPlayer fontSilverFrLCmed">
            <div class="pAvatarFrame"><div class="pAvatarPicture"></div></div>
            <div class="pInfo pVillageInfo"></div>
            <div class="pInfo pName"></div>
            <div class="pInfo pClan"></div>
        </div>
        <div class="mapGuiSubPanel mapGuiNote fontSilverFrLCmed" style="display:none;"></div>
        <div class="mapGuiSubPanel mapGuiTargets fontSilverFrLCmed" style="display:none;"></div>
        <div class="mapGuiSubPanel mapGuiTags" style="display:none;"></div>
        <div class="mapGuiSubPanel mapGuiTroop fontGrayNumbersSm" style="display:none;"></div>
        <div class="mapGuiSubPanel mapGuiInout"></div>
    </div>
    <div id="mapGuiButtons" class="mapGuiPanel" style="display:none;"></div>
    


    <% if (FbgPlayer.User.Gift_GiftRecentlyAccepted != Int32.MinValue) {
           FbgPlayer.User.Gift_ClearRecentlyAcceptedFlag();
           %>
    <script type="text/javascript">
        $(function () {
            var popupbox = '<div class=fontSilverFrLCmed >You\'ve accepted a gift!<BR><BR>';
            popupbox += 'You can see and use your gift via the inventory popup, as well as return the favor by sending a gift back to your friend. ';
            popupbox += '</div>';

            ROE.Frame.popupInfo("Gifts", "230px", "center", "rgba(0,0,0,0.3)", popupbox, false);

           
        });
    </script>
    <%}%>

        <script type="text/javascript">
            $(document).ready(function () {   
                $('.toggleAnimationSettings').addClass('on');
            });
        </script>

    <% if (FbgPlayer.Realm.Theme == Fbg.Bll.Realm.Themes.desert){ %>

        <script type="text/javascript">
            GLOBAL_MAP_BIGTILE = "https://static.realmofempires.com/images/map/des/bigtile.jpg";
        </script>
        <style>
            #mapwrap {
                background: url(https://static.realmofempires.com/images/map/des/bigtile.jpg);
            }
            .earth {
                background: url(https://static.realmofempires.com/images/map/des/bigtile.jpg);
                background-size: 100% 100%;
            }
        </style>

    <%}else if(FbgPlayer.Realm.Theme == Fbg.Bll.Realm.Themes.highlands){%>

        <script type="text/javascript">
            GLOBAL_MAP_BIGTILE = "https://static.realmofempires.com/images/map/highlands/bigtile.jpg";
        </script>
        <style>
            #mapwrap {
                background: url(https://static.realmofempires.com/images/map/highlands/bigtile.jpg);
            }
            .earth {
                background: url(https://static.realmofempires.com/images/map/highlands/bigtile.jpg);
                background-size: 100% 100%;
            }
        </style>

    <%} else{%>

        <script type="text/javascript">
            GLOBAL_MAP_BIGTILE = "https://static.realmofempires.com/images/map/bigtile.jpg";
        </script>
        <style>
            #mapwrap {
                background: url(https://static.realmofempires.com/images/map/bigtile_5.jpg);
            }
            .earth {
                background: url(https://static.realmofempires.com/images/map/bigtile_5.jpg);
                background-size: 100% 100%;
            }
        </style>

    <%}%>




    <script>



        // Is in iframe check -- if not, don't show fullscreen btn
        if (!ROE.Frame.isInIframe())
        {
            $('#roeOptions .fullscreen').hide();
        } else
        {
            $('#roeOptions .fullscreen_back').hide();
        }
    </script>


    
   
    <!-- REALM PROMO AREA, add new items with class "promoItem" -->


        <style>
            #PromoWrapper {
                position: absolute;
                bottom: 202px;
                right: 15px;
                width: 400px;
            }

                #PromoWrapper .video {
                    background-size: 20px;
                    background-position: left;
                    background-image: url("https://static.realmofempires.com/images/icons/M_icoYouTube.png");
                    background-repeat: no-repeat;
                    padding-left: 20px;
                }


            .promoItem {
                position: relative;
                width: auto;
                height: auto;
                float: right;
                cursor: pointer;
                text-align: right;
                font: 14px "IM Fell French Canon", serif;
                background-color: rgba(0, 0, 0, 0.7);
                padding: 5px 10px;
                border-radius: 5px;
                overflow: hidden;
                box-shadow: 0px 0px 5px #000000;
                clear: both;
                margin-top: 4px;
                margin-bottom: 2px;
            }

            #askDevs {
                background-position: left;
                background-image: url("https://static.realmofempires.com/images/icons/onAir.png");
                background-repeat: no-repeat;
                background-size: 42px;
            }
                #askDevs a {
                    text-decoration: none;
                    color: white;
                    margin-left: 30px;
                }
                #askDevs .tinyfont {
                    font: 11px "IM Fell French Canon", serif;
                }
                #askDevs .countdown {
                    font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
                    font-size: 11px;
                }



            #newRealms, #newRXs, #customRXs {
                width: auto;
                height: auto;
                right: 0px;
                bottom: 0px;
                cursor: pointer;
                border-radius: 5px;
                font: 13px "IM Fell French Canon", serif;
                transition: all .2s ease-out;
                text-shadow: 0px 0px 1px #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000;
                color: #fff;
                background-color: rgba(0, 0, 0, 0.3);
                /* background-image: url(https://static.realmofempires.com/images/icons/M_HD_RealmList2.png); */
                /* background-size: auto 22px; */
                background-position: 98% center;
                background-repeat: no-repeat;
                line-height: 13px;
                padding: 2px 5px;
                text-align: center;
            }
                #newRealms:hover, #newRXs:hover, #customRXs:hover {
                    background-color: rgba(36, 98, 125, 0.8);
                }

                #newRealms a, #newRealms a, #customRXs a {
                    text-decoration: none;
                    color: white;
                }
                #newRealms .tinyfont , #newRXs .tinyfont , #customRXs .tinyfont{
                    font: 11px "IM Fell French Canon", serif;
                }
                #newRealms .countdown , #newRXs .countdown , #customRXs .countdown {
                    font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
                    font-size: 11px;
                }
                #newRealms .contest {
                    color: #39ff00;
                }

        </style>


    <!-- PROMO AREA end -->

     
    

   

    

</body>
</html>
