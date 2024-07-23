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

using Gmbc.Common.Diagnostics.ExceptionManagement;
using Fbg.DAL;
using Fbg.Bll;

using System.Text.RegularExpressions;

public partial class Controls_VillageHeaderInfo : BaseControl
{
    protected Fbg.Bll.VillageBasicB _village;
    private Fbg.Bll.Player _player;
    DataTable tblAttack;
    List<VillageBase> _villages;
    bool _getFullVillageObject;
    int _village_IndexOf = Int32.MinValue;
    //bool forceVillageToBeInList = false;
    bool isCurrentFillageNotInFilter = false;

    public static Regex svidRegEx = new Regex(CONSTS.QuerryString.SelectedVillageID + "=(\\d+)");
    public delegate void ShowNotificationDelegate(string message);
    public ShowNotificationDelegate ShowNotification;

    public Controls_VillageHeaderInfo()
    {
        BaseResName = "VillageHeaderInfo.ascx";
    }

    public class localCONSTS
    {
        public static string NoFilterName = "Show all villages";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        curServerTime.Text = DateTime.Now.ToUniversalTime().ToString("HH:mm:ss");
    }

    private void GetVillage(bool getFullVillageObject)
    {
        HttpCookie cookie;
        if (_player == null)
        {
            throw new InvalidOperationException("set Player first");
        }



        if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.SelectedVillageID]))
        {
            int villageID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.SelectedVillageID]);

            _village = RetrieveVillage(villageID, getFullVillageObject);
            if (_village == null)
            {
                throw new ArgumentException("passed in CONSTS.QuerryString.SelectedVillageID invalid. Got:" + villageID);
            }

             cookie = new HttpCookie(CONSTS.QuerryString.SelectedVillageID, _village.id.ToString());
            Response.Cookies.Add(cookie);
        }
        else
        {
            cookie = Request.Cookies[CONSTS.QuerryString.SelectedVillageID];
            int vid;
            if (cookie != null)
            {
                vid = Convert.ToInt32(cookie.Value);
            }
            else
            {
                vid = 0; // this basically means we don't know the selected village therefore the code will select the first village. 
                _player.LogEvent("No SelectedVillageID in querry string or cookie!", "");
            }

            _village = RetrieveVillage(vid, getFullVillageObject);
            if (_village == null)
            {
                throw new ArgumentException("Cannot get village from SelectedVillageID in cookie. Cookie.Value:" + cookie.Value);
            }

            if (cookie == null)
            {
                cookie = new HttpCookie(CONSTS.QuerryString.SelectedVillageID, _village.id.ToString());
                Response.Cookies.Add(cookie);
            }
        }
    }

    /// <summary>
    /// may return null if village not found
    /// </summary>
    private VillageBasicB RetrieveVillage(int villageID, bool getFullVillageObject)
    {
        VillageBasicB villageRetrieved;
        //
        // retrieve the village. 
        //
        if (getFullVillageObject)
        {
            villageRetrieved = _player.Village(villageID);
            if (villageRetrieved == null && _villages.Count >= 0)
            {
                villageID = _villages[0].id;
                villageRetrieved = _player.Village(villageID);
            }
        }
        else
        {
            villageRetrieved = _player.VillageBasicB(villageID);
            if (villageRetrieved == null && _villages.Count >= 0)
            {
                villageID = _villages[0].id;
                villageRetrieved = _player.VillageBasicB(villageID);
            }
        }
        //
        // see if this village is in our village list/filter
        VillageBase v = _villages.Find(delegate(VillageBase vb) { return vb.id == villageID; });
        if (v == null)
        {
            isCurrentFillageNotInFilter = true;
        }

        return villageRetrieved;
    }
    /// <summary>
    /// Call this and control will load the village by itself. 
    /// village to load will be identified by querry parameter CONSTS.QuerryString.VillageID
    /// and if not found will be read from a cookie
    /// </summary>
    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages)
    {
        Initialize(player, villages, false);
    }
    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, bool getFullVillageObject)
    {
        if (player == null)
        {
            throw new InvalidOperationException("Player is null");
        }
        if (villages == null)
        {
            throw new InvalidOperationException("villages is null");
        }
        if (villages.Count < 1)
        {
            // a relatively quick look at this reveals that this occurs if player loses his last village when he is logged in. 
            //  so rather than throwing an erorr, we will send him to chooserealm page which should re-log him ack in and give him a new village. 
            Response.Redirect("~/chooserealm.aspx");
            //throw new InvalidOperationException("villages is empty");
        }

        _player = player;
        _villages = villages;
        _getFullVillageObject = getFullVillageObject;

        GetVillage(getFullVillageObject);

        //if (_villages.Count == 0 && player.SelectedFilter != null)
        //{
        //    _villages = new List<VillageBase>(1);
        //    _villages.Add(_village);
        //    player.SelectedFilter = null;
        //}

        GetIncomingAttack();

        PopulateControl();
        panelVillageHeaderInfo.Visible = true;

        _village.CoinsUpdatedEvent += new VillageBasicA.CoinsUpdated(_village_CoinsUpdatedEvent);


        InitSleepModeIndicator();

        Fbg.Bll.Player.VacationModeStatus VacationModeStatus = _player.GetVacationModeStatus();
        Fbg.Bll.Player.WeekendModeStatus weekendModeStatus = _player.GetWeekendModeStatus();

        if (VacationModeStatus.active) { Response.Redirect("VacationMode_Active_NoLogin.aspx"); }

        if(weekendModeStatus.active) { Response.Redirect("WeekendMode_Active_NoLogin.aspx"); }

        if (_player.SleepMode_IsActiveNow) { Response.Redirect("SleepMode_Active_NoLogin.aspx"); }
        if (!_player.Stewardship_CanMyActivityContinue) { Response.Redirect(_player.Stewardship_IsLoggedInAsSteward ? "AccountStewards_OwnerCancels.aspx" : "AccountStewards_DeactivateBeforeLogin.aspx"); }
        if (_player.Realm.ConsolidationGet.IsInFreezeTime(DateTime.Now)
            && _player.Realm.ConsolidationGet.TimeOfConsolidation <= DateTime.Now
            && _player.Realm.ConsolidationGet.TimeOfConsolidation != _player.Realm.ConsolidationGet.AttackFreezeStartOn)
        {
            Response.Redirect("ConsolidationInProgress_NoLogin.aspx");
        }
    }

    private void InitSleepModeIndicator()
    {
        if (_player.SleepMode_ActivatingOn != DateTime.MinValue)
        {
            sleepModeIndicator.Visible = true;
            lblSleepMode_Countdown.Text = Utils.FormatDuration(_player.SleepMode_ActivatingOn.Subtract(DateTime.Now));
        }
    }
     void _village_CoinsUpdatedEvent(VillageBasicA village)
    {
        RefreshCoins();
    }

    /// <summary>
    /// Get the selected Village. 
    /// 
    /// If getFullVillageObject=true was passed to Initialize, then this can be safely cast to Village object. 
    /// 
    /// The village is guaranteed to be one of the villages in the list of villages passed in to Initialize 
    /// method BUT, it will NOT be the exact same object - just the same village 
    /// </summary>
    public Fbg.Bll.VillageBasicB Village
    {
        get
        {
            return _village;
        }
    }

    /// <summary>
    /// Get the index of the selected Village in the village list.
    /// This is cached for performance, OK to call multiple times. 
    /// Guaranteed to be between 0 and _villages.Count-1
    /// </summary>
    public int Village_IndexOf
    {
        get
        {
            if (_village_IndexOf == Int32.MinValue)
            {
                _village_IndexOf = _villages.IndexOf(_player.VillageBase(Village.id, _villages));
            }

            return _village_IndexOf;
        }
    }

    private void PopulateControl()
    {
        lblTrasury.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_TreasurySize);
        lblCoins.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_CurSilver);
        spanProd.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_SilverProd);
        lblPopulation.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_Food);

        linkVillageName.NavigateUrl = String.Format("~/VillageOverview.aspx?{0}={1}"
            , CONSTS.QuerryString.SelectedVillageID, _village.id);
        //linkVillageOverview.NavigateUrl = linkVillageName.NavigateUrl;
        linkVillageName.Text = string.Format("{0} ({1},{2})", _village.name, _village.Cordinates.X
                , _village.Cordinates.Y);


        linkMap.NavigateUrl = string.Format(linkMap.NavigateUrl
            , CONSTS.QuerryString.SelectedVillageID
            , _village.id
            , CONSTS.QuerryString.XCord
            , _village.Cordinates.X
            , CONSTS.QuerryString.YCord
            , _village.Cordinates.Y);

        //
        // if only 1 village, than do not display village selection drop down and prev|next navigation
        if (_player.NumberOfVillages < 2)
        {
            linkVillageSelectiondd.Visible = false;
            linkNextVillage.Visible = false;
            linkPrevVillage.Visible = false;
            panelFiltersAndTagsParent.Visible = false;
        }
        else
        {
            //
            // if only 1 village in current list of villages, then hide the prev|next buttons
            if (_villages.Count < 2)
            {
                linkNextVillage.Visible = false;
                linkPrevVillage.Visible = false;
            }
            else
            {
                linkNextVillage.Visible = true;
                linkPrevVillage.Visible = true;
            }
            //
            // if filter applied, then display the filter icon
            if (_player.SelectedFilter != null)
            {
                linkFilter.Visible = true;
                linkFilter.ImageUrl = isCurrentFillageNotInFilter ? "https://static.realmofempires.com/images/funnel3.png" : "https://static.realmofempires.com/images/funnel2.png";
                linkFilter.ToolTip = "'" + _player.SelectedFilter.Name + "' filter applied.";
            }
            else
            {
                linkFilter.Visible = true;
                linkFilter.ImageUrl = "https://static.realmofempires.com/images/funnel1.png";
            }

            FillFilters();
        }
        //
        // coins
        //
        RefreshCoins();
        //
        // Population
        lblPopulation.Text = Utils.FormatCost(_village.CurrentPopulation) + "/" + Utils.FormatCost(_village.MaxPopulation);
        //
        // get more silver icon glow
        //
        if (_player.NumberOfVillages > 1)
        {
            if (_village.AreTransportsToThisVillageAvailable)
            {
                imgSilver.ImageUrl = "https://static.realmofempires.com/images/GetMore2.gif";
                imgSilver.CssClass = "getMoreSilverIcon";

            }
        }
    }

    private void GetIncomingAttack()
    {
        // no attack indicator for closed realms. 
        if (DateTime.Now >= _player.Realm.ClosingOn) {
            linkIncomingAttacks.Visible = false;
            return;
        }

        Fbg.Bll.Player.IncomingAttackInfo att = _player.IncomingAttack();
        if (att.NumAttacks > 0)
        {
            TimeSpan tsLeft = att.FirstAttackArrivalTime.Subtract(DateTime.Now);

            lblIncomingCount.Text = " " + att.NumAttacks.ToString() + RS("_in");
            lblCountdown.Text = Utils.FormatDuration(tsLeft);

            linkIncomingAttacks.Visible = true;
        }
        else
        {
            linkIncomingAttacks.Visible = false;
        }

    }

    public void RefreshCoins()
    {
        //
        // Treasury
        int treasurySize = 0;
        try
        {
            treasurySize = _village.TreasurySize;
            lblCoins.Text = Utils.FormatCost(_village.coins);
            lblTrasury.Text = Utils.FormatCost(treasurySize);
            if (_village.coins >= treasurySize)
            {
                lblCoins.CssClass += " silverOverflow";
                lblTrasury.CssClass += " silverOverflow";
            }
            else
            {
                lblCoins.CssClass = lblCoins.CssClass.Replace("silverOverflow", "");
                lblTrasury.CssClass = lblTrasury.CssClass.Replace("silverOverflow", "");

            }

            lblProduction.Text = Utils.FormatCost(Convert.ToInt32(Math.Round(_village.PerHourCoinIncome, 0))) + "/h";
        }
        catch (Exception ex)
        {
            BaseApplicationException be = new BaseApplicationException("Error populating trasury info", ex);
            be.AdditionalInformation.Add("villageID", _village.id.ToString());
            be.AdditionalInformation.Add("treasurySize", treasurySize.ToString());
            _village.SerializeToNameValueCollection(be.AdditionalInformation);
            throw be;
        }

    }

    private void FillFilters()
    {
        //
        // init list of filters
        //
        panelFilterList.Controls.Clear();
        panelTagList.Controls.Clear();
        HyperLink h;
        LinkButton btn;
        Image img;

        btn = new LinkButton();
        btn.ID = "b-1";
        btn.Text = RS("showAllVill");//localCONSTS.NoFilterName;
        btn.CausesValidation = false;
        btn.CssClass = "";
        btn.CommandArgument = "-1";
        btn.Command += new CommandEventHandler(btn_Command);
        panelFilterList.Controls.Add(btn);

        foreach (FilterBase filter in _player.Filters)
        {
            btn = new LinkButton();
            btn.ID = "b" + filter.ID;
            btn.Text = filter.Name;

            btn.CssClass = _player.SelectedFilter == filter ? " Filter FilterSelected" : " Filter";
            btn.CommandArgument = filter.ID.ToString();
            btn.Command += new CommandEventHandler(btn_Command);
            btn.CausesValidation = false;
            panelFilterList.Controls.Add(btn);
        }
        h = new HyperLink();
        h.Text = RS("link_EditFilters");
        h.NavigateUrl = "~/tags_Filters.aspx";
        h.CssClass = " configSml";
        panelFilterList.Controls.Add(h);


        foreach (TagBase tag in _player.Tags)
        {
            btn = new LinkButton();
            btn.ID = "t" + tag.ID;
            btn.Text = tag.Name;
            if (_village.HasTag(tag.ID))
            {
                btn.CssClass = "Tag";
                btn.CommandName = "-";
            }
            else
            {
                btn.CssClass = "noTag";
                btn.CommandName = "+";
            }
            btn.Attributes["op"] = btn.CommandName;
            btn.Attributes["tagid"] = tag.ID.ToString();
            btn.Attributes["vilid"] = _village.id.ToString();
            btn.CommandArgument = tag.ID.ToString();
            btn.Command += new CommandEventHandler(btn_Tag);
            btn.CausesValidation = false;
            panelTagList.Controls.Add(btn);
        }

        h = new HyperLink();
        h.Text = RS("link_EditTags"); ;
        h.NavigateUrl = "~/Tags.aspx";
        h.CssClass = " configSml";
        panelTagList.Controls.Add(h);
    }

    void btn_Tag(object sender, CommandEventArgs e)
    {
        if (_player.PF_HasPF(Fbg.Bll.CONSTS.PFs.TagsAndFilters))
        {
            if (e.CommandName == "+")
            {
                Fbg.Bll.Tags.AddVillageTag(_player, Convert.ToInt32(e.CommandArgument), _village.id);
            }
            else
            {
                Fbg.Bll.Tags.DeleteVillageTag(_player, Convert.ToInt32(e.CommandArgument), _village.id);
            }
            // get the list of villages again in case removing or adding the tag cause the list of villages to change. 
            _villages = _player.Villages;

            RedirectAfterFilterRelatedChange();
        }
        else
        {
            ShowNotification(RS("featLocked") + "<a Target='_blank' OnClick='return popupUnlock(this);' href='" + NavigationHelper.PFOffMessage_NoTilda(Fbg.Bll.CONSTS.PFs.TagsAndFilters)
                + "'>" + RS("learnToUnlock") + "</a>");
        }
    }

    void btn_Command(object sender, CommandEventArgs e)
    {
        if (_player.PF_HasPF(Fbg.Bll.CONSTS.PFs.TagsAndFilters))
        {
            //
            // figure out which filter was selected
            //
            int selectedFilterID = Convert.ToInt32(e.CommandArgument);
            FilterBase selectedFilter = null;
            if (selectedFilterID == -1)
            {
                selectedFilter = null;
            }
            else
            {
                selectedFilter = _player.Filters.Find(delegate(FilterBase f) { return f.ID == selectedFilterID; });
            }
            //
            // now get the villages this filters give us
            //
            _villages = _player.GetVillages(selectedFilter, true);
            if (_villages.Count == 0)
            {
                ShowNotification(RS("filterNotSelVill") + "<a href='tags_filters.aspx'>" + RS("link_Examine") + "</a>" + RS("seeWhy"));
            }
            else
            {
                _player.SelectedFilter = selectedFilter;

                RedirectAfterFilterRelatedChange();
            }
        }
        else
        {
            if (e.CommandArgument != "-1") // don't show this message if player clicks show all villages; just not to confuse him 
            {
                ShowNotification(RS("featLocked") + "<a Target='_blank' OnClick='return popupUnlock(this);' href='" + NavigationHelper.PFOffMessage_NoTilda(Fbg.Bll.CONSTS.PFs.TagsAndFilters)
                    + "'>" + RS("learnToUnlock") + "</a>");
            }
        }
    }


    private void RedirectAfterFilterRelatedChange()
    {
        //
        // see if this village is in our new village list. if not, we pick first village in the list and display that. 
        //
        int villageID = _village.id;
        VillageBase v = _villages.Find(delegate(VillageBase vb) { return vb.id == villageID; });
        if (v == null)
        {
            // not found, so just take the first village in list. 
            villageID = _villages[0].id;

        }
        //
        // refresh the same page but ensure that we either change the svid or add a new svid param 
        //
        if (svidRegEx.IsMatch(Request.Url.ToString()))
        {
            Response.Redirect(svidRegEx.Replace(Request.Url.ToString(), String.Format("{0}={1}", CONSTS.QuerryString.SelectedVillageID, villageID.ToString())));
        }
        else
        {            
            Response.Redirect(Request.Url.ToString() + String.Format(Request.Url.Query == String.Empty ? "?{0}={1}" : "&{0}={1}"
                , CONSTS.QuerryString.SelectedVillageID, villageID.ToString()));
        }
    }
}
