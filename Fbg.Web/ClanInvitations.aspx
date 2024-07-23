<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanInvitations.aspx.cs"
    Inherits="ClanInvitations" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="header" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link type="text/json" rel="help" href="static/help/u_ClanInvites.json.aspx" />
    <%if (isMobile) { %>
    <style>
        
        .clanPage .action
        {
            font-size: 12pt;
            margin: 8px 0px 8px 0px;
        }
        .clanPage .actions a
        {
            font-size: 12pt;
            margin: 8px 0px 8px 0px;
        }
        
        .clanPage, .clanPage .clanProfile
        {
            width: 100%;
        }
        .clanPage .column
        {
            display: inline;
            width: 100%;
        }
        .clanPage .pageSize
        {
            padding:10px;
        }

        .clanPage .inviteList td
        {
            padding-top:10px;
            padding-bottom:10px;
        }

            .clanPage .notOnMob
            {
                display:none;
            }
    </style>
    <script>
   
    </script>
     <%} else if(isD2) { %>
<style>
    /* Clan Overview */
    html,body {
        background:none !important;
    }
    body { font-family: Georgia, 'Playfair Display'; font-size:12px; }
    a, a:active, a:link { color: #d3b16f; }   
    a:hover { color: #d3b16f; }
    .tempMenu {
        background-color: rgba(88, 140, 173, 0.3);
        border-bottom:2px solid  #9d9781;
        padding: 4px;
        padding-bottom: 2px;
    }
    .tempMenu a { text-shadow:1px 1px 1px #000; }
    .BoxContent {
       background-color: rgba(88, 140, 173, 0.3);
    }
     .TypicalTable .DataRowNormal {
       background-color: rgba(88, 140, 173, 0.3);
    }
    .TypicalTable .DataRowAlternate {
       background-color: rgba(88, 140, 173, 0.2);
    }
    .TDContent {
        background-color: rgba(0, 0, 0, 0.7) !important;
        height: 100%;
        position: absolute;
        overflow: auto;
    }
     .clanPage {
        padding-left: 12px;
        padding-right: 12px;
        padding-bottom: 12px;
    }
    
    .TypicalTable TD {
        padding: 4px;
    }
    .Padding {
        padding: 4px;
    }
    .Sectionheader {
        color: #FFFFFF;
        font-weight: bold;
        padding: 4px;
        background-color: rgba(49, 81, 108, 0.7);
    }

    /* on this page, th is inside tableheaderrow which 
        messes with the styles so we have to manual
        style the th's instead */
    .TableHeaderRow th {
        font-size: 12px !important;
        font-weight: bold;
        background-color: rgba(49, 81, 108, 0.7) !important;
        padding: 4px;
    }
    .column {
        margin-bottom:12px;
    }

    .TextBox {
        /* so text area fits within width */
        -webkit-box-sizing: border-box;
        -moz-box-sizing: border-box;
        box-sizing: border-box;
        background-color: rgba(255,255,255,0.8);
        font-weight:normal;
        font-size: 12px;
        font-family: Georgia, 'Playfair Display';
        padding: 4px;
    }

    
    .inputbutton, .inputsubmit {
        font-weight:bold;
        font-size: 12px;
        font-family: Georgia, 'Playfair Display';
        margin-left: 0px;
        margin-top: 4px;
        padding: 4px 6px;
        color: #d3b16f;
        border-color: #efe1b9;
        background-color:rgba(25, 55, 74, 0.7);
        -moz-box-shadow: inset 0 0 5px 1px #000000;
        -webkit-box-shadow:  inset 0 0 5px 1px #000000;
        box-shadow:  inset 0 0 5px 1px #000000;
        height:initial;
    }

        .inputbutton:hover, .inputsubmit:hover {
        -moz-box-shadow: inset 0 0 5px 1px #efe1b9;
        -webkit-box-shadow: inset 0 0 5px 1px #efe1b9;
        box-shadow: inset 0 0 5px 1px #efe1b9;
    }



    /* Clan Public Profile */
    td.TableHeaderRow {
        font-size: 12px !important;
        font-weight: bold;
        background-color: rgba(49, 81, 108, 0.7) !important;
        padding: 4px;
    }

      div.TableHeaderRow {
    /*  margin-top:12px;*/  
    }

    .TableHeaderRow #ContentPlaceHolder1_lbl_ClanName {
         font-size: 12px !important;
         font-weight:bold;
    }

    #ContentPlaceHolder1_lblPageExplanationToMemebers {
        display:block;
    }

    #ContentPlaceHolder1_pnl_Profile td {
        padding:4px;
    }

    

    .clanPage table {
        /* remove spacing on outside of table */
        border-collapse: separate;
        border-spacing: 1px;
        margin: -1px;
        margin-top: 4px;
        border: 0 !important;
    }

    #ContentPlaceHolder1_lbl_PublicProfile {
        padding: 4px;
        display: block;
    }

        

