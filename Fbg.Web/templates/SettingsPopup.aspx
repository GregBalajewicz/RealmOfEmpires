<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="SettingsPopup.aspx.cs" Inherits="templates_SettingsPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
  
    <%if (isMobile) { %>
    <div id="background">
        <img src="https://static.realmofempires.com/images/misc/Intro.jpg" class="stretch" alt="" />
    </div>
    <%} %>

    <div class="templ_settingpopup">
                
        <section class="themeM-tabPanel noBoxAroundContent clearfix">
            
            <section class="themeM-tabContent">
                <div class="bg"></div>
                <div class="fg settings_container">

                    <div  class="tabContent tabs-out notifyContainer">                        
                        <div id="notification_popup" class="themeM-panel">

                                     <div class="notificationMasterSwitch notifyactive" data-notifyID="0" data-notifyTypeStatus="%notify.masterstateID%" ><%=RS("AllRealm") %> <span> </span> <%=RS("Notifications") %>: <img src="https://static.realmofempires.com/images/icons/M_IcoCancel.png"  ></div>
                                     <img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" >

                                <div class="notifyList" >           
                                    <div class="notifyItem" >
                                        <div class="notify_mainblock"  data-notifyID="%notify.notificationID%" >
                                            <DIV class="notifyspace"></DIV>
                                            <DIV class="%notify.cssMainClass% notifytext" ><b>%notify.name%:</b> <div class="desc"> %notify.desc% </div></DIV>
                                        </div>                   
                                        <div class="notify_block" data-notifyID="%notify.notificationID%"  >
                                                <li  data-notifyTypeID="activeStateID" data-notifyTypeStatus="%notify.activeStateID%" class="notifyactive notifyButton" ><img src='%notify.IMG0%' ></li>
                                                <% if (isMobile) { %> <li  data-notifyTypeID="muteSettingID" data-notifyTypeStatus="%notify.muteSettingID%" class="%notify.cssClass% notifyButton" ><img class="mini" src='%notify.IMG1%' > <%=RS("Muteatnight") %></li>
                                                <li  data-notifyTypeID="soundSettingID" data-notifyTypeStatus="%notify.soundSettingID%" class="%notify.cssClass% notifyButton" ><img class="mini" src='%notify.IMG2%' > <%=RS("Sound") %></li> <%}%>
                                        </div>
                                        <img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png" >
                                    </div>            
                                 </div>   
                                                             
                             <div class="notify_save"><%=RS("Saving") %></div>  
                          </div>
                    </div>

                    <div class="tabContent tabs-in soundContainer">
                        <% if (Device != CONSTS.Device.Amazon){ %>
                            <div class="oneOption option setMusic ButtonTouch sfx2"><span><%=RS("Music") %></span></div>
                        <%} %>

                        <div class="oneOption setSound ButtonTouch"><span><%=RS("Sounds") %></span></div>

                        <% if (isD2){ %> 
                            <div class="oneOption setUIDisplayMode ButtonTouch"><span></span></div>
                            <div class="oneOption toggleMapScrollZoom ButtonTouch"><span></span></div>
                        <%} %>

                        <div class="oneOption restart ButtonTouch"><span><%=RS("Restart") %></span></div>

                        <div class="oneOption selfhelp ButtonTouch">
                            <a onclick="ROE.UI.Sounds.click(); ROE.Frame.popGeneric('Personal Statistics','',840,650); ROE.Frame.showIframeOpenDialog('#genericDialog','AccountInfo/accountInfo.aspx');"><span>Personal Statistics</span></a>
                        </div>     
                        
                        <div class="oneOption realmParams ButtonTouch">
                            <a onclick="ROE.UI.Sounds.click(); ROE.Frame.popGeneric('Realm Parameters','',840,650); ROE.Frame.showIframeOpenDialog('#genericDialog','RealmInfo.aspx');"><span>Realm Parameters</span></a>
                        </div>                

                        <% if (isMobile) { %> 
                            <div class="oneOption support ButtonTouch"><a href="ContactSupport.aspx" onclick="ROE.UI.Sounds.click(); return popupWindowFromLink(this,'Support',false);"><span><%= (FbgUser.VIP_isVIP ? "Priority " : "") +  RS("Support") %></span></a></div>                   
                        <%}else{%>
                            <div class="oneOption support ButtonTouch"><a href="#" onclick="ROE.UI.Sounds.click(); return ROE.Frame.popupContactSupport();"><span><%=(FbgUser.VIP_isVIP ? "Priority " : "") + RS("Support") %></span></a></div>   
                        <%}%>
                        
                        <%if (FbgPlayer.Realm.StewardshipType != Fbg.Bll.Realm.StewardshipTypes.NoStewardShip 
                              && !(FbgPlayer.Realm.AccessDeviceTypeLimitation == Fbg.Bll.Realm.AccessDeviceTypeLimitations.MobDevicesOnly)
                              && !isD2)
                          { %>
                            <div class="oneOption steward ButtonTouch"><a href="AccountStewards.aspx" onclick="ROE.UI.Sounds.click(); return popupWindowFromLink(this,'<%=RS("AssignSteward") %>',false);"><span><%=RS("Steward") %></span></a></div>
                        <%} %> 

                        <% if (isMobile) { %> 
                            <div class="oneOption tou ButtonTouch"><a href="tou.aspx" onclick="ROE.UI.Sounds.click(); return popupWindowFromLink(this,'<%=RS("tou") %>',false);"><span><%=RS("tou") %></span></a></div>                      
                        <%}else{%>
                            <div class="oneOption tou ButtonTouch"><a href="#" onclick="ROE.UI.Sounds.click(); return ROE.Frame.popupTou();"><span><%=RS("tou") %></span></a></div>
                        <%}%>
                        
                        <% if (Device != CONSTS.Device.Amazon){ %> 
                            <div class="oneOption resetLT ButtonTouch"><a href="#" ><span><%=RS("resetlogintype") %></span></a></div>
                        <%} %>

                         <% if (isTactica) { %> 
                            <div class="oneOption changepwd ButtonTouch"><a href="login_changepassword.aspx" onclick="ROE.UI.Sounds.click();"><span><%=RS("changepwd") %></span></a></div>                   
                        <%}%>

                        <% if (isD2){ %> 
                            <div class="oneOption settingsRow ButtonTouch playOnMInfo"  onclick="ROE.UI.Sounds.click(); return ROE.Frame.showIframeOpenDialog('#playOnMDialog', 'playonM.aspx');">
                                <div class="rowIcon playOnMOption" ><div class="innerIcon" data-iconstate="1" ></div></div>
                                <script>
                                    //this code is placed here so if the icon isnt shown then the script wont run niether: performance
                                    $(document).ready(function () {
                                        setInterval(function () {
                                            var mInfoIcon = $('.playOnMInfo .playOnMOption .innerIcon').fadeOut(300, function () {
                                                var nextState = parseInt(mInfoIcon.attr('data-iconState')) + 1;
                                                if (nextState > 3) { nextState = 1; }
                                                mInfoIcon.attr('data-iconState', nextState).fadeIn(300);
                                            });
                                        }, 2500);
                                    });
                                </script>
                                <div class="btnText">Play on Mobile</div>
                            </div>
                        <%}%>

                         <% if (false) { // temp fix - we dont want logout for now
                             //if (!isMobile) { %> 
                            <img class="notifysepartor" src="https://static.realmofempires.com/images/misc/m_listbar.png">
                            <div class="logout ButtonTouch oneOption"><a href="logout.aspx" onclick="ROE.UI.Sounds.click();"><span><%=RS("logout") %></span></a></div>                   
                        <%}%>
                    </div>
                </div>
            </section>

            <section class="themeM-tabs" id="settings_tab">
                <ul>
                    <li><a href="#" tab="tabs-in" class="oneLine  sfx2" data-container="notifyContainer" ><%=RS("Notifications") %></a></li>
                    <li><a href="#" tab="tabs-out" class="oneLine selected sfx2" data-container="soundContainer" ><%=RS("Account") %></a></li>
                </ul>
            </section>

        </section>
        
        <div class="phrases">
            <div ph="0"><%=RS("Note0") %></div>
            <div ph="1"><%=RS("Note1") %></div>
            <div ph="2"><%=RS("Note2") %></div>
            <div ph="3"><%=RS("Note3") %></div>
            <div ph="4"><%=RS("Note4") %></div>
            <div ph="5"><%=RS("Note5") %></div>
            <div ph="6"><%=RS("Note6") %></div>
            <div ph="7"><%=RS("Note7") %></div>
            <div ph="8"><%=RS("Note8") %></div>
            <div ph="9"><%=RS("Note9") %></div>
            <div ph="10"><%=RS("Note10") %></div>
            <div ph="11"><%=RS("ON") %></div>
            <div ph="12"><%=RS("OFF") %></div>
        </div>

    </div>
</asp:Content>