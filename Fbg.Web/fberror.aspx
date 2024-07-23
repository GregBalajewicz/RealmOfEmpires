<%@ Page Language="C#" AutoEventWireup="true" CodeFile="fberror.aspx.cs" Inherits="fberror" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Realm Of Empires</title>
</head>
<body style="margin-left: 0; margin-right: 0; background: #898E92; color: white">
    <br />
    <table>
        <tr>
            <td>
            </td>
            <td>
                <img height="250" src='<%=RSic("Roe")%>' /></td>
            <td align="center">
                    <img src='<%=RSic("Logo")%>' style="font-weight: bold" /><br />
                    <br />
                <strong>
                        Our apologies but we are experiencing some problems interfacing with Facebook.
                    <br />
                </strong>
                <br />
                For the last little while, we have been experiencing failures when trying to connect
                to Facebook and are working to resolve this.
                <br />
                <br />
                This is an intermittent problem so you can just ignore it and continue -
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/VillageOverview.aspx">Go back to village overview now</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
    </table>
</body>
</html>
