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

using Facebook;
using Facebook.WebControls;

public partial class ClanForum : MyCanvasIFrameBasePage
{


    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.IsMobile = isMobile;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Forum;
        if (isMobile)
        {
            ClanMenu1.Visible = false;
        }
        if (!this.IsPostBack)
        {
            BindGrid();
        }
        if (dlstForums.Items.Count > 0)
        {
            lbl_Message.Text = "";
            
        }
        else
        {
            lbl_Message.Text = RS("noForum");
        }

        FbgPlayer.ForumChanged_SetAsChecked();
    }
    void BindGrid()
    {
        int ClanID = 0;
        if (FbgPlayer.Clan != null)
        {
            ClanID = FbgPlayer.Clan.ID;
        }
        //Security Part
        if (ClanID != 0)
        {
            if (!new Fbg.Forum.SqlForumsProvider().IsPlayerInClan(FbgPlayer.ID, ClanID, FbgPlayer.Realm.ConnectionStr))
            {
                //
                // make sure player is in clan
                if (FbgPlayer.Clan == null)
                {
                    Response.Redirect("ClanOverview.aspx");
                }

            }
        }
        else
        {
            Response.Redirect("ClanOverview.aspx");
        }

        dlstForums.DataSource = new Fbg.Forum.SqlForumsProvider().GetForumsByClanID(ClanID, FbgPlayer.ID, false, FbgPlayer.Realm.ConnectionStr);
        dlstForums.DataBind();
        
        dlstSharedForum.DataSource = new Fbg.Forum.SqlForumsProvider().GetSharedForumByClanID(ClanID, FbgPlayer.ID, false, FbgPlayer.Realm.ConnectionStr);
        dlstSharedForum.DataBind();
    }
    protected void dlstForums_ItemCommand(object source, DataListCommandEventArgs e)
    {
       
         if (e.CommandName == "Read")
            {
                int ForumID = Convert.ToInt32(e.CommandArgument);
                new Fbg.Forum.SqlForumsProvider().SetForumAsRead(FbgPlayer.ID, ForumID, FbgPlayer.Realm.ConnectionStr);
                BindGrid();

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
