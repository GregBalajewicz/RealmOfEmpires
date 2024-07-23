using System;
using System.Diagnostics;
using System.Windows.Forms;

using System.Data;

namespace Gmbc.Common.Diagnostics
{

	/// <summary>
	/// Write the Trace and Debug messgaes to a text box
	/// </summary>
	/// <example>
	/// This is an example of how this should be used:
	/// 	private void Form1_Load(object sender, System.EventArgs e) {
	/// 		// Create a new instance of the listenere passing it the text box to which to write the messages
	/// 		// The text box must be multiline
	///			TextBoxListener tbl = new TextBoxListener(this.txtStatus, null);
	///			// Add this trace listenere to the listeners to tha it response to trace calls
	///			Trace.Listeners.Add(tbl);
	///		}
	/// </example>
	public class TextBoxListener : TraceListener {
		private string GetCurDateTime() {
			return "[" +System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString() + "]";
		}

		public override void Write(string msg) {
			txtStatusBox.Text =  msg + txtStatusBox.Text;
		}

		public override void WriteLine(string msg) {
			txtStatusBox.Text = msg +  CF + LF + txtStatusBox.Text;
			base.WriteIndent();
			txtStatusBox.Text = GetCurDateTime() + txtStatusBox.Text;
		}

		public override void WriteLine(string msg, string cat) {
			this.WriteLine(cat + " : " + msg);
		}

		public override void Write(object obj, string cat) {
			this.Write(cat + " : " + obj.ToString());
		}
		public override void Write(string msg, string cat) {
			this.Write(cat + " : " + msg);
		}
		public TextBoxListener(System.Windows.Forms.TextBox txtStatusBox){
			this.txtStatusBox = txtStatusBox; 
		}
		private System.Windows.Forms.TextBox txtStatusBox;

		private const char LF = (char)10;
		private const char CF = (char)13;
	}
	


	/// <summary>
	/// Write the Tract and Debug messgaes to a list box
	/// </summary>
	/// <example>
	///		private void Form1_Load(object sender, System.EventArgs e) {
	/// 		// Create a new instance of the listener passing it the list box to which to write the messages
	///			ListBoxListener lbl = new ListBoxListener(this.lbStatus);
	///			// Add this trace listenere to the listeners to tha it response to trace calls
	///			Trace.Listeners.Add(lbl);
	///		}
	/// </example>
	public class ListBoxListener : TraceListener {
		private string GetCurDateTime() {
			return "[" +System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString() + "]";
		}

		public override void Write(string msg) {
			string sLine;
			if (lbStatusBox.Items.Count == 0) {
				lbStatusBox.Items.Add(msg);				
			} else {
				sLine = (string)lbStatusBox.Items[0];
				sLine += msg;
			}

		}

		public override void WriteLine(string msg) {
			int iTotIndentSize;
			iTotIndentSize = base.IndentSize * base.IndentLevel;

			string sLine;
			sLine = GetCurDateTime();
			sLine = sLine.PadRight(sLine.Length + iTotIndentSize);
			sLine += msg;
			lbStatusBox.Items.Insert(0,sLine);
		}

		public override void WriteLine(string msg, string cat) {
			this.WriteLine(cat + " : " + msg);
		}

		public override void Write(object obj, string cat) {
			this.Write(cat + " : " + obj.ToString());
		}
		public override void Write(string msg, string cat) {
			this.Write(cat + " : " + msg);
		}
		public ListBoxListener(System.Windows.Forms.ListBox lbStatusBox){
			this.lbStatusBox = lbStatusBox;
		}

