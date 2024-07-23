<%@ Page Language="C#" MasterPageFile="~/master_PopupFullFunct.master" AutoEventWireup="true" CodeFile="Quests.aspx.cs" Inherits="Quests" Title="Untitled Page" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="Controls/Quests.ascx" TagName="Quests" TagPrefix="uc1" %>
<%@ Register Src="Controls/Quests2.ascx" TagName="Quests2" TagPrefix="q2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
    <style type="text/css">
        div.quests
        {
            font-size: 14px;
        }
        .quests div.ach_hdr
        {
            line-height: 28px;
            height: 28px;
            text-align:center;
        }
        .quests .questName
        {
            color: #e59f25;
            font-size: 14pt;
            width: 100%;
            display: block;
            text-decoration: none;
        }               
       
        
        .quests .rewardBox
        {
            width:125px;
            height:90px;           
            background-image: url( 'https://static.realmofempires.com/images/reward_box.png' );
            background-repeat:no-repeat;
        }
        .quests .rewardBox img
        {
            padding-left:7px;
            padding-right:2px;
        }        
        .quests .riimg
        {
            float:left;padding-right:2px;vertical-align: middle;
        }    
        .quests .questlist 
        {
            vertical-align: top;
            white-space:nowrap
        }

        .questCompleted .questDescription
        ,  .questCompleted .questDescription img {
            opacity:0.5;
        }

        .troopType,
        .spellType,
        .silverRewardIcon,
        .servantRewardIcon
        
        {
            height: 35px;
            vertical-align: middle;
        }


        .researchIcon {
            position: relative;
            float: left;
            vertical-align: middle;
            width: 100px;
            height: 100px;
            border-radius: 15px;
            background-size: 1022px 3470px;
            background-image: url("https://static.realmofempires.com/images/REsearch/allResearch.jpg");
            margin-right: 10px;
            border: 2px outset #CEC598;
        }

        #pageReloader {
            display: none;
            position: fixed;
            bottom: 10px;
            right: 10px;
            z-index: 1000;
            animation: pulsate 2s ease-out infinite;
            -webkit-animation: pulsate 2s ease-out infinite;
            -moz-animation: pulsate 2s ease-out infinite;
            -o-animation: pulsate 2s ease-out infinite;
        }
            #pageReloader:hover {
                -webkit-animation: none;
            }
            #pageReloader .icon {
                background-image: url('https://static.realmofempires.com/images/buttons/M_RefreshSm.png');
                position: absolute;
                top: 0px;
                left: 0px;
                right: 0px;
                bottom: 0px;
                background-size: 100% 100%;
                background-repeat: no-repeat;
                border: 2px solid rgba(57, 199, 183, 0.8);
                border-radius: 40px;
            }
            @-webkit-keyframes pulsate {
            0% {
                -webkit-transform: scale(0.1, 0.1);
                opacity: 0.0;
            }

            50% {
                opacity: 1.0;
            }

            80% {
                opacity: 1.0;
            }

            100% {
                -webkit-transform: scale(1.2, 1.2);
                opacity: 0.0;
            }
        }
        @keyframes pulsate {
            0% {
                -webkit-transform: scale(0.1, 0.1);
                opacity: 0.0;
            }

            50% {
                opacity: 1.0;
            }

            80% {
                opacity: 1.0;
            }

            100% {
                -webkit-transform: scale(1.2, 1.2);
                opacity: 0.0;
            }
        }
    </style>
            
    <%if (!mainMasterPage.isMobile) {  %>      
        <style>
           
            .quests div.questDetails {height:550px;overflow:auto; }
            .quests td.questDetails { vertical-align: top;}
            .quests .questName:hover
            {
                background-color: #22170d;
                text-decoration: none;
            }
            .quests .questNameSelected
            {
                background-color: #22170d;
            }
            .quests .getreward
        {
            display: block;
            width: 190px;
            height: 50px;
            background-image: url( 'https://static.realmofempires.com/images/buttons/Get_reward.png' );
        }
        .quests .getreward:hover
        {
            background-image: url( 'https://static.realmofempires.com/images/buttons/Get_reward_hover.png' );
        }
                    

        </style>
    <% } else  { %>
        <style>
              body { font-family: Georgia, 'Playfair Display'; font-size:12px; }
            a, a:active, a:link { color: #d3b16f; }   
            a:hover { color: #d3b16f; }
              .quests .rewardBox {
                background: none !important;
                width: 150px;
            }
            .quests .questDetails
            {
                display: none;
            }
            .quests .vdiv
            {
                display: none;
            }
            .quests img.hdiv
            {
                display: none;
            }
            .quests .questList td
            {
                padding-bottom: 5px;
            }
            .questDetails .goal
            {
                font: 16px/1.0em "charlemagne_stdbold";
                margin-bottom:10px;
            }
            .quests .themeM-more {
                position: absolute;
            }
            .quests .themeM-more > .fg > .arrow-l {
                left: 3px;
            }
            .quests .ClaimError,.quests .Error {
                display:block;
                background:rgba(0,0,0,0.7);
                color:red;
                padding:6px;
            }


            div.quests {
                position: relative;
                width: 100%;
                height: 100%;
                overflow: hidden;
            }
            .questsTable {
                position: relative;
                width: 100%;
                height: 100%;
                overflow: auto;
            }

              .highlightedD2 {
            color: #FFFFFF;
            font-weight: initial;
            padding: 4px !important;
              background: rgba(255,255,255,0.7) !important;
            text-align: left;
           
        }

              .d2_rowQuestTitle {
            display: block;
            float: left;
            margin-left: 4px;
        }

        .d2_rowQuestGoal {
            display: block;
            float: left;
            clear: left;
            font: 15px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
            margin-left: 24px;        

        }
         .highlightedD2 .d2_rowQuestTitle {
              color: black;
              text-shadow: none;
              font-weight: bold;
        }

         .highlightedD2 .d2_rowQuestGoal {

                color: black;
              text-shadow: none;
         }

        .d2_rowQuestStatusIcon {

            display:block;
            float:left;
        }

         .quests .getreward
        {
            display: block;
            width: 181px;
            height: 46px;
            background-image: url( 'https://static.realmofempires.com/images/d2_Get_reward.png' );
            background-repeat: no-repeat;
        }
        .quests .getreward:hover
        {
            background-image: url( 'https://static.realmofempires.com/images/d2_Get_reward.png' );
        }
        .quests .getreward:active
        {
            background-image: url( 'https://static.realmofempires.com/images/d2_Get_reward_hover.png' );
        }


        </style>
    <% } %>

     <% if(isD2) { %>
    <style>
        html {
            background-color:#000000;
            background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Research.jpg') no-repeat center center fixed; 
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
        }
        body { font-family: Georgia, 'Playfair Display'; font-size:12px; }
        a, a:active, a:link { color: #d3b16f; }   
        a:hover { color: #d3b16f; }

         .TDContent {
            /*background-color: rgba(0, 0, 0, 0.7) !important;
            background-color: rgba(8, 15, 20, 0.51) !important;*/
            background-color: rgba(6, 20, 31, 0.71) !important;
            height: 100%;
            position: absolute;
            overflow: auto;
        }

         .quests .questName {
            color: #ffffff;
            /* font-size: 14pt; */
            width: 100%;
            display: block;
            text-decoration: none;
            font: 19px/1.0em "IM Fell French Canon SC", serif !important;
            text-shadow: 0px 0px 1px #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000;
            padding: 1px 0px;
            overflow: auto;
        }

        .highlightedD2 {
            color: #FFFFFF;
            font-weight: initial;
            padding: 4px !important;
          
       
       /*    background: -moz-linear-gradient(left,  rgba(60,217,200,0) 0%, rgba(60,217,200,0.5) 33%, rgba(60,217,200,0.5) 66%, rgba(60,217,200,0) 100%); 
            background: -webkit-gradient(linear, left top, right top, color-stop(0%,rgba(60,217,200,0)), color-stop(33%,rgba(60,217,200,0.5)), color-stop(66%,rgba(60,217,200,0.5)), color-stop(100%,rgba(60,217,200,0))); 
            background: -webkit-linear-gradient(left,  rgba(60,217,200,0) 0%,rgba(60,217,200,0.5) 33%,rgba(60,217,200,0.5) 66%,rgba(60,217,200,0) 100%); 
            background: -o-linear-gradient(left,  rgba(60,217,200,0) 0%,rgba(60,217,200,0.5) 33%,rgba(60,217,200,0.5) 66%,rgba(60,217,200,0) 100%);
            background: -ms-linear-gradient(left,  rgba(60,217,200,0) 0%,rgba(60,217,200,0.5) 33%,rgba(60,217,200,0.5) 66%,rgba(60,217,200,0) 100%); 
            background: linear-gradient(to right,  rgba(60,217,200,0) 0%,rgba(60,217,200,0.5) 33%,rgba(60,217,200,0.5) 66%,rgba(60,217,200,0) 100%);
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#003cd9c8', endColorstr='#003cd9c8',GradientType=1 ); 
        */ 
         background: rgba(255,255,255,0.7) !important;
            text-align: left;
           
        }

         .quests .questName:hover
            {
                background:none !important;
                text-decoration: underline;
            }
            .quests .questNameSelected
            {
                /*background-color:#ffff00;*/
            }


        .questlist td {
            padding:4px;
        }

        .quests .rewardBox {
            background:none !important;
            width:150px;
        }



        .questDetails {
            color: #d3b16f;
        }


        .quests .getreward
        {
            display: block;
            width: 181px;
            height: 46px;
            background-image: url( 'https://static.realmofempires.com/images/d2_Get_reward.png' );
            background-repeat: no-repeat;
        }
        .quests .getreward:hover
        {
            background-image: url( 'https://static.realmofempires.com/images/d2_Get_reward.png' );
        }
        .quests .getreward:active
        {
            background-image: url( 'https://static.realmofempires.com/images/d2_Get_reward_hover.png' );
        }

        /* Added into the html for easy re-styling */
        .d2_questTitle {
            /* https://static.realmofempires.com/images/d2_quest.png */
        }

        .questListsWrapper {
            height: 312px;
            width: 300px;
            overflow: auto;
            /*position: relative;*/
            padding-bottom: 238px;
            overflow-x:hidden;

        }

        #cph1_DetailsView1 {
            margin-bottom:20px;
        }

        .quests .riimg {            
            padding-right: 8px;       
        }

        .questAdvisor {
            background-image:url('https://static.realmofempires.com/images/misc/M_AdvisorSm.png');
            position:absolute;
            bottom:0px;
            left:0px;
            width:115px;
            height:238px;
        }

        .TextBox {
            width: 40px;
            margin: 2px;
            margin-left: 6px;
            margin-top: 6px;
            padding: 4px;
            height: 20px;
            width: 80px;
            border-radius: 6px;
            font: 14px/1.0em "IM Fell French Canon", serif;
            color: #242424;
            border: 1px solid #616161;
        }


        .d2_rowQuestTitle {
            display: block;
            float: left;
            margin-left: 4px;
        }

        .d2_rowQuestGoal {
            display: block;
            float: left;
            clear: left;
            font: 15px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
            margin-left: 24px;        

        }

         .highlightedD2 .d2_rowQuestTitle {
              color: black;
              text-shadow: none;
              font-weight: bold;
        }

         .highlightedD2 .d2_rowQuestGoal {

                color: black;
              text-shadow: none;
         }

        .d2_rowQuestStatusIcon {

            display:block;
            float:left;
        }

        .researchLink {
            color:#ffffff !important;

        }

        .questResearchImg {
            vertical-align: middle;
        }

    </style>
    <% } %>


    <script>
        
        
        function ShowHideDetails(showDetails) {
            if (showDetails) {
                $('.questDetails').show();
                $('.questlist').hide();
            } else {
                $('.questDetails').hide();
                $('.questlist').show();
            }
        }
    </script>

    <% if (isMobile) { %>
    <script>
        
        function researchLink_onClick(event) {
            event.preventDefault();
            var riid = $(this).attr("data-riid");
            <% if (!IsiFramePopupsBrowser) { %>
                window.opener.ROE.Research.showResearchPopupBySpecificItem(riid);
                window.close();
                window.opener.$("#imgIframeClose").click();
            <%} else {%>                           
                parent.ROE.Research.showResearchPopupBySpecificItem(riid);
                //parent.$('.IFrameDivTitle .action.close').click(); //this was nuking the research popup as well
                parent.$('#popupIframe_popup .IFrameDivTitle .action.close').click(); //more surgical, nukes quest iframe popup, leaves the freshly opened research alone
            <%} %>
        }


        function CustomOnLoad() {

            <% if (!IsiFramePopupsBrowser) { %>
            //$('.quests-startTutorial').click(function () { opener.ROE.Tutorial.stepset(); opener.BDA.Database.LocalSet('TutorialStep', 0); opener.ROE.Tutorial.recover(); $('.IFrameDivTitle .action.close').click(); return false; });
            $('.quests-startTutorial').click(function () {
                opener.BDA.Database.LocalSet('TutorialStep', 0);
                opener.$.cookie('TutorialStart', 'true');
                opener.location.reload();
            });
            <%} else {%>
            //$('.quests-startTutorial').click(function () { parent.ROE.Tutorial.stepset(); parent.BDA.Database.LocalSet('TutorialStep', 0); parent.ROE.Tutorial.recover(); parent.$('.IFrameDivTitle .action.close').click(); return false; });
            $('.quests-startTutorial').click(function () {
                parent.BDA.Database.LocalSet('TutorialStep', 0);
                parent.$.cookie('TutorialStart', 'true');
                parent.location.reload();
            });
            <%} %>
        }

        function removeButton(element) {
            $(element).remove(); //remove btn to prevent multiple clicks
        }

        function refreshCoins(data) {
            $('#pageReloader').hide();
            <% if (!IsiFramePopupsBrowser) { %>
            //window.opener.ROE.Frame.questRewardAccepted_ReloadUI();
            window.opener.ROE.Frame.questRewardAcceptCB(data);
            <%} else {%>
            //window.parent.ROE.Frame.questRewardAccepted_ReloadUI();
            window.parent.ROE.Frame.questRewardAcceptCB(data);
            <%} %>
        }

    </script>
    <% }
       else if (isD2) {%>
        <script>    
            function researchLink_onClick(event) {           
                event.preventDefault();
                var riid = $(this).attr("data-riid");          
                parent.ROE.Research.showResearchPopupBySpecificItem(riid);
            }

            function removeButton(element) {
                $(element).remove(); //remove btn to prevent multiple clicks
            }

            //maybe lets call this questRewardAcceptClick() or something similar
            function refreshCoins(data) {
                $('#pageReloader').hide();
                window.parent.ROE.Frame.questRewardAcceptCB(data);
            }

            function CustomOnLoad() {
                $('.quests-startTutorial').click(function () {
                    //parent.ROE.Tutorial.stepset();
                    parent.BDA.Database.LocalSet('TutorialStep', 0);
                    //parent.ROE.Tutorial.recover();
                    //parent.$('.ui-dialog[aria-describedby=questsDialog] .ui-dialog-titlebar-close').click(); //hack. this will cloase the quest dialog. ugly but works for now
                    $.cookie('TutorialStart', 'true');
                    parent.location.reload();
                    return false;
                });
            }

        </script>
    <%} %>


    <% if (isMobile || isD2) { %>
    <script>
        
        //document.ready commented out because it doesn't fire in Amazon Apps. 
        //Its ok, its a delegate anyway, doesnt need elements to be loaded.

        //$(document).ready(function () {
            
            //$(document).delegate(".researchLink", "click", researchLink_onClick);
        $(document).delegate("a[href*='?riid=']", "click", researchLink_onClick);
        $(document).delegate(".questName", "click", function () { $('#pageReloader').hide(); });

        
        //});
    </script>
    <% } %>

    


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
<%=Utils.GetIframePopupHeaderForNotPopupBrowser(RS("QuestTitle") , (isMobile) && !IsiFramePopupsBrowser,"https://static.realmofempires.com/images/icons/M_Quests.png")%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="quests">
                
                <% if (!isMobile && !isD2) { %>
                <center style="font-size: 24px;">
                    <% if (isD2) { %>
                    <img class="d2_questTitle" src="https://static.realmofempires.com/images/d2_quest.png" />
                    <%} else {%>
                    <img src="https://static.realmofempires.com/images/quest.png" />
                    <% } %>
                    <asp:HyperLink ID="linkClose" Visible="false" runat="server" CssClass="Cancel" NavigateUrl="." OnClick="return reloadParent(this);" Text="Click here to close this window when done!" />
                    <img class=hdiv src="https://static.realmofempires.com/images/quests_horizontal_devider.png" />
                </center>
                <%} %>
                <table class="questsTable" width="100%">
                    <tr>
                        <td class=questlist nowrap width="10%" >
                            <% if(isD2) { %>
                           <div class="questAdvisor"></div>
                           <div class="questListsWrapper">
                              
                            <%}%>
                            
                            <asp:Panel ID="pnlCompletedQuests" runat="server" Visible="false" CssClass="completed">
                                <% if (!isMobile) { %>
                                <div class="ach_hdr">
                                     <% if (isD2) { %>
                                    <img class="d2_ d2_completedQuests" src="https://static.realmofempires.com/images/d2_Completed_Quests.png" />
                                    <%} else {%>
                                    <img class="d2_" src="https://static.realmofempires.com/images/Completed_Quests.png" />
                                    <% } %>
                                                             
                                </div>
                                <%} else {%>
                                <div class="themeM-panel style-more">
                                    <div class="bg">
                                        <div class="corner-tl">
                                        </div>
                                        <div class="corner-br">
                                        </div>
                                    </div>
                                    <div class="fg clearfix">
                                        <div class="themeM-panel style-link ">
                                            <div class="bg">
                                                <div class="corner-br">
                                                </div>
                                            </div>
                                            <div class="fg">
                                                <div class="label">
                                                    <%=RS("CompletedQuest") %>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="clearfix" style="padding:5px">
                                <%} %>



                                <div>
                                  
                                    <asp:DataList RepeatDirection="Horizontal" RepeatColumns="1" ID="dlClaimRewardList" runat="server" CellPadding="0" CellSpacing="0" GridLines="none" OnSelectedIndexChanged="dvSubjectTypes_SelectedIndexChanged" SelectedItemStyle-CssClass="highlightedD2" SelectedItemStyle-BackColor="#22170d" CssClass="questList" Width="100%">
                                    
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="row" CommandArgument='<% #Eval("ID")%>' OnCommand="LinkButtonCompletedQuests_Command" CssClass="questName sfx2">
                                                <% if(isD2 || isMobile) { %>
                                                <img src="https://static.realmofempires.com/images/d2_CheckMark_Quests.png" style="height: 20px;" border="0" class="d2_rowQuestStatusIcon" />
                                                <asp:Label ID="Label5" runat="server" Text='<%#Eval("Title") %>' Style="" CssClass="d2_rowQuestTitle" />
                                                <asp:Label ID="Label8" runat="server" Text='<%#Eval("Goal") %>' Style="" CssClass="d2_rowQuestGoal" />
                                                <% } else { %>
                                                <img src="https://static.realmofempires.com/images/CheckMark_Quests.png" style="height: 20px;" border="0" />
                                                 <asp:Label ID="Label9" runat="server" Text='<%#Eval("Title") %>' Style="" />
                                                
                                                 <%}%>
                                              
                                            </asp:LinkButton></ItemTemplate><ItemStyle Wrap=false />
                                    </asp:DataList>
                                 
                                </div>
                                

                                <% if (isMobile) { %>
                                        </div>
                                    </div>
                                </div>
                                <%} else { %>
                                   <br />
                                <%}%>
                            </asp:Panel>
                            
                            <% if (!isMobile) { %>
                                <div  class="ach_hdr">
                                    <% if (isD2) { %>
                                    <img class="d2_nextSteps" src="https://static.realmofempires.com/images/d2_Recommended_nextsteps.png" />
                                    <%} else {%>
                                    <img src="https://static.realmofempires.com/images/Recommended_nextsteps.png" />
                                    <% } %>
                                </div>
                            <%} else {%>
                                <div class="themeM-panel style-more">
                                    <div class="bg">
                                        <div class="corner-tl">
                                        </div>
                                        <div class="corner-br">
                                        </div>
                                    </div>
                                    <div class="fg clearfix">
                                        <div class="themeM-panel style-link ">
                                            <div class="bg">
                                                <div class="corner-br">
                                                </div>
                                            </div>
                                            <div class="fg">
                                                <div class="label">
                                                    <%=RS("Recommended") %></div></div></div><div class="clearfix" style="padding:5px">

                            <%} %>
                            <div>
                                <asp:LinkButton ID="buttonBrainQuests" runat="server" OnClick="buttonBrainQuests_Click" CssClass='questName sfx2'><img src="https://static.realmofempires.com/images/light_bulb.png" border="0" style="float:left;"  /><%=RS("MasteryQuests") %><BR /><span style="font-size:11pt;">&nbsp;&nbsp;&nbsp;&nbsp;<%=RS("Genius") %></span></asp:LinkButton><asp:DataList RepeatDirection="Horizontal" RepeatColumns="1" ID="dvRecommendedQuests" runat="server" CellPadding="0" CellSpacing="0" GridLines="none" OnSelectedIndexChanged="dvSubjectTypes_SelectedIndexChanged" DataKeyField="ID" DataMember="ID" SelectedItemStyle-BackColor="#22170d" Width="100%" CssClass="sfx2 questList" >
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="row" CommandArgument='<% #Eval("ID")%>' OnCommand="LinkButtonRecommendedQuest_Command" class='questName sxf2'>
                                             <% if (isD2) { %>
                                                <img src="https://static.realmofempires.com/images/icons/M_Quests.png" style="float: left; height: 20px;" border="0"  />
                                            <% } else { %>
                                                <img src="https://static.realmofempires.com/images/questIcon.png" style="float: left; height: 20px;" border="0"  />
                                            <% } %>
                                            <asp:Label ID="Label3" runat="server" Text='<%#Eval("Title") %>' CssClass="d2_rowQuestTitle" />
                                            
                                            <% if(isD2 || isMobile) { %>
                                                <asp:Label ID="Label8" runat="server" Text='<%#Eval("Goal") %>' CssClass="d2_rowQuestGoal" />
                                            <% } %>
                                            
                                        </asp:LinkButton></ItemTemplate><ItemStyle />
                                </asp:DataList>
                                <br />
                            </div>

                                <% if (isMobile) { %>
                                        </div>
                                    </div>
                                </div>
                                <%} else { %>
                                   <br />
                                <%}%>
                            <center>
                                <asp:Label Font-Size="Large" ForeColor="Green" ID="lblMore" runat="server" Text=""></asp:Label><br /></center></td><td width="2px" valign="top" class=vdiv>
                            <img src="https://static.realmofempires.com/images/quests_vertical_devider.png" />
                        </td>
                            <% if(isD2) { %>
                            </div>
                            <%}%>
                        <td class=questDetails >
                        <div class=questDetails >
                            <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="false" GridLines="None" >
                                <Fields>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Panel runat="server" ID="pn" Style="font-size: 14pt;"  CssClass='<%#(bool)Eval("IsCompleted") ? "questCompleted" : "" %>' >
                                            <% if (isMobile) { %>
                                                <div class="themeM-panel style-link linkBack BarTouch sfx2"  id=hideDetails onclick="ShowHideDetails(false); return false">
                                                    <div class="bg">
                                                        <div class="corner-br"></div>
                                                    </div>

                                                    <div class="fg">
                                                        <div class="themeM-more">
                                                            <div class="bg">
                                                                <div class="corner-tl"></div>
                                                            </div>

                                                            <div class="fg">
                                                                <div class="label">
                                                                    <span></span><br>
                                                                </div>

                                                                <div class="arrow-l"></div>
                                                            </div>
                                                        </div>

                                                        <div class="label">
                                                            <span><%=RS("Back") %></span><br>
                                                        </div>
                                                    </div>
                                                </div>                                             
                                                <div class=goal><% if (isD2 || isMobile)
                                                                   { %><asp:Image ID="Image7" runat="server" src="https://static.realmofempires.com/images/d2_CheckMark_Quests.png" Style="height: 20px;" Visible='<%#(bool)Eval("IsCompleted") %>' /><% } else { %><asp:Image ID="Image1" runat="server" src="https://static.realmofempires.com/images/CheckMark_Quests.png" Style="height: 20px;" Visible='<%#(bool)Eval("IsCompleted") %>' /> <%}%><%#Eval("Goal") %></div>
                                            <%} else { %>
                                                 <% if (isD2 || isMobile)
                                                    { %>
                                                 <img src="https://static.realmofempires.com/images/d2_goal.png" style="vertical-align:middle; margin-bottom: 7px;" />
                                                <asp:Image ID="Image9" runat="server" src="https://static.realmofempires.com/images/d2_CheckMark_Quests.png" Style="height: 20px;" Visible='<%#(bool)Eval("IsCompleted") %>' />
                                                <% } else { %>
                                                <img src="https://static.realmofempires.com/images/goal.png" style="vertical-align:middle;" />
                                                <asp:Image ID="Image8" runat="server" src="https://static.realmofempires.com/images/CheckMark_Quests.png" Style="height: 20px;" Visible='<%#(bool)Eval("IsCompleted") %>' />
                                                 <%}%>    
                                                
                                                <asp:Label ID="Label2" runat="server" Text='<%#Eval("Goal") %>' />
                                                <br />
                                                <br />
                                            <%}%>                                                
                                                <span style="font-size: 12pt;" class="questDescription">
                                                    <%#isD2 ? Eval("DescriptionD2") : ( isMobile ? Eval("DescriptionM") : Eval("Description") ) %></span>
                                                <br />
                                                <br />
                                                <div style="clear: both;">
                                                    <center>


                                                        <div style="margin-bottom: 8px;">
                                                            <% if (isD2 || isMobile)
                                                               { %>
                                                            <img src="https://static.realmofempires.com/images/d2_Reward_.png" />
                                                            <% }
                                                               else
                                                               { %>
                                                            <img src="https://static.realmofempires.com/images/Reward_.png" />
                                                            <%}%>
                                                        </div>

                                                        <asp:Panel ID="Td1" runat="server" Visible='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Silver > 0  %>' >
                                                            <% if (isD2 || isMobile)
                                                               { %>
                                                            <asp:Image ID="Image5" CssClass="silverRewardIcon" runat="server" src="https://static.realmofempires.com/images/gifts/d2_Gift_sack_of_silver.png" />
                                                            <% }
                                                               else
                                                               { %>
                                                            <asp:Image ID="Image3" Style="vertical-align: middle; height: 50px; float: left;" runat="server" src="https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png" /><% } %>
                                                            <asp:Label ID="Label7" runat="server" Text='<%#Utils.FormatCost(((Fbg.Bll.Player.QuestReward)Eval("Reward")).Silver) + " Silver"%>'></asp:Label>
                                                        </asp:Panel>
                                                        <asp:Panel runat="server" Visible='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Credits > 0  %>'>

                                                            <% if (isD2 || isMobile)
                                                               { %>
                                                            <asp:Image ID="Image6" CssClass="servantRewardIcon" runat="server" src="https://static.realmofempires.com/images/M_BuyProductID_200servants.png" />
                                                            <% }
                                                               else
                                                               { %>
                                                            <asp:Image ID="Image4" Style="vertical-align: middle; height: 50px; float: left;" runat="server" src="https://static.realmofempires.com/images/tutorial_advisor2.png" /><% } %>
                                                            <asp:Label ID="Label6" runat="server" Text='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Credits.ToString() + " Servants"%>'></asp:Label>
                                                            
                                                        </asp:Panel>

                                                        <asp:Panel runat="server" Visible='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops.Count > 0%>'>
                                                            <asp:Image ID="Label10" CssClass="troopType" runat="server" ImageUrl='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops.Count > 0 ? ((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops[0].UnitType.IconUrl_ThemeM : ""%>' Style="vertical-align: middle;"></asp:Image>
                                                            <asp:Label ID="Label4" runat="server" Text='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops.Count > 0 ? ((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops[0].UnitType.Name + " x " + ((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops[0].Amount.ToString() : ""%>'></asp:Label>
                                                        </asp:Panel>
                                                        <asp:Panel runat="server" Visible='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops.Count > 1%>'>
                                                            <asp:Image ID="Image2" CssClass="troopType" runat="server" ImageUrl='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops.Count > 1 ? ((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops[1].UnitType.IconUrl_ThemeM : ""%>' Style="vertical-align: middle;"></asp:Image>
                                                            <asp:Label ID="Label11" runat="server" Text='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops.Count > 1 ? ((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops[1].UnitType.Name + " x " + ((Fbg.Bll.Player.QuestReward)Eval("Reward")).Troops[1].Amount.ToString() : ""%>'></asp:Label>
                                                        </asp:Panel>
                                                        <asp:Panel ID="Panel1" runat="server" Visible='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration.Count > 0%>'>
                                                            <asp:Image ID="Image10" CssClass="spellType" runat="server" ImageUrl='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration.Count > 0 ? MiscHtmlHelper.PremiumFeaturePackageIcon_Large(((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration[0].PFPackageID, true) : ""%>' Style="vertical-align: middle;"></asp:Image>
                                                            <asp:Label ID="Label12" runat="server" Text='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration.Count > 0 ? Fbg.Bll.CONSTS.PF_NameForPackage(((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration[0].PFPackageID) + " for  " + Utils.FormatDuration_Long2(new TimeSpan(0, ((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration[0].DurationInMinutes,0)) : ""%>'></asp:Label>
                                                        </asp:Panel>
                                                        <asp:Panel ID="Panel2" runat="server" Visible='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration.Count > 1%>'>
                                                            <asp:Image ID="Image11" CssClass="spellType" runat="server" ImageUrl='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration.Count > 1 ? MiscHtmlHelper.PremiumFeaturePackageIcon_Large(((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration[1].PFPackageID, true) : ""%>' Style="vertical-align: middle;"></asp:Image>
                                                            <asp:Label ID="Label13" runat="server" Text='<%#((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration.Count > 1 ? Fbg.Bll.CONSTS.PF_NameForPackage(((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration[1].PFPackageID) + " for  " + Utils.FormatDuration_Long2(new TimeSpan(0, ((Fbg.Bll.Player.QuestReward)Eval("Reward")).PFWithDuration[1].DurationInMinutes,0)) : ""%>'></asp:Label>
                                                        </asp:Panel>
                                               
                                               

                                                    <asp:LinkButton ID="btnAcceptReward" runat="server" OnCommand="Button1_Command" CommandArgument='<%# Eval("ID")%>' Visible='<%#(bool)Eval("IsCompleted") %>' CssClass="sfx2 getreward" onclientclick="removeButton(this);"  />
                                                </center>
                                                     </div>
                                            </asp:Panel>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                            <center><asp:Label Font-Size="Large" ID="Label1" runat="server" Text="" style="color:#00FF33;"></asp:Label></center><asp:Label ID="lblClainError" CssClass="ClaimError" runat="server" Text="" Visible="false"></asp:Label><asp:Panel runat="server" ID="pnOldQuests" Style="font-size: 12pt;">
                                            <% if (isMobile) { %>
                                                <div class="themeM-panel style-link linkBack sfx2"  id=hideDetails onclick="ShowHideDetails(false); return false">
                                                    <div class="bg">
                                                        <div class="corner-br"></div>
                                                    </div>

                                                    <div class="fg">
                                                        <div class="themeM-more">
                                                            <div class="bg">
                                                                <div class="corner-tl"></div>
                                                            </div>

                                                            <div class="fg">
                                                                <div class="label">
                                                                    <span></span><br>
                                                                </div>

                                                                <div class="arrow-l"></div>
                                                            </div>
                                                        </div>

                                                        <div class="label">
                                                            <span><%=RS("Back") %></span><br>
                                                        </div>
                                                    </div>
                                                </div>                                             
                                            <%} %>

                                <uc1:Quests ID="Quests1" runat="server" />
                                <q2:Quests2 ID="Q2" runat="server" />
                            </asp:Panel>
                        </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <div class="preload">
        <img src='https://static.realmofempires.com/images/buttons/Get_reward_hover.png' />
    </div>
     <% if(!isMobile) { %>
    <cc1:UpdatePanelAnimationExtender ID="UpdatePanelAnimationExtender1" runat="server" TargetControlID="UpdatePanel1" BehaviorID="animationUpgrade">
       
    <Animations>
            <OnUpdating>
                <Sequence>
                    <Parallel duration="0.2" Fps="30">
                        <FadeOut AnimationTarget="animUpgrade" minimumOpacity="0.5"></FadeOut>
                    </Parallel>
                </Sequence>
            </OnUpdating>
            <OnUpdated>
                <Sequence>
                    <Parallel duration="0.2" Fps="30">
                        <FadeIn AnimationTarget="animUpgrade" minimumOpacity="0.5"></FadeIn>
                    </Parallel>
                </Sequence>
            </OnUpdated>
        </Animations>

    </cc1:UpdatePanelAnimationExtender>
     <%}  %>
    <% if(isD2) { %>
    <script type="text/javascript">
        $('.getreward').click(function () {


            window.parent.ROE.UI.Sounds.click();

        });

        // NOTE: Tried to implement a scroll to position for when you click on a quest,
        // but can't because of the server post back code.
       
    </script>
    <%}  %>

    <div id="pageReloader" class="smallRoundButtonDark "><div class="icon" onclick="window.location.reload()"></div> </div>

</asp:Content>