<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login_register_verifyemail.aspx.cs" Inherits="login_register_verifyemail" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">

    <title><%=RSc("GameName") %></title>
    <meta charset="utf-8">

    <%if (Device == CONSTS.Device.iOS)
      { %>
    <meta name="viewport" content="width=320,inital-scale=1.0,maximum-scale=5.0,user-scalable=0">
    <%}
      else
      { %>
    <meta name="viewport" content="width=321" />
    <%} %>

    <link href="https://fonts.googleapis.com/css?family=IM+Fell+French+Canon+SC|IM+Fell+French+Canon+LC" rel="stylesheet" type="text/css" />
    <link href="main.2.css" rel="stylesheet" type="text/css" />
    <script src="<%= Config.InDev ? "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.js" : "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js" %>" type="text/javascript"></script>



    <%if (!isMobile)
      { %>

    <style>
        * {
        outline: none;
        }
        html {
            position: fixed;
            width: 100%;
            height: 100%;
            overflow: hidden;
        }

        body {
            background-image: url(https://static.realmofempires.com/images/backgrounds/BGIntro.jpg);
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            background-attachment: fixed;
            height: 100%;
            width: 100%;
            margin: 0px;
            overflow-x: hidden;
            overflow-y: auto;
        }

        #knightsL {
            position: fixed;
            pointer-events: none;
            left: -295px;
            top: 0px;
            width: 100%;
            height: 100%;
            background-image: url(https://static.realmofempires.com/images/backgrounds/Warlords_Left.png);
            background-size: auto 100%;
            background-position: 50% 0px;
            background-repeat: no-repeat;
        }

        #knightsR {
            position: fixed;
            pointer-events: none;
            left: 315px;
            top: 0px;
            width: 100%;
            height: 100%;
            background-image: url(https://static.realmofempires.com/images/backgrounds/Warlords_Right.png);
            background-size: auto 100%;
            background-position: 50% 0px;
            background-repeat: no-repeat;
        }

        #main {
            position: relative;
            width: 450px;
            top: 28%;
            left: 50%;
            margin-left: -225px;
            background-color: rgba(0, 0, 0, 0.8);
            border-radius: 20px;
            padding: 20px;
            padding-top: 60px;
        }

        .roeLogo {
            position: absolute;
            left: 0px;
            right: 0px;
            top: -145px;
            z-index: 1;
            margin: 0 auto;
            height: 196px;
            background-image: url(https://static.realmofempires.com/images/d2test/HeaderWarlordsRising.png);
            background-position: center;
            background-repeat: no-repeat;
        }

        .holderbkg {
            position: absolute;
            left: 0px;
            right: 0px;
            bottom: 0px;
            top: 0px;
        }

            .holderbkg .corner {
                position: absolute;
                background-size: 100% 100%;
            }

            .holderbkg .corner-top-left {
                width: 35px;
                height: 36px;
                left: 0px;
                top: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_TL.png');
            }

            .holderbkg .corner-top-right {
                width: 35px;
                height: 36px;
                right: 0px;
                top: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_TR.png');
            }

            .holderbkg .corner-bottom-left {
                width: 35px;
                height: 46px;
                left: 0px;
                bottom: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_BL.png');
            }

            .holderbkg .corner-bottom-right {
                width: 35px;
                height: 46px;
                right: 0px;
                bottom: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_BR.png');
            }

            .holderbkg .border {
                position: absolute;
                background-size: 100% 100%;
            }

            .holderbkg .lb-border-top {
                top: 0px;
                left: 35px;
                right: 35px;
                height: 36px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_TC.png');
            }

            .holderbkg .lb-border-bottom {
                left: 35px;
                right: 35px;
                height: 46px;
                bottom: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_BC.png');
            }

            .holderbkg .lb-border-left {
                left: 0px;
                top: 36px;
                bottom: 46px;
                width: 35px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_L.png');
            }

            .holderbkg .lb-border-right {
                right: 0px;
                top: 36px;
                bottom: 46px;
                width: 35px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_R.png');
            }


        #main #content {
            position: relative;
            width: 100%;
            height: auto;
            padding: 5px 0px;
            font: 13px "IM Fell French Canon";
            text-shadow: 0px 1px #000, 0px -1px 1px #000;
        }

        #LoginView1_CreateUserWizard1 {
            margin: 0 auto;
            width: 300px;
        }

        .introText {
            font-size: 20px;
            padding-bottom: 13px;
        }

        input[type="text"], input[type="password"] {
            font-size: 12px;
            width: 170px;
            padding: 5px 5px;
            border-radius: 5px;
            box-shadow: inset -2px 2px 5px rgba(0, 0, 0, 0.4);
            border: none;
        }
        input[type="submit"] {
            position: relative;
            height: 37px;
            width: 135px;
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
        }

        .validationError {
            display: block;
            text-align: center !important;
            color: #FF2121;
        }

        #gotobattle {
            position: relative;
            height: 37px;
            width: 135px;
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
            display: block;
            padding-top: 11px;
        }


    </style>

    <%}
      else
      {%>

    <style>
        html {
            position: absolute;
            width: 100%;
            height: 100%;
            overflow: hidden;
        }

        body {
            position: relative;
            /*background-image: url(https://static.realmofempires.com/images/backgrounds/BGIntro.jpg);*/
            background-image: url(https://static.realmofempires.com/images/misc/M_BG_VillageList.jpg);
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            height: 100%;
            width: 100%;
            margin: 0px;
            overflow: hidden;
        }

        #knightsL, #knightsR {
            display: none;
        }

        #main {
            position: absolute;
            width: 100%;
            top: 90px;
            bottom: 0px;
            background-color: rgba(0, 0, 0, 0.7);
            border-radius: 20px;
            padding: 15px 8px;
            box-sizing: border-box;
        }

        .roeLogo {
            position: absolute;
            left: 0px;
            right: 0px;
            top: -84px;
            z-index: 1;
            margin: 0 auto;
            height: 100px;
            background-image: url(https://static.realmofempires.com/images/forum/NavLogo.png);
            background-position: center 0px;
            background-size: auto 100%;
            background-repeat: no-repeat;
        }

        .holderbkg {
            position: absolute;
            left: 0px;
            right: 0px;
            bottom: 0px;
            top: 0px;
        }

            .holderbkg .corner {
                position: absolute;
                background-size: 100% 100%;
            }

            .holderbkg .corner-top-left {
                width: 35px;
                height: 36px;
                left: 0px;
                top: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_TL.png');
            }

            .holderbkg .corner-top-right {
                width: 35px;
                height: 36px;
                right: 0px;
                top: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_TR.png');
            }

            .holderbkg .corner-bottom-left {
                width: 35px;
                height: 46px;
                left: 0px;
                bottom: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_BL.png');
            }

            .holderbkg .corner-bottom-right {
                width: 35px;
                height: 46px;
                right: 0px;
                bottom: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_BR.png');
            }

            .holderbkg .border {
                position: absolute;
                background-size: 100% 100%;
            }

            .holderbkg .lb-border-top {
                top: 0px;
                left: 35px;
                right: 35px;
                height: 36px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_TC.png');
            }

            .holderbkg .lb-border-bottom {
                left: 35px;
                right: 35px;
                height: 46px;
                bottom: 0px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_BC.png');
            }

            .holderbkg .lb-border-left {
                left: 0px;
                top: 36px;
                bottom: 46px;
                width: 35px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_L.png');
            }

            .holderbkg .lb-border-right {
                right: 0px;
                top: 36px;
                bottom: 46px;
                width: 35px;
                background-image: url('https://static.realmofempires.com/images/forum/MainBox_R.png');
            }

        #main #content {
            position: relative;
            width: 100%;
            height: auto;
            padding: 5px 0px;
            font: 13px "IM Fell French Canon";
            text-shadow: 0px 1px #000, 0px -1px 1px #000;
        }

        #LoginView1_CreateUserWizard1 {
            margin: 0 auto;
            width: 300px;
        }

        .introText {
            font-size: 20px;
            padding-bottom: 13px;
        }

        input[type="text"], input[type="password"] {
            font-size: 12px;
            width: 170px;
            padding: 5px 5px;
            border-radius: 5px;
            box-shadow: inset -2px 2px 5px rgba(0, 0, 0, 0.4);
            border: none;
        }
        input[type="submit"] {
            position: relative;
            height: 37px;
            width: 135px;
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
        }

        .validationError {
            display: block;
            text-align: center !important;
            color: #FF2121;
        }

        #gotobattle {
            position: relative;
            height: 37px;
            width: 135px;
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
            display: block;
            padding-top: 11px;
        }
    </style>

    <link href="static/bda-ui.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        //set BG based on time of day
        $(document).ready(function () {
            var hours = (new Date()).getHours();
            if (hours >= 7 && hours <= 20) {
                $("body").css("background-image", 'url(https://static.realmofempires.com/images/misc/M_BG_VillageList.jpg)');
            } else {
                $("body").css("background-image", 'url(https://static.realmofempires.com/images/backgrounds/M_LoginCastleNight.jpg)');
            }
        });
    </script>

    <%} %>
</head>


<body>


</body>
</html>
