using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Linq;

public partial class Controls_Quests : System.Web.UI.UserControl
{

    /// <summary>
    /// tells if you the browser is such that we do not do iframe popups. like android
    /// </summary>
    public bool IsiFramePopupsBrowser
    {
        get
        {
            return Utils.IsiFramePopupsBrowser(Request);
        }
    }

    private enum Quests 
    {
       // Wait = 0,
//        Tutorial,
  //      SMLvl3,
       // Resarch,
        //SMLvl5,
        //BrrcksLvl1,
        Invite=0,
        Friends,
        Silver,
        Food,
        MyPoints,
        JoinClan,
        ClanLeaders, //new
        ClanAllies, //new
        ClanMates, //new
        SilverProduction,
        DefenseFactor,
        InfantryRecruitTime,
        BarracksRecruitTime,
        BattleSimSpies,
        BattleSimCM,
        BattleSimLC,
        BattleSimLCWithWall,
        BattleSimRamAndTreb,
        Map1, //new 
        Map2, //new 
        Support,
        TOU,
        OtherRealms, // new
        Tavern, //new 
        Stable, //new 
        FindInactives, //new
        RecruitSpies, // new 
        LootingInactives, //new
        Mail, //new 
        Reports //new
      //  Advisor2 //new
    }

    private class CONST
    {
        //public class WaitTimes
        //{
        //    public const int AfterOptions = 3;
        //    public const int AfterFriends = 2;
        //    public const int AfterFood = 2;
        //    public const int AfterClanStuff = 3;
        //    public const int AfterDefenseFactor = 2;
        //    public const int AfterBarracks  = 2;
        //    public const int AfterBattleSim = 4;
        //    public const int AfterTOU = 4;

        //}

    }

    static double[] _questsRewards = new double[] {
       // 0 // this is for Quests.Wait
       // ,3 //Tutorial
        //,0.25 //SMLvl3
        //,0.25 // research
        //,0.25 //SMLvl4
        //,1 //Barracks lvl 1
        2 //Invite
        ,2 //Friends
        ,2 // Silver
        ,2 // Food
        ,1 // my points
        ,3 // JoinClan
        ,2 //ClanLeaders
        ,1 //ClanAllies
        ,2 //ClanMates
        ,1 //SilverProduction,
        ,2 //DefenseFactor,
        ,1 //BarracksRecruitTime,
        ,2 //InfantryRecruitTime,
        ,1 //BattleSimSpies,
        ,1 //BattleSimCM,
        ,1 //BattleSimLC,
        ,1 //BattleSimLCWithWall,
        ,1 //BattleSimRamAndTreb,
        ,1 //Map1
        ,2 // Map2
        ,5 //Support,
        ,2 //TOU
        ,2 // OtherRealms
        ,3 //Tavern
        ,2 //stable 
        ,2 //FindInactives
        ,1 // RecruitSpies
        ,1 // LootingInactives
        ,1 // Mail
        ,1 // Reports
        //,1 //Advisor2

    };

    static bool[] _rewardNow = new bool[] {
    //    true // this is for Quests.Wait
      //  ,true //Tutorial
       // ,false //SMLvl2
       // ,false
        //,false //SMLvl3
       // ,//false //barracks level 1
        true //Invite
        ,true //Friends
        ,true // Silver
        ,true // Food
        ,true // my points
        ,true // JoinClan
        ,true //ClanLeaders
        ,true //ClanAllies
        ,true //ClanMates
        ,true //SilverProduction,
        ,true //DefenseFactor,
        ,true //BarracksRecruitTime,
        ,true //InfantryRecruitTime,
        ,true //BattleSimSpies,
        ,true //BattleSimCM,
        ,true //BattleSimLC,
        ,true //BattleSimLCWithWall,
        ,true //BattleSimRamAndTreb,
        ,true //Map1
        ,true // Map2
        ,true //Support,
        ,true //TOU
        ,true // OtherRealms
        ,false //Tavern
        ,false //stable 
        ,true //FindInactives
        ,false // RecruitSpies
        ,true // LootingInactives
        ,true // Mail
        ,true // Reports
       // ,true //Advisor2

    };
    static object[] _questCompletedFlags = new object[] { 
        //null // this is for Quests.Wait which is never used
        //, Fbg.Bll.Player.Flags.Quests_TutorialQ
        //, Fbg.Bll.Player.Flags.Quests_SMLvl3Q
        /////, Fbg.Bll.Player.Flags.Quests_Research
        //, Fbg.Bll.Player.Flags.Quests_SMLvl5
        //, Fbg.Bll.Player.Flags.Quests_BrrcksLvl1
        Fbg.Bll.Player.Flags.Quests_InviteQ
        , Fbg.Bll.Player.Flags.Quests_FriendsQ //Friends
        , Fbg.Bll.Player.Flags.Quests_SilverQ //silver
        , Fbg.Bll.Player.Flags.Quests_FoodQ 
        , Fbg.Bll.Player.Flags.Quests_MyPointsQ 
        , Fbg.Bll.Player.Flags.Quests_JoinClanQ 
        , Fbg.Bll.Player.Flags.Quests_ClanLeaders
        , Fbg.Bll.Player.Flags.Quests_ClanAllies
        , Fbg.Bll.Player.Flags.Quests_ClanMates
        , Fbg.Bll.Player.Flags.Quests_SilverProduction
        , Fbg.Bll.Player.Flags.Quests_DefenseFactor
        , Fbg.Bll.Player.Flags.Quests_InfantryRecruitTime
        , Fbg.Bll.Player.Flags.Quests_BarracksRecruitTime
        , Fbg.Bll.Player.Flags.Quests_BattleSimSpies
        , Fbg.Bll.Player.Flags.Quests_BattleSimCM 
        , Fbg.Bll.Player.Flags.Quests_BattleSimLC
        , Fbg.Bll.Player.Flags.Quests_BattleSimLCWithWall 
        , Fbg.Bll.Player.Flags.Quests_BattleSimRamAndTreb
        , Fbg.Bll.Player.Flags.Quests_Map1
        , Fbg.Bll.Player.Flags.Quests_Map2
        , Fbg.Bll.Player.Flags.Quests_Support
        , Fbg.Bll.Player.Flags.Quests_TOU
        , Fbg.Bll.Player.Flags.Quests_OtherRealms
        , Fbg.Bll.Player.Flags.Quests_Tavern
        , Fbg.Bll.Player.Flags.Quests_Stable
        , Fbg.Bll.Player.Flags.Quests_FindInactives
        , Fbg.Bll.Player.Flags.Quests_RecruitSpies
        , Fbg.Bll.Player.Flags.Quests_LootingInactives
        , Fbg.Bll.Player.Flags.Quests_Mail
        , Fbg.Bll.Player.Flags.Quests_Reports
        //, Fbg.Bll.Player.Flags.Quests_Advisor2
        };

