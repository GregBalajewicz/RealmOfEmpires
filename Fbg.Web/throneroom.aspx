<%@ Page Language="C#" AutoEventWireup="true" CodeFile="throneroom.aspx.cs" Inherits="throneroom" %>
<%@ Register Src="Controls/ListOfRealms.ascx" TagName="ListOfRealms" TagPrefix="uc1" %>
<%@ Register Src="Controls/ListOfRealmsCompact.ascx" TagName="ListOfRealmsCompact" TagPrefix="uc1" %>

<!DOCTYPE html>

<html>

<head id="Head1" runat="server">

    <title><%=  RSc("GameName")%></title>
    <meta charset="utf-8" />
    <meta name="description" content="Throne Room" />
    <meta name="viewport" content="width=320, initial-scale=1.0, maximum-scale=1, user-scalable=no" />

    <!-- FB Meta -->
    <%if (!isSpectatorView) { %>
    <meta property="og:url"           content="<%=Config.BaseUrl%>throne-<%= isSpectatorLoggedIn ? FbgUser.ID.ToString() : ""%>" />
    <%} %>
    <meta property="og:type"          content="website" />
    <meta property="og:title"         content="My Throne Room in Realm of Empires" />
    <meta property="og:description"   content="Check out all the realms I played in. Lots of cool stats!  Visit my Throne Room and give me a 'Like' while you're there." />
    <meta property="og:image"         content="https://static.realmofempires.com/images/throneRoom/trprev.jpg" />
    <meta property="og:image:secure_url"         content="https://static.realmofempires.com/images/throneRoom/tRprev.jpg" />

    <link rel="shortcut icon" href="https://static.realmofempires.com/images/icons/favicon.ico"/>
    <!--<link href="roe-pregame.css" rel="stylesheet" type="text/css" />-->
    <link href=<%=GetStaticDir("roe-frame_c.css")%> rel="stylesheet" type="text/css" />
    <link href=<%=GetStaticDir("roe-throne.css")%> rel="stylesheet" type="text/css" />
    <link href=<%=GetStaticDir("roe-avatar.css")%> rel="stylesheet" type="text/css" />
    <link href=<%=GetStaticDir("roe-ui.css")%> rel="stylesheet" type="text/css" />
    <link href=<%=GetStaticDir("chat2.css")%> rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/css?family=IM+Fell+French+Canon+SC|IM+Fell+French+Canon+LC" rel="stylesheet" type="text/css" />
    <link href=<%=GetStaticDir("jqueryui.css")%> rel="stylesheet" type="text/css" />

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js" type="text/javascript"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.0/jquery-ui.min.js" type="text/javascript"></script>  
    <script src="script-nochange/jquery.json-2.2.js" type="text/javascript"></script>  
    <script src=<%=GetScriptDir("interfaces.js")%> type="text/javascript"></script>  
    <script src=<%=GetScriptDir("bda.js")%> type="text/javascript"></script>
    <script src=<%=GetScriptDir("bda-exception.js")%> type="text/javascript"></script>
    <script src=<%=GetScriptDir("bda-utils.js")%> type="text/javascript" ></script>
    <script src=<%=GetScriptDir("bda-templates.js")%> type="text/javascript" ></script>
    <script src=<%=GetScriptDir("roe-utils.js")%> type="text/javascript" ></script>
    <script src=<%=GetScriptDir("roe-frame.js")%> type="text/javascript" ></script>
    <script src=<%=GetScriptDir("bda-console.js")%> type="text/javascript" ></script>
    <script src=<%=GetScriptDir("bda-broadcast.js")%> type="text/javascript"></script>
    <script src=<%=GetScriptDir("misc.js")%> type="text/javascript" ></script>
    <script src=<%=GetScriptDir("roe-api.js")%> type="text/javascript"></script>
    <script src=<%=GetScriptDir("bda-timer.js")%> type="text/javascript"></script>
    <script src=<%=GetScriptDir("roe-frame.js")%> type="text/javascript" ></script>
    <script src=<%=GetScriptDir("roe-LocalServerStorage.js")%> type="text/javascript" ></script>

    <script src="Scripts/jquery.signalR-2.2.0.min.js"></script>
