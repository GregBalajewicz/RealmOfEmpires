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
using System.Linq;

using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;


public abstract class MasterIntroBase : System.Web.UI.MasterPage
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="villageName"></param>
    /// <param name="showFood"></param>
    /// <param name="showSilver">0 - no, 1 production only, 2 prod and treasury</param>
    public abstract void Initialize(string villageName, bool showFood, int showSilver);
    public abstract void Initialize(string villageName, bool showFood);
    public abstract void Initialize(string villageName);
    public abstract void SetVillageNameLinkUrl(string url);

}