using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Collections.Generic;


public class TroopMovementVillageAndPlayerFilterHelper
{
    public int fromPlayerID
    {
        get
        {
            return _ddFromPlayer == null || String.IsNullOrEmpty(_ddFromPlayer.SelectedValue) ? -1 : Convert.ToInt32(_ddFromPlayer.SelectedValue);
        }
    }
    public int fromVillageID
    {
        get
        {
            return (!_ddFromVillage.Visible || String.IsNullOrEmpty(_ddFromVillage.SelectedValue)) ? -1 : Convert.ToInt32(_ddFromVillage.SelectedValue);
        }
    }
    public int toVillageID
    {
        get
        {
            return (!_ddToVillage.Visible || String.IsNullOrEmpty(_ddToVillage.SelectedValue)) ? -1 : Convert.ToInt32(_ddToVillage.SelectedValue);
        }
    }
    public int toPlayerID
    {
        get
        {
            return _ddToPlayer == null || String.IsNullOrEmpty(_ddToPlayer.SelectedValue) ? -1 : Convert.ToInt32(_ddToPlayer.SelectedValue);
        }
    }
    public enum DirectionType
    {
        incoming,
        outgoing
    }
    public DirectionType _type;
    public DropDownList _ddFromPlayer;
    public DropDownList _ddFromVillage;
    public DropDownList _ddToVillage;
    public DropDownList _ddToPlayer;
    public Fbg.Bll.VillageBasicB _curVillage;
    public bool _hasPF;


    public ITroopMovementDisplay _troopMoveDisplay;

    public void Init()
    {
        if (_ddFromPlayer != null)
        {
            _ddFromPlayer.SelectedIndexChanged += new System.EventHandler(ddFromP_SelectedIndexChanged);
        }
        _ddFromVillage.SelectedIndexChanged += new EventHandler(ddFromV_SelectedIndexChanged);
        if (_ddToPlayer != null)
        {
            _ddToPlayer.SelectedIndexChanged += new System.EventHandler(ddToP_SelectedIndexChanged);
        }
    }


