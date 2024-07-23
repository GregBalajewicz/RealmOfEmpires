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
using System.Security.Principal;
using System.Text.RegularExpressions ;
using Fbg.Bll;
using System.Collections.Generic;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class ChooseQuad : MyCanvasIFrameBasePage 
{
    public Realm realm ;
    int realmID;

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        if (FbgPlayer.Villages.Count > 0)
        {
            Response.Redirect("CHooserealm.aspx");
        }

        #region Localize Controls
        this.btnGo.DataBind();
        this.btnGoLink.DataBind();

        //localize item lists
        DropDownList1.Items[0].Text = RS("li_Random");
        DropDownList1.Items[1].Text = RS("li_NE");
        DropDownList1.Items[2].Text = RS("li_SE");
        DropDownList1.Items[3].Text = RS("li_SW");
        DropDownList1.Items[4].Text = RS("li_NW");
        #endregion

        if (isMobile) {
            btnGo.Visible = false;
            btnGoLink.Visible = true;
        }
        else {
            btnGo.Visible = true;
            btnGoLink.Visible = false;
        }
        Utils.PreventDoubleSubmit(this, btnGo);
        Utils.PreventDoubleSubmit(this, btnGoLink, "");

        //
        // get the realm
        // 
        realmID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RealmID]);
        realm = Realms.Realm(realmID);
        if (realm == null)
        {
            ExceptionManager.Publish(new Exception(String.Format("UNREPORTED EXCEPTION: realm ID {0} not found. UserID={1}", realmID, FbgUser.ID)));
            Response.Redirect("ChooseRealm.aspx");
        }

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        int? sel = null;
        if (DropDownList1.SelectedIndex != 0)
        {
            sel = DropDownList1.SelectedIndex;
        }
        FbgPlayer.Restart_GetNewVillage(sel);
        MyVillagesInvalidate();
        if (FbgPlayer.Villages.Count < 1)
        {
            BaseApplicationException bex = new BaseApplicationException("in ChooseQuad.aspx - no villages after  FbgPlayer.Restart_GetNewVillage(sel)");
            bex.AddAdditionalInformation("sel",sel);
            FbgPlayer.SerializeToNameValueCollection(bex.AdditionalInformation);
            ExceptionManager.Publish(bex);
        }
        Response.Redirect(string.Format("LoginToRealm.aspx?pid={0}&rid={1}", FbgPlayer.ID, realm.ID));                
    }

    public int AvatarIDFromSession()
    {
        try
        {

            if (Session[CONSTS.Session.fbgAvatar] == null || String.IsNullOrEmpty((string)Session[CONSTS.Session.fbgAvatar]))
            {
                return 1;
            }

            return Convert.ToInt32(Session[CONSTS.Session.fbgAvatar]);
        }
        catch { }

        return 1;
    }
}
