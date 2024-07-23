<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ManageForums.aspx.cs"
    Inherits="ManageForums" ValidateRequest="false" %>

<%@ Register Src="~/Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">

 <%if (isMobile) { %>
    <style>
        .Popup a {
            text-decoration: none;
            font: 14px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
        }

        .clanPage .action
        {
            font-size: 12pt;
            margin: 8px 0px 8px 0px;
        }
        .clanPage .actions a
        {
            display:block;
            padding:5px;
        }

        .clanPage ul.actions {
            display: block;
            padding: 5px;
            list-style-type: none !important;
            margin: 0px;
            background-color: rgba(0, 0, 0, 0.6);
        }
        .clanPage .TableHeaderRow {
            background-image: none;
            background-color: rgba(29, 29, 29, 0.6);
            border-bottom: 1px solid rgba(0, 0, 0, 0.8);
            color: #D7D7D7;
            font-weight: normal;
            padding: 0px 5px;
        }
            .clanPage .TableHeaderRow th:nth-child(0) {
                width: 10%;
            }
            .clanPage .TableHeaderRow th:nth-child(1) {
                width: 10%;
            }
            .clanPage .TableHeaderRow th:nth-child(2) {
                width: 10%;
            }
            .clanPage .TableHeaderRow th:last-child {
                width: 100%;
            }
        .clanPage tr {
            background-color: rgba(0, 0, 0, 0.6);
        }
        .clanPage .tableaction
        {            
            padding: 0px 0px 5px 0px;
            display:block;
        }

        .clanPage .editForum a
        {
            padding: 5px 5px 5px 5px;
            display:table-cell;

        }


        .clanPage .TypicalTable TD {
            max-width: 90px;
            overflow: hidden;
            word-break:break-all;
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

        .clanPage .members td
        {
            padding-top:10px;
            padding-bottom:10px;
        }

        .clanPage .showSafe,
        .clanPage .activityCol 
        {
            display:none;
        }

        .editForum tr:nth-child(3) {
            display:none;
        }

        .TDContent {
            position: absolute;
            left: 0px;
            right: 0px;
            top: 0px;
            bottom: 0px;
            overflow: auto;
        }

    </style>
    <script>


    </script>
    <%} else  if(isD2) { %>
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
    <% } %>
    
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
<div class=clanPage>
    <ul style="list-style-type: square" class=actions>
       <li>
            <asp:HyperLink ID="lnkManageUnapprovedPosts" runat="server" NavigateUrl="ManageUnapprovedPosts.aspx"><%= RS("mngUnapprovedPosts")%></asp:HyperLink></li>
        <li>
            <asp:HyperLink ID="lnkManageDeletedForums" runat="server" NavigateUrl="ManageDeletedForums.aspx"><%= RS("mngDelForums")%></asp:HyperLink></li>
        <li>
            <asp:HyperLink ID="lnkManageSharedForums" runat="server" NavigateUrl="ManageSharedForums.aspx"><%= RS("mngMySharedForum")%> </asp:HyperLink></li>
        <li>
            <asp:HyperLink ID="lnkManageWhiteListClan" runat="server" NavigateUrl="~/ManageWhiteListClans.aspx"><%= RS("mngWhiteList")%></asp:HyperLink></li>
    </ul>
    <p>
        <asp:CheckBox ID="cbShowSafe" runat="server" AutoPostBack="True" Checked="True" OnCheckedChanged="cbShowSafe_CheckedChanged"
            Text='<%# RS("cb_ViewInSafeMode")%>' CssClass="showSafe" />
    </p>
    <asp:GridView ID="gvwForums" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
        OnRowDeleted="gvwForums_RowDeleted" OnRowCreated="gvwForums_RowCreated" OnSelectedIndexChanged="gvwForums_SelectedIndexChanged"
        OnRowDeleting="gvwForums_RowDeleting" CellPadding="1" CellSpacing="1" GridLines="none"
        ShowHeader="True" CssClass="TypicalTable stripeTable">
        <Columns>
            <asp:CommandField ItemStyle-VerticalAlign="Top" ButtonType="Image" SelectImageUrl="https://static.realmofempires.com/images/item_rename.png"
                SelectText="Edit forum" ShowSelectButton="True">
                <ItemStyle HorizontalAlign="Center" Width="20px" />
            </asp:CommandField>
            <asp:CommandField ItemStyle-VerticalAlign="Top" ButtonType="Image" DeleteImageUrl="https://static.realmofempires.com/images/cancel.png"
                DeleteText="Delete forum" ShowDeleteButton="True">
                <ItemStyle HorizontalAlign="Center" Width="20px" />
            </asp:CommandField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Image ImageAlign="left" ID="Image2" ToolTip='<%# RS("imgOldPostTT")%>' runat="server"
                        ImageUrl="https://static.realmofempires.com/images/OldPost.PNG" />
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle VerticalAlign="Top" />
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                <HeaderTemplate>
                    <%= RS("hSort")%></HeaderTemplate>
                <ItemTemplate>
                    <div class="sectionsubtitle">
                        <asp:Literal runat="server" ID="lblImportance" Text='<%# Eval("Importance") %>' />
                    </div>
                    <itemstyle verticalalign="Top" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <%= RS("hNameDecs")%></HeaderTemplate>
                <ItemTemplate>
                    <div class="sectionsubtitle">
                        <asp:Literal runat="server" ID="lblForumTitle" Text='<%# RemoveHtmlConditional(Eval("Title")) %>' />
                        <asp:Literal runat="Server" ID="lblIsModerated" Text='<%# RS("lblModerated") %>'
                            Visible='<%# Eval("Moderated") %>' />
                    </div>
                    <br />
                    <asp:Literal runat="server" ID="lblDescription" Text='<%# RemoveHtmlConditional(Eval("Description")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
           
        </Columns>
        <RowStyle CssClass="DataRowNormal highlight" />
        <HeaderStyle CssClass="TableHeaderRow highlight" />
        <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
        <EmptyDataTemplate>
            <b>
                <%= RS("noForums") %></b></EmptyDataTemplate>
    </asp:GridView>
    <asp:ObjectDataSource ID="objAllForums" runat="server" SelectMethod="GetForums" TypeName="MB.TheBeerHouse.BLL.Forums.Forum"
        DeleteMethod="DeleteForum"></asp:ObjectDataSource>
    <p>
        <HR></HR>
    </p>
    <asp:DetailsView ID="dvwForum" runat="server" AutoGenerateRows="False" Width="95%"
        AutoGenerateEditButton="True" AutoGenerateInsertButton="True" HeaderText='<%# RS("forumDetail") %>'
        OnItemInserted="dvwForum_ItemInserted" OnItemUpdated="dvwForum_ItemUpdated" DataKeyNames="ID"
        OnItemCreated="dvwForum_ItemCreated" DefaultMode="Insert" OnItemCommand="dvwForum_ItemCommand"
        OnItemInserting="dvwForum_ItemInserting" OnItemUpdating="dvwForum_ItemUpdating"
        OnModeChanging="dvwForum_ModeChanging" HeaderStyle-Font-Bold="true" GridLines="None"
        BorderStyle="none" CssClass="editForum" >
        <FooterStyle CssClass=actions />
        <FieldHeaderStyle Width="1%" Font-Bold="true" />
        <Fields>
            <asp:TemplateField HeaderText="Title" SortExpression="Title" Visible="False">
                <ItemTemplate>
                    <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>' Visible="false"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="AddedDate" InsertVisible="False" ReadOnly="True" SortExpression="AddedDate" />
            <asp:BoundField DataField="AddedBy" InsertVisible="False" ReadOnly="True" SortExpression="AddedBy" />
            <asp:TemplateField SortExpression="Title">
                <HeaderTemplate>
                    <%= RS("hTitle") %></HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtTitle" runat="server" Text='<%# Bind("Title") %>' MaxLength="256"
                        Width="98%" CssClass="TextBox"></asp:TextBox>
                    <asp:RequiredFieldValidator CssClass="Error" ID="valRequireTitle" runat="server"
                        ControlToValidate="txtTitle" SetFocusOnError="true" Text='<%# RS("reqTitle") %>'
                        ToolTip='<%# RS("reqTitle") %>' Display="Dynamic"></asp:RequiredFieldValidator>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="Moderated" >
                <HeaderTemplate>
                    <%= RS("hModerated") %></HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkModerated" runat="server" Checked='<%# Bind("Moderated") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkModerated" runat="server" Checked='<%# Bind("Moderated") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="Description" ConvertEmptyStringToNull="False">
                <HeaderTemplate>
                    <%= RS("hDescription")%></HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description") %>' Width="99%"></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtDescription" runat="server" Text='<%# Bind("Description") %>'
                        Rows="5" TextMode="MultiLine" MaxLength="4000" Width="98%" CssClass="TextBox"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="SortOrder">
                <HeaderTemplate>
                    <%= RS("hSortOrder")%></HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblImportance" runat="server" Text='<%# Eval("Importance") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtImportance" runat="server" Text='<%# Bind("Importance") %>' MaxLength="256"
                        Width="5%" CssClass="TextBox"></asp:TextBox>
                    <asp:RangeValidator ID="RangeValidator1" MinimumValue="0" MaximumValue="1000" Type="Integer"
                        ControlToValidate="txtImportance" runat="server" CssClass="Error" ErrorMessage='<%# RS("onlyNumsVal") %>'></asp:RangeValidator>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="AlertClanMembers">
                <HeaderTemplate>
                    <%= RS("hAlertMembers")%></HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkAlertClanMembers" runat="server" Checked='<%# Bind("AlertClanMembers") %>'
                        Text='<%# RS("alertClanMembers_desc") %>' />
                     <div class="jaxHide" rel='<%= RS("AlertLegendTitle")%>' >
                        <BR><%= RS("AlertLegendBody")%>
                    </div>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chkAlertClanMembers" runat="server" Checked='<%# Bind("AlertClanMembers") %>'
                        Text='<%# RS("alertClanMembers_desc") %>' /> 
                     <div class="jaxHide" rel='<%= RS("AlertLegendTitle")%>' >
                        <BR></BR><%= RS("AlertLegendBody")%>
                    </div>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField SortExpression="SecurityLevel">
                <HeaderTemplate>
                    <%= RS("hSecurityLevel") %></HeaderTemplate>
                <ItemTemplate>
                    <asp:DropDownList ID="cmb_SecurityLevel" runat="server" AppendDataBoundItems="true">
                        <asp:ListItem Selected="True" Value="0">All clan members</asp:ListItem>
                        <asp:ListItem Value="1">Owners & Admins</asp:ListItem>
                        <asp:ListItem Value="2">Owners to Diplomats</asp:ListItem>
                        <asp:ListItem Value="3">Owners to Forum Admins</asp:ListItem>
                        <asp:ListItem Value="4">Owners to Inviters</asp:ListItem>
                    </asp:DropDownList>                    
                </ItemTemplate>
                <EditItemTemplate>                    
                    <asp:DropDownList ID="cmb_SecurityLevel" runat="server" AppendDataBoundItems = "true">
                    </asp:DropDownList>                    <br />
                    <div class="jaxHide" rel='<%= RS("SecurityLegendTitle")%>' >
                        <%= RS("SecurityLegendBody")%>
                    </div>
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
    <asp:ObjectDataSource ID="objCurrForum" runat="server" SelectMethod="GetForums" TypeName="MB.TheBeerHouse.DAL.SqlClient.SqlForumsProvider"
        OldValuesParameterFormatString="original_{0}" InsertMethod="InsertForum" OnInserting="objCurrForum_Inserting"
        OnUpdating="objCurrForum_Updating" UpdateMethod="UpdateForum">
        <InsertParameters>
            <asp:Parameter Name="AddedDate" Type="DateTime" />
            <asp:Parameter Name="AddedBy" Type="String" />
            <asp:Parameter Name="Title" Type="String" />
            <asp:Parameter Name="Moderated" Type="Boolean" />
            <asp:Parameter Name="Importance" Type="Int32" />
            <asp:Parameter Name="ImageUrl" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="ClanID" Type="Int32" />
            <asp:ControlParameter ControlID="TextBox1" Name="ConnectionStr" PropertyName="Text"
                Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="Title" Type="String" />
            <asp:Parameter Name="Moderated" Type="Boolean" />
            <asp:Parameter Name="Importance" Type="Int32" />
            <asp:Parameter Name="ImageUrl" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="ClanID" Type="Int32" />
            <asp:Parameter Name="ForumID" Type="Int32" />
            <asp:Parameter Name="ConnectionStr" Type="String" />
        </UpdateParameters>
    </asp:ObjectDataSource>
    <asp:TextBox ID="TextBox1" runat="server" Visible="False"></asp:TextBox>
    </div>
</asp:Content>
