using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.Specialized;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Sql.MS
{
	/// <summary>
	///	An instance of a StoredProcedure is a holder for a SqlCommand object for
	/// making stored procedure calls 
	/// <p>
	/// The StoredProcedure class implements Finalize/Dispose semantics for 
	/// closing the active database connection. </p>
	/// </summary>
	public class StoredProcedure : IDisposable {
		private SqlCommand command;

		///	<summary>
		///	Construct a new StoredProcedure.
		/// </summary>
		///	<param name='sprocName'>
		/// Name of the stored procedure to execute</param>
		///	<param name='parameters'>
		/// Array of SqlParameter's to the stored procedure. Can be null</param>
		/// <param name="ConnectionString">Connection string used to open a connection to the database</param>
		/// <exception cref="">If the connection cannot be opened, an exception will be generated 
		/// (see documentation for SqlConnection.Open() to see what exception can be generated - this method does
		/// not catch these exceptions, just allows them to propagate).
		/// <P>Please note that is an exception is thrown then the then call to this constructor does not result in 
		/// a properly created StoredProcedure object</P>
		/// </exception>
		public StoredProcedure( string sprocName, SqlParameter[] parameters, String ConnectionString ) {
			command = new SqlCommand( sprocName, new SqlConnection( ConnectionString ) );
			command.CommandType = CommandType.StoredProcedure;

			if (parameters != null) {
				foreach ( SqlParameter parameter in parameters )
					command.Parameters.Add( parameter );
			}

			command.Parameters.Add( 
				new SqlParameter( "ReturnValue",
				SqlDbType.Int,
				/* int size */ 4,
				ParameterDirection.ReturnValue,
				/* bool isNullable */ false,
				/* byte precision */ 0,
				/* byte scale */ 0,
				/* string srcColumn */ string.Empty,
				DataRowVersion.Default,
				/* value */ null 
				)
				);
			if (Config.CommandTimeout != -1) {
				command.CommandTimeout = Config.CommandTimeout;
			}
			command.Connection.Open();
		}

		///	<summary>
		///	Dispose of this StoredProcedure.
		/// </summary>
		public void Dispose() {
			if ( command != null ) {
				SqlConnection connection = command.Connection;
				Debug.Assert( connection != null );
				command.Dispose();
				command = null;
				connection.Dispose();
			}
		}

		///	<summary>
		/// Execute this stored procedure.
		///	</summary>
		///	<returns>Int32 value returned by the stored procedure</returns>
		public int Run() {
			if ( command == null )
				throw new ObjectDisposedException( GetType().FullName );
			command.ExecuteNonQuery(); 
			return ( int )command.Parameters[ "ReturnValue" ].Value;
		}

		/// <summary>
		///	Fill a DataTable with the result of executing this stored procedure.
		/// </summary>
		/// <param name='dataTable'>
		/// DataTable that will be filled with the results of executing the stored procedure. cannot be null</param>
		/// <returns>
		/// Int32 value returned by the stored procedure</returns>
		public int Run( DataTable dataTable ) {
			if ( command == null )
				throw new ObjectDisposedException( GetType().FullName );
			if (dataTable == null) {
				ArgumentException ex = new ArgumentException("value cannot be null", "DataTable dataTable");
				throw ex;
			}

			SqlDataAdapter dataAdapter = new SqlDataAdapter();

			dataAdapter.SelectCommand = command;
			dataAdapter.Fill( dataTable);

			return ( int )dataAdapter.SelectCommand.Parameters[ "ReturnValue" ].Value;
		}

		/// <summary>
		///	Fill a DataSet with the result of executing this stored procedure.
		/// <param name='ds'>
		/// DataSet to be filled with the results of executing the stored procedure. cannot be null</param>
		/// <returns>
		/// Int32 value returned by the stored procedure</returns>
		/// </summary>
		public int Run( DataSet ds ) {
			if ( command == null )
				throw new ObjectDisposedException( GetType().FullName );
			if (ds == null) {
				ArgumentException ex = new ArgumentException("value cannot be null", "DataSet ds");
				throw ex;
			}

			SqlDataAdapter dataAdapter = new SqlDataAdapter();

			dataAdapter.SelectCommand = command;
			dataAdapter.Fill( ds);

			return ( int )dataAdapter.SelectCommand.Parameters[ "ReturnValue" ].Value;
		}

		///	<summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
		///	</summary>
		///	<returns>object returned by the stored procedure</returns>
		public object RunScalar() 
		{
			if ( command == null )
				throw new ObjectDisposedException( GetType().FullName );
			return command.ExecuteScalar();
		}
	}
}
