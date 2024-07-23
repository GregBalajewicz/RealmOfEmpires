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
/*
 */

public partial class LostLastVilInClosedRealm : MyCanvasIFrameBasePage
{


    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        lblCurRealm.Text = String.Format("{0}", Convert.ToInt32(Request.QueryString[global::CONSTS.QuerryString.RealmID]) );
        //
        // make sure the person cannot enter again.
        //
        this.InvalidateFbgPlayer();
        Response.Cookies.Add(new HttpCookie(CONSTS.Cookies.PlayerID){ Expires = DateTime.Now.AddMonths(-1)});

        if (!String.IsNullOrEmpty(Request.QueryString[global::CONSTS.QuerryString.RealmID]))
        {
            PlayerRanking1.Initalize(null, null, Fbg.Bll.Realms.Realm(Convert.ToInt32(Request.QueryString[global::CONSTS.QuerryString.RealmID])));

        }
        else
        {
            PlayerRanking1.Visible = false;
        }

        
    }

}
