<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChooseLoginType.aspx.cs" Inherits="ChooseLoginType" %>



<!doctype html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <meta charset="utf-8">

     <%if (Device == CONSTS.Device.iOS) { %>
    <meta name="viewport" content="width=320,inital-scale=1.0,maximum-scale=5.0,user-scalable=0">
    <%} else { %>
    <meta name="viewport" content="width=321"/>
    <%} %>

    <link href='https://fonts.googleapis.com/css?family=IM+Fell+French+Canon+SC|IM+Fell+French+Canon' rel='stylesheet' type='text/css' />
    <link href="static/bda-ui-transition.css" rel="stylesheet" type="text/css" />
    <link href="main.2.css" rel="stylesheet" type="text/css" />    
    <link href="static/bda-ui.css" rel="stylesheet" type="text/css" />  

    <script src="script-nochange/modernizr.custom.js" type="text/javascript"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js" type="text/javascript"></script>
    <script src="script/bda-ui-transition.js" type="text/javascript"></script> 
             
      <style>
          
        .mobileMainContent  {
            position:relative;
            width:100%;
        }
        #background {
            width: 100%; 
            height: 100%; 
            position: fixed; 
            left: 0px; 
            top: 0px; 
        }       
        .titletoplogo {
            position: fixed;
            top: 0;
            background: url('https://static.realmofempires.com/images/misc/RegistrationHeader.png');
            width:100%;
            height:57px;   
            margin-top: 4px;  
            z-index: 100;      
        }
          .playthegame {
  position: absolute;
  width: 100%;
  text-align: center;
  top: 49px;
  left: 0;
  color: #FFF;
  font: 17px "IM Fell French Canon";
  padding: 20px 9px;
  background: rgba(0, 0, 0, 0.45);
  box-sizing: border-box;
  text-shadow: 0px 1px 0px #2E2E2E;
          }


          .mainbutton {
              position: relative;
              width: 100%;
              text-align: center;
              color: #FFF;
              font: 18px/1.0em "IM Fell French Canon SC", serif;
              padding: 8px 0px;
              margin: 12px 0;
              background-color: rgba(0, 0, 0, 0.6);
              height: 38px !important;
              text-shadow: 1px 1px 0px #2E2E2E,-1px 1px 0px #2E2E2E,-1px -1px 0px #2E2E2E,1px -1px 0px #2E2E2E;
          }
              .mainbutton.FB {
                height: 54px !important;
                display:none;
              }
              .mainbutton .topBar, .mainbutton .bottomBar {
                  position: absolute;
                  left: 0px;
                  right: 0px;
                  height: 8px;
                  background-image: url("https://static.realmofempires.com/images/misc/m_listbar2.png");
                  background-position: center center;
                  background-repeat: no-repeat;
              }
              .mainbutton .topBar {
                  top: -4px;
              }
              .mainbutton .bottomBar {
                  bottom:-4px;
              }

          .clickableActive, .mainbutton:active {
              -webkit-box-shadow: inset 0 0 15px #ffffff;
              -moz-box-shadow: inset 0 0 15px #ffffff;
              box-shadow: inset 0 0 15px #ffffff;
          }

          .bottomLinks {
              position: absolute;
              width:100%;
              left:0;
              bottom:0;
              text-align:center;
              color:white;
              padding: 3px 0;
          }

          .bottomLinks A {
              color:white;
              text-decoration: underline;
          }
          A,A:hover,A:active,A:visited {
              color:white;
              text-decoration: none;
          }

        .mainBack {
            height:52px;
          }
        .larrow {
              margin: -6px 0;
          }

          .setContainer {
              position: absolute;
              top: 130px;
              left: 0px;
              right: 0px;
              bottom: 50px;
              overflow: auto;
          }

          .mainset {
              position: absolute;
              left: 0px;
              top: 0px;
              width: 100%;
              margin: 0;
          }

          .optionLink {
              display: block;
              position: absolute;
              top: 0px;
              right: 0px;
              bottom: 0px;
              left: 0px;
          }

          .logo1 {
              height: 60px;
              position: absolute;
              top: -3px;
              left: 10px;
          }
          .logoFB {
              position: absolute;
              top: 9px;
              left: 50%;
              width: 250px;
              height: 49px;
              margin-left: -129px;
              background-size: 256px auto;
              background-position: -3px -3px;
              border-radius: 4px;
              background-image: url(https://fbcdn-dragon-a.akamaihd.net/hphotos-ak-xap1/t39.2178-6/851558_575761415814161_1164808389_n.png);
          }
           .logoFB-sm {

               height: 44px;
              position: absolute;
              top: 6px;
              left: 12px;
          }
          .logoTactica {
              height: 44px;
              position: absolute;
              top: 6px;
              left: 12px;
          }
          .logoEmail {
              position: absolute;
              top: 6px;
              left: 22px;
          }

          .titletext0 {
              position: relative;
              margin: 0 auto;
              margin-top: 17px;
          }
          .titletext1 {
              margin-top: 16px;
              margin-left: 30px;
              font: 16px/0px "IM Fell French Canon SC", serif;
          }
          .tacticaLoginText {
              position: relative;
              margin: 0 auto;
              margin-top: 17px;
          }
          .emailrecoveryText {
              position: relative;
              margin: 0 auto;
              margin-top: 17px;
          }

      </style>

       
</head>

<body style="margin-left: 0; margin-right: 0; color: white; ">
    
     
        <img id="background" src="https://static.realmofempires.com/images/backgrounds/M_BG_Login.jpg" class="stretch" alt="" />
            
        <div class="titletoplogo"></div>
        <div class="playthegame"><%=RS("Playthegame") %></div>

        <div class="setContainer">

            <div class="mainset slideRightTo transition" >

                <div class="mainbutton clickable">
                    <div class="topBar"></div>
                    <a class="optionLink" href="<%=LoginModeHelper.GetROELoginUrl(LoginModeHelper.LoginModeEnum.mobile, Server.UrlEncode("ChooseRealmMobile.aspx"))%>">
                        <img class="logo1" src="https://static.realmofempires.com/images/misc/RoE_WelcomeSM.png" />
                        <div class="titletext0"><%=RS("Letmeinnow") %></div>
                        <!--<div class="titletext1"><%=RS("Iplayononemobiledevice") %></div>-->
                    </a>
                    <div class="bottomBar"></div>
                </div>

                <div class="mainbutton clickable" >
                    <div class="topBar"></div>
                    <a class="optionLink" href="<%=LoginModeHelper.GetROELoginUrl(LoginModeHelper.LoginModeEnum.bda, Server.UrlEncode("ChooseRealmMobile.aspx")) %>" >
                        <img class="logoTactica" src="https://static.realmofempires.com/images/misc/tacticaIconL.png" />
                        <div class="tacticaLoginText">Login with Tactica</div>
                    </a>
                    <div class="bottomBar"></div>
                </div>

                
                <div class="mainbutton FB clickable">
                    <div class="topBar"></div>
                    <a class="optionLink" href="<%=LoginModeHelper.GetROELoginUrl(LoginModeHelper.LoginModeEnum.facebook, Server.UrlEncode("ChooseRealmMobile.aspx")) %>" >
                        <div class="logoFB"></div>
                    </a>
                    <div class="bottomBar"></div>
                </div>
                
                <div class="mainbutton FB-roelook clickable">
                    <div class="topBar"></div>
                    <a class="optionLink" href="<%=LoginModeHelper.GetROELoginUrl(LoginModeHelper.LoginModeEnum.facebook, Server.UrlEncode("ChooseRealmMobile.aspx")) %>" >                
                        <img class="logoFB-sm" src="https://static.realmofempires.com/images/icons/M_icoFB.png" />
                        <div class="tacticaLoginText"><%=RS("LoginviaFacebook") %></div>                     
                    </a>
                    <div class="bottomBar"></div>
                </div>
                <div class="mainbutton clickable" >
                    <div class="topBar"></div>
                    <a class="optionLink" href="<%=LoginModeHelper.GetROELoginUrl(LoginModeHelper.LoginModeEnum.bda, Server.UrlEncode("ChooseRealmMobileKong.aspx")) %>" >
                        <img class="logoTactica" src="https://static.realmofempires.com/images/icons/M_icoKong.png" />
                        <div class="tacticaLoginText">Login with Kongregate</div>
                    </a>
                    <div class="bottomBar"></div>
                </div>
                <div class="mainbutton clickable" >
                    <div class="topBar"></div>
                    <a class="optionLink" href="<%=LoginModeHelper.GetROELoginUrl(LoginModeHelper.LoginModeEnum.bda, Server.UrlEncode("ChooseRealmMobileAG.aspx")) %>" >
                        <img class="logoTactica" src="https://static.realmofempires.com/images/icons/M_icoAG.png" />
                        <div class="tacticaLoginText">Login with Armor Games</div>
                    </a>
                    <div class="bottomBar"></div>
                </div>
            </div>
        </div> 

    <div class="bottomLinks">
        <a href="RecoverAccount0.aspx" >Advanced Account Recovery</a>
        <br /><br />
        <div class="support" ><%=RS("Contact") %> support@realmofempires.com</div>   
    </div>
               

    <script type="text/javascript">

        $(function () {
            $('.clickable').bind("touchstart",
                function (e) {
                    $(e.currentTarget).addClass('clickableActive');
                }
            );

            $('.clickable').bind("touchend",
                function (e) {
                    $(e.currentTarget).removeClass('clickableActive');
                }
            );
            $('.clickable').bind("touchcanel",
                function (e) {
                    $(e.currentTarget).removeClass('clickableActive');
                }
            );
            $('.clickable').bind("touchmove",
                function (e) {
                    $(e.currentTarget).removeClass('clickableActive');
                }
            );

        });

    </script>
</body>
</html>
