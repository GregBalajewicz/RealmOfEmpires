<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="VillageList_m.aspx.cs" Inherits="templates_VillageList_m" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    
    <div id="villageListPopup">
        <div class="filterArea">

            <div onclick="ROE.UI.VillageList.refreshVillList();" class="villlist_refresh ButtonTouch sfx2"></div>
            

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
            <div class="filtersCmdBtn smallRoundButtonDark sfx2">
                <span class="filterIcon"></span>
            </div>
        </div>
        <div class="villageList">
            <div x="%x%" y="%y%" vid="%id%" opid="<%=FbgPlayer.ID.ToString() %>" data-vname="%villageName%" class="jsV sfx2 villlist_itemlist %currentVillageClass%" data-villistid="%id%">
                <div class="villageImage" style="background-image: url('%bonusType%'), url('%villageImageUrl%');"></div>              
                <a class="villListName fontGoldFrLCmed">%villageName%</a>
                <span class="villCoorPts fontDarkGoldNumbersMed"><span class="villCoors">(%x%,%y%)</span><span class="villPts">[%points%]</span></span>
                
                <span class="incoming incomingMiniIndicators">%incomingImages%</span>
                
                <div class="details">                   
                    <span class="silver %coinsClass%  ">%coins%</span>
                    <span class="food %foodMaxed% " >%popRemaining%</span>
                    <!-- //disabled untill proper images are made
                    <span class="buildingIcon %isBuilding%"></span>
                    <span class="recruitingIcon %isRecruiting%"></span>
                        -->
                    <span class="yey %yeyClass% ">%yey%</span>
                </div>

                <div class="troops">  
                    <div class="troopSource troopsA">
                        <div class="sourceLabel">A</div>
                        <asp:Repeater EnableViewState="false" ID="Repeater2" runat="server">
                            <ItemTemplate>
                                <div class="unit %troops.id<%#Eval("ut.ID")%>.YourUnitsTotalCount_font%" data-uid='<%#Eval("ut.ID") %>' style="background-image:url('<%#Eval("ut.IconUrl") %>')">
                                    %troops.id<%#Eval("ut.ID")%>.TotalNowInVillageCount%<span class="rec">%troops.id<%#Eval("ut.ID")%>.CurrentlyRecruiting%</span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>   
                    </div>  
                    <div class="troopSource troopsY">
                        <div class="sourceLabel">Y</div>
                        <asp:Repeater EnableViewState="false" ID="Repeater3" runat="server">
                            <ItemTemplate>
                                <div class="unit %troops.id<%#Eval("ut.ID")%>.YourUnitsTotalCount_font%" data-uid='<%#Eval("ut.ID") %>' style="background-image:url('<%#Eval("ut.IconUrl") %>')">
                                    %troops.id<%#Eval("ut.ID")%>.YourUnitsTotalCount%<span class="rec">%troops.id<%#Eval("ut.ID")%>.CurrentlyRecruiting%</span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>   
                    </div>            
                    <div class="troopSource troopsC">
                        <div class="sourceLabel">C</div>
                        <asp:Repeater EnableViewState="false" ID="Repeater4" runat="server">
                            <ItemTemplate>
                                <div class="unit %troops.id<%#Eval("ut.ID")%>.YourUnitsCurrentlyInVillageCount_font%" data-uid='<%#Eval("ut.ID") %>' style="background-image:url('<%#Eval("ut.IconUrl") %>')">
                                    %troops.id<%#Eval("ut.ID")%>.YourUnitsCurrentlyInVillageCount%<span class="rec">%troops.id<%#Eval("ut.ID")%>.CurrentlyRecruiting%</span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>   
                    </div> 
                    <div class="troopSource troopsS">
                        <div class="sourceLabel">S</div>
                        <asp:Repeater EnableViewState="false" ID="Repeater5" runat="server">
                            <ItemTemplate>
                                <div class="unit %troops.id<%#Eval("ut.ID")%>.SupportCount_font%" data-uid='<%#Eval("ut.ID") %>' style="background-image:url('<%#Eval("ut.IconUrl") %>')">
                                    %troops.id<%#Eval("ut.ID")%>.SupportCount%
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>   
                    </div>                            
                </div>
                 <div class="buildings">  
                    <asp:Repeater EnableViewState="false" ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <div class="building %buildings.id<%#Eval("ID")%>.lvl_font%" data-uid='<%#Eval("ID") %>'  style="background-image:url('<%#Eval("IconUrl_ThemeM") %>')">
                                 %buildings.id<%#Eval("ID")%>.busyTimeDisplay% %buildings.id<%#Eval("ID")%>.lvl%
                            </div>

                        </ItemTemplate>
                    </asp:Repeater>                                     
                </div>
              
                <div class="sectionDivider"></div>                        
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
