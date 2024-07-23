using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using Fbg.Bll;
using System.Collections.Generic;

/// <summary>
/// Summary description for MiscHtmlHelper
/// </summary>
public class MiscHtmlHelper
{
    private  MiscHtmlHelper()
    {
        //
        // TODO: Add constructor logic here
        //
    }



    public static void SpeedUpDropDownCreate(ref Panel pnl
        , ref UpgradeEvent_CurrentlyUpgrading currentlyUpgradingUpgrade
        , CommandEventHandler handler
        , ref TimeSpan timeLeft
        , int minutesOfSpeedupsAllowed)
    {
        Panel pnl2;
        HyperLink h;
        LinkButton button;
        int maxCost=0;
        bool allowFinishNow =true;
        
        h = new HyperLink();
        h.CssClass = "jsMaster jsTriger";
        h.Text = "Speed Up!";
        h.Attributes.Add("style", "background: url('https://static.realmofempires.com/images/downarrow.png') top right no-repeat; padding: 0px 15px 0px 0px;");

        pnl.Controls.Add(h);

        pnl2 = new Panel();
        pnl2.CssClass = "jsOptions ui_menu";
        pnl2.Style.Add(HtmlTextWriterStyle.TextAlign, "left");
        pnl2.Attributes.Add("Style", "border: 1px solid rgb(30,30,30); background-color: rgb(75, 61, 48);padding:4px;");

        pnl.Controls.Add(pnl2);

        List<int> listOfTimeCuts;
        int finishNowCost = Fbg.Bll.Village.UpgradeSpeedUp_CalculateFinishNow(timeLeft.TotalMinutes, out listOfTimeCuts, minutesOfSpeedupsAllowed, ref allowFinishNow);

        if (finishNowCost > Village.CostOfTimeCut(1))
        {
            button = new LinkButton();
            button.ID = "1min";
            button.Text = "Cast Time Snip Spell - <B>cut 1 min</b> - 1 servant";
            button.CommandArgument = "1";
            button.CommandName = currentlyUpgradingUpgrade.ID.ToString();
            button.Command += new CommandEventHandler(handler);
            pnl2.Controls.Add(button);
            maxCost = Village.CostOfTimeCut(1);
        }


        if (timeLeft.TotalMinutes >= 5 && finishNowCost > Village.CostOfTimeCut(15))
        {
            button = new LinkButton();
            button.Text = "Cast Time Chop Spell - <B>cut 15 min</b> - 5 servants";
            button.ID = "15min";
            button.CommandArgument = "15";
            button.CommandName = currentlyUpgradingUpgrade.ID.ToString();
            button.Command += new CommandEventHandler(handler);
            pnl2.Controls.Add(button);
            maxCost = Village.CostOfTimeCut(15);
        }

        if (timeLeft.TotalMinutes >= 20 && finishNowCost > Village.CostOfTimeCut(60))
        {
            button = new LinkButton();
            button.ID = "60min";
            button.Text = "Cast Time Cut Spell - <B>cut 1 hour</b> - 10 servants";
            button.CommandArgument = "60";
            button.CommandName = currentlyUpgradingUpgrade.ID.ToString();
            button.Command += new CommandEventHandler(handler);
            pnl2.Controls.Add(button);
            maxCost = Village.CostOfTimeCut(60);
        }

        if (timeLeft.TotalMinutes > 140 && finishNowCost > Village.CostOfTimeCut(240))
        {
            button = new LinkButton();
            button.ID = "240min";
            button.Text = "Cast Time Slash Spell - <B>cut 4 hour</b> - 30 servants";
            button.CommandArgument = "240";
            button.CommandName = currentlyUpgradingUpgrade.ID.ToString();
            button.Command += new CommandEventHandler(handler);
            pnl2.Controls.Add(button);
            maxCost = Village.CostOfTimeCut(240);
        }

        if (allowFinishNow)
        {
            button = new LinkButton();
            button.ID = "now";
            button.Text = String.Format("Cast Time Stop Spell - <B>Finish Now!</b> - {0} servants", finishNowCost);
            button.CommandArgument = "9999";
            button.CommandName = currentlyUpgradingUpgrade.ID.ToString();
            button.Command += new CommandEventHandler(handler);
            pnl2.Controls.Add(button);
        }
    }



