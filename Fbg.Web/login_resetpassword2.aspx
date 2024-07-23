<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login_resetpassword2.aspx.cs" Inherits="login_resetpassword2" %>


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

        input[type="text"],input[type="password"] {
            font-size: 12px;
            width: 190px;
            padding: 5px 7px;
            border-radius: 5px;
            box-shadow: inset -2px 2px 5px rgba(0, 0, 0, 0.4);
            border: none;
        }

        input[type="submit"] {
            position: relative;
            margin-top: 10px;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            font: 13px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
            height: 40px;
            width: 179px;
            text-align: center;
            box-sizing: border-box;
            cursor: pointer;
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -200px -350px;
            overflow: hidden;
        }

    </style>

</head>


<body class="viewShared">

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
            <form id="Form1" runat="server" style="text-align:center;   font-size: 17px;">
                <div>
                    Your password has been reset.<br />Please check your email for your new random password, and use it to login. You can close this page. 
                </div>              
            </form>
        </div>

    </div>

</body>
</html>
