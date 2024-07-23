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

public partial class MasterMobileMain : MasterBase_Main
{
    public override bool isMobile { get { return true; } }

    public override CONSTS.Device isDevice
    {
        get
        {return Utils.ToDevice(Request.UserAgent);
        }
    }
    
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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //hdrServerTime.Text = DateTime.Now.ToUniversalTime().ToString("HH:mm:ss");
            DateTime nowLocal = DateTime.UtcNow.AddHours(_player.User.TimeZone);
            //curLocalTime.Text = nowLocal.ToString("HH:mm:ss");
            //curLocalDate.Text = nowLocal.ToString("MMM dd ");

            if (IsiFramePopupsBrowser)
            {
                linkQuests.HRef = "";
                linkQuests.Attributes.Add("onclick", "return !popupModalIFrame2('Quests.aspx', 'popup', 'Quests', 'https://static.realmofempires.com/images/icons/M_Quests.png', closeModalIFrameAndReloadHeader); ");
                                
            }


            HttpCookie isMobile;
            if (isDevice == CONSTS.Device.Other && !(Context.User.IsInRole("Admin") || Context.User.IsInRole("tester")))
            {
                isMobile = new HttpCookie("isM", "0");
                isMobile.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(isMobile);
                Response.Redirect("chooserealm.aspx", false);
            }
        }


        //
        // place the FREE SERVANTS icon. there are two icons, one for review, the other for free offer. one for review , takes precidence
        //
        if (isDevice != CONSTS.Device.Other &&
            _player.User.HasFlag(User.Flags.Reward_MobileAppRate) == null)
        {
            mobAppTryGift.Visible = true;
        }
        else
        {
            //
            // display the free servant offer if available
            //
            if (_player.User.Offers_HasOffer(User.Offers.Number2))
            {
                Offer2.Visible = true;
            }
        }

        if (Config.SaleType == 0 ) {
            saleIndicator.Style.Add(HtmlTextWriterStyle.Display, "none");
        }


        ///REALM CONSOLIDATION


        Realm.Consolidation c = _player.Realm.ConsolidationGet;
        if (c.isActiveOnThisRealm)
        {
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

        ///REALM CONSOLIDATION


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

   
    override public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, bool getFullVillageObject)
    {
        _player = player;
        _villages = villages;
        // tutorial needs the full village object so get it if it is running
        _getFullVillageObject = getFullVillageObject;
        ctrlVillageHeaderInfo.Initialize(_player, _villages, _getFullVillageObject);

       // GenerateVillageList();
     
        WriteOutROEInfoForScript();

        hdrPName.InnerText = _player.Name;
        //hdrPoints.InnerText = Utils.FormatCost(_player.Points);

        if (IsTesterRoleOrHigher)
        {
            lblCurRealm.Text = String.Format("R{0}", _player.Realm.Tag);
            lblCurRealm.Visible = true;
        } 
       // SwitchRealm.HRef = Config.BaseUrl.TrimEnd('/') + Page.ResolveUrl(SwitchRealm.HRef);
    }



    private void WriteOutROEInfoForScript()
    {
        roeentities.Text = MiscHtmlHelper.GenerateROEEntitiesJS(_player);
        lblJSONStruct.Text = MiscHtmlHelper.GenerateBasicROEJS(_player, CurrentlySelectedVillage.id, isMobile, false, isDevice, Request, Context);

        //lblJSONStruct.Text += String.Format(CONSTS.ROEScript3
        //    , _player.ID
        //    , "false"
        //    , _player.NumberOfVillages
        //    , Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(_player.Realm)
        //    ,  Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(_player.Realm)
        //    , _player.Realm.ID
        //    , CurrentlySelectedVillage.id
        //    , _player.Name
        //    , _player.Avatar.Image1Url
        //    , Config.InDev ? "true" : "false"
        //    , isMobile ? "true" : "false"                                               //10 
        //    , (int)isDevice // this relies on the fact that non-device value is 0 (aka false) , while all devices > 0 (aka true)
        //    , _player.Realm.IsVPrealm ? "true" : "false"
        //    , LoginModeHelper.LoginMode(Request).ToString()
        //    , _player.Realm.BattleHandicap.IsActive ? "true" : "false"
        //    , _player.Realm.UnitDesertionScalingFactor
        //    , UnitMovements.CONSTS.Desertion.MinDistance
        //    , UnitMovements.CONSTS.Desertion.MaxPopulation
        //    , _player.Realm.BattleHandicap.Param_MaxHandicap
        //    , _player.Realm.BattleHandicap.Param_StartRatio
        //    , _player.Realm.BattleHandicap.Param_Steepness                              //20 
        //    , Utils.UserLevel(Context)
        //    , "" // 22 empty
        //    , _player.Realm.BonusVillChange ? "true" : "false"
        //    , _player.GetRestartCost()
        //    , _player.User.Credits                                                      //25
        //    , _player.Stewardship_IsLoggedInAsSteward ? "true" : "false"
        //    , _player.Clan == null ? "" : String.Format("ROE.Player.Clan={{ id: {0}}};", _player.Clan.ID) // this is needed mostly for the chat UI - it needs to know the clan so that it can init the right tab
        //    , Convert.ToInt32(Config.CollectAnalyticsOnThisRealm(_player.Realm.ID))
        //    , _player.Realm.LocalDBVersion
        //    , _player.Realm.RealmType                                                   //30
        //    , Fbg.Bll.CONSTS.UnitIDs.Gov
        //    , Fbg.Bll.CONSTS.UnitIDs.Ram
        //    , Fbg.Bll.CONSTS.UnitIDs.Treb
        //    , Fbg.Bll.CONSTS.UnitIDs.Spy
        //    , 0);
    }

    /// <summary>
    /// Call Initialize again.
    /// When this is to be used? when ever a page changes something that effect the header. The call this so that the header will re-init itself.
    /// </summary>
    public void ReInitalize()
    {
        this.Initialize(_player, _villages, _getFullVillageObject);

    }

    protected void btnSwitchToDesktop_onCommand(object sender, CommandEventArgs e)
    {
        HttpCookie isMobile = Request.Cookies["isM"];
        if (isMobile == null)
        {
            isMobile = new HttpCookie("isM", "0");
        }
        isMobile.Value = "0";
        isMobile.Expires = DateTime.Now.AddDays(-10);
        Response.Cookies.Add(isMobile);
        Response.Redirect("chooserealm.aspx", false);
    }


    protected double GetDeviceVersion()
    {
        switch (isDevice)
        {
            case CONSTS.Device.Android:
                return Config.LatestAppVer_Android;
            case CONSTS.Device.iOS:
                return Config.LatestAppVer_iOS;
            default:
                return 0;
        }
    }


    public bool AllowFriendReward
    {
        get
        {
            if ( //!IsLoggedInAsSteward && 
                //!_player.LoginType_isArmoredGames 
                 _player.Title.ID >= 4
                && !_player.Realm.IsTemporaryTournamentRealm
                && 1 == 2 // we turn off friend reward for now
                )
            {
                return true;
            }
            return false;
        }
    }

    
    
}
