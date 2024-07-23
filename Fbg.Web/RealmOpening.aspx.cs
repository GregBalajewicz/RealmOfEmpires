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


public partial class RealmOpening : MyCanvasIFrameBasePage
{
    
    
    public Realm realm = null;
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        int rid;
        string realmID = Request.QueryString[CONSTS.QuerryString.RealmID];
        string playerID = Request.QueryString[CONSTS.QuerryString.PlayerID];

        if (Int32.TryParse(realmID, out rid))
        {
            try
            {
                realm = Realms.Realm(rid);
            }
            catch { };
        }

        if (realm != null)
        {
            panelJoinAnotherRealm.Visible = !realm.IsTemporaryTournamentRealm;
            panelJoinAnotherRealm2.Visible = !realm.IsTemporaryTournamentRealm;
            panelPublishStory.Visible = false;// !realm.IsTemporaryTournamentRealm;

            panelTournamentRealm.Visible = realm.IsTemporaryTournamentRealm;
            panelTournamentRealm2.Visible = realm.IsTemporaryTournamentRealm;


            if (realm.OpenOn.Date == DateTime.Today)
            {
                lblOpensOn.Text = "today at " + realm.OpenOn.ToUniversalTime().ToString("HH:mm:ss");
                lblOpensOn2.Text = "today at " + realm.OpenOn.ToUniversalTime().ToString("HH:mm:ss");
            }
            else
            {
                lblOpensOn.Text = realm.OpenOn.ToUniversalTime().ToString("dddd MMM d HH:mm:ss");
                lblOpensOn2.Text = realm.OpenOn.ToUniversalTime().ToString("dddd MMM d HH:mm:ss");

            }

            lblCountdown.Text = Utils.FormatDuration(realm.OpenOn.Subtract(DateTime.Now));
            lblCountdown2.Text = Utils.FormatDuration(realm.OpenOn.Subtract(DateTime.Now));

            lblCountdown.Attributes.Add("redir", String.Format("LoginToRealm.aspx?{0}={1}", CONSTS.QuerryString.PlayerID, playerID));
            lblCountdown2.Attributes.Add("redir", String.Format("LoginToRealm.aspx?{0}={1}", CONSTS.QuerryString.PlayerID, playerID));

            if (!String.IsNullOrEmpty(realm.ExtendedDesc.Trim()))
            {
                lblRealmInfo.Text = realm.ExtendedDesc;
                lblRealmInfo2.Text = realm.ExtendedDesc;
                pnlRealmInfo.Visible = true;
                pnlRealmInfo2.Visible = true;
            }

        }
        else
        {
            //
            // this really should not happen but if it does, we dont want errors
            //
            panelJoinAnotherRealm.Visible =false;
            panelPublishStory.Visible = false;
            lblCountdown.Visible = false;
        }
    }
}
