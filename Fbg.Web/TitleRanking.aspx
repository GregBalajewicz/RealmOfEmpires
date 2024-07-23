<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="TitleRanking.aspx.cs"
    Inherits="TitleRanking" %>

<%@ Register Assembly="Facebook.WebControls" Namespace="Facebook.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/RankingMenu.ascx" TagName="RankingMenuControl" TagPrefix="uc1" %>
<asp:Content ContentPlaceHolderID="HeadPlaceHolder" ID="h" runat="server">
    <style type="text/css">
        tr.selected
        {
            background-color: Maroon;
        }
    </style>




<%if (isMobile)
  { %>
<style>
.ranking showFacebookFriends
{
    display:table-cell;
    height:44px;
    width:44px;
}
.ranking .showFacebookFriends
, .ranking .notMob
{
    display:none;
}

.ranking .rankinglist td
, .ranking .rankinglist th
{
    padding-top:8px;
    padding-bottom:8px;
}


/*big pager*/
.ranking .ListPager {font-size:14pt;}
.ranking .ListPager table {width:100%; }
.ranking .ListPager a {width:100%; display:block}


.ranking .TextBox.jsplayers{width:100px;}
.ranking  .main {width:100%;}

.ranking .titleNav img 
{
    width:33px;
}
</style>

<script>


    page.load.push(function () {

        $('.rankinglist').delegate("tr", 'click',
        function (event) {
            if ($(event.target).parent("tr").length > 0 &&
            $($(event.target).parent("tr")[0]).find('a.playerName').length > 0) {
                window.location = $($(event.target).parent("tr")[0]).find('a.playerName').attr('href');
            }
        });

    }
    );
</script>
       <% } else if(isD2) { %>

    <style>

              html {
        background-color:#000000;
        background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg') no-repeat center center fixed; 
        -webkit-background-size: cover;
        -moz-background-size: cover;
        -o-background-size: cover;
        background-size: cover;
    }
    body { font-family: Georgia, 'Playfair Display'; font-size:12px; }
    a, a:active, a:link { color: #d3b16f; }   
    a:hover { color: #d3b16f; }
    .tempMenu {
        background-color: rgba(88, 140, 173, 0.3);
        border-bottom:2px solid  #9d9781;
        padding: 4px;
        padding-bottom: 2px;
    }
    .tempMenu a { text-shadow:1px 1px 1px #000; }
    .BoxContent {
       background-color: rgba(88, 140, 173, 0.3);
    }
          .TypicalTable .DataRowNormal {
       background-color: rgba(88, 140, 173, 0.1);
    }
    .TypicalTable .DataRowAlternate {
       background:none;
    }
    .TDContent {
        background-color: rgba(6, 20, 31, 0.9) !important;
        height: 100%;
        position: absolute;
        overflow: auto;
    }

    .TypicalTable TD {
        padding: 4px;
    }
    .Padding {
        padding: 4px;
    }
    .Sectionheader {
        color: #FFFFFF;
        font-weight: bold;
        padding: 4px;
        background-color: rgba(49, 81, 108, 0.7);
    }

     .TableHeaderRow {
        font-size: 12px !important;
        font-weight: normal;
        background-color: rgba(49, 81, 108, 0.7) !important;
        padding: 4px;
    }

        .TableHeaderRow th {
             padding: 4px 6px;
        }

        .TableHeaderRow th.dividerColumn {
             padding: 2px;
        }
    

        .playerRanking {
        padding-left: 12px;
        padding-right: 12px;
        padding-bottom: 12px;
        }

        tr.selected {
            background-color: #3D372A !important;
        }


        .inputbutton  {
            color: #FFFFFF;
            font-weight: initial;
            padding: 4px !important;
            padding-bottom: 3px !important;
            background-color: #181819 !important;
            -moz-box-shadow: inset 0 0 14px #154B86,  0px 1px 2px 0px #A69D85;
            -webkit-box-shadow: inset 0 0 14px #154B86,  0px 1px 2px 0px #A69D85;
            box-shadow: inset 0 0 14px #154B86,  0px 1px 2px 0px #A69D85;
            border:1px solid #A69D85;
            -moz-border-radius: 6px;
            -webkit-border-radius: 6px;
            border-radius: 6px;
            font-size: 12px !important;
            font-family: Georgia, 'Playfair Display';
            height:initial;
        }

        table.stripeTable {
            margin-top:8px;
        }

        /* TitleRanking specific */

        .BoxHeader {            
            font-size: 12px !important;           
            background-color: rgba(49, 81, 108, 0.7) !important;
            padding: 4px;
            color: #FFFFFF;
            font-weight: bold;
        }

        .filterControls {
             background-color: rgba(49, 81, 108, 0.7) !important;
        }

        .showFacebookFriends {
            padding:4px;
        }

        #ContentPlaceHolder1_PnlPlayerSearch {
            padding:4px;
            padding-left: 6px;
        }

        .ui-menu-item {
            color:#000;
            background-color: #CED5DA;
        }
    </style>
<%} else { %>

<%}%>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%=Utils.GetIframePopupHeaderForNotPopupBrowser("Title Ranking" , (isMobile && !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/m_ranking.png")%>
    <uc1:RankingMenuControl ID="RMenuControl" runat="Server" />
    <br />
<div class=ranking>
    <table cellpadding="0" cellspacing="0" border="0" class=main>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                    <tr class="filterControls">
                        <td>
                            <asp:Panel runat="Server" ID="PnlPlayerSearch" Width="100%">
                                <strong>
                                    <%= RS("showPlayer") %>&nbsp;</strong><asp:TextBox runat="server" ID="TxtPlayerSearch"
                                        Text="" CssClass="TextBox jsplayers"></asp:TextBox>&nbsp;
                                <asp:Button runat="Server" ID="BtnPlayerSearch" Text='<%# RS("show_btnText") %>'
                                    CssClass="inputbutton sfx2" OnClick="BtnPlayerSearch_Click" /><br />
                                <asp:Label runat="server" ID="LblPlayerError" Visible="False" CssClass="Error"></asp:Label></asp:Panel>
                        </td>
                        <% if (LoginModeHelper.isFB(Request))
                           { %>
                        <td valign="top" align="right" class=showFacebookFriends>
                            <asp:Button runat="Server" ID="BtnFBFriends" Text='<%# RS("showOnlyFrds_btn") %>' CssClass="inputbutton sfx2" OnClick="BtnFBFriends_Click" />
                        </td>
                        <% } %>
                    </tr>
                </table>
                <asp:Panel Visible="false" runat="Server" ID="panelInviteMoreFriends" Width="100%">
                    <table cellspacing="0" cellpadding="2" border="0" class="Box">
                        <tr>
                            <td style="padding: 4px;" class="TableHeaderRow" valign="middle" width="1%">
                                <asp:HyperLink  ID="HyperLink1" runat="server" NavigateUrl="~/invite.aspx" ImageUrl="https://static.realmofempires.com/images/navIcons/Invite1.gif"></asp:HyperLink>
                            </td>
                            <td>
                                <p>
                                    <%= RS("paragraph1")%>
                               </p>
                                <p>
                                    <string></string>
                                    <asp:HyperLink ID="HyperLink2" runat="server" Font-Bold="True" NavigateUrl="~/invite.aspx"><%= RS("inviteLink")%></asp:HyperLink>
                                    <%= RS("hint") %>
                                </p>
                                <p>
                                   <%= RS("enjoyGame") %>
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
                            <td >
                                <p>
                                  <%= RS("noFriends") %>
                                </p>
                                <p>
                                    <strong>
                                        <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/invite.aspx"><%= RS("inviteLink")%></asp:HyperLink></strong>
                                     <%= RS("hint") %>
                                </p>
                                <p>
                                    <%= RS("enjoyGame") %>
                                </p>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="BoxHeader">
                <asp:HyperLink  CssClass="titleNav sfx2" ID="linkPrevious" runat="server" ImageUrl="https://static.realmofempires.com/images/leftArrow.png"
                    Visible="False"></asp:HyperLink>
                <asp:HyperLink runat="Server" CssClass="LargerText sfx2" ID="linkTitle" />
                <asp:Label CssClass="LightText" runat="Server" ID="lblLevel" />
                <asp:HyperLink  CssClass="titleNav sfx2" ID="linkNext" runat="server" ImageUrl="https://static.realmofempires.com/images/RightArrow.png"
                    Visible="False"></asp:HyperLink><br />
                <asp:Label CssClass="LightText" runat="Server" ID="lblPoints" />
                <br />
                <asp:Label CssClass="LightText" Style="font-style: italic;" runat="Server" ID="lblDesc" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="GridView1" runat="server" GridLines="None" CssClass="TypicalTable stripeTable rankinglist"
                    CellPadding="1" CellSpacing="1" AutoGenerateColumns="False" AllowPaging="True"
                    EmptyDataText='<%# RS("noPlayer") %>' AllowSorting="True" PageSize="25" OnRowDataBound="GridView1_RowDataBound"
                    OnPageIndexChanging="GridView1_PageIndexChanging" Width=100%>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle HorizontalAlign="Center" CssClass="ListPager" />
                    <PagerSettings Mode="NumericFirstLast" FirstPageImageUrl="https://static.realmofempires.com/images/LeftArrow.png"
                        FirstPageText="First page" LastPageImageUrl="https://static.realmofempires.com/images/RightArrow.png"
                        LastPageText="Last page" />
                    <Columns>
                        <asp:TemplateField HeaderText="Player Name">
                            <ItemTemplate>
                                <asp:HyperLink runat="Server" ID="LnkPlayerName" class="playerName sfx2"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Clan">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" ID="LnkClan"></asp:HyperLink>
                            </ItemTemplate>
                            <ItemStyle CssClass="notMob" />
                            <HeaderStyle CssClass="notMob" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Overall Rank">
                            <ItemTemplate>
                                <asp:Label runat="Server" ID="LblRank"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rank Within Title">
                            <ItemTemplate>
                                <%#Container.DataItemIndex +1 %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="# Of Villages">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblNumbeOfVillages" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            <ItemStyle CssClass="notMob" />
                            <HeaderStyle CssClass="notMob" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total Points">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblTotalPoints" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Average Points">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblAvgPoints" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            <ItemStyle CssClass="notMob" />
                            <HeaderStyle CssClass="notMob" />
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="DataRowNormal highlight" />
                    <HeaderStyle CssClass="TableHeaderRow highlight" />
                    <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
    <br />
    <br />
    <div style="color: gainsboro; text-align: right;">
        <%= RSc("statFooterInfo") %>
    </div>
</div>
</asp:Content>
