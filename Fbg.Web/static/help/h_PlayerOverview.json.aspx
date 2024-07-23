<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "hNotes"
        ,"hPoints"
        ,"hClan"
        ,"hRank"
        ,"hAt"
        ,"hSu"
        ,"hT"
        ,"hM"
        , "hxp"
    };

    return BuildHelpJSON(entires, "_title", "_st");
 }

</script>
<%=s() %>
