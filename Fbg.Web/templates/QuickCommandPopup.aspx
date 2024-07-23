<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="QuickCommandPopup.aspx.cs" Inherits="templates_QuickCommandPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">


    <div id="background">
        <img src="https://static.realmofempires.com/images/backgrounds/M_BG_Battle1.jpg" class="stretch" alt="" />
    </div>


    <div class="targetVillage">
        <div class='sectionBar'></div>
        <div class="targetImg"></div>
        <div class="targetName">
            <span>
                <img src="https://static.realmofempires.com/images/misc/busy_tinyRed.gif"></span>
        </div>
        <div class='sectionBar'></div>
        <div class="refreshBtn ButtonTouch sfx2"></div>
        <div class="filtersCmdBtn smallRoundButtonDark sfx2"><span class="filterIcon"></span></div>
    </div>

    <div class="QCMoraleContainer"></div>

    <div class="filterandsettings" style="display: none">
        <div class="showLandTimeSelection">
            show land time by
            <asp:Repeater EnableViewState="false" ID="Repeater2" runat="server">
                <ItemTemplate>
                    <img id="Img2" class="landTimeByTroopSelector" data-uid='<%#Eval("ut.ID") %>' border="0" runat="server" src='<%#Eval("ut.IconUrl") %>' />
                </ItemTemplate>
            </asp:Repeater>
            <div>
                <span class="showOnlyVillagesWithLandByTroops" data-name="Show only villages with those troops"></span>
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

    <table class="villageListMainTable" width="100%" cellspacing="0" cellpadding="0">

        <tbody class="villageList">
            <tr class="oneVillage attackIconSheet %attack.villageHasNoLandByTroopsClass%" data-villid="%attack.villID%" data-idx="%attack.idx%" data-idx="%attack.idx%" data-vx="%attack.villX%" data-vy="%attack.villY%">
                <td>
                    <div class="panel">
                        <div class="attackVilPanel" data-villid="%attack.villID%">
                            <div class="villName">%attack.villName%</div>
                            <div class="incoming incomingMiniIndicators">%incomingImages%</div>
                            <div>(<span class="coord">%attack.villX%,%attack.villY%</span>) <span class="dist" data-distExact="%attack.distExact%">%attack.dist% <%=RS("square") %></span></div>
                            <div class="landtime">
                                <div class="landTimeByTroopIcons">
                                    <asp:Repeater EnableViewState="false" ID="Repeater3" runat="server">
                                        <ItemTemplate>
                                            <img data-uid='<%#Eval("ut.ID") %>' border="0" src='<%#Eval("ut.IconUrl") %>' class="landTimeByTroopIcon" />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <span class="Time landtimeNum" data-offset="%attack.landingOn_offset%">00:00:00</span>
                            </div>
                            
                        </div>
                        <div class="attackTroopsPanel">
                            %attack.troops%
                        </div>
                    </div>
                    <div class="cover"></div>
                </td>
            </tr>
        </tbody>

        <div class="sendPanel">
            <div class="dividerTop"></div>
            <div class="dividerBottom"></div>
            <div class="villFrom sfx2 smallRoundButtonDark fontSilverNumbersLrg">#</div>
            <div class="set mobile">
                <div class="travelTime"><%=RS("Travel") %>: <span class="travelTimeNum"></span></div>
                <div class="warns"></div>
                <div class="desertion"></div>
            </div>
            <div class="savePreset smallRoundButtonDark fontSilverFrSClrg helpTooltip" >Save</div>
            <div class="attackBtn BtnBSm2 fontSilverFrSClrg sfx2"></div>
            <div class="premiumPanel pflust"></div>
            <div class="premiumPanel pfrr"></div>
            <!--<div class="premiumPanel pftest"></div>-->
        </div>
        <div class="getMoreVill">
            <img src="https://static.realmofempires.com/images/icons/M_MoreList.png"></div>




        <div class="phrases">
            <div ph="1"><%=RS("Error1") %></div>
            <div ph="2"><%=RS("Error2") %></div>
            <div ph="3"><%=RS("Error3") %></div>
            <div ph="4"><%=RS("Error4") %></div>
            <div ph="5"><%=RS("Error5") %></div>
            <div ph="6"><%=RS("Error6") %></div>
            <div ph="7"><%=RS("Error7") %></div>
            <div ph="8"><%=RS("Error8") %></div>
            <div ph="9"><%=RS("Error9") %></div>
            <div ph="13"><%=RS("Error13") %></div>
            <div ph="14"><%=RS("Error14") %></div>
            <div ph="troopsSent0"><%=RS("troopsSent0") %></div>
            <div ph="troopsSent1"><%=RS("troopsSent1") %></div>
            <div ph="noTroops"><%=RS("noTroops") %></div>
            <div ph="Button1"><%=RS("Button1") %></div>
            <div ph="Button0"><%=RS("Button0") %></div>
        </div>
</asp:Content>
