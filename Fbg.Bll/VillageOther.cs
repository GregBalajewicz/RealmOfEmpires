using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public class VillageOther : ISerializableToNameValueCollection
    {
        internal class CONSTS
        {
            public class VillageOtherTableIndex
            {
                public static int VillageID = 0;
            }

            public class VillageOtherColumnIndex
            {
                public static int VillageID = 0;
                public static int VillageName = 1;
                public static int XCord = 2;
                public static int YCord = 3;
                public static int Points = 4;
                public static int OwnerName = 5;
                public static int OwnerPlayerID = 6;
                public static int OwnersPoints = 7;
                public static int Clan = 8;
                public static int RegisteredOn = 9;
                public static int Note = 10;
                public static int ClanID = 11;
                public static int SleepModeActiveFrom = 12;
                public static int VillageTypeID = 13;
                public static int IsCapitalVillage = 14;
                public static int VacationModeRequestOn = 15;
                public static int VacationModeDaysUsed = 16;
                public static int OwnersXPCached = 17;
                public static int WeekendModeTakesEffectOn = 18;
            }
        }

        private Realm _realm;
        private System.Drawing.Point _cordinates;
        private DataRow _drVillage;
        private bool isUnderBeginnerProtection;
        private DateTime beginnerProtectionEndsOn;
        private DateTime capitalVillageProtectionEndsOn;


        /// <summary>
        /// returns null if village not found
        /// </summary>
        /// <returns></returns>
        public static VillageOther GetVillage(Realm realm, int xcord, int ycord) 
        {
            if (realm != null)
            {
                DataSet ds = DAL.Villages.VillageInfo_min(realm.ConnectionStr, xcord, ycord);

                if (ds.Tables.Count != 0 && ds.Tables[CONSTS.VillageOtherTableIndex.VillageID].Rows.Count != 0)
                {
                    if (ds.Tables[0].Rows.Count > 1) //sanity check
                    {
                        BaseApplicationException bex = new BaseApplicationException("Got more then one village at one cord. count=" + ds.Tables[0].Rows.Count.ToString());
                        bex.AdditionalInformation.Add("xcord", xcord.ToString());
                        bex.AdditionalInformation.Add("ycord", ycord.ToString());
                        throw bex;
                    }
                    return new VillageOther(realm, ds.Tables[CONSTS.VillageOtherTableIndex.VillageID], xcord, ycord);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException("Realm is null");
            }
        }

        /// <summary>
        /// returns null if village not found
        /// </summary>
        /// <returns></returns>
        public static VillageOther GetVillage(Realm realm, string villageName)
        {
            if (realm != null)
            {
                DataSet ds = DAL.Villages.VillageInfo_min(realm.ConnectionStr, villageName );

                if (ds.Tables.Count != 0 && ds.Tables[CONSTS.VillageOtherTableIndex.VillageID].Rows.Count != 0)
                {
                    if (ds.Tables[0].Rows.Count > 1) //sanity check
                    {
                        BaseApplicationException bex = new BaseApplicationException("Got more then one village at one cord. count=" + ds.Tables[0].Rows.Count.ToString());
                        bex.AdditionalInformation.Add("VillageName", villageName );

                        throw bex;
                    }
                    return new VillageOther(realm, ds.Tables[CONSTS.VillageOtherTableIndex.VillageID], Convert.ToInt32(ds.Tables[CONSTS.VillageOtherTableIndex.VillageID].Rows[0][CONSTS.VillageOtherColumnIndex.XCord]), Convert.ToInt32(ds.Tables[CONSTS.VillageOtherTableIndex.VillageID].Rows[0][CONSTS.VillageOtherColumnIndex.YCord]));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException("Realm is null");
            }
        }

        /// <summary>
        /// returns true if Player is new and in Protection Period
        /// </summary>
        public bool IsUnderBeginnerProtection
        {
            get
            {
                return isUnderBeginnerProtection;
            }
        }

        /// <summary>
        /// returns Date when Player's Protection period ends
        /// </summary>
        public DateTime BeginnerProtectionEndsOn
        {
            get
            {
                return beginnerProtectionEndsOn;
            }
        }

        /// <summary>
        /// returns null if village not found
        /// </summary>
        /// <returns></returns>
        public static VillageOther GetVillage(int OwnerID,Realm realm, int villageID)
        {
            if (realm != null)
            {
                DataSet ds = DAL.Villages.GetOtherVillage(realm.ConnectionStr, villageID, OwnerID);

                if (ds.Tables.Count != 0 && ds.Tables[CONSTS.VillageOtherTableIndex.VillageID].Rows.Count != 0)
                {
                    return new VillageOther(realm, ds.Tables[CONSTS.VillageOtherTableIndex.VillageID], Convert.ToInt32(ds.Tables[CONSTS.VillageOtherTableIndex.VillageID].Rows[0][CONSTS.VillageOtherColumnIndex.XCord]), Convert.ToInt32(ds.Tables[CONSTS.VillageOtherTableIndex.VillageID].Rows[0][CONSTS.VillageOtherColumnIndex.YCord]));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException("Realm is null");
            }
        } 

        private VillageOther(Realm realm, DataTable village, int xcord, int ycord)
        {
            _cordinates = new System.Drawing.Point(xcord, ycord);
            _realm = realm;
            _drVillage = village.Rows[0];

            isUnderBeginnerProtection = false;
            beginnerProtectionEndsOn = DateTime.MinValue;
            if (!IsOwnerSpecialPlayer)
            {
                isUnderBeginnerProtection = Convert.ToDateTime(_drVillage[CONSTS.VillageOtherColumnIndex.RegisteredOn]).AddDays(realm.BeginnerRegistrationDays) > DateTime.Now ? true : false;
                beginnerProtectionEndsOn = Convert.ToDateTime(_drVillage[CONSTS.VillageOtherColumnIndex.RegisteredOn]).AddDays(realm.BeginnerRegistrationDays);
            } 
            VillageType = realm.VillageTypes[(short)_drVillage[CONSTS.VillageOtherColumnIndex.VillageTypeID]];

            if (IsCapitalVillage)
            {
                capitalVillageProtectionEndsOn = Convert.ToDateTime(_drVillage[CONSTS.VillageOtherColumnIndex.RegisteredOn]).AddDays(realm.CapitalVillageProtectionDurationInDays);
            }
        }

        public static string getVillageNotes(int VillageID, int OwnerID, string ConnectionStr)
        {
            return Fbg.DAL.VillageOther.getVillageNotes(VillageID, OwnerID, ConnectionStr);
        }

        public static void saveVillageNotes(int VillageID, int OwnerID, string Notes, string ConnectionStr)
        {
            Fbg.DAL.VillageOther.saveVillageNotes(VillageID, OwnerID, Notes, ConnectionStr);
        }

        public static void insertVillageNotes(int VillageID, int OwnerID, string Notes, string ConnectionStr)
        {
            Fbg.DAL.VillageOther.saveVillageNotes(VillageID, OwnerID, Notes, ConnectionStr);
        }


        public VillageTypes.VillageType VillageType
        {
            get;
            internal set;
        }


        #region properties
        public bool IsCapitalVillage
        {
            get
            {
                if (_realm.AreCapitalVillagesActive && (int)_drVillage[CONSTS.VillageOtherColumnIndex.IsCapitalVillage] > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// returns the date that the capital village protection is *supposed* to end BUT notice that capital village protection is dependent on script 
        /// that runs daily so always say "expires in less than a day" when counters is a day in the future or in the past. always check IsCapitalVillage first however
        /// </summary>
        public DateTime CapitalVillageProtectionEndsOn
        {
            get
            {
                return capitalVillageProtectionEndsOn;
            }
        }


        public string VillageName
        {
            get
            {
                return (string)_drVillage[CONSTS.VillageOtherColumnIndex.VillageName];
            }
        }
        public string OwnerName
        {
            get
            {
                return (string)_drVillage[CONSTS.VillageOtherColumnIndex.OwnerName];
            }
        }
        public int OwnerPlayerID
        {
            get
            {
                return (int)_drVillage[CONSTS.VillageOtherColumnIndex.OwnerPlayerID];
            }
        }
        public bool IsOwnerSpecialPlayer
        {
            get
            {
                return Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(_realm) == OwnerPlayerID
                    || Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(_realm) == OwnerPlayerID
                    || Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerId(_realm) == OwnerPlayerID;
            }
        }
        public int OwnerPoints
        {
            get
            {
                return (int)_drVillage[CONSTS.VillageOtherColumnIndex.OwnersPoints];
            }
        }
        public int XCord
        {
            get
            {
                return (int)_drVillage[CONSTS.VillageOtherColumnIndex.XCord];
            }
        }
        public int YCord
        {
            get
            {
                return (int)_drVillage[CONSTS.VillageOtherColumnIndex.YCord];
            }
        }
        public int Points
        {
            get
            {
                return (int)_drVillage[CONSTS.VillageOtherColumnIndex.Points];
            }
        }

        /// <summary>
        /// Name of the clan this village's owner belongs to. Can be empty string if owner not part of clan
        /// </summary>
        public string Clan
        {
            get
            {
                if (_drVillage[CONSTS.VillageOtherColumnIndex.Clan] is DBNull ) {
                    return String.Empty;
                } else {
                    return (string)_drVillage[CONSTS.VillageOtherColumnIndex.Clan];
                }
            }
        }

        /// <summary>
        /// ID of the clan this village's owner belongs to. Can be 0 if owner not part of clan
        /// </summary>
        public int ClanID
        {
            get
            {
                if (_drVillage[CONSTS.VillageOtherColumnIndex.ClanID] is DBNull)
                {
                    return 0;
                }
                else
                {
                    return (int)_drVillage[CONSTS.VillageOtherColumnIndex.ClanID];
                }
            }
        }
        public int ID
        {
            get
            {
                return (int)_drVillage[CONSTS.VillageOtherColumnIndex.VillageID];
            }
        }

        public string Note
        {
            get
            {
                return (string)_drVillage[CONSTS.VillageOtherColumnIndex.Note];
            }
        }

        public System.Drawing.Point Cordinates
        {
            get
            {
                return _cordinates;
            }
        }

        public bool IsInSleepMode
        {
            get
            {
                return _realm.SleepModeGet.IsPlayerInSleepMode(
                    _drVillage[CONSTS.VillageOtherColumnIndex.SleepModeActiveFrom] is DBNull ? DateTime.MinValue
                    : (DateTime)_drVillage[CONSTS.VillageOtherColumnIndex.SleepModeActiveFrom]);
            }
        }
        /// <summary>
        /// result of this unknown if !IsInSleepMode
        /// </summary>
        public DateTime InSleepModeUntil
        {
            get
            {
                if (!IsInSleepMode)
                {
                    return DateTime.MinValue;
                }
                return ((DateTime)_drVillage[CONSTS.VillageOtherColumnIndex.SleepModeActiveFrom]).AddHours(_realm.SleepModeGet.Duration);
            }
        }

        public DateTime IsInVacationModeUntill
        {

            get
            {
                Realm.VacationMode realmVacation = _realm.VacationModeGet;
                if (realmVacation.Allowed)
                {
                    bool pendingRequestExists = _drVillage[CONSTS.VillageOtherColumnIndex.VacationModeRequestOn] is DBNull ? false : true;
                    if (pendingRequestExists)
                    {

                        int playerVacationDaysUsed = (Int32)_drVillage[CONSTS.VillageOtherColumnIndex.VacationModeDaysUsed];
                        int playerXP = (int)_drVillage[CONSTS.VillageOtherColumnIndex.OwnersXPCached];
                        int additionalDaysFromXp = Fbg.Bll.Player.convertXpToVacationDays(playerXP);
                        int playerMaxDays = realmVacation.RealmBaseDays + additionalDaysFromXp;
                        int playerDaysLeft = playerMaxDays - playerVacationDaysUsed;
                        int playerVacationDelayDays = realmVacation.ActivationDelayDays;
                        DateTime playerVacationRequestDate = (DateTime)_drVillage[CONSTS.VillageOtherColumnIndex.VacationModeRequestOn];
                        DateTime playerVacationEffectiveDate = playerVacationRequestDate.AddDays(playerVacationDelayDays);
                        DateTime playerVacationEndDate = playerVacationEffectiveDate.AddDays(Math.Min(playerDaysLeft, realmVacation.PerUseMaximum));
                        
                        DateTime now = DateTime.Now;
                        if (playerVacationEffectiveDate <= now && playerVacationEndDate > now)
                        {
                            return playerVacationEndDate;
                        }

                    }
                }

                return DateTime.MinValue;
            }
        }

        public DateTime IsInWeekendModeUntill
        {

            get
            {
                Realm.WeekendMode realmWeekendMode = _realm.WeekendModeGet;
                if (realmWeekendMode.Allowed)
                {
                    bool pendingRequestExists = _drVillage[CONSTS.VillageOtherColumnIndex.WeekendModeTakesEffectOn] is DBNull ? false : true;
                    if (pendingRequestExists)
                    {
                        
                        DateTime playerWeekendModeTakesEffectOnDate = (DateTime)_drVillage[CONSTS.VillageOtherColumnIndex.WeekendModeTakesEffectOn];
                        DateTime playerWeekendEndsOnDate = playerWeekendModeTakesEffectOnDate.AddDays(realmWeekendMode.RealmBaseDays);


                        if (playerWeekendModeTakesEffectOnDate.Ticks <= DateTime.Now.Ticks && playerWeekendEndsOnDate.Ticks > DateTime.Now.Ticks)
                        {
                            return playerWeekendEndsOnDate;
                        }

                    }
                }

                return DateTime.MinValue;
            }
        }

        #endregion 

        #region ISerializableToNameValueCollection Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            try
            {
                string pre = "Village[" + ID.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in Village.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
 
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in VillageOther.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion

    }
}