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


public partial class WeekendMode_Active_NoLogin : MyCanvasIFrameBasePage
{


    new protected void Page_Load(object sender, EventArgs e)
    {
        Fbg.Bll.Player.WeekendModeStatus weekendModeStatus = FbgPlayer.GetWeekendModeStatus();

        if (!weekendModeStatus.active)
        {
            Response.Redirect("ChooseRealm.aspx");
        }

        dateTakesEffectOn.Text = "Effective date: " + weekendModeStatus.takesEffectOn.ToUniversalTime().ToShortDateString() + " " + weekendModeStatus.takesEffectOn.ToUniversalTime().ToShortTimeString() + " (UTC)";
        dateEndsOn.Text = "Ends date: " + weekendModeStatus.endsOn.ToUniversalTime().ToShortDateString() + " " + weekendModeStatus.endsOn.ToUniversalTime().ToShortTimeString() + " (UTC)";

        if (FbgPlayer.Stewardship_IsLoggedInAsSteward)
        {
            warn1.Text = "Stewards can not cancel Weekend Mode";
            LinkButton1.Visible = false;
        }
        else
        {
            warn1.Text = "Please note: Next activation will be allowed allowed " + FbgPlayer.Realm.WeekendModeGet.ReactivationDelayDays + " days after End or Cancel.";
        }

    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        FbgPlayer.WeekendMode_Cancel();
        Response.Redirect("ChooseRealm.aspx");
    }

}
