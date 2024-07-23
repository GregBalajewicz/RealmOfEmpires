<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ExclusiveRealmMessage.aspx.cs" Inherits="ExclusiveRealmMessage" %>

<%@ Register Src="Controls/ListOfRealms.ascx" TagName="ListOfRealms" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%=RSc("RealmOfEmpires") %></title>
   
     <%if (isDevice == CONSTS.Device.iOS) { %>
    <meta name="viewport" content="width=320,inital-scale=1.0,maximum-scale=5.0,user-scalable=0">
    <%} else if (isDevice == CONSTS.Device.Amazon) { %>
    <meta name="viewport" content="width=321, user-scalable=0">
    <%} else { %>
    <meta name="viewport" content="width=321"/>
    <%} %>

    <link href="main.2.css" rel="stylesheet" type="text/css" />
    <link href='https://fonts.googleapis.com/css?family=IM+Fell+French+Canon+SC' rel='stylesheet' type='text/css'>
    <link href="static/roe-ui.css" rel="stylesheet" type="text/css" />

    <script src="script-nochange/jquery-1.2.3.js" type="text/javascript"></script>

    <script src="script/countdown_old.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(initTimers);
    </script>

    <style>
        BODY  {
            overflow-x: hidden !important;
        }
        #txtTitle {
            color: #FFD886;
        }
    </style>
    <%if (isMobile) { %>

    <style>
        BODY  {
            overflow-x: hidden !important;
            width:100%  !important;
        }
        #titleArea {
            background: rgba(0,0,0,0.50);
            padding: 0 5px;
            margin: 18px 0;
            height: 68px;
            vertical-align:middle;
        }
        .titleseparator{
            position:absolute;  
            height: 18px;
            width: 100%;
            text-align: center;
            margin: -6px auto;
            background: url('https://static.realmofempires.com/images/misc/m_listbar.png') no-repeat center;
          }
        #logo {
            position: relative;
            float:left; 
            width: 80px;
            height: 97px;
            margin: -12px 0; 
            z-index: 100;
            display:block;
        }
        #title { 
            text-shadow: 2px 2px 1px #000000;
            font: 26px/1.0em "IM Fell French Canon SC", serif; 
            display:block; 
            padding: 12px 10px;
            color: white; 
            text-align:center;           
        }
        #title DIV, #realmregister .label{ 
            font: 16px/1.0em "IM Fell French Canon SC", serif; 
            text-transform: none;
        }
        #realmregister {
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.50);
            font: 12px/1.0em "lucida grande" , tahoma, verdana, arial, sans-serif; 
            text-shadow: 2px 2px 1px #000000;
        }
        #realmregister .themeM-more > .fg > .arrow-l  {
              left: 3px;
          }
        #realmregister .themeM-more {
              float: left;
          }
        #realmregister .themeM-panel, .themeM-group, .themeM-tabPanel  {
              margin: 2px 20px;
          }
        #realmregister A {
            text-decoration: none;
          }
        .reqnotmet {
            text-shadow: 2px 2px 1px #000000;
            font: 22px/1.0em "IM Fell French Canon SC", serif; 
            color: #FFD886;
            padding: 14px 0;
        }
        .reqtext {
            padding: 0 20px;
            text-align: left;
            margin: 10px 0;
        }
        #realmparam {
            border: 0px !important;
            margin: 8px 10px;
            padding: 4px 10px !important;
            font: 11px/1.0em "lucida grande", tahoma, verdana, arial, sans-serif;
            text-align: left;
            text-shadow: 2px 2px 1px #000000;
            line-height:16px;
        }
        #realmparam DIV {
            border: 0px !important;
            background-color: transparent !important;
            font: 16px/1.0em "IM Fell French Canon SC", serif;            
            text-align: center;
            padding: 8px 0;
        }
        .stretch {
            width:100%;
            height:100%;
        }
        #background {
            width: 100%; 
            height: 100%; 
            position: fixed; 
            left: 0px; 
            top: 0px; 
            z-index: -1; /* Ensure div tag stays behind content; -999 might work, too. */
        }
        .reqtext SPAN {
            color: #FFD886;
        }
    </style>

    <% } else {%>
    <style>
                html {
            position: fixed;
            width: 100%;
            height: 100%;
            overflow: hidden;
        }
        body {
            background-image: url(https://static.realmofempires.com/images/backgrounds/BGIntro.jpg);
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            background-attachment:fixed;
            height: 100%;
            width: 100%;
            margin: 0px;
            overflow-x: hidden;
            overflow-y: auto;
        }


	         #knightsL{
position: fixed;
pointer-events: none;
left: -295px;
top: 0px;
width: 100%;
height: 100%;
background-image: url(https://static.realmofempires.com/images/backgrounds/Warlords_Left.png);
background-size: auto 100%;
background-position: 50% 0px;
background-repeat: no-repeat;
         }
         #knightsR{
position: fixed;
pointer-events: none;
left: 295px;
top: 0px;
width: 100%;
height: 100%;
background-image: url(https://static.realmofempires.com/images/backgrounds/Warlords_Right.png);
background-size: auto 100%;
background-position: 50% 0px;
background-repeat: no-repeat;
         }

         .holderbkg{
         position:absolute;
         left:0px;right:0px;bottom:0px;top:0px}
          .holderbkg .corner{position:absolute;background-size:100% 100%}
          .holderbkg .corner-top-left{width:35px;height:36px;left:0px;top:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TL.png')}
          .holderbkg .corner-top-right{width:35px;height:36px;right:0px;top:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TR.png')}
          .holderbkg .corner-bottom-left{width:35px;height:46px;left:0px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BL.png')}
          .holderbkg .corner-bottom-right{width:35px;height:46px;right:0px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BR.png')}
          .holderbkg .border{position:absolute;background-size:100% 100%}
          .holderbkg .lb-border-top{top:0px;left:35px;right:35px;height:36px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TC.png')}
          .holderbkg .lb-border-bottom{left:35px;right:35px;height:46px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BC.png')}
          .holderbkg .lb-border-left{left:0px;top:36px;bottom:46px;width:35px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_L.png')}
          .holderbkg .lb-border-right{right:0px;top:36px;bottom:46px;width:35px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_R.png')}

        #main {
            position: relative;
            width: 450px;
            top: 45%;
            left: 50%;
            margin-left: -225px;
            background-color: rgba(0, 0, 0, 0.8);
            border-radius: 20px;
            padding: 20px;
            padding-top: 60px;
            text-align: center;
        }

        .roeLogo {
            position: absolute;
            left: 0px;
            right: 0px;
            top: -145px;
            z-index: 1;
            margin: 0 auto;
            height: 196px;
            background-image: url(https://static.realmofempires.com/images/d2test/HeaderWarlordsRising.png);
            background-position: center;
            background-repeat: no-repeat;
        }
                
        #realmNameSection {
            position: relative;
            padding: 20px;
            padding-top:5px;
            text-align: center;
        }
            #realmNameSection .realmName {
                font: 32px "IM Fell French Canon SC", serif;
            }

        .backButton {
            position: relative;
            display: inline-block;
            height: 37px;
            width: 135px;
            text-align: center;
            box-sizing: border-box;
            padding-top: 11px;
            cursor: pointer;
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none !important;
            margin-bottom: 15px;
        }
    </style>
    <%} %>
</head>

<body>

  
        <%if (!isMobile) { %>

    <div id="knightsR"></div>
    <div id="knightsL"></div>

            <div id="main" >

                <div class="roeLogo"></div>
                <div class="holderbkg">
			        <div class="corner corner-top-left"></div>
			        <div class="corner corner-top-right"></div>
			        <div class="corner corner-bottom-left"></div>
			        <div class="corner corner-bottom-right"></div>
			        <div class="border lb-border-top"></div>
			        <div class="border lb-border-right"></div>
			        <div class="border lb-border-bottom"></div>
			        <div class="border lb-border-left"></div>
		        </div>

                <div id=realmNameSection class="fontSilverFrSClrg">
                    <div class="realmName"><%=realm.Name%></div>
                    <div class="realmDesc"><%=realm.Desc%></div>
                </div>

                <span class="fontSilverFrSClrg">
                   <%=RS("ExclusiveForActivePlayers") %> <asp:Label ID="txtTitle" runat="server" Text=""></asp:Label> <%=RS("OnAtLeastOneRealm") %>
                    <br /><br />
                   <%=RS("FirstProveYourselfOnARealm") %>                              
                    <br /><br />
                </span>
                
                <a class="backButton" href=ChooseRealm.aspx>Back</a>
            </div>



        <% } else { %>
        <center>
        <div id=titleArea>
                <image id=logo src=https://static.realmofempires.com/images/fancylogo.png />
                <div class="titleseparator"></div>
                <span id=title><%=realm.Name%><div><%=realm.Desc%></div></span>
                <div class="titleseparator"></div>
        </div>
             <div id="background">
                    <img src="https://static.realmofempires.com/images/backgrounds/M_BG_Login.jpg" class="stretch" alt="" />
                </div>
         
        <br />
        <br />
        <section id="realmregister">
        
        <div class="titleseparator"></div>
        <div class="fg">
            <br />
            <a href='ChooseRealm.aspx' >
            <div class="themeM-panel style-link action-back">
                    <div class="bg">
                        <div class="corner-br"></div>
                    </div>

                    <div class="fg">
                        <div class="themeM-more">
                            <div class="bg">
                                <div class="corner-tl"></div>
                            </div>

                            <div class="fg">
                                <div class="label">
                                    <span></span><br>
                                </div>

                                <div class="arrow-l sfxOpen"></div>
                            </div>
                        </div>

                        <div class="label sfxOpen">
                            <span><%=RS("Back2") %></span><br>
                        </div>
                    </div>
                </div></a>

            <div class="reqnotmet" ><%=RS("reqnotmet") %></div>

            <img src="https://static.realmofempires.com/images/misc/M_SpacerBottom.png" />
            <div class="reqtext" >
                
                
                <%=RS("ExclusiveForActivePlayers") %> 
                    <asp:Label ID="txtTitle2" runat="server" Text=""></asp:Label> <%=RS("OnAtLeastOneRealm") %> 
            
                <br /><br /><%=RS("FirstProveYourselfOnARealm") %>
                </div>
                <img src="https://static.realmofempires.com/images/misc/M_SpacerTop.png" /><br />
            </div>


            <div id="realmparam">
                <div><%=RS("realmparams") %></div>
                <%=realm.ExtendedDesc%>
            </div>
            <br /><br />

            <div class="titleseparator"></div>
        </section>

            </center>
       
            
        
        <% } %>

</body>
</html>
