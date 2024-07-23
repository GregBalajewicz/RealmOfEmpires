<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanSettings.aspx.cs" Inherits="ClanSettings" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
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
    <%}       %>



    <%if (isMobile)
      { %>
    <style>
        .Popup a {
            text-decoration: none;
            font: 14px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
        }

        .TDContent table tr {
            background-color: rgba(0,0,0,.8);
        }

            .TDContent table tr td {
                padding-bottom: 5px;
                padding-left: 5px;
                font-size: 11px !important;
            }

        .TableHeaderRow {
            background-image: -webkit-linear-gradient(top, rgba(35, 35, 35, 0.3) 0%, rgba(108, 108, 108, 0) 100%);
            color: #FFF;
            font-weight: bold;
            padding: 1px;
        }

        .inputbutton, .inputsubmit {
            height: 40px;
            width: 110px !important;
            text-align: center;
            box-sizing: border-box;
            cursor: pointer;
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -50px;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            font: 15px/1.0em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 2px 2px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
        }

        .TDContent {
            position: absolute;
            left: 0px;
            right: 0px;
            top: 0px;
            bottom: 0px;
            overflow: auto;
        }

        .line {
            margin-top: 10px;
            margin-left: 10px;
        }



        .sectionDividerA {
            height: 8px;
            background-image: url('https://static.realmofempires.com/images/misc/M_SP_UIMisc.png');
            background-repeat: no-repeat;
            background-position: 0px 0px;
        }
        .notclaimed {
            color:gray;
        }
    </style>
  
   
    <%}       %>







    <%if (isMobile)
      { %>

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
    

    <%}%>























</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />

    <table class="TypicalTable stripeTable" cellspacing="1">
        <tr class="highlight">
            <td class="TableHeaderRow">
                <%= RS("defaultRoles") %>
            </td>
            <td>
                <asp:CheckBox ID="cbRole_Inviter" runat="server" Text='<%# RS("inviter")%>' Font-Bold="True" /><br />
                <%= RS("whenChecked")%><br />
            </td>
        </tr>
    </table>
    <br />
    Claim System Settings
    <table class="TypicalTable stripeTable" cellspacing="1">
        <tr class="highlight">

            <td class="TableHeaderRow">Who can see all claim details                      
            </td>
            <td>
                <asp:DropDownList ID="cmb_SecurityLevel" runat="server" AppendDataBoundItems="true">
                    <asp:ListItem Selected="True" Value="0">All clan members</asp:ListItem>
                    <asp:ListItem Value="1">Owners & Admins</asp:ListItem>
                    <asp:ListItem Value="2">Owners to Diplomats</asp:ListItem>
                    <asp:ListItem Value="3">Owners to Forum Admins</asp:ListItem>
                    <asp:ListItem Value="4">Owners to Inviters</asp:ListItem>
                </asp:DropDownList>
                <br /> <font color="gray">if you limit who can see all claim details, those who are not of the required role, can only see their own claims, and that a village was claimed by other clan member on the map but not by whom it was claimed.                </font> 
            </td>
            
        </tr>
        <tr class="highlight">
            <td class="TableHeaderRow">Maximum number of claims per player</td>
            <td>
                <asp:TextBox ID="txtMaxClaims" type="number" min="1" max="99999" Text="99999" runat="server"></asp:TextBox>

            </td>
            <td></td>
        </tr>
        <tr class="highlight">
            <td class="TableHeaderRow">Claim Expiry</td>
            <td>
                <asp:TextBox ID="txtClaimExpiry" type="number" min="1" max="99999" Text="99999" runat="server"></asp:TextBox>
                hours
                                    <br />
                   <font color="gray">the amount of time (in hours) until a claim expires, from the moment it was made</font>

            </td>
            
        </tr>
        <tr class="highlight">
            <td class="TableHeaderRow">Share with Allied clans?</td>
            <td>
                <asp:RadioButton runat="server" ID="rbsharewithallies_y" GroupName="sharewithallies" Text="Yes" />
                <asp:RadioButton runat="server" ID="rbsharewithallies_n" GroupName="sharewithallies" Text="No" />
                <br /><font color="gray"> if yes, clans that you allied with and that are allied with you, will be able to see which villages your clanmates claimed</font>
            </td>
          
        </tr>
        <tr class="highlight">
            <td class="TableHeaderRow">Share with NAP clans?</td>
            <td>
                <asp:RadioButton runat="server" ID="rbsharewithNap_yes" GroupName="sharewithNap" Text="Yes" />
                <asp:RadioButton runat="server" ID="rbsharewithNap_no" GroupName="sharewithNap" Text="No" />
                <br /><font color="gray">if yes, clans that you have a NAP with, that are NAP (or allied) with you, will be able to see which villages your clanmates claimed</font>
            </td>
          
        </tr>

    </table>
    <br />
    <asp:Button ID="lnk_Update" runat="server" CssClass="inputbutton sfx2" OnClick="lnk_Update_Click" Text='<%# RS("save")%>' />
    
    <%if (isMobile) { %>
        <!-- padding to allow easier M key input -->
        <div style="margin-bottom:150px;"></div>
    <%} %>

</asp:Content>
