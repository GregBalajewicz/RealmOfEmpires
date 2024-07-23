<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="ReportsPopup.aspx.cs" Inherits="templates_ReportsPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

    <%if (isMobile) { %>
   <div id="background" style="display:none;">
        <img src="https://static.realmofempires.com/images/misc/SplashScreenMuted.jpg" class="stretch" alt="" />
    </div>
    <%} %>

    <section id="reports_popup" class="themeM-page">
        <div class="fg main-fg">
            <section class="themeM-view default transition">
                <div class="list">
                  
                     <div class="listTools">
                        <div class="listToolsFilter filterReports">
                             <div class="reload smallRoundButtonDark sfx2 listToolBtn" onclick="ROE.Reports.showLatestReports();">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/buttons/M_RefreshSm.png');"></div>
                            </div>
                           
                                <div class="hiddenDuringFilter">
                                    <input type="text" id="reportFilterInput" class="filterInput" />
                                    <div class="filterListBtn listToolBtn sfx2" onclick="ROE.Reports.filterReports();">
                                        <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/magnifyingGlass.png');"></div>
                                    </div>                               
                                                 
                                    <div class="filterStarredOnlyBtn listToolBtn sfx2" onclick="ROE.Reports.showStarredOnly();">
                                        <div class="listToolIcon starredIcon"></div>
                                    </div>  
                                </div>

                            
                               
                                <div class="hideToggleBtn smallRoundButtonDark sfx2 listToolBtn">
                                    <div class="listToolIcon"></div>
                                </div>
                            
                                <div class="filterTerm">Filtering by:<br /><span id="filterApplied"></span> (<span id="numReportsFound"></span>)</div> 
                                <div class="clearFilterBtn listToolBtn sfx2" onclick="ROE.Reports.clearReportsFilter();">
                                    <img src="https://static.realmofempires.com/images/icons/M_X.png">
                                </div>                     
                            </div>
                        
                        <div class="listToolsSelect hide">     
                                 
                            <div class="filterSpyOnlyBtn listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showSpyOnly();">
                                <div class="listToolIcon spyIcon"></div>
                            </div>  
                                                                        
                             <div class="filterGovAttacksOnlyBtn listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showGovAttacksOnly();">
                                <div class="listToolIcon govIcon"></div>
                            </div>  

                            <div class="filterSuccessOnlyBtn listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showSuccessReportsOnly();">
                                <div class="listToolIcon successIcon"></div>
                            </div> 

                            <div class="filterDefeatOnlyBtn listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showDefeatReportsOnly();">
                                <div class="listToolIcon defeatIcon"></div>
                            </div> 
                                                    
                            <div class="selectAllItemsBtn listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.selectAllReports();">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_ReportsSelectAll.png');"></div>
                            </div>
                            <div class="deselectAllItemsBtn listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.deselectAllReports();">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_ReportsDeselectAll.png');"></div>
                            </div>
                           
                            <div class="forwardSelectedReports listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.forwardSelected();">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_FwdReport.png');"></div>
                            </div>
                            <div class="deleteSelectedBtn listToolBtn confirmAction" onclick="ROE.UI.Sounds.click(); ROE.Reports.deleteSelected($('.listToolIcon', this));">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_DeleteReport.png');"></div>
                            </div>

                            <div class="nukeReportsBtn listToolBtn confirmAction" onclick="ROE.UI.Sounds.click(); ROE.Reports.nukeAllBtnClick(this);">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/nuke.png');"></div>
                            </div>  
                                           
                        </div>
                    </div>
                     <div id="newReportsIndicator" class="fontFellFCLClg"><%=RS("NewReports") %></div>
                    <div class="empty"><%=RS("NoReports") %></div>
                    <div class="items"></div>
                </div>
                
                <div class="template item %new%" rid="%id%" data-type="%type%">
                    <div class="btnCheckboxToggle medUI sfx2" data-selected="false">
                       <div class="checkbox">
                            <div class="checkmark off"></div>
                       </div>
                      
                    </div>
                    <div class="btnStarToggleWrapper">
                        <div class="btnStarToggle medUI sfx2 %starstate%" data-selected="false"></div>
                    </div> 
                    <div class="hotspot %sound%">
                        <div class="icon reportTypeIcon" style="background-image:url('%iconSrc%');"></div>
                        <div class="extraIconWrapper">
                            <div class="indicator flag2" style="background-image:url('%flag2src%');"></div>
                            <div class="indicator flag3" style="background-image:url('%flag3src%');"></div>
                        </div>
                        <a class="subjectBlock" href="#"><span class="forwarded %forwarddisp%"><%=RS("Forwarded") %></span><span class="subjectLine">%subject%</span></a>                       
                        <span class="time">%time%</span>
                    </div>
                </div>
                
                <div class="detail" style="width: 100%; margin: 0px; overflow: hidden; position: absolute">
                    <div class="detail-margin" style="width: 95%; height: 100%; margin: 8px; overflow: hidden; position: absolute">

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

                                        <div class="arrow-l sfxOpen"></div>
                                    </div>
                                </div>

                                <div class="label sfxOpen">
                                    <span><%=RS("Back") %></span><br>
                                </div>
                            </div>
                        </div>
                    
                        <div class="function">
                            <div class="delete sfx2"><img src="https://static.realmofempires.com/images/icons/M_DeleteReport.png"></div>
                            <div class="forward sfx2"><img src="https://static.realmofempires.com/images/icons/M_FwdReport.png"></div>
                            <div class="forward-search">
                                <input type="text" class="selected"></input>
                                <input type="button" class="send" value="Forward"></input>
                                <div class="result"></div>
                                <div class="message"></div>
                            </div>
                        </div>


                        <div class="info"></div>
                        
                    </div>
                </div>
            </section>
                   
            <section id="reportDetails_section" class="themeM-view reportDetailsView slideRightFrom transition">
                <div class="reportDetailContainer">
                    <span>empty</span>
                </div>
                <div class="reportDetailControlBar">
                    <div class="sectionDivider adjustForControlBar"></div>
                    <div class="bgStrip"></div>
                    <div class="slideBackToReportList BtnDSm2n fontSilverFrSClrg"><span class="smallArrowLeft"></span><%=RS("Back") %></div>

                    <a class="smallRoundButtonDark btnNextReport" onclick="ROE.Reports.showNextReport($(this));">
                        <span class="smallArrowDown"></span>
                    </a>
                    <a class="smallRoundButtonDark btnPrevReport" onclick="ROE.Reports.showPrevReport($(this));">
                        <span class="smallArrowUp"></span>
                    </a>

                    <div class="btnStar"></div>
                    <a class="btnDeleteReport confirmAction"></a>
                    <a class="btnForwardReport" onclick="ROE.UI.Sounds.click(); ROE.Reports.forwardReportPopup();"></a>
                   
                </div>
                <div class="forwardReportContainer">
                    <div class="forward-search">
                        <input type="text" class="selected"></input>
                        <input type="button" class="send" value="Forward"></input>
                        <div class="result"></div>
                        <div class="message"></div>
                    </div>
                </div>
            </section>
        </div>


        <div class="template forwardPopup">
            <input class="forwardTo" type="text" />
            <div class="forwardSendBtn BtnBSm2 fontSilverFrSClrg">Forward</div>            
            <ul class="forwardSearchList"></ul>  
            <div class="loadingSearchList"></div>   
        </div>

        <div id="ReportPhrases" style="display:none;">
            <%= RSdiv("DeleteSuccessful") %>
            <%= RSdiv("ForwardedSuccessful") %>
            <%= RSdiv("PlayerNameNotFound") %>
            <%= RSdiv("PlayerBlocked") %>
        </div>

    </section>
</asp:Content>