<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="HireResearcherTempl.aspx.cs" Inherits="templates_HireResearcherTempl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    <section class="hireResearcher containerBlock">        
        <div class="rowBlock">            
            <div class='icon' style='background-image:url(%researcherAvatar%);'></div>            
            <div class="leftContentColumn">
                <p class='fontGrayFrLClrg'><%=RS("givesYou1ExtraResearcher") %></p>
            </div>           
        </div>        
        <div class="sectionDivider"></div>
        <div class="rowBlock">
            <a class='hireBtn BtnBMed1 fontGoldFrSCxlrg' onclick="ROE.Research.hireResearcher();"><%=RS("hire") %></a>
            <div class="rightContentColumn">
                <p class='fontGoldFrSClrg2 hireResearcherP'><%=RS("hireResearcherFor50") %></p>
            </div>            
        </div>
    </section>

    <section class="notEnough containerBlock" style="display:none;">
        <div class="rowBlock">
            <div class='icon servants' style='background-image:url(https://static.realmofempires.com/images/M_BuyProductID_1000servants.png);'></div>   
            <div class="rightContentColumn2 centerContent">
                <p class='fontGoldFrSCxlrg'><%=RS("notEnoughServants") %></p>
                <p class='fontGrayFrLClrg'><%=RS("hireMoreServants") %></p>
                <div class='hireBtn BtnBMed1 fontGoldFrSCxlrg' onclick="$(this).closest('.simplePopupOverlay').remove(); ROE.Frame.showBuyCredits();"><%=RS("hire") %></div>
            </div>
            
        </div>
    </section>

    <section class="researcherHired containerBlock" style="display:none;">
        <div class="rowBlock">
            <div class='icon' style='background-image:url(%researcherAvatar%);'></div>   
            <div class="rightContentColumn2">
                <p class='fontGoldFrSCxlrg'><%=RS("researcherHired") %></p>
                <p class='fontGrayFrLClrg'><%=RS("startResearching") %></p>                
            </div>            
        </div>        
    </section>

    <section class="maxResearchers containerBlock" style="display:none;">
        <div class="rowBlock">
            <div class='icon' style='background-image:url(https://static.realmofempires.com/images/icons/M_ResearchList.png);'></div>   
            <div class="rightContentColumn2">
                <p class='fontGoldFrSCxlrg'><%=RS("hireCancelled") %></p>
                <p class='fontGrayFrLClrg'><%=RS("haveMaxResearchers") %></p>                
            </div>            
        </div>        
    </section>

    <section class="otherFailure containerBlock" style="display:none;">
        <div class="rowBlock">
            <div class='icon' style='background-image:url(%researcherAvatar%);'></div>   
            <div class="rightContentColumn2">
                <p class='fontGoldFrSCxlrg'><%=RS("failedToHire") %></p>
                <p class='fontGrayFrLClrg'><%=RS("tryAgainLater") %></p>                
            </div>            
        </div>        
    </section>

    
</asp:Content>