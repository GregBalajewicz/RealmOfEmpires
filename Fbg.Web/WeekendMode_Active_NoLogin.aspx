<%@ Page Language="C#" MasterPageFile="~/master_PopupFullFunct.master" AutoEventWireup="true" CodeFile="WeekendMode_Active_NoLogin.aspx.cs" Inherits="WeekendMode_Active_NoLogin" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <style>
        html {
            background: url('https://static.realmofempires.com/images/forum/BGcolor6b6670.jpg') !important;
            background-repeat: no-repeat !important;
            background-size: 100% auto !important;
            background-color: #6B6670!important;
        }
        .content {
            position: absolute;
            top: 10%;
            width: 300px;
            left: 50%;
            margin-left: -150px;
            padding: 15px;
            box-sizing: border-box;
            background-color: rgba(0, 0, 0, 0.8);
        }
            .content .section {
                text-align: center;
                padding: 10px;
            }
                .content .section.dates div {
                    color: #FFF;
                    margin-bottom: 3px;
                }

                .content .section.warn div {
                    color: #FF3030;
                }
    </style>
    
    <div class="content">
        <div class="section title">PLAYER WEEKEND MODE ACTIVE ON THIS REALM</div>

        <div class="section dates">
            <div><asp:Label ID="dateTakesEffectOn" runat="server" /></div>
            <div><asp:Label ID="dateEndsOn" runat="server" /></div>
        </div>

        <div class="section warn">
            <div><asp:Label ID="warn1" runat="server" /></div>
        </div>

        <div class="section">
            <asp:LinkButton ID="LinkButton1" CssClass="actionButton" runat="server" onclick="LinkButton1_Click">Login and Cancel Weekend Mode</asp:LinkButton>
        </div>

        <div class="section">
            <a href="LogoutOfRealm.aspx" class="actionButton">Back to realm selection</a> 
        </div>

    </div>
    

</asp:Content>
