<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="PlayerNewPopup.aspx.cs" Inherits="templates_PlayerNewPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <script src="script-nochange/jquery.flot.min.js" type="text/javascript"></script>
    <script src="script-nochange/jquery.flot.navigate.min.js" type="text/javascript"></script>
    <script src="script-nochange/jquery.flot.time.min.js" type="text/javascript"></script>

    <div id="playernew_popup" >
        
        <div class="mainset slideRightTo transition"  >

            <section>
               <div class="block ButtonTouch sfx2" ></div>                
               <div class="title"></div> 
               <div class="plname"></div>
               <!--<div class="sendMessage ButtonTouch sfx2 helpTooltip" data-toolTipID="playerPopupSendMessage" ></div>-->
               <div class="chatWith ButtonTouch sfx2 helpTooltip" data-toolTipID="playerPopupChatWith" ></div>  
            </section>
            <div class="separator2"></div>
                                        
            <section>
                <div class="rank"><%=RS("Rank") %> <span></span> <%=RS("inRealm") %></div>
                <div class="claninfo"><%=RS("ofClan") %> <span class="clanlink ButtonTouch"></span></div>
                <div class="inviteClan ButtonTouch helpTooltip" data-toolTipID="playerPopupInviteClan" ></div>
            </section>
            <div class="separator1"></div>

            <!--
            <div class="avatorChanger">
                <div class="avatarLeft ButtonTouch" ></div>
                <div class="avatarRight ButtonTouch" ></div>
            </div>
            -->


            <div class="bottominfo" >     
                <div class="viewThrone"></div>           
                <div class="separator1"></div>
                    <div class="govtype" onclick="ROE.UI.Sounds.click(); parent.ROE.UI.GovTypeSelect.init_DisplayOnly();" ></div>
                    <div class="vills infos" data-info="vills" ><span class="villages"><span></span> <%=RS("pts") %></span> <span class="lands"><%=RS("SovereignLands") %></span></div>
                <div class="separator2"></div>
                    <div class="profile infos" data-info="profile" ><span class="lands"><%=RS("PlayerProfile") %></span></div>
                <div class="separator2"></div>
                    <div class="notes infos" data-info="notes" ><span class="lands"><%=RS("MyNotes") %></span></div>
                <div class="separator2"></div>
                    <div class="stats infos" data-info="stats" ><%=RS("HistoryAccomplishments") %></div>
            </div>

        </div>
        <div class="sideset slideRightFrom transition" >

            <div class="mainBack themeM-panel style-link BarTouch sfx2">
                <div class="bg">
                    <div class="corner-br"></div>
                </div>

                <div class="fg">
                    <div class="themeM-more">
                        <div class="bg">
                        </div>

                        <div class="fg">
                            <div class="label">
                                <span></span><br>
                            </div>

                            <div class="arrow-l"></div>
                        </div>
                    </div>

                    <div class="label">
                        <span><%=RS("BACK") %></span><br>
                    </div>
                </div>
            </div>
                        

                <div class="info vills">
                    <div class="infotitle"><%=RS("SovereignLands") %></div>
                    <div class="villtable"></div>
                </div>

                <div class="info profile">
                    <div class="infotitle"><%=RS("PlayerProfile") %></div>
                    <div class="editButton">
                        <div class="Edit customButtomBG" ><%=RS("Edit") %></div>
                    </div>
                    <div class="ProfileText"></div>
                    <div class="EditMessage">
                        <textarea rows=10 id="EditorProfile" autofocus onfocus="ROE.Frame.inputFocused();" onblur="ROE.Frame.inputBlured();"></textarea>
                        <div class="paddinator2000"></div>
                    </div>
                </div>

                <div class="info notes">
                    <div class="infotitle"><%=RS("MyNotes") %></div>
                    <div class="editButton">
                        <div class="Edit customButtomBG" ><%=RS("Edit") %></div>
                    </div>
                    <div class="NotesText"></div>
                    <div class="EditMessage">
                        <textarea rows=10 id="EditorNotes" autofocus onfocus="ROE.Frame.inputFocused();" onblur="ROE.Frame.inputBlured();"></textarea>
                        <div class="paddinator2000"></div>
                    </div>
                </div>

                <div class="info stats">
                    <div class="infotitle"><%=RS("HistoryAccomplishments") %></div>
                    <div class="hs panel">
                        <div class="graph" style="width: 99%; height: 200px;"></div>
                                    
                        <div class="links">
                            <a tp="villages" class="action" class="sfx2"><%=RS("StatNumVillage") %></a>| 
                            <a tp="points" class="action" class="sfx2"><%=RS("StatPointsVillage") %></a>|
                            <a tp="attack" class="action" class="sfx2"><%=RS("StatAttackPoints") %></a>|
                            <a tp="defence" class="action" class="sfx2"><%=RS("StatDefencePoints") %></a>|                       
                            <a time="last"class="sfx2"><%=RS("StatLast30days") %></a>| 
                            <a time="all" class="sfx2"><%=RS("StatFullHistory") %></a>
                        </div>
                    </div>
                </div>

        </div>

        <div class="phrases">
            <div ph="1"><%=RS("Avatarchanged") %></div>
            <div ph="2"><%=RS("VillageName") %></div>
            <div ph="3"><%=RS("VillagePoints") %></div>
            <div ph="4"><%=RS("Edit") %></div>
            <div ph="5"><%=RS("Save") %></div>
            <div ph="6"><%=RS("PlayerProfile") %></div>
            <div ph="7"><%=RS("MyNotes") %></div>
        </div>
    </div>

</asp:Content>