<%--    <script src="<%=ResolveClientUrl("~/signalr/hubs") %>"></script>--%>
    <%--<script src=<%=GetScriptDir("chat3.js")%> type="text/javascript"></script>
    --%><script src=<%=GetScriptDir("roe-ui-chat2.js")%> type="text/javascript"></script>
    <script src=<%=GetScriptDir("roe-avatar.js")%> type="text/javascript"></script>
    <script src=<%=GetScriptDir("roe-throne.js")%> type="text/javascript"></script>

    <script>
        var QSUID = '<%= Request.QueryString["uid"]%>';
        var UVID = '<%= isSpectatorLoggedIn ? FbgUser.ID.ToString() : ""%>';
        var isMobile = <% = Convert.ToInt32(isMobile)%>;
        var BaseURL = '<%=Config.BaseUrl%>';
    </script>

        <script>
     <%
        /*
        March 30 2018 
        FB again warned us nto to pop out of frame. 
        so this was added, to pop into frame 
         */
         
        if (!isMobile && String.IsNullOrEmpty(FacebookConfig.DisconnectedFromFacebookUserID)
              && ( FbgUser.LoginType == Fbg.Common.UserLoginType.FB || FbgUser.LoginType == Fbg.Common.UserLoginType.FB_inferred )
              && ( Request.Cookies["fullscreen"] == null || Request.Cookies["fullscreen"].Value != "1"))
        {%>
        if (window.self === window.top){
            location.href = '<%=FacebookConfig.CanvasPageUrl_Full%>';
        }
        <%}%>
        </script>

</head>

