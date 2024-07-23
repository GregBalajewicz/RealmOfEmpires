<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="QuickCommandPopup_d2.aspx.cs" Inherits="templates_QuickCommandPopup_d2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">



    <div class="targetVillage">
        <div class='sectionBar'></div>
        <div class="targetImg"></div>
        <div class="targetName"><span>
            <img src="https://static.realmofempires.com/images/misc/busy_tinyRed.gif"></span></div>
        <div class='sectionBar'></div>
        <div class="refreshBtn ButtonTouch"></div>
        <!--<div class="moraleBtn smallRoundButtonDark sfx2"><span class="icon"></span></div>-->
        <div class="filtersCmdBtn smallRoundButtonDark sfx2"><span class="filterIcon"></span></div>
       
    </div>
    <div class="filterandsettings" style="display: none">
        <div class="showLandTimeSelection">
            show land time by
            <asp:Repeater EnableViewState="false" ID="Repeater2" runat="server">
                <ItemTemplate>
                    <img id="Img2" class="landTimeByTroopSelector" data-uid='<%#Eval("ut.ID") %>' border="0" runat="server" src='<%#Eval("ut.IconUrl") %>' />
                </ItemTemplate>
            </asp:Repeater>
            <div style="margin-bottom: 10px;">
                <span class="showOnlyVillagesWithLandByTroops" data-name="Show only villages containing troops above"></span>
                <div class="showMinWrapper">Have at least <input class="showOnlyHavingMinimum" type="number" value="0" /> of selected troops.</div>
            </div>
            <div>
                <span class="askAreYouSureOnCommand" data-name="Ask 'Are you sure?' before launching %cmdName%"></span>
            </div>
             <div>
                <span class="oneClickCommand" data-name="Enable one-click %cmdName%"></span>
            </div>
           
        </div>
    </div>

    <div class="QCMoraleContainer"></div>

        <table class="villageListMainTable " width="100%">
            <thead>
                <tr>
                    <th></th>
                    <th class="DistColumn">Dist.</th>
                    <th class="TravelTimeColumn">
                        <div class="landTimeByTroopIcons">
                            <asp:Repeater EnableViewState="false" ID="Repeater3" runat="server">
                                <ItemTemplate>
                                    <img data-uid='<%#Eval("ut.ID") %>' border="0" src='<%#Eval("ut.IconUrl") %>' class="landTimeByTroopIcon" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </div><div class="landTimeText">Time</div></th>
                    <th class="LandingTimeColumn">Landing Time</th>
                    <asp:Repeater EnableViewState="false" ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <th class="troopPickHeader <%#Eval("widthclass") %>">
                                <img class="vilres_unitimg" id="Img1" border="0" runat="server" src='<%#Eval("ut.IconUrl") %>' />
                            </th>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </thead>
            <tbody class="villageList">
                <tr class="oneVillage attackIconSheet panel cover %attack.villageHasNoLandByTroopsClass%" data-villid="%attack.villID%" data-idx="%attack.idx%" data-VX="%attack.villX%" data-VY="%attack.villY%">
                    <td nowrap>
                        <div class="villNameContainer">
                            <a class="villName villFrom jsV" href="#" x="%attack.villX%" y="%attack.villY%" vid="%attack.villID%" opid="%attack.pid%" data-addCustomDefaultActions="ROE.QuickCommand.buildObjectClickCustomIcon(%attack.villID%);">
                                %attack.villName% (%attack.villX%,%attack.villY%)
                            </a>
                            <span class="incoming incomingMiniIndicators helpTooltip" data-toolTipID="incomingMiniIndicators">%incomingImages%</span>
                        </div>
                    </td>
                    <td>
                        <div class="attackVilPanel " data-villid="%attack.villID%">
                            <div><span class="dist" data-distExact="%attack.distExact%">%attack.dist%</span></div>
                        </div>
                    </td>
                    <td>
                        <div class="attackVilPanel " data-villid="%attack.villID%">
                            <div class="travelTime"><span class="travelTimeNum">%attack.travelTime%</span></div>
                        </div>

                    </td>
                    <td class="landtime">
                        <span class="Time landtimeNum" data-offset="%attack.landingOn_offset%"></span>
                    </td>
                    <asp:Repeater EnableViewState="false" ID="dlTroops" runat="server">
                        <ItemTemplate>
                            <td class="troopPick  %v.troops.id<%#Eval("ut.ID")%>.zeroClass% <%#Eval("widthclass") %> <%#Eval("not1ClickSendable")%>" data-troopid="<%#Eval("ut.ID")%>" data-originaltroopcount="%v.troops.id<%#Eval("ut.ID")%>.count%">
                                <span class="troopCount">%v.troops.id<%#Eval("ut.ID")%>.count%</span><input style="display: none;" class='troopCountEntry' type='number' min="0" />
                            </td>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </tbody>
        </table>

        <div class="sendPanel">
            <div class="set">
                <div class="warns"></div>
                <div class="desertion"></div>
            </div>
            <div class="savePreset BtnBSm2 fontSilverFrSClrg helpTooltip" data-toolTipID="saveAsPreset">Save</div>
            <div class="closeSendPanel"></div>
            <div class="attackBtn BtnBSm2 fontSilverFrSClrg"></div>
            <div class="premiumPanel pflust"></div>
            <div class="premiumPanel pfrr"></div>
        </div>

    </div>
    <div class="getMoreVill">load more ...</div>

    <div class="phrases">
        <div ph="1"><%=RS("Error1") %></div>
        <div ph="1a"><%=RS("Error1a") %></div>
        <div ph="2"><%=RS("Error2") %></div>
        <div ph="3"><%=RS("Error3") %></div>
        <div ph="4"><%=RS("Error4") %></div>
        <div ph="5"><%=RS("Error5") %></div>
        <div ph="6"><%=RS("Error6") %></div>
        <div ph="7"><%=RS("Error7") %></div>
        <div ph="8"><%=RS("Error8") %></div>
        <div ph="9"><%=RS("Error9") %></div>
        <div ph="13"><%=RS("Error13") %></div>
        <div ph="troopsSent0"><%=RS("troopsSent0") %></div>
        <div ph="troopsSent1"><%=RS("troopsSent1") %></div>
        <div ph="noTroops"><%=RS("noTroops") %></div>
        <div ph="Button1"><%=RS("Button1") %></div>
        <div ph="Button0"><%=RS("Button0") %></div>
        <div ph="wrongTroopCounts"><%=RS("wrongTroopCounts") %></div>
        <div ph="attackThisVillage"><%=RS("attackThisVillage") %></div>
        <div ph="supportThisVillage"><%=RS("supportThisVillage") %></div>
    </div>

</asp:Content>
