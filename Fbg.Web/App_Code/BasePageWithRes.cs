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
/// Summary description for BasePageWithRes
/// </summary>
public class BasePageWithRes : System.Web.UI.Page
{

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
                _R = new ResourceManager("Resources." + Config.Theme + "." + GetCurrentPageName(), typeof(Resources.global).Assembly);
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



    /// <summary>
    /// helper to build the help json file 
    /// </summary>
    /// <param name="entires"></param>
    /// <returns></returns>
    protected string BuildHelpJSON(string[] entires)
    {
        return BuildHelpJSON(entires, "_title", "_t");
    }
    /// <summary>
    /// helper to build the help json file 
    /// </summary>
    /// <param name="entires"></param>
    /// <returns></returns>
    protected string BuildHelpJSON(string[] entires, string postFix_Title, string postFix_Text)
    {

        System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        json_serializer.MaxJsonLength = CONSTS.MaxJsonLength;
        System.Collections.Generic.Dictionary<String, object> dic
            = new System.Collections.Generic.Dictionary<string, object>();
        System.Collections.Generic.Dictionary<String, String> dic1;

        System.Collections.Generic.List<object> list = new System.Collections.Generic.List<object>(entires.Length);
        foreach (string s in entires)
        {
            if (s != string.Empty) // we ignore empty strings
            {
                dic1 = new System.Collections.Generic.Dictionary<string, string>();

                dic1.Add("id", s);
                dic1.Add("title", RS(s + postFix_Title));
                dic1.Add("text", RS(s + postFix_Text));

                list.Add(dic1);
            }
        }


        dic.Add("help", list);

        return json_serializer.Serialize(dic);
    }


}

