<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Quests.ascx.cs" Inherits="Controls_Quests" %>
<%if (IsMobile) {%>
<style>
    .masteryQuests .title 
    {display:none;}
</style>
<%} else if(IsD2) { %>
<style>
.cph1_Quests1_panelClaimReward {
    padding-top:20px;
}

    .getreward {
        margin-top:10px;
    }
</style>
<%} else { %>
<%} %>
<table border="0" cellpadding="0" cellspacing="0" class="Box masteryQuests" width="100%" >
    <tr>
        <td class="" colspan=2 >
            <asp:Panel ID="panelReward" runat="server" Visible="False">                
                <asp:Label ID="lblTitle" CssClass=title runat="server"
                    Font-Italic="True"></asp:Label>
                <br />
               
            </asp:Panel>
           
            <asp:Panel ID="panel_QuestCompletedTitle" runat="server">
                <em>Quest Completed!</em>
                <br />           
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td class="" colspan=2 >
            <asp:Panel ID="panel_QInvite" runat="server" Visible="False">
                Realm Of Empires is designed to be played in co-operation with other players.
                               <center></VR><BR /><BR /><B>More Friends = More Power!</B></center>
                <br /><br />
                <img src= "https://static.realmofempires.com/images/navIcons/Invite1.gif" align="left" style="padding-right: 5px" />Locate this invite button on top right of the screen and click it to invite friends and obtain your reward!!
                </asp:Panel>

            <asp:Panel ID="panel_QFriends" runat="server" Visible="False">
                There are several ways of finding out which of your friends are already playing Realm Of Empires.
                <br /><br />
                <img src= "https://static.realmofempires.com/images/navIcons/settings1.gif" align="left" style="padding-right: 5px" />Your quest is to find out how many of your friends are playing on this realm. To do this, select
                 'Switch Realm' from the Tools menu . This will take you to a page listing the realms. 
                 Click the 'Show my Facebook Friends' link to see a list of all your friends playing on this realm.
                <br /><br />
                Enter the number of your friends in this realm here and click 'Claim Your Reward' below.
                <br /><br />
                # of my friends on this realm: <asp:TextBox ID="txt_QFriends" runat="server" CssClass="TextBox" Width="40px" style="font-size:13pt;" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txt_QFriends"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txt_QFriends"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>
            </asp:Panel>
            <asp:Panel ID="panel_QSilver" runat="server" Visible="False">
                 <%if (IsMobile) {%>                
                Enter your current silver amount and treasury capacity.
                <br />                    
                <%}
                  else { %>
                <img src= "https://static.realmofempires.com/images/Misc/treasury.png" align="left" style="padding-right: 5px" /> There are two things every ruler should know - how much silver your mines are producing, and how much your treasury can hold.
               
                <br /><br />
                 Otherwise you risk involuntarily 
                distributing your wealth to your subjects and what kind of medieval ruler would you be?
                <br /><br /> 
                Enter below your current silver totals and treasury size and click 'Claim Your Reward' below.
                <br /><br />
                <%} %>
                Current Silver: <asp:TextBox ID="txt_QSilver_Silver" runat="server"  pattern="[0-9]*"  CssClass="TextBox" Width="80px" style="font-size:13pt;" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txt_QSilver_Silver"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txt_QSilver_Silver"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>
                <br />
                Treasury Capacity: <asp:TextBox ID="txt_QSilver_Treasury" runat="server"  pattern="[0-9]*"  CssClass="TextBox"
                 Width="80px" style="font-size:13pt;" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txt_QSilver_Treasury"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txt_QSilver_Treasury"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>
                <br /><br />

                <%if (IsMobile) {%>
                    Close this screen and click the pile of coins in the top right of your Village Overview to find the answer.
                 <% } else if (IsD2) {%>
                    These values are found in the Village Overview panel at the bottom next to the icon:<br /><img src="https://static.realmofempires.com/images/icons/Silver44.png" /><br />There are two numbers: the top number is the silver you have in that village and the bottom is the total amount you can hold (the village's Treasury capacity). The Village Overview panel can always be found in the lower-left corner of the screen.
                <%}
                  else { %>
                    Both numbers are on very top left corner of your screen. If you are unable to locate these numbers, I suggest you go over the beginner tutorial again. 
                <%} %>

            </asp:Panel>
            <asp:Panel ID="panel_QFood" runat="server" Visible="False">
                <img src= "https://static.realmofempires.com/images/misc/farm.png" align="left" style="padding-right: 5px" />            
                They say an army marches on its stomach, and yours is no exception!
                <br /><br /><%if (IsMobile) {%>Food supply and consumption is found on the same screen you just checked for silver. Enter these amounts below.
                
                <%} else if(IsD2) { %>
                 Farmland produces the food that supports your kingdom and armies. Without enough food supply, you will be unable to construct new buildings or recruit more troops. 
                <br /><br />
                The current food supply remaining, just like silver, can be found on the Village Overview panel next to the icon:<br /><img src="https://static.realmofempires.com/images/icons/Sheep44.png" /><br />Enter this number below and click 'Get Reward' to claim your reward.
                <br /><br />
                 Food remaining:<asp:TextBox ID="txt_QFood_Remain" runat="server" CssClass="TextBox"
                 Width="40px" style="font-size:13pt;" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" ControlToValidate="txt_QFood_Remain"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator13" runat="server" ControlToValidate="txt_QFood_Prod"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>
                <br />
               <%} else  { %>
                Farmland produces the food that supports your kingdom and armies. Without enough food supply, you will be unable to construct new buildings or recruit more troops. 
                <br /><br />
                Enter below your current food consumption, and your current food production and click 'Claim Your Reward' below.
                <%}
                  
                  if(!IsD2) { %>          
                Food consumption:<asp:TextBox ID="txt_QFood_Consumption" runat="server" CssClass="TextBox" Width="40px" style="font-size:13pt;" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txt_QFood_Consumption"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txt_QFood_Consumption"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>
                <br />
               
                Food production:<asp:TextBox ID="txt_QFood_Prod" runat="server" CssClass="TextBox"
                 Width="40px" style="font-size:13pt;" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txt_QFood_Prod"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txt_QFood_Prod"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>
                <br />
              
                <%} %>
                <br />

                <%if (!IsMobile && !IsD2) {%>Both numbers are on top of your screen, to the right of your silver. If you are unable to locate these numbers, I suggest you go over the beginner tutorial again.<%} %>
            </asp:Panel>
            <asp:Panel ID="panel_QMyPoints" runat="server" Visible="False">
                'Village points' reflect your growth and power. 
                <br />
                <br />
                Each new building and upgrade you construct increases your village points. 
                This also gives you an idea of the overall strength of your neighbours' villages. 
                <br />
                <br />
                 <%if (IsMobile) {%>
                     Close this screen, and you points are on top left of the screen, inside the progress bar with your player name in it. 
                 <%}else if(IsD2) { %>
                Your points are displayed next to your village name and village coordinates. This can be found in the top header of the Village Overview panel.
                <% } else { %>
                    Your points are displayed on top right of the screen. Look to the right of your current title, next to your avatar image. 
                 <%}%>
                
                <br />
                <br />            
                My current village points:<asp:TextBox ID="txt_QMyPoints_Points" runat="server" CssClass="TextBox"
                    Width="40px" style="font-size:13pt;" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ValidationGroup="oldquests" runat="server" ControlToValidate="txt_QMyPoints_Points"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" >Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ValidationGroup="oldquests" runat="server" ControlToValidate="txt_QMyPoints_Points"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" >Only numbers allowed</asp:RegularExpressionValidator>
                <br />
            </asp:Panel>
            <asp:Panel ID="panel_QJoinClan" runat="server" Visible="False">
                 <%if (IsD2) {%>
                    <img src= "https://static.realmofempires.com/images/icons/m_Clan.png" align="left" style="padding-right: 5px" />
                    Joining a clan is essential to your empire's survival. Here are a few things to consider when choosing a clan.
                    <br />
                    <ul>
                    <li>Read a clan's profile to learn more about it.</li>
                    <li>Look for clans with strong, active members.</li>
                    <li>Make sure some of those members are near you!</li>
                        </ul>
                    Join a clan now and claim your reward. 
                    <br /><br /> PS, creating a dummy clan of one will not do!  
                <br />
                <%}else { %>
                    <%if (!IsMobile) {%><img src= "https://static.realmofempires.com/images/NAVICONS/CLAN1.gif" align="left" style="padding-right: 5px" /><%} %>
                    Joining a clan is essential to your empire's survival. Here are a few things to consider when choosing a clan.
                    <br /><br />
                    <ul>
                    <li>Read a clan's profile to learn more about it.</li>
                    <li>Look for clans with strong, active members.</li>
                    <li>Make sure some of those members are near you!</li>
                        </ul>
                    <br /><br />
                    Join a clan now and claim your reward   
                    <br /><br /> PS, creating a dummy clan of one will not do!
                 <%}%>            
            </asp:Panel>
            <asp:Panel ID="panel_QClan_Leaders" runat="server" Visible="False">
                
                Now that you are part of a clan, it's time to learn more about it. You need to know how leadership is structured, and who is in charge.

                <Br /><br />All clans have at least one owner, the ultimate administrator of the clan - someone who created the clan or someone whom the creator granted this role.
                
                <%if (IsMobile) {%>
                    <br /><br />Close this screen, click on clan <img src= "https://static.realmofempires.com/images/icons/m_Clan.png" align="left" style="padding-right: 5px" /> 
                    icon on the bottom of screen, then go to the clan <b>members</b> page and find one of the owners. 
                <%} else if(IsD2) { %>
                    <br /><br /><img src= "https://static.realmofempires.com/images/icons/m_Clan.png" align="left" style="padding-right: 5px" /> 
                    Open the Clan Overview page by clicking on the flag icon in the upper-left menu, select the <b>Members</b> page and find a player with <i>Owner</i> highlighted under Roles. 
                <%} else { %>
                    <br /><br /> <img src= "https://static.realmofempires.com/images/NAVICONS/CLAN1.gif" align="left" style="padding-right: 5px" /> 
                    Go to the clan <b>members</b> page and find one of the owners. 
                <%}%>

                <Br /><br />One of my clan's owners is:<asp:TextBox ID="txt_QClan_Leaders" 
                    runat="server" CssClass="TextBox jsplayers ui-autocomplete-input"
                    Width="150px" style="font-size:13pt;" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="txt_QClan_Leaders"
                    CssClass="Error" ValidationGroup="oldquests" Display="Dynamic" ErrorMessage="Enter a number">Enter a name</asp:RequiredFieldValidator>               
            </asp:Panel>
            <asp:Panel ID="panel_QClan_Allies" runat="server" Visible="False">                 
                While your clanmates are your closest compatriots, most clans will also form alliances with other clans.
    
                <br /><br />On the map, your clanmates are marked with a green shield <img src="https://static.realmofempires.com/images/map/fc.png" />
                . Your clan's allies are marked with a purple shield <img src="https://static.realmofempires.com/images/map/fa.png" />.
                
                <%if (IsMobile) {%>
                <br /><br />
                Allies are listed on your clan's <b>diplomacy</b> page.
                <br /> <br />
                Name one of your clan's allies:
                <% } else if(IsD2) { %>
                To display the map legend, open the Map Highlights panel by clicking on a village and selecting the Highlight icon:<br />                
                <img src="https://static.realmofempires.com/images/icons/highlight2.png" /><br />
                You can also customize your own map highlights by adding new rules.<br /><br />
                Besides the map, Allies can also be found listed on your clan's diplomacy page. <br /><br />
                Name one of your clan's allies:          
                <%}
                  else { %><br /><br /><img src="https://static.realmofempires.com/images/map/legend1.png" align="left" style="padding-right: 5px" />  Look for a 
                KEY icon on the map&#39;s border to display the legend.

                <br /><br />
                <img align="left" 
                    src="https://static.realmofempires.com/images/NAVICONS/CLAN1.gif" 
                    style="padding-right: 5px" />Also, you can see the list of your allies from 
                your clan&#39;s <b>Diplomacy</b> page.
                

                <br /> <br />
                Name one clan that is an ally of your clan:<%} %>
                <BR /></BRT><asp:TextBox ID="txt_panel_QClan_Allies" 
                    runat="server" CssClass="TextBox"
                    Width="150px" style="font-size:13pt;" ></asp:TextBox>
                <br />
                * OR * <asp:CheckBox ID="cb_panel_QClan_Allies" Text="We have no allies" runat="server" />
            </asp:Panel>   
            <asp:Panel ID="panel_QClan_Intro" runat="server" Visible="False"><%if (IsMobile) {%>                
                Realm of Empires is a game of grand collaboration and epic warfare. Getting to know your clanmates is important!
                <br /><br />
                For this quest, introduce yourself to the clan. Most clans will have a 'Introduce yourself' thread on the forum. Find the appropriate thread or create one of your own. Say hello to everyone!
                <%}
                  else { %>
                Realm of Empires is a game of grand collaboration and epic warfare - it is VERY difficult to be successful on your own.

                <BR /><BR />The key is working closely with your clanmates and other players around you. Get to know other players near you and in your clan. After all, people like to help friends, not strangers! 

                <BR /><BR />For this quest, introduce yourself to the clan. Most clans will have a 'Introduce yourself' thread on the forum. But be careful not to spam the clan forum! Find the appropriate thread or create one of your own. Treat your clan mates with respect please!

                <%} %><BR /><BR />
                <% if(IsD2) { %>
                 <img align="left" 
                    src="https://static.realmofempires.com/images/icons/m_Clan.png" 
                    style="padding-right: 5px" />So, make at least one post on the <b>Forum</b> to introduce yourself. The Forum is accessed through the Clan panel.
                <% } else { %>
                <img align="left" 
                    src="https://static.realmofempires.com/images/NAVICONS/CLAN1.gif" 
                    style="padding-right: 5px" />So, make at least one post on the <b>forum </b>
                to introduce yourself. 
                <% } %>
            </asp:Panel>             
                     
            <asp:Panel ID="panel_QSilverProduction" runat="server" Visible="False">
                <img src= "https://static.realmofempires.com/images/misc/mine.png" align="left" style="padding-right: 5px" />
                <%if (IsMobile) {%>
                Let's learn more about the silver mine, since it is the source of your empire's wealth. 
                <br /><br />
                Close this screen, tap on your Silver Mine, then tab the Efficiency Info panel to get this information:<br /><br />
                <%}
                  else { %>As you already know, the Silver Mine is your source of wealth. As you upgrade your Silver Mine,
                 your silver production will increase. 
                <br /><br />
                How much silver will your mines produce at a level 40 Silver Mine? 
                <br /><br />
                <%if (IsD2) {%>
                You can find this information in the <i>Building Levels</i> section of the Silver Mine <b>Building</b> page. Click on the Silver Mine in the Village Overview to display its Building page.<br /><br />
                 <% } else { %>
                <img src= "https://static.realmofempires.com/images/navIcons/Help1.gif" align="right" style="padding-left: 5px" />Look this up in the <b>Buildings</b> help page, 
                accessible from the help menu item. 
                <br /><br /><% } } %>               
                A level 40 Mine produces<asp:TextBox ID="txt_QSilverProduction" 
                    runat="server" CssClass="TextBox"
                    Width="60px" style="font-size:13pt;"></asp:TextBox> silver.
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txt_QSilverProduction"
                    CssClass="Error" Display="Dynamic" ValidationGroup="oldquests" ErrorMessage="Enter a number">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ControlToValidate="txt_QSilverProduction"
                    CssClass="Error" Display="Dynamic" ValidationGroup="oldquests" ErrorMessage="Only numbers allowed" ValidationExpression="\d*">Only numbers allowed</asp:RegularExpressionValidator>
                                    
            </asp:Panel>
            <asp:Panel ID="panel_QDefenseFactor" runat="server" Visible="False">
                <img src= "https://static.realmofempires.com/images/misc/Tower_S3R.png" align="left" style="padding-right: 5px;height:100px;" />
                <%if (IsMobile) {%>Walls and towers help your brave troops to defend against enemy attacks.<br /><br />
                Each defensive fortification adds a percentage bonus to your total defense, multiplying your effective number of defending units!
                 <%}
                  else { %>
                Fortify your village by constructing defensive walls and towers. Defensive fortifications provide powerful bonuses to your troops when defending against enemy attacks.
                <br /><br />
                Each level of walls and towers provides a percentage bonus to your defending troops. For example, if your bonus 
                is 200%, that means your troops are fighting as if there was twice as many of them. 
                <%} %> <br /><br />

                <%if (IsMobile) {%>
                    <div>You can find this information in the <i>Building Levels</i> section of the Defensive Towers <b>Building</b> page.</div>
                    <div class="BtnBSm2 fontButton1L" onclick="<% if (IsiFramePopupsBrowser) { %>
                        window.parent.ROE.UI.Sounds.click(); 
                        window.parent.ROE.Building2.showBuildingPagePopup('7');
                        <%} else {%>
                        window.opener.ROE.UI.Sounds.click();                         
                        window.opener.ROE.Building2.showBuildingPagePopup('7');  
                        window.close();                       
                        <%}  %>">Towers</div>

                <% } else if(IsD2) { %>
                <div>You can find this information in the <i>Building Levels</i> section of the Defensive Towers <b>Building</b> page.</div>
                <div class="BtnBSm2 fontButton1L" onclick="window.top.ROE.Building2.showBuildingPagePopup('7');">Towers</div>

                <% } else { %>
                    <img src= "https://static.realmofempires.com/images/navIcons/Help1.gif" align="left" style="padding-right: 5px" />Look this up in the <b>Buildings</b> help page, 
                    accessible from the help menu item. 
                <%}%>

                <br /><br />  
                <img src= "https://static.realmofempires.com/images/BuildingIcons/Tower.png" style="padding-right: 5px" />Level 4 defensive towers grant a <asp:TextBox ID="txt_QDefenseFactor" runat="server" CssClass="TextBox"
                    Width="60px" style="font-size:13pt;"></asp:TextBox>% bonus.
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txt_QDefenseFactor"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests" >Enter a number</asp:RequiredFieldValidator>                
               <br /><br />                              
            </asp:Panel>

            <asp:Panel ID="panel_QInfantryRecruitTime" runat="server" Visible="False">
                <%if (IsMobile) {%>
                Troops recruit slowly over time. "Maxing" out a village can take several weeks of steady recruitment. 
                <br /><br />
                Infantry, recruited at the barracks, is your strongest defensive option. Let's see how long infantry takes to recruit.

                <%} else { %>


                Troops recruit slowly over time. "Maxing" out a village can take several weeks of steady recruitment. 
                <br /><br />
                Keep in mind that Realm Of Empires was specifically designed this way to allow you to play a low maintenance game. 
                <br /><br />
                Infantry, recruited at the barracks, is your strongest defensive option. Lets check how long it takes to recruit Infantry.<%} %> 
                <br /><br />

                <%if (IsMobile) {%>
                    <div>You can find this information in the <i>Troop Details</i> section of the Barracks <b>Building</b> page.</div>
                        <div class="BtnBSm2 fontButton1L" onclick="<% if (IsiFramePopupsBrowser) { %>
                        window.parent.ROE.UI.Sounds.click(); 
                        window.parent.ROE.Building2.showBuildingPagePopup('1');
                        <%} else {%>
                        window.opener.ROE.UI.Sounds.click();                         
                        window.opener.ROE.Building2.showBuildingPagePopup('1');  
                        window.close();                       
                        <%}  %>">Barracks</div>

                 <% } else if(IsD2) { %>
                    <div>You can find this information in the <i>Troop Details</i> section of the Barracks <b>Building</b> page.</div>
                    <div class="BtnBSm2 fontButton1L" onclick="window.top.ROE.Building2.showBuildingPagePopup('1');">Barracks</div>

                <% } else { %>
                    <img src= "https://static.realmofempires.com/images/navIcons/Help1.gif" align="left" style="padding-right: 5px" />Look this up in the <b>Units</b> help page, accessible from the help menu item. 
                <%}%>

                
                <br /><br />
                The base recruitment time for Infantry is <asp:TextBox ID="txt_QInfantryRecruitTime" runat="server" CssClass="TextBox"
                    Width="60px" style="font-size:13pt;"></asp:TextBox> minutes. (Ignore seconds)
                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txt_QInfantryRecruitTime"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ControlToValidate="txt_QInfantryRecruitTime"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>
                                             <br /><br />                               
            </asp:Panel>
            <asp:Panel ID="panel_QBarracksRecruitTime" runat="server" Visible="False">
                <img src= "https://static.realmofempires.com/images/misc/barracks.png" 
                    align="left" style="padding-right: 5px" />
                    <%if (IsMobile) {%>
                    Recruiting units takes time, but you can speed up this process by upgrading your buildings. 
                    <br /><br />
                    For example, the higher the level of barracks, the faster you can recruit each Infantry and Citizen Militia.
                    <%} else { %>
                Troops are recruited in their recruitment buildings like Barracks for Infantry or Stable for Knights.
                <br /><br />
                As you already know, recruiting troops takes time, but you can speed this process up by upgrading the recruitment building. 
                <br /><br />
                For example, the higher the level of barracks, the faster you can recruit each Infantry and Citizen Militia.<%} %> 
                <br /><br />
                
                <%if (IsMobile) {%>
                        <div>You can find this information in the <i>Building Levels</i> section of the Barracks <b>Building</b> page.</div>
                        <div class="BtnBSm2 fontButton1L" onclick="<% if (IsiFramePopupsBrowser) { %>
                        window.parent.ROE.UI.Sounds.click(); 
                        window.parent.ROE.Building2.showBuildingPagePopup('1');
                        <%} else {%>
                        window.opener.ROE.UI.Sounds.click();                         
                        window.opener.ROE.Building2.showBuildingPagePopup('1');  
                        window.close();                       
                        <%}  %>">Barracks</div>
                  <% } else if(IsD2) { %>
                    <div>You can find this information in the <i>Building Levels</i> section of the Barracks <b>Building</b> page.</div>
                    <div class="BtnBSm2 fontButton1L" onclick="window.top.ROE.Building2.showBuildingPagePopup('1');">Barracks</div>
                <% } else { %>
               
                    <img src= "https://static.realmofempires.com/images/navIcons/Help1.gif" align="left" style="padding-right: 5px" />Look this up in the <b>Buildings</b> help page, 
                    accessible from the help menu item. 
                <%}%>

                <br /><br />

                The <B>recruitment time factor</B> for a level 13 <img src= "https://static.realmofempires.com/images/BuildingIcons/barracks.png" style="padding-right: 5px" />Barracks is<asp:TextBox ID="txt_QBarracksRecruitTime" runat="server" CssClass="TextBox"
                    Width="60px" style="font-size:13pt;"></asp:TextBox>%
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="txt_QBarracksRecruitTime"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>                
                              <br /><br />                               
            </asp:Panel>
            <asp:Panel ID="panel_QBattleSimSpies" runat="server" Visible="False">
            <%if (IsMobile) {%>
            Spies help you learn what troop and building levels your opponents have. Your spies also defend against enemy spies!
            <br /><br />
            Go to the Battle Simulator*. Put in 100 attacking spies versus 50 defending spies. Then, enter the missing information below.
            <%} else { %>
                Spies are the medieval 007s! You can use spies to obtain intelligence on other villages. 
                The more spies you send in one attack, the higher the chance that they will be successful - and that your identity will not be revealed. 
                <br /><br />
               
               
                 <% if(IsD2) { %>
                <img src= "https://static.realmofempires.com/images/icons/m_battleSim.png" align="left" style="padding-right: 5px" />
                 Go to the Battle Simulator. Enter 100 spies for the attacking side, and 50 spies for the defender. Simulate it! 
                <br /><br />                 
                You can access the Battle Simulator by clicking on the bullseye icon at the right side of the screen.
                 <br /><br />
                <%} else { %>
                 Go to the Battle Simulator. Enter 100 spies for the attacking side, and 50 spies for the defender. Simulate it! 
                <br /><br />
                (Remember, you can access the Battle Simulator from the Troops menu - middle, top of the screen.)
                <br /><br />
                <%}%>
                Enter the missing information below.
                <%} %><br /><br />  
                <table border=0 cellpadding=0 cellspacing=0 class="stripeTable TypicalTable" style="border:solid 1px black;width:99%;" >
                    <tr class="DataRowAlternate highlight">
                        <td >
                            Chance that spies will be<br /> successful</td>
                        <td>
                            <asp:Label ID="lbl_QBattleSimSpies_SpySucessChance" runat="server" Text="0"></asp:Label></td>
                    </tr>
                     <tr class="DataRowNormal highlight">
                        <td>
                            Chance the defender will <br />know the identity of spies</td>
                        <td>
                            <asp:TextBox ID="txt_QBattleSimSpies_SpyIdentityKnown" runat="server" CssClass="TextBox"
                                Width="60px" style="font-size:13pt;"></asp:TextBox>%
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="txt_QBattleSimSpies_SpyIdentityKnown"
                                CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number"  ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>                
                        </td>
                    </tr>
                     <tr class="DataRowAlternate highlight">
                        <td>
                            Chance the defender will<br /> see the spies coming</td>
                        <td>
                            <asp:Label ID="lbl_QBattleSimSpies_SpiesComming" runat="server" Text="0"></asp:Label></td>
                    </tr>
                    </table> 
            <%if (IsMobile) {%><br /><img src='https://static.realmofempires.com/images/icons/m_battleSim.png' style="float:left;padding-right:3px" /> *To enter the battle simulator, close this screen, then tap the right most icon on the bottom of the screen, then select the battle simulator<%} %>                              
            </asp:Panel>            
            <asp:Panel ID="panel_QBattleSimCM" runat="server" Visible="False">
                The Battle Simulator is truly an indispensible tool! Learn to use it well, and you will 
                be able to estimate the outcome of any battle.
                 <br /><br />
                 <% if(IsD2) { %>
                <img src= "https://static.realmofempires.com/images/icons/m_battleSim.png" align="left" style="padding-right: 5px" />
                <%} %>
                In the Battle Simulator, enter 100 <img src= "https://static.realmofempires.com/images/units/Militia.png" />
                Citizen Militia for the attacker. Also enter 100 <img src= "https://static.realmofempires.com/images/units/Militia.png" />
                Citizen Militia for the defender. Hit 'simulate' and see what happens!
                <br /><br />
                How many defending troops survive?<asp:TextBox ID="txt_QBattleSimCM" runat="server" CssClass="TextBox"
                    Width="60px" style="font-size:13pt;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="txt_QBattleSimCM"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" ControlToValidate="txt_QBattleSimCM"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>                                    
            </asp:Panel>            
            <asp:Panel ID="panel_QBattleSimLC" runat="server" Visible="False">
                The last battle didn't go very well for the attacker, did it? <img src= "https://static.realmofempires.com/images/units/Militia.png" />
                Citizen Militia are poor offensive troops.
                <br /><br />
                 Let's try <img src= "https://static.realmofempires.com/images/units/cavalry.png" /> Light Cavalry instead, which are much more effective! 
                <br /><br />
                 <% if(IsD2) { %>
                <img src= "https://static.realmofempires.com/images/icons/m_battleSim.png" align="left" style="padding-right: 5px" />
                <%} %>
                Go back to the Battle Simulator, and set 15 Light Cavalry to attack 100 Citizen Militia.
                <br /><br />
                How many of the attacking troops are left after the fight? <asp:TextBox ID="txt_QBattleSimLC" runat="server" CssClass="TextBox"
                    Width="60px" style="font-size:13pt;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="txt_QBattleSimLC"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server" ControlToValidate="txt_QBattleSimLC"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>                                    
            </asp:Panel>            
            <asp:Panel ID="panel_QBattleSimLCWithWall" runat="server" Visible="False">
                <img src= "https://static.realmofempires.com/images/misc/Tower_S3R.png" 
                    align="left" style="padding-right: 5px;height:100px;" />
                You've seen that as few as 15 Light Cavalry can destroy a garrison of 100 Citizen Militia. But what if their are walls and defensive towers in place? 
                <br /><br />
                 <% if(IsD2) { %>
                <br /><img src= "https://static.realmofempires.com/images/icons/m_battleSim.png" align="left" style="padding-right: 5px" />
                <%} %>
                Go to the Battle Simulator, and set 15 Light Cavalry to attack 100 Citizen Militia again. However, this time give the defender
                level 10 defensive walls and level 10 defensive towers. This fight might not end so well for the Light Cavalry!
                <br /><br />
                How many of defending troops are remaining? <asp:TextBox ID="txt__QBattleSimLCWithWall" runat="server" CssClass="TextBox"
                      Width="60px" style="font-size:13pt;"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="txt__QBattleSimLCWithWall"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator11" runat="server" ControlToValidate="txt__QBattleSimLCWithWall"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>                                    
            </asp:Panel>            
            <asp:Panel ID="panel_QBattleSimRamAndTreb" runat="server" Visible="False">            
                Walls and towers make battles much more difficult for the attacker. There is a way to deal with them, however - by sending <img src= "https://static.realmofempires.com/images/units/ram.png" style="padding-right: 5px" />Rams and <img src= "https://static.realmofempires.com/images/units/treb.png" style="padding-right: 5px" />Trebuchets to smash them apart!
                <br /><br />
                 <% if(IsD2) { %>
                <img src= "https://static.realmofempires.com/images/icons/m_battleSim.png" align="left" style="padding-right: 5px" />
                <% } %>                             
                In the Battle Simulator, send 15 Light Cavalry to attack 100 Citizen Militia, with level 10 defensive 
                walls and towers. This time, also include 40 Rams and 40 Trebuchets with the attack. 
                <BR><BR /><%if (IsMobile) {%>(For this Quest, set the defender's <i>research defense bonus</i> to 0%.)
                <br /><br /><%} else { %>Also, set the <I>Defender's Wall Research Defence Bonus</I> to 0% (under the Advanced Battle Parameters in battle simulator) - this is a typical value for a defender early on in the game 
                <br /><br />
                You will find that this time the attacker wins!
                <br /><br /><%} %>
                How many of attacking Light Cavalry are remaining? <asp:TextBox ID="txt_QBattleSimRamAndTreb" runat="server" CssClass="TextBox"
                      Width="60px" style="font-size:13pt;"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="txt_QBattleSimRamAndTreb"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator12" runat="server" ControlToValidate="txt_QBattleSimRamAndTreb"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Only numbers allowed" ValidationExpression="\d*" ValidationGroup="oldquests">Only numbers allowed</asp:RegularExpressionValidator>                                    
            </asp:Panel>      
            
            
            
            
             <asp:Panel ID="panel_QMap1" runat="server" Visible="False">
                  Your village has a physical location in the realm. Sending troops (to attack or to support) takes more time as the distance to target increases.
                 <BR /><BR />
                 <% if(IsD2) { %>
                    You can find your village and your neighbours by browsing the map. Your villages will have the white shield icon on them.
                    <BR /><BR />
                    On the Village Overview panel, at the bottom, you can click on the village centering icon. <img src="https://static.realmofempires.com/images/icons/centerVillage.png" align="right" style="padding-right: 5px;" />
                 This will center the map on the village you are currently looking at in the Village Overview.<br /><br />
                 Go to the Village Overview panel and try clicking the village centering icon now.


                 <% } else { %>
                    You can find your village and your neighbours by browsing the map. The map has some functions you should be aware of. 

                    <BR /><BR />
                
                    <img src="https://static.realmofempires.com/images/map/legend2.png" align="left" style="padding-right: 5px;" /> The 
                        KEY icon displays the legend. 

                    <BR /><BR />
                    <img src="https://static.realmofempires.com/images/map/globe2.png" align="left" style="padding-right: 5px;"/> The 
                        GLOBE icon displays a small overview map.</LI>

                    <BR /><BR />You will find both on the map's border. Go to the map and try those functions now. 
                 <% } %>
            </asp:Panel>
            <asp:Panel ID="panel_QMap2" runat="server" Visible="False">
                The default map is a 7x7 square display. Larger map sizes available if you want to see more of the realm at the same time.:<br />
                <ul>
                    <li><b>LARGE MAP</b> - map sizes up to 19x19</li>
                    <li><b>GIANT MAP</b> - map sizes 25x25 and 45x45</li>
                </ul>
                 <%if (!_player.Realm.IsVPrealm){ %>Due to higher server load with the larger maps, those are available as <a href="pfbenefits.aspx">
                    premium features</a> but we also offer free trials.<%} %>
                <br />
                <br />
                For this quest, please switch your map size to 19x19 and claim your reward. <%if (!_player.Realm.IsVPrealm){ %>Unlock
                the free trial if needed.<%} %>
            </asp:Panel>
            <asp:Panel ID="panel_QSupport" runat="server" Visible="False">
            <%if (IsMobile || IsD2) {%>  
                There are many strategic options in Realm of Empires. One of the most important is 
                the ability to send <img src= "https://static.realmofempires.com/images/support.png" /><b>support</b>
                to other villages.
                <br /><br />
                Troops sent as support will help defend the village they are sent to. They are still supplied with
                food by their home village, and must return home before being sent anywhere else. Also, the supporting 
                units cannot be sent to attack anyone else. With support you
                can amass vast armies to defend yourself or others against enemy attacks. 
                <br /><br />
                Your quest is to contact someone and convince them to send you support
                - even a single unit will suffice. I suggest starting with your clanmates. Good luck!
            <%} else { %>              
                One of the most important strategic options in Realm of Empires is the ability to send
                <img src= "https://static.realmofempires.com/images/support.png" /> support. 
                When you send troops to another village as <B>
<I>support</I></B>, you are sending them to help 
                defend that village. The owner of this village cannot send them to attack anyone else - they 
                will only defend the village when it is attacked. 
                <br /><br />
                Supporting troops do not consume the food at the village they are stationed at, but 
                are supplied from their home village. You can amass large armies to defend your villages by using support, and by asking friends and clanmates to send support when you are under attack. 
                <br /><br />
                Your quest is to contact someone and convince them to send you support - 1 unit of any kind will do. 
                You can also <a href="invite.aspx">invite your friends</a> to join you at Realm of Empires and ask them to help you complete this quest. 
                <br /><br />
                Once this support arrives, you may claim your reward. 
            <%} %>

            </asp:Panel>
            <asp:Panel ID="panel_QTOU" runat="server" Visible="False">
                <%if (IsMobile) {%>            
                My Liege, there is a power in the realm, a great eye always watching - the administrators of the realm! 
                <br /><br /><%} else { %>My Liege, there is a power in the realm, a great eye...always watching - the administrators of the realm! 
                <br /><br /> 
                Wielding absolute power they roam the realm like wraiths, day and night, looking for cheaters! 
                When found, they are banished from all the realms and custody of their villages is given to a 
                souless lord called 'Banned'. This power has been exercised on many occasions and we all have 
                heard their victims' screams and grinding of teeth. 
                <br /><br /><%} %>
                Though their wrath is terrible, they are also just, and in their 
                benevolence have <a href="tou.aspx" target=_blank>published the Terms of Use</a>,<%if (IsMobile) {%> rules designed to foster fair play.<%} else { %>
                rules forged in the depth of windowless offices, before the beginning of time, rules designed to foster fair play.<%} %>
                <br /><br />
                <a href="tou.aspx" target= _blank>Read the Terms of Use now</a> 
                and select all the examples that are considered breaking the rules. <%if (IsD2) {%>The Terms of Use can be found under Settings (<img src="https://static.realmofempires.com/images/icons/M_MoreSettings.png" style="vertical-align:middle" width="20" height="20" />) in the main HUD.<%} %>
                <br /><br />

                <asp:CheckBox ID="cb_QTOU1" runat="server" Text="Playing with multiple accounts on one realm." /><br />
                <asp:CheckBox ID="cb_QTOU2" runat="server" Text="Sending silver or support to another clan."/><br />
                <asp:CheckBox ID="cb_QTOU3" runat="server" Text="Creating an account for the sole purpose of aiding another account." /><br />
                <asp:CheckBox ID="cb_QTOU4" runat="server" Text="Threatening destruction to someone's village or troops."/><br />
                <asp:CheckBox ID="cb_QTOU5" runat="server" Text="Calling another player an idiot."/><br />


            </asp:Panel>                 
            
            <asp:Panel ID="panel_QRealms" runat="server" Visible="False">                
                Did you know that Realm of Empires offers you different realms, each one a self-contained world for you to play in?

                <BR /><BR />You may play in as many realms at once as you like and have different identities (nickname) in each realm. Each realm is a fresh start - new people, new clans, new neighbours, new opportunities to conquer!

                <BR /><BR />Some realms are currently opened to players, others are closed to new entrants. Different realms may have alternate features such as Research and Spells - try them all to see what you like best!

                <BR /><BR />Realms can have different speeds. Slower paced realms are easier to play if you have less time to devote to the game. Faster paced realms have more action and more combat!

                <BR /><BR />Look at the summary of <b>Differences between Realms</b> on our <a href="http://roe.wikia.com/wiki/Differences_Between_Realms" target="_blank">WIKI</a>. 

                <BR /><BR />What is the speed factor of silver production on Realm 4?:<asp:TextBox ID="txt_panel_QRealms" MaxLength="4"  runat="server" CssClass="TextBox"
                      Width="30px" style="font-size:13pt;"></asp:TextBox>X
                <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" ControlToValidate="txt_panel_QRealms"
                    CssClass="Error" Display="Dynamic" ErrorMessage="Enter a number" ValidationGroup="oldquests">Enter a number</asp:RequiredFieldValidator>               
            </asp:Panel>         
            
            
                    
            <asp:Panel ID="panel_QTavern" runat="server" Visible="False">                
                <img src="https://static.realmofempires.com/images/misc/tavern.png" align="left" style="padding-right: 5px;" /> 
                Now it's time to build a <b>Tavern</b>! This is where you recruit spies, shifty fellows 
                who know how to sneak into enemy towns and bring back key intelligence.
    

                <BR /><BR />Everyone needs spies, so build your Tavern now.   
                             <BR /><BR />
                <%if (IsD2 || IsMobile) {%>Important: To be able to build a Tavern, your Barracks must be at least level 5. If you do not have the Barracks building, you'll first need to get your Silver Mine to level 5.<br />
                <%if (IsD2){%>
                    <div class="BtnBSm2 fontButton1L" onclick="window.top.ROE.Building2.showBuildingPagePopup('12');">Build</div> <%} 
                else if (IsMobile){%>
                    <div class="BtnBSm2 fontButton1L" onclick="<% if (IsiFramePopupsBrowser) { %>
                        window.parent.ROE.UI.Sounds.click(); 
                        window.parent.ROE.Building2.showBuildingPagePopup('12');
                        <%} else {%>
                        window.opener.ROE.UI.Sounds.click();                         
                        window.opener.ROE.Building2.showBuildingPagePopup('12');  
                        window.close();                       
                        <%}  %>">Build</div> <%} %>
                <%} %>
            </asp:Panel>        

            <asp:Panel ID="panel_QStables" runat="server" Visible="False">                
                <img src="https://static.realmofempires.com/images/misc/stable.png" align="left" style="padding-right: 5px;" /> 
                The Stables is where you recruit your best offensive units, such as the speedy Light Cavalry.

                <BR /><BR />Build your Stables now so you can recruit Light Cavalry, an excellent unit for raiding nearby Rebel villages.                                            
            </asp:Panel>        
 
            <asp:Panel ID="panel_QFindInactives" runat="server" Visible="False">
            <%if (IsMobile) {%> 
            Sometimes one of your neighbors will stop playing. If they fail to login for 14 days they will go <b>abandoned</b> and turn grey on the map.<br /><br />
            It's a good idea to look for players who are rarely logging in. You can take their silver, or even their village!<br /><br />
            <%} else { %>               
                Sometimes your neighbors stop playing. If a player does not login for 14 days his villages become 'abandoned' and turn grey on the map. 
                The village still functions - the troops stay and protect the village, the silver mine keeps producing silver etc... silver you can steal by attacking!
                
                <BR /><BR />While it takes 14 days of inactivity before the village becomes officially abandoned, you can often identify inactive players well before then. <%} %>Here are some tips to finding easy targets:
                <BR /><BR />
                <ol>
                    <li>The player is not part of any clan.</li>
                    <li>The player's points have not increased for several days.</li>
                    <li>The player has no attack points.</li>
                    <li>The player does not respond to messages (for at least a day).</li>
                </ol>
                <BR />Find at least one suspected village nearby and send the owner a message. If they don't respond or grow in points, it may be time to attack! 
            </asp:Panel>        

            <asp:Panel ID="panel_QRecruitSpies" runat="server" Visible="False">                
               <%if (IsD2 || IsMobile) {%><img src="https://static.realmofempires.com/images/units/Spy_M.png" align="left" /><%} %>
                    Recruit at least one Spy. Spies will let you find out if there are any defenders in Rebel villages, find "bonus" villages, and scout out other players.
            </asp:Panel>        
            
            


            <asp:Panel ID="panel_QLootInactives" runat="server" Visible="False">                
                Your neighbors' mines are producing silver... silver that should belong to <b>you</b>! Send spies to find out what troops are there. If the village is weak or empty, attack it and watch your treasury fill up!!

                <BR /><BR />As a review, select all items that indicate when a player may be inactive:

                <br /><br /><asp:CheckBox runat="server" Text="Player does not join your clan when you invite them." ID="cb_QLootInactives_ClanInvite" />
                <br /><asp:CheckBox runat="server" Text="You don't get responses to your messages." ID="cb_QLootInactives_Messages" />
                <br /><asp:CheckBox runat="server" Text="Village has fewer points than you." ID="cb_QLootInactives_LessPoints" />
                <br /><asp:CheckBox runat="server" Text="Village's points are not increasing over many days." ID="cb_QLootInactives_PointsNoUp" />
                <br /><asp:CheckBox runat="server" Text="Player is not part of a clan." ID="cb_QLootInactives_NoClan" />

            </asp:Panel>                    


            <asp:Panel ID="panel_QMail" runat="server" Visible="False">                
                You are already familiar with the internal mail system. You can communicate with other players without disclosing any personal & real-life information.

                <BR /><BR />Please note that due to space considerations, messages older than 14 days are archived and messages older than 30 days are permanently deleted. 

                <BR /><BR />You can keep important messages indefinitely by <%if (IsD2 || IsMobile) {%><img src="https://static.realmofempires.com/images/icons/starred.png" style="vertical-align:middle" width="20" height="20" /> Starring them.<% } else { %> moving them into custom mail folder that you create. This is a premium feature that also allows you to view archived messages.<%} %>

                <BR /><BR />For this quest, please <%if (IsD2 || IsMobile) {%>Star at least one message.<% } else { %>create (at least) one custom mail folder. 
                Perhaps call it 'Important' or 'To Keep'. There is a free trial for this feature. <%} %>
            </asp:Panel>                    


            <asp:Panel ID="panel_QReports" runat="server" Visible="False">   
                <%if (IsD2 || IsMobile) {%> As with mail, reports older than 14 days are archived and those older than 30 days are deleted. To keep important reports indefinitely, like mail, you can <img src="https://static.realmofempires.com/images/icons/starred.png" style="vertical-align:middle" width="20" height="20" /> Star them.
                <BR /><BR />For this quest, please Star a report.
                <% } else { %>             
                As with mail, reports older than 14 days are archived and those older than 30 days are deleted.

                <BR /><BR />Storing reports in custom folders prevents their deletion.

                <BR /><BR />For this quest, please create (at least) one custom reports folder, perhaps call it 'Important' or 'Villages To Loot'. There is a free trial for this feature.
                <%} %> 
            </asp:Panel>                    


            <asp:Panel ID="panel_QAdvisor2" runat="server" Visible="False">
                My Liege, if you are ever stuck, the advisor is there to guide you and offer you a hint about what to do next. 

                <BR /><BR />Access the advisor again now and see what it has to say. 

                <img src= "https://static.realmofempires.com/images/navIcons/Help1.gif" align="left" style="padding-right: 5px" />It is accessible from the Help menu icon, under the "Help! What do I do now? Please advise." menu item.                 
            </asp:Panel>

                
            <asp:Panel ID="panelClaimReward" runat="server" >
                
                <asp:Label ID="lblClainError" runat="server" CssClass="Error" Visible="False"></asp:Label>
                <center>
                    
                    <asp:LinkButton ID="btnClaimYourReward" runat="server" OnClick="lbClaimYourReward_Click"
                        CssClass="getreward" ValidationGroup="oldquests" ></asp:LinkButton>
                    <asp:HyperLink ID="linkCheat" runat="server" class="LockedFeatureLink" onclick ="return popupUnlock(this);" href="PFOff.aspx?pfid=22">rewardNow&trade;</asp:Hyperlink>
                    <asp:LinkButton CausesValidation="false" ID="btnCheat" runat="server" 
                        onclick="btnCheat_Click" style="font-size: 10px">rewardNow&trade;</asp:LinkButton>
                    &nbsp;&nbsp;<asp:LinkButton style="float:right;font-size: 10px" CausesValidation="false" ID="btnSkipQuest" runat="server" onclick="btnSkipQuest_Click" 
                        OnClientClick="return confirm('Are you sure? If you skip a quest, you will never be able to get the reward for it.')">skip</asp:LinkButton>
                    &nbsp;</center>
                <div style="clear: both;">
                    <center>
                        <% if(IsD2 || IsMobile) { %>
                        
                        <img src="https://static.realmofempires.com/images/d2_Reward_.png" /><br />
                         <% } else { %>
                        <img src="https://static.realmofempires.com/images/Reward_.png" /><br />
                        <% } %>
                        <table cellpadding="2" cellspacing="2">
                            <tr>
                                <td id="Td1" runat="server">
                                    <% if(IsD2 || IsMobile) { %>
                                    <asp:Image ID="Image4" Style="vertical-align: middle; height: 50px; float: left;" runat="server" src="https://static.realmofempires.com/images/gifts/d2_Gift_sack_of_silver.png" />
                                    <% } else { %>
                                    <asp:Image ID="Image3" Style="vertical-align: middle; height: 50px; float: left;" runat="server" src="https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png" />
                                    <% } %>
                                    
                                    <asp:Label ID="lblRewardAmount" runat="server"></asp:Label>
                                    Silver
                                </td>                            
                            </tr>
                        </table>
                    </center>
                </div>
            </asp:Panel>
            <asp:Panel ID="panelCompletedQuests" runat="server">
                <br />
                <asp:Label ID="lblQuestCompletedMsg" runat="server"></asp:Label><br />
                <br />
                <center>
                <asp:LinkButton ID="btnNextQuest" runat="server" OnClick="btnNextQuest_Click" CssClass="StandoutLink" style="font-size:14px;">OK, next quest please!</asp:LinkButton></center>
            </asp:Panel>
            <asp:Panel ID="panelAllQuestsCompleted" runat="server">
                <br />
                <center>
                CONGRATULATIONS!!!
                <br />
                You have completed all Mastery quests! 
                <br /><br />
                We hope that you will find Realm of Empires to be an exciting and unique gaming experience. With time you will build your own mighty empire, discover many exciting aspects of gameplay, and hopefully make some new friends along the way.
                <br /><br />There will be more quests as you grow from one village to many, and are ready to explore more advanced parts of Realm Of Empires!
                </center>
                <center>
                    <br />
                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="StandoutLink" OnClick="LinkButton1_Click">OK</asp:LinkButton></center>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td class="" align="right" style="" >
            <asp:Label ID="lblQuestsAvailUntil" CssClass="TypicalText"  Style="font-size: 10px" runat="server"></asp:Label>
        </td>
    </tr>    
</table>
