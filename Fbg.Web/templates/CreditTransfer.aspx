<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="CreditTransfer.aspx.cs" Inherits="templates_CreditTransfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    
    <div id="background">
        <img src="https://static.realmofempires.com/images/misc/M_BG_Generic.jpg" class="stretch" alt="" />
    </div>

    <section id="transfercredits" class="themeM-page" >
        <div class="bg">
            <div class="corner-tl"></div>
            <div class="corner-br"></div>
        </div>
        <div class="fg mainSection">
            <section class="themeM-panel description">
                <!--<div class="bg">
                </div>-->
                <div class="fg fontSilverFrLCmed ">
                    <div class="textWrapper">
                        <div class="fontGoldFrLCmed title"><%= RS("HereTransferServants")%></div>
                        <div class="body"><%= RS("TransferDisclaimer")%>
                        </div>
                        <%= RS("TransferRestrictions")%>
                        <div class="fontGoldFrLCmed title hideAfterSuccess"><%= RS("YouHave")%> <span class="maxTransferAvail">0</span> <%= RS("ServantsAvailable")%></div>
                    </div>

                    <table class="inputlayout hideAfterSuccess">
                        <tr>
                            <td><%= RS("Amount")%></td>                            
                            <td><input type="number" id="transferQuantity" class="stylishInputBox" min="5" max="1111111" value="5" onfocus="ROE.Frame.inputFocused();" onblur="ROE.Frame.inputBlured();" /></td>
                        </tr>
                        <tr class="hideAfterSuccess">
                            <td>To Player</td>
                            <td><input type="text" id="transferToPlayer" class="stylishInputBox" onfocus="ROE.Frame.inputFocused();" onblur="ROE.Frame.inputBlured();" />
                                
                            </td>
                        </tr>
                       
                        <!--<tr class="notEnoughTransferAvail resultsMessage">
                            <td colspan="2">
                                <div class="fontGoldFrLCmed title reqNotMet"><%= RS("NotEnoughServants")%></div>
                            </td>
                        </tr>-->
                        <tr>
                             <td></td>
                             <td>
                                 <div class="searchResultsArea">
                                    <ul class="transferSearchList"></ul>  
                                    <div class="loadingSearchList"></div>
                                </div>
                             </td>
                         </tr>
                    </table>
                   
                   
                    
                    <div class="actionButton BtnBLg1 fontButton1L centerBtn hideDuringConfirm transferBtn" onclick="ROE.Credits.transferCredits();"><%= RS("Transfer")%></div>
                     <div class="confirmTransferBlock">
                        <div class="fontGoldFrLCmed title"><%=RS("PleaseConfirm") %></div>
                        <div class="body"><%=RS("Transfering") %> <span class="numServantsToTransfer">0</span> <%=RS("ServantsTo") %> <a class="sendingToPlayer" href="#">no one.</a></div>
                        <div class="body"><%=RS("TapToReviewPlayer") %></div>
                        <div class="body"><%=RS("Proceed") %></div>
                        <div class="buttonWrapper">
                            <div class="transferYES BtnBSm2 fontSilverFrSClrg"><%=RS("Yes") %></div><div class="transferNO BtnBSm2 fontSilverFrSClrg"><%=RS("No") %></div>
                        </div>
                    </div>
                    
                    <div class="fontGoldFrLCmed title transferSuccessMsg"><%= RS("TransferSuccess")%></div>
                    <div class="fontGoldFrLCmed title reqNotMet transferFailMsg"><%= RS("NotEnoughServants")%></div>
                   
                </div>
            </section>

        </div>  
    </section>

    <div class="paddinator2000"></div>

    <div id="CreditTransferPhrases" style="display:none;">
        <div ph="ErrorSteward"><%=RS("ErrorSteward") %></div>
        <div ph="ErrorAmount"><%=RS("ErrorAmount") %></div>
        <div ph="ErrorToPlayer"><%=RS("ErrorToPlayer") %></div>
        <div ph="ErrorYourself"><%=RS("ErrorYourself") %></div>
        <div ph="ErrorCatchAll"><%=RS("ErrorCatchAll") %></div>
        <div ph="NotEnoughServants"><%=RS("NotEnoughServants") %></div>
        <div ph="NoMatching"><%=RS("NoMatching") %></div>
    </div>

</asp:Content>
