<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ExclusiveRealmMessage_NPOnly.aspx.cs" Inherits="ExclusiveRealmMessage_NPOnly" %>

<%@ Register Src="Controls/ListOfRealms.ascx" TagName="ListOfRealms" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%=RSc("RealmOfEmpires") %></title>
    <link href="main.2.css" rel="stylesheet" type="text/css" />

    <script src="script-nochange/jquery-1.2.3.js" type="text/javascript"></script>

    <script src="script/countdown_old.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(initTimers);
    </script>

</head>
<body style="margin-left: 0; margin-right: 0; color: white; width: 730px;">
    <center>
        <img src='<%=RSic("Main2c_hedaer")%>' />
            <br /><span style="font-size: 12pt;"><%=RS("ExclusiveRealmForNobility") %> 
            
            <br /><br /><%=RS("MustHaveNPFromAnotherRealm") %><br />
        <br /><a target=_blank href="http://www.facebook.com/topic.php?uid=10471770557&topic=22872"><%=RS("ClickForDetails") %></a>
        <br /><br /><a href=ChooseRealm2.aspx><%=RS("Back") %></a>
            </span><br />
    </center>
</body>
</html>
