<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "m_SendBack"
        ,"m_MoreInfo"
        ,"m_RecallSome"
       
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
