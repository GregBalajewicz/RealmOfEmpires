using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Security.Permissions;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Configuration
{
	/// <summary>
	/// Represents a exception that means that a configuration key (in the applications .config file)
	/// is somehow invalid
	/// </summary>
	[Serializable]
	public class ConfigurationKeyInvalidException : ConfigurationException
	{

		#region Constructors
		/// <summary>
		/// Basic constructor
		/// </summary>
		public ConfigurationKeyInvalidException() : base() {}
	
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public ConfigurationKeyInvalidException(string message) : base(message) 
		{
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public ConfigurationKeyInvalidException(string message,Exception inner) : base(message, inner) 
		{
		}
	
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected ConfigurationKeyInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) 
		{
			invalidConfigKeyName = info.GetString("invalidConfigKeyName");
			invalidConfigKeyValue = info.GetString("invalidConfigKeyValue");

		}
		#endregion

		#region Declare Member Variables
		private string invalidConfigKeyName;
		private string invalidConfigKeyValue;
		#endregion

		#region Public Properties
		/// <summary>
		/// Get/Set the name of the configuration key that is missing or invalid
		/// If this does not apply to you, you may leave it blank
		/// </summary>
		public string InvalidConfigKeyName 
		{
			get 
			{
				return invalidConfigKeyName;
			}
			set
			{
				invalidConfigKeyName = value;
			}
		}
		public string InvalidConfigKeyValue 
		{
			get 
			{
				return invalidConfigKeyValue;
			}
			set
			{
				invalidConfigKeyValue = value;
			}
		}
		#endregion	

		/// <summary>
		/// Used for serialization
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
			info.AddValue("invalidConfigKeyName", invalidConfigKeyName, typeof(string));
			info.AddValue("invalidConfigKeyValue", invalidConfigKeyValue, typeof(string));
			base.GetObjectData(info,context);
		}

	}
}
