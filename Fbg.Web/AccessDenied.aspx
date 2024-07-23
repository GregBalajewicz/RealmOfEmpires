<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="AccessDenied.aspx.cs"
    Inherits="AccessDenied" %>
<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <uc1:ClanMenu ID="ClanMenu1" runat="server" />
  <asp:Label id="lbl_Error" runat="server" Text='<%#RS("YouAreNotAMemberOfClan") %>'></asp:Label>
    
    
    
</asp:Content>
