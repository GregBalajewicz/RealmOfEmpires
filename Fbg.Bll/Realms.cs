using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public class Realms
    {
        internal class CONSTS
        {
            public class RealmsColumnIndex
            {
                public const int RealmID = 0;
                public const int Name = 1;
                public const int Description = 2;
                public const int PlayerCount = 3;
                public const int Version = 4;
                public const int ConnectionStr= 5;
                public const int MaxPlayers = 6;
                public const int OpenOn = 7;
                public const int AllowPrereg = 8;
                public const int ActivitStatus = 9;
                public const int ExtendedDesc = 10;
                public const int ClosingOn = 11;
                public const int PlayerGenerated_ByUserID = 12;
                public const int PlayerGenerated_Password = 13;

            }
            public class CreditPackages
            {
                public const int CreditsPackageID = 0;
                public const int Credits = 1;
                public const int RealmCost = 2;
            }

            public class TableIndex
            {
                public const int Realms = 0;
                public const int CreditPackages = 1;
                public const int Polls = 2;
                public const int PollOptions = 3;
                public const int CreditPackages_Device = 4;
                public const int Avatars = 5;
                //public const int AvatarAttributes = 6;
            }

        }

        private static List<Realm> realms;
        private static List<Realm> realmsReversed;
        private static DataTable dtRealms;
        private static DataTable dtCreditPackages;
        private static DataTable dtCreditPackagesDevice;
        private static DataTable dtPolls;
        private static DataTable dtPollOptions;

        private static DataSet ds;
        private static bool _loadCalled = false;
        private static bool _loadSuccessful = false;
        private static List< Poll> _polls = null;
        static readonly object padlock = new object();
        static Avatars _avatars;

        public enum ActiveStatus : short
        {
            Active,
            /// <summary>
            /// inactive realms are not even loaded to the realms collection so you will never have a realm object with this activestatus
            /// </summary>
            Inactive
        }

        public static int Count
        {
            get
            {
                return dtRealms.Rows.Count;
            }
        }


        public static List<Poll> Polls
        {
            get
            {
                return _polls;
            }
        }
        public static List<Realm> AllRealms
        {
            get
            {
                return realms;
            }
        }
        public static List<Realm> AllRealmsReversed
        {
            get
            {
                return realmsReversed;
            }
        }

        /// <summary>
        /// Will throw an exception if not found
        /// </summary>
        /// <param name="realmID"></param>
        /// <returns></returns>
        public static Realm Realm(int realmID)
        {
            return Realm(realmID, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realmID"></param>
        /// <param name="noException">if pass in true, will return null if realm not found rather than raising an exeption</param>
        /// <returns></returns>
        public static Realm Realm(int realmID, bool noException)
        {
            foreach (Realm r in AllRealms)
            {
                if (r.ID == realmID)
                {
                    return r;
                }
            }

            if (noException)
            {
                return null;
            }
            else
            {
                //
                // realm not found, throw exception
                //
                BaseApplicationException bex = new BaseApplicationException("Could not find realm with ID:" + realmID.ToString());
                Realms.SerializeToNameValueCollection(bex.AdditionalInformation);
                throw bex;
            }
        }

        public static Avatars Avatars
        {
            get
            {
                return _avatars;
            }
        }

        static Realms()
        {
            LoadData();
        }


        public static void ADMINONLY_REINIT()
        {           
            _loadSuccessful = false;
            LoadData();
        }

        private static void LoadData()
        {
            try
            {
                lock (padlock)
                {
                    if (!_loadSuccessful)
                    {
                        _loadCalled = true;
                        _loadSuccessful = false;
                        if (ds != null)
                        {
                            ds = null;
                        }

                        ds = DAL.Realm.GetAllRealms();

                        dtRealms = ds.Tables[CONSTS.TableIndex.Realms];
                        dtCreditPackages = ds.Tables[CONSTS.TableIndex.CreditPackages];
                        dtCreditPackagesDevice = ds.Tables[CONSTS.TableIndex.CreditPackages_Device];
                        dtPolls = ds.Tables[CONSTS.TableIndex.Polls];
                        dtPollOptions          = ds.Tables[CONSTS.TableIndex.PollOptions];
                        BuildPolls();

                        _avatars = new Avatars(ds.Tables[CONSTS.TableIndex.Avatars]);
                        
                        dtCreditPackages.PrimaryKey = new DataColumn[] { dtCreditPackages.Columns[CONSTS.CreditPackages.CreditsPackageID] };

                        ds.Relations.Add("PollOptions", dtPolls.Columns[Fbg.Bll.Poll.CONSTS.PollCloumnIndex.PollID]
                            , dtPollOptions.Columns[Fbg.Bll.PollOptions.CONSTS.PollOptionsCloumnIndex.PollID]);

                        if (dtRealms.Rows.Count < 1)
                        {
                            throw new Exception("dtRealms.Rows.Count < 1");
                        }

                        object o = Realms.CreditPackagesDevice(); // trigger creating the packages so that it is thread safe

                        BuildRealms();
                        _loadSuccessful = true;
                    }
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error in LoadData()", e);
                Realms.SerializeToNameValueCollection(ex.AdditionalInformation);
                throw ex;
            }
        }

        /// <summary>
        /// NOT thread safe. Call from within thread safe method only like LoadData()
        /// </summary>
        private static void BuildRealms()
        {

            realms = null;

            realms = new List<Realm>(dtRealms.Rows.Count);
            realmsReversed = new List<Realm>(dtRealms.Rows.Count);

            foreach (DataRow dr in dtRealms.Rows)
            {
                realms.Add(new Realm(dr));
                realmsReversed.Add(new Realm(dr));
            }
            //realmsReversed.Reverse();

            realmsReversed.Sort(delegate (Realm x, Realm y)
            {
                if (x.ID >= 0 || y.ID >= 0)
                {
                    return -(x.ID.CompareTo(y.ID));
                } else
                {
                    return x.OpenOn.CompareTo(y.OpenOn);
                }
            });
        }


        /// <summary>
        /// will return null if package with this ID does not exist
        /// </summary>
        /// <param name="creditPackageID"></param>
        /// <returns></returns>
        public static CreditPackage CreditPackage(int creditPackageID)
        {
            DataRow dr = dtCreditPackages.Rows.Find(creditPackageID);
            if (dr == null)
            {
                return null;
            }
            else
            {
                return new CreditPackage(dr); 
            }

        }

        static List<CreditPackageDevice> _creditPackagesDevice;       
        public static List<CreditPackageDevice> CreditPackagesDevice()
        {
          
                if (_creditPackagesDevice == null) {
                    _creditPackagesDevice = new List<CreditPackageDevice>();
                    foreach (DataRow dr in dtCreditPackagesDevice.Rows) {
                        _creditPackagesDevice.Add(new CreditPackageDevice(dr));
                    }
                }
                return _creditPackagesDevice;
            
        }

        public static List<CreditPackageDevice> CreditPackagesDevice(int saleType, int deviceType) 
        {
            return Realms.CreditPackagesDevice().FindAll(p => p.SaleType == saleType && p.DeviceType == deviceType);
        }

        public static CreditPackageDevice CreditPackageDevice(string productID)
        {
            return Realms.CreditPackagesDevice().Find(p => p.ProductID == productID);

        }


        private static void BuildPolls()
        {
            _polls = new List<Poll>();
            DataRow[] drOptions;
            foreach (DataRow drPoll in dtPolls.Rows)
            {
                 drOptions = dtPollOptions.Select(Fbg.Bll.Poll.CONSTS.PollCloumnName.PollID 
                    + "=" + drPoll[Fbg.Bll.Poll.CONSTS.PollCloumnIndex.PollID].ToString());
                 _polls.Add(new Poll(drPoll, drOptions));
            }
        }

        /// <summary>
        /// this check is valid ONLY for a brand new player, player not registered on any other realm. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>0 if this player name has been taken by someone else already. anyother value if name available.</returns>
        public static int RegisterPlayer_CheckNameOnly(string name, Guid userID)
        {
            return DAL.Realm.RegisterPlayer_CheckNameOnly(name, userID);
        }

        #region ISerializableToNameValueCollection Members

        public static void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            try
            {
                if (col == null)
                {
                    ExceptionManager.Publish("Error in Realms.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    col.Add("dtRealms", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtRealms, true));
                    BaseApplicationException.AddAdditionalInformation(col, "_loadCalled", _loadCalled);
                    BaseApplicationException.AddAdditionalInformation(col, "_loadSuccessful", _loadSuccessful);
                    col.Add("dtCreditPackages", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtCreditPackages, true));

                    if (realms == null)
                    {
                        BaseApplicationException.AddAdditionalInformation(col, "realms", "is null");
                    }
                    else
                    {
                        foreach (Realm r in realms)
                        {
                            BaseApplicationException.AddAdditionalInformation(col, "r" , r);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in Realms.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion
    }
}
