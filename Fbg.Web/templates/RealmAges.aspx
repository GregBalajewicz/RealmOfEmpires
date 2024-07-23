<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="RealmAges.aspx.cs" Inherits="templates_RealmAges" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

<style>
    .pContainer {
        background-color: rgba(0, 0, 0, 0.5) !important;
    }
    .realmAgeImg {        
        margin: 24px 10px 0 10px;
    }    
    .realmAgeTitle {
        color:#ffd776;
        font: 16px/20px "IM Fell French Canon SC", serif;
        text-shadow: 1px 1px 4px #000000;
        text-align: center;
    }
    .realmAgeInfo {
        color:#b2b2b2;
        font: 14px/18px "IM Fell French Canon Pro", serif;
        text-shadow: 1px 1px 4px #000000;
    }
    .realmAgeDate {
        color:white;
        font: 12px/20px "IM Fell French Canon Pro", serif;
        text-shadow: 1px 1px 4px #000000;
    }
    .realmAgeBox {
        display: table-cell;
        height: 74px;
        vertical-align: top;
    }
    .ageSeparator {
        width: 100%;
        height: 18px;
        background: url("https://static.realmofempires.com/images/misc/M_ListBar2.png") no-repeat center;
    }
    .realmAgeImgCurrent {
        margin: 0px 2px 0 0 !important;
    }
</style>
            
    <DIV class="AgeSheet" data-ageid="%age.id%"   >
        <div class="realmAgeBox" >
            <img class="realmAgeImg %age.currentimg%" src="%age.img%" ></img>
        </div>
        <div class="realmAgeBox" >
            <div class="realmAgeTitle">%age.title%</div>
            <div class="realmAgeInfo">%age.info%</div>
            <div class="realmAgeDate">%age.date%</div>
        </div>
        <div class="realmAgeProgress"></div>                     
    </DIV>
       

</asp:Content>
