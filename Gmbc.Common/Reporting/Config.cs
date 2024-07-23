using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;  
using System.Web;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Reporting
{
	internal class Config : IConfigurationSectionHandler, ISerializableToNameValueCollection 
	{
		private static NameValueCollection colSettings;
		private static Boolean bSettingsLoaded = false;
		private static bool m_bAddedToExceptionManager=false;



		private static string CONFIG_SECTION = "Gmbc.Common.Reporting";
		private static string REPORT_DIR = "ReportDirectory";	//mandatory

		/// <summary>
		/// Get the base report directory as specified in the .config file
		/// </summary>
		public static String ReportDirectory 
		{
			get
			{
				LoadSetting();
				string s = ReadSetting(REPORT_DIR, "");

				if (s.Substring(0,1) == "~") 
				{
					HttpContext context = HttpContext.Current;
					if (context == null) 
					{
						Exception ex = new Exception("Error while reading the configuration file. The value ("+ s +") of key '" 
							+ REPORT_DIR + "' begins with a '~' but a '~' can only be specifed for web application / web services");
						throw ex;
					}

					s = context.Server.MapPath(s);
				}

				// ensure the path ends with a '\'
				return s.EndsWith(@"\") ? s : s + @"\";
			}
		}

		/// <summary>
		/// Get xslt file name that represents the report identified by the parameter 'ReportName'. A mapping
		/// between a report name and the corresponding XSLT file is made in the .config file.
		/// </summary>
		/// <param name="ReportName">Name of a report. Must be the same as in the .config file</param>
		/// <returns>file name of the report. If report with this name cannot be found, an exception is raised</returns>
		/// <remarks>
		/// Although not enforced, it is recommended that the file name returned here is relative to the path
		/// specified with the ReportDiectory property (<see cref="Config.ReportDirectory"/>). ie, 
		/// Config.ReportDirectory + Config.ReportXsltFile("SomeReportName") should give you a pull path to the 
		/// xslt file
		/// </remarks>
		/// <exception cref="Exception">If report with the name specified cannot be found, an exception is raised</exception>
		public static String ReportXsltFile(string ReportName)
		{
			System.Configuration.ConfigurationSettings.GetConfig(CONFIG_SECTION);

			if (ReadSetting(ReportName, "") == "") {
				Exception ex = new Exception("While reading the " + CONFIG_SECTION + " configuration section, could not find key (ie report name)" + ReportName);
				throw(ex);
			}

			return ReadSetting(ReportName, "");
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
			if (!bSettingsLoaded) 
			{
				System.Configuration.ConfigurationSettings.GetConfig(CONFIG_SECTION);
				//
				// Check to see if all the not optional keys exist. 
				//
				//  EDIT THIS IF YOU ADD A NEW KEY!!!
				//
				if (ReadSetting(REPORT_DIR, "") == "") 
				{
					Exception ex = new Exception("While reading the " + CONFIG_SECTION 
						+ " configuration section, found the " + REPORT_DIR + " key to missing or empty");
					throw(ex);
				}
			} 
			bSettingsLoaded = true;
		}

		//
		// Helper functions used to read a setting and convert it into the right format 
		//
		private static Boolean ReadSetting(String key, Boolean defaultValue) 
		{
			try 
			{
				Object setting = colSettings[key];

				return (setting == null) ? defaultValue : Convert.ToBoolean(setting);
			}
			catch 
			{
				return defaultValue;
			}
		}

		private static String ReadSetting(String key, String defaultValue) 
		{
			try 
			{
				Object setting = colSettings[key];

				return (setting == null) ? defaultValue : (String)setting;
			}
			catch 
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// called by the .net runtime. Do not invoke explicitly
		/// </summary>
		public Object Create(Object parent, object configContext, XmlNode section) 
		{
			try 
			{
				NameValueSectionHandler baseHandler = new NameValueSectionHandler();
				colSettings = (NameValueCollection)baseHandler.Create(parent, configContext, section);
			}
			catch 
			{
				colSettings = null;
			}


			return colSettings;			
		}

		/// <summary>
		/// Pass it a NameValueCollection object and this function will add all its relevant
		/// attributes to the collection. This is used mostly for error handling and such when you need to get the state
		/// of the configuration quickly
		/// <seealso cref="Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection"/>
		/// </summary>
		/// <example>
		/// <pre>
		///	try {
		///		// some code which can throw an exception 
		///	} catch (Exception e) {
		///		BaseApplicationException ex = new BaseApplicationException("Error", e);
		///		ex.AdditionalInformation.Add("Value of SomeVariable", SomeVariable);
		///		MyConfigurationClass.SerializeToNameValueCollection(ex.AdditionalInformation);
		///		throw ex;
		///	}
		/// </pre>
		/// </example>
		/// <param name="col">cannot be null, must be a valid NameValueCollection</param>
		public void SerializeToNameValueCollection(NameValueCollection col)
		{
			string sClassName = this.GetType().ToString();
			col.Add(sClassName + ".CONFIG_SECTION", CONFIG_SECTION);
			col.Add(sClassName + ".REPORT_DIR", REPORT_DIR);
			
			try 
			{
				col.Add(sClassName + ".ReportDirectory", ReportDirectory);
			} 
			catch (Exception e)
			{
				col.Add(sClassName + " Error occured in SerializeToNameValueCollection. Exception.Message=", e.Message);
			}
		}


		/// <summary>
		/// Add this class to the ExceptionManager using
		/// Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.AddISerializeToNameValueCollectionObject.
		/// It is safe to call this method multiple times because it guarantees that 
		/// ExceptionManager.AddISerializeToNameValueCollectionObject is called only once
		/// <see cref="Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.AddISerializeToNameValueCollectionObject"/>
		/// </summary>
		public static void AddToExceptionManager()
		{
			if (!m_bAddedToExceptionManager) 
			{
				ExceptionManager.AddISerializeToNameValueCollectionObject(new Config());
				m_bAddedToExceptionManager = true; 
			}
		}

	}
}
