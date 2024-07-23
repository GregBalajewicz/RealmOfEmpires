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


public partial class SleepMode_Active_NoLogin : MyCanvasIFrameBasePage
{


    new protected void Page_Load(object sender, EventArgs e)
    {
        
        RealmID.Text = String.Format("{0}", FbgPlayer.Realm.Tag);
        DateTime sleepLength = FbgPlayer.SleepMode_ActiveOn.AddHours(FbgPlayer.Realm.SleepModeGet.Duration);
        DateTime sleepLengthSafety = sleepLength.AddSeconds(10); //extra 10 sec for counter finish AFTER end of sleepmode (not get stuck)
        SleepTimer.Text = String.Format("{0}", sleepLengthSafety.Subtract(DateTime.Now));

        redir.Text = "LoginToRealm.aspx?rid=" + FbgPlayer.Realm.Tag + "&pid=" +FbgPlayer.ID;
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        FbgPlayer.SleepMode_Cancel();
        Response.Redirect("ChooseRealm.aspx");
    }

    public static double SerializeDate(DateTime date)
    {        
        return date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

}
