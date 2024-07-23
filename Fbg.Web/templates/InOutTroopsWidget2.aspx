<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="InOutTroopsWidget2.aspx.cs" Inherits="InOutTroopsWidget2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <style>
       
    </style>

    <div class="widget_inOutTroops getInOutDataChangedEvent">
        
        <div class="cmdList">
            <div class="refresh-NewData sfx2" style="text-align:center"><%=RS ("NewData") %></div>
            <div class="filter expanded">
                <div class="filterHeader  sfx2"> <span class="fontSilverFrSClrg"></span></div>
                <div class="byType"><div class="bydest"  style="display:block;overflow:hidden;"></div><div class="byTo"></div>
                        <div class="byFromOrToVillOrPlayer hidden"><span class="toVorP toV"><%=RS("TOV") %></span><span class="toVorP fromV"><%=RS("FROMV") %></span><span class="toVorP toP"><%=RS("TOP") %></span><span class="toVorP fromP"><%=RS("FROMP") %></span> <span class="actualVorP" /><a class="clearFilter">
                            <img src='https://static.realmofempires.com/images/icons/M_X.png' /></a></div>
                </div>
            </div>
            <div class="summary"></div>
            <table class="TypicalTable stripeTable incomingTroops" cellspacing="1" cellpadding="1" border="0">
                <thead>
                    <tr class="TableHeaderRow highlight">
                    </tr>
                </thead>
                <tbody>
                    <tr class="highlight cmd" data-eid="%cmd.eid%" data-hideState="%hidden%" data-etime="%cmd.etime%" data-ovid="%cmd.ovid%" data-opid="%cmd.opid%" data-dvid="%cmd.dvid%" data-dpid="%cmd.dpid%" data-ovname="%ov.name%" data-dvname="%dv.name%" >
                        <td class="data ">
                            <div class="troopMoveDetails1">

                                <div class="troopIconSheet44 cmdIcon %type%" ></div> 

                                <!--<div class="avatarIcon"></div>-->
                                <div class="cmdTimer countdown2 fontMonoNumberSm" data-finishesOn="%cmd.etime%" data-refreshCall="ROE.Troops.InOutWidget.commandCountdownAtZero(%cmd.eid%)" > %timeleft.h%:%timeleft.m%:%timeleft.s% </div>                                                               
                                <% if(realm.ID >= 100){ %><div class="speed fontMonoNumberSm">%speed%</div><%} %>
                                <% if(realm.Morale.IsActiveOnThisRealm){ %><div class="morale fontMonoNumberSm">%morale%</div><%} %>
                                <div class="dpname fontDarkGoldFrLCmed">%dp.name%</div>
                                <div class="dvcords fontDarkGoldFrLCmed">(%dv.x%,%dv.y%)</div>
                                <div class="dv fontDarkGoldFrLCmed">%dv.name%</div>

                                <!--<div class="villageIcon"></div>-->
                                <div class="arriveOn fontMonoNumberSm">%arrivalTime%</div> 
                                <div class="opname fontDarkGoldFrLCmed">%op.name%</div>
                                <div class="ovcords fontDarkGoldFrLCmed">(%ov.x%,%ov.y%)</div>                   
                                <div class="ov fontDarkGoldFrLCmed">%ov.name%</div>

                                <div class="ovunknown fontDarkGoldFrLCmed"><%=RS ("Unknown") %></div>
                            </div>                                            
                        </td>
                    </tr>
                </tbody>
            </table>
        <div class="utilFooter"></div>
    </div>

        <span class="htmlResources">
            <img src="https://static.realmofempires.com/images/troopIconSheet.png" />
        </span>
        <div class="troopMoveDetails2" style="display:none;">     
                   
            <div class="cmdTroopList">
                <div class="label">
                    <center>
                    <span class="loading"><%=RS ("loading troops")%> <img src='https://static.realmofempires.com/images/misc/busy_tinyred.gif'></img></span>
                    <asp:DataList EnableViewState="false" RepeatDirection="Horizontal" RepeatColumns="1" ID="dlTroops2" runat="server" CellPadding="0" CellSpacing="0" style="display:none;">
                        <ItemTemplate>
                            <img class="unitimg" border="0" runat="server" src='<%#Eval("IconUrl") %>' /><BR>
                            <span class="unitcountnumb" id='id<%#Eval("ID")%>' data-troopid="<%#Eval("ID")%>">%v.troops.id<%#Eval("ID")%>.YourUnitsTotalCount%</span>
                        </ItemTemplate>
                        <ItemStyle Wrap="false" CssClass="" />
                    </asp:DataList>
                        </center>
                                                       
                </div>
            </div>       

            <div class="lineInfo">

                <span class="LightText"><%=RS ("To:") %></span> 
                
                <span class="mode-outgoing dpname">
                    <a onclick="ROE.Frame.popupPlayerProfile('%dp.name%'); return false;" class="sfx2">%dp.name%</a> 
                    <img src="https://static.realmofempires.com/images/funnel1.png" class="filterButton dp"></img> - 
                </span>

                <a x="%dv.x%" y="%dv.y%" vid="%cmd.dvid%" opid="%dp.id%" class="jsV dv sfx2">%dv.name%(%dv.x%,%dv.y%)</a> 
                
                <img src="https://static.realmofempires.com/images/funnel1.png" class="filterButton dv"></img>
                
                
                <BR />
                
                
                
                <span class="LightText"><%=RS ("From:") %></span> 
                
                <span class="mode-incoming opname">
                    <a opid="%cmd.opid%" onclick="ROE.Frame.popupPlayerProfile('%op.name%'); return false;"class="sfx2">%op.name%</a>
                    <img src="https://static.realmofempires.com/images/funnel1.png" class="filterButton op"></img> -
                </span>
                
                <a x="%ov.x%" y="%ov.y%" vid="%cmd.ovid%" opid="%cmd.opid%"  class="jsV ov sfx2">%ov.name%(%ov.x%,%ov.y%)</a>
                
                <img src="https://static.realmofempires.com/images/funnel1.png" class="filterButton ov"></img>
                
                <span class="ovunknown">Unknown...</span>                                                               

            </div>

            <center>
                <div class=troopMoveActions> 
                    <div class="filtersCmdBtn smallRoundButtonDark " ><span class="filterIcon "></span></div>
                    <div class="profilesCmdBtn smallRoundButtonDark fontSilverNumbersLrg " >...</div>
                    <div data-eid="%eid%" class="troopIconSheet44 toggleHide hide-%hidden% clickable " />  
                    <div data-eid="%eid%" class="troopIconSheet44 cancel clickable " ></div> 
                    <div class="troopIconSheet44 cancel notAvail " ></div> 
                </div> 
            </center>

        </div>

    </div>
</asp:Content>
