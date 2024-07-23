<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="Chat.aspx.cs" Inherits="Chat" %>

<asp:Content ID="Chat" ContentPlaceHolderID="cph1" runat="Server">
    <div class="BtnBLg1 fontButton1L chatBar chatButton unselectable">
        <div class="barName">%chatName%</div>
        <span class="notification"></span>
        <div class="chatButton close"></div>
    </div>

    <div class="chatWindow">
        <div class="userArea">
            <div class="fontSilverFrLCXlrg title">Users</div>
            <div class="users"></div>
        </div>     
            
        <div class="chatArea">
            <div class="titleBar">
                <div class="fontSilverFrLClrg handle chatName">%chatName%</div>
                <div class="chatButton minimize">-</div>
                <div class="chatButton toggleUsers">users</div>
                <!--<div class="chatButton close">x</div>-->
            </div>

            <div class="messages"></div>

            <div class="fontSilverFrLClrg newMessages">New messages<div class="icon"></div></div>
            <div class="chatInput">
                <input type="text" class="textbox" maxlength="256" placeholder="Type a message..."/>
                <input type="button" class="send" value="Send" />
            </div>
        </div>
    </div>
</asp:Content>