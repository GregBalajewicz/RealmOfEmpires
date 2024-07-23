using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;


namespace Gmbc.Common.Web.Controls
{
	[ParseChildren(true)]
	public class CustomDataGrid : DataGrid
	{

		#region Custom Styles
		public class StyleCustom 
		{
			string cssClass = string.Empty;
			public string CssClass 
			{
				get 
				{
					return cssClass;
				}
				set 
				{
					cssClass = value;
				}
			}
		}

		private StyleCustom mouseOverItemStyleCustom;
		//		private StyleCustom alternatingItemStyleCustom;
		//		private StyleCustom itemStyleCustom;
		//		private StyleCustom headerStyleCustom;
		private StyleCustom headerSortableLinkColumnStyleCustom;
		private StyleCustom pagerCurrentPageStyleCustom;
		private StyleCustom pagerNotCurrentPageStyleCustom;


		public StyleCustom HeaderSortableLinkColumnStyleCustom
		{
			get
			{
				return headerSortableLinkColumnStyleCustom;
			}
		}
		public StyleCustom PagerCurrentPageStyleCustom
		{
			get
			{
				return pagerCurrentPageStyleCustom;
			}
		}
		public StyleCustom PagerNotCurrentPageStyleCustom
		{
			get
			{
				return pagerNotCurrentPageStyleCustom;
			}
		}
		public StyleCustom MouseOverItemStyleCustom
		{
			get
			{
				return mouseOverItemStyleCustom;
			}
		}

		//		public string MouseOverItemStyleCustomClass
		//		{
		//			get
		//			{
		//				return Attributes["MouseOverItemStyleCustomClass"];
		//			}
		//			set
		//			{
		//				Attributes["MouseOverItemStyleCustomClass"] = value;
		//			}
		//		}
		//		public StyleCustom AlternatingItemStyleCustom
		//		{
		//			get
		//			{
		//				return alternatingItemStyleCustom;
		//			}
		//		}
		//		public StyleCustom ItemStyleCustom 
		//		{
		//			get
		//			{
		//				return itemStyleCustom;
		//			}
		//		}
		//		public StyleCustom HeaderStyleCustom 
		//		{
		//			get
		//			{
		//				return headerStyleCustom;
		//			}
		//		}
		#endregion

		#region Sorting 
		public String SortColumn 
		{
			get { return Attributes["SortColumn"]; }
			set { Attributes["SortColumn"] = value; }
		}


		/// <summary>
		/// Returns 'desc' or 'asc'
		/// </summary>
		public string SortDirection 
		{
			get { return Attributes["SortDirection"]; }
			set { 
				if (value != "desc" && value != "asc") 
				{
					throw new ArgumentException("SortDirection accepts only 'desc' or 'asc'");
				}
				Attributes["SortDirection"] = value; 
			}
		}

		private void OnSortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (willGridBeReBound(this)) 
			{
				string sCurrentSortColumn = e.SortExpression;
				string sCurrentSortDirection;

				string sLastSortColumn = Attributes["SortColumn"];
				string sLastSortDirection = Attributes["SortDirection"];


				if (sCurrentSortColumn == sLastSortColumn) 
				{
					if (sLastSortDirection == "desc") 
					{
						sCurrentSortDirection = "asc";
					}
					else 
					{
						sCurrentSortDirection = "desc";
					}
				} 
				else 
				{
					sCurrentSortDirection = "desc";
				}

				Attributes.Add("SortColumn", sCurrentSortColumn);
				Attributes.Add("SortDirection", sCurrentSortDirection);

				CurrentPageIndex = 0;
			}
		}
		#endregion

		#region WillGridBeReBoundDelegate and related stuff
		public delegate bool WillGridBeReBoundDelegate(DataGrid sender);
		private WillGridBeReBoundDelegate willGridBeReBound;
		public WillGridBeReBoundDelegate WillGridBeReBound
		{
			set {willGridBeReBound = value;}
		}

		private bool DefaultWillGridBeReBoundImplementation(DataGrid sender) 
		{
			return true;
		}
		#endregion

