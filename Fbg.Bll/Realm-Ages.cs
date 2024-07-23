using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public partial class Realm 
    {

        public RealmAgeInfo Age
        {
            get; internal set;
        }


        public void Age_Init(DataTable dt)
        {
            ////
            //// this code is not thread safe and it will be called by many threads. it shoudl be called in a thread safe way
            ////

            Age = new RealmAgeInfo();
            if (dt.Rows.Count > 0) {
                Age.href = (string)dt.Rows[0][Realm.CONSTS.RealmAgesColIndex.InfoUrl];
                Age.Ages = new List<RealmAge>(dt.Rows.Count);
                foreach (DataRow dr in dt.Rows) {
                    Age.Ages.Add(new RealmAge() { AgeNumber = (int)dr[Realm.CONSTS.RealmAgesColIndex.AgeNum]
                        , Desc = (string)dr[Realm.CONSTS.RealmAgesColIndex.Desc]
                        , Until = (DateTime)dr[Realm.CONSTS.RealmAgesColIndex.Untill]
                        , Info = dr[Realm.CONSTS.RealmAgesColIndex.InfoText] is DBNull ? "" : (string)dr[Realm.CONSTS.RealmAgesColIndex.InfoText]
                    });

                   
                }
            }

            object o = Age.CurrentAge;
        }


        public class RealmAgeInfo
        {
            public string Description
            {
                get
                {

                    return CurrentAge.Desc;
                }
            }
            public string CurrentAgeNumber
            {
                get
                {

                    return CurrentAge.AgeNumber.ToString();
                }
            }
            public bool isFeatureActive { get { return Ages != null; } }
            public List<RealmAge> Ages { get; set; }

            RealmAge _currentAge;           
            public RealmAge CurrentAge
            {
                get
                {

                    if (!isFeatureActive)
                    {
                        return null;
                    }

                    // get the latest current age if it was never gotten, or expired
                    if (_currentAge == null
                        || DateTime.Now < _currentAge.Until)
                    {
                        foreach (RealmAge age in Ages)
                        {
                            if ( DateTime.Now <= age.Until)
                            {
                                _currentAge = age;
                                break;
                            }

                        }
                    }

                    return _currentAge;
                }
            }

            /// <summary>
            /// may return 0 if not available
            /// </summary>
            public RealmAge NextAge
            {
                get
                {
                    return Ages.Find(f => f.AgeNumber == CurrentAge.AgeNumber + 1);
                }
            }
            /// <summary>
            /// may return 0 if not available
            /// </summary>
            public RealmAge PreviousAge
            {
                get
                {
                    return Ages.Find(f => f.AgeNumber == CurrentAge.AgeNumber - 1);
                }
            }
            public RealmAge AgeByNumber(int ageNumber)
            {
                return Ages.Find(f => f.AgeNumber == ageNumber);
            }
            public string href { get; internal set; }
        }
    }
}
