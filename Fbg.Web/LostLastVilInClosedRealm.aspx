<%@ Page Language="C#" MasterPageFile="~/master_PopupFullFunct.master" AutoEventWireup="true"
    CodeFile="LostLastVilInClosedRealm.aspx.cs" Inherits="LostLastVilInClosedRealm" %>

<%@ Register Src="Controls/PlayerRanking.ascx" TagName="PlayerRanking" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

<style>

    #background {
        width: 100%;
        height: 100%;
        position: fixed;
        left: 0px;
        top: 0px;
        z-index: -1;
    }
        #background > .stretch {
            width: 100%;
            height: 100%;
        }

    .mobilecontent {
        position: absolute;
        bottom: 0;
        height: 100%;
        width: 100%;
        text-align: center;
    }
    .titletoplogo {
        background: url('https://static.realmofempires.com/images/misc/RegistrationHeader.png') no-repeat center;
        width: 100%;
        height: 57px;
        margin: -112px auto 0 auto;
    }
    .titlenew {
        text-align: center;
        width: 100%;
        height: 69px;
        background: rgba(0,0,0,0.5);
        margin-top: 14px;
        position: fixed;
    }
    .titlelogo {
        position: absolute;
        margin: -85px 3px;
        text-align: left;
        z-index: 1000;
    }
    .titletext {
        font: 26px/1.8em "IM Fell French Canon SC", serif;
        color: white;
        text-shadow: 2px 2px 1px #000000;
        text-align: center;
        padding: 0 0 0 30px;
    }
    .titleseparator1 {
        position: absolute;
        top: -6px;
        height: 18px;
        width: 100%;
        text-align: center;
        background: url('https://static.realmofempires.com/images/misc/m_listbar.png') no-repeat center;
    }
    .titleseparator2 {
        position: relative;
        bottom: -6px;
        height: 18px;
        width: 100%;
        text-align: center;
        background: url('https://static.realmofempires.com/images/misc/m_listbar.png') no-repeat center;
    }
    .titleseparator3 {
        position: relative;
        top: -7px;
        height: 16px;
        width: 100%;
        text-align: center;
        background: url('https://static.realmofempires.com/images/misc/m_listbar.png') no-repeat center;
    }
    .titletext1, .BackButton .label SPAN {
        font: 16px/0.2em "IM Fell French Canon SC", serif;
        color: white;
        text-shadow: 2px 2px 1px #000000;
        text-align: center;
        padding: 0 0 6px 0;
        text-transform: initial;
    }
    #logo {
        width: 80px;
        height: 97px;
    }
    .bottom {
        position: absolute;
        bottom: 0;
        height: 239px;
        width: 100%;
        text-align: center;
        background: rgba(0,0,0,0.5);
    }
    .sleepText2 {
        font: 16px/1.0em "IM Fell French Canon SC", serif;
        color: #FFD776;
        text-shadow: 2px 2px 1px #000000;
        text-align: center;
    }
    .sleepText3 {
        font: 14px/1.1em "IM Fell French Canon LC", serif;
        color: #acacac;
        padding: 0 10px;
        text-shadow: 2px 2px 1px #000000;
        text-align: center;
    }
    .spacerTop {
        width: 100%;
        height: 20px;
        background: url(https://static.realmofempires.com/images/misc/M_SpacerBottom.png) no-repeat center;
    }
    .spacerBottom {
        width: 100%;
        height: 20px;
        background: url(https://static.realmofempires.com/images/misc/M_SpacerTop.png) no-repeat center;
    }
    A:active, A:hover {
        text-decoration: none !important;
    }
    .themeM-more {
        float: left;
    }
        .themeM-more > .fg > .arrow-l {
            left: 2px;
        }
    .themeM-panel.style-link {
        margin: 13px 8px 20px;
    }
    .sidepanel {
        display: none;
        width: 100%;
        position: absolute;
        top: 100px;
        bottom: 0;
        left: 0;
        overflow: hidden;
    }
    .themeM-more > .fg > .arrow-r {
        right: 2px;
    }
    .rankingtable {
        width: 100%;
        height: 100%;
        overflow: auto;
    }
    .bottom .themeM-panel.style-link > .fg > .themeM-more {
        float: left;
    }
    .realmID {
        margin-left: 28px;
    }

</style>


    <%if(!isMobile) {%>
    <style>
        body {
            background-image: url('https://static.realmofempires.com/images/forum/BGcolor6b6670.jpg') !important;
            background-size: cover;
            background-position: 50% 50%;
            background-attachment: fixed;
        }
        .limitor {
            position: absolute;
            top: 50%;
            margin-top: -350px;
            left: 50%;
            margin-left: -250px;
            width: 500px;
            height: 700px;
            overflow: hidden;
            background-image: url(https://static.realmofempires.com/images/backgrounds/M_BG_Defeat.jpg);
            background-size: cover;
            background-position: 50% 50%;
            border-radius: 20px;
        }

        .holderbkg {position: absolute;left: 0px;right: 0px;bottom: 0px;top: 0px;}
        .holderbkg .corner{position:absolute;background-size:100% 100%}
        .holderbkg .corner-top-left{width:35px;height:36px;left:0px;top:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TL.png')}
        .holderbkg .corner-top-right{width:35px;height:36px;right:0px;top:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TR.png')}
        .holderbkg .corner-bottom-left{width:35px;height:46px;left:0px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BL.png')}
        .holderbkg .corner-bottom-right{width:35px;height:46px;right:0px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BR.png')}
        .holderbkg .border{position:absolute;background-size:100% 100%}
        .holderbkg .lb-border-top{top:0px;left:35px;right:35px;height:36px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TC.png')}
        .holderbkg .lb-border-bottom{left:35px;right:35px;height:46px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BC.png')}
        .holderbkg .lb-border-left{left:0px;top:36px;bottom:46px;width:35px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_L.png')}
        .holderbkg .lb-border-right{right:0px;top:36px;bottom:46px;width:35px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_R.png')}


        .titlenew {
            text-align: center;
            width: auto !important;
            height: 69px;
            background: rgba(0, 0, 0, 0.5);
            margin-top: 14px;
            position: absolute;
            left: 10px;
            right: 10px;
        }
        .mobilecontent  {
            position: absolute;
            top: 10px !important;
            bottom: 20px !important;
            left: 10px !important;
            right: 10px !important;
            height: auto !important;
            width: auto !important;
            text-align: center;
        }
        .sidepanel  {
            position: absolute;
            top: 110px !important;
            bottom: 20px !important;
            left: 10px !important;
            right: 10px !important;
            height: auto !important;
            width: auto !important;
            text-align: center;
        }

    </style>
    <%} %>

         <%if(!isMobile) {%>
            <div class="limitor">
                <div class="holderbkg">
			        <div class="corner corner-top-left"></div>
			        <div class="corner corner-top-right"></div>
			        <div class="corner corner-bottom-left"></div>
			        <div class="corner corner-bottom-right"></div>
			        <div class="border lb-border-top"></div>
			        <div class="border lb-border-right"></div>
			        <div class="border lb-border-bottom"></div>
			        <div class="border lb-border-left"></div>
		        </div>
         <%} else{ %>
            <img id="background" src="https://static.realmofempires.com/images/backgrounds/M_BG_Defeat.jpg" class="stretch" alt="" />
        <%} %>
           
        
            
            <DIV class="titlenew">  
                <div class="titleseparator1"></div>                                         
                    <div class="titletext"><%=RS("Defeat") %></div> 
                    <div class="titletext1 realmID"><%=RS("Realm") %> 
                        <asp:Label  ID="lblCurRealm" runat="server" ></asp:Label>
                    </div>
                <div class="titleseparator2"></div>
                <div class="titlelogo"><img id=logo src="<%=RSi("fancyLogo") %>" /></div>
            </DIV>   

             <div class="mobilecontent  slideLeftTo transition">
                                       
                    <div class="bottom">
                        <div class="titleseparator3"></div>
                        
                        <div class="sleepText2" ><%=RS("YouHaveLostYourVillages") %></div>
                        <div class="spacerTop" ></div>
                        <div class="sleepText3" ><%=RS("NewChallengesAwait") %></div>
                        <div class="spacerBottom" ></div>    
                            

                        <a href="ChooseRealm.aspx">
                            <div class="BackButton themeM-panel style-link BarTouch sfx2">
                                <div class="bg">
                                    <div class="corner-br"></div>
                                </div>

                                <div class="fg">
                                    <div class="themeM-more">
                                        <div class="bg">
                                        </div>

                                        <div class="fg">
                                            <div class="label">
                                                <span></span><br />
                                            </div>

                                            <div class="arrow-l"></div>
                                        </div>
                                    </div>

                                    <div class="label">
                                        <span><%=RS("SelectNewRealm") %></span><br />
                                    </div>
                                </div>
                            </div>
                        </a>
                        
                        
                            <div class="ShowRanking BackButton themeM-panel style-link BarTouch sfx2">
                                <div class="bg">
                                    <div class="corner-br"></div>
                                </div>

                                <div class="fg">
                                    <div class="themeM-more">
                                        <div class="bg">
                                        </div>

                                        <div class="fg">
                                            <div class="label">
                                                <span></span><br />
                                            </div>

                                            <div class="arrow-l"></div>
                                        </div>
                                    </div>

                                    <div class="label">
                                        <span><%=RS("CurrentRanking") %></span><br />
                                    </div>
                                </div>
                            </div>
                        
                    </div>
            </div>
            
         <div class="sidepanel  slideLeftFrom transition" >

             <div class="BackButton themeM-panel style-link BarTouch sfx2">
                                <div class="bg">
                                    <div class="corner-br"></div>
                                </div>

                                <div class="fg">
                                    <div class="themeM-more">
                                        <div class="bg">
                                        </div>

                                        <div class="fg">
                                            <div class="label">
                                                <span></span><br />
                                            </div>

                                            <div class="arrow-r"></div>
                                        </div>
                                    </div>

                                    <div class="label">
                                        <span><%=RS("BACK") %></span><br />
                                    </div>
                                </div>
                            </div>
            
             <div class="rankingtable" >

    <div style="clear: both;background:rgba(0,0,0,0.5);">
        <center>
            <span style="font-size: 1.3em"><%=RS("CurrentRanking") %></span>
            <uc1:PlayerRanking ID="PlayerRanking1" runat="server" />
        </center>
    </div>


        </div>
    </div>

     <%if(!isMobile) {%></div><%} %>

      <script>   
          $(".mobilecontent .ShowRanking").click(function () {

            $(".sidepanel").show();

            BDA.UI.Transition.slideRight($(".sidepanel"), $(".mobilecontent"), function () { $(".mobilecontent").hide(); });
        });

          $(".sidepanel .BackButton").click(function () {

            $(".mobilecontent").show();
            BDA.UI.Transition.slideLeft($(".mobilecontent"), $(".sidepanel"), function () { $(".sidepanel").hide(); });
        });
    </script>
    
      <%
          int realmId = Convert.ToInt32(Request.QueryString[global::CONSTS.QuerryString.RealmID]);

          switch (realmId)
          {
              case 75:
                  %><script>(function (t, e, o, n) { var s, c, a; t.SMCX = t.SMCX || [], e.getElementById(n) || (s = e.getElementsByTagName(o), c = s[s.length - 1], a = e.createElement(o), a.type = "text/javascript", a.async = !0, a.id = n, a.src = ["https:" === location.protocol ? "https://" : "http://", "widget.surveymonkey.com/collect/website/js/p9RORfytjRRvaADWHCygRQQSN4bHkMuCwUyZiJKjeGT_2BWlBaWW8oLABTU5tHhuHY.js"].join(""), c.parentNode.insertBefore(a, c)) })(window, document, "script", "smcx-sdk");</script><%
                  break;
              case 72:
                  %><script>(function (t, e, o, s) { var n, c, r; t.SMCX = t.SMCX || [], e.getElementById(s) || (n = e.getElementsByTagName(o), c = n[n.length - 1], r = e.createElement(o), r.type = "text/javascript", r.async = !0, r.id = s, r.src = ["https:" === location.protocol ? "https://" : "http://", "widget.surveymonkey.com/collect/website/js/YhNwNAS6gYC2s9tFevtGSFfLHfrKaGERr0LUHXO38q7o5gt1UJrv4jxx5IvyXJFo.js"].join(""), c.parentNode.insertBefore(r, c)) })(window, document, "script", "smcx-sdk");</script><%
                  break;
              case 71:
                  %><script>(function (e, t, s, c) { var n, o, i; e.SMCX = e.SMCX || [], t.getElementById(c) || (n = t.getElementsByTagName(s), o = n[n.length - 1], i = t.createElement(s), i.type = "text/javascript", i.async = !0, i.id = c, i.src = ["https:" === location.protocol ? "https://" : "http://", "widget.surveymonkey.com/collect/website/js/xwd1RIre0zVG7fvrhOXxcCMh4_2BuLNtxekwAf_2B_2Fkiiwszj4RkREYyl2OV7zO224BK.js"].join(""), o.parentNode.insertBefore(i, o)) })(window, document, "script", "smcx-sdk");</script><%
                  break;
              default:
                  break;
          }
          
           %>



</asp:Content>
