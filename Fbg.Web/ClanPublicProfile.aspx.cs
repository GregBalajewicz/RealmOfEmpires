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
using Facebook;
using Facebook.WebControls;
using Gmbc.Common.Diagnostics.ExceptionManagement;
public partial class ClanPublicProfile : MyCanvasIFrameBasePage
{

    public ClanPublicProfile()
    {
        R_OverridePageName = "ClanPublicProfile.aspx";
    }

    internal class DSCONSTS
    {

        public class ClanPublicProfileTableIndex
        {
            public static int Points = 0;
            public static int Members = 1;
            public static int Clan = 2;
        }
    }

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.IsMobile = isMobile;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Public_Profile;
      
        if (!IsPostBack)
        {
            #region Localize Controls
            this.lblPageExplanationToMemebers.DataBind();
            this.div_invalidChars.DataBind();
            this.btn_Save.DataBind();
            #endregion

            seeAllMembers.Visible = !isMobile;

            if (Request.QueryString[CONSTS.QuerryString.ClanID] != null)
            {
                #region  query string exist so looking at someone elses clan
                //
                //query string exist so looking at someone elses clan
                //
                int clanid;
                if (Int32.TryParse(Request.QueryString["clanid"], out clanid))//try to get clan id
                {
                    lnk_Members.NavigateUrl = "~/ClanMembers.aspx?clanid=" + clanid;
                    DataSet ds = Clan.ViewClanPublicProfile(FbgPlayer, clanid);
                    DataTable dt = Fbg.Bll.Stats.GetClanRanking(FbgPlayer.Realm, 0);
                    if (ds != null)
                    {//get clan public profile info

                        if (ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Points].Rows.Count > 0)
                        {
                            lbl_points.Text = Utils.FormatCost((int)ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Points].Rows[0]["villagepoints"]);
                        }
                        if (ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Members].Rows.Count > 0)
                        {
                            lbl_Members.Text = Utils.FormatCost((int)ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Members].Rows[0]["PlayersCount"]);
                        }
                        if (ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows.Count > 0)
                        {
                            lbl_ClanName.Text = ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows[0]["Name"].ToString();
                        }

                        if (dt.Rows.Count > 0)
                        {
                            int ClanIndex = dt.Rows.IndexOf(dt.Rows.Find(clanid));
                            LblRank.Text = Convert.ToString(ClanIndex < 0 ? dt.Rows.Count : ClanIndex + 1);
                        }

                        if (ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows.Count > 0)
                        {
                            lbl_PublicProfile.Text = BBCodes.ClanPublicProfileToHTML(FbgPlayer.Realm, ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows[0]["PublicProfile"].ToString());
                            txt_PublicProfile.Text = BBCodes.CleanUpPreProcessedBBCodes(FbgPlayer.Realm, ds.Tables[DSCONSTS.ClanPublicProfileTableIndex.Clan].Rows[0]["PublicProfile"].ToString());
                        }
                        else
                        {
                            lbl_Message.Text = "The clan does not exist. Perhaps it was disbanded.<br>If you believe this to be a problem please contact support.";
                            lbl_Message.Visible = true;
                            pnl_Profile.Visible = false;
                            //ClanNotFoundException ex = new ClanNotFoundException("Error:Requested Clan ID not Exist",clanid );
                            //throw ex;
                        }
                        if (FbgPlayer.Clan != null)
                        {
                            if (FbgPlayer.Clan.ID == clanid && (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator)))
                            {
                                lnk_Edit.Visible = true;
                                lblPageExplanationToMemebers.Visible = true;
                            }

                        }

                        if (isD2 || isMobile)
                        {                           
                            // Since we're using this only on D2 and Mobile but in a iframe popup, we need to use window.parent
                            // to get to the launch tool. If we move this into regular code, we'll need to remove window.parent.
                            int cid = clanid;
                            if (FbgPlayer.Clan != null && FbgPlayer.Clan.ID == cid) {
                                lbl_clanRequestInvite.Visible = false;
                            } else {
                                lbl_clanRequestInvite.Text = "<a class='requestClanInviteBtn' style='cursor:pointer' onclick=' window.parent.ROE.Frame.launchClanInviteRequestMessage(" + cid + ");'>" + RS("requestInvite") +"</a>";
                            }
                        }


                    }
                    else
                    {
                        BaseApplicationException bex = new BaseApplicationException("Error Getting Clan");
                        bex.AddAdditionalInformation("ClanID", clanid);

                        throw bex;
                    }
                }
                else
                {
                    BaseApplicationException bex = new BaseApplicationException("Error Getting Clan");
                    bex.AddAdditionalInformation("ClanID", clanid);

                    throw bex;
                }
                #endregion
            }
            else
            {
                //
                // no clan id in querry string - looking at my own clan
                //
                               
                // Only do this stuff for D1 and D2
                if (!isMobile)
                {

                    if (FbgPlayer.Clan != null)
                    {
                        Response.Redirect("ClanPublicProfile.aspx?clanid=" + FbgPlayer.Clan.ID.ToString());
                        lnk_Members.NavigateUrl = "~/ClanMembers.aspx?clanid=" + FbgPlayer.Clan.ID.ToString();
                    }
                    else
                    {

                        Response.Redirect("ClanOverview.aspx");
                    }
                } // else  don't redirect, just go to url

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
