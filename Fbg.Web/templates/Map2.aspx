<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="Map2.aspx.cs" Inherits="templates_Map2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    <div id="map">
        <div id="mapwrap">
            <div id="surface"><div id="select">&nbsp;</div><div id="hover">&nbsp;</div></div>
        </div>
        <!--<div class="getInOutDataChangedEvent"></div>-->
        <div class="phrases">
            <%= RSdiv("InfoMinZoom")   %>
            <%= RSdiv("InfoMaxZoom")   %>
            <%= RSdiv("PopupTroopsAll")%>
            <%= RSdiv("PopupTroopsYourAll") %>
            <%= RSdiv("PopupTroopsYour")   %>
            <%= RSdiv("PopupTroopsSupport") %>
            <%= RSdiv("PopupTroopsGo") %>
            <%= RSdiv("PopupTroopsTitle") %>
            <%= RSdiv("PopupTroopsDescription") %>
            <%= RSdiv("TroopsNoSelectedTypes") %>
            <%= RSdiv("PopupPromotionTitle") %>
        </div>
    </div>

    <div id="promotionPopupTemplate" style="display: none">
        <div class="promotionPopup">
            <h3>Welcome to Village Promotion Feature - BETA 2!</h3>
            <ol>
                <li>Please pick the villages that you want to keep by &quot;promoting&quot; them - click it on the map, and click the thumbs up icon</li>
                <li>Keep promoting villages until there are no white shields left</li>
                <li>As you promote a village, <%=FbgPlayer.Realm.ConsolidationGet.NumVillagesAbsorbed.ToString() %> villages closest to it will be marked for absorption</li>
                <li>Villages that are promoted will move on to another age. Other villages will be absorbed to the remaining ones.</li>
            </ol>

            <br />

            <span style=font-size:14pt>
                You have <span class=countdown><%=Utils.FormatDuration(FbgPlayer.Realm.ConsolidationGet.TimeOfConsolidation.Subtract(DateTime.Now))%></span>
                left to promote all villages
            </span>
        
            <br />
        
            That is - ensure there are no white shields left.

            <h3>LEGEND</h3>

            <table>
                <tr><td><img src=https://static.realmofempires.com/images/map/fme.png /></td><td><b>Your village</b></td></tr>
                <tr><td><img src=https://static.realmofempires.com/images/map/Shield4_Yellow.png /></td><td><b>Your Promoted village</b><br />This village will remain in the new age</td></tr>
                <tr><td><img src=https://static.realmofempires.com/images/map/Shield4_Teal.png /></td><td><b>Your Absorbed village</b><br />This village will be absorbed to one of the promoted villages in the new age</td></tr>
            </table>
        
            <br />

            <%--<input type="button" class="promote" value="Promote">
            <input type="button" class="unpromote" value="UnPromote">--%>
            <input type="button" class="okBtn" value="Got It!">
        </div>
    </div>
    <div class="unitsMapDisplay">
        <table cellspacing="0" cellpadding="0">
            <tbody>
                <tr class="unitHdr" data-type="all">
                    <td class="troopTypeIcon">A:</td>
                    <asp:Repeater EnableViewState="false" ID="Repeater1" runat="server" >
                        <ItemTemplate>
                            <td><img class="unitimg" border="0" src='<%#Eval("IconUrl") %>' /></td>
                            <td><span class="unitcountnumb" data-troopid="<%#Eval("ID")%>">%v.troops.id<%#Eval("ID")%>.TotalNowInVillageCount%</span></td>
                        </ItemTemplate>
                    </asp:Repeater>
                    <td rowspan="4" width="50%" style="text-align: right">
                        <img src="https://static.realmofempires.com/images/icons/M_MoreSettings.png" style="height: 14px" class="troopSettings"></td>
                </tr>
                <tr class="unitHdr" data-type="your-all">
                    <td class="troopTypeIcon">Y:</td>
                    <asp:Repeater EnableViewState="false" ID="dlTroops2" runat="server" >
                        <ItemTemplate>
                            <td><img class="unitimg" border="0" src='<%#Eval("IconUrl") %>' /></td>
                            <td><span class="unitcountnumb" data-troopid="<%#Eval("ID")%>">%v.troops.id<%#Eval("ID")%>.YourUnitsTotalCount%</span></td>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
                <tr class="unitHdr" data-type="your">
                    <td class="troopTypeIcon">C:</td>
                    <asp:Repeater EnableViewState="false" ID="Repeater2" runat="server" >
                        <ItemTemplate>
                            <td><img class="unitimg" border="0" src='<%#Eval("IconUrl") %>' /></td>
                            <td><span class="unitcountnumb" data-troopid="<%#Eval("ID")%>">%v.troops.id<%#Eval("ID")%>.YourUnitsCurrentlyInVillageCount%</span></td>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
                <tr class="unitHdr" data-type="support">
                    <td class="troopTypeIcon">S:</td>
                    <asp:Repeater EnableViewState="false" ID="Repeater3" runat="server" >
                        <ItemTemplate>
                            <td><img class="unitimg" border="0" src='<%#Eval("IconUrl") %>' /></td>
                            <td><span class="unitcountnumb" data-troopid="<%#Eval("ID")%>">%v.troops.id<%#Eval("ID")%>.SupportCount%</span></td>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="unitsMapDisplay_loading">
        <table cellspacing="0" cellpadding="0">
            <tbody>
                <tr class="unitHdr" data-type="all">
                    <td class="troopTypeIcon">A:</td>
                    <asp:Repeater EnableViewState="false" ID="Repeater4" runat="server" >
                        <ItemTemplate>
                            <td><img class="unitimg" border="0" src='<%#Eval("IconUrl") %>' /></td>
                            <td><span class="unitcountnumb" data-troopid="<%#Eval("ID")%>">?</span></td>
                        </ItemTemplate>
                    </asp:Repeater>
                    <td rowspan="4" width="50%" style="text-align: right">
                        <img src="https://static.realmofempires.com/images/icons/M_MoreSettings.png" style="height: 14px" class="troopSettings"></td>
                </tr>
                <tr class="unitHdr" data-type="your-all">
                    <td class="troopTypeIcon">Y:</td>
                    <asp:Repeater EnableViewState="false" ID="Repeater5" runat="server" >
                        <ItemTemplate>
                            <td><img class="unitimg" border="0" src='<%#Eval("IconUrl") %>' /></td>
                            <td><span class="unitcountnumb" data-troopid="<%#Eval("ID")%>">?</span></td>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
                <tr class="unitHdr" data-type="your">
                    <td class="troopTypeIcon">C:</td>
                    <asp:Repeater EnableViewState="false" ID="Repeater6" runat="server" >
                        <ItemTemplate>
                            <td><img class="unitimg" border="0" src='<%#Eval("IconUrl") %>' /></td>
                            <td><span class="unitcountnumb" data-troopid="<%#Eval("ID")%>">?</span></td>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
                <tr class="unitHdr" data-type="support">
                    <td class="troopTypeIcon">S:</td>
                    <asp:Repeater EnableViewState="false" ID="Repeater7" runat="server" >
                        <ItemTemplate>
                            <td><img class="unitimg" border="0" src='<%#Eval("IconUrl") %>' /></td>
                            <td><span class="unitcountnumb" data-troopid="<%#Eval("ID")%>">?</span></td>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
