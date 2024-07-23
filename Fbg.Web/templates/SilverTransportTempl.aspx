<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="SilverTransportTempl.aspx.cs" Inherits="templates_SilverTransportTempl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <section id="silvertransport_popup" class="themeM-page">

        <div class="fg">

            <section class="themeM-view default transition">

                <div class="headerPanel">

                    <div class="destinationWrapper">
                        <span class="fontGoldFrLClrg"><%=RS("transportsilverto") %></span>
                        <span class="fontSilverFrLClrg destinationName"></span>
                        <span class="fontSilverNumbersLrg destinationCords"></span>
                    </div>

                    <div class="mainBtns">
                        <div class="getMaxAll BtnDLg2 fontButton1L sfx2" onclick="ROE.SilverTransport.getMaxAll();"></div>
                        <div class="viewTransports BtnDLg2 fontButton1L sfx2" onclick="ROE.SilverTransport.viewTransports();">View Transports</div>
                    </div>

                    <div class="tableHead fontGoldFrLClrg">
                        <div class="tp tpLeft"><%=RS("from") %></div>
                        <div class="tp tpMid"><%=RS("silver") %>/<br /><%=RS("time") %></div>
                        <div class="tp tpRight"></div>
                    </div>

                    <div class="showFilters smallRoundButtonDark sfx2" onclick="ROE.SilverTransport.showFilterOptions();"><span class="filterIcon"></span></div>
        
                </div>

                <div class="villagesPanel">
                    <div class="noTransports fontSilverFrLClrg"><%=RS("notranspsavail") %></div>
                    <div class="villageRow">
                        <div class="tp tpLeft">
                                                        <div class="villageName fontSilverFrLCmed">%Name%</div>
                            <div class="villageCords fontSilverNumbersMed">(%XCord%,%YCord%)</div>
                        </div>
                        <div class="tp tpMid fontDarkGoldNumbersLrg">


                                                        <div class="silverAmount">%maxSendFormatted%</div>
                            <div class="transportTime">%TravelTime%</div>
                        </div>
                        <div class="tp tpRight">
                            <div class="sendMax smallRoundButtonDark fontButton1M sfx2" onclick="ROE.SilverTransport.getAmountFromVill(%VillageID%,%maxSend%);"><%=RS("max") %></div>
                            <div class="sendAmount smallRoundButtonDark fontButton1M sfx2" onclick="ROE.SilverTransport.activateInputForRow($(this));">#</div>

                            <div class="deactivateRow smallRoundButtonDark fontButton1L sfx2" onclick="ROE.SilverTransport.deactivateInputForRow($(this));"></div>
                            <input class="sendAmountInput" type="number" pattern="\d*" value="" onfocus="ROE.Frame.inputFocused();" onblur="ROE.Frame.inputBlured();"/>
                            <div class="sendAmountGo smallRoundButtonDark fontButton1L sfx2" data-max="%maxSend%" onclick="ROE.SilverTransport.getTransportFromInputGo($(this),%VillageID%);">Go!</div>                         
                        </div>
                    </div>


                </div>


            </section>

        </div>

    </section>
                
    <!-- Phrases -->
    <div id="SilverTransportPhrases" style="display:none;">
        <div ph="MainLoadingMsg1"><%=RS("MainLoadingMsg1") %></div>
        <div ph="MainLoadingMsg2"><%=RS("MainLoadingMsg2") %></div>
        <div ph="MainLoadingMsg3"><%=RS("MainLoadingMsg3") %></div>
        <div ph="MainLoadingMsg4"><%=RS("MainLoadingMsg4") %></div>
        <div ph="MainLoadingMsg5"><%=RS("MainLoadingMsg5") %></div>       
    </div>

</asp:Content>
