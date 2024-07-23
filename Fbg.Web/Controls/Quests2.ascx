<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Quests2.ascx.cs" Inherits="Controls_Quests2" %>
<table border="0" cellpadding="0" cellspacing="0" class="Box" width="100%">
    <tr>
        <td class="" colspan="2">
<asp:Panel ID="panelReward" runat="server" Visible="False">                
                <asp:Label ID="lblTitle" runat="server"
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
        <td class=" Padding" colspan="2">
            <asp:Panel ID="panel_QCreateTags" runat="server" Visible="False">
                My Liege, Congratulations on your progress! Now that you lead a mighty empire, let me show you how you can group villages to enhance your experience. Tags & Filters is an extremely powerful tool for this!
                <br />
                <br />
                For this quest, start by creating these tags:
                <ul>
                    <li>Attack</li>
                    <li>Defend</li>
                    <li>Spy</li>
                    <li>SectorA</li>
                    <li>SectorB</li>
                    <li>UnderDev</li>
                </ul>
                You can create tags by moving your mouse over the tiny funnel/filter icon
                <img src='https://static.realmofempires.com/images/funnel1.png' />
                on the very top left of the screen, to the left of your village name and selecting "<a class="confignohrSml">Edit Tags</a>" from the popup.
                <br />
                <br />
                Create the above tags now and claim your reward!
            </asp:Panel>
            <asp:Panel ID="panel_QApplyTags" runat="server" Visible="False">
                <img src='https://static.realmofempires.com/images/misc/quests_tagspopup.jpg' style="float: right; padding-left: 2px;" />
                OK, so now, the filter
                <img src='https://static.realmofempires.com/images/funnel1.png' />
                icon popup should look like this ->
                <br />
                <br />
                The TAGS panel shows you all the tags you have available. You can apply a tag to your current village, simply by clicking the tag.
                <br />
                <br />
                <img src='https://static.realmofempires.com/images/misc/quests_clicktag.gif' style="float: left; padding-right: 3px;" />
                For this quest, lets <i>apply</i> both the <b>SectorB</b> and <b>Defend</b> tags to your current village.
                <br />
                <br />
                Also, switch to another village, and give it tag <b>Defend</b> only. Switch to yet another village and give it the <b>SectorB</b> tag.
                <br />
                <br />
                At the end you should have:
                <ul>
                    <li>One village with <b>SectorB</b> and <b>Defend</b> tags</li>
                    <li>One village with <b>SectorB</b> tag only</li>
                    <li>One village with <b>Defend</b> tag only</li>
                </ul>
            </asp:Panel>
            <asp:Panel ID="panel_QFilters" runat="server" Visible="False">
                <img src='https://static.realmofempires.com/images/misc/quests_tagspopup.jpg' style="float: right; padding-left: 2px;" />
                OK, since now you know how to create tags, how to tag and untag villages, now lets look at FILTERS.
                <br />
                <br />
                You should have these filters ->
                <br />
                <br />
                A filter is a way of selecting a group of villages that have certain tags.
                <br />
                <br />
                <img src='https://static.realmofempires.com/images/misc/quests_clickfilter.gif' style="float: left; padding-right: 3px;" />
                For example, in the last quest, you tagged 2 villages with DEFEND tag. To now look at only those villages, apply the DEFEND filter by clicking on it.
                <br />
                <br />
                Do so now. Once you apply a filter, you will notice your filter icon will change from
                <img src='https://static.realmofempires.com/images/funnel1.png' />
                to
                <img src='https://static.realmofempires.com/images/funnel2.png' />
                to signal a filter is applied.
                <br />
                <br />
                Apply a filter now and click on claim your reward.
            </asp:Panel>
            <asp:Panel ID="panel_QFiltersSummary" runat="server" Visible="False">
                Now that you have a filter applied, you can step through just the villages that fall under this filter (ie, that have the correct tag applied to them) using the village navigation arrows
                <img src="https://static.realmofempires.com/images/leftarrow.png" /><img src="https://static.realmofempires.com/images/rightarrow.png" />. Try that now.
                <br />
                <br />
                Also, summary pages will now list only the villages that fall under the filter. Remember about this as this will be a powerful tool as you expand your empire. Go to Buildings Summary page and Units Summary page now.
                <br />
                <br />
                Once you have tried both, claim your reward
            </asp:Panel>
            <asp:Panel ID="panel_QMultiFilter" runat="server" Visible="False">
                OK, so now you have seen how filters work but the best is yet to come!
                <br />
                <br />
                What if you want to see DEFENSE villages in SECTORB? or DEFENSE villages in SECTORB that are UNDERDEVelopment? You can create filter just for that!
                <br />
                <br />
                A filter simply selects villages which have one (or more) tags applied so you can define a filter that show you villages that have a DEFENSE tag and a SECTORB tag.
                <br />
                <br />
                Why browse through all your villages (especially when you have many!) to see what buildings need upgrading, when most of the villages are fully built up? Instead, just create an 'Under Development' tag and tag those new villages with it. Then just look at villages with this tag using a filter.
                <br />
                <br />
                For this quest, create a custom filter. From the filter
                <img src='https://static.realmofempires.com/images/funnel1.png' />
                icon popup, click <a class="confignohrSml">Edit Filters</a>
                <br />
                <br />
                Create a new filter, call it 'SectorB.Defend'. That will create an empty filter. Then select it (click the 'select' link next to it on that same page) and under Filter's Tags, check mark 'SectorB' tag, and 'Defend' tag, click Update.
            </asp:Panel>
            <asp:Panel ID="panelClaimReward" runat="server">
                <div style="clear: both;">
                    <center>
                        <img src="https://static.realmofempires.com/images/Reward_.png" /><br />
                        <table cellpadding="2" cellspacing="2">
                            <tr>
                                <td id="Td1" runat="server">
                                    <asp:Image ID="Image3" Style="vertical-align: middle; height: 50px; float: left;" runat="server" src="https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png" />
                                    <asp:Label ID="lblRewardAmount" runat="server"></asp:Label>
                                    <br />
                                    Silver
                                </td>                            
                            </tr>
                        </table>
                    </center>
                </div>
                <asp:Label ID="lblClainError" runat="server" CssClass="Error" Visible="False"></asp:Label>
                <center>
                    <asp:LinkButton ID="btnClaimYourReward" runat="server" OnClick="lbClaimYourReward_Click"
                        CssClass="getreward" ValidationGroup="oldquests"></asp:LinkButton>
