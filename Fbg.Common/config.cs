using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;


namespace Fbg.Common
{
    public class Config : ConfigurationSection
    {
        private static string _CONFIG_SECTION = "Fbg.Common";

        #region singleton implementation
        private static Config _config;
        static Config()
        {
            _config = (Config)ConfigurationSettings.GetConfig(_CONFIG_SECTION);
        }
        #endregion

        public static string jsScriptFileDirectoryPostfix
        {
            get
            {
                return _config._jsScriptFileDirectoryPostfix;
            }
        }
        public static bool UseDebugFiles
        {
            get
            {
                return _config._UseDebugFiles;
            }
        }
       

        #region public properties the define the config items we are looking for
        [ConfigurationProperty("jsScriptFileDirectoryPostfix", IsRequired = false)]
        public string _jsScriptFileDirectoryPostfix
        {
            get
            {
                return (string)this["jsScriptFileDirectoryPostfix"];
            }
        }
        [ConfigurationProperty("UseDebugFiles", IsRequired = false)]
        public bool _UseDebugFiles
        {
            get
            {
                return (bool)this["UseDebugFiles"];
            }
        }
        #endregion
    }
}
