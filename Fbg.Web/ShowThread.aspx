<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ShowThread.aspx.cs"
    Inherits="ShowThread" EnableEventValidation="false" ValidateRequest="false" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">

 <%if (isMobile) { %>
    <style>

        .clanPage .actions
        {
            margin: 8px 0px 8px 0px;
            display:table-cell;
        }

        
        .clanPage a.action
        {
            margin: 8px 0px 8px 0px;
        }

        .clanPage .actions a {
            font: 15px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 2px 2px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
        }


        blockquote {
            margin: 10px;
        }
       

        .clanPage .notMob
        {
            display:none;
        }

         /*BEGIN For clan Forum */
        .clanPage .posttitle
        {
	        font-weight: bold;
            width: 295px;
        }

        .Popup a {
            text-decoration: none;
            font: 14px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
        }

        .clanPage .DataRowAlternate, .clanPage .DataRowNormal {
            background-color: rgba(0, 0, 0, 0.7);
            //background-image: url('https://static.realmofempires.com/images/misc/metalgrad320x1.png');
            background-repeat: no-repeat;
            background-size: 100% 1px;
            background-position: 0px 0px;
        }
            .clanPage .DataRowAlternate td, .clanPage .DataRowNormal td {
                padding: 0px;
                padding-bottom: 10px;
            }
                .clanPage .DataRowAlternate td .TableHeaderRow, .clanPage .DataRowNormal td .TableHeaderRow {
                    background-image:none;
                    background-color: rgba(29, 29, 29, 0.6);
                    border-bottom: 1px solid rgba(0, 0, 0, 0.8);
                    color: #D7D7D7;
                    font-weight: normal;
                    padding: 0px 5px;
                }
                .clanPage .DataRowAlternate td .postbody, .clanPage .DataRowNormal td .postbody {
                    padding: 0px 5px;
                    width: 304px;
                    overflow: hidden;
                    word-break: break-all;
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
            $('.forumList').delegate("tr", 'click',
            function (event) {
                if ($(event.target).parents("tr").length > 0 &&
                    $($(event.target).parents("tr")[0]).find('a.forumName').length > 0) {
                    window.location = $($(event.target).parents("tr")[0]).find('a.forumName').attr('href');
                }
            });

            $('.bbcode_v').removeAttr('onclick');
            $('.bbcode_p').removeAttr('onclick');
            $('.bbcode_c').removeAttr('onclick');
            $('.bbcode_c').removeAttr('href');

            //
            // handle some bbcodes
            //
            $('.TDContent').delegate('.bbcode_p', 'click', function () {
                window.parent.ROE.Frame.popupPlayerProfile($(event.target).html());
                return false;
            });
            $('.TDContent').delegate('.bbcode_v', 'click', function () {
                window.parent.ROE.Frame.popupVillageProfile($(event.target).attr('data-vid'));
                return false;
            });
            $('.TDContent').delegate('.bbcode_c', 'click', function () {
                //since the clan popup window is a singleton, we cannot open a new one when we already got one opened...
                window.parent.ROE.Frame.popupClan($(event.target).attr('data-cid'));
                return false;
            });
        });

    </script>
    <%} else if(isD2) { %>
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

     <script>

         $(function () {
            

             $('.bbcode_v').removeAttr('onclick');
             $('.bbcode_p').removeAttr('onclick');
             $('.bbcode_c').removeAttr('onclick');
             $('.bbcode_c').removeAttr('href');

             //
             // handle some bbcodes
             //
             $('.TDContent').delegate('.bbcode_p', 'click', function () {
                 window.parent.ROE.Frame.popupPlayerProfile($(event.target).html());
                 return false;
             });
             $('.TDContent').delegate('.bbcode_v', 'click', function () {
                 window.parent.ROE.Frame.popupVillageProfile($(event.target).attr('data-vid'));
                 return false;
             });
             $('.TDContent').delegate('.bbcode_c', 'click', function () {
                 //since the clan popup window is a singleton, we cannot open a new one when we already got one opened...
                 window.parent.ROE.Frame.popupClan($(event.target).attr('data-cid'));
                 return false;
             });
         });

    </script>
    <%}
      else { %>
    <style>
         .clanPage .actions
        {
            margin: 0px 8px 0px 8px;
            display:table-cell;
        }
         /*BEGIN For clan Forum */
        .clanPage .posttitle
        {
	        padding: 3px;
	        margin-bottom: 10px;
	        font-weight: bold;
        }

        .clanPage .postbody
        {
	        padding: 3px;
	        color: #ffffff;
        }
        /*BEGIN For clan Forum */
    </style>
    <%}%>


