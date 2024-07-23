<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "r_Plunder"
        
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>

