<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="ClanPublicProfile.aspx.cs" Inherits="ClanPublicProfile" Title="Public Profile" ValidateRequest="false" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="c2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <%if (isMobile) { %>
    <style>
        #ctl00_ContentPlaceHolder1_HyperLink6 {display:none;}
        #ctl00_ContentPlaceHolder1_lbl_PublicProfile {
            overflow: hidden;
            display: block;
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
        
        .clanPage, .clanPage .clanProfile
        {
            width: 100%;
        }
        .column
        {
            display: inline;
            width: 100%;
        }
        font {
            background-size: 320px auto !important;
            max-width: 320px !important;
        }
    </style>
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
        .clanPage, .clanPage .clanProfile
        {
            width: 100%;

        }

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
    <%=Utils.GetIframePopupHeaderForNotPopupBrowser(RS("PublicProfile") , (isMobile & !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/M_Clan.png")%>
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />
    <div class="clanPage">
        <asp:Label ID="lblPageExplanationToMemebers" runat="server" Text='<%# RS("pubProfInfo")%>' Visible="False"></asp:Label>
        <br />
        <asp:Panel ID="pnl_Profile" runat="server" CssClass="clanProfile">
            <div class="column">
                <div class="TableHeaderRow">
                    <asp:Label ID="lbl_ClanName" runat="server" style="font-size:12pt" Text="Label" />
                </div>
                <table border="0" cellpadding="0" cellspacing="1">
                    <tr>
                        <td class="TableHeaderRow">
                            <%= RS("points")%>
                        </td>
                        <td>
                            <asp:Label ID="lbl_points" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableHeaderRow">
                            <%= RS("rank")%>
                        </td>
                        <td>
                            <asp:Label ID="LblRank" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableHeaderRow" nowrap>
                            <%= RS("numOfMembers")%>
                        </td>
                        <td>
                            <asp:Label ID="lbl_Members" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                        </td>
                    </tr>
                    <tr runat="server" id="seeAllMembers">
                        <td colspan="2">
                            <asp:HyperLink ID="lnk_Members" runat="server"><%= RS("seeAllMemLnk")%></asp:HyperLink>
                        </td>
                    </tr>
                    <% if(isD2 || isMobile) { %>
                    <tr>                        
                        <td colspan="2">
                            <asp:Label ID="lbl_clanRequestInvite" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                    <% } %>
                </table>
            </div>
            <div class="column">
                <div class="TableHeaderRow"  style="font-size:12pt">
                    <%= RS("publicMsg")%>
                    <asp:LinkButton ID="lnk_Edit" runat="server" Font-Bold="false" OnClick="lnk_Edit_Click" Visible="false" class="sfx2">
                    <%= RS("editLnkBtn")%>
                    </asp:LinkButton>
                </div>
                <asp:Label ID="lbl_PublicProfile" runat="server"></asp:Label>
                <asp:Panel ID="pnl_PublicProfile" runat="server" Visible="false">
                    <asp:TextBox ID="txt_PublicProfile" runat="server" TextMode="MultiLine" Height="160px" Width="100%" CssClass="TextBox"></asp:TextBox>
                    <br />
                    <asp:Button ID="btn_Save" runat="server" CssClass="inputbutton sfx2" Text='<%# RS("saveBtn")%>' OnClick="btn_Save_Click" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="https://roe.fogbugz.com/default.asp?W365" Target="_blank">
                        <%= RS("bbHelpLnk")%>
                    </asp:HyperLink>
                    <br />
                    <br />
                    <br />
                    <%= RS("hints")%>
                    </ul>
                    <div class="jaxHide" id="div_invalidChars" rel='<%# RS("invCharsHelp")%>' runat="server">
                        <%= RS("charsToBeRemoved")%>
                        <ul>
                            <li>
                                <%= RS("anyHTMLEle")%></li>
                            <li>
                                <%= RS("lessThan")%>
                                - <b>&lt</b> - <span style="color: #B5B4B4">
                                    <%= RS("typeInstead")%></span></li>
                            <li>
                                <%= RS("dblSlashes")%>
                                <b>\\</b></li>
                        </ul>
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>
        <asp:Label ID="lbl_Message" runat="server" Text='<%# RS("noClan")%>' Visible="False"></asp:Label>
    </div>
</asp:Content>
