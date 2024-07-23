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
using System.Security.Principal;
using System.Text.RegularExpressions;
using Fbg.Bll;
using System.Collections.Generic;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Fbg.DAL;
using Org.BouncyCastle.Ocsp;
using Fbg.Common.DataStructs;
using Fbg.Common;
using System.Text;

public partial class Create : MyCanvasIFrameBasePage
{
    public Fbg.Bll.Realm realm;
    int realmID;
    int invitationID = int.MinValue;

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        #region Localize Controls
        this.Label2.DataBind();
        this.RequiredFieldValidator1.DataBind();
        this.RegularExpressionValidator1.DataBind();
        this.btnGo.DataBind();
        this.btnGoLink.DataBind();

        //localize item lists
        DropDownList1.Items[0].Text = RS("li_Random");
        DropDownList1.Items[1].Text = RS("li_NE");
        DropDownList1.Items[2].Text = RS("li_SE");
        DropDownList1.Items[3].Text = RS("li_SW");
        DropDownList1.Items[4].Text = RS("li_NW");
        #endregion

        if (isMobile)
        {
            btnGo.Visible = false;
            btnGoLink.Visible = true;
            txtNickName.MaxLength = 15;
        }
        else
        {
            btnGo.Visible = true;
            btnGoLink.Visible = false;
            txtNickName.MaxLength = 25;
        }



