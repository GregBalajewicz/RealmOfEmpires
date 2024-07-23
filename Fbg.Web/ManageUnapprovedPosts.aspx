<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="ManageUnapprovedPosts.aspx.cs"
    Inherits="ManageUnapprovedPosts"   ValidateRequest="false"  %>

<%@ Register Src="~/Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
    <% if(isD2) { %>

<style>
    /* Clan Overview */
    html {
        background-color:#000000;
        background: url('https://static.realmofempires.com/images/misc/M_BG_Generic.jpg') no-repeat center center fixed; 
        -webkit-background-size: cover;
        -moz-background-size: cover;
        -o-background-size: cover;
        background-size: cover;
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
        height:initial !important;
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

    
    /* Clan Settings */
    .inputbutton, .inputsubmit {
         height:initial !important;
    }    

</style>

    <%}%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <div class="sectiontitle">
        <asp:Literal runat="server" ID="lblPageTitle" Text='<%# RS("lbl_WaitngPosts") %>'/></div>
    <p>
    </p>
    <%if (isMobile) { %><a href="manageforums.aspx">Back to Clan Forum Admin</a> <Br></Br><Br></Br>  <%} %>
    <asp:GridView ID="gvwPosts" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
        OnRowCommand="gvwPosts_RowCommand" OnRowCreated="gvwPosts_RowCreated" OnRowDeleting="gvwPosts_RowDeleting"
        CssClass="TypicalTable stripeTable" CellPadding="1" CellSpacing="1" GridLines="none">
        <Columns>
            <asp:TemplateField HeaderText="Title">
            <HeaderTemplate><%= RS("hTitle") %></HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title") %>'></asp:Label><br />
                    <asp:Label ID="lblBody" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Body")) %>'></asp:Label><br />
                    <small><%= RS("_by")%>
                        <asp:HyperLink runat="Server" ID="LnkAddedBy" Text='<%# Eval("AddedBy") %>' NavigateUrl='<%# "Player.aspx?pName=" + Eval("AddedBy") %>'></asp:HyperLink></small>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Label ID="lblTitle" runat="server" Font-Bold="true" Text='<%# Eval("Title") %>' /><br />
                    <small><%= RS("_by")%>
                        <asp:HyperLink runat="Server" ID="LnkAddedBy" Text='<%# Eval("AddedBy") %>' NavigateUrl='<%# "Player.aspx?pName=" + Eval("AddedBy") %>'></asp:HyperLink><br />
                        <br />
                        <div style="border-top: dashed 1px; border-right: dashed 1px;">
                            <asp:Label runat="server" ID="lblBody" Text='<%#HttpUtility.HtmlEncode((string) Eval("Body")) %>'></asp:Label></div>
                    </small>
                </EditItemTemplate>
                <HeaderStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:BoundField DataField="ForumTitle" HeaderText="Forum" ReadOnly="True">
                <ItemStyle HorizontalAlign="Center" Width="150px" />
                <HeaderStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField>
                <HeaderTemplate>
                  <%= RS("hAddedDate")%>
                </HeaderTemplate>
                <ItemTemplate>
                   <%# Utils.FormatEventTime((DateTime)Eval("LastPostDate")) %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="150px" />
                <HeaderStyle HorizontalAlign="Center" />
            </asp:TemplateField>
           
            <asp:ButtonField ButtonType="Image" ImageUrl="https://static.realmofempires.com/images/Ok.gif" CommandName="Approve">
                <ItemStyle HorizontalAlign="Center" Width="20px" />
            </asp:ButtonField>
            <asp:ButtonField ButtonType="Image" ImageUrl="https://static.realmofempires.com/images/cancel.png" CommandName="Delete">
                <ItemStyle HorizontalAlign="Center" Width="20px" />
            </asp:ButtonField>
        </Columns>
        <EmptyDataTemplate>
            <b> <%= RS("noPostsToShow")%></b></EmptyDataTemplate>
        <RowStyle CssClass="DataRowNormal highlight" />
        <HeaderStyle CssClass="TableHeaderRow highlight" />
        <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
    </asp:GridView>
    &nbsp;&nbsp;
</asp:Content>
