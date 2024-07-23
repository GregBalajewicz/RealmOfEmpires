using System;
using System.Collections;
using System.Collections.Specialized;
using Gmbc.Common.Xml;
using Gmbc.Common.IO;

namespace Gmbc.Common.Reporting
{

	public class ReportGenerator
	{
		
		public ReportGenerator()
		{
		}

		public String GenerateXSLReport(String Source, String Report, Hashtable Params) 
		{
			try
			{
				return XslTransformer.Transform(Source, Config.ReportDirectory + Config.ReportXsltFile(Report), Params);
			}
			catch(Exception te)
			{
				ReportingException tex = new ReportingException("Error Formatting Report", te);
				tex.AdditionalInformation.Add("Source", Source);
				tex.AdditionalInformation.Add("Report", Report);
				try {
					if (Params !=null ) {
						foreach (DictionaryEntry entry in Params) {
							tex.AdditionalInformation.Add("Params[" + entry.Key.ToString() + "]", entry.Value.ToString());						
						}
					}
				} catch (Exception e) {
					tex.AdditionalInformation.Add("Error while iterating through Params Hashtable to show all its entries.", "Exception.Message=" + e.Message) ;						
				}
				Config.AddToExceptionManager();
				throw tex;
			}
		}

	}
}
