<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="BuildingTempl2.aspx.cs" Inherits="templates_BuildingTempl2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <div class="headerLevel">%curLevel%</div>

    <section id="building_popup2" class="themeM-page">

        <section class="themeM-view default transition %slideClassDefault%">
            <div class="panelWrapper" data-lvl="%curLevel%">

            <div class="buildingPanel descPanel">

                <!-- Building Specific Descriptions -->
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="1"><%=RS("desc_bid1") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="2"><%=RS("desc_bid2") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="3"><%=RS("desc_bid3") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="4"><%=RS("desc_bid4") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="5"><%=RS("desc_bid5") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="6"><%=RS("desc_bid6") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="7"><%=RS("desc_bid7") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="8"><%=RS("desc_bid8") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="9"><%=RS("desc_bid9") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="10"><%=RS("desc_bid10") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="11"><%=RS("desc_bid11") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="12"><%=RS("desc_bid12") %></div>
                <div class="buildDescription bldSpecific fontGoldFrLClrg" data-bid="13"><%=RS("desc_bid13") %></div>

                <!-- Building Specific Effect texts -->
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="1"><%=RS("effectTxt_bid1") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="2"><%=RS("effectTxt_bid2") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="3"><%=RS("effectTxt_bid3") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="4"><%=RS("effectTxt_bid4") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="5"><%=RS("effectTxt_bid5") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed %treasVar1Display%" data-bid="6"><%=RS("effectTxt_bid6") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed %treasVar2Display%" data-bid="6"><%=RS("effectTxt_bid6b") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="7"><%=RS("effectTxt_bid7") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="8"><%=RS("effectTxt_bid8") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="9"><%=RS("effectTxt_bid9") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="10"><%=RS("effectTxt_bid10") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="11"><%=RS("effectTxt_bid11") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="12"><%=RS("effectTxt_bid12") %></div>
                <div class="buildingEffect bldSpecific fontDarkGoldFrLCmed" data-bid="13"><%=RS("effectTxt_bid13") %></div>

                <div class="buildingIcon" style="background-image:url('%buildingIcon%');"></div>
            </div>

            <div class="buildingPanel upgradePanel" data-canupgrade="%canUpgrade%" data-upgradeState="%upgradeState%">
                   
                <div class="actionButton BtnBLg1 fontButton1L %actionButtonGrayout%" onclick="ROE.Building2.doUpgradeBuilding($(this),'%upgradeNextToLevel%');">
                    <%=RS("upgrade") %><span class="smallArrow"></span>
                </div>
                <div class="actionDetail">
                    <div class="toLevel fontGoldFrLC21"><%=RS("tolevel") %>&nbsp%upgradeNextToLevel%</div>
                    <div class="toEffect fontDarkGoldFrLCmed">%upgradeNextBonus%</div>
                </div>

                <div class="actionRequirements">
                    <!-- requirements get pouplated in here -->
                </div>

                <div class="primaryRequirements">

                    <div class="nextReq nextReqCost">
                        <div class="needCol">
                            <div class="label fontDarkGoldFrLCsm">Need</div>
                            <div class="need fontSilverNumbersSm %enoughSilver%">%upgradeNextCost%&nbsp/</div>
                        </div>
                        <div class="haveCol">
                            <div class="label fontDarkGoldFrLCsm">Have</div>
                            <div class="have fontSilverNumbersSm realtimeSilverUpdate" data-vid="%villageID%">%currentSilver%</div>
                        </div>
                    </div>

                    <div class="nextReq nextReqFood">
                        <div class="needCol">
                            <div class="label fontDarkGoldFrLCsm">Need</div>
                            <div class="need fontSilverNumbersSm %enoughFood%">%upgradeNextFood%&nbsp/</div>
                        </div>
                        <div class="haveCol">
                            <div class="label fontDarkGoldFrLCsm">Have</div>
                            <div class="have fontSilverNumbersSm">%currentFood%</div>
                        </div>                      
                    </div>

                    <div class="nextReq fontSilverNumbersSm nextReqTime">
                        <div class="need">%upgradeNextTime%</div>
                    </div>

                </div>



                <div class="maxedNote fontGoldFrLC21"><%=RS("upgradetomaxlevel") %></div>

                <div class="progressPanel">
                    <div class="sProgressContainer fontGoldFrLClrg">
                        <div class="sProgressBar progress-indicator countdownProgressBar id_buildingUpgrade" style="width: 0%;"></div>
                        <div class="sTimeBar">
                            <span class="sTimer %countDown%" data-finisheson="%bFinishesOn%" data-refreshcall="ROE.Building2.upgradeProgressComplete();" 
                                data-progressid="buildingUpgrade" data-totalMS="%maxDuration%" data-eptime="30000" data-epfunc="ROE.Building2.freeUpgradeReady();"></span>
                            <span class="sToLevel"> to (%upgradingToLevel%)</span>
                        </div>
                    </div>
                    <div class="cancelUpgrade" data-eventid="%firstQEventID%" data-isq="notQ" onclick="ROE.Building2.doCancelUpgrade($(this));"></div>
                    <div class="speedUpButton BtnBLg1 fontButton1L" onclick="ROE.Building2.showSpeedUpOptions($(this));"><%=RS("speedup") %><span></span></div>
                    <div class="speedUpFreeButton BtnBLg1 fontButton1L effect-smoothpulse" onclick="ROE.Building2.doSpeedUpFree();">Finish<div class="icon"></div></div>
                </div>

                <div class="qPreviewPanel %showQPreview%">
                    <div class="label fontGoldFrLClrg">Queue:</div>
                    <div class="queuedItemsArea"></div>
                    <div class="fullQButton smallRoundButtonDark" onclick="ROE.Building2.slideToAlternateView('.pageFullQueue');"><span class="smallArrowRight"></span></div>
                </div>

            </div>

            <div class="buildingPanel specialPanel %showPanelSpecial%">

                <!-- For Unit Training Buildings -->
                <div class="specialSubPanel goToTrainingPanel %showPanelTraining%">
                    <!--<div class="goToTrainingBtn BtnBLg2 fontButton1L" onclick="ROE.Building2.slideToAlternateView('.pageSpecial');"><%=RS("traintroops") %><span class="smallArrowRight"></span></div>-->
                    <div class="sectionDivider sD1"></div>
                    <div class="goToTrainingBtn BtnBLg2 fontButton1L" onclick="ROE.UI.Sounds.click(); ROE.QuickRecruit.showPopup('vov');"><%=RS("traintroops") %><div class="qrIcon"></div></div>
                    <div class="sectionDivider sD2"></div>
                    <div class="unitInformationSection buttonsSection"></div>
                    <div class="unitInformationSection descriptionsSection"></div>
                    <div class="sectionDivider sD3"></div>
                </div>
                
                <!-- For Silver Mine
                <div class="specialSubPanel specialSilverMine %showPanelSilverMine%">something / somethingelse</div> -->

                <!-- For Treasury 
                <div class="specialSubPanel specialTreasury %showPanelTreasury%">    
                    <div><span>1111</span><span> / </span><span>111111</span></div>                                                                                                                
                </div>-->

            </div>
            
            <!-- EFFICIENCY BAR PANEL -->
            <div class="buildingPanel efficiencyPanel">
                <div class="totalEfficiency eProgressContainer" onclick="ROE.UI.Sounds.click(); $(this).parent().toggleClass('expanded');">
                    <div class="eProgressBar" style="width:%effectPanel.totalPerc%%;"></div>
                    <div class="eProgressLabel fontGoldFrLClrg">Total Efficiency</div>
                    <span class="smallArrow">
                </div>

                <div class="subEfficiencyBox">
                    
                    <div class="setOne">
                        <div class="eProgressContainer eFromLevel %effectPanel.showLevelPerc%">
                            <div class="eProgressBar" style="width:%effectPanel.levelPerc%%;"></div>
                            <div class="eProgressLabel fontGoldFrLClrg">Building Level &nbsp&nbsp %effectPanel.curLevel% / %effectPanel.maxLevel%</div>
                        </div>
                        <div class="eProgressContainer eFromResearch %effectPanel.showResPerc%">
                            <div class="eProgressBar" style="width:%effectPanel.resPerc%%;"></div>
                            <div class="eProgressLabel fontGoldFrLClrg">Research Bonus &nbsp&nbsp %effectPanel.resPercCur%% / %effectPanel.resPercMax%%</div>
                        </div>
                        <div class="eProgressContainer eFromMisc1 %effectPanel.showMiscPerc%">
                            <div class="eProgressBar" style="width:%effectPanel.miscPerc%%;"></div>
                            <div class="eProgressLabel fontGoldFrLClrg">%effectPanel.miscLabel%</div>
                        </div>
                        <div class="eProgressContainer eFromBonus %effectPanel.showBonusPerc%">
                            <div class="eProgressBar" style="width:0%;"></div>
                            <div class="eProgressLabel fontGoldFrLClrg">From Village Bonus</div>
                        </div>
                    </div>
                        
                </div>

            </div>

            <div class="buildingPanel troopDetailsBtnPanel %troopDetails%">
                <div class="troopDetailsBtn BtnDLg2 fontButton1L" onclick="ROE.UI.Sounds.click(); ROE.Building2.slideToAlternateView('.pageTroopDetails');">Troop Details<span class="smallArrowRight"></span></div>
            </div>
            
            <div class="buildingPanel buildingLevelBtnPanel">
                <div class="buildingLevelBtn BtnDLg2 fontButton1L" onclick="ROE.UI.Sounds.click(); ROE.Building2.slideToAlternateView('.pageBuildingLevels');">Building Levels<span class="smallArrowRight"></span></div>
            </div>
            </div>
            <!-- Resource Footer Container -->
            <div class="resourceFooter footer1"></div>
        </section>

        <section class="themeM-view alternate transition %slideClassAlternate%">

            <div class="alternateSubPage pageFullQueue" data-alt="pageFullQueue">
                <div class="queueHeader">
                    <div class="qLvl">lvl</div>
                    <div class="qName">Building</div>
                    <div class="qTime">Time</div>
                    <div class="qCancel">Cancel</div>
                    <div class="sectionDivider"></div>
                </div>
                <div class="queueContainer">
                    <!-- Queued Item Row template -->
                    <div class="queuedItemRow template" style="background-image:url('%qIcon%');">
                        <div class="qLvl">%qLvl%</div>
                        <div class="qName">%qName%</div>
                        <div class="qTime" refresh="false">%qTime%</div>
                        <div class="qCancel" data-eventId="%qEventId%" data-isQ="%qIsInQ%" data-bid="%qBID%" data-qType="%qType%" onclick="ROE.Building2.doCancel($(this));" ></div>
                    </div>

                </div>
            </div>
            
            <div class="alternateSubPage pageTroopDetails" data-alt="pageTroopDetails">
                <!-- Old Troop Details method from ASPX -->
                <asp:Table runat="server" CellPadding="0" CellSpacing="1" ID="tblUnitHelp"></asp:Table>
            </div>
            
            <div class="alternateSubPage pageBuildingLevels" data-alt="pageBuildingLevels">
                <!-- Old Building Levels method from ASPX -->
                <asp:Label ID="Label1" runat="server" Text="" ></asp:Label>
            </div>
            
            <div class="alernateViewFooter">
                <div class="stripe"></div>
                <div class="backToDefault BtnDSm2n fontButton1L" onclick="ROE.UI.Sounds.click(); ROE.Building2.slideBackToDefaultView();"><%=RS("back") %><span class="smallArrowLeft"></span></div>
            </div>
            
        </section>

    </section>

    <!-- speedUp panel template -->
    <div class="speedUpTemplate template">
        <div class="sHeader fontGoldFrLClrg">
            <div class="sIcon"></div>
            <div class="sProgressContainer">
                <div class="sProgressBar progress-indicator countdownProgressBar id_buildingUpgrade"></div>
                <div class="sTimeBar">
                    <span class="sTimer countdown" data-finisheson="" data-refreshcall="ROE.Building2.speedUpCompleted();" 
                        data-progressid="buildingUpgrade" data-totalMS="00:00:00"></span>
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
            <div class="sServantLabel fontSilverNumbersMed"><%=RS("servants") %></div>
            <div class='sServantHiremore BtnDSm2 fontButton1L' onclick="ROE.UI.Sounds.click(); $(this).closest('.simplePopupOverlay').remove(); ROE.Frame.showBuyCredits();"><%=RS("hiremore") %></div>
        </div>
    </div>
    <!-- end of speedUp panel template -->
    
    <!-- unit description template -->
    <div class="unitDescriptionBox template %descSpecial%" data-unitId="%descUID%">
        <div class="uDescIcon" style="background-image:url(%descIcon%)"></div>
        <div class="uDescText fontSilverFrLCmed">%descText%</div>
        <div class="uDescCost">
            <div class="uDescSilver fontSilverNumbersSm">%descSilver%</div>
            
            <div class="uDescFood fontSilverNumbersSm">%descFood%</div>
            <div class="uDescTime fontSilverNumbersSm">%descTime%</div>
        </div>
        <div class="uDescStats">          
            <div class="stat"><span class="category fontGrayFrLCmed">Attack:</span><span class="value fontSilverNumbersSm">%descAttack%</span></div>
            <div class="stat"><span class="category fontGrayFrLCmed">Defense:</span>
                <span class="value fontSilverNumbersSm" style="background-image:url(%descDef.icon11%)">%descDef.11%</span>
                <span class="value fontSilverNumbersSm" style="background-image:url(%descDef.icon2%)">%descDef.2%</span>
                <span class="value fontSilverNumbersSm" style="background-image:url(%descDef.icon5%)">%descDef.5%</span>
                <span class="value fontSilverNumbersSm" style="background-image:url(%descDef.icon6%)">%descDef.6%</span>
            </div>
