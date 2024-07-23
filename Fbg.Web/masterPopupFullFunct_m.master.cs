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

public partial class master_PopupFullFunct_m : MasterBase_PopupFullFunct
{

    Fbg.Bll.Player _player;
    public Village _village;
    List<VillageBase> _villages;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (isMobile) {
            SetDefaultBackgroundColor();
        }
    }

    private void GetVillage()
    {
        if (_player == null)
        {
            throw new InvalidOperationException("set Player first");
        }

        if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.SelectedVillageID]))
        {
            int villageID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.SelectedVillageID]);

            _village = (Fbg.Bll.Village)RetrieveVillage(villageID, true);
            if (_village == null)
            {
                throw new ArgumentException("passed in CONSTS.QuerryString.SelectedVillageID invalid. Got:" + villageID);
            }
        }
        else
        {
            HttpCookie cookie = Request.Cookies[CONSTS.QuerryString.SelectedVillageID];
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

            _village = (Fbg.Bll.Village)RetrieveVillage(vid, true);
            if (_village == null)
            {
                throw new ArgumentException("Cannot get village from SelectedVillageID in cookie. Cookie.Value:" + cookie.Value);
            }

        }
    }


    /// <summary>
    /// may return null if village not found
    /// 
    /// NOTE!! this is almost an exact copy of Controls_VillageHeaderInfo.RetrieveVillage except the commented out part at the very bottom. 
    /// 
    /// The only reason we copied it, since this master page does not have the Controls_VillageHeaderInfo control. PERHAPS IT SHOULD JUST TO GET ITS funcitonality??
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
        /*
        //
        // see if this village is in our village list/filter
        VillageBase v = _villages.Find(delegate(VillageBase vb) { return vb.id == villageID; });
        if (v == null)
        {
            isCurrentFillageNotInFilter = true;
        }
         * */

        return villageRetrieved;
    }
    override public Fbg.Bll.Village CurrentlySelectedVillage
    {
        get
        {
            return _village;
        }
    }
    protected override void Render(HtmlTextWriter writer)
    {       
        base.Render(writer);
    }


    override public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, bool displayVillageHeader)
    {
        _player = player;
        _villages = villages;
        GetVillage();
        WriteOutROEInfoForScript();

        if (displayVillageHeader)
        {
            villageHeader.Visible = true;
            PopulateControl();
        }
    }


    override public void SetDefaultBackgroundColor()
    {
        body.Style.Remove("background-color");
        body.Style.Remove("color");
    }




    private void WriteOutROEInfoForScript()
    {
        lblJSONStruct.Text = String.Format(CONSTS.ROEScript
            , _player.ID
            , "true"
            , _player.NumberOfVillages
            , Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(_player.Realm)
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

    private void PopulateControl()
    {
       

        
        linkVillageName.Text = string.Format("{0} ({1},{2})", _village.name, _village.Cordinates.X
                , _village.Cordinates.Y);

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
        //if (_player.NumberOfVillages > 1)
        //{
        //    if (_village.AreTransportsToThisVillageAvailable)
        //    {
        //        imgSilver.ImageUrl = "https://static.realmofempires.com/images/GetMore2.gif";
        //        imgSilver.CssClass = "getMoreSilverIcon";

        //    }
        //}
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



    public override bool isMobile { get { return true; } }

    public override CONSTS.Device isDevice
    {
        get
        {
            return Utils.ToDevice(Request.UserAgent);
        }
    }
}
