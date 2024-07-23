using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Security.Permissions;

using Gmbc.Common.Diagnostics.ExceptionManagement;


namespace Gmbc.Common.Configuration
{
	/// <summary>
	/// Represents a exception that means that some error occured related to application's configuration, 
	/// especially in regards to the applications's configuration xml file
	/// </summary>
	[Serializable]
	public class ConfigurationException : BaseApplicationException 
	{ 
		#region Constructors
		/// <summary>
		/// Basic constructor
		/// </summary>
		public ConfigurationException() : base() {}
	
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public ConfigurationException(string message) : base(message) 
		{
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public ConfigurationException(string message,Exception inner) : base(message, inner) 
		{
		}
	
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) 
		{
			configSectionName = info.GetString("configSectionName");

		}
		#endregion

		#region Declare Member Variables
		private string configSectionName;
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
		#endregion	

		/// <summary>
		/// Used for serialization
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
			info.AddValue("configSectionName", configSectionName, typeof(string));
			base.GetObjectData(info,context);
		}
	
	
	}
}
