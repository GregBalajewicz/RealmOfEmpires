<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="BrowseThreads.aspx.cs"
    Inherits="BrowseThreads" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>




<asp:Content ID="Content2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">

 <%if (isMobile) { %>
    <style>
        
        .clanPage .postIcon
        , .clanPage .postIconNewPosts
        {            
            width:33px;
        }
       
        .clanPage .DataRowNormal, .clanPage .DataRowAlternate {
            background-color:rgba(0,0,0,0.7);
        }
        
        .clanPage .DataRowNormal td:nth-child(2), .clanPage .DataRowAlternate td:nth-child(2) {
            width:100%;
        }

        
        .Popup a {
            text-decoration: none;
            font: 14px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
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

        $(function () {
            $('.threadList').delegate("td:nth-child(2)", 'click',
            function (event) {
                if (!$(event.target).hasClass('postIcon')) {
                    if ($(event.target).parents("tr").length > 0 &&
                    $($(event.target).parents("tr")[0]).find('a.thread').length > 0) {
                        window.location = $($(event.target).parents("tr")[0]).find('a.thread').attr('href');
                    }
                }
            });
        });

    </script>

     <% } if(isD2) { %>

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
        background-color: rgba(0, 0, 0, 0.95) !important;
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
    <%}
      else { %>
    <style>
    </style>
    <%}%>


    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
<div class=clanPage>
    <div class="sectiontitle">
        <asp:Literal runat="server" ID="lblPageTitle" Text='<%# RS("threadsFrom")%>' Visible="False" />
        <asp:DropDownList ID="ddlForums" runat="server" onchange="javascript:document.location.href='BrowseThreads.aspx?ForumID=' + this.value;"
            Visible="False">
        </asp:DropDownList>
        <asp:ObjectDataSource ID="objForums" runat="server" SelectMethod="GetForums" TypeName="MB.TheBeerHouse.BLL.Forums.Forum">
        </asp:ObjectDataSource>
    </div>
    <p>
    </p>
    <div style="text-align: left; font-weight: bold">
        <%if (isMobile) { %><a href="ClanForum.aspx"><%= RS("backToList")%></a><BR><BR><%} %>
        <asp:HyperLink runat="server" ID="lnkNewThread1" NavigateUrl="AddEditPost.aspx?ForumID={0}"><%= RS("createNewThread")%></asp:HyperLink>

    </div>
    <p>
    </p>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:GridView ID="gvwThreads" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                PageSize="25" DataKeyNames="ID" OnRowCommand="gvwThreads_RowCommand" OnRowCreated="gvwThreads_RowCreated"
                OnRowDeleting="gvwThreads_RowDeleting" CellPadding="1" CellSpacing="1" GridLines="none"
                ShowHeader="True" CssClass="TypicalTable stripeTable" OnPageIndexChanging="gvwThreads_PageIndexChanging">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Read" Visible='<%# HasPostChanges(Container.DataItem) %>'
                                ImageUrl="https://static.realmofempires.com/images/NewPost.PNG" CommandArgument='<%# Eval("ID") %>'
                                ToolTip='<%# RS("markPostRead")%>' />
                            <asp:Image ImageAlign="left" ID="Image2" ToolTip='<%# RS("oldPost")%>' runat="server" ImageUrl="https://static.realmofempires.com/images/OldPost.PNG"
                                Visible='<%# !HasPostChanges(Container.DataItem)%>' />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Title">
                        <HeaderTemplate>
                            <%= RS("title")%>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkTitle" runat="server" Text='<%# Eval("Title") %>' NavigateUrl='<%# "ShowThread.aspx?ID=" + Eval("ID") %>'
                                CssClass='<% #BindStyle(Container.DataItem) %>' /><br />
                            <small>by
                                <%--<asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("AddedBy") %>' CssClass='<% #BindStyle(Container.DataItem) %>'></asp:Label>--%>
                                <asp:HyperLink runat="Server" ID="LnkAddedBy" Text='<%# Eval("AddedBy") %>' NavigateUrl='<%# "Player.aspx?pName=" + Eval("AddedBy") %>'
                                    CssClass='<% #BindStyle(Container.DataItem) %>'></asp:HyperLink>
                            </small>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Last Post">
                        <HeaderTemplate>
                            <%= RS("lastPost")%>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <small class='<%#BindStyle(Container.DataItem) %>'>
                                <asp:Label ID="lblLastPostDate" runat="server" Text='<%# Utils.FormatEventTime((DateTime)Eval("LastPostDate")) %>'></asp:Label><br />
                                by
                                <%--<asp:Label ID="lblLastPostBy" runat="server" Text='<%# Eval("LastPostBy") %>'></asp:Label>--%>
                                <asp:HyperLink runat="Server" ID="LnkLastPostBy" Text='<%# Eval("LastPostBy") %>'
                                    NavigateUrl='<%# "Player.aspx?pName=" + Eval("LastPostBy") %>'></asp:HyperLink>
                            </small>
                        </ItemTemplate>
                        <ItemStyle Wrap="false" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ReplyCount">
                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ViewCount">
                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:HyperLinkField Text="&lt;img border='0' src='https://static.realmofempires.com/images/MovePost.png' alt='Move thread' /&gt;"
                        DataNavigateUrlFormatString="~/MoveThread.aspx?ThreadID={0}" DataNavigateUrlFields="ID">
                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                    </asp:HyperLinkField>
                    <asp:ButtonField ButtonType="Image" ImageUrl="https://static.realmofempires.com/images/Lockpost.png"
                        CommandName="Close">
                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                    </asp:ButtonField>
                    <asp:ButtonField ButtonType="Image" ImageUrl="https://static.realmofempires.com/images/cancel.png"
                        CommandName="Delete">
                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                    </asp:ButtonField>
                </Columns>
                <RowStyle CssClass="DataRowNormal highlight" />
                <HeaderStyle CssClass="TableHeaderRow highlight" />
                <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
                <EmptyDataTemplate>
                    <b>
                        <%= RS("noThreads")%>
                    </b>
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:GridView ID="gvwThreads_m" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                PageSize="25" DataKeyNames="ID" OnRowCommand="gvwThreads_RowCommand" OnRowCreated="gvwThreads_RowCreated"
                OnRowDeleting="gvwThreads_RowDeleting" CellPadding="1" CellSpacing="1" GridLines="none"
                ShowHeader="false" CssClass="TypicalTable stripeTable threadList" OnPageIndexChanging="gvwThreads_PageIndexChanging">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton CssClass=postIconNewPosts ID="ImageButton1" runat="server" CommandName="Read" Visible='<%# HasPostChanges(Container.DataItem) %>'
                                ImageUrl="https://static.realmofempires.com/images/NewPost.PNG" CommandArgument='<%# Eval("ID") %>'
                                ToolTip='<%# RS("markPostRead")%>' />
                            <asp:Image CssClass=postIcon ImageAlign="left" ID="Image2" ToolTip='<%# RS("oldPost")%>' runat="server" ImageUrl="https://static.realmofempires.com/images/OldPost.PNG"
                                Visible='<%# !HasPostChanges(Container.DataItem)%>' />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Title">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkTitle" runat="server" Text='<%# Eval("Title") %>' NavigateUrl='<%# "ShowThread.aspx?ID=" + Eval("ID") %>'
                                CssClass='<% #BindStyle(Container.DataItem) %>' /><br />
                            <small>Started by
                                <B><%# Eval("AddedBy") %></B>
                            </small>
                            <small class='<%#BindStyle(Container.DataItem) %>'>
                                <BR /><%= RS("lastPost")%> <asp:Label ID="lblLastPostDate" runat="server" Text='<%# Utils.FormatEventTime((DateTime)Eval("LastPostDate")) %>'></asp:Label>
                                by <B><%# Eval("LastPostBy") %></B>
                             <BR /><%= RS("replies")%>: <%# Eval("ReplyCount")%>
                             <%= RS("views")%> :<%# Eval("ViewCount")%>
                            
                            
                            </small>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>                   
                </Columns>
                <RowStyle CssClass="DataRowNormal highlight" />
                <HeaderStyle CssClass="TableHeaderRow highlight" />
                <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
                <EmptyDataTemplate>
                    <b>
                        <%= RS("noThreads")%>
                    </b>
                </EmptyDataTemplate>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <p>
    </p>
    <div style="text-align: left; font-weight: bold">
        <asp:TextBox ID="txtConnectionStr" runat="server" Visible="False"></asp:TextBox>
        <asp:HyperLink runat="server" ID="lnkNewThread2" NavigateUrl="AddEditPost.aspx?ForumID={0}"><%= RS("createNewThread")%></asp:HyperLink></div>
    <asp:ObjectDataSource ID="objThreads" runat="server" SelectMethod="GetThreads" SelectCountMethod="GetThreadCount"
        TypeName="MB.TheBeerHouse.DAL.SqlClient.SqlForumsProvider" EnablePaging="True"
        SortParameterName="sortExpression">
        <SelectParameters>
            <asp:QueryStringParameter Name="forumID" QueryStringField="ForumID" Type="Int32" />
            <asp:Parameter Name="sortExpression" Type="String" DefaultValue="" />
            <asp:Parameter DefaultValue="0" Name="pageIndex" Type="Int32" />
            <asp:Parameter DefaultValue="25" Name="pageSize" Type="Int32" />
            <asp:ControlParameter ControlID="txtConnectionStr" DefaultValue="" Name="ConnectionStr"
                PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</div>
</asp:Content>
