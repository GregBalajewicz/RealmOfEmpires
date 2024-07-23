<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MailMenu.ascx.cs" Inherits="Controls_MailMenu" %>
<style>
a.folderPreDef 
{
	background : url('https://static.realmofempires.com/images/ThreadHot_sml.gif') 2px center no-repeat; 
	text-decoration : none; 
	padding : 2px 4px 2px 16px; 
	font-weight:bold;
}
 a.folderPreDef:hover 
{ 
	text-decoration : underline; 
}
a.folder 
{
	background : url('https://static.realmofempires.com/images/folder_sml.gif') 2px center no-repeat; 
	text-decoration : none; 
	padding : 2px 4px 2px 16px; 
}
 a.folder:hover 
{ 
	text-decoration : underline; 
}

</style>


<%if (_isMobile)
  { %>
<style>



.mailMenu { 
    width:100%; 
    overflow:auto;
    border-spacing: 2px;
    -webkit-border-vertical-spacing: 0px;
}
.mailMenu a
{
    display:table-cell;
    height:44px;
    width:44px;
    min-width:44px;
    padding:5px;
    border: 2px solid #846824;
    
    -webkit-border-radius: 15px;
            border-radius: 15px;
    vertical-align:middle;
}
.mailMenu a:hover 
{
    text-decoration:none;
}

.mailMenu a.SelectedMenuItem 
{
    border: 2px solid rgb(226, 185, 84);
    background-color: rgb(170, 130, 31);
}

.mailMenu a.SelectedMenuItem 
{
    color:Black;
}

.mailMenu img
{
    display:none;
}

</style>

<%}%>
<div class="mailMenu sfx2">
<%if (_isMobile)
  { %>
  <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/messagespopup.aspx" ><%=RS("Inbox")%></asp:HyperLink>
  <%}%>
<div runat=server id=folders class="jsFakeSelect">
    <asp:HyperLink runat="server" ID="linkSelectedFolder" CssClass="jsMaster jsTriger" Style="background: url('https://static.realmofempires.com/images/downarrow.png') top right no-repeat; padding: 0px 15px 0px 0px;" />
    <asp:Panel ID="panelFolders" runat="server" class="jsOptions ui_menu" Style="border: 1px solid rgb(30,30,30); background-color: rgb(75, 61, 48);">
    </asp:Panel>
</div><img class="sep" src='<%=RSic("menuItemSeperator")%>' /><asp:HyperLink ID="linkCreateMail" runat="server" NavigateUrl="~/messages_create.aspx"><%=RS("CreateMail") %></asp:HyperLink><img class=sep src='<%=RSic("menuItemSeperator")%>' /><asp:HyperLink ID="linkBlockedPlayers" runat="server" NavigateUrl="~/BlockedPlayers.aspx"><%=RS("BlockedPlayers") %></asp:HyperLink>
</div>
