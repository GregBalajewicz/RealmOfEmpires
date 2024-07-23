using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;  
using System.Web;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Diagnostics 
{ 
	internal class Config : IConfigurationSectionHandler 
	{ 
		private static Boolean bSettingsLoaded = false;

		private static string CONFIG_SECTION = "Gmbc.Common.Diagnostics"; 

		/// <summary>
		/// This function loads the setting from the config file for later use.
		/// </summary>
		private static void LoadSetting() 
		{
			if (!bSettingsLoaded) 
			{
				try 
				{
					System.Configuration.ConfigurationSettings.GetConfig(CONFIG_SECTION);
				} 
				catch (Exception e) 
				{
					throw new BaseApplicationException("Error reading config section '" + CONFIG_SECTION + "'", e);
				}
			} 
			bSettingsLoaded = true;
		}

		//
		// called by the .net runtime.
		//
		public Object Create(Object parent, object configContext, XmlNode section) 
		{
			TextFileLogger.ReadSetting(section);
			EmailLogger.ReadSetting(section);
			WindowsEventLogger.ReadSetting(section);
            DBLogger.ReadSetting(section);

			return configContext;			
		}


		/// <summary>
		/// Helper function. Pass it a NameValueCollection object and this function will add all its relevant
		/// attributes to the collection. This is used mostly for error handling and such when you need to get the state
		/// of the configuration quickly
		/// </summary>
		/// <param name="col">cannot be null, must be a valid NameValueCollection</param>
		internal static void SerializeToNameValueCollection(NameValueCollection col)
		{
			col.Add("Gmbc.Common.Diagnostics.Config.CONFIG_SECTION", CONFIG_SECTION);
			TextFileLogger.SerializeToNameValueCollection(col);
			EmailLogger.SerializeToNameValueCollection(col);
			WindowsEventLogger.SerializeToNameValueCollection(col);
            DBLogger.SerializeToNameValueCollection(col);
		}


		#region TextFileLogger class
			//
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
			// TextFileLogger class
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
			//
			internal class TextFileLogger 
			{
				#region member variables and such
				private static string m_sFileName;
				private static bool m_bUseForPublish = true;
				#endregion

				#region Properties
				internal static string FileName 
				{
					get 
					{
						Config.LoadSetting();
						return m_sFileName;
					}
				}
				internal static bool UseForPublish 
				{
					get 
					{
						Config.LoadSetting();
						return m_bUseForPublish;
					}
				}
				#endregion

				static TextFileLogger() 
				{
					//
					// If text file logger setting were not provided, set the default log name to be 
					// ErrorLog.txt in the base application directory
					//
					m_sFileName = @".\ApplicationError.log";
					HttpContext context = HttpContext.Current;
					if (context != null) 
					{
						m_sFileName = context.Server.MapPath(m_sFileName);
					}
				}

				internal static void ReadSetting(XmlNode section) 
				{
					XmlNode node = section.SelectSingleNode("TextFileLogger");
					if (node != null) 
					{
						//
						// FileName attribute
						XmlAttribute attribute = node.Attributes["FileName"];
						if (attribute == null || attribute.Value == "") 
						{
							BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for TextFileLogger. Found TextFileLogger node but no FileName attribute, or FileName attribute is empty.");
							e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
							throw e;
						} 
						else 
						{
							m_sFileName = attribute.Value;

							if (attribute.Value.Substring(0,1) == "~") 
							{
								HttpContext context = HttpContext.Current;
								if (context == null) 
								{
									CustomConfigKeyMissingOrInvalidException ex = new CustomConfigKeyMissingOrInvalidException("Error while reading the configuration file. The value ("+ attribute.Value +") of key 'FileName' attribute of 'TextFileLogger' node is a filename. It begins with a '~' but a '~' can only be specifed for web application / web services");
									ex.ConfigSectionName = CONFIG_SECTION;
									ex.MissingConfigKeyName = "TextFileLogger node, FileName attribute";
									ex.MissingConfigKeyValue = attribute.Value;
									throw ex;
								}

								m_sFileName = context.Server.MapPath(m_sFileName);
							}

							
						}

						//
						// UseForPublish attribute
						attribute = node.Attributes["UseForPublish"];
						if (attribute == null) 
						{
							m_bUseForPublish = false;
						} 
						else if (attribute.Value != "false" && attribute.Value != "true")
						{	
							BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for TextFileLogger. Found TextFileLogger and UseForPublish attribute, but UseForPublish attribute has a value other than 'true' or 'false'.");
							e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
							throw e;
						} 
						else 
						{
							m_bUseForPublish = Convert.ToBoolean(attribute.Value);
						}
					}
				}

				/// <summary>
				/// Helper function. Pass it a NameValueCollection object and this function will add all its relevant
				/// attributes to the collection. This is used mostly for error handling and such when you need to get the state
				/// of the configuration quickly
				/// </summary>
				/// <param name="col">cannot be null, must be a valid NameValueCollection</param>
				internal static void SerializeToNameValueCollection(NameValueCollection col)
				{
					col.Add("Gmbc.Common.Diagnostics.Config.TextFileLogger.FileName", m_sFileName);
					col.Add("Gmbc.Common.Diagnostics.Config.TextFileLogger.UseForPublish", m_bUseForPublish.ToString());
				}

			}
			#endregion //TextFileLogger class

		#region EmailLogger class
			//
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
			// TextFileLogger class
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
			//
			internal class EmailLogger 
			{
				#region member variables and such
				private static string m_sToEmailAddress = "";
				private static string m_sFromEmailAddress = "";
				private static string m_sSubjectLine = "";
				private static bool m_bUseForPublish = false;
				#endregion

				#region Properties
				internal static string ToEmailAddress 
				{
					get 
					{
						Config.LoadSetting();
						return m_sToEmailAddress;
					}
				}
				internal static string FromEmailAddress 
				{
					get 
					{
						Config.LoadSetting();
						return m_sFromEmailAddress;
					}
				}
				internal static string SubjectLine 
				{
					get 
					{
						Config.LoadSetting();
						return m_sSubjectLine;
					}
				}

				internal static bool UseForPublish 
				{
					get 
					{
						Config.LoadSetting();
						return m_bUseForPublish;
					}
				}
				#endregion

				internal static void ReadSetting(XmlNode section) 
				{
					XmlNode node = section.SelectSingleNode("EmailLogger");
					if (node != null) 
					{
						XmlAttribute attribute = node.Attributes["ToEmailAddress"];
						if (attribute == null || attribute.Value == "") 
						{
							BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for EmailLogger. Found EmailLogger node but no ToEmailAddress attribute, or ToEmailAddress attribute is empty.");
							e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
							throw e;
						} 
						else 
						{
							m_sToEmailAddress = attribute.Value;
						}

						attribute = node.Attributes["FromEmailAddress"];
						if (attribute == null || attribute.Value == "") 
						{
							BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for EmailLogger. Found EmailLogger node but no FromEmailAddress attribute, or FromEmailAddress attribute is empty.");
							e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
							throw e;
						} 
						else 
						{
							m_sFromEmailAddress = attribute.Value;
					
						}
						attribute = node.Attributes["SubjectLine"];
						if (attribute == null ) 
						{
							BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for EmailLogger. Found EmailLogger node but no SubjectLine attribute.");
							e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
							throw e;
						} 
						else 
						{
							m_sSubjectLine = attribute.Value;
						}

						attribute = node.Attributes["UseForPublish"];
						if (attribute == null) 
						{
							m_bUseForPublish = false;
						} 
						else if (attribute.Value != "false" && attribute.Value != "true") 
						{
							BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for EmailLogger. Found EmailLogger and UseForPublish attribute but UseForPublish attribute has a value other than 'true' or 'false'.");
							e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
							throw e;
						} 
						else 
						{
							m_bUseForPublish = Convert.ToBoolean(attribute.Value);
						}
					}
				}

				/// <summary>
				/// Helper function. Pass it a NameValueCollection object and this function will add all its relevant
				/// attributes to the collection. This is used mostly for error handling and such when you need to get the state
				/// of the configuration quickly
				/// </summary>
				/// <param name="col">cannot be null, must be a valid NameValueCollection</param>
				internal static void SerializeToNameValueCollection(NameValueCollection col)
				{
					col.Add("Gmbc.Common.Diagnostics.Config.EmailLogger.ToEmailAddress",m_sToEmailAddress );
					col.Add("Gmbc.Common.Diagnostics.Config.EmailLogger.FromEmailAddress", m_sFromEmailAddress);
					col.Add("Gmbc.Common.Diagnostics.Config.EmailLogger.SubjectLine", m_sSubjectLine);
					col.Add("Gmbc.Common.Diagnostics.Config.EmailLogger.UseForPublish", m_bUseForPublish.ToString());
				}

			}
			#endregion EmailLogger class

		#region WindowsEventLogger class
		//
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
		// WindowsEventLogger class
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
		//
		internal class WindowsEventLogger 
		{
			#region member variables and such
			private static string m_sSource = "";
			private static string m_sLog = "";
			private static bool m_bUseForPublish = false;
			#endregion

			#region Properties
			internal static string Source 
			{
				get 
				{
					Config.LoadSetting();
					return m_sSource;
				}
			}
			internal static string Log 
			{
				get 
				{
					Config.LoadSetting();
					return m_sLog;
				}
			}
			internal static bool UseForPublish 
			{
				get 
				{
					Config.LoadSetting();
					return m_bUseForPublish;
				}
			}
			#endregion

			internal static void ReadSetting(XmlNode section) 
			{
				XmlNode node = section.SelectSingleNode("WindowsEventLogger");
				if (node != null) 
				{
					XmlAttribute attribute = node.Attributes["Source"];
					if (attribute == null || attribute.Value == "") 
					{
						BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for WindowsEventLogger. Found WindowsEventLogger node but no Source attribute, or Source attribute is empty.");
						e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
						throw e;
					} 
					else 
					{
						m_sSource = attribute.Value;
					}

					attribute = node.Attributes["Log"];
					if (attribute == null || attribute.Value == "") 
					{
						BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for WindowsEventLogger. Found WindowsEventLogger node but no Log attribute, or Log attribute is empty.");
						e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
						throw e;
					} 
					else 
					{
						m_sLog = attribute.Value;
					}

					attribute = node.Attributes["UseForPublish"];
					if (attribute == null) 
					{
						m_bUseForPublish = false;
					} 
					else if (attribute.Value != "false" && attribute.Value != "true") 
					{
						BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for WindowsEventLogger. Found WindowsEventLogger and UseForPublish attribute but UseForPublish attribute has a value other than 'true' or 'false'.");
						e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
						throw e;
					} 
					else 
					{
						m_bUseForPublish = Convert.ToBoolean(attribute.Value);
					}
				}
			}

			/// <summary>
			/// Helper function. Pass it a NameValueCollection object and this function will add all its relevant
			/// attributes to the collection. This is used mostly for error handling and such when you need to get the state
			/// of the configuration quickly
			/// </summary>
			/// <param name="col">cannot be null, must be a valid NameValueCollection</param>
			internal static void SerializeToNameValueCollection(NameValueCollection col)
			{
				col.Add("Gmbc.Common.Diagnostics.Config.WindowsEventLogger.Source",Source );
				col.Add("Gmbc.Common.Diagnostics.Config.WindowsEventLogger.Log", Log);
				col.Add("Gmbc.Common.Diagnostics.Config.WindowsEventLogger.UseForPublish", m_bUseForPublish.ToString());
			}

		}
		#endregion WindowsEventLogger class

		#region DBLogger class
			//
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
			// DBLogger class
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
			//
			internal class DBLogger 
			{
				#region member variables and such
				private static string m_connectionString;
                private static string m_ErrorSource;
                private static bool m_bUseForPublish = false;
				#endregion

				#region Properties
                internal static string ErrorSource
                {
                    get
                    {
                        Config.LoadSetting();
                        return m_ErrorSource;
                    }
                }
                internal static string ConnectionString 
				{
					get 
					{
						Config.LoadSetting();
                        return m_connectionString;
					}
				}
				internal static bool UseForPublish 
				{
					get 
					{
						Config.LoadSetting();
						return m_bUseForPublish;
					}
				}
				#endregion

				static DBLogger() 
				{
					
				}

				internal static void ReadSetting(XmlNode section) 
				{
					XmlNode node = section.SelectSingleNode("DBLogger");
					if (node != null) 
					{
						//
						// conn string attribute
						XmlAttribute attribute = node.Attributes["ConnectionString"];
						if (attribute == null || attribute.Value == "") 
						{
                            BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for DBLogger. Found DBLogger node but no ConnectionString attribute, or ConnectionString attribute is empty.");
							e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
							throw e;
						} 
						else 
						{
							m_connectionString = attribute.Value;		
						}
                        //
                        // soiurce attribute
                         attribute = node.Attributes["ErrorSource"];
                        if (attribute == null || attribute.Value == "")
                        {
                            BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for DBLogger. Found DBLogger node but no ErrorSource attribute, or ErrorSource attribute is empty.");
                            e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
                            throw e;
                        }
                        else
                        {
                            m_ErrorSource= attribute.Value;
                        }

						//
						// UseForPublish attribute
						attribute = node.Attributes["UseForPublish"];
						if (attribute == null) 
						{
							m_bUseForPublish = false;
						} 
						else if (attribute.Value != "false" && attribute.Value != "true")
						{	
							BaseApplicationException e = new BaseApplicationException("Error while reading config section (" + CONFIG_SECTION + ") for DBLogger. Found DBLogger and UseForPublish attribute, but UseForPublish attribute has a value other than 'true' or 'false'.");
							e.AdditionalInformation.Add("section.InnerXml", section.InnerXml);
							throw e;
						} 
						else 
						{
							m_bUseForPublish = Convert.ToBoolean(attribute.Value);
						}
					}
				}

				/// <summary>
				/// Helper function. Pass it a NameValueCollection object and this function will add all its relevant
				/// attributes to the collection. This is used mostly for error handling and such when you need to get the state
				/// of the configuration quickly
				/// </summary>
				/// <param name="col">cannot be null, must be a valid NameValueCollection</param>
				internal static void SerializeToNameValueCollection(NameValueCollection col)
				{
					col.Add("Gmbc.Common.Diagnostics.Config.DBLogger.ConnectionString", m_connectionString);
					col.Add("Gmbc.Common.Diagnostics.Config.DBLogger.UseForPublish", m_bUseForPublish.ToString());
				}

			}
			#endregion //DBLogger class

	}
}