    protected void ddToP_SelectedIndexChanged(object sender, EventArgs e)
    {
        _ddToVillage.Visible = false;
        if (toPlayerID == -1)
        {
            _ddToVillage.Items.Clear();
            // reinit the ToPlayer drop down since we just decided to list all TO player therefore there may be some to players that 
            //  were not in the list before. This will default the DD to 'ALL' be default which is good since this is the current selection
            PopupatePlayerDropDown(_ddToPlayer, _troopMoveDisplay.GetToPlayers());
        }
        else
        {
            if (_troopMoveDisplay.GetToVillages(toPlayerID).Count > 1)
            {
                PopupateVillagesDropDown(_ddToVillage, _troopMoveDisplay.GetToVillages(toPlayerID));
                _ddToVillage.Visible = true;
            }
        }
    }
    protected void ddFromP_SelectedIndexChanged(object sender, EventArgs e)
    {
        _ddFromVillage.Visible = false;
        if (fromPlayerID == -1)
        {
            _ddFromVillage.Items.Clear();
        }
        else
        {
            if (_troopMoveDisplay.GetFromVillages(fromPlayerID).Count > 1)
            {
                PopupateVillagesDropDown(_ddFromVillage, _troopMoveDisplay.GetFromVillages(fromPlayerID));
                _ddFromVillage.Visible = true;
            }
        }
    }
    protected void ddFromV_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (fromVillageID == -1)
        {
            PopupateVillagesDropDown(_ddFromVillage, _troopMoveDisplay.GetFromVillages(fromPlayerID));
        }
    }

    /// <summary>
    /// call this from Page_load when !IsPostback
    /// </summary>
    public void InitOnPageFirstLoad()
    {
        if (_hasPF)
        {
            //
            // if has PF, the populate all drop downs. 
            //
            if (_ddToPlayer == null)
            {
                // if we got TO player drown down, then it means we are showing incoming troops and current player is the 
                //  TO player thefore we show the TO villages down down by default
                PopupateVillagesDropDown(_ddToVillage, _troopMoveDisplay.GetToVillages(-1));
                _ddToVillage.Visible = true;
            }
            else
            {
                PopupatePlayerDropDown(_ddToPlayer, _troopMoveDisplay.GetToPlayers());
            }

            if (_ddFromPlayer == null)
            {
                // if we got no FROM player drown down, then it means we are showing ougoting troops and current player is the 
                //  from player thefore we show the FROM villages down down by default
                PopupateVillagesDropDown(_ddFromVillage, _troopMoveDisplay.GetFromVillages(-1));
                _ddFromVillage.Visible = true;
            }
            else
            {
                PopupatePlayerDropDown(_ddFromPlayer, _troopMoveDisplay.GetFromPlayers());
            }
        }
        else
        {
            if (_ddToPlayer != null)
            {
                _ddToPlayer.Enabled = false;
            }

            if (_ddFromPlayer != null)
            {
                _ddFromPlayer.Enabled = false;
            }
            _ddToVillage.Enabled = false;
            _ddFromVillage.Enabled = false;
            //
            // if got no pf, then init the drop downs accordignly
            //
            if (_type == DirectionType.outgoing)
            {
                _ddToPlayer.Items.Add(new ListItem() { Text = "all", Value = "-1" });
                _ddFromVillage.Items.Add(new ListItem() { Text = String.Format("{0}({1},{2})", _curVillage.name, _curVillage.xcord, _curVillage.ycord), Value = _curVillage.id.ToString() });
                _ddFromVillage.Visible = true;
            }
            else
            {
                _ddFromPlayer.Items.Add(new ListItem() { Text = "all", Value = "-1" });
                _ddToVillage.Items.Add(new ListItem() { Text = String.Format("{0}({1},{2})", _curVillage.name, _curVillage.xcord, _curVillage.ycord), Value = _curVillage.id.ToString() });
                _ddToVillage.Visible = true;
            }
        }

    }

    /// <summary>
    /// call this if you think the drop down choices should change. 
    /// the drop down items will reinit and we will try to maintain the current selection.
    /// </summary>
    /// <returns>true if some dd down, that had an item other then 'all' selected, had its selection changed.
    /// This means, that some drop down was selecting a particular player or village and in the process of reinit, the 
    /// selection changed to ALL which may require that the list of incoming/outgoing is retrieved again</returns>
    public bool ReInitDropDowns()
    {

        bool selectionChanged = false; 

            if (!RePopupatePlayerDropDownMaintainSelection(_ddFromPlayer, _troopMoveDisplay.GetFromPlayers()))
            {
                // since the original selection could not be maintained, 
                //  then signal the the selection has changed and therefore requires reload of troops table
                //  NOTE, if the original selection was ALL, then it would have been maintained. 
                selectionChanged = true;
            }
            if (!RePopupatePlayerDropDownMaintainSelection(_ddToPlayer, _troopMoveDisplay.GetToPlayers()))
            {
                // since the original selection could not be maintained, 
                //  then signal the the selection has changed and therefore requires reload of troops table
                //  NOTE, if the original selection was ALL, then it would have been maintained. 
                selectionChanged = true;
            }


        if (_ddFromPlayer == null)
        {
            // If FROM PLAYER is null then we know we are in outoging troops since the logged in player is the FROM player
            //  therefore we know that the from village is alwyas showing and that it does not depend on the FROM player 
            //  dd therefore it is ok to repopulate it 
            if (_ddFromVillage.Visible == true)
            {
                if (!RePopupateVillagesDropDownMaintainSelection(_ddFromVillage, _troopMoveDisplay.GetFromVillages(-1)))
                {
                    // since the original selection could not be maintained, 
                    //  then signal the the selection has changed and therefore requires reload of troops table
                    //  NOTE, if the original selection was ALL, then it would have been maintained. 
                    selectionChanged = true;
                }
            }
        }

        if (_ddToPlayer == null)
        {
            // If FROM PLAYER is null then we know we are in outoging troops since the logged in player is the FROM player
            //  therefore we know that the from village is alwyas showing and that it does not depend on the FROM player 
            //  dd therefore it is ok to repopulate it 
            if (_ddToVillage.Visible == true)
            {
                if (!RePopupateVillagesDropDownMaintainSelection(_ddToVillage, _troopMoveDisplay.GetToVillages(-1)))
                {
                    // since the original selection could not be maintained, 
                    //  then signal the the selection has changed and therefore requires reload of troops table
                    //  NOTE, if the original selection was ALL, then it would have been maintained. 
                    selectionChanged = true;
                }
            }
        }

        return selectionChanged;
    }

    private void PopupateVillagesDropDown(DropDownList dd, List<Fbg.Common.DataStructs.Village> villages)
    {
        dd.DataSource = villages;
        dd.DataTextField = "NameFull";
        dd.DataValueField = "ID";
        dd.DataBind();
        dd.Items.Insert(0, new ListItem() { Text = "all", Value = "-1" });
        dd.SelectedIndex = 0;
    }

    private void PopupatePlayerDropDown(DropDownList dd, List<Fbg.Common.DataStructs.Player> players)
    {
        dd.DataSource = players;
        dd.DataTextField = "Name";
        dd.DataValueField = "ID";
        dd.DataBind();
        dd.Items.Insert(0, new ListItem() { Text = "all", Value = "-1" });
        dd.SelectedIndex = 0;
    }

    /// <summary>
    /// tries to maintain selection, returns FALSE if it could not
    /// </summary>
    /// <param name="dd"></param>
    /// <param name="players"></param>
    private bool RePopupatePlayerDropDownMaintainSelection(DropDownList dd, List<Fbg.Common.DataStructs.Player> players)
    {
        if (dd != null)
        {
            int prevSelection = String.IsNullOrEmpty(dd.SelectedValue) ? -1 : Convert.ToInt32(dd.SelectedValue); ;
            PopupatePlayerDropDown(dd, players);

            ListItem item = dd.Items.FindByValue(prevSelection.ToString());

            if (item != null)
            {
                dd.SelectedIndex = dd.Items.IndexOf(item);
            }
            else
            {
                // since the original selection could not be maintained, 
                //  then signal the the selection has changed and therefore requires reload of troops table
                //  NOTE, if the original selection was ALL, then it would have been maintained. 
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// tries to maintain selection, returns FALSE if it could not
    /// </summary>
    /// <param name="dd"></param>
    /// <param name="players"></param>
    private bool RePopupateVillagesDropDownMaintainSelection(DropDownList dd, List<Fbg.Common.DataStructs.Village> villages)
    {
        if (dd != null)
        {
            int prevSelection = String.IsNullOrEmpty(dd.SelectedValue) ? -1 : Convert.ToInt32(dd.SelectedValue); ;
            PopupateVillagesDropDown(dd, villages);

            ListItem item = dd.Items.FindByValue(prevSelection.ToString());

            if (item != null)
            {
                dd.SelectedIndex = dd.Items.IndexOf(item);
            }
            else
            {
                // since the original selection could not be maintained, 
                //  then signal the the selection has changed and therefore requires reload of troops table
                //  NOTE, if the original selection was ALL, then it would have been maintained. 
                return false;
            }
        }
        return true;
    }

}
