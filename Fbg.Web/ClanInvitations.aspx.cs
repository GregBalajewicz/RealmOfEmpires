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
using Fbg.Bll;
using Facebook;
using Facebook.WebControls;
using Fbg;

public partial class ClanInvitations : MyCanvasIFrameBasePage
{
    DataTable  dt;

    int invitesLeft;
    DateTime moreInvitesOn;
    bool hasInviteRights=false;
    internal class gvw_InvitedPlayersColumns
    {

        public class AdminColumnIndex
        {
            public static int DeleteInvitation = 2;

        }
    }

    internal class gvw_ViewViewClanInvitationColumns
    {
        public class ClanColumnIndex
        {
            public static int ClanID = 0;
            public static int PlayerID = 1;
            public static int PlayerName = 2;
        }
    }
    private int PageSizeIndex
    {
        get
        {
            if (ViewState["PageSize"] != null)
            {
                return (int) ViewState["PageSize"];
            }
            else
            {
                return 0;
            }
        }
        set
        {
            ViewState.Add("PageSize", value);
        }
    }
    new protected void Page_Load(object sender, EventArgs e)
    {
        lbl_Error.Visible = false;
       
        base.Page_Load(sender, e);

        #region Localize Controls
        if (!IsPostBack)
        {
            this.RegularExpressionValidator1.DataBind();
            this.RegularExpressionValidator2.DataBind();
            this.lnk_Invite.DataBind();
            this.lbl_Error.DataBind();
            this.lblNoClanMessage.DataBind();
            this.lblMoreInvitesOnLabel.DataBind();
            this.btn_Search.DataBind();
        }
        #endregion

        //
        // make sure player is in clan
        if (FbgPlayer.Clan == null)
        {
            Response.Redirect("ClanOverview.aspx");
        }

        //
        // find out if player has invite rights
        if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) 
            || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) 
            || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Inviter)) 
        {
            hasInviteRights = true;
        } else {
            hasInviteRights = false;
        }

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.IsMobile = isMobile;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Invitations;

        
        if (!IsPostBack)
        {
            ddlPage.SelectedIndex = PageSizeIndex;
            gvw_InvitedPlayers.PageSize = Convert.ToInt32(ddlPage.SelectedValue);
            BindGrid();
        }
        
        Page.Form.DefaultButton = lnk_Invite.UniqueID;

        UpdateInvitesLeftPanel();

        //
        // clan size limit?
        if (FbgPlayer.Realm.ClanLimit > 0)
        {
            lblClanSizeLimit.Text = String.Format("Clans are limited to {0} members", FbgPlayer.Realm.ClanLimit); 
        }
    }

    private void UpdateInvitesLeftPanel()
    {
        //
        // get my invite limit
        //
        if (hasInviteRights)
        {
            FbgPlayer.Clan.GetInvitesLeft(out invitesLeft, out moreInvitesOn);

            if (invitesLeft != Int32.MaxValue)
            {
                panelInviteLimit.Visible = true;
                panelNoInviteLimit.Visible = false;

                lblInvitesLeft.Text = invitesLeft.ToString();

                if (moreInvitesOn > DateTime.Now)
                {
                    TimeSpan tsLeft = moreInvitesOn.Subtract(DateTime.Now);
                    lblMoreInvitesIn.Text = Utils.FormatDuration(tsLeft);

                    lblMoreInvitesIn.Visible = true;
                    lblMoreInvitesOnLabel.Visible = true;
                }
                else
                {
                    lblMoreInvitesIn.Visible = false;
                    lblMoreInvitesOnLabel.Visible = false;
                }
            }
        }
    }

    private void BindGrid()
    {
        ShowInvitations();

        if (FbgPlayer.Clan != null)//Security for Inviting part
        {
            if (hasInviteRights)
            {
                pnl_Invite.Visible = true;
                gvw_InvitedPlayers.Columns[gvw_InvitedPlayersColumns.AdminColumnIndex.DeleteInvitation].Visible = true;
            }
            else
            {
                pnl_Invite.Visible = false;
                gvw_InvitedPlayers.Columns[gvw_InvitedPlayersColumns.AdminColumnIndex.DeleteInvitation].Visible = false;
            }
        }
        else
        {
            pnl_Invite.Visible = false;
            gvw_InvitedPlayers.Columns[gvw_InvitedPlayersColumns.AdminColumnIndex.DeleteInvitation].Visible = false;

        }
    }
    private void ShowInvitations()
    {
        dt = Fbg.Bll.Clan.ViewClanInvitations(FbgPlayer,txt_SearchName.Text.Trim() );
        if (dt != null)
        {
            if (!string.IsNullOrEmpty(txt_SearchName.Text.Trim()))
            {
                gvw_InvitedPlayers.EmptyDataText = RS("noInvsFound");
            }
            else
            {
                gvw_InvitedPlayers.EmptyDataText = RS("noPendingInv");
            }
            gvw_InvitedPlayers.DataSource = dt;
            gvw_InvitedPlayers.DataBind();
            
        }
        else
        {
            lblNoClanMessage.Visible = true;
        }
    }
    protected void lnk_Invite_Click(object sender, EventArgs e)
    {
        txt_PlayerName.Text = Utils.ClearHTMLCode(txt_PlayerName.Text);


        if (hasInviteRights)
        {
            if (txt_PlayerName.Text.Trim () != null && txt_PlayerName.Text.Trim () != "")
            {
                string playerNameToInvite = txt_PlayerName.Text.Trim ();
                DateTime moreInvitesOn;
                Clan.InviteResult result = Fbg.Bll.Clan.InvitePlayer(playerNameToInvite, FbgPlayer, out moreInvitesOn);

                if (result != Clan.InviteResult.Success)
                {
                    lbl_Error.Text = String.Format(GetMessageFromCode (result), Utils.FormatEventTime(moreInvitesOn));
                    lbl_Error.Visible = true;
                }
                else
                {
                    ShowInvitations();
                    txt_PlayerName.Text = "";
                    UpdateInvitesLeftPanel();
                    Utils.SendInvitedToClanNotification(PlayerOther.GetPlayer(FbgPlayer.Realm, playerNameToInvite, FbgPlayer.ID)
                        , FbgPlayer);
                }
            }
            else
            {
                lbl_Error.Text = RS("errEnterName");
                lbl_Error.Visible = true;
            }
        }
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
            if (hasInviteRights)
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).Parent.Parent;
                int PlayerID = (int)gvw_InvitedPlayers.DataKeys[row.RowIndex][0];
                Fbg.Bll.Clan.DeleteInvitation(FbgPlayer, PlayerID);
                
                Response.Redirect("ClanInvitations.aspx");
            }
        }
    }
    protected string BindURL(object dataItem)
    {
        int playerID = (int)DataBinder.Eval(dataItem, "PlayerID");       
        return isMobile ? "" :   NavigationHelper.PlayerPublicOverview(playerID );
    }
    protected void ddlPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        PageSizeIndex = ddlPage.SelectedIndex;
        gvw_InvitedPlayers .PageSize = Convert.ToInt32(ddlPage.SelectedValue);
        BindGrid();
    }
    protected void gvw_InvitedPlayers_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvw_InvitedPlayers.PageIndex = e.NewPageIndex;
        BindGrid();

    }
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        BindGrid();
    }

    public string GetMessageFromCode(Clan.InviteResult result)
    {
        string msg = string.Empty;
        switch (result)
        {
            case Clan.InviteResult.Player_Already_In_Clan:
                msg = RS("msg_plyrPartOfClan");
                break;
            case Clan.InviteResult.Player_Already_invited:
                msg = RS("msg_hasInvite");
                break;
            case Clan.InviteResult.Player_Not_Found:
                msg = RS("msg_plyrNotFound");
                break;
            case Clan.InviteResult.Success:
                break;
            case Clan.InviteResult.RanOutOfInvites:
                msg = RS("msg_allInvUsed");
                break;
            case Clan.InviteResult.RealmClanLimit:
                msg = RS("msg_limitReached");
                break;
            default:
                throw new Exception("Unrecognized value of Clan.InviteResult:" + result.ToString());
        }
        return msg;
    }

    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }
        else if (isD2)
        {
            base.MasterPageFile = "masterMain_d2.master";
        }
        base.OnPreInit(e);
    }
}
