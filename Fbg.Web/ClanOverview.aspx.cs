using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Fbg.Bll;
using Facebook;
using Facebook.WebControls;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class ClanOverview : MyCanvasIFrameBasePage
{

    internal class gvwInvitations
    {
            public static int JoinClan = 2;
            public static int JoinClanbtn = 1;
     
    }
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Overview;
        ClanMenu1.IsMobile = isMobile;

        if (!IsPostBack)
        {
            #region Localize Controls
            //localize controls using <%#...%>
            this.btn_RenameClan.DataBind();
            this.btn_LeaveClan.DataBind();
            this.lnk_DeleteClan.DataBind();
            this.div_invalidChars1.DataBind();
            this.div_invalidChars2.DataBind();
            this.div_invalidChars3.DataBind();
            this.lnk_CreateClan.DataBind();
            #endregion
            

            GridView1.DataSource = Fbg.Bll.Clan.ViewPlayerInvitations(FbgPlayer);
            GridView1.DataBind();

            BindEventsGrid();
        }
        if (FbgPlayer.Clan != null)
        {
            pnl_ClanOverview.Visible = true;
            pnl_CreateClan.Visible = false;
            LnkClanName.Text = FbgPlayer.Clan.Name;
           
                LnkClanName.NavigateUrl = NavigationHelper.ClanPublicProfile(FbgPlayer.Clan.ID);
           

            lbl_ClanDesc.Text = FbgPlayer.Clan.Desc;
            if (!IsPostBack)
            {
                txt_RenameClan.Text = FbgPlayer.Clan.Name;
            }
            if (!FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner))
            {
                lnk_DeleteClan.Visible = false;
                pnl_RenameClan.Visible = false;
            }
            if ((FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)))
            {
                lnk_Edit.Visible = true;
            }
        }
        else
        {
            pnl_ClanOverview.Visible = false;
            pnl_CreateClan.Visible = true;
            lnk_DeleteClan.Visible = false;
            pnl_RenameClan.Visible = false;



        }

    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Join")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).Parent.Parent;
            int ClanID = (int)GridView1.DataKeys[row.RowIndex][0];
            int? prevClanId = null;
            if (FbgPlayer.Clan != null) { prevClanId = FbgPlayer.Clan.ID; }
            Clan.LeaveClanResult leaveResult = Fbg.Bll.Clan.LeaveClan(FbgPlayer);
            if (leaveResult == Clan.LeaveClanResult.OK)
            {
//                if (prevClanId.HasValue) { ChatHub2.ChatHub2.DismissClanChat(FbgPlayer.ID.ToString(), prevClanId.Value, FbgPlayer.Realm.ID.ToString()); }//dismiss player from previous clan
                Fbg.Bll.Clan.JoinResult result= Fbg.Bll.Clan.JoinClan(FbgPlayer, ClanID);
                if (result == Fbg.Bll.Clan.JoinResult.Success)
                {
                    //ChatHub2.ChatUser chatuser = new ChatHub2.ChatUser(FbgPlayer.User.ID, false);

                    //updates the ChatUserEntityPlayer's clan so JoinOrCreate properly adds them to chat
                    //ChatHub2.ChatHub2.addClanToChatUser(FbgPlayer.ID.ToString(), FbgPlayer.Clan.ID, FbgPlayer.Realm.ID.ToString());
                    //ChatHub2.ChatHub2.JoinOrCreateClanChat(FbgPlayer.ID.ToString(), ClanID, FbgPlayer.Realm.ID.ToString()); //add player to clan

                    lbl_Error.Text = "";
                    InvalidateFbgPlayerRoles();
                    PublishJoinedAClan(GridView1.Rows[row.RowIndex].Cells[0].Text);
                    Response.Redirect("ClanOverview.aspx");
                }
                else
                {
                    lbl_Error.Text = GetJoinMessageFromCode(result);//Fbg.Bll.Clan.GetJoinMessageFromCode(result);
                    lbl_Error.Visible = true;
                }
            }
            else if (leaveResult == Clan.LeaveClanResult.Failed_NoClanChangesAllowedAnyLonger)
            {
                lbl_Error.Visible = true;
                lbl_Error.Text = "Leaving and joining clans no longer allowed";
            }
            
            else
            {
                lbl_Error.Visible = true;
                lbl_Error.Text = RS("errorLeaving");
            }
            

        }
        if (e.CommandName == "Cancel")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).Parent.Parent;
            int ClanID = (int)GridView1.DataKeys[row.RowIndex][0];
             Fbg.Bll.Clan.CancelInvitation(FbgPlayer, ClanID);
            lbl_Error.Text = "";
            Response.Redirect("ClanOverview.aspx");

        }
    }
    protected void lnk_CreateClan_Click(object sender, EventArgs e)
    {
        txt_ClanName.Text = Utils.ClearHTMLCode(txt_ClanName.Text);
        txt_ClanName.Text = Utils.ClearInvalidChars(txt_ClanName.Text);
        txt_ClanName.Text = Utils.StripNonAscii(txt_ClanName.Text);
        txt_ClanDesc.Text = Utils.ClearHTMLCode(txt_ClanDesc.Text);
        if (txt_ClanName.Text.Trim() != "")
        {
            if (Fbg.Bll.Clan.CreateNewClan(txt_ClanName.Text, txt_ClanDesc.Text, FbgPlayer) == null)
            {
                lbl_Error.Visible = true;
                lbl_Error.Text = RS("clanExists");
            }
            else
            {
                //updates the ChatUserEntityPlayer's clan so JoinOrCreate properly adds them to chat
                //ChatHub2.ChatHub2.addClanToChatUser(FbgPlayer.ID.ToString(), FbgPlayer.Clan.ID, FbgPlayer.Realm.ID.ToString());
                //ChatHub2.ChatHub2.JoinOrCreateClanChat(FbgPlayer.ID.ToString(), FbgPlayer.Clan.ID, FbgPlayer.Realm.ID.ToString()); //add player to clan chat
                InvalidateFbgPlayerRoles();
                lbl_Error.Text = "";      
                PublishCreateAClan(txt_ClanName.Text);
                Response.Redirect("ClanOverview.aspx");
            }
        }
        else
        {
            RequiredFieldValidator1.IsValid = false;
        }
    }
    protected void btn_LeaveClan_Click(object sender, EventArgs e)
    {
        int clanId = FbgPlayer.Clan.ID;
        Clan.LeaveClanResult leaveResult = Fbg.Bll.Clan.LeaveClan(FbgPlayer);

        if (leaveResult == Clan.LeaveClanResult.Failed)
        {
            lbl_Error.Visible = true;
            lbl_Error.Text = RS("errorLeaving");
        }
        else if (leaveResult == Clan.LeaveClanResult.Failed_NoClanChangesAllowedAnyLonger)
        {
            lbl_Error.Visible = true;
            lbl_Error.Text = "Leaving and joining clans no longer allowed";
        }
        else
        {
            //ChatHub2.ChatHub2.DismissClanChat(FbgPlayer.ID.ToString(), clanId, FbgPlayer.Realm.ID.ToString()); //delete player from clan chat
            InvalidateFbgPlayerRoles();
            InvalidateFbgPlayerClan();
            Response.Redirect("ClanOverview.aspx");
        }

    }
    protected void lnk_DeleteClan_Click(object sender, EventArgs e)
    { 
        if (FbgPlayer.Clan != null)
        {
            int clanId = FbgPlayer.Clan.ID;
            if (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner))
            {
                Fbg.Bll.Clan.DeleteClan(FbgPlayer);
              //  ChatHub2.ChatHub2.DeleteClanChat(FbgPlayer.ID.ToString(), clanId, FbgPlayer.Realm.ID.ToString()); //delete all players from clan chat
                InvalidateFbgPlayerClan();
                Response.Redirect("ClanOverview.aspx");
            }
        }

    }
    
    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (FbgPlayer.Clan != null)
            {
                LinkButton btnJoin = e.Row.Cells[gvwInvitations.JoinClan].Controls[gvwInvitations.JoinClanbtn] as LinkButton;
                string message = RS("joinNewClan");
                btnJoin.OnClientClick = "if (confirm('"+message+"') == false) return false;";
                btnJoin.ToolTip = RS("joinClan_TT");
            }
            
        }

    }
    protected void btn_RenameClan_Click(object sender, EventArgs e)
    {
        txt_RenameClan.Text = Utils.ClearHTMLCode(txt_RenameClan.Text);
        txt_RenameClan.Text = Utils.ClearInvalidChars(txt_RenameClan.Text);
        txt_RenameClan.Text = Utils.StripNonAscii(txt_RenameClan.Text);

        if (!string.IsNullOrEmpty(txt_RenameClan.Text.Trim ()))
        {
            if (Fbg.Bll.Clan.RenameClan(FbgPlayer, txt_RenameClan.Text.Trim()))
            {
                LnkClanName.Text = txt_RenameClan.Text;
                lbl_Error.Visible = false;
            }
            else
            {
                lbl_Error.Visible = true;
                lbl_Error.Text =RS("clanExists");
            }
            
            //Response.Redirect ("ClanOverview.aspx");
        }
    }

    private void PublishJoinedAClan(string clanName)
    {
        if (!isMobile && LoginModeHelper.isFB(Request))
        {
            Response.Redirect(String.Format("allowss.aspx?{0}={1}", CONSTS.QuerryString.StoryToPublish, (int)StoriesToPublish.JoinedClan));
        }
        
        //try
        //{
        //    if (!IsLoggedInAsSteward)
        //    {

        //        if (!string.IsNullOrEmpty(FacebookConfig.StoryTemplate_JoinedClan))
        //        {
        //            LoginToFacebook();

        //            bool success;
        //            Dictionary<String, String> data;
        //            data = new Dictionary<string, string>(1);
        //            data.Add("clanname", clanName);
        //            success = FBService.PublishUserAction(FacebookConfig.StoryTemplate_JoinedClan, data);

        //            Utils.RecordPublishedStoryOrAttempt(FbgPlayer.ID, Fbg.Bll.CONSTS.Stories.ClanJoined, data, success);
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    BaseApplicationException bex = new BaseApplicationException("PublishStory Failed - PublishJoinedAClan",ex);
        //    bex.AddAdditionalInformation("FBGPlayer", FbgPlayer);
        //    bex.AddAdditionalInformation("clanName", clanName);
        //    Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(bex);    
        //    //
        //    // we eat the exception on purpose!
        //    //
        //}
    }
    private void PublishCreateAClan(string clanName)
    {
        if (!isMobile && LoginModeHelper.isFB(Request))
        {
            Response.Redirect(String.Format("allowss.aspx?{0}={1}", CONSTS.QuerryString.StoryToPublish, (int)StoriesToPublish.CreatedClan));
        }
        /*
        try
        {
            if (!IsLoggedInAsSteward)
            {
                if (!string.IsNullOrEmpty(FacebookConfig.StoryTemplate_CreatedClan))
                {
                    LoginToFacebook();

                    bool success;
                    Dictionary<String, String> data;
                    data = new Dictionary<string, string>(1);
                    data.Add("clanname", clanName);
                    success = FBService.PublishUserAction(FacebookConfig.StoryTemplate_CreatedClan, data);

                    Utils.RecordPublishedStoryOrAttempt(FbgPlayer.ID, Fbg.Bll.CONSTS.Stories.ClanCreate, data, success);

                }
            }
        }
        catch (Exception ex)
        {
            BaseApplicationException bex = new BaseApplicationException("PublishStory Failed - PublishCreateAClan", ex);
            bex.AddAdditionalInformation("FBGPlayer", FbgPlayer);
            bex.AddAdditionalInformation("clanName", clanName);
            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(bex);
            //
            // we eat the exception on purpose!
            //
        } */       
    }




    private void BindEventsGrid()
    {
        if (FbgPlayer.Clan != null)
        {
            //Bind Part
            DataSet ds = Fbg.Bll.Clan.GetClanEvents(FbgPlayer, true);

            if (ds != null)
            {
                gvw_Events.DataSource = ds;
                gvw_Events.DataBind();

            }

        }
    }



    protected void lnk_Edit_Click(object sender, EventArgs e)
    {
        //hide edit buton and show textbox
        HandleView(false);
    }
    /// <summary>
    /// this function to handle the Visable of panels and buttons when user do actions like save or edit
    /// </summary>
    /// <param name="Case"></param>
    private void HandleView(bool Case)
    {
        pnl_PublicProfile.Visible = !Case;
        lnk_Edit.Visible = Case;
        lbl_PublicProfile.Visible = Case;
    }
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        string profileText = Utils.ClearHTMLCode(txt_PublicProfile.Text.Trim());
        profileText = Utils.ClearInvalidChars(profileText);
        profileText = BBCodes.PreProcessBBCodes(FbgPlayer.Realm, BBCodes.Medium.ClanPublicProfile, profileText);

        FbgPlayer.Clan.UpdateClanPublicProfile(profileText);

        //hide textbox and return to the frist state
        HandleView(true);
        int clanid = Convert.ToInt32(Request.QueryString["clanid"]);
        Response.Redirect("ClanPublicProfile.aspx?clanid=" + clanid.ToString());
    }

    public string GetJoinMessageFromCode(Clan.JoinResult result)
    {
        string msg = string.Empty;
        switch (result)
        {
            case Clan.JoinResult.Clan_Dont_Exist:
                msg = RS("msg_clanDontExist");
                break;
            case Clan.JoinResult.Player_dont_have_invitation:
                msg = RS("msg_noInvite");
                break;
            case Clan.JoinResult.ClanLimit:
                msg = RS("msg_limitReached");
                break;
            case Clan.JoinResult.ClanChangesNoLongerAllowed:
                msg = "Leaving and joining clans no longer allowed";
                break;
            case Clan.JoinResult.Success:
                break;
            default:
                throw new Exception("Unrecognized value of Clan.JoinResult:" + result.ToString());
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
