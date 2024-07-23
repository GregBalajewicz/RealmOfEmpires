<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="Restart.aspx.cs" Inherits="templates_Restart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

<style>

    #popup_Restart {
        width: 320px; 
        height: 480px;   
        background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Defeat.jpg');
        background-size: cover; 
        -webkit-box-shadow: 0px 0px 6px 6px rgba(0,0,0, 0.5);
        box-shadow: 0px 0px 6px 6px rgba(0,0,0, 0.5);  
        left: 0px !important;  
    }
    #popup_Restart .popupBody{
        overflow: hidden;
    }

    #popup_Restart .title {
        border: 2px solid #846824;
        -webkit-border-radius: 15px;
        border-radius: 15px;
        height:22px;
        color: #FFD776;
        font: 20px/1.0em "IM Fell French Canon SC", serif;
        text-align: center;
        margin:8px;
        text-shadow: 0 0 4px #000000;
        background-color:black;
        padding: 2px 3px;
    }   
    #popup_Restart .title:before {
        float: left;
        content: " ";
        margin-left: -9px;
        margin-top: -12px;
        width: 22px;
        height: 31px;
        background: url("https://static.realmofempires.com/images/misc/M_BoxTLGold.png") no-repeat;
    }
    #popup_Restart .titleClose {
        position: absolute;
        left: auto;
        top: -6px;
        right: 1px;
        bottom: auto;
        width: 44px;
        height: 44px;
        background-image: url("https://static.realmofempires.com/images/icons/M_X.png");
        background-repeat: no-repeat;
    }
    #popup_Restart .title .shadedBg {
        width:100%;
        height:100%;
        margin-top:-20px;
        -webkit-border-radius: 15px;
        border-radius: 15px;
        background-image: -webkit-gradient(linear, left top, right top, color-stop(0%, rgba(234, 234, 234, 0.5)), color-stop(100%, rgba(71, 71, 71, 0.5)));
        background-image: -webkit-linear-gradient(left, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
        background-image: -moz-linear-gradient(left, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
        background-image: -ms-linear-gradient(left, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
        background-image: -o-linear-gradient(left, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
        background-image: linear-gradient(to right, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
    }
    #Restart {
        height:434px;
        margin: -8px 20px 0 20px;
        padding: 5px;
        background: rgba(0, 0, 0, 0.75); 
        color: #FFD776;   
        text-align:center;
    }    
    #Restart .desc {
        min-height:18px;
        margin: 14px 0 3px;
        font-size: 11pt;
        text-align: center;
    } 
    #Restart .restartCost, #Restart .playerServants {
        color: white;
    }
    #Restart .bigDesc {
        margin:10px;
        font-size: 13pt;
        text-align: center;
    }   
    #Restart .notEnoughServants
    , #Restart .noRespawns {
        color: red;
        font-weight:bold;
        font: 14px;        
    }
    #Restart .phrases {
        display: none;
    }
    #Restart .hireservant {
        visibility:hidden;
    }
</style>
   
        
    <div class="title">
        <%=RS("Restart") %>
        <div class="shadedBg"></div>
        <div class="titleClose sfx2 pressedEffect"></div>
    </div>

    <div id="Restart">        
        <br />
        <span class="restartTitle bigDesc"><%=RS("RestartTitle")%></span><br />
        <div class="desc"
        <span><%=RS("RestartDesc1")%></span>
        </div><br />
        <div class="desc">
        <span><%=RS("RestartDesc2")%></span><span class="restartCost"></span><span><%=RS("RestartDesc3")%></span>
        </div><br />
        <div class="desc">
        <span><%=RS("RestartDesc4")%></span><span class="playerServants"></span><span><%=RS("RestartDesc3")%></span>
        </div>  
        <div class="desc"> 
        <span class="notEnoughServants ui-helper-hidden"><%=RS("NotEnoughServants")%></span>        
        <span class="noRespawns ui-helper-hidden"><%=RS("noRespawns")%></span>        
        </div> 
        <br />
        <span class="hireservant sfx2 customButtomBG" onclick="ROE.Frame.showBuyCredits();closeMe();"><%=RSc("HireServant") %></span>
        <br /><br />
        <img src="https://static.realmofempires.com/images/misc/M_SpacerBottom.png" >
        <br /><br /><br />
        <div class="restartOK customButtomBG sfx2"><%=RS("OK") %></div>
        <div class="phrases">
            <div ph="1"><%=RS("AreYouSure")%></div>
        </div>
    </div>
</asp:Content>