    protected Fbg.Bll.Player _player;
    Quests _currentQuest;
    List<Fbg.Bll.VillageBase> _myVillages;
    Village _village;
    //static DateTime tmpStartDate = new DateTime(2008, 12, 12);
    HttpCookie questsCookie;
    bool _allowRewardNowPF = false;


    protected void Page_Load(object sender, EventArgs e)
    {
      

    }

    public bool AreAnyQuestVisible {
        get
        {
            
            return true;
        }
    }
    public bool AreAllQuestsDone
    {
        get
        {
            if (_player.HasFlag(Player.Flags.Quests_alldone) != null)
            {
                return true;
            }
            return false;
        }
    }

    public bool IsMobile;
    public bool IsD2;

    public delegate void GetRewardClick();

    private GetRewardClick _getRewardClick; 
    public void Initialize(Fbg.Bll.Player player, List<Fbg.Bll.VillageBase> myVillages, Village _currentVillage, bool isMobile, bool isD2, GetRewardClick getRewardClick)
    {
        _player = player;
        _myVillages = myVillages;
        _village = _currentVillage;
        IsMobile = isMobile;
        IsD2 = isD2;
        _getRewardClick = getRewardClick;
        
        //
        // tournament realms have no quests
        //
        if (_player.Realm.IsTemporaryTournamentRealm)
        {
            this.Visible = false;
            return;
        }
        
        //
        // are all quests completed?
        //
        if (AreAllQuestsDone)
        {
            this.Visible = false;
            return;
        }


        DateTime playerRegisteredOn = player.RegisteredOn;

        // check if we have the quests window minimized - this will be needed else where
        //questsCookie = Request.Cookies[_player.ID + global::CONSTS.Cookies.Quests];
        //
        // display the quests
        //
        DisplayCurrentQuest();

        //
        // Show rewardNow link only if player has the PF and has at an account on at least one other realm. 
        //
        bool hasRewardNowPF = player.PF_HasPF(Fbg.Bll.CONSTS.PFs.RewardNow);
        _allowRewardNowPF = hasRewardNowPF && (player.User.Players.Count > 1) && _rewardNow[(int)_currentQuest];
        if (hasRewardNowPF && _allowRewardNowPF)
        {
            linkCheat.Visible = false;
            btnCheat.Visible = true;
        }
        else if (!hasRewardNowPF && (player.User.Players.Count > 1))
        {
            linkCheat.Visible = true;
            btnCheat.Visible = false;
        }
        else
        {
            linkCheat.Visible = false;
            btnCheat.Visible = false;
        }

       


    }


    public double GetWaitTime(int waitTime)
    {
        return 0;
        //if (_player.Realm.IsVPrealm)
        //{
        //    return 0.05;
        //}
        //else
        //{
        //    return Convert.ToDouble(waitTime);
        //}
    }
   


    private bool DisplayCurrentQuest()
    {
        double spySucessChance=0;
        double spyIdentityKnownChance=0;
        double spyAttackVisibleChance=0;

        lblClainError.Visible = false;
        try
        {
            //
            // hide everything
            //
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel)
                {
                    ctrl.Visible = false;
                }
            }

