<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Create.aspx.cs" Inherits="Create" ValidateRequest="false" MasterPageFile="~/main2.master"%>

<%@ Register Src="Controls/Tutorial.ascx" TagName="Tutorial" TagPrefix="uc1" %>
<%@ Register Src="Controls/NewPlayerIntro.ascx" TagName="NewPlayerIntro" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMeta" runat="Server">
    <title>Create | <%=  RSc("GameName")%></title>
    <meta name="description" content="Welcome back to Realm of Empires." />

    
     <%if (!isMobile){ %>
        
    <style>
          

        #form1 {
            position: absolute;
            right: 0px;
            top: 0px;
            left: 0px;
            bottom: 0px;
            overflow-x: hidden;
            overflow-y: auto;
        }

        .container {
            position: absolute;
            width: 600px;
            top: 28%;
            left: 50%;
            margin-left: -300px;
            background-color: rgba(0, 0, 0, 0.8);
            border-radius: 20px;
            padding: 20px;
            padding-top: 50px;
        }

            .container .column {
                position: relative;
                display: table-cell;
                width: 50%;
                vertical-align: top;
                z-index: 3;
            }

         .nickname, .startin, .fieldlabel {
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

        #realmNameSection {
            position: relative;
            padding: 20px;
            padding-top: 10px;
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
            font: 15px/1.0em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            outline: none;
            text-decoration: none;
            padding-top: 11px;
        }

        .realmParamTitle, .realmRegTitle {
            font-weight: bold;
            text-align: center;
            margin-bottom: 3px;
        }

        </style>

    <%} else { %>

      <style>

          #form1 {
              height: 100%;
              width: 100%;
          }
          .container {
              position: absolute;
              top:110px;
              bottom:0px;
              left:0px;
              right:0px;
              background: rgba(0,0,0,0.50);
              overflow-x: hidden;
              overflow-y: auto;
          }
          .container .column {
              display: inline;
              width: 100%;
          }

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
            padding-top:15px;
        }
        .nickname, .startin, .fieldlabel {
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
            z-index: -1; /* Ensure div tag stays behind content; -999 might work, too. */
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
            margin-top: -14px;
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
</asp:Content>


<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">

    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <%if (!isMobile) { %>

    <%} else { %>

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

    <uc2:NewPlayerIntro ID="NewPlayerIntro1" runat="server" />
    
<div class="container" >
    
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
            <div class="realmRegTitle fontSilverFrSClrg">Please Register...</div>
        <%} %>

    <%if (isMobile) { %>
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
                            <span><%= RS("BackToRealmSelection")%></span><br>
                        </div>
                    </div>
                </div></a>
                    
        <%  }    %>


    <table class=registrationForm>
        <tr>
            <td valign="top">
                <span class="nickname fontSilverFrSClrg" ><asp:Label ID="Label2" runat="server" Text='<%# RS("lbl_Nickname")%>' ></asp:Label></span>
            <%if (!isMobile) { %>
            </td>
            <td class="fontSilverFrLCmed">
            <%}%>
                <asp:TextBox ID="txtNickName" runat="server" CssClass="TextBox" MaxLength="25" Width="55%" />
                    <BR />
                <%= RS("chooseWisely")%>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNickName"
                    ErrorMessage='<%# String.Concat("<br/>", RS("rfv_EnterName")) %>' CssClass="Error"
                    ForeColor="" Display="Dynamic" Font-Size="11px" />
                <asp:Label ID="lbl_Error" runat="server" Text="<BR>Player Name already in use. Please try a different name"
                    Visible="False" CssClass="Error" Font-Size="11px" />
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtNickName"
                    ErrorMessage='<%# String.Concat("<br/>", RS("rev_InvUserName")) %>' ValidationExpression="^[a-zA-Z0-9._]{1,25}$"
                    CssClass="Error" ForeColor="" Font-Size="11px" Display="Dynamic" />
            </td>
        </tr>
        <asp:Panel runat="server" ID="panelStartIn">
            <tr>
                <td>
                    <span class="startin"><%= RS("startIn")%>: </span>
                <%if (!isMobile) { %>
                </td>
                <td >
                <%}%>
                    <asp:DropDownList ID="DropDownList1" runat="server">
                        <asp:ListItem Value="0">Random (optimal) Location</asp:ListItem>
                        <asp:ListItem Value="1">North East</asp:ListItem>
                        <asp:ListItem Value="2">South East</asp:ListItem>
                        <asp:ListItem Value="3">South West</asp:ListItem>
                        <asp:ListItem Value="4">North West</asp:ListItem>
                    </asp:DropDownList>
                    <span id='startInDiscl' style="font-size: 10pt;">
                        <%= RS("tryToPlace")%>
                    </span>

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
        <asp:Panel runat="server" ID="pnlPassword" Visible="false">
            <tr>
                <td valign="top">
                    <span class="fieldlabel fontSilverFrSClrg">Password:</span>
                <%if (!isMobile) { %>
                </td>
                <td class="fontSilverFrLCmed">
                <%}%>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="TextBox" Font-Size="12px" MaxLength="25" Width="55%" />
                    <BR />
                    This is a private realm that requires a password to enter
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage='Enter a password' CssClass="Error"
                    ForeColor="" Display="Dynamic" Font-Size="11px" />
                      <asp:CustomValidator ID="CustomValidator_password" runat="server" CssClass="Error" Display="Dynamic"
                        ErrorMessage="CustomValidator" Font-Size="11px">
                        Password is incorrect
                    </asp:CustomValidator>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td colspan="2" style="text-align:center;" class="fontSilverFrLCmed">
                    <%= RS("agreeWithTerms")%>
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/tou.aspx" Target="_blank"><%= RS("link_TOU")%></asp:HyperLink>
                    <span style="font-size: 10pt"></span>
                    <asp:CheckBox ID="cbAgreeToTOU" runat="server" Checked="True" />
                    <asp:CustomValidator ID="CustomValidator_TOU" runat="server" CssClass="Error" Display="Dynamic"
                        ErrorMessage="CustomValidator" Font-Size="11px">
                        <br/><%= RS("mustAgree")%>
                    </asp:CustomValidator><br />
                
                <asp:Panel ID="tournamentRCOnfirm" runat="server" style="font-size: 10pt" Visible=false>
                    I agree with <a target="_blank" href="http://realmofempires.blogspot.ca/2017/01/internet-connection-sharing-rule-for.html" >the no interaction rule</a> when sharing an internet connection <asp:CheckBox ID="cbNoSharing" runat="server" Checked="false" /> 
                    <asp:CustomValidator ID="CustomValidator_Sharing" runat="server" CssClass="Error" Display="Dynamic"
                        ErrorMessage="CustomValidator" Font-Size="11px">You must agree to this rule</asp:CustomValidator> 
                </asp:Panel>
                <asp:Panel ID="entryFee" runat="server" style="font-size: 10pt" Visible=false>
                    <asp:Label ID="lblEntryFee" runat="server" Text="Label"></asp:Label> <asp:CheckBox ID="cbEntryFee" runat="server" Checked="false" />
                    <asp:CustomValidator ID="CustomValidator_entryFee" runat="server" CssClass="Error" Display="Dynamic"
                        ErrorMessage="CustomValidator" Font-Size="11px">This realm has an entry fee. You must agree to this if you want to enter</asp:CustomValidator> 
                    <asp:CustomValidator ID="CustomValidator_entryFee_noservants" runat="server" CssClass="Error" Display="Dynamic"
                        ErrorMessage="CustomValidator" Font-Size="11px">You don't have enough servants. Login to another realm and purchase some</asp:CustomValidator> 
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:center;">
                <a id="btnBack" href="chooserealm.aspx">Back</a>
                <asp:Button ID="btnGo" runat="server" OnClick="Button1_Click" Text='<%# RS("btn_GO") %>' CssClass=inputbutton  />
                <asp:LinkButton ID="btnGoLink" runat="server" OnClick="Button1_Click" Text='<%# RS("btn_GO") %>' CssClass=ThemeA />          
                <div id="tournamentRealmInfo" runat="server" visible="false" style="float: right;"><%= RS("NOTE")%></div>
            </td>
        </tr>
    </table>
    <%if (isMobile) { %>
            
        </div>
    </section>
        <%} %>


    </div>

    <div class=column>
        <%if (!isMobile) { %>
            <div class="realmParamTitle fontSilverFrSClrg">Realm Parameteres</div>
        <%} %>
        <asp:Panel runat="server" ID="pnlRealmInfo" Visible="false" Style="padding: 4px;border: solid 1px rgba(88, 83, 50, 0.7);border-radius: 5px;background: rgba(48, 48, 48, 0.4);">
            <asp:Label ID="lblRealmInfo" runat="server" Text="" CssClass="fontSilverFrLCmed"></asp:Label>
        </asp:Panel>
   </div>
    <div id="lblErrorNotEnoughCredits" visible="false" runat="server">
        <span class="Error"><%= RS("notEnoughServants")%></span>
        <a href="chooserealm.aspx"><%= RS("link_BuyServants")%></a>
        <%= RS("beforeEntering")%>
    </div>
    
    <br />
    <uc1:Tutorial ID="Tutorial1" runat="server" />

    <div class="titleseparator"></div>
    </div>
    </form>


</asp:Content>
