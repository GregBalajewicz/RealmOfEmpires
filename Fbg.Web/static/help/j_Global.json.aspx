<%@ Page Language="C#" Inherits="BasePageWithRes"%>


<script runat=server>


protected string s() 
{

    string[] entires = new string[] { "jOverdue"
        , "jVillName"
        ,"jSilver"
        ,"jFood"
        ,"jSilverProd"
        ,"jServerTime"
        ,"jFromUnknown"
        ,"jIncomingAttack"
    };

    return BuildHelpJSON(entires);
 }

</script>

<%=s() %>
