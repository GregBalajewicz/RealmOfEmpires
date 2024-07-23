<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BlockPerson.ascx.cs" Inherits="Controls_BlockPerson" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode=Conditional>
<ContentTemplate>
<asp:Panel ID="pnl_BlockPlayer" runat="server" Visible="false">
    <asp:LinkButton ID="lnk_BlockPlayerTop" OnClientClick="if (confirm('Blocking the sender will prevent this person from sending you any messages or forwarding any reports. Are you sure you want to block this person?') == false) return false;"
        runat="server" OnClick="lnk_BlockPlayer_Click"><%=RS("BracketsBlockPlayer") %></asp:LinkButton>
    <asp:Label ID="lbl_Error" runat="server" CssClass="Error" Visible="false"></asp:Label>
    <asp:Label ID="lblConfirm" runat="server" CssClass="ConfirmationMsg" Font-Bold="True"
        Text='<%#RS("SenderHasBeenBlocked") %>' Visible="False"></asp:Label>
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
