using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Principal;
using System.Security.Permissions;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Sql.MS
{
	
	[Serializable]
	public class ConnectionNotProperlyDisposedException : BaseApplicationException 
	{
		#region Constructors
		/// <summary>
		/// Constructor with no params.
		/// </summary>
		public ConnectionNotProperlyDisposedException() : base() 
		{
		}
	
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public ConnectionNotProperlyDisposedException(string message) : base(message) 
		{
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public ConnectionNotProperlyDisposedException(string message,Exception inner) : base(message, inner) 
		{
		}
	
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected ConnectionNotProperlyDisposedException(SerializationInfo info, StreamingContext context) : base(info, context) 
		{
			connectionString = info.GetString("connectionString");
		}
		#endregion


		/// <summary>
		/// Override the GetObjectData method to serialize custom values.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
			info.AddValue("connectionString", connectionString, typeof(string));
			base.GetObjectData(info,context);
		}

		#region Declare Member Variables
		private string connectionString;
		#endregion

		#region Public Properties
		public string ConnectionString 
		{
			get 
			{
				return connectionString;
			}
			set
			{
				connectionString = value;
			}
		}
		#endregion
	}

}

