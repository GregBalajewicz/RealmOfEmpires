<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TroopsMenu.ascx.cs" Inherits="Controls_TroopsMenu" %>
<asp:HyperLink ID="linkCommand" runat="server"  NavigateUrl="~/CommandTroops.aspx"><%=RS("Command") %></asp:HyperLink><img
    src= "<%=RSic("menuItemSeperator") %>" />
<asp:HyperLink ID="linkRecruit" runat="server" NavigateUrl="~/VillageUnitRecruit.aspx"><%=RS("Recruit") %></asp:HyperLink>
<img src="<%=RSic("menuItemSeperator") %>" /><asp:HyperLink ID="linkIncoming" runat="server"
    NavigateUrl="~/TroopMovementsIn.aspx"><%=RS("IncomingTroops") %></asp:HyperLink><img src="<%=RSic("menuItemSeperator") %>" /><asp:HyperLink
        ID="linkOutgoing" runat="server" NavigateUrl="~/TroopMovementsOut.aspx" ><%=RS("OutgoingTroops") %></asp:HyperLink><img
            src="<%=RSic("menuItemSeperator") %>" /><asp:HyperLink ID="linkAbroad" runat="server"
                NavigateUrl="~/UnitsAbroad.aspx?PS=1" CssClass="jsTAMenu"><%=RS("TroopsAbroad") %></asp:HyperLink><img src="<%=RSic("menuItemSeperator") %>" /><asp:HyperLink
                    ID="linkSupport" runat="server" NavigateUrl="~/UnitsSupporting.aspx" CssClass="jsSupMenu"><%=RS("Support") %></asp:HyperLink><img
                        src="<%=RSic("menuItemSeperator") %>" /><asp:HyperLink ID="linkSim" runat="server"
                            NavigateUrl="~/BattleSimulator.aspx"><%=RS("BattleSimulator") %></asp:HyperLink><br /><br />