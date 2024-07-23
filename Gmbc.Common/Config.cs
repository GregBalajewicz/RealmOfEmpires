using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized; 

using Gmbc.Common.Diagnostics.ExceptionManagement; 
using Gmbc.Common.Configuration;

namespace Gmbc.Common
{
	internal class Config
	{ 
		private static string CONFIG_SECTION = "Gmbc.Common"; 
		private static string CONNECTION_TYPE = "ConnectionType";

		private static Gmbc.Common.Configuration.Config config;

		static Config() 
		{
			config = new Gmbc.Common.Configuration.Config(CONFIG_SECTION);
			config.AddSettingDefinition(CONNECTION_TYPE, Gmbc.Common.Configuration.Config.SettingType.String,true,null);
		}

		public enum ConnectionTypes
		{
			/// <summary>
			/// Must do all the data access through web services
			/// </summary>
			Remote,
			/// <summary>
			/// Were are on a local LAN, can do all the data access using OLEDB etc
			/// </summary>
			Local
		}
		public static ConnectionTypes ConnectionType
		{
			get
			{
				string s = (String)Config.config.GetSettingValue(CONNECTION_TYPE);
				switch(s) 
				{
					case "Local":
						return ConnectionTypes.Local;
					case "Remote":
						return ConnectionTypes.Remote;
					default:
						throw new Configuration.ConfigurationKeyInvalidException("Invalid value for '" 
							+ CONNECTION_TYPE + "' Key in config section '" + CONFIG_SECTION 
							+ "' in the configuration file. Allowed only 'Local' or 'Remote' values");
				}
			}
		}

		/// <summary>
		/// This function loads the setting from the config file for later use.
		/// </summary>
		private static void LoadSetting() 
		{
			Config.config.Initialize();
		}
	}
}