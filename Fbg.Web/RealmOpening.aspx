<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RealmOpening.aspx.cs" Inherits="RealmOpening" MasterPageFile="~/main2.master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMeta" runat="Server">

    <title>Realm Opening | <%=  RSc("GameName")%></title>
    <meta name="description" content="Welcome back to Realm of Empires." />
    <script src="script/countdown_old.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(initTimers);
    </script>

    

    <%if (isMobile) { %>
      <style>
            body {width:100%;  }
            #background {
                width: 100%; 
                height: 100%; 
                position: fixed; 
                left: 0px; 
                top: 0px; 
                z-index: -1; /* Ensure div tag stays behind content; -999 might work, too. */
            }
            .stretch {
                width:100%;
                height:100%;
            }
            #mobilecontent {
              position:relative;
              height: 95px;
            }

            #titleArea {
                margin:  3px auto 45px auto;
            }
            .titletoplogo {
                background: url('https://static.realmofempires.com/images/misc/RegistrationHeader.png') no-repeat center;
                width:100%;
                height:57px;
                margin: -111px auto 0 auto;
              }
          .titlenew {
            text-align:center;
            width:100%;
            background: rgba(0,0,0,0.4);  
            margin: 52px auto 0 auto;              
          }
          .titlelogo { 
             margin: -85px 6px;
             text-align: left;
          }
          .titletext {
            font: 24px/1.0em "IM Fell French Canon SC", serif;
            display: inline-block;
            color:white;
            padding: 9px 0 8px 76px;
            text-shadow: 2px 2px 1px #000000;
            text-align: center;
            height: 50px;
          }
          #logo { 
              width:80px;
              height:97px;
          }
          .titleseparator {
                height:18px;
                width: 100%;
                text-align: center;
                margin: 46px auto 0 auto;
                background: url('https://static.realmofempires.com/images/misc/m_listbar.png') no-repeat center;
          }
          .titleseparator2 {
              position: absolute;
              height: 18px;
              width: 100%;
              text-align: center;
              margin: -6px auto;
              background: url('https://static.realmofempires.com/images/misc/m_listbar.png') no-repeat center;
          }
          #title { 
            text-shadow: 2px 2px 1px #000000;
            font: 26px/1.0em "IM Fell French Canon SC", serif; 
            display:block; 
            padding: 12px 10px 12px 75px;
            color: white; 
            text-align:center;           
            }
          #title DIV{ 
            font: 16px/1.0em "IM Fell French Canon SC", serif; 
            text-transform: none;
            }

          #realmregister {
            text-align:center;
            width:100%;
            background: rgba(0,0,0,0.4);  
            margin: 15px auto 0 auto;
          }
          .bgborder {
            display: block;
            margin: 20px 10px;
            padding: 20px 10px;
            background: rgba(0,0,0,0.50);
            border: 2px solid #846824;
            -webkit-border-radius: 15px;
            border-radius: 15px;
            font: 13px/1.0em "IM Fell French Canon", serif;
            text-shadow: 2px 2px 1px #000000;
          }
          .bgborder >DIV {
              margin:-10px 0 30px 0 !important; 
          }
          .bgborder >DIV >SPAN{
              font: 20px/1.0em "IM Fell French Canon SC", serif !important; 
          }
          .bgborder TD, #panelJoinAnotherRealm {
              font: 16px/1.0em "IM Fell French Canon", serif !important;
              text-shadow: 2px 2px 1px #000000;
              }
          .bgborder #timer0 {
              display: block;
              font: 24px/1.0em "Georgia", serif !important; 
              text-shadow: 2px 2px 1px #000000;
              color: #FFD886;
              margin: 15px 0;
              }
      </style>

          <% } else {%>
    <style>

        #main #content {
            text-align:center;
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

        #HyperLink3 {
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

        #panelTournamentRealm2 {
            padding-top: 10px;
        }
        #panelJoinAnotherRealm2 {
            position:relative;
            padding:5px;
        }

    </style>
    <%} %>

