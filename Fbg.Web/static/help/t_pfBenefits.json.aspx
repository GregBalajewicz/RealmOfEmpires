<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "tActivateAndRefund"
        ,"tExtend"
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
