using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Text;
using System.Security;
using System.Security.Principal;
using System.Security.Permissions;
using System.Collections.Specialized;
using System.Resources;
using System.Data;
using System.Collections.Generic;


namespace Gmbc.Common.Diagnostics.ExceptionManagement {
	/// <summary>
	/// Base Application Exception Class. You can use this as the base exception object from
	/// which to derive your applications exception hierarchy.
	/// </summary>
	[Serializable]
	public class BaseApplicationException : ApplicationException {
		#region Constructors
		/// <summary>
		/// Constructor with no params.
		/// </summary>
		public BaseApplicationException() : base() {
			InitializeEnvironmentInformation();
		}
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public BaseApplicationException(string message) : base(message) {
			InitializeEnvironmentInformation();
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public BaseApplicationException(string message,Exception inner) : base(message, inner) {
			InitializeEnvironmentInformation();
		}
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected BaseApplicationException(SerializationInfo info, StreamingContext context) : base(info, context) {
			machineName = info.GetString("machineName");
			createdDateTime = info.GetDateTime("createdDateTime");
			appDomainName = info.GetString("appDomainName");
			threadIdentity = info.GetString("threadIdentity");
			windowsIdentity = info.GetString("windowsIdentity");
			exceptionUniqueID = (Guid)info.GetValue("windowsIdentity", typeof(Guid));
			additionalInformation = (NameValueCollection)info.GetValue("additionalInformation",typeof(NameValueCollection));
		}
		#endregion

		#region Declare Member Variables
		// Member variable declarations
		private string machineName; 
		private string appDomainName;
		private string threadIdentity; 
		private string windowsIdentity;
		private Guid exceptionUniqueID;
		private DateTime createdDateTime = DateTime.Now;

		// Collection provided to store any extra information associated with the exception.
		private NameValueCollection additionalInformation = new NameValueCollection();
		
		#endregion

		/// <summary>
		/// Override the GetObjectData method to serialize custom values.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
			info.AddValue("machineName", machineName, typeof(string));
			info.AddValue("createdDateTime", createdDateTime);
			info.AddValue("appDomainName", appDomainName, typeof(string));
			info.AddValue("threadIdentity", threadIdentity, typeof(string));
			info.AddValue("windowsIdentity", windowsIdentity, typeof(string));
			info.AddValue("exceptionUniqueID", exceptionUniqueID, typeof(Guid));
			info.AddValue("additionalInformation", additionalInformation, typeof(NameValueCollection));
			base.GetObjectData(info,context);
		}

		#region Public Properties
		/// <summary>
		/// Machine name where the exception occurred.
		/// </summary>
		public string MachineName {
			get {
				return machineName;
			}
		}

		/// <summary>
		/// Date and Time the exception was created.
		/// </summary>
		public DateTime CreatedDateTime {
			get {
				return createdDateTime;
			}
		}

		/// <summary>
		/// AppDomain name where the exception occurred.
		/// </summary>
		public string AppDomainName {
			get {
				return appDomainName;
			}
		}

		/// <summary>
		/// Identity of the executing thread on which the exception was created.
		/// </summary>
		public string ThreadIdentityName {
			get {
				return threadIdentity;
			}
		}

		/// <summary>
		/// Windows identity under which the code was running.
		/// </summary>
		public string WindowsIdentityName {
			get {
				return windowsIdentity;
			}
		}

		/// <summary>
		/// Collection allowing additional information to be added to the exception.
		/// </summary>
		public NameValueCollection AdditionalInformation {
			get {
				return additionalInformation;
			}
		}

		/// <summary>
		/// Uniquelly identifies this exception
		/// </summary>
		public Guid ExceptionUniqueID{
			get {
				return exceptionUniqueID;
			}
		}

		#endregion

        /// <summary>
        /// Simmillar to doing AdditionalInformation.Add(name, object.ToString()) except this this function will check if object is null 
        /// and if it is, it will do AdditionalInformation.Add(name, "null") instad of ToString()
        /// </summary>
        public void AddAdditionalInformation(string name, object value)
        {
            BaseApplicationException.AddAdditionalInformation(AdditionalInformation, name, value);
        }

        /// <summary>
        /// Simmillar to doing AdditionalInformation.Add(name, object.ToString()) except this this function will check if object is null 
        /// and if it is, it will do AdditionalInformation.Add(name, "null") instad of ToString()
        /// </summary>
        public static void AddAdditionalInformation(NameValueCollection additionalInfo, string name, object value)
        {
            if (value is ISerializableToNameValueCollection2)
            {
                ((ISerializableToNameValueCollection2)value).SerializeToNameValueCollection(additionalInfo, name + ".");
            }
            else if (value is ISerializableToNameValueCollection)
            {
                ((ISerializableToNameValueCollection)value).SerializeToNameValueCollection(additionalInfo);
            }
            else if (value is DataRow)
            {
                AddAdditionalInformation(additionalInfo, name, (DataRow)value);
            }
            else
            {
                additionalInfo.Add(name == null ? "null" : name, value == null ? "null" : value.ToString());
            }
        }


