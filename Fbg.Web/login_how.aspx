<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login_how.aspx.cs" Inherits="login_how"  MasterPageFile="~/main2.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphMeta" runat="Server">
    <title>Login choice | <%=  RSc("GameName")%></title>
    <meta name="description" content="Choose a method of login to Realm of Empires." />

    <%if (!isMobile) { %>

    <style>
        
        .toolTipBox {
            position: absolute;
            border-left:1px solid rgba(111, 111, 111, 0.6);
            border-right:1px solid rgba(111, 111, 111, 0.6);
            padding: 10px;
            border-radius: 10px;
            background-color: rgba(0, 0, 0, 1);
            font: 15px "IM Fell French Canon", serif;
            color: #E2E2E2;
            max-width: 220px;
            min-width: 70px;
            text-align: center;
            box-shadow:0px 3px 5px rgba(0,0,0,0.6);
            z-index: 9999999999;
        }
        .ttArrow {
            position: absolute;
            bottom: -5px;
            left: 50%;
            width: 10px;
            height: 5px;
            margin-left: -5px;
            border-bottom-left-radius: 5px;
            border-bottom-right-radius: 5px;
            background-color: rgba(0,0,0,1);
        }

    </style>

    <script type="text/javascript">          
        $(document).ready(function () {
            var toolTipTimer = null;
            $(".typeBda").mouseover(function () {
                var element = $(this);
                toolTipTimer = setTimeout(function () { _showTooltip(element); }, 500);
            });
            $(".typeBda").mouseout(function () {
                $('.toolTipBox').stop().fadeOut(200, function () { $(this).remove(); });
                window.clearTimeout(toolTipTimer);
            });

            if (document.referrer.indexOf('kongregate_shell') > -1
                || window.parent.location.pathname.indexOf('kongregate_shell') > -1) {
                document.write('');
                window.location.href = "login_kongregate_failed.aspx";
            }
        });

        function _showTooltip(element) {
            var text = element.attr('data-toolTip');
            var toolTipBox = $('<div>').addClass('toolTipBox').html(text).hide().appendTo('body').position({
                my: "center bottom",
                at: "center top-2",
                of: element,
                collision: "flipfit"
            }).fadeIn();
            toolTipBox.append('<div class="ttArrow"></div>');
        }
    </script>

    <% } %>

    <!-- D2/M Shared Style -->
    <style>

        .introText {
            padding: 0px 40px;
            margin-bottom: 15px;
            text-align: center;
        }

        /* unused version of FB button */
        .approvedFBbutton {
            display: block;
            position: relative;
            width: 286px;
            height: 57px;
            margin: 0 auto;
            margin-bottom: 10px;
            margin-top: 10px;
            text-decoration: none;
            background-image: url(https://fbcdn-dragon-a.akamaihd.net/hphotos-ak-xap1/t39.2178-6/851558_575761415814161_1164808389_n.png);
            background-size: 290px auto;
            background-position: -2px -2px;
            border-radius: 11px;
        }

        .logintype {
            display: block;
            position: relative;
            width: 225px;
            height: 40px;
            margin: 0 auto;
            margin-bottom: 5px;
            text-decoration: none;
            padding-right: 20px;
            /* text-align: right !important; */
            font-size: 15px !important;
        }
            .logintype .icon {
                position: absolute;
                top: 7px;
                right: 18px;
                height: 25px;
                width: 25px;
                background-size: auto 100%;
                border-radius: 5px;
                box-sizing: border-box;
            }

        .typeBda .icon {
            background-image: url(https://static.realmofempires.com/images/misc/tacticaIcon.png);
        }
        .typeFb  .icon {
            background-image: url(https://static.realmofempires.com/images/icons/M_icoFB.png);
        }
        .typeKong  .icon {
            background-image: url(https://static.realmofempires.com/images/icons/M_icoKong.png);
        }
        .typeAg .icon {
            background-image: url(https://static.realmofempires.com/images/icons/M_icoAG.png);
            border: 1px solid #000;
        }


    </style>

</asp:Content>

<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">

    <div id="main">

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

        <div id="content">

            <div class="introText fontSilverFrLCXlrg">
                Welcome.<br /> Login via Realm of Empire's own Tactica account or using your own Facebook, Kongregate or Armor Games account.
            </div>

          
        </div>

    </div>

    <asp:Label ID="lblFriendsFreshedDebugMessage" runat="server" Text=""></asp:Label>

</asp:Content>