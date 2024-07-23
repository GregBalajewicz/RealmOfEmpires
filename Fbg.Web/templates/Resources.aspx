<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="Resources.aspx.cs" Inherits="templates_Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    
    <div id="background">
        <img src="https://static.realmofempires.com/images/misc/M_BG_Treasury.jpg" class="stretch" alt="" />
    </div>

    <section id="resources"  >
         
         <section class="Village">

             <div class="topbar" ></div>
             <span class="vilres_nameloc_txt">%village_name%</span> 
             <span>(<span class="vilres_coord">%village_x%, %village_y%</span>)</span>
             <span class="nameCngIcon">
                 <a href="#" class="renV sfx2" ><img src="https://static.realmofempires.com/images/icons/Q_Rename.png" width="35" height="35" /></a>
             </span>
         
             <div class="yeaprogress">
                    <div class="progress-indicator" style="width: 0%;"></div>
                    <div class="yeaLabel"><%=RS("CouncilofElders"/*Council of Elders: %village_loyalty%% Yea*/) %></div>
             </div>
             

             <div class="boostLoyalty">
                 <%if (realm.IsVPrealm){%>
                        <div class="boostLoyaltyBlock" >
                            <div class="boostLoyaltyText" ><%=RS("CharismaBoost") %></div>
                            <div class="ButtonTouch smallRoundButtonDark buttonRight">
                                <div class="boostLoyaltyClick sfx2" ></div>
                            </div>
                        </div>
                        <div class="notEnoughServants"><%=RS("NotEnough") %>
                            <br /><br />
                            <a class="sfx2 customButtomBG" onclick="ROE.Frame.showBuyCredits()"><%=RSc("HireServant") %></a>
                        </div>
                 <%}%>
             </div>
                           
        </section>

         <section class="resourcesSilver">
                <img src="https://static.realmofempires.com/images/BuildingIcons/m_treasury.png" />
                <div class="silverText">                    
                    <%=RS ("Silver") %>
                    <br /> 
                    <div class="plainLabel SilverCoins"><span class="lblCoins realtimeSilverUpdate" data-vid="%village_id%">%village_coins%</span> / %village_tresMax%</div>
                    <div class="plainLabel">%village_coinsPerHour% <span class="mini"><%=RS("EveryHour") %></span></div>
                </div>                                          
                <div class="smallRoundButtonDark ButtonTouch sfx2 quickTransport buttonRight" onclick="ROE.Frame.showSilverTransportPopup('%village_id%',true);" style="display:none;" >
                    <div class="sendSilverClick">+</div>
                </div>                                      
                                   
         </section>

         <section class="resourcesFarm"> 
                    <img src="https://static.realmofempires.com/images/BuildingIcons/m_farm2.png" />
                    <span class="label"><%=RS ("Food") %></span><br />
                    <div class="plainLabel">%village_food% <span class="mini"><%=RS("used") %></span> / %village_maxFood% <span class="mini"><%=RS("capacity") %></span></div>
                    <div class="plainLabel">%village_foodRem% <span class="mini"><%=RS("Remaining") %></span></div>
         </section>
         <div class="separator" ></div>

         <section class="bonusVillage" >             
                <div class="separator" ></div>
                <div class="bonusTitle">
                    <div class="bonusIcon" ></div>
                    <div class="yellowLabel"><%=RS("Bonus"/*BONUS VILLAGE! %village_bonusVillage%*/) %></div>
                </div>
                <br></br><div class="bonusVillChange customButtomBG sfx2"><%=RS ("ChangeBonusType") %></div>
        </section>
                        
    </section>

</asp:Content>