<%--            <div class="stat"><span class="category "></span><span class="value fontSilverNumbersSm" style="background-image:url(%descDef.icon2%)">%descDef.2%</span></div>
            <div class="stat"><span class="category "></span><span class="value fontSilverNumbersSm" style="background-image:url(%descDef.icon5%)">%descDef.5%</span></div>
            <div class="stat"><span class="category "></span><span class="value fontSilverNumbersSm" style="background-image:url(%descDef.icon6%)">%descDef.6%</span></div>--%>
            <div class="stat"><span class="category fontGrayFrLCmed">Movement:</span><span class="value fontSilverNumbersSm">%descMovement%</span></div>
        </div>
    </div>
    <!-- end of unit description template -->


    <!-- Phrases -->
    <div id="BuildingPhrases" style="display:none;">
        <div ph="desc_bid1"><%=RS("desc_bid1") %></div>
        <div ph="desc_bid2"><%=RS("desc_bid2") %></div>
        <div ph="desc_bid3"><%=RS("desc_bid3") %></div>
        <div ph="desc_bid4"><%=RS("desc_bid4") %></div>
        <div ph="desc_bid5"><%=RS("desc_bid5") %></div>
        <div ph="desc_bid6"><%=RS("desc_bid6") %></div>
        <div ph="desc_bid7"><%=RS("desc_bid7") %></div>
        <div ph="desc_bid8"><%=RS("desc_bid8") %></div>
        <div ph="desc_bid9"><%=RS("desc_bid9") %></div>
        <div ph="desc_bid10"><%=RS("desc_bid10") %></div>
        <div ph="desc_bid11"><%=RS("desc_bid11") %></div>
        <div ph="desc_bid12"><%=RS("desc_bid12") %></div>
        <div ph="desc_bid13"><%=RS("desc_bid13") %></div>

        <div ph="FillingUpChests"><%=RS("FillingUpChests") %></div>
        <div ph="Recruiting"><%=RS("Recruiting") %></div>
        <div ph="PreparingGovernor"><%=RS("PreparingGovernor") %></div>
        <div ph="SpeedUpCastFail"><%=RS("SpeedUpCastFail") %></div>

    </div>

</asp:Content>
