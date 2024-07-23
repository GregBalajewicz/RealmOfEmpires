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

public partial class main : MasterBase_Main
{

    override public bool isMobile
    {
        get
        {
            return Utils.isMobile(Request);
        }
    }
    public bool isD2
    {
        get
        {
            return Utils.isD2(Request);
        }
    }

    override public CONSTS.Device isDevice
    {
        get
        {

            return CONSTS.Device.Other;
        }
    }

    public bool IsTesterRoleOrHigher
    {
        get
        {
            return (Context.User.IsInRole("Admin") || Context.User.IsInRole("tester"));
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        
        lblBuildNumber.Text = BuildNumConfig.BuildNumber;

        if (!IsPostBack)
        {
            curServerTime.Text = DateTime.Now.ToUniversalTime().ToString("HH:mm:ss");
            curServerDate.Text = DateTime.Now.ToUniversalTime().ToString("MMM dd ");          
        }

        if (isMobile)
        {
            Response.Redirect("mobilemain.aspx");
        }

     
       
    }

    /*
     * https://roe.fogbugz.com/f/cases/28605/Clan-notification-on-Buy-Servants-page
     */
    protected override void Render(HtmlTextWriter writer)
    {
        // it is imperative that this is done here because we want all work to be done; so that when we go to the Reports page
        //  , the reports can be 
        //  retrieved and hence it would be registered that reports just got checked hence no need to show the highlight
        //  Feb 19 2009 update -> this code stoped working correctly for the new reports and messages indicator
        //      because now the indicator is retrieved earlier. The retrieval is triggered by hte call to Player.NumberOfVillages 
        //      early on in page load cycle. This cases actions such as getting list of messages not to clear the 
        //      new message indicator, and also does not recognize any action on postback resulting in a new message or report
        //      - as an example, disband a clan. you will notice the new report icon does not appear right away. 
        //      To resolve this properly, we would need to sepereate the call to Player.NumberOfVillages and Player.ReportInfo
        //      into 2 seperate DB calls. we don't want do that now so a quick fix is implement by adding the 
        //      "&& !Request.Url.AbsolutePath.Contains("Reports.aspx")"
        //      and "&& !Request.Url.AbsolutePath.Contains("messages.aspx")"
        //      which ensures the new report/message icon do not appear upon access to these pages which should really clear them. 
        bool isNotificationShowing = false;

        if (_player != null
            && _player.ReportInfo
            && !Request.Url.AbsolutePath.Contains("Reports.aspx")
            )
        {
            linkReports.CssClass += " newReport";
            //if (!Tutorial1.isRunning 
            //    && _player.HasFlag(Player.Flags.Notification_NewReportSeen) == null
            //    && (!isNotificationShowing)
            //    )
            //{
            //    panelNotifications.Visible = true;
            //    _player.SetFlag(Player.Flags.Notification_NewReportSeen);
            //    lblText.Text = R_notifications.GetString("NewReport"); //String.Format(Resources.notifications.NewReport, 'https://static.realmofempires.com/images/navIcons/Reports3.gif");
            //    isNotificationShowing = true;
            //}
        }
        if (_player != null
            && _player.MessageInfo
            && !Request.Url.AbsolutePath.Contains("messages.aspx")
            )
        {
            linkMail.CssClass += " newMail";
            //if (!Tutorial1.isRunning
            //    && _player.HasFlag(Player.Flags.Notification_NewMsgSeen) == null
            //    && (!isNotificationShowing)
            //    )
            //{
            //    panelNotifications.Visible = true;
            //    _player.SetFlag(Player.Flags.Notification_NewMsgSeen);
            //    lblText.Text = R_notifications.GetString("NewMessage"); //String.Format(Resources.notifications.NewMessage,'https://static.realmofempires.com/images/navIcons/Mail3.gif");
            //    isNotificationShowing = true;
            //}
        }
        if (_player != null 
            && _player.ForumChanged
            )
        {
            linkClan.CssClass += " newClan";
            //if (!Tutorial1.isRunning
            //    && _player.HasFlag(Player.Flags.Notification_NewClanForumPostSeen) == null
            //    && (!isNotificationShowing)
            //    )
            //{
            //    panelNotifications.Visible = true;
            //    _player.SetFlag(Player.Flags.Notification_NewClanForumPostSeen);
            //    lblText.Text = R_notifications.GetString("newClan"); //String.Format(Resources.notifications.newClan, 'https://static.realmofempires.com/images/navIcons/Clan3.gif");
            //    isNotificationShowing = true;
            //}
        }
        //if (_player != null)
        //{
        //    linkHelp.NavigateUrl = "Help.aspx?" + CONSTS.QuerryString.RealmID.ToString() + "=" + _player.Realm.ID.ToString();
        //}
        //else
        //{
        //    linkHelp.NavigateUrl = "Help.aspx";
        //}
       
        base.Render(writer);
    }
    

    /// <summary>
    /// Gets the currently selected village. This is ONLY valid if you first call Initialize(...)
    ///     and if you had passed in true for 'getFullVillageObject'
    /// </summary>
    override public Fbg.Bll.Village CurrentlySelectedVillage
    {
        get 
        {
            if (_getFullVillageObject)
            {
                return (Village)ctrlVillageHeaderInfo.Village;
            }
            else
            {
                throw new InvalidOperationException("CurrentlySelectedVillage is not valid unless you first call Initialize(...) with true passed in for 'getFullVillageObject'");
            }
        }
    }

    /// <summary>
    /// This gives you access to the currently selected village in form of VillageBasicB.
    /// If you called Initialize(...) and passed in true for 'getFullVillageObject', then you can also 
    /// get the currently selected village in form of a Village object from CurrentlySelectedVillage
    /// </summary>
    override public Fbg.Bll.VillageBasicB CurrentlySelectedVillageBasicB
    {
        get
        {
            return ctrlVillageHeaderInfo.Village;
        }
    }


    // hack - 28605 Clan notification on Buy Servants page
    private static class Consts
    {
        public static string CookieName = "Tutorial";
        public static string Cookie_isRunning = "isRunning";
        public static string Cookie_isRunning_Yes = "1";
        public static string Cookie_isRunning_No = "0";
        public static string Cookie_curStep = "curStep";
        public static string CookieTutorialPosName = "tutPos";


    }


    override public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, bool getFullVillageObject)
    {
        _player = player;
        _villages = villages;
        // tutorial needs the full village object so get it if it is running
        _getFullVillageObject = Tutorial1.isRunning == true ? true : getFullVillageObject;
        ctrlVillageHeaderInfo.ShowNotification = new Controls_VillageHeaderInfo.ShowNotificationDelegate(ShowNotification);
        ctrlVillageHeaderInfo.Initialize(_player, _villages, _getFullVillageObject);

        GenerateVillageList();
        WriteOutROEInfoForScript();
    
        


        //
        // Init the tutorial. 
        //  if it is not running, we do not need to pass in the village object but still need to call it
        //  so that it hides itself. 
        //
        if (Tutorial1.isRunning && !isD2)
        {
            Tutorial1.DisplayCurrentStep((Village)ctrlVillageHeaderInfo.Village);
        } else if (Tutorial1.isRunning && isD2)
        {
            // hack - kill the tutorial in d2 - 28605 Clan notification on Buy Servants page
            HttpCookie cookie = Request.Cookies[Consts.CookieName];
            if (cookie != null)
            {
                cookie.Values[Consts.Cookie_isRunning] = Consts.Cookie_isRunning_No;
                cookie.Values[Consts.Cookie_curStep] = "1";
                cookie.Expires = DateTime.Now.AddDays(10);
                Response.Cookies.Add(cookie);
            }
            Tutorial1.DisplayCurrentStep(null);
        }
        else
        {
            Tutorial1.DisplayCurrentStep(null);
        }


        DateTime nowLocal = DateTime.UtcNow.AddHours(_player.User.TimeZone);
        curLocalTime.Text = nowLocal.ToString("HH:mm:ss");
        curLocalDate.Text = nowLocal.ToString("MMM dd ");
        lblCurRealm.Text = String.Format("Realm {0}", _player.Realm.Tag);
        HyperLink h;
        Label l;
        spanRealmSwitch.Controls.Clear();
        foreach (Fbg.Bll.Player p in _player.User.Players)
        {
            if (p == _player) { continue; }
            l = new Label();
            l.Text = "-";
            h = new HyperLink();
            h.Text = String.Format(" {0} ", p.Realm.Tag);
            h.NavigateUrl = "~/LoginToRealm.aspx?rid=" + p.Realm.ID.ToString() + "&pid=" + p.ID;
            spanRealmSwitch.Controls.Add(l);
            spanRealmSwitch.Controls.Add(h);
        }
        SwitchRealm.InnerText = _player.Stewardship_IsLoggedInAsSteward ? "Leave " + _player.Name + "'s kingdom" : "Switch Realm";

        // a bit of a simplified test; here we basically hide the link for all non-d2 realms for everyone except early testers
        if (
            ( !BetaD2.isD2OnlyRealm(_player.Realm.ID) && !BetaD2.isEarlyTester(_player.User))
            || _player.Stewardship_IsLoggedInAsSteward || isD2)
        {
            betaUI.Visible = false;
        }
    }

