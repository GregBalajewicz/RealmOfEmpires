<%@ Page Inherits="BasePageWithRes" Language="C#" %>

<script runat="server">


protected string s() 
{

    string[] entires = new string[] { 
        "s_BuildingsCost"
        ,"s_BuildingsCost"
        ,"s_BuildingsFood"
        ,"s_BuildingsNoFood"
        ,"s_BuildingsPoints"
        ,"s_BuildingsEffect"
        ,"s_UnitsCost"
        ,"s_UnitsRecruitTime"
        ,"s_UnitsAttackStrength"
        ,"s_UnitsDefenseStrength"
        ,"s_UnitsFood"
        ,"s_UnitsMovementSpeed"
        ,"s_UnitsCarry"
    };

    return BuildHelpJSON(entires);
 }

</script>
<%=s() %>
