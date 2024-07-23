using System;
using System.Xml;
using System.Collections;

namespace Gmbc.Common.Xml
{
	/// <summary>
	/// This singleton XmlDataEditor helper class for edit 
	/// and extract information from XML Document
	/// </summary>
	public class XmlDataEditor
	{
		private XmlDataEditor() {}
		/// <summary>
		/// Add attribute to XML Document using the specified Attribute path, name and value
		/// </summary>
		/// <param name="doc">XML Document to edit</param>
		/// <param name="sPath">Path to select Nodes for editing</param>
		/// <param name="sAttributeName">Attribute name to insert</param>
		/// <param name="sAttributeValue">Attribute value to insert</param>
		/// <returns>updated XML Document</returns>
		public static XmlDocument AddAttributeToNodeList(XmlDocument doc, string sPath, string sAttributeName, string sAttributeValue) 
		{
			XmlNodeList nodeList;
			nodeList = doc.SelectNodes(sPath);
			int ResultCount = nodeList.Count;
			foreach(XmlNode node in nodeList)
			{
				XmlAttributeCollection attrColl = node.Attributes;				
				XmlAttribute newAttr = doc.CreateAttribute(sAttributeName);
				newAttr.Value = sAttributeValue;
				attrColl.Append(newAttr);
			}
			return doc;
		}

		/// <summary>
		/// Node Counter for specified nodes in the XPath
		/// </summary>
		/// <param name="doc">XML Document to search</param>
		/// <param name="sPath">Path to select Nodes for count</param>
		/// <returns>int counter</returns>
		public static int NodeCounter(XmlDocument doc, string sPath) 
		{
			XmlNodeList nodeList = doc.SelectNodes(sPath);
			return nodeList.Count;
		}

	}
}
