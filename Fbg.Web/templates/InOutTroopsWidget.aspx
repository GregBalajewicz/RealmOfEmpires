<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="InOutTroopsWidget.aspx.cs" Inherits="InOutTroopsWidget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <style>
       
    </style>

    <div class="widget_inOutTroops getInOutDataChangedEvent">
        
        <div class="cmdList">
            <div class="refresh-NewData sfx2" style="text-align:center"><%=RS ("NewData") %></div>
            <div class="filter">
                <div class="filterHeader  sfx2"> <span><%=RS ("Filter") %></span> <div class="action-expandColapse"></div></div>
                <div class="byType"><div class="bydest" style="display:block;overflow:hidden;"></div></div>
            </div>
            <table class="TypicalTable stripeTable incomingTroops" cellspacing="1" cellpadding="1" border="0">
                <thead>
                    <tr class="TableHeaderRow highlight">
                        <%if (isMobile){ %>
                        <%} else { %>
                        <th scope="col">
                            &nbsp;
                        </th>
                        <th scope="col">
                            &nbsp;
                        </th>
                        <th scope="col">
                           <%=RS ("From") %> 
                        </th>
                        <th scope="col">
                           <%=RS ("To") %> 
                        </th>
                        <th scope="col">
                            <%=RS ("ArrivalIn") %>
                        </th>
                        <th scope="col">
                           <%=RS ("ArrivalOn") %> 
                        </th>
                        <th scope="col">
                            &nbsp;
                        </th>
                        <th scope="col">
                            &nbsp;
                        </th>
                        <%} %>
                    </tr>
                </thead>
                <tbody>
                    <tr class="highlight cmd RowTouch" data-eid="%cmd.eid%" data-hideState="%hidden%">
                        <%if (isMobile){ %>
                        <td class="data sfxOpen">
                            <div class="troopIconSheet44 %type% " ></div>
                            <img src='https://static.realmofempires.com/images/misc/M_ArrowR.png' class="rightArrows"></img> 
                            <span class="LightText"><%=RS ("To") %></span> <span class="mode-outgoing dpname">%dp.name% - </span>
                                <span class=" dv">%dv.name%(%dv.x%,%dv.y%)</span>
                            <BR /><span class="LightText"><%=RS ("From") %></span> <span class="mode-incoming opname">%op.name% -</span>
                                        <span class=" ov">%ov.name%(%ov.x%,%ov.y%)</span><span class="ovunknown"><%=RS ("Unknown") %></span>
                            <BR /><span class="countdown" refresh=false style="white-space: nowrap;"> %timeleft.h%:%timeleft.m%:%timeleft.s% </span><span class=arriveOn>%arrivalTime%</span>                                                
                        </td>
                        <%} else { %>
                        <td style="white-space: nowrap;">
                            <div class="troopIconSheet %type%"  ></div>
                        </td>
                        <td class="NoPad" style="white-space: nowrap;" >              
                            <div onclick="Cancel(this, %cmd.eid%);return false;" class="troopIconSheet cancel clickable" ></div>                                  
                        </td>
                        <td style="white-space: nowrap;">
                            <span class="mode-incoming opname">
                            <a href="PlayerPopup.aspx?pid=%cmd.opid%" onclick="ROE.Frame.popupPlayerProfile('%op.name%'); return false;">%op.name%</a> - </span>
                            <a href="VillageOverview.aspx?svid=%cmd.ovid%" x="%ov.x%" y="%ov.y%" vid="%cmd.ovid%" opid="%cmd.opid%"  class="jsV">%ov.name%(%ov.x%,%ov.y%)</a>
                        </td>
                        <td style="white-space: nowrap;">
                            <span class="mode-outgoing dpname">
                            <a href="PlayerPopup.aspx?pid=%cmd.opid%" onclick="ROE.Frame.popupPlayerProfile('%dp.name%'); return false;">%dp.name%</a> - </span>
                            <a href="VillageOverview.aspx?svid=%cmd.dvid%" x="%dv.x%" y="%dv.y%" vid="%cmd.dvid%" opid="%dp.id%" class="jsV">%dv.name%(%dv.x%,%dv.y%)</a>                                                    
                        </td>
                        <td class="countdown" refresh=false style="white-space: nowrap;">
                            %timeleft.h%:%timeleft.m%:%timeleft.s%
                        </td>
                        <td style="white-space: nowrap;">
                            %arrivalTime%
                        </td>
                        <td class="NoPad" style="white-space: nowrap;">
                            <div onclick="Details_get(this, %cmd.eid%);return false;" class="troopIconSheet rep clickable" ></div>
                        </td>
                        <td class="NoPad" style="white-space: nowrap;">
                            <div onclick="ToggleHide(this,  %cmd.eid%);return false;" class="troopIconSheet hide-%hidden% clickable" />
                                                        
                        </td>
                        <%} %>
                    </tr>
                </tbody>
            </table>
        <div class="utilFooter"></div>
    </div>

        <span class="htmlResources">
            <img src="https://static.realmofempires.com/images/troopIconSheet.png" />
        </span>
         <%if (isMobile)
           { %>
         

        <div class="troopMoveDetails slideLeftTo"> 
            <div class="themeM-panel style-link action-back BarTouch">
                <div class="bg">
                    <div class="corner-br"></div>
                </div>

                <div class="fg">
                    <div class="themeM-more">
                        <div class="bg">
                            <div class="corner-tl"></div>
                        </div>

                        <div class="fg">
                            <div class="label">
                                <span></span><br>
                            </div>

                            <div class="arrow-l sfx2"></div>
                        </div>
                    </div>

                    <div class="label sfxOpen">
                        <span><%=RS ("Back") %></span><br>
                    </div>
                </div>
            </div>

           
            <div class="troopIconSheet44 %cmdType_className%" ></div>
            <span class="cmdType out attack LightText"><%=RS ("OutgoingAttack") %></span><span class="cmdType out support LightText"><%=RS ("Outgoing Support") %></span>
            <span class="cmdType in attack LightText"><%=RS ("IncomingAttack") %></span><span class="cmdType in support LightText"><%=RS ("IncomingSupport") %></span>
            <span class="cmdType in return LightText"><%=RS ("Returning troops") %></span>
            <BR/><span class="arriveOnLine"><%=RS ("Arriving") %><span class=arriveOn>%arrivalTime%</span></span>   

            <BR">
            <div class="lineInfo">
                <span class="LightText"><%=RS ("To:") %></span> <span class="mode-outgoing dpname"><a href="PlayerPopup.aspx?pid=%cmd.opid%" onclick="ROE.Frame.popupPlayerProfile('%dp.name%'); return false;" class="sfx2">%dp.name%</a> - </span>
                    <a href="VillageOverview.aspx?svid=%cmd.dvid%" x="%dv.x%" y="%dv.y%" vid="%cmd.dvid%" opid="%dp.id%" class="jsV dv sfx2">%dv.name%(%dv.x%,%dv.y%)</a>
                <BR /><span class="LightText"><%=RS ("From:") %></span> <span class="mode-incoming opname"><a href="PlayerPopup.aspx?pid=%cmd.opid%" opid="%cmd.opid%" onclick="ROE.Frame.popupPlayerProfile('%op.name%'); return false;"class="sfx2">%op.name%</a> -</span>
                            <a href="VillageOverview.aspx?svid=%cmd.ovid%" x="%ov.x%" y="%ov.y%" vid="%cmd.ovid%" opid="%cmd.opid%"  class="jsV ov sfx2">%ov.name%(%ov.x%,%ov.y%)</a><span class="ovunknown">Unknown...</span>
                <BR /><span class="LightText"><%=RS("ArrivingIn") %></span> <span class="countdown" refresh=false style="white-space: nowrap;"> %timeleft.h%:%timeleft.m%:%timeleft.s% </span>
                                                            
            </div>
                <center>
            <div class=troopMoveActions> 
                <div onclick="ROE.Troops.InOut2.cancel(this, %eid%);" class="troopIconSheet44 cancel clickable sfx2" ></div> 
                <div class="troopIconSheet44 cancel notAvail" ></div> 
                <div onclick="ROE.Troops.InOut2.toggleHide(this, %eid%);" class="troopIconSheet44 hide-%hidden% clickable sfx2" />
            </div> 
                    </center>
            <div class="cmdTroopList">
                <div class="themeM-panel">
                    <div class="bg">
                        <div class="corner-tl"></div>
                        <div class="corner-br"></div>
                    </div>

                    <div class="fg">
                        

                        <div class="label">
                            <center>
                            <span class="loading"><%=RS ("loading troops")%> <img src='https://static.realmofempires.com/images/misc/busy_tinyred.gif'></img></span>
                            <asp:DataList EnableViewState="false" RepeatDirection="Horizontal" RepeatColumns="2" ID="dlTroops" runat="server" CellPadding="0" CellSpacing="0" style="display:none;">
                                <ItemTemplate>
                                    <img class="unitimg" border="0" runat="server" src='<%#Eval("IconUrl_ThemeM") %>' />
                                    <span class="unitcountnumb" id='id<%#Eval("ID")%>' data-troopid="<%#Eval("ID")%>">%v.troops.id<%#Eval("ID")%>.YourUnitsTotalCount%</span>
                                </ItemTemplate>
                                <ItemStyle Wrap="false" CssClass="" />
                            </asp:DataList>
                                </center>
                                                       
                        </div>
                    </div>
                </div>
            </div>
        </div> 
        <%} %>
    </div>
</asp:Content>
