<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VillageHeaderInfo.ascx.cs"
    Inherits="Controls_VillageHeaderInfo" %>
<%@ Register Src="QuickTransportCoins.ascx" TagName="QuickTransportCoins" TagPrefix="uc1" %>

<asp:Panel ID="panelVillageHeaderInfo" runat="server" Width="100%" Visible="False" CssClass="header">
<table border="0" cellpadding="0" cellspacing="0"  >
<tr><td>
<table width="100%" height="23" border="0" cellpadding="0" cellspacing="0" style="font-size: 13px" >
    <tr> 
      <td class="silverIcon"><a href="QuickTransportCoins.aspx?svid=<%=_village.id.ToString() %>" OnClick="return popupQuickSilverTransport(this);" > <asp:Image runat="server" id="imgSilver"  ImageUrl="https://static.realmofempires.com/images/coin.png" /></a></td>
      <td class="S">
          <asp:UpdatePanel ID="UpdatePanel1" runat="server">
          <ContentTemplate>
            <span class="help" rel="jSilver"><asp:Label CssClass="tut" ID="lblCoins" runat="server"></asp:Label>/<asp:Label
                CssClass="tut" ID="lblTrasury" runat="server"></asp:Label></span>
                <span runat="server" id="spanProd" class="tut SP" rel="jSilverProd">(+<asp:Label CssClass="help" rel="jSilverProd" ID="lblProduction" runat="server"></asp:Label>)</span>
            </ContentTemplate>
            </asp:UpdatePanel>           
       </td>          
      <td width="23" class="FI"><img src='<%=RSic("Food")%>'></td>
      <td class="help F" rel="jFood"><asp:UpdatePanel ID="UpdatePanel2" runat="server"><ContentTemplate><asp:Label ID="lblPopulation" CssClass="tut" runat="server"></asp:Label></ContentTemplate></asp:UpdatePanel></td>
      <td class="R"></td>
        <td width="150" >
            <a ID="linkIncomingAttacks" class="IncomingAttacksWarning help" rel="jIncomingAttack" 
                runat="server" href="..\TroopMovementsIn.aspx?vid=-1">
                <img border=0 src="<%=RSic("IncomingAttack")%>" style="vertical-align:middle;" />
                <asp:Label ID="lblIncomingCount" CssClass="IncomingAttacksWarning" 
                    runat="server" style="vertical-align:middle;" />
                <asp:Label ID="lblCountdown" CssClass="IncomingAttacksWarning countdown" 
                    refresh="false" runat="server" style="vertical-align:middle;" />
            </a>
            

        </td>          
    </tr>
</table>
<table height="23" border="0" cellpadding="0" cellspacing="0">
    <tr> 
        <td width="12" background="https://static.realmofempires.com/images/hdrBG_vL.gif">&nbsp;</td>
        <td background="https://static.realmofempires.com/images/hdrBG_vC.gif">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr> 
                  <td nowrap >
                    <asp:Panel ID="panelFiltersAndTagsParent" runat="server" CssClass="jsFakeSelect">
                        <asp:HyperLink ID="linkFilter" runat="server" CssClass="jsMaster jsTriger" ImageUrl="https://static.realmofempires.com/images/funnel2.png" NavigateUrl="" Visible="false" />
                        <asp:Panel ID="panelFiltersAndTags" runat="server" CssClass="jsOptions ui_menu FT" >
                            <table cellpadding="0" cellspacing="0"><tr><td valign="top" style="border-right:solid rgb(195, 144, 55) 1px;">
                                <span class="FT_Title"><%= RS("filters")%></span>
                                <asp:Panel ID="panelFilterList" runat="server" style="padding:2px; ">
                                </asp:Panel>
                            </td><td valign="top">
                                <span class="FT_Title"><%= RS("tags")%></span>
                                <asp:Panel ID="panelTagList" CssClass="tags" runat="server" style="padding:2px; ">
                                </asp:Panel>
                            </td></tr></table>
                        </asp:Panel>
                    </asp:Panel>
                    <asp:HyperLink ID="linkVillageSelectiondd" runat="server" OnClick="return popupVillageSelectionList(this);" ImageUrl="https://static.realmofempires.com/images/dropdown.png" NavigateUrl="~/MyVillages.aspx" class="jsVddMenu"></asp:HyperLink>
                  </td>
                  <td width="270" nowrap style="font-weight: bold;" class="tut" rel="<%=CONSTS.TutorialScreenElements.VillageHUD_VilName%>"> 
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" RenderMode="Inline">
                        <ContentTemplate>         
                       <asp:HyperLink ForeColor="black" ID="linkVillageName" runat="server"
                                NavigateUrl="~/VillageOverview.aspx?{0}={1}">HyperLink</asp:HyperLink></ContentTemplate></asp:UpdatePanel></td>
                </tr>
            </table>  
        </td>
        <td background="https://static.realmofempires.com/images/hdrBG_vC.gif" ><asp:HyperLink ID="linkPrevVillage" CssClass="jPrevVillage" runat="server" ImageUrl="https://static.realmofempires.com/images/leftarrow.png"></asp:HyperLink></td>              
        <td width="30" background="https://static.realmofempires.com/images/hdrBG_vRtrans.gif" align="left" ><asp:HyperLink ID="linkNextVillage" CssClass="jNextVillage"  runat="server" ImageUrl="https://static.realmofempires.com/images/rightarrow.png"></asp:HyperLink></td>
        <td align=left nowrap> 
            <span class="tut" rel="<%=CONSTS.TutorialScreenElements.MapLink%>" ><asp:HyperLink CssClass="genericMenuFake map" ID="linkMap" runat="server" NavigateUrl="~/Map.aspx?{0}={1}&{2}={3}&{4}={5}"><%= RS("map")%></asp:HyperLink></span>
            <a href="CommandTroops.aspx" class="jsTroopsMenu"><%= RS("troops")%></a>
            <asp:Label runat="server" ID="curServerTime" CssClass="Time help" Rel="jServerTime" />
        </td> 
        
    </tr>
  </table>
  </td><td valign=top nowrap style="font-size: 13px" id=sleepModeIndicator runat=server visible=false><a class=sleep href=playeroptions.aspx><img src="https://static.realmofempires.com/images/misc/SleepIcon.png" style="vertical-align:middle;" border=0 /><asp:Label ID="lblSleepMode_Countdown" runat="server" CssClass="countdown"></asp:Label></a></td>
  </tr>
  </table>
</asp:Panel>

