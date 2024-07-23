using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fbg.Bll
{



    public class QuestTemplate_Research : BDA.Achievements.QuestTemplate
    {
        // reward, sequence spot, dependant, group
        public ResearchItem ResearchItem { get; set; }

        public QuestTemplate_Research(string tag, string title, ResearchItem researchItem, string desc)
        {
            ResearchItem = researchItem;
            Title = title;
            Goal = "Research " + researchItem.Name;
            
            //Description = string.Format("<img src='{0}' class='riimg'/>", researchItem.ImageUrl) + desc + string.Format("<br /><img class='questResearchImg' border='0' align='center' src='https://static.realmofempires.com/images/misc/Arrow_pointer_East.gif' width='32'> <a class='researchLink' data-riid='{1}' href=research.aspx?riid={1}>Research {0} Now</A> ", researchItem.Name, researchItem.ID);
            Description = "<div class=\"researchIcon\" style=\"background-position: -" + researchItem.SpriteSheetLocX + "px -" + researchItem.SpriteSheetLocY  + "px;\"></div>" + 
            desc + 
            string.Format("<br /><img class='questResearchImg' border='0' align='center' src='https://static.realmofempires.com/images/misc/Arrow_pointer_East.gif' width='32'> <a class='researchLink' data-riid='{1}' href=research.aspx?riid={1}>Research {0} Now</A> ", researchItem.Name, researchItem.ID);
            Tag = tag;
        }
    }



    public class QuestTemplate_NumVillages: BDA.Achievements.QuestTemplate
    {
        // reward, sequence spot, dependant, group
        public int NumVillges { get; set; }

        public QuestTemplate_NumVillages(string tag, int numVillges)
        {
            NumVillges = numVillges;
            Title = "Expand Your Empire";
            Goal = string.Format("Capture Your {0} Village", utils.Ordinal(numVillges));
            Description = string.Format("Increase the number of villages you control to {0} by attacking ", numVillges);
            Tag = tag;
        }
    }



    public class QuestTemplate_Title : BDA.Achievements.QuestTemplate
    {
        // reward, sequence spot, dependant, group
        public Fbg.Bll.Title TitleLevel { get; set; }

        public QuestTemplate_Title(string tag, Fbg.Bll.Title title)
        {
            TitleLevel = title;
           
            Title = "Advance Social Standing";
            Goal = "Reach Realm Title of " + title.TitleName_Male;
            Description = "Increase your overall village points by building and upgrading buildings, and the Council of Elders will grant you a new title.";
            Tag = tag;
        }
    }


    public class QuestTemplate_BuildingLevel : BDA.Achievements.QuestTemplate
    {
        // reward, sequence spot, dependant, group
        public Fbg.Bll.BuildingTypeLevel BuildingLevel { get; set; }

        public QuestTemplate_BuildingLevel(string tag, BuildingTypeLevel buildingLevel, string customDescription, string title, string goal)
        {

            BuildingLevel = buildingLevel;
            Title = title == null ? GetTitle(BuildingLevel.Building, buildingLevel.Level) : title;

            if (goal == null)
            {
            if (buildingLevel.Level == 1)
            {
                Goal = String.Format("Build {0}", buildingLevel.Building.Name);
            }
            else
            {
                Goal = String.Format("Upgrade {0} To Level {1}", buildingLevel.Building.Name, buildingLevel.Level);
            }
            }
            else
            {
                Goal = goal;
            }
            Descriptions = new Dictionary<int, string>(2);
            Descriptions.Add(0, String.Format("<img src='{0}' style='float: left;padding: 2px; height: 105px;' />{2}To upgrade your {1}, close this window, then click on it from your village overview and upgrade it.",
                GetBuildingImage(BuildingLevel.Building), buildingLevel.Building.Name, customDescription));
            Descriptions.Add(1, String.Format("<img src='{0}' style='float: left;padding: 2px; height: 105px;' />{2}To upgrade your {1}, close this window, then click on it from your village overview and upgrade it.",
                GetBuildingImage(BuildingLevel.Building), buildingLevel.Building.Name, customDescription));
            Tag = tag;
        }


        /// <summary>
        /// used only as a helper for quests. 
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public static string GetBuildingImage(Fbg.Bll.BuildingType bt)
        {
            switch (bt.ID) {
                case Fbg.Bll.CONSTS.BuildingIDs.VillageHQ:
                    return "https://static.realmofempires.com/images/misc/hq.png";
                case Fbg.Bll.CONSTS.BuildingIDs.Barracks:
                    return "https://static.realmofempires.com/images/misc/barracks.png";
                case Fbg.Bll.CONSTS.BuildingIDs.CoinMine:
                    return "https://static.realmofempires.com/images/misc/mine.png";
                case Fbg.Bll.CONSTS.BuildingIDs.Palace:
                    return "https://static.realmofempires.com/images/misc/palace.png";
                case Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop:
                    return "https://static.realmofempires.com/images/misc/seige.png";
                case Fbg.Bll.CONSTS.BuildingIDs.Stable:
                    return "https://static.realmofempires.com/images/misc/stable.png";
                case Fbg.Bll.CONSTS.BuildingIDs.TradePost:
                    return "https://static.realmofempires.com/images/misc/trade.png";
                case Fbg.Bll.CONSTS.BuildingIDs.Treasury:
                    return "https://static.realmofempires.com/images/misc/treasury.png";
                case Fbg.Bll.CONSTS.BuildingIDs.Wall:
                    return "https://static.realmofempires.com/images/misc/wall.png";
                case Fbg.Bll.CONSTS.BuildingIDs.Tavern:
                    return "https://static.realmofempires.com/images/misc/tavern.png";
                case Fbg.Bll.CONSTS.BuildingIDs.Farmland:
                    return "https://static.realmofempires.com/images/misc/farm.png";
                case Fbg.Bll.CONSTS.BuildingIDs.DefenseTower:
                    return "https://static.realmofempires.com/images/misc/tower.png";
                case Fbg.Bll.CONSTS.BuildingIDs.HidingSpot:
                    return "https://static.realmofempires.com/images/BuildingIcons/m_HidingSpot.png";
                default:
                    throw new Exception("Unrecognized bt.ID=" + bt.ID.ToString());

            }

        }


        /// <summary>
        /// used only as a helper for quests. 
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public static string GetTitle(Fbg.Bll.BuildingType bt, int level)
        {
            switch (bt.ID) {
                case Fbg.Bll.CONSTS.BuildingIDs.VillageHQ:
                    return "Upgrade Buildings Faster";
                case Fbg.Bll.CONSTS.BuildingIDs.Barracks:
                    return level == 1 ? "Build Barracks" : "Expand Barracks";
                case Fbg.Bll.CONSTS.BuildingIDs.CoinMine:
                    return "Enlarge Your Mine";
                case Fbg.Bll.CONSTS.BuildingIDs.Palace:
                    return "Build Your Palace";
                case Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop:
                    return "Expand Workshop";
                case Fbg.Bll.CONSTS.BuildingIDs.Stable:
                    return level == 1 ? "Build Stables" : "Expand Stables";
                case Fbg.Bll.CONSTS.BuildingIDs.TradePost:
                    return "Share The Wealth";
                case Fbg.Bll.CONSTS.BuildingIDs.Treasury:
                    return "Store More Silver";
                case Fbg.Bll.CONSTS.BuildingIDs.Wall:
                    return "Strengthen Your Wall";
                case Fbg.Bll.CONSTS.BuildingIDs.Tavern:
                    return level == 1 ? "Build Tavern" : "Expand Tavern";
                case Fbg.Bll.CONSTS.BuildingIDs.Farmland:
                    return "Expand Farm Land";
                //case Fbg.Bll.CONSTS.BuildingIDs.DefenseTower:
                //    return "https://static.realmofempires.com/images/misc/tower.png";
                case Fbg.Bll.CONSTS.BuildingIDs.HidingSpot:
                    return "Hide Some Silver";
                default:
                    throw new Exception("Unrecognized bt.ID=" + bt.ID.ToString());

            }

        }
    }





}
