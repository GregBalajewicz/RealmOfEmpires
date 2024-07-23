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
 * HOW THIS PAGE WORKS
 * 
 * person is taken to LoginToRealm.aspx.
 * 
 * this writes out a player id cookie and sends player here
 * 
 * this page invalidates FBGPlayer thus preventing the player. FBGPlayer get also checks if steward is active and
 *  sends player here in case it is. so the combination of these 2, ensures that player cannot leave this page.
 *  when ever a player tries, he will be transfered here. 
 * 
 * now, if we get querry string param 'y', then we first cancel stewardship, and redirect to chooserealm.aspx.
 *  since a cookie with player id has been written, player will be taken straight to his realm. 
 */

public partial class AccountStewards_DeactivateBeforeLogin : MyCanvasIFrameBasePage
{

    public class localCONST
    {
        public class MyStewardsGridColumnIndex
        {
            public static int Cancel = 0;
        }
        public class StewardshipGridColumnIndex
        {
            public static int Status = 2;
            public static int Cancel = 0;
        }
    }

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

      
        if (!string.IsNullOrEmpty(Request.QueryString["y"]))
        {
            DataTable dt = FbgPlayer.Stewardship_GetMyStewards();
            if (dt.Rows.Count > 0)
            {
                FbgPlayer.Stewardship_DeleteAppointedSteward((int)dt.Rows[0][Fbg.Bll.Player.CONSTS.MyStewardsColIndex.RecordID]);
                Response.Redirect("ChooseRealm.aspx");
            }
        }
        //
        // make sure the person cannot enter again.
        //
        this.InvalidateFbgPlayer();
    }

}
