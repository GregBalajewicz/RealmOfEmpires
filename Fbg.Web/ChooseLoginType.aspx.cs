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
using System.Xml;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using Facebook.Entity;
using Fbg.Bll;

public partial class ChooseLoginType : MyCanvasIFrameBasePage
{
    /// <summary>
    /// returns the type of device (browser) that is accessing the page
    /// </summary>
    public CONSTS.Device Device
    {
        get
        {
            return Utils.ToDevice(Request.UserAgent);
        }
    }

    new protected void Page_Load(object sender, EventArgs e)
    {

       
        if (Session["request_ids"] == null && Request["request_ids"] != null)
        {
            Session["request_ids"] = Request["request_ids"];
        }

       
    }





}