using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BDA.Achievements;

namespace Fbg.Bll
{




    partial class Realm
    {

        BDA.Achievements.QuestTemplates _quests;
        public BDA.Achievements.QuestTemplates QuestTemplates
        {
            get
            {
                return _quests;
            }
        }


        private void PopupateQuestTemplates(DataTable dt, DataTable dtProgression, DataTable dtRewardTroops, DataTable dtRewardPFWithDuration, DataTable dtDesc)
        {
            PopupateQuestTemplates_new(dt, dtProgression, dtRewardTroops, dtRewardPFWithDuration, dtDesc);
        }



        private void PopupateQuestTemplates_new(DataTable dt, DataTable dtProgression, DataTable dtRewardTroops, DataTable dtRewardPFWithDuration, DataTable dtDesc)
        {


            _quests = new BDA.Achievements.QuestTemplates();


            string tagName;
            string title;
            string goal;
            string desc;
            string dependantQuestTagName;
            int? reward_silver;
            int? reward_credits;
            int? completeCondition_Building_Level;
            int? completeCondition_Building_ID;
            int? completeCondition_TitleLevel;
            int? completeCondition_NumVillages;
            int? completeCondition_ResearchItemID;
            BDA.Achievements.QuestTemplate template;
            DataRow[] rewards;

            foreach (DataRow dr in dt.Rows) {
                tagName = (string)dr["TagName"];
                title = dr["Title"] is DBNull ? null : (string)dr["Title"];
                goal = dr["Goal"] is DBNull ? null : (string)dr["Goal"];
                desc = dr["Description"] is DBNull ? null : (string)dr["Description"];
                dependantQuestTagName = dr["DependantQuestTagName"] is DBNull ? null : (string)dr["DependantQuestTagName"];
                reward_silver = dr["reward_silver"] is DBNull ? null : (int?)dr["reward_silver"];
                reward_credits = dr["reward_credits"] is DBNull ? null : (int?)dr["reward_credits"];
                completeCondition_Building_Level = dr["CompleteCondition_Building_Level"] is DBNull ? null : (int?)dr["CompleteCondition_Building_Level"];
                completeCondition_Building_ID = dr["CompleteCondition_Building_ID"] is DBNull ? null : (int?)dr["CompleteCondition_Building_ID"];
                completeCondition_TitleLevel = dr["CompleteCondition_TitleLevel"] is DBNull ? null : (int?)dr["CompleteCondition_TitleLevel"];
                completeCondition_NumVillages = dr["CompleteCondition_NumVillages"] is DBNull ? null : (int?)dr["CompleteCondition_NumVillages"];
                completeCondition_ResearchItemID = dr["CompleteCondition_ResearchItemID"] is DBNull ? null : (int?)dr["CompleteCondition_ResearchItemID"];


                if (completeCondition_TitleLevel != null) {
                    template = new QuestTemplate_Title(tagName
                        , TitleByLevel((int)completeCondition_TitleLevel)
                        );

                }
                else if (completeCondition_Building_ID != null) {
                    template = new QuestTemplate_BuildingLevel(tagName
                        , BuildingType((int)completeCondition_Building_ID).Level((int)completeCondition_Building_Level)
                       , desc ,title, goal);
                }
                else if (completeCondition_NumVillages != null) {
                    template = new QuestTemplate_NumVillages(tagName
                        , (int)completeCondition_NumVillages
                        );
                }
                else if (completeCondition_ResearchItemID != null) {
                    template = new QuestTemplate_Research(tagName
                        , title
                        , Research.ResearchItemByID(1, (int)completeCondition_ResearchItemID)
                        , desc
                   );
                }
                else {
                    template = new BDA.Achievements.QuestTemplate()
                    {
                        Tag = tagName,
                        Description = desc,
                        Goal = goal,
                        Title = title
                    };

    
                }


                template.DependentQuestTemplate = dependantQuestTagName == null ? null : _quests.GetQuestTemplateByTag(dependantQuestTagName);
                Player.QuestReward questReward = new Player.QuestReward()
                {
                    Silver = reward_silver == null ? 0 : (int)reward_silver,
                    Credits = reward_credits == null ? 0 : (int)reward_credits
                };

                rewards = findRowsByTagName(dtRewardTroops, tagName);
                if (rewards.Length > 0)
                {
                    questReward.Troops = new List<Player.QuestReward.TroopQuestReward>(rewards.Length);
                    foreach (DataRow drreward in rewards)
                    {
                        questReward.Troops.Add(new Player.QuestReward.TroopQuestReward() { UnitType = GetUnitTypesByID((int)drreward["UnitTypeID"]), Amount = (int)drreward["Amount"] });
                    }
                } 
                rewards = findRowsByTagName(dtRewardPFWithDuration, tagName);
                if (rewards.Length > 0)
                {
                    questReward.PFWithDuration = new List<Player.QuestReward.PFWithDurationQuestReward>(rewards.Length);
                    foreach (DataRow drreward in rewards)
                    {
                        questReward.PFWithDuration.Add(new Player.QuestReward.PFWithDurationQuestReward() { PFPackageID = (int)drreward["PFPackageID"], DurationInMinutes = (int)drreward["DurationInMinutes"] });
                    }
                }

                template.Reward = questReward;

                //
                // descriptions
                //  some quest auto generate descriptions so we only set this here if descriptions is not initialized. 
                //
                if (template.Descriptions == null)
                {
                    DataRow[] drDescriptions = findRowsByTagName(dtDesc, tagName);
                    template.Descriptions = new Dictionary<int, string>(drDescriptions.Length);
                    foreach (DataRow row in drDescriptions)
                    {
                        template.Descriptions.Add((int)row["UITypeID"], (string)row["Description"]);
                    }
                }

                _quests.AddQuestTemplate(template);

            }




            //
            //
            // QUEST progression
            //
            //
            List<QuestProgressionItem> tagsAtOneLevel = new List<QuestProgressionItem>(10);
            List<QuestProgressionItem[]> questProgressionByTag;
            questProgressionByTag = new List<QuestProgressionItem[]>(dtProgression.Rows.Count);

            int currentLevel = -1;
            foreach (DataRow dr in dtProgression.Rows) {
                // if on new level, add the tags to the progression and clear the list
                if (currentLevel != (int)dr["Step"]) {
                    if (tagsAtOneLevel.Count > 0) {
                        questProgressionByTag.Add(tagsAtOneLevel.ToArray());
                    }
                    tagsAtOneLevel.Clear();
                }

                currentLevel = (int)dr["Step"];
                tagsAtOneLevel.Add(new QuestProgressionItem((string)dr["TagName"], (dr["isMandatory"] is DBNull ? 1 : (Int16)dr["isMandatory"])));
            }
            if (tagsAtOneLevel.Count > 0) {
                questProgressionByTag.Add(tagsAtOneLevel.ToArray());
            }



            _quests.QuestProgressionByTag = questProgressionByTag;




        }

        private  DataRow[] findRowsByTagName(DataTable dt, string tagname)
        {
            return dt.Select(string.Format("TagName = '{0}'", tagname));
        }

        private void AddQuestOldQuest(BDA.Achievements.QuestTemplates _quests, Player.QuestTags tag, string title, string goal, BDA.Achievements.QuestTemplate dependant, BDA.Achievements.QuestReward reward, string desc)
        {
            _quests.AddQuestTemplate(tag.ToString(), title, goal, dependant, reward, desc);
        }

        private BDA.Achievements.QuestTemplate GetQuestByTagOldQuest(BDA.Achievements.QuestTemplates _quests, Player.QuestTags tag)
        {
            return _quests.GetQuestTemplateByTag(tag.ToString());
        }
      

    }


}
