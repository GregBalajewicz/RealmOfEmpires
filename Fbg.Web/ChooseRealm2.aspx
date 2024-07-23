<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChooseRealm2.aspx.cs" Inherits="ChooseRealm2" %>

<%@ Register Src="Controls/ListOfRealms.ascx" TagName="ListOfRealms" TagPrefix="uc1" %>




<!doctype html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Realm Of Empires</title>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0"/>

    <link href="main.2.css" rel="stylesheet" type="text/css" />

    <script src="<%= Config.InDev ? "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.js" : "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js" %>" type="text/javascript"></script>

    <script src="script/countdown_old.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(initTimers);
    </script>


    <%if (!isMobile)
      { %>
     <style>   
        #main {background-image: url('<%= RSi("introBG")%>'); width: 740px; height: 758px; border: solid 1px black}
        body {width: 740px;}
    </style>
    <%}
      else
      {%>
      <style>
        body {width:100%;  }
        #logo {float:left; width:100px;}
        #title { text-shadow: 2px 2px 2px #0a0a0a;   font-size:18pt;}
        .tblRealms {clear:both;}
        .tblRealms a.showFBFriends {display:none;}
      </style>
    <%} %>

</head>
<body style="margin-left: 0; margin-right: 0; color: white; ">
    <center>
        <%if (!isMobile)
            { %>
            <br />
            <img src='<%=RSic("Main2c_hedaer")%>'/>
            <div style="position:relative;margin-top:-45px;margin-left:-40px;">
                <span style="font-size: 12pt;">
                    <asp:Label ID="lblWelcome" runat="server" Text="Welcome"></asp:Label>
                    <br />
                &nbsp;&nbsp;&nbsp;<%=RS ("Choose") %></span><br />
            </div>
        <%} else{ %>
            <image id=logo src=https://static.realmofempires.com/images/fancylogo.png />
            <span id=title><%=RSc ("Welcome") %> </span>
        <%} %>
        <div style="margin-top:8px;" class=tblRealms>
         <uc1:ListOfRealms ID="ListOfRealms1" runat="server" />
         </div>
        <%if (Context.User.IsInRole("Admin") || Context.User.IsInRole("tester")) {%><a href="admin_tester/chooserealm2.aspx">Admin short cut -Access Any Realm</a><%} %>
    </center>
<%if (Config.Theme == "roe" && !isMobile)
  {%>    
    <script type="text/javascript">
var uservoiceOptions = {
  /* required */
  key: 'roe',
  host: 'roe.uservoice.com', 
  forum: '96839',
  showTab: true,  
  /* optional */
  alignment: 'left',
  background_color:'#f00', 
  text_color: 'white',
  hover_color: '#06C',
  lang: 'en'
};

function _loadUserVoice() {
  var s = document.createElement('script');
  s.setAttribute('type', 'text/javascript');
  s.setAttribute('src', ("https:" == document.location.protocol ? "https://" : "http://") + "cdn.uservoice.com/javascripts/widgets/tab.js");
  document.getElementsByTagName('head')[0].appendChild(s);
}
_loadSuper = window.onload;
window.onload = (typeof window.onload != 'function') ? _loadUserVoice : function() { _loadSuper(); _loadUserVoice(); };
</script>
<%} %>
</body>
</html>
