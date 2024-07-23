using System;

namespace Gmbc.Common
{
	/// <summary>
	/// Use this class to generate a time stamp which is consistent throughout all  applications. You can use this datetime
	/// stamp for files names or other purposes. Please extend this class as required
	/// </summary>
	public class DateTimeStamp
	{
		private DateTimeStamp() {} // singleton

		public enum StampType
		{
			/// <summary>
			/// Returns stamp in this format - yyyy-MM-dd_HH.mm.ss.
			/// EX: 2004-12-25_18.30.66 - December 25th, 2004 at 6:30PM and 66 seconds
			/// </summary>
			FullDateTime,
			/// <summary>
			/// Returns stamp in this format - yyyy-MM-dd.HH.mm.ss.
			/// EX: 2004-12-25 - December 25th, 2004
			/// </summary>
			Date
		}

		/// <summary>
		/// Return the date time stamp in the specified type of the current time
		/// </summary>
		public static string GetStamp(StampType stampType)
		{
			return GetStamp(DateTime.Now, stampType);
		}

		/// <summary>
		/// Return the date time stamp in the specified type of date time specified by dateTimeOfStamp parameter
		/// </summary>
		public static string GetStamp(DateTime dateTimeOfStamp, StampType stampType)
		{
			string stampFormat = GetStampFormatString(stampType);
			return dateTimeOfStamp.ToString(stampFormat);

		}

		private static string GetStampFormatString(StampType stampType) 
		{
			string stampFormat;
			switch (stampType) 
			{
				case StampType.Date:
					stampFormat = "yyyy-MM-dd";
					break;
				case StampType.FullDateTime:
					stampFormat = "yyyy-MM-dd_HH.mm.ss";
					break;
				default:
					throw new NotImplementedException("Unrecognized StampType:" + stampType.ToString());
			}
			return stampFormat;
		}

		/// <summary>
		/// Parse (ie interpret) the string in stamp parameter as a DateTime. This string should be in format
		/// specifed by the stampType parameter, otherwise a FormatException is thrown
		/// </summary>
		public static DateTime ParseStamp(string stamp, StampType stampType)
		{
			if (stamp == null) 
			{
				throw new ArgumentNullException("string stamp","stamp parameter cannot be null");
			}
			if (stamp.Equals(String.Empty)) 
			{
				throw new ArgumentOutOfRangeException("string stamp","", "stamp parameter cannot be empty");
			}
			string stampFormat = GetStampFormatString(stampType);
			System.IFormatProvider format = new System.Globalization.CultureInfo("fr-FR", true);

			DateTime time;
			try 
			{
				time = DateTime.ParseExact(stamp, stampFormat,format, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
			} 
			catch(FormatException e)
			{
				throw new FormatException("value of the stamp parameter (" + stamp + ") is not in the correct format for this stampType (" + stampType.ToString() + " -> ie: '" + stampFormat + "')",e);
			}
			catch (Exception e) 
			{
				throw new FormatException("Error while parsing the string in the stamp parameter (" + stamp + ") and trying to interpret is as stampType." + stampType.ToString() + " which is '" + stampFormat + "'.",e);
			}

			return time;
		}


	}
}
