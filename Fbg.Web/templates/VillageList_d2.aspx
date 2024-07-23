<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="VillageList_d2.aspx.cs" Inherits="templates_VillageList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <div id="villageListPopup">
        <div class="filterArea">
            <div class="villlist_refresh ButtonTouch sfx2">
                <img src="https://static.realmofempires.com/images/icons/M_Refresh.png" />
            </div>

            <%if (FbgPlayer.NumberOfVillages > 2)
              {  %>
            <div class="massfeatures">
                <div class="massfeature massrecruit" data-title="[Beta] Mass Recruit" data-href="villagemassrecruit.aspx"></div>
                <div class="massfeature massupgrade" data-title="[Beta] Mass Upgrade" data-href="villagemassupgradeb.aspx"></div>
                <div class="massfeature massdowngrade" data-title="[Beta] Mass Downgrade" data-href="VillageMassDowngrade.aspx"></div>
                <div class="massfeature massdisband" data-title="[Beta] Mass Disband" data-href="VillageMassDisband.aspx"></div>
            </div>
            <%} %>
            <div class="hiddenDuringFilter">
                <input type="text" class="filterSearchString" />
                <div class="filterListBtn listToolBtn sfx2 smallRoundButtonDark">
                    <div class="listToolIcon" style="background-image: url('https://static.realmofempires.com/images/icons/magnifyingGlass.png');"></div>
                </div>
            </div>
            <div class="filtersCmdBtn smallRoundButtonDark sfx2"><span class="filterIcon"></span></div>
        </div>

        <div class="villageListsWrapper">

            <div class="villageListHeader"></div>

            <div class="villageList">
                <table cellpadding="1" cellspacing="1">
                    <thead>
                        <tr>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                                         
                            <th class="troopsA headerTroopType" colspan="<%=FbgPlayer.Realm.GetUnitTypes().Count %>">A: All troops in this village now</th>
                            <th class="troopsY headerTroopType" colspan="<%=FbgPlayer.Realm.GetUnitTypes().Count %>">Y: Your troops from this village</th>
                            <th class="troopsC headerTroopType" colspan="<%=FbgPlayer.Realm.GetUnitTypes().Count %>">C: Your troops Currently home</th>
                            <th class="troopsS headerTroopType" colspan="<%=FbgPlayer.Realm.GetUnitTypes().Count %>">S: Support troops at this village</th>
                            <th class="building headerTroopType" colspan="<%=FbgPlayer.Realm.BuildingTypes.Count %>">Buildings in village</th>

                        </tr>
                        <tr class="dataHeaderRow">
                            <th></th>
                            <th></th>
                            <th class="yeyHeader"></th>
                            <th ></th>
                            <th class="silverIcon"></th>
                            <th class="foodIcon"></th>
                      

                            <asp:Repeater EnableViewState="false" ID="Repeater6" runat="server">
                                <ItemTemplate>
                                    <th class="troopsA unitIcon" style="background-image: url('<%#Eval("ut.IconUrl") %>')"></th>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Repeater EnableViewState="false" ID="Repeater7" runat="server">
                                <ItemTemplate>
                                    <th class="troopsY unitIcon" style="background-image: url('<%#Eval("ut.IconUrl") %>')"></th>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Repeater EnableViewState="false" ID="Repeater8" runat="server">
                                <ItemTemplate>
                                    <th class="troopsC unitIcon" style="background-image: url('<%#Eval("ut.IconUrl") %>')"></th>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Repeater EnableViewState="false" ID="Repeater9" runat="server">
                                <ItemTemplate>
                                    <th class="troopsS unitIcon" style="background-image: url('<%#Eval("ut.IconUrl") %>')"></th>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Repeater EnableViewState="false" ID="Repeater10" runat="server">
                                <ItemTemplate>
                                    <th class="building fontSilverNumbersSm" style="background-image: url('<%#Eval("IconUrl_ThemeM") %>')"></th>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </thead>
                    <tbody>
                    <tr x="%x%" y="%y%" vid="%id%" opid="<%=FbgPlayer.ID.ToString() %>" data-vname="%villageName%" class="jsV sfx2 villlist_itemlist %currentVillageClass%" data-villistid="%id%" data-villnameselector=".villListName" data-vsjexcludedoptions="mapitX">
                        <td class="villNameCont">
                            <div class="villageImage" style="background-image: url('%bonusType%'), url('%villageImageUrl%');"></div>
                            <a class="villListName fontGoldFrLCmed sfx2">%villageName%</a>
                            <span class="villCoors fontDarkGoldNumbersSm">(%x%,%y%)</span>
                        </td>
                        <td class="villNameCont">
                            <span class="incoming incomingMiniIndicators helpTooltip" data-tooltipid="incomingMiniIndicators">%incomingImages%</span>
                        </td>
                        <td class="villNameCont">
                            <span class="yey %yeyClass% fontSilverNumbersSm">%yey%</span>
                        </td>
                        <td class="villCoorPts fontDarkGoldNumbersSm villPts">
                            %points%
                        </td>
                        <td class="silver %coinsClass%  ">                
                            %coins%
                        </td>

                        <td class="food %foodMaxed% ">
                            %popRemaining%
                        </td>
                   
                   
                        <asp:Repeater EnableViewState="false" ID="Repeater2" runat="server">
                            <ItemTemplate>
                                <td class="unit troopsA %troops.id<%#Eval("ut.ID")%>.TotalNowInVillageCount_font%" data-uid='<%#Eval("ut.ID") %>'>
                                    %troops.id<%#Eval("ut.ID")%>.TotalNowInVillageCount% <span class="rec">%troops.id<%#Eval("ut.ID")%>.CurrentlyRecruiting%</span>    
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Repeater EnableViewState="false" ID="Repeater3" runat="server">
                            <ItemTemplate>
                                <td class="unit troopsY %troops.id<%#Eval("ut.ID")%>.YourUnitsTotalCount_font%" data-uid='<%#Eval("ut.ID") %>' >
                                    %troops.id<%#Eval("ut.ID")%>.YourUnitsTotalCount% <span class="rec">%troops.id<%#Eval("ut.ID")%>.CurrentlyRecruiting%</span>    
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Repeater EnableViewState="false" ID="Repeater4" runat="server">
                            <ItemTemplate>
                                <TD class="unit troopsC %troops.id<%#Eval("ut.ID")%>.YourUnitsCurrentlyInVillageCount_font%" data-uid='<%#Eval("ut.ID") %>' >
                                    %troops.id<%#Eval("ut.ID")%>.YourUnitsCurrentlyInVillageCount% <span class="rec">%troops.id<%#Eval("ut.ID")%>.CurrentlyRecruiting%</span>    
                                </TD>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Repeater EnableViewState="false" ID="Repeater5" runat="server">
                            <ItemTemplate>
                                <TD class="unit troopsS %troops.id<%#Eval("ut.ID")%>.SupportCount_font%" data-uid='<%#Eval("ut.ID") %>' >
                                    %troops.id<%#Eval("ut.ID")%>.SupportCount%
                                </TD>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Repeater EnableViewState="false" ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <td class="building %buildings.id<%#Eval("ID")%>.lvl_font%" data-uid='<%#Eval("ID") %>'>%buildings.id<%#Eval("ID")%>.lvl% %buildings.id<%#Eval("ID")%>.busyTimeDisplay%</td>

                            </ItemTemplate>
                        </asp:Repeater>
                  



                    
                    </tr>
                    </tbody>
                </table>
            </div>
        

        </div>

        
        <div class="incomingMiniIndicatorsTooltips" style="display:none"></div>

        <div class="filterandsettings" style="display: none">
            <div class="detailSettings">
                <div><span class="troops" data-name="Enable"></span></div>
                <div><span class="troopsA" data-name="Enable"></span></div>
                <div><span class="troopsY" data-name="Enable"></span></div>
                <div><span class="troopsC" data-name="Enable"></span></div>
                <div><span class="troopsS" data-name="Enable"></span></div>
                <div class="indented"><span class="recruitingTroopNumbers" data-name="Enable"></span></div>
                <div><span class="buildings" data-name="Enable"></span></div>
                <div class="indented"><span class="buildingBusyTime" data-name="Enable"></span></div>
            </div>
        </div>


    </div>

</asp:Content>
