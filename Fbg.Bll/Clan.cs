using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions; 
using Gmbc.Common.Diagnostics.ExceptionManagement;
namespace Fbg.Bll
{
    public class Clan
    {
        public class CONSTS
        {

            public class ClanColumnIndex
            {
                public static int ClanID = 0;
                public static int Name = 1;
                public static int Desc = 2;
                public static int MemberCount = 3;
            }

            public class ClanMemberColumnIndex
            {
                public static int VillagePoints = 0;
                public static int PlayerID = 1;
                public static int PlayerName = 2;
                public static int VillagesCount = 3;
                public static int LastLogin = 4;
                public static int StewardPlayerID = 5;
                public static int StewardPlayerName = 6;
            }
            public class ClanMemberLiteColumnIndex
            {
                public static int PlayerID = 0;
                public static int PlayerName = 1;

            }
            public class ClanMembersTableIndex
            {
                public static int Points = 0;
                public static int Roles = 1;

            }

            public class ClanMembersColName
            {
                public static string PlayerID = "PlayerID";
                public static string LastLoginTime = "LastLoginTime";
                public static string PlayerName = "Name";
                public static string StewardPlayerID = "StewardPlayerID";
                public static string StewardPlayerName = "StewardPlayerName";
                public static string SleepModeActiveFrom = "SleepModeActiveFrom";
                public static string VacationModeRequestOn = "VacationModeRequestOn";
                public static string WeekendModetakesEffectOn = "WeekendModetakesEffectOn";
            }

            public class RolesColumnIndex
            {
                public static int PlayerID = 0;
                public static int ClanID = 1;
                public static int RoleID = 2;
            }
            public class RolesColumnNames
            {
                public static string PlayerID = "PlayerID";
                public static string RoleID = "RoleID";
            }

            public class Diplomacy_Allies_ColNames
            {
                public static string Name = "Name";
                public static string ClanID = "ClanID";
                public static string OtherClanID = "OtherClanID";
                public static string StatusID = "StatusID";
            }


            public class CommBriefTblIndex
            {
                public static int ClanAnnoucement = 0;
                public static int RecentForumPost = 1;
            }
            public class ClanAnnoucementColIndex
            {
                public static int PlayerID = 0;
                public static int ClanID = 1;
                public static int RoleID = 2;
            }
            public class RecentForumPostColIndex
            {
                public static int PostDate = 0;
                public static int PostByPlayerName = 1;
                public static int PostID = 2;
                public static int PostTitle = 3;
                public static int Body = 4;
                public static int ForumTitle= 5;
            }
        }
        public enum InviteResult : int
        {
            Success = 0,
            Player_Not_Found = 1,
            Player_Already_In_Clan = 2,
            Player_Already_invited = 3,
            Inviter_Donnot_have_Permssoin = 4,
            /// <summary>
            /// means the player has used up all his invites in the 24h period
            /// </summary>
            RanOutOfInvites = 5,
            RealmClanLimit = 6
        }
        public enum DiplomacyResult : int
        {
            Success = 0,
            Diplomacy_Already_Exist = 1,
            Clan_Dont_Exist = 2,
            This_is_your_clan = 3
        }
        public enum JoinResult : int
        {
            Success = 0,
            Clan_Dont_Exist = 1,
            Player_dont_have_invitation = 2,
            ClanLimit = 3,
            ClanChangesNoLongerAllowed = 4
        }
        public enum Diplomacy : int
        {
            Ally = 0,
            Enemy = 1,
            NAP = 2
        }
        DataRow drClans;
        Fbg.Bll.Player _player;

        public Clan(DataRow dr, Fbg.Bll.Player player)
        {
            drClans = dr;
            _player = player;
        }
        public int MemberCount
        {
            get
            {
                return (int)drClans[Clan.CONSTS.ClanColumnIndex.MemberCount];
            }
        }
        public int ID
        {
            get
            {
                return (int)drClans[Clan.CONSTS.ClanColumnIndex.ClanID];
            }
        }
        public string Name
        {
            get
            {
                return (string)drClans[Clan.CONSTS.ClanColumnIndex.Name];
            }
        }
        public string Desc
        {
            get
            {
                return (string)drClans[Clan.CONSTS.ClanColumnIndex.Desc];
            }
        }

