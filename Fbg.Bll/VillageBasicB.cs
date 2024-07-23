using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using Gmbc.Common.Diagnostics.ExceptionManagement;

using Fbg.Common;

namespace Fbg.Bll
{
    public class VillageBasicB : VillageBasicA, ISerializableToNameValueCollection
    {
        internal class CONSTS
        {
            public class TableIndex
            {
                public static int VillageBasicInfo = 0;
                public static int MaxPop = 1;
                public static int CurPop = 2;
                public static int AreCoinTransportsAvail = 3;
                public static int Tags = 4;
            }

            public class VillageBasicInfo
            {
                public static int Name = 0;
                public static int Coins = 1;
                public static int Points = 2;
                public static int XCord = 3;
                public static int YCord = 4;
                public static int loyalty = 5;
                public static int CoinMineLevel=6;
                public static int TreasuryLevel=7;
                public static int CoinsLastUpdates = 8;
                public static int VillageTypeID = 9;
            }

        }

        bool _areTransportsToThisVillageAvailable;
        private int _maxPopulation = 0;
        protected int _currentPopulation = 0;
        DataTable dtTags;


        /// <summary>
        /// may return null if no such village for this owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="villageID"></param>
        /// <returns></returns>
        public static VillageBasicB GetVillage(Player owner, int villageID, bool getAreTransportsAvail)
        {
            DataSet ds = DAL.Villages.VillageInfo(owner.Realm.ConnectionStr, owner.ID, villageID,  getAreTransportsAvail);

            if (ds.Tables.Count <= 1)
            {
                return null;
            }

            return new VillageBasicB(owner, villageID, ds);
        }

        protected VillageBasicB(Player owner, int id, DataSet ds)
            : base(owner, id)
        {

            DataTable basicInfo = ds.Tables[CONSTS.TableIndex.VillageBasicInfo];


            Init((string)basicInfo.Rows[0][CONSTS.VillageBasicInfo.Name]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.Coins]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.Points]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.XCord]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.YCord]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.loyalty]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.CoinMineLevel]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.TreasuryLevel]
                , (DateTime)basicInfo.Rows[0][CONSTS.VillageBasicInfo.CoinsLastUpdates]
                , Convert.ToInt32(Math.Floor((Convert.ToDouble(ds.Tables[CONSTS.TableIndex.MaxPop].Rows[0][0]))))
                , Convert.ToInt32(ds.Tables[CONSTS.TableIndex.CurPop].Rows[0][0])
                , Convert.ToBoolean(ds.Tables[CONSTS.TableIndex.AreCoinTransportsAvail].Rows[0][0])
                , (short)basicInfo.Rows[0][CONSTS.VillageBasicInfo.VillageTypeID]
                , ds.Tables[CONSTS.TableIndex.Tags]);
        }

        /// <summary>
        /// When calling this constructor, you MUST call Init after or the object will not be fully initialized!!1
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="id"></param>
        public VillageBasicB(Player owner, int id, bool loadData)
            : base(owner, id)
        {
            if (loadData) 
            {
                throw new InvalidOperationException("only value of false is allowed for loadData param");
            }
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
        , int maxPopulation
        , int currentPopulation
        , bool areTransportsToThisVillageAvailable
        , short villageTypeID
        , DataTable tags)
        {
            base.Init(name, coins, points, xcord, ycord, loyalty, coinMineLevel, treasuryLevel, coinsLastUpdateOn, villageTypeID);

            //float villageBonus = this.VillageType.Bonus(owner.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Farmland));
            //float researchBonus = this.owner.MyResearch.Bonus(owner.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Farmland));

            _maxPopulation = maxPopulation;
            _currentPopulation = currentPopulation;
            _areTransportsToThisVillageAvailable = areTransportsToThisVillageAvailable;
            dtTags = tags; ;

        }



        public virtual void Init(
         VillageBasicB vtocopy)
        {
            base.Init(vtocopy.name, vtocopy.coins, vtocopy.Points, vtocopy.xcord, vtocopy.ycord, vtocopy.Loyalty, vtocopy._coinMineLevel, vtocopy._treasuryLevel, vtocopy._coinsLastUpdateOn, vtocopy.VillageType.ID);

            //float villageBonus = this.VillageType.Bonus(owner.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Farmland));
            //float researchBonus = this.owner.MyResearch.Bonus(owner.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Farmland));

            _maxPopulation = vtocopy.MaxPopulation;
            _currentPopulation = vtocopy.CurrentPopulation;
            _areTransportsToThisVillageAvailable = vtocopy.AreTransportsToThisVillageAvailable;
            dtTags = vtocopy.dtTags ;

        }


        public int CurrentPopulation
        {
            get
            {
                
                return _currentPopulation;
            }
        }

        public int MaxPopulation
        {
            get
            {
                
                return _maxPopulation;
            }
        }
        public int RemainingPopulation
        {
            get
            {
                
                return _maxPopulation - _currentPopulation;
            }
        }

        /// <summary>
        /// indicates if this village can receive transports of coins from other villages
        /// </summary>
        /// <returns></returns>
        public bool AreTransportsToThisVillageAvailable
        {
            get
            {
                if (coins < TreasurySize)
                {

                    return _areTransportsToThisVillageAvailable;
                }
                else
                {
                    // treasury is full so no. 
                    return false;
                }
            }
        }

        private static string TagSearch = "TagID = {0}";
        public bool HasTag(int tagID)
        {
            if (dtTags.Rows.Count > 0)
            {
                return dtTags.Select(String.Format(TagSearch, tagID)).Length == 0 ? false : true;
            }
            return false;
        }

        #region ISerializableToNameValueCollection Members

        public new void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            try
            {
                string pre = "Village[" + id.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in VillageBasicB.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    base.SerializeToNameValueCollection(col);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_areTransportsToThisVillageAvailable", _areTransportsToThisVillageAvailable);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_maxPopulation", _maxPopulation);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_currentPopulation", _currentPopulation);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in VillageBasicB.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion





    }
}
