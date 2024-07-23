<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="Credits.aspx.cs" Inherits="templates_Credits" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    
    <div id="background">
        <img src="https://static.realmofempires.com/images/misc/M_BG_Treasury.jpg" class="stretch" alt="" />
    </div>

    <section id="credits" class="themeM-page" >

        <section id="typicalPurchase">
            <div class="bg">
                <div class="corner-tl"></div>
                <div class="corner-br"></div>
            </div>
            <div class="fg"<% if(isMobile) { %> style="overflow:auto;" <% } %>>
                <section class="themeM-panel description">
                    <div class="bg">
                    </div>
                    <div class="fg">
                        <span class="headerMessage"><%=RS("Servants"/* You currently have %credits% servants*/)%></span>
                        <span class="headerMessageSale"><%=RS("Servants_Sale"/* You currently have %credits% servants*/)%></span>
                        <BR>
                        <span class="offer2Msg"><%=RS("ServantOffer500")%></span>
                    </div>
                </section>
            
                <section class="themeM-panel creditpackages">
                    <div class="bg">
                        <div class="themeM-squiggle-t"></div>
                        <div class="themeM-squiggle-b"></div>
                    </div>
                    <div class="fg">                        
                        <br />
                        <section class="creditpackagerow template">
                            <table class ="creditpackagetable">
                                <tr class ="creditpackagetablerow">
                                    <td>
                                        <section id="creditpackage1" class="themeM-panel creditpackage ButtonTouch sfx2">
                                            <div class="bg hidden">
                                                <div class="corner-tl"></div>
                                                <div class="corner-br"></div>
                                            </div>
                                            <div class="fg hidden">
                                                <img src="" /><br />
                                                <span class="credits"></span><br />                                      
                                                <span class="price"></span>                                        
                                                <br /><span class="salePrice">&nbsp;</span>                                        
                                            </div>
                                        </section>
                                    </td>
                                    <td>
                                        <section id="creditpackage2" class="themeM-panel creditpackage ButtonTouch sfx2">
                                            <div class="bg hidden">
                                                <div class="corner-tl"></div>
                                                <div class="corner-br"></div>
                                            </div>
                                            <div class="fg hidden">
                                                <img src="" /><br />
                                                <span class="credits"></span><br />                                            
                                                <span class="price"></span>                                        
                                                <br /><span class="salePrice">&nbsp;</span>                                        
                                            </div>
                                        </section>
                                    </td>
                                    <td>
                                        <section id="creditpackage3" class="themeM-panel creditpackage ButtonTouch sfx2">
                                            <div class="bg hidden">
                                                <div class="corner-tl"></div>
                                                <div class="corner-br"></div>
                                            </div>
                                            <div class="fg hidden">
                                                <img src="" /><br />
                                                <span class="credits"></span><br />                                          
                                                <span class="price"></span>                                        
                                                <br /><span class="salePrice">&nbsp;</span>                                        
                                            </div>
                                        </section>
                                    </td>
                                </tr>
                            </table>
                        </section>                    
                    </div>
                </section>
                <center>All prices are in USD</center>

                <div class="otherPayment BtnBXLg1" style="display:none"><%=RS("OtherPaymentOptions")%></div>

                <div class="transferBtn">
                
                    <div class="actionButton BtnBLg2 fontButton1L " onclick="ROE.Credits.showTransferPopup();">Transfer Servants</div>
                    <!--<a href="pfTransferCredits.aspx" class="actionButton BtnBLg2 fontButton1L">Transfer Servants</a>-->
                </div>

            </div>  
        </section>

        
        <section id="kongPurchase" class="themeM-page" >
            <div class="bg">
                <div class="corner-tl"></div>
                <div class="corner-br"></div>
            </div>
            <div class="fg"<% if(isMobile) { %> style="overflow:auto;" <% } %>>
                <section class="themeM-panel description">
                    <div class="bg">
                    </div>
                    <div class="fg">
                        <span class="headerMessage"><%=RS("KongMsg")%></span>
                    </div>
                </section>
               </div>
        </section>
    </section>

    <div class="otherPaymentContent" >
        <div class="otherPaymentContentBox" >
            <span><%=RS("PreferredPayment")%></span>
            <div class="otherPaymentContentClick BtnBLg2" ><%=RS("MobileCarrier")%></div>
         </div>
    </div>




</asp:Content>
