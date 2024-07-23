<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChooseQuad.aspx.cs" Inherits="ChooseQuad"
    ValidateRequest="false" %>

<%@ Register Src="Controls/Tutorial.ascx" TagName="Tutorial" TagPrefix="uc1" %>

<!doctype html>
<%@ Register Src="Controls/NewPlayerIntro.ascx" TagName="NewPlayerIntro" TagPrefix="uc2" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%= RSc("GameName") %></title>

    <%if (Device == CONSTS.Device.iOS) { %>
    <meta name="viewport" content="width=320,inital-scale=1.0,maximum-scale=5.0,user-scalable=0">
    <%} else { %>
    <meta name="viewport" content="width=321"/>
    <%} %>
    

    <link href='https://fonts.googleapis.com/css?family=IM+Fell+French+Canon+SC' rel='stylesheet' type='text/css'>
    <link href="https://static.realmofempires.com/fonts/CharlemagneStd-Bold.css" rel="stylesheet" type="text/css" />
    <link href="static/roe-ui.css" rel="stylesheet" type="text/css" />
    <link href="main.2.css" rel="stylesheet" type="text/css" />
    <script src="<%= Config.InDev ? "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.js" : "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js" %>" type="text/javascript"></script>
    <script src="script-nochange/jquery.dimensions.js" type="text/javascript"></script>

    <style>
        body {
                margin: 0 !important;
                padding: 0 !important;
                width:100%  !important;
            }
    </style>

     <%if (!isMobile)
       { %>
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
        #form1 {
            position: absolute;
            right: 0px;
            top: 0px;
            left: 0px;
            bottom: 0px;
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

        .container {
            position: absolute;
            width: 600px;
            top: 45%;
            left: 50%;
            margin-left: -300px;
            background-color: rgba(0, 0, 0, 0.8);
            border-radius: 20px;
            padding: 20px;
            padding-top: 50px;
        }


                .container .column {
                    position: relative;
                    /*display: table-cell;*/
                    width: 100%;
                    vertical-align: top;
                    z-index: 3;
                }
         .nickname, .startin {
                font-size: 12pt;
                padding: 1px;
            }

         .roeLogo{
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

          .holderbkg {
              position: absolute;
              left: 0px;
              right: 0px;
              bottom: 0px;
              top: 0px;
          }
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

        #realmNameSection {
            position: relative;
            padding: 20px;
            padding-top:10px;
            text-align: center;
        }
            #realmNameSection .realmName {
                font: 32px "IM Fell French Canon SC", serif;
            }
            #txtNickName {
                font-size: 12px;
                width: initial !important;
                padding: 5px 7px;
                border-radius: 7px;
                box-shadow: inset -2px 2px 5px rgba(0, 0, 0, 0.4);
            }

            #btnGo {
                height: 40px;
                width: 110px;
                text-align: center;
                box-sizing: border-box;
                padding: 0px;
                cursor: pointer;
                background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
                background-position: -100px -50px;
                overflow: hidden;
                font: 15px/1.0em "IM Fell French Canon SC", serif;
                color: #D7D7D7;
                text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
                border: none;
                background-color: rgba(0, 0, 0, 0);
                outline: none;
                clear:both;
                float: left;
                margin-bottom: 20px;
            }

        #btnBack {
            display: inline-block;
            height: 40px;
            width: 110px;
            text-align: center;
            box-sizing: border-box;
            padding: 0px;
            cursor: pointer;
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -50px;
            overflow: hidden;
            font: 15px/1.0em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            outline: none;
            text-decoration: none;
            padding-top: 11px;
            float: left;
            margin-top: 10px;
        }

        .realmParamTitle, .realmRegTitle {
            font-weight: bold;
            text-align: center;
            margin-bottom: 3px;
        }


            .registrationForm {
                width:100%;
            }

            .tableColRight {
                vertical-align:top;
                text-align: left;
                width: 60%;
            }
            .tableColLeft {
               vertical-align:top;
               text-align:right;
            }

        </style>
    <%}
       else
       { %>
      <style>
       
        .container {
            width:100%; 
            background: rgba(0,0,0,0.50);
        }
        .container .column {display:inline;width:100%;}

         .buttonGO {
              display: inline-block;
              background: url('https://static.realmofempires.com/images/buttons/M_Btn2_C.png') no-repeat center;
              width:74px;
              height:42px;
          }
         .buttonGoText {
             color: #FFD886;
             font: 24px/1.0em "IM Fell French Canon SC", serif; 
             text-align:center;
             padding-top: 8px;
          }
          #pnlRealmInfo {
              border: 0px !important;
              margin: 0 10px 8px 10px;
              padding: 4px 10px !important;
              font: 14px/1.0em "IM Fell French Canon", serif;
              line-height:20px;
          }
         #pnlRealmInfo DIV {
             border: 0px  !important;
             background-color: transparent  !important;
             font: 20px/1.0em "IM Fell French Canon SC", serif;
             text-shadow: 2px 2px 1px #000000;             
              }
         .registrationForm {
             display:block;
             margin: 8px 10px;
             padding: 4px 10px;
             background: rgba(0,0,0,0.50);
             border: 2px solid #846824;
             -webkit-border-radius: 15px;
             border-radius: 15px;
             font: 13px/1.0em "IM Fell French Canon", serif;
             text-shadow: 2px 2px 1px #000000;
          }
        .registrationForm td {
            padding-top:10px;
        }
        .nickname {
                font: 20px/1.0em "IM Fell French Canon SC", serif; 
            }
        #titleArea {
            background: rgba(0,0,0,0.50);
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
            margin-left:7px;
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
        #realmregister .themeM-more > .fg > .arrow-l  {
              left: 3px;
          }
        #realmregister .themeM-more {
              float: left;
          }
        #realmregister .themeM-panel, .themeM-group, .themeM-tabPanel  {
              margin: 2px 20px;
          }
        #background {
            width: 100%; 
            height: 100%; 
            position: fixed; 
            left: 0px; 
            top: 0px; 
            z-index: -1;
        }
        #realmregister A {
            text-decoration: none;
          }
        .stretch {
            width:100%;
            height:100%;
        }

        a.ThemeA:hover {
            text-decoration:none;
        }
        a.ThemeA {
            display: inline-block;
            width:147px;
            height:42px;
            background: url('https://static.realmofempires.com/images/buttons/M_Btn2_C.png') no-repeat center;
            color: #FFD886;
            font: 24px/1.0em "IM Fell French Canon SC", serif; 
            text-align:center;
            line-height: 40px;
            /*margin-top: -14px;*/
            margin-top: 0px;
          }
        a.ThemeA:before {
            
            width:36px;
            height:42px;
            background: url('https://static.realmofempires.com/images/buttons/M_Btn2_L.png') no-repeat center;
            float: left;
            content:"  ";
        }
        a.ThemeA:after {
            width:37px;
            height:42px;
            background: url('https://static.realmofempires.com/images/buttons/M_Btn2_R.png') no-repeat center;
            float: right;
            content:"  ";
        }
        a.ThemeA.pressed, a.ThemeA:active  {
            text-shadow: 2px 2px 4px #ffffff;
        }
        INPUT {
            height:18px;
            -webkit-border-radius: 6px;
            border-radius: 6px;
          }

       

      </style>
      <%} %>

     <%if (!isMobile)
       { %>
    <script type="text/javascript">
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-29966616-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();
    </script>
    <%}
       else
       { %>
    <script type="text/javascript">

        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-33297531-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();

    </script>
    <%} %>

    <%if (isMobile) { %>
    <script type="text/javascript">

        $(function () {
            $('a.ThemeA').bind("touchstart",
                function (e) {
                    $(e.target).addClass('pressed');
                }
            );

            $('a.ThemeA').bind("touchend",
                function (e) {
                    $(e.target).removeClass('pressed');
                }
            );
            $('a.ThemeA').bind("touchcanel",
                function (e) {
                    $(e.target).removeClass('pressed');
                }
            );
            $('a.ThemeA').bind("touchmove",
                function (e) {
                    $(e.target).removeClass('pressed');
                }
            );

        });

    </script>
    <%}%>

