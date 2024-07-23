using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Xml;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using Facebook.Entity;
using Fbg.Bll;


public partial class LoginToRealm : MyCanvasIFrameBasePage 
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        bool publishVillageTakenoverStory = false;        
        bool isPlayerNew = false; //tell you if player has just registered
        base.Page_Load(sender, e);
        Session["Researchers"] = null; // make sure the researchers are refreshed between realm changes

        int pid = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.PlayerID]);
        Fbg.Bll.Player p = FbgUser.PlayerByID(pid, true);
        if (!IsThisAdminLoggedInAsThisPlayer)
        {
            p.UpdateLastActivity = true;
        }
        if (p == null)
        {
            Response.Redirect("ChooseRealm.aspx");
        }
        else
        {
            FbgPlayer = p;
        }

        // if logging in as steward, get the steward record ID and if all is well, change the FBGUser and FBGPlayer to be that of the 
        //  account you are stewarding. 
        if (HaveLoginAsStewardCookie)
        {
            LoginAsSteward();
        }


        
        if (FbgPlayer.IsSuspended && !Context.User.IsInRole("Admin"))
        {
            Response.Redirect("suspension.aspx", true);
            return;
        }
        //
        // if realm is not yet opened, than kick the player out. 
        //  invalidate the FBGPlayer object to make sure the player cannot login to this realm. 
        //
        if (DateTime.Now < FbgPlayer.Realm.OpenOn)
        {
            int realmID = FbgPlayer.Realm.ID;
            InvalidateFbgPlayer();
            Response.Redirect(String.Format("RealmOpening.aspx?{0}={1}&{2}={3}"
                , CONSTS.QuerryString.RealmID, realmID
                , CONSTS.QuerryString.PlayerID, pid), true);
        }
        //
        // if realm is mob app only and player not on mob device
        //  invalidate the FBGPlayer object to make sure the player cannot login to this realm. 
        //
        if (( FbgPlayer.Realm.AccessDeviceTypeLimitation == Realm.AccessDeviceTypeLimitations.MobDevicesOnly && !isDevice)
            && !( Context.User.IsInRole("Admin") || Context.User.IsInRole("tester")) 
            )
        {
            int realmID = FbgPlayer.Realm.ID;
            InvalidateFbgPlayer();
            Response.Redirect("ChooseRealm.aspx", true);
        }
        isPlayerNew = Request.QueryString["new"] == "1";
        //
        // write the cookie that will be used to remember the player ID. 
        //  this cookie will be used to recover the player when session expires
        //
        Response.Cookies.Add(new HttpCookie(CONSTS.Cookies.PlayerID, pid.ToString()));

        //
        // if player has active steward present, we do not want to allow him to login but he must end stewardship first
        //
        if (FbgPlayer.Stewardship_ActiveStewardPlayerID != null && !IsLoggedInAsSteward)
        {
            Response.Redirect("AccountStewards_DeactivateBeforeLogin.aspx", true);
        }

        //
        // Log the login 
        //
        try
        {
            if (!IsLoggedInAsSteward && !IsThisAdminLoggedInAsThisPlayer)
            {
                long remotePort = 0;
                Int64.TryParse(Request.ServerVariables["REMOTE_PORT"], out remotePort);
                if (Fbg.Bll.utils.LogLogin(FbgPlayer
                    , Request.ServerVariables["REMOTE_ADDR"]
                    , remotePort
                    , Request.ServerVariables["HTTP_USER_AGENT"]))
                {
                    // PublishVillageTakeoverStory();
                    publishVillageTakenoverStory = true;
                }
            }
        }
        catch (Exception ex)
        {
            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            BaseApplicationException.AddAdditionalInformation(col, "FbgUser", FbgUser);
            BaseApplicationException.AddAdditionalInformation(col, "LoggedInFacebookUser", LoggedInFacebookUser);
            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(new Exception("error while trying to Log the login ", ex), col);
            //
            // we eat the exception on purpose. do not want failure if login cannot be logged. 
        }

        //
        // get player's villages.
        //  if no villages, that means player lost his last village so give him a new one. 
        //  (unless this is a closed realm)
        //
        //if (FbgPlayer.NumberOfVillages == 0)        
        if (MyVillages.Count == 0)
        {
            if (FbgPlayer.Realm.IsOpen == Realm.RealmState.Running_ClosedToNewPlayer)
            {
                //
                // no respan in a closed realm. 
                //
                string redirectToUrl = String.Format("LostLastVilInClosedRealm.aspx?{0}={1}", CONSTS.QuerryString.RealmID, FbgPlayer.Realm.ID );
                InvalidateFbgPlayer(); // this is probably unnecessary since we do redirect so cookies are not stored anyway but still...
                InvalidateFbgUser();
                Response.Redirect(redirectToUrl, true); // we force end response on purpose. we dont want any cookies saved
            }
            // do not create a village for special players except roe_team
            if (!Fbg.Bll.utils.IsSpecialPlayer(FbgPlayer.User.ID)
                || FbgPlayer.User.ID == Fbg.Bll.CONSTS.SpecialPlayers.roe_team_UserID)
            {
                Response.Redirect("ChooseQuad.aspx?rid=" + Request.QueryString["Rid"]);
                //FbgPlayer.FoundAVillage();
                //MyVillagesInvalidate();
            }
        }

        if (!IsLoggedInAsSteward)
        {
          //  DoSomeFacebookStuff(FbgPlayer);
            //
            // if we got a list of invited people in session then record the invite/gift
            //  see fbInviteCallBack.aspx for more explanation
            //
            if (Session[global::CONSTS.Session.InvitedFBIds] != null)
            {
                FbgPlayer.Invites_RegisterInvited(Session[global::CONSTS.Session.GiftID]
                    , Session[global::CONSTS.Session.InvitedFBIds]);
                
                Session.Remove(global::CONSTS.Session.InvitedFBIds); //critical! must be done
                Session.Remove(global::CONSTS.Session.GiftID);
            }
        }
        //
        // if this is a new player, then message all his friends
        //  (except the friend who's invite the new player acceted (if invite was accepted))
        //  informing his friends that their friend just entered the game. 
        //
        #region if this is a new player, then message all his friends
        try
        {
            if (isPlayerNew)
            {
                int invitingPlayerID = 0;
                if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.InviteID]))
                {
                    Int32.TryParse(Request.QueryString[CONSTS.QuerryString.InviteID], out invitingPlayerID);
                }

                foreach (DataRow dr in FbgPlayer.Friends.Rows)
                {
                    if ((int)dr[Fbg.Bll.Player.CONSTS.PlayerFriendsColName.FriendPlayerID] != invitingPlayerID)
                    {
                        SendMessage(FbgPlayer, (int)dr[Fbg.Bll.Player.CONSTS.PlayerFriendsColName.FriendPlayerID], LoggedInFacebookUser.Name);
                    }
                }
            }
        }
        catch (System.Threading.ThreadAbortException) { }
        catch (Exception ex)
        {
            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(
                new FacebookConnectionException("Error while messaging friends of new player", ex)
                , col);
            //
            // we eat the exception on purpose 
            //
        }
        #endregion

        //
        // see if this player needs to get the email about the servant offer
        //
        SendMessageAboutOfferIfNeeded();


        if (publishVillageTakenoverStory && !isMobile && LoginModeHelper.isFB(Request))
        {
            //
            // ask player to pust story about his new village
            //
            Response.Redirect(String.Format("allowss.aspx?{0}={1}&returnurl={2}&{3}={4}"
                , CONSTS.QuerryString.StoryToPublish
                , (int)StoriesToPublish.VIllageTakeover
                , Server.UrlEncode(NavigationHelper.VillageOverview(MyVillages[0].id))
                , CONSTS.QuerryString.SelectedVillageID
                , MyVillages[0].id));
        }
        else
        {


            Response.Redirect(NavigationHelper.VillageOverview(MyVillages[0].id), false);

        }
    }



    private void SendMessage(Fbg.Bll.Player newPlayer, int friendPlayerID, string newPlayerRealName)
    {
        PlayerOther po = PlayerOther.GetPlayer(newPlayer.Realm, friendPlayerID, newPlayer.ID);
        Fbg.Bll.Mail.sendEmail(newPlayer.ID, friendPlayerID.ToString()
            , String.Format(R_MiscMessages.GetString("email_MsgToFriendUponRealmJoin_Subject"), newPlayerRealName)
            , String.Format(R_MiscMessages.GetString("email_MsgToFriendUponRealmJoin_Body"), po.PlayerName, newPlayerRealName, newPlayer.Name)
            , po.PlayerName, newPlayer.Realm.ConnectionStr);
    }


    private void SendMessageAboutOfferIfNeeded()
    {
        if (!FbgUser.Offers_HasOffer(Fbg.Bll.User.Offers.Number2) 
            && FbgUser.Offers_IsOfferAvailable(Fbg.Bll.User.Offers.Number2, FbgPlayer.RegisteredOn)
            && FbgUser.HasFlag(Fbg.Bll.User.Flags.Offers_HasUsedServantOfferNumber2) == null // this is a hack. this check is redundant once Offers_IsOfferAvailable is fixed to include this check
            )
        {
            int servantAmount = FbgUser.Offers_GetServantOfferAmount(Fbg.Bll.User.Offers.Number2);
            FbgUser.Offers_SetOfferAsOffered(Fbg.Bll.User.Offers.Number2, servantAmount);

            Fbg.Bll.Mail.sendEmail(Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerId(FbgPlayer.Realm), FbgPlayer.ID.ToString(), "Free Servants Offer"
                , String.Format("Congratulations {0} {1}! <BR><BR>You have been selected to receive {2} FREE SERVANTS with your next purchase of <i>any</i> servant package! <BR><BR><A target=_parent href='pfcredits.aspx' class='applyMobileAction' data-mobileClickAction='ROE.Frame.showBuyCredits' data-mobileText='Tap here to go to the store.'>Click here to go to the store.</a><BR><BR>Thanks for playing Realm of Empires.", FbgPlayer.Title.TitleName(FbgPlayer.Sex), FbgPlayer.Name, servantAmount), FbgPlayer.Name, FbgPlayer.Realm.ConnectionStr);

        }
    }


    private void PublishVillageTakeoverStory()
    {
        
    }

}
