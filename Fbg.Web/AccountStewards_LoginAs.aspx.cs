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


public partial class AccountStewards_LoginAs : MyCanvasIFrameBasePage
{

    public class localCONST
    {
        
    }

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        Request.Cookies.Remove(CONSTS.Cookies.StewardLoggedInAsRecordID);

        int recordID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RecordID]);
        int loginAsPlayerID = Int32.MinValue;

        int myPlayerID = FbgPlayer.ID;

        DataTable myStewardship = FbgPlayer.Stewardship_GetMyStewardship();
        foreach (DataRow dr in myStewardship.Rows)
        {
            if ((int)dr[Fbg.Bll.Player.CONSTS.MyStewardshipColIndex.RecordID] == recordID)
            {
                loginAsPlayerID = (int)dr[Fbg.Bll.Player.CONSTS.MyStewardshipColIndex.AccountOwnerPlayerID];
                break;
            }
        }

        if (loginAsPlayerID != Int32.MinValue)
        {
            //
            // HOW DOES THIS WORK??
            //  See LoginToRealm.aspx.cs 
            //
            Fbg.Bll.PlayerOther p = Fbg.Bll.PlayerOther.GetPlayer(FbgPlayer.Realm, loginAsPlayerID, FbgPlayer.ID);
            Response.Cookies.Add(new HttpCookie(CONSTS.Cookies.StewardLoggedInAsRecordID, recordID.ToString()));
            InvalidateFbgPlayer();
            InvalidateFbgUser();
            Response.Redirect(String.Format("LoginToRealm.aspx?{0}={1}"
                , CONSTS.QuerryString.PlayerID
                , myPlayerID), false);

        }
        else
        {
            //
            // remove the cookie telling me to login as someone else just in case.
            //
            HttpCookie stewardCookie = new HttpCookie(CONSTS.Cookies.StewardLoggedInAsRecordID);
            stewardCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(stewardCookie);

            Response.Redirect("~/ChooseRealm.aspx", false);
        }

    }
}
