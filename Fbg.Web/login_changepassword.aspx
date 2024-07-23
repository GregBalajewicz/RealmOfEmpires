<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login_changepassword.aspx.cs" Inherits="login_changepassword" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">

    <title><%=RSc("GameName") %></title>

    <meta charset="utf-8" />
    <%if (Device == CONSTS.Device.iOS){ %>
    <meta name="viewport" content="width=320, inital-scale=1.0, maximum-scale=5.0, user-scalable=0" />
    <%}else{ %>
    <meta name="viewport" content="width=321, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <%} %>

    <link href="https://fonts.googleapis.com/css?family=IM+Fell+French+Canon+SC|IM+Fell+French+Canon+LC" rel="stylesheet" type="text/css" />
    <link href="roe-pregame.css" rel="stylesheet" type="text/css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js" type="text/javascript"></script>


    <!-- page specific styling -->
    <style>

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
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            height: 37px;
            width: 135px;
        }

        .validationError {
            display: block;
            text-align: center !important;
            color: #FF2121;
        }
    </style>


<%if (isMobile){ %>
    <style>
        input[type="text"], input[type="password"] {
            font-size: 12px;
            width: 140px;
            padding: 5px 5px;
            border-radius: 5px;
            box-shadow: inset -2px 2px 5px rgba(0, 0, 0, 0.4);
            border: none;
        }
    </style>

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


<body class="<%if (isMobile){ %>viewMobile<%}else{%>viewDesktop<%}%>">

    <div id="knightsR"></div>
    <div id="knightsL"></div>

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
            <form id="Form1" runat="server">
                <asp:LoginView ID="LoginView1" runat="server">
                    <LoggedInTemplate>
                        <center>
                            <asp:ChangePassword ContinueDestinationPageUrl="ChooseRealm.aspx" CancelDestinationPageUrl="ChooseRealm.aspx" id="cp" runat="server"></asp:ChangePassword>                            
                        </center>
                    </LoggedInTemplate>

                    <AnonymousTemplate>
                        Please <a href="login_enter.aspx">login first </a>
                    </AnonymousTemplate>
                </asp:LoginView>
            </form>
        </div>

    </div>


</body>

<script type="text/javascript">
    $(document).ready(function () {
        $('input[value="Change Password"]').attr('value','Confirm');
    });
</script>

</html>
