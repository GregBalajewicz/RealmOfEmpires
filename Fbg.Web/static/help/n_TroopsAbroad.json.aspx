<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "n_Recall"
        ,"n_RecallSome"
        ,"n_MoreInfo"
   
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
