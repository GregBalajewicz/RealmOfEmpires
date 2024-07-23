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

public partial class ExclusiveRealmMessage : MyCanvasIFrameBasePage
{
    public Realm realm ;
    int realmID;

    public CONSTS.Device isDevice
    {
        get
        {
            return Utils.ToDevice(Request.UserAgent);
        }
    }

    new protected void Page_Load(object sender, EventArgs e)
    {   
        
        base.Page_Load(sender, e);
        Realm r = Realms.Realm(Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RealmID]));
        Title title = r.TitleByID(r.RealmTitleEntryLimitations);
        txtTitle.Text = title.TitleName_Male + "/" + title.TitleName_Female;


        if (title.TitleName_Male != title.TitleName_Female) { txtTitle2.Text = title.TitleName_Male + "/" + title.TitleName_Female; }
        else { txtTitle2.Text = title.TitleName_Male; }

        realmID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RealmID]);
        realm = Realms.Realm(realmID);

    }

}