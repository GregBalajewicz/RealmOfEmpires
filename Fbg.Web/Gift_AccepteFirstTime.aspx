<%@ Page Language="C#" MasterPageFile="~/master_PopupFullFunct.master" EnableEventValidation="true"
    AutoEventWireup="true" CodeFile="Gift_AccepteFirstTime.aspx.cs" Inherits="Gift_AccepteFirstTime"
    ValidateRequest="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <span style="font-size: 1.6em;">
        <center>
            <%= RS("congrats")%>
            <br />
            <br />
            <div style="width: 500px;">
                <asp:Image runat="server" ID="imgGift" Style="float: left" />
                <%= RS("acceptedGift")%>
                <br />
                <br />
                <asp:HyperLink ID="LinkReturnFavour" runat="server" NavigateUrl="~/Gift_send.aspx"><B><%= RS("link_returnFavor")%></B></asp:HyperLink>
                <br />
                <br />
                <%= RS("beGenerous")%>
    </span>
    <br />
    <br />
    <asp:HyperLink ID="HyperLink1" Style="font-size: 1.3em" runat="server" NavigateUrl="villageoverview.aspx"><%= RS("link_NotNow")%></asp:HyperLink>
    </div> </center>
</asp:Content>
