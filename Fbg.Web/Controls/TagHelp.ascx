<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TagHelp.ascx.cs" Inherits="Controls_TagHelp" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <span style="font-size: 1.1em"><%= RS("tagsAndFiltersAllow")%>
            <br />
            <asp:LinkButton ID="linkHelpDetails" runat="server" OnClick="LinkButton2_Click"></asp:LinkButton><br />
            <asp:Panel ID="panelHelpDetails" runat="server" Visible="False">
                <br />
                <%= RS("infoParagraph") %>
                <hr size="1" noshade />
            </asp:Panel>
        </span>
        <br />
    </ContentTemplate>
</asp:UpdatePanel>
