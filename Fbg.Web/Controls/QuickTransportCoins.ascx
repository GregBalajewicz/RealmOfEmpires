<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuickTransportCoins.ascx.cs" Inherits="QuickTransportCoins" %>
<%if (IsMobile) {%>
        <style>
        .config 
        {
            display:none;
        }
        .clickableSize 
        , .NearestVillages td
        {
            margin-bottom:10px;
            margin-top:10px;
            height:30px;
        }
        .GetMax {font-size:10pt;}
        </style>
    <%} else if(IsD2) { %>
    <style>
     html {
            background-color: #000000;
            background: url('https://static.realmofempires.com/images/misc/M_BG_Treasury.jpg') no-repeat center center fixed;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
        }

        body {

            font-family: Georgia, 'Playfair Display';
            font-size:12px !important;
            color:#F7F7F7;
        }
         .TDContent {
            background-color: rgba(6, 20, 31, 0.7) !important;
            height: 100%;
            position: absolute;
            overflow: auto;
            color:#fff;
        }

        .d2_container {
            padding:10px;
        }

        .d2_transportIcon {
            text-align:left;
            margin-left: 10px;
            margin-right: 10px;
        }

        .d2_headertable {
            margin-top:10px;
        }

        .NearestVillages {
           margin-top: 10px;
        }

    </style>

    <%} else { %>
    
    <%} %>
<div class="genericMenuSilver" width="100%">
    <table width="100%" class="d2_headertable">
        <tr>
            <td width="1%">
                <% if(IsD2) { %>
                <img class="d2_transportIcon" src="https://static.realmofempires.com/images/vov/d_trade3.png" width="80"/>
                <% } else { %>
                <asp:Image ID="Image1" runat="server" ImageUrl="https://static.realmofempires.com/images/tempBuilding.gif" ImageAlign="Left" Width="100px" />
                <%} %>
            </td>
            <td valign="top">
                <b><%=RS("ConvenientSilverTransport") %></b>
                <asp:HyperLink Visible="False" ID="linkLocked" runat="server" ImageUrl="https://static.realmofempires.com/images/LockedFeature.png" ToolTip="Unlock this feature now">HyperLink</asp:HyperLink>
                <br />
                <%=RS("EasilyTransportSilver") %>
                <asp:Label ID="lbl_Message" runat="server" ForeColor="#00CC00"></asp:Label>
            </td>
        </tr>
    </table>
    <asp:Panel runat="server" Font-Size="Medium" ID="panelNoPF" Visible="true" class="padding:3px;">
        <asp:HyperLink ID="HyperLink2" runat="server"><%=RS("UnlockPremiumFeature") %></asp:HyperLink><%=RS("VillagesCapableOfSilverTransport") %>
        <asp:HyperLink ID="LinkButton2" runat="server"><%=RS("UnlockThisFeature") %></asp:HyperLink>
        <%=RS("FreeTrialAvailableQuestion") %>
    </asp:Panel>
    <asp:Panel runat="server" ID="panelHasPF" Visible="false" CssClass="d2_container">
        <asp:Panel ID="pnl_TransportList" runat="server" Width="100%">
                    <%=RS("TransportsAvailableTo") %> 
                    <asp:Label ID="lblCurVillage" runat="server" Font-Bold="True"></asp:Label><% if(IsD2) { %>:<%} %><br />
                    <asp:DataList ID="dl_NearestVillages" runat="server" OnItemCommand="dl_NearestVillages_ItemCommand" CellPadding="0" CellSpacing="0" CssClass="NearestVillages">
                        <ItemTemplate>
                            <%if (playerHasPF)
                              { %>
                            <asp:LinkButton CssClass="mi" ID="LinkButton1" CausesValidation="false" runat="server" CommandArgument='<%#Eval("VillageID") %>' CommandName='<%#BindAmount(Container.DataItem) %>'><%#BindVillages(Container.DataItem) %></asp:LinkButton>
                            <% }
                              else
                              { %>
                            <asp:HyperLink CssClass="mi" NavigateUrl="<%#NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport)%>" ID="LinkButton2" runat="server"><%#BindVillages(Container.DataItem) %></asp:HyperLink>
                            <%} %>
                            <asp:Label ID="lbl_MinAmount" runat="server" Text='<%#BindAmount(Container.DataItem) %>' Visible="false"></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="false" CssClass="QuickSilverTransportTbl" />
                    </asp:DataList>
                    <br />
                    <asp:LinkButton CssClass="mi GetMax" ToolTip='<%#RS("InitiatesAsManyCanAccomodate") %>' ID="lnk_GetMax"  runat="server" OnClick="lnk_GetMax_Click" CausesValidation="false"></asp:LinkButton>
                    <asp:HyperLink Visible="false" CssClass="mi" Target="_blank" ID="linkGetMax_NoPF" runat="server" OnClick="return popupUnlock(this);" />
                    <br />
                    <asp:HyperLink ID="HyperLink1" runat="server" CssClass="config" onclick="return loadInParent(this);" NavigateUrl="~/NoQTransportVillages.aspx"><%=RS("ConfigureElipses") %></asp:HyperLink>
                    <br />
        </asp:Panel>
        <asp:Panel ID="pnl_NoTransports" runat="server" Width="100%" Font-Size="13pt">
            <%=RS("NoMoreTransports") %>
        </asp:Panel> 
        <asp:Panel ID="panelShowVillagesWith1000Silver" runat="server" CssClass="clickableSize">
            <%=RS("NotShowingWithLessThanThousand") %> <asp:LinkButton ID="linkNoLimitTo1000Silver" runat="server" OnClick="linkNoLimitTo1000Silver_Click"><%=RS("ShowAll") %></asp:LinkButton>
        </asp:Panel>
        <asp:Panel ID="panelShow20VillsOnly" runat="server" CssClass="clickableSize">
            <%=RS("ShowingUpTo20Villages") %> <asp:LinkButton ID="linkNoLimitTo20Vills" runat="server" OnClick="linkNoLimitTo20Vills_Click"><%=RS("ShowAll") %></asp:LinkButton>
        </asp:Panel>
    </asp:Panel>
</div>
