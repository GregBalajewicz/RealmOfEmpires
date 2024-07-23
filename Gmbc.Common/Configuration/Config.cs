using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using System.Web;

using Gmbc.Common.Diagnostics.ExceptionManagement;

using Gmbc.Common.Diagnostics.Logging;

namespace Gmbc.Common.Configuration
{
	/// <summary>
	/// Encapsulates common functionality required to read the application configuration files.
	/// 
	/// Author: Greg Balajewicz
	/// 
	/// </summary>
	/// <example>
	///		<configuration>
	///				<configSections>
	///					<section name="ConfigSectionName" type="Gmbc.Common.Configuration.Config, Gmbc.Common"/> 
	///				</configSections>
	///				<ConfigSectionName>
	///					<add key="DefinitionFile" value="~\fins\BusinessRules.xml"/>
	///					<add key="OutputDirectory" value="~\output"/>
	///					<add key="WaitDelay" value="10"/>
	///					<add key="SpendingLimit" value="30.50"/>
	///					<add key="CreateExtensiveErrorLog" value="true"/>
	///				</ConfigSectionName>
	///		</configuration>
	///			
	///		// Create a new Config object. This object will be reading a configuration section called "ConfigSectionName"
	///		config = new Gmbc.Common.Configuration.Config("ConfigSectionName");
	///		//
	///		// Define settings / keys 
	///		//
	///		// This key is defined as FileName. Since it uses a '~' in the config file (above), 
	///		//		it will be considered a relative path, relative to virtual directory root
	///		config.AddSettingDefinition("DefinitionFile", Gmbc.Common.Configuration.Config.SettingType.FileName , true ,null);
	///		config.AddSettingDefinition("OutputDirectory", Gmbc.Common.Configuration.Config.SettingType.Directory, true ,null);
	///		// This in optional setting with a default value of 100
	///		config.AddSettingDefinition("WaitDelay", Gmbc.Common.Configuration.Config.SettingType.Integer, false , 100);
	///		config.AddSettingDefinition("OptionalMissingParam", Gmbc.Common.Configuration.Config.SettingType.Integer, false , 99);
	///		config.AddSettingDefinition("SpendingLimit", Gmbc.Common.Configuration.Config.SettingType.Double, true , null);
	///		config.AddSettingDefinition("CreateExtensiveErrorLog", Gmbc.Common.Configuration.Config.SettingType.Boolean, true , null);
	///		//
	///		// The commented lines below will case errors. Uncomment them if you want to see
	///		//
	///		//// This line will cause an exception during config.Initialize() since a key by this name does not exist
	///		//config.AddSettingDefinition("RequiredButMissingKey", Gmbc.Common.Configuration.Config.SettingType.Boolean, true , null);
	///		//// This line will throw an excetion since the default value of "wrongDefaultValue" is not of type Integer
	///		//config.AddSettingDefinition("OptionalKeyWithWrongDefault", Gmbc.Common.Configuration.Config.SettingType.Integer, false , "wrongDefaultValue");
	///		//
	///		// A call to this function will cause the object to load in the configuration file and validate it. 
	///		//		a call to this function is not required, since it is called automatically the first time you try to access
	///		//		methods such as GetSettingValue() however it is good practice to call it explicitly to make sure the config file is
	///		//		validated. Please note that if there is something wrong with the config file, this method will throw exceptions
	///		config.Initialize();
	///		//
	///		// Read the settings
	///		//
	///		// This will result in "[virtual_directory_path]\fins\BusinessRules.xml" if this is a web application. If this is not a web application, 
	///		//		config.Initialize() would have thrown an excetpion saying it does not know how to interpret "~"
	///		string fileName = (string)config.GetSettingValue("DefinitionFile");	
	///		// This will result in "[virtual_directory_path]\output\" if this is a web application (note the leading "\" - this was added by the system.
	///		string dir		= (string)config.GetSettingValue("OutputDirectory");	
	///		int delay		= (System.Int32)config.GetSettingValue("WaitDelay");	// will result in 10
	///		int optional	= (System.Int32)config.GetSettingValue("OptionalMissingParam");	// will result in 99
	///		double limit	= (System.Double)config.GetSettingValue("SpendingLimit");	// will result in 30.50
	///		bool log		= (System.Boolean)config.GetSettingValue("CreateExtensiveErrorLog");	// will result in true
	/// </example>
	public class Config : GmbcBaseClass, IConfigurationSectionHandler, IEnumerable, ISerializableToNameValueCollection 
	{
		private Boolean bSettingsLoaded = false;
		private string configurationSectionName;
		internal HybridDictionary configSettings;

