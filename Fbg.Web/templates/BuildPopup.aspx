<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="BuildPopup.aspx.cs" Inherits="templates_BuildPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    
    <STYLE>
        .popupDialogs .BuildPopup {
            position: absolute;
            top: 0px;
            bottom: 0px;
            left: 0px;
            right: 0px;
            overflow: auto;
        }
        .BuildPopup #buildpagePopup {
            background-color: rgba(0, 0, 0, 0.8);
        }
        .popupDialogs .BuildPopup #buildpagePopup {
            background-color: rgba(0, 0, 0, 0.5);
        }
        .BuildPopup .separator {
            width: 100%;
            text-align: center;
            margin: -6px 0;
        }
        .BuildPopup .phrases  {
            display: none;
        }
        .BuildPopup .buildBox {
            color: #FFD776;
            font: 16px/1.0em "Georgia", serif;
            margin:0;
            padding:1px 8px;
        }
        .BuildPopup .noteAboutQuickbuild {
            background-color: rgba(0, 0, 0, 0.4);
            margin: 0;
            padding: 12px 8px;
            margin-bottom: 10px;
            text-align: center;
        }
        .BuildPopup .buildBox IMG {            
            padding-right:6px;
        }
        .BuildPopup .buildButton ,.BuildPopup .buildDetailButton {
            float:right;
            margin: 9px 36px 0 0;
            cursor:pointer;
        }
        .BuildPopup .buildButton DIV,.BuildPopup .buildDetailButton DIV{
            margin-top: -6px;
        }
        .BuildPopup .buildName {
            display: inline-block;
            width:40%;
            height: 22px;
        }
        .BuildPopup .buildingDetailPanel {
            position: relative;
            width: 100%;
            font-size: 11px;
            color: white;
            height: 0;
            overflow: hidden;
            transition: height .5s;
            -webkit-transition: height .5s;
        }
        .BuildPopup .buildingDetailPanel.infoOpen {
            height: 36px;
        }
        .BuildPopup .buildDetailInfo {
            display: inline-block;
            height:18px;
            width:43%;
            padding: 0 0 0 20px;
            float: left;
            background: url("https://static.realmofempires.com/images/misc/M_SmNotEnough.png") left no-repeat;
        }
        .customButtomBG2 {
            position: relative;
            display: inline-block;
            min-width: 70px;
            height: 42px;
            color: #FFD886;
            font: 18px/1.0em "IM Fell French Canon SC", serif;
            text-shadow: 
                0px 3px 3px #000000, 
                0px -3px 3px #000000,
                -4px 0px 3px #000000,
                4px 0px 3px #000000;
            text-align: center;
            line-height: 40px;
            margin-top: -14px;
            background: url('https://static.realmofempires.com/images/buttons/M_BtnL_P.png') no-repeat center;
        }
        .customButtomBG2:before {
            width: 36px;
            height: 42px;
            background: url('https://static.realmofempires.com/images/buttons/M_Btn2_L.png') no-repeat center;
            float: left;
            content: " ";
            margin-left: -26px;
            }
        .customButtomBG2:after {
            width: 37px;
            height: 42px;
            background: url('https://static.realmofempires.com/images/buttons/M_Btn2_R.png') no-repeat center;
            float: right;
            content: " ";
            margin-right: -37px;
            }

        .BuildPopup .customButtomBG:after {
            width: 36px;
            margin-right: -36px;
            background: url('https://static.realmofempires.com/images/buttons/M_Btn2c_R.png') no-repeat center;
        }
        .customButtomBG2:hover {    
            top: 1px;
            left: 1px;
        }
    </STYLE>

    <%if (isMobile) { %>
    <div id="background">
        <img src="https://static.realmofempires.com/images/misc/M_BG_Treasury.jpg" class="stretch" alt="" />
    </div>
    <%} %>
     
    <div id="buildpagePopup" >                    
    </div>
   
    <div class="phrases">
        <div ph="1"><%=RS("Build") %></div>
        <div ph="2"><%=RS("Details") %></div>
        <div ph="3"><%=RS("level") %></div>
        <div ph="4"><%=RS("AllBuilt") %></div>
        <div ph="noteAboutQuickbuild"><%=RS("noteAboutQuickbuild") %></div>
     </div>
    
</asp:Content>