    public static void HandleBonusRows(BuildingType bt, Village village, HtmlTableRow bonusVillageRow, HtmlTableRow researchBonusRow, Label bonusVillageLbl, Label researchBonusLbl)
    {
        if (bonusVillageRow != null)
        {
            bonusVillageLbl.Text = String.Format("+ {0}%", village.VillageType.Bonus(bt) * 100);

            bonusVillageRow.Visible = village.VillageType.Bonus(bt) > 0; ;
        }
        if (researchBonusRow != null)
        {
            if (village.owner.Realm.Research.MaxBonus(bt) > 0)
            {
                researchBonusLbl.Text = String.Format("+ {0}%", village.owner.MyResearch.Bonus(bt) * 100);
            }
            else 
            {
                researchBonusLbl.Text = "Not active on this realm";
            }

            researchBonusRow.Visible = true;
        }

    }



    public static void HandleFixColumnAndMore(BuildingType bt, Village village, Label lblCurValue, HtmlTableRow trCurValue
         , Label lblRB, HtmlTableRow trRB, Label lblTotal, HtmlTableRow bonusVillageRow, Label bonusVillageLbl, bool addImproveColumn)
    {
        HandleFixColumnAndMore(bt, village, lblCurValue, trCurValue
         , lblRB, trRB, lblTotal, bonusVillageRow, bonusVillageLbl, null, null, false, addImproveColumn);
    }
    public static void HandleFixColumnAndMore(BuildingType bt, Village village, Label lblCurValue, HtmlTableRow trCurValue
         , Label lblRB, HtmlTableRow trRB, Label lblTotal, HtmlTableRow bonusVillageRow, Label bonusVillageLbl
        , Label lblPF, HtmlTableRow trPF, bool isPFActive, bool addImproveColumn)
    {
        //
        // value from level
        //
        HtmlTableCell valueCell = (HtmlTableCell)lblCurValue.Parent;
        valueCell.Style.Add(HtmlTextWriterStyle.BackgroundColor, red);
        valueCell.Style.Add(HtmlTextWriterStyle.Color, white);
        double value = village.GetBuildingLevelObject(bt.ID) == null ? 0.0 : Convert.ToDouble(village.GetBuildingLevelObject(bt.ID).Effect);
        double maxValue = Convert.ToDouble(bt.Levels[bt.MaxLevel - 1].Effect);
        if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.VillageHQ
            || bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Barracks
            || bt.ID == Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop
            || bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Stable
            || bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Tavern
            )
        {
            ColorCell(valueCell, 1 / value, 1 / maxValue, trCurValue, " Improve: Upgrade " + bt.Name, addImproveColumn);
        }
        else
        {
            ColorCell(valueCell, value, maxValue, trCurValue, " Improve: Upgrade " + bt.Name, addImproveColumn);
        }

        //
        // research
        //
        HtmlTableCell rbCell;
        float maxResearchBonus = 0.0F;
        float curResearchBonus = 0.0F;
        if (trRB != null)
        {
            rbCell = (HtmlTableCell)lblRB.Parent;
            maxResearchBonus = village.owner.Realm.Research.MaxBonus(bt);
            curResearchBonus = village.owner.MyResearch.Bonus(bt);
            if (maxResearchBonus > 0)
            {

                lblRB.Text = String.Format("+ {0}%", village.owner.MyResearch.Bonus(bt) * 100);
                ColorCell(rbCell, curResearchBonus, maxResearchBonus, trRB
                    , String.Format(" Improve: Do <a href='#' onclick='return popupResearch(\"?bid={0}\");return false;'>Research</a>", bt.ID), addImproveColumn);
                
            }
            else
            {
                lblRB.Text = "Not active on this realm";
                ColorCell(rbCell, 7, 8, trRB
                , "", addImproveColumn);
                

            }
        }
        //
        // bonus village
        //
        if (bonusVillageRow != null)
        {
            bonusVillageLbl.Text = String.Format("+ {0}%", village.VillageType.Bonus(bt) * 100);

            bonusVillageRow.Visible = village.VillageType.Bonus(bt) > 0; ;
        }
        //
        // PF 
        //
        if (lblPF != null)
        {
            ColorCell((HtmlTableCell)lblPF.Parent, isPFActive ? 1 : 0, isPFActive ? 1 : 0, trPF, "Improve: <a href='pfbenefits.aspx'>Activate this Feature</a>", addImproveColumn);
        }

