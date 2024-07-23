<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "q_DefenderWallLevel"
        ,"q_DefenderTowerLevel"
        ,"q_TargetLevel"
        ,"q_Handicap"
        ,"q_desert"
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
