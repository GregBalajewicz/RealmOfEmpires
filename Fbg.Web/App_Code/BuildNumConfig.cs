using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized; 


public class BuildNumConfig : ConfigurationSection
{
    private static string _CONFIG_SECTION = "BuildNumber";

    #region singleton implementation
    private static BuildNumConfig _config;
    static BuildNumConfig()
    {
        _config = (BuildNumConfig)ConfigurationSettings.GetConfig(_CONFIG_SECTION);
    }
    #endregion 

    public static string BuildNumber
    {
        get
        {
            return _config.BUILD_NUMBER;
        }
    }
    public static float CurrentBuildID
    {
        get
        {
            return _config.Current_BuildID;
        }
    }

    public static string CurrentBuildIDWhatsNew
    {
        get
        {
            return _config.Current_BuildIDWhatsNew;
        }
    }

    #region public properties the define the config items we are looking for
    [ConfigurationProperty("BuildNumber", IsRequired = false)]
    public string BUILD_NUMBER
    {
        get
        {
            return _config["BuildNumber"] as string;
        }
    }
    [ConfigurationProperty("CurrentBuildID", IsRequired = false)]
    public float Current_BuildID
    {
        get
        {
            return (float)_config["CurrentBuildID"];
        }
    }
     [ConfigurationProperty("CurrentBuildIDWhatsNew", IsRequired = false)]
    public string Current_BuildIDWhatsNew
    {
        get
        {
            return (string)_config["CurrentBuildIDWhatsNew"];
        }
    }


    
    #endregion
} 
 


