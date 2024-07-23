<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="CommandTroopsPopup.aspx.cs" Inherits="templates_CommandTroopsPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <%if (isMobile) { %> 
    <div id="background">
        <img src="https://static.realmofempires.com/images/misc/SplashScreenMuted.jpg" class="stretch" alt="" />
    </div>
    <%} %>

    <div id="attack_popup" >
            
            <div class="commandMorale"></div>    

            <DIV class="currentVillage" ><%=RS("From") %>: <span><img src="https://static.realmofempires.com/images/misc/busy_tinyRed.gif"><span></span></span></DIV>
            <DIV class="targetVillage"><%=RS("Target") %>: <span><img src="https://static.realmofempires.com/images/misc/busy_tinyRed.gif"></span></DIV>
            

            <DIV class="attackUnitList bg" >
                            <div id="attackupdater"><%=RS("Checkinglatestdata") %></div>
                            
                            <DIV class="attackIconSheet %attack.show%" id="AttackUnit_%attack.id%" data-unitspeed="%attack.speed%"  data-unitspy="%attack.spy%"   >
                                    <div class="attackImg" style="background-image:url('%attack.img%');" ></div>
                                    <div class="attackButton"><input type="number" pattern="\d*" value="" min="0" max="%attack.max%" onfocus="ROE.Frame.inputFocused();" onblur="ROE.Frame.inputBlured();"/></div> 
                                    <div class="attackMax attackButton ButtonTouch sfx2" data-unitid="%attack.id%" >Max:</BR><span>%attack.max%</span></div>
                                    <div class="attackOne attackButton ButtonTouch sfx2" data-unitid="%attack.id%" >[ 1 ]</div>  
                                    <div class="attackTarget attackButton sfx2" id="attackTarget_%attack.id%"  data-unitid="%attack.id%"  data-unitstr="%attack.strength%"><IMG src="https://static.realmofempires.com/images/icons/M_Target.png"><DIV class="ATarget_%attack.id% attackTargetLayer MessagePopupLayer" data-targetid="%attack.target%" ></DIV></div> 
                                    <div class="desertFactor"><span></span> <%=RS("deserter") %></div>
                                    <div class='themeM-squiggle-t'></div>
                            </DIV>
            </DIV>

            <DIV class="attackInfo" >

                       <DIV>
                                <SPAN id="attackDistance" ><%=RS("Distance") %>: <span>0</span> <%=RS("squares") %> </SPAN> 
                                <SPAN id="attackTraveltime" > <%=RS("TravelTime") %>: <span>00:00:00</span></SPAN>
                                <div id="attackLandingtime" > <%=RS("LandingTime") %>: <span class="landtime" >00:00:00</span></div>
                       </DIV>
                       <DIV>
                                <SPAN id="attackHandicap" ><%=RS("Handicap") %>: <span> </span></SPAN>
                                <SPAN id="Desertion"> <%=RS("Desertion") %>:<span></span></SPAN>
                       </DIV>

                    

                   <DIV id="doAttackButton"></DIV>  
                   <div class="savePreset BtnBSm2 fontSilverFrSClrg helpTooltip" data-toolTipID="saveAsPreset">Save</div>

                    <DIV id="targetInfo" ><%=RS("Warning:") %> <span></span></DIV>
                    <DIV id="targetError" ><%=RS("Error:") %> <span></span></DIV>
                    <DIV id="unitDie" ><%=RS("UnitsDie") %> <span class="unitDieWhy"><%=RS("Why") %></span></DIV>
                    <DIV id="unitDieInfo" ><%=RS("UnitsDieInfo") %></DIV>
                    <DIV id="unitDesert" ><SPAN></SPAN> <span class="unitDesertWhy"><%=RS("Why") %></span></DIV>
                    <DIV id="unitDesertInfo" ><%=RS("UnitsDesertInfo") %></DIV>

                    <div class="outgoing" >
                        <div class="showingMsg"></div>
                    </div>
            </DIV>
            
            <div class="paddinator2000"></div>

            <div class="phrases">
                <div ph="0a"><%=RS("Error0_TroopsSent") %></div>
                <div ph="0b"><%=RS("Error1_TroopsSent") %></div>
                <div ph="1"><%=RS("Error2_NoTroopsinVillage") %></div>
                <div ph="2"><%=RS("Error3_BeginnerProtection") %></div>
                <div ph="3"><%=RS("Error4_TargetUnderSleepMode") %></div>
                <div ph="4"><%=RS("Error5_CannotSupportRebels") %></div>
                <div ph="5"><%=RS("Error6_CannotAttackRebelsMoreThan22Away") %></div>
                <div ph="6"><%=RS("Error7_AttackFreeze") %></div>
                <div ph="7"><%=RS("Error8_UnknownError") %></div>
                <div ph="8"><%=RS("Error9_TargetVillageSameAsOrigin") %></div>
                <div ph="9"><%=RS("Error10_AllToopsDesert") %></div>
                <div ph="10"><%=RS("Error11_NoTroopsSelected") %></div>
                <div ph="11"><%=RS("Error12_StewardAttacking") %></div>
                <div ph="12"><%=RS("Error13_StewardSupport") %></div>
                <div ph="13"><%=RS("Error_TargetInVacationMode") %></div>
                <div ph="13"><%=RS("Error_TargetInVacationMode") %></div>
                <div ph="14"><%=RS("Error_TargetInWeekendMode") %></div>
                <div ph="TargetClanIsYours"><%=RS("Error15_TargetClanIsYours") %></div>
                <div ph="15"><%=RS("Error16_TargetVillageSame") %></div>
                <div ph="16"><%=RS("Error17_TargetOwnerChanged") %></div>
                <div ph="17"><%=RS("Error18_TargetBeginnerProtection") %></div>
                <div ph="18"><%=RS("Error19_TargetSleepModeUntil") %> </div>
                <div ph="19"><%=RS("Error20_TargetAlly") %></div>
                <div ph="20"><%=RS("Error21_TargetNAP") %></div>
                <div ph="21"><%=RS("Error10_AllToopsDesert") %></div>
                <div ph="22"><%=RS("Error22_XNumberDeserter") %></div>
                <div ph="23"><%=RS("Error23_CapitalVillage") %></div>
                <div ph="30"><%=RS("today") %></div>
                <div ph="31"><%=RS("tomorrow") %></div>
                <div ph="32"><%=RS("troopsChanged") %></div>
                <div ph="noTroops"><%=RS("Error14_NoTroopsSelected") %></div>
             </div>

        

    </div>
   
    
</asp:Content>