            //if (_player.HasFlag(Fbg.Bll.Player.Flags.Quests_TutorialQ) == null)
            //{
            //    //
            //    // do the tutorial 
            //    //
            //    _currentQuest = Quests.Tutorial;
            //    panel_QTutorial.Visible = true;
            //}
            //else if (_player.HasFlag(Fbg.Bll.Player.Flags.Quests_SMLvl3Q) == null)
            //{
            //    //
            //    //
            //    _currentQuest = Quests.SMLvl3;
            //    panel_QSMLvl3.Visible = true;
            //}
            // if (_player.HasFlag(Fbg.Bll.Player.Flags.Quests_Research) == null && _player.Realm.Research.IsResearchActive)
            //{
            //    //
            //    //
            //    _currentQuest = Quests.Resarch;
            //    panel_QResearch.Visible = true;
            //}
            //else if (_player.HasFlag(Fbg.Bll.Player.Flags.Quests_SMLvl5) == null)
            //{
            //    //
            //    // edit options
            //    //
            //    _currentQuest = Quests.SMLvl5;
            //    panel_QSMLvl5.Visible = true;
            //}
            //else if (_player.HasFlag(Fbg.Bll.Player.Flags.Quests_BrrcksLvl1) == null)
            //{
            //    //
            //    // edit options
            //    //
            //    _currentQuest = Quests.BrrcksLvl1;
            //    panel_QBrcksLvl1.Visible = true;
            //}
            //else if (!HaveHoursPassed(_player.HasFlag(Fbg.Bll.Player.Flags.Quests_AdvisorQ), GetWaitTime(CONST.WaitTimes.AfterOptions))
            //    && _player.HasFlag(Player.Flags.Quests_InviteQ) == null
            //    && questsAvilUntil.TotalDays > 3)
            //{
            //    //
            //    // ***** WAIT ***** 
            //    //
            //    _currentQuest = Quests.Wait;
            //    SetTimeLeft(Player.Flags.Quests_AdvisorQ, GetWaitTime(CONST.WaitTimes.AfterOptions));
            //}
            if (_player.HasFlag(Player.Flags.Quests_InviteQ) == null && !IsMobile && !IsD2 )
            {
                //
                // locate the invite button
                //
                _currentQuest = Quests.Invite;
                panel_QInvite.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_FriendsQ) == null && !IsMobile && !IsD2)
            {
                //
                // How many friends are playing with me?
                //
                _currentQuest = Quests.Friends;
                panel_QFriends.Visible = true;
            }
            //else if (!HaveHoursPassed(_player.HasFlag(Fbg.Bll.Player.Flags.Quests_FriendsQ), GetWaitTime(CONST.WaitTimes.AfterFriends))
            //    && _player.HasFlag(Player.Flags.Quests_SilverQ) == null
            //    && questsAvilUntil.TotalDays > 3)
            //{
            //    //
            //    // ***** WAIT ***** 
            //    //
            //    _currentQuest = Quests.Wait;
            //    SetTimeLeft(Player.Flags.Quests_FriendsQ, GetWaitTime(CONST.WaitTimes.AfterFriends));
            //}
            else if (_player.HasFlag(Player.Flags.Quests_SilverQ) == null)
            {
                //
                // how much silver do I have ?
                //
                _currentQuest = Quests.Silver;
                panel_QSilver.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_FoodQ) == null)
            {
                //
                // how much food do I have ?
                //
                _currentQuest = Quests.Food;
                panel_QFood.Visible = true;
            }
            //else if (!HaveHoursPassed(_player.HasFlag(Fbg.Bll.Player.Flags.Quests_FoodQ), GetWaitTime(CONST.WaitTimes.AfterFood))
            //  && _player.HasFlag(Player.Flags.Quests_MyPointsQ) == null
            //   && questsAvilUntil.TotalDays > 3)
            //{
            //    //
            //    // ***** WAIT ***** 
            //    //
            //    _currentQuest = Quests.Wait;
            //    SetTimeLeft(Player.Flags.Quests_FoodQ, GetWaitTime(CONST.WaitTimes.AfterFood));
            //}
            else if (_player.HasFlag(Player.Flags.Quests_MyPointsQ) == null)
            {
                //
                // how many points do i have?
                //
                _currentQuest = Quests.MyPoints;
                panel_QMyPoints.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_JoinClanQ) == null)
            {
                //
                // Join a clan!
                //
                _currentQuest = Quests.JoinClan;
                panel_QJoinClan.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_ClanLeaders) == null)
            {
                //
                // who are my clan owers
                //
                _currentQuest = Quests.ClanLeaders;
                panel_QClan_Leaders.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_ClanAllies) == null)
            {
                //
                // Join a clan!
                //
                _currentQuest = Quests.ClanAllies;
                panel_QClan_Allies.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_ClanMates) == null && !IsMobile)
            {
                //
                // Join a clan!
                //
                _currentQuest = Quests.ClanMates;
                panel_QClan_Intro.Visible = true;
            }
            //else if (!HaveHoursPassed(_player.HasFlag(Fbg.Bll.Player.Flags.Quests_JoinClanQ), GetWaitTime(CONST.WaitTimes.AfterClanStuff))
            //    && _player.HasFlag(Player.Flags.Quests_SilverProduction) == null
            //    && questsAvilUntil.TotalDays > 3)
            //{
            //    //
            //    // ***** WAIT ***** 
            //    //
            //    _currentQuest = Quests.Wait;
            //    SetTimeLeft(Player.Flags.Quests_JoinClanQ, GetWaitTime(CONST.WaitTimes.AfterClanStuff));
            //}
            else if (_player.HasFlag(Player.Flags.Quests_SilverProduction) == null)
            {
                //
                // Silver mine @ level 40 
                //
                _currentQuest = Quests.SilverProduction;
                panel_QSilverProduction.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_DefenseFactor) == null)
            {
                //
                // towers level 4
                //
                _currentQuest = Quests.DefenseFactor;
                panel_QDefenseFactor.Visible = true;
            }
            //else if (!HaveHoursPassed(_player.HasFlag(Fbg.Bll.Player.Flags.Quests_DefenseFactor), GetWaitTime(CONST.WaitTimes.AfterDefenseFactor))
            //    && _player.HasFlag(Player.Flags.Quests_InfantryRecruitTime) == null
            //    && questsAvilUntil.TotalDays > 3)
            //{
            //    //
            //    // ***** WAIT ***** 
            //    //
            //    _currentQuest = Quests.Wait;
            //    SetTimeLeft(Player.Flags.Quests_DefenseFactor, GetWaitTime(CONST.WaitTimes.AfterDefenseFactor));
            //}
            else if (_player.HasFlag(Player.Flags.Quests_InfantryRecruitTime) == null)
            {
                //
                // base recruit time of Infantry
                //
                _currentQuest = Quests.InfantryRecruitTime;
                panel_QInfantryRecruitTime.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_BarracksRecruitTime) == null)
            {
                //
                // Barracks recruitmnent time
                //
                _currentQuest = Quests.BarracksRecruitTime;
                panel_QBarracksRecruitTime.Visible = true;
            }
            //else if (!HaveHoursPassed(_player.HasFlag(Fbg.Bll.Player.Flags.Quests_BarracksRecruitTime), GetWaitTime(CONST.WaitTimes.AfterBarracks))
            //    && _player.HasFlag(Player.Flags.Quests_BattleSimSpies) == null
            //    && questsAvilUntil.TotalDays > 3)
            //{
            //    //
            //    // ***** WAIT ***** 
            //    //
            //    _currentQuest = Quests.Wait;
            //    SetTimeLeft(Player.Flags.Quests_BarracksRecruitTime, GetWaitTime(CONST.WaitTimes.AfterBarracks));
            //}
            else if (_player.HasFlag(Player.Flags.Quests_BattleSimSpies) == null)
            {
                //
                // Battle Sim - spies
                //
                _currentQuest = Quests.BattleSimSpies;
                panel_QBattleSimSpies.Visible = true;
                Simulation_BattleSimSpies(ref spySucessChance
                , ref  spyIdentityKnownChance
                , ref  spyAttackVisibleChance);

                lbl_QBattleSimSpies_SpiesComming.Text = spyAttackVisibleChance.ToString() + "%";
                lbl_QBattleSimSpies_SpySucessChance.Text = spySucessChance.ToString() + "%";
                //txt_QBattleSimSpies_SpyIdentityKnown.Text = spyIdentityKnownChance + "%";
            }
            else if (_player.HasFlag(Player.Flags.Quests_BattleSimCM) == null)
            {
                //
                // Battle Sim - attack with CM 
                //
                _currentQuest = Quests.BattleSimCM;
                panel_QBattleSimCM.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_BattleSimLC) == null)
            {
                //
                // Battle Sim - attack with LC
                //
                _currentQuest = Quests.BattleSimLC;
                panel_QBattleSimLC.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_BattleSimLCWithWall) == null)
            {
                //
                // Battle Sim - attack with LC on a village with walls 
                //
                _currentQuest = Quests.BattleSimLCWithWall;
                panel_QBattleSimLCWithWall.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_BattleSimRamAndTreb) == null)
            {
                //
                // Battle Sim - attack with LC, ram, treb on a village with walls
                //
                _currentQuest = Quests.BattleSimRamAndTreb;
                panel_QBattleSimRamAndTreb.Visible = true;
            }
            //else if (!HaveHoursPassed(_player.HasFlag(Fbg.Bll.Player.Flags.Quests_BattleSimRamAndTreb), GetWaitTime(CONST.WaitTimes.AfterBattleSim))
            //    && _player.HasFlag(Player.Flags.Quests_Support) == null
            //    && questsAvilUntil.TotalDays > 3)
            //{
            //    //
            //    // ***** WAIT ***** 
            //    //
            //    _currentQuest = Quests.Wait;
            //    SetTimeLeft(Player.Flags.Quests_BattleSimRamAndTreb, GetWaitTime(CONST.WaitTimes.AfterBattleSim));
            //}
            else if (_player.HasFlag(Player.Flags.Quests_Map1) == null && !IsMobile)
            {
                //
                // check out the map quest
                //
                _currentQuest = Quests.Map1;
                panel_QMap1.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_Map2) == null && !IsMobile && !IsD2)
            {
                //
                // set map to 19x19
                //
                _currentQuest = Quests.Map2;
                panel_QMap2.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_Support) == null)
            {
                //
                // Battle Sim - attack with LC on a village with walls 
                //
                _currentQuest = Quests.Support;
                panel_QSupport.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_TOU) == null && !IsMobile)
            {
                //
                // Battle Sim - attack with LC, ram, treb on a village with walls
                //
                _currentQuest = Quests.TOU;
                panel_QTOU.Visible = true;
            }
            else if (_player.HasFlag(Player.Flags.Quests_OtherRealms) == null && !IsMobile && !IsD2)
            {
                //
                // 
                //
                _currentQuest = Quests.OtherRealms;
                panel_QRealms.Visible = true;
            }
            //else if (_player.HasFlag(Player.Flags.Quests_Tavern) == null)
            //{
            //    //
            //    // 
            //    //
            //    _currentQuest = Quests.Tavern;
            //    panel_QTavern.Visible = true;
            //}
            //else if (_player.HasFlag(Player.Flags.Quests_Stable) == null)
            //{
            //    //
            //    // 
            //    //
            //    _currentQuest = Quests.Stable;
            //    panel_QStables.Visible = true;
            //}
            //else if (_player.HasFlag(Player.Flags.Quests_FindInactives) == null)
            //{
            //    //
            //    // 
            //    //
            //    _currentQuest = Quests.FindInactives;
            //    panel_QFindInactives.Visible = true;
            //}
            //else if (_player.HasFlag(Player.Flags.Quests_RecruitSpies) == null)
            //{
            //    //
            //    // 
            //    //
            //    _currentQuest = Quests.RecruitSpies;
            //    panel_QRecruitSpies.Visible = true;
            //}
            //else if (_player.HasFlag(Player.Flags.Quests_LootingInactives) == null)
            //{
            //    //
            //    // 
            //    //
            //    _currentQuest = Quests.LootingInactives;
            //    panel_QLootInactives.Visible = true;
            //}
            //else if (_player.HasFlag(Player.Flags.Quests_Mail) == null && !IsMobile)
            //{
            //    //
            //    // 
            //    //
            //    _currentQuest = Quests.Mail;
            //    panel_QMail.Visible = true;
            //}
            //else if (_player.HasFlag(Player.Flags.Quests_Reports) == null && !IsMobile)
            //{
            //    //
            //    // 
            //    //
            //    _currentQuest = Quests.Reports;
            //    panel_QReports.Visible = true;
            //}
            //else if (_player.HasFlag(Player.Flags.Quests_Advisor2) == null)
            //{
            //    //
            //    // 
            //    //
            //    _currentQuest = Quests.Advisor2;
            //    panel_QAdvisor2.Visible = true;
            //}
            else if (_player.HasFlag(Player.Flags.Quests_alldone) == null)
            {
                //
                // ALL QUESTS COMPLETED MSG.
                //

                panelAllQuestsCompleted.Visible = true;
                return true;
            }
            //
            // if not waiting, then display some common pannels
            //
            //if (_currentQuest != Quests.Wait)
            //{
                //
                // if we have the minimize cookie, then hide the quest details. 
                //
                //if (questsCookie != null)
                //{
                //    foreach (Control ctrl in this.Controls)
                //    {
                //        if (ctrl is Panel)
                //        {
                //            ctrl.Visible = false;
                //        }
                //    }
                //    //btnMinimizeOrShow.CssClass = String.Empty;
                //    //btnMinimizeOrShow.Text = "SHOW QUEST";
                //}
                //else
                //{
                    panelClaimReward.Visible = true;
                //}

                lblTitle.Text = Resources.QuestTitles.ResourceManager.GetString(_currentQuest.ToString());
                panelReward.Visible = true;
                lblRewardAmount.Text = Utils.FormatCost(GetQuestRewardAmount());
