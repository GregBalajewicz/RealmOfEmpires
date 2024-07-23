<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="BonusVillageChange.aspx.cs" Inherits="templates_BonusVillageChange" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <div id="BonusVillageSelection">
        <div class="title">
            <span><%=RS("BonusVillageChange") %></span>
            <img class="closeX pressedEffect sfx2" src="https://static.realmofempires.com/images/icons/M_X.png" />
        </div>
        <div class="currentBonusTypes">
            <img class="icon" src="https://static.realmofempires.com/images/map/VillBsilver3a.png"></img>
            <div>
                <div><%=RS("CurrentBonusVillage") %></div>
                <div class="villName"></div>
            </div>
        </div>

        <div class="bonusswitch"><%=RS("ConvertToFollowings") %></div>
        <div class="bonusList">
            <br>
            <br>
            <%=RS("loading") %></div>

        <div class="phrases">
            <div ph="0"><%=RS("FeatureNotThisRealm") %></div>
            <div ph="1"><%=RS("SavingSelection") %></div>
            <div ph="2"><%=RS("servants") %></div>
            <div ph="3"><%=RS("BonusChnaged") %></div>
            <div ph="4"><%=RS("ConvertCost") %></div>
            <div ph="5"><%=RS("Convert") %></div>
            <div ph="6"><%=RS("ErrorConvert") %></div>
            <div ph="7"><%=RS("NotEnoughServants") %></div>
            <div ph="8"><%=RS("BONUSVILLAGE") %></div>
            <div ph="9"><%=RS("NextChnage") %></div>
            <div ph="10"><%=RS("HireServants") %></div>
            <div ph="11"><%=RS("Silver") %></div>
            <div ph="12"><%=RS("Barrack") %></div>
            <div ph="13"><%=RS("Recruit") %></div>
            <div ph="14"><%=RS("Farm") %></div>
            <div ph="15"><%=RS("Wall") %></div>
            <div ph="16"><%=RS("Stable") %></div>
            <div ph="17"><%=RS("Tavern") %></div>
        </div>
    </div>





</asp:Content>
