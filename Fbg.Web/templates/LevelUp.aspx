<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="LevelUp.aspx.cs" Inherits="templates_BuyInviteResearcher" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
<%if (false) { %><script src="script/roe-api.js" type="text/javascript"></script><%} %>
    <style>
        /*MOVE THESE STYLES TO .css file*/
        
        
        #popup_levelUp
        {
            cursor: pointer;
            margin-bottom: 2px;
            text-align: center;
            padding: 3px;
            vertical-align: middle;
            color: rgb(59, 35, 20);
            background: #e5ddc4; /* Old browsers */
            background: -moz-linear-gradient(top,  #e5ddc4 0%, #c6b699 92%, #958362 95%, #a69b7b 100%); /* FF3.6+ */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#e5ddc4), color-stop(92%,#c6b699), color-stop(95%,#958362), color-stop(100%,#a69b7b)); /* Chrome,Safari4+ */
            background: -webkit-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* Chrome10+,Safari5.1+ */
            background: -o-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* Opera 11.10+ */
            background: -ms-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* IE10+ */
            background: linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e5ddc4', endColorstr='#a69b7b',GradientType=0 ); /* IE6-9 */
            font-size: 12pt;
            margin:10px;
        }

         #popup_levelUp .acceptButton {
            width:130px;
            height:50px;
            display:block;
            background-image:url( https://static.realmofempires.com/images/buttons/accept.png);
            background-repeat:no-repeat;
        }

          #popup_levelUp .acceptButton:hover {
            background-image:url(https://static.realmofempires.com/images/buttons/accept_hover.png);
        }
    </style>
    <div id="popup_levelUp">
        <img src="https://static.realmofempires.com/images/tutorial_advisor.png" style="float: left; height: 150px;" />
        <center>
                    <span style="font-size: 14pt;font:16px/1.0em 'charlemagne_stdbold'">Congratulations!<br />
                    </span>
                </center>
        Thanks to your tremendous progress, the Council of Elders has decided to grant you a new title of <b>%newTitleName%!</b>
        <br />
        <span style="font-size: 13pt;">+ %xp% Global XP <span class="unlockedGiftNewTitle">
            <br />
            <img src='%giftImg_NewTitle%' style="height: 30px" />
            %giftName_NewTitle% item unlocked </span><span class="newRealmNewTitle">
                <br />
                %realm_NewTitle% unlocked</span> </span>
        <br />
        <br />
        Next Title <b>%nextTitleName%</b> <span style="font-size: 13pt;">
            <br />
            + %nextTitle_xp% Global XP <span class="unlockedGiftNextTitle">
                <br />
                Unlocks
                <img src='%giftImg_NextTitle%' style="height: 30px" />
                %giftName_NextTitle% item </span><span class="newRealmNextTitle">
                    <br />
                    Unlocks %realm_NextTitle% </span></span>
        <center>
                    <a  class=acceptButton></a></center>
        <br />
    </div>


    <script>
        function levelUp_CallBack(data) {
            ROE.Frame.reloadFrame();
        }

        function levelUp_Init() {
            $("#popup_levelUp .acceptButton").click(function () {
                ROE.UI.Sounds.clickVictory();
                ROE.Api.call("acceptnewtitle", {}, levelUp_CallBack);

            
                closeMe();
            });
        }

        levelUp_Init();
    </script>
</asp:Content>