        /// <summary>
        /// Same thing as doing AdditionalInformation.Add(name, value == null ? "null" :Gmbc.Common.Data.DataSetUtilities.WriteXml(value, true))
        /// Basically, it will serialize the DataSet into a string, and it is also safe to pass in null for value 
        /// </summary>
        public void AddAdditionalInformation(string name, DataSet value)
        {
            //AdditionalInformation.Add(name == null ? "null" : name, value == null ? "null" : Gmbc.Common.Data.DataSetUtilities.WriteXml(value, true));
            BaseApplicationException.AddAdditionalInformation(additionalInformation, name, value);
        }

        /// <summary>
        /// writes out the DataSet nicely, no need to worry its not null etc
        /// </summary>
        public static void AddAdditionalInformation(NameValueCollection additionalInfo, string name, DataSet value)
        {
            try
            {
                if (value == null)
                {
                    additionalInfo.Add(name == null ? "null" : name, "null");
                }
                else
                {
                    additionalInfo.Add(name, Gmbc.Common.Data.DataSetUtilities.WriteXml(value, true));
                }
            }
            catch (Exception e)
            {
                additionalInfo.Add(name == null ? "null" : name
                    , "Expected error occured in AddAdditionalInformation(NameValueCollection additionalInfo, string name, DataSet value). Message:" + e.Message);
            }
        }
        /// <summary>
        /// writes out the DataTable nicely, no need to worry its not null etc
        /// </summary>
        public static void AddAdditionalInformation(NameValueCollection additionalInfo, string name, DataTable value)
        {
            try
            {
                if (value == null)
                {
                    additionalInfo.Add(name == null ? "null" : name, "null");
                }
                else
                {
                    additionalInfo.Add(name, Gmbc.Common.Data.DataSetUtilities.WriteXml(value, true));
                }
            }
            catch (Exception e)
            {
                additionalInfo.Add(name == null ? "null" : name
                    , "Expected error occured in AddAdditionalInformation(NameValueCollection additionalInfo, string name, DataTable value). Message:" + e.Message);
            }
        }

        /// <summary>
        /// Same thing as doing AdditionalInformation.Add(name, value == null ? "null" :Gmbc.Common.Data.DataSetUtilities.WriteXml(value, true))
        /// Basically, it will serialize the datatable into a string, and it is also safe to pass in null for value 
        /// </summary>
        public void AddAdditionalInformation(string name, DataTable value)
        {
            //AdditionalInformation.Add(name == null ? "null" : name, value == null ? "null" : Gmbc.Common.Data.DataSetUtilities.WriteXml(value, true));
            BaseApplicationException.AddAdditionalInformation(additionalInformation, name, value);
        }

        /// <summary>
        /// writes it all the valeus in the datarow nicely, no need to worry its not null etc
        /// </summary>
        public static void AddAdditionalInformation(NameValueCollection additionalInfo, string name, DataRow value)
        {
            try
            {
                if (value == null)
                {
                    additionalInfo.Add(name == null ? "null" : name, "null");
                }
                else
                {
                    object[] os = value.ItemArray;
                    for (int i = 0; i < os.Length; i++)
                    {
                        BaseApplicationException.AddAdditionalInformation(additionalInfo, name + "[" + i.ToString() + "]", os[i]);
                    }
                }
            }
            catch (Exception e)
            {
                additionalInfo.Add(name == null ? "null" : name
                    , "Expected error occured in AddAdditionalInformation(NameValueCollection additionalInfo, string name, DataRow value). Message:" + e.Message);
            }
        }
 
        /// <summary>
        /// writes it all the valeus in the datarow nicely, no need to worry its not null etc
        /// </summary>
        public void AddAdditionalInformation(string name, DataRow value)
        {
            try
            {
                BaseApplicationException.AddAdditionalInformation(AdditionalInformation, name, value);
            }
            catch (Exception e)
            {
                AdditionalInformation.Add(name == null ? "null" : name
                    , "Expected error occured in AddAdditionalInformation(string name, DataRow value). Message:" + e.Message);
            }
        }
        /// <summary>
        /// writes it all the valeus in the array nicely, no need to worry its not null etc
        /// This method goes through all items in the array and writes out its members  by calling AddAdditionalInformation on each one.
        /// </summary>
        public static void AddAdditionalInformation(NameValueCollection additionalInfo, string name, object[] value)
        {
            name = name == null ? "null" : name;
            try
            {
                if (value == null)
                {
                    additionalInfo.Add(name, "null");
                }
                else
                {
                    BaseApplicationException.AddAdditionalInformation(additionalInfo, name + ".length", value.Length);

                    for (int i = 0; i < value.Length; i++)
                    {
                        BaseApplicationException.AddAdditionalInformation(additionalInfo, name + "[" + i.ToString() + "]", value[i]);
                    }
                }
            }
            catch (Exception e)
            {
                BaseApplicationException.AddAdditionalInformation(additionalInfo, name
                    , "Expected error occured in AddAdditionalInformation(NameValueCollection additionalInfo, string name, DataRow[] value). Message:" + e.Message);
            }
        }

