using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

namespace Gmbc.Common.Web
{

	/// <summary>
	/// This is a class derived directly from System.Web.UI.Page and provides some additional functionality 
	/// that any web application should find usefull
	/// </summary>
	public class BasePage : System.Web.UI.Page {

		public  BasePage () : base() {
			m_queryString = new QueryString();
		}

		private QueryString m_queryString;
		public QueryString queryString{
			get {
				return m_queryString;
			}
		}

		#region class QueryString 
		public class QueryString {
			private NameValueCollection _queryStringCollection;
			public QueryString () {
				
			}

			private NameValueCollection queryStringCollection 
			{
				get 
				{
					if (_queryStringCollection == null) 
					{
						_queryStringCollection = System.Web.HttpContext.Current.Request.QueryString;
					}
					return _queryStringCollection;
				}
			}
			/// <summary>
			/// Check if item sItem exists in Request.QueryString. 
			/// </summary>
			/// <returns>true if item sItem exists in Request.QueryString; false otherwise </returns>
			/// <param name="sItem">The string representing the item to check for in Request.QueryString</param>
			public System.Boolean Exists(String sItem) {
				if (queryStringCollection.Get(sItem) != null) {
					return true;
				} else {
					return false;
				}
			}

			/// <summary>
			/// Call this function to check if Request.QueryString item is empty or not. 
			/// </summary>
			/// <param name="sItem">The string representing the item to find in Request.QueryString</param>
			/// <returns>Returns true if the item sItem is empty; false otherwise</returns>
			/// <exception cref="ArgumentException">If an item sItem does not exist in 
			/// Request.QueryString, this exception is raised. One should use the Exists method
			/// to determine if an item exists</exception>
			public System.Boolean IsEmpty(String sItem) {
				if (!Exists(sItem)) {
					throw new ArgumentException(GetItemDoesNotExistExceptionMessage(sItem));
				}
				if (queryStringCollection.Get(sItem) == "") {
					return true;
				} else {
					return false;
				}
			}

			/// <summary>
			/// Use this to check if item sItem in Request.QueryString can be converted to System.Int32 using Convert.ToInt32
			/// </summary>
			/// <param name="sItem">The string representing the item of Request.QueryString to check</param>
			/// <returns>Returns True if the value of item sItem in Request.QueryString is convertable to System.Int32.
			/// ie, returns true implies that Convert.ToInt32(QueryString_GetString(sItem)) will not fail due to conversion
			/// problems. 
			/// If the function returns false then either:
			///		- the value of item cannot be converted to System.Int32
			///		- the item sItem does not exists in Request.QueryString (you can use Exists to determine if this is the case)</returns>
			public System.Boolean IsInt32(String sItem) {
				if (!Exists(sItem)) {
					return false;
				}
				try {
					Convert.ToInt32(queryStringCollection.Get(sItem));
				} catch {
					return false;
				}
				return true;			
			}

			/// <summary>
			/// Use this to check if item sItem in Request.QueryString can be converted to System.Boolean using Convert.ToBoolean
			/// </summary>
			/// <param name="sItem">The string representing the item of Request.QueryString to check</param>
			/// <returns>Returns True if the value of item sItem in Request.QueryString is convertable to System.Boolean.
			/// ie, returns true implies that Convert.ToBoolean(QueryString_GetString(sItem)) will not fail due to conversion
			/// problems. 
			/// If the function returns false then either:
			///		- the value of item cannot be converted to System.Boolean
			///		- the item sItem does not exists in Request.QueryString (you can use Exists to determine if this is the case)</returns>
			public System.Boolean IsBoolean(String sItem) {
				if (!Exists(sItem)) {
					return false;
				} 
				try {
					Convert.ToBoolean(queryStringCollection.Get(sItem));
				} catch {
					return false;
				}
				return true;			
			}
		
			/// <summary>
			/// This method returns the value of item sItem of Request.QueryString
			/// </summary>
			/// <param name="sItem"></param>
			/// <exception cref="ArgumentException">If an item sItem does not exist in 
			/// Request.QueryString then exception is raised. One should use the Exists method
			/// to determine if an item exists</exception>
			/// <returns></returns>
			public System.String GetString(String sItem) {
				if (queryStringCollection.Get(sItem) == null) {
					throw new ArgumentException(GetItemDoesNotExistExceptionMessage(sItem));
				}
				return queryStringCollection.Get(sItem);
			}

			/// <summary>
			/// This method returns the value of item sItem of Request.QueryString as Int32
			/// </summary>
			/// <param name="sItem"></param>
			/// <exception cref="ArgumentException">If an item sItem does not exist in 
			/// Request.QueryString then exception is raised. One should use the Exists method
			/// to determine if an item exists</exception>
			/// <returns></returns>
			public System.Int32 GetInt32(String sItem) {
				if (queryStringCollection.Get(sItem) == null) {
					throw new ArgumentException(GetItemDoesNotExistExceptionMessage(sItem));
				}
				return Convert.ToInt32(queryStringCollection.Get(sItem));
			}

            /// <summary>
            /// This method returns the value of item sItem of Request.QueryString as Boolean
            /// </summary>
            /// <param name="sItem"></param>
            /// <exception cref="ArgumentException">If an item sItem does not exist in 
            /// Request.QueryString then exception is raised. One should use the Exists method
            /// to determine if an item exists</exception>
            /// <returns></returns>
            public Boolean GetBoolean(String sItem)
            {
                if (queryStringCollection.Get(sItem) == null)
                {
                    throw new ArgumentException(GetItemDoesNotExistExceptionMessage(sItem));
                }
                return Convert.ToBoolean(queryStringCollection.Get(sItem));
            }



			private String GetItemDoesNotExistExceptionMessage(String sItem) {
				return "The item '" + sItem + "' does not exists!. Please use Exists to verify item's existance";
			}
		}
		#endregion

	}
}
