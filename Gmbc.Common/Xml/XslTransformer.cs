using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Collections;

namespace Gmbc.Common.Xml
{
	public class XslTransformer 
	{
		public XslTransformer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Transform the XML using the specified XSLT and return the result as string
		/// </summary>
		/// <param name="sXmlDocFileName">File name of the XML document to transform</param>
		/// <param name="sXsltFileName">File name of the XSLT to use to do the transformation</param>
		/// <returns>Transformation result as string</returns>
		/// 

		public static string Transform(string source, string sXsltFileName, Hashtable parameters) 
		{	
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(source);
			XslTransform xslTransform = new XslTransform();
			xslTransform.Load(sXsltFileName);

			XsltArgumentList args = new XsltArgumentList();
		
			if (parameters != null && parameters.Count != 0)
			{
				IDictionaryEnumerator paramEnum = parameters.GetEnumerator();

				while (paramEnum.MoveNext())
				{
					args.AddParam(paramEnum.Key.ToString(), "", paramEnum.Value);
				}
			}
				
			return Transform(doc, xslTransform, args);
		}

		/// <summary>
		/// Transform the XML using the specified XSLT and return the result as string
		/// </summary>
		/// <param name="xmlDoc">XmlDocument representing the document to transform</param>
		/// <param name="sXsltFileName">File name of the XSLT to use to do the transformation</param>
		/// <returns>Transformation result as string</returns>
		/// 

		/*		public static string Transform(XmlDocument sourceDOM, string stylesheet) 
				{
					XslTransform xslTransform = new XslTransform();
					xslTransform.Load(stylesheet);
			
					return TransformXmlToSting(xmlDoc, xslTransform);
				}
		*/
		/// <summary>
		/// Transform the XML using the specified XSLT and return the result as string
		/// </summary>
		/// <param name="xmlDoc">XmlDocument representing the document to transform</param>
		/// <param name="xslTransform">XslTransform object representing the XSLT to use to transformation</param>
		/// <returns>Transformation result as string</returns>

		public static string Transform(XmlDocument xmlDoc, XslTransform xslTransform, XsltArgumentList args) 
		{
			if (xmlDoc == null) 
			{
				throw new ArgumentException("Argument cannot be null", "XmlDocument xmlDoc");
			}
			if (xslTransform == null) 
			{
				throw new ArgumentException("Argument cannot be null", "XslTransform xslTransform");
			}
			
			StringWriter writer = new StringWriter();
			xslTransform.Transform(xmlDoc, args, writer, null);

			return writer.ToString();
		}
	}
}
