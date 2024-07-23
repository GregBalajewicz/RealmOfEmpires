<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="ClanRanking.aspx.cs" Inherits="ClanRanking" %>

<%@ Register Assembly="Facebook.WebControls" Namespace="Facebook.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/RankingMenu.ascx" TagName="RankingMenuControl" TagPrefix="uc1" %>
<asp:Content ContentPlaceHolderID="HeadPlaceHolder" ID="h" runat="server">
    <style type="text/css">
        tr.selected
        {
            background-color: Maroon;
        }
    </style>
    <%if (false ) {%><script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4.js" type="text/javascript"></script><%} %>
    <%if (false ) {%><script src="../script/bda-ui-checkbox.js" type="text/javascript"></script><%} %>
    <%if (false ) {%><script src="../script/bda-ui-radio.js" type="text/javascript"></script><%} %>



    
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
, .ranking .title
, .ranking .areaHelp
, .ranking .dividerColumn 
, .ranking .clanNameContainer
{
    display:none;
}

.ranking .rankinglist td
, .ranking .rankinglist th
{
    padding-top:8px;
    padding-bottom:8px;
}

.ranking .area
, .ranking .searchForPlayer
, .ranking .mobOptions
{
    padding-top:8px;
    padding-bottom:8px;
}

.ranking .battleStatsColumn 
{
    display:none;
}

/*big pager*/
.ranking .ListPager {font-size:14pt;}
.ranking .ListPager table {width:100%; }
.ranking .ListPager a {width:100%; display:block}


.playerNameContainer .clanName { display:block; margin: 8px 0px 8px 0px;font-style:italic; color:#D3D3D3;}

.ranking .TextBox.jsclans{width:100px;}
.ranking  .main {width:100%;}
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
        var radios = BDA.UI.Radio.init($('#statType'), playerRanking_radioClicked, ['<%=RS("General")%>', '<%=RS("BattleStats")%>'], $.cookie("r") === "b" ? 1 : 0);
        
        if ($.cookie("r") == "b") {
            playerRanking_clickb();
        } else if ($.cookie("r") == "b") {
            playerRanking_clickg();
        }

        if ($.cookie("cn") === "1") {
            playerRanking_showClanChecked();
        }


        $('a.clanName').click(
        function (event) {

            event.preventDefault();
            /*
            var targetLink = $(event.currentTarget).attr("href").split("=");
            var clanID = targetLink[1];

            //parent.closeModalIFrame('Ranking', true);

            parent.ROE.Frame.popupClan(clanID);
           
            
            if ($(event.target).parent("tr").length > 0 &&
            $($(event.target).parent("tr")[0]).find('a.clanName').length > 0) {
                window.location = $($(event.target).parent("tr")[0]).find('a.clanName').attr('href');
            }
            */
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
        padding: 2px;
    }
    .Padding {
        padding: 2px;
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
             padding: 0px;
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

                .ui-menu-item {
            color:#000;
            background-color: #CED5DA;
        }

        .areaHelpInlineImage {
           width: 307px;
            height: 267px;
            position: absolute;
            top: 10px;
            right: 10px;
            box-shadow: 0px 5px 10px 2px #000;
        }

        .areaHelpOn img.areaHelpInlineImage {
            display:block;
        }

        .areaHelpOff img.areaHelpInlineImage {
            display:none;
        }

        .ranking .areaHelp {
        display: inline-block;
        float: right;
        }

        td.generalStatsColumn, td.battleStatsColumn {
            font-family: 'Droid Sans Mono', monospace;
            text-shadow: 1px 1px 0 #000,1px 1px 0 #000;
        }

    </style>
<%} else { %>
<style>
.ranking .areaHelp
{ 
    display: inline-block;float:right
}
</style>
<%}%>



</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%/*=Utils.GetIframePopupHeaderForNotPopupBrowser(RS("ClanRanking") , (isMobile && !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/m_ranking.png")*/%>
    <uc1:RankingMenuControl ID="RMenuControl" runat="Server" />
    <br />
<div class=ranking>
    <table cellpadding="1" cellspacing="1" class="TypicalTable main" border="0">
        <tr class="TableHeaderRow">
            <td class=area>
                <asp:Label ID="lblCurArea" runat="server" Text=""></asp:Label>
                <div class="jaxHide" rel='<%=RS ("Change") %>'  style="display:none">
                    <strong><%=RS ("ShowAreaNumber") %></strong><asp:TextBox runat="server" ID="txtArea" Text="" Width="25" CssClass="TextBox"></asp:TextBox>&nbsp;
                    <asp:Button runat="Server" ID="btnChangeArea" Text='<%#RS ("ShowArea") %>' CssClass="inputbutton sfx2" CommandName="one" OnCommand="btnChangeArea_Command" />
                    <asp:Button runat="Server" ID="btnShowAllAreas" Text='<%#RS ("ShowAllClans") %>' CssClass="inputbutton sfx2" CommandName="all" OnCommand="btnChangeArea_Command" /><br />
                    <asp:Label runat="server" ID="Label1" Visible="False" CssClass="Error"></asp:Label>
                </div>
                    <a href="https://static.realmofempires.com/images/misc/areas.jpg?615,534" class=areaHelp target="_blank"><img border="0" src="https://static.realmofempires.com/images/HelpIcon.png" /></a>
            </td>
        </tr>
        <tr class="TableHeaderRow ">
            <td class=searchForPlayer>
                <strong><%=RS ("ShowClan") %>&nbsp;</strong>&nbsp;<asp:TextBox runat="server" ID="TxtClanSearch" Text="" CssClass="TextBox jsclans"></asp:TextBox>&nbsp;
                <asp:Button runat="Server" ID="BtnClanSearch" Text='<%#RS ("Show") %>' OnClick="BtnClanSearch_Click" CssClass="inputbutton sfx2" />
                <br />
                <asp:Label ID="LblClanError" runat="server" CssClass="Error" Visible="False"></asp:Label>
            </td>
        </tr>
        <%if (isMobile) { %>
        <tr class="TableHeaderRow">
            <td class="mobOptions">
                <span id=statType></span>
            </td>
        </tr>
        <%} %>
        <tr>
            <td style="padding:0px;">
                <asp:GridView ID="gvClanRanking" runat="server" GridLines="None" CssClass="TypicalTable stripeTable rankinglist" CellPadding="1" CellSpacing="1" AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" PageSize="25" OnRowDataBound="gvClanRanking_RowDataBound" OnPageIndexChanging="gvClanRanking_PageIndexChanging" EmptyDataText="No record found" Width=100%>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle HorizontalAlign="Center" CssClass="ListPager" />
                    <PagerSettings Mode="NumericFirstLast" FirstPageImageUrl="https://static.realmofempires.com/images/LeftArrow.png" FirstPageText='<%=RS ("FirstPage") %>' LastPageImageUrl="https://static.realmofempires.com/images/RightArrow.png" LastPageText='<%=RS ("LastPage") %>' />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="Server" ID="lblClanRank"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink runat="Server" ID="linkClanName" CssClass="clanName sfx2"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblNumbeOfPlayer" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="generalStatsColumn" />
                            <HeaderStyle CssClass="generalStatsColumn" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblNumbeOfVillages" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="generalStatsColumn" />
                            <HeaderStyle CssClass="generalStatsColumn" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblTotalPoints" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="generalStatsColumn" />
                            <HeaderStyle CssClass="generalStatsColumn" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblAttackPoints" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="battleStatsColumn" />
                            <HeaderStyle CssClass="battleStatsColumn" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblDefencePoints" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="battleStatsColumn" />
                            <HeaderStyle CssClass="battleStatsColumn" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblAttackPercent" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="battleStatsColumn" />
                            <HeaderStyle CssClass="battleStatsColumn" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblDefencePercent" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="battleStatsColumn" />
                            <HeaderStyle CssClass="battleStatsColumn" />
                        </asp:TemplateField>
                        
                         <asp:TemplateField Visible="true">
                             
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LblInvite" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" CssClass="generalStatsColumn" />
                            <HeaderStyle CssClass="generalStatsColumn" />
                          
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
    <div style="color: gainsboro; text-align: right;">
        <%=RS ("StatisticsGenerate") %>
    </div>
</div>
        <% if(isD2) { %>
    <script type="text/javascript">

        var areaHelp = $('.areaHelp');
        areaHelp.addClass('areaHelpOff');
        var href = areaHelp.attr('href');
        areaHelp.append($("<img class='areaHelpInlineImage' src='" + href + "' />"));

        $('.areaHelp').click(function (e) {
            e.preventDefault();
           
            var a = $(e.currentTarget);
            if (a.hasClass('areaHelpOff')) {
                a.removeClass('areaHelpOff').addClass('areaHelpOn');
            } else {
                a.removeClass('areaHelpOn').addClass('areaHelpOff');
            }

        });

    </script>
     <% } %> 
</asp:Content>