        /// <summary>
        /// writes it all the valeus in the dictionary nicely, no need to worry its not null etc
        /// </summary>
        public static void AddAdditionalInformation(NameValueCollection additionalInfo, string name,  Dictionary<object, object> value)
        {
            if (String.IsNullOrEmpty(name))
            {
                return;
            }

            try
            {
                if (value == null)
                {
                    additionalInfo.Add(name, "null");
                }
                else
                {
                    foreach (KeyValuePair<object, object> kvp in value)
                    {
                        BaseApplicationException.AddAdditionalInformation(additionalInfo
                            , name + "[" + kvp.Key.ToString() + "]", kvp.Value);
                    }
                }
            }
            catch (Exception e)
            {
                additionalInfo.Add(name
                    , "Expected error occured in AddAdditionalInformation(NameValueCollection additionalInfo, string name, Dictionary<object, object> value). Message:" + e.Message);
            }
        }
        /// <summary>
        /// writes it all the valeus in the dictionary nicely, no need to worry its not null etc
        /// </summary>
        public static void AddAdditionalInformation(NameValueCollection additionalInfo, string name, Dictionary<string, string> value)
        {
            if (String.IsNullOrEmpty(name))
            {
                return;
            }

            try
            {
                if (value == null)
                {
                    additionalInfo.Add(name, "null");
                }
                else
                {
                    foreach (KeyValuePair<string, string> kvp in value)
                    {
                        BaseApplicationException.AddAdditionalInformation(additionalInfo
                            , name + "[" + kvp.Key + "]", kvp.Value);
                    }
                }
            }
            catch (Exception e)
            {
                additionalInfo.Add(name
                    , "Expected error occured in AddAdditionalInformation(NameValueCollection additionalInfo, string name, Dictionary<object, object> value). Message:" + e.Message);
            }
        }


        /// <summary>
        /// writes it all the valeus in the dictionary nicely, no need to worry its not null etc
        /// </summary>
        public void AddAdditionalInformation(string name, Dictionary<object, object> value)
        {
            BaseApplicationException.AddAdditionalInformation(this.additionalInformation, name, value);
        }
        /// <summary>
        /// writes it all the valeus in the dictionary nicely, no need to worry its not null etc
        /// </summary>
        public void AddAdditionalInformation(string name, Dictionary<string, string> value)
        {
            BaseApplicationException.AddAdditionalInformation(this.additionalInformation, name, value);
        }

        /// <summary>
        /// writes it all the valeus in the datarows nicely, no need to worry its not null etc
        /// </summary>
        public void AddAdditionalInformation(string name, object[] value)
        {
            try
            {
                BaseApplicationException.AddAdditionalInformation(AdditionalInformation, name, value);
            }
            catch (Exception e)
            {
                AdditionalInformation.Add(name
                    , "Expected error occured in AddAdditionalInformation(string name, DataRow[] value). Message:" + e.Message);
            }
        }

		/// <summary>
		/// Initialization function that gathers environment information safely.
		/// </summary>
		private void InitializeEnvironmentInformation() {									
			try {				
				machineName = Environment.MachineName;
			}
			catch(SecurityException) {
				machineName = "Cannot read, Permission denied";
				
			}
			catch {
				machineName = "Cannot read, unexpected error";
			}
					
			try {
				threadIdentity = Thread.CurrentPrincipal.Identity.Name;
			}
			catch(SecurityException) {
				threadIdentity = "Cannot read, Permission denied";
			}
			catch {
				threadIdentity = "Cannot read, unexpected error";
			}			
			
			try {
				windowsIdentity = WindowsIdentity.GetCurrent().Name;
			}
			catch(SecurityException) {
				windowsIdentity = "Cannot read, Permission denied";
			}
			catch {
				windowsIdentity = "Cannot read, unexpected error";
			}
			
			try {					
				appDomainName = AppDomain.CurrentDomain.FriendlyName;
			}
			catch(SecurityException) {
				appDomainName = "Cannot read, Permission denied";
			}
			catch {
				appDomainName = "Cannot read, unexpected error";
			}	
		
			exceptionUniqueID = Guid.NewGuid();
		}
	}	
}