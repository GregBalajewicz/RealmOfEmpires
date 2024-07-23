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



   public partial class MoveThread : MyCanvasIFrameBasePage
   {
      int threadID = 0;

      new protected void Page_Load(object sender, EventArgs e)
      {
          base.Page_Load(sender, e);

          MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
          mainMasterPage.Initialize(FbgPlayer, MyVillages);
          ClanMenu1.Player = FbgPlayer;
          Fbg.Bll.VillageBasicB village = mainMasterPage.CurrentlySelectedVillageBasicB;
          ClanMenu1.IsMobile = isMobile;
          ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Forum;

         threadID = int.Parse(this.Request.QueryString["ThreadID"]);

         if (!this.IsPostBack)
         {
             #region Localize Controls
             this.lblPageTitle.DataBind();
             this.btnSubmit.DataBind();
             #endregion
             //bind DropDown List
             ddlForums.DataSource = new Fbg.Forum.SqlForumsProvider().GetForumsByClanID(FbgPlayer.Clan.ID, FbgPlayer.ID,false, FbgPlayer.Realm.ConnectionStr);
             ddlForums.DataBind();

             Fbg.Forum.PostDetails post = new Fbg.Forum.SqlForumsProvider().GetPostByID(threadID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
            lblThreadTitle.Text = post.Title;
            lblForumTitle.Text = post.ForumTitle;
            ddlForums.SelectedValue = post.ForumID.ToString();
         }
      }

      protected void btnSubmit_Click(object sender, EventArgs e)
      {
         int forumID = int.Parse(ddlForums.SelectedValue);
         new Fbg.Forum.SqlForumsProvider().MoveThread(threadID, forumID, FbgPlayer.Realm.ConnectionStr);
         this.Response.Redirect("~/BrowseThreads.aspx?ForumID=" + forumID.ToString());
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