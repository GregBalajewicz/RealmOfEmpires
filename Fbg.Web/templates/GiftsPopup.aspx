<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="GiftsPopup.aspx.cs" Inherits="templates_GiftsPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
     
    <div id="gifts_popup" >
            
            <!-- Default Page -->
            <div class="giftList transition">
                <div class="giftContainer bg">
                    <h1><%=RS("Title") %><div class="smallRoundButtonDark infoBtn helpTooltip" data-tooltipid="getMoreInfo"><div class="icon"></div></div></h1>
                    <div id="giftdaily"><%=RS("YouCanUse") %></div>
                    <div class="giftsIconSheet RowTouch" data-giftindex="%gift.index%" data-giftcnt="%gift.available%" id="Gift_%gift.id%" >
                        <div class="giftsIcon" style="background-image:url('%gift.img%')"></div>
                        <div class="giftDescriptionWrapper">
                            <%=RS("giftName") %> 
                            <span class="availableText"><%=RS("availableText") %></span>
                        </div>
                        <img src='https://static.realmofempires.com/images/misc/M_ArrowR.png' class="giftRArrow" />
                    </div>
                    <div class="giftsIconSheetNot" data-giftindex="%gift.index%" data-giftcnt="%gift.available%"   id="Div1" >
                        <div class="giftsIcon" style="background-image:url('%gift.img%')"></div> 
                        <div> 
                            <%=RS("giftName") %>
                            <span ><%=RS("availableText") %></span>
                            <div class="giftreq"><%=RS("Unlock_part1") %> <%=RS("reqtitle") %>  <%=RS("Unlock_part2") %></div> 
                        </div>                            
                    </div>
                </div> 
            </div>         
            
            <div class="giftInfo slideLeftTo">
                <div class="bazzarContainer">

                    <div class="bazzarPanel panelYouOwn">
                        <div class="fontGoldFrLClrg" style="margin-bottom: -10px;">Inventory</div>
                        <div class="fontDarkGoldFrLCmed">You own <span class="youOwnNum">0</span> items of <div class="itemIcon"></div></div>
                    </div>

                    <div class="bazzarPanel panelIsWorth">
                        <div class="fontGoldFrLClrg" style="margin-bottom: -10px;">Bazaar</div>
                        <div class="fontDarkGoldFrLCmed">Each item is worth <span class="isWorthNum">0</span> <div class="itemIcon"></div></div>
                    </div>

                    <div class="bazzarPanel panelSilverPF">
                        <div class="pfTtitle fontGoldFrLClrg">Silver Items could be worth more!</div>
                        <div class="pfContainer"></div>
                    </div>

                    <div class="bazzarPanel panelDetails">
                        <div class="fontGoldFrLC21"><span class="itemsSelected">0</span> items selected</div>
                        <div class="giftButtonList">
                            <div class="giftInfoAdd" data-giftadd="1" ></div>
                            <div class="giftInfoAdd" data-giftadd="10" ></div>
                            <div class="giftInfoAdd" data-giftadd="-10" ></div>
                            <div class="giftInfoAdd" data-giftadd="-1" ></div>
                        </div>
                        <div class="maxButtons">
                            <div class="BtnDLf1 fontButton1L maxFree"></div>
                            <div class="BtnDLf1 fontButton1L max"></div>
                        </div>
                        <div class="fontGoldFrLClrg summeryPanel">
                            <div class="summaryL1" style="color:white;">You are getting:</div>
                            <div class="summaryL2" style="color:white;">0 Spies for free and,</div>
                            <div class="summaryL3" style="color:white;">60 Spies for 0 servants.</div>
                            <div class="summaryL4">Total: 60 Spies.</div>
                        </div>
                    </div>

                    <div class="bazzarPanel getItemsPanel">
                        <div class="BtnBLg2 fontButton1L getItemsButton">Get Items</div>
                        <div id="giftError" class="fontSilverFrLCmed"></div>
                    </div>

                </div>
            
                <div class="backFooter">
                    <div class="stripe"></div>
                    <div class="slideBackToDefaultPage BtnDSm2n fontSilverFrSClrg" onclick="ROE.QuickBuild.slideBackToDefaultPage();"><span class="smallArrowLeft"></span>Back</div>
                </div>
            
            </div>

            <div class="infoPanel">
                <div>You can use your stored gifts, or buy more here. The items are recieved in your currently selected village.</div><br />
                <div>In this realm the daily limit is: <span class="hl">%dailyLimit% gifts.</span></div><br />
                <div>If you don't use your gifts in a certain day, the unused ones can be carried over and stored for a few days.</div><br />
                <div>In this realm, the daily limit is carried over for a maximum of <span class="hl">%dailyLimitCarryOverDays% days.</span></div><br />
            </div>
        
            
            <div class="phrases">
                <div ph="1"><%=RS("Error1_DailyLimit") %></div>
                <div ph="2"><%=RS("Error2_ExpectedCostLower") %></div>
                <div ph="3"><%=RS("Error3_NoneUsed") %></div>
                <div ph="4"><%=RS("Error4_SomeUsed") %></div>
                <div ph="5"><%=RS("Error5_CreditMissing") %></div>
                <div ph="6"><%=RS("Error6_SomeCreditMissing") %></div>
                <div ph="7"><%=RS("Error7_GiftNotActive") %></div>
                <div ph="8"><%=RS("Error8_TreasuryLimit") %></div>
                <div ph="9"><%=RS("Error9_FarmLImit") %></div>
                <div ph="10"><%=RS("Error10_NeedServant") %></div>
                <div ph="11"><%=RS("YouGotXXUnits") %></div>
                <div ph="12"><%=RSc("HireServant") %></div>
            </div>
    </div>
   
    
</asp:Content>