		/// <summary>
		/// Specifies the type of a particular configuration setting
		/// </summary>
		public enum SettingType 
		{
			/// <summary>
			/// This setting/key is simply a string- internally it is represented by System.String
			/// </summary>
			String,
			/// <summary>
			/// This setting/key is an Integer (number) - internally it is represented by System.Int32
			/// </summary>
			Integer,
			/// <summary>
			/// This setting/key is a floating point number - internally it is represented by System.Double
			/// </summary>
			Double,
			/// <summary>
			/// This setting/key is a boolean value - internally it is represented by System.Boolean
			/// </summary>
			Boolean,
			/// <summary>
			/// This setting/key represents a directory. It is represented by System.String however
			/// there are additional checks / functionality for this type of a setting. 
			/// For this setting, a leading "\" is always added even if it is ommited in the config file. 
			/// Also, if you are running a web app/ web service, you can specify a web relative path in 
			/// your web.config file as such "~\MyDirectoryName" and the system will resolve this to an absolute path
			/// </summary>
			Directory,
			/// <summary>
			/// This setting/key represents a directory. It is represented by System.String however
			/// there are additional checks / functionality for this type of a setting. 
			/// For this setting,  if you are running a web app/ web service, you can specify a web relative path in 
			/// your web.config file as such "~\MyXmlFileName.xml" and the system will resolve this to an absolute path
			/// </summary>
			FileName
		}

		internal Config (){}	// this is needed by the runtime when the config section is being loaded

		/// <summary>
		/// 
		/// </summary>
		/// <param name="configurationSectionName">The name of the configuration section that this class will be reading</param>
		public Config(string configurationSectionName) {
			this.configurationSectionName = configurationSectionName;
			configSettings = new HybridDictionary(false);
			this.AddToExceptionManager();
		}


