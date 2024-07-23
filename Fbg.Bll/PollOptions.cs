using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
   
    public class PollOptions
    {
        public class CONSTS
        {

            public class PollOptionsCloumnIndex
            {
                public static int PollID = 0;
                public static int PollOptionID = 1;
                public static int Text = 2;
            }
        }
        DataRow drPollOptions;

        public PollOptions(DataRow drPollOptions)
        {
            this.drPollOptions = drPollOptions;
        }
        public int ID
        {
            get
            {
                return (int)drPollOptions[CONSTS.PollOptionsCloumnIndex.PollOptionID];
            }
        }
        public string Text
        {
            get
            {
                return (string)drPollOptions[CONSTS.PollOptionsCloumnIndex.Text];
            }
        }

    }
}
