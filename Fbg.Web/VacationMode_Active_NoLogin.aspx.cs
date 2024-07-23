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


public partial class VacationMode_Active_NoLogin : MyCanvasIFrameBasePage
{


    new protected void Page_Load(object sender, EventArgs e)
    {
        Fbg.Bll.Player.VacationModeStatus VacationModeStatus = FbgPlayer.GetVacationModeStatus();

        if (VacationModeStatus.requestedOn == DateTime.MinValue) {
            Response.Redirect("ChooseRealm.aspx");
        }

        dateRequestedOn.Text = "Requested date: "+VacationModeStatus.requestedOn.ToShortDateString();
        dateTakesEffectOn.Text = "Effective date: " + VacationModeStatus.takesEffectOn.ToShortDateString();
        dateEndsOn.Text = "Ends date: " + VacationModeStatus.endsOn.ToShortDateString();

        if (FbgPlayer.Stewardship_IsLoggedInAsSteward)
        {
            warn1.Text = "Stewards can not cancel Vacation mode.";
            LinkButton1.Visible = false;
        }
        else {
            warn1.Text = "Please note: Cancelling vacation mode early, still consumes a minimum of " + FbgPlayer.Realm.VacationModeGet.PerUseMinimum + " vacation days, from your total.";
        }
        
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        FbgPlayer.VacationMode_Cancel();
        Response.Redirect("ChooseRealm.aspx");
    }

}
