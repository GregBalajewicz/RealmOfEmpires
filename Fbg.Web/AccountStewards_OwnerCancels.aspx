<%@ Page Language="C#" MasterPageFile="~/master_PopupFullFunct.master" AutoEventWireup="true" CodeFile="AccountStewards_OwnerCancels.aspx.cs" Inherits="AccountStewards_DeactivateBeforeLogin" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
             <% if(isD2) { %>
    <style>
        body {
            font-family: Georgia, 'Playfair Display';
            font-size: 12px;
        }

        html {
            background-color: #000000;
            background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg') no-repeat center center fixed;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
        }

        .TDContent {
            background-color: rgba(6, 20, 31, 0.9) !important;
            height: 100%;
            position: absolute;
            overflow: auto;
            color:#fff;
        }

        .BoxHeader {
            color: #FFFFFF;
            font-size: 14px !important;
            font-weight: normal;
            background-color: rgba(49, 81, 108, 0.7) !important;
            padding: 4px;
        }

        .d2_inputbutton {
           
            color: #FFFFFF;
            font-weight: initial;
            padding: 4px !important;
            padding-bottom: 3px !important;
            background-color: #181819 !important;
            -moz-box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            -webkit-box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            border: 1px solid #A69D85;
            -moz-border-radius: 6px;
            -webkit-border-radius: 6px;
            border-radius: 6px;
            font-size: 12px !important;
            font-family: Georgia, 'Playfair Display';
            height: initial;
            margin: 4px 0px;
            text-decoration:none;
            display: block;
            width: 120px;
        }

        .d2_inputbutton:hover {
             text-decoration:none !important;
             cursor:pointer;
        }

        .d2_boxedContent {
            padding:6px !important;
        }

        .d2_tableWrapper {
            border-spacing:0 !important;
        }

        .d2_tableWrapper td {
            padding: 0px;
            padding-bottom: 6px;
        }

      
        .d2_border {
           
            border:none !important;
        }

        .d2_stripeRow {
            background-color: rgba(88, 140, 173, 0.1);
        }

        .d2_stripeRowCol {
            padding:0;
        }

        .d2_container {
            font-size:14px;
        }
    </style>
    <% } %>
    <div style="font-size: 1.3em">
        <center>
            <% if(!isD2) { %>
    <img src='<%=RSic("Logo")%>' />
            <% } else { %>
            <br />
            <br />
            <% } %>
        <br />
        Account's owner cancelled the stewardship and you have been logged out. <br />
        On behalf of this player - thank you!<br /><br />
        
        <a href="LogoutOfRealm.aspx" class="d2_inputbutton">OK</a>        
    </div>
    </center>
</asp:Content>
