<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="MapHighlights.aspx.cs" Inherits="templates_MapHighlights" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <%if (isMobile) { %>
    <div id="background">
        <img src="https://static.realmofempires.com/images/backgrounds/M_BG_Research.jpg" class="stretch" alt="" />
    </div>
    <%} %>

    <div id="highlight_popup">
        
        <div class="bg themeM-tabContent">

            <!-- List Section -->
            <div class="list section showed">

                <div class="description highlightPanel fontGoldFrLCmed ">
                    <%=RS("Description") %>
                </div>
                
                <div class="addNew highlightPanel fontGoldFrLCmed ">
                    <div class="addNewRule BtnBLg2 fontButton1L"><%=RS("AddNewRule") %><span class="largeArrowR"></span></div>
                </div>
                            
                <div class="legendsRulesList highlightPanel">
                    <div class="panelTitle fontGoldFrLCXlrg"><%=RS("Highlights") %></div>
                    <div class="sectionDivider"></div>
                    <table class="rows" width="100%" >
                        <tbody></tbody>
                    </table>
                    <h1 class="empty"><center><%=RS("NoHighlights") %></center></h1>
                </div>

                <div class="legendsList highlightPanel"> 
                    <div class="panelTitle fontGoldFrLCXlrg"><%=RS("Legend") %></div>
                    <div class="sectionDivider"></div>
                    <div class="legendsListBorder">                  
                        <%if (isMobile) { %>
                        <div>
                            <img src="https://static.realmofempires.com/images/map/Shield2b_Black.png" />
                            <span class="fontSilverFrSCmed"><%=RS("YourSelectedVillage") %></span>
                        </div>
                        <%} %>
                        <div>
                            <img src="https://static.realmofempires.com/images/map/fme.png" />
                            <span class="fontSilverFrSCmed"><%=RS("YourVillage") %></span>
                        </div>
                        <div>
                            <img src="https://static.realmofempires.com/images/map/fc.png" />
                            <span class="fontSilverFrSCmed"><%=RS("YourClanmates") %></span>
                        </div>
                        <div>
                            <img src="https://static.realmofempires.com/images/map/fa.png" />
                            <span class="fontSilverFrSCmed"><%=RS("AllyVillage") %></span>
                        </div>
                        <div>
                            <img src="https://static.realmofempires.com/images/map/fn.png" />
                            <span class="fontSilverFrSCmed"><%=RS("NAPvillage") %></span>
                        </div>
                        <div>
                            <img src="https://static.realmofempires.com/images/map/fe.png" />
                            <span class="fontSilverFrSCmed"><%=RS("EnemyVillage") %></span>
                        </div>
                        <div>
                            <img src="https://static.realmofempires.com/images/map/freb.png" />
                            <span class="fontSilverFrSCmed"><%=RS("RebelVillage") %></span>
                        </div>
                        <div>
                            <img src="https://static.realmofempires.com/images/map/fabd.png" />
                            <span class="fontSilverFrSCmed"><%=RS("AbandonedVillage") %></span>
                        </div>
                    </div> 
                </div>

            </div>

            <!-- Adding Section -->
            <div class="add section">
                <div class="highlightPanel">
                    <div class="addType BtnDLg2 fontButton1L" type="clan"><%=RS("Clan") %><span class="largeArrowR"></span></div>
                </div>
                 <div class="highlightPanel">   
                    <div class="addType BtnDLg2 fontButton1L" type="player"><%=RS("Player") %><span class="largeArrowR"></span></div>
                </div>
                <div class="highlightPanel">
                    <div class="addType BtnDLg2 fontButton1L" type="playernote"><%=RS("PlayerNote") %><span class="largeArrowR"></span></div>
                    <div class="fontGoldFrLClrg"><%=RS("PlayerNoteDesc") %></div>
                </div>
                <div class="highlightPanel">
                    <div class="addType BtnDLg2 fontButton1L" type="villagenote"><%=RS("VillageNote") %><span class="largeArrowR"></span></div>
                    <div class="fontGoldFrLClrg"><%=RS("VillageNoteDesc") %></div>
                </div>
                <div class="sectionFooter">
                    <div class="stripe"></div>
                    <div class="back BtnDSm2n fontButton1L"><%=RS("Back") %><span class="smallArrowLeft"></span></div>
                </div>
            </div>


            <!-- Editing Section -->
            <div class="edit section">
                <div class="highlightPanel">
                    <div class="type fontGoldFrLCXlrg"></div>
                    <div class="changeShield BtnDLg2 fontButton1L"><%=RS("ChangeShield") %><span class="flag"></span></div>
                    <input type="text" class="keyword"></input>
                    <div class="autocomplete"></div> 
                    <div class="editButtonPanel">
                        <div class="saving BtnBSm1 fontButton1L" ><%=RS("SAVE") %></div>
                        <div class="remove BtnBSm1 fontButton1L" ><%=RS("REMOVE") %></div>
                        <div class="cancel BtnBSm1 fontButton1L" ><%=RS("CANCEL") %></div>
                    </div>
                </div>                
                <div class="sectionFooter">
                    <div class="stripe"></div>
                    <div class="back BtnDSm2n fontButton1L"><%=RS("Back") %><span class="smallArrowLeft"></span></div>
                </div>
            </div>

            <div class="template item">
                <div class="hand-icon" ></div>
                <img class="flag-icon" />
                <div class="keyword fontGoldFrLClrg"></div>
                <span class="type fontSilverFrSCmed"></span>
            </div>

        </div>

        <div class="phrases">
            <%= RSdiv("InfoEmptyKeyword") %>
        </div>
    </div>

    

</asp:Content>