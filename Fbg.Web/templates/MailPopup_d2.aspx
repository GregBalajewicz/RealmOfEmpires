<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="MailPopup_d2.aspx.cs" Inherits="templates_MailPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

   

    
    <div id="mail_popup" class="themeM-tabPanel">

        <div class="bg themeM-tabContent">
            <div class="list section showed">
                <div class="itemsPane">
                   <div class="listTools">
                    <div class="listToolsFilter">
                        <div class="alignLeft">
                            <div class="reload smallRoundButtonDark listToolBtn">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/buttons/M_RefreshSm.png');"></div>
                            </div>
                            <div class="hideDuringFilter">                            
                                <div class="filterStarredOnlyBtn smallRoundButtonDark off listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Mail.filterStarredOnly('.list');">
                                    <!--<div class="listToolIcon cancelIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_IcoCancelb.png'); z-index:2"></div>-->
                                    <div class="listToolIcon starredIcon"></div>
                                </div>

                            </div> 
                            <div class="filterTerm">Filtering by:<br />STARRED (<span id="numMailFound"></span>)</div> 
                            <div class="clearFilterBtn listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Mail.clearStarredFilter();">
                                <img src="https://static.realmofempires.com/images/icons/M_X.png">
                            </div>                             
                        </div>
                        <div class="alignRight">                 
                            <div class="selectAllItemsBtn smallRoundButtonDark listToolBtn"  onclick="ROE.UI.Sounds.click(); ROE.Mail.selectAllItems('.list');">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_MailSelectAll.png');"></div>
                            </div>
                            <div class="deselectAllItemsBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Mail.deselectAllItems('.list');">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_MailDeselectAll.png');"></div>
                            </div>   
                            <div class="deleteSelectedBtn smallRoundButtonDark confirmAction listToolBtn" onclick="ROE.Mail.deleteSelected('.list', $('.listToolIcon', this));">
                                <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_DeleteMail.png');"></div>
                            </div>

                        </div>   
                    </div>
                </div>
                    <div class="items">
                        <div class="more-wrap" style="text-align: -webkit-center"><div class="more sfxScroll" offset="0"></div></div>
                        <div class="empty"><center><%=RS ("NoMail")%></center></div>
                    </div>
                </div>
                <div class="contentPane">
                    <div class="noMsgSelected"><%=RS ("NoMsgSelected")%></div>
                    <div class="detail section">
                
                        <div class="info">

                            <div class="mailheader">
                                <div class="subject" style="margin: 4px; color: #f3d08c; font-size: 12px"></div>
                                <div><%=RS ("From")%> <span class="from"></span> (<a class="doblock sfx2"><%=RS ("Block")%></a>) - <span class="time"></span></div>
                                <div class="to"><%=RS ("To")%> <span class="tos"></span></div>
                            </div>

                            <div class="message"></div>

                        </div>
                
                        <div class="sectionBar" style="bottom:40px;"></div>

                        <div class="function">
                            <div class="next sfx2"></div>
                            <div class="prev sfx2"></div>
                            <div class="reply sfx2"></div>
                            <div class="replyall sfx2"></div>
                            <div class="forward sfx2"></div>
                            <div class="delete confirmAction"><span>&nbsp;</span></div>
                        </div>
                    </div>
                </div>
            </div>
       
            <div class="sentList section">
                
                  <div class="itemsPane">
                   <div class="listTools">
                    <div class="listToolsFilter alignLeft">                            
                        <!--<div class="clearFilterListBtn smallRoundButtonDark listToolBtn sfx2" onclick="ROE.Mail.clearFilterList('.list');">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_IcoCancelb.png');"></div>
                        </div>     -->                       
                        <!-- <div class="filterStarredOnlyBtn off listToolBtn sfx2" onclick="ROE.Mail.filterStarredOnly('.sentList');">
                            <div class="listToolIcon cancelIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_IcoCancelb.png'); z-index:2"></div>
                            <div class="listToolIcon starredIcon"></div>
                        </div>            -->                 
                         <div class="reload smallRoundButtonDark listToolBtn">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/buttons/M_RefreshSm.png');"></div>
                        </div>   
                    </div>
                    <div class="listToolsSelect alignRight">
                                           
                        <div class="selectAllItemsBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Mail.selectAllItems('.sentList');">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_MailSelectAll.png');"></div>
                        </div>
                        <div class="deselectAllItemsBtn smallRoundButtonDark listToolBtn" onclick="ROE.UI.Sounds.click(); ROE.Mail.deselectAllItems('.sentList');">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_MailDeselectAll.png');"></div>
                        </div>   
                        <div class="deleteSelectedBtn smallRoundButtonDark confirmAction listToolBtn" onclick="ROE.Mail.deleteSelected('.sentList', $('.listToolIcon', this));">
                            <div class="listToolIcon" style="background-image:url('https://static.realmofempires.com/images/icons/M_DeleteMail.png');"></div>
                        </div>          
                    </div>
                </div>
                    <div class="items">
                        <div class="more-wrap" style="text-align: -webkit-center"><div class="more sfxScroll" offset="0"></div></div>
                        <div class="empty"><center><%=RS ("NoMail")%></center></div>
                    </div>
                </div>
                <div class="contentPane">
                    <div class="noMsgSelected"><%=RS ("NoMsgSelected")%></div>
                    <div class="detail section">
                
                        <div class="info">

                            <div class="mailheader">
                                <div class="subject" style="margin: 4px; color: #f3d08c; font-size: 12px"></div>
                                <div><%=RS ("From")%> <span class="from"></span> (<a class="doblock sfx2"><%=RS ("Block")%></a>) - <span class="time"></span></div>
                                <div class="to"><%=RS ("To")%> <span class="tos"></span></div>
                            </div>

                            <div class="message"></div>

                        </div>
                
                        <div class="sectionBar" style="bottom:40px;"></div>

                        <div class="function">
                            <div class="next sfx2"></div>
                            <div class="prev sfx2"></div>
                            <div class="reply sfx2"></div>
                            <div class="replyall sfx2"></div>
                            <div class="forward sfx2"></div>
                            <div class="delete confirmAction"><span>&nbsp;</span></div>
                        </div>
                    </div>
                </div>
            
            </div>

            <div class="template item">
                <table class="itemTable">
                    <tr>
                        <td class="touchBtnCol checkboxWrapper">
                            <div class="lt-checkboxToggleBtn medUI" data-selected="false">
                                <div class="checkbox">
                                    <div class="checkmark off"></div>
                                </div>                      
                            </div>
                        </td>
                        <td class="touchBtnCol starWrapper">
                            <div class="lt-starToggleBtn lrgUI" data-selected="false">
                                <div class="lt-starIcon off"></div>
                            </div> 
                        </td>
                        <td class="subjectCol hotspot">
                                                       
                                <div class="nameWrapper">                                    
                                    <div class="name"></div>                                
                                </div>
                                <div class="time"></div>
                            
                 
                        </td>
                        <td class="arrowCol hotspot">
                            <div class="arrowIcon" src="" />
                        </td>
                    </tr>
                </table>
                <!--
                <div class="lt-checkboxToggleBtn lrgUI sfx2" data-selected="false">
                    <div class="checkbox">
                        <div class="checkmark off"></div>
                    </div>                      
                </div>
                <div class="lt-starToggleBtnWrapper">
                    <div class="lt-starToggleBtn lrgUI editable sfx2 off" data-selected="false"></div>
                </div> 
                <div class="hotspot sfxOpen">
                    <img height="44px" style="float: left;" class="icon" />
                    <div class="nameWrapper">
                        <div class="name"></div>

                    </div>
                        <div class="time"></div>

                    
                 </div>-->
            </div>

            <div class="block section" style="display: none">
                <div class="users"></div>
                <div class="empty"><center><%=RS ("NoBlocked")%></center></div>
            </div>

            <div class="template user">
                <div class="name cell"></div>
                <a class="remove cell"><%=RS ("Unblock")%></a>
            </div>
            
            <div class="create1step step section">

                <div style="display: none"><input type="checkbox" class="to-hide"></input><%=RS ("HideRecipients")%></div>             
                <div class="notification"></div>
                
                <div class="mailSection">
                    <div class="label fontSilverFrSClrg"><%=RS ("To")%></div>
                    <input type="text" class="to" placeholder="Recipient Name"></input>
                    <div class="autocomplete"></div>
                </div>

                <div class="mailSection">
                    <div class="label fontSilverFrSClrg"><%=RS ("Subject")%></div>
                    <input type="text" class="subject" placeholder="Please enter subject"></input>
                </div>

                <div class="mailSection">
                    <div class="label fontSilverFrSClrg"><%=RS ("MessageColon")%></div>             
                    <textarea class="message" rows="13" placeholder="Please enter message here"></textarea>
                </div>

                <div class="action-save BtnBSm2 fontSilverFrSClrg"><%=RS ("Send")%></div>

            </div>

            <!--<div class="detail section">
                
                <div class="info">

                    <div class="mailheader">
                        <div class="subject" style="margin: 4px; color: #f3d08c; font-size: 12px"></div>
                        <div><%=RS ("From")%> <span class="from"></span> <a class="doblock sfx2">block</a> - <span class="time"></span></div>
                        <div class="to"><%=RS ("To")%> <span class="tos"></span></div>
                    </div>

                    <div class="message"></div>

                </div>
                
                <div class="sectionBar" style="bottom:40px;"></div>

                <div class="function">
                    <div class="next sfx2"></div>
                    <div class="prev sfx2"></div>
                    <div class="reply sfx2"></div>
                    <div class="replyall sfx2"></div>
                    <div class="forward sfx2"></div>
                    <div class="delete sfx2"></div>
                </div>
            </div>-->
                   
        </div>

        <div class="phrases">
            <%= RSdiv("EnterValidSubject") %>
            <%= RSdiv("PlayersCouldNotBeFound") %>
            <%= RSdiv("UnableToSendMail") %>
            <%= RSdiv("NoPlayers") %>
            <%= RSdiv("NoSubject") %>
            <%= RSdiv("InvalidRecipients") %>
        </div>
    </div>

    <section id="mail_listtabs">
        <!--<div class="mailTabBtn mailTabIcon reload sfx2"></div>-->
        <div class="mailTabBtn BtnDLf1 fontButton1L create sfx2"><span></span><%=RS ("Create")%></div>
        <div class="mailTabBtn BtnDLf1 fontButton1L inbox sfx2 selected"><span></span><%=RS ("Inbox")%></div>
        <div class="mailTabBtn BtnDLf1 fontButton1L outbox sfx2"><span></span><%=RS ("Outbox")%></div>       
        <div class="mailTabBtn BtnDLf1 fontButton1L block sfx2"><span></span><%=RS ("Blocked")%></div>
    </section>
    
</asp:Content>
