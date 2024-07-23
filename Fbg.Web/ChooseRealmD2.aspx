<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChooseRealmD2.aspx.cs" Inherits="ChooseRealmD2" %>


<!doctype html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0">


    <link href="main.2.css" rel="stylesheet" type="text/css" />
    <script src="<%= Config.InDev ? "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.js" : "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js" %>" type="text/javascript"></script>
    <script src="script-nochange/jquery.dimensions.js" type="text/javascript"></script>
    <script src="script/iphonefix.js" type="text/javascript" ></script>

      <style>
        body {width:100%; font-size:14pt;  text-shadow: 2px 2px 2px #0a0a0a;}
        #logo {float:left; width:100px;}
        #title { text-shadow: 2px 2px 2px #0a0a0a;   font-size:18pt;}
        .tblRealms {clear:both;}
        .tblRealms a.showFBFriends {display:none;}
      </style>
      <script>
          $(function () {
              if (window.parent !== window) {
                  $('.entermgame').attr('target', '_blank');
              }
          });
      </script>
</head>
<body style="margin-left: 0; margin-right: 0; color: white; ">
    <center>
        <div id=main >           
                <image id=logo src=https://static.realmofempires.com/images/fancylogo.png />
                <span id=title>Welcome to Realm of Empires Mobile</span>
           <center>
            <div style="clear:both;"><a class=entermgame href='chooserealmmobile2.aspx'>Try our new, pre-alpha, still-pretty-ugly mobile version</a>
            <span style="font-size:9pt;text-shadow:none"><br />(Switch back anytime, from the options menu)</span>
            <br /> ~
            <br /><a href='chooserealmmobileOldVersion.aspx'>Enter the current production version</a></div>

            </center>
            
            </div>

    </center>
</body>
</html>
