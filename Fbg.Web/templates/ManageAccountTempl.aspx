<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="ManageAccountTempl.aspx.cs" Inherits="templates_ManageAccountTempl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <%if (isMobile) { %>
   <div id="background" style="display:none;">
        <img src="https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg" class="stretch" alt="" />
    </div>
    <%} %>

    <section id="account_popup" class="themeM-page">
        <div class="fg main-fg">
            <section class="themeM-view accountPages default">              
                <table>
                    <tr>
                        <td>
                            <div class="contentWrapper">
                                <h1 class="fontGoldFrLClrg"><%=RS("TitleProtect") %></h1>
                                <% if (isMobile) {%> <%=RS("Page1EmailNotVerified") %> <% } else { %> <%=RS("Page1EmailNotVerified_d2") %>   <% } %>
                            </div>
                        </td>
                    </tr> 
                    <tr>
                        <td class="buttonColumn"><input type="text" id="accountRecoveryEmail" class="stylishInputBox" /></td>
                    </tr>
                    <tr>
                        <td class="buttonColumn"><div class="BtnBLg1 fontButton1L" onclick="ROE.Account.recoveryEmail_check();"><%=RS("Ok") %></div></td>
                    </tr>  
                    <tr>
                        <td><div class="contentWrapper"><p class="errMsg"><%=RS("InputErrorInvalidEmail") %></p></div></td>
                    </tr>
                     
                </table>              
            </section> 
            
            <section class="themeM-view accountPages registeredViaMobile">              
                <table>
                    <tr>
                        <td>
                            <div class="contentWrapper">
                                <h1><%=RS("TitleProtect") %></h1>
                                <%=RS("Page1SetupTactica") %>                                
                            </div>
                        </td>
                    </tr>                    
                    <tr>
                        <td class="buttonColumn"><a class="BtnBXLg1 fontButton1L" style="display: inline-block;" href="login_MobToTactica.aspx" ><%=RS("SecureAccount") %></a></td>
                    </tr>
                </table>
            </section>           

            <section class="themeM-view accountPages confirmPage">
             
                <table>
                    <tr>
                        <td colspan="2">
                            <div class="contentWrapper fontSilverFrLCmed">
                                <h1 class="fontGoldFrLClrg"><%=RS("TitleConfirm") %></h1>
                                <%=RS("Page2IsCorrect") %><!--Is <span class="currEmailAccount"></span> correct?-->
                            </div></td>
                    </tr>
                     <tr>
                        <td class="buttonColumn"><div class="BtnBLg1 fontButton1L" onclick="ROE.Account.recoveryEmail_handleAccept();"><%=RS("Confirm") %></div></td><td class="buttonColumn"><div class="BtnBLg1 fontButton1L" onclick="ROE.Account.recoveryEmail_handleCancel();"><%=RS("Cancel") %></div></td>
                    </tr>
                </table>               
            </section>
            <section class="themeM-view accountPages responsePage">
            
                <table>
                    <tr>
                        <td>
                            <div class="contentWrapper fontSilverFrLCmed">
                                 <h1 class="fontGoldFrLClrg responseTitle"><%=RS("TitleResponse") %></h1>
                                 <span class="responseMsg fontDarkGoldFrLCmed"></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="buttonColumn"><div class="BtnBLg1 fontButton1L" onclick="ROE.Account.recoveryEmail_handleBack();"><%=RS("Back") %></div></td>
                    </tr> 
                </table>
            </section>
        </div>
        <div id="AccountPhrases" style="display:none;">
            <%= RSdiv("SubmitResponseOK") %>  
            <%= RSdiv("SubmitResponseEmailTaken") %>  
            <%= RSdiv("SubmitResponseError") %>           
        </div>

    </section>
</asp:Content>