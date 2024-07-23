using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fbg.Bll
{
    public class RealmAge
    {
        public int AgeNumber { get; internal set; }
        string _desc;
        public string Desc
        {
            get
            {
                return String.Format(_desc, Until.Subtract(DateTime.Now).TotalDays < 0 ? 0 : Until.Subtract(DateTime.Now).TotalDays);
            }
            internal set { _desc = value; }
        }
        public DateTime Until { get; internal set; }
        public string Info { get; internal set; }
    }
}
