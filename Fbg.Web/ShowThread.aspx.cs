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
using System.Collections.Generic;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class ShowThread : MyCanvasIFrameBasePage
{
    int threadPostID = 0;
    Hashtable profiles = new Hashtable();

    protected void Page_Init(object sender, EventArgs e)
    {
        //gvwPosts.PageSize = Globals.Settings.Forums.PostsPageSize;
    }

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
        GetRightGrid.Visible = true;

        #region Localize Controls
        if (!IsPostBack) {
            this.btnCloseThread1.DataBind();
            this.btnCloseThread2.DataBind();

            this.lblPageTitle.DataBind();
        }
        #endregion


        threadPostID = int.Parse(this.Request.QueryString["ID"]);

        BindGrid(); /// we are not using view state so we want to bind the grid each time. 
    }

    private void BindGrid()
    {
        Fbg.Forum.PostDetails post = null;
        try {
            GetRightGrid.DataSource = new Fbg.Forum.SqlForumsProvider().GetThreadByID(threadPostID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
            if (((IList)GetRightGrid.DataSource).Count == 0) {
                panelUI.Visible = false;
                panelNoMsg.Visible = true;
            }
            else {
                GetRightGrid.DataBind();
                threadPostID = int.Parse(this.Request.QueryString["ID"]);
                //this function gethe the post and also update the thread as viewd all this done in SP
                post = new Fbg.Forum.SqlForumsProvider().GetPostByID(threadPostID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
                new Fbg.Forum.SqlForumsProvider().IncrementPostViewCount(threadPostID, FbgPlayer.Realm.ConnectionStr);
                this.Title = string.Format(this.Title, post.Title);
                lblPageTitle.Text = string.Format(RS("thread") + ": <a href='BrowseThreads.aspx?ForumID={0}'>{1}</a> / {2}", post.ForumID, post.ForumTitle, post.Title);
                ShowCommandButtons(post.Closed, post.ForumID, threadPostID, post.AddedBy);
            }
        }
        catch (Exception e) {
            BaseApplicationException ex = new BaseApplicationException("Error while binding posts", e);
            ex.AddAdditionalInformation("threadPostID", threadPostID);
            ex.AddAdditionalInformation("post.Title", post == null ? "post=null" : post.Title);
            ex.AddAdditionalInformation("post.ForumID", post == null ? "post=null" : post.ForumID.ToString());
            ex.AddAdditionalInformation("post.ForumTitle", post == null ? "post=null" : post.ForumTitle);
            throw ex;
        }


    }

    public GridView GetRightGrid
    {
        get
        {
            if (isMobile) {
                return gvwPosts_m;
            }
            else {
                return gvwPosts;
            }
        }
    }

    private void ShowCommandButtons(bool isClosed, int forumID, int threadPostID, string addedBy)
    {
        if (isClosed) {
            lnkNewReply1.Visible = false;
            lnkNewReply2.Visible = false;
            btnCloseThread1.Visible = false;
            btnCloseThread2.Visible = false;
            panClosed.Visible = true;
            lnkMove.Visible = false;
        }
        else {
            bool Admin = (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner));

            lnkNewReply1.NavigateUrl = string.Format(lnkNewReply1.NavigateUrl, forumID, threadPostID);
            lnkNewReply2.NavigateUrl = lnkNewReply1.NavigateUrl;

            lnkMove.Visible = Admin;
            lnkMove.NavigateUrl = string.Format(lnkMove.NavigateUrl, threadPostID);

            btnCloseThread1.Visible = Admin;
            btnCloseThread2.Visible = btnCloseThread1.Visible;
        }
    }

    protected void gvwPosts_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) {
            Fbg.Forum.PostDetails post = null;
            HyperLink lnkEditPost = new HyperLink();
            ImageButton btnDeletePost = new ImageButton();
            HyperLink lnkQuotePost = new HyperLink();
            try {
                post = e.Row.DataItem as Fbg.Forum.PostDetails;
                int threadID = (post.ParentPostID == 0 ? post.ID : post.ParentPostID);
                bool isAdmin = (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) ||
                FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator) || post.AddedBy == FbgPlayer.Name);
                // the link for editing the post is visible to the post's author, and to
                // administrators, editors and moderators
                lnkEditPost = e.Row.FindControl("lnkEditPost") as HyperLink;
                lnkEditPost.NavigateUrl = string.Format(lnkEditPost.NavigateUrl, post.ForumID, threadID, post.ID);
                lnkEditPost.Visible = isAdmin;
                // the link for deleting the thread/post is visible only to administrators, editors and moderators
                btnDeletePost = e.Row.FindControl("btnDeletePost") as ImageButton;
                btnDeletePost.OnClientClick = string.Format(btnDeletePost.OnClientClick,
                   post.ParentPostID == 0 ? RS("entireThread") : RS("_post"));
                btnDeletePost.CommandName = (post.ParentPostID == 0 ? "DeleteThread" : "DeletePost");
                btnDeletePost.CommandArgument = post.ID.ToString();
                btnDeletePost.Visible = isAdmin;
                // if the thread is not closed, show the link to quote the post
                lnkQuotePost = e.Row.FindControl("lnkQuotePost") as HyperLink;
                lnkQuotePost.NavigateUrl = string.Format(lnkQuotePost.NavigateUrl,
                   post.ForumID, threadID, post.ID);
                lnkQuotePost.Visible = !(post.ParentPostID == 0 ? post.Closed : new Fbg.Forum.SqlForumsProvider().GetPostByID(post.ParentPostID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr).Closed);
            }
            catch (Exception ex) {
                {
                    BaseApplicationException bex = new BaseApplicationException("Error while binding posts", ex);
                    bex.AddAdditionalInformation("post", post);
                    bex.AddAdditionalInformation("lnkEditPost", lnkEditPost);
                    bex.AddAdditionalInformation("btnDeletePost", btnDeletePost);
                    bex.AddAdditionalInformation("lnkQuotePost", lnkQuotePost);
                    throw bex;
                }
            }
        }
    }

    protected void gvwPosts_m_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Fbg.Forum.PostDetails post = null;
            HyperLink lnkEditPost = new HyperLink();
            LinkButton btnDeletePost = new LinkButton();
            HyperLink lnkQuotePost = new HyperLink();
            try
            {
                post = e.Row.DataItem as Fbg.Forum.PostDetails;
                int threadID = (post.ParentPostID == 0 ? post.ID : post.ParentPostID);
                bool isAdmin = (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) ||
                FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator) || post.AddedBy == FbgPlayer.Name);
                // the link for editing the post is visible to the post's author, and to
                // administrators, editors and moderators
                lnkEditPost = e.Row.FindControl("lnkEditPost") as HyperLink;
               // lnkEditPost.Text = RS("edit");
                lnkEditPost.NavigateUrl = string.Format(lnkEditPost.NavigateUrl, post.ForumID, threadID, post.ID);
                lnkEditPost.Visible = isAdmin;
                // the link for deleting the thread/post is visible only to administrators, editors and moderators
                btnDeletePost = e.Row.FindControl("btnDeletePost") as LinkButton;
                //btnDeletePost.Text = RS("delete");
                btnDeletePost.OnClientClick = string.Format(btnDeletePost.OnClientClick,
                   post.ParentPostID == 0 ? RS("entireThread") : RS("_post"));
                btnDeletePost.CommandName = (post.ParentPostID == 0 ? "DeleteThread" : "DeletePost");
                btnDeletePost.CommandArgument = post.ID.ToString();
                btnDeletePost.Visible = isAdmin;
                // if the thread is not closed, show the link to quote the post
                lnkQuotePost = e.Row.FindControl("lnkQuotePost") as HyperLink;
                lnkQuotePost.NavigateUrl = string.Format(lnkQuotePost.NavigateUrl,
                   post.ForumID, threadID, post.ID);
                lnkQuotePost.Visible = !(post.ParentPostID == 0 ? post.Closed : new Fbg.Forum.SqlForumsProvider().GetPostByID(post.ParentPostID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr).Closed);
            }
            catch (Exception ex)
            {
                {
                    BaseApplicationException bex = new BaseApplicationException("Error while binding posts", ex);
                    bex.AddAdditionalInformation("post", post);
                    bex.AddAdditionalInformation("lnkEditPost", lnkEditPost);
                    bex.AddAdditionalInformation("btnDeletePost", btnDeletePost);
                    bex.AddAdditionalInformation("lnkQuotePost", lnkQuotePost);
                    throw bex;
                }
            }
        }
    }

    protected void gvwPosts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteThread") {
            int threadPostID = Convert.ToInt32(e.CommandArgument);
            int forumID = new Fbg.Forum.SqlForumsProvider().GetPostByID(threadPostID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr).ForumID;
            new Fbg.Forum.SqlForumsProvider().DeletePost(threadPostID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
            this.Response.Redirect("BrowseThreads.aspx?ForumID=" + forumID.ToString());

        }
        else if (e.CommandName == "DeletePost") {
            int postID = Convert.ToInt32(e.CommandArgument);
            new Fbg.Forum.SqlForumsProvider().DeletePost(postID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
            BindGrid();
        }
    }

    protected void btnCloseThread_Click(object sender, EventArgs e)
    {
        new Fbg.Forum.SqlForumsProvider().CloseThread(threadPostID, FbgPlayer.Realm.ConnectionStr);
        ShowCommandButtons(true, 0, 0, "");
        GetRightGrid.DataBind();
    }

    // Retrieves and returns the profile of the specified user. The profile is cached once
    // retrieved for the first time, so that it is reused if the profile for the same user
    // will be requested more times on the same request
    //protected ProfileCommon GetUserProfile(object userName)
    //{
    //   string name = (string)userName;
    //   if (!profiles.Contains(name))
    //   {
    //      ProfileCommon profile = this.Profile.GetProfile(name);

    //      profiles.Add(name, profile);
    //      return profile;
    //   }
    //   else
    //      return profiles[userName] as ProfileCommon;
    //}

    // Returns the poster level description, according to the input post count
    protected string GetPosterDescription(int posts)
    {
        if (posts >= 1000)
            return "Golden";
        else if (posts >= 500)
            return "Silver";
        if (posts >= 100)
            return "Bronze";
        else
            return "";
    }
    protected void gvwPosts_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
    
        GetRightGrid.PageIndex = e.NewPageIndex;
        BindGrid();
    }    

    public BBCodes.Env getEnv()
    {
        if (isMobile)
        {
            return BBCodes.Env.Mobile_IframePopupBrowser;
        }
        else if (isD2)
        {
            return BBCodes.Env.Desktop2;
        }
        else
        {
            return BBCodes.Env.Desktop;
        }
    }
    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile) {
            base.MasterPageFile = "masterMain_m.master";
        }
        else if (isD2)
        {
            base.MasterPageFile = "masterMain_d2.master";
        }
        base.OnPreInit(e);
    }

}

