using System;
using System.Data.SqlClient;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Sql.MS
{
	/// <summary>
	/// Manages SqlConnection. When ever you need a SqlConnection, create an instance of this class and use
	/// the "SqlConnection" property to get the SqlConnection. Getting the connection from this property will open the connection
	/// and it will ensure that multiple calls open the connection only once.
	/// Once done, you must call Dispose() method in order
	///  to close the connection (alternatively, you can call the Close() method - it does the same thing 
	///  as Dispose()
	/// </summary>
	/// <remarks>
	/// - This class is not thread safe
	/// <p>
	/// <B>Why use this class instead of just SqlConnection?</B>
	/// </p>
	/// <p>
	/// This object is a very thin wrapper. The reason you should use this is bacause this class
	/// will warn you if you do not close the connection properly. Ie, not closing a SqlConnection by using Close()
	/// results in the connection being opened untill the connection is garbage collected which could be for a while.
	/// This is a serious problem in say, web application, where there can be many requests to the DB at one time - if 
	/// each request opens a connection and does not close it, you can run out of connections before the 
	/// garbage collector kicks in. 
	/// </p>
	/// <p>If you use this class, and you do not call Dispose() or Close(), an error message is logged
	/// using Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(). 
	/// Also, if you include this in your applications .config file ....
	/// <xmp> 
	///		<configSections>
	///			<section name="Gmbc.Common.Sql.MS" type="Gmbc.Common.Sql.MS.Config, Gmbc.Common.Sql.MS"/>
	///		</configSections>
	///	
	///		<Gmbc.Common.Sql.MS>
	///			<add key="RaiseErrorIfConnectionNotProperlyDisposed" value="true"/>
	///		</Gmbc.Common.Sql.MS>
	/// </xmp> 
	/// .... then this class will raise a ConnectionNotProperlyDisposedException if the object was not properly disposed
	/// </p>
	/// <P><B>Please note</B> Because this class writes to the error log using Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish()
	/// please make sure the Gmbc.Common.Diagnostics assebly is properly configured through your 
	/// applicataion's .config file. Refer to the documentation for this assembly for more information</P>
	/// </remarks>
	public class Connection : IDisposable 
	{
		private SqlConnection myConnection;
		private string connectionString;
		private bool m_bDisposed = false; // Track whether Dispose has been called.

		public Connection(string connectionString) 
		{
			this.connectionString = connectionString;
			myConnection = null;
		}

		/// <summary>
		/// Use this to get the an instance of SqlConnection. Multiple calls to this property ensure that
		/// the connection is opened only once and that you always get the same connection
		/// </summary>
		/// <returns></returns>
		public SqlConnection SqlConnection
		{
			get 
			{
				if(this.m_bDisposed) 
				{
					throw new ObjectDisposedException("Connection");
				}

				if (myConnection == null) 
				{
					myConnection = new SqlConnection(connectionString);
					myConnection.Open();
				}

				return myConnection;
			}
		}

		/// <summary>
		/// Use this method to close the connection properly once you are done with it
		/// Alternatively, you can call the Dispose() method which does the exact same thing
		/// Once you close the connection you must instantiate a new
		/// instance of the object to open the connection again
		/// </summary>
		public void Close () 
		{
			Dispose();
		}

		/// <summary>
		/// Use this method to close the connection properly once you are done with it
		/// 
		/// Alternatively, you can call the Close() method which does the exact same thing
		/// Once you close the connection you must instantiate a new
		/// instance of the object to open the connection again
		/// </summary>
		public void Dispose() 
		{
			Dispose(true);
			// Take yourself off of the Finalization queue to prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		// Dispose(bool disposing) executes in two distinct scenarios.
		// If disposing equals true, the method has been called directly
		// or indirectly by a user's code. 
		//
		// If bExplicitCall equals false, the method has been called by the 
		// runtime from inside the finalizer
		protected virtual void Dispose(bool bExplicitCall) 
		{
			if(bExplicitCall) 
			{
				if (myConnection != null) 
				{
					myConnection.Close();
				}
			} else {
				//
				// This is a serious warning. This means that the client of this object
				// did not call Dispose() which means the connection could have been opened untill
				// the object was finalized
				//
				ConnectionNotProperlyDisposedException ex = new ConnectionNotProperlyDisposedException("Connection was not disposed using Close() or Dispose() but was close in finalizer. This is inefficient");
				ex.ConnectionString = connectionString;
				ExceptionManager.Publish(ex);
				if (Config.RaiseErrorIfConnectionNotProperlyDisposed) 
				{
					throw ex;
				}
			}

			m_bDisposed = true;
		}

		// This finalizer will run only if the Dispose method does not get called.
		~Connection() 
		{
			Dispose(false);
		}

	} 
}
