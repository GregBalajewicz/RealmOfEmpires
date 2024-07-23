using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public class MyResearch
    {
        public class CONSTS
        {
            public class ColIndx
            {
                /// <summary>
                /// My Research item columns
                /// </summary>
                public class MyRIs
                {
                    public static int ResearchItemTypeID = 0;
                    public static int ResearchItemID = 1;
                }
                /// <summary>
                /// My Research item columns
                /// </summary>
                public class RIInProg
                {
                    public static int EventID = 0;
                    public static int EventTime = 1;
                    public static int ResearchItemTypeID = 2;
                    public static int ResearchItemID = 3;
                }
            }
        }

        DataTable _dtResearch;
        DataTable _dtResearchInProgress;
        Realm _realmRef;
        List<ResearchItem> _researchItems;
        List<ResearchItemInProgress> _researchInProgress;
        Player _playerRef;
        public MyResearch(Realm realmRef, Player plr, DataTable dtMyResearch,DataTable dtResearchInProgress)
        {
            _dtResearch = dtMyResearch;
            _dtResearchInProgress = dtResearchInProgress;
            _realmRef = realmRef;
            _playerRef = plr;
        }

        public bool IsCompleted(ResearchItem ri)
        {
            return ResearchItems.Exists(delegate(ResearchItem ri2) { return ri2 == ri; });
        }

        public bool IsAllReseachCompleted
        {
            get
            {
                return ResearchItems.Count == _realmRef.Research.AllResearchItems.Count;
            }
        }
        /// <summary>
        /// completed research items
        /// </summary>
        public List<ResearchItem> ResearchItems
        {
            get
            { 
                if (_researchItems == null)
                {
                    _researchItems = new List<ResearchItem>(_dtResearch.Rows.Count);
                    foreach (DataRow dr in _dtResearch.Rows)
                    {
                        _researchItems.Add(_realmRef.Research.ResearchItemByID((int)dr[CONSTS.ColIndx.MyRIs.ResearchItemTypeID]
                            , (int)dr[CONSTS.ColIndx.MyRIs.ResearchItemID]));
                    }
                }
                return _researchItems;
            }
        }

        public bool AreAllRequirementsSatisfied(List<ResearchItem> req)
        {
            return !req.Exists(delegate(ResearchItem ri){ return !IsCompleted(ri);});
        }

        /// <summary>
        /// if 1 then no bonus, otherwise, returns > 1 
        /// </summary>
        public float CoinBonusMultiplier
        {
            get
            {
                return CoinBonus + 1;
            }

        }
        /// <summary>
        /// 0 then no bonus, 0.1 means 10% bonus
        /// </summary>
        public float CoinBonus
        {
            get
            {
                return Bonus(_realmRef.BuildingType_CoinMine);
            }
        }


        Dictionary<BuildingType, decimal> _bonuses = new Dictionary<BuildingType, decimal>();
        /// <summary>
        /// 0 then no bonus, 0.1 means 10% bonus
        /// </summary>
        public float Bonus(BuildingType bt)
        {
            decimal bonus;

            if (!_bonuses.TryGetValue(bt, out bonus))
            {
                bonus = 0;
                List<ResearchItem> ris = ResearchItems.FindAll(delegate(ResearchItem ri) { return ri.Property_BuldingItEffects == bt; });

                foreach (ResearchItem ri in ris)
                {
                    bonus += Convert.ToDecimal(ri.PropertyAsFloat);
                }

                _bonuses.Add(bt, bonus);
            }

            return Convert.ToSingle(bonus);
        }

        /// <summary>
        /// 0 then no bonus, 0.1 means 10% bonus
        /// </summary>
        public float Bonus_DefenceFactor()
        {
            return Bonus(_playerRef.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower)) + Bonus(_playerRef.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Wall));
        }

        /// <summary>
        /// returns TimeSpan.MinValue if not in progress
        /// </summary>
        public TimeSpan IsResearchItemInProgress(ResearchItem ri)
        {
            ResearchItemInProgress riip = ResearchInProgress.Find(delegate(ResearchItemInProgress r) { return r.ri == ri; });
            if (riip == null)
            {
                return TimeSpan.MinValue;
            }
            else
            {
                return riip.completesOn.Subtract(DateTime.Now);
            }
        }

        public List<ResearchItemInProgress> ResearchInProgress
        {
            get
            {
                if (_researchInProgress == null)
                {
                    ResearchItemInProgress inp;
                    _researchInProgress = new List<ResearchItemInProgress>(_dtResearchInProgress.Rows.Count);
                    foreach (DataRow dr in _dtResearchInProgress.Rows)
                    {
                        inp = new ResearchItemInProgress()
                        {
                            ri = _realmRef.Research.ResearchItemByID((int)dr[CONSTS.ColIndx.RIInProg.ResearchItemTypeID]
                                , (int)dr[CONSTS.ColIndx.RIInProg.ResearchItemID])
                                ,
                            eventID = (Int64)dr[CONSTS.ColIndx.RIInProg.EventID]
                                ,
                            completesOn = (DateTime)dr[CONSTS.ColIndx.RIInProg.EventTime]
                        };
                        _researchInProgress.Add(inp);

                        if (researcherIdleIn == null)
                        {
                            researcherIdleIn = inp.completesOn;
                        }
                        else
                        {
                            if (researcherIdleIn > inp.completesOn)
                            {
                                researcherIdleIn = inp.completesOn;
                            }
                        }
                    }
                }

                return _researchInProgress;
            }
        }

        /// <summary>
        /// tells you, if there are researchers idle, hence you are able to do more research. 
        /// </summary>
        /// <param name="isFacebook">can be obtained via LoginModeHelper.isFB(Request) </param>
        /// <returns></returns>
        public bool areThereIdleResearchers(bool isFacebook)
        {
            return numOfIdleResearchers(isFacebook) > 0;
        }

        /// <summary>
        /// tells you, how many researchers are idle
        /// </summary>
        /// <param name="isFacebook">can be obtained via LoginModeHelper.isFB(Request) </param>
        /// <returns></returns>
        public int numOfIdleResearchers(bool isFacebook)
        {
            return Math.Max(0,_playerRef.Researchers_All(isFacebook) -  ResearchInProgress.Count );
        }

        /// <summary>
        /// tells you, if there are researchers idle, hence you are able to do more research. 
        /// </summary>
        /// <param name="isFacebook">can be obtained via LoginModeHelper.isFB(Request) </param>
        /// <returns></returns>
        public DateTime? researcherIdleIn
        {
            get;
            private set;
        }


        public class ResearchItemInProgress
        {
            public ResearchItem ri;
            public long eventID;
            public DateTime completesOn; 
        }


        /// <summary>
        ///  
        /// </summary>
        /// <param name="item2_ResearchSpeedup"></param>
        /// <param name="eventTimeBeforeSpeedUp">event time of the research item that was speed up; only valid if return is not 0</param>
        /// <returns>return 0 if not successful, or the ID of the research item that was speed up if successful</returns>
        internal int SpeedUp(Items2.Item2_ResearchSpeedup item2_ResearchSpeedup, out DateTime eventTimeBeforeSpeedUp)
        {
            return DAL.Players.ResearchSpeedUpUpgradeFromItem(this._playerRef.Realm.ConnectionStr, item2_ResearchSpeedup.AmountOfMinutesToSpeedUp, this._playerRef.ID, out eventTimeBeforeSpeedUp);
        }

        /// <summary>
        /// gets the # of hours a player is behind on doing research. CAREFUl! MAKES A DB EACH TIME!
        /// </summary>
        /// <returns></returns>
        public double HoursBehind()
        {
            return DAL.Players.ResearchHoursBehind(this._playerRef.Realm.ConnectionStr, this._playerRef.ID);
        }

        /// <summary>
        /// gets the # of hours a player is behind on doing research. CAREFUl! MAKES A DB EACH TIME!
        /// </summary>
        /// <returns></returns>
        public bool SpeedUp_ViaCatchup(out int researchItemIDSpedUp, out DateTime completionTimeBeforeSpeedUp, out int costOfTheCatchup)
        {

            researchItemIDSpedUp =  DAL.Players.SpeedUpCurrentlyResearchingViaCatchup(this._playerRef.Realm.ConnectionStr, this._playerRef.ID, out completionTimeBeforeSpeedUp, out costOfTheCatchup);
            return researchItemIDSpedUp != 0;
        }
    }
}
