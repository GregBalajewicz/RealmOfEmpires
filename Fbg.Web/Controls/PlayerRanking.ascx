<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PlayerRanking.ascx.cs" Inherits="Controls_PlayerRanking" %>

<%if (false ) {%><script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4.js" type="text/javascript"></script><%} %>
<%if (false ) {%><script src="script/bda-ui-checkbox.js" type="text/javascript"></script><%} %>
<%if (false ) {%><script src="script/bda-ui-radio.js" type="text/javascript"></script><%} %>


    <style type="text/css">
        tr.selected
        {
            background-color: Maroon;
        }
    </style> 
    
    <script type="text/javascript">
        page.load.push(function() {
            $('.panel .area').keypress(function(e) {
                // only numbers
                if (!(e.which >= 48 && e.which <= 57)) {
                    return false;
                }
                // not more than 2 digits
                if ($(this).val().length >= 2) {
                    return false;
                }
            });
        });
    </script>

<%if (IsMobile)
  { %>
<style>
.playerRanking showFacebookFriends
{
    display:table-cell;
    height:44px;
    width:44px;
}
.playerRanking .showFacebookFriends
, .playerRanking .title
, .playerRanking .areaHelp
, .playerRanking .dividerColumn 
, .playerRanking .clanNameContainer
{
    display:none;
}

.playerRanking .rankinglist td
, .playerRanking .rankinglist th
{
    padding-top:8px;
    padding-bottom:8px;
}

.playerRanking .area
, .playerRanking .searchForPlayer
, .playerRanking .mobOptions
{
    padding-top:8px;
    padding-bottom:8px;
}

.playerRanking .battleStatsColumn 
{
    display:none;
}

/*big pager*/
.playerRanking .ListPager {font-size:14pt;}
.playerRanking .ListPager table {width:100%; }
.playerRanking .ListPager a {width:100%; display:block}


.playerNameContainer .clanName { display:block; margin: 8px 0px 8px 0px;font-style:italic; color:#D3D3D3;}

.playerRanking .TextBox.jsplayers {width:100px;}
.playerRanking  .main {width:100%;}
</style>

<script>
    var playerRanking_clickg = function () {
        if ($('.generalStatsColumn:visible').length === 0) {
            $('.generalStatsColumn').show();
            $('.battleStatsColumn').hide();
            $('.generalStatsColumn span').fadeIn('slow');
            $.cookie("r", "g");
        }
    };
    var playerRanking_clickb = function () {
        if ($('.battleStatsColumn:visible').length === 0) {
            $('.battleStatsColumn').show();
            $('.generalStatsColumn').hide();
            $('.battleStatsColumn span, .battleStatsColumn a').fadeIn('slow');
            $.cookie("r", "b");
        }
    }

    var playerRanking_showClanChecked = function () {
        ROE.UI.Sounds.click();
        if ($('.playerNameContainer .clanName').length > 0) {
            $('.playerNameContainer .clanName').fadeOut('slow', function () { $(this).remove() });
            $.cookie("cn", "0");
        } else {
            $('.clanName').each(function () {
                $($(this).parents('tr')[0]).find('.playerNameContainer').append($(this).clone().fadeIn('slow'));
                $.cookie("cn", "1");
            })
        }
    };



    var playerRanking_radioClicked = function (index) {
        ROE.UI.Sounds.click();
        if (index === 0) {
            playerRanking_clickg();
        } else {
            playerRanking_clickb();
        }
    };

    page.load.push(function () {
        var ch = BDA.UI.CheckBox.init($('#showClanNameCheck'), playerRanking_showClanChecked, '<%=RS("ShowClanNames")%>', $.cookie("cn") === "1");
        var radios = BDA.UI.Radio.init($('#statType'), playerRanking_radioClicked, ['<%=RS("General")%>', '<%=RS("BattleStats")%>'], $.cookie("r") === "b" ? 1 : 0);
        if ($.cookie("r") == "b") {
            playerRanking_clickb();
        } else if ($.cookie("r") == "b") {
            playerRanking_clickg();
        }

        if ($.cookie("cn") === "1") {
            playerRanking_showClanChecked();
        }


        //$('.rankinglist').delegate("tr", 'click',
        //function (event) {
        //    if ($(event.target).parent("tr").length >0  &&
        //    $($(event.target).parent("tr")[0]).find('a.playerName').length > 0) {
        //         window.location = $($(event.target).parent("tr")[0]).find('a.playerName').attr('href');
        //    }
        //});

    }
    );
</script>
<%} else { %>
<style>
.playerRanking .areaHelp
{ 
    display: inline-block;float:right
}
</style>
<%}%>
<div class=playerRanking>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" border="0" class=main>
                <tr>
                    <td>
                        <table cellpadding="1" cellspacing="1" class="TypicalTable" border="0" width="100%">
                            <tr class="TableHeaderRow">
                                <td colspan="2" class=area>
                                    <asp:Panel runat="Server" ID="pnlArea" Width="100%" CssClass="panel">
                                        <asp:Label ID="lblCurArea" runat="server" Text=""></asp:Label>
                                        <div class="jaxHide sfx2" rel=<%=RS ("Change") %> style="display:none">
                                            <%=RS ("ShowAreaNumber") %><asp:TextBox runat="server" ID="txtArea" Text="" Width="25" CssClass="TextBox area"></asp:TextBox>&nbsp;
                                            <asp:Button runat="Server" ID="btnChangeArea" Text='<%#RS ("ShowArea") %>' CssClass="inputbutton sfx2" CommandName="one" OnCommand="btnChangeArea_Command" />
                                            <asp:Button runat="Server" ID="btnShowAllAreas" Text='<%#RS ("ShowPlayerAll") %>' CssClass="inputbutton sfx2" CommandName="all" OnCommand="btnChangeArea_Command" /><br />
                                            <asp:Label runat="server" ID="Label1" Visible="False" CssClass="Error"></asp:Label>
                                        </div>
                                         <a href="https://static.realmofempires.com/images/misc/areas.jpg?615,534" class=areaHelp  target="_blank"><img border="0" src="https://static.realmofempires.com/images/HelpIcon.png" /></a>
                                    </asp:Panel>
                               </td>
                            </tr>
                            <tr class="TableHeaderRow">
                                <td class=searchForPlayer>
                                    <asp:Panel runat="Server" ID="PnlPlayerSearch" Width="100%">
                                        <strong><%=RS ("FindPlayer") %>&nbsp;</strong><asp:TextBox runat="server" ID="TxtPlayerSearch" Text="" CssClass="TextBox jsplayers"></asp:TextBox>&nbsp;
                                        <asp:Button runat="Server" ID="BtnPlayerSearch" Text='<%#RS ("Find") %>' CssClass="inputbutton sfx2" OnClick="BtnPlayerSearch_Click" /><br />
                                        <asp:Label runat="server" ID="LblPlayerError" Visible="False" CssClass="Error"></asp:Label></asp:Panel>
                                </td>
                                <% if (LoginModeHelper.isFB(Request)){ %>
                                <td valign="top" align="right" width="1%" class=showFacebookFriends>
                                    <asp:Button runat="Server" ID="BtnFBFriends" Text='<%#RS ("ShowFacebookOnly") %>' CssClass="inputbutton" OnClick="BtnFBFriends_Click" />
                                </td>
                                <% } %>
                            </tr>
                            <%if (IsMobile) { %>
                            <tr class="TableHeaderRow">
                                <td class=mobOptions>
                                    <span id=showClanNameCheck style="display:table-cell"></span>
                                    <span id=statType style="display:table-cell"></span>
                                </td>
                            </tr>
                            <%} %>
                        </table>
                        <asp:Panel Visible="false" runat="Server" ID="panelInviteMoreFriends" Width="100%">
                            <table cellspacing="0" cellpadding="2" border="0" class="Box">
                                <tr>
                                    <td style="padding: 4px;" class="TableHeaderRow" valign="middle" width="1%">
                                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/invite.aspx" ImageUrl="https://static.realmofempires.com/images/navIcons/Invite1.gif"></asp:HyperLink>
                                    </td>
                                    <td>
                                        <p>
                                            <%=RS ("HereFacebookFriends") %></p>
                                        <p>
                                            <string><asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/invite.aspx" Font-Bold="True"><%#RS("InviteMoreFriends") %></asp:HyperLink></string>
                                            <%=RS ("AddAPersonalMessage") %>
                                        </p>
                                        <p>
                                            <%=RS("EnjoyAndAdmiration") %>
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel Visible="false" runat="Server" ID="panelInviteFriends" Width="100%">
                            <table cellspacing="0" cellpadding="2" border="0" class="Box">
                                <tr>
                                    <td style="padding: 4px;" class="TableHeaderRow" valign="middle" width="1%">
                                        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/invite.aspx" ImageUrl="https://static.realmofempires.com/images/navIcons/Invite1.gif"></asp:HyperLink>
                                    </td>
                                    <td>
                                        <p>
                                            <%=RS ("NoFacebookFriends") %>
                                        </p>
                                        <p>
                                            <strong>
                                                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/invite.aspx"><%#RS("InviteYourFriends") %></asp:HyperLink></strong> <%#RS("AddAPersonalMessage") %>
                                        </p>
                                        <p>
                                            <%=RS ("EnjoyAndAdmiration") %>
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="GridView1" runat="server" GridLines="None" CssClass="TypicalTable stripeTable rankinglist" CellPadding="1" CellSpacing="1" AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" PageSize="25" OnRowDataBound="GridView1_RowDataBound" OnPageIndexChanging="GridView1_PageIndexChanging" OnSorting="GridView1_Sorting" OnRowCreated="GridView1_RowCreated" Width=100%>
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <PagerStyle HorizontalAlign="Center" CssClass="ListPager" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageImageUrl="https://static.realmofempires.com/images/LeftArrow.png" FirstPageText='<%=RS ("FirstPage") %>' LastPageImageUrl="https://static.realmofempires.com/images/RightArrow.png" LastPageText='<%=RS ("LastPage") %>' />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Label runat="Server" ID="LblRank"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Wrap="false">
                                    <ItemTemplate >
                                        <asp:HyperLink Style="color: #A6A2A3;" runat="Server" ID="linkTitle" CssClass="title" />
                                        <asp:HyperLink runat="Server" ID="LnkPlayerName" CssClass="playerName sfx2"></asp:HyperLink>
                                    </ItemTemplate>
                                    <ItemStyle CssClass=playerNameContainer />
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="LnkClan" CssClass="clanName"></asp:HyperLink>
                                    </ItemTemplate>
                                    <ItemStyle CssClass=clanNameContainer />
                                    <HeaderStyle CssClass=clanNameContainer />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="NumberOfVillages">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="LblNumbeOfVillages" Text=""></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" CssClass="generalStatsColumn" />
                                    <HeaderStyle CssClass="generalStatsColumn sfx2" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="Total Points"> 
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="LblTotalPoints" Text=""></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" CssClass="generalStatsColumn" />
                                    <HeaderStyle CssClass="generalStatsColumn sfx2" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="AveragePoints">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="LblAvgPoints" Text=""></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" CssClass="generalStatsColumn" />
                                    <HeaderStyle CssClass="generalStatsColumn sfx2" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ItemStyle-BackColor="Black" HeaderStyle-BackColor="Black"  >
                                    <ItemTemplate>
                                        
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" CssClass="dividerColumn" />
                                    <HeaderStyle CssClass="dividerColumn" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="PointsAsAttacker">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblAttackPoints" Text="" ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" CssClass="battleStatsColumn" />
                                    <HeaderStyle CssClass="battleStatsColumn sfx2"  />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="PointsAsDefender">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDefencePoints" Text=""></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" CssClass="battleStatsColumn" />
                                    <HeaderStyle CssClass="battleStatsColumn sfx2" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="GovKilledAsDefender">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblGivKilled" Text=""></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" CssClass="battleStatsColumn" />
                                    <HeaderStyle CssClass="battleStatsColumn sfx2" />
                                </asp:TemplateField>
                                
                            </Columns>
                            <RowStyle CssClass="DataRowNormal highlight rankingRow" />
                            <HeaderStyle CssClass="TableHeaderRow highlight " />
                            <AlternatingRowStyle CssClass="DataRowAlternate highlight rankingRow" />
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br /><br />
        <div style="color: gainsboro;text-align:right;">
    '<%=RS ("StatisticGenerate") %>'
        </div>
</div>       