</asp:Content>

    
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
 
    <%if (!isMobile){ %>

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

            <div id="content">
                <span class="fontSilverFrSClrg"><%=RS ("Preresgistered")%></span>
                <br /><br />


                <span class="fontSilverFrSClrg"><%=RS ("RealmOpens")%></span> 
                <asp:Label ID="lblCountdown2" class="countdown" redir="ChooseRealm.aspx" runat="server"></asp:Label>
                <span style="font-size: 10pt"><asp:Label  ID="lblOpensOn2" runat="server"></asp:Label><span>(UTC)</span></span>
       
                <asp:Panel runat=server ID=panelTournamentRealm2>

                    <asp:Panel runat="server" ID="pnlRealmInfo2" Visible="false" Style="padding: 4px; border: solid 1px brown;">
                        <div style="margin-bottom: 2px; padding: 1px; border: solid 1px brown; background-color: Brown; text-align: center;">
                        <%=RS ("Parameters")%>
                        </div>
                        <asp:Label ID="lblRealmInfo2" runat="server" Text=""></asp:Label>
                    </asp:Panel>

                    <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="ChooseRealm.aspx"><%=RSc("back") %></asp:HyperLink>

                </asp:Panel>
                
                <br /><br />

                <asp:Panel runat=server ID=panelJoinAnotherRealm2>
                    <div class="fontSilverFrSClrg"><%=RS ("DontWait")%></div>
                    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/LogoutOfRealm.aspx"><%=RS ("JoinRealm")%></asp:HyperLink>
                </asp:Panel>    
  
            </div> 
        </div>

    <%} else { %>

           <center>

            <img id="background" src="https://static.realmofempires.com/images/backgrounds/M_BG_Login.jpg" class="stretch" alt="" />
                <div id="mobilecontent">
                    
                    <DIV class="titlenew">                        
                            <span id='title'><%=realm.Name%><div><%=realm.Desc%></div></span>
                    </DIV>
                    <div class="titletoplogo"></div>
                    <div class="titleseparator"></div>
                    <div class="titlelogo"><img id='logo' src="<%=RSi("fancyLogo") %>" /></div>


                </div>


        <section id="realmregister">
            <div class="titleseparator2"></div>
            <br />

            <div class="bgborder">

                <div style="position: relative; margin-top: -35px; margin-left: -20px;">
                    <span style="font-size: 13pt;"><%=RS ("Preresgistered")%></span>
                </div>


                <table>
                    <tr>
                        <td style="font-size: 13pt; text-align: center;" width="33%">
                            <%=RS ("RealmOpens")%>
                            <br />
                            <asp:Label ID="lblCountdown" redir="ChooseRealm.aspx" runat="server"></asp:Label><br />
                            <span style="font-size: 10pt">(<asp:Label Style="font-size: 10pt;" ID="lblOpensOn" runat="server"></asp:Label>UTC)</span>
                        </td>
                    </tr>
                </table>

                <asp:Panel runat=server ID=panelTournamentRealm>
                    <br />
                    <asp:Panel runat="server" ID="pnlRealmInfo" Visible="false" Style="padding: 4px; border: solid 1px brown;">
                        <div style="margin-bottom: 2px; padding: 1px; border: solid 1px brown; background-color: Brown; text-align: center;">
                        <%=RS ("Parameters")%>
                        </div>
                        <asp:Label ID="lblRealmInfo" runat="server" Text=""></asp:Label>
                    </asp:Panel>

                    <br />
                    <br />

                    <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="ChooseRealm.aspx"><%=RSc("back") %></asp:HyperLink>             

                </asp:Panel>


            </div>

        <asp:Panel runat=server ID=panelJoinAnotherRealm>
            <div><%=RS ("DontWait")%></div>
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/LogoutOfRealm.aspx"><%=RS ("JoinRealm")%></asp:HyperLink>
        </asp:Panel>

        <asp:Panel runat=server ID=panelPublishStory>
            <hr />
            <center>
                <table border="0">
                    <tr>
                        <td>
                            <img src="https://static.realmofempires.com/images/blushsmily.gif" />
                        </td>
                        <td nowrap style="text-align: center;">
                            <b><%=RS ("PleaseHelp")%></b>
                        </td>
                        <td>
                            <img src="https://static.realmofempires.com/images/blushsmily.gif" />
                        </td>
                    </tr>
                    <tr>
                        <% if (LoginModeHelper.isFB(Request))
                           { %>
                        <td colspan="3" style="text-align: center;">
                            <a href="allowss.aspx?stp=6"><b><%=RS ("Publish")%></a></B>
                            <br />
                            <span style="font-size: 8pt;"><%=RS ("preview")%></span>
                        </td>
                        <% } %>
                    </tr>
                </table>
            </center>
        </asp:Panel>

            
        <br /><br />
  
        <div class="titleseparator2"></div>
    </section>
    </center>


    <%} %>


</asp:Content>
