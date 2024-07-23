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



    public partial class BrowseThreads : MyCanvasIFrameBasePage
   {
        internal class gvwThreadsColumns
        {

            public class AdminColumnIndex
            {
                public static int MoveThread = 5;
                public static int LockThread = 6;
                public static int DeleteThread = 7;
            }
        }
        protected void Page_Init(object sender, EventArgs e)
      {
       //  gvwThreads.PageSize = Globals.Settings.Forums.ThreadsPageSize;
      }
        protected  int GetPageIndex(int startRowIndex, int maximumRows)
        {
            if (maximumRows <= 0)
                return 0;
            else
                return (int)Math.Floor((double)startRowIndex / (double)maximumRows);
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
            txtConnectionStr.Text = FbgPlayer.Realm.ConnectionStr;
            if (!this.IsPostBack)
            {
               
                BindGrid();

            }
        }
        private void BindGrid()
        {
            
                int forumID= 0;
                if (int.TryParse(this.Request.QueryString["ForumID"],out forumID))
                {

                    GetRightGrid.DataSource = new Fbg.Forum.SqlForumsProvider().GetThreads(forumID, "", GetPageIndex(0, 25), int.MaxValue, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr);
                    GetRightGrid.DataBind();


                    lnkNewThread1.NavigateUrl = string.Format(lnkNewThread1.NavigateUrl, forumID);
                    lnkNewThread2.NavigateUrl = lnkNewThread1.NavigateUrl;

                    //Scurity Part
                    if (!isMobile) {
                        bool Admin = (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner));
                        GetRightGrid.Columns[gvwThreadsColumns.AdminColumnIndex.MoveThread].Visible = Admin;
                        GetRightGrid.Columns[gvwThreadsColumns.AdminColumnIndex.LockThread].Visible = Admin;
                        GetRightGrid.Columns[gvwThreadsColumns.AdminColumnIndex.DeleteThread].Visible = Admin;
                    }

                }
        }
        protected void gvwThreads_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!isMobile) {
                    ImageButton btnClose = e.Row.Cells[gvwThreadsColumns.AdminColumnIndex.LockThread].Controls[0] as ImageButton;
                    btnClose.OnClientClick = "if (confirm('" + RS("sureToClose") + "') == false) return false;";
                    btnClose.ToolTip = "Close this thread";
                    ImageButton btnDelete = e.Row.Cells[gvwThreadsColumns.AdminColumnIndex.DeleteThread].Controls[0] as ImageButton;
                    btnDelete.OnClientClick = "if (confirm('" + RS("sureToDelete") + "') == false) return false;";

                    GetRightGrid.HeaderRow.Cells[3].Text = RS("replies");
                    GetRightGrid.HeaderRow.Cells[4].Text = RS("views");
                }
            }
        }

        protected void gvwThreads_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int forumID = int.Parse(this.Request.QueryString["ForumID"]);
            if (e.CommandName == "Close")
            {
                if (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) 
                    || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) 
                    || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
                {
                    int threadPostID = Convert.ToInt32(
                       GetRightGrid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);

                    new Fbg.Forum.SqlForumsProvider().CloseThread(threadPostID, FbgPlayer.Realm.ConnectionStr);
                    BindGrid();

                }
            }
            else if (e.CommandName == "Move")
            {
                if (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) 
                    || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) 
                    || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
                {
                    int threadPostID = Convert.ToInt32(
                      GetRightGrid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);

                    new Fbg.Forum.SqlForumsProvider().MoveThread(threadPostID, forumID, FbgPlayer.Realm.ConnectionStr);
                }
            }
            else if (e.CommandName == "Read")
            {
                int threadPostID = Convert.ToInt32(e.CommandArgument);
                new Fbg.Forum.SqlForumsProvider().SetThreadAsRead(FbgPlayer.ID, threadPostID, FbgPlayer.Realm.ConnectionStr);
                BindGrid();

            }


        }
        protected void gvwThreads_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int threadPostID = Convert.ToInt32(((GridView)sender).DataKeys[e.RowIndex].Value);
            if (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) 
                || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) 
                || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
            {
                new Fbg.Forum.SqlForumsProvider().DeletePost(threadPostID,FbgPlayer.ID , FbgPlayer.Realm.ConnectionStr);
                BindGrid();
            }
            
        }

        protected string BindStyle(object dataItem)
        {


            if (HasPostChanges(dataItem))
            {
                return "NewReport thread";
            }
            else
            {
                return "thread";
            }
        }

        protected bool HasPostChanges(object dataItem)
        {
            if (FbgPlayer.Name.Trim() != DataBinder.Eval(dataItem, "LastPostBy").ToString().Trim())
            {
                if (DataBinder.Eval(dataItem, "IsViewed") != DBNull.Value && DataBinder.Eval(dataItem, "IsViewed") != "")
                {
                    bool _isviewed = (bool)DataBinder.Eval(dataItem, "IsViewed");

                    if (_isviewed)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        protected bool IsPostClosed(object dataItem)
        {
            bool isClosed = (bool)DataBinder.Eval(dataItem, "Closed");

            if (isClosed)
            {
                return true;
            }
            return false;

        }
        protected void gvwThreads_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GetRightGrid.PageIndex = e.NewPageIndex;
            BindGrid();
        }



        public GridView GetRightGrid
        {
            get
            {
                if (isMobile) {
                    return gvwThreads_m;
                }
                else {
                    return gvwThreads;
                }
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

