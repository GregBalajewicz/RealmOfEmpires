<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanEvents.aspx.cs" Inherits="ClanEvents" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link type="text/json" rel="help" href="static/help/ga_ClanMembers.json.aspx" />
    <style>
a.MissingRole { 
  color: #000000; 
  text-decoration: none; } 
 
a.MissingRole:hover { 
    color: #00C125; 
  text-decoration: none; } 

a.GotRole:hover { 
  color: #FA0625; 
  text-decoration: none; } 
 
a.GotRole { 
    color: #FFFFFF; 
  text-decoration: none; } 
.MissingRole { 
  color: #000000; 
  text-decoration: none; } 
 .GotRole { 
    color: #ffffff; 
  text-decoration: none; }
</style>
     <% if(isD2) { %>
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
    <%}       %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%=Utils.GetIframePopupHeaderForNotPopupBrowser("Events" , (isMobile & !IsiFramePopupsBrowser))%>
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />
    <table border="0">
        <tr>
            <td>
                <asp:Label ID="lbl_Error" CssClass="Error" runat="server" Visible="False"></asp:Label>
                <asp:GridView ID="gvw_Events" runat="server" EmptyDataText="No Events" AutoGenerateColumns="False" 
                GridLines="None" CellSpacing="1" CssClass="TypicalTable help" ShowHeader="false" OnPageIndexChanging="gvw_Events_PageIndexChanging" AllowPaging="true" PageSize="50">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label Style="color: #A6A2A3;" ID="lblLastPostDate" runat="server" Text='<%# Utils.FormatEventTime((DateTime)Eval("Time")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Message" HeaderText="Events" />
                    </Columns>
                    <RowStyle CssClass="DataRowNormal" />
                    <HeaderStyle CssClass="TableHeaderRow" />
                    <AlternatingRowStyle CssClass="DataRowAlternate" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
    </table>
</asp:Content>
