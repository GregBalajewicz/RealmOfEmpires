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


   public partial class ManageUnapprovedPosts : MyCanvasIFrameBasePage
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
           int ClanID = 0;

           if (!IsPostBack)
           {
               this.lblPageTitle.DataBind();
           }

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


           if (!IsPostBack)
           {
               BindGrid();
           }
       }
       private void BindGrid()
       {
           gvwPosts.DataSource = new Fbg.Forum.SqlForumsProvider().GetUnapprovedPosts(FbgPlayer.Clan.ID, FbgPlayer.Realm.ConnectionStr);
           gvwPosts.DataBind();
       }
      protected void gvwPosts_RowCommand(object sender, GridViewCommandEventArgs e)
      {
         if (e.CommandName == "Approve")
         {
             if (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
             {
                 int postID = Convert.ToInt32(
                    gvwPosts.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);

                 new Fbg.Forum.SqlForumsProvider().ApprovePost(postID, FbgPlayer.Realm.ConnectionStr);

             } BindGrid();
         }
      }

      protected void gvwPosts_RowCreated(object sender, GridViewRowEventArgs e)
      {
         if (e.Row.RowType == DataControlRowType.DataRow)
         {
            ImageButton btnApprove = e.Row.Cells[3].Controls[0] as ImageButton;
            btnApprove.OnClientClick = "if (confirm('" + RS("sureToApprove") + "') == false) return false;";
            btnApprove.ToolTip = RS("TT_ApprovePost");
            ImageButton btnDelete = e.Row.Cells[4].Controls[0] as ImageButton;
            btnDelete.OnClientClick = "if (confirm('" + RS("sureToDelete") + "') == false) return false;";
            btnDelete.ToolTip = RS("TT_DeletePost");

            gvwPosts.HeaderRow.Cells[1].Text = RS("hForum");
         }
      }
       protected void gvwPosts_RowDeleting(object sender, GridViewDeleteEventArgs e)
       {
           int PostID = Convert.ToInt32(gvwPosts.DataKeys[e.RowIndex].Value);
           if (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
           {
               new Fbg.Forum.SqlForumsProvider().DeletePost(PostID,FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
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
