<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="QuickRecruitTempl_d2.aspx.cs" Inherits="templates_QuickRecruitTempl_d2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <section id="quickrecruit_popup" class="themeM-page">

        <div id="d2fg" class="fg">

            <section class="themeM-view default transition">
                <div class="recruitSummary"><div class="unitsSummaryHolder"></div></div>
                <div class="recruitPanelContainer"></div>
                <div class="resourceFooter footer1"></div>
            </section>

            <section class="themeM-view queuepage slideRightFrom transition" >
                <div class="queueWrapper" >
                    <div class="queueHeader">
                        <div class="qCount"><%=RS("Queue")%></div>
                        <div class="qName"><%=RS("Unit")%></div>
                        <div class="qTime"><%=RS("Time")%></div>
                        <div class="qCancel"><%=RS("Cancel")%></div>
            <%--            <div class="sectionDivider"></div>--%>
                    </div>
                    <div class="queueContainer">


                    </div>
                    <div class="sectionDivider" style="bottom: 28px;z-index: 1;"></div>
                    <div class="queueFooter">
                        <div class="stripe"></div>
                        <div class="slideBackToDefaultPage BtnDSm2n fontSilverFrSClrg" onclick="ROE.QuickRecruit.slideBackToDefaultPage();"><span class="smallArrowLeft"></span><%=RS("Back")%></div>
                    </div>
                </div>
            </section>

        </div>

    </section>


    <!-- TEMPLATES -->

    <!-- BUILDING RECRUIT PANEL -->
    <div class="recruitPanel template" data-buildingId="%pId%" data-buildingName="%pName%">
        <div class="queuedItemsArea" style="background-image:url(%pIcon%);">               
        </div>
        <div class="slideToQueuePage BtnDSm2n fontSilverFrSClrg" data-buildingId="%pId%" 
            onclick="ROE.QuickRecruit.slideToQueuePage($(this));"><%=RS("Queue")%><span class="smallArrowRight"></span></div>
    </div>
        
    <!-- UNIT RECRUIT SECTION -->
    <div class="unitSection template %lockedDueTo%" data-unitId="%uId%" data-uCanRec="%uCanRec%" data-uRecMax="%uRecMax%" data-unitName="%uName%" data-unitIcon="%uIcon%">
        <div class="unitIcon" style="background-image:url('%uIcon%');"></div>
        <div class="unitInfo">
            <div class="fontGoldFrLCmed">%uName%</div>
            <div class="fontSilverNumbersSm">&nbsp; %uCurrentCount%</div>
        </div>
        <div class="unitRecruitCommands">
<%--            <div class="recruitCirc" onclick="ROE.QuickRecruit.doRecruit($(this));"><div class="rcLabel fontGoldFrLClrg">25%</div><div class="rcNum fontWhiteNumbers">%uRec25%</div></div>
            --%>
            <input class="recruitAmount" type="number" min="0" />
            <div class="BtnDSm2n half" onclick="ROE.QuickRecruit.doRecruit($(this));"><div class="rcLabel fontGoldFrLClrg">50%:</div><div class="rcNum fontWhiteNumbers">%uRec50%</div></div>
            <div class="BtnDSm2n max" onclick="ROE.QuickRecruit.doRecruit($(this));"><div class="rcLabel fontGoldFrLClrg"><%=RS("Max")%>:&nbsp;</div><div class="rcNum fontWhiteNumbers">%uRecMax%</div></div> 
            <%--<div class="BtnDSm2n" onclick="ROE.QuickRecruit.doRecruit($(this));"><div class="rcNum fontSilverFrSClrg">Recruit</div></div>--%>
   <%--         <div class="recruitCirc" onclick="ROE.QuickRecruit.openRecruitInputField($(this));"><div class="rcHash fontGoldFrLClrg">#</div></div>--%>
        </div>
        <div class="unitResearchButton BtnBLg1 fontSilverFrSClrg" onclick="ROE.Research.showResearchPopup('u%uId%');">Research<div></div></div>
        <div class="missingBuildingLevels fontSilverFrSClrg">%missingBuilding%</div>
