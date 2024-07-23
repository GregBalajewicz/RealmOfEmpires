<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "v_UR"
       
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
