using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDA.Achievements;

namespace Fbg.Bll
{
    partial class Player
    {


        /// <summary>
        /// return true if reward was not set as claimed before, returns false if it was set claimed before. 
        /// in either case, the reward is set as claimed. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool SetRewardClaimed(int id)
        {
            bool rewardAlreadyClaimed = IsRewardClaimed(id, true);
            SetFlag(id, "1");
            return !rewardAlreadyClaimed;
        }

        /// <summary>
        /// Tells you if reward was claimed but does not check the DB meaning if reward was claimed in a different session,
        /// this will return an invalid value, call IsRewardClaimed(id, true) if you want to be sure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsRewardClaimed(int id)
        {
            /// NOTE we pass false, we do NOT to the DB, therefore it reward was claimed in a different session,
            /// this will return an invalid value
            return IsRewardClaimed(id, false);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsRewardClaimed(int id, bool goToDatabase)
        {

            Object o = HasFlag(id, !goToDatabase);
            o = HasFlag_GetData(id);
            return o == null || o is DBNull ? false : ((string)o == "1" ? true : false);
        }




        private DateTime QuestCompledOn(int id)
        {
            Object o = HasFlag(id, true);
            return o == null ? DateTime.MinValue : (DateTime)o;
        }

        /// <summary>
        /// repreciated; do not use this any longer. 
        /// if you need to use the value from this enum, use the text version instead. 
        /// </summary>
        public enum QuestTags
        {            
            Tutorial=1,
            level_2,
            level_3,
            level_4,
            level_5,
            level_6,
            level_7,
            level_8,
            level_9,
            level_10,
            level_11,
            level_12,
            level_13,
            level_14,
            level_15,
            level_16,
            level_17,
            level_18,
            level_19,
            level_20,
            level_21,
            level_22,
            level_23,
            level_24,
            level_25,
            level_26,
            level_27,
            level_28,
            level_29, // dummy not used but cannot remove as existing quests will break
            level_30, // dummy not used but cannot remove as existing quests will break

            B_SM_lvl3,
            B_SM_lvl5,
            B_SM_lvl7,
            B_SM_lvl10,
            B_SM_lvl15,
            B_SM_lvl20,
            B_SM_lvl25,

            B_HQ_lvl3,
            B_HQ_lvl5,
            B_HQ_lvl7,
            B_HQ_lvl10,
            B_HQ_lvl15,
            B_HQ_lvl20,

            B_Stable_build,
            B_Stable_lvl3,
            B_Stable_lvl5,
            B_Stable_lvl7,
            B_Stable_lvl10,
            B_Stable_lvl15,
            B_Stable_lvl20,

            B_TP_build,
            B_TP_lvl3,
            B_TP_lvl5,
            B_TP_lvl7,
            B_TP_lvl10,
            B_TP_lvl15,

            B_Wall_build,
            B_Wall_lvl2,
            B_Wall_lvl3,
            B_Wall_lvl4,
            B_Wall_lvl5,

            B_Bar_build,
            B_Bar_lvl3,
            B_Bar_lvl5,

            B_Trsy_lvl3,
            B_Trsy_lvl5,
            B_Trsy_lvl7,
            B_Trsy_lvl10,
            B_Trsy_lvl15,

            B_Tarv_build,
            B_Tarv_lvl2,

            B_SW_build,
            B_SW_lvl3,
            B_SW_lvl5,
            B_SW_lvl7,
            B_SW_lvl10,
            
            B_Farm_lvl3,
            B_Farm_lvl5,
            B_Farm_lvl7,
            B_Farm_lvl10,
            B_Farm_lvl15,

            B_HS_build,

            B_Palace_build,

            //Capture Village Quests
            Vill2, Vill3, Vill4, Vill5, Vill6, Vill7, Vill8, Vill9, Vill10,
            Vill15, Vill20, Vill25, Vill30, Vill35, Vill40, Vill45, Vill50, Vill55,
            Vill60, Vill65, Vill70, Vill75, Vill80, Vill85, Vill90, Vill95, Vill100,
            Vill110, Vill120, Vill130, Vill140, Vill150, Vill160, Vill170, Vill180, Vill190, Vill200,
            Vill210, Vill220, Vill230, Vill240, Vill250, Vill260, Vill270, Vill280, Vill290, Vill300,
            Vill310, Vill320, Vill330, Vill340, Vill350, Vill360, Vill370, Vill380, Vill390, Vill400,
            Vill410, Vill420, Vill430, Vill440, Vill450, Vill460, Vill470, Vill480, Vill490, Vill500, 
            Vill550, Vill600, Vill650, Vill700, Vill750, Vill800, Vill850, Vill900, Vill950, Vill1000,

            //Profit_Res1,
            //Profit_Res2,
            //Profit_Res3,
            //Profit_Res4,

            //Research Quests
            Res_1, Res_2, Res_3, Res_4, Res_5, Res_6, Res_7, Res_8, Res_9, Res_10, 
            Res_11, Res_12, Res_13, Res_14, Res_15, Res_16, Res_17, Res_18, Res_19, Res_20,
            Res_21, Res_22, Res_23, Res_24, Res_25, Res_26, Res_27, Res_28, Res_29, Res_30,
            Res_31, Res_32, Res_33, Res_34, Res_35, Res_36, Res_37, Res_38, Res_39, Res_40,

            BuildingSpeedUp1,
            BuildingSpeedUp2,
            PF1,
            Gifts_BuySilver,
            Gifts_BuyInfantry, // not used. cannot remove as this will change following quest ids
            Gifts_Send,
            Avatar_TryChange,
            ChangeVillageName,
            SpyOnVillage,
            DiscoverBonusVillage
        }
        BDA.Achievements.Quests _quests;
        public BDA.Achievements.Quests Quests2
        { 
            get
            {
                if (_quests == null)
                {

                    
                    _quests = new BDA.Achievements.Quests(CONSTS.QuestsFlagStartOffset
                        , QuestCompledOn
                        , SetFlag
                        ,SetRewardClaimed
                        , IsRewardClaimed
                        , PrepareForQuestEvaluteCompledCalls
                        , realm.QuestTemplates);


                    // for old realms using old quests, use the old function
                    Quest.EvaluateCompletedDelegate delg;
                    if (realm.OpenOn < new DateTime(2012, 8, 1, 12, 0, 0)) {
                        delg = new Quest.EvaluateCompletedDelegate(QuestEvaluteCompled_OLD);
                    }
                    else {
                        delg = new Quest.EvaluateCompletedDelegate(QuestEvaluteCompled);
                    }
                    //
                    // create the quests from templates
                    //
                    foreach (QuestTemplate qt in realm.QuestTemplates)
                    {
                        _quests.AddQuest(qt, delg);
                    }

                }
                return _quests;
            }
        }


        public void PrepareForQuestEvaluteCompledCalls()
        {
            // here we know that in a moment we will get many calls to QuestEvaluteCompled(int) thereofre setup anything
            //  that needs to be setup

            // make sure when we check info about this village, it is up to date. 
            this.VillagesCache.InvalidateVillage(Villages_TempCache()[0].id);
        }
        public bool QuestEvaluteCompled(string tag)
        {
            QuestTemplate qt = this.realm.QuestTemplates.GetQuestTemplateByTag(tag);

            if (qt is QuestTemplate_Title) {
                return this.Title.Level >= ((QuestTemplate_Title)qt).TitleLevel.Level;
            }
            else if (qt is QuestTemplate_BuildingLevel) {
                return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(((QuestTemplate_BuildingLevel)qt).BuildingLevel.Building) >= ((QuestTemplate_BuildingLevel)qt).BuildingLevel.Level; 
            }
            else if (qt is QuestTemplate_Research) {
                return this.MyResearch.IsCompleted(((QuestTemplate_Research)qt).ResearchItem);
            }
            else if (qt is QuestTemplate_NumVillages) {
                return this.NumberOfVillages >= ((QuestTemplate_NumVillages)qt).NumVillges;
            }
            else {

                switch (tag) {
                    case "Tutorial":
                        return this.HasFlag(Flags.Advisor_BeginnerTutorialCompleted) != null;
                    case "JoinAClan":
                        return (this.Clan!= null && this.Clan.MemberCount > 1);
                    
                    case "CaptureAVillage2":
                        if (this.NumberOfVillages > 1) {
                            return true;
                        } else {
                            return this.VillagesCache.Village(Villages_TempCache()[0].id).GetVillageUnit(this.realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov)).YourUnitsTotalCount > 0;
                        }
                    case "CaptureAVillage3":
                        return this.NumberOfVillages >= 2;
                    case "CaptureAVillage4":
                        return this.NumberOfVillages >= 3;
                    default:
                        //throw new NotImplementedException(tag.ToString());
                        return false;
                }
            }
        }