//                lblQuestNumber.Text = ((int)_currentQuest).ToString();
                //lblRewardDesc.Text = "(" + GetQuestRewardMultiplier().ToString("0.#") + " X your silver production)";
            //}
            //else
            //{
            //    panel_Countdown.Visible = true;
            //    //btnMinimizeOrShow.Visible = false; //hide the Minimize/show quest button
            //}
        }

        catch (Exception ex)
        {
            BaseApplicationException bex = new BaseApplicationException("Error in Display quest", ex);
            bex.AddAdditionalInformation("_currentQuest", _currentQuest);
            bex.AddAdditionalInformation("_village", _village);
            bex.AddAdditionalInformation("questsCookie", questsCookie);
            bex.AddAdditionalInformation("spySucessChance", spySucessChance);
            bex.AddAdditionalInformation("spyIdentityKnownChance", spyIdentityKnownChance);
            bex.AddAdditionalInformation("spyAttackVisibleChance", spyAttackVisibleChance);
            throw bex;
        }


        return true;
    }

    private int GetQuestRewardAmount()
    {
        double multiplier;
        int production;
        //int reward;

        multiplier = _questsRewards[(int)_currentQuest];
        production = Convert.ToInt32(_village.PerHourCoinIncome);
        return Convert.ToInt32(production * multiplier);
    }
    private double GetQuestRewardMultiplier()
    {
        return _questsRewards[(int)_currentQuest];
    }

    protected void lbClaimYourReward_Click(object sender, EventArgs e)
    {
        ClaimReward(true);
    }
    
    private void ClaimReward(bool needCorrectAnswer)
    {

        int iVal, iVal2, iVal3;
        double dVal;
        bool bFailedQuest=false;
        double spySucessChance=0;
        double spyIdentityKnownChance=0;
        double spyAttackVisibleChance=0;
        double junk;
        bool spyExists = false;
        List<AttackingTroops> ats;
        List<DefendingTroops> dts;
        List<BuildingAttacks> ba;
        int currentQuestReward;
        int treasurySize;
        DataRow dr;
        DataSet ds;
        DataTable dt;
        Fbg.Bll.PlayerOther playerOther;
        string selectQuery;

        try
        {
            // 
            // what it the reward and can the treasury handle it???
            //
            currentQuestReward = GetQuestRewardAmount();
            treasurySize = _village.TreasurySize;
            //if (treasurySize - _village.coins < currentQuestReward)
            //{
            //    lblClainError.Visible = true;
            //    lblClainError.Text = String.Format(Resources.QuestsNotCompletedMsgs.NoRoomInTreasury
            //        , Utils.FormatCost(_village.coins)
            //        , Utils.FormatCost(treasurySize)
            //        , Utils.FormatCost(treasurySize - _village.coins));
            //}
            //else


            {
                #region complete the quest

                lblClainError.Visible = false; //lets start by saying there is no erorr. 
                #region figure out if quest is completed, or skip if no need to complete the quest
                //
                // figure out if quest is completed
                //
                if (needCorrectAnswer)
                {
                    switch (_currentQuest)
                    {
                        //case Quests.Tutorial:
                        //    if (_player.HasFlag(Fbg.Bll.Player.Flags.Advisor_BeginnerTutorialCompleted) == null)
                        //    {
                        //        lblClainError.Visible = true;
                        //    }
                        //    break;
                        //case Quests.Resarch:
                        //    if (_player.MyResearch.ResearchItems.Count < 1 )
                        //    {
                        //        lblClainError.Visible = true;
                        //    }
                        //    break;
                        //case Quests.SMLvl3:
                        //    if (_village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.CoinMine) < 3) 
                        //    {
                        //        lblClainError.Visible = true;
                        //    }
                        //    break;
                        //case Quests.SMLvl5:
                        //    if (_village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.CoinMine) < 5)
                        //    {
                        //        lblClainError.Visible = true;
                        //    }
                        //    break;
                        //case Quests.BrrcksLvl1:
                        //    if (_village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Barracks) < 1)
                        //    {
                        //        lblClainError.Visible = true;
                        //    }
                        //    break;
                        case Quests.Invite:
                            if (!_player.Quests.InviteButtonClicked)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.Friends:

                            Int32.TryParse(txt_QFriends.Text, out iVal);

                            dr = Fbg.Bll.Stats.GetPlayersFriendRanking_ByPlayer(_player.Realm, _player.ID);
                            if (dr != null)
                            {
                                if (iVal != (int)dr[Fbg.Bll.Stats.CONSTS.PlayersFriendRanking.FriendCount])
                                {
                                    lblClainError.Visible = true;
                                }
                            }
                            break;
                        case Quests.Silver:
                            Int32.TryParse(txt_QSilver_Silver.Text, out iVal);
                            Int32.TryParse(txt_QSilver_Treasury.Text, out iVal2);

                            bFailedQuest = true;
                            if (iVal < _village.coins + 20 && iVal > _village.coins - 20)
                            {
                                //so we got the coins right, now check treasury
                                if (iVal2 == treasurySize)
                                {
                                    bFailedQuest = false;
                                }
                            }
                            lblClainError.Visible = bFailedQuest;
                            break;
                        case Quests.Food:

                            Int32.TryParse(txt_QFood_Consumption.Text, out iVal);
                            Int32.TryParse(txt_QFood_Prod.Text, out iVal2);
                            Int32.TryParse(txt_QFood_Remain.Text, out iVal3);

                            if (!IsD2)
                            {                                                               
                                if (iVal != _village.CurrentPopulation
                                    || iVal2 != _village.MaxPopulation)
                                {
                                    lblClainError.Visible = true;
                                }
                            }
                            else
                            {                               
                                if (iVal3 != _village.RemainingPopulation)
                                {
                                    lblClainError.Visible = true;
                                }
                            }
                            break;
                        case Quests.MyPoints:
                            Int32.TryParse(txt_QMyPoints_Points.Text, out iVal);

                            if (iVal != _player.Points)
                            {
                                // if points do not match, it could be that the points are old so lets try that 
                                dr = Stats.GetPlayerRanking(_player.Realm).Rows.Find(_player.ID);
                                if (dr != null)
                                {
                                    if (iVal != (int)dr[Stats.CONSTS.PlayerRanking.TotalPoints])
                                    {
                                        lblClainError.Visible = true;
                                    }
                                }
                            }
                            break;
                        case Quests.JoinClan:
                            if (_player.Clan == null || _player.Clan.MemberCount < 2 )
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.ClanLeaders:
                            bFailedQuest = true;
                            if (_player.Clan != null)
                            {
                                ds = Fbg.Bll.Clan.ViewClanMembers(_player, _player.Clan.ID, true);
                                if (ds != null && ds.Tables[Fbg.Bll.Clan.CONSTS.ClanMembersTableIndex.Roles] != null)
                                {
                                    dt = ds.Tables[Fbg.Bll.Clan.CONSTS.ClanMembersTableIndex.Roles];
                                    playerOther = Fbg.Bll.PlayerOther.GetPlayer(_player.Realm, txt_QClan_Leaders.Text.Trim(), _player.ID);
                                    if (playerOther != null)
                                    {
                                        selectQuery = String.Format("{0} = {1} and {2} = {3}"
                                            , Fbg.Bll.Clan.CONSTS.RolesColumnNames.PlayerID, playerOther.PlayerID
                                            , Fbg.Bll.Clan.CONSTS.RolesColumnNames.RoleID, (int)Role.MemberRole.Owner);
                                        if (dt.Select(selectQuery).Length > 0)
                                        {
                                            bFailedQuest = false;
                                        }
                                    }
                                }
                            }
                            if (bFailedQuest == true)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.ClanAllies:
                            bFailedQuest = true;
                            if (_player.Clan != null)
                            {
                                Fbg.Bll.ClanDiplomacy clanDiplomacy = Fbg.Bll.Clan.ViewMyClanDiplomacy(_player);
                                dt = clanDiplomacy.GetAllies();
                                if (
                                    (dt == null || dt.Rows.Count == 0 )
                                    && cb_panel_QClan_Allies.Checked
                                    )
                                {
                                    // if no allies and check box is checked, then quest passed
                                    bFailedQuest = false;
                                }
                                else if (!cb_panel_QClan_Allies.Checked)
                                {
                                    // if no allies checkbox NOT checked, then see if we got this clan as an ally                                    
                                    string clanName = txt_panel_QClan_Allies.Text.Trim();
                                    selectQuery = String.Format("{0} = '{1}'"
                                        , Fbg.Bll.Clan.CONSTS.Diplomacy_Allies_ColNames.Name, clanName.Replace("'","''"));
                                    if (dt.Select(selectQuery).Length > 0)
                                    {
                                        bFailedQuest = false;
                                    }
                                }
                                
                            }
                            if (bFailedQuest == true)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.ClanMates:
                            if (_player.Quests.HasPlayerMadeAForumPost == false)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.SilverProduction:
                            Int32.TryParse(txt_QSilverProduction.Text, out iVal);
                            if (iVal != _player.Realm.BuildingType_CoinMine.Level(40).EffectAsInt)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.DefenseFactor:
                            Double.TryParse(txt_QDefenseFactor.Text, out dVal);
                            if (Convert.ToInt32(Math.Floor(dVal)) != Convert.ToInt32(Math.Floor(Convert.ToDouble(
                                _player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower).Level(4).Effect))))
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.InfantryRecruitTime:
                            Int32.TryParse(txt_QInfantryRecruitTime.Text, out iVal);
                            if (iVal != _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Infantry).RecruitmentTime().Minutes)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.BarracksRecruitTime:
                            Double.TryParse(txt_QBarracksRecruitTime.Text, out dVal);
                            if (Convert.ToInt32(Math.Floor(dVal)) != Convert.ToInt32(Math.Floor(Convert.ToDouble(_player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Barracks).Level(13).Effect))))
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.BattleSimSpies:
                            Double.TryParse(txt_QBattleSimSpies_SpyIdentityKnown.Text, out dVal);

                            Simulation_BattleSimSpies(ref spySucessChance
                            , ref  spyIdentityKnownChance
                            , ref  spyAttackVisibleChance);

                            if (Math.Round(dVal, 2) != spyIdentityKnownChance)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.BattleSimCM:
                            Int32.TryParse(txt_QBattleSimCM.Text, out iVal);

                            ats = new List<AttackingTroops>();
                            dts = new List<DefendingTroops>();
                            ba = new List<BuildingAttacks>();

                            ats.Add(new AttackingTroops(Fbg.Bll.CONSTS.UnitIDs.CM, 100, -1));
                            dts.Add(new DefendingTroops(Fbg.Bll.CONSTS.UnitIDs.CM, 100, 0, 0));
                            ba.Add(new BuildingAttacks());
                            //no ATTAck or Defend Bouns
                            bool isAttackBouns = false;
                            bool isDefendBouns = false;
                            Fbg.Bll.BattleSimulation.Simulate(ref ats, ref dts, ref ba, ref spySucessChance
                                , ref spyIdentityKnownChance, ref spyAttackVisibleChance, ref spyExists, _player, 0.0F, 0, isAttackBouns, isDefendBouns, out junk
                                , _player.Realm.Research.MaxBonus_VillageDefenceFactor()*100, _player.Realm.Research.MaxBonus_AttackBonus());

                            if (iVal != dts[0].UnitAmount - dts[0].UnitKilled)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.BattleSimLC:
                            Int32.TryParse(txt_QBattleSimLC.Text, out iVal);
                            ats = new List<AttackingTroops>();
                            dts = new List<DefendingTroops>();
                            ba = new List<BuildingAttacks>();


                            ats.Add(new AttackingTroops(Fbg.Bll.CONSTS.UnitIDs.LC, 15, -1));
                            dts.Add(new DefendingTroops(Fbg.Bll.CONSTS.UnitIDs.CM, 100, 0, 0));
                            ba.Add(new BuildingAttacks());
                            isAttackBouns = false;
                            isDefendBouns = false;
                            Fbg.Bll.BattleSimulation.Simulate(ref ats, ref dts, ref ba, ref spySucessChance
                                , ref spyIdentityKnownChance, ref spyAttackVisibleChance, ref spyExists, _player, 0.0F, 0, isAttackBouns, isDefendBouns, out junk
                                , _player.Realm.Research.MaxBonus_VillageDefenceFactor() * 100, _player.Realm.Research.MaxBonus_AttackBonus());

                            if (iVal != ats[0].UnitAmount - ats[0].UnitKilled)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.BattleSimLCWithWall:
                            Int32.TryParse(txt__QBattleSimLCWithWall.Text, out iVal);
                            ats = new List<AttackingTroops>();
                            dts = new List<DefendingTroops>();


                            ats.Add(new AttackingTroops(Fbg.Bll.CONSTS.UnitIDs.LC, 15, -1));
                            dts.Add(new DefendingTroops(Fbg.Bll.CONSTS.UnitIDs.CM, 100, 0, 0));
                            ba = BattleSimulation.InitDefenderBuilding(_player);
                            ba = BattleSimulation.GetBuildingUnderAttack(ba, ats, dts, 10, 10, _player);

                            Fbg.Bll.BattleSimulation.Simulate(ref ats, ref dts, ref ba, ref spySucessChance, _player, 0.0F, 0, false, false, 0,
                                _player.Realm.Research.MaxBonus_DefenceFactor(), _player.Realm.Research.MaxBonus_VillageDefenceFactor() * 100, _player.Realm.Research.MaxBonus_AttackBonus());

                            if (iVal != dts[0].UnitAmount - dts[0].UnitKilled)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.BattleSimRamAndTreb:
                            Int32.TryParse(txt_QBattleSimRamAndTreb.Text, out iVal);
                            ats = new List<AttackingTroops>();
                            dts = new List<DefendingTroops>();

                            ats.Add(new AttackingTroops(Fbg.Bll.CONSTS.UnitIDs.LC, 15, -1));
                            ats.Add(new AttackingTroops(Fbg.Bll.CONSTS.UnitIDs.Ram, 40, Fbg.Bll.CONSTS.BuildingIDs.Wall));
                            ats.Add(new AttackingTroops(Fbg.Bll.CONSTS.UnitIDs.Treb, 40, Fbg.Bll.CONSTS.BuildingIDs.DefenseTower));
                            dts.Add(new DefendingTroops(Fbg.Bll.CONSTS.UnitIDs.CM, 100, 0, 0));
                            ba = BattleSimulation.InitDefenderBuilding(_player);
                            ba = BattleSimulation.GetBuildingUnderAttack(ba, ats, dts, 10, 10, _player);

                            Fbg.Bll.BattleSimulation.Simulate(ref ats, ref dts, ref ba, ref spySucessChance, _player, 0.0F, 0, false, false, 0
                                , 0
                                , _player.Realm.Research.MaxBonus_VillageDefenceFactor() * 100, _player.Realm.Research.MaxBonus_AttackBonus());

                            if (iVal != ats[0].UnitAmount - ats[0].UnitKilled)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.Map1:
                            break; // this quest is alwyas successful
                        case Quests.Map2:
                            //
                            // check the size of the map
                            string mapSizeCookieName = _player.Realm.ID + CONSTS.Cookies.MapSize;
                            if (Request.Cookies[mapSizeCookieName] != null
                                && Request.Cookies[mapSizeCookieName].ToString() != "0")
                            {
                                if (Request.Cookies[mapSizeCookieName].Value != "19")
                                {
                                    lblClainError.Visible = true;
                                }
                            }

                            break;
                        case Quests.Support:
                            bFailedQuest = true;
                            List<UnitInVillage> units = _village.GetVillageUnits();
                            foreach (UnitInVillage unit in units)
                            {
                                if (unit.SupportCount != 0)
                                {
                                    bFailedQuest = false;
                                    break;
                                }
                            }
                            lblClainError.Visible = bFailedQuest;
                            break;
                        case Quests.TOU:
                            if (!cb_QTOU1.Checked
                                || cb_QTOU2.Checked
                                || !cb_QTOU3.Checked
                                || cb_QTOU4.Checked
                                || !cb_QTOU5.Checked)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.OtherRealms:                          
                            if (txt_panel_QRealms.Text.Trim() != "1.25")
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.Tavern:
                            if (_village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Tavern) == 0 )
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.Stable:
                            if (_village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Stable) == 0)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.FindInactives:
                            if (Fbg.Bll.Mail.getSentItems(_player.ID, _player.Realm.ConnectionStr, true).Tables[0].Rows.Count <= 0)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.RecruitSpies:
                            if (_village.GetVillageUnit(_player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Spy)).YourUnitsTotalCount < 1)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.LootingInactives:
                            if (cb_QLootInactives_ClanInvite.Checked
                                || cb_QLootInactives_LessPoints.Checked
                                || !cb_QLootInactives_Messages.Checked
                                || !cb_QLootInactives_NoClan.Checked
                                || !cb_QLootInactives_PointsNoUp.Checked)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.Mail:
                            if (_player.Folders.GetFolders(Folders.FolderType.Mail).Count <1 ) 
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        case Quests.Reports:
                            if (_player.Folders.GetFolders(Folders.FolderType.Reports).Count < 1)
                            {
                                lblClainError.Visible = true;
                            }
                            break;
                        //case Quests.Advisor2:
                        //    // this quest is always successful
                        //    break;
                        default:
                            return; // invalid quest, no reward.
                    }
                }
                #endregion


                if (!lblClainError.Visible)
                {
                    //
                    // check to make sure this player does not have a flag for this quest. This is to ensure
                    //  that player is not trying to claim the reward twice from different browser sessions. 
                    if (_player.HasFlag((Player.Flags)_questCompletedFlags[(int)_currentQuest], false) == null)
                    {
                        //
                        // apply the reward for the quest
                        //
                        _player.Items2_Give(null, GetQuestRewardAmount());
                        //_village.UpdateCoins(GetQuestRewardAmount());

                        //
                        // Mark the quest completed
                        //
                        _player.SetFlag((Player.Flags)_questCompletedFlags[(int)_currentQuest]);

                        //
                        // hide evertything and display quest completed message only 
                        //
                        foreach (Control ctrl in this.Controls)
                        {
                            if (ctrl is Panel)
                            {
                                ctrl.Visible = false;
                            }
                        }
//                        lblRewardAmount2.Text = Utils.FormatCost(GetQuestRewardAmount());
                        panelCompletedQuests.Visible = true;
                        panel_QuestCompletedTitle.Visible = true;
                        lblQuestCompletedMsg.Text = GetQuestCompletedMsg();
                        
                        //add a JS like data object
                        string qComplData = "{credits:" + _player.User.Credits + ", completedQuests:" + _player.Quests2.CompletedQuests_RewardNotClaimed().Count + " }";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh_coins", "refreshCoins(" + qComplData + ");", true);
                    }
                }
                else
                {
                    if (IsD2)
                    {
                        // Check to see if there is a D2 specific message
                        lblClainError.Text = Resources.QuestsNotCompletedMsgs.ResourceManager.GetString("D2_"+_currentQuest.ToString());
                        // If not, then just get the regular one, if that exists.
                        if (String.IsNullOrEmpty(lblClainError.Text)) {
                            lblClainError.Text = Resources.QuestsNotCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString());
                        }
                    }
                    else
                    {
                        lblClainError.Text = Resources.QuestsNotCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString());
                    }
                    
                    if (String.IsNullOrEmpty(lblClainError.Text))
                    {
                        lblClainError.Text = "No, this is not correct.";
                    }


                }
                #endregion
            }
        }
        catch (Exception ex)
        {
            BaseApplicationException bex = new BaseApplicationException("Error in claim your reward", ex);
            bex.AddAdditionalInformation("_currentQuest", _currentQuest);
            bex.AddAdditionalInformation("_village", _village);
            bex.AddAdditionalInformation("questsCookie", questsCookie);
            throw bex;
        }

        _getRewardClick();
    }

    private string GetQuestCompletedMsg()
    {
        double spySucessChance = 0;
        double spyIdentityKnownChance = 0;
        double spyAttackVisibleChance = 0;

        switch (_currentQuest)
        {
            case Quests.DefenseFactor:
                return String.Format(Resources.QuestsCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString())
                    , _player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower).Level(4).EffectAsInt
                    , _player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower).Level(4).EffectAsInt - 100
                    , 100 * (_player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower).Level(4).EffectAsInt/(double)100));
            case Quests.InfantryRecruitTime:
                return String.Format(Resources.QuestsCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString())
                    , _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Infantry).RecruitmentTime().Minutes);
            case Quests.BarracksRecruitTime:
                double baserecruitTime = _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Infantry).RecruitmentTime().TotalMinutes;
                return String.Format(Resources.QuestsCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString())
                    , Math.Round(baserecruitTime, 2)
                    , Math.Round(baserecruitTime * Convert.ToDouble(_player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Barracks).Level(13).Effect)/100, 2)
                    , Math.Round(baserecruitTime * Convert.ToDouble(_player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Barracks).Level(25).Effect)/100, 2)
                    );
            case Quests.BattleSimSpies:
                Simulation_BattleSimSpies(ref spySucessChance
                    , ref  spyIdentityKnownChance
                    , ref  spyAttackVisibleChance);
                return String.Format(Resources.QuestsCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString())
                    , spyIdentityKnownChance
                    , spySucessChance
                    , spyAttackVisibleChance);
            default:
                return Resources.QuestsCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString());
        }
    }

    private bool HaveHoursPassed(object fromDate, double numberOfHours)
    {
        return ((DateTime)fromDate).AddHours(numberOfHours / _player.Realm.OverAllSpeedFactor) < DateTime.Now;
    }
    //private void  SetTimeLeft(Fbg.Bll.Player.Flags flag, double numberOfHours)
    //{
    //    lblCountdown.Text = Utils.FormatDuration(
    //    ((DateTime)_player.HasFlag(flag)).AddHours(numberOfHours / _player.Realm.OverAllSpeedFactor).Subtract(DateTime.Now));
    //}

    protected void btnNextQuest_Click(object sender, EventArgs e)
    {
        DisplayCurrentQuest();
    }


    private void Simulation_BattleSimSpies(ref double spySucessChance
            , ref double spyIdentityKnownChance
            , ref double spyAttackVisibleChance
          
)
    {
        List<AttackingTroops> AT_Objs = new List<AttackingTroops>();
        List<DefendingTroops> DT_Objs = new List<DefendingTroops>();
        List<BuildingAttacks> ba = new List<BuildingAttacks>();

        AT_Objs.Add(new AttackingTroops(Fbg.Bll.CONSTS.UnitIDs.Spy, 100, -1));
        DT_Objs.Add(new DefendingTroops(Fbg.Bll.CONSTS.UnitIDs.Spy, 50,0,0));
        ba.Add(new BuildingAttacks());
        bool spyExists = false;
        double junk;
        bool isAttackBouns=false;
        bool isDefendBouns=false;
        Fbg.Bll.BattleSimulation.Simulate(ref AT_Objs, ref DT_Objs, ref ba,  ref spySucessChance
            , ref spyIdentityKnownChance, ref spyAttackVisibleChance, ref spyExists, _player, 0.0F, 0,isAttackBouns ,isDefendBouns, out junk
                                , _player.Realm.Research.MaxBonus_VillageDefenceFactor() *100, _player.Realm.Research.MaxBonus_AttackBonus());

        spySucessChance = Math.Round(spySucessChance*100, 2);
        spyIdentityKnownChance = Math.Round(spyIdentityKnownChance*100, 2);
       spyAttackVisibleChance = Math.Round(spyAttackVisibleChance*100, 2);

      
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        _player.SetFlag(Player.Flags.Quests_alldone);
        this.Visible = false;
    }
    protected void btnMinimize_Click(object sender, EventArgs e)
    {
        if (questsCookie == null)
        {
            questsCookie = new HttpCookie(_player.ID + global::CONSTS.Cookies.Quests, "1");
            questsCookie.Expires = DateTime.Now.AddDays(5);
            Response.Cookies.Add(questsCookie);
        }
        else
        {
            questsCookie = new HttpCookie(_player.ID + global::CONSTS.Cookies.Quests, "1");
            questsCookie.Expires = DateTime.Now.Subtract(new TimeSpan(11,11,11));
            Response.Cookies.Add(questsCookie);
        }
        Response.Redirect(NavigationHelper.VillageOverview(_village.id),false);
    }
    protected void btnSkipQuest_Click(object sender, EventArgs e)
    {
        //
        // Mark the quest completed
        //
        _player.SetFlag((Player.Flags)_questCompletedFlags[(int)_currentQuest]);
        DisplayCurrentQuest();
    }
    protected void btnCheat_Click(object sender, EventArgs e)
    {
        if (_allowRewardNowPF)
        {
            ClaimReward(false);
        }
    }

}
