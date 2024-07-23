using System;
using System.Data;

namespace Gmbc.Common.Data
{
	/// <summary>
	/// Singleton providing various functions that can help you work with System.Data.DataSet class
	/// </summary>
	public class DataSetUtilities
	{
		private DataSetUtilities(){}


        /// <summary>
		/// Same as colling WriteXml() on the dataset except that the result is returned as a string. 
		/// The string is indented and formated so that we can view the xml nicely formatted in a text editor
		/// </summary>
        public static string WriteXml(DataSet ds)
        {
            return WriteXml(ds, false);
        }

		/// <summary>
		/// Same as colling WriteXml() on the dataset except that the result is returned as a string. 
		/// The string is indented and formated so that we can view the xml nicely formatted in a text editor
		/// </summary>
        /// <param name="noException">Pass true if you do not want an exception thrown in case of one occuring but would rather want the exeception message returned as string</param>
		public static string WriteXml(DataSet ds, bool noException) 
		{
			if (ds == null) 
			{

                return string.Empty;
			}

            try
            {
                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(sw);
                writer.Formatting = System.Xml.Formatting.Indented;
                ds.WriteXml(writer, XmlWriteMode.IgnoreSchema);
                writer.Flush();
                writer.Close();

                string ret = sw.ToString();
                sw.Close();

                return ret;
            }
            catch (Exception ex)
            {
                if (noException)
                {
                    return ex.Message;
                }
                else
                {
                    throw ex;
                }
            }
		}


                /// <summary>
        /// Same as calling WriteXml() on the dataTable except that the result is returned as a string. 
        /// The string is indented and formated so that we can view the xml nicely formatted in a text editor
        /// </summary>
        public static string WriteXml(DataTable dt)
        {
            return WriteXml(dt, false);
        }

        /// <summary>
        /// Same as calling WriteXml() on the dataTable except that the result is returned as a string. 
        /// The string is indented and formated so that we can view the xml nicely formatted in a text editor
        /// </summary>
        /// <param name="noException">Pass true if you do not want an exception thrown in case of one occuring but would rather want the exeception message returned as string</param>
        public static string WriteXml(DataTable dt, bool noException)
        {
            if (dt == null)
            {

                return string.Empty;
            }

            try
            {
                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(sw);
                writer.Formatting = System.Xml.Formatting.Indented;
                dt.WriteXml(writer, XmlWriteMode.IgnoreSchema);
                writer.Flush();
                writer.Close();

                string ret = sw.ToString();
                sw.Close();

                return ret;
            }
            catch (Exception ex)
            {
                if (noException)
                {
                    return ex.Message;
                }
                else
                {
                    throw ex;
                }
            }
        }
	}
}
