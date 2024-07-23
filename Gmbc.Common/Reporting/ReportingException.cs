using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Principal;
using System.Security.Permissions;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Reporting
{
	[Serializable]
	public class ReportingException : BaseApplicationException 
	{
		#region Constructors
		/// <summary>
		/// Constructor with no params.
		/// </summary>
		public ReportingException() : base() 
	{
		//			InitializeEnvironmentInformation();
	}
	
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public ReportingException(string message) : base(message) 
	{
		//			InitializeEnvironmentInformation();
	}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public ReportingException(string message,Exception inner) : base(message, inner) 
	{
		//			InitializeEnvironmentInformation();
	}
	
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected ReportingException(SerializationInfo info, StreamingContext context) : base(info, context) 
	{
		//			machineName = info.GetString("machineName");
		//			createdDateTime = info.GetDateTime("createdDateTime");
		//			appDomainName = info.GetString("appDomainName");
		//			threadIdentity = info.GetString("threadIdentity");
		//			windowsIdentity = info.GetString("windowsIdentity");
		//			additionalInformation = (NameValueCollection)info.GetValue("additionalInformation",typeof(NameValueCollection));
	}
		#endregion

		#region Declare Member Variables
		#endregion

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
			base.GetObjectData(info,context);
		}
	
		#region Public Properties
		#endregion
	}
	
}
