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


public partial class PlayerOptions : MyCanvasIFrameBasePage
{

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        if (!IsPostBack)
        {
            chk_Anonymous.Checked = FbgPlayer.Anonymous;


            ddlTimeZone.SelectedValue = FbgUser.TimeZone.ToString();
            lbl_CurrentTime.Text = DateTime.UtcNow.AddHours(FbgUser.TimeZone).ToString("MMMM dd yyyy HH:mm");

            InitAlertsSection();

            InitHQSection();
        }

        #region localzing some controls
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        if (!IsPostBack)
        {
            this.chk_Anonymous.DataBind();
            //this.lnk_Update.DataBind();
        }
        #endregion

        // On VP Realm
        //  OR if player has unlimited build q PF on
        //  then hide the night build mode
        if (FbgPlayer.Realm.IsVPrealm
            || (FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.BuildingQ)))
        {
            rowNBM.Visible = false;
        }
        else
        {
            rowNBM.Visible = true;
            InitNightBuild();
        }
        InitSleepMode();

    }
    private void InitAlertsSection()
    {
        if (IsLoggedInAsSteward)
        {
            panelEmails_NotForStewards.Visible = true;
            panelEmails_NoEmail.Visible = false;
            panelEmails_GotEmail.Visible = false;
        }
        else
        {
            //
            // init the email alerts panel
            //
            if (LoggedInMembershipUser.Email == Fbg.Bll.CONSTS.DummyEmail)
            {
                panelEmails_NoEmail.Visible = true;
                panelEmails_GotEmail.Visible = false;
            }
            else
            {
                panelEmails_NoEmail.Visible = false;
                panelEmails_GotEmail.Visible = true;
                lblEmial_ConfirmationMsg.Text = String.Format(RS("ThankYouForSharingEmail"), LoggedInMembershipUser.Email);

                if (FbgPlayer.OptOutOfEmails)
                {
                    lblEmail_OptOut.Text = RS("WARNINGYouDisabledEmailAlerts");
                    linkEmail_OptOutLink.Text = RS("ClickHereToEnableAlerts");
                }
                else
                {
                    lblEmail_OptOut.Text = RS("YouAreReceivingEmailAlerts");
                    linkEmail_OptOutLink.Text = RS("OptOut");

                }
            }
        }
    }

    //protected void lnk_Update_Click(object sender, EventArgs e)
    //{
    //    FbgPlayer.Update(chk_Anonymous.Checked, (Fbg.Common.Sex)Convert.ToInt16(ddSex.SelectedValue));
    //    FbgUser.Update(Convert.ToSingle( ddlTimeZone.SelectedValue));
    //}
    protected void ddlTimeZone_SelectedIndexChanged(object sender, EventArgs e)
    {
        lbl_CurrentTime.Text = DateTime.UtcNow.AddHours(Convert.ToDouble(ddlTimeZone.SelectedValue)).ToString("MMMM dd yyyy HH:mm");
        FbgUser.Update(Convert.ToSingle(ddlTimeZone.SelectedValue));
    }
    protected void playerOptionChanges(object sender, EventArgs e)
    {
        FbgPlayer.Update(chk_Anonymous.Checked, FbgPlayer.Sex);
    }
    protected void linkNightBuild_Activate_Click(object sender, EventArgs e)
    {
        FbgPlayer.NightBuild_Activate();
        InitNightBuild();
    }

    private void InitNightBuild()
    {
        lblNightBuildMsg.Visible = false;
        lblNightBuild_Countdown.Visible = false;
        linkNightBuild_Activate.Visible = false;
        if (FbgPlayer.NightBuild_IsActive)
        {
            lblNightBuildMsg.Visible = true;
            lblNightBuild_Countdown.Visible = true;
            lblNightBuildMsg.Text = RS("NightBuildIsActive");
            lblNightBuild_Countdown.Text = Utils.FormatDuration(FbgPlayer.NightBuild_ExpiresOn.Subtract(DateTime.Now));
        }
        else
        {
            if (FbgPlayer.NightBuild_CanActivateAgainOn < DateTime.Now)
            {

                linkNightBuild_Activate.Visible = true;
            }
            else
            {
                lblNightBuildMsg.Visible = true;
                lblNightBuild_Countdown.Visible = true;
                lblNightBuildMsg.Text = RS("NightBuildCanBeUsedAgainIn");
                lblNightBuild_Countdown.Text = Utils.FormatDuration(FbgPlayer.NightBuild_CanActivateAgainOn.Subtract(DateTime.Now));
            }
        }
    }

    public void InitSleepMode()
    {

        Fbg.Bll.Realm.SleepMode sleepMode = FbgPlayer.Realm.SleepModeGet;
        if (sleepMode.IsAvailableOnThisRealm) 
        {
            panelSleepModeIsActive.Visible = true;
            panelSleepModeIsNotActive.Visible = false;

            linkActivateSleepMode.Visible = false;
            lblSleepMode_Countdown.Visible = false;
            lblSleepModeActivating.Visible = false;
            lblSleepModeActive.Visible = false;
            lblSleepModeCanBeActivatedIn.Visible = false;

            lblSleepMode.Text = String.Format(R_MiscMessages.GetString("SleepModeExplanation")
                , sleepMode.AavailableOnceEveryXHours, sleepMode.Duration, sleepMode.TimeTillActive*60);

            if (FbgPlayer.SleepMode_IsActiveNow)
            {
                lblSleepModeActive.Visible = true;
                lblSleepMode_Countdown.Visible = true;
                lblSleepMode_Countdown.Text = Utils.FormatDuration(
                    FbgPlayer.SleepMode_ActiveOn.AddHours(sleepMode.Duration).Subtract(DateTime.Now));
            }
            else if (FbgPlayer.SleepMode_ActivatingOn != DateTime.MinValue)
            {
                lblSleepModeActivating.Visible = true;
                lblSleepMode_Countdown.Visible = true;
                lblSleepMode_Countdown.Text = Utils.FormatDuration(FbgPlayer.SleepMode_ActivatingOn.Subtract(DateTime.Now));
            }
            else if (FbgPlayer.SleepMode_CanActivateIn > DateTime.Now)
            {
                lblSleepModeCanBeActivatedIn.Visible = true;
                lblSleepMode_Countdown.Visible = true;
                lblSleepMode_Countdown.Text = Utils.FormatDuration(FbgPlayer.SleepMode_CanActivateIn.Subtract(DateTime.Now));
            }
            else
            {
                linkActivateSleepMode.Visible = true;
                linkActivateSleepMode.Text = String.Format(RS("ClickToActivateSleepMode"), sleepMode.TimeTillActive*60);
            }
        }
        else
        {
            panelSleepModeIsNotActive.Visible = true;
            panelSleepModeIsActive.Visible = false;
        }
    }

    private void InitHQSection()
    {
        if (FbgPlayer.HasFlag(Fbg.Bll.Player.Flags.Misc_AdvancedHQ) != null)
        {
            linkAdvancedHQ.Visible = false;
            lblAdvancedHQModeActive.Visible = true;
        }
        else
        {
            lblAdvancedHQModeActive.Visible = false;
            linkAdvancedHQ.Text = RS("ClickToActivateAdvancedMode");
        }
    }
            //
    protected void linkActivateSleepMode_Click(object sender, EventArgs e)
    {
        FbgPlayer.SleepMode_Activate();
        InitSleepMode();
    }
    protected void lblEmail_OptOutLink_Click(object sender, EventArgs e)
    {
        FbgPlayer.OptOutOfEmails = !FbgPlayer.OptOutOfEmails;
        InitAlertsSection();
    }
    protected void linkAdvancedHQ_Click(object sender, EventArgs e)
    {
        if (FbgPlayer.HasFlag(Fbg.Bll.Player.Flags.Misc_AdvancedHQ) == null)
        {
            FbgPlayer.SetFlag(Fbg.Bll.Player.Flags.Misc_AdvancedHQ);
        }
        InitHQSection();
    }

    


    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }
        base.OnPreInit(e);
    }
}
