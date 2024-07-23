<%@ Page Language="C#" Inherits="BasePageWithRes"%>


<script runat=server>


protected string s() 
{

    string[] entires = new string[] { "tblBuildings"
        , "tblUpgrades"
        ,"aSilverLacking"
        ,"aBusy"
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>