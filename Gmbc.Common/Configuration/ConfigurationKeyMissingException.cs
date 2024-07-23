using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Security.Permissions;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Configuration
{
	/// <summary>
	/// Represents a exception that means that a key, which was supposed to be provided
	/// in the applications .config file, is missing.
	/// </summary>
	[Serializable]
	public class ConfigurationKeyMissingException : ConfigurationException
	{

		#region Constructors
		/// <summary>
		/// Basic constructor
		/// </summary>
		public ConfigurationKeyMissingException() : base() {}
	
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public ConfigurationKeyMissingException(string message) : base(message) 
		{
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public ConfigurationKeyMissingException(string message,Exception inner) : base(message, inner) 
		{
		}
	
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected ConfigurationKeyMissingException(SerializationInfo info, StreamingContext context) : base(info, context) 
		{
			missingConfigKeyName = info.GetString("missingConfigKeyName");

		}
		#endregion

		#region Declare Member Variables
		private string missingConfigKeyName;
		#endregion

		#region Public Properties
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
		#endregion	

		/// <summary>
		/// Used for serialization
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
			info.AddValue("missingConfigKeyName", missingConfigKeyName, typeof(string));
			base.GetObjectData(info,context);
		}

	}
}
