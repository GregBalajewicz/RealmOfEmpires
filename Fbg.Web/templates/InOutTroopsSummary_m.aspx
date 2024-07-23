<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="InOutTroopsSummary_m.aspx.cs" Inherits="InOutTroopsSummary_m" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    <div class="inoutsummary getInOutDataChangedEvent">
        <span class="title" data-direction="0" >Summary of Incoming by destination. <A class="showall" data-direction="0">Show all incoming</A></span>
        <span class="title" data-direction="1" >Summary of Outgoing by destination. <A class="showall" data-direction="1">Show all outgoing</A></span>
        <table class="summary" cellspacing="1" cellpadding="1" border="0">
            <thead>
                <tr class="">
                </tr>
            </thead>
            <tbody>
                <tr class="highlight summaryItem" data-dvid="%village.id%" data-dvname="%village.name%" data-dvx="%village.x%" data-dvy="%village.y%" data-direction="0">
                    <td class="">
                        <div class="subRow"><a onclick="ROE.Frame.popupVillageProfile(%village.id%);" class="fontGoldFrLCsm">%village.name%<span class="fontDarkGoldFrLCmed">(%village.x%,%village.y%)</span></a></div>
                        <span class="incoming incomingMiniIndicators">
                            <span class="incomingMiniIndicator indicatorTypeSupport  ">%numIncomingOther%</span>
                            <span class="incomingMiniIndicator indicatorTypeSupport indicatorHidden">%numIncomingOtherkHidden%</span>
                            <span class="incomingMiniIndicator indicatorTypeAttack indicatorHidden ">%numIncomingAttackHidden%</span>
                            <span class="incomingMiniIndicator indicatorTypeAttack ">%numIncomingAttack%</span>
                        </span>
                        <span class="longMiniSummaryContainer">[<span class="incoming incomingMiniIndicators longMiniSummary"></span>]</span>
                        <span class="earliestLanding countdown attack fontMonoNumberSm" data-finisheson="%earliestLandingAttack%"></span>
                        <span class="earliestLanding countdown attackHidden fontMonoNumberSm" data-finisheson="%earliestLandingAttackHidden%"></span>
                        <span class="earliestLanding countdown other fontMonoNumberSm" data-finisheson="%earliestLandingOther%"></span>
                        <span class="earliestLanding countdown otherHidden fontMonoNumberSm" data-finisheson="%earliestLandingOtherHidden%"></span>
                    </td>
                    <td>
                        <div class="filtersCmdBtn smallRoundButtonDark sfx2"><span class="filterIcon"></span></div>
                        <div class="helpButton smallRoundButtonDark sfx2"><span class="helpIcon"></span></div>
                    </td>
                    <td style="width:1px;"><div class="setupSupport helpTooltip" data-tooltipid="incSummarySetupSupport"></div></td>
                </tr>
                <tr class="highlight summaryItem" data-dvid="%village.id%" data-dvname="%village.name%" data-dvx="%village.x%" data-dvy="%village.y%" data-direction="1">
                    <td class="">
                        <div class="subRow" style='%showPlayer%'><a class="fontGoldFrLCsm">%village.ownerName%</a></div>
                        <div class="subRow"><a onclick="ROE.Frame.popupVillageProfile(%village.id%);" class="fontGoldFrLCsm">%village.name%<span class="fontDarkGoldFrLCmed">(%village.x%,%village.y%)</span></a></div>
                        <span class="incoming incomingMiniIndicators">
                            <span class="incomingMiniIndicator indicatorTypeSupport  ">%numIncomingOther%</span>
                            <span class="incomingMiniIndicator indicatorTypeSupport indicatorHidden">%numIncomingOtherkHidden%</span>
                            <span class="incomingMiniIndicator indicatorTypeAttack indicatorHidden ">%numIncomingAttackHidden%</span>
                            <span class="incomingMiniIndicator indicatorTypeAttack ">%numIncomingAttack%</span>
                        </span>
                        <span class="earliestLanding countdown attack fontMonoNumberSm" data-finisheson="%earliestLandingAttack%"></span>
                        <span class="earliestLanding countdown attackHidden fontMonoNumberSm" data-finisheson="%earliestLandingAttackHidden%"></span>
                        <span class="earliestLanding countdown other fontMonoNumberSm" data-finisheson="%earliestLandingOther%"></span>
                        <span class="earliestLanding countdown otherHidden fontMonoNumberSm" data-finisheson="%earliestLandingOtherHidden%"></span>
                    </td><td>
                        <div class="filtersCmdBtn smallRoundButtonDark sfx2"><span class="filterIcon"></span></div>
                        <div class="helpButton smallRoundButtonDark sfx2"><span class="helpIcon"></span></div>
                    </td>
                </tr>
                <tr class="highlight noSummaryMsg" data-direction="0" >
                    <td class="">you have no incoming troops to your villages</td>
                </tr>
                <tr class="highlight noSummaryMsg" data-direction="1" >
                    <td class="">you have no outgoing troops from your villages</td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
