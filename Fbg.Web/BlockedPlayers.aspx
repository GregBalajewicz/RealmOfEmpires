<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BlockedPlayers.aspx.cs" Inherits="BlockedPlayers"
    MasterPageFile="~/main.master" %>
<%@ Register Src="~/Controls/MailMenu.ascx" TagName="MailMenuControl" TagPrefix="uc1" %>
<asp:Content ID="header" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link type="text/json" rel="help" href="static/help/e_TransportSilver.json.aspx" />
    <link type="text/json" rel="help" href="static/help/q_BattleSimulator.json.aspx" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%=Utils.GetIframePopupHeaderForNotPopupBrowser("Blocked players" , (isMobile),"https://static.realmofempires.com/images/icons/M_Mail.png")%>
<uc1:MailMenuControl runat="server" id="MailMenu1" cssClass="sfx2"></uc1:MailMenuControl><br />

    <%if (isMobile) {%>
    <style>
    #blockedPlayersContainer {font-size:11pt;}
    #blockedPlayersContainer .TypicalTable a{font-size:12pt;}
    </style>
    <%}%>
<span id=blockedPlayersContainer>
 <%=RS("SeePlayersBlocked") %>
    <br />
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <asp:GridView ID="gv_BlockedPlayers" runat="server" AutoGenerateColumns="False" ShowHeader="false"
        GridLines="None" CellSpacing="1" CssClass="TypicalTable" AllowPaging="True" PageSize="30"
        DataKeyNames="PlayerID" OnPageIndexChanging="gv_BlockedPlayers_OnPageIndexChanging" OnRowCommand="gv_BlockedPlayers_RowCommand" EmptyDataText='<%#RS("NoOneBlocked") %>'>
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton CommandArgument='<%# Eval("PlayerID")%>' ID="btn_UnBlock" runat="server" CssClass="sfx2" CommandName="UnBlock"><%=RS("Unblock") %></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Name" HeaderText='<%=RS("PlayerName") %>' />
        </Columns>
        <RowStyle CssClass="DataRowNormal" />
        <HeaderStyle CssClass="TableHeaderRow" />
        <AlternatingRowStyle CssClass="DataRowAlternate" />
    </asp:GridView>
    </ContentTemplate>
    </asp:UpdatePanel>

    </span>
</asp:Content>
