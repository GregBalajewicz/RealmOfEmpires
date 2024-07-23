using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Gmbc.Common.Diagnostics.ExceptionManagement;

using Fbg.Bll;
using Fbg.Common;

using BDA.Neighbours;
using System.Text;


public partial class VillageOverview : MyCanvasIFrameBasePage
{
    private bool showDetails = false;
    private bool hasImprovedVoVPF;
    private TimeOfDay _timeOfDay;
    HttpCookie villageTypeCookie;
    bool graphicalVov = true;
    Fbg.Bll.Village village = null;
    MasterBase_Main mainMasterPage;

    public bool IsTesterRoleOrHigher
    {
        get
        {
            return (Context.User.IsInRole("Admin") || Context.User.IsInRole("tester"));
        }
    }


    new protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Request.QueryString["mobile"] != "1")
            {
                if (isMobile)
                {
                    Response.Redirect("mobilemain.aspx");
                }
            }

            if (isD2)
            {
                dlGifts.RepeatColumns = 1;
                Response.Redirect("main.aspx");
            }
            else if (BetaD2.isD2OnlyRealm(FbgPlayer.Realm.ID)
                || !BetaD2.canEnterD1(IsTesterRoleOrHigher, LoggedInMembershipUser.CreationDate) // case 27364
                )
            {
                HttpCookie isM = Request.Cookies["isD2"];
                if (isM == null)
                {
                    Response.Redirect("ChooseRealmD2.aspx");
                }
            }


        }
        catch (System.Threading.ThreadAbortException) { }
        catch (Exception ex)
        {
            BaseApplicationException bex = new BaseApplicationException("error in page_load", ex);
            if (village != null)
            {
                village.SerializeToNameValueCollection(bex.AdditionalInformation);
            }
            throw bex;
        }

        // FbgUser.LogEvent(369, "D1", FbgPlayer.Realm.ID.ToString());
    }


    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }
        base.OnPreInit(e);
    }

}
