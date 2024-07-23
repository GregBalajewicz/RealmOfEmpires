<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="GiftsPopup_d2.aspx.cs" Inherits="templates_GiftsPopup_d2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
    <div id="fb-root"></div>
    <script src="https://connect.facebook.net/en_US/all.js"></script>   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
     
    <div id="gifts_popup" >
            
            <div class="giftList transition">
                <div class="giftContainer bg">
                    <h1><%=RS("Title") %><div class="smallRoundButtonDark infoBtn helpTooltip" data-tooltipid="getMoreInfo"><div class="icon"></div></div></h1>
                    <% if (LoginModeHelper.isFB(Request)){ %>
                        <a class="sendGiftsToggleBtnWrapper" href="javascript:ROE.Frame.popupGiftSend();"><span class="sendGiftsToggleBtn customButtomBG sfx2"><%=RS("SendToFriends") %></span></a>
                    <%} %>
                    <div id="giftdaily"><%=RS("YouCanUse") %></div>
                    <div class="giftsIconSheet RowTouch" data-giftindex="%gift.index%" data-giftcnt="%gift.available%" id="Gift_%gift.id%" >
                        <div class="giftsIcon" style="background-image:url('%gift.img%')"></div>
                        <div class="giftDescriptionWrapper">
                            <span class="sendItemToFriend"><%=RS("Send") %></span>  
                            <%=RS("giftName") %> 
                            <span class="availableText"><%=RS("availableText") %></span>
                        </div>
                        <img src='https://static.realmofempires.com/images/misc/M_ArrowR.png' class="giftRArrow" />
                    </div>
                    <div class="giftsIconSheetNot" data-giftindex="%gift.index%" data-giftcnt="%gift.available%"   id="Gift_%gift.id%" >
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

                        <div class="inputControl">                                    
                            <input class="giftUnits" style="width: 240px;" type="number" value="0" onchange="ROE.Gifts.selectedItemsChangeFromInput();" onkeyup="ROE.Gifts.selectedItemsChangeFromInput();" />                                 
                        </div>

                        <div class="maxButtons">
                            <div class="BtnDLf1 fontButton1L maxFree"></div>
                            <div class="BtnDLf1 fontButton1L max"></div>
                        </div>
                        <div class="fontGoldFrLClrg summeryPanel">
                            <div class="summaryL1" style="color:white;">You are getting:</div>
                            <div class="summaryL2" style="color:white;"></div>
                            <div class="summaryL3" style="color:white;"></div>
                            <div class="summaryL4"></div>
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
                <div>Daily limit resets at 12:00AM Game Time (UTC)</div><br />
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
                <div ph="13"><%=RS("SendToFriends") %></div>
                <div ph="14"><%=RS("TurnOffSendToFriends") %></div>
            </div>
    </div>
   
    
</asp:Content>