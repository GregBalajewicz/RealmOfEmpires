<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RankingMenu.ascx.cs" Inherits="Controls_RankingMenu" %>


<%if (IsMobile)
  { %>
<style>
</style>

<script>
    $(function () {
        $('a.SelectedMenuItem').parent("span").addClass('SelectedMenuItem');
    });
</script>
<%}%>
<div class="tempMenu sfx2">

<asp:Menu ID="mnuRankingMenu" runat="server" RenderingMode="Table" Orientation="Horizontal" StaticSubMenuIndent="10px">
    <DynamicMenuStyle BackColor="#E3EAEB" />
    <StaticSelectedStyle CssClass="SelectedMenuItem"/>
    <DynamicSelectedStyle CssClass="SelectedMenuItem"/>
    <StaticMenuItemStyle ItemSpacing="1px" />
    <Items>
        <asp:MenuItem  Value="Player Ranking" NavigateUrl="~/stats.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
        <asp:MenuItem  Value="Clan Ranking" NavigateUrl="~/clanRanking.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
        <asp:MenuItem  Value="Titles" NavigateUrl="~/TitlesRanking.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
        <asp:MenuItem  Value="Global Stats" NavigateUrl="~/globalstats.aspx"></asp:MenuItem>
    </Items>
</asp:Menu>
</div>