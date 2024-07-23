<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "k_buychest"
        ,"k_notran"
        ,"k_tran"
        ,"k_SelectedFilter"
        ,""
        ,""
        ,""
        ,""
        ,""
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
