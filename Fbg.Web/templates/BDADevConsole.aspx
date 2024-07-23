<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="BDADevConsole.aspx.cs" Inherits="templates_BDADevConsole" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    <style>
        #BDAConsoleWin {color: black; background-color:white;}
        #BDAConsoleWin .logentry.error {color:black;}
        #BDAConsoleWin .button {border:solid 1px green; cursor:pointer; height:30px; display :table-cell; vertical-align:middle; font-weight:bolder; padding:3px;}
    </style>
     <div id=BDAConsoleWin style='clear:both;'>
        <span class='button generateError' >test- generate error && try-catch</span>
        <span class='button generateError2' >test- generate error no try-catch</span>
        <span class='button clear' >clear</span>
        <div class=logcontent></div>
     </div>

    <script><%=IsInDesignMode ? "ROE.Api.apiLocationPrefix = '../';" : "" %></script>
</asp:Content>
