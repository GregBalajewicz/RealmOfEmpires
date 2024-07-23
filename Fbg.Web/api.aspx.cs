using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Fbg.Bll;
using Fbg.Bll.Api;
using Fbg.Common;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;

using BDA.Neighbours;
using BDA.Achievements;

public partial class api : MyCanvasIFrameBasePage
{

    public bool IsiFramePopupsBrowser
    {
        get
        {
            return Utils.IsiFramePopupsBrowser(Request);
        }
    }
    new protected void Page_Load(object sender, EventArgs e)
    {
    }


    System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

    enum functionRequiredLoginLevel
    {
        /// <summary>
        /// do not access fbguser or fbgplayer
        /// </summary>
        noLogin,
        /// <summary>
        /// OK to access fbguser but not fbgplayer
        /// </summary>
        userLevelOnly,
        /// <summary>
        /// OK to access fbguser and fbgplayer
        /// </summary>
        playerLevel
    }

    protected string s()
    {
        json_serializer.MaxJsonLength = Int32.MaxValue;

        #region performance logging
        DateTime beforeRun;
        TimeSpan duration;
        beforeRun = DateTime.Now;
        #endregion  

        string function = Request.QueryString["fn"] == null ? "" : Request.QueryString["fn"];
        string vid = Request.QueryString["vid"];
        functionRequiredLoginLevel thisfunctionRequiredLoginLevel = getFunctionRequiredLoginLevel(function);

        if (thisfunctionRequiredLoginLevel != functionRequiredLoginLevel.noLogin)
        {
            if (FbgUser.IsSuspended && !Context.User.IsInRole("Admin"))
            {
                return ApiHelper.RETURN_REDIRECT("suspension.aspx");
            }
        }
        if (thisfunctionRequiredLoginLevel == functionRequiredLoginLevel.playerLevel)
        {
            if (FbgPlayer.Realm.ConsolidationGet.IsInFreezeTime(DateTime.Now)
                   && FbgPlayer.Realm.ConsolidationGet.TimeOfConsolidation <= DateTime.Now
                   && FbgPlayer.Realm.ConsolidationGet.TimeOfConsolidation != FbgPlayer.Realm.ConsolidationGet.AttackFreezeStartOn)
            {
                return ApiHelper.RETURN_REDIRECT("ConsolidationInProgress_NoLogin.aspx");
            }

        }

        if (thisfunctionRequiredLoginLevel == functionRequiredLoginLevel.playerLevel)
        {
            if (!FbgPlayer.Stewardship_CanMyActivityContinue)
            {
                return ApiHelper.RETURN_REDIRECT(FbgPlayer.Stewardship_IsLoggedInAsSteward ? "AccountStewards_OwnerCancels.aspx" : "AccountStewards_DeactivateBeforeLogin.aspx");
            }
        }

        if (Config.InDev) { System.Threading.Thread.Sleep(300); } // TEMP TEMP TEMP -- SIMULATING LATENCY

        string retval = "";
        switch (function.ToLower())
        {
            //case "villagebasicb":
            //    retval = Village(vid);
            //    break;
            case "playerfull":
                retval = PlayerFull(vid);
                break;
            case "playerref":
                // retval = PlayerRefresh(vid, new DateTime(1970, 01, 01).AddMilliseconds(Convert.ToDouble(Request.QueryString["lastHandledVillagesCacheTimeStamp"])));

                retval = PlayerRefresh(vid
                    , Utils.ConvertKnownJSDateStringFromAPI(Request.QueryString["lastHandledVillagesCacheTimeStamp"]));

                break;
            //case "playerinfo":
            //    retval = GetPlayer();
            //    break;
            case "villagelista":
                retval = VillageListA();
                break;
            case "myvillageextendedbasicinfo":
                retval = myVillageExtendedBasicInfo();
                break;
            case "villagedetail":
                retval = VillageDetail();
                break;
            //case "research":
            //    retval = Research();
            //    break;
            case "researchstate":
                retval = ResearchState();
                break;
            case "research_do":
                retval = ResearchDo(vid);
                break;
            case "buyresearcher":
                retval = BuyResearcher();
                break;
            case "research_getresearchers":
                retval = Research_getResearchers();
                break;
            case "research_catchup":
                retval = Research_catchup();
                break;

            case "tutorial_done":
                retval = TutorialDone(vid);
                break;
            case "mapland":
                retval = MapLand();
                break;
            case "loge":
                LogError();
                break;
            case "incoming":
                retval = IncomingTroops();
                break;
            case "outgoing":
                retval = OutgoingTroops();
                break;
            case "report":
            case "report_getlist":
                //retval = new Fbg.Bll.Api.Reports.Report(FbgPlayer).GetReportList();
                retval = new Fbg.Bll.Api.Reports.Report(FbgPlayer).GetReportListAll();
                break;
            case "report_remove":
                retval = new Fbg.Bll.Api.Reports.Report(FbgPlayer).Delete(Request["rids"]);
                break;
            case "report_nuke": //seperated from above explicitly, for no mistakes
                retval = new Fbg.Bll.Api.Reports.Report(FbgPlayer).Delete(null);
                break;
            case "report_getbattlereport":
                retval = new Fbg.Bll.Api.Reports.Report(FbgPlayer).GetBattleReport(
                    Convert.ToInt32(Request.QueryString["recordID"]));
                break;
            case "report_getmiscreport":
                retval = new Fbg.Bll.Api.Reports.Report(FbgPlayer).GetMiscReport(
                    Convert.ToInt32(Request.QueryString["recordID"]));
                break;
            case "report_getsupportattackedreport":
                retval = new Fbg.Bll.Api.Reports.Report(FbgPlayer).GetSupportAttackedReport(
                    Convert.ToInt32(Request.QueryString["recordID"]));
                break;
            case "player_search":
                retval = PlayerSearch();
                break;
            case "report_forward":
                retval = new Fbg.Bll.Api.Reports.Report(FbgPlayer).Forward(Request["record"], Request["player"]);

                break;
            case "mail":
                retval = MailList();
                break;
            case "mail_sent":
                retval = MailSentList();
                break;
            case "mail_blocked":
                retval = MailBlocked();
                break;
            case "mail_unblock":
                retval = MailUnBlock();
                break;
            case "mail_detail":
                retval = MailDetail();
                break;
            case "mail_delete":
                retval = MailDelete();
                break;
            case "mail_recipients_check":
                retval = MailRecipientsCheck();
                break;
            case "mail_create":
                retval = MailCreate();
                break;
            case "star_mail":
                retval = StarMail(Request["rids"]);
                break;
            case "unstar_mail":
                retval = UnstarMail(Request["rids"]);
                break;
            case "player_setavatarid":
                retval = PlayerSetAvatarID(Convert.ToInt32(Request.QueryString["avatarid"]));
                break;
            case "player_setsex":
                retval = PlayerSetSex(Convert.ToInt32(Request.QueryString["sex"]));
                break;
            case "player_other":
                retval = PlayerOther();
                break;
            case "player_other_villages":
                retval = PlayerOtherVillages();
                break;
            case "player_profile_set":
                retval = PlayerProfileSet();
                break;
            case "player_other_note_clear":
                retval = PlayerOtherNoteClear();
                break;
            case "player_other_note_save":
                retval = PlayerOtherNoteSave();
                break;
            case "player_other_invite_to_clan":
                retval = PlayerOtherInviteToClan();
                break;
            case "village_promote_add":
                retval = VillagePromoteAdd();
                break;
            case "village_promote_remove":
                retval = VillagePromoteRemove();
                break;
            case "village_promote_list":
                retval = VillagePromoteList();
                break;
            case "village_other_save_note":
                retval = VillageOtherSaveNote();
                break;
            case "pf_getplayerpfpackages":
                retval = PF_GetPlayerPFPackages();
                break;
            case "pf_activatepackage":
                retval = PF_ActivatePFPackages();
                break;
            case "acceptnewtitle":
                retval = AcceptNewTitle();
                break;
            case "boostapproval":
                retval = BoostApproval(Convert.ToInt32(vid));
                break;
            case "getvillage":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).GetVillage(Convert.ToInt32(Request.QueryString["vid"]));
                break;
            case "recruit_all_recruitinfo":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Recruit_GetRecruitInfo(Convert.ToInt32(vid));
                break;
            case "upgrade_all_getupgradeinfo":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_GetUpgradeInfo(Convert.ToInt32(vid));
                break;
            case "upgrade_getupgradeinfo":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_GetUpgradeInfo(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["bid"]));
                break;
            case "upgrade_doupgrade":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_DoUpgrade(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["bid"]));
                break;
            case "upgrade_doupgrade2":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_DoUpgrade2(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["bid"]), Convert.ToInt32(Request.QueryString["levelToUpgradeTo"]));
                break;
            case "upgrade_cancelupgrade":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_Cancel(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["bid"]), Convert.ToInt64(Request.QueryString["eid"]), Convert.ToBoolean(Request.QueryString["isq"]));
                break;
            case "upgrade_cancelupgrade2":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_Cancel(Convert.ToInt32(vid), Convert.ToInt64(Request.QueryString["eid"]), Convert.ToBoolean(Request.QueryString["isq"]));
                break;
            case "upgrade_speedupupgrade":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_SpeedUp(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["bid"]), Convert.ToInt64(Request.QueryString["eid"]), Convert.ToInt32(Request.QueryString["min"]));
                break;
            case "upgrade_speedupupgrade2":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_SpeedUp(Convert.ToInt32(vid), null, Convert.ToInt64(Request.QueryString["eid"]), Convert.ToInt32(Request.QueryString["min"]));
                break;
            case "upgrade_speedupupgradefree":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Upgrade_SpeedUpFree(Convert.ToInt32(vid), Convert.ToInt64(Request.QueryString["eid"]));
                break;
            case "downgrade_dodowngrade":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Downgrade_DoDwngrade(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["bid"]));
                break;
            case "downgrade_cancel":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Downgrade_Cancel(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["eid"]), Convert.ToBoolean(Request.QueryString["isq"]));
                break;
            case "downgrade_speedup":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Downgrade_SpeedUp(Convert.ToInt32(vid), Convert.ToInt64(Request.QueryString["eid"]));
                break;
            case "recruit_dorecruit2":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Recruit_DoRecruit(Convert.ToInt32(Request.QueryString["vid"])
                    , json_serializer.Deserialize<List<Fbg.Bll.Api.VillageApi.RecruitCommand>>((Request.QueryString["recruitCommands"])));
                break;
            case "recruit_dorecruit":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Recruit_DoRecruit(Convert.ToInt32(vid)
                    , Convert.ToInt32(Request.QueryString["bid"])
                    , json_serializer.Deserialize<List<Fbg.Bll.Api.VillageApi.RecruitCommand>>((Request.QueryString["recruitCommands"])));
                break;
            case "recruit_cancel":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Recruit_Cancel(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["bid"]), Convert.ToInt32(Request.QueryString["eid"]), VillageApi.Recruit_CancelReturnType.buildinginfo);
                break;
            case "disband":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Disband(Convert.ToInt32(Request.QueryString["vid"])
                    , json_serializer.Deserialize<List<Fbg.Bll.Api.VillageApi.RecruitCommand>>((Request.QueryString["recruitCommands"])));
                break;
            case "recruit_cancel2":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Recruit_Cancel(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["bid"]), Convert.ToInt32(Request.QueryString["eid"]), VillageApi.Recruit_CancelReturnType.recruitinfo);
                break;
            case "gov_getinfo":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Gov_GetInfo(Convert.ToInt32(vid));
                break;
            case "gov_buychest":
                bool returnRecruitInfoInsteadOfJustGovInfo = String.IsNullOrEmpty(Request.QueryString["returnRecruitInfoInsteadOfJustGovInfo"]) ? false : (Request.QueryString["returnRecruitInfoInsteadOfJustGovInfo"] == "1" ? true : false);
                if (String.IsNullOrEmpty(Request.QueryString["count"]))
                {
                    retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Gov_BuyChests(Convert.ToInt32(vid), returnRecruitInfoInsteadOfJustGovInfo);
                }
                else
                {
                    retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Gov_BuyChests(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["count"]), returnRecruitInfoInsteadOfJustGovInfo);
                }
                break;
            case "gov_buychest_leavesilver":
                bool returnRecruitInfoInsteadOfJustGovInfo2 = String.IsNullOrEmpty(Request.QueryString["returnRecruitInfoInsteadOfJustGovInfo"]) ? false : (Request.QueryString["returnRecruitInfoInsteadOfJustGovInfo"] == "1" ? true : false);
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Gov_BuyChests_LeaveSilver(Convert.ToInt32(vid), Convert.ToInt32(Request.QueryString["silverToLeave"]), returnRecruitInfoInsteadOfJustGovInfo2);
                break;
            case "gov_cancelrecruit":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Gov_CancelRecruit(Convert.ToInt32(vid));
                break;
            case "gov_dorecruit":
                retval = new Fbg.Bll.Api.VillageApi(FbgPlayer).Gov_DoRecruit(Convert.ToInt32(vid));
                break;
            case "items_getmyitems":
                retval = Items_GetMyItems(Convert.ToInt32(vid));
                break;
            case "items_buy":
                retval = Items_Buy(Convert.ToInt32(Request.QueryString["gid"])
                    , Convert.ToInt32(Request.QueryString["vid"])
                    , Convert.ToInt32(Request.QueryString["cnt"])
                    , Convert.ToInt32(Request.QueryString["expectedCost"]));
                break;
            case "items2_getmyitems":
                retval = Fbg.Bll.Api.Items2.Items2.GetAllItems(FbgPlayer);
                break;
            case "items2_getmyitemgroups":
                bool getLatest = string.IsNullOrWhiteSpace(Request.QueryString["getLatest"]) ? false : Request.QueryString["getLatest"] == "1";
                retval = Fbg.Bll.Api.Items2.Items2.GetAllItemGroups(FbgPlayer, getLatest);
                break;
            case "call_items2_use":
                retval = Fbg.Bll.Api.Items2.Items2.UseItem(FbgPlayer, Convert.ToInt32(Request.QueryString["vid"]), Convert.ToInt32(Request.QueryString["itemid"]), Request.QueryString["groupID"]);
                break;
            case "quest_getcompletedcount":
                retval = GetCompletedUnclaimedQuestsCount();
                break;
            case "getcreditpackages":
                retval = GetCreditPackages(Convert.ToInt32(Request.QueryString["dvc"]));
                break;
            case "othervillageinfo":
                retval = OtherVillageInfo(Convert.ToInt32(Request.QueryString["vid"]));
                break;
            case "cmd_execute":
                retval = Cmd_Execute(Convert.ToInt32(Request.QueryString["ovid"])
                    , Convert.ToInt32(Request.QueryString["tvid"])
                    , (Fbg.Common.UnitCommand.CommandType)Convert.ToInt32(Request.QueryString["cmdtype"])
                    , json_serializer.Deserialize<List<Fbg.Common.UnitCommand.Units>>((Request.QueryString["units"]))
                    , Request.QueryString["v"]
                    );
                break;

            case "notifsetting_get":
                retval = NotificationSettings_get();
                break;
            case "notifsetting_update":
                retval = NotificationSettings_update(Convert.ToInt32(Request.QueryString["nid"]), Convert.ToInt16(Request.QueryString["v"])
                    , Convert.ToInt16(Request.QueryString["s"]), Convert.ToInt16(Request.QueryString["a"]));
                break;
            case "notifsetting_setrealmwideactive":
                retval = NotificationSettings_setRealmWideActive(Convert.ToBoolean(Request.QueryString["onOrOff"]));
                break;
            case "mobileapprate":
                retval = RewardForMobileAppRating();
                break;
            case "govtypeselect":
                retval = GovTypeSelect(Convert.ToInt32(Request.QueryString["govTypeID"]));
                break;
            case "getmydailyreward":
                retval = GetMyDailyReward(Convert.ToInt32(Request.QueryString["sel"]));
                break;
            case "getclanpublicprofile":
                retval = GetClanPublicProfile(Convert.ToInt32(Request.QueryString["cid"]));
                break;
            case "getclanmemberlist":
                retval = GetClanMemberList(Convert.ToInt32(Request.QueryString["cid"]));
                break;
            case "getclandiplomacy":
                retval = GetClanDiplomacy(Convert.ToInt32(Request.QueryString["cid"]));
                break;
            case "getclaninvites":
                retval = GetClanInvites(Convert.ToInt32(Request.QueryString["pid"]));
                break;
            case "getclanplayerinvites":
                retval = GetClanPlayerInvites(Convert.ToInt32(Request.QueryString["pid"]));
                break;
            case "cancelclaninvite":
                retval = CancelClanInvite(Convert.ToInt32(Request.QueryString["cid"]), Convert.ToInt32(Request.QueryString["pid"]));
                break;
            case "clanleave":
                retval = DoClanLeave(Convert.ToInt32(Request.QueryString["pid"]));
                break;
            case "clanjoin":
                retval = DoClanJoin(Convert.ToInt32(Request.QueryString["cid"]));
                break;
            case "clandissmiss":
                retval = DoClanDissmiss(Convert.ToInt32(Request.QueryString["pid"]));
                break;
            case "clanrevoke":
                retval = RevokeClanJoin(Convert.ToInt32(Request.QueryString["pid"]));
                break;
            case "renameclan":
                retval = RenameClan(Request.QueryString["newname"]);
                break;
            case "deleteclan":
                retval = DeleteClan();
                break;
            case "createclan":
                retval = CreateNewClan(Request.QueryString["clanname"]);
                break;
            case "saveclanprofile":
                retval = SaveClanProfile(Request.QueryString["txt"]);
                break;
            case "saveclandiplomacy":
                retval = InsertClanDiplomacy(Request.QueryString["clan"], Convert.ToInt32(Request.QueryString["dip"]));
                break;
            case "deleteclandiplomacy":
                retval = DeleteClanDiplomacy(Request.QueryString["clan"], Convert.ToInt32(Request.QueryString["dip"]));
                break;
            case "getclansettings":
                retval = GetClanSettings();
                break;
            case "saveclansettings":
                retval = SaveClanSettings(Convert.ToBoolean(Request.QueryString["flag"]));
                break;
            case "recoveryemail":
                retval = Fbg.Bll.Api.MiscApi.RecoveryEmail(Request.QueryString["e"], LoggedInMembershipUser, FbgUser, FbgPlayer, Request.Url.ToString());
                break;
            case "getbonustypes":
                retval = GetBonusTypes(vid);
                break;
            case "setbonustype":
                retval = SetBonusType(vid, Request.QueryString["bt"]);
                break;
            case "restart":
                retval = Restart();
                break;
            case "activatesleepmode":
                retval = ActivateSleepMode();
                break;
            case "activatevacation":
                retval = ActivateVacationMode(Convert.ToInt32(Request.QueryString["duration"]));
                break;
            case "cancelvacation":
                retval = CancelVacationMode();
                break;
            case "weekendmoderequest":
                retval = WeekendModeRequest(Request.QueryString["desiredActivationDate"]);
                break;
            /* not used as an API call yet
            case "weekendmodecancel":
                retval = WeekendModeCancel();
                break;
            */
            case "troopmovement_cancel":
                retval = new Fbg.Bll.Api.CommandTroops.UnitMovements(FbgPlayer).Cancel(Convert.ToInt64(Request.QueryString["eventID"]));
                break;
            case "troopmovement_togglehide":
                retval = new Fbg.Bll.Api.CommandTroops.UnitMovements(FbgPlayer).ToggleHide(Convert.ToInt64(Request.QueryString["eventID"]));
                break;
            case "troopmovement_getdetails":
                retval = new Fbg.Bll.Api.CommandTroops.UnitMovements(FbgPlayer).GetDetails(Convert.ToInt64(Request.QueryString["eventID"]));
                break;
            case "star_report":
                retval = StarReports(Request["rids"]);
                break;
            case "unstar_report":
                retval = UnstarReports(Request["rids"]);
                break;
            case "setlocalserverstorage":
                retval = SetLocalServerStorage(Request["data"]);
                break;
            case "transfercredits":
                retval = TransferCredits();
                break;
            case "maxtransfercredits":
                retval = MaxTransferCredits();
                break;
            case "getservertimeoffset":
                retval = GetServerTimeOffset(Convert.ToInt64(Request.QueryString["localTimeCall"]));
                break;
            case "silvertransport_getnearestvillages":
                retval = new Fbg.Bll.Api.SilverTransport.SilverTransport(FbgPlayer).GetNearestVillages(
                        Convert.ToInt32(Request.QueryString["vid"]),
                        Convert.ToInt32(Request.QueryString["minAmount"]),
                        Convert.ToInt32(Request.QueryString["xNumVillages"])
                    );
                break;
            case "silvertransport_getnearestvillagesforeign":
                retval = new Fbg.Bll.Api.SilverTransport.SilverTransport(FbgPlayer).GetNearestVillagesForeign(
                        Convert.ToInt32(Request.QueryString["vid"])
                    );
                break;
            case "silvertransport_getmaxsilverfromnearestvillages":
                retval = new Fbg.Bll.Api.SilverTransport.SilverTransport(FbgPlayer).GetMaxSilverFromNearestVillages(
                        Convert.ToInt32(Request.QueryString["vid"]),
                        Convert.ToInt32(Request.QueryString["minAmount"]),
                        Convert.ToInt32(Request.QueryString["xNumVillages"])
                    );
                break;
            case "silvertransport_sendamount":
                retval = new Fbg.Bll.Api.SilverTransport.SilverTransport(FbgPlayer).SendAmount(
                        Convert.ToInt32(Request.QueryString["vidFrom"]),
                        Convert.ToInt32(Request.QueryString["vidTo"]),
                        Convert.ToInt32(Request.QueryString["vToX"]),
                        Convert.ToInt32(Request.QueryString["vToY"]),
                        Convert.ToInt32(Request.QueryString["amount"]),
                        Request.QueryString["isMine"] == "true"
                    );
                break;
            case "playermapevent_activate":
                retval = Fbg.Bll.Api.MapEvents.MapEvents.playerMapEventActivate(
                    FbgPlayer,
                    Convert.ToInt32(Request.QueryString["eventID"]),
                    Convert.ToInt32(Request.QueryString["typeID"]),
                    Convert.ToInt32(Request.QueryString["xCord"]),
                    Convert.ToInt32(Request.QueryString["yCord"]));
                break;

            case "tr_userinfo":
                if (string.IsNullOrWhiteSpace(Request.QueryString["uid"]) && string.IsNullOrWhiteSpace(Request.QueryString["pid"]))
                {
                    if (Request.Cookies[CONSTS.Cookies.StewardLoggedInAsRecordID] != null)
                    {
                        return "access denied";
                    }
                    retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.UserInfo(FbgUser);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Request.QueryString["pid"]))
                    {
                        thisfunctionRequiredLoginLevel = functionRequiredLoginLevel.noLogin;
                        retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.UserInfo_spectatorView(Request.QueryString["uid"].Trim());
                    }
                    else
                    {
                        retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.UserInfo_spectatorView(Convert.ToInt32(Request.QueryString["rid"]), Convert.ToInt32(Request.QueryString["pid"]), Convert.ToInt32(Request.QueryString["viewerpid"]));
                    }
                }
                break;
            case "tr_renameuser":
                // note, this function will not work if a user never, ever created a player on a realm, because he will have no 
                // record in the fbgcommon..users table, however the function will report all went well. 
                retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.RenameUser(FbgUser, Request.QueryString["newName"]);
                break;
            case "tr_realmleaderboard":
                retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.RealmAllPlayerLeaderBoard(Convert.ToInt32(Request.QueryString["rid"]));
                break;
            case "tr_togglechatvip":
                retval = UserSetVIPChatBorderDisplay(Convert.ToInt32(Request.QueryString["status"]));
                break;
            case "tr_getplayerlistsettings":
                retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.GetPlayerListSettings(FbgUser);
                break;
            case "tr_saveplayerlistsetting":
                retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.SavePlayerListSetting(FbgUser, Convert.ToInt32(Request.QueryString["pid"]), Convert.ToInt32(Request.QueryString["displaystatus"]));
                break;
            case "tr_getlikes":
                retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.GetLikes(Request.QueryString["uid"], Request.QueryString["pid"]);
                break;
            case "tr_setlike":
                retval = Fbg.Bll.Api.ThroneRoom.ThroneRoom.SetLike(Request.QueryString["uid"], Request.QueryString["pid"], Request.ServerVariables["REMOTE_ADDR"]);
                break;
            case "user_setavatarid":
                // note, this function will not work if a user never, ever created a player on a realm, because he will have no 
                // record in the fbgcommon..users table, however the function will report all went well. 
                retval = UserSetAvatar(Convert.ToInt32(Request.QueryString["avatarid"]));
                break;
            case "user_setsex":
                // note, this function will not work if a user never, ever created a player on a realm, because he will have no 
                // record in the fbgcommon..users table, however the function will report all went well. 
                retval = UserSetSex(Convert.ToInt32(Request.QueryString["sex"]));
                break;
            case "villageclaim":
                retval = Fbg.Bll.Api.MiscApi.ClaimVillage(FbgPlayer, Request.QueryString["vid"]);
                break;
            case "villageclaim_unclaim":
                retval = Fbg.Bll.Api.MiscApi.ClaimVillage_Unclaim(FbgPlayer, Request.QueryString["vid"]);
                break;
            case "avatars_getall":
                retval = AvatarsGetAll();
                break;
            case "avatars_purchase":
                retval = Fbg.Bll.Api.MiscApi.AvatarsPurchase(FbgUser, Convert.ToInt32(Request.QueryString["avatarid"]));
                break;
            case "playermapevent_caravan_cardreveal":
                retval = Fbg.Bll.Api.MapEvents.MapEvents.playerMapEventCaravanCardReveal(FbgPlayer, Request.QueryString["eventID"]);
                break;
            case "playermapevent_caravan_cardpick":
                retval = Fbg.Bll.Api.MapEvents.MapEvents.playerMapEventCaravanCardpick(FbgPlayer, Request.QueryString["eventID"], Convert.ToInt32(Request.QueryString["cardindex"]));
                break;
            case "playermapevent_caravan_cardreroll":
                retval = Fbg.Bll.Api.MapEvents.MapEvents.playerMapEventCaravanCardReroll(FbgPlayer, Request.QueryString["eventID"]);
                break;
            case "playermapevent_caravan_catchup":
                retval = Fbg.Bll.Api.MapEvents.MapEvents.playerMapEventCaravanCatchup(FbgPlayer, Request.QueryString["eventID"]);
                break;
            case "definedtargets_get":
                retval = Fbg.Bll.Api.DefinedTargets.GetAll(FbgPlayer);
                break;
            case "definedtargets_add":

                /* some work in progress, migth be usefull later
                Int64 rawMS = Convert.ToInt64(Request.QueryString["settime"]);
                DateTime newdt = (new DateTime(1970, 1, 1, 0, 0, 0,DateTimeKind.Local).AddMilliseconds(Convert.ToInt64(Request.QueryString["settime"])));
                DateTime newdtUTC = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddMilliseconds(Convert.ToInt64(Request.QueryString["settime"]))).ToUniversalTime();
                DateTime newdt2 = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToInt64(Request.QueryString["settime"]))).ToLocalTime();
                */

                retval = Fbg.Bll.Api.DefinedTargets.Add(FbgPlayer
                    , Convert.ToInt32(Request.QueryString["vid"])
                    , Convert.ToInt16(Request.QueryString["type"])

                    //whats happening here is we create a time, defined as UTC, add MS to it, then convert it to local time.
                    , string.IsNullOrWhiteSpace(Request.QueryString["settime"]) ? null :
                        (DateTime?)(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToInt64(Request.QueryString["settime"]))).ToLocalTime()

                    , Request.QueryString["note"]
                    , Convert.ToInt32(Request.QueryString["expiresInXDays"])
                    , string.IsNullOrWhiteSpace(Request.QueryString["assignedTo"]) ? null : Request.QueryString["assignedTo"]
                    );
                break;
            case "definedtargets_edit":
                retval = Fbg.Bll.Api.DefinedTargets.Edit(FbgPlayer
                    , Convert.ToInt32(Request.QueryString["definedtargetid"])

                    //whats happening here is we create a time, defined as UTC, add MS to it, then convert it to local time.
                    , string.IsNullOrWhiteSpace(Request.QueryString["settime"]) ? null :
                        (DateTime?)(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToInt64(Request.QueryString["settime"]))).ToLocalTime()

                    , Request.QueryString["note"]
                    , Convert.ToInt32(Request.QueryString["expiresInXDays"])
                    , string.IsNullOrWhiteSpace(Request.QueryString["assignedTo"]) ? null : Request.QueryString["assignedTo"]
                    );
                break;
            case "definedtargets_delete":
                retval = Fbg.Bll.Api.DefinedTargets.Delete(FbgPlayer
                    , Convert.ToInt32(Request.QueryString["definedtargetid"])

                    );
                break;
            case "definedtargetresponse_editdeladd":
                retval = Fbg.Bll.Api.DefinedTargets.AddEditDeleteResonse(FbgPlayer
                    , Convert.ToInt32(Request.QueryString["definedtargetid"])
                    , Convert.ToInt16(Request.QueryString["responseTypeID"])
                    , Request.QueryString["response"]
                    );
                break;
            case "next_recommended_quest":
                retval = Fbg.Bll.Api.MiscApi.GetNExtRecommendeQuest(FbgPlayer);
                break;
            case "quest_getrewardforcompleted":
                retval = Fbg.Bll.Api.MiscApi.Quest_GetRewardForCompletedQuest(FbgPlayer, Request.QueryString["questTag"].Trim());
                break;
            case "vaar": // video add - reset available
                retval = Fbg.Bll.Api.MiscApi.VideoAdd_ResetIsAvailableFlag(FbgUser);
                break;
            case "raids_getplayerraids":
                retval = Fbg.Bll.Api.Raids.Raids.Api_GetPlayerRaids(FbgPlayer);
                break;
            case "raids_sendraid":
                retval = Fbg.Bll.Api.Raids.Raids.Api_SendRaid(
                        FbgPlayer
                        , Convert.ToInt32(Request.QueryString["ovid"])
                        , Convert.ToInt32(Request.QueryString["raidid"])
                        , json_serializer.Deserialize<List<Fbg.Common.UnitCommand.Units>>((Request.QueryString["units"]))
                    );
                break;
            /*
        case "raids_getraidplayermovements":
            retval = Fbg.Bll.Api.Raids.Raids.Api_GetRaidPlayerMovements(FbgPlayer, Convert.ToInt32(Request.QueryString["raidid"]));
            break;
            */
            case "raids_getraiddetails":
                retval = Fbg.Bll.Api.Raids.Raids.Api_GetRaidDetails(FbgPlayer, Convert.ToInt32(Request.QueryString["raidid"]));
                break;
            case "raids_acceptrewards":
                retval = Fbg.Bll.Api.Raids.Raids.Api_AcceptRewards(FbgPlayer, Convert.ToInt32(Request.QueryString["raidid"]));
                break;
            // Cannot allow quest completion by API as could
            // potentially be exploited by player
            //case "quest_completed":
            //    retval = SetQuestAsCompleted(Request.QueryString["qtag"]);
            //    break;
            default:
                retval = "unrecognized api name";
                break;
        }

        #region performance logging
        try
        {
            DoNotDoRedirectOnFbgPlayerGet = true;  // just in case calling FbgPlayer woudl result in a redirect
            duration = DateTime.Now.Subtract(beforeRun);
            Fbg.DAL.Logger.LogEvent(

                (thisfunctionRequiredLoginLevel == functionRequiredLoginLevel.playerLevel ? "API.R" + FbgPlayer.Realm.ID : "API.?")

                , function, "QS:" + Request.QueryString.ToString(), duration.Ticks);
        }
        catch (Exception e)
        {
            BaseApplicationException ex = new BaseApplicationException("NOT CRITICAL. INFO ONLY. Error Logging event", e);
            ExceptionManager.Publish(ex);
            // WE EAT THE EXCEPTION ON PURPOSE!!
        }
        #endregion

        return retval;
    }



    private functionRequiredLoginLevel getFunctionRequiredLoginLevel(string fn)
    {
        if (fn.StartsWith("tr_") || fn.StartsWith("user_") || fn.StartsWith("avatars_"))
        {
            return functionRequiredLoginLevel.userLevelOnly;
        }
        else
        {
            switch (fn)
            {
                case "tr_getlikes":
                case "tr_setlike":

                    return functionRequiredLoginLevel.noLogin;

            }
        }
        return functionRequiredLoginLevel.playerLevel;
    }

    public string GetServerTimeOffset(long localTimeCall)
    {

        Double serializedNow = SerializeDate(DateTime.Now);
        Double diff = serializedNow - localTimeCall;

        return json_serializer.Serialize(new
        {
            success = true,
            @object = new
            {
                serverNow = serializedNow,
                serverDifference = diff
            }
        });
    }


    private string GovTypeSelect(int govTypeID)
    {
        if (FbgPlayer.HasFlag(Fbg.Bll.Player.Flags.Misc_GovTypeChosen, false) == null)
        {
            FbgPlayer.ChooseGovType(govTypeID);
            FbgPlayer.SetFlag(Fbg.Bll.Player.Flags.Misc_GovTypeChosen, govTypeID.ToString());
        }
        return RETURN_SUCCESS(new
        {
            ok = true
        });
    }

    private string RewardForMobileAppRating()
    {
        InvalidateFbgUser(); // DOING THIS TO prevent double reward - want flags to be refreshed
        InvalidateFbgPlayer();
        if (FbgPlayer.User.HasFlag(Fbg.Bll.User.Flags.Reward_MobileAppRate) == null)
        {
            if (!Fbg.Bll.utils.Admin_GiveServants(FbgPlayer.User.ID, 50, utils.GiveServantsReason.RewardOrPromo))
            {
                // WE DO NOT Alert the player to this error but just log it 
                BaseApplicationException bex = new BaseApplicationException("Admin_GiveServants(FbgPlayer.User.ID, 50, utils.GiveServantsReason.RewardOrPromo) failed in API, RewardForMobileAppRating");
                bex.AddAdditionalInformation("FbgPlayer", FbgPlayer);
                ExceptionManager.Publish(bex);

                return RETURN_SUCCESS(new
                {
                    ok = false
                });
            }
            else
            {
                FbgPlayer.User.SetFlag(Fbg.Bll.User.Flags.Reward_MobileAppRate);
                return RETURN_SUCCESS(new
                {
                    ok = true
                });
            }
        }
        else
        {
            return RETURN_SUCCESS(new
            {
                ok = false
            });
        }
    }

    private string NotificationSettings_get()
    {
        List<Fbg.Bll.Api.PlayerNotificationSettings.NotificationSetting> settings = Fbg.Bll.Api.PlayerNotificationSettings.Get(FbgPlayer);

        return RETURN_SUCCESS(new
        {
            notificationsActiveOnRealm = !FbgPlayer.OptOutOfNotifications,
            NotfifcationSettings = settings

        });
    }

    private string NotificationSettings_update(int noficationID, Int16 MuteAtNightSettingID, Int16 soundSettingID, Int16 activeStateID)
    {
        return RETURN_SUCCESS(new
        {
            notificationsActiveOnRealm = !FbgPlayer.OptOutOfNotifications,
            NotfifcationSettings = Fbg.Bll.Api.PlayerNotificationSettings.Update(FbgPlayer, noficationID, MuteAtNightSettingID, soundSettingID, activeStateID)

        });
    }
    private string NotificationSettings_setRealmWideActive(bool onORoff)
    {
        FbgPlayer.OptOutOfNotifications = !onORoff;

        return RETURN_SUCCESS(new
        {
            notificationsActiveOnRealm = !FbgPlayer.OptOutOfNotifications,
            NotfifcationSettings = Fbg.Bll.Api.PlayerNotificationSettings.Get(FbgPlayer)
        });
    }

    private string GetCompletedUnclaimedQuestsCount()
    {
        //FbgPlayer.Quests2.CompletedQuests_RewardNotClaimed_Invalidate();
        return RETURN_SUCCESS(new
        {
            questsCompletedCount = FbgPlayer.Quests2.CompletedQuests_RewardNotClaimed().Count
        });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="originVID"></param> 
    /// <param name="targetVID"></param>
    /// <param name="commandType"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    /// <example>
    /// example of valid manual GET calls:
    /// ..../api.aspx?fn=cmd_execute&ovid=7&tvid=6&cmdtype=1&units=[{utID:6,sendCount:1,targetBuildingTypeID:1}]
    /// 
    /// </example>
    private string Cmd_Execute(int originVID, int targetVID, Fbg.Common.UnitCommand.CommandType commandType, List<Fbg.Common.UnitCommand.Units> unitsToSend, string scriptingCatchParam)
    {
        Village originVillage = Village.GetVillage(FbgPlayer, originVID, false, false);

        if (originVillage == null)
        {
            throw new Exception("wrong ovid");
            //TODO finish 
        }

        VillageOther targetVillage = VillageOther.GetVillage(FbgPlayer.ID, FbgPlayer.Realm, targetVID);

        if (targetVillage == null)
        {
            throw new Exception("wrong tvid");
            //TODO finish 
        }

        Fbg.Common.UnitCommand cmd = new UnitCommand();

        cmd.command = commandType;
        cmd.originVillageID = originVillage.id;
        cmd.targetVillageID = targetVillage.ID;
        cmd.UnitDesertionFactor = 0; //TODO finish
        cmd.unitsSent = unitsToSend;

        TimeSpan travelTime;
        DateTime arrivalTime;
        UnitMovements.Desertion desertion;
        Fbg.Bll.UnitType slowestUnit;
        int playerMoraleAfterCmd = FbgPlayer.Morale.Morale;
        DateTime playerMoraleLastUpdatedAfterCmd = FbgPlayer.Morale.MoraleLastUpdatedOn;

        Village.CanCommandUnitsResult canCommand = originVillage.CanCommandUnits(targetVillage, cmd, IsLoggedInAsSteward, out travelTime, out arrivalTime
            , out desertion, out slowestUnit, FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.attackSpeedUp), FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.subscription));

        if (canCommand == Village.CanCommandUnitsResult.Yes)
        {
            canCommand = originVillage.CommandUnits(targetVillage, slowestUnit, cmd, false, false, FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.attackSpeedUp), out playerMoraleAfterCmd, out playerMoraleLastUpdatedAfterCmd);
            FbgPlayer.Morale_Set(playerMoraleAfterCmd, playerMoraleLastUpdatedAfterCmd);

            try
            {
                if (targetVillage.IsOwnerSpecialPlayer)
                {

                    if (DoBootDetectionNow())
                    {
                        if (Session["boots_attackLog"] == null)
                        {
                            Session["boots_attackLog"] = new List<DateTime>();
                        }
                        List<DateTime> l = (List<DateTime>)Session["boots_attackLog"];
                        l.Add(DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
                BaseApplicationException.AddAdditionalInformation(col, "FbgUser", FbgUser);
                BaseApplicationException.AddAdditionalInformation(col, "LoggedInFacebookUser", LoggedInFacebookUser);
                Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(new Exception("Error in boot protection", ex), col);
                //
                // we eat the exception on purpose. do not want failure if login cannot be logged. 
            }
        }

        return RETURN_SUCCESS(new { canCommand = canCommand, playerMoraleAfterCmd = playerMoraleAfterCmd, playerMoraleLastUpdatedAfterCmd = playerMoraleLastUpdatedAfterCmd });
    }

    private string OtherVillageInfo(int vid)
    {
        Fbg.Bll.VillageOther vo = Fbg.Bll.VillageOther.GetVillage(FbgPlayer.ID, FbgPlayer.Realm, vid);

        Fbg.Bll.ClanDiplomacy clanDiplomacy = null;
        if (vo.ClanID != 0 && FbgPlayer.Clan != null)
        {
            clanDiplomacy = Fbg.Bll.Clan.ViewMyClanDiplomacy(FbgPlayer);
        }

        if (vo != null)
        {
            var cpProtectionEndsInDays = 0;
            if (vo.IsCapitalVillage)
            {
                if (vo.CapitalVillageProtectionEndsOn <= DateTime.Now)
                {
                    cpProtectionEndsInDays = 1; // we alwayas set a value < 1 to 1, even if it is < 0. WHY? see comment on vo.CapitalVillageProtectionEndsOn
                }
                else
                {
                    cpProtectionEndsInDays = Convert.ToInt32(Math.Ceiling(vo.CapitalVillageProtectionEndsOn.Subtract(DateTime.Now).TotalDays));
                }
            }

            DateTime inVacationTill = vo.IsInVacationModeUntill;
            DateTime inWeekendModeUntill = vo.IsInWeekendModeUntill;

            var vo2 = new
            {
                clanName = vo.Clan
                ,
                clanID = vo.ClanID
                ,
                coords = vo.Cordinates
                ,
                InSleepModeUntil = SerializeDate(vo.InSleepModeUntil)
                ,
                vo.IsCapitalVillage,
                IsCapitalVillage_ProtectionEndsInDays = cpProtectionEndsInDays
                ,
                vo.IsInSleepMode
                ,
                vo.IsUnderBeginnerProtection
                ,
                BeginnerProtectionEndsDate = Utils.FormatEventTime(vo.BeginnerProtectionEndsOn)
                ,
                vo.Note
                ,
                targetId = vid
                ,
                vo.OwnerName
                ,
                vo.OwnerPlayerID
                ,
                vo.OwnerPoints
                ,
                vo.Points
                ,
                vo.VillageName
                ,
                vo.VillageType.ID
                ,
                clanDiplomacy_isAlly = clanDiplomacy == null ? false : clanDiplomacy.IsAlly(vo.ClanID)
                ,
                clanDiplomacy_isNap = clanDiplomacy == null ? false : clanDiplomacy.IsNAP(vo.ClanID)

                ,
                IsInVacationMode = inVacationTill != DateTime.MinValue
                ,
                IsInVacationModeUntill = SerializeDate(inVacationTill)

                ,
                IsInWeekendMode = inWeekendModeUntill != DateTime.MinValue
                ,
                IsInWeekendModeUntill = SerializeDate(inWeekendModeUntill)

                ,
                // THE NEXT FEW LINES are left for compatibility. 
                //  as we merged the village_other call into this one, the roe.ui.villageoverview.js had no time to be changed to use the new properties, 
                //  so the properties fromn village_other are duplicated for compatibility
                id = vo.ID,
                name = vo.VillageName,
                points = vo.Points,
                x = vo.XCord,
                y = vo.YCord,
                note = Utils.ChangeLineBreaks(vo.Note),
                pid = vo.OwnerPlayerID,
                mine = vo.OwnerPlayerID == FbgPlayer.ID,
                player = new
                {
                    id = vo.OwnerPlayerID,
                    PN = vo.OwnerName,
                    clan = vo.ClanID == 0 ? null : new { id = vo.ClanID, CN = vo.Clan }
                }
                // END OF duplicates for village_other
            };

            return RETURN_SUCCESS(vo2);
        }
        else
        {
            return Json(new { success = false, @object = "VillageNotFound" });
        }
    }

    /// <summary>
    /// Star reports - puts reports in a special STARRED folder. This folder is a regular folder
    /// and when viewed on old D1 will show up as a folder just as any other. If the player
    /// already has a folder named STARRED (created through this or manually by the player) it 
    /// will use that folder. If not, it will be created.
    /// </summary>
    /// <param name="reportRecordIDs">Must a be a string of id's separated by , (comma) with no spaces</param>
    /// <returns></returns>
    protected string StarReports(string reportRecordIDs)
    {
        int moveToFolderID = -1;

        try
        {
            List<long> recs = new List<long>();
            recs.AddRange(reportRecordIDs.Split(',').Select(i => Convert.ToInt64(i)));
            //recs.ForEach(item => System.Diagnostics.Debug.WriteLine(item + ", "));

            //Fbg.Bll.Report.checkPlayer result = Fbg.Bll.Report.ForwardReport(forwardToPlayerName, _player, recs);

            // Create the Star folder if it does not exist yet
            if (FbgPlayer.Folders.AddFolder(Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred, Fbg.Bll.Folders.FolderType.Reports))
            {
                // Folder didn't exist, but now has been created...

                // Add the report to this folder (if not already in this folder)
                moveToFolderID = FbgPlayer.Folders.GetFolderIDByName(Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred, Folders.FolderType.Reports);
                Report.MoveReporsToFolder(FbgPlayer, recs, moveToFolderID);

            }
            else
            {
                // Folder already exists

                // Add the report to this folder (if not already in this folder)
                moveToFolderID = FbgPlayer.Folders.GetFolderIDByName(Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred, Folders.FolderType.Reports);
                Report.MoveReporsToFolder(FbgPlayer, recs, moveToFolderID);
            }

            // Build the output structure (which is formatted conveniently for the database js functions client side)
            object retVal = recs.AsEnumerable().Select(r => new { id = r, folderID = moveToFolderID, folderName = Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred });

            // { id: 'id', fields: [{ 'id': 10, 'folderID': r.folderID, 'folderName': r.folderName }]
            return ApiHelper.RETURN_SUCCESS(retVal);
        }
        catch (Exception exc)
        {
            return ApiHelper.RETURN_FAILURE(exc.Message);
        }
    }

    /// <summary>
    /// Unstars reports - Moves the report of the STARRED folder. If STARRED does not exist, it will not be created
    /// and nothing will happen except an failed api will be received. The reports folder becomes null (which
    /// essentially removes it from any folder and places it in the Inbox again).
    /// </summary>
    /// <param name="reportRecordIDs">Must a be a string of id's separated by , (comma) with no spaces</param>
    /// <returns>The results</returns>
    protected string UnstarReports(string reportRecordIDs)
    {
        try
        {
            List<long> recs = new List<long>();
            recs.AddRange(reportRecordIDs.Split(',').Select(i => Convert.ToInt64(i)));

            // Check if starred exists and remove this report if the report is in it.          
            int moveToFolderID = FbgPlayer.Folders.GetFolderIDByName(Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred, Folders.FolderType.Reports);
            if (moveToFolderID == -1)
            {
                return ApiHelper.RETURN_FAILURE("Starred not setup");
            }

            // Set the folder for the report to -1 which converts to null (thus removing it from a folder)
            Report.MoveReporsToFolder(FbgPlayer, recs, -1);

            // Cannot assign null to a type property so define a null object and assign
            // that (because we can assign objects to type properties)
            object noFolderName = null;
            // Build the output structure (which is formatted conveniently for the database js functions client side)
            object retVal = recs.AsEnumerable().Select(r => new { id = r, folderID = -1, folderName = noFolderName });

            return ApiHelper.RETURN_SUCCESS(retVal);

        }
        catch (Exception exc)
        {
            return ApiHelper.RETURN_FAILURE(exc.Message);
        }
    }





    /// <summary>
    /// Star reports - puts reports in a special STARRED folder. This folder is a regular folder
    /// and when viewed on old D1 will show up as a folder just as any other. If the player
    /// already has a folder named STARRED (created through this or manually by the player) it 
    /// will use that folder. If not, it will be created.
    /// </summary>
    /// <param name="reportRecordIDs">Must a be a string of id's separated by , (comma) with no spaces</param>
    /// <returns></returns>
    protected string StarMail(string mailRecordIDs)
    {
        int moveToFolderID = -1;

        try
        {
            List<int> recs = new List<int>();
            recs.AddRange(mailRecordIDs.Split(',').Select(i => Convert.ToInt32(i)));

            // Create the Star folder if it does not exist yet
            if (FbgPlayer.Folders.AddFolder(Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred, Fbg.Bll.Folders.FolderType.Mail))
            {
                // Folder didn't exist, but now has been created...

                // Add the report to this folder (if not already in this folder)
                moveToFolderID = FbgPlayer.Folders.GetFolderIDByName(Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred, Folders.FolderType.Mail);
                Fbg.Bll.Mail.MoveMessageToFolder(FbgPlayer, Fbg.Bll.Mail.CONSTS.MessageType.Inbox, recs, moveToFolderID, true);

            }
            else
            {
                // Folder already exists

                // Add the report to this folder (if not already in this folder)
                moveToFolderID = FbgPlayer.Folders.GetFolderIDByName(Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred, Folders.FolderType.Mail);
                // Note: we don't care if it was in another folder, it will be moved to STARRED.
                Fbg.Bll.Mail.MoveMessageToFolder(FbgPlayer, Fbg.Bll.Mail.CONSTS.MessageType.Inbox, recs, moveToFolderID, true);
            }

            // Build the output structure (which is formatted conveniently for the database js functions client side)
            object retVal = recs.AsEnumerable().Select(r => new { id = r, folderID = moveToFolderID, folderName = Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred });

            // { id: 'id', fields: [{ 'id': 10, 'folderID': r.folderID, 'folderName': r.folderName }]
            return ApiHelper.RETURN_SUCCESS(retVal);
        }
        catch (Exception exc)
        {
            return ApiHelper.RETURN_FAILURE(exc.Message);
        }
    }

    /// <summary>
    /// Unstars reports - Moves the report of the STARRED folder. If STARRED does not exist, it will not be created
    /// and nothing will happen except an failed api will be received. The reports folder becomes null (which
    /// essentially removes it from any folder and places it in the Inbox again).
    /// </summary>
    /// <param name="reportRecordIDs">Must a be a string of id's separated by , (comma) with no spaces</param>
    /// <returns>The results</returns>
    protected string UnstarMail(string mailRecordIDs)
    {
        try
        {
            List<int> recs = new List<int>();
            recs.AddRange(mailRecordIDs.Split(',').Select(i => Convert.ToInt32(i)));

            // Check if starred exists and remove this report if the report is in it.          
            // *** We don't care that the report is in STARRED, only that it was in a folder, and now it won't be.***
            //int moveToFolderID = FbgPlayer.Folders.GetFolderIDByName(Fbg.Bll.Folders.CONSTS.SpecialFolderNames.Starred, Folders.FolderType.Mail);
            //if (moveToFolderID == -1)
            //{
            //    return ApiHelper.RETURN_FAILURE("Starred not setup");
            //}

            // Set the folder for the report to -1 which converts to null (thus removing it from a folder)
            Fbg.Bll.Mail.MoveMessageToFolder(FbgPlayer, Fbg.Bll.Mail.CONSTS.MessageType.Inbox, recs, -1, true);

            // Cannot assign null to a type property so define a null object and assign
            // that (because we can assign objects to type properties)
            object noFolder = null; // <-- not using folder name anymore

            // Build the output structure (which is formatted conveniently for the database js functions client side)
            object retVal = recs.AsEnumerable().Select(r => new { id = r, folderID = -1 });

            return ApiHelper.RETURN_SUCCESS(retVal);

        }
        catch (Exception exc)
        {
            return ApiHelper.RETURN_FAILURE(exc.Message);
        }
    }






    private struct LogErrorError
    {
        public string errormsg;
        public int count;
    }

    protected void LogError()
    {
        string msg = Request.Form["msg"];
        string actualErrorMsg;
        //json_serializer.Deserialize<List<Object>>(msg);
        List<LogErrorError> errors = json_serializer.Deserialize<List<LogErrorError>>(msg);
        foreach (LogErrorError e in errors)
        {
            actualErrorMsg = e.count > 1 ? string.Format("{{\"message\":{0},\"NumberOfTimesThisSameErrorOccured\":{1}}}", e.errormsg, e.count) : e.errormsg;
            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(actualErrorMsg);
        }
    }



    protected string PlayerFull(string vid)
    {
        //Village v = Fbg.Bll.Village.GetVillage(FbgPlayer, Convert.ToInt32(vid), false, true);

        Dictionary<string, object> player = PlayerRefreshOrFullCommonStuff();

        player.Add("name", FbgPlayer.Name);
        player.Add("titleName", FbgPlayer.TitleName);
        player.Add("titleLvl", FbgPlayer.Title.Level);
        player.Add("titleIDR", NumberToRomanConvertor.NumberToRoman(FbgPlayer.Title.ID + 1));
        player.Add("credits", FbgPlayer.User.Credits);
        player.Add("PFPckgs", Helper_GetPlayersPFPackages());

        player.Add("creditFarmBonusDateEnds", ApiHelper.SerializeDate(FbgPlayer.Realm.CreditFarmBonusDateEnds));
        player.Add("creditFarmBonusMultiplier", FbgPlayer.Realm.CreditFarmBonusMultiplier);
        player.Add("creditFarmBonusDesc", FbgPlayer.Realm.CreditFarmBonusDesc);
        player.Add("creditFarmBonusIcon", FbgPlayer.Realm.CreditFarmBonusIcon);

        player.Add("itemGroups", FbgPlayer.Items2ItemGRoups);
        player.Add("AvatarList", Fbg.Bll.Api.MiscApi.AvatarsGetListAll(FbgPlayer.User.ID));
        player.Add("avatarID", FbgPlayer.Avatar.AvatarID);

        

        json_serializer.RegisterConverters(new JavaScriptConverter[] {
            new Fbg.Bll.Api.ApiHelper.Converter(),
            new Fbg.Bll.Api.ApiHelper.Items2Converter()
        });
        return json_serializer.Serialize(new { success = true, @object = player });
    }


    protected string PF_GetPlayerPFPackages()
    {
        var player = new
        {
            PFPckgs = Helper_GetPlayersPFPackages()
        };

        json_serializer.RegisterConverters(new JavaScriptConverter[] {
            new Fbg.Bll.Api.ApiHelper.Converter()});
        return json_serializer.Serialize(new { success = true, @object = player });
    }

    private object Helper_GetPlayersPFPackages()
    {

        return FbgPlayer.PF_PlayerPFPackages2.FindAll(s => { return true; })
            .ToDictionary(p => p.Package.Id.ToString(),
             p => new
             {
                 id = p.Package.Id
                 ,
                 ExpiresOn = SerializeDate(p.ExpiresOn)
             });
    }




    protected string PlayerRefresh(string vid, DateTime villageCacheLastProcessedTimeStamp)
    {
        FbgPlayer.LastHandledVillageCacheTimeStamp = villageCacheLastProcessedTimeStamp;
        var villagesChanged = from rows in FbgPlayer.ListOfVillagesChangedSinceLastHandledVillageCacheTimeStamp.AsEnumerable()
                              select new { vid = rows.Field<int>("VillageID").ToString(), ts = rows.Field<DateTime>("timestamp").ToString("MM/dd/yyyy HH:mm:ss:fff") };

        Dictionary<string, object> player = PlayerRefreshOrFullCommonStuff();

        player.Add("chooseGovType", (PlayerRefHelper_ShowGovSelection()));
        player.Add("CurrentBuildID", BuildNumConfig.CurrentBuildID);
        player.Add("CurrentBuildIDWhatsNew", BuildNumConfig.CurrentBuildIDWhatsNew);
        player.Add("ChangedVillages", villagesChanged);
        player.Add("LocalDBVersion", FbgPlayer.Realm.LocalDBVersion);

        if (Config.SaleType != 0)
        {
            player.Add("isSaleActive", true);
        }
        if (Config.FriendReward_SaleEndsOn > DateTime.Now)
        {
            player.Add("FriendRewardBonusUntil", ApiHelper.SerializeDate(Config.FriendReward_SaleEndsOn));
        }

        HandleVideoAd(player);


        HandleBoots(player);

        try
        {
            // not that the person is active (has logged-in) every 60 minutes
            if (!IsLoggedInAsSteward && !IsThisAdminLoggedInAsThisPlayer)
            {
                if (!SessionCache.stillInCache("LogLogin logged"))
                {
                    SessionCache.put("LogLogin logged", 3600000/*60 minutes*/);
                    long remotePort = 0;
                    Int64.TryParse(Request.ServerVariables["REMOTE_PORT"], out remotePort);
                    Fbg.Bll.utils.LogLogin(FbgPlayer
                        , Request.ServerVariables["REMOTE_ADDR"]
                        , remotePort
                        , Request.ServerVariables["HTTP_USER_AGENT"]);
                }
            }
        }
        catch (Exception ex)
        {
            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            BaseApplicationException.AddAdditionalInformation(col, "FbgUser", FbgUser);
            BaseApplicationException.AddAdditionalInformation(col, "LoggedInFacebookUser", LoggedInFacebookUser);
            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(new Exception("error while trying to Log the login ", ex), col);
            //
            // we eat the exception on purpose. do not want failure if login cannot be logged. 
        }


        json_serializer.RegisterConverters(new JavaScriptConverter[] {
            new Fbg.Bll.Api.ApiHelper.Converter()});
        return json_serializer.Serialize(new { success = true, @object = player });
    }

    private bool DoBootDetectionNow()
    {
        if (FbgPlayer.Realm.ID == 318)
        {
            return true;
        }
        return false;
    }

    private void HandleBoots(Dictionary<string, object> player)
    {
        if (!DoBootDetectionNow())
        {
            return;
        }
        int const_numAtt = 50;
        int const_showEveryXMinutes = 30;
        try
        {
            
            challengepayload c = Session["boots_challenge"] == null ? null : (challengepayload)Session["boots_challenge"];
            if (c != null)
            {
                // if challenge is active, and not yet responded to, add it to return
                if (c.correctlyAnsweredOn == DateTime.MinValue)
                {
                    player.Add("challenge", c.msg);
                    return;
                }
                // challenge already responded to, and time required between challenges, has not yet elapsed
                else if (c.correctlyAnsweredOn.AddMinutes(const_showEveryXMinutes) > DateTime.Now)
                {
                    return;
                }

                // this means the challenge has been answered, and the required time between challenges, has elapsed
                c = null;
                Session["boots_challenge"] = null;
            }



            //
            // calculate if a challenge is to be presented 
            //
            int a,b, answer;
            string msg ;
            if (Session["boots_attackLog"] != null)
            {
                Random r = new Random();
                int numbAtt = const_numAtt + r.Next(const_numAtt/2);
                
                List<DateTime> l = (List<DateTime>)Session["boots_attackLog"];
                if (l.Count > numbAtt)
                {
                    if (l[l.Count - numbAtt] > DateTime.Now.AddMinutes(0 - const_showEveryXMinutes))
                    {
                        a = r.Next(4) + 1;
                        b = r.Next(3) + 1;
                        if (b > a)
                        {
                            b = a;
                        }
                        msg = string.Format("{0} - {1} = ", a, b);
                        answer = a - b;
                        player.Add("challenge", msg);
                        FbgPlayer.LogEvent(19 /*Fbg.Bll.CONSTS.UserLogEvents.AttackScriptPrevention_challenge*/
                            , msg, null);

                        Session["boots_challenge"] = new challengepayload() { a = a, b = b, msg = msg };
                    }
                }
            }           
        }
        catch (Exception ex)
        {
            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            BaseApplicationException.AddAdditionalInformation(col, "FbgUser", FbgUser);
            BaseApplicationException.AddAdditionalInformation(col, "LoggedInFacebookUser", LoggedInFacebookUser);

            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(new Exception("Error in boot protection", ex), col);
            //
            // we eat the exception on purpose. do not want failure if login cannot be logged. 
        }
    }

    private void HandleVideoAd(Dictionary<string, object> player)
    {
        if (!isMobile) { return; }

        // this tells you, how many minutes after we request that that mobile app checkes if video add is avaialble, that we keep checking for the flag
        //  that will be set by the api called by the mobile app ,to say that the video ad is available; if the call comes later than those minutes, then 
        //  this session will never get the flag. 
        const int numMinutesToKeepGettingIsAvailFlagFromDB = 2;
        const int expireIsAvailableFlagInMin = 60; //2
        const int minToWaitTillNextIsAvailableCheck = 60 * 4; //1; 

        /*
         player REFRESH
           if AailFLAG == YES 
               if AvailFlag more than 15 min old, clear it 
               else 
                   tell UI to display icon. 
           else 
                if CheckedFlag missing or 4 hours ago last checked            tell UI to check if ad is available. 
         * */

        // check if "add is available" 
        //  we do this by getting a flag. and, to ensure we read the latest flag, we grab it from the database, for 2 minutes since 
        //  we last requested to check if ad is available. 
        //  IF ad is available, an deviceapi call gets made, Fbg.Bll.User.Flags.VideAd_IsAvailable is set to "available" and this code, will grab this value from DB
        //      if the api call comes in withing 2 minutes from requesting it, which it should. 
        DateTime timeWhenLastCheckedIfAdAvailable = FbgUser.HasFlag(Fbg.Bll.User.Flags.VideAd_Checked) != null ? (DateTime)FbgUser.HasFlag(Fbg.Bll.User.Flags.VideAd_Checked) : DateTime.MinValue;
        bool forceDBCheck = DateTime.Now.Subtract(timeWhenLastCheckedIfAdAvailable).TotalMinutes < numMinutesToKeepGettingIsAvailFlagFromDB ? true : false;
        object isAvailableAdFlag = FbgUser.HasFlag(Fbg.Bll.User.Flags.VideAd_IsAvailable, !forceDBCheck);

        if (isAvailableAdFlag != null
            && !(FbgUser.HasFlag_GetData(Fbg.Bll.User.Flags.VideAd_IsAvailable) is DBNull)
            && (string)FbgUser.HasFlag_GetData(Fbg.Bll.User.Flags.VideAd_IsAvailable) == "available")
        {
            // if "is Available" flag is 15 min old, then it could be too old, so reset it, which will force the system to again ask if an ad is available. 
            if (DateTime.Now.Subtract((DateTime)isAvailableAdFlag).TotalMinutes > expireIsAvailableFlagInMin)
            {
                FbgUser.SetFlag(Fbg.Bll.User.Flags.VideAd_IsAvailable, "");
            }
            else
            {
                player.Add("vaa", 1); // means add is available
            }
        }
        else
        {
            //player.Add("vaa", DateTime.Now.Subtract(timeWhenLastCheckedIfAdAvailable).TotalMinutes);
            if (DateTime.Now.Subtract(timeWhenLastCheckedIfAdAvailable).TotalMinutes > minToWaitTillNextIsAvailableCheck)
            {
                player.Add("vaa", 2); // means check if add is available
                FbgUser.SetFlag(Fbg.Bll.User.Flags.VideAd_Checked);
            }
        }
    }

    private bool PlayerRefHelper_ShowGovSelection()
    {
        bool show = false;
        if (FbgPlayer.Realm.GovernmentTypesEnabled == 1
        && FbgPlayer.HasFlag(Fbg.Bll.Player.Flags.Misc_GovTypeChosen) == null)
        {
            if (FbgPlayer.Realm.RealmType_isNoob)
            {
                if (FbgPlayer.Title.Level >= Fbg.Bll.CONSTS.TitleLevels.GKnight)
                {
                    show = true;
                }
            }
            else
            {
                show = true;
            }

        }
        return show;
    }

    private Dictionary<string, object> PlayerRefreshOrFullCommonStuff()
    {
        FbgPlayer.ForceRetrievePlayerExtraInfoNextTime();

        //Fbg.Bll.Player.IncomingAttackInfo incoming = FbgPlayer.IncomingAttack();
        //var incomingSerialized = new { firstAttackArrivalTime = SerializeDate(incoming.FirstAttackArrivalTime), numAttacks = incoming.NumAttacks };


        Fbg.Bll.Player.DailyRewardStatus DailyRewardStatus = FbgPlayer.GetDailyRewardStatus();
        Fbg.Bll.Player.VacationModeStatus VacationModeStatus = FbgPlayer.GetVacationModeStatus();
        Fbg.Bll.Player.WeekendModeStatus weekendModeStatus = FbgPlayer.GetWeekendModeStatus();

        List<Fbg.Bll.Api.MapEvents.MapEvents.PlayerMapEvent> PlayerMapEventsList = Fbg.Bll.Api.MapEvents.MapEvents.GetPlayerMapEvents(FbgPlayer);
        //List<Fbg.Bll.Api.Raids.Raids.Raid> PlayerRaidsLisr = Fbg.Bll.Api.Raids.Raids.GetPlayerRaids(FbgPlayer);

        Dictionary<string, object> player = new Dictionary<string, object>()
        {
            {"playerID", FbgPlayer.ID },
            {"ppoints", FbgPlayer.Points },

            //quest related
            {"questsCompl", FbgPlayer.Quests2.CompletedQuests_RewardNotClaimed().Count },
            { "nextRecommendeQuests",  Fbg.Bll.Api.MiscApi.GetNextRecommendeQuestRaw(FbgPlayer) },

            { "Indicators", new { report = FbgPlayer.ReportInfo, mail = FbgPlayer.MessageInfo, clanForum = FbgPlayer.ForumChanged } },
            //{"incomingAtt", incomingSerialized },
            {"titleProgress", coe.TitleProgress(FbgPlayer) },
            {"Clan", (FbgPlayer.Clan == null ? null : new { id = FbgPlayer.Clan.ID }) },
            {"CacheItems", FbgPlayer.CacheItemInfo.raw.ToDictionary(r => r.Key.ToString(), r => SerializeDate(r.Value))},
            {"SleepMode_IsActiveNow", FbgPlayer.SleepMode_IsActiveNow},
            {"PlayersNumVillages", FbgPlayer.GetVillages_BasicA(null).Count()},

            //Daily Reward
            {"DailyRewardAvail", DailyRewardStatus.available },
            {"DailyRewardLevel", DailyRewardStatus.level },
            {"DailyRewardNext", ApiHelper.SerializeDate(DailyRewardStatus.nextTime) },

            {"RecEmailState", (int)FbgUser.GetRecoveryEmailState(LoggedInMembershipUser.Email)},
            {"RecEmail", ((LoggedInMembershipUser.Email == Fbg.Bll.CONSTS.DummyEmail) ? "" : LoggedInMembershipUser.Email)},

            // crude temp code that should change once tactica work is completed
            {"RecEmail_isDeviceLogin",  FbgUser.LoginType_isMobileDeviceIDLogin || FbgUser.LoginType_isAmazon},
            {"RecEmail_isTacticaLogin", Utils.GetLoginType(LoggedInMembershipUser) == Fbg.Common.Utils.LoginType.Tactica},
            {"RecEmail_isKLogin",  FbgUser.LoginType_isKong}, 

            //Sleep Mode
            {"SleepModeOn", SerializeDate(FbgPlayer.SleepMode_ActivatingOn) },
            {"SleepModeCountdown", Utils.FormatDuration(FbgPlayer.SleepMode_CanActivateIn.Subtract(DateTime.Now)) },
            {"SleepinRealm", FbgPlayer.Realm.SleepModeGet }, // object, has: IsAvailableOnThisRealm, Duration, TimeTillActive  
   
            //Vacation Mode
            {"VacationStatus", VacationModeStatus },
            {"VacationInRealm", FbgPlayer.Realm.VacationModeGet }, //allowed, baseMaxDays, minimumdays, activationDelay

            //Weekend Mode
            {"WeekendModeStatus", weekendModeStatus },
            {"WeekendModeInRealm", FbgPlayer.Realm.WeekendModeGet }, //Allowed, RealmBaseDays, ActivationDelayMinimumHours, ReactivationDelayDays
            
            //MapEvents
            {"MapEvents", PlayerMapEventsList },

            {"RestartCost", FbgPlayer.GetRestartCost()},
            {"NumberOfVillages", FbgPlayer.NumberOfVillages},
            {"MyResearch",  new {numOfIdleResearchers = FbgPlayer.MyResearch.numOfIdleResearchers(LoginModeHelper.isFB(Request))
                , researcherIdleIn = FbgPlayer.MyResearch.researcherIdleIn == null ? 0 : SerializeDate((DateTime)FbgPlayer.MyResearch.researcherIdleIn)  }},
            {"LocalStorageOnServer", FbgUser.HasFlag_GetData(Fbg.Bll.User.Flags.LocalStorageOnServer)},
            {"chestCount", FbgPlayer.Chests},
            {"beginnerProtected", FbgPlayer.BeginnerProtection_IsNowActive},
            {"beginnerProtection", ApiHelper.SerializeDate(FbgPlayer.BeginnerProtection_ExpiresOn)},
            {"approvalBoostAllowedWhen", SerializeDate(FbgPlayer.HasFlag(Player.Flags.Misc_BoostedApproval) == null ? DateTime.Now.AddDays(-1) : ((DateTime)FbgPlayer.HasFlag(Player.Flags.Misc_BoostedApproval)).AddDays(1)) },
            {"morale",FbgPlayer.Morale.Morale },
            {"Targets", FbgPlayer.DefinedTargets.Get() },
            {"NextRaid", SerializeDate(FbgPlayer.Raids_TopRaidActByTime) },
            {"NumOfRaidRewardsToCollect", FbgPlayer.Raids_NumRaidsToCollectReward },

        };


        if (FbgUser.Offers_HasOffer(Fbg.Bll.User.Offers.Number2))
        {
            player.Add("hasOffer2", FbgUser.Offers_GetServantOfferAmount(Fbg.Bll.User.Offers.Number2));
        }

        if (FbgPlayer.Realm.ConsolidationGet.IsAttackFreezeActiveNow && FbgPlayer.Realm.ConsolidationGet.TimeOfConsolidation != FbgPlayer.Realm.ConsolidationGet.AttackFreezeStartOn)
        {
            player.Add("IsConsolidationAttackFreezeActiveNow", FbgPlayer.Realm.ConsolidationGet.IsAttackFreezeActiveNow);
        }


        return player;
    }

    protected string VillageListA()
    {
        List<VillageBasicA> list = FbgPlayer.GetVillages_BasicA(null);

        var list2 = list.Select(v => new
        {
            id = v.id,
            name = v.name,
            x = v.Cordinates.X,
            y = v.Cordinates.Y,
            points = v.Points,
            villagetypeid = v.VillageType.ID
        });

        return json_serializer.Serialize(new
        {
            success = true,
            @object = list2,
            playerid = FbgPlayer.ID
        });
    }



    protected string myVillageExtendedBasicInfo()
    {

        var villageId = Convert.ToInt32(Request["vid"]);
        var village = VillageBasicB.GetVillage(FbgPlayer, villageId, false);

        json_serializer.RegisterConverters(new JavaScriptConverter[] {
            new Fbg.Bll.Api.ApiHelper.Converter()});
        return json_serializer.Serialize(new
        {
            success = true,
            @object =
                new
                {
                    village = village
                }
        });
    }

    protected string VillageDetail()
    {
        var villageId = Convert.ToInt32(Request["vid"]);
        var village = Village.GetVillage(FbgPlayer, villageId, false, false);



        return ApiHelper.RETURN_SUCCESS(new Fbg.Bll.Api.VillageApi(FbgPlayer).GetVOVInfo(village));
    }

    protected string BuyResearcher()
    {
        /*    
         * stats: OK | lackservants | maxresearchers
         * 
         * */
        Fbg.Common.BuyResearcherResult ret = FbgPlayer.Researchers_Buy(LoginModeHelper.isFB(Request));
        int boughtResearchers = FbgPlayer.Researchers_Bought(true); //this line is for refresh purpose only

        return json_serializer.Serialize(new
        {
            success = true,
            @object = new
            {
                status = (ret == Fbg.Common.BuyResearcherResult.ok ? "OK" : (ret == Fbg.Common.BuyResearcherResult.failed_noCredits ? "lackservants" : "maxresearchers"))
            ,
                servants = FbgUser.Credits
            }
        });
    }


    /// <summary>
    /// Returns VillageBasicA.CanResearchResult :
    ///     {
    ///         OK=0,                   //0
    ///         NO_AlreadyResearched,   //1
    ///         NO_ReqNotMet,           //2
    ///         NO_ResearchersBusy      //3
    ///     }
    ///     
    /// Not sure what happens when no silver to research... 
    /// 
    /// If research item is already researching, you will get a response OK but 
    ///     the reesarch will not be initiated twice, silver will not be deducted etc; all will be well, just a stupid return value
    /// 
    /// </summary>
    /// <param name="villageID"></param>
    /// <returns></returns>
    protected string ResearchDo(string villageIDRaw)
    {
        int villageID = Convert.ToInt32(villageIDRaw);
        int riid = Convert.ToInt32(Request.QueryString["rid"]);
        ResearchItem ri = FbgPlayer.Realm.Research.ResearchItemByID(1, riid);

        VillageBasicB vb = FbgPlayer.VillageBasicB(villageID);

        VillageBasicA.CanResearchResult result = vb.DoResearch(ri, LoginModeHelper.isFB(Request));


        return RETURN_SUCCESS(result);
    }

    protected string TutorialDone(string villageId)
    {
        var _village = FbgPlayer.VillageBasicB(Convert.ToInt32(villageId));

        _village.owner.SetFlag(Fbg.Bll.Player.Flags.Advisor_BeginnerTutorialCompleted);
        _village.owner.Quests2.CompletedQuests_RewardNotClaimed_Invalidate();

        return RETURN_SUCCESS(null);
    }

   
    public string Research_getResearchers()
    {

        Neighbours _researchers;

        NeighbourListEntry_PlayingFriend n;
        string friendsList = String.Empty;


        _researchers = new Neighbours();
        List<NeighbourListEntry_Person> list = new List<NeighbourListEntry_Person>(1);

        int boughtResearchers = FbgPlayer.Researchers_Bought();

        if (LoginModeHelper.isFB(Request))
        {
            #region Under Facebook - include friends

            //
            // add bought researchers
            //
            int b = 0;
            while (list.Count < Math.Min(FbgPlayer.Researchers_Bought(), FbgPlayer.Researchers_All(LoginModeHelper.isFB(Request)) - 1))
            {
                b++;
                list.Add(new NeighbourListEntry_PlayingFriend("faculty") { Title = "dummy-preventsremoval", ImageUrl = "https://static.realmofempires.com/images/research/researcher0" + b + ".png" });
            }
            //
            // add friends if necessary
            //
            if (list.Count < 4)
            {

                List<Fbg.Common.UsersFriend> playersFriends = FbgPlayer.Researchers_Friends(false);
                if (playersFriends.Count() > 0)
                {

                    foreach (Fbg.Common.UsersFriend userFriend in playersFriends)
                    {
                        n = new NeighbourListEntry_PlayingFriend("Friend") { ImageUrl = "https://static.realmofempires.com/images/research/researcher0" + (++b) + ".png" };
                        n.Title = "dummy-preventsremoval";
                        list.Add(n);
                        if (list.Count >= 4) { break; }
                    }
                }
            }


            #endregion
        }
        else
        {
            //
            // bought researchers
            //
            int b = 0;
            while (list.Count < FbgPlayer.Researchers_All(LoginModeHelper.isFB(Request)) - 1)
            {
                b++;
                list.Add(new NeighbourListEntry_PlayingFriend("faculty") { Title = "dummy-preventsremoval", ImageUrl = "https://static.realmofempires.com/images/research/researcher0" + b + ".png" });
            }
        }

        //
        // add me to the list
        //
        list.Insert(0, new NeighbourListEntry_Me("YOU") { Level = 0, Title = FbgPlayer.TitleName, XP = FbgPlayer.Points, ImageUrl = FbgPlayer.Avatar.ImageUrlS });



        return RETURN_SUCCESS(new { researchers = from entry in list select new { name = entry.Name, url = entry.ImageUrl_Secure }, maxResearchers = FbgPlayer.Realm.Research_MaxResearchersAllowed });
    }
    /// <summary>
    /// </example>
    public string Research_catchup()
    {

        int researchItemIDSpedUp;
        DateTime completionTimeBeforeSpeedUp;
        int costOfTheCatchup;

        bool success = FbgPlayer.MyResearch.SpeedUp_ViaCatchup(out researchItemIDSpedUp, out completionTimeBeforeSpeedUp, out costOfTheCatchup);

        if (success)
            return RETURN_SUCCESS(new
            {
                success = 1,
                playersCreditsNow = FbgPlayer.User.Credits,
                costOfCatchup = costOfTheCatchup
            });
        else
        {
            return RETURN_SUCCESS(new { success = 0 });
        }
    }




    protected string AcceptNewTitle()
    {

        FbgPlayer.Title_AcceptNext();

        // we no longer do it, see case 19912, and especially 19913
        // coe.SendMessageAfterTitleAcceptIfNeeded(FbgPlayer); 

        
        return RETURN_SUCCESS(new
        {
            titleProgress = coe.TitleProgress(FbgPlayer)
        }
           );
    }
    protected string BoostApproval(int vid)
    {
        Village vb = FbgPlayer.Village(vid);
        Village.BoostApprovalResults result = vb.BoostApproval();

        return ApiHelper.RETURN_SUCCESS(
          new
          {
              boostSuccessful = result == Village.BoostApprovalResults.boosted,
              ifNotBoostedThenisDueToCoolDown = result == Village.BoostApprovalResults.notboosted_cooldownperiod,
              loyalty = vb.Loyalty,
              creditsLeft = FbgPlayer.User.Credits,
              Village = new Fbg.Bll.Api.VillageApi(FbgPlayer).GetVillage_raw(vb)
          }, new Fbg.Bll.Api.ApiHelper.Converter());

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>0 means sucess ;1 means not enough servants</returns> 
    protected string PF_ActivatePFPackages()
    {
        int packageID = Convert.ToInt32(Request.QueryString["pckgid"]);
        int retval = FbgPlayer.PF_ActivatePackage(packageID);


        if (packageID == 22)
        {
            FbgPlayer.Quests2.SetQuestAsCompleted(Fbg.Bll.Player.QuestTags.PF1.ToString());
        }

        return json_serializer.Serialize(new
        {
            success = true,
            @object = new
            {
                status = retval
                ,
                servants = FbgUser.Credits
                ,
                PFPckgs = Helper_GetPlayersPFPackages()
                ,
                activatedPackageID = packageID
            }
        });
    }



    protected string IncomingAttacks()
    {
        Fbg.Bll.Player p = FbgPlayer;
        Fbg.Bll.Player.IncomingAttackInfo att = FbgPlayer.IncomingAttack();

        return json_serializer.Serialize(new
        {
            success = true,
            @object = att
        });
    }

    protected string MapLand()
    {
        var pl = FbgPlayer;

        string coord = Request["coords"];
        string landcoords = Request["landcoords"];
        bool hasAllLandmarkTypes = Convert.ToBoolean(Request["hasAllLandTypes"]);

        Fbg.Bll.Clan clan = FbgPlayer.Clan;
        int clanID = clan == null ? -1 : clan.ID;

        DataSet dsMap = Fbg.Bll.Map.GetMapInfo(pl.Realm, coord, landcoords, clanID, pl.ID, false, hasAllLandmarkTypes);

        return json_serializer.Serialize(new
        {
            success = true,
            @object = new MapJson(dsMap, FbgPlayer),
            playerid = FbgPlayer.ID
        });
    }


    protected string MailList()
    {
        var ds = Fbg.Bll.Mail.getInboxAll(FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr, true /*archived*/);

        return Json(new
        {
            success = true,
            @object = ds.Tables[0].AsEnumerable().Select(
                v => new
                {
                    id = v[Fbg.Bll.Mail.CONSTS.InboxColumnIndex.RecordID].ToString(),
                    subject = v[Fbg.Bll.Mail.CONSTS.InboxColumnIndex.Subject].ToString(),
                    timesent = (DateTime)v[Fbg.Bll.Mail.CONSTS.InboxColumnIndex.TimeSent],
                    sender = v[Fbg.Bll.Mail.CONSTS.InboxColumnIndex.SenderName].ToString(),
                    viewed = (bool)v[Fbg.Bll.Mail.CONSTS.InboxColumnIndex.IsViewed],
                    folderID = v[Fbg.Bll.Mail.CONSTS.InboxColumnIndex.FolderID]
                }
            ),
            playerid = FbgPlayer.ID
        });
    }

    protected string MailSentList()
    {
        var ds = Fbg.Bll.Mail.getSentItems(FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr, true /*archived*/);

        return Json(new
        {
            success = true,
            @object = ds.Tables[0].AsEnumerable().Select(
                v => new
                {
                    id = v[Fbg.Bll.Mail.CONSTS.SentColumnIndex.RecordID].ToString(),
                    subject = v[Fbg.Bll.Mail.CONSTS.SentColumnIndex.Subject].ToString(),
                    timesent = (DateTime)v[Fbg.Bll.Mail.CONSTS.SentColumnIndex.TimeSent],
                    sender = v[Fbg.Bll.Mail.CONSTS.SentColumnIndex.SenderName].ToString(),
                    receiver = v[Fbg.Bll.Mail.CONSTS.SentColumnIndex.ReceiverNames].ToString(),
                    viewed = (bool)v[Fbg.Bll.Mail.CONSTS.SentColumnIndex.IsViewed]
                }
            ),
            playerid = FbgPlayer.ID
        });
    }

    protected string MailBlocked()
    {
        var dt = Fbg.Bll.Mail.GetBlockedPlayers(FbgPlayer);

        return Json(new
        {
            success = true,
            @object = dt.AsEnumerable().Select(
                v => new {
                    id = v["PlayerID"].ToString(),
                    name = v["Name"].ToString()
                }
            ),
            playerid = FbgPlayer.ID
        });
    }

    protected string MailUnBlock()
    {
        Fbg.Bll.Mail.UnBlockPlayer(FbgPlayer, Convert.ToInt32(Request["bpid"]));

        return Json(new
        {
            success = true,
            @object = Convert.ToInt32(Request["bpid"]),
            playerid = FbgPlayer.ID
        });
    }

    protected string MailDelete()
    {
        int res = Fbg.Bll.Mail.deleteMessage(Request["recid"], FbgPlayer.Realm.ConnectionStr, FbgPlayer.ID);

        return Json(new
        {
            success = true,
            @object = res,
            playerid = FbgPlayer.ID
        });
    }

    protected string MailDetail()
    {
        DataTable dt = Fbg.Bll.Mail.getMessageDetail(FbgPlayer, Convert.ToInt32(Request["recid"]));

        DataRow v = dt.Rows[0];

        return Json(new
        {
            success = true,
            @object = new
            {
                id = v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.RecordID].ToString(),
                message = BBCodes.InternalMailToHTMLm(FbgPlayer, v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.Message].ToString(), IsiFramePopupsBrowser),
                message_bbcode = v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.Message].ToString(),
                subject = v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.Subject].ToString(),
                timesent = (DateTime)v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.SentTime],
                to = v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.RecipientNames].ToString(),
                from = v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.SenderName].ToString(),
                next = v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.NextRec].ToString(),
                prev = v[Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex.PrevRec].ToString()
            },
            playerid = FbgPlayer.ID
        });
    }

    private string MailRecipientsCheck()
    {
        string to = Request["to"].Replace(";", ",").Replace(" ", ",");

        DataSet ds = Fbg.Bll.Mail.CheckRecipientsLight(FbgPlayer, to);

        var notFoundPlayers = ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerNameDetails];

        if (notFoundPlayers.Rows.Count > 0)
        {
            return Json(new
            {
                success = false,
                @object = notFoundPlayers.AsEnumerable().Select(r => r.Field<string>(0)),
                playerid = FbgPlayer.ID
            });
        }

        return Json(new
        {
            success = true,
            playerid = FbgPlayer.ID
        });
    }


    private string MailCreate()
    {
        int result = 0;
        string csvIDs = "";
        string csvNames = "";
        string messageBody = "";
        string subject = "";
        string to = "";

        try
        {
            to = Request["to"].Replace(";", ",").Replace(" ", ",");

            DataSet ds = Fbg.Bll.Mail.CheckRecipientsLight(FbgPlayer, to);

            if (ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerIDDetails].Rows.Count > 0
                && ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerNameDetails].Rows.Count == 0)
            {
                for (int i = 0; i < ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerIDDetails].Rows.Count; i++)
                {
                    csvIDs += ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerIDDetails].Rows[i][0].ToString();
                    csvNames += ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerIDDetails].Rows[i][1].ToString();
                    if (i != ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerIDDetails].Rows.Count - 1)
                    {
                        csvIDs += ", ";
                        csvNames += ", ";
                    }
                }

                subject = Utils.ClearHTMLCode(Request["subject"].Trim());
                subject = Utils.ClearInvalidChars(subject);
                messageBody = Utils.CleanupInputText(Request["message"].Trim());
                messageBody = Utils.ClearInvalidChars(messageBody);
                messageBody = Utils.ChangeLineBreaks(messageBody);

                //Checking if Subject or MessageBody is blank after removing HTML Text
                if (subject.Trim() == "")
                {
                    return Json(new { success = false, @object = "EnterValidSubject" });
                }
                else
                {
                    if (Request["hide"] == "true")
                    {
                        csvNames = Fbg.Bll.Mail.CONSTS.HiddenRecipients;
                    }

                    //
                    // SEND THE EMAIL. Do any last minute steps
                    //
                    messageBody = BBCodes.PreProcessBBCodes(FbgPlayer.Realm, BBCodes.Medium.InternalMail, messageBody);
                    result = Fbg.Bll.Mail.sendEmail(FbgPlayer.ID, csvIDs, subject, messageBody, csvNames, FbgPlayer.Realm.ConnectionStr);
                    if (result == 0)
                    {
                        return Json(new
                        {
                            success = false,
                            @object = "UnableToSendMail",
                            playerid = FbgPlayer.ID
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = true,
                            playerid = FbgPlayer.ID
                        });
                    }

                    //if this is a reply or forward, then we have record id, then redirect back to original message 
                    //if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.RecordID]))
                    //{
                    //    recordID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RecordID]);
                    //    Response.Redirect(NavigationHelper.MessageDetails(recordID));
                    //}
                }
            }
            else
            {
                if (ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerNameDetails].Rows.Count > 0)
                {
                    //for (int i = 0; i < ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerNameDetails].Rows.Count; i++)
                    //{
                    //    LblErrMsg.Text += ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerNameDetails].Rows[i][0].ToString();
                    //    if (i != (ds.Tables[Fbg.Bll.Mail.CONSTS.CheckRecipientsTableIndex.PlayerNameDetails].Rows.Count - 1))
                    //    {
                    //        LblErrMsg.Text += ", ";
                    //    }
                    //}
                    //LblErrMsg.Text = LblErrMsg.Text.Replace("<", "&lt;");
                    //LblErrMsg.Text = LblErrMsg.Text.Replace(">", "&gt;");

                    return Json(new
                    {
                        success = false,
                        @object = "PlayersCouldNotBeFound",
                        playerid = FbgPlayer.ID
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        @object = "UnableToSendMail",
                        playerid = FbgPlayer.ID
                    });
                }
            }
        }
        catch (Exception ex)
        {
            BaseApplicationException be = new BaseApplicationException("Error in API.create_mail() method", ex);
            be.AdditionalInformation.Add("csvIDs", csvIDs);
            throw be;
        }
    }

    private PlayerOther GetPlayerOtherEnt()
    {
        PlayerOther po = null;

        int playerId = 0; Int32.TryParse(Request.QueryString["pid"], out playerId);
        string playerName = Request.QueryString["pname"];

        if (playerId != 0)
        {
            playerName = "";
            po = Fbg.Bll.PlayerOther.GetPlayer(FbgPlayer.Realm, playerId, FbgPlayer.ID);
        }
        else if (!String.IsNullOrEmpty(playerName))
        {
            playerId = 0;
            po = Fbg.Bll.PlayerOther.GetPlayer(FbgPlayer.Realm, playerName, FbgPlayer.ID);
        }
        /*
        if (po == null)
        {
            throw new Exception("NotFound");
        }
        */
        return po;
    }

    protected string PlayerSetAvatarID(int AvatarID)
    {

        bool success = FbgPlayer.SetAvatarID(AvatarID);
        if (success)
        {
            ChatHub2.ChatHub2.updateChatUserEntityPlayerAvatar(FbgPlayer);
            FbgPlayer.Quests2.SetQuestAsCompleted(Fbg.Bll.Player.QuestTags.Avatar_TryChange.ToString());
        }

        return RETURN_SUCCESS(new
        {
            success = success,
            AvatarID = AvatarID
        });

    }

    protected string PlayerSetSex(int sex)
    {
        //0 female, 1 male
        if (sex == 0 || sex == 1)
        {
            FbgPlayer.Update(true, sex == 0 ? Sex.Female : Sex.Male);

            //FbgPlayer.Realm.TitleByID(FbgPlayer.Title.TitleName

            return RETURN_SUCCESS(new
            {
                success = true,
                sex = sex,
                title = FbgPlayer.Title.TitleName(FbgPlayer.Sex)
            });
        }
        else
        {
            return "ERROR: faulty sex input";
        }
    }

    private string PlayerOther()
    {
        try
        {
            PlayerOther po = GetPlayerOtherEnt();

            DataTable dt = Fbg.Bll.Stats.GetPlayerRanking(FbgPlayer.Realm);
            DataRow dr = dt.Rows.Find(po.PlayerID);
            string rank = dr == null ? dt.Rows.Count.ToString() : dr[Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerRank].ToString();
            //string xp = Utils.FormatCost(po.XP.XP);
            //string level = po.XP.Level.ToString();
            string GovName = "";

            if (FbgPlayer.Realm.OpenOn > new DateTime(2013, 07, 17))
            {
                GovType gt = po.GovernmentType;
                if (gt != null)
                {
                    GovName = gt.Name;
                }
            }

            return Json(new
            {
                success = true,
                @object = new
                {
                    id = po.PlayerID,
                    PN = po.PlayerName,
                    PP = Utils.FormatCost(po.Points),
                    rank = rank,
                    //xp = xp,
                    //level = level,
                    title = po.TitleName,
                    sex = po.Sex,
                    Av = po.Avatar.AvatarID,
                    CID = po.ClanID,
                    profile = BBCodes.PlayerProfileToHTML(FbgPlayer.Realm, Utils.ChangeLineBreaks(po.Profile), true),
                    Pe = Utils.ChangeLineBreaks(po.Note),
                    clan = new { CN = po.ClanName, id = po.ClanID },
                    gov = GovName
                }
            });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }

    private string PlayerOtherVillages()
    {
        try
        {
            PlayerOther po = GetPlayerOtherEnt();
            DataTable pvi = po.PlayerVillageInfo;

            return Json(new
            {
                success = true,
                @object = pvi.AsEnumerable().Select(
                    pvii => new {
                        name = pvii.Field<string>(Fbg.Bll.PlayerOther.CONSTS.PlayerVillageColumnIndex.VillageName),
                        x = pvii.Field<int>(Fbg.Bll.PlayerOther.CONSTS.PlayerVillageColumnIndex.XCord),
                        y = pvii.Field<int>(Fbg.Bll.PlayerOther.CONSTS.PlayerVillageColumnIndex.YCord),
                        vid = pvii.Field<int>(Fbg.Bll.PlayerOther.CONSTS.PlayerVillageColumnIndex.VillageID),
                        points = Utils.FormatCost(pvii.Field<int>(Fbg.Bll.PlayerOther.CONSTS.PlayerVillageColumnIndex.Points)),
                        opid = po.PlayerID
                    }
                )
            });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }

    private string PlayerProfileSet()
    {
        try
        {
            PlayerOther po = GetPlayerOtherEnt();

            string profile = BBCodes.PreProcessBBCodes(FbgPlayer.Realm, BBCodes.Medium.PlayerProfile, Request["text"]);
            profile = Utils.CleanupInputText(profile); //remove any html etc

            po.ProfileSave(profile, FbgPlayer.ID);

            return Json(new { success = true, @object = BBCodes.PlayerProfileToHTML(FbgPlayer.Realm, Utils.ChangeLineBreaks(profile), true) });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }

    private string PlayerOtherNoteClear()
    {
        try
        {
            PlayerOther po = GetPlayerOtherEnt();
            Fbg.Bll.PlayerOther.savePlayerNotes(po.PlayerID, FbgPlayer.ID, "", FbgPlayer.Realm.ConnectionStr);
            return Json(new { success = true });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }

    private string PlayerOtherNoteSave()
    {
        try
        {
            PlayerOther po = GetPlayerOtherEnt();
            string strippedNote = Utils.CleanupInputText(Request["text"]);
            Fbg.Bll.PlayerOther.savePlayerNotes(po.PlayerID, FbgPlayer.ID, strippedNote, FbgPlayer.Realm.ConnectionStr);
            return Json(new { success = true, @object = Utils.ChangeLineBreaks(strippedNote) });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }

    private string PlayerOtherInviteToClan()
    {
        try
        {
            PlayerOther po = GetPlayerOtherEnt();
            DateTime moreInvitesOn;

            Fbg.Bll.Clan.InviteResult result = Fbg.Bll.Clan.InvitePlayer(po.PlayerName, FbgPlayer, out moreInvitesOn);

            if (result != Fbg.Bll.Clan.InviteResult.Success)
            {
                return Json(new
                {
                    success = true,
                    @object = new
                    {
                        alert = true,
                        message = String.Format(Fbg.Bll.Clan.GetMessageFromCode(result),
                                                Utils.FormatEventTime(moreInvitesOn))
                    }
                });
            }
            else
            {
                // send a notification to invited player under certain conditions
                Utils.SendInvitedToClanNotification(po, FbgPlayer);

                return Json(new { success = true, @object = new { message = "Invitation sent successfully", code = 1 } });
            }
        }
        catch (Exception exc)
        {
            //return Json(new { success = false, @object = exc.Message });
            return Json(new { success = true, @object = new { message = "Error: Invalid name entry!", code = 0 } });
        }
    }

    public class PromotionResult
    {
        public int[] absorbed { get; set; }
        public int[] normal { get; set; }
        public int[] promoted { get; set; }
    }

    private PromotionResult VillagePromotingRepack(IEnumerable<Player.VillageReduction> vr, bool isNormal)
    {
        var pr = new PromotionResult();

        pr.absorbed = vr.Where(v => v.Status == Fbg.Bll.Player.VillageReduction.PromotionStatus.absorbed)
                         .Select(v => v.VillageID).ToArray();

        if (isNormal)
        {
            pr.normal = vr.Where(v => v.Status == Fbg.Bll.Player.VillageReduction.PromotionStatus.normal)
                          .Select(v => v.VillageID).ToArray();
        }


        pr.promoted = vr.Where(v => v.Status == Fbg.Bll.Player.VillageReduction.PromotionStatus.promoted)
                        .Select(v => v.VillageID).ToArray();

        return pr;
    }

    private string VillagePromoteAdd()
    {
        try
        {
            FbgPlayer.VillageReduction_AddPromoted(Convert.ToInt32(Request["vid"]));

            return Json(new { success = true, @object = VillagePromotingRepack(FbgPlayer.VillageReduction_Villages(), false) });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }

    private string VillagePromoteRemove()
    {
        try
        {
            FbgPlayer.VillageReduction_RemovePromoted(Convert.ToInt32(Request["vid"]));

            return Json(new { success = true, @object = VillagePromotingRepack(FbgPlayer.VillageReduction_Villages(), false) });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }

    private string VillagePromoteList()
    {
        try
        {
            bool isNormal = Convert.ToBoolean(Request["normal"]);

            return Json(new { success = true, @object = VillagePromotingRepack(FbgPlayer.VillageReduction_Villages(), isNormal) });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }


    private string VillageOtherSaveNote()
    {
        try
        {
            string note = BBCodes.PreProcessBBCodes(FbgPlayer.Realm, BBCodes.Medium.Note, Request["note"]);
            note = Utils.CleanupInputText(note);

            Fbg.Bll.VillageOther.saveVillageNotes(Convert.ToInt32(Request["vid"]), FbgPlayer.ID, note, FbgPlayer.Realm.ConnectionStr);

            return Json(new { success = true, @object = BBCodes.NoteToHTML(FbgPlayer.Realm, Utils.ChangeLineBreaks(Request["note"]), true) });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, @object = exc.Message });
        }
    }

    private string Json(object obj)
    {
        return json_serializer.Serialize(obj);
    }

    protected string PlayerSearch()
    {
        System.Data.DataTable dt = Fbg.Bll.Stats.GetPlayerRanking(FbgPlayer.Realm, 0, string.Empty);

        var term = Request["term"];

        var players = from r in dt.AsEnumerable()
                      where r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName).StartsWith(term, StringComparison.CurrentCultureIgnoreCase)
                      orderby r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName)
                      select new { value = r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName).ToString() };

        return Json(new { success = true, @object = players });
    }


    protected string ResearchState()
    {
        object o;

        json_serializer.RegisterConverters(new JavaScriptConverter[] {
            new Fbg.Bll.Api.ApiHelper.Converter()});

     
        FbgPlayer.MyResearch_ForceRefresh();

        return json_serializer.Serialize(new
        {
            success = true,
            @object = new
            {
                inProgress = FbgPlayer.MyResearch.ResearchInProgress,
                completedResearch = from s in FbgPlayer.MyResearch.ResearchItems select s.ID,
                hoursBehind = FbgPlayer.MyResearch.HoursBehind()
            }
        });
    }

    public string IncomingTroops()
    {
        DataSet ds = Fbg.Bll.UnitMovements.GetIncomingTroops2(FbgPlayer.Realm
            , FbgPlayer);

        try
        {
            var players =
                ds.Tables[Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.TablesIdx.players].AsEnumerable().ToDictionary(
                r => r.Field<Int32>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.PlayersColIdx.PID).ToString()
                , r => new
                {
                    name = r.Field<string>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.PlayersColIdx.PNAME)
                }
                );

            var villages =
                ds.Tables[Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.TablesIdx.villages].AsEnumerable().ToDictionary(
                r => r.Field<Int32>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.VillagesColIdx.VID).ToString()
                , r => new
                {
                    name = r.Field<string>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.VillagesColIdx.VName)
                    ,
                    x = r.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.VillagesColIdx.X)
                    ,
                    y = r.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.VillagesColIdx.Y)
                }
                );

            var query =
                from dr in ds.Tables[Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.TablesIdx.commands].AsEnumerable()
                select
                new
                {
                    type = dr.Field<Int16>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.CommandType)
                    ,
                    dvid = dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.DestinationVillageID)
                    ,
                    dpid = FbgPlayer.ID
                    ,
                    opid = dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.OriginPlayerID)
                    ,
                    ovid = dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.OriginVillageID)
                    ,
                    eid = dr.Field<Int64>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.EventID)
                    ,
                    etime = SerializeDate(dr.Field<DateTime>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.EventTime))
                    ,
                    visibleToTarget = dr.Field<Int16>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.VisibleToTarget)
                    ,
                    hidden = dr.Field<Int16>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.Hidden),

                    starttime =
                        dr.Field<long>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.Duration) == 0 ? 0 :
                            (SerializeDate(dr.Field<DateTime>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.EventTime).Subtract(
                                new TimeSpan(dr.Field<long>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.Duration))))
                            )
                    ,

                    // need to handle origin village ID of -999 (unknown)
                    speed = dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.OriginVillageID) == -999 ? 0 :
                        _getCmdSpeed(
                        Village.CalculateDistance(
                            villages[dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.OriginVillageID).ToString()].x,
                            villages[dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.OriginVillageID).ToString()].y,
                            villages[dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.DestinationVillageID).ToString()].x,
                            villages[dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.DestinationVillageID).ToString()].y)
                    , dr.Field<long>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.Duration))

                };

            DateTime timeStamp = ds.Tables[Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.TablesIdx.cacheTimeStamp].Rows.Count > 0 ?
                (DateTime)ds.Tables[Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.TablesIdx.cacheTimeStamp].Rows[0][0]
                : DateTime.Now;

            var outgoing = new { players = players, villages = villages, commands = query, cacheTimeStamp = SerializeDate(timeStamp) };

            return RETURN_SUCCESS(outgoing);

        }
        catch (Exception e)
        {
            BaseApplicationException bex = new BaseApplicationException("error in IncomingTroops", e);
            bex.AddAdditionalInformation("FbgPlayer.Realm.ID", FbgPlayer.Realm.ID);
            bex.AddAdditionalInformation("FbgPlayer.Realm.ID", ds);
            throw bex;
        }
    }


    public string OutgoingTroops()
    {
        DataSet ds = Fbg.Bll.UnitMovements.GetOutgoingTroops2(FbgPlayer.Realm, FbgPlayer);

        var players =
            ds.Tables[Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.TablesIdx.players].AsEnumerable().ToDictionary(
            r => r.Field<Int32>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.PlayersColIdx.PID).ToString()
            , r => new
            {
                name = r.Field<string>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.PlayersColIdx.PNAME)
            }
            );

        var villages =
            ds.Tables[Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.TablesIdx.villages].AsEnumerable().ToDictionary(
            r => r.Field<Int32>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.VillagesColIdx.VID).ToString()
            , r => new
            {
                name = r.Field<string>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.VillagesColIdx.VName)
                ,
                x = r.Field<int>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.VillagesColIdx.X)
                ,
                y = r.Field<int>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.VillagesColIdx.Y)
            }
            );

        var query =
            from dr in ds.Tables[Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.TablesIdx.commands].AsEnumerable()
            select
            new
            {
                type = dr.Field<Int16>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.CommandType)
                ,
                dvid = dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.DestinationVillageID)
                ,
                dpid = dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.DestinationPlayerID)
                ,
                opid = FbgPlayer.ID,
                ovid = dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.OriginVillageID)
                ,
                eid = dr.Field<Int64>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.EventID)
                ,
                etime = SerializeDate(dr.Field<DateTime>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.EventTime))
                ,
                visibleToTarget = dr.Field<Int16>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.VisibleToTarget)
                ,
                hidden = dr.Field<Int16>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.Hidden)
                ,
                starttime = SerializeDate(dr.Field<DateTime>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.EventTime).Subtract(
                new TimeSpan(dr.Field<long>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.Duration))))
                ,
                speed = _getCmdSpeed(
                    Village.CalculateDistance(
                        villages[dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.OriginVillageID).ToString()].x,
                        villages[dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.OriginVillageID).ToString()].y,
                        villages[dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.DestinationVillageID).ToString()].x,
                        villages[dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.DestinationVillageID).ToString()].y)
                , dr.Field<long>(Fbg.Bll.UnitMovements.CONSTS.IncomingTroops2.CommandsColIdx.Duration))
                ,
                isUnderBloodLust = dr.Field<int>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.isUnderBloodLust)
                ,
                morale = Convert.ToInt32(dr.Field<string>(Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.CommandsColIdx.Morale))
            };

        DateTime timeStamp = ds.Tables[Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.TablesIdx.cacheTimeStamp].Rows.Count > 0 ?
           (DateTime)ds.Tables[Fbg.Bll.UnitMovements.CONSTS.OutgoingTroops2.TablesIdx.cacheTimeStamp].Rows[0][0]
           : DateTime.Now;

        var incoming = new { players = players, villages = villages, commands = query, cacheTimeStamp = SerializeDate(timeStamp) };

        return RETURN_SUCCESS(incoming);
    }

    /// <summary>
    /// given distance(map squares) and duration(ticks) outputs speed(squares per hour)
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private double _getCmdSpeed(double distance, long duration)
    {
        if (duration == 0) { return 0; }
        TimeSpan t = new TimeSpan(duration);
        return Math.Round(distance / t.TotalHours, 1);
    }



    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="villageID"></param>
    /// <returns></returns>
    protected string Items_GetMyItems(int villageID)
    {
        Fbg.Bll.Api.Items.MyItemsInfo i = Fbg.Bll.Api.Items.GetMyItems(FbgPlayer, villageID);

        return RETURN_SUCCESS(new
        {
            myGiftInfo = i

        });
    }

    protected string GetCreditPackages(int deviceType)
    {
        if (Config.SaleType == 0)
        {
            List<Fbg.Bll.CreditPackageDevice> orders = Fbg.Bll.Realms.CreditPackagesDevice(Config.SaleType, deviceType);
            return RETURN_SUCCESS(new
            {
                credits = FbgPlayer.User.Credits,
                CreditPackages = orders,
                st = Config.SaleType
            });
        }
        else
        {
            //
            // NOTE!! The client code assiumes that the orders and orders_reg has the same list of packages, in the same order, just prices differ etc
            //
            List<Fbg.Bll.CreditPackageDevice> orders = Fbg.Bll.Realms.CreditPackagesDevice(Config.SaleType, deviceType);
            List<Fbg.Bll.CreditPackageDevice> orders_reg = Fbg.Bll.Realms.CreditPackagesDevice(0, deviceType);
            return RETURN_SUCCESS(new
            {
                credits = FbgPlayer.User.Credits,
                CreditPackages = orders,
                CreditPackagesRegular = orders_reg,
                st = Config.SaleType
            });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="giftID">ID of the gift type</param>
    /// <param name="expectedCost">expected cost in credits for this amount of gifts - we compare this to actual and only make the transaction if it works</param>
    /// <param name="amountToBuy">amount of the gifts to *GET* - this is a total number to get - thish may be some gifts to be used, so to buy</param>
    /// <param name="vid">Village id of the currnt village</param>
    /// <returns>
    /// resultCode -> one of the values of Fbg.Bll.Api.Items.BuyResult enum 
    /// 
    ///     OK -- all completed. see resultDetails for how man existing items were used, and how many were bought etc
    ///     
    ///     FAIL_DailyLimitReached -- the request amountToBuy of items would put the player over their current 
    ///         daily max - no items were bought or used - resultDetails shoudl be ignored
    ///     
    ///     FAIL_ExpectedCostLowerThanActualCost -- this can happen in case a player now has fewer gifts that the 
    ///         caller expected thus more have to be bought so we abort - resultDetails shoudl be ignored
    ///     
    ///     FAIL_NoneUsedDueToTresOrFarmLimitation -- either tresury cannot hold more silver or farm land cannot 
    ///         support more troops so NO items were used/bought - resultDetails shoudl be ignored
    ///     
    ///     PARTIAL_UsedSomeButNotAllDueToTresOrFarmLimitation -- simillar to FAIL_NoneUsedDueToTresOrFarmLimitation in that not all gifts could be bought
    ///         because either tresury cannot hold more silver or farm land cannot support more troops however, SOME of the gifts were used / bought. 
    ///         Examine resultDetails to see how many were actually bought or used
    ///     
    ///     FAIL_NoneUsedDueToCreditsMissing -- not enough credits to buy even one gift so NO items were used/bought - resultDetails shoudl be ignored
    ///     
    ///     PARTIAL_UsedSomeButNotAllDueToCreditsMissing -- simillar to FAIL_NoneUsedDueToCreditsMissing in that not all gifts could be bought
    ///         because not enough credits however, SOME of the gifts were used / bought. 
    ///         Examine resultDetails to see how many were actually bought and/or used
    ///     
    /// 
    /// resultDetails -> object that give you details about what actually happend. This is only valid depending on the value of resultCode.
    /// 
    /// myGiftInfo --> same as calling "Items_GetMyItems" after all the gifts were bought / used
    /// 
    /// </returns>
    protected string Items_Buy(int giftID, int vid, int amountToBuy, int expectedCost)
    {
        Fbg.Bll.Api.Items.BuyResult result = Fbg.Bll.Api.Items.Buy(FbgPlayer, giftID, vid, amountToBuy, expectedCost);

        Fbg.Bll.Api.Items.MyItemsInfo info = Fbg.Bll.Api.Items.GetMyItems(FbgPlayer, vid);

        return RETURN_SUCCESS(new
        {
            resultCode = (int)result.resultCode,
            resultDetails = result.details,
            myGiftInfo = info,
            credits = FbgPlayer.User.Credits
        });
    }


    protected string MaxTransferCredits()
    {
        int max = FbgUser.TransferableCredits;
        return ApiHelper.RETURN_SUCCESS(new { allowed = max });
    }

    protected string TransferCredits()
    {
        try
        {



            PlayerOther playerToTransferTo = null;
            string playerName = "";
            int amount = 0;
            int maxServantsToTransfer = FbgUser.TransferableCredits;


            // Is player logged in as steward?
            if (IsLoggedInAsSteward)
            {
                return ApiHelper.RETURN_SUCCESS(new { resultcode = 1, max = maxServantsToTransfer });
            }

            // Is amount an integer?
            if (!Int32.TryParse(Request.QueryString["amount"], out amount))
            {
                return ApiHelper.RETURN_SUCCESS(new { resultcode = 2, max = maxServantsToTransfer });
            }


            // Validate amount
            if (amount < 5 || amount > maxServantsToTransfer)
            {
                return ApiHelper.RETURN_SUCCESS(new { resultcode = 3, max = maxServantsToTransfer });
            }

            // Is player id an integer?
            playerName = Request.QueryString["pname"].Trim();
            if (playerName == "")
            {
                return ApiHelper.RETURN_SUCCESS(new { resultcode = 4, max = maxServantsToTransfer });
            }

            // Got through a whole bunch of checks.
            // now get the player to transfer to...
            playerToTransferTo = Fbg.Bll.PlayerOther.GetPlayer(FbgPlayer.Realm, playerName, FbgPlayer.ID);
            // Is player id the same as the player sending?
            if (playerToTransferTo != null && playerToTransferTo.PlayerID == FbgPlayer.ID)
            {
                return ApiHelper.RETURN_SUCCESS(new { resultcode = 5, max = maxServantsToTransfer });
            }

            // Finally, if the player isn't null and we got this far, lets try and do
            // the transfer.
            if (playerToTransferTo != null)
            {
                bool success = FbgUser.TransferCredits(amount, playerToTransferTo.PlayerID);
                if (success)
                {
                    // Update the maximum allowed so the player knows.
                    maxServantsToTransfer = FbgUser.TransferableCredits;

                    return RETURN_SUCCESS(new
                    {
                        resultcode = 0,
                        max = maxServantsToTransfer
                    });
                }
                else
                {
                    // at this moment, transfer never fails like this so exception is sufficient. 
                    throw new Exception("TransferCredits returned false");
                }
            }
            else
            {
                return ApiHelper.RETURN_SUCCESS(new { resultcode = 6, max = maxServantsToTransfer });
            }

        }
        catch (Exception exc)
        {
            //return ApiHelper.RETURN_SUCCESS(new { resultcode = 7, errmsg = exc.Message });
            // May be risky to pass the actual error message.
            return ApiHelper.RETURN_SUCCESS(new { resultcode = 7, max = 0 });
        }


    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sel">1-9</param>
    /// <returns>if null then the player has already received reward or this is nonavailable</returns>
    protected string GetMyDailyReward(int sel)
    {
        InvalidateFbgUser(); // <----------- DOING THIS TO prevent double bonus - want flags to be refreshed
        InvalidateFbgPlayer(); // <---------
        bool playerUpdated = false;

        Fbg.Bll.Player.DailyRewardStatus DailyRewardStatus = FbgPlayer.GetDailyRewardStatus();
        int dailyRewardLevel = DailyRewardStatus.level;
        bool dailyRewardAvail = DailyRewardStatus.available;
        DateTime dailyRewardNext = DailyRewardStatus.nextTime;

        List<DailyReward.DailyRewardItem> rewards = null;
        List<DailyReward.DailyRewardItem> nextDayRewards = null;
        string playerGot = null;
        if (dailyRewardAvail)
        {
            DailyReward dailyReward = new DailyReward(FbgPlayer, dailyRewardLevel);
            rewards = dailyReward.GetShuffledRewards();
            playerUpdated = rewards[sel - 1].UpdatePlayer();
            playerGot = string.Format("{0}", rewards[sel - 1].GetLongDesc());
            DailyReward nextDailyReward = new DailyReward(FbgPlayer, dailyRewardLevel + 1);
            nextDayRewards = nextDailyReward.GetRewards();
        }
        return RETURN_SUCCESS(new
        {
            Sel = sel,
            PlayerGot = playerGot,
            DailyRewardLevel = dailyRewardLevel,
            PlayerUpdated = playerUpdated,
            Rewards = rewards,
            NextDayRewards = nextDayRewards,
        });
    }
    /// <summary>
    /// constants used for GetClanPublicProfile
    /// </summary>
    internal class DSCONSTS
    {

        public class ClanPublicProfileTableIndex
        {
            public static int Points = 0;
            public static int Members = 1;
            public static int Clan = 2;
        }
    }
    /// <summary>
    /// returns data for Clan Public Profile
    /// </summary>
    /// <param name="cid">the clan id</param>
    /// <returns>some JSON</returns>
    protected string GetClanPublicProfile(int clanId)
    {
        string name = null,
            publicMessageHtml = "",
            publicMessageBB = "",
            getClanSettings = "";
        int points = 0,
            rank = 0,
            numOfMembers = 0;
        bool playerInClan = false,
            playerIsOnwer = false,
            playerIsAdmin = false,
            playerIsForumAdmin = false,
            playerIsInviter = false,
            playerIsDiplomat = false;
        DataSet ds = Fbg.Bll.Clan.ViewClanPublicProfile(FbgPlayer, clanId);
        DataTable dt = Fbg.Bll.Stats.GetClanRanking(FbgPlayer.Realm, 0);
        if (ds != null)
        {
            if (ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Points].Rows.Count > 0)
            {
                points = (int)ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Points].Rows[0]["villagepoints"];
            }
            if (ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Members].Rows.Count > 0)
            {
                numOfMembers = (int)ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Members].Rows[0]["PlayersCount"];
            }
            if (ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows.Count > 0)
            {
                name = ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows[0]["Name"].ToString();
            }
            if (dt.Rows.Count > 0)
            {
                int ClanIndex = dt.Rows.IndexOf(dt.Rows.Find(clanId));
                rank = ClanIndex < 0 ? dt.Rows.Count : ClanIndex + 1;
            }
            if (ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows.Count > 0)
            {
                publicMessageHtml = BBCodes.ClanPublicProfileToHTML(FbgPlayer.Realm, ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows[0]["PublicProfile"].ToString());
                publicMessageBB = BBCodes.CleanUpPreProcessedBBCodes(FbgPlayer.Realm, ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows[0]["PublicProfile"].ToString());
            }
            if (FbgPlayer.Clan != null)
            {
                if (FbgPlayer.Clan.ID == clanId)
                {
                    playerInClan = true;
                    if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator))
                    {
                        playerIsAdmin = true;
                    }
                    if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner))
                    {
                        playerIsOnwer = true;
                    }
                    if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Inviter))
                    {
                        playerIsInviter = true;
                    }
                    if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.ForumAdministrator))
                    {
                        playerIsForumAdmin = true;
                    }
                    if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Diplomat))
                    {
                        playerIsDiplomat = true;
                    }
                }

            }
        }
        return RETURN_SUCCESS(new
        {
            ID = clanId,
            Name = name,
            Points = points,
            Rank = rank,
            NumOfMembers = numOfMembers,
            PublicMessageHtml = publicMessageHtml,
            PublicMessageBB = publicMessageBB,
            PlayerInClan = playerInClan,
            PlayerIsOnwer = playerIsOnwer,
            PlayerIsAdmin = playerIsAdmin,
            PlayerIsForumAdmin = playerIsForumAdmin,
            PlayerIsInviter = playerIsInviter,
            PlayerIsDiplomat = playerIsDiplomat,
            ClanSettings = getClanSettings
        });
    }
    public class ClanMember
    {
        public int VillagePoints { get; set; }
        public int PlayerID { get; set; }
        public string Name { get; set; }
        public int VillagesCount { get; set; }
        public string LastLoginTime { get; set; }
        public int StewardPlayerID { get; set; }
        public string StewardPlayerName { get; set; }
        public bool IsPlayerInSleepMode { get; set; }
        public ClanMember(DataRow row, Fbg.Bll.Player p)
        {
            VillagePoints = (int)row["villagepoints"];
            PlayerID = (int)row["PlayerID"];
            Name = (string)row["Name"];
            VillagesCount = (int)row["VillagesCount"];
            LastLoginTime = ((DateTime)row["LastLoginTime"]).ToString();
            if (row["SleepModeActiveFrom"] != System.DBNull.Value)
            {
                IsPlayerInSleepMode = p.Realm.SleepModeGet.IsPlayerInSleepMode((DateTime)row["SleepModeActiveFrom"]);
            }
            if (row["StewardPlayerID"] != System.DBNull.Value)
            {
                StewardPlayerID = (int)row["StewardPlayerID"];
                StewardPlayerName = (string)row["StewardPlayerName"];
            }
        }
    }
    public class ClanMemberRole
    {
        public int PlayerID { get; set; }
        public int ClanID { get; set; }
        public int RoleID { get; set; }
        public ClanMemberRole(DataRow row)
        {
            PlayerID = (int)row["PlayerID"];
            ClanID = (int)row["ClanID"];
            RoleID = (int)row["RoleID"];
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="clanID"></param>
    /// <returns></returns>
    protected string GetClanMemberList(int clanID)
    {
        double SleepDuration = 0;

        if (FbgPlayer.Realm.SleepModeGet.IsAvailableOnThisRealm)
        {
            SleepDuration = FbgPlayer.Realm.SleepModeGet.Duration;
        }

        DataSet ds = Fbg.Bll.Clan.ViewClanMembers(FbgPlayer, clanID, true);
        List<ClanMember> members = new List<ClanMember>();
        List<ClanMemberRole> roles = new List<ClanMemberRole>();
        if (ds != null)
        {
            DataTable dt0 = ds.Tables[0];
            foreach (DataRow row in dt0.Rows)
            {
                members.Add(new ClanMember(row, FbgPlayer));
            }
            DataTable dt1 = ds.Tables[1];
            foreach (DataRow row in dt1.Rows)
            {
                roles.Add(new ClanMemberRole(row));
            }
        }
        return RETURN_SUCCESS(new
        {
            Members = members,
            Roles = roles,
            hdServerTime = SerializeDate(DateTime.Now.ToUniversalTime()).ToString(),
            sleepDuration = SleepDuration
        });
    }
    public class ClanDiplomacy
    {
        public string Name { get; set; }
        public int ClanID { get; set; }
        public int OtherClanID { get; set; }
        public int StatusID { get; set; }
        public ClanDiplomacy(DataRow row)
        {
            Name = (string)row["Name"];
            ClanID = (int)row["ClanID"];
            OtherClanID = (int)row["OtherClanID"];
            StatusID = (int)row["StatusID"];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clanID"></param>
    /// <returns></returns>
    protected string GetClanDiplomacy(int clanID)
    {
        List<ClanDiplomacy> allies = new List<ClanDiplomacy>();
        List<ClanDiplomacy> enemies = new List<ClanDiplomacy>();
        List<ClanDiplomacy> nap = new List<ClanDiplomacy>();
        if (FbgPlayer.Clan != null)
        {
            Fbg.Bll.ClanDiplomacy diplomacy = Fbg.Bll.Clan.ViewMyClanDiplomacy(FbgPlayer);
            DataTable alliesdt = diplomacy.GetAllies();
            foreach (DataRow row in alliesdt.Rows)
            {
                allies.Add(new ClanDiplomacy(row));
            }
            DataTable enemiesdt = diplomacy.GetEnemies();
            foreach (DataRow row in enemiesdt.Rows)
            {
                enemies.Add(new ClanDiplomacy(row));
            }
            DataTable napdt = diplomacy.GetNAP();
            foreach (DataRow row in napdt.Rows)
            {
                nap.Add(new ClanDiplomacy(row));
            }
        }
        return RETURN_SUCCESS(new
        {
            Allies = allies,
            Enemies = enemies,
            NAP = nap,
        });
    }

    protected string SetQuestAsCompleted(string questTag)
    {
        BDA.Achievements.Quests.QuestUpdateStatus status = FbgPlayer.Quests2.SetQuestAsCompletedWithResult(questTag);

        return RETURN_SUCCESS(new
        {
            Status = (int)status
        });
    }


    #region helpers
    public string RETURN_SUCCESS(object o)
    {
        return Fbg.Bll.Api.ApiHelper.RETURN_SUCCESS(o);

    }



    class NumberToRomanConvertor
    {
        // Converts an integer value into Roman numerals
        static public string NumberToRoman(int number)
        {
            // Validate
            if (number < 0 || number > 3999)
                throw new ArgumentException("Value must be in the range 0 - 3,999.");

            if (number == 0) return "N";

            // Set up key numerals and numeral pairs
            int[] values = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] numerals = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

            // Initialise the string builder
            StringBuilder result = new StringBuilder();

            // Loop through each of the values to diminish the number
            for (int i = 0; i < 13; i++)
            {
                // If the number being converted is less than the test value, append
                // the corresponding numeral or numeral pair to the resultant string
                while (number >= values[i])
                {
                    number -= values[i];
                    result.Append(numerals[i]);
                }
            }

            // Done
            return result.ToString();
        }
    }

    public static double SerializeDate(DateTime date)
    {
        return Fbg.Bll.Api.ApiHelper.SerializeDate(date);
    }


    public static double SerializeTimeSpan(TimeSpan timespan)
    {
        return Fbg.Bll.Api.ApiHelper.SerializeTimeSpan(timespan);
    }

    #endregion

    protected string GetClanInvites(int playerID)
    {
        DataSet ds = Fbg.Bll.Clan.ViewPlayerInvitations(FbgPlayer);

        List<ClanInviter> inviter = new List<ClanInviter>();

        if (ds != null)
        {
            DataTable dt0 = ds.Tables[0];
            foreach (DataRow row in dt0.Rows)
            {
                inviter.Add(new ClanInviter(row));
            }

        }

        return RETURN_SUCCESS(new
        {
            InviterList = inviter
        });


    }

    public class ClanInviter
    {

        public int ClanID { get; set; }
        public string Name { get; set; }
        public string InvitedOn { get; set; }
        public int PlayerID { get; set; }

        public ClanInviter(DataRow row)
        {
            ClanID = (int)row["ClanID"];
            Name = (string)row["Name"];
            PlayerID = (int)row["PlayerID"];
            //InvitedOn = ((DateTime)row["InvitedOn"]).ToString();
            InvitedOn = Math.Round((((DateTime)row["InvitedOn"]).AddDays(10) - DateTime.Now).TotalDays, 1).ToString();
        }
    }


    protected string GetClanPlayerInvites(int playerID)
    {

        // txt_PlayerName.Text = Utils.ClearHTMLCode(txt_PlayerName.Text);

        List<ClanInviter> inviter = new List<ClanInviter>();

        DataTable dt = Fbg.Bll.Clan.ViewClanInvitations(FbgPlayer, "");

        if (dt != null)
        {

            DataTable dt0 = dt;
            foreach (DataRow row in dt0.Rows)
            {
                inviter.Add(new ClanInviter(row));
            }

        }

        int invitesLeft;
        DateTime moreInvitesOn;

        FbgPlayer.Clan.GetInvitesLeft(out invitesLeft, out moreInvitesOn);

        return RETURN_SUCCESS(new
        {
            inviterList = inviter,
            invitesLeft = invitesLeft,
            moreInvitesOn = ((DateTime)moreInvitesOn).ToString()
        });
    }


    protected string CancelClanInvite(int ClanID, int playerID)
    {

        Fbg.Bll.Clan.CancelInvitation(FbgPlayer, ClanID);

        DataSet ds = Fbg.Bll.Clan.ViewPlayerInvitations(FbgPlayer);

        List<ClanInviter> inviter = new List<ClanInviter>();

        if (ds != null)
        {
            DataTable dt0 = ds.Tables[0];
            foreach (DataRow row in dt0.Rows)
            {
                inviter.Add(new ClanInviter(row));
            }

        }

        return RETURN_SUCCESS(new
        {
            InviterList = inviter
        });
    }


    protected string DoClanLeave(int playerID)
    {
        string lbl_Error = "";
        Fbg.Bll.Clan.LeaveClanResult result = Fbg.Bll.Clan.LeaveClan(FbgPlayer);
        if (result == Fbg.Bll.Clan.LeaveClanResult.Failed)
        {
            lbl_Error = "errorLeaving";
        }
        else if (result == Fbg.Bll.Clan.LeaveClanResult.Failed_NoClanChangesAllowedAnyLonger)
        {
            lbl_Error = "errorLeaving_changesNoLongerAllowed";
        }
        else
        {
            InvalidateFbgPlayerRoles();
            InvalidateFbgPlayerClan();
            lbl_Error = "success";
        }

        return RETURN_SUCCESS(new
        {
            playerID = playerID,
            result = lbl_Error
        });
    }


    protected string DoClanDissmiss(int playerID)
    {
        string lbl_Error = "";

        Fbg.Common.Clan.DissmissFromClanResult result = FbgPlayer.Clan.DismissPlayer(playerID);

        if (result == Fbg.Common.Clan.DissmissFromClanResult.TryingToDismissLastOwner)
        {
            lbl_Error = "onlyOwnerNoDismiss";
        }
        else if (result == Fbg.Common.Clan.DissmissFromClanResult.AdminTryingToDismissOwner)
        {
            lbl_Error = "adminCannotDismiss";
        }
        else
        {
            InvalidateFbgPlayerRoles();
            lbl_Error = "success";
        }

        return RETURN_SUCCESS(new
        {
            playerID = playerID,
            result = lbl_Error
        });
    }


    protected string DoClanJoin(int ClanID)
    {
        int lbl_Error;

        if (Fbg.Bll.Clan.LeaveClan(FbgPlayer) == Fbg.Bll.Clan.LeaveClanResult.OK)
        {
            Fbg.Bll.Clan.JoinResult result = Fbg.Bll.Clan.JoinClan(FbgPlayer, ClanID);
            if (result == Fbg.Bll.Clan.JoinResult.Success)
            {
                lbl_Error = 0; //"success";
                InvalidateFbgPlayerRoles();

                //updates the ChatUserEntityPlayer's clan so JoinOrCreate properly adds them to chat
                ChatHub2.ChatHub2.addClanToChatUser(FbgPlayer.ID.ToString(), FbgPlayer.Clan.ID, FbgPlayer.Realm.ID.ToString());
                ChatHub2.ChatHub2.JoinOrCreateClanChat(FbgPlayer.ID.ToString(), ClanID, FbgPlayer.Realm.ID.ToString()); //add player to clan
            }
            else
            {
                lbl_Error = (int)result;
            }
        }
        else
        {
            lbl_Error = 9;// "errorLeaving";
        }

        return RETURN_SUCCESS(new
        {
            ClanID = ClanID,
            result = lbl_Error
        });
    }


    protected string RevokeClanJoin(int playerID)
    {

        Fbg.Bll.Clan.DeleteInvitation(FbgPlayer, playerID);

        return RETURN_SUCCESS(new
        {
            playerID = playerID
        });

    }


    protected string RenameClan(string newClanName)
    {
        newClanName = Utils.ClearHTMLCode(newClanName);
        newClanName = Utils.ClearInvalidChars(newClanName);
        newClanName = Utils.StripNonAscii(newClanName);

        bool ret = Fbg.Bll.Clan.RenameClan(FbgPlayer, newClanName);

        return RETURN_SUCCESS(new
        {
            newName = newClanName
        });
    }


    protected string DeleteClan()
    {
        int lbl_Error = 0;
        if (FbgPlayer.Clan != null)
        {
            if (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner))
            {
                Fbg.Bll.Clan.DeleteClan(FbgPlayer);
                FbgPlayer.Clan = null;
                lbl_Error = 1;
            }
            else { lbl_Error = 2; }

        }

        return RETURN_SUCCESS(new
        {
            info = lbl_Error
        });
    }

    protected string CreateNewClan(string ClanName)
    {

        ClanName = Utils.ClearHTMLCode(ClanName);
        ClanName = Utils.ClearInvalidChars(ClanName);
        ClanName = Utils.StripNonAscii(ClanName);

        string txt_ClanDesc = "...";
        int newClandid = 0;

        if (ClanName.Trim() != "")
        {
            if (Fbg.Bll.Clan.CreateNewClan(ClanName, txt_ClanDesc, FbgPlayer) == null)
            {
                newClandid = 0;
            }
            else
            {
                InvalidateFbgPlayerRoles();
                newClandid = FbgPlayer.Clan.ID;

                //updates the ChatUserEntityPlayer's clan so JoinOrCreate properly adds them to chat
                ChatHub2.ChatHub2.addClanToChatUser(FbgPlayer.ID.ToString(), FbgPlayer.Clan.ID, FbgPlayer.Realm.ID.ToString());
                ChatHub2.ChatHub2.JoinOrCreateClanChat(FbgPlayer.ID.ToString(), newClandid, FbgPlayer.Realm.ID.ToString()); //add player to clan
            }
        }


        return RETURN_SUCCESS(new
        {
            newclandid = newClandid
        });
    }


    protected string SaveClanProfile(string PublicProfileText)
    {
        string profileText = Utils.ClearHTMLCode(PublicProfileText.Trim());
        profileText = Utils.ClearInvalidChars(profileText);
        profileText = BBCodes.PreProcessBBCodes(FbgPlayer.Realm, BBCodes.Medium.ClanPublicProfile, profileText);

        FbgPlayer.Clan.UpdateClanPublicProfile(profileText);

        return RETURN_SUCCESS(new
        {
            info = "done"
        });
    }


    protected string InsertClanDiplomacy(string txt_ClanName, int Diplomacy)
    {

        Fbg.Bll.Clan.DiplomacyResult result = Fbg.Bll.Clan.DiplomacyResult.Success;
        txt_ClanName = Utils.ClearHTMLCode(txt_ClanName);

        if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Diplomat))
        {
            switch (Diplomacy)
            {
                case 0:
                    result = Fbg.Bll.Clan.AddDiplomacy(FbgPlayer, txt_ClanName.Trim(), Fbg.Bll.Clan.Diplomacy.Ally);
                    break;
                case 1:
                    result = Fbg.Bll.Clan.AddDiplomacy(FbgPlayer, txt_ClanName.Trim(), Fbg.Bll.Clan.Diplomacy.Enemy);
                    break;
                case 2:
                    result = Fbg.Bll.Clan.AddDiplomacy(FbgPlayer, txt_ClanName.Trim(), Fbg.Bll.Clan.Diplomacy.NAP);
                    break;
            }

        }
        return RETURN_SUCCESS(new
        {
            clanName = txt_ClanName,
            rcode = (int)result
        });
    }


    protected string DeleteClanDiplomacy(string txt_ClanName, int DiplType)
    {

        if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Diplomat))
        {
            if (DiplType == 0) { Fbg.Bll.Clan.DeleteDiplomacy(FbgPlayer, txt_ClanName, Fbg.Bll.Clan.Diplomacy.Ally); }
            if (DiplType == 1) { Fbg.Bll.Clan.DeleteDiplomacy(FbgPlayer, txt_ClanName, Fbg.Bll.Clan.Diplomacy.Enemy); }
            if (DiplType == 2) { Fbg.Bll.Clan.DeleteDiplomacy(FbgPlayer, txt_ClanName, Fbg.Bll.Clan.Diplomacy.NAP); }
        }

        return RETURN_SUCCESS(new
        {
            info = "done"
        });
    }


    protected string GetClanSettings()
    {
        bool cbRole_Inviter;

        if (FbgPlayer.Clan.GetClanSettings().Tables[0].Rows.Count > 0)
        {
            cbRole_Inviter = true;
        }
        else
        {
            cbRole_Inviter = false;
        }

        return RETURN_SUCCESS(new
        {
            inviter = cbRole_Inviter
        });
    }


    protected string SaveClanSettings(bool inviterFlag)
    {
        FbgPlayer.Clan.UpdateClanSettings(inviterFlag);

        bool cbRole_Inviter;

        if (FbgPlayer.Clan.GetClanSettings().Tables[0].Rows.Count > 0)
        {
            cbRole_Inviter = true;
        }
        else
        {
            cbRole_Inviter = false;
        }
        return RETURN_SUCCESS(new
        {
            inviter = cbRole_Inviter
        });
    }




    /// <summary>
    /// 
    /// </summary>
    public class BonusVillageTypes : IEnumerable
    {
        public BonusVillageTypes(VillageTypes vts, int cost, int nextCost)
        {
            BonusTypes = new List<BonusVillageType>();
            foreach (VillageTypes.VillageType vt in vts)
            {
                if (vt.IsBonus)
                {
                    BonusTypes.Add(new BonusVillageType(vt, cost, nextCost));
                }
            }
        }
        public BonusVillageType this[int villageTypeID]
        {
            get
            {
                return BonusTypes.Find(delegate (BonusVillageType vt) { return vt.ID == villageTypeID; });
            }
        }
        public List<BonusVillageType> BonusTypes
        {
            get;
            private set;
        }
        public IEnumerator GetEnumerator()
        {
            return BonusTypes.GetEnumerator();
        }
        /// <summary>
        /// This is TEMPORARY throw-away code ... will probably be replaced by DB eventually
        /// </summary>
        public class BonusVillageType
        {
            VillageTypes.VillageType _vt;
            public BonusVillageType(VillageTypes.VillageType vt, int cost, int nextCost)
            {
                _vt = vt;
                Cost = cost;
                NextCost = nextCost;
            }
            public short ID
            {
                get
                {
                    return _vt.ID;
                }
            }
            public string Name
            {
                get
                {
                    return _vt.Name;
                }
            }
            public int Cost
            {
                get;
                private set;
            }
            public int NextCost
            {
                get;
                private set;
            }
            public string ImgUrl
            {
                get
                {
                    string imgurl = "";
                    switch (ID)
                    {
                        // 50% more Silver Production
                        case 1:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Silver.png"; //
                            break;
                        // 50% faster recruitment at the Barracks
                        case 2:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Barracks.png"; //
                            break;
                        // 20% faster recruitment at all buildings
                        case 3:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Recruit.png"; //
                            break;
                        // 40% higher farm yield
                        case 4:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Farm.png";
                            break;
                        // 40% Wall and Tower defence bonus
                        case 5:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Wall.png";
                            break;
                        // 50% faster recruitment at the Stables
                        case 6:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Stables.png"; //
                            break;
                        // 50% faster recruitment at the Tavern
                        case 7:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Tavern.png"; //
                            break;
                        case 8:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Barracks.png"; //
                            break;
                        case 9:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Tavern.png"; //
                            break;
                        case 10:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Recruit.png"; //
                            break;
                        case 11:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Silver.png"; //
                            break;
                        case 12:
                            imgurl = "https://static.realmofempires.com/images/illustrations/BonusV_Stables.png";//
                            break;
                    }
                    return imgurl;
                }
            }
        }
    }
    // cost per each successive bonus vill change
    const int BONUSTYPECONVERSION = 20;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vid"></param>
    /// <returns></returns>
    protected string GetBonusTypes(string vid)
    {
        int numBonusVillChange = 0;
        bool bonusChange = FbgPlayer.Realm.BonusVillChange;
        int currentBonusType = FbgPlayer.Village(Convert.ToInt32(vid)).VillageType.ID;
        InvalidateFbgUser();
        int currentServants = FbgUser.Credits;
        BonusVillageTypes bonusVillageTypes = null;
        try
        {
            if (bonusChange)
            {
                if (FbgPlayer.HasFlag(Player.Flags.Misc_NumBonusVillChange, false) != null)
                {
                    numBonusVillChange = Convert.ToInt32((string)FbgPlayer.HasFlag_GetData(Player.Flags.Misc_NumBonusVillChange));
                }
                // 100 servants max
                int cost = Math.Min(numBonusVillChange * BONUSTYPECONVERSION, 100);
                int nextCost = Math.Min((numBonusVillChange + 1) * BONUSTYPECONVERSION, 100);
                bonusVillageTypes = new BonusVillageTypes(FbgPlayer.Realm.VillageTypes, cost, nextCost);
            }
        }
        catch
        {
            bonusChange = false;
            bonusVillageTypes = null;
        }
        return RETURN_SUCCESS(new
        {
            changes = numBonusVillChange,
            BonusChange = bonusChange,
            CurrentBonusType = currentBonusType,
            CurrentServants = currentServants,
            BonusTypes = bonusVillageTypes == null ? null :
                (FbgPlayer.Realm.ID <= 94 ? bonusVillageTypes.BonusTypes : bonusVillageTypes.BonusTypes.FindAll(s => { return s.ID <= 7; })),
        });
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="villageID"></param>
    /// <param name="villageTypeID"></param>
    /// <returns></returns>
    protected string SetBonusType(string villageID, string villageTypeID)
    {
        bool bonusChange = FbgPlayer.Realm.BonusVillChange;
        bool changed = false;
        bool notEnoughServants = false;
        int vid = Convert.ToInt32(villageID);
        int vt = Convert.ToInt32(villageTypeID);
        try
        {

            InvalidateFbgUser();
            if (bonusChange)
            {
                int numBonusVillChange = 0;
                if (FbgPlayer.HasFlag(Player.Flags.Misc_NumBonusVillChange, false) != null)
                {
                    numBonusVillChange = Convert.ToInt32((string)FbgPlayer.HasFlag_GetData(Player.Flags.Misc_NumBonusVillChange));
                }
                // 100 servants max
                int cost = Math.Min(numBonusVillChange * BONUSTYPECONVERSION, 100);
                if (cost > FbgUser.Credits)
                {
                    notEnoughServants = true;
                }
                else
                {
                    Village village = Village.GetVillage(FbgPlayer, vid, false, false);
                    if (SetBonusType_IsConversionBetweenTheseTypesAllowed(village.VillageType.ID, vt, FbgPlayer.Realm.ID))
                    {
                        if (village.SetType(vt, cost))
                        {
                            FbgUser.UseCredits(cost, 27, FbgPlayer.Realm.ID);
                            FbgPlayer.SetFlag(Player.Flags.Misc_NumBonusVillChange, (numBonusVillChange + 1).ToString());
                            InvalidateFbgUser();
                            changed = true;
                        }
                    }
                }
            }
        }
        catch (Exception x)
        {
            throw new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("error in SetBonusType", x);
        }
        return RETURN_SUCCESS(new
        {
            BonusChange = bonusChange,
            NotEnoughServants = notEnoughServants,
            CurrentServants = FbgUser.Credits,
            Changed = changed,
        });
    }

    /// <summary>
    /// HACK : do not allow changing LEGENDARY BONUS TYPES introduced in r94
    /// </summary>
    /// <param name="fromVTID"></param>
    /// <param name="toVTID"></param>
    /// <returns></returns>
    private bool SetBonusType_IsConversionBetweenTheseTypesAllowed(int fromVTID, int toVTID, int realmID)
    {
        if ((realmID <= 95 && realmID >= 1)
            || realmID == 13
            || realmID == 21
            || realmID == 0)
        {
            return true;
        }
        else
        {
            return (fromVTID <= 7 && toVTID <= 7);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string Restart()
    {
        bool notEnoughServants = false;
        bool noRespawns = false;
        bool restarted = false;
        string redirect = null;
        try
        {
            InvalidateFbgUser();
            Fbg.Bll.Player.RestartOnRealmReturnVal ret = FbgPlayer.RestartOnRealm_abandon();
            if (ret == Player.RestartOnRealmReturnVal.failed_noRespawnsOnRealm)
            {
                noRespawns = true;
            }
            else if (ret == Player.RestartOnRealmReturnVal.failed_notEnoughServangs)
            {
                notEnoughServants = true;
            }
            else if (ret == Player.RestartOnRealmReturnVal.success)
            {
                InvalidateFbgUser();
                restarted = true;
                MyVillagesInvalidate();
                redirect = string.Format("LoginToRealm.aspx?pid={0}&rid={1}", FbgPlayer.ID, FbgPlayer.Realm.ID);
            }
        }
        catch (Exception x)
        {
            throw new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("error in Restart", x);
        }
        return RETURN_SUCCESS(new
        {
            Credits = FbgUser.Credits,
            NotEnoughServants = notEnoughServants,
            noRespawns = noRespawns,
            Restarted = restarted,
            Redirect = redirect,
        });
    }


    protected string SetLocalServerStorage(string data)
    {
        FbgUser.SetFlag(Fbg.Bll.User.Flags.LocalStorageOnServer, data);
        return ApiHelper.RETURN_SUCCESS(true);
    }


    protected string ActivateSleepMode()
    {
        bool activated = FbgPlayer.SleepMode_Activate();

        return RETURN_SUCCESS(new
        {
            activate = activated
        });
    }

    protected string ActivateVacationMode(int duration)
    {
        bool activated = FbgPlayer.VacationMode_Activate();
        Fbg.Bll.Player.VacationModeStatus status = FbgPlayer.GetVacationModeStatus();
        return RETURN_SUCCESS(new
        {
            status = status,
            result = activated
        });
    }

    protected string CancelVacationMode()
    {
        bool result = FbgPlayer.VacationMode_Cancel();
        Fbg.Bll.Player.VacationModeStatus status = FbgPlayer.GetVacationModeStatus();
        return RETURN_SUCCESS(new
        {
            status = status,
            result = result
        });
    }

    protected string WeekendModeRequest(string desiredActivationDateQuery)
    {

        long desiredActivationDateRaw = Convert.ToInt64(desiredActivationDateQuery);

        //convert UTC activation time to Server Time
        DateTime desiredActivationDateConverted = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(desiredActivationDateRaw).ToLocalTime();

        bool weekendModeRequestGranted = FbgPlayer.WeekendMode_Request(desiredActivationDateConverted);

        Fbg.Bll.Player.WeekendModeStatus newWeekendModeStatus = FbgPlayer.GetWeekendModeStatus();

        return RETURN_SUCCESS(new
        {
            desiredActivationDateQuery = desiredActivationDateQuery,
            desiredActivationDateConverted = desiredActivationDateConverted.ToShortDateString() + " " + desiredActivationDateConverted.ToShortTimeString(),
            status = newWeekendModeStatus,
            result = weekendModeRequestGranted
        });
    }

    /* not gonna be called through API yet
    protected string WeekendModeCancel()
    {
    
        bool result = FbgPlayer.VacationMode_Cancel();
        Fbg.Bll.Player.VacationModeStatus status = FbgPlayer.GetVacationModeStatus();
        return RETURN_SUCCESS(new
        {
            status = status,
            result = result
        });
    
    }
    */

    protected string UserSetVIPChatBorderDisplay(int statusToSet)
    {
        int updatedStatus = FbgUser.toggleDisplayChatVIP(statusToSet);
        int userVIPLevel = Convert.ToInt32(FbgUser.HasFlag_GetData(Fbg.Bll.User.Flags.VIPLevel));
        ChatHub2.ChatHub2.updateChatUserEntityUserAvatarBorderID(FbgUser.ID, updatedStatus, userVIPLevel);
        return RETURN_SUCCESS(new
        {
            newStatus = updatedStatus
        });
    }

    protected string UserSetAvatar(int AvatarID)
    {

        int result = 0;
        result = FbgUser.SetAvatarID(AvatarID);
        if (result == 0)
        {
            ChatHub2.ChatHub2.updateChatUserEntityUserAvatar(FbgUser.ID, AvatarID);
        }

        return RETURN_SUCCESS(new
        {
            result = result,
            AvatarID = AvatarID
        });
    }

    protected string UserSetSex(int Sex)
    {

        int result = 0;
        result = FbgUser.SetSex(Sex);

        return RETURN_SUCCESS(new
        {
            result = result,
            Sex = Sex
        });

    }

    public string AvatarsGetAll()
    {

        return json_serializer.Serialize(new
        {
            success = true,
            @object = MiscApi.AvatarsGetListAll(FbgUser.ID)

        });

    }


}