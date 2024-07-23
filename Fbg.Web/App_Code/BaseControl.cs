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
/// Summary description for BaseControl
/// </summary>
public class BaseControl : System.Web.UI.UserControl
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


    #region resources

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

    private ResourceManager _R_MiscMessages;
    public ResourceManager R_MiscMessages
    {
        get
        {
            if (_R_MiscMessages == null)
            {
                _R_MiscMessages = new ResourceManager("Resources." + Config.Theme + ".MiscMessages", typeof(Resources.global).Assembly);
            }
            return _R_MiscMessages;
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

    private ResourceManager _R_resStories;
    public ResourceManager R_resStories
    {
        get
        {
            if (_R_resStories == null)
            {
                _R_resStories = new ResourceManager("Resources." + Config.Theme + ".resStories", typeof(Resources.global).Assembly);
            }
            return _R_resStories;
        }
    }

    private ResourceManager _R_Tutorial;
    public ResourceManager R_Tutorial
    {
        get
        {
            if (_R_Tutorial == null)
            {
                _R_Tutorial = new ResourceManager("Resources." + Config.Theme + ".Tutorial", typeof(Resources.global).Assembly);
            }
            return _R_Tutorial;
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
