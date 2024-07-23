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

using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class Controls_ListOfRealmsCompact : System.Web.UI.UserControl
{

   /// <summary>
    /// 
    /// </summary>
    /// <param name="currentlyLoggedInPlayer">The ID of the currently logged in player if know</param>
    /// <returns>Hyperlinkg of the currently logged in player.</returns>
    public HyperLink Initialize(int currentlyLoggedInPlayer, Fbg.Bll.User currentUser, bool isDevice)
    {


        TableCell cell;
        HyperLink hyperlink = null;
        HyperLink hyperlinkOfCurrentlyLoggedInPlayer = null;
        Fbg.Bll.Player player = null;
        Label lbl;
        Utils.LoginLinkType loginLinkType;
        int numberOfRealmsRegistered = 0;
        foreach (Realm realm in Realms.AllRealmsReversed)
        {

            try
            {              
                if (realm.isLimitAccessToVIPsOnly && !currentUser.VIP_isVIP)
                {
                    continue;
                }
                // only show realms that are opening 14 days or less from now.
                if (realm.OpenOn.AddDays(-14) > DateTime.Now)
                {
                    continue;
                }
                player = currentUser.PlayerByRealmID(realm.ID);
                hyperlink = new HyperLink();
                hyperlink = Utils.GetLoginLink(currentUser, realm, out loginLinkType, false, isDevice);

                if (hyperlink == null )
                {
                    continue;
                }

                //for the quicklinks only allow these types
                if (!loginLinkType.ToString().Equals("Login") && !loginLinkType.ToString().Equals("Register") && !loginLinkType.ToString().Equals("PreRegister"))
                {
                    continue;
                }


                hyperlink.CssClass = "realmLink";
                if (realm.isLimitAccessToVIPsOnly)
                {
                    hyperlink.CssClass += " vipexclusive";
                }
                if (realm.PlayerGenerated.IsPlayerGenerated && realm.PlayerGenerated.Private.isPrivate)
                {
                    hyperlink.CssClass += " private";
                }

                hyperlink.Attributes.Add("loginLinkType",loginLinkType.ToString());

                //
                // if hyperlink is null that means this player has no access to this realm. 
                //
                if (hyperlink != null)
                {

                    if (realm.ClosingOn < DateTime.Now)
                    {
                        hyperlink.CssClass += " realmEnded";
                    }
                    
                    if (player != null)
                    {
                        //
                        // is this a the logged in player represented by the cookie
                        if (player.ID == currentlyLoggedInPlayer)
                        {
                            hyperlinkOfCurrentlyLoggedInPlayer = hyperlink;
                        }

                        if (loginLinkType == Utils.LoginLinkType.Login)
                        {
                            //
                            // players registered on this realm. note this realm, will be used for later on, if player only plays on one realm.
                            //
                            numberOfRealmsRegistered++;
                            onerealm_LoginLink = hyperlink.NavigateUrl;
                            onerealm_Realm = realm;
                        }
                    }

                    //hyperlink.Text += " R" + realm.Tag;
                    hyperlink.Text = realm.Tag;

                    realmListCompact.Controls.Add(hyperlink);
                }

            }
            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("error in foreach (Realm realm in Realms.AllRealms)", ex);
                bex.AddAdditionalInformation("realm", realm);
                bex.AddAdditionalInformation("player", player);
                bex.AddAdditionalInformation("hyperlink.NavigateUrl", hyperlink == null ? "null" : hyperlink.NavigateUrl);
                throw bex;
            }

        }

        if (numberOfRealmsRegistered > 1)
        {
            // if player playing on only one realm, leave those untouched, otherwise null them
            onerealm_Realm = null;
            onerealm_LoginLink = null;
        }

        return hyperlinkOfCurrentlyLoggedInPlayer;
    }

    /// <summary>
    /// null if player is not registered at exactly one realm
    /// </summary>
    public string onerealm_LoginLink { get; private set; }
    /// <summary>
    /// null if player is not registered at exactly one realm
    /// </summary>
    public Realm onerealm_Realm { get; private set; }


}


