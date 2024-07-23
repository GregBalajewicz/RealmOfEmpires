using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fbg.Bll;
using Fbg.Bll.Items2;
using System.Dynamic;
using System.Data;


namespace Fbg.Bll.Api.Raids
{


    public class Raids
    {

        public enum RaidType
        {
            privateRaid,
            clanRaid,
            globalRaid
        }

        public enum RaidRarity
        {
            common,
            uncommon,
            rare,
            epic,
            legendary
        }

        public enum RaidRewardType
        {

            //unit rewards
            CitizenMillitia = 11,
            Infantry = 2,
            LightCavalry = 5,
            Knight = 6,
            Spy = 12,
            Ram = 7,
            Treb = 8,
            Governor = 10,

            //PFs
            ElvenEfficiencySpell = 22,
            BraverySpell = 23,
            BloodLustSpell = 24,
            GodSpeedSpell = 30,
            RebelRushSpell = 32,

            //speeduprewards
            SpeedUpResearch = 100,
            SpeedUpBuilding = 101,

            //silver
            SilverBag = 102,

            //servants for example
            Servants = 200

        }

        public class Raid
        {

            public int raidID { get; set; }
            public DateTime createdOn { get; set; }
            public int actByDuration { get; set; } //minutes raid stays up for, NULL for non expiration
            public DateTime expirationDate { get; set; }
            public double distance { get; set; }
            public RaidRarity raidRarity { get; set; }
            public int raidTemplateID { get; set; }

            public string name { get; set; }
            public string desc { get; set; }
            public string imageUrlBG { get; set; }
            public string imageUrlIcon { get; set; }
            public int casualtyPerc { get; set; }
            public int currentHealth { get; set; }
            public int maxHealth { get; set; }

            public int playerID { get; set; }
            public int clanRaidPlayerID { get; set; }


            public int size { get; set; } //raid system size (how many vills the raid is tuned for at instantiation)
            public int playerCount { get; set; } //number of people in raid, 1 for solo, X+ for clans and glboal
            public int playerVillageCount { get; set; } //how many vills player had at raid instantiation

            public bool acceptedReward { get; set; }

            //if false, its a partial instance from raids list, and info is missing
            public bool hasDetails { get; set; }

            public List<RaidResult> raidResults;
            public List<RaidReward> raidRewards;
            public List<RaidMovement> raidMovements;

            public RaidType raidType
            {
                get
                {
                    if (playerID > 0)
                    {
                        return RaidType.privateRaid;
                    }
                    else if (clanRaidPlayerID > 0)
                    {
                        return RaidType.clanRaid;
                    }
                    else
                    {
                        return RaidType.globalRaid;
                    }
                }
            }

            public double createdOnMilli //for JS consumption
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(createdOn);
                }
            }

