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

public partial class ManageSharedForums : MyCanvasIFrameBasePage
{
    private DataSet _sharedForumsWithClans;
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
        _sharedForumsWithClans = new Fbg.Forum.SqlForumsProvider().GetSharedForumsWithClansByClanID(FbgPlayer.Clan.ID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
        gvwMySharedForums.DataSource = _sharedForumsWithClans.Tables[0];
        gvwMySharedForums.DataBind();
    }
    protected void gvwMySharedForums_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Remove")
        {
            int whileListClanID = Convert.ToInt32(e.CommandArgument);
            if (new Fbg.Forum.SqlForumsProvider().DeleteWhiteListClan(whileListClanID, FbgPlayer.Clan.ID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr))
            {
                Response.Redirect("ManageSharedForums.aspx");
            }

        }
        else if (e.CommandName == "Add")
        {
            int forumID = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = (GridViewRow)(((Button)e.CommandSource).NamingContainer);
            string clanName = ((TextBox)row.FindControl("txt_ClanName")).Text;
            if (!new Fbg.Forum.SqlForumsProvider().AddClanNameToSharedForum(forumID, clanName, FbgPlayer.Clan.ID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr))
            {
                Label lblError = (Label)row.FindControl("lbl_Error");
                lblError.Visible = true;
                lblError.Text = "You cannot share with this clan since this clan does not accept shared forums from you. Contact the clan admins, and ask them to white list your clan for sharing";
               
            }
            else
            {
                row.FindControl("pnl_Add").Visible = false;
                Response.Redirect("ManageSharedForums.aspx");
            }
        }
      
    }
    protected void dl_Clans_ItemCommand(object sender, DataListCommandEventArgs e)
    {
        if (e.CommandName == "Remove")
        {
            string [] str= e.CommandArgument.ToString().Split(new char[] { ',' });
            int sharedforumID = int.Parse(str[1]);
            int sharedWithClanID = int.Parse(str[0]);
            if (new Fbg.Forum.SqlForumsProvider().RemoveClanFromSharedForum(sharedforumID, sharedWithClanID,FbgPlayer.Clan.ID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr))
            {
                Response.Redirect("ManageSharedForums.aspx");
            }

        }
        
    }

    protected void gvwMySharedForums_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataList dl = ((DataList)e.Row.FindControl("dl_Clans"));
            DataView dv = new DataView(_sharedForumsWithClans.Tables[1], "ForumID=" + ((HiddenField)e.Row.FindControl("hdn_ForumID")).Value, "", DataViewRowState.OriginalRows);
            dl.DataSource = dv.ToTable();
            dl.DataBind();
        }
    }
    protected void gvwMySharedForums_SelectedIndexChanged(object sender, EventArgs e)
    {
        gvwMySharedForums.Rows[gvwMySharedForums.SelectedIndex].FindControl("pnl_Add").Visible = true;
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
