using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fbg.Common;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Data;
using Fbg.Bll.ThroneRoom;
using System.Text;
using System.Text.RegularExpressions;


namespace Fbg.Bll.Api.ThroneRoom
{
    public class ThroneRoom
    {
        public static class CONSTS
        {
            public static DateTime fullOpenOn = new DateTime(2015, 11, 12, 11, 00, 00);
        }
        public static bool isFullyOpen()
        {
            return isFullyOpen(DateTime.Now);
        }
        public static bool isFullyOpen(DateTime now)
        {
            return IsTesterRoleOrHigher || now >= CONSTS.fullOpenOn;
        }
        private static bool IsTesterRoleOrHigher
        {
            get
            {
                return (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("tester"));
            }
        }
        public static string UserInfo_spectatorView(string userID)
        {
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            json_serializer.RegisterConverters(new JavaScriptConverter[] { new Converter() });
            Fbg.Bll.User user = null;
            //
            // get user from userID
            try
            {
                user = new Fbg.Bll.User(new Guid(userID));
            }
            catch
            {
                return json_serializer.Serialize(new
                {
                    success = true,
                    @object = "no such user"
                });
            }
            return UserInfo_spectatorView(user, null);
        }

        public static string UserInfo_spectatorView(int realmID, int playerID, int viewerPlayerID)
        {
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            json_serializer.RegisterConverters(new JavaScriptConverter[] { new Converter() });

            if (!isFullyOpen())
            {
                return json_serializer.Serialize(new
                {
                    success = false,
                    availableOn = ApiHelper.SerializeDate(CONSTS.fullOpenOn),
                    @object = "not yet avilable"
                });
            }
            else
            {
                //PlayerOther po = null;
                Guid userID = Guid.Empty;
                try
                {
                    //po = PlayerOther.GetPlayer(Realms.Realm(realmID), playerID, viewerPlayerID);
                    DataTable dt = Fbg.DAL.User.GetAnyEventDeletedPlayerInfo(playerID, null);
                   
                    if (dt.Rows.Count > 0)
                    {
                        userID = (Guid)dt.Rows[0]["userid"];
                    }
                    if (userID == Guid.Empty)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    return json_serializer.Serialize(new
                    {
                        success = true,
                        @object = "no such player"
                    });
                }

                return UserInfo_spectatorView(new Fbg.Bll.User(userID), playerID);
            }
        }

