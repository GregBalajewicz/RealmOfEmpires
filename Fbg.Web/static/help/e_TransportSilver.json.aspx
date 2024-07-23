<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "eReservedchk"
        ,"bOutgoingTroops"
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
