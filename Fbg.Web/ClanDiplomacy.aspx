<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanDiplomacy.aspx.cs"
    Inherits="ClanDiplomacy" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="c2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <%if (isMobile) { %>
    <style>
        #ctl00_ContentPlaceHolder1_HyperLink6
        {
            display: none;
        }
        
        
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
        
        .clanPage .TypicalTable td
        ,.clanPage .TypicalTable th
        {
            padding-top: 8px;
            padding-bottom: 8px;
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
    .TableHeaderRow {
        font-size: 12px !important;
        font-weight: normal;
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

    }

    #ContentPlaceHolder1_lbl_PublicProfile {
        padding: 4px;
        display: block;
    }

        

</style>
    <%}
      else { %>
    <style>
      
    </style>
    <%}%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%=Utils.GetIframePopupHeaderForNotPopupBrowser(RS("Diplomacy") , (isMobile & !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/M_Clan.png")%>

    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
<div class=clanPage>
    <br />
    <%= RS("info") %>
    <br />
    <table border="0" class="Box"  class="TypicalTable">
        <tr>
            <td>
                <asp:Panel ID="pnl_CreateDiplomacy" runat="server" Visible="false" >
                    <asp:Label ID="lbl_Error" runat="server" CssClass="Error" Visible="false"></asp:Label><br />
                    <%= RS("clanName")%>
                    <span class="ui-widget"><asp:TextBox ID="txt_ClanName" runat="server" CssClass="TextBox jsclans"></asp:TextBox></span>
                    <%=isMobile ? "<BR>" : "" %>
                    <asp:Button ID="btn_Enemy" runat="server" Text='<%# RS("setEnemyBtn") %>' OnClick="btn_Enemy_Click"
                        CssClass="inputbutton sfx2" />
                    <asp:Button ID="btn_Ally" runat="server" Text='<%# RS("setAllyBtn") %>' OnClick="btn_Ally_Click"
                        CssClass="inputbutton sfx2" />
                    <asp:Button ID="btn_NAP" runat="server" Text='<%# RS("setNAPBtn") %>' OnClick="btn_NAP_Click"
                        CssClass="inputbutton sfx2" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="TableHeaderRow">
                <strong><%= RS("hEnemies")%> </strong>
            </td>
        </tr>
        <tr>
            <td class="BoxContent">
                <asp:GridView DataKeyNames="Name" ID="grv_Enemies" runat="server" EmptyDataText=""
                    AutoGenerateColumns="False" ShowHeader="false" Width="100%" OnRowCreated="gdv_Enemies_RowCreated"
                    OnRowDeleting="grv_Enemies_RowDeleting" GridLines="None" CellSpacing="1" CssClass="TypicalTable">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#NavigationHelper.ClanPublicProfile((int)Eval("OtherClanID")) %>'
                                    Text='<%#Eval("Name") %>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" ImageUrl="https://static.realmofempires.com/images/cancel.png"
                            CommandName="Delete">
                            <ItemStyle HorizontalAlign="Center" Width="20px" />
                        </asp:ButtonField>
                    </Columns>
                    <RowStyle CssClass="DataRowNormal" />
                    <HeaderStyle CssClass="TableHeaderRow" />
                    <AlternatingRowStyle CssClass="DataRowAlternate" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td class="TableHeaderRow">
                <%= RS("hAllies")%>
            </td>
        </tr>
        <tr>
            <td class="BoxContent">
                <asp:GridView DataKeyNames="Name" ID="grv_Allies" runat="server" AutoGenerateColumns="False"
                    ShowHeader="false" Width="100%" OnRowCreated="grv_Allies_RowCreated" OnRowDeleting="grv_Allies_RowDeleting"
                    GridLines="None" CellSpacing="1" CssClass="TypicalTable">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#NavigationHelper.ClanPublicProfile((int)Eval("OtherClanID")) %>'
                                    Text='<%#Eval("Name") %>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" ImageUrl="https://static.realmofempires.com/images/cancel.png"
                            CommandName="Delete">
                            <ItemStyle HorizontalAlign="Center" Width="20px" />
                        </asp:ButtonField>
                    </Columns>
                    <RowStyle CssClass="DataRowNormal" />
                    <HeaderStyle CssClass="TableHeaderRow" />
                    <AlternatingRowStyle CssClass="DataRowAlternate" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td class="TableHeaderRow">
                <%= RS("hNAP")%>
            </td>
        </tr>
        <tr>
            <td class="BoxContent">
                <asp:GridView DataKeyNames="Name" ID="grv_NAP" runat="server" AutoGenerateColumns="False"
                    ShowHeader="false" Width="100%" OnRowCreated="grv_NAP_RowCreated" OnRowDeleting="grv_NAP_RowDeleting"
                    GridLines="None" CellSpacing="1" CssClass="TypicalTable">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#NavigationHelper.ClanPublicProfile((int)Eval("OtherClanID")) %>'
                                    Text='<%#Eval("Name") %>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" ImageUrl="https://static.realmofempires.com/images/cancel.png"
                            CommandName="Delete">
                            <ItemStyle HorizontalAlign="Center" Width="20px" />
                        </asp:ButtonField>
                    </Columns>
                    <RowStyle CssClass="DataRowNormal" />
                    <HeaderStyle CssClass="TableHeaderRow" />
                    <AlternatingRowStyle CssClass="DataRowAlternate" />
                </asp:GridView>
            </td>
        </tr>
    </table>
    </div>
</asp:Content>
