using System;
using System.Diagnostics;

namespace Gmbc.Common
{
	/// <summary>
	/// 
	/// </summary>
	public class GmbcBaseClass
	{
		/// <summary>
		/// This is a helper property holding text which can be used as a Trace category. The contect of this property
		/// can be customized by classes inheriting from GNBaseClass. By default, the value of this property is 
		/// initialized to a fully qualified name of the class. This initialization occurs during object construction by 
		/// executing "this.TRACE_CAT = this.GetType().FullName;". 
		/// For example, if you have a class "public class MyClass : GmbcBaseClass" in namespace "GMBC.SomeProject.BL", then
		/// by default, the value of TRACE_CAT will be GMBC.SomeProject.BL.MyClass.
		/// </summary>
		private string traceCategory;  
		public GmbcBaseClass.Trace TRACE;

		/// <summary>
		/// Default constructor
		/// </summary>
		public GmbcBaseClass()
		{
			this.traceCategory = this.GetType().FullName;

			TRACE = new Trace(this.GetType().Module.ScopeName, this.traceCategory);
		}


		public GmbcBaseClass(string traceSwitchName)
		{
			this.traceCategory = this.GetType().FullName;

			TRACE = new Trace(traceSwitchName, this.traceCategory);
		}

		protected string TRACE_CAT  
		{
			get 
			{
				return this.traceCategory;
			}
			set 
			{
				this.traceCategory = value;
				TRACE.DefaultTraceCategory = value;
			}
		}



		/// <summary>
		/// Helper class for the predefined System.Diagnostics.Trace class. 
		/// This class encapsulates some functionality making using Trace class easier
		/// </summary>
		public class Trace 
		{
			private string defaultTraceCategory;
			private string traceSwitchName;
			private  TraceSwitch traceSwitch;

//			public Trace()
//			{
//				this.traceSwitchName = "";
//				traceSwitch = new TraceSwitch(traceSwitchName, "Controls Tracing");
//
//				this.defaultTraceCategory = "";
//			}


			public Trace(string traceSwitchName, string defaultTraceCategory)
			{
				this.traceSwitchName = traceSwitchName;
				traceSwitch = new TraceSwitch(traceSwitchName, "Controls Tracing");

				this.defaultTraceCategory = defaultTraceCategory;
			}

			public string TraceSwitchName 
			{
				get 
				{
					return traceSwitchName;
				}
				set 
				{
					if (value != traceSwitchName) 
					{
						traceSwitchName = value;
						traceSwitch = new TraceSwitch(traceSwitchName, "Controls Tracing");
					}
				}
			}

			public string DefaultTraceCategory 
			{
				get 
				{
					return defaultTraceCategory;
				}
				set 
				{
					defaultTraceCategory = value;
				}
			}


			#region WriteLine(msg, cat) overloads
			public void InfoLine (string sMsg, string sCategory) 
			{
				System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceInfo, sMsg, sCategory);
            }
			public void VerboseLine (string sMsg, string sCategory) 
			{
				System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceVerbose, sMsg, sCategory);
			}
			public void ErrorLine (string sMsg, string sCategory) 
			{
				System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceError, sMsg, sCategory);
			}
			public void WarningLine(string sMsg, string sCategory) 
			{
				System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceWarning, sMsg, sCategory);
			}
			#endregion

			#region WriteLine(msg) overloads
			public void InfoLine (string sMsg) 
			{
                System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceInfo, sMsg, defaultTraceCategory);              

			}
			public void VerboseLine (string sMsg) 
			{
				System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceVerbose, sMsg, defaultTraceCategory);
			}
			public void ErrorLine (string sMsg) 
			{
				System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceError, sMsg, defaultTraceCategory);
			}
			/// <summary>
			/// This method allows you to specify a summary and detailed error message. This method
			/// also adds some formatting to seperate the two messages for you. 
			/// </summary>
			/// <param name="summaryMessage"></param>
			/// <param name="detailedMessage"></param>
			public void ErrorLine_WithFormatting (string summaryMessage, string detailedMessage) 
			{
				string msg = summaryMessage + Environment.NewLine + ":::::DETAILED MESSAGE::::::" + Environment.NewLine + detailedMessage;

				System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceError, msg, defaultTraceCategory);
			}
			public void WarningLine(string sMsg) 
			{
				System.Diagnostics.Trace.WriteLineIf(traceSwitch.TraceWarning, sMsg, defaultTraceCategory);
			}
			#endregion

			#region Write(msg) overloads
			public void Info (string sMsg) 
			{
				System.Diagnostics.Trace.WriteIf(traceSwitch.TraceInfo, sMsg);
			}
			public void Verbose (string sMsg) 
			{
				System.Diagnostics.Trace.WriteIf(traceSwitch.TraceVerbose, sMsg);
			}
			public void Error (string sMsg) 
			{
				System.Diagnostics.Trace.WriteIf(traceSwitch.TraceError, sMsg);
			}
			public void Warning(string sMsg) 
			{
				System.Diagnostics.Trace.WriteIf(traceSwitch.TraceWarning, sMsg);
			}
			#endregion 
		}

	}
}
