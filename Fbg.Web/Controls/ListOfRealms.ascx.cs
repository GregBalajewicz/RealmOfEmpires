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

public partial class Controls_ListOfRealms : System.Web.UI.UserControl
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentlyLoggedInPlayer">The ID of the currently logged in player if know</param>
    /// <returns>Hyperlinkg of the currently logged in player.</returns>
    public HyperLink Initialize(int currentlyLoggedInPlayer, Fbg.Bll.User currentUser, bool isDevice, DateTime dateTheUserRegisteredAtTheGame)
    {
        return this.Initialize(currentlyLoggedInPlayer, currentUser, -1, isDevice, dateTheUserRegisteredAtTheGame);
    }
    public HyperLink Initialize(int currentlyLoggedInPlayer, Fbg.Bll.User currentUser, int realmIDToHighlight, bool isDevice, DateTime dateTheUserRegisteredAtTheGame)
    {
        return Initialize(currentlyLoggedInPlayer, currentUser, realmIDToHighlight, false, isDevice, dateTheUserRegisteredAtTheGame);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentlyLoggedInPlayer">The ID of the currently logged in player if know</param>
    /// <param name="realmIDToHighlight">pass in -1 if you do not want any realm highlighted</param>
    /// <returns>Hyperlinkg of the currently logged in player.</returns>
    public HyperLink Initialize(int currentlyLoggedInPlayer, Fbg.Bll.User currentUser, int realmIDToHighlight, bool isAdmin, bool isDevice, DateTime dateTheUserRegisteredAtTheGame)
    {
        if (Theme == Themes.ThemeM) {
            tableRealms.Visible = false;
            ThemeA.Visible = true;
        }

        TableRow realmName;
        TableRow realmDescription;
        TableCell cell;
        HyperLink hyperlink = null;
        HyperLink hyperlinkOfCurrentlyLoggedInPlayer = null;
        Fbg.Bll.Player player = null;
        Label lbl;
        Utils.LoginLinkType loginLinkType;
        foreach (Realm realm in Realms.AllRealmsReversed)
        {

            try
            {
                if (realm.isLimitAccessToVIPsOnly && !currentUser.VIP_isVIP)
                {
                    continue;
                }

                // only show realms that are opening 14 days or less from now.
                if (realm.OpenOn.AddDays(-14) > DateTime.Now && !isAdmin)
                {
                    continue;
                }

                realmName = new TableRow();
                realmDescription = new TableRow();
              
                // left bracket
                cell = new TableCell();
                cell.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url('https://static.realmofempires.com/images/hdrBG_TLtrans.gif'");
                cell.Style.Add(HtmlTextWriterStyle.Width, "8px");
                cell.Style.Add(HtmlTextWriterStyle.Height, "23px");
                realmName.Cells.Add(cell);

                // realm name
                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Left;
                cell.Style.Add("font-size", realm.ID == realmIDToHighlight ? "11pt" : "10pt");
                cell.Style.Add(HtmlTextWriterStyle.BackgroundImage, "https://static.realmofempires.com/images/hdrBG_TC.gif");
                cell.Style.Add(HtmlTextWriterStyle.Padding, "3px");
                //cell.Text = realm.Name + " - " + realm.Desc;
                cell.Text = realm.Name;
                if (!string.IsNullOrEmpty(realm.Desc))
                {
                    cell.Text += " - " + realm.Desc;
                }

                cell.Style.Add("font-size", realm.ID == realmIDToHighlight ? "11pt" : "10pt");
                realmName.Cells.Add(cell);

                // circle
                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.Style.Add(HtmlTextWriterStyle.Width, "23px");
                cell.Style.Add(HtmlTextWriterStyle.BackgroundImage, "https://static.realmofempires.com/images/hdrBG_Tbutton.gif");
                realmName.Cells.Add(cell);

                // login/register link 
                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url('https://static.realmofempires.com/images/hdrBG_TC.gif'");
                cell.Style.Add(HtmlTextWriterStyle.Height, "23px");
                cell.Wrap = false;
                cell.Style.Add("font-size", realm.ID == realmIDToHighlight ? "11pt" : "10pt");

                hyperlink = new HyperLink();
                if (Theme == Themes.ThemeM)
                {
                    player = currentUser.PlayerByRealmID(realm.ID);
                }
                hyperlink = Utils.GetLoginLink(currentUser, realm, out loginLinkType, isAdmin, isDevice, dateTheUserRegisteredAtTheGame);

                //
                // if hyperlink is null that means this player has no access to this realm. 
                //
                if (hyperlink != null)
                {

                    hyperlink.CssClass = Theme == Themes.ThemeM ? "oneRealm sfx2" : "";
                    if (realm.ClosingOn < DateTime.Now)
                    {
                        hyperlink.CssClass += " realmEnded";
                    }
                    if (realm.isLimitAccessToVIPsOnly)
                    {
                        hyperlink.CssClass += " vipexclusive";
                    }
                    if (realm.PlayerGenerated.IsPlayerGenerated && realm.PlayerGenerated.Private.isPrivate)
                    {
                        hyperlink.CssClass += " private";
                    }

                    System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
                    sb.Append(Utils.FormatCost(realm.Population) + " active players.");
                    if (realm.IsOpen != Fbg.Bll.Realm.RealmState.PreRegistration)
                    {
                        TimeSpan ts = DateTime.Now.Subtract(realm.OpenOn);
                        sb.Append(" Open for ");
                        sb.Append(ts.Days == 0 ? ts.TotalDays.ToString("#.##") : ts.Days.ToString());
                        sb.Append(" days.");
                    }
                    else if (realm.IsOpen == Fbg.Bll.Realm.RealmState.PreRegistration) {
                        TimeSpan ts = DateTime.Now.Subtract(realm.OpenOn);
                        sb.Append(" Opens: ");
                        sb.Append(realm.OpenOn.ToUniversalTime().ToString("MMM dd, HH:mm"));
                        sb.Append("(UTC)");
                    }

                    if (Theme == Themes.ThemeM) {
                        hyperlink.Text = "<div class=realm >Realm</div> <div class=realmNumber >" + realm.Name2 + "</div><div class=realmInfo><div class=realmName>" + realm.Desc
                            + "</div><div class=realmDetail>" + sb + "</div><div class=realmLogin>" + hyperlink.Text + "</div></div>";
                    }


                    if (player != null)
                    {
                        //
                        // is this a the logged in player represented by the cookie
                        if (player.ID == currentlyLoggedInPlayer)
                        {
                            hyperlinkOfCurrentlyLoggedInPlayer = hyperlink;
                        }
                    }
                    cell.Controls.Add(hyperlink);
                    realmName.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Style.Add(HtmlTextWriterStyle.BackgroundImage, "https://static.realmofempires.com/images/hdrBG_TRtrans.gif");
                    cell.Style.Add(HtmlTextWriterStyle.Width, "8px");
                    realmName.Cells.Add(cell);

                    tableRealms.Rows.Add(realmName);


                    //
                    // Realm details
                    //
                    cell = new TableCell();
                    realmDescription.Cells.Add(cell);

                    cell = new TableCell();
                    cell.ColumnSpan = 3;
                    cell.Style.Add("font-size", realm.ID == realmIDToHighlight ? "11pt" : "auto");
                    cell.HorizontalAlign = HorizontalAlign.Left;

                    lbl = new Label();

                    lbl.Text = Utils.GetRealmDetailsText(realm, loginLinkType);
                    cell.Controls.Add(lbl);

                    realmDescription.Cells.Add(cell);

                    tableRealms.Rows.Add(realmDescription);

                    if (Theme == Themes.ThemeM) {
                        ThemeA.Controls.Add(hyperlink);
                    }
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

        return hyperlinkOfCurrentlyLoggedInPlayer;


    }

    public enum Themes
    {
        old,
        ThemeM
    }

    public Themes Theme { get; set; }
}


