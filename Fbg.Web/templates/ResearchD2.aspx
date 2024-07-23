<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="ResearchD2.aspx.cs" Inherits="templates_Research" %>

<asp:Content ContentPlaceHolderID="cph1" runat="Server">
    <section class="themeM-page d2template" id="research" >
        <!--<div class="bg"></div>-->

        <!-- Research Members Section -->
        <section class="themeM-panel header">

                <div class="fg">
                    
                    <div class="researchMemberBusy">
                        <span>0</span> <%=RS("Researching") %><br/>
                    </div>
                    <div class="researchMemberFree">
                        <span>0</span>  <%=RS("Idle") %><br/>
                    </div>
                    <div class="researchMemberNone">
                        <span>0</span> <%=RS("VacantPositions") %><br/>
                    </div>

                    <div class="memberBar"></div>

                    <ul class="list-y researchMember" data-type="researchMember">
                        <li class="template" data-id="">

                            <div class="busy">                
                                <div class="status fontSilverFrLCmed">
                                    <div class="icon"></div>
                                    <div class="anim"></div>
                                    <div class="time">
                                        <span></span><br/>
                                    </div>
                                    <div class="name">
                                        <span></span><br/>
                                    </div>
                                    <div class="bonus"></div>
                                </div>
                                <div class="avatar"></div>
                            </div>

                            <div class="free">
                                <div class="status fontSilverFrLCmed">
                                    <div class="idle">
                                        <%=RS("Idle")%><br/>
                                    </div>
                                </div>
                                <div class="avatar"></div>
                            </div>

                            <div class="none">     
                                <div class="status fontSilverFrLCmed">
                                    <div class="idle">Vacant</div>
                                    <a class="hire" href="#">
                                        <div class="buttonL"></div>
                                        <div class="buttonC">Hire</div>
                                        <div class="buttonR"></div>
                                    </a>
                                </div>
                                <div class="avatar"></div>
                            </div>

                            <div class="memberDivider"></div>
                        </li>
                    </ul>

                </div>
            </section>


            <section class="themeM-panel research">

                    <section class="themeM-view master">
                        <div class="scroll">
                            <div class="scroll-content">
                                <div class="infoLabel">
                                    This is the list of research fields. Each field includes multiple technologies that can be researched.
                                </div>
                                <ul class="list-y researchType" data-type="researchType">
                                    <li class="template" data-id="">
                                        <a href="#">
                                            <div class="themeM-progress">
                                                <div class="bg">
                                                    <div class="progress-container">
                                                        <div class="progress-indicator" style="width: 0%;"></div>
                                                    </div>
                                                </div>
                                                <div class="fg"></div>
                                                <div class="curBonus fontSilverNumbersLrg"></div>
                                            </div>
                                            <div class="icon themeM-icon scale-large">
                                                <img src="" alt=""/><br />
                                            </div>
                                            <div class="more some">
                                                <span>0</span> <%=RS("Available")%><br/>
                                            </div>
                                            <div class="more none">
                                                <%=RS("AllCompleted")%><br/>
                                            </div>
                                            <div class="name">
                                                <span></span><br/>
                                            </div>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </section>
                
                    <section class="themeM-view detail">
                        <div id="mainpane">
			            </div>
                        <div class="strandInfo">
                            <a href="#" class="home fontGoldFrLClrg">Back</a>
                            <div class="strandFlurish"></div>
                            <div class="strandIcon"></div>
                            <div class="strandText"></div>
                        </div>
                    </section>

   
            </section>

    </section>

    <div id="nodeContentDivTemplate" class="nodeContentDiv">
        <div class="cOverlay"></div>
        <span class="cStatus"></span>
        <span class="cName"></span>
        <span class="cBonus"></span>
        <span class="cTime"></span>
    </div>
</asp:Content>
