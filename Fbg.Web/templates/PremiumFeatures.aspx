<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="PremiumFeatures.aspx.cs" Inherits="templates_PremiumFeatures" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
       
    <div class="templ_premiumFeatures">
        <section class="themeM-panel ">
            <div class='pfHeaderText'>
                <%=RS ("PremiumFeaturesListed")%><br/>
                <%=RS ("ServantNumber")%><br/><br/>
                <a href="#" class="sfx2 buymore customButtomBG" onclick="ROE.Frame.showBuyCredits();"><%=RSc("HireServant") %></a><br />
            </div>
            <section class="themeM-group clearfix ">
                <div id="elmtDrop">
                    <section class="themeM-panel onePF" data-pfid="%pfpid%">
                        <div class="icon"  style="background-image:url('%img%');"></div>
                        <div id="%btnid%" class="action toggle clickable button" style="background-image:url('%button%');"></div>
                        <div class="txt">
                            <span class="pfTitle">%name%</span> <span class="pfDesc">
                                <br />
                                %descr%
                                <br />
                                [%servants% servants / %period%]</span>
                            <br />
                            <span class=state>%state% <span format="long2" class="countdown">%countdown%</span></span>
                        </div>
                    </section>
                </div>
            </section>
        </section>

        <!-- PREMIUM FEATURE INLINE ROW TEMPLATE -->
        <div class="PFInlineRow">
            <div class="PFIcon" style="background-image:url('%img%');"></div>
            <div class="PFText fontGoldFrLCsm">
                <div class="PFDesc">%descr%</div>
                <div class="PFDura">%servants% SERVANTS FOR %period%</div>
                <div class="PFState %state%">%stateText%</div>
                <div class="PFTimer fontSilverNumbersSm %stateCountdown%">%countdown%</div>
            </div>           
            <div class="castSpell BtnDSm2n fontSilverFrSClrg sfx2" data-pfpid="%btnid%">%buttonTxt%</div>
        </div>

        <script><%=IsInDesignMode ? "ROE.Api.apiLocationPrefix = '../';" : "" %></script>
    </div>
</asp:Content>
