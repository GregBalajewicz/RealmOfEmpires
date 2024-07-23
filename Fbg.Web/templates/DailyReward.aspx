<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="DailyReward.aspx.cs" Inherits="templates_DailyReward" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

<style>

    #popup_DailyReward {
        width: 320px; 
        height: 480px;   
        background: url('https://static.realmofempires.com/images/backgrounds/M_DailyRewardBG.png');
        background-size: cover; 
        -webkit-box-shadow: 0px 0px 6px 6px rgba(0,0,0, 0.5);
        box-shadow: 0px 0px 6px 6px rgba(0,0,0, 0.5);    
    }
        #popup_DailyReward .title {
            position: relative;
            z-index: 1;
            border: 2px solid #846824;
            border-radius: 15px;
            height: 30px;
            width: 100%;
            color: #FFD776;
            font: 20px/1.0em "IM Fell French Canon SC", serif;
            text-align: center;
            margin: 4px;
            box-sizing: border-box;
            text-shadow: 0 0 4px #000;
            background-color: #000;
            padding: 2px 3px;
        }
        #popup_DailyReward.popupDialogs  .title {
            display:none;
        }
    #popup_DailyReward .title:before {
        float: left;
        content: " ";
        margin-left: -9px;
        margin-top: -12px;
        width: 22px;
        height: 31px;
        background: url("https://static.realmofempires.com/images/misc/M_BoxTLGold.png") no-repeat;
    }
    #popup_DailyReward .titleClose {
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
    #popup_DailyReward .title .shadedBg {
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
        #popup_DailyReward.popupDialogs #RewardSelection {
            margin: 0px;
        }
    #RewardSelection {
        position: relative;
        overflow: hidden;
        margin: -8px 0px 0px 0px;
        color: #FFD776;
        text-align: center;
    }
        #RewardSelection .rewardContentHead {
            position: relative;
            overflow: hidden;
            margin-bottom: 10px;
            min-height: 70px;
            background: rgba(0, 0, 0, 0.75);
        }

            #RewardSelection .rewardContentHead .fortuneBadge {
                position: absolute;
                top: 5px;
                left: 0px;
                height: 60px;
                width: 94px;
                background-image: url(https://static.realmofempires.com/images/icons/SP_waxSeal.png);
            }

            #RewardSelection .rewardContentHead .desc,
            #RewardSelection .rewardContentHead .rewardDays {
                position: relative;
                overflow: hidden;
                margin-left: 99px;
                text-align: left;
            }
            #RewardSelection .rewardContentHead .desc {
                  margin-top: 5px;
            }
            #RewardSelection .rewardContentHead .rewardDays {
                margin-top: 5px;
                margin-bottom: 5px;
            }
      
        #RewardSelection .rewardContentBody {
            background: rgba(0, 0, 0, 0.75);
        }

    #RewardSelection .rewardList {
        display:inline-block; 
        list-style-type: none; 
        padding:0; 
        margin:0; 
    }
    #RewardSelection .rewardListItem {         
        cursor:pointer;       
        width: 80px;
        height: 80px;
        position: relative;        

        text-align:center;
        -webkit-perspective: 800px;
        -moz-perspective: 800px;
        -o-perspective: 800px;
        perspective: 800px;
    }
    #RewardSelection .rewardTypeImg { 
        width: 77px;
        height: 60px;  
        background: url('https://static.realmofempires.com/images/icons/M_ClosedChest.png') center no-repeat;    
        }
    #RewardSelection .rewardOK, #RewardSelection .phrases , #RewardSelection .inventory  {
        display: none;
        }
    #RewardSelection .rewardTypeDesc  {
        color: white;
        margin-top: -6px;
        text-align: center;
        display: block;
        font: 13px/1.0em serif;
        height: 25px;        
    }
    #RewardSelection .greyscale { 
        /*
        filter: grayscale(100%);
        -webkit-filter: grayscale(100%);
        -moz-filter: grayscale(100%);
        filter: grayscale(100%);
        */
        opacity:0.45;
        filter:alpha(opacity=45);
    }
    #RewardSelection .rewardType {
        width: 100%;
        height: 100%;
        position: absolute;   

        -webkit-transition: -webkit-transform 1s;
        -moz-transition: -moz-transform 1s;
        -o-transition: -o-transform 1s;
        transition: transform 1s;

        -webkit-transform-style: preserve-3d;
        -moz-transform-style: preserve-3d;
        -o-transform-style: preserve-3d;
        transform-style: preserve-3d;
    }
    #RewardSelection .rewardType .figure {
        display: block;
        position: absolute;
        width: 100%;
        height: 100%;

        backface-visibility: hidden;
        -webkit-backface-visibility: hidden;
        -moz-backface-visibility: hidden;
        -o-backface-visibility: hidden;
    }
    #RewardSelection .cardfront {
        /*background-color:#846824;*/
    }
    #RewardSelection .cardback {
        /*background-color:#FFD776;*/
        -webkit-transform: rotateY( 180deg );
        -moz-transform: rotateY( 180deg );
        -o-transform: rotateY( 180deg );
        transform: rotateY( 180deg );
    
        -webkit-transition: opacity 1s ease-in;
        -moz-transition: opacity 1s ease-in;
        -ms-transition: opacity 1s ease-in;
        -o-transition: opacity 1s ease-in;
        transition: opacity 1s ease-in;
    }
    #RewardSelection .cardback .rewardTypeDesc{
        color:white;
        font: 12px/1.0em serif;
        margin-top:-5px;
    }
    #RewardSelection .rewardType.flipped {    
        -webkit-transform: rotateY( 180deg );
        -moz-transform: rotateY( 180deg );
        -o-transform: rotateY( 180deg );
        transform: rotateY( 180deg );
    }
    #RewardSelection  TABLE {
        margin: 0 auto;
    }
        #RewardSelection .rewardOK {
            margin-top: 1px;
            float: left;
            margin-left: 31px;
            min-width:0px;
        }
        #RewardSelection .inventory {
            float: right;
            margin-top: 1px;
            margin-right: 31px;
        }

        #RewardSelection .notAvail {
            padding: 10px 40px;
        }

    /*fixes IE issue by reflipping the desc*/
    #RewardSelection .flipped .rewardTypeDesc.ieFixDescFlip{
        transform: rotateY( 180deg );
    }