</center>
                
            </asp:Panel>
            <asp:Panel ID="panelCompletedQuests" runat="server">
                <br />
                <asp:Label ID="lblQuestCompletedMsg" runat="server"></asp:Label><br />
                <br />
                <center>
                    <asp:LinkButton ID="btnNextQuest" runat="server" OnClick="btnNextQuest_Click" CssClass="StandoutLink">OK, next quest please!</asp:LinkButton></center>
            </asp:Panel>
            <asp:Panel ID="panelAllTagQuestsCompleted" runat="server">
                <br />
                <center>
                    CONGRATS!!
                    <br /><br />You have completed all Tag & Filter related quests! They will greatly simplify your game play, especially
                    as you conquer more and more villages.               
                    <br /><br />We hope you will consider supporting Realm of Empires by activating this premium feature!
                    <br />
                    <br />
                    This feature can be activated on its own (under the name of &nbsp;'<em>Grouping Villages</em>') or you can get it as part of our best value <em>Nobility Package</em>.</center>
                <center>
                    <br />
                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="StandoutLink" OnClick="LinkButton1_Click">OK</asp:LinkButton></center>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td class="" align="left" style="height: 19px">
        </td>
        <td class="" align="right" style="height: 19px">
            <asp:Label ID="lblQuestsAvailUntil"  Style="font-size: 10px" runat="server">Mastery Quests expire.</asp:Label>
        </td>
    </tr>
</table>