        public bool AllowMessageAllMembers(Role role)
        {
            if (role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || role.IsPlayerPartOfRole(Role.MemberRole.Owner))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// if invitesLeft returns Int32.MaxValue means there is no limit. 
        /// </summary>
        /// <param name="invitesLeft"></param>
        /// <param name="moreInvitesAvailableOn"></param>
        public void GetInvitesLeft(out int invitesLeft, out DateTime moreInvitesAvailableOn)
        {
            DAL.Clans.GetInvitesLeft(_player.Realm.ConnectionStr, _player.ID, this.ID, out invitesLeft, out moreInvitesAvailableOn);
        }

        /// <summary>
        /// Invites the player with ID=playerID to this clan. to the clan of the owner 
        /// </summary>
        /// <param name="playerID"></param>
        public static InviteResult InvitePlayer(string PlayerName, Player inviter, out DateTime moreInvitesAvailableOn)
        {
            if (inviter == null)
            {
                throw new ArgumentNullException("inviter is null");

            }

            if (inviter.Clan == null)
            {
                throw new ArgumentNullException("inviter.Clan is null");

            }

            if (!Clan.HasInvitePermisions(inviter))
            {
                throw new SecurityException("player=" + inviter.Name);
            }

            if (inviter.Realm.ClanLimit != 0 && inviter.Realm.ClanLimit <= inviter.Clan.MemberCount)
            {
                moreInvitesAvailableOn = DateTime.Now;
                return InviteResult.RealmClanLimit;
            }

            return (InviteResult)Enum.Parse(typeof(InviteResult), DAL.Clans.InvitePlayer(inviter.Realm.ConnectionStr, inviter.ID, PlayerName, inviter.Clan.ID, out moreInvitesAvailableOn).ToString());


        }
        public Fbg.Common.Clan.DissmissFromClanResult DismissPlayer(int PlayerID)
        {

            if (PlayerID == _player.ID)
            {
                //
                //delete my self
                //
                return DAL.Clans.LeaveClan(_player.Realm.ConnectionStr, _player.ID, PlayerID, ID, true);
            }
            else
            {
                //
                // delete other player
                //
                if (!(_player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || _player.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)))
                {
                    throw new SecurityException("only owners and admins can dismiss players");
                }
                return DAL.Clans.LeaveClan(_player.Realm.ConnectionStr, _player.ID, PlayerID, ID, false);
            }
        }
        public static void DeleteClan(Player owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner is null");

            }

            if (owner.Clan == null)
            {

                throw new ArgumentNullException("owner.Clan is null");

            }
            DAL.Clans.DeleteClan(owner.Realm.ConnectionStr, owner.Clan.ID, owner.Name);