<%--        <div class="sectionDivider"></div>--%>
    </div>

    <!-- UNIT RECRUIT SECTION: GOV EDITION -->
    <div class="unitSectionGov template" data-unitId="%uId%" data-uCanRec="%uCanRec%" data-uRecMax="%uRecMax%">
        <div class="unitIcon" style="background-image:url('%uIcon%');"></div>
        <div class="unitInfo">
            <div class="fontGoldFrLCmed">%uName%</div>
            <div class="fontSilverNumbersSm">&nbsp; %uCurrentCount%</div>
        </div>
        <div class="unitRecruitCommands %enoughChests%">
            <div class="currentGovCost fontGoldFrLClrg"><%=RS("Use")%> <span class="fontGoldFrLClrg">%costOfNextGov%</span> <%=RS("Chests")%></div>
            <div class="govFoodCost fontGoldFrLClrg %enoughFood%">%uGovFoodCost%</div>
            <div class="BtnDSm2n" onclick="ROE.QuickRecruit.doRecruitSingleGov($(this));"><div class="rcNum fontSilverFrSClrg" style="font-size: 18px;">Recruit</div></div>
        </div>
        <%--<div class="sectionDivider"></div>--%>  
    </div> 

    <!-- Chests Section -->
    <div class="chestSection template %lockedDueTo%" data-unitId="%uId%" data-uCanRec="%uCanRec%" data-uRecMax="%uRecMax%" data-unitName="%uName%" data-unitIcon="%uIcon%">
        <div class="unitIcon" style="background-image:url('%uIcon%');"></div>
        <div class="unitInfo">
            <div class="fontGoldFrLCmed">%uName%</div>
            <div class="fontSilverNumbersSm">&nbsp; %uCurrentCount%</div>
        </div>
        <div class="unitRecruitCommands">
            <input class="recruitAmount" type="number" min="0" />
            <div class="BtnDSm2n chestBuy" onclick="ROE.QuickRecruit.doRecruit($(this));"><div class="rcNum fontSilverFrSClrg">Buy</div></div>
            <div class="BtnDSm2n max" onclick="ROE.QuickRecruit.doRecruit($(this));"><div class="rcLabel fontGoldFrLClrg"><%=RS("Max")%>:&nbsp;</div><div class="rcNum fontWhiteNumbers">%uRecMax%</div></div> 
        </div>
        <div class="unitResearchButton BtnBLg1 fontSilverFrSClrg" onclick="ROE.Research.showResearchPopup('u%uId%');">Research<div></div></div>
        <div class="missingBuildingLevels fontSilverFrSClrg">%missingBuilding%</div>
    </div>

    <!-- MASS CHEST SECTION -->
    <div class="massChestSection template">
        <div class="unitIcon" style="background-image:url('%uIcon%');"></div>
        <div class="unitRecruitCommands">
            <div class="massChestInfo fontGoldFrLCmed"><%=RS("massChestInfo_d2")%></div>
            <div class="BtnDSm2n" onclick="ROE.QuickRecruit.massChestPopup();"><div class="rcMassBuy fontSilverFrSClrg"><%=RS("MassBuy")%></div></div>
        </div>
        <%--<div class="sectionDivider"></div>--%>
    </div>

    <!-- UNIT DISBAND SECTION -->
    <div class="disbandUnitSection template" data-unitId="%uId%" data-unitName="%uName%" data-unitIcon="%uIcon%" data-uYourUnitsCurrentlyInVillageCount="%uYourUnitsCurrentlyInVillageCount%">
        <div class="unitIcon" style="background-image:url('%uIcon%');"></div>
        <div class="unitInfo">
            <div class="fontGoldFrLClrg">%uName%</div>
            <div class="fontSilverNumbersLrg">%uYourUnitsCurrentlyInVillageCount%</div>
        </div>
        <div class="unitRecruitCommands">
            <div class="recruitCirc" onclick="ROE.QuickRecruit.maxDisband($(this));"><div class="rcLabel fontGoldFrLClrg"><%=RS("Max")%></div><div class="rcNum fontWhiteNumbers">%uYourUnitsCurrentlyInVillageCount%</div></div>
            <div class="recruitCirc" onclick="ROE.QuickRecruit.openDisbandInputField($(this));"><div class="rcHash fontGoldFrLClrg">#</div></div>
        </div>
        <%--<div class="sectionDivider"></div>--%>  
    </div>
       
    <!-- QUEUE PAGE ROW -->  
    <div class="queuedItemRow template">
        <div class="qCount">%qCount%</div>
        <div class="qIcon" style="background-image:url('%qIcon%');"></div>
        <div class="qName">%qName%</div>
        <div class="qTime" data-refreshcall="ROE.QuickRecruit.reInitContent();">%qTime%</div>
        <div class="qCancel" data-eventId="%qEventId%" data-bid="%qBID%" onclick="ROE.QuickRecruit.doCancelRecruit($(this));" ></div>
    </div>

    <!-- RECRUIT ALL ROW -->
    <div class="recruitAllRow fontSilverFrSClrg template">
        <div class="BtnDSm2n" onclick="ROE.UI.Sounds.click(); ROE.QuickRecruit.recruitAll($(this));">Recruit All</div>
    </div>

    <!-- TOGGLE MODE TEMPLATE -->
    <div class="switchModeArea leftActive fontSilverFrSClrg template">
        <div class="switchModeBtn left" onclick="ROE.QuickRecruit.switchMode('toRecruit');"><%=RS("Recruit") %></div>
        <div class="switchModeBtn right" onclick="ROE.QuickRecruit.switchMode('toDisband');"><%=RS("Disband") %></div>
    </div>

    <!-- Phrases -->
    <div id="QuickRecruitPhrases" style="display:none;">
        <div ph="QuickRecruit"><%=RS("QuickRecruit") %></div>
        <div ph="Max"><%=RS("Max") %></div>
        <div ph="BuyingChestsIn"><%=RS("BuyingChestsIn") %></div>
        <div ph="MassChestBuyStopped"><%=RS("MassChestBuyStopped") %></div>           
        <div ph="Buy"><%=RS("Buy") %></div>
        <div ph="FillingUpChests"><%=RS("FillingUpChests") %></div>
        <div ph="Recruit"><%=RS("Recruit") %></div>
        <div ph="Disband"><%=RS("Disband") %></div>
        <div ph="Recruiting"><%=RS("Recruiting") %></div>
        <div ph="Disbanding"><%=RS("Disbanding") %></div>
        <div ph="PreparingGovernor"><%=RS("PreparingGovernor") %></div>
        <div ph="PreparingVillageList"><%=RS("PreparingVillageList") %></div>   
        <div ph="massChestInfo"><%=RS("massChestInfo") %></div>
        <div ph="Stop"><%=RS("Stop") %></div>
        <div ph="StoppingMassChestBuy"><%=RS("StoppingMassChestBuy") %></div>
        <div ph="MassBuyCompleted"><%=RS("MassBuyCompleted") %></div>      
        <div ph="StartMassBuy"><%=RS("StartMassBuy") %></div>      
    </div>

</asp:Content>