		public string GetAsString() {
			char LF = (char)10;
			char CF = (char)13;
			string sFile="";
			foreach (Object item in lbStatusBox.Items) {
				sFile += (string)item + CF + LF;
			}
			return sFile;
		}
		private System.Windows.Forms.ListBox lbStatusBox;
	}



	
	/// <summary>
	/// Write the Tract and Debug messgaes to a list box
	/// </summary>
	/// <example>
	///		private void Form1_Load(object sender, System.EventArgs e) {
    ///         DataTable table = new DataTable();
	/// 		// Create a new instance of the listener passing it the list box to which to write the messages
    ///			DataTableListener listener = new DataTableListener(table);
	///			// Add this trace listenere to the listeners to tha it response to trace calls
    ///			Trace.Listeners.Add(listener);
	///		}
	/// </example>
	public class DataTableListener : TraceListener 
	{
		private DataTable dtTable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtTable">cannot be null</param>
        public DataTableListener(DataTable dtTable)
		{
            if (dtTable == null)
            {
                throw new ArgumentNullException("DataTable dtTable");
            }
            this.dtTable = dtTable;
            this.dtTable.Columns.Clear();

            this.dtTable.Columns.Add("Time", System.Type.GetType("System.DateTime"));
            this.dtTable.Columns.Add("Category", System.Type.GetType("System.String"));
            this.dtTable.Columns.Add("Message", System.Type.GetType("System.String"));
		}

		public override void Write(string msg) 
		{
            if (dtTable.Rows.Count == 0) 
			{
				PrivateWriteLine("",msg);
			} 
			else 
			{
                dtTable.Rows[dtTable.Rows.Count - 1][2] = (string)dtTable.Rows[dtTable.Rows.Count - 1][2] + msg;
			}

		}

		private void PrivateWriteLine(string cat, string msg)  
		{
            dtTable.Rows.Add(new object[] {DateTime.Now, cat, msg });		}


		public override void WriteLine(string msg) 
		{
			PrivateWriteLine("", msg);
		}

		public override void WriteLine(string msg, string cat) 
		{
			this.PrivateWriteLine(cat, msg);
		}

		public override void Write(object obj, string cat) 
		{
			this.PrivateWriteLine(cat, obj.ToString());
		}
		public override void Write(string msg, string cat) 
		{
			this.PrivateWriteLine(cat, msg);
		}
	}





    /// <summary>
    /// Write the Tract and Debug messgaes to a DataTable
    /// </summary>
    /// <example>
    ///		private void Form1_Load(object sender, System.EventArgs e) {
    /// 		// Create a new instance of the listener passing it the list box to which to write the messages
    ///			ListBoxListener lbl = new ListBoxListener(this.lbStatus);
    ///			// Add this trace listenere to the listeners to tha it response to trace calls
    ///			Trace.Listeners.Add(lbl);
    ///		}
    /// </example>
    public class ListViewListener : TraceListener
    {
        private System.Windows.Forms.ListView lvStatusBox;

        public ListViewListener(System.Windows.Forms.ListView lvStatusBox)
        {
            this.lvStatusBox = lvStatusBox;
            this.lvStatusBox.Columns.Clear();

            this.lvStatusBox.Columns.Add("Time", 100, System.Windows.Forms.HorizontalAlignment.Left);
            this.lvStatusBox.Columns.Add("Category", 300, System.Windows.Forms.HorizontalAlignment.Left);
            this.lvStatusBox.Columns.Add("Message", lvStatusBox.ClientSize.Width - 420, System.Windows.Forms.HorizontalAlignment.Left);

            this.lvStatusBox.View = View.Details;
            this.lvStatusBox.FullRowSelect = true;
        }

        public override void Write(string msg)
        {
            if (lvStatusBox.Items.Count == 0)
            {
                PrivateWriteLine("", msg);
            }
            else
            {
                lvStatusBox.Items[0].SubItems[2].Text = lvStatusBox.Items[0].SubItems[2].Text + msg;
            }

        }

        private void PrivateWriteLine(string cat, string msg)
        {
            ListViewItem lvi = new ListViewItem(GetCurDateTime());
            lvi.SubItems.Add(cat);
            lvi.SubItems.Add(msg);
            lvStatusBox.Items.Insert(0, lvi);
        }


        public override void WriteLine(string msg)
        {
            PrivateWriteLine("", msg);
        }

        public override void WriteLine(string msg, string cat)
        {
            this.PrivateWriteLine(cat, msg);
        }

        public override void Write(object obj, string cat)
        {
            this.PrivateWriteLine(cat, obj.ToString());
        }
        public override void Write(string msg, string cat)
        {
            this.PrivateWriteLine(cat, msg);
        }


        private string GetCurDateTime()
        {
            return System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString();
        }

    }
}