</head>
<body >
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <%if (!isMobile)
      { %>

          <div id="knightsR"></div>
        <div id="knightsL"></div>

        <!--<center>
        <img src='<%=RSic("Main2c_hedaer")%>' />
        <div style="position: relative; margin-top: -45px; margin-left: -20px;">
            <span style="font-size: 12pt;">
                <%= RS("pleaseRegister")%>...

        </div>
        </center>-->
          <%}
      else
      { %>
            <div id=titleArea>
                <image id=logo src=https://static.realmofempires.com/images/fancylogo.png />
                <div class="titleseparator"></div>
                <span id=title><%=realm.Name%><div><%=realm.Desc%></div></span>
                <div class="titleseparator"></div>
            </div>
             <div id="background">
                    <img src="https://static.realmofempires.com/images/backgrounds/M_BG_Login.jpg" class="stretch" alt="" />
                </div>
            <%} %>
    
    <br />
    <br />
<div class=container>
 
    <%if (!isMobile) { %>
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
    <%} %>



	<div class=column>
    <%if (!isMobile) { %>
         <div class="realmRegTitle fontSilverFrSClrg">Choose a Quadrant...</div>
    <%} %>

    <%if (isMobile) { %>
    <section id="realmregister">
        
        <div class="titleseparator"></div>
        <div class="fg">
            <br />
            <a href='LogoutOfRealm.aspx?isM=1' >
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
                            <span><%= RS("BackToRealmSelection")%></span><br>
                        </div>
                    </div>
                </div></a>
                    
        <%  }    %>
    <table class=registrationForm>
        <asp:Panel runat="server" ID="panelStartIn">
            <tr>
                <td class="tableColLeft">
                    <span class="startin"><%= RS("startIn")%>: </span>
                <%if (!isMobile) { %>
                </td>
                <td class="tableColRight">
                <%}%>
                    <asp:DropDownList ID="DropDownList1" runat="server">
                        <asp:ListItem Value="0">Random (optimal) Location</asp:ListItem>
                        <asp:ListItem Value="1">North East</asp:ListItem>
                        <asp:ListItem Value="2">South East</asp:ListItem>
                        <asp:ListItem Value="3">South West</asp:ListItem>
                        <asp:ListItem Value="4">North West</asp:ListItem>
                    </asp:DropDownList>
                    <div id='startInDiscl' style="font-size: 10pt;">
                        <%= RS("tryToPlace")%>
                    </div>

                    <script type="text/javascript">
                        $("#startInDiscl").hide();
                        $('#DropDownList1')
                            .change(function () {
                                if ($(this).val() > 0)
                                    $("#startInDiscl").show();
                                else
                                    $("#startInDiscl").hide();
                            });
                    </script>

                </td>
            </tr>
        </asp:Panel>
        <tr>
            <%if (!isMobile) { %>
            <td>
            </td>
            <%}%>

            <td>
                 <%if (!isMobile) { %>
                    <a id="btnBack" href="LogoutOfRealm.aspx">Back</a>
                <% } %>
                    <asp:Button ID="btnGo" runat="server" OnClick="Button1_Click" Text='<%# RS("btn_GO") %>' CssClass=inputbutton  />
                    <asp:LinkButton ID="btnGoLink" runat="server" OnClick="Button1_Click" Text='<%# RS("btn_GO") %>' CssClass=ThemeA />
                    
                    
                <div id="tournamentRealmInfo" runat="server" visible="false" style="float: right;">
                    <%= RS("NOTE")%>
                </div>
            </td>
    </table>
    <%if (isMobile) { %>
            
        </div>
    </section>
        <%} %>


    </div>

                <!-- <center><br /><br /><a styl="font-size:20px;font-weight:bold;" href="logoutofrealm.aspx" > < Back</a></center>-->


    <div class="titleseparator"></div>
    </div>
    </form>
</body>
</html>