            // invalidate the cached clan in player object. this will ensure that calling player.Clan will trigger a DB call
            owner.Clan = null;
        }
        /// <summary>
        /// BeCareful ,this method reload each time you call it 
        /// this function need to handle if no clan to the player
        /// this method return a dataset please return to clan class to see the dataset members.
        /// </summary>
        public static DataTable ViewClanInvitations(Player Player, string searchName)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            if (Player.Clan == null)
            {
                //this code to handle if the player in No Clan
                return null;


            }
            return DAL.Clans.ViewClanInvitations(Player.Realm.ConnectionStr, Player.Clan.ID, searchName);

        }
        public static DataSet ViewPlayerInvitations(Player Player)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }


            return DAL.Clans.ViewPlayerInvitations(Player.Realm.ConnectionStr, Player.ID);


        }
        /// <summary>
        /// BeCareful ,this method reload each time you call it 
        /// this function need to handle if no clan to the player
        /// this method return null if the player in No Clan
        /// this method return a dataset please return to clan class to see the dataset members.
        /// MoreDetails is indicating to get the clan members full detailed OR just (PlayerID,Name)
        /// true for detailed info;false for lite info
        /// clan member Lite cloumn Description in Fbg.Bll.Clan.CONSTS.ClanMemberLiteColumnIndex
        /// clan member Detailed column description in Fbg.Bll.Clan.CONSTS.ClanMemberColumnIndex
        /// </summary>
        public static DataSet ViewClanMembers(Player Player, int ClanID, bool MoreDetails)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            if (MoreDetails)
            {
                return DAL.Clans.ViewClanMembers(Player.Realm.ConnectionStr, ClanID);
            }
            else
            {
                return DAL.Clans.ViewClanMembersLite(Player.Realm.ConnectionStr, ClanID);
            }

        }
        public static Clan CreateNewClan(string name, string desc, Player owner)
        {
            try
            {
                if (owner == null)
                {
                    throw new ArgumentNullException("owner is null");

                }


                if (owner.Clan == null)
                {
                    name = Regex.Replace(name, @"\s{2,}", " "); //this line to remove any number of spaces between words and make it just one space
                    name = Clan.ClearInvalidCharsInName(name);
                    DataSet ds = DAL.Clans.CreateNewClan(owner.Realm.ConnectionStr, name, desc, owner.ID);
                    Clan ClanObj;
                    if (ds == null)//the Clan Name Alreday Exist 
                    {
                        return null;
                    }

                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            ClanObj = new Clan(ds.Tables[0].Rows[0], owner);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                    return ClanObj;
                }
                else
                    throw new Exception("You are already part of clan");
            }
            catch (Exception ex)
            {
                BaseApplicationException be = new BaseApplicationException("Error Creating Clan", ex);
                be.AdditionalInformation.Add("name", name);
                be.AdditionalInformation.Add("Player ID", owner == null ? "null" : owner.ID.ToString());
                throw be;
            }


        }


        public enum LeaveClanResult {
            Failed = 0,
            OK = 1,
            Failed_NoClanChangesAllowedAnyLonger=2
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns>true if success false if failed</returns>
        public static LeaveClanResult LeaveClan(Player player)
        {
            Fbg.Common.Clan.DissmissFromClanResult retVal;

            if (player == null)
            {
                throw new ArgumentNullException("Player is null");

            }
            if (player.Clan == null)
            {
                return LeaveClanResult.OK;
            }

            if (!player.Realm.AllowClanChanges)
            {
                return LeaveClanResult.Failed_NoClanChangesAllowedAnyLonger;
            }

            retVal = DAL.Clans.LeaveClan(player.Realm.ConnectionStr, player.ID, player.ID, player.Clan.ID, true);

            // invalidate the cached clan in player object. this will ensure that calling player.Clan will trigger a DB call
            player.Clan = null;

            if( retVal == Fbg.Common.Clan.DissmissFromClanResult.Success )
            {
                return LeaveClanResult.OK;
            } else
            {
                return LeaveClanResult.Failed;
            }
        }
        public static JoinResult JoinClan(Player Player, int clanID)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");
            }

            if (!Player.Realm.AllowClanChanges)
            {
                return JoinResult.ClanChangesNoLongerAllowed;
            }

            if (Player.Clan == null)
            {
                return (JoinResult)Enum.Parse(typeof(JoinResult), DAL.Clans.JoinClan(Player.Realm.ConnectionStr, Player.ID, clanID).ToString());
            }
            else
            {
                throw new Exception("You are already part of clan");
            }

        }
        public static void CancelInvitation(Player Player, int clanID)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            DAL.Clans.CancelInvitation(Player.Realm.ConnectionStr, Player.ID, clanID);


        }
        /// <summary>
        /// delete a invitation made by admin,owner,inviter
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="clanID"></param>
        public static void DeleteInvitation(Player Player, int PlayerID)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            if (Player.Clan != null)
            {
                DAL.Clans.DeleteInvitation(Player.Realm.ConnectionStr, PlayerID, Player.Clan.ID);
            }


        }
        public static ClanDiplomacy ViewMyClanDiplomacy(Player Player)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            if (Player.Clan == null)
            {
                throw new ArgumentNullException("Clan is null");
            }

            return new ClanDiplomacy(DAL.Clans.ViewMyClanDiplomacy(Player.Realm.ConnectionStr, Player.Clan.ID));
        }
        public static DiplomacyResult AddDiplomacy(Player Player, string OtherClanName, Diplomacy DiplomacyStatus)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            if (Player.Clan == null)
            {
                throw new ArgumentNullException("Clan is null");
            }
            if (Player.Clan.Name == OtherClanName.Trim())
            {
                return DiplomacyResult.This_is_your_clan;
            }
            return (DiplomacyResult)Enum.Parse(typeof(DiplomacyResult), DAL.Clans.AddDiplomacy(Player.Realm.ConnectionStr, Player.Clan.ID, OtherClanName, (int)DiplomacyStatus).ToString());
        }
        public static void DeleteDiplomacy(Player Player, string OtherClanName, Diplomacy DiplomacyStatus)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            if (Player.Clan == null)
            {
                throw new ArgumentNullException("Clan is null");
            }

            DAL.Clans.DeleteDiplomacy(Player.Realm.ConnectionStr, Player.Clan.ID, OtherClanName, (int)DiplomacyStatus);
        }

        public static DataSet ViewClanPublicProfile(Player Player, int ClanID)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            return DAL.Clans.ViewClanPublicProfile(Player.Realm.ConnectionStr, ClanID);

        }

        public static string GetMessageFromCode(Clan.InviteResult result)
        {
            string msg = string.Empty;
            switch (result)
            {
                case Clan.InviteResult.Player_Already_In_Clan:
                    msg = "This player is already part of this clan.";
                    break;
                case Clan.InviteResult.Player_Already_invited:
                    msg = "This player already has a pending invitation to the clan.";
                    break;
                case Clan.InviteResult.Player_Not_Found:
                    msg = "Player name not found.";
                    break;
                case Clan.InviteResult.Success:
                    break;
                case Clan.InviteResult.RanOutOfInvites:
                    msg = "You have used up all your invites. More invites available on {0}. Remember you can still <a href='invite.aspx' style='color:#000000 !important; text-decoration:underline'>invite your Facebook friends</a> to your clan";
                    break;
                case Clan.InviteResult.RealmClanLimit:
                    msg = "Clan size limit reached. You cannot issue any more invites.";
                    break;
                default:
                    throw new Exception("Unrecognized value of Clan.InviteResult:" + result.ToString());
            }
            return msg;
        }
        //public static string GetJoinMessageFromCode(Clan.JoinResult result)
        //{
        //    string msg = string.Empty;
        //    switch (result)
        //    {
        //        case Clan.JoinResult.Clan_Dont_Exist:
        //            msg = "The clan does not exist. Perhaps it was disbanded.<br>If you believe this to be a problem,<a href='http://www.new.facebook.com/board.php?uid=10471770557 '> contact support</a>";
        //            break;
        //        case Clan.JoinResult.Player_dont_have_invitation:
        //            msg = "You don't have invitation to this clan.";
        //            break;
        //        case JoinResult.ClanLimit:
        //            msg = "You cannot join this clan since this clan reached the maximum member size already.";
        //            break;
        //        case Clan.JoinResult.Success:
        //            break;
        //        default:
        //            throw new Exception("Unrecognized value of Clan.JoinResult:" + result.ToString());
        //    }
        //    return msg;
        //}

        public void UpdateClanPublicProfile(string PublicProfile)
        {
            if (!(_player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || _player.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)))
            {
                throw new SecurityException("only owners and admins can do this");
            }

            DAL.Clans.UpdateClanPublicProfile(_player.Realm.ConnectionStr, _player.ID, ID, PublicProfile);
        }
        public static bool RenameClan(Player Player, string newClaName)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }
            if (Player.Clan == null)
            {
                throw new ArgumentNullException("Clan is null");
            }
            if (!Player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner))
            {
                throw new InvalidConstraintException("Player not owner of clan");
            }
            newClaName = Regex.Replace(newClaName, @"\s{2,}", " "); //this line to remove any number of spaces between words and make it just one space
            return DAL.Clans.RenameClan(Player.Realm.ConnectionStr, Player.Clan.ID, newClaName, Player.ID);

        }
        public static bool HasInvitePermisions(Player Player)
        {
            if (Player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || Player.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || Player.Role.IsPlayerPartOfRole(Role.MemberRole.Inviter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static DataSet GetClanEvents(Player Player)
        {
            return GetClanEvents(Player, false);
        }
        public static DataSet GetClanEvents(Player Player, bool topRowsOnly)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }
            if (Player.Clan == null)
            {
                throw new ArgumentNullException("Clan is null");

            }
            return DAL.Clans.GetClanEvents(Player.Realm.ConnectionStr, Player.Clan.ID, topRowsOnly);
        }
        public DataSet GetClanSettings()
        {
            return DAL.Clans.GetClanSettings(_player.Realm.ConnectionStr, this.ID);
        }
        public void UpdateClanSettings(bool inviterFlag)
        {
            if (!_player.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)
                && !_player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner))
            {
                throw new SecurityException();
            }
            DAL.Clans.UpdateClanSettings(_player.Realm.ConnectionStr, this.ID, inviterFlag);
        }
        /// <summary>
        /// removes some characters not allowed in
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ClearInvalidCharsInName(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            text = Regex.Replace(text, "\\[", ""); // remove '['
            text = Regex.Replace(text, "\\]", ""); // remove ']'
            return text;
        }


        public static DataSet GetCommunicationBrief(Fbg.Bll.Player player)
        {
            return Fbg.DAL.Clans.GetClanCommunicationBrief(player.Realm.ConnectionStr, player.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="villageID"></param>
        /// <returns>1 - claimed by player; either now or before; either case, the vilalge is now claimed by player
        /// 2 - claimed by someone else in the clan
        /// 0 - claim was not sucessfull; not sure why</returns>
        public int ClaimVillage(int villageID)
        {
            return Fbg.DAL.Clans.ClaimVillage(_player.Realm.ConnectionStr, _player.ID, ID, villageID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="villageID"></param>
        /// <returns>1 - claimed by player; either now or before; either case, the vilalge is now claimed by player
        /// 2 - claimed by someone else in the clan
        /// 0 - claim was not sucessfull; not sure why</returns>
        public int ClaimVillage_Unclaim(int villageID)
        {
            return Fbg.DAL.Clans.ClaimVillage_Unclaim(_player.Realm.ConnectionStr, _player.ID, ID, villageID);
        }

        //public struct ClaimSystemSettings
        //{
        //    int detailsViewLevel;
        //    int maxClaims;
        //    int maxClaimDuration;
        //}


        public enum ClaimVillage_SettingIDs
        {
            detailsViewLevel = 1
            ,maxClaims = 2
            ,maxClaimDuration =3
            , sharewith_allies = 4
            , sharewith_nap = 5
        
        }
        
        public DataSet ClaimVillage_GetSettings()
        {
            if (!_player.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)
    && !_player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner))
            {
                throw new SecurityException();
            }



            return DAL.Clans.ClaimVillage_GetSettings(_player.Realm.ConnectionStr, this.ID);

        }


        public void ClaimVillage_SaveSettings(ClaimVillage_SettingIDs setting, int value)
        {
            if (!_player.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)
    && !_player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner))
            {
                throw new SecurityException();
            }

            DAL.Clans.ClaimVillage_SaveSetting(_player.Realm.ConnectionStr, this.ID, (int)setting, value);

        }

        
    }



    //public class ClanEvents
    //{
    //    private static string playerInvited = "{0} invited {1}";
    //    private static string playerJoined = "{0} joined the clan.";
    //    private static string playerLeft = "{0} has left the clan.";
    //    private static string clanAlly = "{0} set as ally";
    //    private static string clanEnemy = "{0} set as enemy";
    //    private static string clanNAP = "{0} set as NAP";
    //    private static string clanPublicProfile = "Clan's public message has changed";

    //    public static void PlayerInvited(Player Player, string invited)
    //    {
    //       string message= String.Format(playerInvited, Player.Name, invited);
    //        if (Player == null)
    //        {
    //            throw new ArgumentNullException("Player is null");

    //        }
    //        if (Player.Clan == null)
    //        {
    //            throw new ArgumentNullException("Clan is null");
    //        }
    //        if (!Player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner)||!Player.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)||!Player.Role.IsPlayerPartOfRole(Role.MemberRole.Inviter ))
    //        {
    //            throw new InvalidConstraintException("invalid Player role");
    //        }
    //        DAL.Clans.AddClanEvent(Player.Realm.ConnectionStr, Player.Clan.ID, message,DateTime.Now );
    //    }
    //    public static void PlayerJoined(Player Player, string invited)
    //    {
    //        string message = String.Format(playerInvited, Player.Name, invited);
    //        if (Player == null)
    //        {
    //            throw new ArgumentNullException("Player is null");

    //        }
    //        if (Player.Clan == null)
    //        {
    //            throw new ArgumentNullException("Clan is null");
    //        }
    //        if (!Player.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || !Player.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || !Player.Role.IsPlayerPartOfRole(Role.MemberRole.Inviter))
    //        {
    //            throw new InvalidConstraintException("invalid Player role");
    //        }
    //        DAL.Clans.AddClanEvent(Player.Realm.ConnectionStr, Player.Clan.ID, message, DateTime.Now);
    //    }

    //}
}
