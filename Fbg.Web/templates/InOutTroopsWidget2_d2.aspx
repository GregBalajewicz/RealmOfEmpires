<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="InOutTroopsWidget2_d2.aspx.cs" Inherits="InOutTroopsWidget2_d2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <style>
       
    </style>

    <div class="widget_inOutTroops getInOutDataChangedEvent">
        
        <div class="cmdList">
            <div class="refresh-NewData sfx2" style="text-align:center"><%=RS ("NewData") %></div>
            <div class="filter">
                <div class="filterHeader  sfx2">
                    <span class="fontSilverFrSClrg filterTitle"><%=RS ("Filter") %></span>
                    <div class="byType">
                        <div class="byFromOrToVillOrPlayer hidden"><span class="toVorP toV"><%=RS("TOV") %></span><span class="toVorP fromV"><%=RS("FROMV") %></span><span class="toVorP toP"><%=RS("TOP") %></span><span class="toVorP fromP"><%=RS("FROMP") %></span> <span class="actualVorP" /><a class="clearFilter">
                            <img src='https://static.realmofempires.com/images/icons/M_X.png' /></a></div>
                    </div>
                </div>
                <div class="smallRoundButtonDark fontSilverNumbersLrg loadMany" ><div class="icon"></div></div>
                <div class="BtnBLg1 fontSilverFrSClrg summaryButton">Summary</div>
            </div>
            <table class="TypicalTable stripeTable incomingTroops" cellspacing="1" cellpadding="1" border="0">
                <thead>
                    <tr class="TableHeaderRow highlight">                      
                        <th scope="col" style="min-width: 24px;">
                            &nbsp;
                        </th>
                        <th scope="col">
                           <%=RS ("From") %> 
                        </th>
                        <th scope="col" class="dv">
                           <%=RS ("To") %> 
                        </th>
                        <th scope="col" style="white-space: nowrap;">
                            <%=RS ("ArrivalIn") %>
                        </th>
                        <th scope="col" style="white-space: nowrap;" class="arrivalOnColumn">
                           <%=RS ("ArrivalOn") %> 
                        </th>
                         <% if(realm.ID >= 100){ %><th class="speed"></th><%} %>
                        <% if(realm.Morale.IsActiveOnThisRealm){ %><th class="morale"></th><%} %>
                        <th scope="col" style="min-width: 16px;">
                            &nbsp;
                        </th>
                        <th scope="col" style="min-width: 12px;">
                            &nbsp;
                        </th>
                        <asp:Repeater EnableViewState="false" ID="Repeater1" runat="server" >
                            <ItemTemplate>
                                <th class="troopCount">
                                    <img border="0" runat="server" src='<%#Eval("IconUrl") %>' />
                                </th>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </thead>
                <tbody>
                    <tr class="highlight cmd" data-eid="%cmd.eid%" data-hideState="%hidden%" data-etime="%cmd.etime%" data-ovid="%cmd.ovid%" data-opid="%cmd.opid%" data-dvid="%cmd.dvid%" data-dpid="%cmd.dpid%" data-ovname="%ov.name%" data-dvname="%dv.name%" >
                        <td style="white-space: nowrap;">
                            <div class="troopIconSheet %type%"  ></div>
                            <div data-eid="%cmd.eid%" class="troopIconSheet cancel clickable" ></div>                                  
                        </td>
                        <td style="white-space: nowrap;">
                            <span class="mode-incoming opname">
                            <a onclick="ROE.Frame.popupPlayerProfile('%op.name%'); return false;">%op.name%</a> - </span>
                            <a x="%ov.x%" y="%ov.y%" vid="%cmd.ovid%" opid="%cmd.opid%" class="jsV ov" data-vname="%ov.name%"
                                data-addCustomActions="ROE.Troops.InOutWidget.getFilterAction('from', %cmd.ovid%, '%ov.name%', %ov.x%,%ov.y%, %widgetID%)">(%ov.x%,%ov.y%) %ov.name%</a>
                             <div class="ovunknown fontDarkGoldFrLCsm"><%=RS ("Unknown") %></div>
                        </td>
                        <td style="white-space: nowrap;" class="dv">
                            <span class="mode-outgoing dpname">
                            <a onclick="ROE.Frame.popupPlayerProfile('%dp.name%'); return false;">%dp.name%</a> - </span>
                            <a x="%dv.x%" y="%dv.y%" vid="%cmd.dvid%" opid="%cmd.dpid%" class="jsV"  data-vname="%dv.name%"
                                data-addCustomActions="ROE.Troops.InOutWidget.getFilterAction('to', %cmd.dvid%, '%dp.name%', %dv.x%,%dv.y%, %widgetID%)">(%dv.x%,%dv.y%) %dv.name%</a>                                                    
                        </td>
                        <td class="countdown2" data-finishesOn="%cmd.etime%" data-refreshCall="ROE.Troops.InOutWidget.commandCountdownAtZero(%cmd.eid%)" style="white-space: nowrap;">
                            %timeleft.h%:%timeleft.m%:%timeleft.s%
                        </td>
                        <td style="white-space: nowrap;" class="arrivalOnColumn">
                            %arrivalTime%
                        </td>
                         <% if(realm.ID >= 100){ %><td class="NoPad speed">%speed%</td><%} %>
                        <% if(realm.Morale.IsActiveOnThisRealm){ %><td class="NoPad morale">%morale%</td><%} %>
                        <td class="NoPad" style="white-space: nowrap;">
                            <div data-eid="%cmd.eid%" class="troopIconSheet toggleHide hide-%hidden% clickable" />
                                                        
                        </td>
                        <td class="NoPad" style="white-space: nowrap;" colspan="">
                            <div class="troopIconSheet rep clickable loadDetails" ></div>
                        </td>
                        <asp:Repeater EnableViewState="false" ID="Repeater2" runat="server" >
                            <ItemTemplate>
                                <td class="troopCount ZeroUnitCount" data-unitid="<%#Eval("ID")  %>">
                                    
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </tbody>
            </table>
        <div class="utilFooter"></div>
    </div>

        <span class="htmlResources">
            <img src="https://static.realmofempires.com/images/troopIconSheet.png" />
        </span>
    </div>
</asp:Content>
