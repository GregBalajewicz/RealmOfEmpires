<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="InOutTroopsFilter.aspx.cs" Inherits="templates_InOutTroopsFilter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">    
    <span class="templ_inOutFilter">
        <div style=" padding: 2px; text-align: center;">
            <div class="toggle attack clickable on sfx2" data-rel="attack" data-state="on"></div>
            <div class="toggle support clickable on sfx2" data-rel="support" data-state="on"></div>
            <div class="toggle returning clickable on sfx2" data-rel="returning" data-state="on"></div>
            <div class="toggle hidden clickable sfx2" data-rel="hidden" data-state="off"></div>
            <!--
            <div class="toggle attack clickable on sfx2" data-rel="attack" data-state="on"><span class=checkmark></span><span class=icon><div class="troopIconSheet att sfx2"></div></span><span> <%=RS ("Attack") %></span></div>
            <div class="toggle support clickable on sfx2" data-rel="support" data-state="on"><span class=checkmark></span><span class=icon><div class="troopIconSheet sup sfx2"></div></span><span> <%=RS ("Support") %></span></div>
            <div class="toggle returning clickable on sfx2" data-rel="returning" data-state="on"><span class=checkmark></span><span class=icon><div class="troopIconSheet ret sfx2"></div></span><span> <%=RS ("ReturningTroops") %></span></div>
            <div class="toggle hidden clickable sfx2" data-rel="hidden" data-state="off"><span class=checkmark></span><span class=icon><div class="troopIconSheet hide-hidden sfx2"></div></span><span><%=RS ("HiddenCommands") %></span></div>
            -->
        </div>
        <span class="htmlResources">
            <img src="https://static.realmofempires.com/images/troopIconSheet.png" /></span>


        <script><%=IsInDesignMode ? "ROE.Api.apiLocationPrefix = '../';" : "" %></script>

         <div class="phrases">
            <div ph="support_on"><%=RS("Showingsupportcommands") %></div>
            <div ph="support_off"><%=RS("Hidingsupportcommands") %></div>
            <div ph="attack_on"><%=RS("Showingattackcommands") %></div>
            <div ph="attack_off"><%=RS("Hidingattackcommands") %></div>
            <div ph="returning_on"><%=RS("Showingreturningcommands") %></div>
            <div ph="returning_off"><%=RS("Hidingreturningcommands") %></div>
            <div ph="hidden_on"><%=RS("Showinghiddencommands") %></div>
            <div ph="hidden_off"><%=RS("Hidinghiddencommands") %></div>
        </div>
    </span>
</asp:Content>
