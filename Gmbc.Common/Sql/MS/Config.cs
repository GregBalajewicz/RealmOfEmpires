using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;  
using Gmbc.Common.Diagnostics.ExceptionManagement;


namespace Gmbc.Common.Sql.MS
{
	internal class Config : IConfigurationSectionHandler, ISerializableToNameValueCollection {
		private static NameValueCollection colSettings;
		private static Boolean bSettingsLoaded = false;


		private static string CONFIG_SECTION = "Gmbc.Common.Sql.MS";
		private static string COMMAND_TIMEOUT= "CommandTimeout";	//optional 
		private static string RAISE_ERROR_IF_NOT_DISPOSED= "RaiseErrorIfConnectionNotProperlyDisposed";	//optional 

		/// <summary>
		/// If the configuration is not provided for this, then returns -1
		/// </summary>
		public static int CommandTimeout 
		{
			get{
				LoadSetting();
				return ReadSetting(COMMAND_TIMEOUT, -1);
			}
		}

		public static bool RaiseErrorIfConnectionNotProperlyDisposed 
		{
			get
			{
				LoadSetting();
				return ReadSetting(RAISE_ERROR_IF_NOT_DISPOSED, false);
			}
		}


		/// <summary>
		/// This function loads the setting from the config file for later use.
		/// 
		///  IF YOU ADD A NEW CONFIG ITEM WHICH IS NOT OPTIONAL, ADD A CHECK HERE TO
		///  MAKE SURE IT IS IN THE config FILE !!!
		///  
		/// </summary>
		private static void LoadSetting() 
		{
			if (!bSettingsLoaded) {
				System.Configuration.ConfigurationSettings.GetConfig(CONFIG_SECTION);
			} 
			bSettingsLoaded = true;
		}

		//
		// Helper functions used to read a setting and convert it into the right format 
		//
		private static Boolean ReadSetting(String key, Boolean defaultValue) {
			try {
				Object setting = colSettings[key];

				return (setting == null) ? defaultValue : Convert.ToBoolean(setting);
			}
			catch {
				return defaultValue;
			}
		}

		private static int ReadSetting(String key, int defaultValue) {
			try {
				Object setting = colSettings[key];

				return (setting == null) ? defaultValue : Convert.ToInt32(setting);
			}
			catch {
				return defaultValue;
			}
		}

		private static String ReadSetting(String key, String defaultValue) {
			try {
				Object setting = colSettings[key];

				return (setting == null) ? defaultValue : (String)setting;
			}
			catch {
				return defaultValue;
			}
		}

		//
		// called by the .net runtime. Do not touch
		//
		public Object Create(Object parent, object configContext, XmlNode section) {
			try {
				NameValueSectionHandler baseHandler = new NameValueSectionHandler();
				colSettings = (NameValueCollection)baseHandler.Create(parent, configContext, section);
			}
			catch {
				colSettings = null;
			}


			return colSettings;			
		}

		/// <summary>
		/// Helper function. Pass it a NameValueCollection object and this function will add all its relevant
		/// attributes to the collection. This is used mostly for error handling and such when you need to get the state
		/// of the configuration quickly
		/// </summary>
		/// <param name="col">cannot be null, must be a valid NameValueCollection</param>
		public void SerializeToNameValueCollection(NameValueCollection col){
			string sClassName = this.GetType().ToString();
			col.Add(sClassName + ".CONFIG_SECTION", CONFIG_SECTION);
			col.Add(sClassName + ".COMMAND_TIMEOUT", COMMAND_TIMEOUT);
			col.Add(sClassName + ".RAISE_ERROR_IF_NOT_DISPOSED", RAISE_ERROR_IF_NOT_DISPOSED);
			
			try 
			{
				col.Add(sClassName + ".CommandTimeout", CommandTimeout.ToString());
				col.Add(sClassName + ".RaiseErrorIfConnectionNotProperlyDisposed", RaiseErrorIfConnectionNotProperlyDisposed.ToString());
			} 
			catch (Exception e)
			{
				col.Add(sClassName + " Error occured in SerializeToNameValueCollection. Exception.Message=", e.Message);
			}
		}

		private static bool m_bAddedToExceptionManager=false;
		public static void AddToExceptionManager(){
			if (!m_bAddedToExceptionManager) {
				ExceptionManager.AddISerializeToNameValueCollectionObject(new Config());
				m_bAddedToExceptionManager = true; 
			}
		}

	}
}