</style>
   
        
    <div class="title">
        King's Fortune
        <div class="shadedBg"></div>
        <div class="titleClose sfx2 pressedEffect"></div>
    </div>

 <div id="RewardSelection">  
       
     <div class="rewardContentHead">
        <div class="fortuneBadge"></div>
        <div class="rewardDesc desc fontGoldFrLCXlrg"><%=RS("TapChest") %></div>
        <div class="rewardDays fontSilverFrLClrg"><%=RS("TodayBonus") %></div>
     </div>

     <div class="rewardContentBody">

        <img src="https://static.realmofempires.com/images/misc/M_SpacerBottom.png" />

        <table class="rewardTypes" cellspacing="10px" cellpadding="0"></table>
        <div class="notAvail rewardDesc desc fontGoldFrLClrg" style="display:none;"></div>

        <img src="https://static.realmofempires.com/images/misc/M_SpacerTop.png" />

        <div>
            <div class="rewardOK customButtomBG sfx2"><%=RS("OK") %></div>
            <div class="inventory customButtomBG sfx2"><%=RS("MyInventory") %> <img src='https://static.realmofempires.com/images/icons/giftsS.png' /></div>   
        </div>

     </div>
    

    <div class="phrases">
        <div ph="1"><%=RS("Congratulations") %></div>
        <div ph="2"><%=RS("YouGotItems") %></div>
        <div ph="3"><%=RS("ChooseItem") %></div>
        <div ph="4"><%=RS("ComeBackTomorrow") %></div>
        <div ph="5"><%=RS("DontMissADay") %></div>
    </div>

</div>

</asp:Content>
