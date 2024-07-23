<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "x_outgoing"
        ,"x_incoming"
        ,"x_showtags"
        ,"x_summary"
        ,"x_highlights"
        , "x_showalltags"
       
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
