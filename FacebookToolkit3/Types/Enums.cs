using System;
using System.Collections.Generic;
using System.Text;

namespace Facebook.Types
{
	/// <summary>
	/// Facebook Enums
	/// </summary>
	public class Enums
	{
		/// <summary>
		/// Extended Permissions
		/// </summary>
		public enum Extended_Permissions
		{
			/// <summary>
			/// Status Update
			/// </summary>
			status_update,
			/// <summary>
			/// Photo Upload
			/// </summary>
			photo_upload,
			/// <summary>
			/// Create a listing
			/// </summary>
			create_listing
		}


        public enum StorySize
        {
            OneLine=1,
            Short=2,
            Full=4
        }
	}
}
