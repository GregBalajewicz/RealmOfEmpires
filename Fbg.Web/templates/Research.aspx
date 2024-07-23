<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="Research.aspx.cs" Inherits="templates_Research" %>

<asp:Content ContentPlaceHolderID="cph1" runat="Server">
    <section class="themeM-page" id="research">
        <div class="bg">
        </div>

        <div class="fg">
            <section class="themeM-panel header">
                <div class="bg">
                    <div class="corner-tl"></div>
                    <div class="corner-br"></div>
                </div>

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

                    <a href="#" class="handle closed">
                        <div class="innerIcon">
                        </div>
                    </a>

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
                <div class="fg">

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
                                                <div class="fg">
                                                </div>
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
                
                    <section class="themeM-view detail slideLeftTo">
                        <div id="mainpane">
			            </div>
                        <div class="strandInfo">
                            <a href="#" class="home"></a>
                            <div class="strandFlurish"></div>
                            <div class="strandIcon"></div>
                            <div class="strandText"></div>
                        </div>
                    </section>

                </div>
            </section>
        </div>
    </section>
    <div id="nodeContentDivTemplate" class="nodeContentDiv">
        <div class="cOverlay"></div>
        <span class="cStatus"></span>
        <span class="cName"></span>
        <span class="cBonus"></span>
        <span class="cTime"></span>
    </div>
</asp:Content>
