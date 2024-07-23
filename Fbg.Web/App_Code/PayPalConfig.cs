using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized; 


public class PayPalConfig : ConfigurationSection
{
    private static string _CONFIG_SECTION = "PayPal";

    #region singleton implementation
    private static PayPalConfig _config;
    static PayPalConfig()
    {
        _config = (PayPalConfig)ConfigurationSettings.GetConfig(_CONFIG_SECTION);
    }
    #endregion 

    public static string IPNPostUrl
    {
        get
        {
            return _config.IPN_POST_URL;
        }
    }
    public static string IPNNotifyUrl
    {
        get
        {
            return _config.IPN_NOTIFY_URL;
        }
    }
    public static string DonateReturnUrl
    {
        get
        {
            return _config.DONATE_RETURN_URL;
        }
    }
    public static string DonateReturnUrl_Step2
    {
        get
        {
            return _config.DONATE_RETURN_URL_STEP2;
        }
    }
    public static string PurchaseReturnUrl
    {
        get
        {
            return _config.PURCHASE_RETURN_URL;
        }
    }
    public static string PurchaseReturnUrl_Step2
    {
        get
        {
            return _config.PURCHASE_RETURN_URL_STEP2;
        }
    }
    public static string CancelUrl
    {
        get
        {
            return _config.CANCEL_URL;
        }
    }
    public static string OurPrimaryEmail
    {
        get
        {
            return _config.OUR_PRIMARY_EMAIL;
        }
    }

    public static string EncryptedBusinessName
    {
        get
        {
            return _config.ENCRYPTED_BUSINESS_NAME;
        }
    }
    public static string BaseUrl
    {
        get
        {
            return _config.BASE_URL;
        }
    }
    public static string Currency
    {
        get
        {
            return _config.CURRENCY;
        }
    }

    public static string TrialPayNotificationKey
    {
        get
        {
            return _config._TrialPayNotificationKey;
        }
    }


    #region public properties the define the config items we are looking for
    [ConfigurationProperty("IPNPostUrl", IsRequired = true)]
    public string IPN_POST_URL
    {
        get
        {
            return _config["IPNPostUrl"] as string;
        }
    }

    [ConfigurationProperty("IPNNotifyUrl", IsRequired = true)]
    public string IPN_NOTIFY_URL
    {
        get
        {
            return _config["IPNNotifyUrl"] as string;
        }
    }



    [ConfigurationProperty("donateReturnUrl", IsRequired = true)]
    public string DONATE_RETURN_URL
    {
        get
        {
            return _config["donateReturnUrl"] as string;
        }
    }
    [ConfigurationProperty("donateReturnUrl_Step2", IsRequired = true)]
    public string DONATE_RETURN_URL_STEP2
    {
        get
        {
            return _config["donateReturnUrl_Step2"] as string;
        }
    }

    [ConfigurationProperty("purchaseReturnUrl", IsRequired = true)]
    public string PURCHASE_RETURN_URL
    {
        get
        {
            return _config["purchaseReturnUrl"] as string;
        }
    }
    [ConfigurationProperty("purchaseReturnUrl_Step2", IsRequired = true)]
    public string PURCHASE_RETURN_URL_STEP2
    {
        get
        {
            return _config["purchaseReturnUrl_Step2"] as string;
        }
    }


    [ConfigurationProperty("cancelUrl", IsRequired = true)]
    public string CANCEL_URL
    {
        get
        {
            return _config["cancelUrl"] as string;
        }
    }


    [ConfigurationProperty("OurPrimaryEmail", IsRequired = true)]
    public string OUR_PRIMARY_EMAIL
    {
        get
        {
            return _config["OurPrimaryEmail"] as string;
        }
    }
    [ConfigurationProperty("EncryptedBusinessName", IsRequired = true)]
    public string ENCRYPTED_BUSINESS_NAME
    {
        get
        {
            return _config["EncryptedBusinessName"] as string;
        }
    }

    [ConfigurationProperty("BaseUrl", IsRequired = true)]
    public string BASE_URL
    {
        get
        {
            return _config["BaseUrl"] as string;
        }
    }

    [ConfigurationProperty("Currency", IsRequired = true)]
    public string CURRENCY
    {
        get
        {
            return _config["Currency"] as string;
        }
    }

    [ConfigurationProperty("TrialPayNotificationKey", IsRequired = false)]
    public string _TrialPayNotificationKey
    {
        get
        {
            return (string)this["TrialPayNotificationKey"];
        }
    }




#endregion
} 
 