    public Controls_Tutorial TutorialControl
    {
        get
        {
            return Tutorial1;
        }
    }

    public void ShowNotification(string message)
    {
        panelNotifications.Visible = true;
        lblText.Text = message;
    }

    public class VillageJson
    {
        public string name;
        public int X;
        public int Y;
        public int coins;
        public int treasury;
        public int inhour;
        public string food;

        public TroopJson[] builds;
        public BuildingJson[] troops;
    }

    public class TroopJson
    {
        public int id;
        public int num;
    }

    public class BuildingJson
    {
        public int id;
        public int level;
    }



    private void GenerateVillageList() 
    {
        //
        // This is needed for the script to create the prev & next village buttons
        //

        // NOW ALL myVillages instead of 3, to make working in Ajax manner

        int villageLocationInList = ctrlVillageHeaderInfo.Village_IndexOf;
        if (villageLocationInList == -1)
        {
            villageLocationInList = 0;
        }

        int prevVillageIndex = villageLocationInList == 0 ? 0 : villageLocationInList - 1;
        int nextVillageIndex = villageLocationInList == _villages.Count - 1 ? villageLocationInList : villageLocationInList + 1;

        //int[] vills;
        //vills = new int[] { prevVillageIndex, villageLocationInList, nextVillageIndex };
        try
        {
            System.Text.StringBuilder villagesScript = new System.Text.StringBuilder(150);

            for (int i = prevVillageIndex; i <= nextVillageIndex; i++)
            {
                VillageBase vil = _villages[i];

                villagesScript.AppendFormat(main.oneVillageScriptTemplate
                        , vil.id
                        , "name"
                        , "1"
                        , "1"
                        , "1"
                        , "1"
                        );
                villagesScript.Append("\n");
            }

            lblJSONStruct.Text = String.Format(main.villagesScriptTemplate, villagesScript);

        }
        catch (Exception ex)
        {
            #region Collect error info
            BaseApplicationException be = new BaseApplicationException("Error populating list of villages", ex);
            be.AdditionalInformation.Add("FbgPlayer.ID", _player.ID.ToString());
            try
            {
                if (_villages != null)
                {
                    foreach (Fbg.Bll.VillageBase vil in _villages)
                    {
                        vil.SerializeToNameValueCollection(be.AdditionalInformation);
                    }
                }
            }
            catch (Exception e2)
            {
                be.AdditionalInformation.Add("Error when serializing villages. msg:", e2.Message);
            }
            throw be;
            #endregion
        }


    }



    private void WriteOutROEInfoForScript()
    {
        lblJSONStruct.Text += String.Format(CONSTS.ROEScript
            , _player.ID
            , "false"
            , _player.NumberOfVillages, Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(_player.Realm)
            , Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(_player.Realm)
            , _player.Realm.ID
            , isMobile ? "true" : "false"
            , (int)isDevice
            , Utils.UserLevel(Context)
            , _player.Realm.BonusVillChange ? "true" : "false"
            , _player.GetRestartCost()
            , _player.User.Credits
            , _player.Stewardship_IsLoggedInAsSteward ? "true" : "false");
    }

    /// <summary>
    /// Call Initialize again.
    /// When this is to be used? when ever a page changes something that effect the header. The call this so that the header will re-init itself.
    /// </summary>
    public void ReInitalize()
    {
        this.Initialize(_player, _villages, _getFullVillageObject);

    }

    public bool IsForumChanged
    {
        get
        {
            return _IsForumChanged;
        }
        set
        {
            _IsForumChanged = value;
        }
    }

    protected void linkNotification_Close_Click(object sender, EventArgs e)
    {
        panelNotifications.Visible = false;
    }
}