		/// <summary>
		/// Define a setting / key which appears in the config file. You must define each and every setting
		/// using this class before access any of these setting using either GetSetting(), GetSettingValue(), Initialize functions etc.
		/// 
		/// - If you do not define a setting here which appears in the config file, an exception will be raised.
		/// - If you define a setting here and label it as active and the setting cannot be found in the 
		///		config file, an exception will be raised.
		/// - settingName must be unique. 
		/// 
		/// Please note, the settings are only defined using this function. At this point, the configuration
		/// file is not loaded hence it is not yet validated
		/// </summary>
		/// <param name="settingName">Name of the setting. This is a _CASE SENSITIVE_ name of the settings key</param>
		/// <param name="settingType">Type of the setting</param>
		/// <param name="isSettingRequired">If the setting is required, the system will check to make 
		/// sure this settings exists in the configuration file and will throw and exception if it does not</param>
		/// <param name="settingDefaultValue">
		/// You can only specify a default value if setting is not required, ie if isSettingRequired == false
		/// 
		/// Also, if settingDefaultValue != null then it must be of the type of the setting (ie settingType parameter). 
		///		For example, if settingType == SettingType.Integer, this function will try Convert.ToInt32(settingDefaultValue) to make sure it is an integer
		///		For SettingType.Boolean, Convert.ToBoolean() is used. For SettingType.Double, Convert.ToDouble() is used. 
		///		For SettingType.Directiory/FileName/String, Convert.ToString() is used</param> 
		public void AddSettingDefinition(string settingName, SettingType settingType, bool isSettingRequired, object settingDefaultValue) 
		{
			//
			// Validate parameters
			//
			if (this.configSettings[settingName] != null) 
			{
				ArgumentException e = new ArgumentException("A setting with the specified name already exists. You cannot have two settings with the same name","string settingName");
				TRACE.ErrorLine(ExceptionManager.SerializeToString(e), TRACE_CAT);
				throw e;
			}
			if (settingDefaultValue != null && isSettingRequired) 
			{
				ArgumentException e = new ArgumentException("Cannot specify a default value for a required setting. For required settings, set the settingDefaultValue to 'null'","");
				TRACE.ErrorLine(ExceptionManager.SerializeToString(e), TRACE_CAT);
				throw e;
			}

			//
			// Validate the default value
			//
			#region Validate the default value
			if (settingDefaultValue != null) 
			{
				switch(settingType) 
				{
					case SettingType.Boolean:
						try 
						{
							settingDefaultValue = Convert.ToBoolean(settingDefaultValue);
						} 
						catch 
						{
							ArgumentException ex = new ArgumentException("The default value specified ("+ settingDefaultValue.ToString() +"), for setting (" + settingName + ") is not of the type of the setting (" + settingType.ToString() +") ", "object settingDefaultValue");
							TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
							throw (ex);
						}
						break;
					case SettingType.Double:
						try 
						{
							settingDefaultValue = Convert.ToDouble(settingDefaultValue);
						} 
						catch 
						{
							ArgumentException ex = new ArgumentException("The default value specified ("+ settingDefaultValue.ToString() +"), for setting (" + settingName + ") is not of the type of the setting (" + settingType.ToString() +") ", "object settingDefaultValue");
							TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
							throw (ex);
						}
						break;
					case SettingType.Integer:
						try 
						{
							settingDefaultValue = Convert.ToInt32(settingDefaultValue);
						} 
						catch 
						{
							ArgumentException ex = new ArgumentException("The default value specified ("+ settingDefaultValue.ToString() +"), for setting (" + settingName + ") is not of the type of the setting (" + settingType.ToString() +") ", "object settingDefaultValue");
							TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
							throw (ex);
						}
						break;
					case SettingType.Directory:
					case SettingType.FileName:
					case SettingType.String:
						try 
						{
							settingDefaultValue = Convert.ToString(settingDefaultValue);
						} 
						catch 
						{
							ArgumentException ex = new ArgumentException("The default value specified ("+ settingDefaultValue.ToString() +"), for setting (" + settingName + ") is not of the type of the setting (" + settingType.ToString() +") ", "object settingDefaultValue");
							TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
							throw (ex);
						}
						break;
					default:
						throw new NotImplementedException("ERROR: Unrecognized SettingType of " + settingType.ToString());
				}
			}
			#endregion

			//
			// Add the setting to the settings collection
			//
			ConfigSetting configSetting = new ConfigSetting(settingName, settingType, isSettingRequired, settingDefaultValue, null);
			configSettings.Add(settingName, configSetting);
		}

		/// <summary>
		/// Returns the value of the settings specified by the 'settingName' parameter. See Remarks for additional information
		/// </summary>
		/// <param name="settingName"></param>
		/// <returns>See remarks</returns>
		/// <remarks>
		/// This function returns the value of the setting. If the value is specified in the configuration
		/// file, this value is returned. If it is not, the default value is returned.
		/// 
		/// It is always [with one exception, see below] safe to cast the return value of this function
		/// to the type of the setting. Here is the mapping of the safe casts:
		/// IF setting type if SettingType.Integer THEN cast to System.Int32
		/// IF setting type if SettingType.Double THEN cast to System.Double
		/// IF setting type if SettingType.Boolean THEN cast to System.Boolean
		/// IF setting type if SettingType.String/Directory/FileName THEN cast to System.String
		/// 
		/// This is how to cast: int MyValue = (Int32)config.GetSettingValue("somesetting");
		/// 
		/// One exception to the cast rule is when an optional setting is used. If an option setting
		///		is defined using AddSettingDefinition(..) and the setting is missing from the configuration
		///		file, then the default value is returned. Since you can specify a default value of 'null'
		///		then it is possible that GetSettingValue(..) can return null also.
		/// </remarks>
		public Object GetSettingValue(string settingName)
		{
			ConfigSetting configSetting = this.GetSetting(settingName);

			if (!configSetting.IsRequired && configSetting.Value == null) 
			{
				return configSetting.DefaultValue;
			} 
			else if (configSetting.IsRequired && configSetting.Value == null) 
			{
				ConfigurationKeyMissingException e = new ConfigurationKeyMissingException("Required config key not available");
				e.ConfigSectionName = this.configurationSectionName;
				e.MissingConfigKeyName = configSetting.Name;
				TRACE.ErrorLine(ExceptionManager.SerializeToString(e), TRACE_CAT);
				throw e;
			}

			return configSetting.settingCurrentValue;
		}


