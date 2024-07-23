<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "w_dest"
        ,"w_landtime"
        ,"w_hide"
       
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
