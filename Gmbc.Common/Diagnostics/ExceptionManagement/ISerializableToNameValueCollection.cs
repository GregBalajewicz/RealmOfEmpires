using System;
using System.Collections.Specialized;

namespace Gmbc.Common.Diagnostics.ExceptionManagement {  
	/// <summary>
	/// This interface is used in conjunction with ExceptionManager and/or BaseApplicationException or
	/// exceptions derived from this one. Basically, BaseApplicationException exposes a property called
	/// AdditionalInformation which is a NameValueCollection. This can be used to add Name-Value pairs of additional,
	/// supporting information for an exception. If you have a class which has a lot of information that you would
	/// like to have included in BaseApplicationException.AdditionalInformation collection then you can have 
	/// this class implement this interface and have the method SerializeToNameValueCollection include
	/// all that information into the NameValueCollection 
	/// </summary>
	/// <remarks>
	/// <B>Here is an example of a class implementing this interface</B>
	/// <pre>
	///	class MyConfigurationClass : ISerializableToNameValueCollection {
	///		public String SomeAttribute {
	///			get {
	///				// return appropriate value
	///			}
	///		}
	///		public String SomeAttribute {
	///			get {
	///				// return appropriate value
	///			}
	///		}
	///		
	///		// Serialize all the properties into NameValueCollection with one method call...
	///		public void SerializeToNameValueCollection(NameValueCollection col) {
	///			string sClassName = this.GetType().ToString();
	///			col.Add(sClassName + ".SomeAttribute", this.SomeAttribute);
	///			col.Add(sClassName + ".SomeOtherAttribute", this.SomeAttribute);
	///		}
	///	}
	/// </pre>
	/// <B>Here is an example of how a class that implements this interface can be used</B>
	/// <pre>
	///	try {
	///		// some code which can throw an exception 
	///	} catch (Exception e) {
	///		BaseApplicationException ex = new BaseApplicationException("Error", e);
	///		ex.AdditionalInformation.Add("Value of SomeVariable", SomeVariable);
	///		MyConfigurationClass.SerializeToNameValueCollection(ex.AdditionalInformation);
	///		throw ex;
	///	}
	/// </pre>
	/// <P><B>Here is an example of how a class that implements this interface can be used in conjuction
	/// with the ExceptionManager.AddISerializeToNameValueCollectionObject()</B>
	/// <pre>
	///	try {
	///		// some code which can throw an exception 
	///	} catch (Exception e) {
	///		BaseApplicationException ex = new BaseApplicationException("Error", e);
	///		ex.AdditionalInformation.Add("Value of SomeVariable", SomeVariable);
	///		
	///		ExceptionManager.AddISerializeToNameValueCollectionObject(MyConfigurationClass);
	///		// Publish will find the class MyConfigurationClass and automatically call MyConfigurationClass.SerializeToNameValueCollection
	///		ExceptionManager.Publish(ex);	
	///	}
	/// </pre>
	/// </P>
	/// </remarks>
	public interface ISerializableToNameValueCollection
	{
		/// <summary>
		/// Add name-value pair information into the parameter col (NameValueCollection)
		/// </summary>
		/// <param name="col">NameValueCollection which is to be used to add more information to</param>
		void SerializeToNameValueCollection(NameValueCollection col);
	}
}
