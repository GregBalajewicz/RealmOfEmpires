<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanOverview.aspx.cs"
    Inherits="ClanOverview" ValidateRequest="False" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID=c2 ContentPlaceHolderID=HeadPlaceHolder runat=server>


<%if (isMobile)
  { %>
<style>
    .clanPage rr.td {  padding-top:8px;
    padding-bottom:8px;}
    
    .clanPage .action {font-size:12pt; margin: 8px 0px 8px 0px;}
    .clanPage .actions a {font-size:12pt; margin: 8px 0px 8px 0px;}

    .clanPage {width:100%;  }
    .column {display:inline;width:100%}
   

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
        margin-bottom:14px;
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


</style>

<%} else { %>
<style>
    .clanPage {width:100%; display:table;}
    .column {display:table-cell;width:50%; vertical-align:top;}
</style>
<%}%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%=Utils.GetIframePopupHeaderForNotPopupBrowser(RS("Overview") , (isMobile & !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/M_Clan.png")%>
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />
    <div class=clanPage>
    <asp:Label ID="lbl_Error" runat="server" Visible="False" CssClass="Error"></asp:Label>
    <div class=column>
                <table border="0" cellpadding="0" cellspacing="0" class="Box" width="100%">
                    <tr>
                        <td class="Sectionheader">
                            <%= RS("hMyClan") %>
                        </td>
                    </tr>
                    <tr>
                        <td class="BoxContent Padding">
                            <asp:Panel ID="pnl_ClanOverview" runat="server" >
                                <%= RS("currClan") %>
                                <asp:HyperLink runat="Server" ID="LnkClanName" CssClass="sfx2" Text=""></asp:HyperLink>
                                <asp:Label ID="lbl_ClanDesc" runat="server" Text="Label" Visible="False"></asp:Label>
                                <br />
                                <br />
                                <asp:LinkButton CssClass="action sfx2" ID="btn_LeaveClan" runat="server" message='<%# RS("confirmLeave")%>'
                                    OnClientClick='return confirm(this.getAttribute("message"));' OnClick="btn_LeaveClan_Click"><%= RS("leaveClanBtn")%></asp:LinkButton><br />
                                <asp:LinkButton CssClass="action sfx2" ID="lnk_DeleteClan" runat="server" message='<%# RS("confirmDisband")%>'
                                    OnClientClick='return confirm(this.getAttribute("message"));' OnClick="lnk_DeleteClan_Click"><%= RS("disbClan")%></asp:LinkButton>
                            </asp:Panel>
                            <div class="actions">
                            <asp:Panel class="rplClanRename" ID="pnl_RenameClan"  runat="server" >
                                <strong><%= RS("renameName")%></strong>
                                <asp:TextBox ID="txt_RenameClan" runat="server" CssClass="TextBox" MaxLength="30"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txt_RenameClan"
                                    ErrorMessage="*" ValidationGroup="vg_RenameClan"></asp:RequiredFieldValidator>
                                <asp:Button ID="btn_RenameClan" runat="server" Text='<%# RS("saveBtn")%>' CssClass="inputbutton sfx2"
                                    OnClick="btn_RenameClan_Click" ValidationGroup="vg_RenameClan" message='<%# RS("renameClan")%>'
                                    OnClientClick='return confirm(this.getAttribute("message"));' />
                                <br /><br />
                                <div class="jaxHide" id= "div_invalidChars1"  rel='<%# RS("invRemoved")%>' runat="server">
                                    <%= RS("charNotAllowed")%>
                                    <ul>
                                        <li><%= RS("noHTMLEle")%></li>
                                        <li>[</li>
                                        <li>]</li>
                                    </ul>
                                </div>
                            </asp:Panel>
                            </div>
                            <asp:Panel ID="pnl_CreateClan" runat="server">
                                <%= RS("clanDetail")%>
                                <table>
                                    <tr>
                                        <td>
                                            <strong><%= RS("name")%></strong>
                                        </td>
                                        <td>
                                            <asp:TextBox CssClass="TextBox" ID="txt_ClanName" runat="server" MaxLength="30"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txt_ClanName"
                                                ErrorMessage="**" ValidationGroup="vg_create"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:TextBox CssClass="TextBox" ID="txt_ClanDesc" runat="server" Visible="False"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Button CssClass="inputbutton" ID="lnk_CreateClan" runat="server" OnClick="lnk_CreateClan_Click"
                                    ValidationGroup="vg_create" Text='<%# RS("btn_CreateClan")%>'></asp:Button>
                                <br /><br />
                                <div class="jaxHide" id= "div_invalidChars2"  rel='<%# RS("invRemoved")%>' runat="server">
                                    <%= RS("charNotAllowed")%>
                                    <ul>
                                        <li>
                                            <%= RS("noHTMLEle")%></li>
                                        <li>[</li>
                                        <li>]</li>
                                    </ul>
                                </div>
                            </asp:Panel>
                            &nbsp;
                        </td>
                    </tr>
                </table>
                <br />
                <asp:GridView Width="100%" ID="gvw_Events" runat="server" AutoGenerateColumns="False"
                    CellSpacing="0" CssClass="TypicalTable help" GridLines="None" ShowHeader="True">
                    <RowStyle CssClass="DataRowNormal" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <%= RS("hRecentEvents")%></HeaderTemplate>
                                <HeaderStyle HorizontalAlign=Left />
                            <ItemTemplate>
                                <asp:Label Style="color: #A6A2A3;" ID="lblLastPostDate" runat="server" Text='<%# Utils.FormatEventTime((DateTime)Eval("Time")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Message" />
                    </Columns>
                    <HeaderStyle CssClass="TableHeaderRow" />
                    <AlternatingRowStyle CssClass="DataRowAlternate" />
                </asp:GridView>
    </div>
    <div class=column>
                <%if (isMobile) { %><br /><%} %>
                <table border="0" cellpadding="0" cellspacing="0" class="Box">
                    <tr>
                        <td class="Sectionheader">
                            <%= RS("hInvitation")%>
                        </td>
                    </tr>
                    <tr>
                        <td class="BoxContent" valign="top">
                            <asp:GridView DataKeyNames="ClanID" ID="GridView1" runat="server" EmptyDataText='<%# RS("emptyInvites")%>'
                                GridLines="None" CellSpacing="1" CssClass="TypicalTable" AutoGenerateColumns="False"
                                OnRowCommand="GridView1_RowCommand" ShowHeader="false" OnRowCreated="GridView1_RowCreated">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <a href='ClanPublicProfile.aspx?ClanID=<%#Eval("ClanID") %>' class=action>
                                                <%#Eval("Name") %>
                                            </a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <%= RS("expsIn")%>
                                            <%#Math.Round((((DateTime)Eval("InvitedOn")).AddDays(10)-DateTime.Now).TotalDays, 1).ToString()%>
                                            <%= RS("days")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btn_JoinClan" CommandArgument='<%#Eval("ClanID") %>' runat="server" class=action
                                                CommandName="Join"><%= RS("acceptBtn")%></asp:LinkButton>
                                            &nbsp;
                                            <asp:LinkButton ID="btn_CancelClan" CommandArgument='<%#Eval("ClanID") %>' runat="server" class=action
                                                CommandName="Cancel"><%= RS("rejectBtn")%></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle CssClass="DataRowNormal" />
                                <HeaderStyle CssClass="TableHeaderRow" />
                                <AlternatingRowStyle CssClass="DataRowAlternate" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <br />
                <table class="TypicalTable" border="0" cellpadding="0" cellspacing="1" width="100%"
                    runat="server" visible="false">
                    <tr>
                        <td class="TableHeaderRow">
                            <%= RS("hClanAnn")%>
                            <asp:LinkButton ID="lnk_Edit" runat="server" Font-Bold="false" OnClick="lnk_Edit_Click"
                                Visible="false"><%= RS("editlLnkBtn")%></asp:LinkButton>
                            <br />
                            <span style="font-weight: normal">
                                <%= RS("dispToAll")%></span>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="lbl_PublicProfile" runat="server"></asp:Label>
                            <asp:Panel ID="pnl_PublicProfile" runat="server" Visible="false">
                                <asp:TextBox ID="txt_PublicProfile" runat="server" TextMode="MultiLine" Height="160px" Width="100%" CssClass="TextBox"></asp:TextBox>
                                <br />
                                <asp:Button ID="btn_Save" runat="server" CssClass="inputbutton" Text='<%# RS("saveBtn")%>' OnClick="btn_Save_Click" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <div class="jaxHide"  id= "div_invalidChars3"  rel='<%# RS("invRemoved")%>' runat="server">
                                    <%= RS("inputCharNotAll")%>
                                    <ul>
                                        <li>
                                            <%= RS("noHTMLEle")%></li>
                                        <li>
                                            <%= RS("lessThan")%>
                                            - <b>&lt</b> - <span style="color: #B5B4B4">
                                                <%= RS("typeInstead")%></span></li>
                                        <li>
                                            <%= RS("dblBackSlashes")%>
                                            <b>\\</b></li>
                                    </ul>
                                </div>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    </div>
</div>
</asp:Content>
