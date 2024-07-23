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

public partial class ClanClaims : MyCanvasIFrameBasePage
{
    private DataSet ds;
    new protected void Page_Load(object sender, EventArgs e)
    {

        base.Page_Load(sender, e);

        //
        // make sure player is in clan
        if (FbgPlayer.Clan == null)
        {
            Response.Redirect("ClanOverview.aspx");
        }

        

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.IsMobile = isMobile;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Claims;
        if (isMobile)
        {
            ClanMenu1.Visible = false;
        }

        //// only inviter or higher has access to the claims clan UI
        //if (!(FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner)
        //    || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator)
        //    || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator)
        //    || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Inviter))
        //    )
        //{
        //    lbl_Error.Text = "No permission to see clan wide claims";
        //    lbl_Error.Visible = true;
        //    ui.Visible = false;
        //    return;
        //}


       // ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Claims;
        aid.Text = FbgPlayer.ID.ToString();
        av.Text = FbgPlayer.Clan.ID.ToString();
        ClaimedPlayerID.Text = Request.QueryString["ClaimedVillageOwnerID"];
        ClaimedByPlayerID.Text = Request.QueryString["ClaimedByPlayerID"];


        SqlDataSource1.ConnectionString = FbgPlayer.Realm.ConnectionStr;
        SqlDataSource2.ConnectionString = FbgPlayer.Realm.ConnectionStr;
        SqlDataSource3.ConnectionString = FbgPlayer.Realm.ConnectionStr;
        SqlDataSource4.ConnectionString = FbgPlayer.Realm.ConnectionStr;


        BindGrid();
    }


    public bool isInFocusOnPlayerMore
    {
        get
        {
            return !string.IsNullOrEmpty(ClaimedPlayerID.Text);
        }
    }

    public bool isInFocusByPlayerMore
    {
        get
        {
            return !string.IsNullOrEmpty(ClaimedByPlayerID.Text);
        }
    }
    private void BindGrid()
    {
        if (FbgPlayer.Clan != null)
        {
            if (isInFocusOnPlayerMore)
            {
                if (isMobile)
                {
                    GridView_focus_m.DataBind();
                    GridView_focus.Visible = false;
                }
                else
                {
                    GridView_focus.DataBind();
                    GridView_focus_m.Visible = false;
                }
                normalview_notFocusOn.Visible = false;
             
                focusOnPlayerArea_CliamsByPlayer.Visible = false;            }
            else if (isInFocusByPlayerMore)
            {


                if (isMobile)
                {
                    GridView_focusByPlayer_m.DataBind();
                    GridView_focusByPlayer.Visible = false;
                }
                else
                {
                     GridView_focusByPlayer.DataBind();
                     GridView_focusByPlayer_m.Visible = false;
                }
               


               
                normalview_notFocusOn.Visible = false;
                focusOnPlayerArea.Visible = false;
            }
            else
            {
                if (isMobile)
                {
                    GridView1_m.DataBind();
                    GridView2_m.DataBind();
                    GridView1.Visible = false;
                    GridView2.Visible = false;
                }
                else
                {
                    GridView1.DataBind();
                    GridView2.DataBind();
                    GridView1_m.Visible = false;
                    GridView2_m.Visible = false;
                }
                GridView2.DataBind();
                focusOnPlayerArea.Visible = false;
                focusOnPlayerArea_CliamsByPlayer.Visible = false;
                if (!(FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)))
                {
                    // only admins and owners can delete claims
                    GridView1.Columns.RemoveAt(0);
                    GridView2.Columns.RemoveAt(0);
                }
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
