<%@ Page Language="C#" Inherits="BasePageWithRes"%>


<script runat=server>


protected string s() 
{

    string[] entires = new string[] { "bSupportCol"
        , "bTotalCol"
        ,"bInVillageNowCol"
        ,"bYourTroopsTotalCol"
        ,"bsilvermineDesc"
    };

    return BuildHelpJSON(entires);
 }

</script>

<%=s() %>
