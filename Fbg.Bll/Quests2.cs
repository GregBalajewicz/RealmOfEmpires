using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BDA.Achievements
{
    public abstract class QuestReward
    {

    }

    public class Quests : IEnumerable
    {
        public delegate void SetCompletedDelegate(int id);
        public delegate DateTime CompletedOnDelegate(int id);
        /// <summary>
        /// return true if reward was not set as claimed before, returns false if it was set claimed before. 
        /// in either case, the reward is set as claimed. 
        /// </summary>
        public delegate bool SetRewardClaimedDelegate(int id);
        public delegate bool IsRewardClaimedDelegate(int id);
        /// <summary>
        /// this method is called when we refresh the list of "CompletedQuests_RewardNotClaimed". it is called once 
        ///     and before we call the first Quest.EvaluateCompleted. Consumer may use this method to initialize anything that needs to be initialized once
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public delegate void PrepareForEvaluateCompletedCallDelegate();
        SetRewardClaimedDelegate _setRewardClaimedDelegate;
        IsRewardClaimedDelegate _isRewardClaimedDelegate;
        CompletedOnDelegate _completedOnDelegate;
        SetCompletedDelegate _setCompletedDelegate;
        PrepareForEvaluateCompletedCallDelegate _prepareForEvaluateCompletedCallDelegate;
        public Quests(int achievementsIDStartFrom
            , CompletedOnDelegate completedOnDelegate, SetCompletedDelegate setCompletedDelegate
            , SetRewardClaimedDelegate setRewardClaimedDelegate, IsRewardClaimedDelegate isRewardClaimedDelegate
            , PrepareForEvaluateCompletedCallDelegate prepareForEvaluateCompletedCallDelegate
            , QuestTemplates templates)
        {
            _allQuests = new List<Quest>();
            _completedOnDelegate = completedOnDelegate;
            _achievementsIDStartFrom = achievementsIDStartFrom;
            _achievementCurID = achievementsIDStartFrom;
            _setCompletedDelegate = setCompletedDelegate;
            _isRewardClaimedDelegate = isRewardClaimedDelegate;
            _setRewardClaimedDelegate = setRewardClaimedDelegate;
            _prepareForEvaluateCompletedCallDelegate = prepareForEvaluateCompletedCallDelegate;
            _templates = templates;


        }
        QuestTemplates _templates;
        List<Quest> _allQuests;
        public List<Quest> AllQuests
        {
            get { return _allQuests; }
            set { _allQuests = value; }
        }

        #region NextQuestsInProgression
        List<Quest> _nextQuestsInProgression;
        bool _nextQuestsInProgression_Initalized = false;
        public void NextQuestsInProgression_Invalidate()
        {
            _nextQuestsInProgression_Initalized = false;
        }
        public List<Quest> NextQuestsInProgression
        {
            get
            {
                if (!_nextQuestsInProgression_Initalized)
                {
                    _nextQuestsInProgression_Initalized = true;
                    #region NextQuestsInProgression not initialized, init now
                    Quest q;
                    Quest q2;
                    List<Quest> notCompletedDependants;
                    List<Quest> _notCompletedNonMandaroyQuestsFromPrevProgressionStep;

                    /*
                     * The algorithm goes as follows:
                     *  we loop over the quest progressions - this is a list of list of quests. 
                     *  with each list of quests, we see if at least one of those quests in the list is not completed.
                     *      if so, we consider this list to be our current quests in the progression
                     *      we then also loop through the completed quests in this progression and see if perhaps a quest that depends on this one is not completed. if so, add it.                      
                    */
                    _nextQuestsInProgression = new List<Quest>();
                    _notCompletedNonMandaroyQuestsFromPrevProgressionStep = new List<Quest>();
                    bool foundRequiredNotCompletedQuestInProgression;
                    foreach (QuestProgressionItem[] questProgressionItems in QuestProgressionByTag)
                    {
                        foundRequiredNotCompletedQuestInProgression = false;

                        // Tag A.1
                        // if we are starting a new progression step, but we still got quests from previous progression step in _nextQuestsInProgression,
                        //  that means it is some nonmandatory quest from prvious progression is there. those kinds of quests, lets put at the bottom of the list\
                        //  so we remove them, save them in a different list, then copy them to the list of recommended quests at the bottom
                        //      See Tag A.2 for more info
                        if (_nextQuestsInProgression.Count > 0)
                        {
                            _notCompletedNonMandaroyQuestsFromPrevProgressionStep.AddRange( _nextQuestsInProgression);
                            _nextQuestsInProgression.Clear();
                        }


                        //
                        // in this loop, we are looking for any not completed quests
                        //
                        foreach (QuestProgressionItem questProgressionItem in questProgressionItems)
                        {
                            q = GetQuestByTag(questProgressionItem.Tag);

                            //TODO: this chnaged just for now  
                            //if (q == null) throw new NotImplementedException(questProgressionItem.ToString());
                            if (q == null) continue;



                            if (!q.IsCompleted)
                            {
                                // if this is a mandatory quest for this progression, then we stop at this progression till this quest is complted. 
                                //  if not, then we note it, and move on. we note that we found such a quest here
                                if (questProgressionItem.IsMandatory)
                                {
                                    foundRequiredNotCompletedQuestInProgression = true;
                                }
                                // quest is not completed so it certainly should be recommended to do. 
                                //  however, if it depends on another quest that is not completed, then prefer the not completed parent, and so on
                                q2 = GetFirstNotCompletedDependat(q);
                                if (!_nextQuestsInProgression.Exists(delegate(Quest q3) { return q3 == q2; }))
                                {
                                    _nextQuestsInProgression.Add(q2);
                                }
                                //}
                                //else
                                //{
                                //    // this is not a  mandatory quest for this progression, then we note it, and move on
                                //    q2 = GetFirstNotCompletedDependat(q);
                                //    if (!_notCompletedNonMandaroyQuestsFromPrevProgressionStep.Exists(delegate(Quest q3) { return q3 == q2; }))
                                //    {
                                //        _notCompletedNonMandaroyQuestsFromPrevProgressionStep.Add(q2);
                                //    }
                                //}
                            }

                        }

                        //
                        // if we got at least one not completed required quest, then we know we will choose these quests as next quests in the progression. 
                        //  so now lets again loop over those quests, but this time check the completed ones and see if perhaps a quest that depends on the 
                        //  completed one is not completed, in which case add this one as well 
                        //
                        if (foundRequiredNotCompletedQuestInProgression)
                        {
                            foreach (QuestProgressionItem questTag in questProgressionItems)
                            {
                                q = GetQuestByTag(questTag.Tag);

                                //TODO: this is just for now  
                                if (q == null) continue;



                                if (q.IsCompleted && questTag.CompleteBehaviour == QuestProgressionItem.CompletBehaviourEnum.mandatory_showNextQuestWhenCompleted)
                                {
                                    // quest is completed. lets see if a child quest (quest that depends on this one) or a child of a child and so on, are not completed. if so, display it. 
                                    notCompletedDependants = new List<Quest>();
                                    GetNotCompletedDependantQuestOrItsChild(q, ref notCompletedDependants);
                                    foreach (Quest childNotCompleted in notCompletedDependants)
                                    {
                                        if (!_nextQuestsInProgression.Exists(delegate(Quest q3) { return q3 == childNotCompleted; }))
                                        {
                                            _nextQuestsInProgression.Add(childNotCompleted);
                                        }
                                    }
                                }
                            }
                        }
                        //
                        // if we got at least one quest, then we are good to go, this is our list of next quests to just break from the loop.
                        //
                        if (foundRequiredNotCompletedQuestInProgression)
                        {
                            break;
                        }
                    }


                    // Tag A.2
                    // continuation from Tag A.1 - now adding not completed nonmandartory quests from previous progression to recommended quests, at the bottom
                    foreach (Quest qNotCompletedNonMandatory in _notCompletedNonMandaroyQuestsFromPrevProgressionStep)
                    {
                        if (!_nextQuestsInProgression.Exists(delegate(Quest q3) { return q3 == qNotCompletedNonMandatory; }))
                        {
                            _nextQuestsInProgression.Add(qNotCompletedNonMandatory);
                        }
                    }

                    #endregion

                }



                return _nextQuestsInProgression;
            }
        }
        #endregion


        /// <summary>
        /// </summary>
        /// <param name="q"></param>
        /// <param name="notCompletedDependants"></param>
        private void GetNotCompletedDependantQuestOrItsChild(Quest q, ref List<Quest> notCompletedDependants)
        {
            if (!q.IsCompleted)
            {
                notCompletedDependants.Add(q);
            }
            else
            {
                // find any children and return the first not completed on
                List<Quest> l = _allQuests.FindAll(delegate(Quest q2) { return q2.DependentQuest == null ? false : q2.DependentQuest == q; });
                foreach (Quest q3 in l)
                {
                    GetNotCompletedDependantQuestOrItsChild(q3, ref notCompletedDependants);
                }
            }
        }


        private Quest GetFirstNotCompletedDependat(Quest q)
        {
            if (q.DependentQuest != null)
            {
                if (!q.DependentQuest.IsCompleted)
                {
                    return GetFirstNotCompletedDependat(q.DependentQuest);
                }
            }
            if (q.IsCompleted)
            {
                // even this quest if completed so return null - no not completed quests 
                return null;
            }
            else
            {
                // all dependants, if any, are completed and this quest it not completed so return it
                return q;
            }
        }

        List<Quest> _completedQuests_RewardNotClaimed;
        bool _completedQuests_RewardNotClaimed_Refresh=false;
        public List<Quest> CompletedQuests_RewardNotClaimed()
        {
            if (_completedQuests_RewardNotClaimed == null || _completedQuests_RewardNotClaimed_Refresh)
            {
                _completedQuests_RewardNotClaimed_Refresh = false;
                _completedQuests_RewardNotClaimed = new List<Quest>();
                _prepareForEvaluateCompletedCallDelegate();
                foreach (Quest a in _allQuests)
                {
                    a.EvaluateCompleted();
                    if (a.IsCompleted && !a.IsRewardClaimed)
                    {
                        _completedQuests_RewardNotClaimed.Add(a);
                    }
                }
            }

            return _completedQuests_RewardNotClaimed;
        }
        public void CompletedQuests_RewardNotClaimed_Invalidate()
        {
            _completedQuests_RewardNotClaimed_Refresh = true;
        }

        int _achievementsIDStartFrom;
        int _achievementCurID;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="title"></param>
        /// <param name="goal">if you pass null, title will be used for goal. ie Goal = title</param>
        /// <param name="dependant"></param>
        /// <param name="reward"></param>
        /// <param name="evaluateCompleted"></param>
        /// <param name="desc"></param>
        public void AddQuest(QuestTemplate template, Quest.EvaluateCompletedDelegate evaluateCompleted)
        {
            Quest a = new Quest(_achievementCurID, this);
            _achievementCurID++;
            a.QuestTemplate = template;
            a.CompletedOn = _completedOnDelegate;
            a.EvaluateCompletedDlg = evaluateCompleted;
            a.SetCompleted = _setCompletedDelegate;
            a.IsRewardClaimedDelegate = _isRewardClaimedDelegate;
            a.SetRewardClaimedDelegate = _setRewardClaimedDelegate;
            _allQuests.Add(a);
        }


        public Quest GetQuestByID(int questID)
        {
            return AllQuests.Find(delegate(Quest q) { return q.ID == questID; });
        }
        public Quest GetQuestByTag(string questTag)
        {
            return AllQuests.Find(delegate(Quest q) { return q.Tag == questTag; });
        }


        public List<QuestProgressionItem[]> QuestProgressionByTag { get { return _templates.QuestProgressionByTag; } }

        public IEnumerator GetEnumerator()
        {
            return AllQuests.GetEnumerator();
        }
              
        public void SetQuestAsCompleted(string questTags)
        {
            Quest q = GetQuestByTag(questTags);
            if (q != null)  // this IF is there because some quests are not available on all realms
            {               
                if (!q.IsRewardClaimed)
                {
                    q.SetCompleted(q.ID);
                    CompletedQuests_RewardNotClaimed_Invalidate();
                    NextQuestsInProgression_Invalidate();
                }
            }
        }


        public enum QuestUpdateStatus
        {
            Completed=1,
            NotAvailable,
            RewardAlreadyClaimed
        }

        public QuestUpdateStatus SetQuestAsCompletedWithResult(string questTag)
        {
            Quest q = GetQuestByTag(questTag);
            if (q != null)  // this IF is there because some quests are not available on all realms
            {
                if (!q.IsRewardClaimed) {
                    q.SetCompleted(q.ID);
                    CompletedQuests_RewardNotClaimed_Invalidate();
                    NextQuestsInProgression_Invalidate();

                    return QuestUpdateStatus.Completed;
                }
                else {
                    return QuestUpdateStatus.RewardAlreadyClaimed;
                }
            }
            else {
                return QuestUpdateStatus.NotAvailable;
            }
        }
    }

    public class Quest
    {
        // reward, sequence spot, dependant, group
        //public int TagOrdinal { get { return 1; /* FIX THIS FIX THIS FIX THIS FIX THIS FIX THIS FIX THIS FIX THIS FIX THIS */ } }
        public string Tag { get { return QuestTemplate.Tag; } }
        public int ID { get; set; }
        public string Title { get {return QuestTemplate.Title;} }
        public string Goal { get { return QuestTemplate.Goal; } }
        public string Description { get { return QuestTemplate.Description; } }
        public QuestReward Reward { get { return QuestTemplate.Reward; } }
        public Quest DependentQuest
        {
            get
            {

                return QuestTemplate.DependentQuestTemplate == null ? null : _quests.GetQuestByTag(QuestTemplate.DependentQuestTemplate.Tag);
            }
        }
     
        internal QuestTemplate QuestTemplate { get; set; }

        public delegate bool EvaluateCompletedDelegate(string tag);
        internal EvaluateCompletedDelegate EvaluateCompletedDlg { get; set; }
        internal Quests.CompletedOnDelegate CompletedOn { get; set; }
        internal Quests.SetCompletedDelegate SetCompleted { get; set; }
        internal Quests.IsRewardClaimedDelegate IsRewardClaimedDelegate { get; set; }
        internal Quests.SetRewardClaimedDelegate SetRewardClaimedDelegate { get; set; }

        public bool EvaluateCompleted()
        {
            if (!IsCompleted)
            {
                if (EvaluateCompletedDlg(Tag))
                {
                    SetCompleted(ID);
                    _quests.NextQuestsInProgression_Invalidate();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        Quests _quests;
        public Quest(int id, Quests quests)
        {
            ID = id;
            _quests = quests;
        }


        public bool IsCompleted
        {
            get
            {
                return CompletedOn(ID) == DateTime.MinValue ? false : true;
            }
        }

        public bool IsRewardClaimed
        {
            get
            {
                return IsRewardClaimedDelegate(ID);
            }
        }

        internal bool SetRewardClaimed()
        {
            _quests.CompletedQuests_RewardNotClaimed_Invalidate();
            return SetRewardClaimedDelegate(ID);
        }

        /// <summary>
        /// return true if reward was not set as claimed before, returns false if it was set claimed before. 
        /// in either case, the reward is set as claimed. 
        /// </summary>
        /// <returns></returns>
        public bool ClaimReward()
        {
            return SetRewardClaimed();
        }

        public bool HasDependants
        {
            get
            {
                return _quests.AllQuests.Exists(delegate(Quest q) { return q.DependentQuest == this; });
            }
        }



        public string DescriptionM
        {
            get
            {
                return QuestTemplate.GetProperDesc(0);
            }
        }
        public string DescriptionD2
        {
            get
            {
                return QuestTemplate.GetProperDesc(1);
            }
        }

      
    }




    public class QuestTemplate
    {
        // reward, sequence spot, dependant, group
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Goal { get; set; }
        public string Description { get; set; }
        public QuestReward Reward { get; set; }
        public QuestTemplate DependentQuestTemplate { get; set; }
        public Dictionary<int, string> Descriptions{ get; set; }


        public string GetProperDesc(int uiTypeID)
        {
            string desc;
            if (!Descriptions.TryGetValue(uiTypeID, out desc))
            {
                desc = Description;
            }

            return desc;
        }
    }



    public class QuestTemplates : IEnumerable
    {
        public QuestTemplates()
        {
            _allQuestTemplates = new List<QuestTemplate>();

        }

        List<QuestTemplate> _allQuestTemplates;
        public List<QuestTemplate> AllQuestTemplates
        {
            get { return _allQuestTemplates; }
            set { _allQuestTemplates = value; }
        }

  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="title"></param>
        /// <param name="goal">if you pass null, title will be used for goal. ie Goal = title</param>
        /// <param name="dependant"></param>
        /// <param name="reward"></param>
        /// <param name="evaluateCompleted"></param>
        /// <param name="desc"></param>
        public void AddQuestTemplate(string tag, string title, string goal, QuestTemplate dependant, QuestReward reward, string desc)
        {
            QuestTemplate a = new QuestTemplate();
            a.Title = title;
            a.Goal = goal == null ? title : goal;
            a.Description = desc;
            a.Tag = tag;
            a.Reward = reward;
            a.DependentQuestTemplate = dependant;
            _allQuestTemplates.Add(a);
        }

        public void AddQuestTemplate(QuestTemplate qt)
        {
           
            _allQuestTemplates.Add(qt);
        }


        public QuestTemplate GetQuestTemplateByTag(string questTag)
        {
            return AllQuestTemplates.Find(delegate(QuestTemplate q) { return q.Tag == questTag; });
        }




        public IEnumerator GetEnumerator()
        {
            return AllQuestTemplates.GetEnumerator();
        }
        public List<QuestProgressionItem[]> QuestProgressionByTag { get; set; }

    }



    /// <summary>
    /// One quest in quest progression. 
    /// </summary>
    public class QuestProgressionItem
    {
        public enum CompletBehaviourEnum
        {
            optional_showInNextProgression = 0,
            mandatory_showNextQuestWhenCompleted = 1,
            mandatory_doNotShowNextQuestWhenCompleted = 2
        }

        public string Tag { get; set; }
        public CompletBehaviourEnum CompleteBehaviour { get; set; }
        public bool IsMandatory
        {
            get
            {
                return CompleteBehaviour == CompletBehaviourEnum.mandatory_doNotShowNextQuestWhenCompleted || CompleteBehaviour == CompletBehaviourEnum.mandatory_showNextQuestWhenCompleted;
            }
        }
        public QuestProgressionItem(string tag, int completeBehaviour)
        {
            Tag = tag;
            CompleteBehaviour = (CompletBehaviourEnum)completeBehaviour;
        }

    }



}
