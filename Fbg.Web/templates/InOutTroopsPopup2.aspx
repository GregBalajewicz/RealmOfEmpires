<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="InOutTroopsPopup2.aspx.cs" Inherits="templates_InOutTroopsPopup2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    
  <%if (isMobile) { %>
 <div id="background">
                    <img src="https://static.realmofempires.com/images/misc/SplashScreenMuted.jpg" class="stretch" alt="" />
                </div>
    <%} %>
    <div class="templ_inoutpopup inOut2">

        <section class="themeM-tabPanel noBoxAroundContent clearfix">
            <section class="themeM-tabContent">
                <div class="fg vilres_trooptable_cell">
                    <div class="tabContent tabs-in incomingContainer"></div>
                    <div  class="tabContent tabs-out outgoingContainer"> </div>                 
                </div>             
            </section>
        </section>

        <div class="switchModeArea">
            <div class="fontSilverFrSClrg switchModeBtn toInc sfx2" onclick="ROE.Troops.InOutWidget.inoutPopupSwitchMode('toInc');"><%=RS("Incoming")%></div>
            <div class="fontSilverFrSClrg switchModeBtn toOut sfx2" onclick="ROE.Troops.InOutWidget.inoutPopupSwitchMode('toOut');"><%=RS("Outgoing")%></div>
            <div class="inactive reload sfx2" style="opacity:0.5;"></div>
            <div class="outgoing reload sfx2"></div>
            <div class="incoming reload sfx2"></div>
        </div>
        
    </div>

</asp:Content>
