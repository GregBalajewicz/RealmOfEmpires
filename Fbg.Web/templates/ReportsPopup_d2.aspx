<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="ReportsPopup_d2.aspx.cs" Inherits="templates_ReportsPopup_d2" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
   <div id="background" style="display:none;">
        <img src="https://static.realmofempires.com/images/misc/SplashScreenMuted.jpg" class="stretch" alt="" />
    </div>
    
    <section id="reports_popup" class="themeM-page hideMe">
        <div class="fg main-fg">
            <section class="themeM-view default transition">
                 <div class="listTools">
                    <div class="listToolsFilter filterReports">
                        <div class="reload smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showLatestReports();">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/buttons/M_RefreshSm.png');"></div>
                        </div>  
                        <div class="hiddenDuringFilter">
                                                           
                            <input type="text" id="reportFilterInput" class="filterInput" value="Search" onfocus="if (this.value == 'Search') {this.value = '';}" onblur="if (this.value == '') {this.value = 'Search';}" type="text" />
                            <div class="filterListBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.filterReports();">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/magnifyingGlass.png');"></div>
                            </div>                               
                                                 
                            <div class="filterStarredOnlyBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showStarredOnly();">
                                <div class="listToolIcon starredIcon"></div>
                            </div>  

                            <div class="filterGovAttacksOnlyBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showGovAttacksOnly();">
                                <div class="listToolIcon govIcon"></div>
                            </div>  

                            <div class="filterSpyOnlyBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showSpyOnly();">
                                <div class="listToolIcon spyIcon"></div>
                            </div>  

                            <div class="filterSuccessOnlyBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showSuccessReportsOnly();">
                                <div class="listToolIcon successIcon"></div>
                            </div> 

                            <div class="filterDefeatOnlyBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.showDefeatReportsOnly();">
                                <div class="listToolIcon defeatIcon"></div>
                            </div> 
                        </div>
                        <!--<div class="clearFilterBtn smallRoundButtonDark listToolBtn sfx2" onclick="ROE.Reports.clearReportsFilter();">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_IcoCancelb.png');"></div>
                                
                        </div> -->
                            <div class="filterTerm">Filtering by:<br /><span id="filterApplied"></span> (<span id="numReportsFound"></span>)</div> 
                            <div class="clearFilterBtn listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.clearReportsFilter();">

                            <img src="https://static.realmofempires.com/images/icons/M_X.png">
                                </div>
                           
                            
                           
                    </div>
                    <div class="listToolsSelect">                                                      
                                                    
                        <div class="selectAllItemsBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.selectAllReports();">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_ReportsSelectAll.png');"></div>
                        </div>

                        <div class="deselectAllItemsBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Reports.deselectAllReports();">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_ReportsDeselectAll.png');"></div>
                        </div>
                           
                        <div class="forwardSelectedReports smallRoundButtonDark listToolBtn" onclick="ROE.Reports.forwardSelected();">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_FwdReport.png');"></div>
                        </div>

                        <div class="deleteSelectedBtn smallRoundButtonDark listToolBtn confirmAction" onclick="ROE.Reports.deleteSelected($('.listToolIcon', this));">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_DeleteReport.png');"></div>
                        </div>

                        <div class="nukeReportsBtn smallRoundButtonDark listToolBtn confirmAction helpTooltip" data-tooltipid="nukeAllReports" onclick="ROE.Reports.nukeAllBtnClick(this);">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/nuke.png');"></div>
                        </div>   
                                      
                    </div>
                </div>
                <div id="newReportsIndicator" onclick="javascript:$(this).removeClass('newToast')" class="fontFellFCLClg">New Reports Available</div>
                <div class="list">
                    
                    <!--<div class="filterReports">
                        <input type="text" id="reportFilterInput" />
                        <div class="runFilterBtn" onclick="ROE.Reports.filterReports();">Filter</div>                        
                        <div class="clearFilterBtn" onclick="ROE.Reports.clearReportsFilter();">Clear &ldquo;<span id="filterApplied"></span>&rdquo; (<span id="numReportsFound"></span>)</div>
                    </div>
                    <div class="reportModifiers">
                        <div class="deleteSelectedReports" onclick="ROE.Reports.deleteSelected();">&#8998; &#9745;</div>
                        <div class="forwardSelectedReports" onclick="ROE.Reports.forwardSelected();">&rarr; &#9745;</div>
                        <div class="selectAllReports" onclick="ROE.Reports.selectAllReports();">&#9745; All</div>
                        <div class="deselectAllReports" onclick="ROE.Reports.deselectAllReports();">&#9744; All</div>
                        <div class="showStarred" onclick="ROE.Reports.showStarredOnly();">Show &#9733;</div>
                    </div>-->

                    
                    
                    <!--<div class="reload RowTouch sfx2"><img src="https://static.realmofempires.com/images/icons/M_RefreshSm.png" onclick="ROE.Reports.showLatestReports();"></img></div>-->
                    <div class="loading" style="display: none"><%=RS("Loading") %></div>

                    <!--<center style="clear: both; margin-top: 28px">
                        <img src="https://static.realmofempires.com/images/misc/M_SpacerBottom.png"></img>
                    </center>-->


                    <table class="itemListTable items">
                        <thead>
                            <tr class="TableHeaderRow">
                                <th width="20px"></th>
                                <th width="20px"></th>
                                <th width="20px"></th>
                                <th width="20px"></th>
                                <th width="20px"></th>
                                <th>Subject</th>
                                <th>Time</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="template item %new% RowTouch" rid="%id%" data-type="%type%">
                                <td>                                   
                                    <div class="btnCheckboxToggle medUI" data-selected="false">
                                        <div class="checkmark off"></div>
                                    </div>                                    
                                </td>
                                <td><div class="btnStarToggle medUI %starstate%" data-selected="false"></div></td>
                                <td><div class="outcomeIcon icon" style="background-image:url('%iconSrc%');"></div><span class="outcomeDescription">%type%</span></td>
                                <td><div class="indicator flag2" style="background-image:url('%flag2src%');"></div></td>
                                <td><div class="indicator flag3" style="background-image:url('%flag3src%');"></div></td>
                                <td class="subjectColumn hotspot %sound%"><span class="forwarded %forwarddisp%"><%=RS("Forwarded") %></span><a class="subjectLine">%subject%</a></td>
                                <td class="timeColumn"><span class="time">%time%</span></td>
                            </tr>
                            <tr class="more-wrap">
                                <td colspan="7"><div class="more sfxScroll" offset="0"></div></td>
                            </tr>
                            <tr class="empty">
                                <td colspan="7"><center><%=RS("NoReports") %></center></td>
                            </tr>
                        </tbody>
                    </table>                   
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

                    <a class="smallRoundButtonDark btnNextReport" onclick="ROE.UI.Sounds.click(); ROE.Reports.showNextReport($(this));">
                        <span class="smallArrowDown"></span>
                    </a>
                    <a class="smallRoundButtonDark btnPrevReport" onclick="ROE.UI.Sounds.click(); ROE.Reports.showPrevReport($(this));">
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