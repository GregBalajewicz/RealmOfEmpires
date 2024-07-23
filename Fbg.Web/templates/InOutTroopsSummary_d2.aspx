<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="InOutTroopsSummary_d2.aspx.cs" Inherits="InOutTroopsSummary_d2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    <div class="inoutsummary getInOutDataChangedEvent">
        <table class="summary" cellspacing="1" cellpadding="1" border="0" width="100%" >
            <thead>
                <tr class="">
                </tr>
            </thead>
            <tbody>
                <tr class="highlight summaryItem" data-dvid="%village.id%" data-dvname="%village.name%" data-dvx="%village.x%" data-dvy="%village.y%" data-direction="0">
                    <td class="tdName fontGoldFrLCmed">
                        <img class="helpButton sfx2" style="height: 12px;" src="https://static.realmofempires.com/images/helpicon.png" />
                        <span><a x="%village.x%" y="%village.y%" vid="%village.id%" opid="%village.ownerPID%" class="jsV" data-vname="%village.name%" >%village.name%<span class="fontDarkGoldNumbersMed">(%village.x%,%village.y%)</span></a></span>                       
                    </td>
                    <td>
                        <span class="incoming incomingMiniIndicators">
                            <span class="incomingMiniIndicator indicatorTypeSupport  ">%numIncomingOther%</span>
                            <span class="incomingMiniIndicator indicatorTypeSupport indicatorHidden">%numIncomingOtherkHidden%</span>
                            <span class="incomingMiniIndicator indicatorTypeAttack indicatorHidden ">%numIncomingAttackHidden%</span>
                            <span class="incomingMiniIndicator indicatorTypeAttack ">%numIncomingAttack%</span>
                        </span>
                        <span class="longMiniSummaryContainer">[<span class="incoming incomingMiniIndicators longMiniSummary"></span>]</span>
                    </td>
                    <td class="fontMonoNumberSm">                        
                        <span><span class="earliestLanding countdown attack" data-finisheson="%earliestLandingAttack%"></span>
                        <span class="earliestLanding countdown attackHidden" data-finisheson="%earliestLandingAttackHidden%"></span>
                        <span class="earliestLanding countdown other" data-finisheson="%earliestLandingOther%"></span>
                        <span class="earliestLanding countdown otherHidden" data-finisheson="%earliestLandingOtherHidden%"></span></span>                        
                    </td>
                    <td style="width:1px;"><div class="setupSupport helpTooltip" data-tooltipid="incSummarySetupSupport"></div></td>
                </tr>

                <tr class="highlight summaryItem" data-dvid="%village.id%" data-dvname="%village.name%" data-dvx="%village.x%" data-dvy="%village.y%" data-direction="1">
                    <td class="">
                        <td class="tdName fontGoldFrLCmed">
                            <img class="helpButton sfx2" style="height: 12px;" src="https://static.realmofempires.com/images/helpicon.png" />
                            <span style='%showPlayer%'><a>%village.ownerName%</a> - </span>
                            <span><a x="%village.x%" y="%village.y%" vid="%village.id%" opid="%village.ownerPID%" class="jsV" data-vname="%village.name%">%village.name%<span class="fontDarkGoldNumbersMed">(%village.x%,%village.y%)</span></a></span>
                        </td>
                        <td>
                            <span class="incoming incomingMiniIndicators">
                                <span class="incomingMiniIndicator indicatorTypeSupport  ">%numIncomingOther%</span>
                                <span class="incomingMiniIndicator indicatorTypeSupport indicatorHidden">%numIncomingOtherkHidden%</span>
                                <span class="incomingMiniIndicator indicatorTypeAttack indicatorHidden ">%numIncomingAttackHidden%</span>
                                <span class="incomingMiniIndicator indicatorTypeAttack ">%numIncomingAttack%</span>
                            </span>                           
                        </td>
                        <td class="fontMonoNumberSm">
                            <span class="earliestLanding countdown attack" data-finisheson="%earliestLandingAttack%"></span>
                            <span class="earliestLanding countdown attackHidden" data-finisheson="%earliestLandingAttackHidden%"></span>
                            <span class="earliestLanding countdown other" data-finisheson="%earliestLandingOther%"></span>
                            <span class="earliestLanding countdown otherHidden" data-finisheson="%earliestLandingOtherHidden%"></span>
                        </td>
                    </td>
                </tr>
                <tr class="highlight noSummaryMsg ntSilverFrLCmed" data-direction="0" >
                    <td class="">You have no incoming troops to your villages</td>
                </tr>
                <tr class="highlight noSummaryMsg ntSilverFrLCmed" data-direction="1" >
                    <td class="">You have no outgoing troops from your villages</td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
