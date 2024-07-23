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
using Gmbc.Common.Diagnostics.ExceptionManagement;


public partial class AddEditPost : MyCanvasIFrameBasePage
{
    private int forumID = 0;
    private int threadID = 0;
    private int postID = 0;
    private int quotePostID = 0;
    private bool isNewThread = false;
    private bool isNewReply = false;
    private bool isEditingPost = false;
    private bool IsPostOwner = false;
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

        #region Localize Controls
        if (!IsPostBack)
        {
            this.lblNewThread.DataBind();
            this.lblNewReply.DataBind();
            this.lblEditPost.DataBind();
            this.chkClosed.DataBind();
            this.chk_Sticky.DataBind();
            this.btnSubmit.DataBind();
            this.valRequireTitle.DataBind();
            
        }
        #endregion

        // retrieve the querystring parameters
        forumID = int.Parse(this.Request.QueryString["ForumID"]);

       
        if (!string.IsNullOrEmpty(this.Request.QueryString["ThreadID"]))
        {
            threadID = int.Parse(this.Request.QueryString["ThreadID"]);
            if (!string.IsNullOrEmpty(this.Request.QueryString["QuotePostID"]))
            {
                quotePostID = int.Parse(this.Request.QueryString["QuotePostID"]);
            }
        }
        if (!string.IsNullOrEmpty(this.Request.QueryString["PostID"]))
        {
            postID = int.Parse(this.Request.QueryString["PostID"]);
        }

        isNewThread = (postID == 0 && threadID == 0);
        isEditingPost = (postID != 0);
        isNewReply = (!isNewThread && !isEditingPost);
      
      
        // show/hide controls, and load data according to the parameters above
        if (!this.IsPostBack)
        {
            bool Admin = (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator));


            lnkThreadList.NavigateUrl = string.Format(lnkThreadList.NavigateUrl, forumID);
            lnkThreadPage.NavigateUrl = string.Format(lnkThreadPage.NavigateUrl, threadID);
            chkClosed.Visible = isNewThread;

