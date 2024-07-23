using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using Gmbc.Common.Diagnostics.ExceptionManagement;


public class Config : ConfigurationSection
{
    private static string _CONFIG_SECTION = "Misc";

   

    #region singleton implementation
    private static Config _config;
    static Config()
    {
        _config = (Config)ConfigurationSettings.GetConfig(_CONFIG_SECTION);
    }
    #endregion

    public static bool ShowErrorLog
    {
        get
        {
            return _config._ShowErrorLog;
        }
    }
    public static bool PFSystemActive
    {
        get
        {
            return _config._PFSystemActive;
        }
    }
    /// <summary>
    /// something like https://www.realmofempires.com/
    /// will contain https:// and will end with a "/"
    /// </summary>
    public static string BaseUrl
    {
        get
        {
            return _config._BaseUrl;
        }
    }
    /// <summary>
    /// true if running in development environment
    /// </summary>
    public static bool InDev
    {
        get
        {
            return _config._InDevEnvironment;
        }
    }
    public static string Theme
    {
        get
        {
            return _config._Theme;
        }
    }
   
    /// <summary>
    /// the realm on which we should be collecting analytics 
    /// </summary>
    private static SortedList<int, int> _collectAnalyticsOnRealms;
    public static bool CollectAnalyticsOnThisRealm(int rid)
    {
        try
        {
            if (_collectAnalyticsOnRealms == null)
            {
                string[] a = _config._CollectAnalyticsOnRealms.Trim().Split(new char[] { ',' });
                if (a.Length > 0 && !string.IsNullOrEmpty(a[0]))
                {
                    _collectAnalyticsOnRealms = new SortedList<int, int>(a.Length);
                    foreach (string s in a)
                    {
                        _collectAnalyticsOnRealms.Add(Convert.ToInt32(s), Convert.ToInt32(s));
                    }
                }
                else
                {
                    _collectAnalyticsOnRealms = new SortedList<int, int>(0);
                }
            }

            return _collectAnalyticsOnRealms.ContainsKey(rid);
        }
        catch (Exception e)
        {
            BaseApplicationException bex = new BaseApplicationException("something wrong - invalid CollectAnalyticsOnRealms config param ?");
            bex.AddAdditionalInformation("_config._CollectAnalyticsOnRealms", _config._CollectAnalyticsOnRealms);
            throw bex;
        }

    }
 
    /// <summary>
    /// the realm on which we should be collecting analytics 
    /// </summary>
    private static SortedList<string, string> _collectAnalytics_OmitEvents;
    public static bool CollectAnalytics_OmitEvent(string eventName)
    {
        try
        {
            if (_collectAnalytics_OmitEvents == null)
            {
                string[] a = _config._CollectAnalytics_OmitEvents.Trim().Split(new char[] { ',' });
                if (a.Length > 0 && !string.IsNullOrEmpty(a[0]))
                {
                    _collectAnalytics_OmitEvents = new SortedList<string, string>(a.Length);
                    foreach (string s in a)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            _collectAnalytics_OmitEvents.Add(s.ToLower(), s.ToLower());
                        }
                    }
                }
                else
                {
                    _collectAnalytics_OmitEvents = new SortedList<string, string>(0);
                }
            }

