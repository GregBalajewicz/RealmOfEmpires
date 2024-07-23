using Gmbc.Common.Diagnostics.ExceptionManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fbg.Bll.ThroneRoom
{


    public class TournamentRealmStats
    {
        public class CONST {
            public class Tables {
                public static readonly int RXBestResults =0;
                public static readonly int RXBestResultsByRealmLength = 1;
                public static readonly int RSBestResults = 2;
                public static readonly int RSBestResultsByRealmLength = 3;
                public static readonly int AllTournamentRealmFinishes = 4;
            }

            /// <summary>
            /// applies to both RXBestResults and RSBestResults
            /// </summary>
            public class BestResultsTableColIndex
            {
                public static readonly int RankByNumOfVillages = 0;
                public static readonly int NumTimesRankAchieved = 1;

            }

            /// <summary>
            /// applies to both RXBestResultsByRealmLength and RSBestResultsByRealmLength
            /// </summary>
            public class BestResultsByRealmLengthTableColIndex
            {
                public static readonly int RealmLengthInHours = 0;
                public static readonly int RankByNumOfVillages = 1;
                public static readonly int NumTimesRankAchieved = 2;
            }
            /// <summary>
            /// applies to AllTournamentRealmFinishes
            /// </summary>
            public class AllTournamentRealmFinishesColIndex
            {
                public static readonly int RealmLengthInHours = 0;
                public static readonly int RankByNumOfVillages = 1;
                public static readonly int RealmOpenedOn = 2;
            }
        }

        DataSet _ds;
        public TournamentRealmStats(DataSet ds)
        {
            _ds = ds;
        }
        public DataTable RXBestResults
        {
            get
            {
                return _ds.Tables[CONST.Tables.RXBestResults];
            }
        }
        public DataTable RXBestResultsByRealmLength
        {
            get
            {
                return _ds.Tables[CONST.Tables.RXBestResultsByRealmLength];
            }
        }
        public DataTable RSBestResults
        {
            get
            {
                return _ds.Tables[CONST.Tables.RSBestResults];
            }
        }
        public DataTable RSBestResultsByRealmLength
        {
            get
            {
                return _ds.Tables[CONST.Tables.RSBestResultsByRealmLength];
            }
        }
        public DataTable AllTournamentRealmFinishes
        {
            get
            {
                return _ds.Tables[CONST.Tables.AllTournamentRealmFinishes];
            }
        }
    }

    public class MyThroneRoom : ThroneRoom
    {
        internal MyThroneRoom(User u)
            : base(u)
        {
        }

        override public string GlobalPlayerName
        {
            get
            {
                return _user.GlobalPlayerName;
            }
        }
        override public int XP
        {
            get
            {
                return _user.XP;
            }
        }


        //override public bool includeThisPlayer(int playerID, PlayerInfoDisplayStatus displayStatus)
        //{
        //    return true;
        //}
        override public void filterPlayerList(DataTable dt)
        {
        }

        override public PlayerRealmInfo getRealmInfoObj(bool isActive, int pID, string name, Realm r, DataSet dsInfo, ThroneRoom.PlayerInfoDisplayStatus displayStatus)
        {
            if (isActive)
            {
                return new MyActivePlayerInfo(pID, name, r, dsInfo, displayStatus);
            }
            else
            {
                return new MyInactivePlayerInfo(pID, name, r, dsInfo, displayStatus);

            }
        }

        private TournamentRealmStats _tournamentRealmStats;
        public TournamentRealmStats TournamentRealmStats
        {
            get
            {
                if (_tournamentRealmStats == null)
                {
                    _tournamentRealmStats = new TournamentRealmStats(Fbg.DAL.User.GetThroneRoomTournamentRealmStats(this._user.ID));
                }

                return _tournamentRealmStats;
            }
        }
    }
    public class SomeoneElsesThroneRoom : ThroneRoom
    {
        public SomeoneElsesThroneRoom(Guid userID, int? viewingThisplayerID)
            : base(new Fbg.Bll.User(userID))
        {
            _viewingThisplayerID = viewingThisplayerID;
        }

        int? _viewingThisplayerID;

        bool _inStealthMode_DONOTACCESSDIRECTLY = false;
        object dummy = null;
        private bool isInStealthMode
        {
            get
            {
                // why??? cause we can only know if we are in stealth mode, till this job is done 
                dummy = PlayerRealmsInfo;

                return _inStealthMode_DONOTACCESSDIRECTLY;
            }

            set {
                _inStealthMode_DONOTACCESSDIRECTLY = value;
            }
        }

        override public string GlobalPlayerName
        {
            get
            {
                if (isInStealthMode)
                {
                    PlayerRealmInfo hiddenPRI = PlayerRealmsInfo.First().Value;
                    return hiddenPRI.Playername; 
                    
                }
                else
                {
                    return _user.GlobalPlayerName;
                }
            }
        }

        override public int XP
        {
            get
            {
                if (isInStealthMode)
                {
                    PlayerRealmInfo hiddenPRI = PlayerRealmsInfo.First().Value;
                    return hiddenPRI.highestTitleAchieved.XP_Cumulative + 1; 
                    
                }
                else
                {
                    return _user.XP;
                }
            }
        }

        override public PlayerRealmInfo getRealmInfoObj(bool isActive, int pID, string name, Realm r, DataSet dsInfo, ThroneRoom.PlayerInfoDisplayStatus displayStatus)
        {
            if (isActive)
            {
                return new SomeElsesActivePlayerInfo(pID, name, r, dsInfo);
            }
            else
            {
                return new SomeElsesInactivePlayerInfo(pID, name, r, dsInfo);

            }
        }


        private TournamentRealmStats _tournamentRealmStats;
        public TournamentRealmStats TournamentRealmStats
        {
            get
            {
                if (_tournamentRealmStats == null)
                {
                    if (isInStealthMode)
                    {
                        _tournamentRealmStats = new TournamentRealmStats(Fbg.DAL.User.GetThroneRoomTournamentRealmStats(Guid.Empty));
                    }
                    else
                    {
                        _tournamentRealmStats = new TournamentRealmStats(Fbg.DAL.User.GetThroneRoomTournamentRealmStats(this._user.ID));
                    }
                }

                return _tournamentRealmStats;
            }
        }

        //
        // EVERYTHIGN DEPENDS ON THIS METHOD BEING CALLED!! it must be called first!!
        //
        override public void filterPlayerList(DataTable dt)
        {

            if (_viewingThisplayerID != null)
            {
                dt.Constraints.Add("PKey_PlayerID", dt.Columns[ThroneRoom.CONSTS.PlayerInfoColumnIndex.playerID], true);
                DataRow dr = dt.Rows.Find((int) _viewingThisplayerID);
                if (dr != null) // the only way it could be null, is if there is some bug in the UI that sends wrong userid + playerid combination
                {
                    // if this player is set to hidden, then we include only this players info, and get rid of all the others.
                    //  we do this by makring all other players as hidden, and marking the current player as not hidden
                    if ((int)dr[CONSTS.PlayerInfoColumnIndex.displayStatus] == (int)ThroneRoom.PlayerInfoDisplayStatus.hidden)
                    {
                        isInStealthMode = true;
                        dr[CONSTS.PlayerInfoColumnIndex.displayStatus] = (int)ThroneRoom.PlayerInfoDisplayStatus.shown;
                        foreach(DataRow dr2 in dt.Rows) {
                            if (dr2 != dr) {
                                dr2[CONSTS.PlayerInfoColumnIndex.displayStatus] = (int)ThroneRoom.PlayerInfoDisplayStatus.hidden;
                            }
                        }
                    }

                    //TODO : also hide username etc 
                }
            }

            //
            // delete all hidden player
            //
            foreach (DataRow dr in dt.Rows)
            {
                if ((int)dr[CONSTS.PlayerInfoColumnIndex.displayStatus] == (int)ThroneRoom.PlayerInfoDisplayStatus.hidden)
                {
                    dr.Delete();
                }
            }

            dt.AcceptChanges();

        }
    }



    public abstract class ThroneRoom : Gmbc.Common.GmbcBaseClass
    {
         public enum PlayerInfoDisplayStatus
            {
                hidden = 0
                , shown
            }
        public static class CONSTS
        {
           
            public class PlayerInfoColumnIndex
            {
                public static int playerID = 0;
                public static int realmID = 1;
                public static int name = 2;
                public static int isActive = 3;
                public static int displayStatus = 4;
            }
        }
        protected User _user;
        Dictionary<string, PlayerRealmInfo> _playerRealmsInfo;
        GameWideTopStats _gameWideTopStats;

        abstract public string GlobalPlayerName { get; }
        abstract public int XP { get; }
        public int Level
        {
            get
            {
                return Fbg.Bll.UsersXP.CalcLevel(XP);
            }
        }
        public int BonusVacationDays
        {
            get
            {
                return Fbg.Bll.Player.convertXpToVacationDays(XP);
            }
        }
        public int NextVacationXP
        {
            get
            {
                return Fbg.Bll.Player.nextVacationXP(XP);
            }
        }


        internal ThroneRoom(User u)
        {
            _user = u;
        }

        

        public GameWideTopStats GameWideTopStats
        {
            get
            {
                object o = PlayerRealmsInfo; // game wide stats are initialized there 
                return _gameWideTopStats;
            }
        }

        //public abstract bool includeThisPlayer(int playerID, PlayerInfoDisplayStatus displayStatus);
        public abstract void  filterPlayerList(DataTable dt);
        public abstract PlayerRealmInfo getRealmInfoObj(bool isActive, int pID, string name, Realm r, DataSet dsInfo, ThroneRoom.PlayerInfoDisplayStatus displayStatus);

        public Dictionary<string, PlayerRealmInfo> PlayerRealmsInfo
        {
            get
            {
                if (_playerRealmsInfo == null)
                {
                    
                    _gameWideTopStats = new Bll.ThroneRoom.GameWideTopStats();
                    DataSet dsOnePlayerInfo;
                    PlayerRealmInfo onePlayerInfo;
                    Realm r;
                    //table with columns Playerid, RealmID, Name, isActive
                   // DataTable dt = DAL.User.AllPlayers(_user.ID, this is MyThroneRoom ? Fbg.DAL.User.AllPlayersView.my : Fbg.DAL.User.AllPlayersView.someoneelses);
                    DataTable dt = DAL.User.AllPlayers(_user.ID);
                    filterPlayerList(dt);
                    _playerRealmsInfo = new Dictionary<string,PlayerRealmInfo>(dt.Rows.Count);

                    TRACE.VerboseLine(string.Format("PlayerRealmsInfo-{0}-", _user.ID));
                    foreach (DataRow dr in dt.Rows)
                    {
                        r = Realms.Realm((int)dr[CONSTS.PlayerInfoColumnIndex.realmID], true);
                        //if (!includeThisPlayer((int)dr[CONSTS.PlayerInfoColumnIndex.playerID] ,(PlayerInfoDisplayStatus)dr[CONSTS.PlayerInfoColumnIndex.isActive])) {
                        //    continue;
                        //}
                        if (r != null)
                        {
                            TRACE.VerboseLine(string.Format("{2} PlayerRealmsInfo-{0}-calling DAL.Players.PlayerInfo-{1}", _user.ID, dr[CONSTS.PlayerInfoColumnIndex.playerID], DateTime.Now.ToLongTimeString()));
                            dsOnePlayerInfo = DAL.Players.PlayerInfo(r.ConnectionStr, (int)dr[CONSTS.PlayerInfoColumnIndex.playerID]);
                            TRACE.VerboseLine(string.Format("{2} PlayerRealmsInfo-{0}-calling DAL.Players.PlayerInfo-{1} FINISHED", _user.ID, dr[CONSTS.PlayerInfoColumnIndex.playerID], DateTime.Now.ToLongTimeString()));
                            onePlayerInfo = getRealmInfoObj((int)dr[CONSTS.PlayerInfoColumnIndex.isActive] == 1, (int)dr[CONSTS.PlayerInfoColumnIndex.playerID]
                                    , (string)dr[CONSTS.PlayerInfoColumnIndex.name]
                                    , r, dsOnePlayerInfo, (PlayerInfoDisplayStatus)dr[CONSTS.PlayerInfoColumnIndex.displayStatus]);       
                            //
                            // if this realm is not hidden, then consider it for top-stats 
                            //
                            if ((int)dr[CONSTS.PlayerInfoColumnIndex.displayStatus] != 0)
                            {
                                _gameWideTopStats.UpdateIfBetter(GameWideTopStats.CONSTS.Stats.IDs.HighestNumOfVillages, onePlayerInfo.HighestNumOfVillages, onePlayerInfo.PlayerID);
                                _gameWideTopStats.UpdateIfBetter(GameWideTopStats.CONSTS.Stats.IDs.GovKilledAsDefender, onePlayerInfo.GovKilledAsDefender, onePlayerInfo.PlayerID);
                                _gameWideTopStats.UpdateIfBetter(GameWideTopStats.CONSTS.Stats.IDs.PointsAsAttacker, onePlayerInfo.PointsAsAttacker, onePlayerInfo.PlayerID);
                                _gameWideTopStats.UpdateIfBetter(GameWideTopStats.CONSTS.Stats.IDs.PointsAsDefender, onePlayerInfo.PointsAsDefender, onePlayerInfo.PlayerID);
                                _gameWideTopStats.UpdateIfBetter(GameWideTopStats.CONSTS.Stats.IDs.VillagePoints, onePlayerInfo.HighestVillagePoints, onePlayerInfo.PlayerID);
                            }
                            _playerRealmsInfo.Add(onePlayerInfo.PlayerID.ToString(), onePlayerInfo);
                        }
                    }
                }
                return _playerRealmsInfo;
            }
        }

    }
}