		/// <summary>
		/// Value of -1 means the total item count is unavailable
		/// </summary>
		private int totalItemCount=-1;
		private bool showItemCount=true;
		private bool showItemCountOnPager=true;

		/// <summary>
		/// specifies if item count should be displayed. Item count can be displayed on footer or paged.
		/// Use the ShowItemCountOnPager property to specify this.
		/// </summary>
		public bool ShowItemCount 
		{
			get
			{
				return showItemCount;
			}
			set
			{
				showItemCount = value;
			}
		}
		/// <summary>
		/// If set to true, the item count will be displayed on Pager, otherwise it will be displayed on footer. 
		/// Please note that the ShowItemCount property controls if the item count will be displayed at all!
		/// </summary>
		public bool ShowItemCountOnPager 
		{
			get
			{
				return showItemCountOnPager;
			}
			set
			{
				showItemCountOnPager = value;
			}
		}


		// Constructor that sets some styles and graphical properties	
		public CustomDataGrid()
		{
			mouseOverItemStyleCustom = new StyleCustom();
			headerSortableLinkColumnStyleCustom = new StyleCustom();
			pagerCurrentPageStyleCustom = new StyleCustom();
			pagerNotCurrentPageStyleCustom = new StyleCustom();

			this.WillGridBeReBound = new WillGridBeReBoundDelegate(this.DefaultWillGridBeReBoundImplementation);

			this.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.OnItemCreated);
			this.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.OnSortCommand);
			this.PageIndexChanged += new DataGridPageChangedEventHandler(OnPageIndexChanged);

			this.Load += new System.EventHandler(OnLoad);
		}

		private void OnLoad(object sender, System.EventArgs e)
		{
//			// ddlResultsPerPage.ID = this.ID + "_ddlResultsPerPage";
//
//			string ResultPerPage = this.Page.Request.Form[this.ID + "_ddlResultsPerPage"];
//			if (ResultPerPage != null && ResultPerPage.Trim() != "") 
//			{
//
//				PageSize = Convert.ToInt32(ResultPerPage);
//			}

		}


		public override void DataBind()
		{
			if (this.DataSource != null && showItemCount) 
			{
				if (this.DataSource is System.Data.DataTable) 
				{
					totalItemCount = ((System.Data.DataTable)this.DataSource).Rows.Count;
				}
				else if (this.DataSource is System.Collections.ICollection) 
				{
					totalItemCount = ((System.Collections.ICollection)this.DataSource).Count;
				} 
				else 
				{
					throw new InvalidOperationException("ShowItemCount is set to true but the datasource is not ICollection or DataTable hence the count cannot be determined. Set ShowItemCount to false to avoid this error");
				}
			}

			base.DataBind ();
		}


		private void OnItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{

			//
			// - - - - - - - - - - - - - - - ITEMs - - - - - - - - - - - - - -
			//
			if((e.Item.ItemType == ListItemType.Item) || 
				(e.Item.ItemType == ListItemType.AlternatingItem)) 
			{
				e.Item.Attributes.Add("onmouseover", "this.className='" + mouseOverItemStyleCustom.CssClass + "'");
				if((e.Item.ItemType == ListItemType.Item)) 
				{
					e.Item.Attributes.Add("onmouseout", "this.className='" + ItemStyle.CssClass + "'");
				} 
				else if((e.Item.ItemType == ListItemType.AlternatingItem)) 
				{
					e.Item.Attributes.Add("onmouseout", "this.className='" + AlternatingItemStyle.CssClass + "'");
				}
				
			}
			//
			// - - - - - - - - - - - - - - - HEADER - - - - - - - - - - - - - -
			//
			if (e.Item.ItemType == ListItemType.Header)
			{
				String strSortExpression = Attributes["SortColumn"];
				bool m_SortedAscending = (Attributes["SortDirection"]=="desc");
				String strOrder = (m_SortedAscending ? "5" : "6");

				for (int i=0; i< Columns.Count; i++)
				{
					// draw to reflect sorting
					if (strSortExpression == Columns[i].SortExpression)
					{
						TableCell cell = e.Item.Cells[i];
						Label lblSorted = new Label();
						lblSorted.Font.Name = "webdings";
						lblSorted.Font.Size = FontUnit.XSmall;
						lblSorted.Text = strOrder;
						lblSorted.ForeColor = Color.Green;
						cell.Controls.Add(lblSorted);
					}
				}	
				//
				// Style of the Link buttons
				//
				for (int i=0; i< Columns.Count; i++)
				{
					// draw to reflect sorting
					if (e.Item.Cells[i].Controls.Count > 0 && e.Item.Cells[i].Controls[0] is System.Web.UI.WebControls.LinkButton)
					{
						((System.Web.UI.WebControls.LinkButton)e.Item.Cells[i].Controls[0]).CssClass = headerSortableLinkColumnStyleCustom.CssClass;
					}
				}	
			}
			//
			// - - - - - - - - - - - - - - - PAGER - - - - - - - - - - - - - -
			//
			if (e.Item.ItemType == ListItemType.Pager) 
			{
				TableCell pager = (TableCell) e.Item.Controls[0];
		
				if (this.PagerStyle.Mode == PagerMode.NumericPages) 
				{
					Label lblSeperator;

					// Enumerates all the items in the pager...
					for (int i=1; i< pager.Controls.Count; i+=2)
					{
						lblSeperator = new Label();
						lblSeperator.Font.Name = "webdings";
						lblSeperator.Font.Size = FontUnit.Point(5);
						lblSeperator.Text = "&nbsp;=&nbsp;";
						lblSeperator.ForeColor = Color.Black;

						pager.Controls.Remove(pager.Controls[i]);
						pager.Controls.AddAt(i, lblSeperator);
					}

					// Enumerates all the items in the pager...
					for (int i=0; i<pager.Controls.Count; i+=2)
					{
						if (pager.Controls[i] is System.Web.UI.WebControls.LinkButton) 
						{
							LinkButton h = (LinkButton) pager.Controls[i];
							h.CssClass = pagerNotCurrentPageStyleCustom.CssClass;
						} 
						else  
						{
							Label l = (Label) pager.Controls[i];
							l.CssClass = pagerCurrentPageStyleCustom.CssClass;
						}
					}
				} 
				else if (this.PagerStyle.Mode == PagerMode.NextPrev) 
				{
					System.Web.UI.WebControls.LinkButton link;

					for (int i=0; i <= 2; i+=2) 
					{
						string Character;
						if (i == 0) 
						{
							Character = "7";
						} 
						else 
						{
							Character = "8";
						}
						if (pager.Controls[i] is System.Web.UI.WebControls.LinkButton) 
						{
							link = (System.Web.UI.WebControls.LinkButton)pager.Controls[i];
							link.Font.Name = "webdings";
							link.Font.Size = FontUnit.Medium;
							link.Text = Character;
							link.CssClass = "ControlLogTable_PagerNotCurrentPage";
						} 
						else  
						{
							Label l = (Label) pager.Controls[i];
							l.Font.Name = "webdings";
							l.Font.Size = FontUnit.Medium;
							l.Text = Character;
							l.CssClass = "ControlLogTable_PagerCurrentPage";
						}
					}
					int lowerBound, upperBound;
					lowerBound = 1 + (CurrentPageIndex * this.PageSize);
					upperBound = ((CurrentPageIndex +1) * this.PageSize);
					if (upperBound > totalItemCount && totalItemCount != -1) 
					{
						upperBound = totalItemCount;
					}
					LiteralControl lit = (LiteralControl) pager.Controls[1];
					lit.Text = "(showing records " + lowerBound.ToString() + " - " + upperBound.ToString() + ")";
				}

				//
				//
				//
//				System.Web.UI.WebControls.DropDownList ddlResultsPerPage = new DropDownList();
////				ddlResultsPerPage.ID = this.ID + "_ddlResultsPerPage";
//				ddlResultsPerPage.ID = "ddlResultsPerPage";
//				ddlResultsPerPage.AutoPostBack = true;
//				ddlResultsPerPage.SelectedIndexChanged += new System.EventHandler(OnResultsPerPageSelectedIndexChanged);
//				for(int i = 25; i <= 100; i+= 25) 
//				{
//					if (PageSize < i && PageSize > i-25 )
//					{
//						ddlResultsPerPage.Items.Add(PageSize.ToString());
//					}
//					ddlResultsPerPage.Items.Add(i.ToString());
//				}
//				if (PageSize > 100)
//				{
//					ddlResultsPerPage.Items.Add(PageSize.ToString());
//				}
//				ddlResultsPerPage.Items.FindByText(PageSize.ToString()).Selected = true;
//				pager.Controls.AddAt(0,ddlResultsPerPage);
				//
				//
				//
				if (showItemCountOnPager && showItemCount) 
				{
					Label lblItemCount = new Label();
					lblItemCount.Text = "Total number of records: " + totalItemCount.ToString() + "&nbsp;&nbsp; | &nbsp;";
						
					pager.Controls.AddAt(0,lblItemCount);
				}

			}
			//
			// - - - - - - - - - - - - - - - FOOTER - - - - - - - - - - - - - -
			//
			if (e.Item.ItemType == ListItemType.Footer 
				&& totalItemCount != -1 
				&& showItemCount
				&& !showItemCountOnPager) 
			{
				TableCell pager = (TableCell) e.Item.Controls[0];
				e.Item.Cells[0].ColumnSpan = e.Item.Cells.Count;
				e.Item.Cells[0].Text = "Total number of records: " + totalItemCount.ToString();

				int columnsToRemove = e.Item.Cells.Count -1;
				for(int i= 1; i <= columnsToRemove ;i++) 
				{
					e.Item.Cells.RemoveAt(1);
				}	
			}

		}
