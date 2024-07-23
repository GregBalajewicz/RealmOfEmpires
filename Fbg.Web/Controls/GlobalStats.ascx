<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GlobalStats.ascx.cs" Inherits="Controls_GlobalStats" %>

<%if (IsMobile)
  { %>

<style>
   .stats .twoCol {width:100%; }
   .stats .column {display:inline;width:100%}
        
</style>
<%} else { %>
<style>
    .stats .twoCol {width:100%; display:table;}
    .stats .column {display:table-cell;width:50%; vertical-align:top;}

</style>
<%} %>

<div class=stats>
<table cellpadding="0" cellspacing="0">
    <tr class="Sectionheader">
        <td colspan="2">
            <%=RS("GlobalStats") %>
        </td>
    </tr>
    <tr>
        <td valign="top" class=twoCol>
            <div class="column">
                <table class='TypicalTable stripeTable' gridlines='Both' cellspacing="1">
                    <tr class='DataRowNormal highlight'>
                        <td>
                            <%=RS("NumberOfPlayers") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblPlayerNo"></asp:Label>
                        </td>
                    </tr>
                    <tr class='DataRowAlternate hightlight'>
                        <td>
                            <%=RS("NumberOfVillages") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblVillagesNo"></asp:Label>
                        </td>
                    </tr>
                    <tr class='DataRowNormal highlight'>
                        <td>
                            <%=RS("NumberOfVillagesPerPlayer") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblVillagesPlayer"></asp:Label>
                        </td>
                    </tr>
                    <tr class='DataRowAlternate hightlight'>
                        <td>
                            <%=RS("NumberOfClans") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblClanNo"></asp:Label>
                        </td>
                    </tr>
                    <tr class='DataRowNormal highlight'>
                        <td>
                            <%=RS("NumberOfPlayersPerClan") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblPlayersClan"></asp:Label>
                        </td>
                    </tr>
                    <tr class='DataRowAlternate hightlight'>
                        <td>
                            <%=RS("TotalSilver") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblTSilver"></asp:Label>
                        </td>
                    </tr>
                    <tr class='DataRowNormal highlight'>
                        <td>
                            <%=RS("TotalSilverPerPlayer") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblSilverPlayer"></asp:Label>
                        </td>
                    </tr>
                    <tr class='DataRowAlternate hightlight'>
                        <td>
                            <%=RS("TotalSilverProductionPerHour") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblSilverProdHour"></asp:Label>
                        </td>
                    </tr>
                    <tr class='DataRowNormal highlight'>
                        <td>
                            <%=RS("TotalSilverProductionPerHourPerPlayer") %>
                        </td>
                        <td align="right">
                            <asp:Label runat="server" ID="LblSilverProdHourPlayer"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="column">
                <asp:Table runat="Server" ID="TblTTroops" CellSpacing="1" CellPadding="3" Width="100%" CssClass='TypicalTable stripeTable'>
                </asp:Table>
                <asp:Table runat="Server" ID="TblTPlayer" CellSpacing="1" CellPadding="3" Width="100%" CssClass='TypicalTable stripeTable'>
                </asp:Table>
            </div>
        </td>
    </tr>
</table>
</div>