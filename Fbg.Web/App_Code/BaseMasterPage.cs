using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Resources;

/// <summary>
/// Summary description for BaseMasterPage
/// </summary>
public class BaseMasterPage : System.Web.UI.MasterPage
{

    public string _baseResName = string.Empty;
    public string BaseResName
    {
        private get
        {
            if (String.IsNullOrEmpty(_baseResName))
            {
                throw new InvalidOperationException("Must set BaseResName first");
            }
            return _baseResName;
        }
        set
        {
            _baseResName = value;
        }
    }

    private ResourceManager _R_notifications;
    public ResourceManager R_notifications
    {
        get
        {
            if (_R_notifications == null)
            {
                _R_notifications = new ResourceManager("Resources." + Config.Theme + ".notifications", typeof(Resources.global).Assembly);
            }
            return _R_notifications;
        }
    }

        
    #region resources
    public string GetCurrentPageName()
    {
        string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
        System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
        return oInfo.Name;
    }

    public string Language()
    {
        return Request.UserLanguages[0].Split(';')[0];
    }

    private ResourceManager _R;
    public ResourceManager R
    {
        get
        {
            if (_R == null)
            {
                _R = new ResourceManager("Resources." + Config.Theme + "." + BaseResName, typeof(Resources.global).Assembly);
            }
            return _R;
        }
    }
    private ResourceManager _RCommon;
    public ResourceManager RCommon
    {
        get
        {
            if (_RCommon == null) 
            {
                _RCommon = new ResourceManager("Resources." + Config.Theme + ".Common", typeof(Resources.global).Assembly);
            }
            return _RCommon;
        }
    }
 
    private ResourceManager _R_PFs;
    public ResourceManager R_PFs
    {
        get
        {
            if (_R_PFs == null)
            {
                _R_PFs = new ResourceManager("Resources." + Config.Theme + ".PFs", typeof(Resources.global).Assembly);
            }
            return _R_PFs;
        }
    }
    /// <summary>
    /// Acess to the COMMON resource file
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string RSc(string name)
    {
        return RCommon.GetString(name);
    }

    public string RS(string name)
    {
        return R.GetString(name);
    }

    private ResourceManager _RImgsCommon;
    public ResourceManager RImgsCommon
    {
        get
        {
            if (_RImgsCommon == null)
            {
                _RImgsCommon = new ResourceManager("Resources." + Config.Theme + ".ImgsCommon", typeof(Resources.global).Assembly);
            }
            return _RImgsCommon;
        }
    }

    private ResourceManager _RImgs;
    public ResourceManager RImgs
    {
        get
        {
            if (_RImgs == null)
            {
                _RImgs = new ResourceManager("Resources." + Config.Theme + ".Imgs", typeof(Resources.global).Assembly);
            }
            return _RImgs;
        }
    }

    /// <summary>
    /// access to images resource
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string RSi(string name)
    {
        return RImgs.GetString(name);
    }

    /// <summary>
    /// access to images resource
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string RSic(string name)
    {
        return RImgsCommon.GetString(name);
    }
    #endregion

    
}
