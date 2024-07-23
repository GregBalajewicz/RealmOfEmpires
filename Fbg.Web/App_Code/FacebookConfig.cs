using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;
using System.Web;


public class FacebookConfig : ConfigurationSection
{
    private static string _CONFIG_SECTION = "Facebook";

    #region singleton implementation
    private static FacebookConfig _config;
    static FacebookConfig()
    {
        _config = (FacebookConfig)ConfigurationSettings.GetConfig(_CONFIG_SECTION);
    }
    #endregion

    public static string AuthorizeReturnToUrlMobile
    {
        get
        {
            return "";
        }
    }
    public static string AuthorizeReturnToUrl
    {
        get
        {
            return "";
        }
    }
    [ConfigurationProperty("AuthorizePermissions", IsRequired = true)]
    public static string AuthorizePermissions
    {
        get
        {
            return "";
        }
    }

    public static string FacebookApiKey
    {
        get
        {
            return "";
        }
    }
    public static string FacebookSecretKey
    {
        get
        {
            return "";
        }
    }

    public static string DisconnectedFromFacebookUserID
    {
        get
        {


            return "";
        }
    }
   

    public static string KongregateSecretKey
    {
        get
        {
            return "";
        }
    }
    

    public static string ArmorGamesUrl
    {
        get
        {
            return "";
        }
    }
    public static string CanvasPageUrl_Full
    {
        get
        {
            return "";
        }
    }
    


}

