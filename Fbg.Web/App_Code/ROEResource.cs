using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Resources;

/// <summary>
/// Summary description for ROEResource
/// </summary>
public class ROEResource
{

    static private ResourceManager _RCommon;
    static public ResourceManager RCommon
    {
        get
        {
            if (_RCommon == null) {
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
    static public string RSc(string name)
    {
        return RCommon.GetString(name);
    }



    static private ResourceManager _RImgsCommon;
    static public ResourceManager RImgsCommon
    {
        get
        {
            if (_RImgsCommon == null) {
                _RImgsCommon = new ResourceManager("Resources." + Config.Theme + ".ImgsCommon", typeof(Resources.global).Assembly);
            }
            return _RImgsCommon;
        }
    }

    static private ResourceManager _RImgs;
    static public ResourceManager RImgs
    {
        get
        {
            if (_RImgs == null) {
                _RImgs = new ResourceManager("Resources." + Config.Theme + ".Imgs", typeof(Resources.global).Assembly);
            }
            return _RImgs;
        }
    }

    static private ResourceManager _R_MiscMessages;
    static public ResourceManager R_MiscMessages
    {
        get
        {
            if (_R_MiscMessages == null) {
                _R_MiscMessages = new ResourceManager("Resources." + Config.Theme + ".MiscMessages", typeof(Resources.global).Assembly);
            }
            return _R_MiscMessages;
        }
    }

    static private ResourceManager _R_notifications;
    static public ResourceManager R_notifications
    {
        get
        {
            if (_R_notifications == null) {
                _R_notifications = new ResourceManager("Resources." + Config.Theme + ".notifications", typeof(Resources.global).Assembly);
            }
            return _R_notifications;
        }
    }

    static private ResourceManager _R_resStories;
    static public ResourceManager R_resStories
    {
        get
        {
            if (_R_resStories == null) {
                _R_resStories = new ResourceManager("Resources." + Config.Theme + ".resStories", typeof(Resources.global).Assembly);
            }
            return _R_resStories;
        }
    }

    static private ResourceManager _R_Tutorial;
    static public ResourceManager R_Tutorial
    {
        get
        {
            if (_R_Tutorial == null) {
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
    static public string RSi(string name)
    {
        return RImgs.GetString(name);
    }

    /// <summary>
    /// access to images resource
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static public string RSic(string name)
    {
        return RImgsCommon.GetString(name);
    }	
}