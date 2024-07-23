using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Security.Permissions;


namespace Gmbc.Common.Diagnostics.ExceptionManagement
{
	/// <summary>
	/// Represents a generic security exception
	/// </summary>
	[Serializable]
	public class SecurityException : BaseApplicationException { 
		#region Constructors
		/// <summary>
		/// Basic constructor
		/// </summary>
		public SecurityException() : base() {}
	
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public SecurityException(string message) : base(message) {
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public SecurityException(string message,Exception inner) : base(message, inner) {
		}
	
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected SecurityException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
		#endregion

		#region Declare Member Variables
		#endregion

		/// <summary>
		/// Used for serialization
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
			base.GetObjectData(info,context);
		}
	
		#region Public Properties
		#endregion	
	}
}