        //
        // total
        //
        if (lblTotal != null)
        {
            //
            // color the total cell as green if all of the other params are green, yellow otherwise
            ColorCell((HtmlTableCell)lblTotal.Parent, 1
                , value == maxValue && curResearchBonus == maxResearchBonus ? 1 : 2
                , null, null, addImproveColumn);

        }
        //HtmlTableCell improveCell = new HtmlTableCell();
        //improveCell.InnerText = " Improve: Upgrade " + bt.Name;
        //trCurValue.Cells.Add(improveCell);
    }

    public static void ColorCell(HtmlTableCell cell, double bonus, double maxbonus, HtmlTableRow trCurValue, string msg, bool addImproveColumn)
    {

        HtmlTableCell improveCell = new HtmlTableCell();
        improveCell.Align = "left";
        improveCell.Style.Add(HtmlTextWriterStyle.PaddingLeft, "2px");
        improveCell.Style.Add(HtmlTextWriterStyle.FontWeight, "normal");

        if (bonus == 0)
        {
            cell.Style.Add(HtmlTextWriterStyle.BackgroundColor, red);
            cell.Style.Add(HtmlTextWriterStyle.Color, white);
            improveCell.Attributes.Add("class", "improveHelp");
            improveCell.InnerHtml = msg;
        }
        else if (bonus < maxbonus)
        {
            cell.Style.Add(HtmlTextWriterStyle.BackgroundColor, yellow);
            cell.Style.Add(HtmlTextWriterStyle.Color, black);
            improveCell.Attributes.Add("class", "improveHelp");
            improveCell.InnerHtml = msg;
        }
        else
        {
            cell.Style.Add(HtmlTextWriterStyle.BackgroundColor, green);
            cell.Style.Add(HtmlTextWriterStyle.Color, white);
            improveCell.InnerHtml = "<img  src='https://static.realmofempires.com/images/CheckMark_Quests.png' />";
        }
        if (trCurValue != null && addImproveColumn)
        {
            trCurValue.Cells.Add(improveCell);
        }

    }

    static readonly string green = "green";
    static readonly string yellow = "#FFD700";
    static readonly string red = "red";
    static readonly string black = "black";
    static readonly string white = "white";



    public static string PremiumFeaturePackageIcon(int packageID, bool isActive)
    {
        return Fbg.Bll.CONSTS.PremiumFeaturePackageIcon(packageID, isActive);
    }


    public static string PremiumFeaturePackageIcon_Large(int packageID, bool isActive)
    {
        return Fbg.Bll.CONSTS.PremiumFeaturePackageIcon_Large(packageID, isActive);
    }

    public static string GenerateBasicROEJS(Fbg.Bll.Player player, int currentlySelectedVID, bool isM, bool isD2, CONSTS.Device device, HttpRequest request, HttpContext context)
    {
        return String.Format(CONSTS.ROEScript3
            , player.ID
            , "false"
            , player.NumberOfVillages
            , Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(player.Realm)
            , Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(player.Realm)
            , player.Realm.ID
            , currentlySelectedVID
            , player.Name
            , player.Avatar.ImageUrlS
            , Config.InDev ? "true" : "false"
            , isM ? "true" : "false"                                               // 10 
            , (int)device // this relies on the fact that non-device value is 0 (aka false) , while all devices > 0 (aka true)
            , player.Realm.IsVPrealm ? "true" : "false"
            , LoginModeHelper.LoginMode(request).ToString()
            , player.Realm.BattleHandicap.IsActive ? "true" : "false"
            , player.Realm.UnitDesertionScalingFactor
            , player.Realm.UnitDesertionMinDistance
            , player.Realm.UnitDesertionMaxPopulation
            , player.Realm.BattleHandicap.Param_MaxHandicap
            , player.Realm.BattleHandicap.Param_StartRatio
            , player.Realm.BattleHandicap.Param_Steepness                            // 20 
            , Utils.UserLevel(context)
            , ""                                                                        // 22 empty
            , player.Realm.BonusVillChange ? "true" : "false"
            , player.GetRestartCost()
            , player.User.Credits                                                    // 25
            , player.Stewardship_IsLoggedInAsSteward ? "true" : "false"
            , player.Clan == null ? "" : String.Format("ROE.Player.Clan={{ id: {0}}};", player.Clan.ID) // this is needed mostly for the chat UI - it needs to know the clan so that it can init the right tab
            , Convert.ToInt32(Config.CollectAnalyticsOnThisRealm(player.Realm.ID))
            , player.Realm.LocalDBVersion
            , player.Realm.RealmType                                                 //30
            , Fbg.Bll.CONSTS.UnitIDs.Gov
            , Fbg.Bll.CONSTS.UnitIDs.Ram
            , Fbg.Bll.CONSTS.UnitIDs.Treb
            , Fbg.Bll.CONSTS.UnitIDs.Spy                                                //35
            , isD2 ? "true" : "false"    
            , player.User.ID
            );
    }

    public static string GenerateROEEntitiesJS(Fbg.Bll.Player _player)
    {
        System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        json_serializer.MaxJsonLength = CONSTS.MaxJsonLength;

        Dictionary<string, object> pfs = new Dictionary<string, object>(5);
        List<int> packageList = new List<int>(0);
        foreach (PFPackage p in _player.Realm.PFPackages_Object.FindAll(delegate(PFPackage p) { return p.Id == 22 || p.Id == 23 || p.Id == 24 || p.Id == 30 || p.Id == 32 || p.Id == 999 || p.Id == 1000; }))
        {
            pfs.Add(p.Id.ToString(), new
            {
                cost = p.Cost,
                duration = p.Duration,
                p.Id,
                name = Fbg.Bll.CONSTS.PF_NameForPackage(p.Id),
                desc = Fbg.Bll.CONSTS.PF_DescForPackage(p.Id),
                icon_NotActive = MiscHtmlHelper.PremiumFeaturePackageIcon(p.Id, false),
                icon_Active = MiscHtmlHelper.PremiumFeaturePackageIcon(p.Id, true),
                icon_NotActiveL = MiscHtmlHelper.PremiumFeaturePackageIcon_Large(p.Id, false),
                icon_ActiveL = MiscHtmlHelper.PremiumFeaturePackageIcon_Large(p.Id, true),


            });

            if (p.Id == 999)
            {
                packageList.Insert(0, p.Id); //we want NP first
            }
            else
            {
                packageList.Add(p.Id);
            }
        }

        foreach (Fbg.Common.Entities.BuildingType bt in _player.Realm.BuildingTypesEntities.Values)
        {
            bt.IconUrl = VillageOverviewImages.GetBuildingIconUrl(_player.Realm.BuildingType(bt.ID));
        }

        tempResearch research = Research(_player);

        var tempGifts = _player.Realm.Gifts.Select(Gift => new
        {
            id = Gift.Id,
            title = Gift.Title,
            availableImageUrl = Gift.AvailableImageUrl,
            requiredTitleID = Gift.RequiredTitle == null ? 0 : Gift.RequiredTitle.ID,
            type = (Gift is Gift_HourlySilverProd) ? "silver" : "troops",
            payout = (Gift is Gift_HourlySilverProd) ? ((Gift_HourlySilverProd)Gift).ProductionRewardMultiplierBase()
                : ((Gift_Troops)Gift).numOfTroopsBase(),
            unitTypeID = (Gift is Gift_HourlySilverProd) ? "" : ((Gift_Troops)Gift).unitType.ID.ToString()
        });

        return string.Format(@"ROE.Entities.UnitTypes={0};ROE.Entities.BuildingTypes={1};ROE.PFPckgs={2};ROE.PFPckgs.Order={3};ROE.Titles={4};ROE.Avatars={5};ROE.UpgradeSpeedUps_timeCuts={6};
                            ROE.UpgradeSpeedUps_timeCutCosts={7};ROE.Entities.Research.Items={8};
                            ROE.Entities.Research.BuildingsEffected={9};ROE.Entities.Research.UnitUnlocks={10};ROE.Entities.Research.MiscResearchGroups={18};
                            ROE.Entities.Items={11};ROE.MapIcons={12};ROE.Entities.Ages={13};ROE.Entities.Realm={14};ROE.Entities.UnitTypes.SortedList={15};ROE.Entities.VillageTypes={16};ROE.Entities.BuildingTypes.SortedList={17};
                            ROE.PlayerMoraleSettings={19}" // 18 is used. 
            , json_serializer.Serialize(_player.Realm.GetUnitTypesEntities())
            , json_serializer.Serialize(_player.Realm.BuildingTypesEntities)
            , json_serializer.Serialize(pfs)
            , json_serializer.Serialize(packageList)
            , json_serializer.Serialize(Titles(_player))
            , json_serializer.Serialize("DEPRECATED: please use ROE.Avatar.list")
            , json_serializer.Serialize(_player.Realm.IsVPrealm ? Village.timeCuts : new int[0] { })
            , json_serializer.Serialize(_player.Realm.IsVPrealm ? Village.timeCutCosts : new int[0] { })
            , json_serializer.Serialize(research.ResearchItems)
            , json_serializer.Serialize(research.buildingsEffected)
            , json_serializer.Serialize(research.unitUnlocks)
            , json_serializer.Serialize(tempGifts)
            , json_serializer.Serialize(_player.Realm.GetMapVillageIconUrls.ToDictionary(key => key.Key.ToString(), value => value.Value))
            , json_serializer.Serialize((_player.Realm.Age.isFeatureActive == false ? null : new { _player.Realm.Age }))
            , json_serializer.Serialize(new { _player.Realm.Tag, _player.Realm.ID, _player.Realm.Theme })
            , json_serializer.Serialize(_player.Realm.GetUnitTypesEntities().Keys.ToList())
            , json_serializer.Serialize(_player.Realm.VillageTypes.Types.ToDictionary(key => key.ID.ToString(), value => new { id = value.ID, value.IsBonus, value.Name, value.LargeIconUrl }))
            , json_serializer.Serialize(_player.Realm.BuildingTypesEntities.Keys.ToList())
            , json_serializer.Serialize(research.miscResearchGroups) //18
            , json_serializer.Serialize(_player.Realm.Morale.jsSettings) 
            );
    }



    private string NameForPackage(int id)
    {
        switch (id)
        {
            case 999:
                return "Nobility Package";
            case 1:
                return "Giant Map";
            case 22:
                return "Elven Efficiency";
            case 23:
                return "Bravery Spell";
            case 24:
                return "Blood Lust Spell";
            case 30:
                return "God Speed Spell";
            case 32:
                return "Rebel Rush Spell";
            default:
                return "";
        }
    }
    private string DescForPackage(int id)
    {
        switch (id)
        {
            case 999:
                return "";
            case 1:
                return "";
            case 22:
                return "(25% More Silver)";
            case 23:
                return "(20% Defense Bonus)";
            case 24:
                return "(20% Attack Bonus)";
            case 30:
                return "(Support Returns 10X Faster)";
            case 32:
                return "(Attack REBELS 20x faster)";
            default:
                return "";
        }
    }

    private struct tempResearch
    {
        public object ResearchItems;
        public object buildingsEffected;
        public object unitUnlocks;
        public object miscResearchGroups;

    }


    /// <summary>
    /// This function is an exact copy of API.ASPX "Resarch" function
    /// </summary>
    /// <returns></returns>
    private static tempResearch Research(Fbg.Bll.Player _player)
    {
        //
        // why is this generated each time? this shoudl be cached, by realmID
        // they shoudl be cached in FbgPlayer.Realm.Research like FbgPlayer.Realm.Research.AllResearchItems is 
        //
        Dictionary<string, tempRI> ris = new Dictionary<string, tempRI>(_player.Realm.Research.AllResearchItems.Count);
        //Dictionary<string, object> buildingEffects = new Dictionary<string, object>();
        //Dictionary<string, object> unitUnlocks = new Dictionary<string, object>();
        tempRI RI;
        List<ResearchItem> dep;


        object buildingEffects = from x in _player.Realm.Research.UniqueBuildingsEffectedByResearch
                                 select new
                                 {
                                     id = x.ID,
                                     maxBonus = _player.Realm.Research.MaxBonus(x),
                                     rilist = (from y in _player.Realm.Research.ResearchItemsForBuildingType(x) select y.ID)
                                 };

        object unitUnlocks = from x in _player.Realm.Research.UniqueUnitsUnlockedByResearch
                             select new
                             {
                                 id = x.ID,
                                 rilist = (from y in _player.Realm.Research.AllResearchItems.FindAll(delegate(ResearchItem ri) { return ri.UnlocksUnitTye == x; }) select y.ID)
                             };

        object miscResearchGroups = from x in _player.Realm.Research.MiscResearchGroups
                             select new
                             {
                                 id = x,
                                 rilist = (from y in _player.Realm.Research.AllResearchItems.FindAll(delegate(ResearchItem ri) { return ri.ResearchPropertyTypeID == x; }) select y.ID)
                             };


        foreach (ResearchItem ri in _player.Realm.Research.AllResearchItems)
        {
            try
            {
                RI = new tempRI();
                RI.name = ri.Name;
                RI.cost = ri.Cost;
                RI.time = Utils.FormatDuration_Long2(ri.ResearchTime);
                RI.id = ri.ID;
                RI.image = ri.ImageUrl;
                RI.imageL = ri.ImageUrl2;
                RI.shX = ri.SpriteSheetLocX;
                RI.shY = ri.SpriteSheetLocY;


                if (ri.TypesOfResearchItem == ResearchItem.TypesOfResearchItems.BuildingEffect)
                {
                    RI.effectedBuildingTypeID = ri.Property_BuldingItEffects.ID;
                    RI.buildingPercBonus = ri.PropertyAsFloat;
                }
                else if (ri.TypesOfResearchItem == ResearchItem.TypesOfResearchItems.UnitUnlock)
                {
                    RI.unlocksUnitTypeID = ri.UnlocksUnitTye.ID;

                }
                else
                {
                    RI.propertyID = ri.ResearchPropertyTypeID;
                    RI.propertyValue = ri.PropertyAsFloat;
                }

                if (ri.Requirements.Count > 0)
                {
                    RI.parents = new List<int>();
                    foreach (ResearchItem ri2 in ri.Requirements)
                    {
                        RI.parents.Add(ri2.ID);
                    }
                }

                dep = _player.Realm.Research.DependentResearchItems(ri);
                if (dep.Count > 0)
                {
                    RI.children = new List<int>();
                    foreach (ResearchItem ri2 in dep)
                    {
                        RI.children.Add(ri2.ID);
                    }
                }


                ris.Add(RI.id.ToString(), RI);
            }
            catch (Exception e)
            {
                Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException bex = new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("error building research items", e);
                bex.AddAdditionalInformation("ri.Name", ri.Name);
                bex.AddAdditionalInformation("ri.ID", ri.ID);
                bex.AddAdditionalInformation("ri.Property", ri.Property);
                bex.AddAdditionalInformation("_player.ID", _player.ID);
                bex.AddAdditionalInformation("_player.Name", _player.Name);
                bex.AddAdditionalInformation("_player.Realm.ID", _player.Realm.ID);
                throw bex; 
            }
        }


        return new tempResearch() { ResearchItems = ris, buildingsEffected = buildingEffects, unitUnlocks = unitUnlocks, miscResearchGroups = miscResearchGroups };
    }

    private struct tempRI
    {
        public string name;
        public int cost;
        public string time;
        public int id;
        public string image;
        public string imageL;
        public int effectedBuildingTypeID;
        public float buildingPercBonus;
        public int unlocksUnitTypeID;
        public List<int> parents;
        public List<int> children;
        public int shX;
        public int shY;
        public int propertyID;
        public float propertyValue;

    }




    private static object Titles(Fbg.Bll.Player _player)
    {
        return from t in _player.Realm.Titles
               orderby t.Level
               select new
               {
                   id = t.ID,
                   level = t.Level,
                   name = t.TitleName(_player.Sex),
                   xp = t.XP
                   ,
                   unlockedRealmName = (from r in Realms.AllRealms
                                        where (r.IsOpen == Realm.RealmState.PreRegistration || r.IsOpen == Realm.RealmState.Running_OpenToAll)
                                           && r.RealmTitleEntryLimitations == t.Level
                                        select r.Name)
                   ,
                   unlockedGift = from g in _player.Realm.Gifts where g.RequiredTitle == t select new { title = g.Title, img = g.AvailableImageUrl }
               };
    }




}
