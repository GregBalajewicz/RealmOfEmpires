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


public partial class currentAgeInfo : MyCanvasIFrameBasePage
{

    public Fbg.Bll.Realm.RealmAgeInfo ageInfo;
    MasterBase_Main mainMasterPage;

    public Fbg.Bll.RealmAge previousRealmAge;
    public Fbg.Bll.RealmAge thisRealmAge;
    public Fbg.Bll.RealmAge nextRealmAge ;
    public Fbg.Bll.RealmAge currentRealmAge;

    public Fbg.Bll.Player _player;
    
    public System.Data.DataTable dt ;
    public System.Data.DataTable dtglobal ;
   
    new protected void Page_Load(object sender, EventArgs e)
    {

        base.Page_Load(sender, e);
        _player = FbgPlayer;

        ageInfo = FbgPlayer.Realm.Age;

        currentRealmAge = ageInfo.CurrentAge;
        if (!String.IsNullOrWhiteSpace(Request.QueryString["age"]))
        {
            thisRealmAge = ageInfo.AgeByNumber(Convert.ToInt32(Request.QueryString["age"]));
        }
        if (thisRealmAge == null)
        {
            thisRealmAge = currentRealmAge;
        }

        previousRealmAge = ageInfo.AgeByNumber(thisRealmAge.AgeNumber - 1);
        nextRealmAge = ageInfo.AgeByNumber(thisRealmAge.AgeNumber + 1);

        if (DateTime.Now > thisRealmAge.Until)
        {
            lblTimeTillNextAge.Text = string.Format("You have completed Age {0}. You are in Age {1} now.", thisRealmAge.AgeNumber, currentRealmAge.AgeNumber);
        }
        else if (thisRealmAge == currentRealmAge && nextRealmAge != null)
        {
            TimeSpan timeleft = thisRealmAge.Until.Subtract(DateTime.Now);
            lblTimeTillNextAge.Text = string.Format("You are in Age {3} now. There is {0} days {1} hours {2} minutes to Age {4}", timeleft.Days, timeleft.Hours, timeleft.Minutes, currentRealmAge.AgeNumber, nextRealmAge.AgeNumber);
        }
        else if (thisRealmAge != currentRealmAge &&  DateTime.Now < thisRealmAge.Until )
        {
            TimeSpan timeleft = previousRealmAge.Until.Subtract(DateTime.Now);
            lblTimeTillNextAge.Text = string.Format("You are in Age {3} now. There is {0} days {1} hours {2} minutes to Age {4}", timeleft.Days, timeleft.Hours, timeleft.Minutes, currentRealmAge.AgeNumber, thisRealmAge.AgeNumber);
        }
        else
        {
            lblTimeTillNextAge.Visible = false;
        }


        if (previousRealmAge == null)
        {
            prevAge.Visible = false;
        }
        if (nextRealmAge == null)
        {
            nextAge.Visible = false;
        }


        dt = Fbg.Bll.Stats.GetClanRanking(_player.Realm, 0, Fbg.Bll.Stats.CONSTS.ClanRanking_SortExp.NumVillages);
        dtglobal = Fbg.Bll.Stats.GetGlobalStats(_player.Realm).Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Players];
        
        if (FbgPlayer.Realm.IsTemporaryTournamentRealm)
        {
            pnlWC.Visible = false;
            Prizes.Visible = false;
        }
    }

    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }

        if (isD2)
        {
            base.MasterPageFile = "masterMain_d2.master";
        }


        base.OnPreInit(e);
    }
   
}
