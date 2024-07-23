using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

/// <summary>
/// Summary description for TemplatePage
/// </summary>
public class TemplatePage : MyCanvasIFrameBasePage
{
    private bool _isInDesignMode = true;
    public bool IsInDesignMode {
        get
        {
            return _isInDesignMode;
        }
    }
    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (Request["t"] != null)
        {
            _isInDesignMode = false;
            MasterPageFile = "masterEmpty_m.master";
        }
    }

    public bool isMobile
    {
        get
        {
            return Utils.isMobile(Request);
        }
    }


    /// <summary>
    /// tells if you the browser is such that we do not do iframe popups. like android
    /// </summary>
    public bool IsiFramePopupsBrowser
    {
        get
        {
            return Utils.IsiFramePopupsBrowser(Request);
        }
    }


    public bool isTactica
    {
        get
        {
            if (
                String.Equals(LoggedInMembershipUser.UserName, LoggedInMembershipUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }

}