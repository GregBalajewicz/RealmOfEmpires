using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions; 
using Gmbc.Common.Diagnostics.ExceptionManagement;
namespace Fbg.Bll
{
    public class Poll
    {
        DataRow drPoll;
        DataRow []  drPollOptions;
        List<PollOptions> _options=new List<PollOptions> () ;

        public class CONSTS
        {
            /// <summary>
            /// this CONSTS describe the return datarow  of Polls tabls
            /// </summary>
            public class PollCloumnIndex
            {
                public static int PollID = 0;
                public static int Title = 1;
                public static int Desc = 2;
                public static int OfferType=3;
                public static int OfferAmount=4;
                public static int RealmID=5;
                public static int PollType = 6;
                public static int RunFrom = 7;
                public static int RunForXHours = 8;
            }
            /// <summary>
            /// this CONSTS describe the return dataset 
            /// </summary>
            public class PollsTableIndex
            {
                public static int Poll = 0;
                public static int PollOptions = 1;
                public static int PollResponses = 2;
             
            }
            public class PollCloumnName
            {
                public static string PollID = "PollID";
            }
        }
        public enum PollType
        {
            Radio=0,
            Checkbox=1
        }
        public Poll(DataRow drPoll, DataRow [] drPollOptions)
        {
            this.drPoll = drPoll;
            this.drPollOptions = drPollOptions;
        //    this.dtPollResponses = dtPollResponses;
            LoadData(); 
           
           
        }
        private void LoadData()
        {
            foreach (DataRow dr in drPollOptions )
            {
                //DataRow[] drs = dtPollResponses.Select(CONSTS.PollOptionCloumnName.PollOptionID + "=" + dr[PollOptions.CONSTS.PollOptionsCloumnIndex.PollOptionID]);
              //if (drs.Length != 0)
              //{
                PollOptions _po = new PollOptions(dr);
                  _options.Add(_po);
              //}
            }
        }
        public int ID
        {
            get
            {
                return (int)drPoll[CONSTS.PollCloumnIndex.PollID];
            }
        }
        public string Title
        {
            get
            {
                return (string)drPoll[CONSTS.PollCloumnIndex.Title ];
            }
        }  
        public string Desc
        {
            get
            {
                return (string)drPoll[CONSTS.PollCloumnIndex.Desc];
            }
        }
        public int OfferType
        {
            get
            {
                return (int)drPoll[CONSTS.PollCloumnIndex.OfferType ];
            }
        }
        public int OfferAmount
        {
            get
            {
                return (int)drPoll[CONSTS.PollCloumnIndex.OfferAmount ];
            }
        }
        public int? RealmID
        {
            get
            {
                return drPoll[CONSTS.PollCloumnIndex.RealmID] is DBNull ? null : (int?)drPoll[CONSTS.PollCloumnIndex.RealmID];
            }
        }
        public PollType Type
        {
            get
            {
                return (PollType)Enum.Parse(typeof(PollType), drPoll[CONSTS.PollCloumnIndex.PollType ].ToString ());
            }
        }
        public List<PollOptions> Options
        {
            get
            {
                return _options;
            }
        }

        public DateTime RunFrom
        {
            get
            {
                return (DateTime)drPoll[CONSTS.PollCloumnIndex.RunFrom];
            }
        }
        public DateTime RunUntill
        {
            get
            {
                return RunFrom + new TimeSpan((int)drPoll[CONSTS.PollCloumnIndex.RunForXHours], 0,0);
            }
        }

    }
}
