<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="LevelUpNew.aspx.cs" Inherits="templates_BuyInviteResearcher" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
<%if (false) { %><script src="script/roe-api.js" type="text/javascript"></script><%} %>
    <style>
        .simplePopupOverlay .pContainer  {
            background-color: rgba(0, 0, 0, 0.5) !important;
        }
        .simplePopupOverlay .pContainer {
            
        }
        #popup_levelUp {
            text-align:center;
            text-shadow: 0px 2px #000,0px -2px #000,0px 2px #000;
        }
        #popup_levelUp .title {
            color: #e6cd90;
            font: 18px/1.0em "IM Fell French Canon LC", serif;
        }
        #popup_levelUp .titleup {
            color: #d7d7d7;
            font: 18px/2.0em "IM Fell French Canon LC", serif;
        }
        #popup_levelUp .general {
            width:100%;
            height:124px;
            background: url(https://static.realmofempires.com/images/misc/chestScepter.png) no-repeat center;
            margin-bottom: 10px;
        }
            #popup_levelUp .iteminfo {
                margin-top: 3px;
                margin-bottom: 3px;
            }
            #popup_levelUp .itemtext {
                display: inline-block;
                color: #E6CD90;
                font: 13px/1.4em "IM Fell French Canon LC", serif;
                width: 100%;
                margin-bottom: 10px;
                text-align: center;
            }
        #popup_levelUp .itemtext2 {
            display: inline-block;
            color: #e6cd90;
            font: 13px/1.4em "IM Fell French Canon LC", serif;
            width:100%;
            height:50px;
            text-align:center;
        }
        #popup_levelUp .itemtext2 IMG {
                height: 50px;
            }
        #popup_levelUp .itemstore {
            height:44px;
            color: #e6cd90;
            font: 13px/1.0em "IM Fell French Canon LC", serif;
            margin-top: 10px;
        }
        #popup_levelUp .realminfo,#popup_levelUp .questclaim,#popup_levelUp .globalxp {
            color: #e6cd90;
            font: 13px/2.0em "IM Fell French Canon LC", serif;
            text-align:center;
            margin: 3px 0px;
        }

    </style>

    <div id="popup_levelUp">
        
        <div class="title">Congratulations!</div>

        <div class="titleup">You are now a %newTitleName%.</div>

        <div class="general"></div>
        
        <div class="iteminfo">
                <span class="itemtext">You have Unlocked the following items.</span>
                <span class="itemtext2"></span>
                <div class="itemstore">Go to <img src="https://static.realmofempires.com/images/icons/m_gifts.png" class="sfx2 ButtonTouch" /> to get your new items!</div>
        </div>
        
        <div class="realminfo">
            You've gained access to new realms.
        </div>
        
        <div class="globalxp">
        </div>

        <div class="questclaim">
            <span>Have you claimed the quest reward?</span>
            <img src="https://static.realmofempires.com/images/icons/tutorialGuyIcon.png" />
        </div>

    </div>



</asp:Content>