            public double expirationDateMilli //for JS consumption
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(expirationDate);
                }
            }


            public int perspectivePlayerID; //set this at run time, to view raid from that player
            public int totalPlayerDamage //the total damage the perspective player has done to this raid
            {
                get
                {
                    int totalDamageDone = 0;
                    if (perspectivePlayerID != 0) {
                        foreach (RaidResult result in this.raidResults)
                        {
                            if (result.playerID == this.perspectivePlayerID)
                            {
                                totalDamageDone += result.damageHP;
                            }
                        }
                    }
                    
                    return totalDamageDone;
                }
            }



            public Raid(DataRow raidDataRow)
            {
                this.raidID = (int)raidDataRow["RaidID"];
                this.createdOn = (DateTime)raidDataRow["CreatedOn"];
                this.actByDuration = raidDataRow["ActByDuration"] is DBNull ? 0 : (int)raidDataRow["ActByDuration"];
                this.expirationDate = raidDataRow["ExpirationDate"] is DBNull ? DateTime.MaxValue : (DateTime)raidDataRow["ExpirationDate"];
                this.distance = (double)raidDataRow["Distance"];
                this.raidRarity = (RaidRarity)raidDataRow["RarityID"];
                this.raidTemplateID = (int)raidDataRow["raidTemplateID"];
                this.playerID = raidDataRow["PlayerID"] is DBNull ? 0 : (int)raidDataRow["PlayerID"];
                this.clanRaidPlayerID = raidDataRow["ClanRaidPlayerID"] is DBNull ? 0 : (int)raidDataRow["ClanRaidPlayerID"];
                this.size = raidDataRow["Size"] is DBNull ? 0 : (int)raidDataRow["Size"];
                this.playerCount = (int)raidDataRow["PlayerCount"];
                this.playerVillageCount = raidDataRow["PlayerVillageCount"] is DBNull ? 0 : (int)raidDataRow["PlayerVillageCount"];
                this.name = (string)raidDataRow["Name"];
                this.desc = (string)raidDataRow["Desc"];
                this.imageUrlBG = (string)raidDataRow["ImageUrlBG"];
                this.imageUrlIcon = (string)raidDataRow["ImageUrlIcon"];
                this.casualtyPerc = (int)raidDataRow["CasultyPerc"];
                this.currentHealth = (int)raidDataRow["CurrentHealth"];
                this.maxHealth = (int)raidDataRow["MaxHealth"];
                this.raidResults = new List<RaidResult>();
                this.raidRewards = new List<RaidReward>();
                this.raidMovements = new List<RaidMovement>();
                this.acceptedReward = raidDataRow["AcceptedOn"] is DBNull ? false : true; //if info exists, its been accepted
                this.hasDetails = false;
            }

        }


        public class RaidMovement
        {

            public int eventID { get; set; }
            public int playerID { get; set; }
            public int raidID { get; set; }

            public int originVillageID { get; set; }
            public string villageName { get; set; }
            public int villageXCord { get; set; }
            public int villageYCord { get; set; }

            public DateTime startTime { get; set; }
            public DateTime landTime { get; set; }


            public double startTimeMilli //for JS consumption
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(startTime);
                }
            }
            public double landTimeMilli //for JS consumption
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(landTime);
                }
            }

            public class unit
            {
                public int utID;
                public int count;
            }

            public List<unit> unitsList { get; set; }

            public RaidMovement(DataRow raidMovementDataRow)
            {

                this.eventID = Convert.ToInt32(raidMovementDataRow["EventID"]);
                this.playerID = (int)raidMovementDataRow["PlayerID"];
                this.originVillageID = (int)raidMovementDataRow["OriginVillageID"];
                this.raidID = (int)raidMovementDataRow["RaidID"];

                this.startTime = (DateTime)raidMovementDataRow["StartTime"];
                this.landTime = (DateTime)raidMovementDataRow["LandTime"];

                this.villageName = (string)raidMovementDataRow["VillageName"];
                this.villageXCord = (int)raidMovementDataRow["VillageXCord"];
                this.villageYCord = (int)raidMovementDataRow["VillageYCord"];
                this.unitsList = new List<unit>();

            }



        }

        public class RaidResult {

            public int playerID { get; set; }
            public DateTime landTime { get; set; }
            public int damageHP { get; set; }
            public string playerName { get; set; }

            public double landTimeMilli //for JS consumption
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(landTime);
                }
            }

            public RaidResult(DataRow resultDataRow) {
                this.playerID = (int)resultDataRow["PlayerID"];
                this.landTime = (DateTime)resultDataRow["LandTime"];
                this.damageHP = (int)resultDataRow["DamageHP"];
                this.playerName = (string)resultDataRow["PlayerName"];
            }

        }

        public class RaidReward
        {
            public int raidTemplateID { get; set; }
            public RaidRewardType typeID { get; set; }
            public int count { get; set; }
            public string label { get; set; }
            public string icon { get; set; }

            public RaidReward(DataRow rewardDataRow, Fbg.Bll.Player player)
            {
                this.raidTemplateID = (int)rewardDataRow["RaidTemplateID"];
                this.typeID = (RaidRewardType)rewardDataRow["TypeID"];
                this.count = (int)rewardDataRow["Count"];

                //here we can add some custom info based on reward type
                switch (typeID) {

                    //Silver
                    case RaidRewardType.SilverBag:
                        this.label = "Silver";
                        this.icon = "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png";
                        break;

                    case RaidRewardType.RebelRushSpell:
                    case RaidRewardType.BloodLustSpell:
                    case RaidRewardType.BraverySpell:
                    case RaidRewardType.GodSpeedSpell:
                    case RaidRewardType.ElvenEfficiencySpell:
                        this.label = " Minute " + Fbg.Bll.CONSTS.PF_NameForPackage((int)typeID);
                        this.icon = Fbg.Bll.CONSTS.PremiumFeaturePackageIcon_Large((int)typeID, true);
                        break;

                    //Units Section
                    case RaidRewardType.CitizenMillitia:
                    case RaidRewardType.Governor:
                    case RaidRewardType.Infantry:
                    case RaidRewardType.Knight:
                    case RaidRewardType.LightCavalry:
                    case RaidRewardType.Ram:
                    case RaidRewardType.Treb:
                    case RaidRewardType.Spy:
                        Fbg.Bll.UnitType ut = player.Realm.GetUnitTypesByID((int)typeID);
                        this.label = ut.Name;
                        this.icon = ut.IconUrl_ThemeM;
                        break;

                    //SpeedUps
                    case RaidRewardType.SpeedUpBuilding:
                        this.label = "Minute Building Speedup";
                        this.icon = "https://static.realmofempires.com/images/icons/Q_Upgrade2.png";
                        break;
                    case RaidRewardType.SpeedUpResearch:
                        this.label = "Minute Research Speedup";
                        this.icon = "https://static.realmofempires.com/images/icons/M_ResearchList.png";
                        break;

                    //Servants
                    case RaidRewardType.Servants:
                        this.label = "Servants";
                        this.icon = "https://static.realmofempires.com/images/icons/servantCarry_m.png";
                        break;

                    default:
                        this.label = "unkown reward ID";
                        this.icon = "";
                        break;
                }
            }
        }

        /// <summary>
        /// Given an FBG Player, returns all of their private raids, clan riads, and all global raids
        /// Note: makes a DB call
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static List<Raid> GetPlayerRaids(Fbg.Bll.Player player)
        {
            DataSet playerRaidsAndResultsDataSet = DAL.Players.GetPlayerRaids(player.Realm.ConnectionStr, player.ID);
            return convertRaidsTableToRaidsList(playerRaidsAndResultsDataSet);
        }

        /// <summary>
        /// Converts a raids table a list of player's raids
        /// </summary>
        /// <param name="PlayerMapEventsTable"></param>
        /// <returns></returns>
        private static List<Raid> convertRaidsTableToRaidsList(DataSet playerRaidsDataSet)
        {
            List<Raid> playerRaidsList = new List<Raid>();
            DataTable PlayerRaidsTable = playerRaidsDataSet.Tables[0];
            playerRaidsList = (from DataRow row in PlayerRaidsTable.Rows select new Raid(row)).ToList();
            return playerRaidsList;
        }

        /// <summary>
        /// Creates a movements list given a specific raid's details data set
        /// </summary>
        /// <param name="raidDetailsDataSet"></param>
        /// <returns></returns>
        public static List<RaidMovement> createMovementsListFromRaidDetails(DataSet raidDetailsDataSet) {

            DataTable tableMovements = raidDetailsDataSet.Tables[1];
            DataTable tableMovementsUnits = raidDetailsDataSet.Tables[2];

            List<RaidMovement> playerRaidMovementsList = new List<RaidMovement>();

            //loop through the movements, for each one matching teh given raid
            //make new RaidMovement objects and add them to playerRaidMovementsList
            foreach (DataRow movementsDataRow in tableMovements.Rows)
            {

                RaidMovement raidMovement = new RaidMovement(movementsDataRow);

                //loop through movement unit details, if eventID matches the raidmovement above
                //add the unit object to raidmovement's unitMovementsList
                //we do it forloop style so we can remove rows as they get used (so inner loop gets shorter each cycle)
                DataRow movementUnitsDataRow;
                for (int i = tableMovementsUnits.Rows.Count - 1; i >= 0; i--)
                {
                    movementUnitsDataRow = tableMovementsUnits.Rows[i];
                    if (Convert.ToInt32(movementUnitsDataRow["EventID"]) == raidMovement.eventID)
                    {
                        RaidMovement.unit newMovementUnit = new RaidMovement.unit
                        {
                            utID = (int)movementUnitsDataRow["UnitTypeID"],
                            count = (int)movementUnitsDataRow["UnitCOunt"]
                        };

                        raidMovement.unitsList.Add(newMovementUnit);

                        tableMovementsUnits.Rows[i].Delete();
                    }
                }

                tableMovementsUnits.AcceptChanges();

                playerRaidMovementsList.Add(raidMovement);

            }

            return playerRaidMovementsList;

        }

        public static List<RaidResult> createRaidResultsListFromRaidDetails(DataSet raidDetailsDataSet)
        {

            DataTable tableRaidResults = raidDetailsDataSet.Tables[3];

            List<RaidResult> raidResultsList = new List<RaidResult>();

            raidResultsList = (from DataRow row in tableRaidResults.Rows select new RaidResult(row)).ToList();

            return raidResultsList;

        }


        public static List<RaidReward> createRaidRewardsListFromRaidDetails(Fbg.Bll.Player FbgPlayer, Raid raid, DataSet raidDetailsDataSet)
        {

            DataTable tableRaidRewards = raidDetailsDataSet.Tables[4];
            List<RaidReward> raidRewardsList = new List<RaidReward>();
            raidRewardsList = (from DataRow row in tableRaidRewards.Rows select new RaidReward(row, FbgPlayer)).ToList();
            return raidRewardsList;

        }



        public static Raid GetRaidDetails(Fbg.Bll.Player FbgPlayer, int raidID)
        {
            DataSet raidDetailsDataSet = DAL.Players.GetRaidDetails(FbgPlayer.Realm.ConnectionStr, FbgPlayer.ID, raidID);
            Raid raid = new Raid(raidDetailsDataSet.Tables[0].Rows[0]);
            raid.perspectivePlayerID = FbgPlayer.ID;
            raid.hasDetails = true;
            raid.raidMovements = createMovementsListFromRaidDetails(raidDetailsDataSet);
            raid.raidResults = createRaidResultsListFromRaidDetails(raidDetailsDataSet);
            raid.raidRewards = createRaidRewardsListFromRaidDetails(FbgPlayer, raid, raidDetailsDataSet);
            return raid;
        }


        


        /// <summary>
        /// send out a raid, get an updated movements list back
        /// </summary>
        /// <param name="FbgPlayer"></param>
        /// <param name="villageID"></param>
        /// <param name="raidID"></param>
        /// <param name="attackUnits"></param>
        /// <returns></returns>
        public static object SendRaid(Fbg.Bll.Player FbgPlayer, int villageID, int raidID, List<Fbg.Common.UnitCommand.Units> attackUnits)
        {


            //RESULT CODES
            //result 0 => success 
            //result 1 => village missing troop type issue (in iSend sp)
            //result 2 => attack unit issue
            //result 3 => raid not found
            //result 4 => raid already beaten
            //result 5 => raid expired

            //if somehow list of units being sent is empty
            if (attackUnits.Count < 1)
            {
                return new
                {
                    result = 2,
                }; ;
            }

            //first get the raid so we can verify some things and set some settings like distance and time
            //we can do those in the iSend SP if needed but this will do for now
            List<Raid> playerRaidList = GetPlayerRaids(FbgPlayer);
            Raid thisRaid = playerRaidList.Find(r => r.raidID == raidID);

            if (thisRaid == null)
            {
                return new
                {
                    result = 3,
                };
            }

            //if raid already beaten, return error 2
            if (thisRaid.currentHealth < 1)
            {
                return new
                {
                    result = 4,
                }; ;
            }

            //if raid is expired
            if (thisRaid.actByDuration > 0 && thisRaid.expirationDate.Ticks < DateTime.Now.Ticks)
            {
                return new
                {
                    result = 5,
                };
            }



            //find the slowest unit type in the attack list
            UnitType ut;
            UnitType slowestUnitType = null;
            foreach (Common.UnitCommand.Units u in attackUnits)
            {
                ut = FbgPlayer.Realm.GetUnitTypesByID(u.utID);
                if (slowestUnitType == null || slowestUnitType.Speed > ut.Speed)
                {
                    slowestUnitType = ut;
                }

            }

            //get distance, travel and land time
            double raidDistance = thisRaid.distance;
            TimeSpan travelDuration = new TimeSpan(Convert.ToInt64(System.Math.Floor((raidDistance / slowestUnitType.Speed) * TimeSpan.TicksPerHour)));
            DateTime landTime = DateTime.Now.Add(travelDuration);


            //attempt to send a raid, get result code back
            int result = DAL.Players.SendRaid(FbgPlayer.Realm.ConnectionStr, FbgPlayer.ID, villageID, raidID, attackUnits, landTime);

            //get updated latest changes
            Raid updatedRaid = GetRaidDetails(FbgPlayer, raidID);

            return new
            {
                result = result,
                raid = updatedRaid,
            };


        }


        public static bool AcceptReward(Fbg.Bll.Player FbgPlayer, int raidID)
        {


            Raid thisRaid = GetRaidDetails(FbgPlayer, raidID);

            if (thisRaid.currentHealth > 0) {
                return false; //cant accept reward if raid still active
            }

            bool rewardAcceptedOk;

            rewardAcceptedOk = DAL.Players.AcceptReward(FbgPlayer.Realm.ConnectionStr, FbgPlayer.ID, raidID);

            if (rewardAcceptedOk) {
                giveRaidRewardsToPlayer(FbgPlayer, thisRaid);
            }

            return rewardAcceptedOk;

        }



        public static void giveRaidRewardsToPlayer(Fbg.Bll.Player player, Raid raid) {


            List<RaidReward> raidRewardsList = raid.raidRewards;

            //for clan raids, we will need to do some interpretting based on raid data
           if (raid.raidType == RaidType.clanRaid)
            {

                //Size Factored Loot Split - Communal Loot Soft Cap - Split Pool method
                //refer to Raid Design google doc for more exploration -farhad
                int totalDamageDone = raid.totalPlayerDamage;

                float averagePlayerSize = (float)raid.size / (float)raid.playerCount;
                //a ratio of this player size vs average player size
                float inverseClampedSizeFactor = Math.Min(1.25f, Math.Max(0.75f, 1 / ((float)raid.playerVillageCount / averagePlayerSize)));

                //percantage of damage this player did
                float percentageOfDamageDone = Math.Min(1, (float)totalDamageDone / (float)raid.maxHealth);

                //dimishing factor for going over the soft cap
                float differenceDiminishFactor = 0.5f; 

                foreach (RaidReward reward in raidRewardsList)
                {

                    int rewardPool = reward.count * raid.playerCount;
                    int rewardPlayerShare = (int)Math.Ceiling(inverseClampedSizeFactor * percentageOfDamageDone * rewardPool);
                    
                    //the soft cap is basically the normal loot per player
                    int rewardPerPlayerSoftCap = reward.count;
                    int softCapOverage = Math.Max(0, rewardPlayerShare - rewardPerPlayerSoftCap);
                    int lootCount = 0;
                    if (rewardPlayerShare <= rewardPerPlayerSoftCap)
                    {
                        lootCount = rewardPlayerShare;
                    }
                    else
                    {
                        lootCount = rewardPerPlayerSoftCap + (int)Math.Ceiling(softCapOverage * differenceDiminishFactor);
                    }

                    reward.count = lootCount;

                }
               


                /*
                //Even split, Communal Loot Soft Cap - Split Pool method
                //refer to Raid Design google doc for more exploration -farhad


                int playerVillCount = raid.playerVillageCount;
                int playerCount = raid.playerCount;
                int totalDamageDone = raid.totalPlayerDamage;
                float percentageOfDamageDone = (float)totalDamageDone / (float)raid.maxHealth;

                float differenceFactor = 0.5f; //amount of over effort a player keeps

                RaidReward reward;
                foreach (DataRow rewardDataRow in tableRaidRewards.Rows)
                {
                    reward = new RaidReward(rewardDataRow, FbgPlayer);

                    int rewardPool = reward.count * playerCount;
                    int rewardPlayerShare = (int)Math.Ceiling(percentageOfDamageDone * rewardPool);
                    int rewardPerPlayerSoftCap = reward.count;
                    int splitDifference = Math.Max(0, rewardPlayerShare - rewardPerPlayerSoftCap);
                    int lootCount = 0;
                    if (rewardPlayerShare <= rewardPerPlayerSoftCap)
                    {
                        lootCount = rewardPlayerShare;
                    }
                    else
                    {
                        lootCount = rewardPerPlayerSoftCap + (int)Math.Ceiling(splitDifference * differenceFactor);
                    }

                    reward.count = lootCount;

                    raidRewardsList.Add(reward);

                }
                */


                /*
                 
                //Even split, Individual Loot Soft Cap - Split Pool method
                //refer to Raid Design google doc for more exploration -farhad

                int raidVillCount = raid.size;
                int playerVillCount = raid.playerVillageCount;
                float sizeFactor = (float)playerVillCount / (float)raidVillCount;

                int totalDamageDone = raid.totalPlayerDamage;
                float percentageOfDamageDone = (float)totalDamageDone / (float)raid.maxHealth;

                float differenceFactor = 0.5f; //amount of over effort a player keeps

                RaidReward reward;
                foreach (DataRow rewardDataRow in tableRaidRewards.Rows)
                {
                    reward = new RaidReward(rewardDataRow, FbgPlayer);

                    //Individual Loot Soft Cap - Split Pool method
                    //refer to Raid Design google doc for more exploration -farhad
                    int splitRaw = (int)Math.Ceiling(percentageOfDamageDone * reward.count);
                    int splitSoftCap = (int)(sizeFactor * reward.count);
                    int splitDifference = Math.Max(0, splitRaw - splitSoftCap);
                    int lootCount = 0;
                    if (splitRaw <= splitSoftCap)
                    {
                        lootCount = splitRaw;
                    }
                    else {
                        lootCount = splitSoftCap + (int)Math.Ceiling(splitDifference * differenceFactor);
                    }

                    reward.count = lootCount;

                    raidRewardsList.Add(reward);
                }
               */

            }




            foreach (RaidReward RR in raidRewardsList) {

                //here we can add some custom info based on reward type
                switch (RR.typeID)
                {

                    //Silver
                    case RaidRewardType.SilverBag:
                        player.Items2_Give(null, RR.count);
                        break;

                    case RaidRewardType.RebelRushSpell:
                    case RaidRewardType.BloodLustSpell:
                    case RaidRewardType.BraverySpell:
                    case RaidRewardType.GodSpeedSpell:
                    case RaidRewardType.ElvenEfficiencySpell:
                        player.Items2_Give(null, (int)RR.typeID, RR.count);
                        break;

                    //Units Section
                    case RaidRewardType.CitizenMillitia:
                    case RaidRewardType.Governor:
                    case RaidRewardType.Infantry:
                    case RaidRewardType.Knight:
                    case RaidRewardType.LightCavalry:
                    case RaidRewardType.Ram:
                    case RaidRewardType.Treb:
                    case RaidRewardType.Spy:
                        Fbg.Bll.UnitType ut = player.Realm.GetUnitTypesByID((int)RR.typeID);
                        player.Items2_Give(null, ut, RR.count);
                        break;

                    //SpeedUps
                    case RaidRewardType.SpeedUpBuilding:
                        player.Items2_Give_BuildingSpeedup(null, RR.count);
                        break;
                    case RaidRewardType.SpeedUpResearch:
                        player.Items2_Give_ResearchSpeedup(null, RR.count);
                        break;

                    //Servants
                    case RaidRewardType.Servants:
                        
                        //give servants code

                        break;
                    default:
                        //dowhat
                        break;
                }




            }





    }




        /* API SECTION */

        public static string Api_GetPlayerRaids(Fbg.Bll.Player FbgPlayer)
        {
            return ApiHelper.RETURN_SUCCESS(new
            {
                playerRaids = Fbg.Bll.Api.Raids.Raids.GetPlayerRaids(FbgPlayer)
            });

        }

        public static string Api_SendRaid(Fbg.Bll.Player FbgPlayer, int villageID, int raidID, List<Fbg.Common.UnitCommand.Units> attackUnits)
        {
            object sendResults = SendRaid(FbgPlayer, villageID, raidID, attackUnits);

            return ApiHelper.RETURN_SUCCESS(new
            {
                raidID = raidID,
                villageID = villageID,
                sendResults = sendResults
            });

        }

        /*
        public static string Api_GetRaidPlayerMovements(Fbg.Bll.Player FbgPlayer, int raidID)
        {
            return ApiHelper.RETURN_SUCCESS(new
            {
                raidID = raidID,
                movementsList = GetRaidPlayerMovementsList(FbgPlayer, raidID)
            });

        }
        */

        public static string Api_GetRaidDetails(Fbg.Bll.Player FbgPlayer, int raidID)
        {
            Raid raid = GetRaidDetails(FbgPlayer, raidID);
            return ApiHelper.RETURN_SUCCESS(new
            {
                raidID = raid.raidID,
                raid = raid
            });

        }

        

        public static string Api_AcceptRewards(Fbg.Bll.Player FbgPlayer, int raidID)
        {
            return ApiHelper.RETURN_SUCCESS(new
            {
                raidID = raidID,
                acceptResult = AcceptReward(FbgPlayer, raidID)
            });

        }

        

    }




}


