using System;
using System.Collections;

namespace Gmbc.Common.Reporting
{
	/// <summary>
	/// This class provides different reporting options. Currently it allows you to create a report by applying 
	/// XSLT tempate to an XML document.
	/// </summary>
	public class ReportGeneratorProvider
	{
		public ReportGeneratorProvider(){}

		/// <summary>
		/// Applies an XSLT template (Report) to an XML document (Source) with optional parameters passed to the
		/// XSLT tempate using the Params parameter. Output is string
		/// </summary>
		/// <param name="Source">a string representing the XML to transform</param>
		/// <param name="Report">Name of the report to apply. A mapping between this name and an actual XSLT 
		/// file name must exists in the config file
		/// </param>
		/// <param name="Params">a has table of parameters to pass to the XSLT template</param>
		/// <returns>Returns the result of applying the XSLT to the XML</returns>
		public String GenerateXSLReport(String Source, String Report, Hashtable Params) {
			ReportGenerator rg = new ReportGenerator();
			return rg.GenerateXSLReport(Source, Report, Params);
		}

	}
}