            return _collectAnalytics_OmitEvents.ContainsKey(eventName.ToLower());
        }
        catch (Exception e)
        {
            BaseApplicationException bex = new BaseApplicationException("something wrong - invalid CollectAnalytics_OmitEvent config param ?");
            bex.AddAdditionalInformation("_config._CollectAnalytics_OmitEvents", _config._CollectAnalytics_OmitEvents);
            throw bex;
        }

    }


    public static string AppPoolName
    {
        get
        {
            return _config._AppPoolName;
        }
    }


    public static int SaleType
    {
        get
        {
            return _config._SaleType;
        }
    }

    string[] newPlayerRealmIDs = null;
    public static string[] NewPlayerRealmIDs
    {
        get
        {
            if (_config.newPlayerRealmIDs == null)
            {
                string ids = _config._NewPlayerRealmID;
                _config.newPlayerRealmIDs = ids.Split(',');
            }

            return _config.newPlayerRealmIDs;
        }
    }

    string[] newMobilePlayerRealmIDs = null;
    public static string[] NewMobilePlayerRealmID
    {
        get
        {
            if (_config.newMobilePlayerRealmIDs == null)
            {
                string ids = _config._NewMobilePlayerRealmID;
                _config.newMobilePlayerRealmIDs = ids.Split(',');
            }

            return _config.newMobilePlayerRealmIDs;
        }
    }

    public static string awsSecretKey
    {
        get
        {
            return _config._awsSecretKey;
        }
    }
    public static string awsAccessKey
    {
        get
        {
            return _config._awsAccessKey;
        }
    }
    public static string addressToBCCSomeEmailsTo
    {
        get
        {
            return _config._addressToBCCSomeEmailsTo;
        }
    }
    


    public static double LatestAppVer_Android
    {
        get
        {
            return _config._LatestAppVer_Android;
        }
    }
    public static double LatestAppVer_iOS
    {
        get
        {
            return _config._LatestAppVer_iOS;
        }
    }
    public static bool AmazonRVSSandbox
    {
        get
        {
            return _config._AmazonRVSSandbox;
        }
    }
    public static CONSTS.Device? OverrideDeviceType
    {
        get
        {
            return (CONSTS.Device?)_config._overrideDeviceType;
        }
    }


    public static int FriendReward_RewardTheInviterGets
    {
        get
        {
            return _config._FriendReward_RewardTheInviterGets == null ? 30 : (int)_config._FriendReward_RewardTheInviterGets;
        }
    }
    public static int FriendReward_RewardTheInviteeGets
    {
        get
        {
            return _config._FriendReward_RewardTheInviteeGets == null ? 10 : (int)_config._FriendReward_RewardTheInviteeGets;
        }
    }
    public static DateTime FriendReward_SaleEndsOn
    {
        get
        {
            return _config._FriendReward_SaleEndsOn == null ? DateTime.Now.AddYears(-1) : (DateTime)_config._FriendReward_SaleEndsOn;
        }
    }


  
    #region public properties the define the config items we are looking for
    [ConfigurationProperty("ShowErrorLog", IsRequired = false)]
    public bool _ShowErrorLog
    {
        get
        {
            return (bool)this["ShowErrorLog"];
        }
    }
    [ConfigurationProperty("PFSystemActive", IsRequired = false)]
    public bool _PFSystemActive
    {
        get
        {
            return (bool)this["PFSystemActive"];
        }
    }

    [ConfigurationProperty("BaseUrl", IsRequired = true)]
    public string _BaseUrl
    {
        get
        {
            return (string)this["BaseUrl"];
        }
    }
    [ConfigurationProperty("InDevEnvironment", IsRequired = false)]
    public bool _InDevEnvironment
    {
        get
        {
            return (bool)this["InDevEnvironment"];
        }
    }
    [ConfigurationProperty("Theme", IsRequired = false)]
    public string _Theme
    {
        get
        {
            return (string)this["Theme"];
        }
    }
    [ConfigurationProperty("CollectGoogleAnalyticsOnRealm", IsRequired = false)]
    public int _CollectGoogleAnalyticsOnRealm
    {
        get
        {
            return (int)this["CollectGoogleAnalyticsOnRealm"];
        }
    }
    [ConfigurationProperty("AppPoolName", IsRequired = false)]
    public string _AppPoolName
    {
        get
        {
            return (string)this["AppPoolName"];
        }
    }
 
    [ConfigurationProperty("SaleType", IsRequired = false)]
    public int _SaleType
    {
        get
        {
            return (int)this["SaleType"];
        }
    }
    [ConfigurationProperty("NewPlayerRealmID", IsRequired = true)]
    public string _NewPlayerRealmID
    {
        get
        {
            return (string)this["NewPlayerRealmID"];
        }
    }
    [ConfigurationProperty("NewMobilePlayerRealmID", IsRequired = true)]
    public string _NewMobilePlayerRealmID
    {
        get
        {
            return (string)this["NewMobilePlayerRealmID"];
        }
    }


    [ConfigurationProperty("awsSecretKey", IsRequired = false)]
    public string _awsSecretKey
    {
        get
        {
            return (string)this["awsSecretKey"];
        }
    }
    [ConfigurationProperty("awsAccessKey", IsRequired = false)]
    public string _awsAccessKey
    {
        get
        {
            return (string)this["awsAccessKey"];
        }
    }
    [ConfigurationProperty("addressToBCCSomeEmailsTo", IsRequired = false)]
    public string _addressToBCCSomeEmailsTo
    {
        get
        {
            return (string)this["addressToBCCSomeEmailsTo"];
        }
    }
    [ConfigurationProperty("CollectAnalyticsOnRealms", IsRequired = false)]
    public string _CollectAnalyticsOnRealms
    {
        get
        {
            return (string)this["CollectAnalyticsOnRealms"];
        }
    }
    [ConfigurationProperty("CollectAnalyticsOnRealms_People", IsRequired = false)]
    public string _CollectAnalyticsOnRealms_People
    {
        get
        {
            return (string)this["CollectAnalyticsOnRealms_People"];
        }
    }
    [ConfigurationProperty("CollectAnalytics_OmitEvents", IsRequired = false)]
    public string _CollectAnalytics_OmitEvents
    {
        get
        {
            return (string)this["CollectAnalytics_OmitEvents"];
        }
    }
    [ConfigurationProperty("CollectAnalytics_MixPanelID", IsRequired = false)]
    public string _CollectAnalytics_MixPanelID
    {
        get
        {
            return ((string)this["CollectAnalytics_MixPanelID"]).Trim();
        }
    }

    [ConfigurationProperty("LatestAppVer_Android", IsRequired = false)]
    public double _LatestAppVer_Android
    {
        get
        {
            return Convert.ToDouble((this["LatestAppVer_Android"]));
        }
    }
    [ConfigurationProperty("LatestAppVer_iOS", IsRequired = false)]
    public double _LatestAppVer_iOS
    {
        get
        {
            return Convert.ToDouble((this["LatestAppVer_iOS"]));
        }
    }
    [ConfigurationProperty("AmazonRVSSandbox", IsRequired = false)]
    public bool _AmazonRVSSandbox
    {
        get
        {
            return Convert.ToBoolean(this["AmazonRVSSandbox"]);
        }
    }

    [ConfigurationProperty("OverrideDeviceType", IsRequired = false)]
    public int? _overrideDeviceType
    {
        get
        {
            return (int?)this["OverrideDeviceType"];
        }
    }
    [ConfigurationProperty("FriendReward_RewardTheInviterGets", IsRequired = false)]
    public int? _FriendReward_RewardTheInviterGets
    {
        get
        {
            return (int?)this["FriendReward_RewardTheInviterGets"];
        }
    }
    [ConfigurationProperty("FriendReward_RewardTheInviteeGets", IsRequired = false)]
    public int? _FriendReward_RewardTheInviteeGets
    {
        get
        {
            return (int?)this["FriendReward_RewardTheInviteeGets"];
        }
    }
    [ConfigurationProperty("FriendReward_SaleEndsOn", IsRequired = false)]
    public DateTime? _FriendReward_SaleEndsOn
    {
        get
        {
            return (DateTime?)this["FriendReward_SaleEndsOn"];
        }
    }
      
    #endregion
} 
 