//
//		public void OnResultsPerPageSelectedIndexChanged(object sender, System.EventArgs e) 
//		{
//			if (willGridBeReBound(this)) 
//			{
//				PageSize = Convert.ToInt32(((DropDownList)sender).SelectedValue);
//				CurrentPageIndex = 0;
//			}
//		}
		
		private void OnPageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			if (willGridBeReBound(this)) 
			{
				CurrentPageIndex = e.NewPageIndex;
			}
		}


		#region Save / restore information from viewstate
		protected override void LoadViewState(object savedState) 
		{
			base.LoadViewState (savedState);
			mouseOverItemStyleCustom.CssClass = this.ViewState[this.ID + "_mouseOverItemStyleCustom.CssClass"].ToString();
			pagerCurrentPageStyleCustom.CssClass = this.ViewState[this.ID + "_pagerCurrentPageStyleCustom.CssClass"].ToString();
			pagerCurrentPageStyleCustom.CssClass = this.ViewState[this.ID + "_pagerCurrentPageStyleCustom.CssClass"].ToString();
			headerSortableLinkColumnStyleCustom.CssClass = this.ViewState[this.ID + "_headerSortableLinkColumnStyleCustom.CssClass"].ToString();
			totalItemCount = Convert.ToInt32(this.ViewState[this.ID + "_totalItemCount"]);
			showItemCount = Convert.ToBoolean(this.ViewState[this.ID + "_showItemCount"]);
			showItemCountOnPager = Convert.ToBoolean(this.ViewState[this.ID + "_showItemCountOnPager"]);
		}


		protected override object SaveViewState() 
		{
			this.ViewState[this.ID + "_mouseOverItemStyleCustom.CssClass"] = mouseOverItemStyleCustom.CssClass;
			this.ViewState[this.ID + "_pagerCurrentPageStyleCustom.CssClass"] = pagerCurrentPageStyleCustom.CssClass;
			this.ViewState[this.ID + "_pagerCurrentPageStyleCustom.CssClass"] = pagerCurrentPageStyleCustom.CssClass;
			this.ViewState[this.ID + "_headerSortableLinkColumnStyleCustom.CssClass"] = headerSortableLinkColumnStyleCustom.CssClass;
			this.ViewState[this.ID + "_totalItemCount"] = totalItemCount.ToString();
			this.ViewState[this.ID + "_showItemCount"] = showItemCount.ToString();
			this.ViewState[this.ID + "_showItemCountOnPager"] = showItemCountOnPager.ToString();
			
			return base.SaveViewState ();
		}
		#endregion
	}
}
