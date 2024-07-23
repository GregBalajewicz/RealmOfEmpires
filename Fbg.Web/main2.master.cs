using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class main2 : BaseMasterPage
{

     public bool isMobile
    {
        get
        {
            return Utils.isMobile(Request);
        }
    }
    public bool isD2
    {
        get
        {
            return Utils.isD2(Request);
        }
    }

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

    public bool IsTesterRoleOrHigher
    {
        get
        {
            return (Context.User.IsInRole("Admin") || Context.User.IsInRole("tester"));
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        
      
       
    }

}