</style>
    <%}
      else { %>
    <style>
        .clanPage, .clanPage .clanProfile
        {
            width: 100%;

        }lblClanSizeLimit

        .clanPage .clanProfile
        {
            display:table;
            border-collapse: separate;
            border-spacing:1px;
        }
        .column
        {
            display: table-cell;
            width: 50%;
            vertical-align: top;
            margin:1px 1px 1px 1px;
        }
    </style>
    <%}%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%=Utils.GetIframePopupHeaderForNotPopupBrowser(RS("Invitations") , (isMobile & !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/M_Clan.png")%>
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />
    <div class="clanPage">
        <div class="column">
            <asp:Panel ID="pnl_Invite" runat="server" Visible="false">
                <table border="0" width="100%" style="border: solid 1px black">
                    <tr>
                        <td class="TableHeaderRow" colspan="2">
                            <%= RS("invitePlyrs") %>
                            <br />
                            <span style="font-weight: normal">
                                <%= RS("enterName") %>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 1px;">
                            <asp:TextBox ID="txt_PlayerName" runat="server" CssClass="TextBox" />
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txt_PlayerName" CssClass="Error" ErrorMessage="*" ToolTip='<%# RS("revPlyrName") %>' ValidationExpression="^[a-zA-Z0-9._]{1,25}$" ValidationGroup="vg_Invite" />
                            <asp:Button CssClass="inputbutton sfx2" Text='<%# RS("inviteBtn") %>' ID="lnk_Invite" runat="server" OnClick="lnk_Invite_Click" ValidationGroup="vg_Invite" />
                            <asp:Label ID="lbl_Error" CssClass="Error" runat="server" Text='<%# RS("notMemberOf") %>' Visible="False" />
                            &nbsp;
                        </td>
                        <td>
                    </tr>
                </table>
            </asp:Panel>
            <table border="0" width="100%" style="border: solid 1px black">
                <tr>
                    <td class="TableHeaderRow">
                        <%= RS("currInvites") %>
                        <br />
                        <span style="font-weight: normal">
                            <%= RS("peopleInvited") %>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 1px;">
                        <asp:TextBox ID="txt_SearchName" runat="server" CssClass="TextBox" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txt_SearchName" CssClass="Error" ErrorMessage="*" ToolTip='<%# RS("revPlyrName") %>' ValidationExpression="^[a-zA-Z0-9._]{1,25}$" ValidationGroup="vg_Search" />
                        <asp:Button ID="btn_Search" runat="server" Text='<%# RS("searchBtn") %>' CssClass="inputbutton sfx2" OnClick="btn_Search_Click" ValidationGroup="vg_Search" />
                        &nbsp;
                        <asp:GridView ID="gvw_InvitedPlayers" runat="server" DataKeyNames="PlayerID" AutoGenerateColumns="false" OnRowCommand="GridView1_RowCommand" 
                        GridLines="None" CellSpacing="1" CssClass="TypicalTable inviteList" ShowHeader="true" AllowPaging="True" OnPageIndexChanging="gvw_InvitedPlayers_PageIndexChanging">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:HyperLink CssClass="sfx2" runat="server" ID="LnkMembername" NavigateUrl='<%#BindURL(Container.DataItem) %>'><%#Eval("Name") %></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderTemplate>
                                        <%= RS("invitedPlyr") %>
                                    </HeaderTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%#Math.Round((((DateTime)Eval("InvitedOn")).AddDays(10)-DateTime.Now).TotalDays, 1).ToString()%>
                                        <%= RS("day") %>
                                    </ItemTemplate>
                                    <HeaderTemplate>
                                        <%= RS("expIn") %>
                                    </HeaderTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton CssClass="sfx2" ID="btn_CancelClan" CommandArgument='<%#Eval("ClanID") %>' runat="server" CommandName="Delete">
                                            <%= RS("revokeLnk") %>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle CssClass="DataRowNormal" />
                            <HeaderStyle CssClass="TableHeaderRow" />
                            <AlternatingRowStyle CssClass="DataRowAlternate" />
                        </asp:GridView>
                        <asp:Panel runat="server" ID="panelRows" CssClass=pageSize>
                            <%= RS("numOfInvites") %>
                            <asp:DropDownList ID="ddlPage" runat="server" AutoPostBack="True" CssClass="DropDown" OnSelectedIndexChanged="ddlPage_SelectedIndexChanged" DataTextField="RS" DataValueField="RS">
                                <asp:ListItem Value="25">25</asp:ListItem>
                                <asp:ListItem Value="50">50</asp:ListItem>
                                <asp:ListItem Value="100">100</asp:ListItem>
                                <asp:ListItem Value="1000">all</asp:ListItem>
                            </asp:DropDownList>
                        </asp:Panel>
                        <asp:Label ID="lblNoClanMessage" runat="server" Text='<%# RS("notMemberOf") %>' Visible="False"></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:Label ID="lblClanSizeLimit" runat="server"  Font-Size="Medium"></asp:Label>
        </div>

        <% if (LoginModeHelper.isFB(Request)) { %>      
        <div class="column">
            <asp:Panel runat="server" ID="panelNoInviteLimit" Visible="true" CssClass="notOnMob">
                <table border="0" cellpadding="2" cellspacing="0">
                    <tr>
                        <td class="TableHeaderRow" style="padding: 2px" width="1%">
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/invite.aspx" ImageUrl="https://static.realmofempires.com/images/navIcons/Invite1.gif"></asp:HyperLink>
                        </td>
                        <td class="TableHeaderRow" valign="top">
                            <span style="font-weight: normal; font-size: 14px;">
                                <%= RS("didYouKnow") %>
                            </span>
                        </td>
                        <tr>
                            <td colspan="2" style="font-size: 14px;">
                                <p>
                                    <%= RS("infoPart1") %>
                                </p>
                                <p>
                                    <asp:HyperLink ID="HyperLink2" NavigateUrl="~/invite.aspx" runat="server"><B><%= RS("infoPart2") %></B></asp:HyperLink>
                                </p>
                                <p style="font-size: 11px;">
                                    <%= RS("infoPart3") %>
                                </p>
                            </td>
                        </tr>
                </table>
            </asp:Panel>
            <asp:Panel runat="server" ID="panelInviteLimit" Visible="false">
                <table border="0" cellpadding="2" cellspacing="0" rel="u_InviteLimit" class="help" width="99%">
                    <tr>
                        <td class="TableHeaderRow" style="padding: 2px" width="1%">
                            <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/invite.aspx" ImageUrl="https://static.realmofempires.com/images/navIcons/Invite1.gif" class="notOnMob"></asp:HyperLink>
                        </td>
                        <td class="TableHeaderRow" valign="top">
                            <span style="font-weight: normal; font-size: 14px;">
                                <%= RS("canIssue") %>
                                <asp:Label ID="lblInvitesLeft" runat="server"></asp:Label>
                                <%= RS("invitesAtMom") %>
                                <br />
                                <asp:Label Style="color: #A6A2A3;" ID="lblMoreInvitesOnLabel" runat="server" Text='<%# RS("moreInvAvail") %>'></asp:Label>
                                <asp:Label refresh="false" Style="color: #A6A2A3;" ID="lblMoreInvitesIn" runat="server" CssClass="countdown"></asp:Label>
                                &nbsp; </span>
                        </td>
                        <tr class="notOnMob">
                            <td colspan="2" style="font-size: 14px;">
                                <p>
                                    <b>
                                        <%= RS("wantMore") %></b>
                                    <br />
                                    <br />
                                    <%= RS("then") %>
                                    <asp:HyperLink ID="HyperLink5" NavigateUrl="~/invite.aspx" runat="server"><%= RS("inviteFBFrnds") %></asp:HyperLink>
                                    <%= RS("autoInvited") %>
                                </p>
                                <p style="font-size: 11px;">
                                    <%= RS("hint") %>
                                </p>
                            </td>
                        </tr>
                </table>
            </asp:Panel>
        </div>     
        <% }%> 

    </div>
</asp:Content>
