using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Fbg.Common
{
    public class CacheTimeStamps
    {
        Dictionary<Int16, DateTime> _listOfTimeStamps;

        public CacheTimeStamps(DataTable dt)
        {
            _listOfTimeStamps = dt.AsEnumerable().ToDictionary(
                r => r.Field<Int16>(0)
                , r => r.Field<DateTime>(1)
            );

        }

        public CacheTimeStamps(Dictionary<Int16, DateTime> listOfTimeStamps)
        {
            _listOfTimeStamps = listOfTimeStamps;
        }

        /// <summary>
        /// </summary>
        /// <param name="cacheItemID"></param>
        /// <returns>the time stamp or DateTime.MinValue is not found</returns>
        public DateTime GetTimeStamp(Int16 cacheItemID)
        {
            DateTime stamp;
            if (_listOfTimeStamps.TryGetValue(cacheItemID, out stamp))
            {
                return stamp;
            }
            else
            {
                return DateTime.MinValue;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="cacheItemID"></param>
        /// <returns>true if your time stamp is later or equal to latest cache time stamp > meaning, you retrieved the data after the last changed</returns>
        public bool isMyLatest(Int16 cacheItemID, DateTime myLatestTimeStamp)
        {
            DateTime stamp = GetTimeStamp(cacheItemID);

            return myLatestTimeStamp >= stamp;
        }

        public Dictionary<Int16, DateTime> raw
        {
            get
            {
                return _listOfTimeStamps;
            }
        }

    }
}
