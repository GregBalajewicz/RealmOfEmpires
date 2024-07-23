<%@ Page Language="C#" AutoEventWireup="true" CodeFile="welcome2.aspx.cs" Inherits="Welcome2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%= RSc("GameName") %></title>
    <link href="main.2.css" rel="stylesheet" type="text/css" />
</head>
<body style="margin-left: 0; margin-right: 0; font-size: 11pt">
    <center>
        <div style="width: 740px;">
            <center>
                <img src="https://static.realmofempires.com/images/Main2c_hedaer.jpg" />
                <strong>
                    <br />
                    <%= RS("cookieProb") %>
                </strong>
                <p style="text-align: center">
                    <%= RS("browserNoCookie")%>
                    <span style="text-decoration: underline">
                        <%= RS("_not")%></span>
                    <%= RS("pleaseEnable") %>
                    <asp:HyperLink ID="linkTryAgain" runat="server" NavigateUrl="~/welcome.aspx"><%= RS("link_TryAgain")%></asp:HyperLink>.
                    <br />
                    **
                    <%= RS("note")%>
                    '<asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="http://realmofempires.blogspot.com/2009/03/cannot-enter-realm-of-empires-3rd-party.html"
                        Target="_blank"><%= RS("link_thirdParty") %></asp:HyperLink>'.
                </p>
                <p>
                    <%= RS("ifContinuedProb")%>
                    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="http://www.new.facebook.com/apps/application.php?id=10471770557#/apps/application.php?id=10471770557&v=app_2373072738&viewas=579461442"
                        Target="_blank"><%= RS("link_DisForum")%></asp:HyperLink><%= RS("bestOption")%>
                    <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="mailto:support@realmofempires.com"><%= RS("link_MsgSupport")%></asp:HyperLink>.
                    <br />
                    &nbsp;
                </p>
            </center>
        </div>
    </center>
</body>
</html>
