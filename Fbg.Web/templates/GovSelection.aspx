<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="GovSelection.aspx.cs" Inherits="templates_GovSelection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <div id="GovSelection">
        <img class="closeX pressedEffect" src="https://static.realmofempires.com/images/icons/M_X.png" />
        <CENTER><span class="title">View and Select Government Type</span><span class="title ViewOnly" style="display:none;">Explore Government Types</span></CENTER>
        
        
        
        <% if(realm.ID >= 100 || //for new realms
               FbgPlayer.Realm.ID == 0 || FbgPlayer.Realm.ID == 13 || FbgPlayer.Realm.ID == 21 || //for RSs
               FbgPlayer.Realm.ID < 0) { //for RXs
               %>
        
        <div class="govTypes">
            <!--Monarchy-->
            <div class="govType" data-govtype="1">
                <div class="closeX pressedEffect"></div>
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Monarchy1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Monarchy2.jpg" />
                    <span class="arrow"></span>
                </div>
                <span class="desc"> A balanced option for aspiring rulers. Power resides solely in your hands, and all must recognize your greatness. (Not all your peasants agree, though... the unwashed fools!).</span>

                <li>+30% Build Speed </li>
                <li>+10% Barracks Recruitment</li>
                <li>+10% Stables Recruitment</li>
                <li>+10% Attack Factor Bonus</li>
                     <center>
                    <div class=acceptButton ></div></center>
            </div>
            <!--Republic-->
            <div class="govType" data-govtype="2"> 
                <div class="closeX pressedEffect"></div>
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Republic1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Republic2.jpg" />
                    <span class="arrow"></span>
                </div>
                <span class="desc">Greater freedom leads to greater trade and more productive citizens. While your people are just as likely to become artists as soldiers, a Republic at full strength is a fearsome foe.</span>
                <li>+10% Silver
                <li>+20% Farm
                <li>+50% Tavern Recruitment
                <li>Spy Research Unlocked
                     <center>
                    <div  class=acceptButton></div></center>
                    </div>
            <!--Barbarian-->
            <div class="govType" data-govtype="3">
                <div class="closeX pressedEffect"></div>
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Barbarian1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Barbarian2.jpg" />
                    <span class="arrow"></span>
                </div>
                 <span class="desc">You are the horde! Yours is the axe, the sword, the spear! What you can’t pillage, you burn, and what you can’t burn... just kidding, everything burns. (Experienced generals only).</span>
                <li>+40% Stables Recruitment
                <li>+50% Siege Workshop Recruitment
                <li>Light Cavalry Research Unlocked
                     <center>
                    <div  class=acceptButton></div></center>
            </div>
            <!--Merchant House-->
            <div class="govType" data-govtype="4">
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Merchant1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Merchant2.jpg" />
                    <span class="arrow"></span>
                </div>
                 <span class="desc"> They say silver is the life-blood of an empire, and you have silver to spare. Your strength lies in endless treasuries of coin, yet beware, lest jealous empires come to steal your wealth.</span>
                <li>+30% Silver
                <li>+50% Treasury Capacity
                <li>+100% Trading Post
                     <center>
                    <div  class=acceptButton></div></center>
            </div>
            <!--Theocracy   -->
            <div class="govType" data-govtype="5">
                <div class="closeX pressedEffect"></div>
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Theocracy1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Theocracy2.jpg" />
                    <span class="arrow"></span>
                </div>
                 <span class="desc">Faith keeps your people strong and unified. Your people possess an unwavering tenacity and indefatigable vigilance when it comes to defending your lands.</span>
                <li>+10% Build Speed
                <li>+40% Barracks Recruitment
                <li>+20% Village Defense Factor
                <li>Infantry Research Unlocked
                <center><div  class=acceptButton></div></center>
            </div>
        </div>


       <% } else { %>

          <!--Monarchy-->
            <div class="govType" data-govtype="1">
                <div class="closeX pressedEffect"></div>
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Monarchy1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Monarchy2.jpg" />
                    <span class="arrow"></span>
                </div>
                <span class="desc"> A balanced option for aspiring rulers. Power resides solely in your hands, and all must recognize your greatness. (Not all your peasants agree, though... the unwashed fools!).</span>

                <li>+25% Build Speed </li>
                <li>+15% Barracks Recruitment</li>
                <li>+15% Stables Recruitment</li>
                <li>+5% Wall & Tower Bonus</li>
                     <center>
                    <div class=acceptButton ></div></center>
            </div>
            <!--Republic-->
            <div class="govType" data-govtype="2"> 
                <div class="closeX pressedEffect"></div>
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Republic1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Republic2.jpg" />
                    <span class="arrow"></span>
                </div>
                <span class="desc">Greater freedom leads to greater trade and more productive citizens. While your people are just as likely to become artists as soldiers, a Republic at full strength is a fearsome foe.</span>
                <li>+10% Silver
                <li>+20% Farm
                <li>+50% Tavern Recruitment
                <li>Spy Research Unlocked
                     <center>
                    <div  class=acceptButton></div></center>
                    </div>
            <!--Barbarian-->
            <div class="govType" data-govtype="3">
                <div class="closeX pressedEffect"></div>
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Barbarian1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Barbarian2.jpg" />
                    <span class="arrow"></span>
                </div>
                 <span class="desc">You are the horde! Yours is the axe, the sword, the spear! What you can’t pillage, you burn, and what you can’t burn... just kidding, everything burns. (Experienced generals only).</span>
                <li>+40% Stables Recruitment
                <li>+50% Siege Workshop Recruitment
                <li>Light Cavalry Research Unlocked
                     <center>
                    <div  class=acceptButton></div></center>
            </div>
            <!--Merchant House-->
            <div class="govType" data-govtype="4">
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Merchant1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Merchant2.jpg" />
                    <span class="arrow"></span>
                </div>
                 <span class="desc"> They say silver is the life-blood of an empire, and you have silver to spare. Your strength lies in endless treasuries of coin, yet beware, lest jealous empires come to steal your wealth.</span>
                <li>+33% Silver
                <li>+33% Treasury Capacity
                <li>+100% Trading Post
                     <center>
                    <div  class=acceptButton></div></center>
            </div>
            <!--Theocracy   -->
            <div class="govType" data-govtype="5">
                <div class="closeX pressedEffect"></div>
                <div class="cf">
                  <img class="bottom" src="https://static.realmofempires.com/images/illustrations/Gov_Theocracy1.jpg" />
                  <img class="top" src="https://static.realmofempires.com/images/illustrations/Gov_Theocracy2.jpg" />
                    <span class="arrow"></span>
                </div>
                 <span class="desc">Faith keeps your people strong and unified. Your people possess an unwavering tenacity and indefatigable vigilance when it comes to defending your lands.</span>
                <li>+10% Build Speed
                <li>+33% Barracks Recruitment
                <li>+20% Wall & Tower Bonus
                <li>Infantry Research Unlocked
                <center><div  class=acceptButton></div></center>
            </div>

       <% } %>

       
        <center>
         <div class="phrases">
            <div ph="1">Saving your selection....</div>
            <div ph="2">Please make a selection first.</div>
         </div>
            </center>
    </div>





</asp:Content>
