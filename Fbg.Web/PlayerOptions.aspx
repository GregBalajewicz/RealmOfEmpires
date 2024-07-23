<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="PlayerOptions.aspx.cs" Inherits="PlayerOptions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">

<%if (isMobile)
  { %>

    <script>
        function resetLoginType() {
            if (confirm('Are you sure? Use this only when directed by support')) {
                localStorage.clear();
                if (window.parent) {
                    window.parent.location = 'chooselogintype.aspx';
                } else if (window.opener) {
                    window.opener.location = 'chooselogintype.aspx';
                } else {
                    window.location = 'chooselogintype.aspx';
                }
            }
        }

    </script>

<style>
   .optionsPageTitle, .highlightSleep {display:none;}
   
</style>
<%} else { %>
<style>
   .optionsPageTitle {font-size: 1.3em }
</style>
<%} %>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

<%=Utils.GetIframePopupHeaderForNotPopupBrowser(RS("ToolsAndOptions") , (isMobile && !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/M_Settings.png")%>
    <span class=optionsPageTitle ><%=RS("ToolsAndOptions") %><br />
    </span>
    <table class="TypicalTable stripeTable" cellspacing="1">
        <tr class="highlight"  <%if (isMobile) { %>style="display: none;" <%}%> >
            <td class="TableHeaderRow">
                <%=RS("Anonymous") %>?
            </td>
            <td>
                <asp:CheckBox ID="chk_Anonymous" runat="server" Text='<%#RS("IWantAnonymous") %>' Font-Bold="True" AutoPostBack=true OnCheckedChanged="playerOptionChanges" /><br />
                <%=RS("ThisWillHideFacebookProfile") %><br />
            </td>
        </tr> 
        <%if (!isMobile) {%>"
        <tr class="highlight">
            <td class="TableHeaderRow">
                <%=RS("MyTimezone") %>
            </td>
            <td>
                <asp:DropDownList ID="ddlTimeZone" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlTimeZone_SelectedIndexChanged">
            
                    <asp:ListItem Value="-12">(GMT-12:00) International dateline</asp:ListItem>
                    <asp:ListItem Value="-11">(GMT-11:00) Midway Islands</asp:ListItem>
                    <asp:ListItem Value="-10">(GMT-10:00) Hawaii</asp:ListItem>
                    <asp:ListItem Value="-9">(GMT-09:00) Alaska</asp:ListItem>
                    <asp:ListItem Value="-8">(GMT-08:00) Pacific Time (USA)</asp:ListItem>
                    <asp:ListItem Value="-7">(GMT-07:00) Mountain Time (USA)</asp:ListItem>
                    <asp:ListItem Value="-6">(GMT-06:00) Central time (USA)</asp:ListItem>
                    <asp:ListItem Value="-5">(GMT-05:00) Eastern time (USA)</asp:ListItem>
                    <asp:ListItem Value="-4">(GMT-04:00) Atlantic Time (Canada)</asp:ListItem>
                    <asp:ListItem Value="-3.5">(GMT-03:30) Newfoundland</asp:ListItem>
                    <asp:ListItem Value="-3">(GMT-03:00) Brasilia</asp:ListItem>
                    <asp:ListItem Value="-2">(GMT-02:00) Mid-Atlantic</asp:ListItem>
                    <asp:ListItem Value="-1">(GMT-01:00) Azorerne</asp:ListItem>
                    <asp:ListItem Value="0" Selected="True">(GMT) Greenwich Mean Time: London</asp:ListItem>
                    <asp:ListItem Value="1">(GMT+01:00) Amsterdam, </asp:ListItem>
                    <asp:ListItem Value="2">(GMT+02:00) Athen, Istanbul, Minsk</asp:ListItem>
                    <asp:ListItem Value="3">(GMT+03:00) Moscow, St. Petersburg</asp:ListItem>
                    <asp:ListItem Value="3.5">(GMT+03:30) Teheran</asp:ListItem>
                    <asp:ListItem Value="4">(GMT+04:00) Abu Dhabi, Muscat</asp:ListItem>
                    <asp:ListItem Value="4.5">(GMT+04:30) Kabul</asp:ListItem>
                    <asp:ListItem Value="5">(GMT+05:00) Islamabad, </asp:ListItem>
                    <asp:ListItem Value="5.5">(GMT+05:30) Kolkata, New Delhi</asp:ListItem>
                    <asp:ListItem Value="5.75">(GMT+05:45) Katmandu</asp:ListItem>
                    <asp:ListItem Value="6">(GMT+06:00) Astana, Dhaka</asp:ListItem>
                    <asp:ListItem Value="6.5">(GMT+06:30) Rangoon</asp:ListItem>
                    <asp:ListItem Value="7">(GMT+07:00) Bangkok, Hanoi</asp:ListItem>
                    <asp:ListItem Value="8">(GMT+08:00) Beijing, Chongjin</asp:ListItem>
                    <asp:ListItem Value="9">(GMT+09:00) Osaka, Sapporo</asp:ListItem>
                    <asp:ListItem Value="9.5">(GMT+09:30) Adelaide</asp:ListItem>
                    <asp:ListItem Value="10">(GMT+10:00) Canberra, Melbourne</asp:ListItem>
                    <asp:ListItem Value="11">(GMT+11:00) Magadan, Solomon Islands</asp:ListItem>
                    <asp:ListItem Value="12">(GMT+12:00) Fiji, Kamtjatka</asp:ListItem>
                    <asp:ListItem Value="13">(GMT+13:00) Nuku'alofa</asp:ListItem>
              
            </asp:DropDownList>
                <br />
                <%=RS("MyCurrentTimeIs") %>
                <asp:Label ID="lbl_CurrentTime" runat="server"></asp:Label><br />
            </td>
        </tr>
        <%}%>
        
        <tr class="highlight" runat="server" id="rowNBM">
            <td class="TableHeaderRow">
                <%=RS("NightTimeBuild") %>
            </td>
            <td>
                <asp:Label ID="lblNightBuildMsg" runat="server" Font-Bold="True" Visible="False"></asp:Label>
                <asp:Label ID="lblNightBuild_Countdown" runat="server" CssClass="countdown" Font-Bold="True" Visible="False"></asp:Label>
                <asp:LinkButton ID="linkNightBuild_Activate" Font-Bold="True" runat="server" OnClick="linkNightBuild_Activate_Click" Visible="False"><%=RS("ClickToActivate") %></asp:LinkButton><br />
                <%=RS("NightTimeBuild") %>
                <br />
                <%=RS("UsingNightTimeBuild") %> <a href="PFBenefits.aspx"><%=RS("UnlockingUnlimitedBuilding") %></a>)
            </td>
        </tr>
        <tr class="highlightSleep">
            <td class="TableHeaderRow">
            <img src="https://static.realmofempires.com/images/misc/SleepIcon48.png" style=float:left; />
                <%=RS("SleepMode") %>
            </td>
            <td class="highlight">
                <asp:Panel runat="server" ID="panelSleepModeIsNotActive" Visible="false">
                    <%=RS("SleepModeNotAvailable") %></asp:Panel>
                <asp:Panel runat="server" ID="panelSleepModeIsActive" Visible="false">
                    <asp:Label ID="lblSleepModeCanBeActivatedIn" runat="server" Font-Bold="True" Visible="False"><%=RS("SleepModeCanAgain") %></asp:Label>
                    <asp:Label ID="lblSleepModeActive" runat="server" Font-Bold="True" Visible="False"><%=RS("SleepModeIsActive") %></asp:Label>
                    <asp:Label ID="lblSleepModeActivating" runat="server" Font-Bold="True" Visible="False"><%=RS("SleepModeWillActivate") %></asp:Label>
                    <asp:Label ID="lblSleepMode_Countdown" runat="server" CssClass="countdown" Font-Bold="True" Visible="False"></asp:Label>
                    <asp:LinkButton ID="linkActivateSleepMode" Font-Bold="True" runat="server" Visible="False" OnClientClick="return confirm('Are you sure? There is no way to cancel this without using up your sleep mode in this time period.')" OnClick="linkActivateSleepMode_Click"></asp:LinkButton><br />
                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Visible="False"></asp:Label>
                    <asp:Label ID="Label2" runat="server" CssClass="countdown" Font-Bold="True" Visible="False"></asp:Label>
                    <asp:Label ID="lblSleepMode" runat="server"></asp:Label>
                </asp:Panel>
            </td>
        </tr>
        <%if (!isMobile) {%>

        <tr class="highlight">
            <td class="TableHeaderRow">
                <%=RS("EmailAlerts") %>
            </td>
            <td class="highlight">
                <asp:Panel ID="panelEmails_NotForStewards" Visible="false" runat="server"><%=RS("OptionNotAvailable") %></asp:Panel>
                <asp:Panel ID="panelEmails_NoEmail" runat="server">
                    <%=RS("YouMayReceiveEmailAlerts") %>
                    <br />
                    <%=RS("AnExampleOfAnAlert") %>
                    <br />
                    <br />
                    <%=RS("InOrderToReceiveAlerts") %> 
                    <br /><br /><%=RS("WeDoNotShareYourEmail") %> 
                </asp:Panel>
                <asp:Panel ID="panelEmails_GotEmail" runat="server">
                    <asp:Label ID="lblEmial_ConfirmationMsg" runat="server" Text="Label"></asp:Label> 

                    <div class="jaxHide" rel="(change email)">
                        <div style="margin:20px;padding:10px;border:solid 1px brown">
                        <%=RS("ForSecurity")%>
                        <br /><br /> <%=RS("IfYouChange")%>
                        </div>
                    </div>

                    <br /><br /><asp:Label ID="lblEmail_OptOut" runat="server" Text='<%=RS("IfYouChooseNotToAlert") %> '></asp:Label>
                    <asp:LinkButton ID="linkEmail_OptOutLink" runat="server" onclick="lblEmail_OptOutLink_Click"></asp:LinkButton>
                    
                    <br /><br /><%=RS("NoteToEnsureYouDontMiss") %> <U>PlayerAlerts@RealmOfEmpires.com</U> <%=RS("ToSafeSendersList") %>
                </asp:Panel>
            </td>
        </tr>
        <%} %>
        <%if (!isMobile) { %>
        <tr class="highlight">
            <td class="TableHeaderRow">
                <%=RS("Headquarters")%>
            </td>
            <td class="highlight">
                <asp:Label runat="server" id="lblAdvancedHQModeActive"><%=RS("AdvancedHeadquarters") %></asp:Label> <asp:LinkButton ID="linkAdvancedHQ" runat="server" OnClientClick="return confirm('Are you sure? Do this only if you are a veteran player') " onclick="linkAdvancedHQ_Click"><%=RS("AdvancedHQ") %></asp:LinkButton>
            </td>
        </tr>
        <%} %>
        <%if (!isMobile && FbgPlayer.NumberOfVillages == 1 && !IsLoggedInAsSteward)
          { %>
        <tr class="highlight">
            <td class="TableHeaderRow">
                <%=RS("RestartRealm")%>
            </td>
            <td class="highlight">
                <a id="restart" runat="server" onclick="ROE.Restart.showPopup();"><%=RS("Restart") %></a><%=RS("RestartRealmDesc")%>
            </td>
        </tr>
        <%} %>
   
    </table>
    <br />
    <br />
    <%if (isMobile) { %>
        <center>
            <BR><a href="http://www2.realmofempires.com/tou.aspx">Terms of Use</a>
            <BR><BR><BR><a href="http://www2.realmofempires.com/about_credits.aspx">Credits</a>
                <% if (Device != CONSTS.Device.Amazon){ %>
                <BR><BR><BR><BR><a id="A2" runat="server" onclick="resetLoginType();">Reset Login Type</a>
                <%} %>
                <BR><BR><BR><BR><BR>Contact support @ support@RealmOfEmpires.com


        </center>
    <%} %>
</asp:Content>