        NewPlayerIntro1.Visible = false;
        Tutorial1.Hide();
        //
        // get the realm to register to 
        // 
        realmID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RealmID]);
        realm = Realms.Realm(realmID);
        if (realm == null)
        {
            ExceptionManager.Publish(new Exception(String.Format("UNREPORTED EXCEPTION: realm ID {0} not found. UserID={1}", realmID, FbgUser.ID)));
            Response.Redirect("ChooseRealm.aspx");
        }

        //
        // is this person already registred at this realm???
        //
        if (FbgUser.PlayerByRealmID(realmID) != null)
        {
            Response.Redirect("ChooseRealm.aspx");
        }
        //
        // vip only realm? 
        //
        if (realm.isLimitAccessToVIPsOnly && !FbgUser.VIP_isVIP)
        {
            Response.Redirect("ChooseRealm.aspx");
        }
        //
        // is the person accepting an invitation?
        //
        if (!string.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.InviteID]))
        {
            invitationID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.InviteID]);
        }

        if (realm.IsTemporaryTournamentRealm)
        {
            tournamentRCOnfirm.Visible = true;
        }

        if (realm.EntryCost > 0)
        {
            int userscredits = FbgUser.Credits;
            lblEntryFee.Text = String.Format("I agree to pay the non refundable {0} servants realm entry fee (you have {1})", realm.EntryCost, userscredits);
            entryFee.Visible = true;

            if (realm.EntryCost > userscredits)
            {
                CustomValidator_entryFee_noservants.IsValid = false;
                btnGo.Enabled = false;
            }
        }



        //
        // check if this person can register at this realm. 
        //
        Utils.LoginLinkType loginLinkType;
        Utils.GetLoginLink(FbgUser, realm, out loginLinkType, Context.User.IsInRole("Admin") || Context.User.IsInRole("tester"), isDevice, LoggedInMembershipUser.CreationDate);
        if (loginLinkType != Utils.LoginLinkType.PreRegister
            && loginLinkType != Utils.LoginLinkType.Register)
        {
            //
            // if this realm is not in REGISTER or PREREGISTER 'state' for this user, 
            //  we normally do not allow him to entet but there are certain exceptions to this. 
            //  For example, to many realms, we do allow entry for invited players. 
            //
            //      SECURITY PROBLEM!!! we don't validate this is a valid invite. meaning if player manipulates 
            //      a url and adds an invite param, he will be able to enter. 
            //
            if (loginLinkType == Utils.LoginLinkType.NoEntry_NeededTitleNotReached)
            {
                //
                // now we know this player does not normally have an option to register or preregister at this realm
                //  so good chance he should not be here. Lets check one more condition. 
                //      lets see if perhaps this is an exclusive realm but player has an invite. 
                if (invitationID == Int32.MinValue)
                {
                    Response.Redirect("ChooseRealm.aspx");
                }
            }
            else if (loginLinkType == Utils.LoginLinkType.NoEntry_NotANewPlayer
                || loginLinkType == Utils.LoginLinkType.NoEntry_RealmClosedToNewPlayers)
            {
                //
                // allow an invited person to enter a close realm. 
                //
                if (invitationID == Int32.MinValue)
                {
                    Response.Redirect("ChooseRealm.aspx");
                }
            }
            else
            {
                Response.Redirect("ChooseRealm.aspx");
            }
        }

        //
        // hide start in quadrant if 
        //  * this realm does not allow it OR 
        //  * if player accepted an invite. OR
        //  * player is new
        //
        if (realm.VillagePlacementAlgorithmVersion < 3
            || invitationID != Int32.MinValue
            || (FbgUser.Players.Count == 0 && FbgUser.HasFlag(Fbg.Bll.User.Flags.Misc_RegistredAtARealm) == null)
            || realm.IsTemporaryTournamentRealm
            )
        {
            panelStartIn.Visible = false;
        }

        //
        // show realm info if player not registering at his first realm
        //
        if (FbgUser.Players.Count > 0 && !String.IsNullOrEmpty(realm.ExtendedDesc.Trim()))
        {
            lblRealmInfo.Text = realm.ExtendedDesc;
            pnlRealmInfo.Visible = true;
        }


        if (!IsPostBack)
        {
            txtNickName.Text = FbgUser.GlobalPlayerName;
        }

        Utils.PreventDoubleSubmit(this, btnGo);
        Utils.PreventDoubleSubmit(this, btnGoLink, "");
        Page.Form.DefaultButton = btnGo.UniqueID;

        ShowPasswordPannelIfThisRealmIsPrivate();

        //
        // if we got the nick name in the session from the intro, then try to register player 
        //  right away
        //
        if (!IsPostBack && Session[CONSTS.Session.NickNameFromIntro] != null)
        {
            txtNickName.Text = (string)Session[CONSTS.Session.NickNameFromIntro];
            Session.Remove(CONSTS.Session.NickNameFromIntro);
            if (!String.IsNullOrEmpty(txtNickName.Text.Trim()))
            {
                Register();
            }
        }
    }

    private void ShowPasswordPannelIfThisRealmIsPrivate()
    {
        // make the password pannel visible if this realm is private 
        if (realm.PlayerGenerated.IsPlayerGenerated)
        {
            if (realm.PlayerGenerated.Private.isPrivate)
            {
                pnlPassword.Visible = true;
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {

        Register();
    }

    public void Register()
    {
        this.Validate();
        if (!IsValid) { return; }

        txtNickName.Text = Utils.ClearHTMLCode(txtNickName.Text);

        if (Utils.IsBannedName(txtNickName.Text))
        {
            lbl_Error.Visible = true;
            lbl_Error.Text = "<br/>" + RS("lbl_OffensiveName");
            return;
        }

        if (!IsPasswordValidForPrivateRealm())
        {
            return;
        }

        bool isNewPlayerInvitedToClan;
        int invitingPlayerID;
        Fbg.Common.StartInQuadrants startInQuadrant = Fbg.Common.StartInQuadrants.NoneSelected;

        //
        // starting in some quadrant?
        //
        if (panelStartIn.Visible == true)
        {
            startInQuadrant = (Fbg.Common.StartInQuadrants)Convert.ToInt32(DropDownList1.SelectedValue);
        }

        if (!cbAgreeToTOU.Checked)
        {
            //lblAgreeToTOU.Visible = true;
            CustomValidator_TOU.IsValid = false;
        }
        else if (!cbNoSharing.Checked && realm.IsTemporaryTournamentRealm)
        {
            //lblAgreeToTOU.Visible = true;
            CustomValidator_Sharing.IsValid = false;
        }
        else if (realm.EntryCost > 0 && realm.EntryCost > FbgUser.Credits)
        {
            CustomValidator_entryFee_noservants.IsValid = false;
            btnGo.Enabled = false;
        }
        else if (realm.EntryCost > 0 && !cbEntryFee.Checked)
        {
            CustomValidator_entryFee.IsValid = false;
        }
        else
        {

            int PlayerID;
            Profile.FName = LoggedInFacebookUser.FirstName;
            Profile.LName = LoggedInFacebookUser.LastName;
            if (realm.Population < realm.MaxPlayers)
            {

                PlayerID = realm.RegisterPlayer((Guid)LoggedInMembershipUser.ProviderUserKey
                    , txtNickName.Text.Trim()
                    , LoggedInFacebookUser.UserId
                    , Utils.ConvertGender(LoggedInFacebookUser.Sex)
                    , invitationID == int.MinValue ? null : (object)invitationID
                    , out isNewPlayerInvitedToClan
                    , out invitingPlayerID
                    , startInQuadrant
                    , Session[CONSTS.Session.VillageNameFromIntro] == null ? String.Empty : (string)Session[CONSTS.Session.VillageNameFromIntro]
                    , AvatarIDFromSession()

                    // HACK 
                    //  if a kong player is on mobile, they are doing it via tactica. So, we cannot update the login type, as this will 
                    //      change their login type to tactica, which technically, they are not. 
                    , isMobile && FbgUser.LoginType_isKong ? Fbg.Common.UserLoginType.Kong : Utils.getPlayerLoginType(Request)
                    );

                FbgUser.Items2_Inalidate(); // because registration could have given the user some items forthis realms

                if (PlayerID == 0)
                {
                    lbl_Error.Visible = true;
                    lbl_Error.Text = "<BR>" + RS("lbl_NameUsed");
                }
                else if (PlayerID == -1)
                {
                    Response.Redirect("MaxPlayersReached.aspx");
                }
                else
                {
                    //
                    // charge the entry fee if there is one
                    //
                    if (realm.EntryCost > 0)
                    {
                        FbgUser.PayRealmEntryFee(realm);

                    }
                    //
                    // note the event for analytics
                    //
                    //AnalyticsEvent ae = new AnalyticsEvent() { EventName = "RegOnRealm" };
                    //ae.attribs.Add(new AnalyticsEvent.Attrib("RealmID", realmID.ToString()));
                    //Utils.Analytics_PendingEvent_add(Session, ae);

                    // We don't need to trigger this event later, it will happen right now.
                    // No need to use pending event or storing it in a session to send later.
                    if (Config.CollectAnalyticsOnThisRealm(realmID) || realmID == -1)
                    {
                        Dictionary<string, object> eventProps = new Dictionary<string, object>();
                        eventProps["RealmID"] = realmID.ToString();
                        Utils.Analytics_SendEvent(FbgUser, "RegOnRealm", eventProps);
                    }

                    //
                    //
                    FbgUser.SetFlag(Fbg.Bll.User.Flags.Misc_RegistredAtARealm);

                    //
                    // if this is the first realm this user registered on, then 
                    //  present him with the quick intro, otherwise, if player has only 1 player active, with the tutorial. 
                    //
                    if (FbgUser.Players.Count < 1)
                    {
                        NewPlayerIntro1.Start();
                    }
                    else if (FbgUser.Players.Count < 2)
                    {
                        Tutorial1.CreateRunningCookieFromStart();
                    }
                    //
                    // cant remeber why we do this... 
                    //
                    InvalidateFbgUser();


                    //PublishJoinedRealmStory(PlayerID);

                    //
                    // put out a cookie that will collapse the quests at start
                    HttpCookie questsCookie;
                    questsCookie = new HttpCookie(PlayerID.ToString() + global::CONSTS.Cookies.Quests, "1");
                    questsCookie.Expires = DateTime.Now.AddDays(2);
                    Response.Cookies.Add(questsCookie);

                    //
                    // if this registration is for an accepted invite, send a message to inviter
                    //
                    if (invitationID != int.MinValue
                        && invitingPlayerID != 0)
                    {
                        SendMessage(PlayerID, invitingPlayerID, LoggedInFacebookUser.Name, txtNickName.Text.Trim(), isNewPlayerInvitedToClan);
                    }
                    SendWelcomMessageOnNoobRealm(PlayerID, txtNickName.Text.Trim());

                    string loginUrl;

                    if (invitationID != int.MinValue
                        && invitingPlayerID != 0)
                    {
                        loginUrl = String.Format("LoginToRealm.aspx?{0}={1}&{2}={3}&{4}={5}&new=1"
                            , CONSTS.QuerryString.RealmID
                            , realm.ID
                            , CONSTS.QuerryString.PlayerID
                            , PlayerID
                            , CONSTS.QuerryString.InviteID // include this to tell the login screen the person accepted an invite
                            , invitingPlayerID);
                    }
                    else
                    {
                        loginUrl = String.Format("LoginToRealm.aspx?{0}={1}&{2}={3}&new=1"
                            , CONSTS.QuerryString.RealmID
                            , realm.ID
                            , CONSTS.QuerryString.PlayerID
                            , PlayerID);
                    }


                    CreateNPC();


                    Response.Redirect(loginUrl, false);
                }
            }
            else
            {
                Response.Redirect("MaxPlayersReached.aspx?rid=" + realmID.ToString());

            }
        }
    }

    private void CreateNPC()
    {
        
    }
   

    private bool IsPasswordValidForPrivateRealm()
    {
        if (realm.PlayerGenerated.IsPlayerGenerated && realm.PlayerGenerated.Private.isPrivate)
        {
            if (txtPassword.Text.Trim() != realm.PlayerGenerated.Private.EntryPasscode)
            {
                CustomValidator_password.IsValid = false;
                return false;
            }
        }
        return true;
    }


    public int AvatarIDFromSession()
    {
        try
        {

            if (Session[CONSTS.Session.fbgAvatar] == null || String.IsNullOrEmpty((string)Session[CONSTS.Session.fbgAvatar]))
            {
                return 1;
            }

            return Convert.ToInt32(Session[CONSTS.Session.fbgAvatar]);
        }
        catch { }

        return 1;
    }

    private void SendMessage(int newPlayerID, int inviterPlayerID, string newPlayerRealName, string newPlayerNick, bool isNewPlayerInvitedToClan)
    {
        Fbg.Bll.PlayerOther po = Fbg.Bll.PlayerOther.GetPlayer(realm, inviterPlayerID, newPlayerID);
        Fbg.Bll.Mail.sendEmail(newPlayerID, inviterPlayerID.ToString()
            , String.Format(R_MiscMessages.GetString("email_InviteAccepted_Subject"), newPlayerRealName)
            , String.Format(isNewPlayerInvitedToClan ? R_MiscMessages.GetString("email_InviteAccepted1_body") : R_MiscMessages.GetString("email_InviteAccepted2_body")
            , po.PlayerName, newPlayerRealName, newPlayerNick)
            , po.PlayerName, realm.ConnectionStr);
    }
    private void SendWelcomMessageOnNoobRealm(int newPlayerID, string newPlayerNick)
    {
        if (realm.RealmType_isNoob)
        {
            // if player registered less than 
            if (LoggedInMembershipUser.CreationDate < DateTime.Now.AddDays(-14))
            {
                Fbg.Bll.Mail.sendEmail(Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerId(realm), newPlayerID.ToString()
                    , String.Format("Welcome back {0}!", newPlayerNick)
                    , String.Format("Welcome back {0}!<BR><BR> Wondering what has changed?<BR><BR>We have a <a href='http://realmofempires.blogspot.ca/2017/05/what-had-changed-guide-for-veterans.html'>guide</a>, specially for vets, that runs you through what has changed in ROE in the last little while.<BR><BR>Happy hunting!", newPlayerNick)
                , newPlayerNick, realm.ConnectionStr);
            }
        }
    }

}
