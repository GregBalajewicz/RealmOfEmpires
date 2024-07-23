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

public partial class main_m : MasterBase_Main
{

    public Village _village;

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

    override public bool isMobile
    {
        get
        {
            return Utils.isMobile(Request);
        }
    }

    public override CONSTS.Device isDevice
    {
        get
        {
            return Utils.ToDevice(Request.UserAgent);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

       
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
            return _village;
        }
    }




    override public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, bool getFullVillageObject)
    {
        _player = player;
        _villages = villages;
        // tutorial needs the full village object so get it if it is running
        _getFullVillageObject = getFullVillageObject;
        GetVillage();

        WriteOutROEInfoForScript();
    
        DateTime nowLocal = DateTime.UtcNow.AddHours(_player.User.TimeZone);
        HyperLink h;
        Label l;

    }

  




    private void WriteOutROEInfoForScript()
    {
        lblJSONStruct.Text = String.Format(CONSTS.ROEScript
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

}
