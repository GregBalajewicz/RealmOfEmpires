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

public partial class ClanSettings : MyCanvasIFrameBasePage
{
    private DataSet ds;

    private DataSet dsClaimSysSettings;
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        //
        // security check. 
        //
        if (!FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator)
            && !FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner))
        {
            Response.Redirect("ClanOverview.aspx");
        }


        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.IsMobile = isMobile;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Settings;
        if (isMobile)
        {
            ClanMenu1.Visible = false;
        }
        if (!IsPostBack)
        {
            #region Localize Controls
            this.cbRole_Inviter.DataBind();
            this.lnk_Update.DataBind();
            #endregion
            if (FbgPlayer.Clan.GetClanSettings().Tables[0].Rows.Count > 0)
            {
                cbRole_Inviter.Checked = true;
            }
            else
            {
                cbRole_Inviter.Checked = false;
            }
            populateClaimSettings();
        }

      

    }

    protected void populateClaimSettings()
    {
        dsClaimSysSettings = FbgPlayer.Clan.ClaimVillage_GetSettings();
       // dsClaimSysSettings.Tables[0].PrimaryKey = dsClaimSysSettings.Tables[0].Columns[0];

        foreach (DataRow dr in dsClaimSysSettings.Tables[0].Rows)
        {
            switch ((int)dr[0])
            {
                case (int)Clan.ClaimVillage_SettingIDs.detailsViewLevel:
                    cmb_SecurityLevel.ClearSelection();
                    cmb_SecurityLevel.Items.FindByValue(dr[1].ToString()).Selected = true;
                    break;
                case (int)Clan.ClaimVillage_SettingIDs.maxClaims:
                    txtMaxClaims.Text = dr[1].ToString();
                    break;
                case (int)Clan.ClaimVillage_SettingIDs.maxClaimDuration:
                    txtClaimExpiry.Text = dr[1].ToString();
                    break;
                case (int)Clan.ClaimVillage_SettingIDs.sharewith_allies:
                    rbsharewithallies_y.Checked = (int)dr[1] == 1;
                    rbsharewithallies_n.Checked = (int)dr[1] != 1;
                    break;
                case (int)Clan.ClaimVillage_SettingIDs.sharewith_nap:
                    rbsharewithNap_yes.Checked = (int)dr[1] == 1;
                    rbsharewithNap_no.Checked = (int)dr[1] != 1;
                    break;
            }
        }
    }





    protected void lnk_Update_Click(object sender, EventArgs e)
    {
        FbgPlayer.Clan.UpdateClanSettings(cbRole_Inviter.Checked);

        updateClaimSysSetting(Clan.ClaimVillage_SettingIDs.maxClaims, Convert.ToInt32( txtMaxClaims.Text));
        updateClaimSysSetting(Clan.ClaimVillage_SettingIDs.maxClaimDuration, Convert.ToInt32(txtClaimExpiry.Text));
        updateClaimSysSetting(Clan.ClaimVillage_SettingIDs.detailsViewLevel, Convert.ToInt32(cmb_SecurityLevel.SelectedValue));
        updateClaimSysSetting(Clan.ClaimVillage_SettingIDs.sharewith_allies, rbsharewithallies_y.Checked ? 1 :0);
        updateClaimSysSetting(Clan.ClaimVillage_SettingIDs.sharewith_nap, rbsharewithNap_yes.Checked ? 1 : 0);
        
    }


    
    protected void updateClaimSysSetting(Clan.ClaimVillage_SettingIDs settingID, int value)
    {
        FbgPlayer.Clan.ClaimVillage_SaveSettings(settingID, value);
        
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
