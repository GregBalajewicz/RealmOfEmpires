<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ClanMenu.ascx.cs" Inherits="Controls_ClanMenu" %>
<style>

</style>


<%if (IsMobile)
  { %>
<style>
   
</style>

<script>
    $(function () {
        $('a.SelectedMenuItem').parent("span").addClass('SelectedMenuItem');
    });
</script>
<%} else { %>
<%}%>
<div class="tempMenu sfx2 clanMenuWrapper">
 <asp:Menu ID="mnu_MyClan_m" runat="server" Orientation="Horizontal" StaticSubMenuIndent="10px" RenderingMode="Table">
 <StaticMenuItemStyle ItemSpacing="1px" />        
        <DynamicMenuStyle BackColor="#E3EAEB" />
        <StaticSelectedStyle CssClass="SelectedMenuItem"/>
        <DynamicSelectedStyle CssClass="SelectedMenuItem"/>
        <Items>
            <asp:MenuItem Value="Overview" NavigateUrl="~/ClanOverview.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Overview" NavigateUrl="~/ClanPublicProfile.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Invitations" NavigateUrl="~/ClanInvitations.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Members" NavigateUrl="~/ClanMembers.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Diplomacy" NavigateUrl="~/ClanDiplomacy.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
        </Items>
    </asp:Menu>
<asp:Menu ID="mnu_MyClan" runat="server" Orientation="Horizontal" StaticSubMenuIndent="10px" RenderingMode="Table">
 <StaticMenuItemStyle ItemSpacing="1px" />        
        <DynamicMenuStyle BackColor="#E3EAEB" />
        <StaticSelectedStyle CssClass="SelectedMenuItem"/>
        <DynamicSelectedStyle CssClass="SelectedMenuItem"/>
        <Items>
            <asp:MenuItem Value="Overview" NavigateUrl="~/ClanOverview.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Overview" NavigateUrl="~/ClanPublicProfile.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Invitations" NavigateUrl="~/ClanInvitations.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Members" NavigateUrl="~/ClanMembers.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Forum" NavigateUrl="~/ClanForum.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Diplomacy" NavigateUrl="~/ClanDiplomacy.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Events" NavigateUrl="~/ClanEvents.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png" ></asp:MenuItem>
            <asp:MenuItem Value="Claims" NavigateUrl="~/ClanClaims.aspx" ></asp:MenuItem>
        </Items>
    </asp:Menu>
<asp:Menu ID="mnu_MyClanAdmin" runat="server" Orientation="Horizontal" Visible=false StaticSubMenuIndent="10px" RenderingMode="Table">
 <StaticMenuItemStyle  ItemSpacing="1px" />        
        <StaticSelectedStyle CssClass="SelectedMenuItem"/>

        <DynamicMenuItemStyle ForeColor="#C39037" />
        <DynamicMenuStyle HorizontalPadding="2px" VerticalPadding="2px" BackColor="#4B3D32" BorderColor="Black"
            BorderStyle="Solid" BorderWidth="1px" />
        
        <Items>
            <asp:MenuItem Value="Overview" NavigateUrl="~/ClanOverview.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Overview" NavigateUrl="~/ClanPublicProfile.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Invitations" NavigateUrl="~/ClanInvitations.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Members" NavigateUrl="~/ClanMembers.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Forum" NavigateUrl="~/ClanForum.aspx" />
            <asp:MenuItem Value="Forum Admin" NavigateUrl="~/ManageForums.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"> </asp:MenuItem >
            <asp:MenuItem Value="Diplomacy" NavigateUrl="~/ClanDiplomacy.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Events" NavigateUrl="~/ClanEvents.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Settings" NavigateUrl="~/ClanSettings.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Email Members" NavigateUrl="~/ClanEmailMembers.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png" ></asp:MenuItem>
            <asp:MenuItem Value="Claims" NavigateUrl="~/ClanClaims.aspx"  ></asp:MenuItem>
            
        </Items>
    </asp:Menu>
<asp:Menu ID="mnu_MyClanAdmin_m" runat="server" Orientation="Horizontal" Visible=false StaticSubMenuIndent="10px" RenderingMode="Table">
 <StaticMenuItemStyle  ItemSpacing="1px" />        
        <StaticSelectedStyle CssClass="SelectedMenuItem"/>

        <DynamicMenuItemStyle ForeColor="#C39037" />
        <DynamicMenuStyle HorizontalPadding="2px" VerticalPadding="2px" BackColor="#4B3D32" BorderColor="Black"
            BorderStyle="Solid" BorderWidth="1px" />
        
        <Items>
            <asp:MenuItem Value="Overview" NavigateUrl="~/ClanOverview.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Overview" NavigateUrl="~/ClanPublicProfile.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Invitations" NavigateUrl="~/ClanInvitations.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Members" NavigateUrl="~/ClanMembers.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Diplomacy" NavigateUrl="~/ClanDiplomacy.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
            <asp:MenuItem Value="Settings" NavigateUrl="~/ClanSettings.aspx" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png"></asp:MenuItem>
        </Items>
    </asp:Menu>
    <asp:Menu ID="mnu_OtherClan" runat="server" Orientation="Horizontal" StaticSubMenuIndent="10px" RenderingMode="Table">
     <StaticMenuItemStyle ItemSpacing="1px" />        
        <DynamicMenuStyle BackColor="#E3EAEB" />
        <StaticSelectedStyle CssClass="SelectedMenuItem"/>
        <DynamicSelectedStyle CssClass="SelectedMenuItem"/>
        <Items>
           
            <asp:MenuItem Value="Overview" SeparatorImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png">
            </asp:MenuItem>
            <asp:MenuItem Value="Members" ></asp:MenuItem>
            
        </Items>
    </asp:Menu>
     <asp:Menu ID="mnu_NoClan" runat="server" Orientation="Horizontal" Visible=false StaticSubMenuIndent="10px" RenderingMode="Table">
      <StaticMenuItemStyle ItemSpacing="1px" />        
        <DynamicMenuStyle BackColor="#E3EAEB" />
        <StaticSelectedStyle CssClass="SelectedMenuItem"/>
        <DynamicSelectedStyle CssClass="SelectedMenuItem"/>
        <Items>
           
            <asp:MenuItem Value="Overview"  NavigateUrl="~/ClanOverview.aspx">
            </asp:MenuItem>
            
        </Items>
    </asp:Menu>

    

</div>