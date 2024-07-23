<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "dAttack"
        ,"dSupport"
        ,"dReturn"
        ,"dIncomingTroops"
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
