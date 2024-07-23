<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "u_InviteLimit"
      
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
