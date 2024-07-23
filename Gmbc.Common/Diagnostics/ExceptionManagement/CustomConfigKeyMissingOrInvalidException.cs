using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Security.Permissions;


namespace Gmbc.Common.Diagnostics.ExceptionManagement
{
	/// <summary>
	/// WARNING: THIS CLASS IS DEPRECIATED... USE Gmbc.Common.Configuration.ConfigurationException Exception 
	/// or Exceptions derived from this one
	///  
	/// Represents a exception that means that a key, which was supposed to be provided
	/// in the applications .config file, is missing or is somehow invalid. See remarks for more info
	/// </summary>
	/// <remarks>
	/// <B>When to use this exception</B>
	/// <p>This exception should be used when ever appropriate but more specifically, with the current pattern 
	/// in GN's code, it should most likely be used in the Config class - this is the class that many applications
	/// which need to read the .config file implement for this purpose. If a key is found to be missing or invalid
	/// this exception should be thrown
	/// </p>
	/// </remarks>
	[Serializable]
	public class CustomConfigKeyMissingOrInvalidException : BaseApplicationException 
	{ 
		#region Constructors
		/// <summary>
		/// Basic constructor
		/// </summary>
		public CustomConfigKeyMissingOrInvalidException() : base() {}
	
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public CustomConfigKeyMissingOrInvalidException(string message) : base(message) 
		{
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public CustomConfigKeyMissingOrInvalidException(string message,Exception inner) : base(message, inner) 
		{
		}
	
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected CustomConfigKeyMissingOrInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) 
		{
			missingConfigKeyName = info.GetString("missingConfigKeyName");
			configSectionName = info.GetString("configSectionName");
			missingConfigKeyValue = info.GetString("missingConfigKeyValue");

		}
		#endregion

		#region Declare Member Variables
		private string configSectionName;
		private string missingConfigKeyName;
		private string missingConfigKeyValue;
		#endregion

		#region Public Properties
		/// <summary>
		/// Get/Set the configuration section name that was being read. 
		/// If this does not apply to you, you may leave it blank
		/// </summary>
		public string ConfigSectionName
		{
			get 
			{
				return configSectionName;
			}
			set
			{
				configSectionName = value;
			}
		}
		/// <summary>
		/// Get/Set the name of the configuration key that is missing or invalid
		/// If this does not apply to you, you may leave it blank
		/// </summary>
		public string MissingConfigKeyName 
		{
			get 
			{
				return missingConfigKeyName;
				}
			set
			{
				missingConfigKeyName = value;
			}
		}
		/// <summary>
		/// Get/Set the value of the configuration key that is missing or invalid.
		/// If this does not apply to you, you may leave it blank
		/// </summary>
		public string MissingConfigKeyValue
		{
			get 
			{
				return missingConfigKeyValue;
				}
			set
			{
				missingConfigKeyValue = value;
			}
		}
		#endregion	

		/// <summary>
		/// Used for serialization
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
			info.AddValue("missingConfigKeyName", missingConfigKeyName, typeof(string));
			info.AddValue("configSectionName", configSectionName, typeof(string));
			info.AddValue("missingConfigKeyValue", missingConfigKeyValue, typeof(string));
			base.GetObjectData(info,context);
		}
	
	
	}
}


