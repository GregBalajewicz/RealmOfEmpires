<%@ Page Language="C#" MasterPageFile="~/master_PopupFullFunct.master" AutoEventWireup="true" CodeFile="SleepMode_Active_NoLogin.aspx.cs" Inherits="SleepMode_Active_NoLogin" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <asp:Label ID="SleepTimer" runat="server" class="SleepTimer" style="display:none" />
    <asp:Label ID="redir" runat="server" class="redirID" style="display:none" />
    

    <%if (!isMobile)
      { %>

                    <% if(isD2) { %>
    <style>
        body {
            font-family: Georgia, 'Playfair Display';
            font-size: 12px;
        }

        html {
            background-color: #000000;
            background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg') no-repeat center center fixed;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
        }

        .TDContent {
            background-color: rgba(6, 20, 31, 0.9) !important;
            height: 100%;
            position: absolute;
            overflow: auto;
            color:#fff;
        }

        .BoxHeader {
            color: #FFFFFF;
            font-size: 14px !important;
            font-weight: normal;
            background-color: rgba(49, 81, 108, 0.7) !important;
            padding: 4px;
        }

        .d2_inputbutton {
           
            color: #FFFFFF;
            font-weight: initial;
            padding: 4px !important;
            padding-bottom: 3px !important;
            background-color: #181819 !important;
            -moz-box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            -webkit-box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            border: 1px solid #A69D85;
            -moz-border-radius: 6px;
            -webkit-border-radius: 6px;
            border-radius: 6px;
            font-size: 12px !important;
            font-family: Georgia, 'Playfair Display';
            height: initial;
            margin: 4px 0px;
            text-decoration:none;
            display: block;
            width: 120px;
        }

        .d2_inputbutton:hover {
             text-decoration:none !important;
             cursor:pointer;
        }

        .d2_boxedContent {
            padding:6px !important;
        }

        .d2_tableWrapper {
            border-spacing:0 !important;
        }

        .d2_tableWrapper td {
            padding: 0px;
            padding-bottom: 6px;
        }

      
        .d2_border {
           
            border:none !important;
        }

        .d2_stripeRow {
            background-color: rgba(88, 140, 173, 0.1);
        }

        .d2_stripeRowCol {
            padding:0;
        }

        .d2_container {
            font-size:14px;
        }
    </style>
    <% } %>
    <div style="font-size: 1.3em">
        <center>
           <% if(!isD2) { %> <img src='<%=RSic("Logo")%>' />
            <% } else { %>
            <br />
            <br />
            <% } %>
        <br />
        <%=RS("AccountIsCurrentlyInSleepMode") %> <br />
        <%=RS("InOrderToLoginYouMustCancelSleep") %>
        <br /><%=RS("RememberIfYouCancelEarly") %><br /><br />

            <div class="sleepMode countdown" refresh="false" style="font-size: 22px; height:27px;" ></div><br />

            <asp:LinkButton ID="LinkButton1" CssClass="d2_inputbutton" runat="server" onclick="LinkButton1_Click"><%=RS("LoginAndCancelSleepMode") %></asp:LinkButton>
        <br />
            

        <br /><a href="LogoutOfRealm.aspx" class="d2_inputbutton"><%=RS("CANCELLOGIN") %></a>   
    </div>
    </center>
    </div>
    <% }
       else{ %>
        <STYLE>
            #mobilecontent {
            text-align:center;
            }
            #background {
            width: 100%; 
            height: 100%; 
            position: fixed; 
            left: 0px; 
            top: 0px; 
            z-index: -1;
            }  
            #background > .stretch  {
            width:100%;
            height:100%;
            }
            .titletoplogo {
            background: url('https://static.realmofempires.com/images/misc/RegistrationHeader.png') no-repeat center;
            width:100%;
            height:57px;
            margin: -112px auto 0 auto;
            }
            .titlenew {
            text-align:center;
            width:100%;           
            }
            .titlelogo { 
            margin: -89px 2px;
            text-align: left;
            }
            .titletext {
            font: 26px/1.2em "IM Fell French Canon SC", serif;
            color:white;
            text-shadow: 2px 2px 1px #000000;
            text-align: center;
            padding: 0 0 0 30px;
            }
            .titletext1, .BackButton .label SPAN {
            font: 16px/0.8em "IM Fell French Canon SC", serif;
            color:white;
            text-shadow: 2px 2px 1px #000000;
            text-align: center;
            padding: 0 0 6px 0;
            text-transform:initial;
            }
            .titleseparator {
            height:18px;
            width: 100%;
            text-align: center;
            margin: 1px auto 0 auto;
            background: url('https://static.realmofempires.com/images/misc/m_listbar.png') no-repeat center;
            }
            .titleseparator2 {
            height:18px;
            width: 100%;
            text-align: center;
            margin: -6px auto 0 auto;
            background: url('https://static.realmofempires.com/images/misc/m_listbar.png') no-repeat center;
            }
            #logo { 
            width:80px;
            height:97px;
            }
            .sleepMode {
            color: white;
            font: 24px/1.0em "Georgia", serif;
            height:24px;
            }
            .bottom {
            position: absolute;
            bottom: 0;
            height:239px;
            width:100%;
            text-align:center;
            background: rgba(0,0,0,0.5);
            }
            .sleepText2{
            font: 16px/1.0em "IM Fell French Canon SC", serif;
            color: #FFD776;
            text-shadow: 2px 2px 1px #000000;
            text-align: center;
            }
            .sleepText3{
            font: 14px/1.1em "IM Fell French Canon LC", serif;
            color: #acacac;
            padding:0 10px;
            text-shadow: 2px 2px 1px #000000;
            text-align: center;
            }
            .moon {
                float:right;
                margin: -51px 10px 0 0;
            }
            .spacerTop {
                width: 100%;
                height: 20px;
                background: url(https://static.realmofempires.com/images/misc/M_SpacerBottom.png) no-repeat center;
            }
            .spacerBottom {
                width: 100%;
                height: 20px;
                background: url(https://static.realmofempires.com/images/misc/M_SpacerTop.png) no-repeat center;
            }
            A:active, A:hover {
                text-decoration: none !important;
            }
            #mobilecontent .themeM-more {
                float:left;
            }
            #mobilecontent .themeM-more > .fg > .arrow-l {
                left:2px;
            }
        </STYLE>
 
        
            <img id="background" src="https://static.realmofempires.com/images/backgrounds/M_BG_Sleep.jpg" class="stretch" alt="" />
            
                <div id="mobilecontent">
                    
                    <DIV class="titlenew">
                        <div class="titleseparator"></div>                        
                            <div class="titletext"><%=RS("SleepMode") %></div> 
                            <div class="titletext1 realmID"><%=RS("Realm") %> <asp:Label ID="RealmID" runat="server"  /></div>
                            <img class="moon" src="https://static.realmofempires.com/images/icons/M_MoreSleep.png" ></img>
                        <div class="titleseparator"></div>
                    </DIV>                    
                    <div class="titlelogo"><img id=logo src="<%=RSi("fancyLogo") %>" /></div>

                   
                    <div class="bottom">
                        <div class="titleseparator2"></div>

                        <a href="LogoutOfRealm.aspx">
                            <div class="BackButton themeM-panel style-link BarTouch sfx2">
                                <div class="bg">
                                    <div class="corner-br"></div>
                                </div>

                                <div class="fg">
                                    <div class="themeM-more">
                                        <div class="bg">
                                        </div>

                                        <div class="fg">
                                            <div class="label">
                                                <span></span><br />
                                            </div>

                                            <div class="arrow-l"></div>
                                        </div>
                                    </div>

                                    <div class="label">
                                        <span><%=RS("BACK") %></span><br />
                                    </div>
                                </div>
                            </div>
                        </a>
                        
                        <div class="sleepMode countdown" refresh="false" ></div>
                        <div class="spacerTop" ></div>
                        <div class="sleepText2" ><%=RS("AccountIsCurrentlyInSleepMode") %></div>
                        <div class="sleepText3">
                            <%=RS("InOrderToLoginYouMustCancelSleep") %> <br />
                            <%=RS("RememberIfYouCancelEarly") %>
                        </div>
                        <div class="spacerBottom" ></div><br />
                        <asp:LinkButton ID="LinkButton2" runat="server" onclick="LinkButton1_Click">
                            <div class="customButtomBG sfx2"><%=RS("LOGIN") %></div>
                            </asp:LinkButton>

                    </div>
                    
            </div>     
        <% } %>
        <script>
            $(document).ready(function(){

                var SleepTimer = $(".SleepTimer").text().split(".");
                $(".sleepMode").text(SleepTimer[0]);

                var redir = $(".redirID").text();
                $(".sleepMode").attr("redir",redir );
                        
                initTimers();

                //incase the counter stuck force redir
                setInterval(function () {
                    var count = $(".sleepMode").text();

                    if (count == '00:00:00' || count == "Overdue!" || parseInt(count) <0 ) {

                        window.location.assign(redir);
                    }
                }, 300);

            });
            
            
        </script>   
    

</asp:Content>
