<%@ Page Language="C#" Inherits="BasePageWithRes"%>


<script runat=server>


protected string s() 
{

    string[] entires = new string[] { 
        "cDelete"
        ,"cDet"
        ,"cAttack"
        ,"cSupport"
        ,"cOutgoingTroops"
        ,""
        ,""
        ,""
        ,""
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
