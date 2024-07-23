<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "l_Why1"
        ,"l_cellUnit"
        ,"l_cellFood"
        ,"l_cellTime"
        ,"l_cellNotEnoughSilver"
        ,"l_cellMaximumTroops"
        ,"l_cellUnit_Chest"
        ,""
        ,""
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
