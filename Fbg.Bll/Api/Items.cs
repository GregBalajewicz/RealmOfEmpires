using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Fbg.Bll.Api
{
    public static class Items
    {

        public class MyItem
        {
            public int giftID;
            public int count;
            public int costInServants;
            public double payoutMultiplyer;
        }

        public class MyItemsInfo
        {
            public int hourlySilverProdForSilverGift;
            public int todaysDailyLimit;
            public int dailyLimit;
            public int dailyLimitCarryOverDays;
            public IDictionary<string, MyItem> myItems;
        }

        public static MyItemsInfo GetMyItems(Fbg.Bll.Player player, int villageID)
        {
            return GetMyItems(player, Fbg.Bll.Village.GetVillage(player, villageID, false, true));
        }
        public static MyItemsInfo GetMyItems(Fbg.Bll.Player player, Village village)
        {
            DataTable dt = player.User.Gifts_GetMyGifts(player.Realm.ID);

            IDictionary<string, MyItem> myGiftsDict ;
            var myGifts = from r in dt.AsEnumerable()
                          select new { count = r.Field<int>("cnt"), giftID = r.Field<int>("giftID") };
            myGiftsDict = myGifts.AsEnumerable().ToDictionary(k => k.giftID.ToString(), v => new MyItem { giftID = v.giftID, count = v.count
                , costInServants = player.Realm.GiftByID(v.giftID).CostInCredits
                , payoutMultiplyer = player.Realm.GiftByID(v.giftID).GetCorrectMultiplierBasedOnRegisterDate(player.RegisteredOn)
            });

            foreach (Gift g in player.Realm.Gifts)
            {
                if (!myGiftsDict.ContainsKey(g.Id.ToString()))
                {
                    myGiftsDict.Add(new KeyValuePair<string, MyItem>(g.Id.ToString(), new MyItem { giftID = g.Id, count = 0
                    , costInServants = g.CostInCredits
                    , payoutMultiplyer = g.GetCorrectMultiplierBasedOnRegisterDate(player.RegisteredOn)}));
                }
            }

            MyItemsInfo myGiftInfo = new MyItemsInfo() {  
                hourlySilverProdForSilverGift = GetSilverProdForSilverGift(player, village),
                todaysDailyLimit = player.Gifts_NumberOfGiftsICanUseToday(true),
                dailyLimit = player.Realm.MaxNumberOfGiftsPerDayThatCanBeUsed,
                dailyLimitCarryOverDays = player.Realm.MaxNumberOfGiftsThatCanBeUsed_CarryOverDays,
                myItems = myGiftsDict};
            return myGiftInfo;
        }


        public struct BuyResult
        {
            public ItemBuyResult resultCode;
            public Details details;

            public struct Details
            {
                public int numberOfOwnedGiftsUsed;
                public int numberOfItemsBought;
                public int totalCostOfBoughtItems;
            }
        }


        public enum ItemBuyResult
        {
            OK = 0,
            FAIL_DailyLimitReached = 1,
            FAIL_ExpectedCostLowerThanActualCost = 2,
            FAIL_NoneUsedDueToTresOrFarmLimitation = 3,
            PARTIAL_UsedSomeButNotAllDueToTresOrFarmLimitation = 4,
            FAIL_NoneUsedDueToCreditsMissing = 5,
            PARTIAL_UsedSomeButNotAllDueToCreditsMissing = 6,
            FAIL_GiftsNotActive = 7,
            FAIL_TitleReqNotMet = 8
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="giftID">ID of the gift type</param>
        /// <param name="expectedCost">expected cost in credits for this amount of gifts - we compare this to actual and only make the transaction if it works</param>
        /// <param name="amountToBuy">amount of the gifts to *GET* - this is a total number to get - thish may be some gifts to be used, so to buy</param>
        /// <param name="vid">Village id of the currnt village</param>
        /// <returns>
        /// resultCode -> one of the values of ItemBuyResult enum 
        /// 
        ///     OK -- all completed. see resultDetails for how man existing items were used, and how many were bought etc
        ///     
        ///     FAIL_DailyLimitReached -- the request amountToBuy of items would put the player over their current 
        ///         daily max - no items were bought or used - resultDetails shoudl be ignored
        ///     
        ///     FAIL_ExpectedCostLowerThanActualCost -- this can happen in case a player now has fewer gifts that the 
        ///         caller expected thus more have to be bought so we abort - resultDetails shoudl be ignored
        ///     
        ///     FAIL_NoneUsedDueToTresOrFarmLimitation -- either tresury cannot hold more silver or farm land cannot 
        ///         support more troops so NO items were used/bought - resultDetails shoudl be ignored
        ///     
        ///     PARTIAL_UsedSomeButNotAllDueToTresOrFarmLimitation -- simillar to FAIL_NoneUsedDueToTresOrFarmLimitation in that not all gifts could be bought
        ///         because either tresury cannot hold more silver or farm land cannot support more troops however, SOME of the gifts were used / bought. 
        ///         Examine resultDetails to see how many were actually bought or used
        ///     
        ///     FAIL_NoneUsedDueToCreditsMissing -- not enough credits to buy even one gift so NO items were used/bought - resultDetails shoudl be ignored
        ///     
        ///     PARTIAL_UsedSomeButNotAllDueToCreditsMissing -- simillar to FAIL_NoneUsedDueToCreditsMissing in that not all gifts could be bought
        ///         because not enough credits however, SOME of the gifts were used / bought. 
        ///         Examine resultDetails to see how many were actually bought and/or used
        ///     
        /// 
        /// resultDetails -> object that give you details about what actually happend. This is only valid depending on the value of resultCode.
        /// </returns>
        public static  BuyResult Buy(Fbg.Bll.Player player, int giftID, int vid, int amountToBuy, int expectedCost)
        {
            //
            // notes - this code suffers from possible multithreading issues that could result in a person being able to get more gifts per day than is allowed
            //  for example, if 2 istances of this code run at the smae time, they could both check that they are below the daily limit and both woudl give gifts
            //  this is very unlikely scenario, that would be almost impossible to intentionally exploit, so we will not worry about it.
            //  Also, risk of this happening is small as it just results in player getting more gifts
            //
            //  Another possibility is that player uses his gifts (that he owns) twice or more. Again, almost impossible to intentionally expoit
            //
            int numMustBuy = 0; // number of gifts that player needs to by above what he has: = wantToBuy - Has
            int numMustUse = 0; // number of owned gifts that should be used
            int numGiftsInPosession = 0;
            int final_numUsed = 0;      // this counts the actual number used as we loop trough and use them. 
            int final_numBought = 0;    // this counts the actual number bought as we loop trough and buy them. 
            int final_costBought = 0;   // this counts the actual cost of the bought gifts

            int hourlySilverProd; // as it relates to the sivler gift

            Village v;
            v = Fbg.Bll.Village.GetVillage(player, vid, false, true);

            Gift g = player.Realm.GiftByID(giftID);
            ItemBuyResult result;
            result = ItemBuyResult.OK;

           
            //
            // get how many of this gift type the player has
            MyItemsInfo myGifts = GetMyItems(player, v);
            MyItem myGift;

            if (!player.Realm.AreGiftsActive)
            {
                result = ItemBuyResult.FAIL_GiftsNotActive;
            }

            //
            // is the player at the right title to get this gift ?
            //
            if (result == ItemBuyResult.OK)
            {
                if (g.RequiredTitle != null && g.RequiredTitle > player.Title)
                {
                    result = ItemBuyResult.FAIL_TitleReqNotMet;
                }
            }

            if (result == ItemBuyResult.OK)
            {
                myGifts.myItems.TryGetValue(g.Id.ToString(), out myGift);
                numGiftsInPosession = (myGift == null ? 0 : myGift.count);

                //
                // check if below daily limit
                //
                int numGiftsLeftToday = player.Gifts_NumberOfGiftsICanUseToday(false);
                if (numGiftsLeftToday < amountToBuy)
                {
                    result = ItemBuyResult.FAIL_DailyLimitReached;
                }
            }

            //
            // get cost of transaction and check against expected cost
            //       
            if (result == ItemBuyResult.OK)
            {
                numMustUse = amountToBuy >= numGiftsInPosession ? numGiftsInPosession : amountToBuy;
                numMustBuy = amountToBuy - numGiftsInPosession;
                int actualBuyCost = numMustBuy * g.CostInCredits;
                if (expectedCost < actualBuyCost)
                {
                    result = ItemBuyResult.FAIL_ExpectedCostLowerThanActualCost;
                }
            }
            //
            // check for tres or farm space
            //

            //
            // get hourly Silver prod as it relates to the silver item
            //

            hourlySilverProd = GetSilverProdForSilverGift(player, v);

            #region loop through all gifts we should use
            //
            // loop through all gifts we should use 
            //
            if (result == ItemBuyResult.OK)
            {
                for (int i = 0; i < numMustUse; i++)
                {
                    // for greater thread safety, to make sure someone does not use his gifts in another thread, we should 
                    //  call "get my gifts" in the loop to make sure we still have the gift
                    //
                    // we reload the village object so that we have the latest treasury size. this is necessary since coins are not updated untill a reload
                    //
                    v = Fbg.Bll.Village.GetVillage(player, vid, false, true);
                    if (Items_Buy_GetOne(v, g, hourlySilverProd, false) == Items_Buy_GetOne_RESULT.OK)
                    {
                        final_numUsed++;
                    }
                    else
                    {
                        if (final_numUsed == 0)
                        {
                            result = ItemBuyResult.FAIL_NoneUsedDueToTresOrFarmLimitation;
                        }
                        else
                        {
                            result = ItemBuyResult.PARTIAL_UsedSomeButNotAllDueToTresOrFarmLimitation;
                        }
                    }
                }
            }
            #endregion

            #region loop through all gifts we should BUY
            //
            // loop through all gifts we should BUY
            //
            if (result == ItemBuyResult.OK)
            {
                Items_Buy_GetOne_RESULT buyOneResult;
                for (int i = 0; i < numMustBuy; i++)
                {
                    //
                    // we reload the village object so that we have the latest treasury size. this is necessary since coins are not updated untill a reload
                    //
                    v = Fbg.Bll.Village.GetVillage(player, vid, false, true);
                    buyOneResult = Items_Buy_GetOne(v, g, hourlySilverProd, true);
                    if (buyOneResult == Items_Buy_GetOne_RESULT.OK)
                    {
                        final_numBought++;
                    }
                    else
                    {
                        if (final_numBought == 0 && final_numUsed == 0)
                        {
                            result = buyOneResult == Items_Buy_GetOne_RESULT.Fail_NoFarmOrTresSpace ?
                                ItemBuyResult.FAIL_NoneUsedDueToTresOrFarmLimitation
                                : ItemBuyResult.FAIL_NoneUsedDueToCreditsMissing;
                        }
                        else
                        {
                            result = buyOneResult == Items_Buy_GetOne_RESULT.Fail_NoFarmOrTresSpace ?
                                ItemBuyResult.PARTIAL_UsedSomeButNotAllDueToTresOrFarmLimitation
                                : ItemBuyResult.PARTIAL_UsedSomeButNotAllDueToCreditsMissing;
                        }
                    }
                }
            }
            #endregion


            if (g.Id == 1) // silver
            {
                player.Quests2.SetQuestAsCompleted(Fbg.Bll.Player.QuestTags.Gifts_BuySilver.ToString());
            }
            else if (g.Id == 4) //infantry
            {
                player.Quests2.SetQuestAsCompleted(Fbg.Bll.Player.QuestTags.Gifts_BuyInfantry.ToString());
            }

            return new BuyResult() { resultCode = result
                , details = new BuyResult.Details() { numberOfItemsBought = final_numBought, numberOfOwnedGiftsUsed = final_numUsed, totalCostOfBoughtItems = final_costBought } };
        }


        enum Items_Buy_GetOne_RESULT
        {
            OK,
            Fail_NoFarmOrTresSpace,
            Fail_NotEnoughServants

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="g"></param>
        /// <param name="hourlySilverProd"></param>
        /// <param name="isBuy">true means "do BUY". false, means "do USE"</param>
        /// <returns></returns>
        private static  Items_Buy_GetOne_RESULT Items_Buy_GetOne(Village v, Gift g, int hourlySilverProd, bool isBuy)
        {

            int costForGift = g.CostInCredits;

            if (isBuy)
            {
                if (v.owner.User.Credits < costForGift)
                {
                    return Items_Buy_GetOne_RESULT.Fail_NotEnoughServants;
                }
            }

            if (g is Gift_HourlySilverProd)
            {
                double silverFromThisGift = (double)hourlySilverProd * ((Gift_HourlySilverProd)g).ProductionRewardMultiplier(v.owner.Realm);

                int treasurySize = v.TreasurySize;
                if (treasurySize - v.coins < silverFromThisGift)
                {
                    return Items_Buy_GetOne_RESULT.Fail_NoFarmOrTresSpace;
                }

                if (isBuy)
                {
                    v.owner.Gifts_Buy(g, v.owner.Realm.ID, v.id, silverFromThisGift.ToString());
                }

                v.UpdateCoins(Convert.ToInt32(Math.Ceiling(silverFromThisGift)));
            }
            else if (g is Gift_Troops)
            {
                int unitAmt = ((Gift_Troops)g).numOfTroops(v.owner.Realm);
                if (unitAmt * ((Gift_Troops)g).unitType.Pop <= v.RemainingPopulation)
                {
                    if (isBuy)
                    {
                        v.owner.Gifts_Buy(g, v.owner.Realm.ID, v.id, unitAmt.ToString());
                    }

                    v.AddUnitsToVillage(((Gift_Troops)g).unitType, unitAmt);
                }
                else
                {
                    return Items_Buy_GetOne_RESULT.Fail_NoFarmOrTresSpace;
                }
            }

            v.owner.User.Gifts_UseGift(g.Id, v.owner.Realm.ID);
            v.owner.Gifts_RegisterdThatGiftWasUsed();

            return Items_Buy_GetOne_RESULT.OK;
        }

        private static int GetSilverProdForSilverGift ( Fbg.Bll.Player player, int vid) {
            return GetSilverProdForSilverGift(player, Fbg.Bll.Village.GetVillage(player, vid, false, true));

        }
        private static int GetSilverProdForSilverGift(Fbg.Bll.Player player, Village v)
        {
            int hourlySilverProd;

            if (player.Realm.ID < 64)
            {
                // olf way of calculating gift payout 
                if (player.NumberOfVillages == 1)
                {
                    hourlySilverProd = v.GetBuildingLevelObject(Fbg.Bll.CONSTS.BuildingIDs.CoinMine).EffectAsInt;
                }
                else
                {
                    BuildingType silverMine = player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.CoinMine);
                    hourlySilverProd = silverMine.Level(silverMine.MaxLevel).EffectAsInt;
                }
            }
            else
            {
                // figure out hourly silver production for the purpose of calculating the gift payout
                // max payout, is level 35 mine (or max silver mine if it happens to be less then 35 - just protecting the code for some funky realms perhaps in the future) 
                BuildingType silverMine = player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.CoinMine);
                int maxHourlySilverProdForGift = silverMine.Level(silverMine.MaxLevel < 35 ? silverMine.MaxLevel : 35).EffectAsInt;
                if (player.NumberOfVillages == 1)
                {
                    hourlySilverProd = Math.Min(v.GetBuildingLevelObject(Fbg.Bll.CONSTS.BuildingIDs.CoinMine).EffectAsInt, maxHourlySilverProdForGift);
                }
                else
                {
                    hourlySilverProd = maxHourlySilverProdForGift;
                }               
            }
            if (player.Realm.IsTemporaryTournamentRealm)
            {
                // cap max production to 6000. important for RS and RXs
                hourlySilverProd = hourlySilverProd > 6000 ? 6000 : hourlySilverProd;
            }

            if (player.PF_HasPF(Fbg.Bll.CONSTS.PFs.SilverBonus))
            {
                hourlySilverProd = Convert.ToInt32(hourlySilverProd + hourlySilverProd * Fbg.Bll.CONSTS.PF_SilverBonusPercent);
            }

            return hourlySilverProd;
        }

    }
}
