<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="QuickBuildTempl.aspx.cs" Inherits="templates_QuickBuildTempl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <section id="quickbuild_popup" class="themeM-page">

        <div class="fg">

            <section class="themeM-view default transition">
                
                <!-- UPGRADE Q summary area -->
                <div class="qSummaryArea qUpgradeSummaryArea">                   
                    <div class="buildTimeArea">
                        <div class="speedUpButton BtnBLg1 fontSilverFrSClrg upgrade grayout" onclick="ROE.QuickBuild.showSpeedUpOptions($(this));"><%=RS("SpeedUp")%><span></span></div>
                        <div class="speedUpFreeButton BtnBLg1 fontButton1L effect-smoothpulse" onclick="ROE.QuickBuild.doSpeedUpFree();">Finish<div class="icon"></div></div>
                        <span class="currentBuildTimeTitle fontDarkGoldFrLCsm"><%=RS("BuildTime")%></span>
                        <span class="currentBuildTimeTimer" data-refreshcall="ROE.QuickBuild.reInitContent();" data-eptime="30000" data-epfunc="ROE.QuickBuild.freeUpgradeReady();" ></span>
                        <span class="totalBuildTimeTitle fontDarkGoldFrLCsm"><%=RS("TotalTime")%></span>
                        <span refresh="false" class="totalBuildTimeTimer"></span>
                    </div>
                    <div class="sectionDivider"></div>
                    <div class="queuedItemsArea">
                    </div>
                    <div class="slideToQueuePage BtnBSm2 fontSilverFrSClrg grayout" onclick="ROE.QuickBuild.slideToQueuePage($(this));"><%=RS("Queue")%><span class="smallArrowRight"></span></div>
                </div>

                <!-- DOWNGRADE Q summary area -->
                <div class="qSummaryArea qDowngradeSummaryArea" style="display:none;">                   
                    <div class="buildTimeArea">
                        <div class="speedUpButton BtnBLg1 fontSilverFrSClrg downgrade grayout" onclick="ROE.QuickBuild.showSpeedUpOptions($(this));"><%=RS("SpeedUp")%><span></span></div>
                        <span class="currentBuildTimeTitle fontDarkGoldFrLCsm"><%=RS("DowngradingTime")%></span>
                        <span class="currentBuildTimeTimer" data-refreshcall="ROE.QuickBuild.reInitContent();" ></span>
                    </div>
                    <div class="sectionDivider"></div>
                    <div class="queuedItemsArea">
                    </div>
                    <div class="slideToQueuePage BtnBSm2 fontSilverFrSClrg grayout" onclick="ROE.QuickBuild.slideToQueuePage($(this));"><%=RS("Queue")%><span class="smallArrowRight"></span></div>
                </div>

                <div class="upgDowngContainer">

                    <div class="upgradeArea">
                        <div class="buildingRow %upStatus% template" 
                            data-buildingId="%buildingId%" data-currLevel="%currLevel%" data-statusCode="%upCode%"
                            data-nextLevel="%nextLevel%" data-nextLevelMax="%nextLevelMax%">
                            <div class="buildingBtn smallRoundButtonDark" onclick="ROE.QuickBuild.showBuildingPagePopup(%buildingId%,%villageId%);">
                                <div class="buildingIcon" style="background-image:url('%iconUrl%');"></div>
                            </div>
                            <div class="buildingName fontGoldFrLClrg">%name% <span>(%currLevel%%upToLevel%)</span></div>
                            <div class="buildingResources fontGoldFrLClrg"><span class="bResourceIcon bSilver">%nextCost%</span><span class="bResourceIcon bFood">%nextFood%</span></div>
                            <div class="buildingUpgradeBtnOne smallRoundButtonDark fontSilverNumbersLrg" onclick="ROE.QuickBuild.doUpgradeBuilding('%buildingId%','%nextLevel%','%upCode%');">%nextLevel%</div>
                            <div class="buildingUpgradeBtnMore smallRoundButtonDark fontSilverNumbersLrg %btnMoregrayout%" onclick="ROE.QuickBuild.upgradeNumpad($(this).parent());">...</div>
                            <div class="buildingCongrats"><%=RS("Congrats")%></div>
                            <div class="buildingBtnInfo BtnDSm2 fontSilverFrSClrg" onclick="ROE.QuickBuild.expandRow($(this));"><%=RS("Info")%><span class="smallArrowDown"></span></div>
                            <div class="buildingRequirements"></div>
                            <%--<div class="sectionDivider"></div>--%>
                        </div>
                    </div>

                    <div class="downgradeArea" style="display:none;">
                        <div class="buildingRow %downStatus% template" style="background-image:url('%iconUrl%');"
                            data-buildingId="%buildingId%" data-currLevel="%currLevel%" data-statusCode="%downCode%"
                            data-nextLevel="%nextLevel%" data-nextLevelMax="%nextLevelMax%">
                            <div class="buildingName fontGoldFrLClrg">%name% <span>(%currLevel%%downToLevel%)</span></div>             
                            <div class="buildingUpgradeBtnOne BtnDSm2 fontSilverFrSClrg" onclick="ROE.QuickBuild.doDowngradeBuilding('%buildingId%','%downCode%');"><%=RS("Downgrade")%></div>
                            <%--<div class="sectionDivider"></div>--%>
                        </div>
                    </div>

                    <div class="switchModeArea upgradeActive">
                        <div class="fontSilverFrSClrg switchModeBtn toUpgrade" onclick="ROE.QuickBuild.switchMode('toUpgrade');"><%=RS("Upgrade")%></div>
                        <div class="fontSilverFrSClrg switchModeBtn toDowngrade" onclick="ROE.QuickBuild.switchMode('toDowngrade');"><%=RS("Downgrade")%></div>
                    </div>

                </div>

                <!-- Resource Footer Container -->
                <div class="resourceFooter footer1"></div>

                <!-- numPad panel template -->
                <div class="upgradeNumpadTemplate template">
                    <div class="upgradeNumpadHolder">
                        <div class="upgradeNumpadHeader">
                            <div class="buildingName fontGoldFrLClrg"></div>
                            <div class="buildingLevel fontGoldFrLCmed"></div>
                            <div class="buildingUpgradeText fontDarkGoldFrLCmed"></div>
                        </div>
                        <div class="upgradeNumpadBox">
                        </div>
                    </div>
                </div>

                <!-- speedUp panel template -->
                <div class="speedUpTemplate template">
                    <div class="sHeader fontGoldFrLClrg">
                        <div class="sIcon"></div>
                        <div class="sProgressContainer">
                            <div class="sProgressBar progress-indicator countdownProgressBar id_buildingUpgrade"></div>
                            <div class="sTimeBar">
                                <span class="sTimer" data-refreshcall="ROE.QuickBuild.speedUpCompleted();" data-progressid="buildingUpgrade"></span>
                                <span class="sToLevel"></span>
                            </div>
                        </div>
                    </div>
                    <div class="sContent">
                        <div class="sOption">
                            <div class="sName fontDarkGoldFrLCxlrg"></div>
                            <div class="speedUpButton smallRoundButtonLight"><span></span></div>
                            <div class="sCut fontDarkGoldFrLCmed"></div>
                            <div class="sServants fontDarkGoldFrLCmed"></div>
                        </div>
                        <div class="sBusy"></div>
                    </div>
                    <div class="sFooter">
                        <div class="sectionDividerGold"></div>
                        <div class="sServantIcon"></div>                      
                        <div class="sServantCount fontSilverNumbersMed"></div>
                        <div class="sServantLabel fontSilverNumbersMed">Servants</div>
                        <div class='sServantHiremore BtnDSm2 fontButton1L' onclick="ROE.UI.Sounds.click(); $(this).closest('.simplePopupOverlay').remove(); ROE.Frame.showBuyCredits();">Hire More</div>
                    </div>
                </div>

            </section>

            <!-- Main Queue Page -->
            <section class="themeM-view queuepage slideRightFrom transition" >
                <div class="queueWrapper" >
                    <div class="queueHeader">
                        <div class="qLvl">lvl</div>
                        <div class="qName"></div>
                        <div class="qTime">Time</div>
                        <div class="qCancel">Cancel</div>
                        <div class="sectionDivider"></div>
                    </div>
                    <div class="queueContainer">
                        <div class="queuedItemRow template" style="background-image:url('%qIcon%');">
                            <div class="qLvl">%qLvl%</div>
                            <div class="qName">%qName%</div>
                            <div class="qTime" refresh="false">%qTime%</div>
                            <div class="qCancel" data-eventId="%qEventId%" data-isQ="%qIsInQ%" data-bid="%qBID%" data-qType="%qType%" 
                                onclick="ROE.QuickBuild.doCancel($(this));" ></div>
                        </div>
                    </div>
                    <div class="sectionDivider" style="bottom: 32px;z-index: 1;"></div>
                    <div class="queueFooter">
                        <div class="stripe"></div>
                        <div class="slideBackToDefaultPage BtnDSm2n fontSilverFrSClrg" onclick="ROE.QuickBuild.slideBackToDefaultPage();"><span class="smallArrowLeft"></span>Back</div>
                        <div class="cancelAll BtnDSm2n fontSilverFrSClrg" onclick="ROE.QuickBuild.cancelAll();">Cancel All</div>
                    </div>
                </div>
            </section>

        </div>

    </section>
                
    <!-- Phrases -->
    <div id="QuickBuildPhrases" style="display:none;">
        <div ph="qb_cantDowngrade_1"><%=RS("qb_cantDowngrade_1") %></div>
        <div ph="qb_cantDowngrade_2"><%=RS("qb_cantDowngrade_2") %></div>
        <div ph="qb_cantDowngrade_3"><%=RS("qb_cantDowngrade_3") %></div>
        <div ph="qb_cantDowngrade_4"><%=RS("qb_cantDowngrade_4") %></div>
        <div ph="qb_cantDowngrade_5"><%=RS("qb_cantDowngrade_5") %></div>
        <div ph="qb_cantDowngrade_Default"><%=RS("qb_cantDowngrade_Default") %></div>
        <div ph="qb_cantUpgrade_1"><%=RS("qb_cantUpgrade_1") %></div>
        <div ph="qb_cantUpgrade_2"><%=RS("qb_cantUpgrade_2") %></div>
        <div ph="qb_cantUpgrade_3"><%=RS("qb_cantUpgrade_3") %></div>
        <div ph="qb_cantUpgrade_4"><%=RS("qb_cantUpgrade_4") %></div>
        <div ph="qb_cantUpgrade_5"><%=RS("qb_cantUpgrade_5") %></div>
        <div ph="qb_cantUpgrade_6"><%=RS("qb_cantUpgrade_6") %></div>
        <div ph="qb_cantUpgrade_7"><%=RS("qb_cantUpgrade_7") %></div>
        <div ph="qb_cantUpgrade_def"><%=RS("qb_cantUpgrade_def") %></div>
        <div ph="SpeedUpCastSuccess"><%=RS("SpeedUpCastSuccess") %></div>
        <div ph="SpeedUpCastFail"><%=RS("SpeedUpCastFail") %></div>
        <div ph="SpeedUpDowngradeSuccess"><%=RS("SpeedUpDowngradeSuccess") %></div>
        <div ph="SpeedUpDowngradeFail"><%=RS("SpeedUpDowngradeFail") %></div>
        <div ph="MainLoadingMsg1"><%=RS("MainLoadingMsg1") %></div>
        <div ph="MainLoadingMsg2"><%=RS("MainLoadingMsg2") %></div>
        <div ph="MainLoadingMsg3"><%=RS("MainLoadingMsg3") %></div>
        <div ph="MainLoadingMsg4"><%=RS("MainLoadingMsg4") %></div>
        <div ph="MainLoadingMsg5"><%=RS("MainLoadingMsg5") %></div>       
    </div>

</asp:Content>