		/// <summary>
		/// Simillar to GetSettingValue(..) except that instead of returning just the value
		/// of a setting, it returns all the information about this setting, including its value.
		/// </summary>
		/// <param name="settingName"></param>
		/// <returns></returns>
		public ConfigSetting GetSetting(string settingName)
		{
			LoadSetting();

			object o = configSettings[settingName];

			if (o == null) 
			{
				ArgumentException ex = new ArgumentException("Cannot find a setting with this name (" + settingName + "). Have you defined it using AddSettingDefinition() ?");
				TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
				throw ex;
			}

			return (ConfigSetting) o;
		}

		/// <summary>
		/// Call this function if you want this object to load in and validate the configuration file right away. Otherwise this will be done the first time you try to
		/// access the configuration information thourgh GetSetting() or simillar functions
		/// Use this if you want to make sure the config file is validated right away. If there are any errors, an exception will be thrown
		/// </summary>
		public void Initialize() 
		{
			LoadSetting();
		}

		/// <summary>
		/// Represents one configuration setting. 
		/// </summary>
		public class ConfigSetting 
		{
			internal string settingName;
			internal Config.SettingType settingType;
			internal bool isSettingRequired;
			internal object settingDefaultValue;
			internal object settingCurrentValue;

			internal ConfigSetting(
				string settingName
				, Config.SettingType settingType
				, bool isSettingRequired
				, object settingDefaultValue
				, object settingCurrentValue
				) 
			{
				this.settingName = settingName;
				this.settingType = settingType;
				this.isSettingRequired = isSettingRequired;
				this.settingDefaultValue = settingDefaultValue;
				this.settingCurrentValue = settingCurrentValue;
			}

			public string Name 
			{
				get 
				{
					return settingName;
				}
			}
			public Config.SettingType Type
			{
				get 
				{
					return settingType;
				}
			}

			public bool IsRequired
			{
				get 
				{
					return isSettingRequired;
				}
			}

			/// <summary>
			/// Default value of the setting. Can be 'null'
			/// </summary>
			public object DefaultValue
			{
				get 
				{
					return settingDefaultValue;
				}
			}

			/// <summary>
			/// This is the value of the configuration setting found in the configuration file. 
			/// If this setting is optional, and it is missing from the configuration file, then this 
			/// Value property will return 'null'
			/// </summary>
			public object Value
			{
				get 
				{
					return settingCurrentValue;
				}
			}
		}



		#region Private functions


