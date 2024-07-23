<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanForum.aspx.cs"
    Inherits="ClanForum" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">

 <%if (isMobile) { %>
    <style>
        
        .clanPage a.action
        {
            font-size: 12pt;
            margin: 8px 0px 8px 0px;
        }

        .clanPage .forumList {
            background-color: rgba(0, 0, 0, 0.25);
        }
            .clanPage .forumList .action img, .clanPage .forumList img.action, .clanPage .forumList .action {
                width: 25px;
                margin-right: 5px;
                float: left;
            }

            .clanPage .forumList p {
                float: left;
                width: 275px;
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
            $('.forumList').delegate("tr td p", 'click',
            function (event) {
                if ($(event.target).parents("tr").length > 0 &&
                    $($(event.target).parents("tr")[0]).find('a.forumName').length > 0) {
                    window.location = $($(event.target).parents("tr")[0]).find('a.forumName').attr('href');
                }
            });
        });

    </script>
    
    <% } else if(isD2) { %>

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

    <%}%>


</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />
<div class=clanPage>
    <div class="sectiontitle">
        &nbsp;</div>
    <asp:Label ID="lbl_Message" runat="server" Text="Label"></asp:Label>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:DataList ID="dlstForums" EnableTheming="False" runat="server" DataKeyField="ID"
                CssClass="TypicalTable stripeTable forumList" OnItemCommand="dlstForums_ItemCommand" Width=100%>
                <ItemTemplate>
                    <asp:ImageButton CssClass=action ID="ImageButton1" ImageAlign="Left" runat="server" CommandName="Read"
                        Visible='<%# (int)Eval("IsViewed") == 1? false :true%>' ImageUrl="https://static.realmofempires.com/images/NewForum.PNG"
                        CommandArgument='<%# Eval("ID") %>' ToolTip='<%# RS("markPostRead") %>' />
                    <asp:Image  CssClass=action ImageAlign="left" ID="Image2" ToolTip='<%# RS("noNewPosts") %>' runat="server"
                        ImageUrl="https://static.realmofempires.com/images/OldForum.PNG" Visible='<%# (int)Eval("IsViewed") == 1? true:false%>' />
                    <p style="padding: 2px">
                        <asp:HyperLink runat="server" ID="lnkForumTitle" Text='<%# Eval("Title") %>' NavigateUrl='<%# "BrowseThreads.aspx?ForumID=" + Eval("ID") %>'
                            CssClass='<%# (int)Eval("IsViewed") == 1? "forumName" :"NewReport forumName"%>' /><br />
                        <asp:Label runat="server" ID="lblDescription" Text='<%# Eval("Description") %>' Font-Bold='<%# (int)Eval("IsViewed") == 1? false :true%>' />
                    </p>
                </ItemTemplate>
                <AlternatingItemStyle CssClass="DataRowNormal highlight" />
                <ItemStyle CssClass="DataRowAlternate highlight" />
            </asp:DataList>
            <br />
            <br />
            <div class="sectiontitle"><%=RS ("Forms") %></div>
             <asp:DataList ID="dlstSharedForum" EnableTheming="False" runat="server" DataKeyField="ID"
                CssClass="TypicalTable stripeTable forumList" >
                <ItemTemplate>
                    <asp:ImageButton  CssClass=action ID="ImageButton1" ImageAlign="Left" runat="server" CommandName="Read"
                        Visible='<%# (int)Eval("IsViewed") == 1? false :true%>' ImageUrl="https://static.realmofempires.com/images/NewForum.PNG"
                        CommandArgument='<%# Eval("ID") %>' ToolTip='<%# RS("markPostRead") %>' />
                    <asp:Image  CssClass=action ImageAlign="left" ID="Image2" ToolTip='<%# RS("noNewPosts") %>' runat="server"
                        ImageUrl="https://static.realmofempires.com/images/OldForum.PNG" Visible='<%# (int)Eval("IsViewed") == 1? true:false%>' />
                    <p style="padding: 2px">
                        <asp:HyperLink runat="server" ID="lnkForumTitle" Text='<%# Eval("Title") %>' NavigateUrl='<%# "BrowseThreads.aspx?ForumID=" + Eval("ID") %>'
                            CssClass='<%# (int)Eval("IsViewed") == 1? "forumName" :"NewReport forumName"%>' /><br />
                        <asp:Label runat="server" ID="lblDescription" Text='<%# Eval("Description") %>' Font-Bold='<%# (int)Eval("IsViewed") == 1? false :true%>' />
                    </p>
                </ItemTemplate>
                <AlternatingItemStyle CssClass="DataRowNormal highlight" />
                <ItemStyle CssClass="DataRowAlternate highlight" />
                 
            </asp:DataList>
        </ContentTemplate>
    </asp:UpdatePanel>
 </div>
</asp:Content>
