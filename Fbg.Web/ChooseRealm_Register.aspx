<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChooseRealm_Register.aspx.cs"
    Inherits="ChooseRealm_Register" %>

<%@ Register Src="Controls/ListOfRealms.ascx" TagName="ListOfRealms" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Realm of Empires</title>
    <link href="main.2.css" rel="stylesheet" type="text/css" />
    
    <script src="script-nochange/jquery-1.2.3.js" type="text/javascript"></script>


    <script type="text/javascript">
        $(initTimers);
    </script>
</head>
<body style="margin-left: 0; margin-right: 0; color: white; width: 730px;">
    <center>
        <img src='<%=RSic("Main2c_hedaer")%>'/>
        <asp:Panel ID="panelWelcome" runat="server" >
        <table>
            <tr>
                <td valign="top"> 
                    <span style="font-size: 12pt">
                        <asp:Label ID="lblWelcome" runat="server" Text="Welcome"></asp:Label>&nbsp;<br />
                    </span>
                    <span style="font-size: 12pt">
                        <%=RSc ("suggest") %>
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="create.aspx?rid=2"><%=RS ("Realm2") %></asp:HyperLink>
                    </span>
                    &nbsp;<br />
                    <br />
                </td>
            </tr>
        </table>
        </asp:Panel>
    <asp:Panel ID="panelRealms" runat="server" >
        <table >
            <tr>
                <td>
                    <asp:Table ID="tableRealms" runat="server" CellPadding="2" CellSpacing="3">
                    </asp:Table>
                    <uc1:ListOfRealms ID="lor" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
        
    </center>
</body>
</html>