		/// <summary>
		/// This function loads the setting from the config file for later use.
		/// </summary>
		private void LoadSetting() 
		{
			if (!bSettingsLoaded) 
			{
				//
				// Make sure the settings are defined
				//
				if (this.configSettings.Count == 0) 
				{
					throw new InvalidOperationException("Before reading/loading in the settings, you must first define the settings using AddSettingDefinition() method");
				}

				//
				// Load and validate the settings
				//
				try 
				{
					TRACE.InfoLine("BEGIN:Loading configuration settings from section : " + configurationSectionName, TRACE_CAT);
					System.Diagnostics.Trace.Indent();

					#region Signaling to the .NET run time to load in the configuration section using System.Configuration.ConfigurationSettings.GetConfig
					NameValueCollection colSettings;
					try 
					{
						TRACE.VerboseLine("Signaling to the .NET run time to load in the configuration section: " + configurationSectionName, TRACE_CAT);
						colSettings = (NameValueCollection) System.Configuration.ConfigurationSettings.GetConfig(configurationSectionName);
						// Make sure the settings got loaded properly
						if (colSettings == null) 
						{
							throw new ConfigurationException("value returned from '(NameValueCollection) System.Configuration.ConfigurationSettings.GetConfig(" + configurationSectionName + ")' is NULL");
						}
					} 
					catch (Exception e) 
					{
						string errorMsg = "Error while attempting to load the configuration section '" 
							+ configurationSectionName + "'from app .config file. Do you have we .config file setup properly? This message is often caused due to mistakes in .config file." + Environment.NewLine
							+ " Please note that this is roughly what is expected to be in the configuration file: (please note, the following is XML text so it may not display properly in a browser)" + Environment.NewLine
							+ "	<configuration>" + Environment.NewLine
							+ "		<configSections>" + Environment.NewLine
							+ "			<section name=\"" + configurationSectionName+ "\" type=\"Gmbc.Common.Configuration.Config, Gmbc.Common\"/>" + Environment.NewLine
							+ "		</configSections>" + Environment.NewLine
							+ "		<" + configurationSectionName + ">" + Environment.NewLine
							+ "			<add key=\"some key - this is application specific\" value=\"some value of the key\"/>" + Environment.NewLine
							+ "			(... other settings in <add key=\"\" value=\"\"> fashion ... ) " + Environment.NewLine
							+ "		</" + configurationSectionName + ">"  + Environment.NewLine
							+ "	</configuration>";

						ConfigurationException ex = new ConfigurationException(errorMsg, e);
						ex.ConfigSectionName = configurationSectionName;
						TRACE.ErrorLine(ExceptionManager.SerializeToString(ex),TRACE_CAT);
						throw ex;
					}
					#endregion

					//
					// Convert the settings from NameValueCollection to our custom collection
					//
					for(int i = 0; i< colSettings.Count; i++) 
					{
						string settingName, settingValue;
						settingName = colSettings.Keys[i];
						settingValue = colSettings[i];
						TRACE.InfoLine("Found key : " + settingName + " with value of : " + settingValue, TRACE_CAT);

						object o = configSettings[settingName];
						if (o == null) 
						{
							//
							// This is not an error but a condition that we want to log
							//
							LogManager.Log("An undeclared configuration key was found in the application configuration file.  The key" + 
								" will be ignored.  Please check key spelling.  The key name is: " + settingName,
								LogManager.MessageType.Warning);
						}
						else
						{
							ConfigSetting configSetting = (ConfigSetting)o;

							//
							// Set the value of the setting. Also validate its type and make any other modifications as required
							//
							#region Set the value of the setting. Also validate its type and make any other modifications as required
							switch(configSetting.Type) 
							{
								case SettingType.Boolean:
									try 
									{
										configSetting.settingCurrentValue = Convert.ToBoolean(settingValue);
									} 
									catch (Exception e) 
									{
										ConfigurationKeyInvalidException ex = new ConfigurationKeyInvalidException("Error while initializing Configuration reader class - value ("+ configSetting.settingType.ToString() +") of config key (" + settingName+ ") is defined as Gmbc.Common.Configuration.Config.SettingType.Boolean but Convert.ToBoolean() threw an excetpion",e );
										ex.ConfigSectionName = this.configurationSectionName;
										ex.InvalidConfigKeyName = settingName;
										ex.InvalidConfigKeyValue = settingValue;
										TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
										throw (ex);
									}
									break;
								case SettingType.Double:
									try 
									{
										configSetting.settingCurrentValue = Convert.ToDouble(settingValue);
									} 
									catch (Exception e) 
									{
										ConfigurationKeyInvalidException ex = new ConfigurationKeyInvalidException("Error while initializing Configuration reader class - value ("+ configSetting.settingType.ToString() +") of config key (" + settingName+ ") is defined as Gmbc.Common.Configuration.Config.SettingType.Double but Convert.ToDouble() threw an excetpion",e );
										ex.ConfigSectionName = this.configurationSectionName;
										ex.InvalidConfigKeyName = settingName;
										ex.InvalidConfigKeyValue = settingValue;
										TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
										throw (ex);
									}
									break;
								case SettingType.Integer:
									try 
									{
										configSetting.settingCurrentValue = Convert.ToInt32(settingValue);
									} 
									catch (Exception e) 
									{
										ConfigurationKeyInvalidException ex = new ConfigurationKeyInvalidException("Error while initializing Configuration reader class - value ("+ configSetting.settingType.ToString() +") of config key (" + settingName+ ") is defined as Gmbc.Common.Configuration.Config.SettingType.Integer but Convert.ToInt32() threw an excetpion",e );
										ex.ConfigSectionName = this.configurationSectionName;
										ex.InvalidConfigKeyName = settingName;
										ex.InvalidConfigKeyValue = settingValue;
										TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
										throw (ex);
									}
									break;
								case SettingType.Directory:
									if (settingValue.Substring(0,1) == "~") 
									{
										HttpContext context = HttpContext.Current;
										if (context == null) 
										{
											ConfigurationKeyInvalidException ex = new ConfigurationKeyInvalidException("Error while reading the configuration file. The value ("+ settingValue +") of key (" + settingName + ") is a directory. It begins with a '~' but a '~' can only be specifed for web application / web services");
											ex.ConfigSectionName = this.configurationSectionName;
											ex.InvalidConfigKeyName = settingName;
											ex.InvalidConfigKeyValue = settingValue;
											TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
											throw ex;
										}

										settingValue = context.Server.MapPath(settingValue);
									}
									settingValue = settingValue.EndsWith(@"\") ? settingValue  : settingValue  + @"\";

									configSetting.settingCurrentValue = settingValue;
									break;

								case SettingType.FileName:
									if (settingValue.Substring(0,1) == "~") 
									{
										HttpContext context = HttpContext.Current;
										if (context == null) 
										{
											ConfigurationKeyInvalidException ex = new ConfigurationKeyInvalidException("Error while reading the configuration file. The value ("+ settingValue +") of key (" + settingName + ") is a file name. It begins with a '~' but a '~' can only be specifed for web application / web services");
											ex.ConfigSectionName = this.configurationSectionName;
											ex.InvalidConfigKeyName = settingName;
											ex.InvalidConfigKeyValue = settingValue;
											TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
											throw ex;
										}

										settingValue = context.Server.MapPath(settingValue);
									}

									configSetting.settingCurrentValue = settingValue;
									break;
								case SettingType.String:
									configSetting.settingCurrentValue = settingValue;
									break;
								default:
									throw new NotImplementedException("ERROR: Unrecognized SettingType of " + configSetting.settingType.ToString());
							}
							#endregion
						}
					}

					//
					// Validate each setting 
					//
					#region Validate each setting and make sure all settings have proper values
					foreach(DictionaryEntry de in this.configSettings) 
					{
						//
						// If setting is required, it must be specified
						//
						ConfigSetting configSetting = (ConfigSetting)de.Value;
						if (configSetting.IsRequired && configSetting.Value == null) 
						{
							ConfigurationKeyMissingException ex = new ConfigurationKeyMissingException("Error while initializing Configuration reader class - required config key is missing");
							ex.ConfigSectionName = this.configurationSectionName;
							ex.MissingConfigKeyName = configSetting.settingName;

							LogManager.Log("The following key was not found in the application configuration file: <" + de.Key + ">" +
								".  Please check the <" + configurationSectionName + "> section of the application configuration file " +
								" and make sure this key is properly defined."
								,LogManager.MessageType.Error);

							throw(ex);
						}
					}
					#endregion
				} 
				catch (Exception e) 
				{
					ConfigurationException ex = new ConfigurationException("Error while loading the configuration settings for config section:" + configurationSectionName, e);
					ex.ConfigSectionName = configurationSectionName;
					TRACE.ErrorLine(ExceptionManager.SerializeToString(ex),TRACE_CAT);
					throw ex;
				} 
				finally 
				{
					System.Diagnostics.Trace.Unindent();
					TRACE.InfoLine("END:Loading configuration settings from section : " + configurationSectionName, TRACE_CAT);
				}
			}
			bSettingsLoaded = true;
		}

		#endregion

		#region Enumerator
		/// <summary>
		/// Returns the Enumerator
		/// </summary>
		public IEnumerator GetEnumerator() 
		{
			return new ConfigSettingEnumerator(this);
		}
		
		/// <summary>
		/// Enumerator
		/// </summary>
		public class ConfigSettingEnumerator : IEnumerator 
		{
			IEnumerator enumerator;
			Config config;

			public ConfigSettingEnumerator(Config config) 
			{
				this.config = config;
				enumerator = config.configSettings.GetEnumerator();
			}

			public void Reset() 
			{
				enumerator.Reset();
			}

			public bool MoveNext() 
			{
				return enumerator.MoveNext();
			}

			public object Current 
			{
				get 
				{
					return (ConfigSetting)((DictionaryEntry)enumerator.Current).Value;
				}
			}
		}
		#endregion

		#region called by the .net runtime. Do not touch
		public Object Create(Object parent, object configContext, XmlNode section) 
		{
			NameValueCollection colSettings;

			try 
			{
				TRACE.VerboseLine("BEGIN IConfigurationSectionHandler.Create(..)", TRACE_CAT);
				System.Diagnostics.Trace.Indent();
				TRACE.VerboseLine("Here is the raw configuration section:" + section.OuterXml, TRACE_CAT);

				NameValueSectionHandler baseHandler = new NameValueSectionHandler();
				colSettings = (NameValueCollection)baseHandler.Create(parent, configContext, section);
			}
			catch (Exception e)
			{
				ConfigurationException ex = new ConfigurationException("Error while reading the configuration section from app .config file. Error message received:" + e.Message, e);
				ex.AdditionalInformation.Add("Raw content of config section (ie section.OuterXml)", section.OuterXml);
				TRACE.ErrorLine(ExceptionManager.SerializeToString(ex), TRACE_CAT);
				throw ex;
				//colSettings = null;
			} 
			finally
			{
				System.Diagnostics.Trace.Unindent();
				TRACE.VerboseLine("END IConfigurationSectionHandler.Create(..)", TRACE_CAT);
			}

			return colSettings;			
		}
		#endregion

		#region Implementation of ISerializableToNameValueCollection
		/// <summary>
		/// implementation of ISerializableToNameValueCollection.SerializeToNameValueCollection
		/// </summary>
		public void SerializeToNameValueCollection(NameValueCollection col)
		{
			string sClassName = this.GetType().ToString();
			string namePrefix = sClassName + ":CONFIG_SECTION_NAME[" + this.configurationSectionName + "]:SETTING[";
			foreach(ConfigSetting configSetting in this) 
			{
				col.Add(namePrefix + configSetting.Name + "].IsRequired", configSetting.IsRequired.ToString());
				col.Add(namePrefix + configSetting.Name + "].DefaultValue", configSetting.DefaultValue == null ? "<null>" : configSetting.DefaultValue.ToString());
				col.Add(namePrefix + configSetting.Name + "].Type", configSetting.Type.ToString());
				col.Add(namePrefix + configSetting.Name + "].Value", configSetting.Value == null ? "<null>" : configSetting.Value.ToString());
			}
		}

		private bool m_bAddedToExceptionManager=false;
		/// <summary>
		/// This functions ensures that an object (ie a particular instance of this class) is added to 
		/// to exception manager only once
		/// </summary>
		public void AddToExceptionManager()
		{
			if (!m_bAddedToExceptionManager) 
			{
				ExceptionManager.AddISerializeToNameValueCollectionObject(this);
				m_bAddedToExceptionManager = true; 
			}
		}
		#endregion

	}
}

