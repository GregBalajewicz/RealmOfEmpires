using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Gmbc.Common.Web.Controls
{
	/// <summary>
	/// Composite Server Controls
	/// Shows three drop down boxes - one for year, one for month and one for day
	/// </summary>
	[ToolboxData("<{0}:DateControl runat=server></{0}:DateControl>")]
	public class DateControl : System.Web.UI.WebControls.WebControl
	{
		private System.Web.UI.WebControls.DropDownList ddlYear = new DropDownList();
		private System.Web.UI.WebControls.DropDownList ddlMonth = new DropDownList();
		private System.Web.UI.WebControls.DropDownList ddlDay = new DropDownList();

		private System.DateTime currentDate;
		private int currentYear;
		private int currentMonth;
		private int currentDay;
		private bool startWithUnselectedDate = false;
		private bool controlPopulated = false;

		/// <summary>
		/// lowest year to show relative to current year
		/// </summary>
		private int minYear;
		/// <summary>
		/// highest year to show relative to current year
		/// </summary>
		private int maxYear;	

		public DateControl()
		{
			currentDate = DateTime.Now;

			currentDay = currentDate.Day;
			currentMonth = currentDate.Month;
			currentYear = currentDate.Year;

			minYear = - 1;
			maxYear = 2;
		}

		/// <summary>
		/// Minimum (lowest) year to display relative to the current year. ie, this is not an absolute value.
		/// Ex, setting MinRelativeYear to -5 will show years from (current year - 5) 
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">If MinRelativeYear is larger than MaxRelativeYear OR 
		/// If setting MinRelativeYear to this value would make the lower bound year to be outside of the bounds of a valid year</exception>
		[Browsable(true), DefaultValue(-1)]
		public int MinRelativeYear 
		{
			get 
			{
				return minYear;
			}
			set 
			{
				if (value > maxYear) 
				{
					throw new ArgumentOutOfRangeException("", value, "MinRelativeYear cannot be larger than MaxRelativeYear");
				}
				if (currentYear + value < 1 || currentYear + value > 9999)
				{
					throw new ArgumentOutOfRangeException("", value, "Setting MinRelativeYear to this value would make the lower bound year be " + Convert.ToString(currentYear + value) + " which is outside of the bounds of a valid year. See System.DateTime for valid year values");
				}
				bool mustRepopulate = minYear != value ? true: false;

				minYear = value;

				if (mustRepopulate && this.controlPopulated) 
				{ 
					PopulateControl();
				}
			}
		}

		/// <summary>
		/// Maximum (highest) year to display relative to the current year. ie, this is not an absolute value.
		/// Ex, setting MinRelativeYear to 5 will show years to (current year + 5) 
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">If MaxRelativeYear is less than MinRelativeYear OR
		/// If setting MaxRelativeYear to this value would make the lower bound year to be outside of the bounds of a valid year</exception>
		[Browsable(true), DefaultValue(2)]
		public int MaxRelativeYear 
		{
			get 
			{
				return maxYear;
			}
			set 
			{
				if (value < minYear) 
				{
					throw new ArgumentOutOfRangeException("", value, "MaxRelativeYear cannot be less than MinRelativeYear");
				}
				if (currentYear + value < 1 || currentYear + value > 9999)
				{
					throw new ArgumentOutOfRangeException("", value, "Setting MaxRelativeYear to this value would make the upper bound year be" + Convert.ToString(currentYear + value) + " which is outside of the bounds of a valid year. See System.DateTime for valid year values");
				}

				bool mustRepopulate = maxYear != value ? true: false;

				maxYear = value;

				if (mustRepopulate && this.controlPopulated)
				{ 
					PopulateControl();
				}

			}
		}


		/// <summary>
		/// Get or Set the currently selected date
		/// </summary>
		/// <exception cref="InvalidOperationException">If selected date does not represent a valid date.
		///  Use IsValid() before</exception>
		[Browsable(false)]
		public DateTime SelectedDate
		{
			set 
			{
				ListItem li;
				li = ddlYear.Items.FindByValue(value.Year.ToString());
				if (li != null) 
				{
					UnselectCurrentItem(ddlYear);
					li.Selected = true;
				}
				li = ddlMonth.Items.FindByValue(value.Month.ToString());
				if (li != null) 
				{
					UnselectCurrentItem(ddlMonth);
					li.Selected = true;
				}
				li = ddlDay.Items.FindByValue(value.Day.ToString());
				if (li != null) 
				{
					UnselectCurrentItem(ddlDay);
					li.Selected = true;
				}
			}
			get 
			{
				if (!IsValid()) 
				{
					throw new InvalidOperationException("Selected date does not represent a valid date. Use IsValid() before");
				}
				int year, month, day;
				year = Convert.ToInt32(ddlYear.SelectedValue);
				month = Convert.ToInt32(ddlMonth.SelectedValue);
				day = Convert.ToInt32(ddlDay.SelectedValue);
				return new DateTime(year, month, day);
			}
		}

		/// <summary>
		/// Specifies if the control will include the 'unselected' values. That is, for the
		/// year drop down it will include a word "Year". Simillarily for month and day. When
		/// this property is set to true, the control is first initialize to words "Year, Month, Day".
		/// </summary>
		[Browsable(true)]
		public bool StartWithUnselectedDate
		{
			get 
			{
				return startWithUnselectedDate;
			}
			set
			{
				bool mustRepopulate = startWithUnselectedDate != value ? true: false;

				startWithUnselectedDate = value;

				if (mustRepopulate && this.controlPopulated)
				{ 
					PopulateControl();
				}
			}
		}

		/// <summary>
		/// This is a helper function used to transform a particular ValidationResult value
		/// to a human readable message. See Remarks for the ValidationResult value to message mappings
		/// </summary>
		/// <remarks>
		/// <list type="table">
		///		<listheader>
		///			<term>Value</term>
		///			<description>Message</description>
		///		</listheader>
		///		<item>
		///			<term>ValidationResult.OK</term>
		///			<description>"" - ie, empty string is returned</description>
		///		</item>
		///		<item>
		///			<term>ValidationResult.YearNotSelected</term>
		///			<description>"Year has not been selected"</description>
		///		</item>
		///		<item>
		///			<term>ValidationResult.MonthNotSelected</term>
		///			<description>"Month has not been selected"</description>
		///		</item>
		///		<item>
		///			<term>ValidationResult.DayNotSelected</term>
		///			<description>"Day has not been selected"</description>
		///		</item>
		///		<item>
		///			<term>ValidationResult.DayOutOfRange</term>
		///			<description>"The selected day is wrong for the selected month - ex February 31"</description>
		///		</item>
		///		<item>
		///			<term>ValidationResult.YearMonthAndDayNotSelected</term>
		///			<description>"Year, Month and Day have not been selected"</description>
		///		</item>
		///		<item>
		///			<term>ValidationResult.YearAndMonthNotSelected</term>
		///			<description>"Year and Month have not been selected"</description>
		///		</item>
		///		<item>
		///			<term>ValidationResult.YearAndDayNotSelected</term>
		///			<description>"Year and Day have not been selected"</description>
		///		</item>
		///		<item>
		///			<term>ValidationResult.MonthAndDayNotSelected</term>
		///			<description>"Month and Day have not been selected"</description>
		///		</item>
		///	</list>
		/// </returns>
		/// </remarks>
		public static string GetValidationResultAsErrorMessage(ValidationResult validationResult)
		{
			string msg;
			switch (validationResult) 
			{
				case DateControl.ValidationResult.YearNotSelected:
					msg = "Year has not been selected";
					break;
				case DateControl.ValidationResult.MonthNotSelected:
					msg = "Month has not been selected";
					break;
				case DateControl.ValidationResult.DayNotSelected:
					msg = "Day has not been selected";
					break;
				case DateControl.ValidationResult.DayOutOfRange:
					msg = "The selected day is wrong for the selected month - ex February 31";
					break;
				case DateControl.ValidationResult.YearMonthAndDayNotSelected:
					msg = "Year, Month and Day have not been selected";
					break;
				case DateControl.ValidationResult.YearAndMonthNotSelected:
					msg = "Year and Month have not been selected";
					break;
				case DateControl.ValidationResult.YearAndDayNotSelected:
					msg = "Year and Day have not been selected";
					break;
				case DateControl.ValidationResult.MonthAndDayNotSelected:
					msg = "Month and Day have not been selected";
					break;
				default:
					msg = "";
					break;
			}

			return msg;
		}

		/// <summary>
		/// Result value from Validate() function.
		/// </summary>
		public enum ValidationResult 
		{
			/// <summary>
			/// Selected date is valid
			/// </summary>
			OK,
			/// <summary>
			/// Selected date is invalid - Year has not been selected
			/// </summary>
			YearNotSelected,
			/// <summary>
			/// Selected date is invalid - Month has not been selected
			/// </summary>
			MonthNotSelected,
			/// <summary>
			/// Selected date is invalid - Day has not been selected
			/// </summary>
			DayNotSelected,
			/// <summary>
			/// Selected date is invalid -
			/// this means that the day is wrong - ie if you choose February 31 for example.
			/// </summary>
			DayOutOfRange,
			/// <summary>
			/// Selected date is invalid - The Year, Month and Date has not been selected
			/// </summary>
			YearMonthAndDayNotSelected,
			/// <summary>
			/// Selected date is invalid - The Year and Month has not been selected
			/// </summary>
			YearAndMonthNotSelected,
			/// <summary>
			/// Selected date is invalid - The Year and Day has not been selected
			/// </summary>
			YearAndDayNotSelected,
			/// <summary>
			/// Selected date is invalid - The Month and Day has not been selected
			/// </summary>
			MonthAndDayNotSelected
		}


		/// <summary>
		/// Validate the currently selected date. This is a simple validation which only 
		/// returns a subset of possible values of ValidationResult enumeration. If you need
		/// more comprehensive validation, Try the Validate_GetAdvancedResultSet method
		/// </summary>
		/// <returns>
		/// Please note, when this method returns, for example,
		/// ValidationResult.YearNotSelected this means that _at least_ the Year has
		/// not been selected - it could also be true that Month has not been selected also.
		/// If you need to know exactly what has/has not been selected, use Validate_GetAdvancedResultSet()
		/// <list type="bullet">
		///		<item>
		///			<description>ValidationResult.OK</description>
		///		</item>
		///		<item>
		///			<description>ValidationResult.YearNotSelected</description>
		///		</item>
		///		<item>
		///			<description>ValidationResult.MonthNotSelected</description>
		///		</item>
		///		<item>
		///			<description>ValidationResult.DayNotSelected</description>
		///		</item>
		///		<item>
		///			<description>ValidationResult.DayOutOfRange</description>
		///		</item>
		///	</list>
		/// </returns>
		public ValidationResult Validate() 
		{
			int year, month, day;
			year = Convert.ToInt32(ddlYear.SelectedValue);
			month = Convert.ToInt32(ddlMonth.SelectedValue);
			day = Convert.ToInt32(ddlDay.SelectedValue);

			if (year == -1) 
			{
				return ValidationResult.YearNotSelected;
			}
			if (month == -1) 
			{
				return ValidationResult.MonthNotSelected;
			}
			if (day == -1) 
			{
				return ValidationResult.DayNotSelected;
			}
			if (day > DateTime.DaysInMonth(year, month)) 
			{
				return ValidationResult.DayOutOfRange;
			}
			return ValidationResult.OK;; 
		}


		/// <summary>
		/// Validate the currently selected date. This is a more comprehensive validation
		/// then the Validate() method - it returns a more discriptive error message.
		/// </summary>
		/// <returns>
		///	Any value of ValidationResult enumeration. Please note that
		///	when this methods returns, for example,
		/// ValidationResult.YearNotSelected this means that Year and only Year has
		/// not been selected. If, for example, Year and Month is not selected then
		/// the return value will be ValidationResult.YearAndMonthNotSelected.
		/// </returns>
		public ValidationResult Validate_GetAdvancedResultSet() 
		{
			int year, month, day;
			year = Convert.ToInt32(ddlYear.SelectedValue);
			month = Convert.ToInt32(ddlMonth.SelectedValue);
			day = Convert.ToInt32(ddlDay.SelectedValue);

			if (year == -1 && month == -1 && day == -1) 
			{
				return ValidationResult.YearMonthAndDayNotSelected;
			}
			if (year == -1 && month == -1) 
			{
				return ValidationResult.YearAndMonthNotSelected;
			}
			if (month == -1 && day == -1) 
			{
				return ValidationResult.MonthAndDayNotSelected;
			}
			if (year == -1 && day == -1) 
			{
				return ValidationResult.YearAndDayNotSelected;
			}
			return Validate(); 
		}



		/// <summary>
		/// Check if the currently selected date represents a valid date. Simply 
		/// returns true or false but you can use Validate() to see exactly what
		/// has failed
		/// </summary>
		public bool IsNotSelected() 
		{
			if (this.Validate() == ValidationResult.OK) 
			{
				return true;
			}
			else 
			{
				return false; 
			}
		}

		/// <summary>
		/// Check if the currently selected date represents a valid date. Simply 
		/// returns true or false but you can use Validate() to see exactly what
		/// has failed
		/// </summary>
		public bool IsValid() 
		{
			if (this.Validate() == ValidationResult.OK) 
			{
				return true;
			}
			else 
			{
				return false; 
			}
		}

		/// <summary>
		/// Checks the drop down list if one of the items is selected and unselected it. 
		/// </summary>
		private void UnselectCurrentItem(DropDownList list) 
		{
			ListItem li = list.SelectedItem;

			if (li != null) 
			{
				li.Selected = false;
			}
		}

		/// <summary>
		/// Repopulates the control. There should be no need to call this method directly
		/// Please note, calling this method will reset the currently selected date
		/// </summary>
		public void PopulateControl() 
		{
			EnsureChildControls();

			DateTime lastSelectedDate = new DateTime(1);

			ddlYear.Items.Clear();
			ddlMonth.Items.Clear();
			ddlDay.Items.Clear();

			//
			// Init the year
			//
			if (startWithUnselectedDate) 
			{
				ddlYear.Items.Add(new ListItem("Year", "-1"));
			}
			for (int i = currentYear + minYear; i <= currentYear + maxYear; i++)
			{
				ddlYear.Items.Add(new ListItem(i.ToString()));
			}
			//
			// Init the month
			//
			if (startWithUnselectedDate) 
			{
				ddlMonth.Items.Add(new ListItem("Month", "-1"));
			}
			ddlMonth.Items.Add(new ListItem("January", "1"));
			ddlMonth.Items.Add(new ListItem("February", "2"));
			ddlMonth.Items.Add(new ListItem("March", "3"));
			ddlMonth.Items.Add(new ListItem("April", "4"));
			ddlMonth.Items.Add(new ListItem("May", "5"));
			ddlMonth.Items.Add(new ListItem("June", "6"));
			ddlMonth.Items.Add(new ListItem("July", "7"));
			ddlMonth.Items.Add(new ListItem("August", "8"));
			ddlMonth.Items.Add(new ListItem("September", "9"));
			ddlMonth.Items.Add(new ListItem("October", "10"));
			ddlMonth.Items.Add(new ListItem("November", "11"));
			ddlMonth.Items.Add(new ListItem("December", "12"));

			//
			// Init the day 
			//
			if (startWithUnselectedDate) 
			{
				ddlDay.Items.Add(new ListItem("Day", "-1"));
			}
			for (int i = 1; i <= 31; i++)
			{
				ddlDay.Items.Add(new ListItem(i.ToString()));
			}
			//
			// Select the first items in the drop down. 
			//
			ddlYear.Items[0].Selected = false;
			ddlMonth.Items[0].Selected = false;
			ddlDay.Items[0].Selected = false;

			controlPopulated = true;
		}

		protected override void OnInit(EventArgs e)
		{
			EnsureChildControls();
			PopulateControl();
			base.OnInit (e);
		}

		protected override void CreateChildControls()
		{
			Controls.Add(ddlYear);
			Controls.Add(ddlMonth);
			Controls.Add(ddlDay);

			base.CreateChildControls ();
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Web server control is enabled.
		/// </summary>
		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				ddlDay.Enabled = value;
				ddlMonth.Enabled = value;
				ddlYear.Enabled = value;
				base.Enabled = value;
			}
		}

	
	}
}
