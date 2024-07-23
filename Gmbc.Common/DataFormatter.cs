using System;
using System.Globalization;

namespace Gmbc.Common
{
	/// <summary>
	/// DataFormatter singleton helper class for formatting data fields on presentation layer
	/// </summary>
	public class DataFormatter
	{
		private DataFormatter()
		{
		}

		/// <summary>
		/// A helper function used to convert Amount field to culture view currency standart
		/// "en-CA" defines Canada currency standart
		/// decimal delimeter "."
		/// delimiter between thousand ","
		/// example : " $ 1,978,567.87 "
		/// </summary>
		public static string GetFotmattedAmountString(string sAmount)
		{
			return GetFotmattedAmountString(Convert.ToDouble(sAmount));
		}

		/// <summary>
		/// A helper function used to convert Amount field to culture view currency standart
		/// "en-CA" defines Canada currency standart
		/// decimal delimeter "."
		/// delimiter between thousand ","
		/// example : " $ 1,978,567.87 "
		/// </summary>
		public static string GetFotmattedAmountString(double amount)
		{
			// Determines the specific culture
			NumberFormatInfo nfi = new CultureInfo( "en-CA", false ).NumberFormat;
			nfi.CurrencyDecimalDigits = 2;
			nfi.CurrencyNegativePattern = 0;
			return amount.ToString("C",nfi);
		}

	}
}
