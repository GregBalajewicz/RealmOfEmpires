using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;


namespace Fbg.DAL
{
    public class Config : ConfigurationSection
    {
        private static string _CONFIG_SECTION = "Fbg.DAL";

        #region singleton implementation
        private static Config _config;
        static Config()
        {
            _config = (Config)ConfigurationSettings.GetConfig(_CONFIG_SECTION);
        }
        #endregion

        public static string LogDBConnectionStr
        {
            get
            {
                return _config._LogDBConnectionStr;
            }
        }
        public static string UMLOG_Dir
        {
            get
            {
                return _config._UMLOG_Dir;
            }
        }

        #region public properties the define the config items we are looking for
        [ConfigurationProperty("LogDBConnectionStr", IsRequired = false)]
        public string _LogDBConnectionStr
        {
            get
            {
                return (string)this["LogDBConnectionStr"];
            }
        }
        [ConfigurationProperty("UMLOG_Dir", IsRequired = false)]
        public string _UMLOG_Dir
        {
            get
            {
                return (string)this["UMLOG_Dir"];
            }
        }
        #endregion
    }
}
