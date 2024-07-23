using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public class PlayerOther : Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection
    {
        private DataRow _drPlayerInfo;
        private DataRow _drInviteInfo;
        private DataTable _dtPlayerVillageInfo;
        private DataTable _dtProfile;
        private Title _title = null;
        private Realm _realm;


        public static int id;


        public enum ActivityLevel
        {
            VeryLow,
            Normal        
        }
        private ActivityLevel _activityLevel;
        private double _activity_daysSinceLastLogin;
        private int _activity_numerofLogins;
        private bool _isActivityLevelRetried = false;


        public class CONSTS
        {
            internal class PlayerOtherTableIndex
            {
                public static int PlayerInfo = 0;
                public static int InviteInfo = 1;
                public static int PlayerVillageInfo = 2;
                public static int profile = 3;
            }

            internal class PlayerInfoColumnIndex
            {
                public static int PlayerID = 0;
                public static int PlayerName = 1;
                public static int Points = 2;
                public static int ClanName = 3;
                public static int ClanID = 4;
                public static int Note = 5;
                public static int Anonymous = 6;
                public static int Title = 7;
                public static int Sex = 8;
                public static int SleepModeActiveFrom = 9;
                public static int AvatarID = 10;
                public static int XP=11;
                /// <summary>
                /// may be null!!
                /// </summary>
                public static int govType = 12;
                public static int userID = 13;

            }

            internal class InviteInfoColumnIndex
            {
                public static int isInvited = 0;
            }

            public  class PlayerVillageColumnIndex
            {
                public static int VillageID = 0;
                public static int VillageName = 1;
                public static int XCord = 2;
                public static int YCord = 3;
                public static int Points = 4;
            }

            internal class PlayerActivityTableIndex
            {
                public static int LastLoginOn = 0;
                public static int NumOfLogins= 1;
            }

        
        }

        /// <summary>
        /// To get Player's Information, whose ID is supplied
        /// </summary>
        /// <param name="realm">For Connection String</param>
        /// <param name="PlayerID">Player's ID, whose Information we want to show--Optional</param>
        /// <param name="loggedinPlayerID">Current Player's ID--who is generating the request</param>
        /// <returns>returns null if player not found</returns>
        public static PlayerOther GetPlayer(Realm realm, int PlayerID, int loggedinPlayerID)
        {
            id = PlayerID;

            DataSet ds = DAL.Players.GetPlayerOtherInfo(realm.ConnectionStr, PlayerID,"", loggedinPlayerID);

             if (ds.Tables.Count != 0 && ds.Tables[CONSTS.PlayerOtherTableIndex.PlayerInfo].Rows.Count != 0)
            {
                return new PlayerOther(realm, ds);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// To get Player's Information whose Name is supplied
        /// </summary>
        /// <param name="realm">For Connection String</param>
        /// <param name="PlayerName">Player's Name, whose Information we want to show--Optional</param>
        /// <param name="loggedinPlayerID">Current Player's ID--who is generating the request</param>
        /// <returns>returns null if player not found</returns>
        public static PlayerOther GetPlayer(Realm realm, string PlayerName, int loggedinPlayerID)
        {

            DataSet ds = DAL.Players.GetPlayerOtherInfo(realm.ConnectionStr,0, PlayerName, loggedinPlayerID);

            if (ds.Tables.Count != 0 && ds.Tables[CONSTS.PlayerOtherTableIndex.PlayerInfo].Rows.Count != 0)
            {
                return new PlayerOther(realm, ds);
            }
            else
            {
                return null;
            }
        }

        private PlayerOther(Realm realm, DataSet ds)
        {
            _drPlayerInfo = ds.Tables[CONSTS.PlayerOtherTableIndex.PlayerInfo].Rows[0];
            _drInviteInfo = ds.Tables[CONSTS.PlayerOtherTableIndex.InviteInfo].Rows[0];
            _dtPlayerVillageInfo = ds.Tables[CONSTS.PlayerOtherTableIndex.PlayerVillageInfo];
            _dtProfile = ds.Tables[CONSTS.PlayerOtherTableIndex.profile];
            _realm = realm;

            XP = new UsersXP((int)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.XP]);
        }
        public UsersXP XP {  get; private set; }

        Avatar _avatar;
        public Avatar Avatar
        {
            get
            {
                if (_avatar == null)
                {
                    _avatar=  Realms.Avatars.GetAvatar(Convert.ToInt32(_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.AvatarID]));
                    if (_avatar == null)
                    {
                        _avatar = Realms.Avatars.GetAvatar(2);
                    }
                }

                return _avatar;
            }
        }
        public string Profile
        {
            get
            {
                if (_dtProfile.Rows.Count > 0)
                {
                    return (string)_dtProfile.Rows[0][0];
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// get or set the profile. NOTE, setting the profile means making a call to the db right away
        /// </summary>
        public void ProfileSave(string newProfile, int loggedInPlayerID)
        {
            if (loggedInPlayerID != this.PlayerID)
            {
                throw new SecurityException("only can edit your own profile");
            }
            DAL.PlayerOther.SaveProfile(PlayerID, newProfile, _realm.ConnectionStr);
        }

        public int PlayerID
        {
            get
            {
                return (int)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.PlayerID];
            }
        }

        public string PlayerName
        {
            get
            {
                return (string)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.PlayerName];
            }
        }

        public int Points
        {
            get
            {
                return (int)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.Points];
            }
        }

        /// <summary>
        /// returns empty string if member not part of any clan
        /// </summary>
        public string ClanName
        {
            get
            {
                return (string)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.ClanName];
            }
        }

        /// <summary>
        /// Returns 0 if player not part of any clan
        /// </summary>
        public int ClanID
        {
            get
            {
                return (int)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.ClanID];
            }
        }

        /// <summary>
        /// Returns null if player has no gov type chosen
        /// </summary>
        public GovType GovernmentType
        {
            get
            {
                if (!(_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.govType] is DBNull)
                    && !string.IsNullOrEmpty((string)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.govType] ))
                {
                    return _realm.GovType(Convert.ToInt32(_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.govType]));
                }
                else
                {
                    return null;
                }
                
            }
        }



        /// <summary>
        /// NOTE!! Do not reveal this to players as this coudl give away a player who is trying to play incognito in a realm!!!
        ///     Perhaps this shoudl be change to work like FacebookID used to work!!!
        /// </summary>
        public Guid UserID
        {
            get
            {
                return (Guid)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.userID];
            }
        }

  
        public bool IsInvitedToLoggedinPlayersClan
        {
            get
            {
                return (int)_drInviteInfo[CONSTS.InviteInfoColumnIndex.isInvited] == 0 ? false : true;
            }
        }


        public DataTable PlayerVillageInfo
        {
            get
            {
                return _dtPlayerVillageInfo;
            }
        }

        public string Note
        {
            get
            {
                return (string)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.Note];
            }
        }

        public Title Title
        {
            get
            {
                if (_title == null)
                {
                    _title = _realm.TitleByID((int)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.Title]);
                }
                return _title;
            }
        }


        public string TitleName
        {
            get
            {
                return Title.TitleName(this.Sex);
            }
        }

        public Fbg.Common.Sex Sex
        {
            get
            {
                return (Fbg.Common.Sex)(short)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.Sex];
            }
        }
        public bool IsInSleepMode
        {
            get
            {
                return _realm.SleepModeGet.IsPlayerInSleepMode(
                    _drPlayerInfo[CONSTS.PlayerInfoColumnIndex.SleepModeActiveFrom] is DBNull ? DateTime.MinValue 
                    :(DateTime)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.SleepModeActiveFrom]);
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
                return ((DateTime)_drPlayerInfo[CONSTS.PlayerInfoColumnIndex.SleepModeActiveFrom]).AddHours(_realm.SleepModeGet.Duration);
            }
        }

        public ActivityLevel Activity
        {
            get
            {
                if (!_isActivityLevelRetried)
                {
                    DataSet ds = DAL.PlayerOther.GetPlayerActivity(id);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        _activity_daysSinceLastLogin = double.MaxValue;
                    }
                    else
                    {
                        _activity_daysSinceLastLogin = DateTime.Now.Subtract((DateTime)ds.Tables[0].Rows[0][0]).TotalDays;
                    }
                    _activity_numerofLogins = (int)ds.Tables[CONSTS.PlayerActivityTableIndex.NumOfLogins].Rows[0][0];

                    if (
                         _activity_numerofLogins <= 2 || 
                        (_activity_daysSinceLastLogin > 5 && _activity_numerofLogins <= 5) ||
                        _activity_daysSinceLastLogin > 12 
                        )
                    {
                        //
                        // if we got 2 or fewer logins, OR (more than 5 days of inactivy AND 5 or fewer logins) OR inactivity > 12 days
                        // - reason for doing the "if we got 2 or fewer logins" is to get ppl who login once (or twice) and never come back. 
                        _activityLevel = ActivityLevel.VeryLow;
                    }
                    else
                    {
                        _activityLevel = ActivityLevel.Normal;
                    }
                }
                return _activityLevel;
            }
        }
         
        #region ISerializableToNameValueCollection2 Members

        public void SerializeToNameValueCollection2(System.Collections.Specialized.NameValueCollection col, string namePrePend)
        {
            if (col == null)
            {
                ExceptionManager.Publish("In Fbg.Bll.PlayerOther.SerializeToNameValueCollection2 and 'col' param is null");
            }
            else
            {
                try
                {
                    col.Add(namePrePend + "PlayerName", this.PlayerName.ToString());
                    col.Add(namePrePend + "Points", this.Points.ToString());
                    col.Add(namePrePend + "ClanName", this.ClanName.ToString());
                    col.Add(namePrePend + "ClanID", this.ClanID.ToString());
                    col.Add(namePrePend + "IsInvitedToLoggedinPlayersClan", this.IsInvitedToLoggedinPlayersClan.ToString());
                    col.Add(namePrePend + "dtUnitsNowRecruiting", Gmbc.Common.Data.DataSetUtilities.WriteXml(_dtPlayerVillageInfo, true));
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish("Error in Fbg.Bll.PlayerOther.SerializeToNameValueCollection.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
                    col.Add("Fbg.Bll.PlayerOther.SerializeToNameValueCollection.SerializeToNameValueCollection", "Error:" + ExceptionManager.SerializeToString(e));
                }
            }
        }

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            string pre = "Player[" + id.ToString() + "]";

            if (col == null)
            {
                ExceptionManager.Publish("In Fbg.Bll.PlayerOther.SerializeToNameValueCollection2 and 'col' param is null");
            }
            else
            {
                try
                {
                    col.Add(pre + "PlayerName", this.PlayerName.ToString());
                    col.Add(pre + "Points", this.Points.ToString());
                    col.Add(pre + "ClanName", this.ClanName.ToString());
                    col.Add(pre + "ClanID", this.ClanID.ToString());
                    col.Add(pre + "IsInvitedToLoggedinPlayersClan", this.IsInvitedToLoggedinPlayersClan.ToString());
                    col.Add(pre + "dtUnitsNowRecruiting", Gmbc.Common.Data.DataSetUtilities.WriteXml(_dtPlayerVillageInfo, true));
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish("Error in Fbg.Bll.PlayerOther.SerializeToNameValueCollection.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
                    col.Add("Fbg.Bll.PlayerOther.SerializeToNameValueCollection.SerializeToNameValueCollection", "Error:" + ExceptionManager.SerializeToString(e));
                }
            }
        }

        public static string getPlayerNotes(int PlayerID, int OwnerID, string ConnectionStr)
        {
            return Fbg.DAL.PlayerOther.getPlayerNotes(PlayerID, OwnerID, ConnectionStr);
        }

        public static void updatePlayerNotes(int PlayerID, int OwnerID, string Notes, string ConnectionStr)
        {
            Fbg.DAL.PlayerOther.updatePlayerNotes(PlayerID, OwnerID, Notes, ConnectionStr);
        }

        public static void savePlayerNotes(int PlayerID, int OwnerID, string Notes, string ConnectionStr)
        {
            Fbg.DAL.PlayerOther.savePlayerNotes(PlayerID, OwnerID, Notes, ConnectionStr);
        }

        #endregion
    }
}
