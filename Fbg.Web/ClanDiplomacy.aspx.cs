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

public partial class ClanDiplomacy : MyCanvasIFrameBasePage
{


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
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Diplomacy;

        if (!IsPostBack)
        {
            #region Localize Controls
            this.btn_Enemy.DataBind();
            this.btn_Ally.DataBind();
            this.btn_NAP.DataBind();
            #endregion
            BindGrids();  
        }
    }

    private void BindGrids()
    {
        if (FbgPlayer.Clan != null)
        {
            //get all Diplomacies 
            Fbg.Bll.ClanDiplomacy clanDiplomacy = Fbg.Bll.Clan.ViewMyClanDiplomacy(FbgPlayer);


            grv_Allies.DataSource = clanDiplomacy.GetAllies();
            grv_Allies.DataBind();

            grv_Enemies.DataSource = clanDiplomacy.GetEnemies();
            grv_Enemies.DataBind();


            grv_NAP.DataSource = clanDiplomacy.GetNAP();
            grv_NAP.DataBind();


            bool Admin = (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Diplomat));
            grv_Enemies.Columns[1].Visible = Admin;
            grv_Allies.Columns[1].Visible = Admin;
            grv_NAP.Columns[1].Visible = Admin;
            pnl_CreateDiplomacy.Visible = Admin;

        }
    }

   
  
    protected void gdv_Enemies_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            ImageButton btnDelete = e.Row.Cells[1].Controls[0] as ImageButton;
            btnDelete.OnClientClick = "if (confirm('" + RS("areYouSure") + "') == false) return false;";//"if (confirm('Are you sure you want to delete this diplomatic status?') == false) return false;";

        }
    }
    protected void grv_Allies_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            ImageButton btnDelete = e.Row.Cells[1].Controls[0] as ImageButton;
            btnDelete.OnClientClick = "if (confirm('" + RS("areYouSure") + "') == false) return false;";

        }

    }
    protected void grv_NAP_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            ImageButton btnDelete = e.Row.Cells[1].Controls[0] as ImageButton;
            btnDelete.OnClientClick = "if (confirm('" + RS("areYouSure") + "') == false) return false;";

        }
    }
    protected void btn_Enemy_Click(object sender, EventArgs e)
    {
        InsertClanDiplomacy(Clan.Diplomacy.Enemy);
    }
    protected void btn_Ally_Click(object sender, EventArgs e)
    {

        InsertClanDiplomacy(Clan.Diplomacy.Ally);
    }
    protected void btn_NAP_Click(object sender, EventArgs e)
    {
        InsertClanDiplomacy(Clan.Diplomacy.NAP);
       
    }
    private void InsertClanDiplomacy(Clan.Diplomacy Diplomacy)
    {
        txt_ClanName.Text = Utils.ClearHTMLCode(txt_ClanName.Text);
        if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Diplomat))
        {
            Clan.DiplomacyResult result = Fbg.Bll.Clan.AddDiplomacy(FbgPlayer, txt_ClanName.Text.Trim(), Diplomacy);

            if (result != Clan.DiplomacyResult.Success)
            {
                lbl_Error.Visible = true;
                lbl_Error.Text = GetMessage(result);//Fbg.Bll.Clan.GetMessageFromCode(result); //GetMessage(result);
            }
            else
            {
                Response.Redirect("ClanDiplomacy.aspx");
            }
        }
    }
    protected void grv_Enemies_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (((GridView)sender).DataKeys.Count > 0 )
        {
            string OtherClanName = ((GridView)sender).DataKeys[e.RowIndex].Value.ToString();
            if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Diplomat))
            {

                Fbg.Bll.Clan.DeleteDiplomacy(FbgPlayer, OtherClanName, Clan.Diplomacy.Enemy);
                Response.Redirect("ClanDiplomacy.aspx");
            }
        }
    }
    protected void grv_Allies_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (((GridView)sender).DataKeys.Count > 0)
        {
            string OtherClanName = ((GridView)sender).DataKeys[e.RowIndex].Value.ToString();
            if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Diplomat))
            {

                Fbg.Bll.Clan.DeleteDiplomacy(FbgPlayer, OtherClanName, Clan.Diplomacy.Ally);
                Response.Redirect("ClanDiplomacy.aspx");
            }
        }
    }
    protected void grv_NAP_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (((GridView)sender).DataKeys.Count > 0)
        {
            string OtherClanName = ((GridView)sender).DataKeys[e.RowIndex].Value.ToString();
            if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator) || FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Diplomat))
            {
                Fbg.Bll.Clan.DeleteDiplomacy(FbgPlayer, OtherClanName, Clan.Diplomacy.NAP);
                Response.Redirect("ClanDiplomacy.aspx");
            }
        }
    }
    protected string GetMessage(Clan.DiplomacyResult result)
    {
        string msg = string.Empty;
        switch (result)
        {
            case Clan.DiplomacyResult.Clan_Dont_Exist :
                msg = RS("clanNotFound"); 
                break;
            case Clan.DiplomacyResult.Diplomacy_Already_Exist :
                msg = RS("relAlreadyExists");
                break;
            case Clan.DiplomacyResult.This_is_your_clan:
                msg = RS("thisIsYourClan");
                break;

            case Clan.DiplomacyResult.Success:
                break;
            default:
                throw new Exception("Unrecognized value of Clan.DiplomacyResult:" + result.ToString());
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
