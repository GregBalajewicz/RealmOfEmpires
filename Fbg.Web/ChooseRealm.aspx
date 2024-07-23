<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChooseRealm.aspx.cs" Inherits="ChooseRealm" MasterPageFile="~/main2.master" %>

<%@ Register Src="Controls/ListOfRealmsCompact.ascx" TagName="ListOfRealmsCompact" TagPrefix="uc1" %>
<%@ Register Src="Controls/ListOfRealms.ascx" TagName="ListOfRealms" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphMeta" runat="Server">

    <title>Choose Realm | <%=  RSc("GameName")%></title>
    <meta name="description" content="Welcome back to Realm of Empires." />

    <!-- D2 style -->
    <%if (!isMobile) { %>

     <style>
        
         .realmButtons {
            position: relative;
         }

         .realmButtons a {
            display: block;
            height: auto;
            font: 24px/1.0em "IM Fell French Canon SC", serif;
            text-shadow: 2px 2px 2px #000;
            text-align: center;
            padding-left: 10px;
            margin-bottom: 15px;
            text-decoration: none;
            border: 1px solid rgba(255, 255, 255, 0);
        }

             .realmButtons a:hover {
                 border: 1px solid rgba(255, 255, 255, 0.1);
                 box-shadow: 0px 0px 11px rgba(25, 31, 37, 0.5);
                 background: -moz-linear-gradient(left, rgba(30,87,153,0) 0%, rgba(0,0,0,1) 52%, rgba(125,185,232,0) 100%);
                 background: -webkit-gradient(linear, left top, right top, color-stop(0%,rgba(30,87,153,0)), color-stop(52%,rgba(0,0,0,1)), color-stop(100%,rgba(125,185,232,0)));
                 background: -webkit-linear-gradient(left, rgba(30,87,153,0) 0%,rgba(0,0,0,1) 52%,rgba(125,185,232,0) 100%);
                 background: -o-linear-gradient(left, rgba(30,87,153,0) 0%,rgba(0,0,0,1) 52%,rgba(125,185,232,0) 100%);
                 background: -ms-linear-gradient(left, rgba(30,87,153,0) 0%,rgba(0,0,0,1) 52%,rgba(125,185,232,0) 100%);
                 background: linear-gradient(to right, rgba(30,87,153,0) 0%,rgba(0,0,0,1) 52%,rgba(125,185,232,0) 100%);
                 filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#001e5799', endColorstr='#007db9e8',GradientType=1 );
             }

             .realmButtons .realm {
                 position: relative;
                 float: left;
                 top: 13px;
             }

         .tblRealms .realmNumber {
             position: relative;
             top: 5px;
             float: left;
             margin-left: 10px;
             width: 46px;
             height: 28px;
             background: url('https://static.realmofempires.com/images/icons/M_RealmNumber.png') no-repeat;
             text-align: center;
             font: 24px/1.0em "Georgia", serif;
             color: #FFD776;
             text-shadow: 1px 1px 1px #000000;
             padding: 9px 0;
         }
         .tblRealms .vipexclusive .realmNumber {
             box-shadow: 0px 0px 2px 1px rgba(0, 176, 255, 0.8);
             background-image: url('https://static.realmofempires.com/images/icons/M_RealmNumberVipExclusive.png') !important;
             border-radius: 50px;
         }
         .realmButtons a.vipexclusive {
             background: linear-gradient(to right, rgba(30,87,153,0) 0%,rgba(0, 173, 255, 0.4) 50%,rgba(125,185,232,0) 100%);
         }


          .tblRealms .realmInfo {
            display: inline-block;
            position: relative;
     
          }
          .tblRealms .realmLogin {
            font: 19px "IM Fell French Canon SC", serif;        
          }
          .tblRealms .realmName {
            font: 19px  "IM Fell French Canon SC", serif;
            color: #d0d0d0;      
            text-overflow: ellipsis;
            overflow: hidden;
            white-space: nowrap;
            width: 280px;   
          }
         .tblRealms .realmDetail{
            font: 13px "IM Fell French Canon SC", serif;
            color: #B7B7B7;
         }


         .newtr {
             position: absolute;
             top: 2%;
             left: 50%;
             margin-left: -250px;
             height: 100px;
             width: 500px;
             text-align: center;
             box-sizing: border-box;
             padding-top: 9px;
             cursor: pointer;
             overflow: hidden;
             font: 15px "IM Fell French Canon SC", serif;
             color: #FFD886;
             text-decoration: none !important;
             z-index: 99999;
             background-color: rgba(0,0,0,.7);
             border-bottom: 1px solid #c2a969;
             border-radius: 5px;
         }
             .newtr:hover {
                 text-shadow: 0px 0px 5px #FFF;
             }



    .endedToggle {
        position: absolute;
        top: 7px;
        right: 13px;
        width: 33px;
        height: 33px;
        background-size: 100% 100%;
        cursor: pointer;
        z-index: 2;
    }
        .endedToggle.showing {
            background-image: url(https://static.realmofempires.com/images/icons/M_FilterShowHidden.png);
        }
        .endedToggle.hiding {
            background-image: url(https://static.realmofempires.com/images/icons/M_FilterHideHidden.png);
                  
        }
             

    </style>

    <!-- M style -->
    <%} else {%>

      <style>



          /* TEMPORARY OVER RIDE OF MASTER PAGE STYLE */
          /* to allow for TRY TR critical notice */
          #main {
              top: 160px !important;
          }
        /* TEMPORARY OVER RIDE OF MASTER PAGE STYLE */



          .tblRealms {
              position: relative;
              height: 100%;
              margin-bottom: 5px;
          }

          .realmButtons {
              position: relative;
              width: 100%;
              height: 98%;
              overflow: auto;
          }

              .realmButtons a {
                  position: relative;
                  display: block;
                  background: rgba(0, 0, 0, 0.5);
                  height: auto;
                  font: 17px/1.0em "IM Fell French Canon SC", serif;
                  text-shadow: 2px 2px 2px #000;
                  text-align: left;
                  padding-top: 3px;
                  padding-bottom: 3px;
                  padding-left: 5px;
                  margin-bottom: 5px;
                  overflow: hidden;
              }
              .realmButtons a:hover {
                  text-decoration: none;
              }

              .realmButtons a.pressed, .realmButtons a:active {
                  -webkit-box-shadow: inset 0 0 15px #ffffff;
                  -moz-box-shadow: inset 0 0 15px #ffffff;
                  box-shadow: inset 0 0 15px #ffffff;
              }

          .tblRealms .realmNumber {
              position: relative;
              float: left;
              margin: 0px 3px 0px 2px;
              width: 46px;
              height: 28px;
              background: url('https://static.realmofempires.com/images/icons/M_RealmNumber.png') no-repeat;
              text-align: center;
              font: 24px/1.0em "Georgia", serif;
              color: #FFD776;
              text-shadow: 1px 1px 1px #000;
              padding: 9px 0;
          }

          .tblRealms .vipexclusive .realmNumber {
              box-shadow: 0px 0px 2px 1px rgba(0, 176, 255, 0.8);
              background-image: url('https://static.realmofempires.com/images/icons/M_RealmNumberVipExclusive.png') !important;
              border-radius: 50px;
          }
          .realmButtons a.vipexclusive {
              background: linear-gradient(to right, rgba(30,87,153,0) 0%,rgba(0, 173, 255, 0.4) 50%,rgba(125,185,232,0) 100%);
          }

          .tblRealms .realm {
            position: relative;
            float: left;
            height: 25px;
            margin-top: 15px;
          }
          .tblRealms .realmInfo {
              display: inline-block;
              position: absolute;
              right: 0px;
              left: 102px;
          }
          .tblRealms .realmLogin {
              font: 17px "IM Fell French Canon SC", serif;  
          }
          .tblRealms .realmName {
            font: 15px/1.0em "IM Fell French Canon SC", serif;
            color: #d0d0d0;      
            text-overflow: ellipsis;
            overflow: hidden;
            white-space: nowrap;
            width: 100%;
          }
          .tblRealms .realmDetail {
              font: 11px "IM Fell French Canon SC", serif;
              color: #B7B7B7;
              overflow: hidden;
              white-space: nowrap;
              width: 100%;
          }


          .newtr {
              display: block;
              position: absolute;
              top: 0px;
              left: 0px;
              right: 0px;
              text-align: center;
              overflow: hidden;
              font: 15px "IM Fell French Canon SC", serif;
              color: #d7d7d7;
              text-decoration: none !important;
              padding: 10px;
              line-height: 14px;
              z-index: 999999;
              background-color: rgba(0,0,0,.7);
              border-bottom: 1px solid #9c8865;
          }

          .endedToggle {
              position: absolute;
              top: -35px;
              right: 10px;
              width: 33px;
              height: 33px;
              background-size: 100% 100%;
              cursor: pointer;
              z-index: 2;
          }
              .endedToggle.showing {
                  background-image: url(https://static.realmofempires.com/images/icons/M_FilterShowHidden.png);
              }
              .endedToggle.hiding {
                  background-image: url(https://static.realmofempires.com/images/icons/M_FilterHideHidden.png);
                  
              }

      </style>

    
    <script type="text/javascript">

        $(function () {
            $('.realmButtons a').bind("touchstart",
                function (e) {
                    $(e.target).addClass('pressed');
                }
            );

            $('.realmButtons a').bind("touchend",
                function (e) {
                    $(e.target).removeClass('pressed');
                }
            );
            $('.realmButtons a').bind("touchcancel",
                function (e) {
                    $(e.target).removeClass('pressed');
                }
            );
            $('.realmButtons a').bind("touchmove",
                function (e) {
                    $(e.target).removeClass('pressed');
                }
            );

        });

    </script>

    <%} %>


    <script type="text/javascript">
        $(document).ready(function () {

            var realmEndedDisplay = localStorage.getItem('realmEndedDisplay');
            if (realmEndedDisplay && realmEndedDisplay == "hiding") {
                _hideEndedRealms();
            } else {
                _showEndedRealms();
            }

            if ($('.realmEnded').length) {
                $('.endedToggle').show().click(function () {
                    if ($('.endedToggle').hasClass('showing')) {
                        _hideEndedRealms();
                    } else {
                        _showEndedRealms();
                    }
                });
            } else {
                $('.endedToggle').hide();
            }



        });

        function _hideEndedRealms() {
            $('.endedToggle').removeClass('showing').addClass('hiding');
            $('.realmEnded').hide();
            localStorage.setItem('realmEndedDisplay', "hiding");
        }

        function _showEndedRealms() {
            $('.endedToggle').removeClass('hiding').addClass('showing');
            $('.realmEnded').show();
            localStorage.setItem('realmEndedDisplay', "showing");
        }


    </script>

    
    <%if (true) { %>

        <style>
            #killAnim {
                display:none;
                position: absolute;
                top: 2px;
                right: 2px;
                z-index: 9999999;
                font-size: 10px;
                width: 80px;
                padding: 3px;
                border: 1px solid #C0CCDC;
                text-align: center;
                background: rgba(145, 150, 181, 0.4);
                cursor: pointer;
                border-radius: 3px;
            }
        </style>

        <script>
            $(document).ready(function () {
                if (!(localStorage.getItem('animation_engineState') === "OFF")) {
                    $('<div id="killAnim">').html('click to disable map animations').click(function () {
                        localStorage.setItem('animation_engineState', 'OFF');
                        $(this).remove();
                    }).appendTo('body');
                }
            });
        </script>

    <%} %>


</asp:Content>


<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">

    <div id="main" >
        <div class="roeLogo"></div>
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

        <asp:Panel runat=server ID="panelRealms" CssClass="tblRealms">
            <uc1:ListOfRealms ID="ListOfRealms1"  Theme=ThemeM runat="server" />       
        </asp:Panel>
        
        <div class="endedToggle showing"></div>
    </div>


    <a runat="server" id="trytr" href="" class="newtr" visible="false">Critical Notice:
        <BR />Throne Room is becoming the default entry page in less than <%=new DateTime(2017,3,14,11,00,00).Subtract(DateTime.Now).TotalDays.ToString("#.#") %> days. 
        <br />Please click here now to try Throne Room and report any issues to support before the deadline of <%=new DateTime(2017,3,14,11,00,00).ToUniversalTime().ToString("MMM dd hh:mm") %>
    </a>

    

    <asp:Label ID="lblFriendsFreshedDebugMessage" runat="server" Text=""></asp:Label>

</asp:Content>
