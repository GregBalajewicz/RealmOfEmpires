using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fbg.Bll;
using Fbg.Bll.Items2;
using System.Dynamic;
using System.Data;


namespace Fbg.Bll.Api.MapEvents
{
    /// <summary>
    /// Summary description for MapEvents
    /// </summary>
    public class MapEvents
    {


        public class PlayerMapEvent
        {
            public int eventID { get; set; }
            public int typeID { get; set; }
            public string data { get; set; }
            public int xCord { get; set; }
            public int yCord { get; set; }
            public DateTime addedOn { get; set; }
            public double addedOnMilli //for JS consumption
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(addedOn);
                }
            }
            public string stateData { get; set; }
            public string stateData2 { get; set; }
            public PlayerMapEvent(DataRow playerMapEventRow)
            {
                this.eventID = (int)playerMapEventRow["EventID"];
                this.typeID = (int)playerMapEventRow["TypeID"];
                this.data = (string)playerMapEventRow["Data"];
                this.addedOn = (DateTime)playerMapEventRow["AddedOn"];
                this.xCord = (int)playerMapEventRow["XCord"];
                this.yCord = (int)playerMapEventRow["YCord"];
                this.stateData = playerMapEventRow["StateData"] is DBNull ? "" : (string)playerMapEventRow["StateData"];
                this.stateData2 = playerMapEventRow["StateData2"] is DBNull ? "" : (string)playerMapEventRow["StateData2"];
            }
        }

        /// <summary>
        /// Given an FBG Player, returns all of their player map events as a list
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static List<PlayerMapEvent> GetPlayerMapEvents(Fbg.Bll.Player player)
        {
            DataTable PlayerMapEventsTable = DAL.Players.GetPlayerMapEvents(player.Realm.ConnectionStr, player.ID);
            return playerMapEventTableToList(PlayerMapEventsTable);
        }

        /// <summary>
        /// Converts a playerMapevents table to a list of PlayerMapEvent objects
        /// </summary>
        /// <param name="PlayerMapEventsTable"></param>
        /// <returns></returns>
        private static List<PlayerMapEvent> playerMapEventTableToList(DataTable PlayerMapEventsTable)
        {
            List<PlayerMapEvent> playerMapEventsList = new List<PlayerMapEvent>();
            playerMapEventsList = (from DataRow row in PlayerMapEventsTable.Rows select new PlayerMapEvent(row)).ToList();
            return playerMapEventsList;
        }


        public static string playerMapEventActivate(Fbg.Bll.Player player, int eventID, int typeID, int xCord, int yCord)
        {
            var result = playerMapEventActivateRaw(player, eventID, typeID, xCord, yCord);

            List<PlayerMapEvent> PlayerMapEventsList = GetPlayerMapEvents(player);

            return ApiHelper.RETURN_SUCCESS(new
            {
                eventID = eventID,
                typeID = typeID,
                xCord = xCord,
                yCord = yCord,
                result = result,
                MapEvents = PlayerMapEventsList //same map events list that the player object uses in player full/refresh
            }, new Fbg.Bll.Api.ApiHelper.Items2Converter());
        }


        public static object playerMapEventActivateRaw(Fbg.Bll.Player player, int eventID, int typeID, int xCord, int yCord)
        {

            /*
             * event types:
             * 1: Credit Farm
             * 2: TYPE2 has its own special activation
             */
            string dbresult = DAL.Players.MapEventsActivate(player.Realm.ConnectionStr, player.ID, eventID, typeID, xCord, yCord);

            if (typeID == 1)
            {
                var result = new
                {
                    msg = dbresult,
                };

                return result;
            }
            else if (typeID == 2)
            {
                var result = new
                {
                    msg = "deprecated.. howd you get here..."
                };
                return result;
            }
            else
            {
                return new
                {
                    msg = "wrong typeID"
                };
            }


        }




        /// <summary>
        /// Reveal 3 loot cards, checks first to make sure not revealed already
        /// </summary>
        public static string playerMapEventCaravanCardReveal(Fbg.Bll.Player player, string eventID, bool doReroll = false)
        {


            DataTable PlayerMapEventsTable = DAL.Players.GetPlayerMapEvents(player.Realm.ConnectionStr, player.ID);
            DataRow eventRow = PlayerMapEventsTable.Select("EventID = " + eventID).FirstOrDefault();
            if (eventRow == null)
            {
                return "error: no active event found. eventID: " + eventID;
            }

            if (Convert.ToInt32(eventRow["TypeID"]) != 2) {
                return "error: event type mismatch";
            }

            //here we parse event state data, such as collection totals, and levels etc
            string eventStateData = eventRow["StateData"] is DBNull ? "" : (string)eventRow["StateData"];
            string[] splitEventStateData = eventStateData.Split(',');

            //IF something was wrong with eventStateData data, use these defaults
            int collectedTotal = 0;
            int collectedThisLevel = 0;
            int level = 1;

            //if eventStateData is in good shape, use its values
            if (splitEventStateData.Length > 1)
            {
                collectedTotal = Convert.ToInt32(splitEventStateData[0]);
                collectedThisLevel = Convert.ToInt32(splitEventStateData[1]);
                level = Convert.ToInt32(splitEventStateData[2]);
            }

            //here we get specific event data like loot and reroll status
            string eventData = eventRow["Data"] is DBNull ? "" : (string)eventRow["Data"];
            string[] splitEventData = eventData.Split(',');

            string lootID1;
            string lootID2;
            string lootID3;
            int rerolled;

            //if eventStateData is in good shape
            if (splitEventData.Length > 1)
            {
                lootID1 = Convert.ToString(splitEventData[0]);
                lootID2 = Convert.ToString(splitEventData[1]);
                lootID3 = Convert.ToString(splitEventData[2]);
                rerolled = Convert.ToInt32(splitEventData[3]);
            }
            else
            {
                lootID1 = "x";
                lootID2 = "x";
                lootID3 = "x";
                rerolled = 0;
            }


            //here check for a valid reroll conditions
            int playerUserCredits = player.User.Credits;
            if (doReroll)
            {
                if (playerUserCredits < travellingCaravanRerollCost)
                {
                    return "error: not enough credits";
                }
                if (rerolled >= travellingCaravanMaxReroll)
                {
                    return "error: reroll limit";
                }
            }

            //collection stats based on realm open days
            int realmDaysOpen = _realmDaysOpen(player);
            int collectedTotalPossible = realmDaysOpen * travellingCaravanLimitDaily(player);
            int collectedTotalYours = collectedTotal;

            //In this section, we give new random loot, OR retrieve existing loot
            lootRewardDetailed loot1;
            lootRewardDetailed loot2;
            lootRewardDetailed loot3;

            if (doReroll)
            {
                lootID1 = "x";
                lootID2 = "x";
                lootID3 = "x";
            }

            //only if loot doesnt already exist do we need to insert new random loot
            if (lootID1 == "x")
            {

                //if doing a reroll, deduct a credit, and increment the reroll, but dont increment collection again
                if (doReroll)
                {
                    rerolled++;
                    player.User.UseCredits(travellingCaravanRerollCost, 33, player.Realm.ID);
                }
                else
                {
                    //In this section we increment collected stats, and level camp if needed
                    collectedTotal++;
                    collectedTotalYours = collectedTotal;
                    if (level < travellingCaravanMaxLevel)
                    {
                        collectedThisLevel++;
                        if (collectedThisLevel >= travellingCaravanMaxToLevelUp(player))
                        {
                            collectedThisLevel = 0;
                            level++;
                        }
                    }
                    else
                    {
                        level = travellingCaravanMaxLevel;
                    }
                }

                //now give out some random loot
                Random randomInstance = new Random();
                loot1 = getRandomLootCampReward(player, level, randomInstance);
                loot2 = getRandomLootCampReward(player, level, randomInstance);
                loot3 = getRandomLootCampReward(player, level, randomInstance);

                //put the data string together and update the event in db
                string newEventData = loot1.lootID + "," + loot2.lootID + "," + loot3.lootID + "," + rerolled;
                string newStateData = collectedTotal + "," + collectedThisLevel + "," + level;
                string newStateData2 = collectedTotal.ToString();
                eventRow = DAL.Players.MapEventsCaravanCardReveal(player.Realm.ConnectionStr, Convert.ToInt32(eventID), newEventData, newStateData, newStateData2);

            }
            else
            {
                //if lootID exists in state data, its been revealed before so, we get loot detail of the items
                List<lootRewardBase> baseRewardsList = getBaseRewardsList(player);
                loot1 = lootRewardAddDetails(player, baseRewardsList.Find(l => l.lootID == lootID1));
                loot2 = lootRewardAddDetails(player, baseRewardsList.Find(l => l.lootID == lootID2));
                loot3 = lootRewardAddDetails(player, baseRewardsList.Find(l => l.lootID == lootID3));
            }

            PlayerMapEvent newEvent = new PlayerMapEvent(eventRow);

            return ApiHelper.RETURN_SUCCESS(new
            {
                updatedEvent = newEvent,
                loot1 = loot1,
                loot2 = loot2,
                loot3 = loot3,
                credits = player.User.Credits,
                travellingCaravanMaxLevel = travellingCaravanMaxLevel,
                travellingCaravanMaxToLevelUp = travellingCaravanMaxToLevelUp(player),
                realmDaysOpen = realmDaysOpen,
                collectedTotalYours = collectedTotalYours,
                collectedTotalPossible = collectedTotalPossible,
                travellingCaravanCatchupCost = travellingCaravanCatchupCost,
                travellingCaravanCatchupSpawns = travellingCaravanCatchupSpawns
            });

        }


        public static int travellingCaravanLimitDaily(Fbg.Bll.Player player)
        {
            int realmLimit = player.Realm.travellingCaravanLimitDaily; //30 as dummy right now
            return realmLimit;
        }
        public static int travellingCaravanMaxToLevelUp(Fbg.Bll.Player player)
        {
            return travellingCaravanLimitDaily(player) * 7;
        }
        public const int travellingCaravanMaxLevel = 7;
        public const int travellingCaravanMaxReroll = 1;
        public const int travellingCaravanRerollCost = 1;
        public const int travellingCaravanCatchupCost = 2;
        public const int travellingCaravanCatchupSpawns = 10;



        /// <summary>
        /// award the picked loot card, update db and return all active player events
        /// </summary>
        public static string playerMapEventCaravanCardpick(Fbg.Bll.Player player, string eventID, int cardIndex)
        {

            DataTable PlayerMapEventsTable = DAL.Players.GetPlayerMapEvents(player.Realm.ConnectionStr, player.ID);

            DataRow eventRow = PlayerMapEventsTable.Select("EventID = " + eventID).FirstOrDefault();
            if (eventRow == null)
            {
                return "error: no active event found. eventID: " + eventID;
            }

            //get event state data
            string eventStateData = eventRow["StateData"] is DBNull ? "" : (string)eventRow["StateData"];
            string eventStateData2 = eventRow["StateData2"] is DBNull ? "" : (string)eventRow["StateData2"];
            string[] splitEventStateData = eventStateData.Split(',');
            int collectedTotal = Convert.ToInt32(splitEventStateData[0]);
            int collectedThisLevel = Convert.ToInt32(splitEventStateData[1]);
            int level = Convert.ToInt32(splitEventStateData[2]);

            //get event specific data
            string eventData = eventRow["Data"] is DBNull ? "" : (string)eventRow["Data"];
            string[] splitEventData = eventData.Split(',');
            string lootID1 = Convert.ToString(splitEventData[0]);
            string lootID2 = Convert.ToString(splitEventData[1]);
            string lootID3 = Convert.ToString(splitEventData[2]);
            int rerolled = Convert.ToInt32(splitEventData[3]);

            string pickedLootID;
            if (cardIndex == 1)
            {
                pickedLootID = lootID1;
            }
            else if (cardIndex == 2)
            {
                pickedLootID = lootID2;
            }
            else if (cardIndex == 3)
            {
                pickedLootID = lootID3;
            }
            else
            {
                //somehow an invalid card number was sent
                return "error: bad card index";
            }

            List<lootRewardBase> lootRewardBaseList = getBaseRewardsList(player);
            lootRewardDetailed pickedLoot = lootRewardAddDetails(player, lootRewardBaseList.Find(l => l.lootID == pickedLootID));
            if (pickedLoot == null)
            {
                //somehow pickedLootID wasnt valid, or wasnt found in BaseRewards list
                return "error: pickedLoot problem";
            }

            //actually gives out the reward
            awardItem(player, pickedLoot);

            //deactivate the event, and return all player map events
            PlayerMapEventsTable = DAL.Players.MapEventCardPicked(player.Realm.ConnectionStr, player.ID, Convert.ToInt32(eventID));

            return ApiHelper.RETURN_SUCCESS(new
            {
                eventID = eventID,
                MapEvents = playerMapEventTableToList(PlayerMapEventsTable), //same map events list that the player object uses in player full/refresh
                cardIndex = cardIndex,
                collectedTotal = collectedTotal,
                collectedThisLevel = collectedThisLevel,
                level = level,
                pickedLoot = pickedLoot,
                playerItems = player.Items2ItemGRoups,
                credits = player.User.Credits
            }, new Fbg.Bll.Api.ApiHelper.Items2Converter());

        }

        /// <summary>
        /// reshuffle the cards and get 3 new loot items.
        /// </summary>
        public static string playerMapEventCaravanCardReroll(Fbg.Bll.Player player, string eventID)
        {
            return playerMapEventCaravanCardReveal(player, eventID, true);
        }

        /// <summary>
        /// spawn more loot camps
        /// </summary>
        public static string playerMapEventCaravanCatchup(Fbg.Bll.Player player, string eventID)
        {

            //check to make sure enough credits
            int playerCredits = player.User.Credits;
            if (playerCredits < travellingCaravanCatchupCost)
            {
                return ApiHelper.RETURN_SUCCESS(new
                {
                    success = false,
                    credits = player.User.Credits
                });
            }

            //get event info from player map events table
            DataTable PlayerMapEventsTable = DAL.Players.GetPlayerMapEvents(player.Realm.ConnectionStr, player.ID);

            DataRow[] eventRows = PlayerMapEventsTable.Select("TypeID = " + 2);
            if (eventRows.Length > 1)
            {
                return "error: too many active events, shouldnt have been allowed to get here.";
            }

            DataRow eventRow = PlayerMapEventsTable.Select("EventID = " + eventID).FirstOrDefault();
            if (eventRow == null)
            {
                return "error: no active event found. eventID: " + eventID;
            }


            //here we parse event state data, such as collection totals, and levels etc
            string eventStateData = eventRow["StateData"] is DBNull ? "" : (string)eventRow["StateData"];
            string[] splitEventStateData = eventStateData.Split(',');
            if (splitEventStateData.Length < 1)
            {
                return "error: event state data faulty";
            }

            //figure out your total collects, and total possible max
            int realmDaysOpen = _realmDaysOpen(player);
            int collectedTotalPossible = realmDaysOpen * travellingCaravanLimitDaily(player);
            int collectedTotalYours = Convert.ToInt32(splitEventStateData[0]);

            //if still room for more catchups
            if (collectedTotalYours + travellingCaravanCatchupSpawns <= collectedTotalPossible)
            {
                //pay the cost
                player.User.UseCredits(travellingCaravanCatchupCost, 35, player.Realm.ID);

                //catchup (spawn the camps)
                DataTable PlayerMapEventsTableUpdate = DAL.Players.MapEventsCaravanCatchup(player.Realm.ConnectionStr, player.ID, travellingCaravanCatchupSpawns);

                return ApiHelper.RETURN_SUCCESS(new
                {
                    success = true,
                    credits = playerCredits - travellingCaravanCatchupCost,
                    MapEvents = playerMapEventTableToList(PlayerMapEventsTableUpdate)
                });
            }
            else
            {
                return "error: no catchup allowed";
            }

        }

        private static int _realmDaysOpen(Fbg.Bll.Player player)
        {
            return DateTime.Now.Date.Subtract(player.Realm.OpenOn.Date).Days + 1;
        }


        /// <summary>
        /// given a lootRewardBase, flesh out its details
        /// </summary>
        private static lootRewardDetailed lootRewardAddDetails(Fbg.Bll.Player player, lootRewardBase rewardedItemBase)
        {

            if (rewardedItemBase == null)
            {
                return null;
            }

            lootRewardDetailed detailedReward = new lootRewardDetailed(rewardedItemBase);

            if (rewardedItemBase.type == "unit")
            {
                Fbg.Bll.UnitType ut = player.Realm.GetUnitTypesByID(rewardedItemBase.typeID);
                detailedReward.name = ut.Name;
                detailedReward.icon = ut.IconUrl_ThemeM;
                detailedReward.countString = utils.FormatCost(rewardedItemBase.initCount);
            }
            else if (rewardedItemBase.type == "silver")
            {

                int silverAmount = player.Points; //based on player points so it grows a bit
                silverAmount = silverAmount % 1000 >= 500 ? silverAmount + 1000 - silverAmount % 1000 : silverAmount - silverAmount % 1000; //round to 1000th
                silverAmount = Math.Max(silverAmount, 500); //set minimum  silver
                silverAmount = Math.Min(silverAmount, 1000); //set max silver
                silverAmount = silverAmount * rewardedItemBase.initCount; //we use count as a post multiplier for silver
                detailedReward.name = "Bag of Silver";
                detailedReward.icon = "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png";
                detailedReward.count = silverAmount;
                detailedReward.countString = utils.FormatCost(silverAmount);

            }
            else if (rewardedItemBase.type == "pf")
            {
                detailedReward.name = Fbg.Bll.CONSTS.PF_NameForPackage(rewardedItemBase.typeID);
                detailedReward.icon = Fbg.Bll.CONSTS.PremiumFeaturePackageIcon_Large(rewardedItemBase.typeID, true);
                detailedReward.countString = minuteFormatter(rewardedItemBase.initCount);
            }
            else if (rewardedItemBase.type == "buildingspeedup")
            {
                detailedReward.name = "Building Speedup";
                detailedReward.icon = "https://static.realmofempires.com/images/icons/Q_Upgrade2.png";
                detailedReward.countString = minuteFormatter(rewardedItemBase.initCount);
            }
            else if (rewardedItemBase.type == "researchspeedup")
            {
                detailedReward.name = "Research Speedup";
                detailedReward.icon = "https://static.realmofempires.com/images/icons/M_ResearchList.png";
                detailedReward.countString = minuteFormatter(rewardedItemBase.initCount);
            }

            return detailedReward;

        }

        private static string minuteFormatter(int minutes)
        {
            string output = "";

            if (minutes < 60)
            {
                output = Math.Round(Convert.ToDouble(minutes), 1) + " m";
            }
            else
            {
                output = Math.Round(Convert.ToDouble(minutes) / 60, 1) + " hr";
            }

            return output;
        }

        /// <summary>
        /// pick a random lootRewardBase from list of loots
        /// </summary>
        private static lootRewardDetailed getRandomLootCampReward(Fbg.Bll.Player player, int campLevel, Random randomInstance)
        {

            //pick which reward list based on realmID
            List<lootRewardBase> filteredRewards = getBaseRewardsList(player);

            //filter based on camp level
            filteredRewards = filteredRewards.Where(loot => loot.campLevel == campLevel).ToList();

            int rewardsLength = filteredRewards.Count;

            double diceRoll = randomInstance.NextDouble();

            double sumOfProbs = 0.0;
            for (int i = 0; i < filteredRewards.Count; i++)
            {
                sumOfProbs += filteredRewards[i].prob;
            }

            //normalize the cumilitive range
            double cumulativeRange = randomInstance.NextDouble() * sumOfProbs;

            while (sumOfProbs > cumulativeRange)
            {
                sumOfProbs -= filteredRewards[rewardsLength - 1].prob;
                rewardsLength--;
            }

            lootRewardBase rewardedItem = filteredRewards[rewardsLength];

            //flesh out the reward item with specific details
            lootRewardDetailed rewardedItemDetailed = lootRewardAddDetails(player, rewardedItem);

            return rewardedItemDetailed;

        }


        public static void awardItem(Fbg.Bll.Player player, lootRewardDetailed itemToAward)
        {
            if (itemToAward.type == "unit")
            {
                Fbg.Bll.UnitType ut = player.Realm.GetUnitTypesByID(itemToAward.typeID);
                player.Items2_Give(null, ut, itemToAward.count);
            }
            else if (itemToAward.type == "silver")
            {
                player.Items2_Give(null, itemToAward.count);
            }
            else if (itemToAward.type == "pf")
            {
                player.Items2_Give(null, itemToAward.typeID, itemToAward.count);
            }

            else if (itemToAward.type == "buildingspeedup")
            {
                player.Items2_Give_BuildingSpeedup(null, itemToAward.count);
            }

            else if (itemToAward.type == "researchspeedup")
            {
                player.Items2_Give_ResearchSpeedup(null, itemToAward.count);
            }

        }


        /// <summary>
        /// Loot reward template
        /// </summary>
        public class lootRewardBase
        {
            public string lootID; //loot reward id
            public double prob; //probability of selection
            public string type; //type can be "unit" or "pf" etc
            public int typeID; //could be unit id or pfId
            public int initCount; //number / magnitude of items given out, varies based on item, usage in lootRewardAddDetails() 
            public int campLevel; //the level the loot appears in, no more no less
            public string trait; //rairty description of card: common, uncommon, rare, epic, legendary
        };

        /// <summary>
        /// Loot reward with added details from a loot reward template
        /// </summary>
        public class lootRewardDetailed
        {

            public string lootID; //loot reward id
            public string type; //type can be "unit" or "pf" etc
            public int typeID; //could be unit id or pfId
            public string trait; //rairty description of card: common, uncommon, rare, epic, legendary

            public string name; //name of reward
            public string icon; //icon url
            public int count; //final raw number
            public string countString; //final amount of stuff given out, formatted

            public lootRewardDetailed(lootRewardBase rewardBase)
            {
                this.lootID = rewardBase.lootID;
                this.type = rewardBase.type;
                this.typeID = rewardBase.typeID;
                this.count = rewardBase.initCount;
                this.trait = rewardBase.trait;
            }

        }

        public static List<lootRewardBase> getBaseRewardsList(Fbg.Bll.Player player)
        {

            List<lootRewardBase> baseRewardsList;

            if (player.Realm.IsVPrealm)
            {

                if (player.Realm.travellingCaravanLimitDaily < 30)
                {
                    baseRewardsList = BaseRewards3.ToList();
                }
                else if (player.Realm.ID >= 100)
                {
                    baseRewardsList = BaseRewards2.ToList();
                }
                else
                {
                    baseRewardsList = BaseRewards.ToList();
                }

            }
            else {
                baseRewardsList = BaseRewards_nonVIP1.ToList();
            }

            return baseRewardsList;
        }

        /// <summary>
        /// a list of basic rewards, details will have to be added by method: lootRewardAddDetails()
        /// NOTE: never add or modify list content at run time!!!
        /// </summary>
        public static List<lootRewardBase> BaseRewards = new List<lootRewardBase>{
       
            //Level 1
            //common
            new lootRewardBase { lootID = "L1_silverBag1",trait = "common", campLevel = 1, prob = 5, type = "silver", typeID = 0, initCount = 1},  //one count off silver can be 500 - 1000 around this level    
            new lootRewardBase { lootID = "L1_PF_silver1",trait = "common", campLevel = 1, prob = 2, type = "pf", typeID = 22, initCount = 15 },  //count in  minutes     
            new lootRewardBase { lootID = "L1_unit_CM1",trait = "common", campLevel = 1, prob = 5, type = "unit", typeID = 11, initCount = 1 },
            new lootRewardBase { lootID = "L1_unit_CM2",trait = "common", campLevel = 1, prob = 1, type = "unit", typeID = 11, initCount = 4 },
            new lootRewardBase { lootID = "L1_unit_IN1",trait = "common", campLevel = 1, prob = 1, type = "unit", typeID = 2, initCount = 2 },    
            //new lootRewardBase { lootID = "L1_speed_building1",trait = "common", campLevel = 1, prob = 1, type = "buildingspeedup", typeID = 0, initCount = 5 }, //count in minutes
            //new lootRewardBase { lootID = "L1_speed_research1",trait = "common", campLevel = 1, prob = 1, type = "researchspeedup", typeID = 0, initCount = 5 }, 
            //new lootRewardBase { lootID = "L1_unit_LC1",trait = "common", campLevel = 1, prob = .5, type = "unit", typeID = 5, initCount = 1 }, //took LC out, it might cause quest issues
            //uncommon
            //new lootRewardBase { lootID = "L1_speed_building2",trait = "common", campLevel = 1, prob = .25, type = "buildingspeedup", typeID = 0, initCount = 30 }, //count in minutes
            //new lootRewardBase { lootID = "L1_speed_research2",trait = "common", campLevel = 1, prob = .25, type = "researchspeedup", typeID = 0, initCount = 60 }, 
            //rare
            new lootRewardBase { lootID = "L1_silverBag2",trait = "rare", campLevel = 1, prob = .1, type = "silver", typeID = 0, initCount = 10},
            new lootRewardBase { lootID = "L1_unit_KN1",trait = "rare", campLevel = 1, prob = .1, type = "unit", typeID = 6, initCount = 1 },          


            //Level 2
            //common
            new lootRewardBase { lootID = "L2_silverBag1",trait = "common", campLevel = 2, prob = 2.3, type = "silver", typeID = 0, initCount = 5}, //one count is usually 1k silver from here on
            new lootRewardBase { lootID = "L2_PF_silver1",trait = "common", campLevel = 2, prob = 1.2, type = "pf", typeID = 22, initCount = 30 },
            new lootRewardBase { lootID = "L2_unit_CM1",trait = "common", campLevel = 2, prob = 3.5, type = "unit", typeID = 11, initCount = 6 },
            new lootRewardBase { lootID = "L2_unit_IN1",trait = "common", campLevel = 2, prob = 2, type = "unit", typeID = 2, initCount = 4 },
            new lootRewardBase { lootID = "L2_unit_LC1",trait = "common", campLevel = 2, prob = 1, type = "unit", typeID = 5, initCount = 2 },
            new lootRewardBase { lootID = "L2_unit_spy1",trait = "common", campLevel = 2, prob = .5, type = "unit", typeID = 12, initCount = 2 },   
            //new lootRewardBase { lootID = "L2_speed_building1",trait = "common", campLevel = 2, prob = 1, type = "buildingspeedup", typeID = 0, initCount = 5 }, //count in minutes
            //new lootRewardBase { lootID = "L2_speed_research1",trait = "common", campLevel = 2, prob = 1, type = "researchspeedup", typeID = 0, initCount = 15 }, 
            //uncommon
            //new lootRewardBase { lootID = "L2_speed_building2",trait = "common", campLevel = 2, prob = .25, type = "buildingspeedup", typeID = 0, initCount = 30 }, 
            //new lootRewardBase { lootID = "L2_speed_research2",trait = "common", campLevel = 2, prob = .25, type = "researchspeedup", typeID = 0, initCount = 120 }, 
            //rare
            new lootRewardBase { lootID = "L2_silverBag2",trait = "rare", campLevel = 2, prob = .1, type = "silver", typeID = 0, initCount = 30},
            new lootRewardBase { lootID = "L2_unit_KN1",trait = "rare", campLevel = 2, prob = .1, type = "unit", typeID = 6, initCount = 10 }, 
        

            //Level 3
            //common
            new lootRewardBase { lootID = "L3_silverBag1",trait = "common", campLevel = 3, prob = 2.3, type = "silver", typeID = 0, initCount = 8},
            new lootRewardBase { lootID = "L3_PF_silver1",trait = "common", campLevel = 3, prob = 1.1, type = "pf", typeID = 22, initCount = 30 },
            new lootRewardBase { lootID = "L3_unit_CM1",trait = "common", campLevel = 3, prob = 3, type = "unit", typeID = 11, initCount = 10 },
            new lootRewardBase { lootID = "L3_unit_IN1",trait = "common", campLevel = 3, prob = 2, type = "unit", typeID = 2, initCount = 5 },
            new lootRewardBase { lootID = "L3_unit_LC1",trait = "common", campLevel = 3, prob = 1, type = "unit", typeID = 5, initCount = 3 },
            new lootRewardBase { lootID = "L3_unit_spy1",trait = "common", campLevel = 3, prob = .5, type = "unit", typeID = 12, initCount = 3 },   
            //new lootRewardBase { lootID = "L3_speed_building1",trait = "common", campLevel = 3, prob = 1, type = "buildingspeedup", typeID = 0, initCount = 5 }, //count in minutes
            //new lootRewardBase { lootID = "L3_speed_research1",trait = "common", campLevel = 3, prob = 1, type = "researchspeedup", typeID = 0, initCount = 20 }, 
            //uncommon
            //new lootRewardBase { lootID = "L3_speed_building2",trait = "common", campLevel = 3, prob = .25, type = "buildingspeedup", typeID = 0, initCount = 45 },
            //new lootRewardBase { lootID = "L3_speed_research2",trait = "common", campLevel = 3, prob = .25, type = "researchspeedup", typeID = 0, initCount = 180 }, 
            //rare
            new lootRewardBase { lootID = "L3_silverBag2",trait = "rare", campLevel = 3, prob = .1, type = "silver", typeID = 0, initCount = 75},
            new lootRewardBase { lootID = "L3_unit_KN1",trait = "rare", campLevel = 3, prob = .1, type = "unit", typeID = 6, initCount = 20 }, 
   

            //Level 4
            //common
            new lootRewardBase { lootID = "L4_silverBag1",trait = "common", campLevel = 4, prob = 2.5, type = "silver", typeID = 0, initCount = 10},
            new lootRewardBase { lootID = "L4_PF_silver1",trait = "common", campLevel = 4, prob = 1, type = "pf", typeID = 22, initCount = 30 },
            new lootRewardBase { lootID = "L4_unit_IN1",trait = "common", campLevel = 4, prob = 1.5, type = "unit", typeID = 2, initCount = 10 },
            new lootRewardBase { lootID = "L4_unit_LC1",trait = "common", campLevel = 4, prob = 1, type = "unit", typeID = 5, initCount = 5 },
            new lootRewardBase { lootID = "L4_unit_spy1",trait = "common", campLevel = 4, prob = .5, type = "unit", typeID = 12, initCount = 4 },
            new lootRewardBase { lootID = "L4_unit_KN1",trait = "common", campLevel = 4, prob = 1, type = "unit", typeID = 6, initCount = 2 }, 
            //new lootRewardBase { lootID = "L4_speed_building1",trait = "common", campLevel = 4, prob = 1, type = "buildingspeedup", typeID = 0, initCount = 5 }, 
            //new lootRewardBase { lootID = "L4_speed_research1",trait = "common", campLevel = 4, prob = 1, type = "researchspeedup", typeID = 0, initCount = 30 }, 
            //uncommon
            //new lootRewardBase { lootID = "L4_speed_building2",trait = "common", campLevel = 4, prob = .25, type = "buildingspeedup", typeID = 0, initCount = 60 },
            //new lootRewardBase { lootID = "L4_speed_research2",trait = "common", campLevel = 4, prob = .25, type = "researchspeedup", typeID = 0, initCount = 240 }, 
            //rare
            new lootRewardBase { lootID = "L4_silverBag2",trait = "rare", campLevel = 4, prob = .1, type = "silver", typeID = 0, initCount = 150},
            new lootRewardBase { lootID = "L4_unit_KN2",trait = "rare", campLevel = 4, prob = .1, type = "unit", typeID = 6, initCount = 50 }, 


            //Level 5
            //common
            new lootRewardBase { lootID = "L5_silverBag1",trait = "common", campLevel = 5, prob = 2.5, type = "silver", typeID = 0, initCount = 20},
            new lootRewardBase { lootID = "L5_PF_silver1",trait = "common", campLevel = 5, prob = 1, type = "pf", typeID = 22, initCount = 45 },
            new lootRewardBase { lootID = "L5_unit_IN1",trait = "common", campLevel = 5, prob = 1.5, type = "unit", typeID = 2, initCount = 25 },
            new lootRewardBase { lootID = "L5_unit_LC1",trait = "common", campLevel = 5, prob = 1, type = "unit", typeID = 5, initCount = 10 },
            new lootRewardBase { lootID = "L5_unit_spy1",trait = "common", campLevel = 5, prob = .5, type = "unit", typeID = 12, initCount = 8 },
            new lootRewardBase { lootID = "L5_unit_KN1",trait = "common", campLevel = 5, prob = 1, type = "unit", typeID = 6, initCount = 5 },
            new lootRewardBase { lootID = "L5_unit_ram1",trait = "common", campLevel = 5, prob = .5, type = "unit", typeID = 7, initCount = 5 },
            new lootRewardBase { lootID = "L5_unit_treb1",trait = "common", campLevel = 5, prob = .5, type = "unit", typeID = 8, initCount = 5 }, 
            //new lootRewardBase { lootID = "L5_speed_building1",trait = "common", campLevel = 5, prob = 1, type = "buildingspeedup", typeID = 0, initCount = 10 }, 
            //new lootRewardBase { lootID = "L5_speed_research1",trait = "common", campLevel = 5, prob = 1, type = "researchspeedup", typeID = 0, initCount = 30 }, 
            //uncommon
            //new lootRewardBase { lootID = "L5_speed_building2",trait = "common", campLevel = 5, prob = .25, type = "buildingspeedup", typeID = 0, initCount = 60 },
            //new lootRewardBase { lootID = "L5_speed_research2",trait = "common", campLevel = 5, prob = .25, type = "researchspeedup", typeID = 0, initCount = 300 }, 
            //rare
            new lootRewardBase { lootID = "L5_silverBag2",trait = "rare", campLevel = 5, prob = .1, type = "silver", typeID = 0, initCount = 300},
            new lootRewardBase { lootID = "L5_unit_KN2",trait = "rare", campLevel = 5, prob = .1, type = "unit", typeID = 6, initCount = 100 },

            //Level 6
            //common
            new lootRewardBase { lootID = "L6_silverBag1",trait = "common", campLevel = 6, prob = 2.5, type = "silver", typeID = 0, initCount = 25},
            new lootRewardBase { lootID = "L6_PF_silver1",trait = "common", campLevel = 6, prob = 1, type = "pf", typeID = 22, initCount = 60 },
            new lootRewardBase { lootID = "L6_unit_IN1",trait = "common", campLevel = 6, prob = 1.5, type = "unit", typeID = 2, initCount = 35 },
            new lootRewardBase { lootID = "L6_unit_LC1",trait = "common", campLevel = 6, prob = 1, type = "unit", typeID = 5, initCount = 14 },
            new lootRewardBase { lootID = "L6_unit_spy1",trait = "common", campLevel = 6, prob = .5, type = "unit", typeID = 12, initCount = 10 },
            new lootRewardBase { lootID = "L6_unit_KN1",trait = "common", campLevel = 6, prob = 1, type = "unit", typeID = 6, initCount = 8 },
            new lootRewardBase { lootID = "L6_unit_ram1",trait = "common", campLevel = 6, prob = .5, type = "unit", typeID = 7, initCount = 8 },
            new lootRewardBase { lootID = "L6_unit_treb1",trait = "common", campLevel = 6, prob = .5, type = "unit", typeID = 8, initCount = 8 },
            new lootRewardBase { lootID = "L6_silverBag2",trait = "rare", campLevel = 6, prob = .1, type = "silver", typeID = 0, initCount = 500},
            new lootRewardBase { lootID = "L6_unit_KN2",trait = "rare", campLevel = 6, prob = .1, type = "unit", typeID = 6, initCount = 150 },

            //Level 7
            //common
            new lootRewardBase { lootID = "L7_silverBag1",trait = "common", campLevel = 7, prob = 5, initCount = 30, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_PF_silver1",trait = "common", campLevel = 7, prob = 2, initCount = 60, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L7_unit_IN1",trait = "common", campLevel = 7, prob = 3, initCount = 40, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L7_unit_LC1",trait = "common", campLevel = 7, prob = 2, initCount = 16, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L7_unit_spy1",trait = "common", campLevel = 7, prob = 1, initCount = 11, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L7_unit_KN1",trait = "common", campLevel = 7, prob = 2, initCount = 9, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L7_unit_ram1",trait = "common", campLevel = 7, prob = 1, initCount = 9, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L7_unit_treb1",trait = "common", campLevel = 7, prob = 1, initCount = 9, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L7_silverBag2",trait = "rare", campLevel = 7, prob = .1, initCount = 750, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_unit_KN2",trait = "rare", campLevel = 7, prob = .1, initCount = 175, type = "unit", typeID = 6 },

        };


        /// <summary>
        /// List V2: r100+ edition - has building and research speedups
        /// </summary>
        public static List<lootRewardBase> BaseRewards2 = new List<lootRewardBase>{

            //Level 1
            //common
            new lootRewardBase { lootID = "L1_silverBag1", trait = "common", campLevel = 1, prob = 5, initCount = 1, type = "silver", typeID = 0 },  //one count off silver can be 500 - 1000 around this level    
            new lootRewardBase { lootID = "L1_PF_silver1", trait = "common", campLevel = 1, prob = 2, initCount = 15, type = "pf", typeID = 22 },  //count in  minutes     
            new lootRewardBase { lootID = "L1_unit_CM1", trait = "common", campLevel = 1, prob = 5, initCount = 1, type = "unit", typeID = 11  },
            new lootRewardBase { lootID = "L1_unit_CM2", trait = "common", campLevel = 1, prob = 1, initCount = 4, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L1_unit_IN1", trait = "common", campLevel = 1, prob = 1, initCount = 2, type = "unit", typeID = 2  },
            new lootRewardBase { lootID = "L1_speed_building1", trait = "common", campLevel = 1, prob = 1, initCount = 5, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L1_speed_research1", trait = "common", campLevel = 1, prob = 1, initCount = 5, type = "researchspeedup", typeID = 0 }, 
            //new lootRewardBase { lootID = "L1_unit_LC1",trait = "common", campLevel = 1, prob = .5, type = "unit", typeID = 5, initCount = 1 }, //took LC out, it might cause quest issues
            //uncommon
            new lootRewardBase { lootID = "L1_speed_building2", trait = "uncommon", campLevel = 1, prob = .25, initCount = 15, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L1_speed_research2", trait = "uncommon", campLevel = 1, prob = .25, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L1_silverBag2", trait = "rare", campLevel = 1, prob = .1, initCount = 10, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L1_unit_KN1", trait = "rare", campLevel = 1, prob = .1, initCount = 2, type = "unit", typeID = 6 },          


            //Level 2
            //common
            new lootRewardBase { lootID = "L2_silverBag1",trait = "common", campLevel = 2, prob = 2.5, initCount = 5, type = "silver", typeID = 0}, //one count is usually 1k silver from here on
            new lootRewardBase { lootID = "L2_PF_silver1",trait = "common", campLevel = 2, prob = 1, initCount = 30, type = "pf", typeID = 22},
            new lootRewardBase { lootID = "L2_unit_CM1",trait = "common", campLevel = 2, prob = 3.5, initCount = 6, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L2_unit_IN1",trait = "common", campLevel = 2, prob = 2, initCount = 4, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L2_unit_LC1",trait = "common", campLevel = 2, prob = 1, initCount = 2, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L2_unit_spy1",trait = "common", campLevel = 2, prob = .5, initCount = 2, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L2_speed_building1",trait = "common", campLevel = 2, prob = 1, initCount = 5, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L2_speed_research1",trait = "common", campLevel = 2, prob = .5, initCount = 15, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L2_speed_building2",trait = "uncommon", campLevel = 2, prob = .25, initCount = 30, type = "buildingspeedup", typeID = 0},
            new lootRewardBase { lootID = "L2_speed_research2",trait = "uncommon", campLevel = 2, prob = .25, initCount = 60, type = "researchspeedup", typeID = 0}, 
            //rare
            new lootRewardBase { lootID = "L2_silverBag2",trait = "rare", campLevel = 2, prob = .1, initCount = 30, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L2_unit_KN1",trait = "rare", campLevel = 2, prob = .1, initCount = 10, type = "unit", typeID = 6 }, 
        

            //Level 3
            //common
            new lootRewardBase { lootID = "L3_silverBag1",trait = "common", campLevel = 3, prob = 2.5, initCount = 8, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L3_PF_silver1",trait = "common", campLevel = 3, prob = 1, initCount = 30, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L3_unit_CM1",trait = "common", campLevel = 3, prob = 3, initCount = 10, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L3_unit_IN1",trait = "common", campLevel = 3, prob = 2, initCount = 5, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L3_unit_LC1",trait = "common", campLevel = 3, prob = 1, initCount = 3, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L3_unit_spy1",trait = "common", campLevel = 3, prob = .5, initCount = 3, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L3_speed_building1",trait = "common", campLevel = 3, prob = 1, initCount = 5, type = "buildingspeedup", typeID = 0}, //count in minutes
            new lootRewardBase { lootID = "L3_speed_research1",trait = "common", campLevel = 3, prob = 1, initCount = 20, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L3_speed_building2",trait = "uncommon", campLevel = 3, prob = .25, initCount = 45, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L3_speed_research2",trait = "uncommon", campLevel = 3, prob = .25, initCount = 60, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L3_silverBag2",trait = "rare", campLevel = 3, prob = .1, initCount = 75, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L3_unit_KN1",trait = "rare", campLevel = 3, prob = .1, initCount = 20, type = "unit", typeID = 6 }, 
   

            //Level 4
            //common
            new lootRewardBase { lootID = "L4_silverBag1",trait = "common", campLevel = 4, prob = 2.5, initCount = 10, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L4_PF_silver1",trait = "common", campLevel = 4, prob = 1, initCount = 30, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L4_unit_IN1",trait = "common", campLevel = 4, prob = 1.5, initCount = 10, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L4_unit_LC1",trait = "common", campLevel = 4, prob = 1, initCount = 5, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L4_unit_spy1",trait = "common", campLevel = 4, prob = .5, initCount = 4, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L4_unit_KN1",trait = "common", campLevel = 4, prob = 1, initCount = 2, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L4_speed_building1",trait = "common", campLevel = 4, prob = 1, initCount = 5, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L4_speed_research1",trait = "common", campLevel = 4, prob = .5, initCount = 20, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L4_speed_building2",trait = "uncommon", campLevel = 4, prob = .25, initCount = 45, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L4_speed_research2",trait = "uncommon", campLevel = 4, prob = .25, initCount = 120, type = "researchspeedup", typeID = 0 },
            //rare
            new lootRewardBase { lootID = "L4_silverBag2",trait = "rare", campLevel = 4, prob = .1, initCount = 150, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L4_unit_KN2",trait = "rare", campLevel = 4, prob = .1, initCount = 50, type = "unit", typeID = 6 }, 


            //Level 5
            //common
            new lootRewardBase { lootID = "L5_silverBag1",trait = "common", campLevel = 5, prob = 3, initCount = 20, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L5_PF_silver1",trait = "common", campLevel = 5, prob = 1, initCount = 45, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L5_unit_IN1",trait = "common", campLevel = 5, prob = 1.5, initCount = 25, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L5_unit_LC1",trait = "common", campLevel = 5, prob = 1, initCount = 10, type = "unit", typeID = 5},
            new lootRewardBase { lootID = "L5_unit_spy1",trait = "common", campLevel = 5, prob = .5, initCount = 8, type = "unit", typeID = 12},
            new lootRewardBase { lootID = "L5_unit_KN1",trait = "common", campLevel = 5, prob = 1, initCount = 5, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L5_unit_ram1",trait = "common", campLevel = 5, prob = .5, initCount = 5, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L5_unit_treb1",trait = "common", campLevel = 5, prob = .5, initCount = 5, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L5_speed_building1",trait = "common", campLevel = 5, prob = 1, initCount = 10, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L5_speed_research1",trait = "common", campLevel = 5, prob = .5, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L5_speed_building2",trait = "uncommon", campLevel = 5, prob = .25, initCount = 60, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L5_speed_research2",trait = "uncommon", campLevel = 5, prob = .25, initCount = 120, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L5_silverBag2",trait = "rare", campLevel = 5, prob = .1, initCount = 300, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L5_unit_KN2",trait = "rare", campLevel = 5, prob = .1, initCount = 100, type = "unit", typeID = 6 },
            //epic
            new lootRewardBase { lootID = "L5_unit_KN3",trait = "epic", campLevel = 5, prob = .03, initCount = 500, type = "unit", typeID = 6 },

            //Level 6
            //common
            new lootRewardBase { lootID = "L6_silverBag1",trait = "common", campLevel = 6, prob = 5, initCount = 25, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L6_PF_silver1",trait = "common", campLevel = 6, prob = 2, initCount = 60, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L6_unit_IN1",trait = "common", campLevel = 6, prob = 3, initCount = 35, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L6_unit_LC1",trait = "common", campLevel = 6, prob = 2, initCount = 14, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L6_unit_spy1",trait = "common", campLevel = 6, prob = 1, initCount = 10, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L6_unit_KN1",trait = "common", campLevel = 6, prob = 2, initCount = 8, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L6_unit_ram1",trait = "common", campLevel = 6, prob = 1, initCount = 8, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L6_unit_treb1",trait = "common", campLevel = 6, prob = 1, initCount = 8, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L6_speed_building1",trait = "common", campLevel = 6, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L6_speed_research1",trait = "common", campLevel = 6, prob = 1, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L6_speed_building2",trait = "uncommon", campLevel = 6, prob = .25, initCount = 60, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L6_unit_IN2",trait = "uncommon", campLevel = 6, prob = .25, initCount = 75, type = "unit", typeID = 2 }, 
            //rare
            new lootRewardBase { lootID = "L6_speed_research2",trait = "rare", campLevel = 6, prob = .1, initCount = 300, type = "researchspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L6_silverBag2",trait = "rare", campLevel = 6, prob = .1, initCount = 500, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L6_unit_KN2",trait = "rare", campLevel = 6, prob = .1, initCount = 150, type = "unit", typeID = 6 }, 
            //epic
            new lootRewardBase { lootID = "L6_unit_KN3",trait = "epic", campLevel = 6, prob = .03, initCount = 750 , type = "unit", typeID = 6},
            new lootRewardBase { lootID = "L6_unit_GOV1",trait = "epic", campLevel = 6, prob = .03, initCount = 1, type = "unit", typeID = 10 },

            //Level 7
            //common
            new lootRewardBase { lootID = "L7_silverBag1",trait = "common", campLevel = 7, prob = 5, initCount = 30, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_PF_silver1",trait = "common", campLevel = 7, prob = 2, initCount = 60, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L7_unit_IN1",trait = "common", campLevel = 7, prob = 3, initCount = 40, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L7_unit_LC1",trait = "common", campLevel = 7, prob = 2, initCount = 16, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L7_unit_spy1",trait = "common", campLevel = 7, prob = 1, initCount = 11, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L7_unit_KN1",trait = "common", campLevel = 7, prob = 2, initCount = 9, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L7_unit_ram1",trait = "common", campLevel = 7, prob = 1, initCount = 9, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L7_unit_treb1",trait = "common", campLevel = 7, prob = 1, initCount = 9, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L7_speed_building1",trait = "common", campLevel = 7, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L7_speed_research1",trait = "common", campLevel = 7, prob = 1, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L7_speed_building2",trait = "uncommon", campLevel = 7, prob = .25, initCount = 60, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L7_unit_IN2",trait = "uncommon", campLevel = 7, prob = .25, initCount = 90, type = "unit", typeID = 2 },    
            //rare
            new lootRewardBase { lootID = "L7_speed_research2",trait = "rare", campLevel = 7, prob = .1, initCount = 300, type = "researchspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L7_silverBag2",trait = "rare", campLevel = 7, prob = .1, initCount = 750, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_unit_KN2",trait = "rare", campLevel = 7, prob = .1, initCount = 175, type = "unit", typeID = 6 }, 
            //epic
            new lootRewardBase { lootID = "L7_unit_KN3",trait = "epic", campLevel = 7, prob = .03, initCount = 800 , type = "unit", typeID = 6},
            new lootRewardBase { lootID = "L7_unit_GOV1",trait = "epic", campLevel = 7, prob = .03, initCount = 1, type = "unit", typeID = 10 }

        };


        /// <summary>
        /// List V3: Morale Realms edition: designed for 10 spawns a day
        /// https://docs.google.com/spreadsheets/d/1qxfJo-dWL0gZhkNxtalJh8nRUTPjTY02uheFjwKxaCQ/edit#gid=1763330971
        /// </summary>
        public static List<lootRewardBase> BaseRewards3 = new List<lootRewardBase>{

             //Level 1
            //common
            new lootRewardBase { lootID = "L1_silverBag1", trait = "common", campLevel = 1, prob = 5, initCount = 2, type = "silver", typeID = 0 },   
            new lootRewardBase { lootID = "L1_PF_silver1", trait = "common", campLevel = 1, prob = 2, initCount = 15, type = "pf", typeID = 22 },  //count in  minutes     
            new lootRewardBase { lootID = "L1_unit_CM1", trait = "common", campLevel = 1, prob = 5, initCount = 2, type = "unit", typeID = 11  },
            new lootRewardBase { lootID = "L1_unit_CM2", trait = "common", campLevel = 1, prob = 1, initCount = 8, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L1_unit_IN1", trait = "common", campLevel = 1, prob = 1, initCount = 4, type = "unit", typeID = 2  },
            new lootRewardBase { lootID = "L1_speed_building1", trait = "common", campLevel = 1, prob = 1, initCount = 5, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L1_speed_research1", trait = "common", campLevel = 1, prob = 1, initCount = 10, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L1_speed_building_uncommon", trait = "uncommon", campLevel = 1, prob = .25, initCount = 15, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L1_speed_research_uncommon", trait = "uncommon", campLevel = 1, prob = .25, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L1_silverBag_rare", trait = "rare", campLevel = 1, prob = .1, initCount = 20, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L1_unit_KN_rare", trait = "rare", campLevel = 1, prob = .1, initCount = 5, type = "unit", typeID = 6 },          


            //Level 2
            //common
            new lootRewardBase { lootID = "L2_silverBag1",trait = "common", campLevel = 2, prob = 2.5, initCount = 10, type = "silver", typeID = 0}, 
            new lootRewardBase { lootID = "L2_PF_silver1",trait = "common", campLevel = 2, prob = 1, initCount = 30, type = "pf", typeID = 22},
            new lootRewardBase { lootID = "L2_unit_CM1",trait = "common", campLevel = 2, prob = 3.5, initCount = 12, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L2_unit_IN1",trait = "common", campLevel = 2, prob = 2, initCount = 8, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L2_unit_LC1",trait = "common", campLevel = 2, prob = 1, initCount = 4, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L2_unit_spy1",trait = "common", campLevel = 2, prob = .5, initCount = 4, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L2_speed_building1",trait = "common", campLevel = 2, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L2_speed_research1",trait = "common", campLevel = 2, prob = .5, initCount = 20, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L2_speed_building_uncommon",trait = "uncommon", campLevel = 2, prob = .25, initCount = 30, type = "buildingspeedup", typeID = 0},
            new lootRewardBase { lootID = "L2_speed_research_uncommon",trait = "uncommon", campLevel = 2, prob = .25, initCount = 120, type = "researchspeedup", typeID = 0}, 
            //rare
            new lootRewardBase { lootID = "L2_silverBag_rare",trait = "rare", campLevel = 2, prob = .1, initCount = 50, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L2_unit_KN_rare",trait = "rare", campLevel = 2, prob = .1, initCount = 25, type = "unit", typeID = 6 }, 
        

            //Level 3
            //common
            new lootRewardBase { lootID = "L3_silverBag1",trait = "common", campLevel = 3, prob = 2.5, initCount = 16, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L3_PF_silver1",trait = "common", campLevel = 3, prob = 1, initCount = 45, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L3_unit_CM1",trait = "common", campLevel = 3, prob = 3, initCount = 20, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L3_unit_IN1",trait = "common", campLevel = 3, prob = 2, initCount = 10, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L3_unit_LC1",trait = "common", campLevel = 3, prob = 1, initCount = 6, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L3_unit_spy1",trait = "common", campLevel = 3, prob = .5, initCount = 6, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L3_speed_building1",trait = "common", campLevel = 3, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0}, //count in minutes
            new lootRewardBase { lootID = "L3_speed_research1",trait = "common", campLevel = 3, prob = .5, initCount = 20, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L3_speed_building_uncommon",trait = "uncommon", campLevel = 3, prob = .25, initCount = 30, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L3_speed_research_uncommon",trait = "uncommon", campLevel = 3, prob = .25, initCount = 180, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L3_silverBag_rare",trait = "rare", campLevel = 3, prob = .1, initCount = 150, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L3_unit_KN_rare",trait = "rare", campLevel = 3, prob = .1, initCount = 50, type = "unit", typeID = 6 }, 
   

            //Level 4
            //common
            new lootRewardBase { lootID = "L4_silverBag1",trait = "common", campLevel = 4, prob = 2.5, initCount = 20, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L4_PF_silver1",trait = "common", campLevel = 4, prob = 1, initCount = 60, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L4_unit_IN1",trait = "common", campLevel = 4, prob = 1.5, initCount = 20, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L4_unit_LC1",trait = "common", campLevel = 4, prob = 1, initCount = 10, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L4_unit_spy1",trait = "common", campLevel = 4, prob = .5, initCount = 8, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L4_unit_KN1",trait = "common", campLevel = 4, prob = 1, initCount = 5, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L4_speed_building1",trait = "common", campLevel = 4, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L4_speed_research1",trait = "common", campLevel = 4, prob = .5, initCount = 20, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L4_unit_IN_uncommon",trait = "uncommon", campLevel = 4, prob = .25, initCount = 50, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L4_speed_building_uncommon",trait = "uncommon", campLevel = 4, prob = .25, initCount = 45, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L4_speed_research_uncommon",trait = "uncommon", campLevel = 4, prob = .25, initCount = 180, type = "researchspeedup", typeID = 0 },
            //rare
            new lootRewardBase { lootID = "L4_silverBag_rare",trait = "rare", campLevel = 4, prob = .1, initCount = 300, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L4_unit_KN_rare",trait = "rare", campLevel = 4, prob = .1, initCount = 100, type = "unit", typeID = 6 }, 


            //Level 5
            //common
            new lootRewardBase { lootID = "L5_silverBag1",trait = "common", campLevel = 5, prob = 3, initCount = 40, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L5_PF_silver1",trait = "common", campLevel = 5, prob = 1, initCount = 60, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L5_unit_IN1",trait = "common", campLevel = 5, prob = 1.5, initCount = 40, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L5_unit_LC1",trait = "common", campLevel = 5, prob = 1, initCount = 15, type = "unit", typeID = 5},
            new lootRewardBase { lootID = "L5_unit_spy1",trait = "common", campLevel = 5, prob = .5, initCount = 12, type = "unit", typeID = 12},
            new lootRewardBase { lootID = "L5_unit_KN1",trait = "common", campLevel = 5, prob = 1, initCount = 8, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L5_unit_ram1",trait = "common", campLevel = 5, prob = .5, initCount = 8, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L5_unit_treb1",trait = "common", campLevel = 5, prob = .5, initCount = 8, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L5_speed_building1",trait = "common", campLevel = 5, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L5_speed_research1",trait = "common", campLevel = 5, prob = .5, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L5_silverBag_uncommon",trait = "uncommon", campLevel = 5, prob = .25, initCount = 80, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L5_unit_IN_uncommon",trait = "uncommon", campLevel = 5, prob = .25, initCount = 90, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L5_speed_building_uncommon",trait = "uncommon", campLevel = 5, prob = .25, initCount = 45, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L5_speed_research_uncommon",trait = "uncommon", campLevel = 5, prob = .25, initCount = 240, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L5_silverBag_rare",trait = "rare", campLevel = 5, prob = .1, initCount = 500, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L5_unit_KN_rare",trait = "rare", campLevel = 5, prob = .1, initCount = 200, type = "unit", typeID = 6 },
            //epic
            new lootRewardBase { lootID = "L5_unit_KN_epic",trait = "epic", campLevel = 5, prob = .05, initCount = 500, type = "unit", typeID = 6 },

            //Level 6
            //common
            new lootRewardBase { lootID = "L6_silverBag1",trait = "common", campLevel = 6, prob = 5, initCount = 50, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L6_PF_silver1",trait = "common", campLevel = 6, prob = 2, initCount = 90, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L6_unit_IN1",trait = "common", campLevel = 6, prob = 3, initCount = 60, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L6_unit_LC1",trait = "common", campLevel = 6, prob = 2, initCount = 20, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L6_unit_spy1",trait = "common", campLevel = 6, prob = 1, initCount = 15, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L6_unit_KN1",trait = "common", campLevel = 6, prob = 2, initCount = 12, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L6_unit_ram1",trait = "common", campLevel = 6, prob = 1, initCount = 9, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L6_unit_treb1",trait = "common", campLevel = 6, prob = 1, initCount = 9, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L6_speed_building1",trait = "common", campLevel = 6, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L6_speed_research1",trait = "common", campLevel = 6, prob = 1, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L6_silverBag_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 100, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L6_unit_ram_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 15, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L6_unit_treb_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 15, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L6_unit_LC_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 40, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L6_unit_IN_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 120, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L6_speed_building_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 45, type = "buildingspeedup", typeID = 0 },
            //rare
            new lootRewardBase { lootID = "L6_speed_research_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 300, type = "researchspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L6_PF_rebelrush_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 30, type = "pf", typeID = 32 }, //rebel rush 30mins
            new lootRewardBase { lootID = "L6_PF_defend_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 15, type = "pf", typeID = 23 }, //defend spell, 15mins
            new lootRewardBase { lootID = "L6_silverBag_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 750, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L6_unit_KN_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 300, type = "unit", typeID = 6 }, 
            //epic
            new lootRewardBase { lootID = "L6_unit_KN_epic",trait = "epic", campLevel = 6, prob = .05, initCount = 750 , type = "unit", typeID = 6},
            new lootRewardBase { lootID = "L6_unit_GOV_epic",trait = "epic", campLevel = 6, prob = .05, initCount = 1, type = "unit", typeID = 10 },
            new lootRewardBase { lootID = "L6_PF_attack_epic",trait = "epic", campLevel = 6, prob = .05, initCount = 720, type = "pf", typeID = 24 }, //epic 12hr attack PF

            //Level 7
            //common
            new lootRewardBase { lootID = "L7_silverBag1",trait = "common", campLevel = 7, prob = 5, initCount = 60, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_PF_silver1",trait = "common", campLevel = 7, prob = 2, initCount = 90, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L7_unit_IN1",trait = "common", campLevel = 7, prob = 3, initCount = 80, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L7_unit_LC1",trait = "common", campLevel = 7, prob = 2, initCount = 25, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L7_unit_spy1",trait = "common", campLevel = 7, prob = 1, initCount = 20, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L7_unit_KN1",trait = "common", campLevel = 7, prob = 2, initCount = 18, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L7_unit_ram1",trait = "common", campLevel = 7, prob = 1, initCount = 10, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L7_unit_treb1",trait = "common", campLevel = 7, prob = 1, initCount = 10, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L7_speed_building1",trait = "common", campLevel = 7, prob = 1, initCount = 20, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L7_speed_research1",trait = "common", campLevel = 7, prob = 1, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L7_silverBag_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 250, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_unit_ram_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 20, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L7_unit_treb_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 20, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L7_unit_LC_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 60, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L7_unit_IN_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 150, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L7_speed_building_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 60, type = "buildingspeedup", typeID = 0 },   
            //rare
            new lootRewardBase { lootID = "L7_speed_research_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 360, type = "researchspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L7_PF_rebelrush_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 30, type = "pf", typeID = 32 }, //rebel rush 30mins
            new lootRewardBase { lootID = "L7_PF_defend_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 15, type = "pf", typeID = 23 }, //defend spell, 15mins
            new lootRewardBase { lootID = "L7_silverBag_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 1000, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_unit_KN_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 350, type = "unit", typeID = 6 }, 
            //epic
            new lootRewardBase { lootID = "L7_unit_KN_epic",trait = "epic", campLevel = 7, prob = .05, initCount = 800 , type = "unit", typeID = 6},
            new lootRewardBase { lootID = "L7_unit_GOV_epic",trait = "epic", campLevel = 7, prob = .05, initCount = 1, type = "unit", typeID = 10 },
            new lootRewardBase { lootID = "L7_PF_attack_epic",trait = "epic", campLevel = 7, prob = .05, initCount = 720, type = "pf", typeID = 24 }, //epic 12hr attack PF

        };


        ///
        /// BaseRewards_nonVIP1
        /// 10 spawn a day style, but non magic rewards
        /// basically no PFs
        public static List<lootRewardBase> BaseRewards_nonVIP1 = new List<lootRewardBase>{

             //Level 1
            //common
            new lootRewardBase { lootID = "L1_silverBag1", trait = "common", campLevel = 1, prob = 5, initCount = 2, type = "silver", typeID = 0 },
            ///new lootRewardBase { lootID = "L1_PF_silver1", trait = "common", campLevel = 1, prob = 2, initCount = 15, type = "pf", typeID = 22 },  //count in  minutes     
            new lootRewardBase { lootID = "L1_unit_CM1", trait = "common", campLevel = 1, prob = 5, initCount = 2, type = "unit", typeID = 11  },
            new lootRewardBase { lootID = "L1_unit_CM2", trait = "common", campLevel = 1, prob = 1, initCount = 8, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L1_unit_IN1", trait = "common", campLevel = 1, prob = 1, initCount = 4, type = "unit", typeID = 2  },
            new lootRewardBase { lootID = "L1_speed_building1", trait = "common", campLevel = 1, prob = 1, initCount = 5, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L1_speed_research1", trait = "common", campLevel = 1, prob = 1, initCount = 10, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L1_speed_building_uncommon", trait = "uncommon", campLevel = 1, prob = .25, initCount = 15, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L1_speed_research_uncommon", trait = "uncommon", campLevel = 1, prob = .25, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L1_silverBag_rare", trait = "rare", campLevel = 1, prob = .1, initCount = 20, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L1_unit_KN_rare", trait = "rare", campLevel = 1, prob = .1, initCount = 5, type = "unit", typeID = 6 },          


            //Level 2
            //common
            new lootRewardBase { lootID = "L2_silverBag1",trait = "common", campLevel = 2, prob = 2.5, initCount = 10, type = "silver", typeID = 0},
            //new lootRewardBase { lootID = "L2_PF_silver1",trait = "common", campLevel = 2, prob = 1, initCount = 30, type = "pf", typeID = 22},
            new lootRewardBase { lootID = "L2_unit_CM1",trait = "common", campLevel = 2, prob = 3.5, initCount = 12, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L2_unit_IN1",trait = "common", campLevel = 2, prob = 2, initCount = 8, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L2_unit_LC1",trait = "common", campLevel = 2, prob = 1, initCount = 4, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L2_unit_spy1",trait = "common", campLevel = 2, prob = .5, initCount = 4, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L2_speed_building1",trait = "common", campLevel = 2, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 }, //count in minutes
            new lootRewardBase { lootID = "L2_speed_research1",trait = "common", campLevel = 2, prob = .5, initCount = 20, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L2_speed_building_uncommon",trait = "uncommon", campLevel = 2, prob = .25, initCount = 30, type = "buildingspeedup", typeID = 0},
            new lootRewardBase { lootID = "L2_speed_research_uncommon",trait = "uncommon", campLevel = 2, prob = .25, initCount = 120, type = "researchspeedup", typeID = 0}, 
            //rare
            new lootRewardBase { lootID = "L2_silverBag_rare",trait = "rare", campLevel = 2, prob = .1, initCount = 50, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L2_unit_KN_rare",trait = "rare", campLevel = 2, prob = .1, initCount = 25, type = "unit", typeID = 6 }, 
        

            //Level 3
            //common
            new lootRewardBase { lootID = "L3_silverBag1",trait = "common", campLevel = 3, prob = 2.5, initCount = 16, type = "silver", typeID = 0},
            //new lootRewardBase { lootID = "L3_PF_silver1",trait = "common", campLevel = 3, prob = 1, initCount = 45, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L3_unit_CM1",trait = "common", campLevel = 3, prob = 3, initCount = 20, type = "unit", typeID = 11 },
            new lootRewardBase { lootID = "L3_unit_IN1",trait = "common", campLevel = 3, prob = 2, initCount = 10, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L3_unit_LC1",trait = "common", campLevel = 3, prob = 1, initCount = 6, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L3_unit_spy1",trait = "common", campLevel = 3, prob = .5, initCount = 6, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L3_speed_building1",trait = "common", campLevel = 3, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0}, //count in minutes
            new lootRewardBase { lootID = "L3_speed_research1",trait = "common", campLevel = 3, prob = .5, initCount = 20, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L3_speed_building_uncommon",trait = "uncommon", campLevel = 3, prob = .25, initCount = 30, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L3_speed_research_uncommon",trait = "uncommon", campLevel = 3, prob = .25, initCount = 180, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L3_silverBag_rare",trait = "rare", campLevel = 3, prob = .1, initCount = 150, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L3_unit_KN_rare",trait = "rare", campLevel = 3, prob = .1, initCount = 50, type = "unit", typeID = 6 }, 
   

            //Level 4
            //common
            new lootRewardBase { lootID = "L4_silverBag1",trait = "common", campLevel = 4, prob = 2.5, initCount = 20, type = "silver", typeID = 0},
            //new lootRewardBase { lootID = "L4_PF_silver1",trait = "common", campLevel = 4, prob = 1, initCount = 60, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L4_unit_IN1",trait = "common", campLevel = 4, prob = 1.5, initCount = 20, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L4_unit_LC1",trait = "common", campLevel = 4, prob = 1, initCount = 10, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L4_unit_spy1",trait = "common", campLevel = 4, prob = .5, initCount = 8, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L4_unit_KN1",trait = "common", campLevel = 4, prob = 1, initCount = 5, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L4_speed_building1",trait = "common", campLevel = 4, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L4_speed_research1",trait = "common", campLevel = 4, prob = .5, initCount = 20, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L4_unit_IN_uncommon",trait = "uncommon", campLevel = 4, prob = .25, initCount = 50, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L4_speed_building_uncommon",trait = "uncommon", campLevel = 4, prob = .25, initCount = 45, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L4_speed_research_uncommon",trait = "uncommon", campLevel = 4, prob = .25, initCount = 180, type = "researchspeedup", typeID = 0 },
            //rare
            new lootRewardBase { lootID = "L4_silverBag_rare",trait = "rare", campLevel = 4, prob = .1, initCount = 300, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L4_unit_KN_rare",trait = "rare", campLevel = 4, prob = .1, initCount = 100, type = "unit", typeID = 6 }, 


            //Level 5
            //common
            new lootRewardBase { lootID = "L5_silverBag1",trait = "common", campLevel = 5, prob = 3, initCount = 40, type = "silver", typeID = 0},
            //new lootRewardBase { lootID = "L5_PF_silver1",trait = "common", campLevel = 5, prob = 1, initCount = 60, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L5_unit_IN1",trait = "common", campLevel = 5, prob = 1.5, initCount = 40, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L5_unit_LC1",trait = "common", campLevel = 5, prob = 1, initCount = 15, type = "unit", typeID = 5},
            new lootRewardBase { lootID = "L5_unit_spy1",trait = "common", campLevel = 5, prob = .5, initCount = 12, type = "unit", typeID = 12},
            new lootRewardBase { lootID = "L5_unit_KN1",trait = "common", campLevel = 5, prob = 1, initCount = 8, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L5_unit_ram1",trait = "common", campLevel = 5, prob = .5, initCount = 8, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L5_unit_treb1",trait = "common", campLevel = 5, prob = .5, initCount = 8, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L5_speed_building1",trait = "common", campLevel = 5, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L5_speed_research1",trait = "common", campLevel = 5, prob = .5, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L5_silverBag_uncommon",trait = "uncommon", campLevel = 5, prob = .25, initCount = 80, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L5_unit_IN_uncommon",trait = "uncommon", campLevel = 5, prob = .25, initCount = 90, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L5_speed_building_uncommon",trait = "uncommon", campLevel = 5, prob = .25, initCount = 45, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L5_speed_research_uncommon",trait = "uncommon", campLevel = 5, prob = .25, initCount = 240, type = "researchspeedup", typeID = 0 }, 
            //rare
            new lootRewardBase { lootID = "L5_silverBag_rare",trait = "rare", campLevel = 5, prob = .1, initCount = 500, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L5_unit_KN_rare",trait = "rare", campLevel = 5, prob = .1, initCount = 200, type = "unit", typeID = 6 },
            //epic
            new lootRewardBase { lootID = "L5_unit_KN_epic",trait = "epic", campLevel = 5, prob = .05, initCount = 500, type = "unit", typeID = 6 },

            //Level 6
            //common
            new lootRewardBase { lootID = "L6_silverBag1",trait = "common", campLevel = 6, prob = 5, initCount = 50, type = "silver", typeID = 0},
            //new lootRewardBase { lootID = "L6_PF_silver1",trait = "common", campLevel = 6, prob = 2, initCount = 90, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L6_unit_IN1",trait = "common", campLevel = 6, prob = 3, initCount = 60, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L6_unit_LC1",trait = "common", campLevel = 6, prob = 2, initCount = 20, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L6_unit_spy1",trait = "common", campLevel = 6, prob = 1, initCount = 15, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L6_unit_KN1",trait = "common", campLevel = 6, prob = 2, initCount = 12, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L6_unit_ram1",trait = "common", campLevel = 6, prob = 1, initCount = 9, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L6_unit_treb1",trait = "common", campLevel = 6, prob = 1, initCount = 9, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L6_speed_building1",trait = "common", campLevel = 6, prob = 1, initCount = 15, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L6_speed_research1",trait = "common", campLevel = 6, prob = 1, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L6_silverBag_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 100, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L6_unit_ram_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 15, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L6_unit_treb_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 15, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L6_unit_LC_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 40, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L6_unit_IN_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 120, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L6_speed_building_uncommon",trait = "uncommon", campLevel = 6, prob = .25, initCount = 45, type = "buildingspeedup", typeID = 0 },
            //rare
            new lootRewardBase { lootID = "L6_speed_research_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 300, type = "researchspeedup", typeID = 0 },
            //new lootRewardBase { lootID = "L6_PF_rebelrush_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 30, type = "pf", typeID = 32 }, //rebel rush 30mins
            //new lootRewardBase { lootID = "L6_PF_defend_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 15, type = "pf", typeID = 23 }, //defend spell, 15mins
            new lootRewardBase { lootID = "L6_silverBag_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 750, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L6_unit_KN_rare",trait = "rare", campLevel = 6, prob = .1, initCount = 300, type = "unit", typeID = 6 }, 
            //epic
            new lootRewardBase { lootID = "L6_unit_KN_epic",trait = "epic", campLevel = 6, prob = .05, initCount = 750 , type = "unit", typeID = 6},
            new lootRewardBase { lootID = "L6_unit_GOV_epic",trait = "epic", campLevel = 6, prob = .05, initCount = 1, type = "unit", typeID = 10 },
            //new lootRewardBase { lootID = "L6_PF_attack_epic",trait = "epic", campLevel = 6, prob = .05, initCount = 720, type = "pf", typeID = 24 }, //epic 12hr attack PF

            //Level 7
            //common
            new lootRewardBase { lootID = "L7_silverBag1",trait = "common", campLevel = 7, prob = 5, initCount = 60, type = "silver", typeID = 0},
            //new lootRewardBase { lootID = "L7_PF_silver1",trait = "common", campLevel = 7, prob = 2, initCount = 90, type = "pf", typeID = 22 },
            new lootRewardBase { lootID = "L7_unit_IN1",trait = "common", campLevel = 7, prob = 3, initCount = 80, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L7_unit_LC1",trait = "common", campLevel = 7, prob = 2, initCount = 25, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L7_unit_spy1",trait = "common", campLevel = 7, prob = 1, initCount = 20, type = "unit", typeID = 12 },
            new lootRewardBase { lootID = "L7_unit_KN1",trait = "common", campLevel = 7, prob = 2, initCount = 18, type = "unit", typeID = 6 },
            new lootRewardBase { lootID = "L7_unit_ram1",trait = "common", campLevel = 7, prob = 1, initCount = 10, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L7_unit_treb1",trait = "common", campLevel = 7, prob = 1, initCount = 10, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L7_speed_building1",trait = "common", campLevel = 7, prob = 1, initCount = 20, type = "buildingspeedup", typeID = 0 },
            new lootRewardBase { lootID = "L7_speed_research1",trait = "common", campLevel = 7, prob = 1, initCount = 30, type = "researchspeedup", typeID = 0 }, 
            //uncommon
            new lootRewardBase { lootID = "L7_silverBag_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 250, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_unit_ram_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 20, type = "unit", typeID = 7 },
            new lootRewardBase { lootID = "L7_unit_treb_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 20, type = "unit", typeID = 8 },
            new lootRewardBase { lootID = "L7_unit_LC_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 60, type = "unit", typeID = 5 },
            new lootRewardBase { lootID = "L7_unit_IN_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 150, type = "unit", typeID = 2 },
            new lootRewardBase { lootID = "L7_speed_building_uncommon",trait = "uncommon", campLevel = 7, prob = .25, initCount = 60, type = "buildingspeedup", typeID = 0 },   
            //rare
            new lootRewardBase { lootID = "L7_speed_research_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 360, type = "researchspeedup", typeID = 0 },
            //new lootRewardBase { lootID = "L7_PF_rebelrush_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 30, type = "pf", typeID = 32 }, //rebel rush 30mins
            //new lootRewardBase { lootID = "L7_PF_defend_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 15, type = "pf", typeID = 23 }, //defend spell, 15mins
            new lootRewardBase { lootID = "L7_silverBag_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 1000, type = "silver", typeID = 0},
            new lootRewardBase { lootID = "L7_unit_KN_rare",trait = "rare", campLevel = 7, prob = .1, initCount = 350, type = "unit", typeID = 6 }, 
            //epic
            new lootRewardBase { lootID = "L7_unit_KN_epic",trait = "epic", campLevel = 7, prob = .05, initCount = 800 , type = "unit", typeID = 6},
            new lootRewardBase { lootID = "L7_unit_GOV_epic",trait = "epic", campLevel = 7, prob = .05, initCount = 1, type = "unit", typeID = 10 },
            //new lootRewardBase { lootID = "L7_PF_attack_epic",trait = "epic", campLevel = 7, prob = .05, initCount = 720, type = "pf", typeID = 24 }, //epic 12hr attack PF

        };

    }




}


