<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="AddEditPost.aspx.cs"
    Inherits="AddEditPost" ValidateRequest="false" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="c2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
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
        
        .clanPage .txtBody
        {
            height:100px;
        }
    </style>
    <script>
   
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

    <%} else { %>
    <style>
        .clanPage .txtBody
        {
            height:200px;
        }
      
    </style>
    <%}%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
<div class=clanPage>
    <br />
    <asp:Panel ID="panelNoMsg" runat="server" Visible="false" Font-Size="1.3em">
        <br />
        <%= RS("threadNotFound")%>
    </asp:Panel>
    <asp:Panel ID="panelUI" runat="server">
        <div style="font-weight: bold;">
            <asp:Literal runat="server" ID="lblNewThread" Text='<%# RS("createNewThread")%>'
                Visible="false" />
            <asp:Literal runat="server" ID="lblNewReply" Text='<%# RS("replyThread")%>' Visible="false" />
            <asp:Literal runat="server" ID="lblEditPost" Text='<%# RS("editPost")%>' Visible="false" />
        </div>
        <%if (!isMobile){ %><p></p>   <%} %>
        <asp:Panel runat="server" ID="panInput">
            <asp:Panel runat="server" ID="panTitle">
                <%if (!isMobile){ %>
                <b>
                    <%= RS("subject")%>
                </b>
                <br />
                <%} %>
                <asp:TextBox ID="txtTitle" runat="server" Width="95%" MaxLength="256" CssClass="TextBox" /></br>
                <asp:RequiredFieldValidator ID="valRequireTitle" runat="server" ControlToValidate="txtTitle"
                    SetFocusOnError="true" Text='<%# RS("titleFieldReq")%>' ToolTip='<%# RS("titleFieldReq")%>'
                    Display="Dynamic" />
                
            </asp:Panel>
             <%if (!isMobile){ %>
            <b>
                <%= RS("body")%>
            </b>
            <br />
            <%} %>
            <asp:TextBox ID="txtBody" CssClass="TextBox txtBody" runat="server" Width="95%"
                TextMode="MultiLine"></asp:TextBox>
            <p>
            </p>
            <p>
                <asp:CheckBox ID="chkClosed" runat="server" Text='<%# RS("noRepliesToThread")%>' />
            </p>
            <p>
                <asp:CheckBox ID="chk_Sticky" runat="server" Text='<%# RS("stickyThread")%>' Visible="False" />
            </p>
            <asp:Button CssClass="inputbutton" runat="server" ID="btnSubmit" Text='<%# RS("postBtn")%>'
                OnClick="btnSubmit_Click" Width="89px" />
        </asp:Panel>
        &nbsp;
        <asp:Panel runat="server" ID="panFeedback" Visible="false">
            <asp:Label runat="server" ID="lblConfirmation" SkinID="FeedbackOK"><%= RS("postSubmitted")%>
            </asp:Label>
        </asp:Panel>
        <asp:Panel runat="server" ID="panApprovalRequired" Visible="false">
            <asp:Label runat="server" ID="lblApprovalRequired" SkinID="FeedbackOK"><%= RS("postModerated")%></asp:Label>
        </asp:Panel>
        <asp:Panel runat="server" ID="panReplayClosed" Visible="false">
            <asp:Label runat="server" ID="lblReplayClosed" SkinID="FeedbackOK"><%= RS("noReplyClosed")%></asp:Label>
        </asp:Panel>
        <br />
        <asp:HyperLink runat="server" ID="lnkThreadList" NavigateUrl="BrowseThreads.aspx?ForumID={0}"
            Visible="False" CssClass="StandoutLink">
        <%= RS("backToThreadList")%>
        </asp:HyperLink>
        <asp:HyperLink runat="server" ID="lnkThreadPage" NavigateUrl="ShowThread.aspx?ID={0}">
        <%= RS("backToThreadPg")%>
        </asp:HyperLink>
        <br />
        <asp:HyperLink Style="font-weight: normal;" ID="linkBBCodes" runat="server" NavigateUrl="https://roe.fogbugz.com/default.asp?W365"
            Target="_blank">
        <%= isMobile ? "" : RS("bbCodesActive")%>
        </asp:HyperLink>
    </asp:Panel>
</div>
</asp:Content>
