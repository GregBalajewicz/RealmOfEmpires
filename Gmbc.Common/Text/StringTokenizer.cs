using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Gmbc.Common.Text
{
	/// <summary>
	/// A basic string tokenizer.
	/// </summary>
	/// <Author>Greg Balajewicz</Author>
	public class StringTokenizer
	{
		private ArrayList substrings;
		bool areThereMoreSubstrings;
		private IEnumerator enumerator;

		/// <summary>
		/// Initialize the toknizer.
		/// Please note, token cannot include any special regular expression characters since regular expression engine
		/// is used to break up the string to substrings
		/// </summary>
		/// <param name="stringToParse"></param>
		/// <param name="token"></param>
		public StringTokenizer(string stringToParse, string token)
		{
			//
			// Validate Parameters
			//
			if (stringToParse == null) 
			{
				throw new ArgumentNullException("string stringToParse","stringToParse is null");

			}
			if (token == null) 
			{
				throw new ArgumentNullException("string token","token is null");

			}
			//
			// Breakup the string to substrings deliminated with the passed in token. 
			//
			Regex regExp = new Regex(token);
			MatchCollection mc = regExp.Matches(stringToParse);
			if (mc.Count > 0) 
			{
				substrings = new ArrayList(mc.Count);

				int startIndex=0;
				foreach (Match m in mc)
				{
					// if matched first char then disregard since this means string begins with a token
					if (m.Index == 0) 
					{
						startIndex = m.Index + m.Length;
						continue;
					}
					substrings.Add(stringToParse.Substring(startIndex, m.Index - startIndex));
					startIndex = m.Index + m.Length;
				}
				//
				// Do we have any thing after the last token?
				// ie if string is "a,bb,cc" and token is "," then make sure we grab "cc"
				// We do this by checking if there is something after the last token
				//
				Match match = mc[mc.Count-1]; // get the last token
				if (match.Index + match.Length != stringToParse.Length) 
				{
					substrings.Add(stringToParse.Substring(match.Index + match.Length, stringToParse.Length - (match.Index + match.Length)));				
				}
			} 
			else if (stringToParse.Length != 0)
			{
				//
				// We have not found any tokens in the string therefore the whole string is a substring
				//
				substrings = new ArrayList(1);
				substrings.Add(stringToParse);
			}

			if (substrings != null) 
			{
				enumerator = substrings.GetEnumerator();
				areThereMoreSubstrings =  enumerator.MoveNext();
			} 
			else 
			{
				areThereMoreSubstrings = false;
			}
		}

		/// <summary>
		/// Get the next substring. 
		/// If hasMoreTokens() returns false, then this method will throw an InvalidOperationException
		/// </summary>
		/// <returns></returns>
		public string getNextToken()
		{
			if (!hasMoreTokens()) 
			{
				throw new InvalidOperationException("Cannot call getNextToken when there are no more tokens. Use hasMoreTokens() to find out if there are more tokens.");
			}
			string subStr = (string)enumerator.Current;
			areThereMoreSubstrings =  enumerator.MoveNext();
			return subStr;
		}

		public bool hasMoreTokens()
		{
			return areThereMoreSubstrings;
		}
	}
}