</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />
<div class=clanPage>
    <asp:Panel ID="panelNoMsg" runat="server" Visible="false" Font-Size="1.3em">
        <br /><%= RS("threadNotFound")%>
    </asp:Panel>
    <asp:Panel ID="panelUI" runat="server">
        <div class="sectiontitle">
            <asp:Literal runat="server" ID="lblPageTitle"  />
            </div>
        <p>
        </p>
        <div style="text-align: left; font-weight: bold" class="actions">
            <asp:HyperLink runat="server" ID="lnkNewReply1" NavigateUrl="AddEditPost.aspx?ForumID={0}&ThreadID={1}"><%= isMobile ? RS("Reply"): RS("postReply")%><img src=https://static.realmofempires.com/images/menuItemSeperator.png /></asp:HyperLink>
            
            <asp:LinkButton runat="server" ID="btnCloseThread1" OnClick="btnCloseThread_Click" message = '<%# RS("sureToClose") %>' OnClientClick='return confirm(this.getAttribute("message"));'><%= isMobile ? RS("close"):RS("closeThread")%><img src=https://static.realmofempires.com/images/menuItemSeperator.png /></asp:LinkButton>
            
            <asp:HyperLink runat=server ID="lnkMove" NavigateUrl="MoveThread.aspx?ThreadID={0}" ><% =isMobile ? RS("Move") : RS("MoveThread")%></asp:HyperLink>
        </div>
        <p>
        </p>
        <asp:GridView EnableViewState="false" ID="gvwPosts" runat="server" Width="100%" CellPadding="1"
            CellSpacing="1" GridLines="none" AllowPaging="True" AutoGenerateColumns="False"
            ShowHeader="False" PageSize="25" DataKeyNames="ID" OnRowCommand="gvwPosts_RowCommand"
            OnRowDataBound="gvwPosts_RowDataBound" CssClass="TypicalTable stripeTable" OnPageIndexChanging="gvwPosts_PageIndexChanging"
            PagerSettings-Position="TopAndBottom" Visible=false>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle HorizontalAlign="Center" CssClass="ListPager" />
            <PagerSettings Mode="NumericFirstLast" FirstPageImageUrl="https://static.realmofempires.com/images/LeftArrow.png"
                FirstPageText="First page" LastPageImageUrl="https://static.realmofempires.com/images/RightArrow.png"
                LastPageText="Last page" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <div style="text-align: left;">
                            <asp:HyperLink runat="server" ID="lnkEditPost" ImageUrl="https://static.realmofempires.com/images/item_rename.png"
                                NavigateUrl="~/AddEditPost.aspx?ForumID={0}&ThreadID={1}&PostID={2}" CssClass=action />&nbsp;
                            <asp:ImageButton runat="server" ID="btnDeletePost" ImageUrl="https://static.realmofempires.com/images/cancel.png"
                                message = '<%# RS("sureToDelPost") %>' OnClientClick='return confirm(this.getAttribute("message") + "{0}?");' CssClass=action/>&nbsp;&nbsp;
                        </div>
                        <asp:Literal ID="lblAddedDate" runat="server" Text='<%#Utils.FormatEventTime((DateTime)Eval("AddedDate")) %>' />
                        <br />
                        <%= RS("_by") %>
                        <asp:HyperLink runat="Server" ID="LnkAddedBy" Text='<%# Eval("AddedBy") %>' NavigateUrl='<%#NavigationHelper.PlayerPublicProfileByName((string)Eval("AddedBy")) %>'></asp:HyperLink>
                    </ItemTemplate>
                    <ItemStyle CssClass="TableHeaderRow " Width="120px" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <div class="posttitle">
                            <asp:Literal ID="lblTitle" runat="server" Text='<%# Eval("Title") %>' /></div>
                        <div class="postbody">
                            <asp:Literal ID="lblBody" runat="server" Text='<%# BBCodes.ClanForumBodyToHTML(FbgPlayer.Realm, Utils.ChangeTabBreak( (string)Eval("Body")), getEnv()) %>' /><br />
                            <div style="text-align: right;">
                                <asp:HyperLink runat="server" ID="lnkQuotePost" NavigateUrl="~/AddEditPost.aspx?ForumID={0}&ThreadID={1}&QuotePostID={2}"><%= RS("quotePost")%></asp:HyperLink>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="DataRowNormal" />
            <HeaderStyle CssClass="TableHeaderRow" />
            <RowStyle CssClass="DataRowAlternate" />
        </asp:GridView>
        <asp:GridView EnableViewState="false" ID="gvwPosts_m" runat="server" Width="100%" CellPadding="1"
            CellSpacing="1" GridLines="none" AllowPaging="True" AutoGenerateColumns="False"
            ShowHeader="False" PageSize="25" DataKeyNames="ID" OnRowCommand="gvwPosts_RowCommand"
            OnRowDataBound="gvwPosts_m_RowDataBound" CssClass="TypicalTable stripeTable" OnPageIndexChanging="gvwPosts_PageIndexChanging"
            PagerSettings-Position="TopAndBottom" Visible=false>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle HorizontalAlign="Center" CssClass="ListPager" />
            <PagerSettings Mode="NumericFirstLast" FirstPageImageUrl="https://static.realmofempires.com/images/LeftArrow.png"
                FirstPageText="First page" LastPageImageUrl="https://static.realmofempires.com/images/RightArrow.png"
                LastPageText="Last page" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <div class="TableHeaderRow" style="font-weight:normal;">
                           
                            <div style="display:table-cell; vertical-align:middle;">
                                <asp:Literal ID="lblAddedDate" runat="server" Text='<%#Utils.FormatEventTime((DateTime)Eval("AddedDate")) %>' />
                                <%= RS("_by") %>
                                <%# Eval("AddedBy") %>
                                <div class="posttitle">
                                    <asp:Literal ID="lblTitle" runat="server" Text='<%# Eval("Title") %>' /></div>
                            </div>
                        </div>
                        <div class="postbody">
                            <asp:Literal ID="lblBody" runat="server" Text='<%# BBCodes.ClanForumBodyToHTML(FbgPlayer.Realm, Utils.ChangeTabBreak( (string)Eval("Body"))) %>' /><br />
                            <div style="text-align: right; margin-top: 3px;">
                               <asp:HyperLink runat="server" ID="lnkEditPost"  CssClass="action fontGoldFrLClrg" Text='<%# RS("edit")%>' NavigateUrl="~/AddEditPost.aspx?ForumID={0}&ThreadID={1}&PostID={2}" style="padding-right:15px"/>
                                <asp:LinkButton runat="server" ID="btnDeletePost" CssClass="action fontGoldFrLClrg" Text='<%# RS("delete")%>' ImageUrl="https://static.realmofempires.com/images/cancel.png" message='<%# RS("sureToDelPost") %>' OnClientClick='return confirm(this.getAttribute("message") + "{0}?");'  style="padding-right:15px"/>
                                <asp:HyperLink runat="server" ID="lnkQuotePost" CssClass="action fontGoldFrLClrg"  NavigateUrl="~/AddEditPost.aspx?ForumID={0}&ThreadID={1}&QuotePostID={2}"><% /*RS("quotePost")*/%>Quote </asp:HyperLink>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="DataRowNormal" />
            <HeaderStyle CssClass="TableHeaderRow" />
            <RowStyle CssClass="DataRowAlternate" />
        </asp:GridView>
        <p>
        </p>
        <div style="text-align: left; font-weight: bold" >
            <span class=notMob>
            <asp:HyperLink runat="server" ID="lnkNewReply2" NavigateUrl="AddEditPost.aspx?ForumID={0}&ThreadID={1}"><%= RS("postReply") %></asp:HyperLink>
            <asp:LinkButton runat="server" ID="btnCloseThread2" OnClick="btnCloseThread_Click"
                message = '<%# RS("sureToClose") %>' OnClientClick='return confirm(this.getAttribute("message"));' ><br /><%= RS("closeThread")%></asp:LinkButton>
                </span>
            <asp:Panel runat="server" ID="panClosed" Visible="false">
                <asp:Image runat="server" ID="imgClosed" AlternateText="Thread Closed" ImageUrl="https://static.realmofempires.com/images/LockPost.png" />
                <asp:Label runat="server" ID="lblClosed" Font-Bold="true"><%= RS("threadClosed")%></asp:Label>
            </asp:Panel>
        </div>
        <asp:ObjectDataSource ID="objPosts" runat="server" DeleteMethod="DeletePost" SelectMethod="GetThreadByID"
            SelectCountMethod="GetPostCountByThread" TypeName="MB.TheBeerHouse.BLL.Forums.Post"
            EnablePaging="true">
            <DeleteParameters>
                <asp:Parameter Name="id" Type="Int32" />
            </DeleteParameters>
            <SelectParameters>
                <asp:QueryStringParameter Name="threadPostID" QueryStringField="ID" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    </div>
</asp:Content>
