using System;
using System.Collections.Generic;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Globalization;

namespace Fbg.Bll
{
    public class VillageBasicA : VillageBase, ISerializableToNameValueCollection
    {

        /// <summary>
        /// Signals that village's coins have changed.
        /// The village.coins property has the correct coin count
        /// </summary>
        public delegate void CoinsUpdated(VillageBasicA village);
        public event CoinsUpdated CoinsUpdatedEvent;

        /// <summary>
        /// this is the same number as villages.coins but it may not be the correct number, use _coins_actual instead
        /// </summary>
        protected int _coins_raw;
        /// <summary>
        /// this is the correct number of coins in this village
        /// </summary>
        protected int _coins_actual = Int32.MinValue;
        protected DateTime _lastTimeCoinsUpdated;
        protected DateTime _bonusSilverPFExpiresOn;
        protected int _points;
        protected int _loyalty;
        protected int _coinMineLevel;
        protected int _treasuryLevel;
        protected DateTime _coinsLastUpdateOn;
        protected float _perHourBonusIncome;
        protected float _perHourBaseIncome;
        protected float _perHourIncome;
        protected int _treasurySize;
        protected string _name;
        protected System.Drawing.Point _cordinates;
        float _treasuryResearchBonus;

        /// <summary>
        /// Fully initializes the object. 
        /// </summary>
        public VillageBasicA(Player owner
            , string name
            , int id
            , int coins
            , int points
            , int xcord
            , int ycord
            , int loyalty
            , int coinMineLevel
            , int treasuryLevel
            , DateTime coinsLastUpdateOn
            , short villageTypeID)
            : base(owner, id)
        {
            Init(name, coins, points, xcord, ycord, loyalty, coinMineLevel, treasuryLevel, coinsLastUpdateOn, villageTypeID);
        }


        /// <summary>
        /// When calling this constructor, you MUST call Init after or the object will not be fully initialized!!1
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="id"></param>
        public VillageBasicA(Player owner, int id)
            : base(owner, id)
        {
        }


        protected virtual void Init(
         string name
        , int coins
        , int points
        , int xcord
        , int ycord
        , int loyalty
        , int coinMineLevel
        , int treasuryLevel
        , DateTime coinsLastUpdateOn
            , short villageTypeID)
        {
           

            this._name = name;
            this._coins_raw = coins;
            this._points = points;
            _loyalty = loyalty > 100 ? 100 : loyalty;
            _cordinates = new System.Drawing.Point(xcord, ycord);
            _coinMineLevel = coinMineLevel;
            _treasuryLevel = treasuryLevel;
            _coinsLastUpdateOn = coinsLastUpdateOn;

            _perHourBaseIncome = Convert.ToSingle(this.owner.Realm.BuildingType_CoinMine.Level(_coinMineLevel).Effect, CONSTS.ciUS);
            _treasurySize = this.owner.Realm.BuildingType_Treasury.Level(_treasuryLevel).EffectAsInt;
            _treasuryResearchBonus = this.owner.MyResearch.Bonus(this.owner.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Treasury));
            _treasurySize = Convert.ToInt32(_treasurySize * (1 + _treasuryResearchBonus), CONSTS.ciUS);

            _bonusSilverPFExpiresOn = owner.PF_PFExpiresOn(CONSTS.PFs.SilverBonus);
            VillageType = owner.Realm.VillageTypes[villageTypeID];
            //
            // calculate the actual coin income
            //
            // research bonus
            _perHourIncome = _perHourBaseIncome * (_owner.MyResearch.CoinBonus + 1) * (VillageType.Bonus(owner.Realm.BuildingType_CoinMine)+1);

            // PF bonus
            if (_bonusSilverPFExpiresOn >= DateTime.Now)
            {
                _perHourBonusIncome = _perHourIncome * CONSTS.PF_SilverBonusPercent;
                _perHourIncome = _perHourIncome + _perHourBonusIncome;
            }

            
        }


        /// <summary>
        /// add coins to this village.
        /// Passing in a negative number is NOT IMPLEMENTED.
        /// This results in an immediate call to the DB. 
        /// </summary>
        /// <param name="coins">amount of coins to add. must be > 0</param>
        public void UpdateCoins(int coins, out int coinsOverflow)
        {
            this.coins = DAL.Villages.UpdateCoins(owner.Realm.ConnectionStr, id, coins, out coinsOverflow);
            if (CoinsUpdatedEvent != null)
            {
                CoinsUpdatedEvent(this);
            }
        }


        public VillageTypes.VillageType VillageType
        {
            get;
            internal set;
        }
        /// <summary>
        /// coins produced at this village per hour
        /// </summary>
        public float PerHourCoinIncome
        {
            get
            {
                return _perHourIncome;
            }
        }

        public int TreasurySize
        {
            get
            {
                return _treasurySize;
            }
        }

