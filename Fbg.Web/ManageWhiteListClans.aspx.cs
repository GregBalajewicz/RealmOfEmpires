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

public partial class ManageWhiteListClans : MyCanvasIFrameBasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.IsMobile = isMobile;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.ForumAdmin;
        if (isMobile)
        {
            ClanMenu1.Visible = false;
        }
        int ClanID = 0;
        //
        // 
        // get the clan of the logged in player and DO SECURITY CHECK
        //    Ensure this player is part of this clan and has proper roles to see this page. 
        //
        ClanID = FbgPlayer.Clan == null ? 0 : FbgPlayer.Clan.ID;
        if (ClanID == 0)
        {
            Response.Redirect("AccessDenied.aspx");
        }
        if (!FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner)
            && !FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator)
            && !FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
        {
            Response.Redirect("AccessDenied.aspx");
        }

        //
        // since we got access, then lets display the grid
        //
        if (!IsPostBack)
        {
            BindGrid();
        }

    }
    void BindGrid()
    {
        gvwForums.DataSource = new Fbg.Forum.SqlForumsProvider().GetWhiteListClansByClanID(FbgPlayer.Clan.ID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
        gvwForums.DataBind();
    }
    protected void gvwForums_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Remove")
        {
            int whileListClanID = Convert.ToInt32(e.CommandArgument);
            if (new Fbg.Forum.SqlForumsProvider().DeleteWhiteListClan(whileListClanID, FbgPlayer.Clan.ID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr))
            {
                Response.Redirect("ManageWhiteListClans.aspx");
            }

        }
    }
    protected void lnk_Add_Click(object sender, EventArgs e)
    {
        if (txt_ClanName.Text.Trim() != null && txt_ClanName.Text.Trim() != "")
        {

            if (!new Fbg.Forum.SqlForumsProvider().AddWhiteListClan(txt_ClanName.Text, FbgPlayer.Clan.ID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr))
            {
                lbl_Error.Text = "Error Adding Clan -Check Clan Name";
                lbl_Error.Visible = true;
            }
            else
            {
                BindGrid();
            }
        }
        else
        {
            lbl_Error.Text = RS("errEnterName");
            lbl_Error.Visible = true;
        }

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
