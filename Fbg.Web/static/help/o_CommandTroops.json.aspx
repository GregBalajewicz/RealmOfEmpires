<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "o_CommandTroops"
        ,"o_RamTarget"
        ,"o_TrebuchetTarget"
        ,"o_TargetVillage"
        ,"o_Support"
        ,"o_Attack"
        ,"o_IncomingTroops"
        ,"o_OutgoingTroops"
        ,"o_Handicap"
        ,"o_RebelTooFar"
        ,"o_desert"
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
