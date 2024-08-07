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
using System.Collections.Generic;

using System.Net.Mail;
using Gmbc.Common.Diagnostics.ExceptionManagement;
public partial class ClanEmailMembers : MyCanvasIFrameBasePage
{

    Clan myClan;
    int MaxEmailsPerDay;
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
        myClan = FbgPlayer.Clan; // doing this now so that we dont do multiple DB calls
        if (myClan.MemberCount < 5)
        {
            MaxEmailsPerDay = 1;
        }
        else if (myClan.MemberCount < 10)
        {
            MaxEmailsPerDay = 2;
        }
        else
        {
            MaxEmailsPerDay = 5;
        }

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.IsMobile = isMobile;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Email;
        if (isMobile)
        {
            ClanMenu1.Visible = false;
            TxtMessage.Rows = 10;
        }
        lblMaxEmails.Text = EmailLimit_NumberOfEmailsToday(true).ToString();
        if (EmailLimit_NumberOfEmailsToday(true) < 1)
        {
            BtnSend.Enabled = false;
        }

        cbRulesAccepted.CssClass = "";
        lblConfirm.Visible = false;
        lblSendersName.Text = FbgPlayer.Name;

        if (Request.QueryString["messageall"] == "1" && !IsPostBack)
        {
            //
            // send message to all clan members
            //
            DataSet ds = Fbg.Bll.Clan.ViewClanMembers(FbgPlayer, myClan.ID, false);
            string PlayerNames = null;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PlayerNames += dr[Fbg.Bll.Clan.CONSTS.ClanMemberLiteColumnIndex.PlayerName].ToString() + ",";

            }
            TxtTo.Text = PlayerNames;
        }


        lblBanned.Text = IsBannedFromSendingEmails ? "<span class='Error'>Permission Standing: You have lost the right to send emails</span>" : "Permission Standing: You have rights to send emails";
        if (IsBannedFromSendingEmails) { BtnSend.Enabled = false; }



    }

    private bool IsBannedFromSendingEmails
    {
        get
        {
            return !(FbgUser.HasFlag(Fbg.Bll.User.Flags.BannedFromSendingClamEmails) ==null);
        }
    }

    protected void BtnSend_Click(object sender, EventArgs e)
    {
        if (!IsValid)
        {
            return;
        }
        if (!cbRulesAccepted.Checked)
        {
            cbRulesAccepted.CssClass = "Error";
            return;
        }
        if (EmailLimit_NumberOfEmailsToday(true) < 1)
        {
            return;
        }
        if (IsBannedFromSendingEmails)
        {
            return;
        }

        string messageBody = "";
        string subject = "";


        List<string> sentTo_Name  = new List<string>();
        List<string> notSentTo_Names = new List<string>();

        try
        {

            subject = Utils.ClearHTMLCode(TxtSubject.Text.Trim());
            subject = Utils.ClearInvalidChars(subject);
            messageBody = Utils.CleanupInputText(TxtMessage.Text.Trim());
            messageBody = Utils.ClearInvalidChars(messageBody);
            messageBody = Utils.ChangeLineBreaks(messageBody);



            string recipientList = GetRecipientCommaDeliminatedList();
            string[] recipients = recipientList.Split(',');
            PlayerOther po;
            foreach (string recipient in recipients)
            {
                po = PlayerOther.GetPlayer(FbgPlayer.Realm, recipient, FbgPlayer.ID);

                if (po != null && po.ClanID == myClan.ID)
                {
                    if (!sentTo_Name.Contains(recipient))
                    {
                        //
                        // send the email 
                        //
                        sentTo_Name.Add(recipient);
                        SendEmail(po, FbgPlayer, subject, messageBody);
                    }
                }
                else
                {
                    notSentTo_Names.Add(recipient);
                }

            }

            if (sentTo_Name.Count > 0)
            {
                EmailLimit_EmailSent();
                MaxEmailsPerDay = EmailLimit_NumberOfEmailsToday(true);
                lblMaxEmails.Text = MaxEmailsPerDay.ToString();

                //
                // clear the form 
                TxtTo.Text = "";
                TxtSubject.Text = "";
                TxtMessage.Text = "";

            }

            lblConfirm.Text = "Message sent to the best of our abilities - meaning that if, for example, player has chosen not to be notified via email, the player will not receive this email.";
            lblConfirm.Text += "<BR>Sent to:<oL>";
            foreach (string name in sentTo_Name)
            {
                lblConfirm.Text += String.Format("<LI>{0}</li>", name);
            }
            lblConfirm.Text += "</ol>";

            if (notSentTo_Names.Count > 0)
            {
                lblConfirm.Text += "<BR>Not Sent to: (either name not recognized or not in your clan)<oL>";
                foreach (string name in notSentTo_Names)
                {
                    lblConfirm.Text += String.Format("<LI>{0}</li>", name);
                }
                lblConfirm.Text += "</ol>";
            }
            lblConfirm.Visible = true;



        }
        catch (Exception ex)
        {
            BaseApplicationException be = new BaseApplicationException("Error in BtnSend_Click() method", ex);
            throw be;
        }

        
    }


    private string GetRecipientCommaDeliminatedList()
    {
        string to = "";

        to = TxtTo.Text.Replace(";", ",");
        to = to.Replace(" ", ",");

        return to;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="useCache">set true if you want the result to be calculated from the session, pass false if you want the check to be made against the database</param>
    /// <returns></returns>
    public int EmailLimit_NumberOfEmailsToday(bool useCache)
    {
        DateTime now = DateTime.Now;
        object flag = FbgPlayer.HasFlag(Fbg.Bll.Player.Flags.NumEmailsToClamMembersSentToday, useCache);
        if (
            (flag == null) || (flag != null && (DateTime)flag < now && ((DateTime)flag).Day != now.Day)
            )
        {
            return MaxEmailsPerDay;
        }
        else
        {
            if (flag != null)
            {
                object data = FbgPlayer.HasFlag_GetData(Fbg.Bll.Player.Flags.NumEmailsToClamMembersSentToday);
                if (data != null && !(data is DBNull))
                {
                    return MaxEmailsPerDay - Convert.ToInt32(data);
                }
            }
            return MaxEmailsPerDay;
        }
        return MaxEmailsPerDay;
    }

    public void EmailLimit_EmailSent()
    {
        int numLeft = EmailLimit_NumberOfEmailsToday(false);
        if (numLeft > 0)
        {

            FbgPlayer.SetFlag(Fbg.Bll.Player.Flags.NumEmailsToClamMembersSentToday, (MaxEmailsPerDay - numLeft + 1).ToString());

        }
    }


    public  void SendEmail(Fbg.Bll.PlayerOther invitedPlayer
        , Fbg.Bll.Player invitingPlayer, string subject, string body)
    {      
        //
        // send the email 
        //
        try
        {
            MembershipUser user = Membership.GetUser(invitedPlayer.UserID);

            //Utils.SendRealEmail(user.Email, null
            //    , string.Format(R_MiscMessages.GetString("email_EmailFromClanLeader_Subject"), invitingPlayer.Name, subject)
            //    , R_MiscMessages.GetString("email_header") + body + String.Format(R_MiscMessages.GetString("email_footer"), 13));           
        }
        catch (Exception ex)
        {
            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            BaseApplicationException.AddAdditionalInformation(col, "invitingPlayer.Clan.Name", invitingPlayer.Clan.Name);
            invitingPlayer.SerializeToNameValueCollection(col, "invitingPlayer");

            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(new Exception("error while trying to send a an email to clan members from admin", ex), col);
            //
            // we eat the exception on purpose. do not want failure in case of problem here. 
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