        public bool QuestEvaluteCompled_OLD (string tag)
        {
            switch (tag) {
                case "Tutorial":
                    return this.HasFlag(Flags.Advisor_BeginnerTutorialCompleted) != null;
                case "level_2":
                    return this.Level >= 2;
                case "level_3":
                    return this.Level >= 3;
                case "level_4":
                    return this.Level >= 4;
                case "level_5":
                    return this.Level >= 5;
                case "level_6":
                    return this.Level >= 6;
                case "level_7":
                    return this.Level >= 7;
                case "level_8":
                    return this.Level >= 8;
                case "level_9":
                    return this.Level >= 9;
                case "level_10":
                    return this.Level >= 10;
                case "level_11":
                    return this.Level >= 11;
                case "level_12":
                    return this.Level >= 12;
                case "level_13":
                    return this.Level >= 13;
                case "level_14":
                    return this.Level >= 14;
                case "level_15":
                    return this.Level >= 15;
                case "level_16":
                    return this.Level >= 16;
                case "level_17":
                    return this.Level >= 17;
                case "level_18":
                    return this.Level >= 18;
                case "level_19":
                    return this.Level >= 19;
                case "level_20":
                    return this.Level >= 20;
                case "level_21":
                    return this.Level >= 21;
                case "level_22":
                    return this.Level >= 22;
                case "level_23":
                    return this.Level >= 23;
                case "level_24":
                    return this.Level >= 24;
                case "level_25":
                    return this.Level >= 25;
                case "level_26":
                    return this.Level >= 26;
                case "level_27":
                    return this.Level >= 27;
                case "level_28":
                    return this.Level >= 28;


                case "B_SM_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_CoinMine) >= 3;
                case "B_SM_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_CoinMine) >= 5;
                case "B_SM_lvl7":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_CoinMine) >= 7;
                case "B_SM_lvl10":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_CoinMine) >= 10;
                case "B_SM_lvl15":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_CoinMine) >= 15;
                case "B_SM_lvl20":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_CoinMine) >= 20;
                case "B_SM_lvl25":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_CoinMine) >= 25;

                case "B_HQ_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_HQ) >= 3;
                case "B_HQ_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_HQ) >= 5;
                case "B_HQ_lvl7":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_HQ) >= 7;
                case "B_HQ_lvl10":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_HQ) >= 10;
                case "B_HQ_lvl15":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_HQ) >= 15;
                case "B_HQ_lvl20":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_HQ) >= 20;

                case "B_Stable_build":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Stable) > 0;
                case "B_Stable_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Stable) >= 3;
                case "B_Stable_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Stable) >= 5;
                case "B_Stable_lvl7":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Stable) >= 7;
                case "B_Stable_lvl10":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Stable) >= 10;
                case "B_Stable_lvl15":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Stable) >= 15;
                case "B_Stable_lvl20":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Stable) >= 20;

                case "B_TP_build":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_TradePost) > 0;
                case "B_TP_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_TradePost) >= 3;
                case "B_TP_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_TradePost) >= 5;
                case "B_TP_lvl7":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_TradePost) >= 7;
                case "B_TP_lvl10":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_TradePost) >= 10;
                case "B_TP_lvl15":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_TradePost) >= 15;

                case "B_Wall_build":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Wall) > 0;
                case "B_Wall_lvl2":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Wall) >= 2;
                case "B_Wall_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Wall) >= 3;
                case "B_Wall_lvl4":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Wall) >= 4;
                case "B_Wall_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Wall) >= 5;

                case "B_Bar_build":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Barracks) > 0;
                case "B_Bar_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Barracks) >= 3;
                case "B_Bar_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Barracks) >= 5;

                case "B_Trsy_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Treasury) >= 3;
                case "B_Trsy_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Treasury) >= 5;
                case "B_Trsy_lvl7":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Treasury) >= 7;
                case "B_Trsy_lvl10":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Treasury) >= 10;
                case "B_Trsy_lvl15":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Treasury) >= 15;

                case "B_Tarv_build":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Tavern) > 0;
                case "B_Tarv_lvl2":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Tavern) >= 2;

                case "B_SW_build":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Workshop) > 0;
                case "B_SW_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Workshop) >= 3;
                case "B_SW_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Workshop) >= 5;
                case "B_SW_lvl7":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Workshop) >= 7;
                case "B_SW_lvl10":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Workshop) >= 10;

                case "B_Farm_lvl3":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Farm) >= 3;
                case "B_Farm_lvl5":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Farm) >= 5;
                case "B_Farm_lvl7":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Farm) >= 7;
                case "B_Farm_lvl10":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Farm) >= 10;
                case "B_Farm_lvl15":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Farm) >= 15;

                case "B_HS_build":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_HidingSpot) > 0;

                case "B_Palace_build":
                    return this.VillagesCache.Village(Villages_TempCache()[0].id).GetBuildingLevel(realm.BuildingType_Palace) > 0;


                //Research quests
                case "Res_1":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 16));
                case "Res_2":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 28));
                case "Res_3":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 76));
                case "Res_4":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 83));
                case "Res_5":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 93));
                case "Res_6":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 11));
                case "Res_7":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 135));
                case "Res_8":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 134));
                case "Res_9":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 7));
                case "Res_10":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 12));
                case "Res_11":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 49));
                case "Res_12":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 131));
                case "Res_13":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 73));
                case "Res_14":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 142));
                case "Res_15":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 43));
                case "Res_16":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 3));
                case "Res_17":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 113));
                case "Res_18":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 115));
                case "Res_19":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 116));
                case "Res_20":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 111));
                case "Res_21":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 68));
                case "Res_22":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 30));
                case "Res_23":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 4));
                case "Res_24":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 123));
                case "Res_25":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 32));
                case "Res_26":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 117));
                case "Res_27":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 22));
                case "Res_28":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 31));
                case "Res_29":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 52));
                case "Res_30":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 96));
                case "Res_31":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 100));
                case "Res_32":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 26));
                case "Res_33":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 60));
                case "Res_34":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 33));
                case "Res_35":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 45));
                case "Res_36":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 56));
                case "Res_37":
                    return this.MyResearch.IsCompleted(realm.Research.ResearchItemByID(1, 70));



                //case "hearts_1with1":
                //    return ExistsSubjectWithXStarts(this.GetSubjects_TempCache(), 1);
                //case "hearts_1with2":
                //    return ExistsSubjectWithXStarts(this.GetSubjects_TempCache(), 2);
                //case "hearts_1with3":
                //    return ExistsSubjectWithXStarts(this.GetSubjects_TempCache(), 3);
                //case "hearts_1with4":
                //    return ExistsSubjectWithXStarts(this.GetSubjects_TempCache(), 4);
                //case "hearts_1with5":
                //    return ExistsSubjectWithXStarts(this.GetSubjects_TempCache(), 5);
                //case "VisitCourt":
                //    return HasFlag(Flags.FriendsHaremVisitied) != null;


                //Capture village quests
                case "Vill2":
                    return this.NumberOfVillages >= 2;
                case "Vill3":
                    return this.NumberOfVillages >= 3;
                case "Vill4":
                    return this.NumberOfVillages >= 4;
                case "Vill5":
                    return this.NumberOfVillages >= 5;
                case "Vill6":
                    return this.NumberOfVillages >= 6;
                case "Vill7":
                    return this.NumberOfVillages >= 7;
                case "Vill8":
                    return this.NumberOfVillages >= 8;
                case "Vill9":
                    return this.NumberOfVillages >= 9;
                case "Vill10":
                    return this.NumberOfVillages >= 10;

                case "Vill15":
                    return this.NumberOfVillages >= 15;
                case "Vill20":
                    return this.NumberOfVillages >= 20;
                case "Vill25":
                    return this.NumberOfVillages >= 25;
                case "Vill30":
                    return this.NumberOfVillages >= 30;
                case "Vill35":
                    return this.NumberOfVillages >= 35;
                case "Vill40":
                    return this.NumberOfVillages >= 40;
                case "Vill45":
                    return this.NumberOfVillages >= 45;
                case "Vill50":
                    return this.NumberOfVillages >= 50;
                case "Vill55":
                    return this.NumberOfVillages >= 55;
                case "Vill60":
                    return this.NumberOfVillages >= 60;
                case "Vill65":
                    return this.NumberOfVillages >= 65;
                case "Vill70":
                    return this.NumberOfVillages >= 70;
                case "Vill75":
                    return this.NumberOfVillages >= 75;
                case "Vill80":
                    return this.NumberOfVillages >= 80;
                case "Vill85":
                    return this.NumberOfVillages >= 85;
                case "Vill90":
                    return this.NumberOfVillages >= 90;
                case "Vill95":
                    return this.NumberOfVillages >= 95;
                case "Vill100":
                    return this.NumberOfVillages >= 100;

                case "Vill110":
                    return this.NumberOfVillages >= 110;
                case "Vill120":
                    return this.NumberOfVillages >= 120;
                case "Vill130":
                    return this.NumberOfVillages >= 130;
                case "Vill140":
                    return this.NumberOfVillages >= 140;
                case "Vill150":
                    return this.NumberOfVillages >= 150;
                case "Vill160":
                    return this.NumberOfVillages >= 160;
                case "Vill170":
                    return this.NumberOfVillages >= 170;
                case "Vill180":
                    return this.NumberOfVillages >= 180;
                case "Vill190":
                    return this.NumberOfVillages >= 190;
                case "Vill200":
                    return this.NumberOfVillages >= 200;
                case "Vill210":
                    return this.NumberOfVillages >= 210;
                case "Vill220":
                    return this.NumberOfVillages >= 220;
                case "Vill230":
                    return this.NumberOfVillages >= 230;
                case "Vill240":
                    return this.NumberOfVillages >= 240;
                case "Vill250":
                    return this.NumberOfVillages >= 250;
                case "Vill260":
                    return this.NumberOfVillages >= 260;
                case "Vill270":
                    return this.NumberOfVillages >= 270;
                case "Vill280":
                    return this.NumberOfVillages >= 280;
                case "Vill290":
                    return this.NumberOfVillages >= 290;
                case "Vill300":
                    return this.NumberOfVillages >= 300;
                case "Vill310":
                    return this.NumberOfVillages >= 310;
                case "Vill320":
                    return this.NumberOfVillages >= 320;
                case "Vill330":
                    return this.NumberOfVillages >= 330;
                case "Vill340":
                    return this.NumberOfVillages >= 340;
                case "Vill350":
                    return this.NumberOfVillages >= 350;
                case "Vill360":
                    return this.NumberOfVillages >= 360;
                case "Vill370":
                    return this.NumberOfVillages >= 370;
                case "Vill380":
                    return this.NumberOfVillages >= 380;
                case "Vill390":
                    return this.NumberOfVillages >= 390;
                case "Vill400":
                    return this.NumberOfVillages >= 400;
                case "Vill410":
                    return this.NumberOfVillages >= 410;
                case "Vill420":
                    return this.NumberOfVillages >= 420;
                case "Vill430":
                    return this.NumberOfVillages >= 430;
                case "Vill440":
                    return this.NumberOfVillages >= 440;
                case "Vill450":
                    return this.NumberOfVillages >= 450;
                case "Vill460":
                    return this.NumberOfVillages >= 460;
                case "Vill470":
                    return this.NumberOfVillages >= 470;
                case "Vill480":
                    return this.NumberOfVillages >= 480;
                case "Vill490":
                    return this.NumberOfVillages >= 490;
                case "Vill500":
                    return this.NumberOfVillages >= 500;

                case "Vill550":
                    return this.NumberOfVillages >= 550;
                case "Vill600":
                    return this.NumberOfVillages >= 600;
                case "Vill650":
                    return this.NumberOfVillages >= 650;
                case "Vill700":
                    return this.NumberOfVillages >= 700;
                case "Vill750":
                    return this.NumberOfVillages >= 750;
                case "Vill800":
                    return this.NumberOfVillages >= 800;
                case "Vill850":
                    return this.NumberOfVillages >= 850;
                case "Vill900":
                    return this.NumberOfVillages >= 900;
                case "Vill950":
                    return this.NumberOfVillages >= 950;
                case "Vill1000":
                    return this.NumberOfVillages >= 1000;

                default:
                    //throw new NotImplementedException(tag.ToString());
                    return false;
            }
        }

        public class QuestReward : BDA.Achievements.QuestReward
        {
            public QuestReward()
            {
                Troops = new List<TroopQuestReward>();
                PFWithDuration = new List<PFWithDurationQuestReward>();
            }

            public int Silver { get; set; }
            public int Credits { get; set; }
            public List<TroopQuestReward> Troops { get; internal set; }
            public List<PFWithDurationQuestReward> PFWithDuration { get; internal set; }
            
            public class TroopQuestReward
            {
                public UnitType UnitType { get; internal set; }
                public int Amount { get; internal set; }
            }
            public class PFWithDurationQuestReward
            {
                public int PFPackageID { get; internal set; }
                public int DurationInMinutes { get; internal set; }
            }
  
        }


        public Quest Quests_CompleteAQuest(int questID)
        {
            Quest q = Quests2.GetQuestByID(questID);
            return Quests_CompleteAQuest(q);
        }
        public Quest Quests_CompleteAQuest(string questTag)
        {
            Quest q = Quests2.GetQuestByTag(questTag);
            return Quests_CompleteAQuest(q);
        }
        public Quest Quests_CompleteAQuest(Quest q)
        {
            Fbg.Bll.Player.QuestReward rew = (Fbg.Bll.Player.QuestReward)q.Reward;
            bool continueAccepting = true;
            if (q != null)
            {
                if (continueAccepting)
                {
                    if (q.ClaimReward())
                    {
                        if (rew.Credits > 0)
                        {
                            this.User.GetCreditsFromQuest(rew.Credits);
                        }
                        if (rew.Silver > 0)
                        {
                            this.Items2_Give(null, rew.Silver);
                        }
                        foreach (Fbg.Bll.Player.QuestReward.PFWithDurationQuestReward reward in rew.PFWithDuration)
                        {
                            this.Items2_Give(null, reward.PFPackageID, reward.DurationInMinutes);
                        }
                        foreach (Fbg.Bll.Player.QuestReward.TroopQuestReward reward in rew.Troops)
                        {
                            this.Items2_Give(null, reward.UnitType, reward.Amount);
                        }                        
                    }
                }
            }

            return q;
        }

       
        private long Level
        {
            get
            {
                return this.Title.Level;
            }
        }
    }
}