<body class="throneroom">
    
    <% if(!isMobile){ %>
    <!-- INIT FB Widgets -->
    <div id="fb-root"></div>
    <script>(function(d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        //js.src = "//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.5&appId=";
        fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
    </script>
    <% } %>

    <form id="containerForm" runat="server">
    <div id="panelMain" class="">
        <div id="assetStage" class=""></div>
        <!-- <div id="tempSneakStamp">BETA</div> -->
        <div class="floatIconPanel">
            <!--<div id="ideas" class="floatIcon uservoice helpTooltip" data-tooltipid="floatIdeas" ></div>-->
            <!--<div id="supportq" class="floatIcon uservoice helpTooltip" data-tooltipid="floatSupportQ"></div>-->
            <div class="fontSilverFrLCmed floatIcon shareTr helpTooltip" data-tooltipid="shareTr"></div>
            <!--<div class="BtnDSm2n fontButton1L floatIcon likeTr helpTooltip" data-tooltipid="like">Like<div class="count"></div></div>-->
        </div>

        <!-- hidden by default, turned on by JS when needed. -->
        <div class="fontSilverFrLCmed shareRafflePanel">
            <div>Win great prizes!</div>
            <div>You have <span class="tickets"></span> tickets.</div>
            <div>Contest ends in: <span class="countdown" data-finisheson="1448038800000" data-refreshcall="$(this).remove();"></span></div>
        </div>

    </div>

    <div id="panelTop" class="fontSilverFrLCmed">

        <div class="xpVacArea">
            <div class="userXP"></div>
            <div class="userVacation"></div>
            <div class="nextVacationProgress">
                <div class="nextVacationBar"></div>
                <div class="nextVacationXP"></div>
            </div>
        </div>

        <div class="playerNameBox">
            <div runat="server" ID="vipBadge" class="vip" data-viplevel="" visible="false"></div>
            <div class="globalPlayerAvatar"></div>
            <div class="highestTitle helpTooltip" data-tooltipid="highestTitle"></div>
            <div class="globalPlayerName helpTooltip" data-tooltipid="globalPlayerName"></div>
        </div>
        
        <div class="topStatsTicker"></div>

        <!--
        <div class="launchIntro"><span>Welcome to the throne room!</span> <span class="click">Click here to take a short video tour</span></div>
        -->
    </div>   

    <div id="panelOne">
        <div class="btn btnRealmProfileList helpTooltip" data-tooltipid="btnRealmProfileList"></div>
        <div class="btn btnChangelog helpTooltip" data-tooltipid="btnChangelog"></div>
        <!--<div class="btn btnTrophies helpTooltip" data-tooltipid="btnTrophies"></div>-->
        <!--<div class="btn btnNews helpTooltip" data-tooltipid="btnNews"></div>-->

        <div class="btn btnSpeedRealmStandings helpTooltip" data-tooltipid="btnSpeedRealmStandings"></div>
    </div>
    
    <div id="panelBottom" class="fontSilverFrLCmed">

        <div class="btnWrapBottomLeft">          
            <div class="realmButtonsContainer">
                <uc1:ListOfRealmsCompact ID="realmListCompact" CssClass="fontSilverFrLCmed" runat="server" /> 
                <div class="bottomMenuBtn btnChooseRealms">...</div>
                <!--<div class="BtnBLg1 fontButton1L bottomMenuBtn btnRealmProfileList helpTooltip" data-tooltipid="btnRealmProfileList">History</div>-->

                <asp:LinkButton id="loginbutton" CssClass="BtnBLg1 fontButton1L bottomMenuBtn" runat="server" visible="false" clientidmode="Static"  OnClick="loginbutton_Click" >Login</asp:LinkButton>
            </div>
        </div>

    </div>




    <div id="chooseRealmsBox" >
        <asp:Panel runat=server ID=panelRealms CssClass="tblRealms">
            <uc1:ListOfRealms ID="ListOfRealms1"  Theme=ThemeM runat="server" />   
        </asp:Panel>
    </div>
    
    <asp:Label ID="lblFriendsFreshedDebugMessage" runat="server" Text=""></asp:Label>

    <div id="genericDialog" class="popupDialogs"></div>
    <div id="realmProfile" class="popupDialogs"></div>

    <%if (!isSpectatorView) { %>
    <div id="shareDialog" class="popupDialogs" style="display:none;">
        
        <div class="shareTr">

            <div class="panel info">
                <% if(!isMobile){ %>
                    <div class="header">There many ways to share and showcase your Throne Room for all to see!</div>
                    <div class="fbLikeWrapper">
                        <div class="fb-like" data-href="<%=Config.BaseUrl%>throne-<%= isSpectatorLoggedIn ? FbgUser.ID.ToString() : ""%>" data-layout="button" data-action="like" data-show-faces="false" data-share="true"></div>
                    </div>
                    <a class="shareBtn twitter" href="https://twitter.com/share?url=<%=Config.BaseUrl%>throne-<%= isSpectatorLoggedIn ? FbgUser.ID.ToString() : ""%>&text=My Realm Of Empires Throne: " target="_blank"></a>
                <%}else{%>
                    <div class="header">Share your Throne Room link for all to see!</div>
                <%}%>
                <div class="BtnBLg1 fontButton1L shareBtn direct">Direct Link</div>
            </div>

        </div>
    </div>
    <%}%>

    <div class="throneBusyMask"><div class="msg"></div></div>

      
    </form>

    <div class="mChatMask"></div>
    <div class="mChatToggle"></div>
    <div id="chatBarContainer">
        <div class="BtnBLg1  toggleBarContainer"><div class="icon"></div></div>
        <%if ((  FbgUser.LoginType == Fbg.Common.UserLoginType.FB || FbgUser.LoginType == Fbg.Common.UserLoginType.FB_inferred) ) { %> <a href="login_FbToTactica.aspx">SECURE YOUR ACCOUNT</a> <%} %>
    </div>

    <nav class="contextMenu" id="leaderboardUserMenu">
        <div id="oneOnOneChat" class="menuItem">Chat</div>
        <div id="optionViewThrone" class="menuItem">View Throne</div>
    </nav>

    <div id="busy" style="display: none;">
        <div id="busy-content">
            <img src="https://static.realmofempires.com/images/misc/ajax-loader1.gif" /><br />
            <span id="busy-msg-default">working...</span>
            <span id="busy-msg-custom" style="display: none;"></span><br />
            <a id="busy-refresh" href="#"><br><br>        
                <div class="crashMsg" onclick="$('#lnkBacktoCR')[0].click();">Oops! This is taking a while...<br>Click to go back you feel it's stuck!</div>
            </a>            
        </div>
    </div>


    <%if (IsTesterRoleOrHigher){ %>
        <!-- ADMIN TOOLS STUFF -->
        <style>

            #adminToolsBtn {
                position: absolute;
                top: 70px;
                left: 0px;
                width: 44px;
                height: 44px;
                z-index: 1;
                background-image: url(https://static.realmofempires.com/images/icons/M_Knight.png);
                background-size: 100% 100%;
                cursor: pointer;
            }

            #adminToolsPanel {
                display: none;
                position: fixed;
                top: 120px;
                left: 0px;
                right: 0px;
                padding: 20px;
                z-index: 1;
                background-color: rgba(0,0,0,.9);
            }

            #adminCastPanel {
                position: relative;
                width: 400px;
                margin: 0 auto;
            }

            #adminCastText {
                position: relative;
                width: 100%;
                height: 200px;
            }

            #adminCastSend {
                position: relative;
                width: 100px;
                padding: 10px;
                border: 1px solid #808080;
                text-align: center;
                cursor:pointer;
            }
                #adminCastSend:hover {
                    background-color: #fff;
                    color: #f00;
                    border: #f00 1px solid;
                }
        </style>
        

        <div id="adminToolsBtn">
        </div>

        <div id="adminToolsPanel">
            <div id="adminCastPanel">
                <div style="padding:10px; color: #ec1010;">Admin BroadCast: message all global and clan chats.</div>
                <textarea id="adminCastText"></textarea>
                <div id="adminCastSend">BroadCast</div>
            </div>
        </div>

        <script>
            $(document).ready(function(){
                $('#adminToolsBtn').click(function(){
                    $('#adminToolsPanel').toggle();
                });
                $('#adminCastSend').click(function(){
                    ROE.Chat2.adminBroadcast($('#adminCastText').val());
                });
            });
        </script>

    <!-- ADMIN TOOLS STUFF -->  
    <%} %>     


    <!--VIP PANEL -->
    <asp:Panel ClientIDMode="Static" ID="vipPopup" runat="server" style="display:none;" Visible="false">
        <div class="badge" data-viplevel="<%=userVipLevel%>"></div>
        <p>Thank you for being a great supporter of Realm of Empires. You have been granted VIP <%=VipStatusName %> status within the game as a sign of our gratitude.</p>
        <p>As a VIP you will have access to ongoing perks such as unique avatars, priority support, and VIP-only realms.</p>
        <p>Avatars granted for your VIP level:</p>
        <div class="perks avatars"></div>
        <p>Only you can see your VIP status however, you may use a unique avatar to announce it to the world.</p>
        <div>
            <p>You can also display a Border in chats showing your VIP status. Note: this will be on ALL chats in your account!</p>
            <div style="text-align:center;"><div class="BtnBXLg1 fontButton1L vipSwitch off">VIP Chat border is OFF</div></div>
        </div>
        <p>There are 4 levels of VIP status: Bronze, Silver, Gold and Diamond. The criteria is based on various factors, like your level of activity in the game, length of time in the game, and level of monetary support of the game, XP etc; exact formula may change at anytime and is kept private to protect privacy of players who wish to display their status.</p>
        <p>Thank you for making Realms of Empires possible!</p>
    </asp:Panel>

</body>
</html>
