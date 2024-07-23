<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="TroopInformation.aspx.cs" Inherits="templates_TroopInformation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">

 <div id="background">
                    <img src="https://static.realmofempires.com/images/misc/SplashScreenMuted.jpg" class="stretch" alt="" />
                </div>
    <div class="troopInformation_popup">
        <div class="troopHeader">
            <div class="troopHeaderInformation">
                <img src='https://static.realmofempires.com/images/icons/Q_Attack.png' /> <%=RS ("TotalAttack") %> <span class="attackStrength"></span>
                <br />
                <img src='https://static.realmofempires.com/images/icons/Q_Support.png' /> <%=RS ("Average") %> <span class="defenceStrength"></span>
            </div>
        </div>
        <div>
            <center><img src="https://static.realmofempires.com/images/misc/M_SpacerBottom.png" /></center>
            <section class="themeM-tabPanel clearfix">
                <section class="themeM-tabContent">
                    <div class="bg">
                        <div class="corner-br">
                        </div>
                    </div>
                    <div class="fg vilres_trooptable_cell">
                        <asp:DataList EnableViewState="false" RepeatDirection="Horizontal" RepeatColumns="2" ID="dlTroops" runat="server" CellPadding="0" CellSpacing="0" Width="100%">
                            <ItemTemplate>
                                <img class="vilres_unitimg" id="Img1" border="0" runat="server" src='<%#Eval("IconUrl_ThemeM") %>' />
                                <span class="vilres_unitcountnumb" id="id<%#Eval("ID")%>" data-troopid="<%#Eval("ID")%>">%v.troops.id<%#Eval("ID")%>.YourUnitsTotalCount%</span>
                            </ItemTemplate>
                            <ItemStyle Wrap="false" CssClass="" />
                        </asp:DataList>
                        <div class="footerActions">
                            <center>                        
                                <a href="unitssupportingpopup.aspx?vid=%v.id%" target=_blank <%if (IsiFramePopupsBrowser) { %> onclick="return popupWindowFromLink(this,'Detailed View',false);" <%} %> class="detailedSupport sfx2"style="display:none;"><%=RS ("Detailed")%></a>
                             <span class="vilres_mytroopbottomtabs">
                                    <a href="#" class="selected sfx2" id="vilres_myAllTroop" onclick="ROE.Frame.showTroopPopup_toggleTabs($(this).attr('id'))"><%=RS ("AllMyTroops")%></a> | 
                                        <a href="#" class="sfx2" id="vilres_myInVilTroop" onclick="ROE.Frame.showTroopPopup_toggleTabs($(this).attr('id'))"><%=RS ("NowIn")%></a>
                                </span>
                        </center>
                        </div>
                    </div>
                </section>
                <section class="themeM-tabs" id="vilres_trooptabs">
                    <ul>
                        <li><a href="#" class="selected sfx2" id="vilres_allTroop" onclick="ROE.Frame.showTroopPopup_toggleTabs($(this).attr('id'))"><%=RS ("AllTroops")%></a></li>
                        <li><a href="#" class="sfx2" id="vilres_myTroop" onclick="ROE.Frame.showTroopPopup_toggleTabs($(this).attr('id'))"><%=RS ("MyTroops")%></a></li>
                        <li><a href="#" class="oneLine sfx2" id="vilres_support" onclick="ROE.Frame.showTroopPopup_toggleTabs($(this).attr('id'))"><%=RS ("Support")%></a></li>
                    </ul>
                </section>
            </section>
            <center><img src="https://static.realmofempires.com/images/misc/M_SpacerTop.png" /></center>
        </div>
    </div>
</asp:Content>

