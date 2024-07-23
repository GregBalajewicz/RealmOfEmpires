<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="VillageOverview.aspx.cs" Inherits="templates_VillageOverview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    <div id="vov">
        <span class="VovAnimFlags" data-isday="%isday%"></span>

        <img class="bckg vovPaintToCanvas" src="%bg%" />
            
        <img ID="imgTroops4e_A"    class="vovPaintToCanvas vov_troops help" style="left: 127px; top: 207px;z-index:11"  />
        <img ID="imgTroops4e_B_L"  class="vovPaintToCanvas vov_troops help" style="left: 0px; top: 233px"  />
        <img ID="imgTroops5e_A_L"  class="vovPaintToCanvas vov_troops help" style="z-index:12;left: 0px; top: 318px"  />
        <img ID="imgKnights3e_A_R" class="vovPaintToCanvas vov_troops help" style="left: 321px; top: 178px"   />
        <img ID="imgKnights5e_A"   class="vovPaintToCanvas vov_troops help" style="left: 158px; top: 342px" />
        <img ID="imgKnights5e_B"   class="vovPaintToCanvas vov_troops help" style="z-index:12;left: 250px; top: 377px" />
        <img ID="imgWallTroops"    class="vovPaintToCanvas vov_troops help" style="left: 47px; top: 403px;z-index:42;" data-canvaslayer="foreground"/>
        <img ID="imgWallBuildings" class="vovPaintToCanvas vov_troops help" style="left: 51px; top: 405px;z-index:41;" data-canvaslayer="foreground"/>

        <img id="imgTreesR_4e" class="vovPaintToCanvas vov_troops help" style="left: 349px; top: 149px;z-index:13" data-canvaslayer="foreground" />         
        <img id="imgTreesL_4e" class="vovPaintToCanvas vov_troops help" style="left: 0px; top: 149px"  />         
        <img id="imgTreesR_6e" class="vovPaintToCanvas vov_troops help" style="left: 340px; top: 391px;z-index:16;"  />         
        <img id="imgTreesL_6e" class="vovPaintToCanvas vov_troops help" style="left: 0px; top: 391px;z-index:16;" />         
        <img id="imgTrees_2e"  class="vovPaintToCanvas vov_troops help" style="left: 151px; top: 98px"  />         
        <img id="imgTrees_3e"  class="vovPaintToCanvas vov_troops help" style="left: 0px; top: 97px;z-index:1;"  />         
        <img id="imgTrees_5e"  class="vovPaintToCanvas vov_troops help" style="left: 0px; top: 256px;z-index:1;"  />        

        <img id="linkBarracks"   data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Barracks %>" class="vovPaintToCanvas vovM vov help" style="left: 51px; top: 283px;z-index:11"/>
        <img id="imgBarracksU"   data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Barracks %>" class="vovPaintToCanvas vovC"  style="left: 51px; top: 283px;z-index:11"/>
        <img id="linkHQ"         data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.VillageHQ %>" class="vovPaintToCanvas vovM vov tut" style="z-index:11;left: 150px; top: 78px;" />
        <img id="imgHQU"         data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.VillageHQ %>" class="vovPaintToCanvas vovC"  style="left: 150px; top: 78px;z-index:11"/>
        <img id="linkFarm"       data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Farmland %>" class="vovPaintToCanvas vovM vov help" rel="bFarm" style="left: 349px; top: 104px" />
        <img id="imgFarm"        data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Farmland %>" class="vovPaintToCanvas vovC"  style="left: 349px; top: 104px"/>
        <img id="linkPalace"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Palace %>" class="vovPaintToCanvas vovM vov help" rel="bPalace" style="left: 112px; top: 0px;z-index:9 !important" />
        <img id="linkMine"       data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.CoinMine %>" class="vovPaintToCanvas vovM vov help" rel="bMine" style="left: 14px; top: 26px;z-index:9"/>
        <img id="imgPalaceU"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Palace %>" class="vovPaintToCanvas vovC"  style="left: 112px; top: 0px;z-index:9 !important"/>
        <img id="linkSiege"      data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop %>" class="vovPaintToCanvas vovM vov help" rel="bSiege" style="left: 360px; top: 44px" />
        <img id="imgSiege"       data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop %>" class="vovPaintToCanvas vovC"  style="left: 360px; top: 44px"/>
        <img id="linkStable"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Stable %>" class="vovPaintToCanvas vovM vov help" style="left: 20px; top: 159px;z-index:3;" />
        <img id="imgStableU"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Stable %>" class="vovPaintToCanvas vovC"  style="left: 20px; top: 159px;z-index:3;"/>
        <img id="linkTavern"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Tavern %>" class="vovPaintToCanvas vovM vov help" rel="bTavern" style="left: 223px; top: 262px" />
        <img id="imgTaverbU"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Tavern %>" class="vovPaintToCanvas vovC"  style="left: 223px; top: 262px"/>
        <img id="linkTrade"      data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.TradePost %>" class="vovPaintToCanvas vovM vov help" rel="bTrade" style="left: 362px; top: 228px;z-index:12;"/>
        <img id="imgTreade"      data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.TradePost %>" class="vovPaintToCanvas vovC"  style="left: 362px; top: 228px;z-index:12;"/>
        <img id="linkTreasury"   data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Treasury %>" class="vovPaintToCanvas vovM vov help" rel="bTreasury" style="left: 270px; top: 155px;z-index:10;" />
        <img id="imgTreasuryU"   data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Treasury %>" class="vovPaintToCanvas vovC"  style="left: 270px; top: 155px;z-index:10;"/>
        <img id="linkHidingSpot" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.HidingSpot %>" class="vovPaintToCanvas vovM vov" style="left: 399px; top: 344px;" />
        <img id="imgWallU"       data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Wall %>" class="vovPaintToCanvas vovC"  style="left: 47px; top: 403px; z-index: 14;" data-canvaslayer="foreground"/>
        <img id="linkWallLBL"    data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Wall %>" ims="0" class="vovPaintToCanvas vovM vov help" rel="bWall" Style="left: 0px; top: 382px; z-index: 14;" data-canvaslayer="foreground"/>
        <img id="linkWallLBR"    data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Wall %>" ims="2" class="vovPaintToCanvas vovM vov help" rel="bWall" Style="left: 323px; top: 386px; z-index: 14;" data-canvaslayer="foreground"/>
        <img id="linkTowerU"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.DefenseTower %>" class="vovPaintToCanvas vov"  style="left: 329px; top: 330px; z-index: 15;" data-canvaslayer="foreground"/>
        <img id="linkTowerR"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.DefenseTower %>" ims="0" class="vovPaintToCanvas vovM vov_tower help" rel="bTower" style="left: 329px; top: 330px; z-index: 15;" data-canvaslayer="foreground"/>
        <img id="linkTowerL"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.DefenseTower %>" ims="2" class="vovPaintToCanvas vovM vov_tower help" rel="bTower" style="left: -14px; top: 328px; z-index: 15;" data-canvaslayer="foreground"/>
        <img id="imgTowerLU"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.DefenseTower %>" class="vovPaintToCanvas vovC vov_tower"  style="left: 0px; top: 328px; z-index: 15;" data-canvaslayer="foreground"/>
        <img id="linkWall"       data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Wall %>" ims="1" class="vovPaintToCanvas vovM vovWallFront help " rel="bWall" style="left: 47px; top: 403px; z-index: 16;" data-canvaslayer="foreground"/>  

        <div id="shieldIconBarracks"   data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Barracks %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Barracks.png" class="vovShieldIcon" />
        <div id="shieldIconPalace"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Palace %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Palace.png" class="vovShieldIcon" />
        <div id="shieldIconSiege"      data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Seige.png" class="vovShieldIcon" />
        <div id="shieldIconStable"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Stable %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Stables.png" class="vovShieldIcon" />
        <div id="shieldIconTavern"     data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Tavern %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Tavern.png" class="vovShieldIcon" />
        <div id="shieldIconTrade"      data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.TradePost %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Trading.png" class="vovShieldIcon" />
        <div id="shieldIconHidingSpot" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.HidingSpot %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Hiding.png" class="vovShieldIcon" />
        <div id="shieldIconWall"       data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Wall %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Wall.png" class="vovShieldIcon" />
        <div id="shieldIconTower"      data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.DefenseTower %>" data-icon="https://static.realmofempires.com/images/vov/M_Build-Tower.png" class="vovShieldIcon" />

        <img  ID="LinkHQ_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.VillageHQ %>" class="anim vov vovlabel "  style="left: 185px; top: 136px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="LinkHQ_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.VillageHQ %>" class="click ButtonTouch level vov vovlabel zoom2"  style="left: 200px; top: 100px;z-index:5;" />
        
        <% if(!isD2 && !isMobile) { %>
        <img  ID="linkHQ_UAnimAll" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.VillageHQ %>" class="ranim vov vovlabel"  style="left: 185px; top: 160px;z-index:5;" src="https://static.realmofempires.com/images/hammerX3_anim.gif"  />
        <span ID="LinkHQ_LevelCountdown2" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.VillageHQ %>" rel="bHQ" refresh="false" class="recruitcount  vov vovCountdown help"   style="left: 212px; top: 165px;z-index:5;" Text="0:00:00" data-refreshCall="ROE.Frame.reloadView();"/>                       
        <% } %>
        <span ID="LinkHQ_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.VillageHQ %>" rel="bHQ" refresh="true" class="buildcount  vov vovCountdown  help"   style="left: 212px; top: 140px;z-index:5;" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>                       
        
        <img  ID="linkBarracks_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Barracks %>" class="anim vov vovlabel"  style="left: 81px; top: 279px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkBarracks_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Barracks %>" class="click ButtonTouch level vov vovlabel help zoom3" rel="bBarracks_Level" style="left: 100px; top: 254px"/>
        <img  ID="imgBarracksRecruit" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Barracks %>" class="ranim vov vovCountdown "  style="left: 81px; top: 304px" src="https://static.realmofempires.com/images/recruiting_anim.gif"  />
        <span ID="LinkBarraks_LevelCountdown" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Barracks %>" class="recruitcount  vov vovCountdown help" rel="bBarracks" refresh="true" style="left: 108px; top: 310px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        <span ID="LinkBarraks_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Barracks %>" class="buildcount  vov vovCountdown help" rel="bBarracks" refresh="true" style="left: 108px; top: 283px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        
        <img  ID="linkFarm_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Farmland %>" class="anim vov vovlabel "  style="left: 379px; top: 125px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkFarm_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Farmland %>" class="click ButtonTouch level vov vovlabel help zoom2" rel="bFarm_Level"  style="left: 381px; top: 124px"/>
        <span ID="linkFarm_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Farmland %>" class="buildcount  vov vovCountdown help" rel="bFarm_Level" refresh="true"   style="left: 399px; top: 129px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        
        <img  ID="linkMine_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.CoinMine %>" class="anim vov vovlabel "  style="left: 68px; top: 75px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkMine_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.CoinMine %>" rel="bMine_Level" class="click ButtonTouch level vov vovlabel help zoom1"  style="left: 73px; top: 69px"/>
        <span ID="linkMine_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.CoinMine %>" class="buildcount  vov vovCountdown help" rel="bMine_Level" refresh="true"   style="left: 95px; top: 79px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        
        <img  ID="linkHidingSpot_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.HidingSpot %>" class="anim vov vovlabel "  style="left: 392px; top: 331px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkHidingSpot_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.HidingSpot %>" class="click ButtonTouch level vov vovlabel help zoom3"  style="left:394px; top: 315px"/>
        <span ID="linkHidingSpot_LevelCountdownUgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.HidingSpot %>" class="buildcount  vov vovCountdown help" rel="bHidingSpot_Level" refresh="true" style="left: 420px; top: 335px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        
        <img  ID="linkPalace_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Palace %>" class="anim vov vovlabel "  style="left: 197px; top: 46px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkPalace_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Palace %>" rel="bPalace" class="click ButtonTouch level vov vovlabel help zoom1"   style="left: 236px; top: 10px" />
        <span ID="LinkPalace_LevelCountdown" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Palace %>" rel="bPalace" refresh="true" class="recruitcount  vov vovCountdown help"   style="left: 225px; top: 76px"  Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        <span ID="LinkPalace_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Palace %>" rel="bPalace" refresh="true" class="buildcount  vov vovCountdown help"   style="left: 225px; top: 50px"  Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        <img  ID="imgPalaceRecruit" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Palace %>" class="ranim vov vovCountdown "  style="left: 197px; top: 71px" src="https://static.realmofempires.com/images/recruiting_anim.gif"  />
        
        <img  ID="linkSiege_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop %>" class="anim vov vovlabel "  style="left: 363px; top: 56px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkSiege_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop %>" rel="bSiege_Level" class="click ButtonTouch level vov vovlabel help zoom1"  NavigateUrl="~/BuildingTroops.aspx?data-bid=10" style="left: 368px; top: 44px" />
        <span ID="LinkSiege_LevelCountdown" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop %>" rel="bSiege" refresh="true" class="recruitcount  vov vovCountdown help"   style="left: 390px; top: 87px;" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        <span ID="LinkSiege_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop %>" rel="bSiege" refresh="true" class="buildcount  vov vovCountdown help"   style="left: 390px; top: 60px;" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        <img  ID="imgSiegeRecruit" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop %>" class="ranim vov vovCountdown "  style="left: 363px; top: 82px;" src="https://static.realmofempires.com/images/recruiting_anim.gif"  />
        
        <img  ID="linkStable_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Stable %>" class="anim vov vovlabel "  style="left: 60px; top: 181px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkStable_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Stable %>" rel="bStable_Level" class="click ButtonTouch level vov vovlabel help zoom2"  style="left: 65px; top: 165px;" />
        <span ID="LinkStable_LevelCountdown" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Stable %>" rel="bStable" refresh="true" class="recruitcount  vov vovCountdown help"   style="left: 87px; top: 212px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        <span ID="LinkStable_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Stable %>" rel="bStable" refresh="true" class="buildcount  vov vovCountdown help"   style="left: 87px; top: 185px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        <img  ID="imgStableRecruit" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Stable %>" class="ranim vov vovCountdown "  style="left: 60px; top: 206px" src="https://static.realmofempires.com/images/recruiting_anim.gif"  />
        
        <img  ID="linkTavern_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Tavern %>" class="anim vov vovlabel "  style="left: 277px; top: 296px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkTavern_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Tavern %>" rel="bTavern_Level" class="click ButtonTouch level vov vovlabel help zoom3"  style="left: 285px; top: 280px" />
        <span ID="LinkTavern_LevelCountdown" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Tavern %>" rel="bTavern" refresh="true" class="recruitcount  vov vovCountdown help"   style="left: 305px; top: 327px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        <span ID="LinkTavern_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Tavern %>" rel="bTavern" refresh="true" class="buildcount  vov vovCountdown help"   style="left: 305px; top: 300px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();" />
        <img  ID="imgTavernRecruit" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Tavern %>" class="ranim vov vovCountdown "  style="left: 277px; top: 321px" src="https://static.realmofempires.com/images/recruiting_anim.gif"  />
        
        <img  ID="linkTrade_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.TradePost %>" class="anim vov vovlabel "  style="left: 392px; top: 248px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkTrade_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.TradePost %>" rel="bTrade_Level" class="click ButtonTouch level vov vovlabel help zoom3"  style="left: 397px; top: 212px" />
        <span ID="linkTrade_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.TradePost %>" rel="bTrade_Level" refresh="true" class="buildcount  vov vovCountdown help"   style="left: 420px; top: 252px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        
        <img  ID="linkTreasury_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Treasury %>" class="anim vov vovlabel "  style="left: 285px; top: 170px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkTreasury_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Treasury %>" rel="bTreasury_Level" class="click ButtonTouch level vov vovlabel help zoom2"  style="left: 310px; top: 140px" />
        <span ID="linkTreasury_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Treasury %>" rel="bTreasury_Level" refresh="true" class="buildcount  vov vovCountdown help"   style="left: 312px; top: 174px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        
        <img  ID="linkWall_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Wall %>" class="anim vov vovlabel " style="left: 199px; top: 485px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkWall_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Wall %>" rel="bWall_Level" class="click ButtonTouch level vovWallFront vovlabel help zoom3"  style="left: 214px; top: 399px" />
        <span ID="linkWall_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.Wall %>" rel="bWall_Level" refresh="true" class="buildcount  vov vovCountdown help"   style="left: 226px; top: 489px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>
        
        <img  ID="linkTower_UAnim" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.DefenseTower %>" class="anim vov vovlabel "  style="left: 36px; top: 379px;z-index:5;" src="https://static.realmofempires.com/images/hammer_anim.gif"  />
        <div  ID="linkTowerL_Level" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.DefenseTower %>" rel="bTower_Level" class="click ButtonTouch level vov vovlabel help zoom3"  style="left: 61px; top: 353px" />
        <span ID="linkTowerL_LevelCountdownUpgrade" data-bid="<%= Fbg.Bll.CONSTS.BuildingIDs.DefenseTower %>" rel="bTower_Level" refresh="true" class="buildcount  vov vovCountdown help"   style="left: 63px; top: 383px" Text="0:00:00"  data-refreshCall="ROE.Frame.reloadView();"/>

        <canvas id="backgroundlayer" width="475" height="531" style="position:absolute; left: 0px; top: 0px; z-index: 0" data-canvaslayer="background"></canvas>
        <canvas id="foregroundlayer" width="475" height="531" style="position:absolute; left: 0px; top: 0px; z-index: 2" data-canvaslayer="foreground"></canvas>
        <canvas id="animationlayer" width="475" height="531" style="position:absolute; left: 0px; top: 0px; z-index: 1" data-canvaslayer="animation"></canvas>

        <% if(thisDevice == CONSTS.Device.Amazon) { %> 
            <div class="html5SwipeWrapper inVov">
                <div class="html5SwipeFloater">
                </div>
            </div>
        <%} %> 
              

        <div class="phrases"></div>
    </div>
</asp:Content>