        /// <summary>
        /// give you the coins in this village. NOTE THAT this number changes in real time therefore
        ///  it is not a cheap property call - cache the value rather than calling it many times
        /// </summary>
        public int coins
        {
            get
            {
                #region GET

                if (_coins_actual == Int32.MinValue)
                {
                    _coins_actual = _coins_raw;
                }
                if (_coins_actual < TreasurySize)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan ts = now.Subtract(_coinsLastUpdateOn);
                    if (ts.TotalSeconds >= 1)
                    {
                        int change;
                        if (_coinsLastUpdateOn < _bonusSilverPFExpiresOn && _bonusSilverPFExpiresOn < now)
                        {
                            #region Need to do 2 seperate duration calculations
                            //
                            // this is a special situation where coins were last updated BEFORE the bonus silver PF has expired 
                            //  (therefore meaning that the coins must be updated with a bonus from the time of last update to the PF expiry date) 
                            // AND bonus PF feature expired already meaning that from the time it expired till now, we must update the 
                            //  coins without the bonus from expiry date till now. 
                            //
                            // IF PF expired now of after now, then its not a problem since we just do 1 calculation. this is handled by the else clause. here we
                            //  deal with the more difficult case
                            //
                            // IF PF expired before coins where last updated, then again no problem since the bonus has allready been applied. 
                            //

                            //
                            // STEP 1 - update coins for the duration: FROM (last coins updated) TO (PF expiry date)
                            //
                            ts = _bonusSilverPFExpiresOn.Subtract(_coinsLastUpdateOn);
                            change = Convert.ToInt32(Math.Floor((_perHourBaseIncome * (_owner.MyResearch.CoinBonus + 1) * (1 + CONSTS.PF_SilverBonusPercent) / 3600) * Convert.ToInt32(Math.Floor(ts.TotalSeconds))));

                            //
                            // STEP 2 - update coins for the duration: FROM (PF expiry date) TO (now)
                            //
                            ts = now.Subtract(_bonusSilverPFExpiresOn);
                            change += Convert.ToInt32(Math.Floor((_perHourBaseIncome * (_owner.MyResearch.CoinBonus + 1) / 3600) * Convert.ToInt32(Math.Floor(ts.TotalSeconds))));

                            _coinsLastUpdateOn = now;
                            if (_coins_actual + change <= TreasurySize)
                            {
                                _coins_actual = _coins_actual + change;
                            }
                            else
                            {
                                _coins_actual = TreasurySize;
                            }
                            #endregion
                        }
                        else
                        {
                            #region Do a simply duration change calculation
                            _coinsLastUpdateOn = now;
                            change = Convert.ToInt32(Math.Floor((PerHourCoinIncome / 3600) * Convert.ToInt32(Math.Floor(ts.TotalSeconds))));
                            if (_coins_actual + change <= TreasurySize)
                            {
                                _coins_actual = _coins_actual + change;
                            }
                            else
                            {
                                _coins_actual = TreasurySize;
                            }
                            #endregion 
                        }
                    }
                }
                else if (_coins_actual > TreasurySize)
                {
                    _coins_actual = TreasurySize;
                }

                return _coins_actual;
                #endregion
            }
            set
            {
                #region SET
                if (value <= TreasurySize)
                {
                    if (value < 0)
                    {
                        _coins_raw = 0;
                    }
                    else
                    {
                        _coins_raw = value;
                    }
                }
                else
                {
                    _coins_raw = TreasurySize;
                }
                _coins_actual = Int32.MinValue;
                _coinsLastUpdateOn = DateTime.Now;


                #endregion
            }

        }




        public string name
        {
            get
            {
                return _name;
            }
        }
        public int xcord
        {
            get
            {
                return this.Cordinates.X;
            }
        }
        public int ycord
        {
            get
            {
                return this.Cordinates.Y;
            }
        }
        public System.Drawing.Point Cordinates
        {
            get
            {
                return _cordinates;
            }
        }

        public int Points
        {
            get
            {
                return _points;
            }
        }
        public int Loyalty
        {
            get
            {
                return _loyalty;
            }
        }

        public enum CanResearchResult
        {
            OK=0,
            NO_AlreadyResearched,
            NO_ReqNotMet,
            NO_ResearchersBusy
        }

        /// <summary>
        /// research a particular item
        /// </summary>
        public CanResearchResult DoResearch(ResearchItem ri, int maxItemsResearching)
        {
            if (_owner.MyResearch.ResearchInProgress.Count >= maxItemsResearching)
            {
                return CanResearchResult.NO_ResearchersBusy;
            }

            if (_owner.MyResearch.ResearchItems.Exists(delegate(ResearchItem ri2) { return ri == ri2; }))
            {
                return CanResearchResult.NO_AlreadyResearched;
            }
            if (!_owner.MyResearch.AreAllRequirementsSatisfied(ri.Requirements))
            {
                return CanResearchResult.NO_ReqNotMet;
            }

            DAL.Villages.DoResearch(_owner.Realm.ConnectionStr, _owner.ID, this.id, ri.ResearchItemType.ID, ri.ID, maxItemsResearching);
            _owner.MyResearch_ForceRefresh();

            return CanResearchResult.OK;
        }

        /// <summary>
        /// research a particular item
        /// </summary>
        public CanResearchResult DoResearch(ResearchItem ri, bool isInFacebook)
        {            
            int maxItemsReearching = (isInFacebook ? (owner.Researchers_Bought() + owner.Researchers_Friends(false).Count) : owner.Researchers_Bought());
            maxItemsReearching++; // add me. 
            maxItemsReearching = Math.Min(owner.Realm.Research_MaxResearchersAllowed, maxItemsReearching);
            return DoResearch(ri, maxItemsReearching);
        }


        #region ISerializableToNameValueCollection Members

        public new void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            try
            {
                string pre = "Village[" + id.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in VillageBasicA.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    base.SerializeToNameValueCollection(col);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "owner", owner);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_coins_raw", _coins_raw);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_coins_actual", _coins_actual);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_lastTimeCoinsUpdated", _lastTimeCoinsUpdated);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_points", _points);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_loyalty", _loyalty);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_coinMineLevel", _coinMineLevel);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_treasuryLevel", _treasuryLevel);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_coinsLastUpdateOn", _coinsLastUpdateOn);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_perHourIncome", _perHourIncome);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_treasurySize", _treasurySize);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_name", _name);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_cordinates.X", _cordinates.X);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_cordinates.Y", _cordinates.Y);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in VillageBasicA.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion


    }
}
