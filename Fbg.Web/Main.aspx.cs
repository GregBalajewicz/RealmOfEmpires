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

public partial class Main : MyCanvasIFrameBasePage
{
    //public override bool isMobile { get { return true; } }

    
   
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!IsPostBack)
        {
            Initialize(FbgPlayer, MyVillages, true);
            
            //hdrServerTime.Text = DateTime.Now.ToUniversalTime().ToString("HH:mm:ss");
            DateTime nowLocal = DateTime.UtcNow.AddHours(FbgPlayer.User.TimeZone);
         
            HttpCookie isD2;
            if (!BetaD2.canenterD2(Context, FbgUser, FbgPlayer.Realm, LoggedInMembershipUser.CreationDate))
            {
                isD2 = new HttpCookie("isD2", "0");
                isD2.Value = "0";
                isD2.Expires = DateTime.Now.AddDays(-10);
                Response.Cookies.Add(isD2);
                Response.Redirect("chooserealm.aspx", false);
            }          
        }
       

        if (!BetaD2.canEnterD1(IsTesterRoleOrHigher, LoggedInMembershipUser.CreationDate))
        {
            LinkButton1.Visible = false;
        }


        //
        // place the FREE SERVANTS icon (rate us / review).
        // there are two icons, one for review, the other for free offer. Review, takes precidence
        //
        HttpCookie loginMethodCookie = Request.Cookies[CONSTS.Cookies.LoginMethod];
        string loginMethod = "";
        if (loginMethodCookie != null)
        {
            loginMethod = loginMethodCookie.Value;
        }

        if ( loginMethod == "kongregate" && FbgPlayer.User.HasFlag(Fbg.Bll.User.Flags.Reward_MobileAppRate) == null)
        {
            mobAppTryGift.Visible = true;
        }
        if (FbgUser.LoginType_isArmoredGames && FbgPlayer.User.HasFlag(Fbg.Bll.User.Flags.Reward_MobileAppRate) == null && FbgPlayer.Title.Level > 2)
        {
            mobAppTryGift_AG.Visible = true;
        }
        else
        {
            //
            // display the free servant offer if available
            //
            if (FbgPlayer.User.Offers_HasOffer(Fbg.Bll.User.Offers.Number2))
            {
                Offer2.Visible = true;
            }
        }




        HyperLink h;
        Label l;
        spanRealmSwitch.Controls.Clear();
        foreach (Fbg.Bll.Player p in FbgPlayer.User.Players)
        {
            if (p == FbgPlayer) { continue; }
            if (!p.Realm.IsRunning) { continue; }
            if (p.Realm.ID == BetaD2.D1OnlyRealm) { continue; }
            h = new HyperLink();
            h.CssClass = "listedRealm";
            h.Text = String.Format("R{0} ", p.Realm.Tag);
            h.NavigateUrl = "~/LoginToRealm.aspx?rid=" + p.Realm.ID.ToString() + "&pid=" + p.ID;
            spanRealmSwitch.Controls.Add(h);
        }
        SwitchRealm.InnerText = FbgPlayer.Stewardship_IsLoggedInAsSteward ? "Leave " + FbgPlayer.Name + "'s kingdom" : "";


        //
        // put CC tracking code only if necessary
        //        
        string tracking = Utils.Analytics_CCTracking(Session);
        string pendingEvent = Utils.Analytics_PendingEvent_getScript(Session, FbgPlayer.Realm.ID);

      

        initAgeDisplay();
    }
  
    private void initAgeDisplay()
    {

        if (FbgPlayer.Realm.Age.isFeatureActive)
        {
            Realm.RealmAgeInfo ages = FbgPlayer.Realm.Age;
            currentAge.Attributes.CssStyle.Add("background-image", "url('https://static.realmofempires.com/images/icons/Age" + ages.CurrentAge.AgeNumber.ToString() + ".png')");

            if (ages.NextAge != null)
            {
                timeTillNextAge.Attributes.Add("data-finisheson", Fbg.Bll.Api.ApiHelper.SerializeDate(ages.CurrentAge.Until).ToString());
                nextAge.Attributes.CssStyle.Add("background-image", "url('https://static.realmofempires.com/images/icons/Age" + ages.NextAge.AgeNumber.ToString() + ".png')");
            } else
            {
                nextAgeArea.Visible = false;
            }
        }
        else
        {
            agesArea.Visible = false;
            nextAgeArea.Visible = false;
        }

        Realm.Consolidation c = FbgPlayer.Realm.ConsolidationGet;
        if (c.isActiveOnThisRealm) {
            if (c.IsAttackFreezeActiveNow)
            {
                nextAOC.Visible = true;

                //
                // attack freeze now
                //
                if (c.IsConsolidationDone)
                {
                    timeTillAOC.Attributes.Add("data-finisheson", Fbg.Bll.Api.ApiHelper.SerializeDate(c.AttackFreezeeEndsOn).ToString());
                    lblNextAOC.Text = "AoC done! Attack freeze ends in";
                }
                else
                {
                    timeTillAOC.Attributes.Add("data-finisheson", Fbg.Bll.Api.ApiHelper.SerializeDate(c.TimeOfConsolidation).ToString());
                    lblNextAOC.Text = "AoC LIVE! Promote villages in ";
                    nextAOC.CssClass += " warn_now";
                }
            }
            else
            {
                //
                // attack freeze not now, so maybe AOC in the future, or no more
                if (c.AttackFreezeStartOn > DateTime.Now)
                {
                    nextAOC.Visible = true;

                    timeTillAOC.Attributes.Add("data-finisheson", Fbg.Bll.Api.ApiHelper.SerializeDate(c.AttackFreezeStartOn).ToString());

                    if (c.AttackFreezeStartIn.TotalDays <= 7)
                    {
                        nextAOC.CssClass += " warn_1w";
                    }
                    else if (c.AttackFreezeStartIn.TotalDays <= 14)
                    {
                        nextAOC.CssClass += " warn_2w";
                    }
                }
            }
        }
    }


    /// <summary>
    /// Gets the currently selected village. This is ONLY valid if you first call Initialize(...)
    ///     and if you had passed in true for 'getFullVillageObject'
    /// </summary>
    public Fbg.Bll.Village CurrentlySelectedVillage
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
     public Fbg.Bll.VillageBasicB CurrentlySelectedVillageBasicB
    {
        get
        {
            return ctrlVillageHeaderInfo.Village;
        }
    }

   
     public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, bool getFullVillageObject)
    {
        FbgPlayer = player;
        _villages = villages;
        // tutorial needs the full village object so get it if it is running
        _getFullVillageObject = getFullVillageObject;
        ctrlVillageHeaderInfo.Initialize(FbgPlayer, _villages, _getFullVillageObject);

       // GenerateVillageList();
        
        WriteOutROEInfoForScript();

        hdrPName.InnerText = FbgPlayer.Name;
        //hdrPoints.InnerText = Utils.FormatCost(FbgPlayer.Points);

        if (IsTesterRoleOrHigher)
        {
            lblCurRealm.Text = String.Format("R{0}", FbgPlayer.Realm.Tag);
            lblCurRealm.Visible = true;
        } 
       // SwitchRealm.HRef = Config.BaseUrl.TrimEnd('/') + Page.ResolveUrl(SwitchRealm.HRef);
    }



    private void WriteOutROEInfoForScript()
    {
        roeentities.Text = MiscHtmlHelper.GenerateROEEntitiesJS(FbgPlayer);

        lblJSONStruct.Text = MiscHtmlHelper.GenerateBasicROEJS(FbgPlayer, CurrentlySelectedVillage.id, isMobile, isD2, Device, Request, Context);
        //lblJSONStruct.Text += String.Format(CONSTS.ROEScript3
        //    , FbgPlayer.ID
        //    , "false"
        //    , FbgPlayer.NumberOfVillages
        //    , Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(FbgPlayer.Realm)
        //    ,  Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(FbgPlayer.Realm)
        //    , FbgPlayer.Realm.ID
        //    , CurrentlySelectedVillage.id
        //    , FbgPlayer.Name
        //    , FbgPlayer.Avatar.Image1Url
        //    , Config.InDev ? "true" : "false"
        //    , isMobile ? "true" : "false"                                               // 10 
        //    , (int)Device // this relies on the fact that non-device value is 0 (aka false) , while all devices > 0 (aka true)
        //    , FbgPlayer.Realm.IsVPrealm ? "true" : "false"
        //    , LoginModeHelper.LoginMode(Request).ToString()
        //    , FbgPlayer.Realm.BattleHandicap.IsActive ? "true" : "false"
        //    , FbgPlayer.Realm.UnitDesertionScalingFactor
        //    , UnitMovements.CONSTS.Desertion.MinDistance
        //    , UnitMovements.CONSTS.Desertion.MaxPopulation
        //    , FbgPlayer.Realm.BattleHandicap.Param_MaxHandicap
        //    , FbgPlayer.Realm.BattleHandicap.Param_StartRatio                           
        //    , FbgPlayer.Realm.BattleHandicap.Param_Steepness                            // 20 
        //    , Utils.UserLevel(Context)
        //    , ""                                                                        // 22 empty
        //    , FbgPlayer.Realm.BonusVillChange ? "true" : "false"
        //    , FbgPlayer.GetRestartCost()
        //    , FbgPlayer.User.Credits                                                    // 25
        //    , FbgPlayer.Stewardship_IsLoggedInAsSteward ? "true" : "false"
        //    , FbgPlayer.Clan == null ? "" : String.Format("ROE.Player.Clan={{ id: {0}}};", FbgPlayer.Clan.ID) // this is needed mostly for the chat UI - it needs to know the clan so that it can init the right tab
        //    , Convert.ToInt32(Config.CollectAnalyticsOnThisRealm(FbgPlayer.Realm.ID))
        //    , FbgPlayer.Realm.LocalDBVersion                                             
        //    , FbgPlayer.Realm.RealmType                                                 //30
        //    , Fbg.Bll.CONSTS.UnitIDs.Gov
        //    , Fbg.Bll.CONSTS.UnitIDs.Ram
        //    , Fbg.Bll.CONSTS.UnitIDs.Treb
        //    , Fbg.Bll.CONSTS.UnitIDs.Spy                                                //35
        //    , 1
        //    );
    }

    /// <summary>
    /// Call Initialize again.
    /// When this is to be used? when ever a page changes something that effect the header. The call this so that the header will re-init itself.
    /// </summary>
    public void ReInitalize()
    {
        this.Initialize(FbgPlayer, _villages, _getFullVillageObject);

    }

    protected void btnSwitchToDesktop_onCommand(object sender, CommandEventArgs e)
    {
        HttpCookie isMobile = Request.Cookies["isD2"];
        if (isMobile == null)
        {
            isMobile = new HttpCookie("isD2", "0");
            Response.Cookies.Add(isMobile);
        }
        else
        {
            isMobile.Value = "0";
            Response.Cookies.Add(isMobile);
        }
        Response.Redirect("chooserealm.aspx", false);
    }


    protected double GetDeviceVersion()
    {
        switch (Device)
        {
            case CONSTS.Device.Android:
                return Config.LatestAppVer_Android;
            case CONSTS.Device.iOS:
                return Config.LatestAppVer_iOS;
            default:
                return 0;
        }
    }

    


    protected static string villagesScriptTemplate = "myVillages = [{0}];";
    protected static string oneVillageScriptTemplate = "{{ 'id':{0},'name':'{1}','X':'{2}','Y':'{3}','P':'{4}','S':'{5}'}},";


    protected List<VillageBase> _villages;
    protected bool _getFullVillageObject;

    protected bool _IsForumChanged;
    public bool IsTesterRoleOrHigher
    {
        get
        {
            return (Context.User.IsInRole("Admin") || Context.User.IsInRole("tester"));
        }
    }



    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages)
    {
        Initialize(player, villages, false);
    }
   

    private string NameForPackage(int id)
    {
        return Fbg.Bll.CONSTS.PF_NameForPackage(id);
    }
    private string DescForPackage(int id)
    {
        return Fbg.Bll.CONSTS.PF_DescForPackage(id);
    }

    public bool AllowFriendReward
    {
        get
        {
            if (!IsLoggedInAsSteward 
                && FbgPlayer.Title.ID >= 4
                && !FbgPlayer.Realm.IsTemporaryTournamentRealm
                && 1==2 // we turn off friend reward for now
                )
            {
                return true;
            }
            return false;
        }
    }


    
}