            if (isEditingPost)
            {
                // load the post to edit, and check that the current user has the permission to do so

                Fbg.Forum.PostDetails post = new Fbg.Forum.SqlForumsProvider().GetPostByID(postID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
                if (post == null)
                {
                    panelUI.Visible = false;
                    panelNoMsg.Visible = true;
                }
                else
                {
                    IsPostOwner = (FbgPlayer.Name == post.AddedBy);

                    lblEditPost.Visible = true;
                    if (IsPostOwner || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
                    {
                        btnSubmit.Text = RS("updateBtn");
                    }
                    else
                    {
                        btnSubmit.Visible = false;
                    }
                    txtTitle.Text = post.Title;
                    txtBody.Text = Utils.CleanupInputText(post.Body);
                    panTitle.Visible = Admin;
                    chk_Sticky.Visible = (postID == threadID);
                    chk_Sticky.Checked = post.Sticky;
                }
            }
            else if (isNewReply)
            {
                // check whether the thread the user is adding a reply to is still open
                Fbg.Forum.PostDetails post = new Fbg.Forum.SqlForumsProvider().GetPostByID(threadID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
                if (post == null)
                {
                    panelUI.Visible = false;
                    panelNoMsg.Visible = true;
                }
                else
                {
                    if (post.Closed)
                    {
                        panInput.Visible = false;
                        panReplayClosed.Visible = true;
                    }

                    lblNewReply.Visible = true;
                    txtTitle.Text = "Re: " + post.Title;
                    lblNewReply.Text = string.Format(lblNewReply.Text, post.Title);
                    // if the ID of a post to be quoted is passed on the querystring, load that post
                    // and prefill the new reply's body with that post's body
                    if (quotePostID > 0)
                    {
                        Fbg.Forum.PostDetails quotePost = new Fbg.Forum.SqlForumsProvider().GetPostByID(quotePostID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
                        if (quotePost != null)
                        {
                            txtBody.Text = string.Format(@"
[quote]
[b]" + RS("orgPostedBy") + @" {0}[/b] 
{1} 
[/quote]"
                                , quotePost.AddedBy, Utils.CleanupInputText(quotePost.Body));
                        
                        }
                    }
                }
            }
            else if (isNewThread)
            {
                lblNewThread.Visible = true;
                lnkThreadList.Visible = true;
                lnkThreadPage.Visible = false;
                chk_Sticky.Visible = Admin;
            }

        }
        else
        {
            if (isEditingPost)
            {
                // load the post to edit, and check that the current user has the permission to do so

                Fbg.Forum.PostDetails post = new Fbg.Forum.SqlForumsProvider().GetPostByID(postID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
                IsPostOwner = (FbgPlayer.Name == post.AddedBy);
            }
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            //forumID = int.Parse(this.Request.QueryString["ForumID"]);
            Fbg.Forum.ForumDetails _fd = new Fbg.Forum.SqlForumsProvider().GetForumByID(forumID, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
            bool approved = true;
            approved = !_fd.Moderated;//if the forum is modirated so any update or insert should be set to not approved

            string title;
            string body;
            string bodyForChat;
            title = Utils.ClearHTMLCode(txtTitle.Text.Trim());
            title = Utils.ChangeLineBreaks(title);
            body = Utils.ClearHTMLCode(txtBody.Text.Trim());
            body = Utils.ChangeLineBreaks(body);
            bodyForChat = BBCodes.PreProcessBBCodes(FbgPlayer.Realm, BBCodes.Medium.Chat, body.Substring(0, Math.Min(body.Length, CONSTS.ChatMaxLength)));
            bodyForChat = BBCodes.ChatHTML(FbgPlayer.Realm, bodyForChat);
            if (body.Length >= 160) {
                bodyForChat += "...";
            }
            body = BBCodes.PreProcessBBCodes(FbgPlayer.Realm, BBCodes.Medium.ClanForum, body);
            


            if (isEditingPost)
            {
                // when editing a post, a line containing the current Date/Time and the name
                // of the user making the edit is added to the post's body so that the operation gets logged
                body += string.Format("\n\r-- {0}: " +RS("postEditedBy")+ " {1}.",
                  Utils.FormatEventTime_NoToday(DateTime.Now), FbgPlayer.Name);
                // edit an existing post
                if (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) || IsPostOwner || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
                {
                    Fbg.Forum.PostDetails record = new Fbg.Forum.PostDetails(postID, DateTime.Now, FbgPlayer.Name, "", 0, "", 0,
                   txtTitle.Text, body, approved, false, 0, 0, DateTime.Now, "", chk_Sticky.Checked);
                    if (new Fbg.Forum.SqlForumsProvider().UpdatePost(record, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr))
                    {
                        panInput.Visible = false;
                        if (approved)
                        {
                            panFeedback.Visible = true;
                        }
                        else
                        {
                            panApprovalRequired.Visible = true;
                        }
                    }
                }
            }
            else
            {
                // insert the new post
                Fbg.Forum.PostDetails record = new Fbg.Forum.PostDetails(0, DateTime.Now, FbgPlayer.Name, FbgPlayer.Name,
               forumID, "", threadID, title, body, approved, chkClosed.Checked, 0, 0, DateTime.Now, FbgPlayer.Name, chk_Sticky.Checked);
                record.BodyForChat = bodyForChat;
                if (new Fbg.Forum.SqlForumsProvider().InsertPost(record, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr) != 0)
                {
                    panInput.Visible = false;
                    if (approved)
                    {
                        panFeedback.Visible = true;
                    }
                    else
                    {
                        panApprovalRequired.Visible = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            BaseApplicationException bex = new BaseApplicationException("Error while Submiting posts", ex);
            bex.AddAdditionalInformation("forumID", forumID);
            bex.AddAdditionalInformation("isEditingPost", isEditingPost);
            bex.AddAdditionalInformation("threadID", threadID);
            bex.AddAdditionalInformation("postID", postID);
            throw bex;
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