        public static string UserInfo_spectatorView(User user, int? viewingThisPlayerID)
        {
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            json_serializer.RegisterConverters(new JavaScriptConverter[] { new Converter() });


            Fbg.Bll.ThroneRoom.SomeoneElsesThroneRoom tr = new SomeoneElsesThroneRoom(user.ID, viewingThisPlayerID);
           
            return json_serializer.Serialize(new
            {
                success = true,
                @object = new
                {
                    View = "spectator",
                    User = new
                    {
                        /*
                        GlobalPlayerName = user.GlobalPlayerName, // will be empty string if player has no GlobalPlayerName
                        XP = user.XP,
                        Level = Fbg.Bll.UsersXP.CalcLevel(user.XP),
                        BonusVacationDays = Fbg.Bll.Player.convertXpToVacationDays(user.XP)
                         */
                        GlobalPlayerName = tr.GlobalPlayerName,
                        XP = tr.XP,
                        Level = tr.Level,
                        BonusVacationDays = tr.BonusVacationDays,
                        TRLikes = DAL.User.GetThroneRoomLikes(user.ID, viewingThisPlayerID),
                        Sex = user.UserSex,
                        AvatarID = user.UserAvatarID

                    }
                    ,
                    GameWideTopStats = tr.GameWideTopStats
                    ,
                    PlayerRealmsInfo = tr.PlayerRealmsInfo
                    ,
                    AvatarList = Fbg.Bll.Api.MiscApi.AvatarsGetListAll(user.ID),
                    TournamentRStats = new
                    {
                        RSBestResults = from dr in tr.TournamentRealmStats.RSBestResults.AsEnumerable()
                                        select
                                        new
                                        {
                                            rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsTableColIndex.RankByNumOfVillages)
                                            ,
                                            numTimesRankAchieved = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsTableColIndex.NumTimesRankAchieved)

                                        },
                        RXBestResultsByRealmLength = from dr in tr.TournamentRealmStats.RXBestResultsByRealmLength.AsEnumerable()
                                                     select
                                                     new
                                                     {
                                                         realmLengthInH = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.RealmLengthInHours)
                                                         ,
                                                         rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.RankByNumOfVillages)
                                                         ,
                                                         numTimesRankAchieved = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.NumTimesRankAchieved)

                                                     },
                        RXBestResults = from dr in tr.TournamentRealmStats.RXBestResults.AsEnumerable()
                                        select
                                        new
                                        {
                                            rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsTableColIndex.RankByNumOfVillages)
                                            ,
                                            numTimesRankAchieved = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsTableColIndex.NumTimesRankAchieved)

                                        },
                        RSBestResultsByRealmLength = from dr in tr.TournamentRealmStats.RSBestResultsByRealmLength.AsEnumerable()
                                                     select
                                                     new
                                                     {
                                                         realmLengthInH = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.RealmLengthInHours)
                                                         ,
                                                         rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.RankByNumOfVillages)
                                                         ,
                                                         numTimesRankAchieved = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.NumTimesRankAchieved)

                                                     },
                      
                    }
                }
            });

        }

        //Owner of Throne view
        public static string UserInfo(Fbg.Bll.User user)
        {
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            json_serializer.RegisterConverters(new JavaScriptConverter[] { new Converter() });

            return json_serializer.Serialize(new
            {
                success = true,
                @object = new
                {
                    View = "owner",
                    User = new
                    {
                        GlobalPlayerName = user.ThroneRoom.GlobalPlayerName,
                        DisableRenameGlobalPlayerName = user.HasFlag(User.Flags.DisableRenameGlobalPlayerName) != null,
                        XP = user.ThroneRoom.XP,
                        Level = user.ThroneRoom.Level,
                        BonusVacationDays = user.ThroneRoom.BonusVacationDays,
                        NextVacationXP = user.ThroneRoom.NextVacationXP,
                        TRLikes = DAL.User.GetThroneRoomLikes(user.ID, null),
                        Sex = user.UserSex,
                        AvatarID = user.UserAvatarID,
                        DisplayChatVIP = user.HasFlag_GetData(User.Flags.VIPLevel_ShowInChat)
                    }
                    ,
                    GameWideTopStats = user.ThroneRoom.GameWideTopStats
                    ,
                    PlayerRealmsInfo = user.ThroneRoom.PlayerRealmsInfo
                    ,
                    AvatarList = Fbg.Bll.Api.MiscApi.AvatarsGetListAll(user.ID)
                    ,
                    TournamentRStats = new
                    {
                        RSBestResults = from dr in user.ThroneRoom.TournamentRealmStats.RSBestResults.AsEnumerable()
                                        select
                                        new
                                        {
                                            rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsTableColIndex.RankByNumOfVillages)
                                            ,
                                            numTimesRankAchieved = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsTableColIndex.NumTimesRankAchieved)

                                        },
                        RXBestResultsByRealmLength = from dr in user.ThroneRoom.TournamentRealmStats.RXBestResultsByRealmLength.AsEnumerable()
                                                     select
                                                     new
                                                     {
                                                         realmLengthInH = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.RealmLengthInHours)
                                                         ,
                                                         rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.RankByNumOfVillages)
                                                         ,
                                                         numTimesRankAchieved = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.NumTimesRankAchieved)

                                                     },
                        RXBestResults = from dr in user.ThroneRoom.TournamentRealmStats.RXBestResults.AsEnumerable()
                                        select
                                        new
                                        {
                                            rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsTableColIndex.RankByNumOfVillages)
                                            ,
                                            numTimesRankAchieved = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsTableColIndex.NumTimesRankAchieved)

                                        },
                        RSBestResultsByRealmLength = from dr in user.ThroneRoom.TournamentRealmStats.RSBestResultsByRealmLength.AsEnumerable()
                                                     select
                                                     new
                                                     {
                                                         realmLengthInH = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.RealmLengthInHours)
                                                         ,
                                                         rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.RankByNumOfVillages)
                                                         ,
                                                         numTimesRankAchieved = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.BestResultsByRealmLengthTableColIndex.NumTimesRankAchieved)

                                                     },
                        AllTournamentRealmFinishes = from dr in user.ThroneRoom.TournamentRealmStats.AllTournamentRealmFinishes.AsEnumerable()
                                                     select
                                                     new
                                                     {
                                                         realmOpenOn = ApiHelper.SerializeDate(dr.Field<DateTime>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.AllTournamentRealmFinishesColIndex.RealmOpenedOn))
                                                         ,
                                                         realmLengthInH = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.AllTournamentRealmFinishesColIndex.RealmLengthInHours)
                                                         ,
                                                         rankByNumOfVillages = dr.Field<Int32>(Fbg.Bll.ThroneRoom.TournamentRealmStats.CONST.AllTournamentRealmFinishesColIndex.RankByNumOfVillages)

                                                     }


                    }
                }
            });
        }


        /// <summary>
        /// note, this function will not work if a user never, ever created a player on a realm, because he will have no 
        /// record in the fbgcommon..users table, however the function will report all went well. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static string RenameUser(Fbg.Bll.User user, string newName)
        {

            int errorCode = 0;
            // 0 means all went OK
            // 1 - banned words detected
            // 2 - name already exists
            // 3 - rename not allowed 
            // 4 - name too long
            // 5 - name has special characters - on alphanumeric characters are allowed, and a "." and a "_". No spaces
            // 6 - name too short

            Regex r = new Regex("^[a-zA-Z0-9._]{1,15}$");
            bool renameDisabled = user.HasFlag(User.Flags.DisableRenameGlobalPlayerName) != null;
            if (renameDisabled)
            {
                errorCode = 3;
            }
            else
            {
                newName = Utils.ClearHTMLCode(newName);
                if (Utils.IsBannedName(newName))
                {
                    errorCode = 1;
                }
                else if (newName.Length > 15)
                {
                    errorCode = 4;
                }
                else if (newName.Length < 3)
                {
                    errorCode = 6;
                }
                else if (!r.IsMatch(newName))
                {
                    errorCode = 5;
                }
            }

            if (errorCode == 0)
            {
                errorCode = user.ChangeGlobalPlayerName(newName);
                if (errorCode == 0)
                {
                    user.SetFlag(User.Flags.DisableRenameGlobalPlayerName);
                    HttpContext.Current.Session[global::CONSTS.Session.fbgUser] = null; // we need to invalidate the entire object 
                }
            }

            return ApiHelper.RETURN_SUCCESS(new
            {
                nameChanged = errorCode == 0,
                ifNameNotChangedReasonCode = errorCode,
                newName = newName
            });

        }

        public static string RealmAllPlayerLeaderBoard(int realmID)
        {
            Realm r = Realms.Realm(realmID, noException: true);
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            json_serializer.MaxJsonLength = Int32.MaxValue;

            if (r != null)
            {
                return json_serializer.Serialize(new
                {
                    success = true,
                    @object = new
                    {
                        LeaderBoard = Fbg.Bll.Stats.GetThroneRoomPlayerRanking(r).AsEnumerable().Select(
                            v => new
                            {
                                pID = v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.PlayerID].ToString(),
                                pN= v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.PlayerName].ToString(),
                                pSex= v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.Sex].ToString(),
                                isA = v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.PlayerStatus].ToString(),
                                pLA = ApiHelper.SerializeDate((DateTime)v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.LastActivity]),
                                pCN = v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.ClanName].ToString(),
                                hNumV = v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.HighestNumOfVillages].ToString(),
                                hVPts = v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.HighestVillagePoints].ToString(),
                                pAtt = v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.PointsAsAttacker].ToString(),
                                pDef= v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.PointsAsDefender].ToString(),
                                govKil = v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.GovKilledAsDefender].ToString(),
                                pTID = v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.TopTitleID].ToString(),
                                pTN= v[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.TitleName].ToString(),
                            }
                        ),
                        RealmID = realmID
                    }
                });
            }
            else
            {
                return ApiHelper.RETURN_FAILURE(new { failureReason = "unrecognized realmid" });
            }
        }






        public class Converter : System.Web.Script.Serialization.JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                if (obj is Realm)
                {
                    Realm r = (Realm)obj;
                    dic.Add("ID", r.ID);
                    dic.Add("Name", r.Name);
                    dic.Add("Desc", r.Desc);
                    dic.Add("OpenOn", ApiHelper.SerializeDate(r.OpenOn));
                    dic.Add("ClosingOn", ApiHelper.SerializeDate(r.ClosingOn));
                }
                else if (obj is Title)
                {
                    Title t = (Title)obj;

                    dic.Add("Name_Male", t.TitleName_Male);
                    dic.Add("Name_Female", t.TitleName_Female);
                    dic.Add("ID", t.ID);
                }


                return dic;
            }


            public override IEnumerable<Type> SupportedTypes
            {
                get { return new System.Collections.ObjectModel.ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(Title), typeof(Realm) })); }
            }
        }



        public static string GetPlayerListSettings(Fbg.Bll.User user)
        {
            DataTable dt = Fbg.DAL.User.GetPlayerListSettings(user.ID);
            return ApiHelper.RETURN_SUCCESS(new
            {
                success = true,
                playerListSetting = dt.AsEnumerable().ToDictionary(
                    r => r.Field<Int32>(1).ToString()
                    , r => new
                    {
                        displaySetting = r.Field<Int32>(2)
                    }
                )
            });
        }

        public static string SavePlayerListSetting(Fbg.Bll.User user, int playerID, int statusID)
        {
            Fbg.DAL.User.SavePlayerListSetting(user.ID, playerID, statusID);
            user.ThroneRoomInvalidate();
            return GetPlayerListSettings(user);
        }

        public static string GetLikes(string userid, string playerID)
        {
            int likes = DAL.User.GetThroneRoomLikes(string.IsNullOrWhiteSpace(userid) ? null : (Guid?) new Guid(userid)
                , string.IsNullOrWhiteSpace(playerID) ? null : (int?) Convert.ToInt32(playerID));
            return ApiHelper.RETURN_SUCCESS(new
            {
                success = true,
                likes = likes
            });
        }

        public static string SetLike(string userid, string playerID, string IP)
        {
            DAL.User.SetThroneRoomLike(string.IsNullOrWhiteSpace(userid) ? null : (Guid?)new Guid(userid)
                , string.IsNullOrWhiteSpace(playerID) ? null : (int?)Convert.ToInt32(playerID), IP);

            int likes = DAL.User.GetThroneRoomLikes(string.IsNullOrWhiteSpace(userid) ? null : (Guid?)new Guid(userid)
    , string.IsNullOrWhiteSpace(playerID) ? null : (int?)Convert.ToInt32(playerID));

            return ApiHelper.RETURN_SUCCESS(new
            {
                success = true,
                likes = likes
            });
        }
    }
}